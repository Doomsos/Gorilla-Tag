using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005D7 RID: 1495
public class ElderGorilla : MonoBehaviour
{
	// Token: 0x060025B5 RID: 9653 RVA: 0x000C97BC File Offset: 0x000C79BC
	private void Update()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}
		if (GTPlayer.Instance.inOverlay || !GTPlayer.Instance.isUserPresent)
		{
			return;
		}
		this.tHMD = GTPlayer.Instance.headCollider.transform;
		this.tLeftHand = GTPlayer.Instance.GetControllerTransform(true);
		this.tRightHand = GTPlayer.Instance.GetControllerTransform(false);
		if (Time.time - this.timeLastValidArmDist > 1f)
		{
			this.CheckHandDistance(this.tLeftHand);
			this.CheckHandDistance(this.tRightHand);
		}
		this.CheckHeight();
		this.CheckMicVolume();
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x000C9860 File Offset: 0x000C7A60
	private void CheckHandDistance(Transform hand)
	{
		float num = Vector3.Distance(hand.localPosition, this.tHMD.localPosition);
		if (num >= 1f)
		{
			return;
		}
		if (num >= 0.75f)
		{
			this.countValidArmDists++;
			this.timeLastValidArmDist = Time.time;
		}
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000C98B0 File Offset: 0x000C7AB0
	private void CheckHeight()
	{
		float y = this.tHMD.localPosition.y;
		if (!this.trackingHeadHeight)
		{
			this.trackedHeadHeight = y - 0.05f;
			this.timerTrackedHeadHeight = 0f;
		}
		else if (this.trackedHeadHeight < y)
		{
			this.trackingHeadHeight = false;
		}
		if (this.trackingHeadHeight)
		{
			if (this.timerTrackedHeadHeight >= 1f)
			{
				this.savedHeadHeight = y;
				this.trackingHeadHeight = false;
				return;
			}
			this.timerTrackedHeadHeight += Time.deltaTime;
		}
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x000C9936 File Offset: 0x000C7B36
	private void CheckMicVolume()
	{
		float currentPeakAmp = GorillaTagger.Instance.myRecorder.LevelMeter.CurrentPeakAmp;
	}

	// Token: 0x04003157 RID: 12631
	private const float MAX_HAND_DIST = 1f;

	// Token: 0x04003158 RID: 12632
	private const float COOLDOWN_HAND_DIST = 1f;

	// Token: 0x04003159 RID: 12633
	private const float VALID_HAND_DIST = 0.75f;

	// Token: 0x0400315A RID: 12634
	private const float TIME_VALID_HEAD_HEIGHT = 1f;

	// Token: 0x0400315B RID: 12635
	private Transform tHMD;

	// Token: 0x0400315C RID: 12636
	private Transform tLeftHand;

	// Token: 0x0400315D RID: 12637
	private Transform tRightHand;

	// Token: 0x0400315E RID: 12638
	private int countValidArmDists;

	// Token: 0x0400315F RID: 12639
	private float timeLastValidArmDist;

	// Token: 0x04003160 RID: 12640
	private bool trackingHeadHeight;

	// Token: 0x04003161 RID: 12641
	private float trackedHeadHeight;

	// Token: 0x04003162 RID: 12642
	private float timerTrackedHeadHeight;

	// Token: 0x04003163 RID: 12643
	private float savedHeadHeight = 1.5f;
}
