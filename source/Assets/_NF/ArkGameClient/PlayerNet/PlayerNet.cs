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

namespace PlayerNetClient
{

public class PlayerNet 
{
    public enum PLAYER_STATE
    {
        E_NONE,//等待选择服务器登录
        E_WAITING_CONNET_NET,//等待链接               
        E_CONNET_NET_SUCESS_WAITING_ACCOUNT,//等待登录(已经连接成功) 等待输入账号
        E_PLAYER_LOGINING_WAITING_lOGINRESULT,//登录中
        E_PLAYER_LOGIN_SUCCESSFUL,//登录成功
        E_PLAYER_WAITING_WORLD_LIST,//等待拉取worldlist
        E_PLAYER_WORLD_LIST_SUCCESSFUL_WAITING_SELECT_WORLD,//拉取worldlist成功，等待用户选择world
        E_PLAYER_SELECT_WORLD_WAIT_WORK_KEY,//已经选择了world，等待world的key，ip，网关信息
        E_PLAYER_GET_WORLD_KEY_SUCCESSFUL,//拉取world的key，ip信息等成功
        E_START_CONNECT_TO_GATE,//已开始连接网关
        E_START_CONNECT_TO_GATE_SUCESS_FUL,//链接网关成功
        E_HAS_PLAYER_GATE,//已连接gate,没验证KEY
        E_WATING_VERIFY,//已连接gate(等待验证KEY)
        E_VERIFY_KEY_SUCCESS_FULL,//验证成功
        E_WAIT_ROLELIST,//发送选择的server，服务器返回确认有这个server，客户端在发送请求角色
        E_GETROLELIST_SUCCESSFUL,//获得角色成功
        E_WAIT_SELECT_ROLE,//有了角色列表
        E_WAIT_CREATE_ROLE,//等待创建角色
        E_PLAYER_WAITING_TO_GAME,//申请了进游戏，等待进入
        E_PLAYER_GAMEING,//游戏中
        E_DISCOUNT,//掉线

    };

    public PlayerNet()
    {
        mxReciver = new PlayerReciver(this);
        mxSender = new PlayerSender(this);
        mxNet = new NFCNet();
        mxReciver.Init();
    }

    public delegate void StateChangeDelegation(PLAYER_STATE oldState, PLAYER_STATE newState);
    public bool RegisteredPlayerStateChangeDelegation(StateChangeDelegation msgDelegate)
    {
        if (mxStateChangeDelegation == null)
        {
           mxStateChangeDelegation = msgDelegate;
        }
        else
        {
            mxStateChangeDelegation += msgDelegate;
        }

        return true;
    }

    public PLAYER_STATE GetPlayerState()
    {
        return mPlayerState;
    }    

    public bool ChangePlayerState(PLAYER_STATE xNewState)
    {
        if (mPlayerState == xNewState)
        {
            return false;                                        
        }

        if (null != mxStateChangeDelegation)
        {
            mxStateChangeDelegation(mPlayerState, xNewState);
        }

        mPlayerState = xNewState;
        return true;
    }
    /////////////Logic///////////////////////////////////////////////
    public bool mbDebugMode = false;
    public string strReqSwapSceneID = "1";

    public string strReqMoveX = "0.0";
    public string strReqMoveZ = "0.0";

    public string strReqAcceptTaskID = "taskid";

    public string strReqKillNPCID = "npcid";
    public string strReqUseItemID = "itemid";

    public string strPickUpItemID = "0";

    public string strReqSetProperty = "property";
    public string strReqPropertyValue = "value";

    public string strReqAddItem = "additem";
    public string strAddCount = "count";

    public string strSwapOrigin = "swaporigin";
    public string strSwapTarget = "target";

    public string strChatTargetID = "target";
    public string strType = "0";
    public string strChatData = "data";

    public string strWorldIP = "";
    public int nWorldPort = 0;
    public string strKey = "";

    public string strAccount = "server1";
    public string strPassword = "123456";
    public string strRoleName = "";

    public int nServerID = 0;
    public NFCoreEx.NFIDENTID nMainRoleID = new NFCoreEx.NFIDENTID();//主角ID

    public NFCoreEx.NFIDENTID nTarget = new NFCoreEx.NFIDENTID();
    public Int64 nSceneID = 0;
    public Int64 nLineID = 0;

    public NFINet mxNet = null;
    public PlayerReciver mxReciver =  null;
    public PlayerSender mxSender = null;
    private StateChangeDelegation mxStateChangeDelegation = null;
    private PLAYER_STATE mPlayerState = PLAYER_STATE.E_NONE;
    }
}
