using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000637 RID: 1591
public class SuperInfectionSnapPointManager : MonoBehaviour
{
	// Token: 0x0600288E RID: 10382 RVA: 0x000D7C08 File Offset: 0x000D5E08
	public void Awake()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		ISpawnable[] componentsInChildren = base.GetComponentsInChildren<ISpawnable>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].OnSpawn(componentInParent);
		}
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x000D7C3C File Offset: 0x000D5E3C
	public void Start()
	{
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.SnapPoints)
		{
			superInfectionSnapPoint.Initialize();
			this.snapPointDict[superInfectionSnapPoint.jointType] = superInfectionSnapPoint;
		}
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x000D7CA0 File Offset: 0x000D5EA0
	public void Clear()
	{
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.SnapPoints)
		{
			superInfectionSnapPoint.Clear();
		}
		this.snapPointDict.Clear();
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000D7CFC File Offset: 0x000D5EFC
	public SuperInfectionSnapPoint FindSnapPoint(SnapJointType jointType)
	{
		if (jointType == SnapJointType.None)
		{
			return null;
		}
		if (this.snapPointDict.ContainsKey(jointType))
		{
			return this.snapPointDict[jointType];
		}
		return null;
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x000D7D1F File Offset: 0x000D5F1F
	public static SuperInfectionSnapPoint FindSnapPoint(GamePlayer player, SnapJointType jointType)
	{
		if (player == null)
		{
			return null;
		}
		return player.snapPointManager.FindSnapPoint(jointType);
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x000D7D38 File Offset: 0x000D5F38
	public void DropAllSnappedAuthority()
	{
		for (int i = 0; i < this.SnapPoints.Count; i++)
		{
			GameEntity snappedEntity = this.SnapPoints[i].GetSnappedEntity();
			if (!(snappedEntity == null))
			{
				Vector3 position = snappedEntity.transform.position;
				snappedEntity.manager.RequestGrabEntity(snappedEntity.id, false, Vector3.zero, Quaternion.identity);
				snappedEntity.manager.RequestThrowEntity(snappedEntity.id, false, position, Vector3.zero, Vector3.zero);
			}
		}
	}

	// Token: 0x04003406 RID: 13318
	public List<SuperInfectionSnapPoint> SnapPoints;

	// Token: 0x04003407 RID: 13319
	private Dictionary<SnapJointType, SuperInfectionSnapPoint> snapPointDict = new Dictionary<SnapJointType, SuperInfectionSnapPoint>();
}
