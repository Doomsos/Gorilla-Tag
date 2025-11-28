using System;
using GorillaGameModes;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000E0D RID: 3597
	public class CMSTagZone : CMSTrigger
	{
		// Token: 0x060059CB RID: 22987 RVA: 0x001CB997 File Offset: 0x001C9B97
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally)
			{
				GameMode.ReportHit();
			}
		}
	}
}
