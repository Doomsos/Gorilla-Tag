using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004CC RID: 1228
[RequireComponent(typeof(SphereCollider))]
public class CosmeticWardrobeProximityDetector : MonoBehaviour
{
	// Token: 0x06001FB3 RID: 8115 RVA: 0x000A8D14 File Offset: 0x000A6F14
	private void OnEnable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Add(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x000A8D34 File Offset: 0x000A6F34
	private void OnDisable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Remove(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x000A8D58 File Offset: 0x000A6F58
	public static bool IsUserNearWardrobe(string userID)
	{
		int num = LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		}) | LayerMask.GetMask(new string[]
		{
			"Gorilla Body Collider"
		});
		foreach (SphereCollider sphereCollider in CosmeticWardrobeProximityDetector.wardrobeNearbyDetection)
		{
			int num2 = Physics.OverlapSphereNonAlloc(sphereCollider.transform.position, sphereCollider.radius, CosmeticWardrobeProximityDetector.overlapColliders, num);
			num2 = Mathf.Min(num2, CosmeticWardrobeProximityDetector.overlapColliders.Length);
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					Collider collider = CosmeticWardrobeProximityDetector.overlapColliders[i];
					if (!(collider == null))
					{
						GameObject gameObject = collider.attachedRigidbody.gameObject;
						VRRig component = gameObject.GetComponent<VRRig>();
						if (component == null || component.creator == null || component.creator.IsNull || string.IsNullOrEmpty(component.creator.UserId))
						{
							if (gameObject.GetComponent<GTPlayer>() == null || NetworkSystem.Instance.LocalPlayer == null)
							{
								goto IL_135;
							}
							if (userID == NetworkSystem.Instance.LocalPlayer.UserId)
							{
								return true;
							}
						}
						else if (userID == component.creator.UserId)
						{
							return true;
						}
						CosmeticWardrobeProximityDetector.overlapColliders[i] = null;
					}
					IL_135:;
				}
			}
		}
		return false;
	}

	// Token: 0x04002A0D RID: 10765
	[SerializeField]
	private SphereCollider wardrobeNearbyCollider;

	// Token: 0x04002A0E RID: 10766
	private static List<SphereCollider> wardrobeNearbyDetection = new List<SphereCollider>();

	// Token: 0x04002A0F RID: 10767
	private static readonly Collider[] overlapColliders = new Collider[20];
}
