using System;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public class GRFadeAndDestroyLight : MonoBehaviour
{
	// Token: 0x06002C8D RID: 11405 RVA: 0x000F1548 File Offset: 0x000EF748
	private void Start()
	{
		if (this.gameLight != null)
		{
			this.fadeRate = this.gameLight.light.intensity / this.TimeToFade;
		}
		this.timeSinceLastUpdate = Time.time;
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEnable()
	{
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDisable()
	{
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x000F1580 File Offset: 0x000EF780
	public void Update()
	{
		if (Time.time < this.timeSinceLastUpdate || Time.time > this.timeSinceLastUpdate + this.timeSlice)
		{
			this.timeSinceLastUpdate = Time.time;
			float num = this.gameLight.light.intensity;
			num -= this.timeSlice * this.fadeRate;
			if (num <= 0f)
			{
				base.gameObject.Destroy();
				return;
			}
			this.gameLight.light.intensity = num;
		}
	}

	// Token: 0x040039C9 RID: 14793
	public float TimeToFade = 10f;

	// Token: 0x040039CA RID: 14794
	private float fadeRate;

	// Token: 0x040039CB RID: 14795
	public GameLight gameLight;

	// Token: 0x040039CC RID: 14796
	public float timeSlice = 0.1f;

	// Token: 0x040039CD RID: 14797
	public float timeSinceLastUpdate;
}
