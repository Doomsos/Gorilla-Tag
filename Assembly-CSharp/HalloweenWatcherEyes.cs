using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020009BF RID: 2495
public class HalloweenWatcherEyes : MonoBehaviour
{
	// Token: 0x06003FBF RID: 16319 RVA: 0x00155C84 File Offset: 0x00153E84
	private void Start()
	{
		this.playersViewCenterCosAngle = Mathf.Cos(this.playersViewCenterAngle * 0.017453292f);
		this.watchMinCosAngle = Mathf.Cos(this.watchMaxAngle * 0.017453292f);
		base.StartCoroutine(this.CheckIfNearPlayer(Random.Range(0f, this.timeBetweenUpdates)));
		base.enabled = false;
	}

	// Token: 0x06003FC0 RID: 16320 RVA: 0x00155CE3 File Offset: 0x00153EE3
	private IEnumerator CheckIfNearPlayer(float initialSleep)
	{
		yield return new WaitForSeconds(initialSleep);
		for (;;)
		{
			base.enabled = ((base.transform.position - GTPlayer.Instance.transform.position).sqrMagnitude < this.watchRange * this.watchRange);
			if (!base.enabled)
			{
				this.LookNormal();
			}
			yield return new WaitForSeconds(this.timeBetweenUpdates);
		}
		yield break;
	}

	// Token: 0x06003FC1 RID: 16321 RVA: 0x00155CFC File Offset: 0x00153EFC
	private void Update()
	{
		Vector3 normalized = (GTPlayer.Instance.headCollider.transform.position - base.transform.position).normalized;
		if (Vector3.Dot(GTPlayer.Instance.headCollider.transform.forward, -normalized) > this.playersViewCenterCosAngle)
		{
			this.LookNormal();
			this.pretendingToBeNormalUntilTimestamp = Time.time + this.durationToBeNormalWhenPlayerLooks;
		}
		if (this.pretendingToBeNormalUntilTimestamp > Time.time)
		{
			return;
		}
		if (Vector3.Dot(base.transform.forward, normalized) < this.watchMinCosAngle)
		{
			this.LookNormal();
			return;
		}
		Quaternion quaternion = Quaternion.LookRotation(normalized, base.transform.up);
		Quaternion rotation = Quaternion.Lerp(base.transform.rotation, quaternion, this.lerpValue);
		this.leftEye.transform.rotation = rotation;
		this.rightEye.transform.rotation = rotation;
		if (this.lerpDuration > 0f)
		{
			this.lerpValue = Mathf.MoveTowards(this.lerpValue, 1f, Time.deltaTime / this.lerpDuration);
			return;
		}
		this.lerpValue = 1f;
	}

	// Token: 0x06003FC2 RID: 16322 RVA: 0x00155E2A File Offset: 0x0015402A
	private void LookNormal()
	{
		this.leftEye.transform.localRotation = Quaternion.identity;
		this.rightEye.transform.localRotation = Quaternion.identity;
		this.lerpValue = 0f;
	}

	// Token: 0x040050F3 RID: 20723
	public float timeBetweenUpdates = 5f;

	// Token: 0x040050F4 RID: 20724
	public float watchRange;

	// Token: 0x040050F5 RID: 20725
	public float watchMaxAngle;

	// Token: 0x040050F6 RID: 20726
	public float lerpDuration = 1f;

	// Token: 0x040050F7 RID: 20727
	public float playersViewCenterAngle = 30f;

	// Token: 0x040050F8 RID: 20728
	public float durationToBeNormalWhenPlayerLooks = 3f;

	// Token: 0x040050F9 RID: 20729
	public GameObject leftEye;

	// Token: 0x040050FA RID: 20730
	public GameObject rightEye;

	// Token: 0x040050FB RID: 20731
	private float playersViewCenterCosAngle;

	// Token: 0x040050FC RID: 20732
	private float watchMinCosAngle;

	// Token: 0x040050FD RID: 20733
	private float pretendingToBeNormalUntilTimestamp;

	// Token: 0x040050FE RID: 20734
	private float lerpValue;
}
