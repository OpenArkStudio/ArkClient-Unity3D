using System.Collections;
using NFTCPClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

public class NFCNet : NFINet
{
    public NFClientNet mxClient = null;
    public ArrayList mReciveaMsgList = new ArrayList();

    private Hashtable mhtMsgDelegation = new Hashtable();
    private Hashtable mhtEventDelegation = new Hashtable();

    OnConnectDelegation mxOnConnectDelegation;
    OnDisConnectDelegation mxOnDisConnectDelegation;

    public NFCNet()
    {
    }

    public override void StartConnect(string strIP, int nPort)
    {
        mxClient = new NFClientNet(this);        
        mxClient.Connect(strIP, nPort);
    }

    public override void Update()
    {
        if (null != mxClient)
        {
            mxClient.Update();
        }
    }

    public override void SendMsg(int unMsgID, byte[] bodyByte)
    {
        MsgHead head = new MsgHead();
        head.unMsgID = (UInt16)unMsgID;
        head.unDataLen = (UInt32)bodyByte.Length + (UInt32)ConstDefine.NF_PACKET_HEAD_SIZE;

        byte[] headByte = StructureTransform.StructureToByteArrayEndian(head);


        byte[] sendBytes = new byte[head.unDataLen];
        headByte.CopyTo(sendBytes, 0);
        bodyByte.CopyTo(sendBytes, headByte.Length);

        mxClient.SendBytes(sendBytes);

        string strTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
        string strData = "S***:" + strTime + " MsgID:" + head.unMsgID + " Len:" + head.unDataLen;
        mReciveaMsgList.Add(strData);
    }

    public override void Disconnect()
    {
        if (null != mxClient)
        {
            mxClient.Disconnect();
        }
    }

    public override bool RegisteredDelegation(int eMsgID, MsgDelegation msgDelegate)
    {
        if (!mhtMsgDelegation.ContainsKey(eMsgID))
        {
            MsgDelegation myDelegationHandler = new MsgDelegation(msgDelegate);
            mhtMsgDelegation.Add(eMsgID, myDelegationHandler);
        }
        else
        {
            MsgDelegation myDelegationHandler = (MsgDelegation)mhtMsgDelegation[eMsgID];
            myDelegationHandler += new MsgDelegation(msgDelegate);
        }

        return true;
    }

    public override bool RegisteredResultCodeDelegation(int eCode, ResultCodeDelegation msgDelegate)
    {
        if (!mhtEventDelegation.ContainsKey(eCode))
        {
            ResultCodeDelegation myDelegationHandler = new ResultCodeDelegation(msgDelegate);
            mhtEventDelegation.Add(eCode, myDelegationHandler);
        }
        else
        {
            ResultCodeDelegation myDelegationHandler = (ResultCodeDelegation)mhtMsgDelegation[eCode];
            myDelegationHandler += new ResultCodeDelegation(msgDelegate);
        }

        return true;
    }

    public override bool RegisteredDisConnectDelegation(OnDisConnectDelegation onDisConnectDelegate)
    {
        mxOnDisConnectDelegation += onDisConnectDelegate;
        return true;
    }

    public override bool RegisteredConnectDelegation(OnConnectDelegation onConnectDelegate)
    {
        mxOnConnectDelegation += onConnectDelegate;
        return true;
    }

    public void Log(string text)
    {

    }

    public override bool DoResultCodeDelegation(int eCode)
    {
        if (mhtEventDelegation.ContainsKey(eCode))
        {
            ResultCodeDelegation myDelegationHandler = (ResultCodeDelegation)mhtEventDelegation[eCode];
            myDelegationHandler(eCode);

            return true;
        }

        return false;
    }

    public bool DoDelegation(int eMsg, MsgHead head, MemoryStream stream)
    {
        if (mhtMsgDelegation.ContainsKey(eMsg))
        {
            MsgDelegation myDelegationHandler = (MsgDelegation)mhtMsgDelegation[eMsg];
            myDelegationHandler(head, stream);

            return true;
        }

        return false;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void OnMessageEvent(MsgHead head, byte[] bytes)
    {
        if (head.unDataLen != bytes.Length + ConstDefine.NF_PACKET_HEAD_SIZE)
        {
            return;
        }

        int eMsg = (int)head.unMsgID;

        string strTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
        string strData = "R:" + strTime + " MsgID:" + head.unMsgID + " Len:" + head.unDataLen;
        mReciveaMsgList.Add(strData);


        DoDelegation(eMsg, head, new MemoryStream(bytes));
    }

    public void OnDisConnect()
    {
        mxOnDisConnectDelegation();

    }

    public void OnConnect()
    {
        mxOnConnectDelegation();

    }
}
