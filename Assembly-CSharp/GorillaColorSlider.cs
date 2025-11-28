using System;
using UnityEngine;

// Token: 0x02000909 RID: 2313
public class GorillaColorSlider : MonoBehaviour
{
	// Token: 0x06003B1C RID: 15132 RVA: 0x00138B39 File Offset: 0x00136D39
	private void Start()
	{
		if (!this.setRandomly)
		{
			this.startingLocation = base.transform.position;
		}
	}

	// Token: 0x06003B1D RID: 15133 RVA: 0x00138B54 File Offset: 0x00136D54
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float num3 = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(num3, this.startingLocation.y, this.startingLocation.z);
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x06003B1E RID: 15134 RVA: 0x00138BF4 File Offset: 0x00136DF4
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06003B1F RID: 15135 RVA: 0x00138C50 File Offset: 0x00136E50
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
			else
			{
				base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
		}
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x04004B74 RID: 19316
	public bool setRandomly;

	// Token: 0x04004B75 RID: 19317
	public float zRange;

	// Token: 0x04004B76 RID: 19318
	public float maxValue;

	// Token: 0x04004B77 RID: 19319
	public float minValue;

	// Token: 0x04004B78 RID: 19320
	public Vector3 startingLocation;

	// Token: 0x04004B79 RID: 19321
	public int valueIndex;

	// Token: 0x04004B7A RID: 19322
	public float valueImReporting;

	// Token: 0x04004B7B RID: 19323
	public GorillaTriggerBox gorilla;

	// Token: 0x04004B7C RID: 19324
	private float startingZ;
}
