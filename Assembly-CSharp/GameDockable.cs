using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000602 RID: 1538
public class GameDockable : MonoBehaviour
{
	// Token: 0x060026C8 RID: 9928 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x000CE264 File Offset: 0x000CC464
	public GameEntityId BestDock()
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
		GameDock gameDock = null;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2)
			{
				GameEntity snappedEntity = snapPoints[i].GetSnappedEntity();
				if (!(snappedEntity == null))
				{
					GameDock component = snappedEntity.GetComponent<GameDock>();
					if (!(component == null) && component.CanDock(this))
					{
						Transform transform = component.dockMarker.transform;
						Vector3 zero = Vector3.zero;
						Quaternion identity = Quaternion.identity;
						float num2 = Vector3.Distance(transform.TransformPoint(identity * zero), base.transform.position);
						float num3 = this.dockableRadius + component.dockRadius;
						if (num2 < num && num2 < num3)
						{
							num = num2;
							gameDock = component;
						}
					}
				}
			}
		}
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		for (int j = 0; j < 2; j++)
		{
			GameEntity grabbedGameEntity = gamePlayer.GetGrabbedGameEntity(j);
			if (!(grabbedGameEntity == null))
			{
				GameDock component2 = grabbedGameEntity.GetComponent<GameDock>();
				if (!(component2 == null) && component2.CanDock(this))
				{
					Transform transform2 = component2.dockMarker.transform;
					Vector3 zero2 = Vector3.zero;
					Quaternion identity2 = Quaternion.identity;
					float num2 = Vector3.Distance(transform2.TransformPoint(identity2 * zero2), base.transform.position);
					float num4 = this.dockableRadius + component2.dockRadius;
					if (num2 < num && num2 < num4)
					{
						num = num2;
						gameDock = component2;
					}
				}
			}
		}
		if (gameDock == null)
		{
			return GameEntityId.Invalid;
		}
		return gameDock.gameEntity.id;
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x000CE46F File Offset: 0x000CC66F
	public Transform GetDockablePoint()
	{
		if (!(this.dockablePoint == null))
		{
			return this.dockablePoint;
		}
		return base.transform;
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x00002789 File Offset: 0x00000989
	public void OnUndock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
	}

	// Token: 0x0400329B RID: 12955
	public GameEntity gameEntity;

	// Token: 0x0400329C RID: 12956
	public float dockableRadius = 0.15f;

	// Token: 0x0400329D RID: 12957
	public Transform dockablePoint;
}
