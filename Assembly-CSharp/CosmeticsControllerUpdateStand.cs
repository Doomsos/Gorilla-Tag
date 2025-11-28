using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200045E RID: 1118
public class CosmeticsControllerUpdateStand : MonoBehaviour
{
	// Token: 0x06001C5D RID: 7261 RVA: 0x00096A18 File Offset: 0x00094C18
	public GameObject ReturnChildWithCosmeticNameMatch(Transform parentTransform)
	{
		GameObject gameObject = null;
		using (IEnumerator enumerator = parentTransform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = (Transform)enumerator.Current;
				if (child.gameObject.activeInHierarchy && this.cosmeticsController.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => child.name == x.itemName) > -1)
				{
					return child.gameObject;
				}
				gameObject = this.ReturnChildWithCosmeticNameMatch(child);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
		}
		return gameObject;
	}

	// Token: 0x04002662 RID: 9826
	public CosmeticsController cosmeticsController;

	// Token: 0x04002663 RID: 9827
	public bool FailEntitlement;

	// Token: 0x04002664 RID: 9828
	public bool PlayerUnlocked;

	// Token: 0x04002665 RID: 9829
	public bool ItemNotGrantedYet;

	// Token: 0x04002666 RID: 9830
	public bool ItemSuccessfullyGranted;

	// Token: 0x04002667 RID: 9831
	public bool AttemptToConsumeEntitlement;

	// Token: 0x04002668 RID: 9832
	public bool EntitlementSuccessfullyConsumed;

	// Token: 0x04002669 RID: 9833
	public bool LockSuccessfullyCleared;

	// Token: 0x0400266A RID: 9834
	public bool RunDebug;

	// Token: 0x0400266B RID: 9835
	public Transform textParent;

	// Token: 0x0400266C RID: 9836
	private CosmeticsController.CosmeticItem outItem;

	// Token: 0x0400266D RID: 9837
	public HeadModel[] inventoryHeadModels;

	// Token: 0x0400266E RID: 9838
	public string headModelsPrefabPath;
}
