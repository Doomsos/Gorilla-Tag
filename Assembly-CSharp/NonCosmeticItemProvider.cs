using System;
using UnityEngine;

// Token: 0x020003FA RID: 1018
public class NonCosmeticItemProvider : MonoBehaviour
{
	// Token: 0x060018F1 RID: 6385 RVA: 0x000858EC File Offset: 0x00083AEC
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component != null)
		{
			GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", 0, new object[]
			{
				true,
				component.isLeftHand
			});
		}
	}

	// Token: 0x04002247 RID: 8775
	public GTZone zone;

	// Token: 0x04002248 RID: 8776
	[Tooltip("only for honeycomb")]
	public bool useCondition;

	// Token: 0x04002249 RID: 8777
	public int conditionThreshold;

	// Token: 0x0400224A RID: 8778
	public NonCosmeticItemProvider.ItemType itemType;

	// Token: 0x020003FB RID: 1019
	public enum ItemType
	{
		// Token: 0x0400224C RID: 8780
		honeycomb
	}
}
