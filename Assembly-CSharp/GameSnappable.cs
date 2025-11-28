using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000632 RID: 1586
public class GameSnappable : MonoBehaviour
{
	// Token: 0x06002874 RID: 10356 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x06002875 RID: 10357 RVA: 0x000D7410 File Offset: 0x000D5610
	public void GetSnapOffset(SnapJointType jointType, out Vector3 positionOffset, out Quaternion rotationOffset)
	{
		foreach (GameSnappable.SnapJointOffset snapJointOffset in this.snapOffsets)
		{
			if ((snapJointOffset.jointType & jointType) != SnapJointType.None)
			{
				positionOffset = snapJointOffset.positionOffset;
				rotationOffset = Quaternion.Euler(snapJointOffset.rotationOffset);
				return;
			}
		}
		positionOffset = Vector3.zero;
		rotationOffset = Quaternion.identity;
	}

	// Token: 0x06002876 RID: 10358 RVA: 0x000D749C File Offset: 0x000D569C
	public SuperInfectionSnapPoint BestSnapPoint()
	{
		int heldByHandIndex = this.gameEntity.heldByHandIndex;
		if (heldByHandIndex < 0)
		{
			return null;
		}
		SnapJointType snapJointType = GamePlayerLocal.IsLeftHand(heldByHandIndex) ? SnapJointType.HandL : SnapJointType.HandR;
		SnapJointType snapJointType2 = GamePlayerLocal.IsLeftHand(heldByHandIndex) ? SnapJointType.ForearmL : SnapJointType.ForearmR;
		List<SuperInfectionSnapPoint> snapPoints = GamePlayerLocal.instance.gamePlayer.snapPointManager.SnapPoints;
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2 && (snapPoints[i].jointType & this.snapLocationTypes) != SnapJointType.None && !snapPoints[i].HasSnapped())
			{
				Vector3 vector;
				Quaternion quaternion;
				this.GetSnapOffset(snapPoints[i].jointType, out vector, out quaternion);
				float num3 = Vector3.Distance(snapPoints[i].transform.TransformPoint(quaternion * vector), base.transform.position);
				float num4 = this.snapRadius + snapPoints[i].snapPointRadius;
				if (num3 < num && num3 < num4)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		if (num2 >= 0)
		{
			return snapPoints[num2];
		}
		if ((this.snapLocationTypes & SnapJointType.Holster) != SnapJointType.None)
		{
			GameEntityManager currGameEntityManager = GamePlayerLocal.instance.currGameEntityManager;
			IEnumerable<SuperInfectionSnapPoint> points = ((currGameEntityManager != null) ? currGameEntityManager.superInfectionManager : null).GetPoints(SnapJointType.Holster);
			SuperInfectionSnapPoint superInfectionSnapPoint = null;
			float num5 = this.snapRadius;
			foreach (SuperInfectionSnapPoint superInfectionSnapPoint2 in points)
			{
				if (!superInfectionSnapPoint2.HasSnapped())
				{
					Vector3 vector2;
					Quaternion quaternion2;
					this.GetSnapOffset(superInfectionSnapPoint2.jointType, out vector2, out quaternion2);
					float num6 = Vector3.Distance(superInfectionSnapPoint2.transform.TransformPoint(quaternion2 * vector2), base.transform.position);
					if (num6 < num5)
					{
						superInfectionSnapPoint = superInfectionSnapPoint2;
						num5 = num6;
					}
				}
			}
			if (superInfectionSnapPoint != null)
			{
				return superInfectionSnapPoint;
			}
		}
		return null;
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x000D76B0 File Offset: 0x000D58B0
	public GameEntityId BestSnapPointDock()
	{
		int heldByHandIndex = this.gameEntity.heldByHandIndex;
		if (heldByHandIndex < 0)
		{
			return GameEntityId.Invalid;
		}
		SnapJointType snapJointType = GamePlayerLocal.IsLeftHand(heldByHandIndex) ? SnapJointType.HandL : SnapJointType.HandR;
		SnapJointType snapJointType2 = GamePlayerLocal.IsLeftHand(heldByHandIndex) ? SnapJointType.ForearmL : SnapJointType.ForearmR;
		List<SuperInfectionSnapPoint> snapPoints = GamePlayerLocal.instance.gamePlayer.snapPointManager.SnapPoints;
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2 && (snapPoints[i].jointType & this.snapLocationTypes) != SnapJointType.None && snapPoints[i].HasSnapped())
			{
				Vector3 vector;
				Quaternion quaternion;
				this.GetSnapOffset(snapPoints[i].jointType, out vector, out quaternion);
				float num3 = Vector3.Distance(snapPoints[i].transform.TransformPoint(quaternion * vector), base.transform.position);
				float num4 = this.snapRadius + snapPoints[i].snapPointRadius;
				if (num3 < num && num3 < num4)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		if (num2 < 0)
		{
			return GameEntityId.Invalid;
		}
		return snapPoints[num2].GetSnappedEntity().id;
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x000D7808 File Offset: 0x000D5A08
	public bool CanGrabWithHand(bool leftHand)
	{
		if (this.snappedToJoint == null)
		{
			return true;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return (leftHand && jointType != SnapJointType.HandL && jointType != SnapJointType.ForearmL) || (!leftHand && jointType != SnapJointType.HandR && jointType != SnapJointType.ForearmR);
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x000D7856 File Offset: 0x000D5A56
	public void OnSnap()
	{
		this.snapSound.Play(null);
		this.snapHaptic.PlayIfSnappedLocal(this.gameEntity);
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x000D7878 File Offset: 0x000D5A78
	public bool IsSnappedToLeftArm()
	{
		if (this.snappedToJoint == null)
		{
			return false;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return jointType == SnapJointType.HandL || jointType == SnapJointType.ForearmL;
	}

	// Token: 0x0600287B RID: 10363 RVA: 0x000D78B0 File Offset: 0x000D5AB0
	public bool IsSnappedToRightArm()
	{
		if (this.snappedToJoint == null)
		{
			return false;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return jointType == SnapJointType.HandR || jointType == SnapJointType.ForearmR;
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x000D78E7 File Offset: 0x000D5AE7
	public void OnUnsnap()
	{
		this.unsnapSound.Play(null);
	}

	// Token: 0x040033E6 RID: 13286
	public GameEntity gameEntity;

	// Token: 0x040033E7 RID: 13287
	public float snapRadius = 0.15f;

	// Token: 0x040033E8 RID: 13288
	public SuperInfectionSnapPoint snappedToJoint;

	// Token: 0x040033E9 RID: 13289
	public AbilitySound snapSound;

	// Token: 0x040033EA RID: 13290
	public AbilitySound unsnapSound;

	// Token: 0x040033EB RID: 13291
	public AbilityHaptic snapHaptic;

	// Token: 0x040033EC RID: 13292
	public SnapJointType snapLocationTypes;

	// Token: 0x040033ED RID: 13293
	public List<GameSnappable.SnapJointOffset> snapOffsets;

	// Token: 0x02000633 RID: 1587
	[Serializable]
	public struct SnapJointOffset
	{
		// Token: 0x040033EE RID: 13294
		public SnapJointType jointType;

		// Token: 0x040033EF RID: 13295
		public Vector3 positionOffset;

		// Token: 0x040033F0 RID: 13296
		public Vector3 rotationOffset;
	}
}
