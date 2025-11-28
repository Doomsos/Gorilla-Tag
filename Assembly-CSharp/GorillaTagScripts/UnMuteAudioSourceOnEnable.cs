using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000E06 RID: 3590
	public class UnMuteAudioSourceOnEnable : MonoBehaviour
	{
		// Token: 0x0600599F RID: 22943 RVA: 0x001CA9BE File Offset: 0x001C8BBE
		public void Awake()
		{
			this.originalVolume = this.audioSource.volume;
		}

		// Token: 0x060059A0 RID: 22944 RVA: 0x001CA9D1 File Offset: 0x001C8BD1
		public void OnEnable()
		{
			this.audioSource.volume = this.originalVolume;
		}

		// Token: 0x060059A1 RID: 22945 RVA: 0x001CA9E4 File Offset: 0x001C8BE4
		public void OnDisable()
		{
			this.audioSource.volume = 0f;
		}

		// Token: 0x040066C5 RID: 26309
		public AudioSource audioSource;

		// Token: 0x040066C6 RID: 26310
		public float originalVolume;
	}
}
