using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001035 RID: 4149
	public abstract class BaseGuidedRefTargetMono : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x060068C9 RID: 26825 RVA: 0x0010EB19 File Offset: 0x0010CD19
		protected virtual void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x060068CA RID: 26826 RVA: 0x002220C2 File Offset: 0x002202C2
		protected virtual void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<BaseGuidedRefTargetMono>(this, true);
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x060068CB RID: 26827 RVA: 0x002220CB File Offset: 0x002202CB
		// (set) Token: 0x060068CC RID: 26828 RVA: 0x002220D3 File Offset: 0x002202D3
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

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x060068CD RID: 26829 RVA: 0x00071346 File Offset: 0x0006F546
		Object IGuidedRefTargetMono.GuidedRefTargetObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x060068CE RID: 26830 RVA: 0x002220DC File Offset: 0x002202DC
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<BaseGuidedRefTargetMono>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060068D0 RID: 26832 RVA: 0x000743A9 File Offset: 0x000725A9
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060068D1 RID: 26833 RVA: 0x000178ED File Offset: 0x00015AED
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400778C RID: 30604
		public GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
