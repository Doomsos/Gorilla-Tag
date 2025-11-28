using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B4 RID: 4276
	public class SquirtingFlowerBadgeCosmetic : MonoBehaviour, ISpawnable, IFingerFlexListener
	{
		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x06006B0A RID: 27402 RVA: 0x00231D82 File Offset: 0x0022FF82
		// (set) Token: 0x06006B0B RID: 27403 RVA: 0x00231D8A File Offset: 0x0022FF8A
		public VRRig MyRig { get; private set; }

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06006B0C RID: 27404 RVA: 0x00231D93 File Offset: 0x0022FF93
		// (set) Token: 0x06006B0D RID: 27405 RVA: 0x00231D9B File Offset: 0x0022FF9B
		public bool IsSpawned { get; set; }

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06006B0E RID: 27406 RVA: 0x00231DA4 File Offset: 0x0022FFA4
		// (set) Token: 0x06006B0F RID: 27407 RVA: 0x00231DAC File Offset: 0x0022FFAC
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006B10 RID: 27408 RVA: 0x00231DB5 File Offset: 0x0022FFB5
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x06006B11 RID: 27409 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x06006B12 RID: 27410 RVA: 0x00231DBE File Offset: 0x0022FFBE
		private void Update()
		{
			if (!this.restartTimer && Time.time - this.triggeredTime >= this.coolDownTimer)
			{
				this.restartTimer = true;
			}
		}

		// Token: 0x06006B13 RID: 27411 RVA: 0x00231DE4 File Offset: 0x0022FFE4
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

		// Token: 0x06006B14 RID: 27412 RVA: 0x00231E68 File Offset: 0x00230068
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

		// Token: 0x06006B15 RID: 27413 RVA: 0x00231E92 File Offset: 0x00230092
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

		// Token: 0x06006B17 RID: 27415 RVA: 0x00231EA5 File Offset: 0x002300A5
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
