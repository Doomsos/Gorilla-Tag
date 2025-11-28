using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000064 RID: 100
public class CrittersNoiseMaker : CrittersToolThrowable
{
	// Token: 0x06000239 RID: 569 RVA: 0x0000DD06 File Offset: 0x0000BF06
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			if (this.destroyOnImpact || this.playOnce)
			{
				this.PlaySingleNoise();
				return;
			}
			this.StartPlayingRepeatNoise();
		}
	}

	// Token: 0x0600023A RID: 570 RVA: 0x0000DD33 File Offset: 0x0000BF33
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		this.OnImpact(impactedCritter.transform.position, impactedCritter.transform.up);
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000DD51 File Offset: 0x0000BF51
	protected override void OnPickedUp()
	{
		this.StopPlayRepeatNoise();
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000DD5C File Offset: 0x0000BF5C
	private void PlaySingleNoise()
	{
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, this.actorId, base.transform.position);
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000DDD9 File Offset: 0x0000BFD9
	private void StartPlayingRepeatNoise()
	{
		this.StopPlayRepeatNoise();
		this.repeatPlayNoise = base.StartCoroutine(this.PlayRepeatNoise());
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000DDF3 File Offset: 0x0000BFF3
	private void StopPlayRepeatNoise()
	{
		if (this.repeatPlayNoise != null)
		{
			base.StopCoroutine(this.repeatPlayNoise);
			this.repeatPlayNoise = null;
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000DE10 File Offset: 0x0000C010
	private IEnumerator PlayRepeatNoise()
	{
		int num = Mathf.FloorToInt(this.repeatNoiseDuration / this.repeatNoiseRate);
		int num2;
		for (int i = num; i > 0; i = num2 - 1)
		{
			this.PlaySingleNoise();
			yield return new WaitForSeconds(this.repeatNoiseRate);
			num2 = i;
		}
		if (this.destroyAfterPlayingRepeatNoise)
		{
			this.shouldDisable = true;
		}
		yield break;
	}

	// Token: 0x04000292 RID: 658
	[Header("Noise Maker")]
	public int soundSubIndex = 3;

	// Token: 0x04000293 RID: 659
	public bool playOnce = true;

	// Token: 0x04000294 RID: 660
	public float repeatNoiseDuration;

	// Token: 0x04000295 RID: 661
	public float repeatNoiseRate;

	// Token: 0x04000296 RID: 662
	public bool destroyAfterPlayingRepeatNoise = true;

	// Token: 0x04000297 RID: 663
	private Coroutine repeatPlayNoise;
}
