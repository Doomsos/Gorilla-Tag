using System;
using UnityEngine;

namespace DefaultNamespace
{
	// Token: 0x02000D90 RID: 3472
	[RequireComponent(typeof(SoundBankPlayer))]
	public class SoundBankPlayerCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x0600552A RID: 21802 RVA: 0x001ACDBC File Offset: 0x001AAFBC
		// (set) Token: 0x0600552B RID: 21803 RVA: 0x001ACDC4 File Offset: 0x001AAFC4
		public bool TickRunning { get; set; }

		// Token: 0x0600552C RID: 21804 RVA: 0x001ACDCD File Offset: 0x001AAFCD
		private void Awake()
		{
			this.playAudioLoop = false;
		}

		// Token: 0x0600552D RID: 21805 RVA: 0x0001877F File Offset: 0x0001697F
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x0600552E RID: 21806 RVA: 0x00018787 File Offset: 0x00016987
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x0600552F RID: 21807 RVA: 0x001ACDD8 File Offset: 0x001AAFD8
		public void Tick()
		{
			if (!this.playAudioLoop)
			{
				return;
			}
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null && !this.soundBankPlayer.audioSource.isPlaying)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06005530 RID: 21808 RVA: 0x001ACE40 File Offset: 0x001AB040
		public void PlayAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06005531 RID: 21809 RVA: 0x001ACE8C File Offset: 0x001AB08C
		public void PlayAudioLoop()
		{
			this.playAudioLoop = true;
		}

		// Token: 0x06005532 RID: 21810 RVA: 0x001ACE98 File Offset: 0x001AB098
		public void PlayAudioNonInterrupting()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				if (this.soundBankPlayer.audioSource.isPlaying)
				{
					return;
				}
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06005533 RID: 21811 RVA: 0x001ACEF8 File Offset: 0x001AB0F8
		public void PlayAudioWithTunableVolume(bool leftHand, float fingerValue)
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				float volume = Mathf.Clamp01(fingerValue);
				this.soundBankPlayer.audioSource.volume = volume;
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06005534 RID: 21812 RVA: 0x001ACF5C File Offset: 0x001AB15C
		public void StopAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.audioSource.Stop();
			}
			this.playAudioLoop = false;
		}

		// Token: 0x0400621D RID: 25117
		[SerializeField]
		private SoundBankPlayer soundBankPlayer;

		// Token: 0x0400621E RID: 25118
		private bool playAudioLoop;
	}
}
