using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x0200106D RID: 4205
	public class DuplicateAudioSource : MonoBehaviour
	{
		// Token: 0x06006987 RID: 27015 RVA: 0x002252A6 File Offset: 0x002234A6
		public void SetTargetAudioSource(AudioSource target)
		{
			this.TargetAudioSource = target;
			this.StartDuplicating();
		}

		// Token: 0x06006988 RID: 27016 RVA: 0x002252B8 File Offset: 0x002234B8
		[ContextMenu("Start Duplicating")]
		public void StartDuplicating()
		{
			this._isDuplicating = true;
			this._audioSource.loop = this.TargetAudioSource.loop;
			this._audioSource.clip = this.TargetAudioSource.clip;
			if (this.TargetAudioSource.isPlaying)
			{
				this._audioSource.Play();
			}
		}

		// Token: 0x06006989 RID: 27017 RVA: 0x00225310 File Offset: 0x00223510
		[ContextMenu("Stop Duplicating")]
		public void StopDuplicating()
		{
			this._isDuplicating = false;
			this._audioSource.Stop();
		}

		// Token: 0x0600698A RID: 27018 RVA: 0x00225324 File Offset: 0x00223524
		public void LateUpdate()
		{
			if (this._isDuplicating)
			{
				if (this.TargetAudioSource.isPlaying && !this._audioSource.isPlaying)
				{
					this._audioSource.Play();
					return;
				}
				if (!this.TargetAudioSource.isPlaying && this._audioSource.isPlaying)
				{
					this._audioSource.Stop();
				}
			}
		}

		// Token: 0x040078D5 RID: 30933
		public AudioSource TargetAudioSource;

		// Token: 0x040078D6 RID: 30934
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040078D7 RID: 30935
		[SerializeField]
		private bool _isDuplicating;
	}
}
