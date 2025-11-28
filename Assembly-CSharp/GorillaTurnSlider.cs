using System;
using UnityEngine;

// Token: 0x0200092A RID: 2346
public class GorillaTurnSlider : MonoBehaviour
{
	// Token: 0x06003BF4 RID: 15348 RVA: 0x0013C97D File Offset: 0x0013AB7D
	private void Awake()
	{
		this.startingLocation = base.transform.position;
		this.SetPosition(this.gorillaTurn.currentSpeed);
	}

	// Token: 0x06003BF5 RID: 15349 RVA: 0x00002789 File Offset: 0x00000989
	private void FixedUpdate()
	{
	}

	// Token: 0x06003BF6 RID: 15350 RVA: 0x0013C9A4 File Offset: 0x0013ABA4
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float num3 = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(num3, this.startingLocation.y, this.startingLocation.z);
	}

	// Token: 0x06003BF7 RID: 15351 RVA: 0x0013CA28 File Offset: 0x0013AC28
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06003BF8 RID: 15352 RVA: 0x0013CA84 File Offset: 0x0013AC84
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
				return;
			}
			base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
		}
	}

	// Token: 0x04004C84 RID: 19588
	public float zRange;

	// Token: 0x04004C85 RID: 19589
	public float maxValue;

	// Token: 0x04004C86 RID: 19590
	public float minValue;

	// Token: 0x04004C87 RID: 19591
	public GorillaTurning gorillaTurn;

	// Token: 0x04004C88 RID: 19592
	private float startingZ;

	// Token: 0x04004C89 RID: 19593
	public Vector3 startingLocation;
}
