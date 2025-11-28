using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	// Token: 0x02000F65 RID: 3941
	public class SocialCoconutCamera : MonoBehaviour
	{
		// Token: 0x06006292 RID: 25234 RVA: 0x001FC3B3 File Offset: 0x001FA5B3
		private void Awake()
		{
			if (this._propertyBlock == null)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x06006293 RID: 25235 RVA: 0x001FC3EB File Offset: 0x001FA5EB
		public void SetVisualsActive(bool active)
		{
			this._isActive = active;
			this._visuals.SetActive(active);
		}

		// Token: 0x06006294 RID: 25236 RVA: 0x001FC400 File Offset: 0x001FA600
		public void SetRecordingState(bool isRecording)
		{
			if (!this._isActive)
			{
				return;
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, isRecording ? 1 : 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x04007134 RID: 28980
		[SerializeField]
		private GameObject _visuals;

		// Token: 0x04007135 RID: 28981
		[SerializeField]
		private MeshRenderer _bodyRenderer;

		// Token: 0x04007136 RID: 28982
		private bool _isActive;

		// Token: 0x04007137 RID: 28983
		private MaterialPropertyBlock _propertyBlock;

		// Token: 0x04007138 RID: 28984
		private string IS_RECORDING = "_Is_Recording";
	}
}
