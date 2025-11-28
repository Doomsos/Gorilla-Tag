using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B4 RID: 4276
	public class SquirtingFlowerBadgeCosmetic : MonoBehaviour, ISpawnable, IFingerFlexListener
	{
		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x06006B0A RID: 27402 RVA: 0x00231D62 File Offset: 0x0022FF62
		// (set) Token: 0x06006B0B RID: 27403 RVA: 0x00231D6A File Offset: 0x0022FF6A
		public VRRig MyRig { get; private set; }

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06006B0C RID: 27404 RVA: 0x00231D73 File Offset: 0x0022FF73
		// (set) Token: 0x06006B0D RID: 27405 RVA: 0x00231D7B File Offset: 0x0022FF7B
		public bool IsSpawned { get; set; }

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06006B0E RID: 27406 RVA: 0x00231D84 File Offset: 0x0022FF84
		// (set) Token: 0x06006B0F RID: 27407 RVA: 0x00231D8C File Offset: 0x0022FF8C
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006B10 RID: 27408 RVA: 0x00231D95 File Offset: 0x0022FF95
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x06006B11 RID: 27409 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x06006B12 RID: 27410 RVA: 0x00231D9E File Offset: 0x0022FF9E
		private void Update()
		{
			if (!this.restartTimer && Time.time - this.triggeredTime >= this.coolDownTimer)
			{
				this.restartTimer = true;
			}
		}

		// Token: 0x06006B13 RID: 27411 RVA: 0x00231DC4 File Offset: 0x0022FFC4
		private void OnPlayEffectLocal()
		{
			if (this.particlesToPlay != null)
			{
				this.particlesToPlay.Play();
			}
			if (this.objectToEnable != null)
			{
				this.objectToEnable.SetActive(true);
			}
			if (this.audioSource != null && this.audioToPlay != null)
			{
				this.audioSource.GTPlayOneShot(this.audioToPlay, 1f);
			}
			this.restartTimer = false;
			this.triggeredTime = Time.time;
		}

		// Token: 0x06006B14 RID: 27412 RVA: 0x00231E48 File Offset: 0x00230048
		public void OnButtonPressed(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			if (!this.restartTimer || !this.buttonReleased)
			{
				return;
			}
			this.OnPlayEffectLocal();
			this.buttonReleased = false;
		}

		// Token: 0x06006B15 RID: 27413 RVA: 0x00231E72 File Offset: 0x00230072
		public void OnButtonReleased(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			this.buttonReleased = true;
		}

		// Token: 0x06006B16 RID: 27414 RVA: 0x00002789 File Offset: 0x00000989
		public void OnButtonPressStayed(bool isLeftHand, float value)
		{
		}

		// Token: 0x06006B17 RID: 27415 RVA: 0x00231E85 File Offset: 0x00230085
		public bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.leftHand || isLeftHand) && (this.leftHand || !isLeftHand);
		}

		// Token: 0x04007B59 RID: 31577
		[SerializeField]
		private ParticleSystem particlesToPlay;

		// Token: 0x04007B5A RID: 31578
		[SerializeField]
		private GameObject objectToEnable;

		// Token: 0x04007B5B RID: 31579
		[SerializeField]
		private AudioClip audioToPlay;

		// Token: 0x04007B5C RID: 31580
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007B5D RID: 31581
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04007B5E RID: 31582
		[SerializeField]
		private bool leftHand;

		// Token: 0x04007B5F RID: 31583
		private float triggeredTime;

		// Token: 0x04007B60 RID: 31584
		private bool restartTimer;

		// Token: 0x04007B61 RID: 31585
		private bool buttonReleased = true;
	}
}
