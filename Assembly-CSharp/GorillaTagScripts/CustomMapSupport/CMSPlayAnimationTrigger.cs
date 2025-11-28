using System;
using System.Collections.Generic;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000E0B RID: 3595
	public class CMSPlayAnimationTrigger : CMSTrigger
	{
		// Token: 0x060059B2 RID: 22962 RVA: 0x001CAF40 File Offset: 0x001C9140
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(PlayAnimationTriggerSettings))
			{
				PlayAnimationTriggerSettings playAnimationTriggerSettings = (PlayAnimationTriggerSettings)settings;
				this.animatedObjects = playAnimationTriggerSettings.animatedObjects;
				this.animationName = playAnimationTriggerSettings.animationName;
			}
			for (int i = this.animatedObjects.Count - 1; i >= 0; i--)
			{
				if (this.animatedObjects[i].IsNull())
				{
					this.animatedObjects.RemoveAt(i);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x060059B3 RID: 22963 RVA: 0x001CAFC4 File Offset: 0x001C91C4
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			foreach (GameObject gameObject in this.animatedObjects)
			{
				Animator component = gameObject.GetComponent<Animator>();
				if (component.IsNotNull())
				{
					component.Play(this.animationName);
				}
			}
		}

		// Token: 0x040066D1 RID: 26321
		public List<GameObject> animatedObjects = new List<GameObject>();

		// Token: 0x040066D2 RID: 26322
		public string animationName = "";
	}
}
