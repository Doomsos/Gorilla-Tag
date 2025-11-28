using System;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000E08 RID: 3592
	public class CMSLuau : CMSTrigger
	{
		// Token: 0x060059AA RID: 22954 RVA: 0x001CABA8 File Offset: 0x001C8DA8
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally)
			{
				LuauVm.touchEventsQueue.Enqueue(base.gameObject);
			}
		}
	}
}
