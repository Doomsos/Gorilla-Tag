using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x02000EF4 RID: 3828
	[Serializable]
	public class GorillaText
	{
		// Token: 0x0600601B RID: 24603 RVA: 0x001F00B2 File Offset: 0x001EE2B2
		public void Initialize(Material[] originalMaterials, Material failureMaterial, UnityEvent<string> callback = null, UnityEvent<Material[]> materialCallback = null)
		{
			this.failureMaterial = failureMaterial;
			this.originalMaterials = originalMaterials;
			this.currentMaterials = originalMaterials;
			Debug.Log("Original text = " + this.originalText);
			this.updateTextCallback = callback;
			this.updateMaterialCallback = materialCallback;
		}

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x0600601C RID: 24604 RVA: 0x001F00ED File Offset: 0x001EE2ED
		// (set) Token: 0x0600601D RID: 24605 RVA: 0x001F00F5 File Offset: 0x001EE2F5
		public string Text
		{
			get
			{
				return this.originalText;
			}
			set
			{
				if (this.originalText == value)
				{
					return;
				}
				this.originalText = value;
				if (!this.failedState)
				{
					UnityEvent<string> unityEvent = this.updateTextCallback;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke(value);
				}
			}
		}

		// Token: 0x0600601E RID: 24606 RVA: 0x001F0128 File Offset: 0x001EE328
		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.failureText = failText;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(failText);
			}
			this.currentMaterials = (Material[])this.originalMaterials.Clone();
			this.currentMaterials[0] = this.failureMaterial;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x0600601F RID: 24607 RVA: 0x001F0190 File Offset: 0x001EE390
		public void DisableFailedState()
		{
			this.failedState = false;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.originalText);
			}
			this.failureText = "";
			this.currentMaterials = this.originalMaterials;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x04006ECD RID: 28365
		private string failureText;

		// Token: 0x04006ECE RID: 28366
		private string originalText = string.Empty;

		// Token: 0x04006ECF RID: 28367
		private bool failedState;

		// Token: 0x04006ED0 RID: 28368
		private Material[] originalMaterials;

		// Token: 0x04006ED1 RID: 28369
		private Material failureMaterial;

		// Token: 0x04006ED2 RID: 28370
		internal Material[] currentMaterials;

		// Token: 0x04006ED3 RID: 28371
		private UnityEvent<string> updateTextCallback;

		// Token: 0x04006ED4 RID: 28372
		private UnityEvent<Material[]> updateMaterialCallback;
	}
}
