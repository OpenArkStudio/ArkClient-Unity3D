using UnityEngine;
using System.Collections;
using AFTCPClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using AFCoreEx;
using AFMsg;
using ProtoBuf;
using PlayerNetClient;

public class AFStart : MonoBehaviour
{
    //看开启多人模式还是单人模式
    AFConfig mConfig = null;
    PlayerNet mxPlayerNet = null;
    string strTargetIP = "";
    int nPort = 0;
    public bool bCommand = false;
    public bool bDebugMode = false;
    public AFObjectElement  mxObjectElement = new AFObjectElement();

    public Transform[] mTrans;

    public PlayerNet GetPlayerNet()
    {
        return mxPlayerNet;
    }

    public void SetFocusNet(PlayerNet xNet)
    {
        mxPlayerNet = xNet;
    }

    public PlayerSender GetFocusSender()
    {
        if (null != mxPlayerNet)
        {
            return mxPlayerNet.mxSender;
        }

        return null;
    }

    public PlayerReciver GetFocusListener()
    {
        if (null != mxPlayerNet)
        {
            return mxPlayerNet.mxReciver;
        }

        return null;
    }

    #region Instance
    private static AFStart _Instance = null;
    public static AFStart Instance
    {
        get
        {
            return _Instance;
        }

    }
    #endregion



    void Awake()
    {
        _Instance = this;

        foreach (Transform trans in mTrans)
        {
            if (null != trans)
            {
                trans.gameObject.SetActive(true);
            }
        }


    }

    // Use this for initialization
    void Start()
    {
        mConfig = new AFConfig();
        mConfig.Load();
        mConfig.GetSelectServer(ref strTargetIP, ref nPort);
        String strConfigPath = mConfig.GetConfigPath();

        AFCElementManager.Instance.Load(strConfigPath);
        AFCRenderInterface.Instance.Init();

    }



        void OnDestroy()
    {

        if (null != mxPlayerNet)
        {
            mxPlayerNet.mxNet.Disconnect();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            bCommand = !bCommand;
        }

        if (null != GetPlayerNet() && null != GetPlayerNet().mxNet)
        {
              GetPlayerNet().mxNet.Update();
        }
    }

    void OnGUI()
    {

        if (null != GetPlayerNet() && mxPlayerNet.GetPlayerState() == PlayerNet.PLAYER_STATE.E_PLAYER_GAMEING)
        {
            mxObjectElement.OnGUI(AFCKernel.Instance, 1024, 768);
            mxObjectElement.OnOpratorGUI(1024, 768);
        }

        if (null != GetPlayerNet())
        {
            switch (GetPlayerNet().GetPlayerState())
            {
                case PlayerNet.PLAYER_STATE.E_NONE:
                    {
                        if (strTargetIP.Length > 0)
                        {
                            mxPlayerNet.mxNet.StartConnect(strTargetIP, nPort);
                            mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_WAITING_CONNET_NET);
                        }
                    }

                    break;
                case PlayerNet.PLAYER_STATE.E_WAITING_CONNET_NET:
                    {
                       // Wait Reciving Connect event ,when connect sucessful ,enter status :E_CONNET_NET_SUCESS_WAITING_ACCOUNT
                    }

                    break;
                case PlayerNet.PLAYER_STATE.E_CONNET_NET_SUCESS_WAITING_ACCOUNT:
                    {
                        // wait user input account;
                        mxPlayerNet.strAccount = GUI.TextField(new Rect(10, 10, 150, 50), mxPlayerNet.strAccount);
                        mxPlayerNet.strPassword = GUI.TextField(new Rect(10, 100, 150, 50), mxPlayerNet.strPassword);
                        if (GUI.Button(new Rect(10, 200, 150, 50), "Login"))
                        {
                            mxPlayerNet.mxSender.LoginPB(mxPlayerNet.strAccount, mxPlayerNet.strPassword, "");
                            mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_PLAYER_LOGINING_WAITING_lOGINRESULT);
                        }
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_PLAYER_LOGINING_WAITING_lOGINRESULT:
                    {
                        // wait receiving msg EGMI_ACK_LOGIN, when receiving this msg,if receiving error result , show a error,and enter E_CONNET_NET_SUCESS_WAITING_ACCOUNT  status
                        // if login successful, enter E_PLAYER_LOGIN_SUCCESSFUL_WAITING_WORLD_LIST  status
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_PLAYER_LOGIN_SUCCESSFUL:
                    {
                        // if login successful, enter E_PLAYER_LOGIN_SUCCESSFUL_WAITING_WORLD_LIST  status
                        PlayerSender sender = mxPlayerNet.mxSender;
                        if (null != sender)
                        {
                            mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_PLAYER_WAITING_WORLD_LIST);
                            sender.RequireWorldList();
                        }
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_PLAYER_WAITING_WORLD_LIST:
                    {
                        // wait receiving msg EGMI_ACK_WORLD_LIST, when receiving this msg,if receiving error result , show a error,and enter E_CONNET_NET_SUCESS_WAITING_ACCOUNT  status
                        // if successful, enter E_PLAYER_WORLD_LIST_SUCCESSFUL_WAITING_SELECT_WORLD  status
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_PLAYER_WORLD_LIST_SUCCESSFUL_WAITING_SELECT_WORLD:
                    {
                        // show world list 
                        // wait user select world;
                        int nHeight = 50;
                        for (int i = 0; i < mxPlayerNet.aWorldList.Count; ++i)
                        {
                            ServerInfo xInfo = (ServerInfo)mxPlayerNet.aWorldList[i];
                            if (GUI.Button(new Rect(10, i * nHeight, 150, nHeight), System.Text.Encoding.Default.GetString(xInfo.name)))
                            {
                                AFStart.Instance.GetPlayerNet().nServerID = xInfo.server_id;
                                mxPlayerNet.mxSender.RequireConnectWorld(xInfo.server_id);
                                mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_PLAYER_SELECT_WORLD_WAIT_WORK_KEY);
                            }
                        }
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_PLAYER_GET_WORLD_KEY_SUCCESSFUL:
                    {
                        string strWorpdIP = AFStart.Instance.GetPlayerNet().strWorldIP;
                        string strWorpdKey = AFStart.Instance.GetPlayerNet().strKey;
                        string strAccount = AFStart.Instance.GetPlayerNet().strKey;
                        int nPort = AFStart.Instance.GetPlayerNet().nWorldPort;

                        PlayerNet xPlayerNet = new PlayerNet();
                        xPlayerNet.strWorldIP = strWorpdIP;
                        xPlayerNet.strKey = strWorpdKey;
                        xPlayerNet.strAccount = strAccount;
                        xPlayerNet.nWorldPort = nPort;

                        xPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_START_CONNECT_TO_GATE);

                        xPlayerNet.mxNet.RegisteredConnectDelegation(OnGateConnect);
                        xPlayerNet.mxNet.RegisteredDisConnectDelegation(OnGateDisConnect);
                        xPlayerNet.mxNet.StartConnect(xPlayerNet.strWorldIP, nPort);
                        SetFocusNet(xPlayerNet);
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_START_CONNECT_TO_GATE_SUCESS_FUL:
                    {
                        GetPlayerNet().mxSender.RequireVerifyWorldKey(GetPlayerNet().strAccount, GetPlayerNet().strKey);
                        GetPlayerNet().ChangePlayerState(PlayerNet.PLAYER_STATE.E_WATING_VERIFY);
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_WATING_VERIFY:
                    {
                        //wait receiving msg EGMI_ACK_CONNECT_KEY, if receive failed ,show error  enter none status
                        //if have receiving  successful Enter E_VERIFY_KEY_SUCCESS_FULL status;
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_VERIFY_KEY_SUCCESS_FULL:
                    {   
                        // wait user select server;
                        int nWidth = 200;
                        for (int i = 0; i < mxPlayerNet.aServerList.Count; ++i)
                        {
                            ServerInfo xInfo = (ServerInfo)mxPlayerNet.aServerList[i];
                            if (GUI.Button(new Rect(nWidth, i * 50, 150, 50), System.Text.Encoding.Default.GetString(xInfo.name)))
                            {
                                GetPlayerNet().nServerID = xInfo.server_id;
                                GetFocusSender().RequireSelectServer(xInfo.server_id);
                                GetPlayerNet().ChangePlayerState(PlayerNet.PLAYER_STATE.E_WAIT_ROLELIST);
                            }
                        }
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_WAIT_ROLELIST:
                    {
                        // wait receiving EGMI_ACK_SELECT_SERVER msg, 
                        // when receiving this msg, send req role list;
                        // and waiting EGMI_ACK_ROLE_LIST msg, if sucessful ,enter E_HAS_PLAYER_ROLELIST status;
                        
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_GETROLELIST_SUCCESSFUL:
                    {
                        //AFCRenderInterface.Instance.LoadScene("SelectScene");
                        GetPlayerNet().ChangePlayerState(PlayerNet.PLAYER_STATE.E_WAIT_SELECT_ROLE);

                    }
                    break;

                case PlayerNet.PLAYER_STATE.E_WAIT_SELECT_ROLE:
                    {
                        if (mxPlayerNet.aCharList.Count > 0)
                        {
                            for (int i = 0; i < mxPlayerNet.aCharList.Count; ++i)
                            {
                                AFMsg.RoleLiteInfo xLiteInfo = (AFMsg.RoleLiteInfo)mxPlayerNet.aCharList[i];
                                if (GUI.Button(new Rect(200, i * 50, 150, 50), System.Text.Encoding.Default.GetString(xLiteInfo.noob_name)))
                                {
                                    mxPlayerNet.strRoleName = System.Text.Encoding.Default.GetString(xLiteInfo.noob_name);
                                    GetPlayerNet().nMainRoleID = PlayerReciver.PBToAF(xLiteInfo.id);
                                    GetPlayerNet().ChangePlayerState(PlayerNet.PLAYER_STATE.E_PLAYER_WAITING_TO_GAME);
                                    mxPlayerNet.mxSender.RequireEnterGameServer(AFStart.Instance.GetPlayerNet().nMainRoleID, mxPlayerNet.strAccount, mxPlayerNet.strRoleName, mxPlayerNet.nServerID);
                                }
                            }
                        }
                        else
                        {
                            mxPlayerNet.strRoleName = GUI.TextField(new Rect(10, 10, 150, 50), mxPlayerNet.strRoleName);
                            if (GUI.Button(new Rect(10, 200, 150, 50), "CreateRole"))
                            {
                                GetPlayerNet().ChangePlayerState(PlayerNet.PLAYER_STATE.E_WAIT_CREATE_ROLE);
                                mxPlayerNet.mxSender.RequireCreateRole(mxPlayerNet.strAccount, mxPlayerNet.strRoleName, 0, 0, mxPlayerNet.nServerID);
                            }
                        }
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_WAIT_CREATE_ROLE:
                    {
                        // wait receive EGMI_ACK_ROLE_LIST ,  when receive msg, enter E_WAIT_SELECT_ROLE status;
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_PLAYER_WAITING_TO_GAME:
                    {
                        // wait receive EGMI_ACK_SWAP_SCENE ,  when receive msg, enter E_PLAYER_GAMEING status;
                    }
                    break;

                case PlayerNet.PLAYER_STATE.E_PLAYER_GAMEING:
                    //AFCSectionManager.Instance.SetGameState(AFCSectionManager.UI_SECTION_STATE.UISS_GAMEING);
                    break;

                default:
                    break;
            }
        }
        else
        {   
            PlayerNet xPlayerNet = new PlayerNet();
            xPlayerNet.mxNet.RegisteredConnectDelegation(OnConfigConnect);
            SetFocusNet(xPlayerNet);
        }
    }

    public void OnConfigConnect()
    {
        mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_CONNET_NET_SUCESS_WAITING_ACCOUNT);
    }

    public void OnGateConnect()
    {
        mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_START_CONNECT_TO_GATE_SUCESS_FUL);
    }

    public void OnGateDisConnect()
    {
        mxPlayerNet.ChangePlayerState(PlayerNet.PLAYER_STATE.E_DISCOUNT);
    }
}
