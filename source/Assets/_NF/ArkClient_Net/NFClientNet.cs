using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NFTCPClient
{
    public enum NFTCPClientState
    {
        Connecting,
        Connected,
        Disconnected
    }
	
	public enum NFTCPEventType
    {
        None,
        Connected,
        Disconnected,
        ConnectionRefused,
        DataReceived
    }

    public class NFSocketPacket
    {
        public byte[] bytes = null;
        public int bytesCount = 0;

        public NFSocketPacket(byte[] bytes, int bytesCount)
        {
            this.bytes = bytes;
            this.bytesCount = bytesCount;
        }

    }

    public class NFTCPEventParams
    {
        public NFClientNet client = null;
        public int clientID = 0;
        public TcpClient socket = null;
        public NFTCPEventType eventType = NFTCPEventType.None;
        public string message = "";
        public NFSocketPacket packet = null;

    }

    class StructureTransform
    {
        static bool bBig = false;//defalut little
        public static void Reverse(byte[] msg)
        {
            if (!bBig)
            {
                Array.Reverse(msg);
            }
        }

        public static void Reverse(byte[] msg, int nOffest, int nSize)
        {
            if (!bBig)
            {
                Array.Reverse(msg, nOffest, nSize);
            }
        }


        public static bool SetEndian(bool bIsBig)
        {
            bBig = bIsBig;
            return bBig;
        }

        public static void ByteArrayToStructureEndian(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            byte[] temparray = (byte[])bytearray.Clone();
          
            obj = Marshal.PtrToStructure(i, obj.GetType());
           
            object thisBoxed = obj;
            Type test = thisBoxed.GetType();
            int reversestartoffset = startoffset;
            
            foreach (var field in test.GetFields())
            {
                object fieldValue = field.GetValue(thisBoxed); // Get value

                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());  //Get Type
                if (typeCode != TypeCode.Object)  //Èç¹ûÎªÖµÀàÐÍ
                {
                    Reverse(temparray, reversestartoffset, Marshal.SizeOf(fieldValue));
                    reversestartoffset += Marshal.SizeOf(fieldValue);
                }
                else 
                {
                    reversestartoffset += ((byte[])fieldValue).Length;
                }
            }
            try
            {
                Marshal.Copy(temparray, startoffset, i, len);
            }
            catch (Exception ex) { Console.WriteLine("ByteArrayToStructure FAIL: error " + ex.ToString()); }
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);  
        }

        public static byte[] StructureToByteArrayEndian(object obj)
        {
            object thisBoxed = obj; 
            Type test = thisBoxed.GetType();

            int offset = 0;
            byte[] data = new byte[Marshal.SizeOf(thisBoxed)];

            object fieldValue;
            TypeCode typeCode;
            byte[] temp;
            
            foreach (var field in test.GetFields())
            {
                fieldValue = field.GetValue(thisBoxed); // Get value

                typeCode = Type.GetTypeCode(fieldValue.GetType());  // get type

                switch (typeCode)
                {
                    case TypeCode.Single: // float
                        {
                            temp = BitConverter.GetBytes((Single)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Single));
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            temp = BitConverter.GetBytes((Int32)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int32));
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            temp = BitConverter.GetBytes((UInt32)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt32));
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            temp = BitConverter.GetBytes((Int16)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int16));
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            temp = BitConverter.GetBytes((UInt16)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt16));
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            temp = BitConverter.GetBytes((Int64)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int64));
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            temp = BitConverter.GetBytes((UInt64)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt64));
                            break;
                        }
                    case TypeCode.Double:
                        {
                            temp = BitConverter.GetBytes((Double)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Double));
                            break;
                        }
                    case TypeCode.Byte:
                        {
                            data[offset] = (Byte)fieldValue;
                            break;
                        }
                    default:
                        {
                            //System.Diagnostics.Debug.Fail("No conversion provided for this type : " + typeCode.ToString());
                            break;
                        }
                }; // switch
                if (typeCode == TypeCode.Object)
                {
                    int length = ((byte[])fieldValue).Length;
                    Array.Copy(((byte[])fieldValue), 0, data, offset, length);
                    offset += length;
                }
                else
                {
                    offset += Marshal.SizeOf(fieldValue);
                }
            } // foreach

            return data;
        } // Swap
    };
	
    public class NFClientNet
    {
        public NFCNet net = null;

        public NFClientNet(NFCNet xnet)
        {
            net = xnet;
            Init();
        }

        void Init()
        {
            mxState = NFTCPClientState.Disconnected;
            mxEvents = new Queue<NFTCPEventType>();
            mxMessages = new Queue<string>();
            mxPackets = new Queue<NFSocketPacket>();
        }
        // MonoBehaviour
        private int bufferSize = 65536;

        private NFTCPClientState mxState;
        private NetworkStream mxStream;
        private StreamWriter mxWriter;
        private StreamReader mxReader;
        private Thread mxReadThread;
        private TcpClient mxClient;
        private Queue<NFTCPEventType> mxEvents;
        private Queue<string> mxMessages;
        private Queue<NFSocketPacket> mxPackets;

        public bool IsConnected()
        {
            return mxState == NFTCPClientState.Connected;
        }

        public NFTCPClientState GetState()
        {
            return mxState;
        }

        public void Update()
        {
			
            while (mxEvents.Count > 0)
            {
                lock (mxEvents)
                {
                    NFTCPEventType eventType = mxEvents.Dequeue();

                    NFTCPEventParams eventParams = new NFTCPEventParams();
                    eventParams.eventType = eventType;
                    eventParams.client = this;
                    eventParams.socket = mxClient;

                    if (eventType == NFTCPEventType.Connected)
                    {
                        OnClientConnect(eventParams);
                    }
                    else if (eventType == NFTCPEventType.Disconnected)
                    {
                        OnClientDisconnect(eventParams);

                        mxReader.Close();
                        mxWriter.Close();
                        mxClient.Close();

                    }
                    else if (eventType == NFTCPEventType.DataReceived)
                    {
                        lock (mxPackets)
                        {
                            eventParams.packet = mxPackets.Dequeue();
                        
                            OnDataReceived(eventParams);
                        }
                    }
                    else if (eventType == NFTCPEventType.ConnectionRefused)
                    {

                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {

                TcpClient tcpClient = (TcpClient)ar.AsyncState;
                tcpClient.EndConnect(ar);

                SetTcpClient(tcpClient);

            }
            catch (Exception e)
            {
                e.ToString();
                lock (mxEvents)
                {
                    mxEvents.Enqueue(NFTCPEventType.ConnectionRefused);
                }

            }

        }

        private void ReadData()
        {
            bool endOfStream = false;

            while (!endOfStream)
            {
               int bytesRead = 0;
               byte[] bytes = new byte[bufferSize];
               try
               {
                   bytesRead = mxStream.Read(bytes, 0, bufferSize);
               }
               catch (Exception e)
               {
                   e.ToString();
               }

               if (bytesRead == 0)
               {

                   endOfStream = true;

               }
               else
               {
                   lock (mxEvents)
                   {

                       mxEvents.Enqueue(NFTCPEventType.DataReceived);
                   }
                   lock (mxPackets)
                   {
                       mxPackets.Enqueue(new NFSocketPacket(bytes, bytesRead));
                   }

               }
            }

            mxState = NFTCPClientState.Disconnected;

            mxClient.Close();
            lock (mxEvents)
            {
                mxEvents.Enqueue(NFTCPEventType.Disconnected);
            }

        }

        // Public
        public void Connect(string hostname, int port)
        {
            if (mxState == NFTCPClientState.Connected)
            {
                return;
            }

            mxState = NFTCPClientState.Connecting;

            mxMessages.Clear();
            mxEvents.Clear();

            mxClient = new TcpClient();

            mxClient.BeginConnect(hostname,
                                 port,
                                 new AsyncCallback(ConnectCallback),
                                 mxClient);

        }

        public void Disconnect()
        {

            mxState = NFTCPClientState.Disconnected;

            try { if (mxReader != null) mxReader.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxWriter != null) mxWriter.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxClient != null) mxClient.Close(); }
            catch (Exception e) { e.ToString(); }

        }

        public void SendBytes(byte[] bytes)
        {
            SendBytes(bytes, 0, bytes.Length);
        }

        public void SendBytes(byte[] bytes, int offset, int size)
        {

            if (!IsConnected())
                return;

            mxStream.Write(bytes, offset, size);
            mxStream.Flush();

        }

        public void SetTcpClient(TcpClient tcpClient)
        {
            mxClient = tcpClient;

            if (mxClient.Connected)
            {
                mxStream = mxClient.GetStream();
                mxReader = new StreamReader(mxStream);
                mxWriter = new StreamWriter(mxStream);

                mxState = NFTCPClientState.Connected;

                mxEvents.Enqueue(NFTCPEventType.Connected);

                mxReadThread = new Thread(ReadData);
                mxReadThread.IsBackground = true;
                mxReadThread.Start();
            }
            else
            {
                mxState = NFTCPClientState.Disconnected;
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////Listener
        /// </summary>
        private UInt32 mnPacketSize = 0;
        private byte[] mPacket = new byte[ConstDefine.MAX_PACKET_LEN];

        public void OnClientConnect(NFTCPEventParams eventParams)
        {
            net.OnConnect();
        }

        public void OnClientDisconnect(NFTCPEventParams eventParams)
        {
            if (IsConnected())
            {
                Disconnect();
            }

            net.OnDisConnect();

        }

        public void OnClientConnectionRefused(NFTCPEventParams eventParams)
        {
            net.Log("Client refused");
        }

        public void OnDataReceived(NFTCPEventParams eventParams)
        {
            byte[] bytes = eventParams.packet.bytes;
            int bytesCount = eventParams.packet.bytesCount;

            net.Log("OnDataReceived:" + mnPacketSize + "|" + bytesCount);

            if (mnPacketSize + bytesCount < ConstDefine.MAX_PACKET_LEN)
            {
                Array.Copy(bytes, 0, mPacket, mnPacketSize, bytesCount);
                mnPacketSize += (UInt32)bytesCount;

                OnDataReceived();
            }
        }

        void OnDataReceived()
        {
            if (mnPacketSize >= ConstDefine.NF_PACKET_HEAD_SIZE)
            {
                object structType = new MsgHead();
                byte[] headBytes = new byte[Marshal.SizeOf(structType)];

                Array.Copy(mPacket, 0, headBytes, 0, Marshal.SizeOf(structType));
                StructureTransform.ByteArrayToStructureEndian(headBytes, ref structType, 0);
                MsgHead head = (MsgHead)structType;

                if (head.unDataLen == mnPacketSize)
                {
                    byte[] body_head = new byte[head.unDataLen];
                    Array.Copy(mPacket, 0, body_head, 0, head.unDataLen);
                    mnPacketSize = 0;

                    if (false == OnDataReceived(this, body_head, head.unDataLen))
                    {
                        OnClientDisconnect(new NFTCPEventParams());
                    }
                }
                else if (mnPacketSize > head.unDataLen)
                {
                    UInt32 nNewLen = mnPacketSize - head.unDataLen;
                    byte[] newpacket = new byte[ConstDefine.MAX_PACKET_LEN];
                    Array.Copy(mPacket, head.unDataLen, newpacket, 0, nNewLen);

                    byte[] body_head = new byte[head.unDataLen];
                    Array.Copy(mPacket, 0, body_head, 0, head.unDataLen);
                    mnPacketSize = nNewLen;
                    mPacket = newpacket;

                    if (false == OnDataReceived(this, body_head, head.unDataLen))
                    {
                        OnClientDisconnect(new NFTCPEventParams());
                    }

                    OnDataReceived();
                }
            }
        }

        bool OnDataReceived(NFClientNet client, byte[] bytes, UInt32 bytesCount)
        {
            if (bytes.Length == bytesCount)
            {
                object structType = new MsgHead();
                StructureTransform.ByteArrayToStructureEndian(bytes, ref structType, 0);
                MsgHead head = (MsgHead)structType;

                Int32 nBodyLen = (Int32)bytesCount - (Int32)ConstDefine.NF_PACKET_HEAD_SIZE;
                if (nBodyLen > 0)
                {
                    byte[] body = new byte[nBodyLen];
                    Array.Copy(bytes, ConstDefine.NF_PACKET_HEAD_SIZE, body, 0, nBodyLen);

                    client.net.OnMessageEvent(head, body);
                    return true;
                }
                else
                {
                }
            }

            return false;
        }

    }
}