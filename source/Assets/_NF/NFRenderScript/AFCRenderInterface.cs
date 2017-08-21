using UnityEngine;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System;
using AFCoreEx;
using AFTCPClient;
using System.Collections.Generic;

public class AFCRenderInterface
{
    #region Instance
    private static AFCRenderInterface _Instance = null;
    private static readonly object _syncLock = new object();
    public static AFCRenderInterface Instance
    {
        get
        {
            lock (_syncLock)
            {
                if (_Instance == null)
                {
                    _Instance = new AFCRenderInterface();
                    _Instance.Init();
                }
                return _Instance;
            }
        }
    }
    #endregion

    public void Init()
    {
        AFCKernel.Instance.RegisterClassCallBack("Player", OnClassPlayerEventHandler);
        AFCKernel.Instance.RegisterClassCallBack("NPC", OnClassNPCEventHandler);
    }

    private void OnClassPlayerEventHandler(AFIDENTID self, int nContainerID, int nGroupID, AFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
    {
        if (eType == AFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
        {
            string strConfigID = AFCKernel.Instance.QueryPropertyString(self, "ConfigID");
            Vector3 vec = new Vector3();
            vec.x = AFCKernel.Instance.QueryPropertyFloat(self, "X");
            vec.y = AFCKernel.Instance.QueryPropertyFloat(self, "Y");
            vec.z = AFCKernel.Instance.QueryPropertyFloat(self, "Z");

            string strPrefabPath = "";
            if (strConfigID.Length <= 0)
            {
                strPrefabPath = AFCElementManager.Instance.QueryPropertyString("Player", "Prefab");
            }
            else
            {
                strPrefabPath = AFCElementManager.Instance.QueryPropertyString(strConfigID, "Prefab");
            }

            //CreateObject(self, strPrefabPath, vec, strClassName);
        }
        else if (eType == AFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
        {
            //DestroyObject(self);
        }
        
    }

    private void OnClassNPCEventHandler(AFIDENTID self, int nContainerID, int nGroupID, AFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
    {
        if (eType == AFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
        {
            string strConfigID = AFCKernel.Instance.QueryPropertyString(self, "ConfigID");
            Vector3 vec = new Vector3();
            vec.x = AFCKernel.Instance.QueryPropertyFloat(self, "X");
            vec.y = AFCKernel.Instance.QueryPropertyFloat(self, "Y");
            vec.z = AFCKernel.Instance.QueryPropertyFloat(self, "Z");

            string strPrefabPath = "";
            if (strConfigID.Length <= 0)
            {
                strPrefabPath = AFCElementManager.Instance.QueryPropertyString("Player", "Prefab");
            }
            else
            {
                strPrefabPath = AFCElementManager.Instance.QueryPropertyString(strConfigID, "Prefab");
            }

            //CreateObject(self, strPrefabPath, vec, strClassName);
        }
        else if (eType == AFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
        {
            //DestroyObject(self);
        }
        
    }
   
    ///////////////////////////////////////////////////////////////////////////////////////
    Dictionary<AFIDENTID, GameObject> mhtObject = new Dictionary<AFIDENTID, GameObject>();
}