using System;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000636 RID: 1590
public class SuperInfectionSnapPoint : MonoBehaviour
{
	// Token: 0x06002887 RID: 10375 RVA: 0x000D7A64 File Offset: 0x000D5C64
	public void Initialize()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent == null)
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  Expected a VRRig to be in parent hierarchy. Path=\"" + base.transform.GetPathQ() + "\"");
		}
		Transform[] boneXforms;
		string text;
		if (!GTHardCodedBones.TryGetBoneXforms(componentInParent, out boneXforms, out text))
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  Could not get bone transforms: " + text);
		}
		if (this.overrideParentTransform != null)
		{
			this.parentTransform = this.overrideParentTransform;
		}
		else if (!GTHardCodedBones.TryGetBoneXform(boneXforms, this.parentBone.Bone, out this.parentTransform))
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  " + string.Format("Could not find bone Transform `{0}`.", this.parentBone));
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (this.parentTransform != null)
		{
			base.transform.SetParent(this.parentTransform, false);
		}
		base.transform.localPosition = localPosition;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x000D7B67 File Offset: 0x000D5D67
	public void Clear()
	{
		this.Unsnapped();
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x000D7B70 File Offset: 0x000D5D70
	public void Snapped(GameEntity entity)
	{
		this.snappedEntity = entity;
		GameSnappable gameSnappable;
		if (this.snappedEntity.TryGetComponent<GameSnappable>(ref gameSnappable))
		{
			gameSnappable.snappedToJoint = this;
			return;
		}
		Debug.LogError(string.Format("Snapped: entity {0} has no GameSnappable!?", this.snappedEntity));
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x000D7BB0 File Offset: 0x000D5DB0
	public void Unsnapped()
	{
		GameSnappable gameSnappable;
		if (this.snappedEntity.TryGetComponent<GameSnappable>(ref gameSnappable))
		{
			gameSnappable.snappedToJoint = null;
		}
		else
		{
			Debug.LogError(string.Format("Unsnapped: entity {0} has no GameSnappable!?", this.snappedEntity));
		}
		this.snappedEntity = null;
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000D7BF1 File Offset: 0x000D5DF1
	public bool HasSnapped()
	{
		return this.snappedEntity != null;
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x000D7BFF File Offset: 0x000D5DFF
	public GameEntity GetSnappedEntity()
	{
		return this.snappedEntity;
	}

	// Token: 0x040033FC RID: 13308
	private const string preLog = "[SuperInfectionSnapPoint]  ";

	// Token: 0x040033FD RID: 13309
	private const string preErr = "[SuperInfectionSnapPoint]  ERROR!!!  ";

	// Token: 0x040033FE RID: 13310
	public GamePlayer playerForPoint;

	// Token: 0x040033FF RID: 13311
	public SnapJointType jointType;

	// Token: 0x04003400 RID: 13312
	public GTHardCodedBones.SturdyEBone parentBone;

	// Token: 0x04003401 RID: 13313
	public Transform overrideParentTransform;

	// Token: 0x04003402 RID: 13314
	private Transform parentTransform;

	// Token: 0x04003403 RID: 13315
	public bool canSnapOverride;

	// Token: 0x04003404 RID: 13316
	public float snapPointRadius;

	// Token: 0x04003405 RID: 13317
	private GameEntity snappedEntity;
}
