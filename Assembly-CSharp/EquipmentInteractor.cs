using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200040E RID: 1038
public class EquipmentInteractor : MonoBehaviour
{
	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x06001973 RID: 6515 RVA: 0x000885CB File Offset: 0x000867CB
	public GorillaHandClimber BodyClimber
	{
		get
		{
			return this.bodyClimber;
		}
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x06001974 RID: 6516 RVA: 0x000885D3 File Offset: 0x000867D3
	public GorillaHandClimber LeftClimber
	{
		get
		{
			return this.leftClimber;
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06001975 RID: 6517 RVA: 0x000885DB File Offset: 0x000867DB
	public GorillaHandClimber RightClimber
	{
		get
		{
			return this.rightClimber;
		}
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000885E4 File Offset: 0x000867E4
	private void Awake()
	{
		if (EquipmentInteractor.instance == null)
		{
			EquipmentInteractor.instance = this;
			EquipmentInteractor.hasInstance = true;
		}
		else if (EquipmentInteractor.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.autoGrabLeft = true;
		this.autoGrabRight = true;
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x00088638 File Offset: 0x00086838
	private void OnDestroy()
	{
		if (EquipmentInteractor.instance == this)
		{
			EquipmentInteractor.hasInstance = false;
			EquipmentInteractor.instance = null;
		}
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x00088657 File Offset: 0x00086857
	public void ReleaseRightHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		this.autoGrabRight = true;
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x00088696 File Offset: 0x00086896
	public void ReleaseLeftHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		this.autoGrabLeft = true;
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x000886D5 File Offset: 0x000868D5
	public void ForceStopClimbing()
	{
		this.bodyClimber.ForceStopClimbing(false, false);
		this.leftClimber.ForceStopClimbing(false, false);
		this.rightClimber.ForceStopClimbing(false, false);
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x000886FE File Offset: 0x000868FE
	public bool GetIsHolding(XRNode node)
	{
		if (node == 4)
		{
			return this.leftHandHeldEquipment != null;
		}
		return this.rightHandHeldEquipment != null;
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x00088717 File Offset: 0x00086917
	public bool IsGrabDisabled(XRNode node)
	{
		if (node == 4)
		{
			return this.disableLeftGrab;
		}
		return this.disableRightGrab;
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x0008872C File Offset: 0x0008692C
	public void InteractionPointDisabled(InteractionPoint interactionPoint)
	{
		if (this.iteratingInteractionPoints)
		{
			this.interactionPointsToRemove.Add(interactionPoint);
			return;
		}
		if (this.overlapInteractionPointsLeft != null)
		{
			this.overlapInteractionPointsLeft.Remove(interactionPoint);
		}
		if (this.overlapInteractionPointsRight != null)
		{
			this.overlapInteractionPointsRight.Remove(interactionPoint);
		}
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x00088778 File Offset: 0x00086978
	public bool CanGrabLeft()
	{
		return !this.disableLeftGrab && this.leftHandHeldEquipment == null && this.builderPieceInteractor.heldPiece[0] == null;
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x000887A3 File Offset: 0x000869A3
	public bool CanGrabRight()
	{
		return !this.disableRightGrab && this.rightHandHeldEquipment == null && this.builderPieceInteractor.heldPiece[1] == null;
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x000887D0 File Offset: 0x000869D0
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.leftClimber.CheckHandClimber();
		this.rightClimber.CheckHandClimber();
		this.CheckInputValue(true);
		this.isLeftGrabbing = ((this.wasLeftGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasLeftGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis));
		if (this.leftClimber && this.leftClimber.isClimbingOrGrabbing)
		{
			this.isLeftGrabbing = false;
		}
		this.CheckInputValue(false);
		this.isRightGrabbing = ((this.wasRightGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasRightGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis));
		if (this.rightClimber && this.rightClimber.isClimbingOrGrabbing)
		{
			this.isRightGrabbing = false;
		}
		BuilderPiece pieceInHand = this.builderPieceInteractor.heldPiece[0];
		BuilderPiece pieceInHand2 = this.builderPieceInteractor.heldPiece[1];
		this.FireHandInteractions(this.leftHand, true, pieceInHand);
		this.FireHandInteractions(this.rightHand, false, pieceInHand2);
		if (!this.isRightGrabbing && this.wasRightGrabPressed)
		{
			this.ReleaseRightHand();
		}
		if (!this.isLeftGrabbing && this.wasLeftGrabPressed)
		{
			this.ReleaseLeftHand();
		}
		this.builderPieceInteractor.OnLateUpdate();
		if (GameBallPlayerLocal.instance != null)
		{
			GameBallPlayerLocal.instance.OnUpdateInteract();
		}
		if (GamePlayerLocal.instance != null)
		{
			GamePlayerLocal.instance.OnUpdateInteract();
		}
		this.wasLeftGrabPressed = this.isLeftGrabbing;
		this.wasRightGrabPressed = this.isRightGrabbing;
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x000889A0 File Offset: 0x00086BA0
	private void FireHandInteractions(GameObject interactingHand, bool isLeftHand, BuilderPiece pieceInHand)
	{
		if (isLeftHand)
		{
			this.justGrabbed = ((this.isLeftGrabbing && !this.wasLeftGrabPressed) || (this.isLeftGrabbing && this.autoGrabLeft));
			this.justReleased = (this.leftHandHeldEquipment != null && !this.isLeftGrabbing && this.wasLeftGrabPressed);
		}
		else
		{
			this.justGrabbed = ((this.isRightGrabbing && !this.wasRightGrabPressed) || (this.isRightGrabbing && this.autoGrabRight));
			this.justReleased = (this.rightHandHeldEquipment != null && !this.isRightGrabbing && this.wasRightGrabPressed);
		}
		List<InteractionPoint> list = isLeftHand ? this.overlapInteractionPointsLeft : this.overlapInteractionPointsRight;
		bool flag = isLeftHand ? (this.leftHandHeldEquipment != null) : (this.rightHandHeldEquipment != null);
		bool flag2 = pieceInHand != null;
		bool flag3 = isLeftHand ? this.disableLeftGrab : this.disableRightGrab;
		bool flag4 = !flag && !flag2 && !flag3;
		this.iteratingInteractionPoints = true;
		foreach (InteractionPoint interactionPoint in list)
		{
			if (flag4 && interactionPoint != null)
			{
				if (this.justGrabbed)
				{
					interactionPoint.Holdable.OnGrab(interactionPoint, interactingHand);
				}
				else
				{
					interactionPoint.Holdable.OnHover(interactionPoint, interactingHand);
				}
			}
			if (this.justReleased)
			{
				this.tempZone = interactionPoint.GetComponent<DropZone>();
				if (this.tempZone != null)
				{
					if (interactingHand == this.leftHand)
					{
						if (this.leftHandHeldEquipment != null)
						{
							this.leftHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
						}
					}
					else if (this.rightHandHeldEquipment != null)
					{
						this.rightHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
					}
				}
			}
		}
		this.iteratingInteractionPoints = false;
		foreach (InteractionPoint interactionPoint2 in this.interactionPointsToRemove)
		{
			if (this.overlapInteractionPointsLeft != null)
			{
				this.overlapInteractionPointsLeft.Remove(interactionPoint2);
			}
			if (this.overlapInteractionPointsRight != null)
			{
				this.overlapInteractionPointsRight.Remove(interactionPoint2);
			}
		}
		this.interactionPointsToRemove.Clear();
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x00088BF8 File Offset: 0x00086DF8
	public void UpdateHandEquipment(IHoldableObject newEquipment, bool forLeftHand)
	{
		if (forLeftHand)
		{
			if (newEquipment != null && newEquipment == this.rightHandHeldEquipment && !newEquipment.TwoHanded)
			{
				this.rightHandHeldEquipment = null;
			}
			if (this.leftHandHeldEquipment != null)
			{
				this.leftHandHeldEquipment.DropItemCleanup();
			}
			this.leftHandHeldEquipment = newEquipment;
			this.autoGrabLeft = false;
			return;
		}
		if (newEquipment != null && newEquipment == this.leftHandHeldEquipment && !newEquipment.TwoHanded)
		{
			this.leftHandHeldEquipment = null;
		}
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.DropItemCleanup();
		}
		this.rightHandHeldEquipment = newEquipment;
		this.autoGrabRight = false;
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x00088C84 File Offset: 0x00086E84
	public void CheckInputValue(bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.grabValue = ControllerInputPoller.GripFloat(4);
			this.tempValue = ControllerInputPoller.TriggerFloat(4);
		}
		else
		{
			this.grabValue = ControllerInputPoller.GripFloat(5);
			this.tempValue = ControllerInputPoller.TriggerFloat(5);
		}
		this.grabValue = Mathf.Max(this.grabValue, this.tempValue);
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x00088CDD File Offset: 0x00086EDD
	public void ForceDropEquipment(IHoldableObject equipment)
	{
		if (this.rightHandHeldEquipment == equipment)
		{
			this.rightHandHeldEquipment = null;
		}
		if (this.leftHandHeldEquipment == equipment)
		{
			this.leftHandHeldEquipment = null;
		}
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x00088CFF File Offset: 0x00086EFF
	public void ForceDropAnyEquipment()
	{
		this.rightHandHeldEquipment = null;
		this.leftHandHeldEquipment = null;
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x00088D10 File Offset: 0x00086F10
	public void ForceDropManipulatableObject(HoldableObject manipulatableObject)
	{
		if ((HoldableObject)this.rightHandHeldEquipment == manipulatableObject)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
			this.rightHandHeldEquipment = null;
			this.autoGrabRight = false;
		}
		if ((HoldableObject)this.leftHandHeldEquipment == manipulatableObject)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
			this.leftHandHeldEquipment = null;
			this.autoGrabLeft = false;
		}
	}

	// Token: 0x040022DE RID: 8926
	[OnEnterPlay_SetNull]
	public static volatile EquipmentInteractor instance;

	// Token: 0x040022DF RID: 8927
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x040022E0 RID: 8928
	public IHoldableObject leftHandHeldEquipment;

	// Token: 0x040022E1 RID: 8929
	public IHoldableObject rightHandHeldEquipment;

	// Token: 0x040022E2 RID: 8930
	public BuilderPieceInteractor builderPieceInteractor;

	// Token: 0x040022E3 RID: 8931
	public GameObject rightHand;

	// Token: 0x040022E4 RID: 8932
	public GameObject leftHand;

	// Token: 0x040022E5 RID: 8933
	public InputDevice leftHandDevice;

	// Token: 0x040022E6 RID: 8934
	public InputDevice rightHandDevice;

	// Token: 0x040022E7 RID: 8935
	public List<InteractionPoint> overlapInteractionPointsLeft = new List<InteractionPoint>();

	// Token: 0x040022E8 RID: 8936
	public List<InteractionPoint> overlapInteractionPointsRight = new List<InteractionPoint>();

	// Token: 0x040022E9 RID: 8937
	public float grabRadius;

	// Token: 0x040022EA RID: 8938
	public float grabThreshold = 0.7f;

	// Token: 0x040022EB RID: 8939
	public float grabHysteresis = 0.05f;

	// Token: 0x040022EC RID: 8940
	public bool wasLeftGrabPressed;

	// Token: 0x040022ED RID: 8941
	public bool wasRightGrabPressed;

	// Token: 0x040022EE RID: 8942
	public bool isLeftGrabbing;

	// Token: 0x040022EF RID: 8943
	public bool isRightGrabbing;

	// Token: 0x040022F0 RID: 8944
	public bool justReleased;

	// Token: 0x040022F1 RID: 8945
	public bool justGrabbed;

	// Token: 0x040022F2 RID: 8946
	public bool disableLeftGrab;

	// Token: 0x040022F3 RID: 8947
	public bool disableRightGrab;

	// Token: 0x040022F4 RID: 8948
	public bool autoGrabLeft;

	// Token: 0x040022F5 RID: 8949
	public bool autoGrabRight;

	// Token: 0x040022F6 RID: 8950
	private float grabValue;

	// Token: 0x040022F7 RID: 8951
	private float tempValue;

	// Token: 0x040022F8 RID: 8952
	private DropZone tempZone;

	// Token: 0x040022F9 RID: 8953
	private bool iteratingInteractionPoints;

	// Token: 0x040022FA RID: 8954
	private List<InteractionPoint> interactionPointsToRemove = new List<InteractionPoint>();

	// Token: 0x040022FB RID: 8955
	[SerializeField]
	private GorillaHandClimber bodyClimber;

	// Token: 0x040022FC RID: 8956
	[SerializeField]
	private GorillaHandClimber leftClimber;

	// Token: 0x040022FD RID: 8957
	[SerializeField]
	private GorillaHandClimber rightClimber;
}
