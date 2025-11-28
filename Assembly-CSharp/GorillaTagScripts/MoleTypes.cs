using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000D9E RID: 3486
	public class MoleTypes : MonoBehaviour
	{
		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x06005594 RID: 21908 RVA: 0x001AEAB5 File Offset: 0x001ACCB5
		// (set) Token: 0x06005595 RID: 21909 RVA: 0x001AEABD File Offset: 0x001ACCBD
		public bool IsLeftSideMoleType { get; set; }

		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x06005596 RID: 21910 RVA: 0x001AEAC6 File Offset: 0x001ACCC6
		// (set) Token: 0x06005597 RID: 21911 RVA: 0x001AEACE File Offset: 0x001ACCCE
		public Mole MoleContainerParent { get; set; }

		// Token: 0x06005598 RID: 21912 RVA: 0x001AEAD7 File Offset: 0x001ACCD7
		private void Start()
		{
			this.MoleContainerParent = base.GetComponentInParent<Mole>();
			if (this.MoleContainerParent)
			{
				this.IsLeftSideMoleType = this.MoleContainerParent.IsLeftSideMole;
			}
		}

		// Token: 0x04006298 RID: 25240
		public bool isHazard;

		// Token: 0x04006299 RID: 25241
		public int scorePoint = 1;

		// Token: 0x0400629A RID: 25242
		public MeshRenderer MeshRenderer;

		// Token: 0x0400629B RID: 25243
		public Material monkeMoleDefaultMaterial;

		// Token: 0x0400629C RID: 25244
		public Material monkeMoleHitMaterial;
	}
}
