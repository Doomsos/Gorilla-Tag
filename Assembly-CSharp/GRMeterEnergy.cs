using System;
using UnityEngine;

// Token: 0x020006D4 RID: 1748
public class GRMeterEnergy : MonoBehaviour
{
	// Token: 0x06002CD0 RID: 11472 RVA: 0x00002789 File Offset: 0x00000989
	public void Awake()
	{
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x000F2DAC File Offset: 0x000F0FAC
	public void Refresh()
	{
		float num = 0f;
		if (this.tool != null && this.tool.GetEnergyMax() > 0)
		{
			num = (float)this.tool.energy / (float)this.tool.GetEnergyMax();
		}
		num = Mathf.Clamp(num, 0f, 1f);
		GRMeterEnergy.MeterType meterType = this.meterType;
		if (meterType == GRMeterEnergy.MeterType.Linear || meterType != GRMeterEnergy.MeterType.Radial)
		{
			this.meter.localScale = new Vector3(1f, num, 1f);
			return;
		}
		float num2 = Mathf.Lerp(this.angularRange.x, this.angularRange.y, num);
		Vector3 zero = Vector3.zero;
		zero[this.rotationAxis] = num2;
		this.meter.localRotation = Quaternion.Euler(zero);
	}

	// Token: 0x04003A33 RID: 14899
	public GRTool tool;

	// Token: 0x04003A34 RID: 14900
	public Transform meter;

	// Token: 0x04003A35 RID: 14901
	public Transform chargePoint;

	// Token: 0x04003A36 RID: 14902
	public GRMeterEnergy.MeterType meterType;

	// Token: 0x04003A37 RID: 14903
	public Vector2 angularRange = new Vector2(-45f, 45f);

	// Token: 0x04003A38 RID: 14904
	[Range(0f, 2f)]
	public int rotationAxis;

	// Token: 0x020006D5 RID: 1749
	public enum MeterType
	{
		// Token: 0x04003A3A RID: 14906
		Linear,
		// Token: 0x04003A3B RID: 14907
		Radial
	}
}
