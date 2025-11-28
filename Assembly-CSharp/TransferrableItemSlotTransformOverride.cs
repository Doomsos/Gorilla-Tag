using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200049F RID: 1183
public class TransferrableItemSlotTransformOverride : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06001E47 RID: 7751 RVA: 0x0009FF27 File Offset: 0x0009E127
	// (set) Token: 0x06001E48 RID: 7752 RVA: 0x0009FF2F File Offset: 0x0009E12F
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06001E49 RID: 7753 RVA: 0x0009FF38 File Offset: 0x0009E138
	// (set) Token: 0x06001E4A RID: 7754 RVA: 0x0009FF40 File Offset: 0x0009E140
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001E4B RID: 7755 RVA: 0x0009FF4C File Offset: 0x0009E14C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.defaultPosition = new SlotTransformOverride
		{
			positionState = TransferrableObject.PositionState.None
		};
		this.lastPosition = TransferrableObject.PositionState.None;
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			slotTransformOverride.Initialize(this, this.anchor);
		}
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x0009FFBC File Offset: 0x0009E1BC
	public void AddGripPosition(TransferrableObject.PositionState state, TransferrableObjectGripPosition togp)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if (slotTransformOverride.positionState == state)
			{
				slotTransformOverride.AddSubGrabPoint(togp);
				return;
			}
		}
		SlotTransformOverride slotTransformOverride2 = new SlotTransformOverride
		{
			positionState = state
		};
		this.transformOverridesDeprecated.Add(slotTransformOverride2);
		slotTransformOverride2.AddSubGrabPoint(togp);
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001E4F RID: 7759 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001E50 RID: 7760 RVA: 0x000A003C File Offset: 0x0009E23C
	public void SliceUpdate()
	{
		if (this.followingTransferrableObject == null)
		{
			return;
		}
		if (this.followingTransferrableObject.currentState != this.lastPosition)
		{
			SlotTransformOverride slotTransformOverride = this.transformOverridesDeprecated.Find((SlotTransformOverride x) => (x.positionState & this.followingTransferrableObject.currentState) > TransferrableObject.PositionState.None);
			if (slotTransformOverride != null && slotTransformOverride.positionState == TransferrableObject.PositionState.None)
			{
				slotTransformOverride = this.defaultPosition;
			}
		}
		this.lastPosition = this.followingTransferrableObject.currentState;
	}

	// Token: 0x06001E51 RID: 7761 RVA: 0x000A00A6 File Offset: 0x0009E2A6
	private void Awake()
	{
		this.GenerateTransformFromPositionState();
	}

	// Token: 0x06001E52 RID: 7762 RVA: 0x000A00B0 File Offset: 0x0009E2B0
	public void GenerateTransformFromPositionState()
	{
		this.transformFromPosition = new Dictionary<TransferrableObject.PositionState, Transform>();
		if (this.transformOverridesDeprecated.Count > 0)
		{
			this.transformFromPosition[TransferrableObject.PositionState.None] = this.transformOverridesDeprecated[0].overrideTransform;
		}
		foreach (TransferrableObject.PositionState positionState in Enumerable.Cast<TransferrableObject.PositionState>(Enum.GetValues(typeof(TransferrableObject.PositionState))))
		{
			if (positionState == TransferrableObject.PositionState.None)
			{
				this.transformFromPosition[positionState] = null;
			}
			else
			{
				Transform transform = null;
				foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
				{
					if ((slotTransformOverride.positionState & positionState) != TransferrableObject.PositionState.None)
					{
						transform = slotTransformOverride.overrideTransform;
						break;
					}
				}
				this.transformFromPosition[positionState] = transform;
			}
		}
	}

	// Token: 0x06001E53 RID: 7763 RVA: 0x000A01AC File Offset: 0x0009E3AC
	[CanBeNull]
	public Transform GetTransformFromPositionState(TransferrableObject.PositionState currentState)
	{
		if (this.transformFromPosition == null)
		{
			this.GenerateTransformFromPositionState();
		}
		return this.transformFromPosition[currentState];
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x000A01C8 File Offset: 0x0009E3C8
	public bool GetTransformFromPositionState(TransferrableObject.PositionState currentState, AdvancedItemState advancedItemState, Transform targetDockXf, out Matrix4x4 matrix4X4)
	{
		if (currentState != TransferrableObject.PositionState.None)
		{
			foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
			{
				if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None)
				{
					if (!slotTransformOverride.useAdvancedGrab)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					if (advancedItemState.index >= slotTransformOverride.multiPoints.Count)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[advancedItemState.index];
					matrix4X4 = subGrabPoint.GetTransformFromPositionState(advancedItemState, slotTransformOverride, targetDockXf);
					return true;
				}
			}
			matrix4X4 = Matrix4x4.identity;
			return false;
		}
		if (this.transformOverridesDeprecated.Count > 0)
		{
			matrix4X4 = this.transformOverridesDeprecated[0].overrideTransformMatrix;
			return true;
		}
		matrix4X4 = Matrix4x4.identity;
		return false;
	}

	// Token: 0x06001E55 RID: 7765 RVA: 0x000A02CC File Offset: 0x0009E4CC
	public AdvancedItemState GetAdvancedItemStateFromHand(TransferrableObject.PositionState currentState, Transform handTransform, Transform targetDock)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None && slotTransformOverride.multiPoints.Count != 0)
			{
				SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[0];
				float num = float.PositiveInfinity;
				int index = -1;
				for (int i = 0; i < slotTransformOverride.multiPoints.Count; i++)
				{
					SubGrabPoint subGrabPoint2 = slotTransformOverride.multiPoints[i];
					if (!(subGrabPoint2.gripPoint == null))
					{
						float num2 = subGrabPoint2.EvaluateScore(base.transform, handTransform, targetDock);
						if (num2 < num)
						{
							subGrabPoint = subGrabPoint2;
							num = num2;
							index = i;
						}
					}
				}
				AdvancedItemState advancedItemStateFromHand = subGrabPoint.GetAdvancedItemStateFromHand(base.transform, handTransform, targetDock, slotTransformOverride);
				advancedItemStateFromHand.index = index;
				return advancedItemStateFromHand;
			}
		}
		return new AdvancedItemState();
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x000A03CC File Offset: 0x0009E5CC
	public void Edit()
	{
		if (TransferrableItemSlotTransformOverride.OnBringUpWindow != null)
		{
			TransferrableItemSlotTransformOverride.OnBringUpWindow.Invoke(base.GetComponent<TransferrableObject>());
		}
	}

	// Token: 0x04002882 RID: 10370
	[FormerlySerializedAs("transformOverridesList")]
	public List<SlotTransformOverride> transformOverridesDeprecated;

	// Token: 0x04002883 RID: 10371
	[SerializeReference]
	public List<SlotTransformOverride> transformOverrides;

	// Token: 0x04002884 RID: 10372
	private TransferrableObject.PositionState lastPosition;

	// Token: 0x04002885 RID: 10373
	[Tooltip("(2024-08-20 MattO) For cosmetics this is almost always assigned to the TransferrableObject component in the same prefab and almost always belonging to the same gameobject as this Component.")]
	public TransferrableObject followingTransferrableObject;

	// Token: 0x04002886 RID: 10374
	[Tooltip("(2024-08-20 MattO) This is filled in automatically by the cosmetic spawner.")]
	public SlotTransformOverride defaultPosition;

	// Token: 0x04002887 RID: 10375
	[Obsolete("(2024-08-2024) This used to be assigned to `defaultPosition.overrideTransform` before, but was there ever an instance where it wasn't null? Keeping it serialized just in case there is a reason for it.")]
	public Transform defaultTransform;

	// Token: 0x04002888 RID: 10376
	public Transform anchor;

	// Token: 0x0400288B RID: 10379
	public Dictionary<TransferrableObject.PositionState, Transform> transformFromPosition;

	// Token: 0x0400288C RID: 10380
	public static Action<TransferrableObject> OnBringUpWindow;
}
