using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace NFCoreEx
{
	public class NFCLogicEvent : NFILogicEvent
    {
		public NFCLogicEvent()
		{

		}

        #region Instance
        private static NFCLogicEvent _Instance = null;
        public static NFCLogicEvent Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new NFCLogicEvent();
                }
                return _Instance;
            }
        }
        #endregion

		public override void RegisterCallback(int nEventID, NFIEvent.EventHandler handler, NFIDataList valueList)
		{
			mxEventManager.RegisterCallback(nEventID, handler, valueList);
		}

        public override void RemoveCallback(int nEventID, NFIEvent.EventHandler handler, NFIDataList valueList)
        {
             mxEventManager.RemoveCallback(nEventID, handler, valueList);
        }

		public override void DoEvent(int nEventID, NFIDataList valueList)
		{
			mxEventManager.DoEvent(nEventID, valueList);
		}

		NFIEventManager mxEventManager = new NFCEventManager(new NFIDENTID());
    }
}