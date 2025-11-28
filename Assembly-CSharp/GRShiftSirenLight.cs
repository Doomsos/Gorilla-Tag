using System;
using UnityEngine;

// Token: 0x02000702 RID: 1794
public class GRShiftSirenLight : MonoBehaviourTick
{
	// Token: 0x06002E09 RID: 11785 RVA: 0x000FA60C File Offset: 0x000F880C
	public override void Tick()
	{
		if (this.shiftManager == null)
		{
			this.shiftManager = GhostReactor.instance.shiftManager;
			return;
		}
		if (this.redLight.activeSelf != this.shiftManager.ShiftActive)
		{
			this.redLight.SetActive(this.shiftManager.ShiftActive);
		}
		if (this.greenLight.activeSelf == this.shiftManager.ShiftActive)
		{
			this.greenLight.SetActive(!this.shiftManager.ShiftActive);
		}
		if (this.readyRoomLight != null)
		{
			this.readyRoomLight.intensity = (this.shiftManager.ShiftActive ? this.dimLight : this.brightLight);
		}
		if (this.shiftManager.ShiftActive)
		{
			this.redLightParent.localEulerAngles = new Vector3(0f, Time.time * this.rotationRate, 0f);
			return;
		}
		this.greenLightParent.localEulerAngles = new Vector3(0f, Time.time * this.rotationRate, 0f);
	}

	// Token: 0x04003C25 RID: 15397
	public float rotationRate = 1.25f;

	// Token: 0x04003C26 RID: 15398
	public Transform greenLightParent;

	// Token: 0x04003C27 RID: 15399
	public Transform redLightParent;

	// Token: 0x04003C28 RID: 15400
	public GameObject redLight;

	// Token: 0x04003C29 RID: 15401
	public GameObject greenLight;

	// Token: 0x04003C2A RID: 15402
	public GhostReactorShiftManager shiftManager;

	// Token: 0x04003C2B RID: 15403
	public float dimLight;

	// Token: 0x04003C2C RID: 15404
	public float brightLight;

	// Token: 0x04003C2D RID: 15405
	public Light readyRoomLight;
}
