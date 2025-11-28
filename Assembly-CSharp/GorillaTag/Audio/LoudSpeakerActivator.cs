using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02001073 RID: 4211
	public class LoudSpeakerActivator : MonoBehaviour
	{
		// Token: 0x060069B1 RID: 27057 RVA: 0x0022615B File Offset: 0x0022435B
		private void Awake()
		{
			this._isLocal = this.IsParentedToLocalRig();
			if (!this._isLocal)
			{
				this._nonlocalRig = base.transform.root.GetComponent<VRRig>();
			}
		}

		// Token: 0x060069B2 RID: 27058 RVA: 0x00226188 File Offset: 0x00224388
		private bool IsParentedToLocalRig()
		{
			if (VRRigCache.Instance.localRig == null)
			{
				return false;
			}
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				if (parent == VRRigCache.Instance.localRig.transform)
				{
					return true;
				}
				parent = parent.parent;
			}
			return false;
		}

		// Token: 0x060069B3 RID: 27059 RVA: 0x002261E1 File Offset: 0x002243E1
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x060069B4 RID: 27060 RVA: 0x002261EC File Offset: 0x002243EC
		public void StartLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StartBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = true;
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._recorder.AllowVolumeAdjustment = true;
				this._recorder.VolumeAdjustment = this.VolumeAdjustment;
				this._network.StartBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x060069B5 RID: 27061 RVA: 0x002262E4 File Offset: 0x002244E4
		public void StopLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StopBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (!this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = false;
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._recorder.AllowVolumeAdjustment = false;
				this._recorder.VolumeAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x040078FE RID: 30974
		public float PitchAdjustment = 1f;

		// Token: 0x040078FF RID: 30975
		public float VolumeAdjustment = 2.5f;

		// Token: 0x04007900 RID: 30976
		public bool IsBroadcasting;

		// Token: 0x04007901 RID: 30977
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x04007902 RID: 30978
		[SerializeField]
		private GTRecorder _recorder;

		// Token: 0x04007903 RID: 30979
		private bool _isLocal;

		// Token: 0x04007904 RID: 30980
		private VRRig _nonlocalRig;
	}
}
