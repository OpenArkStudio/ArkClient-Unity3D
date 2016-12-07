using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using NFTCPClient;
using ProtoBuf;
using NFMsg;
using NFCoreEx;

namespace PlayerNetClient
{
	public class PlayerReciver
	{
        public ArrayList aWorldList = new ArrayList();
        public ArrayList aServerList = new ArrayList();
        public ArrayList aCharList = new ArrayList();

        public ArrayList aChatMsgList = new ArrayList();

        public PlayerNet mxPlayerNet ;
        public PlayerReciver(PlayerNet xPlayerNet)
        {
            mxPlayerNet = xPlayerNet;
        }

        ~PlayerReciver()
        {
        }

        static public NFCoreEx.NFIDENTID PBToNF(NFMsg.Ident xID)
        {
            NFCoreEx.NFIDENTID xIdent = new NFCoreEx.NFIDENTID();
            xIdent.nHead64 = xID.svrid;
            xIdent.nData64 = xID.index;

            return xIdent;
        }

        public  void OnDisConnect()
        {
        }

        public  void OnConnect()
        {
        }

		public void Init() 
		{

            mxPlayerNet.mxNet.RegisteredConnectDelegation(OnConnect);
            mxPlayerNet.mxNet.RegisteredDisConnectDelegation(OnDisConnect);

            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_LOGIN, EGMI_ACK_LOGIN);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_WORLD_LIST, EGMI_ACK_WORLD_LIST);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_EVENT_RESULT, EGMI_EVENT_RESULT);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_ROLE_LIST, EGMI_ACK_ROLE_LIST);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_CONNECT_WORLD, EGMI_ACK_CONNECT_WORLD);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_CONNECT_KEY, EGMI_ACK_CONNECT_KEY);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_SELECT_SERVER, EGMI_ACK_SELECT_SERVER);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_SWAP_SCENE, EGMI_ACK_SWAP_SCENE);


            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_OBJECT_ENTRY, EGMI_ACK_OBJECT_ENTRY);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_OBJECT_LEAVE, EGMI_ACK_OBJECT_LEAVE);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_MOVE, EGMI_ACK_MOVE);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_MOVE_IMMUNE, EGMI_ACK_MOVE_IMMUNE);

            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_INT, EGMI_ACK_PROPERTY_INT);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_FLOAT, EGMI_ACK_PROPERTY_FLOAT);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_STRING, EGMI_ACK_PROPERTY_STRING);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_OBJECT, EGMI_ACK_PROPERTY_OBJECT);

            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_RECORD_INT, EGMI_ACK_RECORD_INT);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_RECORD_FLOAT, EGMI_ACK_RECORD_FLOAT);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_RECORD_STRING, EGMI_ACK_RECORD_STRING);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_RECORD_OBJECT, EGMI_ACK_RECORD_OBJECT);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_SWAP_ROW, EGMI_ACK_SWAP_ROW);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_ADD_ROW, EGMI_ACK_ADD_ROW);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_REMOVE_ROW, EGMI_ACK_REMOVE_ROW);

            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_OBJECT_RECORD_ENTRY, EGMI_ACK_OBJECT_RECORD_ENTRY);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_OBJECT_PROPERTY_ENTRY, EGMI_ACK_OBJECT_PROPERTY_ENTRY);


            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_SKILL_OBJECTX, EGMI_ACK_SKILL_OBJECTX);
            mxPlayerNet.mxNet.RegisteredDelegation((int)NFMsg.EGameMsgID.EGMI_ACK_CHAT, EGMI_ACK_CHAT);
            
		}



        private void EGMI_EVENT_RESULT(MsgHead head, MemoryStream stream)
        {
            //OnResultMsg
            NFMsg.AckEventResult xResultCode = new NFMsg.AckEventResult();
            xResultCode = Serializer.Deserialize<NFMsg.AckEventResult>(stream);
            NFMsg.EGameEventCode eEvent = xResultCode.event_code;

            mxPlayerNet.mxNet.DoResultCodeDelegation((int)eEvent);
        }

        private void EGMI_ACK_LOGIN(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.AckEventResult xData = new NFMsg.AckEventResult();
            xData = Serializer.Deserialize<NFMsg.AckEventResult>(new MemoryStream(xMsg.msg_data));

            if (EGameEventCode.EGEC_ACCOUNT_SUCCESS == xData.event_code)
            {
                mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_PLAYER_LOGIN_SUCCESSFUL);    
            }
        }

        private void EGMI_ACK_WORLD_LIST(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.AckServerList xData = new NFMsg.AckServerList();
            xData = Serializer.Deserialize<NFMsg.AckServerList>(new MemoryStream(xMsg.msg_data));

            if (ReqServerListType.RSLT_WORLD_SERVER == xData.type)
            {
                for(int i = 0; i < xData.info.Count; ++i)
                {
                    ServerInfo info = xData.info[i];
                    aWorldList.Add(info);
                }
            }
            else if (ReqServerListType.RSLT_GAMES_ERVER == xData.type)
            {
                for (int i = 0; i < xData.info.Count; ++i)
                {
                    ServerInfo info = xData.info[i];
                    aServerList.Add(info);
                }
            }

            mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_PLAYER_WORLD_LIST_SUCCESSFUL_WAITING_SELECT_WORLD);
        }

        private void EGMI_ACK_CONNECT_WORLD(MsgHead head, MemoryStream stream)
        {
            mxPlayerNet.mxNet.Disconnect();

            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.AckConnectWorldResult xData = new NFMsg.AckConnectWorldResult();
            xData = Serializer.Deserialize<NFMsg.AckConnectWorldResult>(new MemoryStream(xMsg.msg_data));

            mxPlayerNet.strKey = System.Text.Encoding.Default.GetString(xData.world_key);
            mxPlayerNet.strWorldIP = System.Text.Encoding.Default.GetString(xData.world_ip);
            mxPlayerNet.nWorldPort = xData.world_port;
            mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_PLAYER_GET_WORLD_KEY_SUCCESSFUL);
        }

        private void EGMI_ACK_CONNECT_KEY(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.AckEventResult xData = new NFMsg.AckEventResult();
            xData = Serializer.Deserialize<NFMsg.AckEventResult>(new MemoryStream(xMsg.msg_data));

            if (xData.event_code == EGameEventCode.EGEC_VERIFY_KEY_SUCCESS)
            {
                //验证成功
                mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_VERIFY_KEY_SUCCESS_FULL);
                mxPlayerNet.nMainRoleID = PBToNF(xData.event_object);

                //申请世界内的服务器列表
                PlayerSender sender = mxPlayerNet.mxSender;
                if (null != sender)
                {
                    sender.RequireServerList();
                }
            }
        }

        private void EGMI_ACK_SELECT_SERVER(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.AckEventResult xData = new NFMsg.AckEventResult();
            xData = Serializer.Deserialize<NFMsg.AckEventResult>(new MemoryStream(xMsg.msg_data));

            if (xData.event_code == EGameEventCode.EGEC_SELECTSERVER_SUCCESS)
            {
                PlayerSender sender = mxPlayerNet.mxSender;
                if (null != sender)
                {
                    sender.RequireRoleList(mxPlayerNet.strAccount, mxPlayerNet.nServerID);
                }
            }
        }
        
        private void EGMI_ACK_ROLE_LIST(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.AckRoleLiteInfoList xData = new NFMsg.AckRoleLiteInfoList();
            xData = Serializer.Deserialize<NFMsg.AckRoleLiteInfoList>(new MemoryStream(xMsg.msg_data));
            
            aCharList.Clear();
            for (int i = 0; i < xData.char_data.Count; ++i)
            {
                NFMsg.RoleLiteInfo info = xData.char_data[i];
                aCharList.Add(info);
            }

            if (PlayerNet.PLAYER_STATE.E_WAIT_SELECT_ROLE != mxPlayerNet.GetPlayerState())
            {

                NFCDataList varList = new NFCDataList();
                varList.AddString("SelectScene");
                NFCLogicEvent.Instance.DoEvent((int)ClientEventDefine.EventDefine_LoadSelectRole, varList);
            }

            mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_GETROLELIST_SUCCESSFUL);
        }

        private void EGMI_ACK_SWAP_SCENE(MsgHead head, MemoryStream stream)
        {
            mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_PLAYER_GAMEING);

            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.ReqAckSwapScene xData = new NFMsg.ReqAckSwapScene();
            xData = Serializer.Deserialize<NFMsg.ReqAckSwapScene>(new MemoryStream(xMsg.msg_data));

            //NFCRenderInterface.Instance.LoadScene(xData.scene_id, xData.x, xData.y, xData.z);

            NFCDataList varList = new NFCDataList();
            varList.AddInt(xData.scene_id);
            varList.AddFloat(xData.x);
            varList.AddFloat(xData.y);
            varList.AddFloat(xData.z);

            NFCLogicEvent.Instance.DoEvent( (int)ClientEventDefine.EventDefine_Swap_Scene, varList);
        }

        private void EGMI_ACK_OBJECT_ENTRY(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.AckPlayerEntryList xData = new NFMsg.AckPlayerEntryList();
            xData = Serializer.Deserialize<NFMsg.AckPlayerEntryList>(new MemoryStream(xMsg.msg_data));

            for (int i = 0; i < xData.object_list.Count; ++i)
            {
                NFMsg.PlayerEntryInfo xInfo = xData.object_list[i];

                NFIDataList var = new NFCDataList();
                var.AddString("X");
                var.AddFloat(xInfo.x);
                var.AddString("Y");
                var.AddFloat(xInfo.y);
                var.AddString("Z");
                var.AddFloat(xInfo.z);
                NFIObject xGO = NFCKernel.Instance.CreateObject(PBToNF(xInfo.object_guid), xInfo.scene_id, 0, System.Text.Encoding.Default.GetString(xInfo.class_id), System.Text.Encoding.Default.GetString(xInfo.config_id), var);
                if (null == xGO)
                {
                    continue;
                }
            }
        }

        private void EGMI_ACK_OBJECT_LEAVE(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.AckPlayerLeaveList xData = new NFMsg.AckPlayerLeaveList();
            xData = Serializer.Deserialize<NFMsg.AckPlayerLeaveList>(new MemoryStream(xMsg.msg_data));

            for (int i = 0; i < xData.object_list.Count; ++i)
            {
                NFCKernel.Instance.DestroyObject(PBToNF(xData.object_list[i]));
            }
		}

        private void EGMI_ACK_MOVE(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.ReqAckPlayerMove xData = new NFMsg.ReqAckPlayerMove();
            xData = Serializer.Deserialize<NFMsg.ReqAckPlayerMove>(new MemoryStream(xMsg.msg_data));
            if (xData.target_pos.Count <= 0)
            {
                return;
            }
            float fSpeed = NFCKernel.Instance.QueryPropertyInt(PBToNF(xData.mover), "MOVE_SPEED") / 10000.0f;

            NFCDataList varList = new NFCDataList();
            varList.AddObject(PBToNF(xData.mover));
            varList.AddFloat(xData.target_pos[0].x);
            varList.AddFloat(xData.target_pos[0].y);
            varList.AddFloat(xData.target_pos[0].z);
            varList.AddFloat(fSpeed);

            NFCLogicEvent.Instance.DoEvent( (int)ClientEventDefine.EventDefine_MoveTo, varList);
            
            //NFCRenderInterface.Instance.MoveTo(PBToNF(xData.mover), new Vector3(xData.target_pos[0].x, xData.target_pos[0].y, xData.target_pos[0].z), fSpeed, true);
        }

        private void EGMI_ACK_MOVE_IMMUNE(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.ReqAckPlayerMove xData = new NFMsg.ReqAckPlayerMove();
            xData = Serializer.Deserialize<NFMsg.ReqAckPlayerMove>(new MemoryStream(xMsg.msg_data));
            if (xData.target_pos.Count <= 0)
            {
                return;
            }

            //其实就是jump
            float fSpeed = NFCKernel.Instance.QueryPropertyInt(PBToNF(xData.mover), "MOVE_SPEED") / 10000.0f;
            fSpeed *= 1.5f;

            NFCDataList varList = new NFCDataList();
            varList.AddObject(PBToNF(xData.mover));
            varList.AddFloat(xData.target_pos[0].x);
            varList.AddFloat(xData.target_pos[0].y);
            varList.AddFloat(xData.target_pos[0].z);
            varList.AddFloat(fSpeed);

            NFCLogicEvent.Instance.DoEvent( (int)ClientEventDefine.EVENTDEFINE_MOVE_IMMUNE, varList);

            //NFCRenderInterface.Instance.MoveImmuneBySpeed(PBToNF(xData.mover), new Vector3(xData.target_pos[0].x, xData.target_pos[0].y, xData.target_pos[0].z), fSpeed, true);

        }
        /////////////////////////////////////////////////////////////////////
        private void EGMI_ACK_PROPERTY_INT(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectPropertyInt propertyData = new NFMsg.ObjectPropertyInt();
            propertyData = Serializer.Deserialize<NFMsg.ObjectPropertyInt>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(propertyData.player_id));
            NFIPropertyManager propertyManager = go.GetPropertyManager();
			
			for(int i = 0; i < propertyData.property_list.Count; i++)
			{
                NFIProperty property = propertyManager.GetProperty(System.Text.Encoding.Default.GetString(propertyData.property_list[i].property_name));
                if(null == property)
                {
                    NFIDataList varList = new NFCDataList();
                    varList.AddInt(0);

                    property = propertyManager.AddProperty(System.Text.Encoding.Default.GetString(propertyData.property_list[i].property_name), varList);
                }

                property.SetInt(propertyData.property_list[i].data);
			}
		}
		
		private void EGMI_ACK_PROPERTY_FLOAT(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectPropertyFloat propertyData = new NFMsg.ObjectPropertyFloat();
            propertyData = Serializer.Deserialize<NFMsg.ObjectPropertyFloat>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(propertyData.player_id));
			
			for(int i = 0; i < propertyData.property_list.Count; i++)
			{
                NFIPropertyManager propertyManager = go.GetPropertyManager();
                NFIProperty property = propertyManager.GetProperty(System.Text.Encoding.Default.GetString(propertyData.property_list[i].property_name));
                if (null == property)
                {
                    NFIDataList varList = new NFCDataList();
                    varList.AddFloat(0.0f);

                    property = propertyManager.AddProperty(System.Text.Encoding.Default.GetString(propertyData.property_list[i].property_name), varList);
                }

                property.SetFloat(propertyData.property_list[i].data);
			}
		}
		
		private void EGMI_ACK_PROPERTY_STRING(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectPropertyString propertyData = new NFMsg.ObjectPropertyString();
            propertyData = Serializer.Deserialize<NFMsg.ObjectPropertyString>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(propertyData.player_id));

			for(int i = 0; i < propertyData.property_list.Count; i++)
			{
                NFIPropertyManager propertyManager = go.GetPropertyManager();
                NFIProperty property = propertyManager.GetProperty(System.Text.Encoding.Default.GetString(propertyData.property_list[i].property_name));
                if (null == property)
                {
                    NFIDataList varList = new NFCDataList();
                    varList.AddString("");

                    property = propertyManager.AddProperty(System.Text.Encoding.Default.GetString(propertyData.property_list[i].property_name), varList);
                }

                property.SetString(System.Text.Encoding.Default.GetString(propertyData.property_list[i].data));
			}
		}
		
		private void EGMI_ACK_PROPERTY_OBJECT(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectPropertyObject propertyData = new NFMsg.ObjectPropertyObject();
            propertyData = Serializer.Deserialize<NFMsg.ObjectPropertyObject>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(propertyData.player_id));
			
			for(int i = 0; i < propertyData.property_list.Count; i++)
			{
                NFIPropertyManager propertyManager = go.GetPropertyManager();
                NFIProperty property = propertyManager.GetProperty(System.Text.Encoding.Default.GetString(propertyData.property_list[i].property_name));
                if (null == property)
                {
                    NFIDataList varList = new NFCDataList();
                    varList.AddObject(new NFIDENTID());

                    property = propertyManager.AddProperty(System.Text.Encoding.Default.GetString(propertyData.property_list[i].property_name), varList);
                }

                property.SetObject(PBToNF(propertyData.property_list[i].data));
			}
		}
		
		private void EGMI_ACK_RECORD_INT(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectRecordInt recordData = new NFMsg.ObjectRecordInt();
            recordData = Serializer.Deserialize<NFMsg.ObjectRecordInt>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(recordData.player_id));
            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(recordData.record_name));

            for (int i = 0; i < recordData.property_list.Count; i++)
            {
                record.SetInt(recordData.property_list[i].row, recordData.property_list[i].col, (int)recordData.property_list[i].data);
            }
		}
		
		private void EGMI_ACK_RECORD_FLOAT(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectRecordFloat recordData = new NFMsg.ObjectRecordFloat();
            recordData = Serializer.Deserialize<NFMsg.ObjectRecordFloat>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(recordData.player_id));
            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(recordData.record_name));

            for (int i = 0; i < recordData.property_list.Count; i++)
            {
                record.SetFloat(recordData.property_list[i].row, recordData.property_list[i].col, (float)recordData.property_list[i].data);
            }
		}
		
		private void EGMI_ACK_RECORD_STRING(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectRecordString recordData = new NFMsg.ObjectRecordString();
            recordData = Serializer.Deserialize<NFMsg.ObjectRecordString>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(recordData.player_id));
            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(recordData.record_name));

            for (int i = 0; i < recordData.property_list.Count; i++)
            {
                record.SetString(recordData.property_list[i].row, recordData.property_list[i].col, System.Text.Encoding.Default.GetString(recordData.property_list[i].data));
            }
		}
		
		private void EGMI_ACK_RECORD_OBJECT(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectRecordObject recordData = new NFMsg.ObjectRecordObject();
            recordData = Serializer.Deserialize<NFMsg.ObjectRecordObject>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(recordData.player_id));
            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(recordData.record_name));


            for (int i = 0; i < recordData.property_list.Count; i++)
            {
                record.SetObject(recordData.property_list[i].row, recordData.property_list[i].col, PBToNF(recordData.property_list[i].data));
            }
		}
		
		private void EGMI_ACK_SWAP_ROW(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectRecordSwap recordData = new NFMsg.ObjectRecordSwap();
            recordData = Serializer.Deserialize<NFMsg.ObjectRecordSwap>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(recordData.player_id));
            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(recordData.origin_record_name));


            //目前认为在同一张表中交换吧
            record.SwapRow(recordData.row_origin, recordData.row_target);
        
        }

        private void ADD_ROW(NFCoreEx.NFIDENTID self, string strRecordName, NFMsg.RecordAddRowStruct xAddStruct)
        {
            NFIObject go = NFCKernel.Instance.GetObject(self);
            NFIRecordManager xRecordManager = go.GetRecordManager();


            Hashtable recordVecDesc = new Hashtable();
            Hashtable recordVecData = new Hashtable();

            for (int k = 0; k < xAddStruct.record_int_list.Count; ++k)
            {
                NFMsg.RecordInt addIntStruct = (NFMsg.RecordInt)xAddStruct.record_int_list[k];

                if (addIntStruct.col >= 0)
                {
                    recordVecDesc[addIntStruct.col] = NFIDataList.VARIANT_TYPE.VTYPE_INT;
                    recordVecData[addIntStruct.col] = addIntStruct.data;
                }
            }

            for (int k = 0; k < xAddStruct.record_float_list.Count; ++k)
            {
                NFMsg.RecordFloat addFloatStruct = (NFMsg.RecordFloat)xAddStruct.record_float_list[k];

                if (addFloatStruct.col >= 0)
                {
                    recordVecDesc[addFloatStruct.col] = NFIDataList.VARIANT_TYPE.VTYPE_FLOAT;
                    recordVecData[addFloatStruct.col] = addFloatStruct.data;

                }
            }

            for (int k = 0; k < xAddStruct.record_string_list.Count; ++k)
            {
                NFMsg.RecordString addStringStruct = (NFMsg.RecordString)xAddStruct.record_string_list[k];

                if (addStringStruct.col >= 0)
                {
                    recordVecDesc[addStringStruct.col] = NFIDataList.VARIANT_TYPE.VTYPE_STRING;
                    recordVecData[addStringStruct.col] = System.Text.Encoding.Default.GetString(addStringStruct.data);

                }
            }

            for (int k = 0; k < xAddStruct.record_object_list.Count; ++k)
            {
                NFMsg.RecordObject addObjectStruct = (NFMsg.RecordObject)xAddStruct.record_object_list[k];

                if (addObjectStruct.col >= 0)
                {
                    recordVecDesc[addObjectStruct.col] = NFIDataList.VARIANT_TYPE.VTYPE_OBJECT;
                    recordVecData[addObjectStruct.col] = PBToNF((NFMsg.Ident)addObjectStruct.data);

                }
            }

            NFIDataList varListDesc = new NFCDataList();
            NFIDataList varListData = new NFCDataList();
            for (int m = 0; m < recordVecDesc.Count; m++)
            {
                if (recordVecDesc.ContainsKey(m) && recordVecData.ContainsKey(m))
                {
                    NFIDataList.VARIANT_TYPE nType = (NFIDataList.VARIANT_TYPE)recordVecDesc[m];
                    switch (nType)
                    {
                        case NFIDataList.VARIANT_TYPE.VTYPE_INT:
                            {
                                varListDesc.AddInt(0);
                                varListData.AddInt((Int64)recordVecData[m]);
                            }

                            break;
                        case NFIDataList.VARIANT_TYPE.VTYPE_FLOAT:
                            {
                                varListDesc.AddFloat(0.0f);
                                varListData.AddFloat((float)recordVecData[m]);
                            }
                            break;
                        case NFIDataList.VARIANT_TYPE.VTYPE_STRING:
                            {
                                varListDesc.AddString("");
                                varListData.AddString((string)recordVecData[m]);
                            }
                            break;
                        case NFIDataList.VARIANT_TYPE.VTYPE_OBJECT:
                            {
                                varListDesc.AddObject(new NFIDENTID());
                                varListData.AddObject((NFIDENTID)recordVecData[m]);
                            }
                            break;
                        default:
                            break;

                    }
                }
                else
                {
                    //报错
                    //Debug.LogException(i);
                }
            }

            NFIRecord xRecord = xRecordManager.GetRecord(strRecordName);
            if (null == xRecord)
            {
                xRecord = xRecordManager.AddRecord(strRecordName, 512, varListDesc);
            }

            xRecord.AddRow(xAddStruct.row, varListData);
        }

        private void EGMI_ACK_ADD_ROW(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectRecordAddRow recordData = new NFMsg.ObjectRecordAddRow();
            recordData = Serializer.Deserialize<NFMsg.ObjectRecordAddRow>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(recordData.player_id));
            NFIRecordManager recordManager = go.GetRecordManager();

            for (int i = 0; i < recordData.row_data.Count; i++)
            {
                ADD_ROW(PBToNF(recordData.player_id), System.Text.Encoding.Default.GetString(recordData.record_name), recordData.row_data[i]);
            }
		}

        private void EGMI_ACK_REMOVE_ROW(MsgHead head, MemoryStream stream)
		{
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

			NFMsg.ObjectRecordRemove recordData = new NFMsg.ObjectRecordRemove();
            recordData = Serializer.Deserialize<NFMsg.ObjectRecordRemove>(new MemoryStream(xMsg.msg_data));

            NFIObject go = NFCKernel.Instance.GetObject(PBToNF(recordData.player_id));
            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(recordData.record_name));

            for (int i = 0; i < recordData.remove_row.Count; i++)
            {
                record.Remove(recordData.remove_row[i]);
            }
		}

        private void EGMI_ACK_OBJECT_RECORD_ENTRY(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.MultiObjectRecordList xMultiObjectRecordData = new NFMsg.MultiObjectRecordList();
            xMultiObjectRecordData = Serializer.Deserialize<NFMsg.MultiObjectRecordList>(new MemoryStream(xMsg.msg_data));

            for (int i = 0; i < xMultiObjectRecordData.multi_player_record.Count; i++)
            {
                NFMsg.ObjectRecordList xObjectRecordList = xMultiObjectRecordData.multi_player_record[i];
                for (int j = 0; j < xObjectRecordList.record_list.Count; j++)
                {
                    NFMsg.ObjectRecordBase xObjectRecordBase = xObjectRecordList.record_list[j];
                    for (int k = 0; k < xObjectRecordBase.row_struct.Count; ++k )
                    {
                        NFMsg.RecordAddRowStruct xAddRowStruct = xObjectRecordBase.row_struct[i];

                        ADD_ROW(PBToNF(xObjectRecordList.player_id), System.Text.Encoding.Default.GetString(xObjectRecordBase.record_name), xAddRowStruct);
                    }
                }
            }
        }

        private void EGMI_ACK_OBJECT_PROPERTY_ENTRY(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.MultiObjectPropertyList xMultiObjectPropertyList = new NFMsg.MultiObjectPropertyList();
            xMultiObjectPropertyList = Serializer.Deserialize<NFMsg.MultiObjectPropertyList>(new MemoryStream(xMsg.msg_data));

            for (int i = 0; i < xMultiObjectPropertyList.multi_player_property.Count; i++)
            {
                NFMsg.ObjectPropertyList xPropertyData = xMultiObjectPropertyList.multi_player_property[i];
                NFIObject go = NFCKernel.Instance.GetObject(PBToNF(xPropertyData.player_id));
                NFIPropertyManager xPropertyManager = go.GetPropertyManager();

                for (int j = 0; j < xPropertyData.property_int_list.Count; j++)
                {
                    string strPropertyName = System.Text.Encoding.Default.GetString(xPropertyData.property_int_list[j].property_name);
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFIDataList varList = new NFCDataList();
                        varList.AddInt(0);

                        xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
                    }

                    xProperty.SetInt(xPropertyData.property_int_list[j].data);
                }

                for (int j = 0; j < xPropertyData.property_float_list.Count; j++)
                {
                    string strPropertyName = System.Text.Encoding.Default.GetString(xPropertyData.property_float_list[j].property_name);
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFIDataList varList = new NFCDataList();
                        varList.AddFloat(0);

                        xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
                    }

                    xProperty.SetFloat(xPropertyData.property_float_list[j].data);
                }

                for (int j = 0; j < xPropertyData.property_string_list.Count; j++)
                {
                    string strPropertyName = System.Text.Encoding.Default.GetString(xPropertyData.property_string_list[j].property_name);
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFIDataList varList = new NFCDataList();
                        varList.AddString("");

                        xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
                    }

                    xProperty.SetString(System.Text.Encoding.Default.GetString(xPropertyData.property_string_list[j].data));
                }

                for (int j = 0; j < xPropertyData.property_object_list.Count; j++)
                {
                    string strPropertyName = System.Text.Encoding.Default.GetString(xPropertyData.property_object_list[j].property_name);
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFIDataList varList = new NFCDataList();
                        varList.AddObject(new NFIDENTID());

                        xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
                    }

                    xProperty.SetObject(PBToNF(xPropertyData.property_object_list[j].data));
                }
            }
        }

        //////////////////////////////////
        private void EGMI_ACK_SKILL_OBJECTX(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.ReqAckUseSkill xReqAckUseSkill = new NFMsg.ReqAckUseSkill();
            xReqAckUseSkill = Serializer.Deserialize<NFMsg.ReqAckUseSkill>(new MemoryStream(xMsg.msg_data));
            NFMsg.Position xNowPos = xReqAckUseSkill.now_pos;
            NFMsg.Position xTarPos = xReqAckUseSkill.tar_pos;

            NFIDataList xObjectList = new NFCDataList();
            NFIDataList xRtlList = new NFCDataList();
            NFIDataList xValueList = new NFCDataList();

            if (xReqAckUseSkill.effect_data.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < xReqAckUseSkill.effect_data.Count; ++i )
            {
                xObjectList.AddObject(PBToNF(xReqAckUseSkill.effect_data[i].effect_ident));
                xRtlList.AddInt((int)xReqAckUseSkill.effect_data[i].effect_rlt);
                xValueList.AddInt((int)xReqAckUseSkill.effect_data[i].effect_value);
            }
            

            string strSkillName = System.Text.Encoding.Default.GetString(xReqAckUseSkill.skill_id);
            //Debug.Log("AckUseSkill:" + strSkillName);

            NFCDataList varList = new NFCDataList();
            varList.AddObject(PBToNF(xReqAckUseSkill.user));
            varList.AddFloat(xNowPos.x);
            varList.AddFloat(xNowPos.z);
            varList.AddFloat(xTarPos.x);
            varList.AddFloat(xTarPos.z);

            if (xObjectList.Count() != xRtlList.Count() || xObjectList.Count() != xValueList.Count())
            {
                return;
            }

            varList.AddInt(xObjectList.Count());
            for (int i = 0; i < xObjectList.Count(); ++i)
            {
                varList.AddObject(xObjectList.ObjectVal(i));
            }

            for (int i = 0; i < xRtlList.Count(); ++i)
            {
                varList.AddInt(xRtlList.IntVal(i));
            }

            for (int i = 0; i < xValueList.Count(); ++i)
            {
                varList.AddInt(xValueList.IntVal(i));
            }


            NFCLogicEvent.Instance.DoEvent( (int)ClientEventDefine.EVENTDEFINE_USESKILL, varList);

            //NFCRenderInterface.Instance.UseSkill(, strSkillName, xNowPos.x, xNowPos.z, xTarPos.x, xTarPos.z, xObjectList, xRtlList, xValueList);
        }

        private void EGMI_ACK_CHAT(MsgHead head, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = new NFMsg.MsgBase();
            xMsg = Serializer.Deserialize<NFMsg.MsgBase>(stream);

            NFMsg.ReqAckPlayerChat xReqAckChat = new NFMsg.ReqAckPlayerChat();
            xReqAckChat = Serializer.Deserialize<NFMsg.ReqAckPlayerChat>(new MemoryStream(xMsg.msg_data));

            aChatMsgList.Add(PBToNF(xReqAckChat.chat_id).ToString() + ":" + System.Text.Encoding.Default.GetString(xReqAckChat.chat_info));
        }
    }

}