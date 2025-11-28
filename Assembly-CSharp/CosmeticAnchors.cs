using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000477 RID: 1143
public class CosmeticAnchors : MonoBehaviour, ISpawnable
{
	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001D0B RID: 7435 RVA: 0x000996A4 File Offset: 0x000978A4
	// (set) Token: 0x06001D0C RID: 7436 RVA: 0x000996AC File Offset: 0x000978AC
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001D0D RID: 7437 RVA: 0x000996B5 File Offset: 0x000978B5
	// (set) Token: 0x06001D0E RID: 7438 RVA: 0x000996BD File Offset: 0x000978BD
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001D0F RID: 7439 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnSpawn(VRRig rig)
	{
	}

	// Token: 0x06001D10 RID: 7440 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001D11 RID: 7441 RVA: 0x000996C8 File Offset: 0x000978C8
	private void AssignAnchorToPath(ref GameObject anchorGObjRef, string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		Transform transform;
		if (!base.transform.TryFindByPath(path, out transform, false))
		{
			this.vrRig = base.GetComponentInParent<VRRig>(true);
			if (this.vrRig && this.vrRig.isOfflineVRRig)
			{
				Debug.LogError("CosmeticAnchors: Could not find path: \"" + path + "\".\nPath to this component: " + base.transform.GetPathQ(), this);
			}
			return;
		}
		anchorGObjRef = transform.gameObject;
	}

	// Token: 0x06001D12 RID: 7442 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x06001D13 RID: 7443 RVA: 0x00002789 File Offset: 0x00000989
	private void OnDisable()
	{
	}

	// Token: 0x06001D14 RID: 7444 RVA: 0x00002789 File Offset: 0x00000989
	public void TryUpdate()
	{
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x00002789 File Offset: 0x00000989
	public void EnableAnchor(bool enable)
	{
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x00099740 File Offset: 0x00097940
	private void SetHuntComputerAnchor(bool enable)
	{
		Transform huntComputer = this.anchorOverrides.HuntComputer;
		if (!GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf || !enable)
		{
			huntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
		}
		else
		{
			huntComputer.parent = this.huntComputerAnchor.transform;
		}
		huntComputer.transform.localPosition = Vector3.zero;
		huntComputer.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x000997B8 File Offset: 0x000979B8
	private void SetBuilderWatchAnchor(bool enable)
	{
		Transform builderWatch = this.anchorOverrides.BuilderWatch;
		if (!GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf || !enable)
		{
			builderWatch.parent = this.anchorOverrides.BuilderWatchAnchor;
		}
		else
		{
			builderWatch.parent = this.builderWatchAnchor.transform;
		}
		builderWatch.transform.localPosition = Vector3.zero;
		builderWatch.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x00099830 File Offset: 0x00097A30
	private void SetCustomAnchor(Transform target, bool enable, GameObject overrideAnchor, Transform defaultAnchor)
	{
		Transform transform = (enable && overrideAnchor != null) ? overrideAnchor.transform : defaultAnchor;
		if (target != null && target.parent != transform)
		{
			target.parent = transform;
			target.transform.localPosition = Vector3.zero;
			target.transform.localRotation = Quaternion.identity;
			target.transform.localScale = Vector3.one;
		}
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x000998A4 File Offset: 0x00097AA4
	public Transform GetPositionAnchor(TransferrableObject.PositionState pos)
	{
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					return null;
				}
				if (!this.chestAnchor)
				{
					return null;
				}
				return this.chestAnchor.transform;
			}
			else
			{
				if (!this.rightArmAnchor)
				{
					return null;
				}
				return this.rightArmAnchor.transform;
			}
		}
		else
		{
			if (!this.leftArmAnchor)
			{
				return null;
			}
			return this.leftArmAnchor.transform;
		}
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x00099912 File Offset: 0x00097B12
	public Transform GetNameAnchor()
	{
		if (!this.nameAnchor)
		{
			return null;
		}
		return this.nameAnchor.transform;
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x0009992E File Offset: 0x00097B2E
	public bool AffectedByHunt()
	{
		return this.huntComputerAnchor != null;
	}

	// Token: 0x06001D1C RID: 7452 RVA: 0x0009993C File Offset: 0x00097B3C
	public bool AffectedByBuilder()
	{
		return this.builderWatchAnchor != null;
	}

	// Token: 0x040026F5 RID: 9973
	[SerializeField]
	private bool deprecatedWarning = true;

	// Token: 0x040026F6 RID: 9974
	[SerializeField]
	protected GameObject nameAnchor;

	// Token: 0x040026F7 RID: 9975
	[SerializeField]
	protected string nameAnchor_path;

	// Token: 0x040026F8 RID: 9976
	[SerializeField]
	protected GameObject leftArmAnchor;

	// Token: 0x040026F9 RID: 9977
	[SerializeField]
	protected string leftArmAnchor_path;

	// Token: 0x040026FA RID: 9978
	[SerializeField]
	protected GameObject rightArmAnchor;

	// Token: 0x040026FB RID: 9979
	[SerializeField]
	protected string rightArmAnchor_path;

	// Token: 0x040026FC RID: 9980
	[SerializeField]
	protected GameObject chestAnchor;

	// Token: 0x040026FD RID: 9981
	[SerializeField]
	protected string chestAnchor_path;

	// Token: 0x040026FE RID: 9982
	[SerializeField]
	protected GameObject huntComputerAnchor;

	// Token: 0x040026FF RID: 9983
	[SerializeField]
	protected string huntComputerAnchor_path;

	// Token: 0x04002700 RID: 9984
	[SerializeField]
	protected GameObject builderWatchAnchor;

	// Token: 0x04002701 RID: 9985
	[SerializeField]
	protected string builderWatchAnchor_path;

	// Token: 0x04002702 RID: 9986
	[SerializeField]
	protected GameObject friendshipBraceletLeftOverride;

	// Token: 0x04002703 RID: 9987
	[SerializeField]
	protected string friendshipBraceletLeftOverride_path;

	// Token: 0x04002704 RID: 9988
	[SerializeField]
	protected GameObject friendshipBraceletRightOverride;

	// Token: 0x04002705 RID: 9989
	[SerializeField]
	protected string friendshipBraceletRightOverride_path;

	// Token: 0x04002706 RID: 9990
	[SerializeField]
	protected GameObject badgeAnchor;

	// Token: 0x04002707 RID: 9991
	[SerializeField]
	protected string badgeAnchor_path;

	// Token: 0x04002708 RID: 9992
	[SerializeField]
	public CosmeticsController.CosmeticSlots slot;

	// Token: 0x04002709 RID: 9993
	private VRRig vrRig;

	// Token: 0x0400270A RID: 9994
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x0400270B RID: 9995
	private bool anchorEnabled;

	// Token: 0x0400270C RID: 9996
	private static GTLogErrorLimiter k_debugLogError_anchorOverridesNull = new GTLogErrorLimiter("The array `anchorOverrides` was null. Is the cosmetic getting initialized properly? ", 10, "\n- ");
}
