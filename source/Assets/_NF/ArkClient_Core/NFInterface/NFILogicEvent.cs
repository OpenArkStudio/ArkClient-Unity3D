using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace NFCoreEx
{
	public abstract class NFILogicEvent
    {
		public abstract void RegisterCallback(int nEventID, NFIEvent.EventHandler handler, NFIDataList valueList);
        public abstract void RemoveCallback(int nEventID, NFIEvent.EventHandler handler, NFIDataList valueList);
		public abstract void DoEvent(int nEventID, NFIDataList valueList);
    }
}