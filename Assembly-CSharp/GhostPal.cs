using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class GhostPal : MonoBehaviour
{
	// Token: 0x0600048F RID: 1167 RVA: 0x0001A0FC File Offset: 0x000182FC
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.trailingPosition = base.transform.position;
		this.triggerAudioClipIndex = this.triggerAudioClips.GetRandomIndex<AudioClip>();
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0001A138 File Offset: 0x00018338
	private IEnumerator BounceOnTrigger()
	{
		float startTime = Time.time;
		while (Time.time - startTime < this.bounceOnTrigger[this.bounceOnTrigger.length - 1].time)
		{
			this.bounceHeight = this.bounceOnTrigger.Evaluate(Time.time - startTime);
			yield return null;
		}
		this.bounceHeight = 0f;
		yield break;
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x0001A148 File Offset: 0x00018348
	private void LateUpdate()
	{
		Vector3 position = this.rig.bodyTransform.position;
		Vector3 vector = base.transform.parent.position - position;
		float num = vector.y * 0.5f + this.orbitHeight;
		vector.y = 0f;
		float num2 = vector.magnitude + this.minDistanceFromPlayer;
		vector = vector.normalized * num2;
		vector.y = num + this.bounceHeight;
		double num3 = (double)this.orbitSpeed * (PhotonNetwork.InRoom ? ((PhotonNetwork.Time - (double)this.rig.OwningNetPlayer.UserId.GetStaticHash()) * (double)((this.rig.OwningNetPlayer.ActorNumber % 2 == 0) ? 1 : -1)) : Time.timeAsDouble);
		Vector3 vector2;
		vector2..ctor(this.orbitRadius * (float)Math.Cos(num3), 0f, this.orbitRadius * (float)Math.Sin(num3));
		Vector3 vector3 = position + vector + vector2;
		Vector3 vector4 = vector3 - this.rig.head.rigTarget.position;
		if (Vector3.Dot(this.rig.head.rigTarget.forward, vector4.normalized) >= this.lookAtDotProductMin)
		{
			this.lookAtTime = Mathf.Min(this.lookAtTime + Time.deltaTime, Mathf.Max(this.rotateTowardsPlayerFromLookTime[this.rotateTowardsPlayerFromLookTime.length - 1].time, this.minLookTimeToTrigger));
			if (this.lookAtTime >= this.minLookTimeToTrigger && !this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.friendlyAnimID);
				this.bounceCoroutine = base.StartCoroutine(this.BounceOnTrigger());
				this.triggerAudioSource.pitch = Random.Range(this.triggerAudioPitchMinMax.x, this.triggerAudioPitchMinMax.y);
				this.triggerAudioSource.clip = this.triggerAudioClips[this.triggerAudioClipIndex];
				this.triggerAudioSource.GTPlay();
				this.triggerAudioClipIndex = (this.triggerAudioClipIndex + Random.Range(0, this.triggerAudioClips.Length - 1)) % this.triggerAudioClips.Length;
				this.hasTriggered = true;
			}
		}
		else
		{
			this.lookAtTime = Mathf.Max(this.lookAtTime - Time.deltaTime, 0f);
			if (this.lookAtTime < this.minLookTimeToTrigger && this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.neutralAnimID);
				this.hasTriggered = false;
			}
		}
		if ((vector3 - this.trailingPosition).sqrMagnitude > 0.1f)
		{
			float num4 = 1f - Mathf.Exp(-this.faceMovementDirectionStrength * Time.deltaTime);
			this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector3, num4);
		}
		Quaternion quaternion = Quaternion.Slerp(Quaternion.LookRotation(vector3 - this.trailingPosition, Vector3.up), Quaternion.LookRotation(-vector4, Vector3.up), this.rotateTowardsPlayerFromLookTime.Evaluate(this.lookAtTime));
		base.transform.SetPositionAndRotation(vector3, quaternion);
	}

	// Token: 0x0400054D RID: 1357
	[SerializeField]
	private float minDistanceFromPlayer = 1f;

	// Token: 0x0400054E RID: 1358
	[SerializeField]
	private float orbitRadius = 1f;

	// Token: 0x0400054F RID: 1359
	[SerializeField]
	private float orbitHeight = 1f;

	// Token: 0x04000550 RID: 1360
	[SerializeField]
	private float orbitSpeed = 0.1f;

	// Token: 0x04000551 RID: 1361
	[SerializeField]
	private float faceMovementDirectionStrength = 1f;

	// Token: 0x04000552 RID: 1362
	[Space]
	[SerializeField]
	private float lookAtDotProductMin = 0.95f;

	// Token: 0x04000553 RID: 1363
	[SerializeField]
	private AnimationCurve rotateTowardsPlayerFromLookTime;

	// Token: 0x04000554 RID: 1364
	[SerializeField]
	private float minLookTimeToTrigger = 2f;

	// Token: 0x04000555 RID: 1365
	[SerializeField]
	private AnimationCurve bounceOnTrigger;

	// Token: 0x04000556 RID: 1366
	[SerializeField]
	private AudioSource triggerAudioSource;

	// Token: 0x04000557 RID: 1367
	[SerializeField]
	private Vector2 triggerAudioPitchMinMax = new Vector2(0.9f, 1.1f);

	// Token: 0x04000558 RID: 1368
	[SerializeField]
	private AudioClip[] triggerAudioClips;

	// Token: 0x04000559 RID: 1369
	private VRRig rig;

	// Token: 0x0400055A RID: 1370
	private Animator animator;

	// Token: 0x0400055B RID: 1371
	private float lookAtTime;

	// Token: 0x0400055C RID: 1372
	private bool hasTriggered;

	// Token: 0x0400055D RID: 1373
	private Coroutine bounceCoroutine;

	// Token: 0x0400055E RID: 1374
	private float bounceHeight;

	// Token: 0x0400055F RID: 1375
	private Vector3 trailingPosition;

	// Token: 0x04000560 RID: 1376
	private int triggerAudioClipIndex;

	// Token: 0x04000561 RID: 1377
	private int neutralAnimID = Animator.StringToHash("Neutral");

	// Token: 0x04000562 RID: 1378
	private int friendlyAnimID = Animator.StringToHash("Friendly");
}
