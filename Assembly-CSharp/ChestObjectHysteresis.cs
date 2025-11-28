using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000473 RID: 1139
public class ChestObjectHysteresis : MonoBehaviour, ISpawnable
{
	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001CE6 RID: 7398 RVA: 0x00099153 File Offset: 0x00097353
	// (set) Token: 0x06001CE7 RID: 7399 RVA: 0x0009915B File Offset: 0x0009735B
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001CE8 RID: 7400 RVA: 0x00099164 File Offset: 0x00097364
	// (set) Token: 0x06001CE9 RID: 7401 RVA: 0x0009916C File Offset: 0x0009736C
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001CEA RID: 7402 RVA: 0x00099178 File Offset: 0x00097378
	void ISpawnable.OnSpawn(VRRig rig)
	{
		if (!this.angleFollower && (string.IsNullOrEmpty(this.angleFollower_path) || base.transform.TryFindByPath(this.angleFollower_path, out this.angleFollower, false)))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ChestObjectHysteresis: DEACTIVATING! Could not find `angleFollower` using path: \"",
				this.angleFollower_path,
				"\". For component at: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
			base.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x06001CEB RID: 7403 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001CEC RID: 7404 RVA: 0x00099206 File Offset: 0x00097406
	private void Start()
	{
		this.lastAngleQuat = base.transform.rotation;
		this.currentAngleQuat = base.transform.rotation;
	}

	// Token: 0x06001CED RID: 7405 RVA: 0x0009922A File Offset: 0x0009742A
	private void OnEnable()
	{
		ChestObjectHysteresisManager.RegisterCH(this);
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x00099232 File Offset: 0x00097432
	private void OnDisable()
	{
		ChestObjectHysteresisManager.UnregisterCH(this);
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x0009923C File Offset: 0x0009743C
	public void InvokeUpdate()
	{
		this.currentAngleQuat = this.angleFollower.rotation;
		this.angleBetween = Quaternion.Angle(this.currentAngleQuat, this.lastAngleQuat);
		if (this.angleBetween > this.angleHysteresis)
		{
			base.transform.rotation = Quaternion.Slerp(this.currentAngleQuat, this.lastAngleQuat, this.angleHysteresis / this.angleBetween);
			this.lastAngleQuat = base.transform.rotation;
		}
		base.transform.rotation = this.lastAngleQuat;
	}

	// Token: 0x040026DF RID: 9951
	public float angleHysteresis;

	// Token: 0x040026E0 RID: 9952
	public float angleBetween;

	// Token: 0x040026E1 RID: 9953
	public Transform angleFollower;

	// Token: 0x040026E2 RID: 9954
	[Delayed]
	public string angleFollower_path;

	// Token: 0x040026E3 RID: 9955
	private Quaternion lastAngleQuat;

	// Token: 0x040026E4 RID: 9956
	private Quaternion currentAngleQuat;
}
