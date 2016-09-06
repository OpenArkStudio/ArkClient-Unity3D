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
        E_WAITING_PLAYER_LOGIN,//等待登录(已经连接成功)
        E_PLAYER_LOGINING,//登录中
        E_HAS_PLAYER_LOGIN,//登录成功
        E_HAS_PLAYER_SELECT_WORLD,//已选择服务器
        E_WAITING_PLAYER_TO_GATE,//等待连接网关
        E_START_CONNECT_TO_GATE,//已开始连接网关
        E_HAS_PLAYER_GATE,//已连接gate,没验证KEY
        E_WATING_VERIFY,//已连接gate(等待验证KEY)
        E_HAS_VERIFY,//验证成功
        E_HAS_PLAYER_ROLELIST,//有了角色列表
        E_PLAYER_WAITING_TO_GAME,//申请了进游戏，等待进入
        E_PLAYER_GAMEING,//游戏中
        E_DISCOUNT,//掉线

    };

    public PlayerNet()
    {
        mxReciver = new PlayerReciver(this);
        mxSender = new PlayerSender(this);
        mxNet = new NFNet();
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
    public PLAYER_STATE mPlayerState = PLAYER_STATE.E_NONE;

    public NFNet mxNet = null;
    public PlayerReciver mxReciver =  null;
    public PlayerSender mxSender = null; 
}
}
