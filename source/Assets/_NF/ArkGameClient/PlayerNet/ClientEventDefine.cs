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
using ProtoBuf;
using NFTCPClient;
using NFMsg;
using NFCoreEx;

namespace PlayerNetClient
{
    enum ClientEventDefine
    {
        EventDefine_LoadSelectRole = 1,//加载选择角色场景
        EventDefine_Swap_Scene = 2,//切换场景
        EventDefine_MoveTo = 3,//移动到
        EVENTDEFINE_MOVE_IMMUNE = 4,//直接移动到
        EVENTDEFINE_USESKILL = 5,//使用技能
        EVENTDEFINE_SHOWWORLDLIST = 6,//服务器列表
    }
}