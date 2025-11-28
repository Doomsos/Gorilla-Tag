using System;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x02000435 RID: 1077
public class WorldTargetItem
{
	// Token: 0x06001A64 RID: 6756 RVA: 0x0008C6C7 File Offset: 0x0008A8C7
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x0008C6E0 File Offset: 0x0008A8E0
	[CanBeNull]
	public static WorldTargetItem GenerateTargetFromPlayerAndID(NetPlayer owner, int itemIdx)
	{
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(owner);
		if (vrrig == null)
		{
			Debug.LogError("Tried to setup a sharable object but the target rig is null...");
			return null;
		}
		Transform component = vrrig.myBodyDockPositions.TransferrableItem(itemIdx).gameObject.GetComponent<Transform>();
		return new WorldTargetItem(owner, itemIdx, component);
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x0008C728 File Offset: 0x0008A928
	public static WorldTargetItem GenerateTargetFromWorldSharableItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		return new WorldTargetItem(owner, itemIdx, transform);
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x0008C732 File Offset: 0x0008A932
	private WorldTargetItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		this.owner = owner;
		this.itemIdx = itemIdx;
		this.targetObject = transform;
		this.transferrableObject = transform.GetComponent<TransferrableObject>();
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x0008C75B File Offset: 0x0008A95B
	public override string ToString()
	{
		return string.Format("Id: {0} ({1})", this.itemIdx, this.owner);
	}

	// Token: 0x040023F7 RID: 9207
	public readonly NetPlayer owner;

	// Token: 0x040023F8 RID: 9208
	public readonly int itemIdx;

	// Token: 0x040023F9 RID: 9209
	public readonly Transform targetObject;

	// Token: 0x040023FA RID: 9210
	public readonly TransferrableObject transferrableObject;
}
