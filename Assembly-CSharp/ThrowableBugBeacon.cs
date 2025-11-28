using System;
using UnityEngine;

// Token: 0x02000CCA RID: 3274
public class ThrowableBugBeacon : MonoBehaviour
{
	// Token: 0x1400008E RID: 142
	// (add) Token: 0x06004FD4 RID: 20436 RVA: 0x0019B3D8 File Offset: 0x001995D8
	// (remove) Token: 0x06004FD5 RID: 20437 RVA: 0x0019B40C File Offset: 0x0019960C
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnCall;

	// Token: 0x1400008F RID: 143
	// (add) Token: 0x06004FD6 RID: 20438 RVA: 0x0019B440 File Offset: 0x00199640
	// (remove) Token: 0x06004FD7 RID: 20439 RVA: 0x0019B474 File Offset: 0x00199674
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnDismiss;

	// Token: 0x14000090 RID: 144
	// (add) Token: 0x06004FD8 RID: 20440 RVA: 0x0019B4A8 File Offset: 0x001996A8
	// (remove) Token: 0x06004FD9 RID: 20441 RVA: 0x0019B4DC File Offset: 0x001996DC
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnLock;

	// Token: 0x14000091 RID: 145
	// (add) Token: 0x06004FDA RID: 20442 RVA: 0x0019B510 File Offset: 0x00199710
	// (remove) Token: 0x06004FDB RID: 20443 RVA: 0x0019B544 File Offset: 0x00199744
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnUnlock;

	// Token: 0x14000092 RID: 146
	// (add) Token: 0x06004FDC RID: 20444 RVA: 0x0019B578 File Offset: 0x00199778
	// (remove) Token: 0x06004FDD RID: 20445 RVA: 0x0019B5AC File Offset: 0x001997AC
	public static event ThrowableBugBeacon.ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier;

	// Token: 0x1700076A RID: 1898
	// (get) Token: 0x06004FDE RID: 20446 RVA: 0x0019B5DF File Offset: 0x001997DF
	public ThrowableBug.BugName BugName
	{
		get
		{
			return this.bugName;
		}
	}

	// Token: 0x1700076B RID: 1899
	// (get) Token: 0x06004FDF RID: 20447 RVA: 0x0019B5E7 File Offset: 0x001997E7
	public float Range
	{
		get
		{
			return this.range;
		}
	}

	// Token: 0x06004FE0 RID: 20448 RVA: 0x0019B5EF File Offset: 0x001997EF
	public void Call()
	{
		if (ThrowableBugBeacon.OnCall != null)
		{
			ThrowableBugBeacon.OnCall(this);
		}
	}

	// Token: 0x06004FE1 RID: 20449 RVA: 0x0019B603 File Offset: 0x00199803
	public void Dismiss()
	{
		if (ThrowableBugBeacon.OnDismiss != null)
		{
			ThrowableBugBeacon.OnDismiss(this);
		}
	}

	// Token: 0x06004FE2 RID: 20450 RVA: 0x0019B617 File Offset: 0x00199817
	public void Lock()
	{
		if (ThrowableBugBeacon.OnLock != null)
		{
			ThrowableBugBeacon.OnLock(this);
		}
	}

	// Token: 0x06004FE3 RID: 20451 RVA: 0x0019B62B File Offset: 0x0019982B
	public void Unlock()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x06004FE4 RID: 20452 RVA: 0x0019B63F File Offset: 0x0019983F
	public void ChangeSpeedMultiplier(float f)
	{
		if (ThrowableBugBeacon.OnChangeSpeedMultiplier != null)
		{
			ThrowableBugBeacon.OnChangeSpeedMultiplier(this, f);
		}
	}

	// Token: 0x06004FE5 RID: 20453 RVA: 0x0019B62B File Offset: 0x0019982B
	private void OnDisable()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x04005E8B RID: 24203
	[SerializeField]
	private float range;

	// Token: 0x04005E8C RID: 24204
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x02000CCB RID: 3275
	// (Invoke) Token: 0x06004FE8 RID: 20456
	public delegate void ThrowableBugBeaconEvent(ThrowableBugBeacon tbb);

	// Token: 0x02000CCC RID: 3276
	// (Invoke) Token: 0x06004FEC RID: 20460
	public delegate void ThrowableBugBeaconFloatEvent(ThrowableBugBeacon tbb, float f);
}
