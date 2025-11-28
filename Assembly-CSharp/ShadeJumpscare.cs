using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class ShadeJumpscare : MonoBehaviour
{
	// Token: 0x0600047B RID: 1147 RVA: 0x00019BBB File Offset: 0x00017DBB
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00019BC9 File Offset: 0x00017DC9
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.startAngle = Random.value * 360f;
		this.audioSource.clip = this.audioClips.GetRandomItem<AudioClip>();
		this.audioSource.GTPlay();
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00019C08 File Offset: 0x00017E08
	private void Update()
	{
		float num = Time.time - this.startTime;
		float num2 = num / this.animationTime;
		this.shadeTransform.SetPositionAndRotation(base.transform.position + new Vector3(0f, this.shadeHeightFunction.Evaluate(num2), 0f), Quaternion.Euler(0f, this.startAngle + num * this.shadeRotationSpeed, 0f));
		float num3 = this.shadeScaleFunction.Evaluate(num2);
		this.shadeTransform.localScale = new Vector3(num3, num3 * this.shadeYScaleMultFunction.Evaluate(num2), num3);
		this.audioSource.volume = this.soundVolumeFunction.Evaluate(num2);
	}

	// Token: 0x04000522 RID: 1314
	[SerializeField]
	private Transform shadeTransform;

	// Token: 0x04000523 RID: 1315
	[SerializeField]
	private float animationTime;

	// Token: 0x04000524 RID: 1316
	[SerializeField]
	private float shadeRotationSpeed = 1f;

	// Token: 0x04000525 RID: 1317
	[SerializeField]
	private AnimationCurve shadeHeightFunction;

	// Token: 0x04000526 RID: 1318
	[SerializeField]
	private AnimationCurve shadeScaleFunction;

	// Token: 0x04000527 RID: 1319
	[SerializeField]
	private AnimationCurve shadeYScaleMultFunction;

	// Token: 0x04000528 RID: 1320
	[SerializeField]
	private AnimationCurve soundVolumeFunction;

	// Token: 0x04000529 RID: 1321
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x0400052A RID: 1322
	private AudioSource audioSource;

	// Token: 0x0400052B RID: 1323
	private float startTime;

	// Token: 0x0400052C RID: 1324
	private float startAngle;
}
