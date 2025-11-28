using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000339 RID: 825
public class ZoneRootRegister : MonoBehaviour
{
	// Token: 0x060013E6 RID: 5094 RVA: 0x0007356A File Offset: 0x0007176A
	private void Awake()
	{
		this.watchableSlot.Value = base.gameObject;
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x0007357D File Offset: 0x0007177D
	private void OnDestroy()
	{
		this.watchableSlot.Value = null;
	}

	// Token: 0x04001E78 RID: 7800
	[SerializeField]
	private WatchableGameObjectSO watchableSlot;
}
