using UnityEngine;
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
using NFCoreEx;
using NFMsg;
using ProtoBuf;
using PlayerNetClient;

public class NFStart : MonoBehaviour
{
    //看开启多人模式还是单人模式
    NFConfig mConfig = null;
    PlayerNet mxPlayerNet = null;
    string strTargetIP = "";
    int nPort = 0;
    public bool bCommand = false;
    public bool bDebugMode = false;
    public NFObjectElement  mxObjectElement = new NFObjectElement();

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
    private static NFStart _Instance = null;
    public static NFStart Instance
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
        mConfig = new NFConfig();
        mConfig.Load();
        mConfig.GetSelectServer(ref strTargetIP, ref nPort);
        String strConfigPath = mConfig.GetConfigPath();

        NFCElementManager.Instance.Load(strConfigPath);
        NFCRenderInterface.Instance.Init();

    }

    void OnDestroy()
    {

        if (null != mxPlayerNet)
        {
            mxPlayerNet.mxNet.Disconnect();
        }
    }

    void OnGUI()
    {

        if (null != mxPlayerNet && mxPlayerNet.mPlayerState == PlayerNet.PLAYER_STATE.E_PLAYER_GAMEING)
        {
            mxObjectElement.OnGUI(NFCKernel.Instance, 1024, 768);
            mxObjectElement.OnOpratorGUI(1024, 768);
        }

        if (null != mxPlayerNet)
        {
            switch (mxPlayerNet.mPlayerState)
            {
                case PlayerNet.PLAYER_STATE.E_NONE:
                    {
                        if (strTargetIP.Length > 0)
                        {
                            mxPlayerNet.mxNet.StartConnect(strTargetIP, nPort);
                            mxPlayerNet.mPlayerState = PlayerNet.PLAYER_STATE.E_WAITING_PLAYER_LOGIN;
                        }
                    }

                    break;
                case PlayerNet.PLAYER_STATE.E_WAITING_PLAYER_LOGIN:
                    {
                        if (mxPlayerNet.strKey.Length > 0)
                        {
                            mxPlayerNet.mPlayerState = PlayerNet.PLAYER_STATE.E_HAS_PLAYER_GATE;
                        }
                        else
                        {
                            mxPlayerNet.strAccount = GUI.TextField(new Rect(10, 10, 150, 50), mxPlayerNet.strAccount);
                            mxPlayerNet.strPassword = GUI.TextField(new Rect(10, 100, 150, 50), mxPlayerNet.strPassword);
                            if (GUI.Button (new Rect (10, 200, 150, 50), "Login"))
                            {
                                mxPlayerNet.mxSender.LoginPB(mxPlayerNet.strAccount, mxPlayerNet.strPassword, "");
                            }
                        }
                    }
                    break;

                case PlayerNet.PLAYER_STATE.E_HAS_PLAYER_LOGIN:
                    {
                        int nHeight = 50;
                        for (int i = 0; i < mxPlayerNet.mxReciver.aWorldList.Count; ++i )
                        {
                            ServerInfo xInfo = (ServerInfo)mxPlayerNet.mxReciver.aWorldList[i];
                            if (GUI.Button(new Rect(10, i * nHeight, 150, nHeight), System.Text.Encoding.Default.GetString(xInfo.name)))
                            {
                                NFStart.Instance.GetPlayerNet().nServerID = xInfo.server_id;
                                mxPlayerNet.mxSender.RequireConnectWorld(xInfo.server_id);
                            }
                        }
                    }
                    break;

                case PlayerNet.PLAYER_STATE.E_WAITING_PLAYER_TO_GATE:
                    {
                        string strWorpdIP = NFStart.Instance.GetPlayerNet().strWorldIP;
                        string strWorpdKey = NFStart.Instance.GetPlayerNet().strKey;
                        string strAccount = NFStart.Instance.GetPlayerNet().strKey;
                        int nPort = NFStart.Instance.GetPlayerNet().nWorldPort;

                        PlayerNet xNet = new PlayerNet();
#if UNITY_EDITOR
                        if (strWorpdIP == "127.0.0.1")
                        {
                            strWorpdIP = strTargetIP;
                        }
#endif
                        xNet.strWorldIP = strWorpdIP;
                        xNet.strKey = strWorpdKey;
                        xNet.strAccount = strAccount;
                        xNet.nWorldPort = nPort;

                        xNet.mPlayerState = PlayerNet.PLAYER_STATE.E_START_CONNECT_TO_GATE;
                        xNet.mxNet.StartConnect(xNet.strWorldIP, nPort);
                        NFStart.Instance.SetFocusNet(xNet);
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_HAS_PLAYER_GATE:
                    {
                        NFStart.Instance.GetPlayerNet().mxSender.RequireVerifyWorldKey(NFStart.Instance.GetPlayerNet().strAccount, NFStart.Instance.GetPlayerNet().strKey);
                        NFStart.Instance.GetPlayerNet().mPlayerState = PlayerNet.PLAYER_STATE.E_WATING_VERIFY;
                    }
                    break;
                case PlayerNet.PLAYER_STATE.E_HAS_VERIFY:
                    {

                        int nWidth = 200;
                        for (int i = 0; i < mxPlayerNet.mxReciver.aServerList.Count; ++i)
                        {
                            ServerInfo xInfo = (ServerInfo)mxPlayerNet.mxReciver.aServerList[i];
                            if (GUI.Button(new Rect(nWidth, i * 50, 150, 50), System.Text.Encoding.Default.GetString(xInfo.name)))
                            {
                                NFStart.Instance.GetPlayerNet().nServerID = xInfo.server_id;
                                NFStart.Instance.GetFocusSender().RequireSelectServer(xInfo.server_id);
                            }
                        }
                    }
                    break;

                case PlayerNet.PLAYER_STATE.E_HAS_PLAYER_ROLELIST:
                    {
                        if (mxPlayerNet.mxReciver.aCharList.Count > 0)
                        {
                            for (int i = 0; i < mxPlayerNet.mxReciver.aCharList.Count; ++i)
                            {
                                NFMsg.RoleLiteInfo xLiteInfo = (NFMsg.RoleLiteInfo)mxPlayerNet.mxReciver.aCharList[i];
                                if (GUI.Button(new Rect(200, i * 50, 150, 50), System.Text.Encoding.Default.GetString(xLiteInfo.noob_name)))
                                {
                                    mxPlayerNet.strRoleName = System.Text.Encoding.Default.GetString(xLiteInfo.noob_name);
                                    NFStart.Instance.GetPlayerNet().nMainRoleID = PlayerReciver.PBToNF(xLiteInfo.id);
                                    mxPlayerNet.mxSender.RequireEnterGameServer(NFStart.Instance.GetPlayerNet().nMainRoleID, mxPlayerNet.strAccount, mxPlayerNet.strRoleName, mxPlayerNet.nServerID);
                                }
                            }
                            
                        }
                        else
                        {
                            mxPlayerNet.strRoleName = GUI.TextField(new Rect(10, 10, 150, 50), mxPlayerNet.strRoleName);
                            if (GUI.Button(new Rect(10, 200, 150, 50), "CreateRole"))
                            {
                                mxPlayerNet.mxSender.RequireCreateRole(mxPlayerNet.strAccount, mxPlayerNet.strRoleName, 0, 0, mxPlayerNet.nServerID);
                            }
                        }
                    }
                    break;

                case PlayerNet.PLAYER_STATE.E_PLAYER_GAMEING:
                    //NFCSectionManager.Instance.SetGameState(NFCSectionManager.UI_SECTION_STATE.UISS_GAMEING);
                    break;

                default:
                    break;

            }
        }
        else
        {
            mxPlayerNet = new PlayerNet();
        }
    }
}
