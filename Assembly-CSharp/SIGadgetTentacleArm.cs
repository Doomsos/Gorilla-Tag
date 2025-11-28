using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class SIGadgetTentacleArm : SIGadget, ICallBack
{
	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000648 RID: 1608 RVA: 0x000239A5 File Offset: 0x00021BA5
	// (set) Token: 0x06000649 RID: 1609 RVA: 0x000239AD File Offset: 0x00021BAD
	public bool isAnchored { get; private set; }

	// Token: 0x0600064A RID: 1610 RVA: 0x000239B8 File Offset: 0x00021BB8
	private void Awake()
	{
		this.tentacleMat = new Material(this.tentacleRenderer.sharedMaterial);
		this.tentacleRenderer.sharedMaterial = this.tentacleMat;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x00023A9F File Offset: 0x00021C9F
	private void Start()
	{
		this.clawVisualPos = this.claw.transform.position;
		this.clawVisualRot = this.claw.transform.rotation;
		this.CallBack();
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x00023AD3 File Offset: 0x00021CD3
	private void OnDestroy()
	{
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x00023AF0 File Offset: 0x00021CF0
	private void OnGrabbed()
	{
		this.isLeftHanded = (this.gameEntity.heldByHandIndex == 0);
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			this.hasRigCallback = true;
			this.rigForCallback = gamePlayer.rig;
			this.rigForCallback.AddLateUpdateCallback(this);
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00023B44 File Offset: 0x00021D44
	private void OnSnapped()
	{
		this.isLeftHanded = (this.gameEntity.snappedJoint == SnapJointType.HandL);
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer))
		{
			this.hasRigCallback = true;
			this.rigForCallback = gamePlayer.rig;
			this.rigForCallback.AddLateUpdateCallback(this);
		}
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00023B98 File Offset: 0x00021D98
	private void OnReleased()
	{
		this.ClearClawAnchor();
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00023AD3 File Offset: 0x00021CD3
	private void OnUnsnapped()
	{
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00023BBC File Offset: 0x00021DBC
	protected override void OnUpdateAuthority(float dt)
	{
		Vector3 position = GTPlayer.Instance.bodyCollider.transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 vector = position2 - position;
		Component controllerTransform = (this.isLeftHanded ? GTPlayer.Instance.LeftHand : GTPlayer.Instance.RightHand).controllerTransform;
		float num = this.isLeftHanded ? ControllerInputPoller.instance.leftControllerIndexFloat : ControllerInputPoller.instance.rightControllerIndexFloat;
		bool flag = num >= 0.9f;
		if (this.isGripBroken)
		{
			if (flag)
			{
				num = 0f;
				flag = false;
			}
			else
			{
				this.isGripBroken = false;
			}
		}
		Vector3 vector2 = position2 + vector;
		Quaternion quaternion = controllerTransform.transform.rotation * Quaternion.Euler(90f, 0f, 0f);
		if ((this.knownSafePosition - vector2).IsLongerThan(1f))
		{
			this.knownSafePosition = position2;
		}
		float num2 = 0.15f;
		this.clawVisualRot = base.transform.rotation;
		RaycastHit raycastHit;
		bool flag2 = Physics.SphereCast(new Ray(this.knownSafePosition, vector2 - this.knownSafePosition), num2, ref raycastHit, (vector2 - this.knownSafePosition).magnitude, this.worldCollisionLayers);
		if (this.isAnchored)
		{
			if (flag)
			{
				Vector3 position3 = GTPlayer.Instance.transform.position;
				this.clawHoldAdjustment -= position3 - this.lastRequestedPlayerPosition;
				Vector3 vector3 = this.clawAnchorPosition - (vector2 + this.clawHoldAdjustment);
				GTPlayer.Instance.RequestTentacleMove(this.isLeftHanded, vector3);
				this.lastRequestedPlayerPosition = position3 + vector3;
				if ((this.clawAnchorPosition - base.transform.position).IsLongerThan(this.maxTentacleLength))
				{
					this.isGripBroken = true;
					this.ClearClawAnchor();
					return;
				}
				this.clawVisualPos = this.clawAnchorPosition;
				this.clawVisualRot = this.clawRotationOnGrab;
				return;
			}
			else
			{
				this.ClearClawAnchor();
			}
		}
		Vector3 vector4 = vector2;
		Quaternion clawRotation = quaternion;
		if (flag2)
		{
			this.knownSafePosition += (vector2 - this.knownSafePosition).normalized * (raycastHit.distance - num2 * 2.01f);
			this.marker.transform.position = raycastHit.point;
			this.marker.transform.rotation = Quaternion.LookRotation(-raycastHit.normal, quaternion * Vector3.up);
			vector4 = raycastHit.point + raycastHit.normal * Mathf.Lerp(0.1f, 0.01f, num);
			clawRotation = Quaternion.Lerp(quaternion, Quaternion.LookRotation(-raycastHit.normal, quaternion * Vector3.up), num * 0.5f + 0.5f);
		}
		else
		{
			this.knownSafePosition = vector2;
		}
		this.clawVisualPos = vector4;
		this.clawVisualRot = clawRotation;
		if (!this.isAnchored && flag && flag2)
		{
			this.SetClawAnchor(vector4, clawRotation, vector4 - vector2);
		}
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00023F04 File Offset: 0x00022104
	protected override void OnUpdateRemote(float dt)
	{
		if (this.isAnchored)
		{
			return;
		}
		int attachedPlayerActorNumber = base.GetAttachedPlayerActorNumber();
		GamePlayer gamePlayer;
		if (attachedPlayerActorNumber < 1 || !GamePlayer.TryGetGamePlayer(attachedPlayerActorNumber, out gamePlayer))
		{
			return;
		}
		Vector3 position = gamePlayer.rig.bodyTransform.position;
		Vector3 position2 = base.transform.position;
		Vector3 vector = position2 - position;
		Vector3 vector2 = position2 + vector;
		Quaternion quaternion = base.transform.rotation * Quaternion.Euler(90f, 0f, 0f);
		if ((this.knownSafePosition - vector2).IsLongerThan(1f))
		{
			this.knownSafePosition = position2;
		}
		float num = 0.15f;
		RaycastHit raycastHit;
		bool flag = Physics.SphereCast(new Ray(this.knownSafePosition, vector2 - this.knownSafePosition), num, ref raycastHit, (vector2 - this.knownSafePosition).magnitude, this.worldCollisionLayers);
		Vector3 vector3 = vector2;
		Quaternion quaternion2 = quaternion;
		if (flag)
		{
			this.knownSafePosition += (vector2 - this.knownSafePosition).normalized * (raycastHit.distance - num * 2.01f);
			vector3 = raycastHit.point + raycastHit.normal * 0.1f;
		}
		else
		{
			this.knownSafePosition = vector2;
		}
		this.clawVisualPos = vector3;
		this.clawVisualRot = quaternion2;
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x0002406E File Offset: 0x0002226E
	private long GetStateLong()
	{
		if (this.isAnchored)
		{
			return this.anchoredBit | BitPackUtils.PackAnchoredPosRotForNetwork(this.clawVisualPos, this.clawVisualRot);
		}
		return 0L;
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x00024094 File Offset: 0x00022294
	private void SetClawAnchor(Vector3 clawPosition, Quaternion clawRotation, Vector3 adjustment)
	{
		this.isAnchored = true;
		this.clawHoldAdjustment = adjustment;
		this.clawAnchorPosition = clawPosition;
		this.clawRotationOnGrab = clawRotation;
		if (this.IsEquippedLocal())
		{
			this.lastRequestedPlayerPosition = GTPlayer.Instance.transform.position;
			GTPlayer.Instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
			this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x00024110 File Offset: 0x00022310
	private void ClearClawAnchor()
	{
		this.isAnchored = false;
		if (this.IsEquippedLocal())
		{
			GTPlayer.Instance.SetVelocity(GTPlayer.Instance.AveragedVelocity);
			GTPlayer.Instance.UnsetGravityOverride(this);
			this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
		}
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x00002789 File Offset: 0x00000989
	private void GravityOverrideFunction(GTPlayer player)
	{
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00024168 File Offset: 0x00022368
	private void OnEntityStateChanged(long oldState, long newState)
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			return;
		}
		if (newState != 0L)
		{
			int attachedPlayerActorNumber = base.GetAttachedPlayerActorNumber();
			GamePlayer gamePlayer;
			if (attachedPlayerActorNumber >= 1 && GamePlayer.TryGetGamePlayer(attachedPlayerActorNumber, out gamePlayer))
			{
				Vector3 clawPosition;
				Quaternion clawRotation;
				BitPackUtils.UnpackAnchoredPosRotForNetwork(newState, gamePlayer.rig.transform.position, out clawPosition, out clawRotation);
				this.SetClawAnchor(clawPosition, clawRotation, Vector3.zero);
				this.clawVisualPos = this.clawAnchorPosition;
				this.clawVisualRot = this.clawRotationOnGrab;
				return;
			}
		}
		else
		{
			this.ClearClawAnchor();
		}
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x000241E4 File Offset: 0x000223E4
	public void CallBack()
	{
		this.claw.transform.position = this.clawVisualPos;
		this.claw.transform.rotation = this.clawVisualRot;
		Vector3 vector = this.tentacleRenderer.transform.InverseTransformPoint(this.tentacleAnchor.position);
		this.tentacleMat.SetVector(this.tentacleEnd, vector);
		Vector3 vector2 = -this.tentacleRenderer.transform.InverseTransformDirection(this.tentacleAnchor.forward);
		this.tentacleMat.SetVector(this.tentacleEndDir, vector2);
	}

	// Token: 0x040007CE RID: 1998
	[SerializeField]
	private GameObject claw;

	// Token: 0x040007CF RID: 1999
	[SerializeField]
	private LayerMask worldCollisionLayers;

	// Token: 0x040007D0 RID: 2000
	[SerializeField]
	private Transform marker;

	// Token: 0x040007D1 RID: 2001
	[SerializeField]
	private float maxTentacleLength;

	// Token: 0x040007D2 RID: 2002
	[SerializeField]
	private MeshRenderer tentacleRenderer;

	// Token: 0x040007D3 RID: 2003
	[SerializeField]
	private Transform tentacleAnchor;

	// Token: 0x040007D4 RID: 2004
	private Material tentacleMat;

	// Token: 0x040007D5 RID: 2005
	private ShaderHashId tentacleEnd = "_TentacleEndPos";

	// Token: 0x040007D6 RID: 2006
	private ShaderHashId tentacleEndDir = "_TentacleEndDir";

	// Token: 0x040007D7 RID: 2007
	private bool isLeftHanded;

	// Token: 0x040007D8 RID: 2008
	private Vector3 knownSafePosition;

	// Token: 0x040007D9 RID: 2009
	private Vector3 clawHoldAdjustment;

	// Token: 0x040007DA RID: 2010
	private Vector3 clawAnchorPosition;

	// Token: 0x040007DB RID: 2011
	private Vector3 lastRequestedPlayerPosition;

	// Token: 0x040007DC RID: 2012
	private Quaternion clawRotationOnGrab;

	// Token: 0x040007DD RID: 2013
	private bool isGripBroken;

	// Token: 0x040007DF RID: 2015
	private bool hasRigCallback;

	// Token: 0x040007E0 RID: 2016
	private VRRig rigForCallback;

	// Token: 0x040007E1 RID: 2017
	private Vector3 clawVisualPos;

	// Token: 0x040007E2 RID: 2018
	private Quaternion clawVisualRot;

	// Token: 0x040007E3 RID: 2019
	private long anchoredBit = 4611686018427387904L;
}
