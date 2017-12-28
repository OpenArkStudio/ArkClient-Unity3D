using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AFCWindowEvent : MonoBehaviour
{
    public enum WCHAT_CONTROL_ENUM
    {
        WCCE_ROOT,
        WCCE_STOP,
        WCCE_GLOBAL,
        WCCE_GUILD,
        WCCE_PRIVATE,
        WCCE_ROOM,
        WCCE_TEAM,


        //WCCE_ROOT = 10,
        WCCE_GLOBAL_PANEL = 11,
        WCCE_GUILD_PANEL = 12,
        WCCE_TEAM_PANEL = 13,
        WCCE_PRIVATE_PANEL = 14,

        WCCE_GLOBAL_DATA = 21,
        WCCE_GUILD_DATA = 22,
        WCCE_TEAM_DATA = 23,
        WCCE_PRIVATE_DATA = 24,

        WCCE_GLOBAL_INPUT = 31,
        WCCE_GUILD_INPUT = 32,
        WCCE_TEAM_INPUT = 33,
        WCCE_PRIVATE_INPUT = 34,

        WCCE_SEND_GLOBAL = 100,
        WCCE_SEND_GUILD = 101,
        WCCE_SEND_TEAM = 102,
        WCCE_SEND_PRIVATE = 103,
    }

    //////////all the windows logic class must need the next three menber elements
    public WCHAT_CONTROL_ENUM eControlType;
    private static Hashtable mhtWindow = new Hashtable();
    /////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////////////

    void Awake()
    {
        mhtWindow[eControlType] = this.gameObject;
    }

    // Use this for initialization
    void Start()
    {
        Button btn = this.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(delegate () { this.OnClick(this.gameObject); });

        if (WCHAT_CONTROL_ENUM.WCCE_ROOT == eControlType)
        {
            //Init Data
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnClick()
    {

    }

    void OnClick(GameObject go)
    {
        switch (eControlType)
        {
            case WCHAT_CONTROL_ENUM.WCCE_STOP:
                {
                    //NFCWindowManager.Instance.HideWindows(NFCWindows.UI_WIN_STATE.UI_WIN_CHAT);
                }
                break;
            case WCHAT_CONTROL_ENUM.WCCE_SEND_GLOBAL:
                {
                    GameObject xInput = (GameObject)mhtWindow[WCHAT_CONTROL_ENUM.WCCE_GLOBAL_INPUT];
                    if (null != xInput)
                    {
                        InputField xUIData = xInput.GetComponent<InputField>();
                        string strTalkData = xUIData.text;

                        Chat(AFMsg.ReqAckPlayerChat.EGameChatType.EGCT_WORLD, strTalkData);
                    }
                }
                break;
            case WCHAT_CONTROL_ENUM.WCCE_SEND_GUILD:
                {
                    GameObject xInput = (GameObject)mhtWindow[WCHAT_CONTROL_ENUM.WCCE_GUILD_INPUT];
                    if (null != xInput)
                    {
                        InputField xUIData = xInput.GetComponent<InputField>();
                        string strTalkData = xUIData.text;

                        Chat(AFMsg.ReqAckPlayerChat.EGameChatType.EGCT_GUILD, strTalkData);
                    }
                }
                break;
            case WCHAT_CONTROL_ENUM.WCCE_SEND_PRIVATE:
                {
                    GameObject xInput = (GameObject)mhtWindow[WCHAT_CONTROL_ENUM.WCCE_PRIVATE_INPUT];
                    if (null != xInput)
                    {
                        InputField xUIData = xInput.GetComponent<InputField>();
                        string strTalkData = xUIData.text;

                        Chat(AFMsg.ReqAckPlayerChat.EGameChatType.EGCT_PRIVATE, strTalkData);
                    }
                }
                break;
            case WCHAT_CONTROL_ENUM.WCCE_SEND_TEAM:
                {
                    GameObject xInput = (GameObject)mhtWindow[WCHAT_CONTROL_ENUM.WCCE_TEAM_INPUT];
                    if (null != xInput)
                    {
                        InputField xUIData = xInput.GetComponent<InputField>();
                        string strTalkData = xUIData.text;

                        Chat(AFMsg.ReqAckPlayerChat.EGameChatType.EGCT_TEAM, strTalkData);
                    }
                }
                break;
            default:
                break;
        }

    }

    void Chat(AFMsg.ReqAckPlayerChat.EGameChatType eType, string strTalkData)
    {
      
                   
    }
}
