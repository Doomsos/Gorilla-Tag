using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000330 RID: 816
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterRippleEffect : MonoBehaviour
{
	// Token: 0x060013B7 RID: 5047 RVA: 0x000726B9 File Offset: 0x000708B9
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.renderer = base.GetComponent<SpriteRenderer>();
		this.ripplePlaybackSpeedHash = Animator.StringToHash(this.ripplePlaybackSpeedName);
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x000726E4 File Offset: 0x000708E4
	public void Destroy()
	{
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x00072700 File Offset: 0x00070900
	public void PlayEffect(WaterVolume volume = null)
	{
		this.waterVolume = volume;
		this.rippleStartTime = Time.time;
		this.animator.SetFloat(this.ripplePlaybackSpeedHash, this.ripplePlaybackSpeed);
		if (this.waterVolume != null && this.waterVolume.Parameters != null)
		{
			this.renderer.color = this.waterVolume.Parameters.rippleSpriteColor;
		}
		Color color = this.renderer.color;
		color.a = 1f;
		this.renderer.color = color;
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x00072798 File Offset: 0x00070998
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 vector = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - vector;
		}
		float num = Mathf.Clamp01((Time.time - this.rippleStartTime - this.fadeOutDelay) / this.fadeOutTime);
		Color color = this.renderer.color;
		color.a = 1f - num;
		this.renderer.color = color;
		if (num >= 1f - Mathf.Epsilon)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x04001E1B RID: 7707
	[SerializeField]
	private float ripplePlaybackSpeed = 1f;

	// Token: 0x04001E1C RID: 7708
	[SerializeField]
	private float fadeOutDelay = 0.5f;

	// Token: 0x04001E1D RID: 7709
	[SerializeField]
	private float fadeOutTime = 1f;

	// Token: 0x04001E1E RID: 7710
	private string ripplePlaybackSpeedName = "RipplePlaybackSpeed";

	// Token: 0x04001E1F RID: 7711
	private int ripplePlaybackSpeedHash;

	// Token: 0x04001E20 RID: 7712
	private float rippleStartTime = -1f;

	// Token: 0x04001E21 RID: 7713
	private Animator animator;

	// Token: 0x04001E22 RID: 7714
	private SpriteRenderer renderer;

	// Token: 0x04001E23 RID: 7715
	private WaterVolume waterVolume;
}
