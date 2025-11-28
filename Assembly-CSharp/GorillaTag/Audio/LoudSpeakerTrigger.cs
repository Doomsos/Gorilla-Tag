using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02001075 RID: 4213
	public class LoudSpeakerTrigger : MonoBehaviour
	{
		// Token: 0x060069C4 RID: 27076 RVA: 0x002267D6 File Offset: 0x002249D6
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x060069C5 RID: 27077 RVA: 0x002267E0 File Offset: 0x002249E0
		public void OnPlayerEnter(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._network.StartBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x060069C6 RID: 27078 RVA: 0x00226834 File Offset: 0x00224A34
		public void OnPlayerExit(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x0400790B RID: 30987
		public float PitchAdjustment = 1f;

		// Token: 0x0400790C RID: 30988
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x0400790D RID: 30989
		[SerializeField]
		private GTRecorder _recorder;
	}
}
