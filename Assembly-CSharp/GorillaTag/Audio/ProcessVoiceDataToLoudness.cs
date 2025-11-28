using System;
using Photon.Voice;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x0200106C RID: 4204
	internal class ProcessVoiceDataToLoudness : IProcessor<float>, IDisposable
	{
		// Token: 0x06006984 RID: 27012 RVA: 0x00225259 File Offset: 0x00223459
		public ProcessVoiceDataToLoudness(VoiceToLoudness voiceToLoudness)
		{
			this._voiceToLoudness = voiceToLoudness;
		}

		// Token: 0x06006985 RID: 27013 RVA: 0x00225268 File Offset: 0x00223468
		public float[] Process(float[] buf)
		{
			float num = 0f;
			for (int i = 0; i < buf.Length; i++)
			{
				num += Mathf.Abs(buf[i]);
			}
			this._voiceToLoudness.loudness = num / (float)buf.Length;
			return buf;
		}

		// Token: 0x06006986 RID: 27014 RVA: 0x00002789 File Offset: 0x00000989
		public void Dispose()
		{
		}

		// Token: 0x040078D4 RID: 30932
		private VoiceToLoudness _voiceToLoudness;
	}
}
