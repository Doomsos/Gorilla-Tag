using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001041 RID: 4161
	public class GuidedRefTargetMonoComponent : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x060068F8 RID: 26872 RVA: 0x002232BC File Offset: 0x002214BC
		// (set) Token: 0x060068F9 RID: 26873 RVA: 0x002232C4 File Offset: 0x002214C4
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

		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x060068FA RID: 26874 RVA: 0x002232CD File Offset: 0x002214CD
		public Object GuidedRefTargetObject
		{
			get
			{
				return this.targetComponent;
			}
		}

		// Token: 0x060068FB RID: 26875 RVA: 0x0010EB19 File Offset: 0x0010CD19
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x060068FC RID: 26876 RVA: 0x002232D5 File Offset: 0x002214D5
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoComponent>(this, true);
		}

		// Token: 0x060068FD RID: 26877 RVA: 0x002232DE File Offset: 0x002214DE
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoComponent>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060068FF RID: 26879 RVA: 0x000743A9 File Offset: 0x000725A9
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06006900 RID: 26880 RVA: 0x000178ED File Offset: 0x00015AED
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040077A6 RID: 30630
		[SerializeField]
		private Component targetComponent;

		// Token: 0x040077A7 RID: 30631
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
