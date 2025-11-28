using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EBB RID: 3771
	public class CosmeticItemRegistry
	{
		// Token: 0x06005E03 RID: 24067 RVA: 0x001E1B0C File Offset: 0x001DFD0C
		public void Initialize(GameObject[] cosmeticGObjs)
		{
			if (this._isInitialized)
			{
				return;
			}
			this._isInitialized = true;
			foreach (GameObject gameObject in cosmeticGObjs)
			{
				string text = gameObject.name.Replace("LEFT.", "").Replace("RIGHT.", "").TrimEnd();
				CosmeticItemInstance cosmeticItemInstance;
				if (this.nameToCosmeticMap.ContainsKey(text))
				{
					cosmeticItemInstance = this.nameToCosmeticMap[text];
				}
				else
				{
					cosmeticItemInstance = new CosmeticItemInstance();
					CosmeticSO cosmeticSOFromDisplayName = CosmeticsController.instance.GetCosmeticSOFromDisplayName(text);
					cosmeticItemInstance.clippingOffsets = ((cosmeticSOFromDisplayName != null) ? cosmeticSOFromDisplayName.info.anchorAntiIntersectOffsets : CosmeticsController.instance.defaultClipOffsets);
					cosmeticItemInstance.isHoldableItem = (cosmeticSOFromDisplayName != null && cosmeticSOFromDisplayName.info.hasHoldableParts);
					this.nameToCosmeticMap.Add(text, cosmeticItemInstance);
				}
				HoldableObject component = gameObject.GetComponent<HoldableObject>();
				bool flag = gameObject.name.Contains("LEFT.");
				bool flag2 = gameObject.name.Contains("RIGHT.");
				if (cosmeticItemInstance.isHoldableItem && component != null)
				{
					if (component is SnowballThrowable || component is TransferrableObject)
					{
						cosmeticItemInstance.holdableObjects.Add(gameObject);
					}
					else if (flag)
					{
						cosmeticItemInstance.leftObjects.Add(gameObject);
					}
					else if (flag2)
					{
						cosmeticItemInstance.rightObjects.Add(gameObject);
					}
					else
					{
						cosmeticItemInstance.objects.Add(gameObject);
					}
				}
				else if (flag)
				{
					cosmeticItemInstance.leftObjects.Add(gameObject);
				}
				else if (flag2)
				{
					cosmeticItemInstance.rightObjects.Add(gameObject);
				}
				else
				{
					cosmeticItemInstance.objects.Add(gameObject);
				}
				cosmeticItemInstance.dbgname = text;
			}
		}

		// Token: 0x06005E04 RID: 24068 RVA: 0x001E1CC4 File Offset: 0x001DFEC4
		public CosmeticItemInstance Cosmetic(string itemName)
		{
			if (!this._isInitialized)
			{
				Debug.LogError("Tried to use CosmeticItemRegistry before it was initialized!");
				return null;
			}
			if (string.IsNullOrEmpty(itemName) || itemName == "NOTHING")
			{
				return null;
			}
			CosmeticItemInstance result;
			if (!this.nameToCosmeticMap.TryGetValue(itemName, ref result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x04006BF6 RID: 27638
		private bool _isInitialized;

		// Token: 0x04006BF7 RID: 27639
		private Dictionary<string, CosmeticItemInstance> nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		// Token: 0x04006BF8 RID: 27640
		private GameObject nullItem;
	}
}
