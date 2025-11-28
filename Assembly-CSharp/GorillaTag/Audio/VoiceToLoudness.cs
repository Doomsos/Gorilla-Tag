using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x0200106B RID: 4203
	[RequireComponent(typeof(Recorder))]
	public class VoiceToLoudness : MonoBehaviour
	{
		// Token: 0x06006981 RID: 27009 RVA: 0x0022522D File Offset: 0x0022342D
		protected void Awake()
		{
			this._recorder = base.GetComponent<Recorder>();
		}

		// Token: 0x06006982 RID: 27010 RVA: 0x0022523C File Offset: 0x0022343C
		protected void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
		{
			VoiceInfo info = photonVoiceCreatedParams.Voice.Info;
			LocalVoiceAudioFloat localVoiceAudioFloat = photonVoiceCreatedParams.Voice as LocalVoiceAudioFloat;
			if (localVoiceAudioFloat != null)
			{
				localVoiceAudioFloat.AddPostProcessor(new IProcessor<float>[]
				{
					new ProcessVoiceDataToLoudness(this)
				});
			}
		}

		// Token: 0x040078D2 RID: 30930
		[NonSerialized]
		public float loudness;

		// Token: 0x040078D3 RID: 30931
		private Recorder _recorder;
	}
}
