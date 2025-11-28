using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001043 RID: 4163
	public class GuidedRefTargetMonoTransform : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x170009DE RID: 2526
		// (get) Token: 0x0600690A RID: 26890 RVA: 0x00223320 File Offset: 0x00221520
		// (set) Token: 0x0600690B RID: 26891 RVA: 0x00223328 File Offset: 0x00221528
		GuidedRefBasicTargetInfo IGuidedRefTargetMono.GRefTargetInfo
		{
			get
			{
				return this.guidedRefTargetInfo;
			}
			set
			{
				this.guidedRefTargetInfo = value;
			}
		}

		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x0600690C RID: 26892 RVA: 0x000743A9 File Offset: 0x000725A9
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x0600690D RID: 26893 RVA: 0x0010EB19 File Offset: 0x0010CD19
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x0600690E RID: 26894 RVA: 0x00223331 File Offset: 0x00221531
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoTransform>(this, true);
		}

		// Token: 0x0600690F RID: 26895 RVA: 0x0022333A File Offset: 0x0022153A
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoTransform>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06006911 RID: 26897 RVA: 0x000743A9 File Offset: 0x000725A9
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06006912 RID: 26898 RVA: 0x000178ED File Offset: 0x00015AED
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040077A9 RID: 30633
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
