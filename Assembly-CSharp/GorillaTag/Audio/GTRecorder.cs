using System;
using System.Collections;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x0200106F RID: 4207
	public class GTRecorder : Recorder, ITickSystemPost
	{
		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x06006995 RID: 27029 RVA: 0x00225C4E File Offset: 0x00223E4E
		// (set) Token: 0x06006996 RID: 27030 RVA: 0x00225C56 File Offset: 0x00223E56
		public bool PostTickRunning { get; set; }

		// Token: 0x06006997 RID: 27031 RVA: 0x001807F9 File Offset: 0x0017E9F9
		private void OnEnable()
		{
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06006998 RID: 27032 RVA: 0x001338D3 File Offset: 0x00131AD3
		private void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06006999 RID: 27033 RVA: 0x00225C5F File Offset: 0x00223E5F
		protected override MicWrapper CreateMicWrapper(string micDev, int samplingRateInt, VoiceLogger logger)
		{
			this._micWrapper = new GTMicWrapper(micDev, samplingRateInt, this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment, logger);
			return this._micWrapper;
		}

		// Token: 0x0600699A RID: 27034 RVA: 0x00225C8D File Offset: 0x00223E8D
		private IEnumerator DoTestEcho()
		{
			base.DebugEchoMode = true;
			yield return new WaitForSeconds(this.DebugEchoLength);
			base.DebugEchoMode = false;
			yield return null;
			this._testEchoCoroutine = null;
			yield break;
		}

		// Token: 0x0600699B RID: 27035 RVA: 0x00225C9C File Offset: 0x00223E9C
		public void PostTick()
		{
			if (this._micWrapper != null)
			{
				this._micWrapper.UpdateWrapper(this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment);
			}
		}

		// Token: 0x040078E8 RID: 30952
		public bool AllowPitchAdjustment;

		// Token: 0x040078E9 RID: 30953
		public float PitchAdjustment = 1f;

		// Token: 0x040078EA RID: 30954
		public bool AllowVolumeAdjustment;

		// Token: 0x040078EB RID: 30955
		public float VolumeAdjustment = 1f;

		// Token: 0x040078EC RID: 30956
		public float DebugEchoLength = 5f;

		// Token: 0x040078ED RID: 30957
		private GTMicWrapper _micWrapper;

		// Token: 0x040078EE RID: 30958
		private Coroutine _testEchoCoroutine;
	}
}
