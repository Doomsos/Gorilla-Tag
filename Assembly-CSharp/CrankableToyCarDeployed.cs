using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class CrankableToyCarDeployed : MonoBehaviour
{
	// Token: 0x06000F27 RID: 3879 RVA: 0x000506F4 File Offset: 0x0004E8F4
	public void Deploy(CrankableToyCarHoldable holdable, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		this.holdable = holdable;
		holdable.OnCarDeployed();
		base.transform.position = launchPos;
		base.transform.rotation = launchRot;
		base.transform.localScale = holdable.transform.lossyScale;
		this.rb.linearVelocity = releaseVel;
		this.startedAtTimestamp = Time.time;
		this.expiresAtTimestamp = Time.time + lifetime;
		this.isRemote = isRemote;
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0005076C File Offset: 0x0004E96C
	private void Update()
	{
		if (!this.isRemote && Time.time > this.expiresAtTimestamp)
		{
			if (this.holdable != null)
			{
				this.holdable.OnCarReturned();
			}
			return;
		}
		if (!this.wheelDriver.hasCollision)
		{
			this.expiresAtTimestamp -= Time.deltaTime;
			if (!this.offGroundDrivingAudio.isPlaying)
			{
				this.offGroundDrivingAudio.GTPlay();
				this.drivingAudio.Stop();
			}
		}
		else if (!this.drivingAudio.isPlaying)
		{
			this.drivingAudio.GTPlay();
			this.offGroundDrivingAudio.Stop();
		}
		float num = Mathf.InverseLerp(this.startedAtTimestamp, this.expiresAtTimestamp, Time.time);
		float num2 = this.thrustCurve.Evaluate(num);
		this.wheelDriver.SetThrust(this.maxThrust * num2);
	}

	// Token: 0x0400129A RID: 4762
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x0400129B RID: 4763
	[SerializeField]
	private FakeWheelDriver wheelDriver;

	// Token: 0x0400129C RID: 4764
	[SerializeField]
	private Vector3 maxThrust;

	// Token: 0x0400129D RID: 4765
	[SerializeField]
	private AnimationCurve thrustCurve;

	// Token: 0x0400129E RID: 4766
	private float startedAtTimestamp;

	// Token: 0x0400129F RID: 4767
	private float expiresAtTimestamp;

	// Token: 0x040012A0 RID: 4768
	private CrankableToyCarHoldable holdable;

	// Token: 0x040012A1 RID: 4769
	[SerializeField]
	private AudioSource drivingAudio;

	// Token: 0x040012A2 RID: 4770
	[SerializeField]
	private AudioSource offGroundDrivingAudio;

	// Token: 0x040012A3 RID: 4771
	private bool isRemote;
}
