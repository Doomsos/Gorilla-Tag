using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001042 RID: 4162
	public class GuidedRefTargetMonoGameObject : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x06006901 RID: 26881 RVA: 0x00223312 File Offset: 0x00221512
		// (set) Token: 0x06006902 RID: 26882 RVA: 0x0022331A File Offset: 0x0022151A
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

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x06006903 RID: 26883 RVA: 0x00013E33 File Offset: 0x00012033
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x06006904 RID: 26884 RVA: 0x0010EB39 File Offset: 0x0010CD39
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06006905 RID: 26885 RVA: 0x00223323 File Offset: 0x00221523
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoGameObject>(this, true);
		}

		// Token: 0x06006906 RID: 26886 RVA: 0x0022332C File Offset: 0x0022152C
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoGameObject>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06006908 RID: 26888 RVA: 0x000743A9 File Offset: 0x000725A9
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06006909 RID: 26889 RVA: 0x000178ED File Offset: 0x00015AED
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040077A8 RID: 30632
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
