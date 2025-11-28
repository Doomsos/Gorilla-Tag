using System;
using System.Collections;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class Ballista : MonoBehaviourPun
{
	// Token: 0x06000A92 RID: 2706 RVA: 0x000396AE File Offset: 0x000378AE
	public void TriggerLoad()
	{
		this.animator.SetTrigger(this.loadTriggerHash);
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x000396C1 File Offset: 0x000378C1
	public void TriggerFire()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
	}

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000A94 RID: 2708 RVA: 0x000396D4 File Offset: 0x000378D4
	private float LaunchSpeed
	{
		get
		{
			if (!this.useSpeedOptions)
			{
				return this.launchSpeed;
			}
			return this.speedOptions[this.currentSpeedIndex];
		}
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x000396F4 File Offset: 0x000378F4
	private void Awake()
	{
		this.launchDirection = this.launchEnd.position - this.launchStart.position;
		this.launchRampDistance = this.launchDirection.magnitude;
		this.launchDirection /= this.launchRampDistance;
		this.collidingLayer = LayerMask.NameToLayer("Default");
		this.notCollidingLayer = LayerMask.NameToLayer("Prop");
		this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
		this.animator.SetFloat(this.pitchParamHash, this.pitch);
		this.appliedAnimatorPitch = this.pitch;
		this.RefreshButtonColors();
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x000397A4 File Offset: 0x000379A4
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
		{
			if (this.prevStateHash == this.fireStateHash)
			{
				this.fireCompleteTime = Time.time;
			}
			if (Time.time - this.fireCompleteTime > this.reloadDelay)
			{
				this.animator.SetTrigger(this.loadTriggerHash);
				this.loadStartTime = Time.time;
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash)
		{
			if (Time.time - this.loadStartTime > this.loadTime)
			{
				if (this.playerInTrigger)
				{
					GTPlayer instance = GTPlayer.Instance;
					Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance);
					Vector3 vector = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
					Vector3 vector2 = playerBodyCenterPosition - vector;
					Vector3 vector3 = Vector3.Lerp(Vector3.zero, vector2, Mathf.Exp(-this.playerPullInRate * deltaTime));
					instance.transform.position = instance.transform.position + (vector3 - vector2);
					this.playerReadyToFire = (vector3.sqrMagnitude < this.playerReadyToFireDist * this.playerReadyToFireDist);
				}
				else
				{
					this.playerReadyToFire = false;
				}
				if (this.playerReadyToFire)
				{
					if (PhotonNetwork.InRoom)
					{
						base.photonView.RPC("FireBallistaRPC", 1, Array.Empty<object>());
					}
					this.FireLocal();
				}
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.fireStateHash && !this.playerLaunched && (this.playerReadyToFire || this.playerInTrigger))
		{
			float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			GTPlayer instance2 = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition2 = this.GetPlayerBodyCenterPosition(instance2);
			float num2 = Vector3.Dot(playerBodyCenterPosition2 - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			float num3 = 0.25f / this.launchRampDistance;
			float num4 = Mathf.Max(num + num3, num2);
			float num5 = num4 * this.launchRampDistance;
			Vector3 vector4 = this.launchDirection * num5 + this.launchStart.position;
			instance2.transform.position + (vector4 - playerBodyCenterPosition2);
			instance2.transform.position = instance2.transform.position + (vector4 - playerBodyCenterPosition2);
			instance2.SetPlayerVelocity(Vector3.zero);
			if (num4 >= 1f)
			{
				this.playerLaunched = true;
				instance2.SetPlayerVelocity(this.LaunchSpeed * this.launchDirection);
				instance2.SetMaximumSlipThisFrame();
			}
		}
		this.prevStateHash = currentAnimatorStateInfo.shortNameHash;
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00039A99 File Offset: 0x00037C99
	private void FireLocal()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
		this.playerLaunched = false;
		if (this.debugDrawTrajectoryOnLaunch)
		{
			this.DebugDrawTrajectory(8f);
		}
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00039AC8 File Offset: 0x00037CC8
	private Vector3 GetPlayerBodyCenterPosition(GTPlayer player)
	{
		return player.headCollider.transform.position + Quaternion.Euler(0f, player.headCollider.transform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, -0.15f) + Vector3.down * 0.4f;
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00039B44 File Offset: 0x00037D44
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = true;
		}
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x00039B78 File Offset: 0x00037D78
	private void OnTriggerExit(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = false;
		}
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x00039BA9 File Offset: 0x00037DA9
	[PunRPC]
	public void FireBallistaRPC(PhotonMessageInfo info)
	{
		this.FireLocal();
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x00039BB4 File Offset: 0x00037DB4
	private void UpdatePredictionLine()
	{
		float num = 0.033333335f;
		Vector3 vector = this.launchEnd.position;
		Vector3 vector2 = (this.launchEnd.position - this.launchStart.position).normalized * this.LaunchSpeed;
		for (int i = 0; i < 240; i++)
		{
			this.predictionLinePoints[i] = vector;
			vector += vector2 * num;
			vector2 += Vector3.down * 9.8f * num;
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x00039C4E File Offset: 0x00037E4E
	private IEnumerator DebugDrawTrajectory(float duration)
	{
		this.UpdatePredictionLine();
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
			DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x00039C64 File Offset: 0x00037E64
	private void OnDrawGizmosSelected()
	{
		if (this.launchStart != null && this.launchEnd != null)
		{
			this.UpdatePredictionLine();
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.launchStart.position, this.launchEnd.position);
			Gizmos.DrawLineList(this.predictionLinePoints);
		}
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x00039CC8 File Offset: 0x00037EC8
	public void RefreshButtonColors()
	{
		this.speedZeroButton.isOn = (this.currentSpeedIndex == 0);
		this.speedZeroButton.UpdateColor();
		this.speedOneButton.isOn = (this.currentSpeedIndex == 1);
		this.speedOneButton.UpdateColor();
		this.speedTwoButton.isOn = (this.currentSpeedIndex == 2);
		this.speedTwoButton.UpdateColor();
		this.speedThreeButton.isOn = (this.currentSpeedIndex == 3);
		this.speedThreeButton.UpdateColor();
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x00039D51 File Offset: 0x00037F51
	public void SetSpeedIndex(int index)
	{
		this.currentSpeedIndex = index;
		this.RefreshButtonColors();
	}

	// Token: 0x04000CDF RID: 3295
	public Animator animator;

	// Token: 0x04000CE0 RID: 3296
	public Transform launchStart;

	// Token: 0x04000CE1 RID: 3297
	public Transform launchEnd;

	// Token: 0x04000CE2 RID: 3298
	public Transform launchBone;

	// Token: 0x04000CE3 RID: 3299
	public float reloadDelay = 1f;

	// Token: 0x04000CE4 RID: 3300
	public float loadTime = 1.933f;

	// Token: 0x04000CE5 RID: 3301
	public float playerMagnetismStrength = 3f;

	// Token: 0x04000CE6 RID: 3302
	public float launchSpeed = 20f;

	// Token: 0x04000CE7 RID: 3303
	[Range(0f, 1f)]
	public float pitch;

	// Token: 0x04000CE8 RID: 3304
	private bool useSpeedOptions;

	// Token: 0x04000CE9 RID: 3305
	public float[] speedOptions = new float[]
	{
		10f,
		15f,
		20f,
		25f
	};

	// Token: 0x04000CEA RID: 3306
	public int currentSpeedIndex;

	// Token: 0x04000CEB RID: 3307
	public GorillaPressableButton speedZeroButton;

	// Token: 0x04000CEC RID: 3308
	public GorillaPressableButton speedOneButton;

	// Token: 0x04000CED RID: 3309
	public GorillaPressableButton speedTwoButton;

	// Token: 0x04000CEE RID: 3310
	public GorillaPressableButton speedThreeButton;

	// Token: 0x04000CEF RID: 3311
	private bool debugDrawTrajectoryOnLaunch;

	// Token: 0x04000CF0 RID: 3312
	private int loadTriggerHash = Animator.StringToHash("Load");

	// Token: 0x04000CF1 RID: 3313
	private int fireTriggerHash = Animator.StringToHash("Fire");

	// Token: 0x04000CF2 RID: 3314
	private int pitchParamHash = Animator.StringToHash("Pitch");

	// Token: 0x04000CF3 RID: 3315
	private int idleStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000CF4 RID: 3316
	private int loadStateHash = Animator.StringToHash("Load");

	// Token: 0x04000CF5 RID: 3317
	private int fireStateHash = Animator.StringToHash("Fire");

	// Token: 0x04000CF6 RID: 3318
	private int prevStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000CF7 RID: 3319
	private float fireCompleteTime;

	// Token: 0x04000CF8 RID: 3320
	private float loadStartTime;

	// Token: 0x04000CF9 RID: 3321
	private bool playerInTrigger;

	// Token: 0x04000CFA RID: 3322
	private bool playerReadyToFire;

	// Token: 0x04000CFB RID: 3323
	private bool playerLaunched;

	// Token: 0x04000CFC RID: 3324
	private float playerReadyToFireDist = 0.1f;

	// Token: 0x04000CFD RID: 3325
	private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

	// Token: 0x04000CFE RID: 3326
	private Vector3 launchDirection;

	// Token: 0x04000CFF RID: 3327
	private float launchRampDistance;

	// Token: 0x04000D00 RID: 3328
	private int collidingLayer;

	// Token: 0x04000D01 RID: 3329
	private int notCollidingLayer;

	// Token: 0x04000D02 RID: 3330
	private float playerPullInRate;

	// Token: 0x04000D03 RID: 3331
	private float appliedAnimatorPitch;

	// Token: 0x04000D04 RID: 3332
	private const int predictionLineSamples = 240;

	// Token: 0x04000D05 RID: 3333
	private Vector3[] predictionLinePoints = new Vector3[240];
}
