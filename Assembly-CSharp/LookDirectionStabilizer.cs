using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class LookDirectionStabilizer : MonoBehaviour, ISpawnable
{
	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06001011 RID: 4113 RVA: 0x00054A02 File Offset: 0x00052C02
	// (set) Token: 0x06001012 RID: 4114 RVA: 0x00054A0A File Offset: 0x00052C0A
	public bool IsSpawned { get; set; }

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06001013 RID: 4115 RVA: 0x00054A13 File Offset: 0x00052C13
	// (set) Token: 0x06001014 RID: 4116 RVA: 0x00054A1B File Offset: 0x00052C1B
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001015 RID: 4117 RVA: 0x00054A24 File Offset: 0x00052C24
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x00054A30 File Offset: 0x00052C30
	private void Update()
	{
		Transform rigTarget = this.myRig.head.rigTarget;
		if (rigTarget.forward.y < 0f)
		{
			Quaternion quaternion = Quaternion.LookRotation(UnityVectorExtensions.ProjectOntoPlane(rigTarget.up, Vector3.up));
			Quaternion rotation = base.transform.parent.rotation;
			float num = Vector3.Dot(rigTarget.up, Vector3.up);
			base.transform.rotation = Quaternion.Lerp(rotation, quaternion, Mathf.InverseLerp(1f, 0.7f, num));
			return;
		}
		base.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x0400140D RID: 5133
	private VRRig myRig;
}
