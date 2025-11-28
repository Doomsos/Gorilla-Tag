using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000619 RID: 1561
public class GameGrabbable : MonoBehaviour
{
	// Token: 0x060027CD RID: 10189 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x000D3E9C File Offset: 0x000D209C
	public bool GetBestGrabPoint(Vector3 handPos, Quaternion handRot, int handIndex, out GameGrab grab)
	{
		float num = 0.15f;
		bool flag = false;
		grab = default(GameGrab);
		grab.position = base.transform.position;
		grab.rotation = base.transform.rotation;
		bool flag2 = GamePlayer.IsLeftHand(handIndex);
		if (this.snapGrabPoints != null)
		{
			for (int i = 0; i < this.snapGrabPoints.Count; i++)
			{
				GameGrabbable.SnapGrabPoints snapGrabPoints = this.snapGrabPoints[i];
				if (snapGrabPoints.isLeftHand == flag2 && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_UP, handRot * GameGrabbable.GRAB_UP) >= 0f && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_PALM, handRot * GameGrabbable.GRAB_PALM) >= 0f && (double)(handPos - snapGrabPoints.handTransform.position).sqrMagnitude <= 0.0225)
				{
					grab.position = handPos + handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation) * -snapGrabPoints.handTransform.localPosition;
					grab.rotation = handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return false;
		}
		Vector3 vector = grab.position - handPos;
		if (vector.sqrMagnitude > num * num)
		{
			grab.position = handPos + vector.normalized * num;
		}
		return true;
	}

	// Token: 0x0400333D RID: 13117
	public GameEntity gameEntity;

	// Token: 0x0400333E RID: 13118
	public List<GameGrabbable.SnapGrabPoints> snapGrabPoints;

	// Token: 0x0400333F RID: 13119
	private static readonly Vector3 GRAB_UP = new Vector3(0f, 0f, 1f);

	// Token: 0x04003340 RID: 13120
	private static readonly Vector3 GRAB_PALM = new Vector3(1f, 0f, 0f);

	// Token: 0x0200061A RID: 1562
	[Serializable]
	public class SnapGrabPoints
	{
		// Token: 0x04003341 RID: 13121
		public bool isLeftHand;

		// Token: 0x04003342 RID: 13122
		public Transform handTransform;
	}
}
