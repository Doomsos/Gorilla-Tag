using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000414 RID: 1044
public class InteractionPoint : MonoBehaviour, ISpawnable, IBuildValidation
{
	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x060019B3 RID: 6579 RVA: 0x0008982F File Offset: 0x00087A2F
	// (set) Token: 0x060019B4 RID: 6580 RVA: 0x00089837 File Offset: 0x00087A37
	public bool ignoreLeftHand { get; private set; }

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x060019B5 RID: 6581 RVA: 0x00089840 File Offset: 0x00087A40
	// (set) Token: 0x060019B6 RID: 6582 RVA: 0x00089848 File Offset: 0x00087A48
	public bool ignoreRightHand { get; private set; }

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x060019B7 RID: 6583 RVA: 0x00089851 File Offset: 0x00087A51
	public IHoldableObject Holdable
	{
		get
		{
			return this.parentHoldable;
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x060019B8 RID: 6584 RVA: 0x00089859 File Offset: 0x00087A59
	// (set) Token: 0x060019B9 RID: 6585 RVA: 0x00089861 File Offset: 0x00087A61
	public bool IsSpawned { get; set; }

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x060019BA RID: 6586 RVA: 0x0008986A File Offset: 0x00087A6A
	// (set) Token: 0x060019BB RID: 6587 RVA: 0x00089872 File Offset: 0x00087A72
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x060019BC RID: 6588 RVA: 0x0008987C File Offset: 0x00087A7C
	public void OnSpawn(VRRig rig)
	{
		this.interactor = EquipmentInteractor.instance;
		this.myCollider = base.GetComponent<Collider>();
		if (this.parentHoldableObject != null)
		{
			this.parentHoldable = this.parentHoldableObject.GetComponent<IHoldableObject>();
		}
		else
		{
			this.parentHoldable = base.GetComponentInParent<IHoldableObject>(true);
			this.parentHoldableObject = this.parentHoldable.gameObject;
		}
		if (this.parentHoldable == null)
		{
			if (this.parentHoldableObject == null)
			{
				Debug.LogError("InteractionPoint: Disabling because expected field `parentHoldableObject` is null. Path=" + base.transform.GetPathQ());
				base.enabled = false;
				return;
			}
			Debug.LogError("InteractionPoint: Disabling because `parentHoldableObject` does not have a IHoldableObject component. Path=" + base.transform.GetPathQ());
		}
		TransferrableObject transferrableObject = this.parentHoldable as TransferrableObject;
		this.forLocalPlayer = (transferrableObject == null || transferrableObject.IsLocalObject() || transferrableObject.isSceneObject || transferrableObject.canDrop);
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x00089967 File Offset: 0x00087B67
	private void Awake()
	{
		if (this.isNonSpawnedObject)
		{
			this.OnSpawn(null);
		}
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x00089978 File Offset: 0x00087B78
	private void OnEnable()
	{
		this.wasInLeft = false;
		this.wasInRight = false;
	}

	// Token: 0x060019C0 RID: 6592 RVA: 0x00089988 File Offset: 0x00087B88
	public void OnDisable()
	{
		if (!this.forLocalPlayer || this.interactor == null)
		{
			return;
		}
		this.interactor.InteractionPointDisabled(this);
	}

	// Token: 0x060019C1 RID: 6593 RVA: 0x000899B0 File Offset: 0x00087BB0
	protected void LateUpdate()
	{
		if (!this.forLocalPlayer)
		{
			base.enabled = false;
			this.myCollider.enabled = false;
			return;
		}
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
			return;
		}
		if (this.interactionRadius > 0f || this.myCollider != null)
		{
			if (!this.ignoreLeftHand && this.OverlapCheck(this.interactor.leftHand.transform.position) != this.wasInLeft)
			{
				if (!this.wasInLeft && !this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Add(this);
					this.wasInLeft = true;
				}
				else if (this.wasInLeft && this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Remove(this);
					this.wasInLeft = false;
				}
			}
			if (!this.ignoreRightHand && this.OverlapCheck(this.interactor.rightHand.transform.position) != this.wasInRight)
			{
				if (!this.wasInRight && !this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Add(this);
					this.wasInRight = true;
					return;
				}
				if (this.wasInRight && this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Remove(this);
					this.wasInRight = false;
				}
			}
		}
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x00089B40 File Offset: 0x00087D40
	private bool OverlapCheck(Vector3 point)
	{
		if (this.interactionRadius > 0f)
		{
			return (base.transform.position - point).IsShorterThan(this.interactionRadius * base.transform.lossyScale);
		}
		return this.myCollider != null && this.myCollider.bounds.Contains(point);
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x00027DED File Offset: 0x00025FED
	public bool BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x04002328 RID: 9000
	[SerializeField]
	[FormerlySerializedAs("parentTransferrableObject")]
	public GameObject parentHoldableObject;

	// Token: 0x04002329 RID: 9001
	private IHoldableObject parentHoldable;

	// Token: 0x0400232C RID: 9004
	[SerializeField]
	private bool isNonSpawnedObject;

	// Token: 0x0400232D RID: 9005
	[SerializeField]
	private float interactionRadius;

	// Token: 0x0400232E RID: 9006
	public Collider myCollider;

	// Token: 0x0400232F RID: 9007
	public EquipmentInteractor interactor;

	// Token: 0x04002330 RID: 9008
	public bool wasInLeft;

	// Token: 0x04002331 RID: 9009
	public bool wasInRight;

	// Token: 0x04002332 RID: 9010
	public bool forLocalPlayer;
}
