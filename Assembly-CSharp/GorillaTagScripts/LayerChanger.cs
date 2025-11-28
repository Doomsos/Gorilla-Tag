using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DF5 RID: 3573
	public class LayerChanger : MonoBehaviour
	{
		// Token: 0x0600591E RID: 22814 RVA: 0x001C7F71 File Offset: 0x001C6171
		public void InitializeLayers(Transform parent)
		{
			if (!this.layersStored)
			{
				this.StoreOriginalLayers(parent);
				this.layersStored = true;
			}
		}

		// Token: 0x0600591F RID: 22815 RVA: 0x001C7F8C File Offset: 0x001C618C
		private void StoreOriginalLayers(Transform parent)
		{
			if (!this.includeChildren)
			{
				this.StoreOriginalLayers(parent);
				return;
			}
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				this.originalLayers[transform] = transform.gameObject.layer;
				this.StoreOriginalLayers(transform);
			}
		}

		// Token: 0x06005920 RID: 22816 RVA: 0x001C8008 File Offset: 0x001C6208
		public void ChangeLayer(Transform parent, string newLayer)
		{
			if (!this.layersStored)
			{
				Debug.LogWarning("Layers have not been initialized. Call InitializeLayers first.");
				return;
			}
			this.ChangeLayers(parent, LayerMask.NameToLayer(newLayer));
		}

		// Token: 0x06005921 RID: 22817 RVA: 0x001C802C File Offset: 0x001C622C
		private void ChangeLayers(Transform parent, int newLayer)
		{
			if (!this.includeChildren)
			{
				if (!LayerMaskExtensions.Contains(this.restrictedLayers, parent.gameObject.layer))
				{
					parent.gameObject.layer = newLayer;
				}
				return;
			}
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (!LayerMaskExtensions.Contains(this.restrictedLayers, transform.gameObject.layer))
				{
					transform.gameObject.layer = newLayer;
					this.ChangeLayers(transform, newLayer);
				}
			}
		}

		// Token: 0x06005922 RID: 22818 RVA: 0x001C80D4 File Offset: 0x001C62D4
		public void RestoreOriginalLayers()
		{
			if (!this.layersStored)
			{
				Debug.LogWarning("Layers have not been initialized. Call InitializeLayers first.");
				return;
			}
			foreach (KeyValuePair<Transform, int> keyValuePair in this.originalLayers)
			{
				keyValuePair.Key.gameObject.layer = keyValuePair.Value;
			}
		}

		// Token: 0x04006632 RID: 26162
		public LayerMask restrictedLayers;

		// Token: 0x04006633 RID: 26163
		public bool includeChildren = true;

		// Token: 0x04006634 RID: 26164
		private Dictionary<Transform, int> originalLayers = new Dictionary<Transform, int>();

		// Token: 0x04006635 RID: 26165
		private bool layersStored;
	}
}
