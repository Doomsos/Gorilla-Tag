using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

public class SIGadgetTentacleArm : SIGadget, ICallBack
{
	public bool isAnchored { get; private set; }

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

	private void Start()
	{
		this.clawVisualPos = this.claw.transform.position;
		this.clawVisualRot = this.claw.transform.rotation;
		this.CallBack();
	}

	private void OnDestroy()
	{
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

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

	private void OnReleased()
	{
		this.ClearClawAnchor();
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

	private void OnUnsnapped()
	{
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

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

	private long GetStateLong()
	{
		if (this.isAnchored)
		{
			return this.anchoredBit | BitPackUtils.PackAnchoredPosRotForNetwork(this.clawVisualPos, this.clawVisualRot);
		}
		return 0L;
	}

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

	private void GravityOverrideFunction(GTPlayer player)
	{
	}

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

	public void CallBack()
	{
		this.claw.transform.position = this.clawVisualPos;
		this.claw.transform.rotation = this.clawVisualRot;
		Vector3 vector = this.tentacleRenderer.transform.InverseTransformPoint(this.tentacleAnchor.position);
		this.tentacleMat.SetVector(this.tentacleEnd, vector);
		Vector3 vector2 = -this.tentacleRenderer.transform.InverseTransformDirection(this.tentacleAnchor.forward);
		this.tentacleMat.SetVector(this.tentacleEndDir, vector2);
	}

	[SerializeField]
	private GameObject claw;

	[SerializeField]
	private LayerMask worldCollisionLayers;

	[SerializeField]
	private Transform marker;

	[SerializeField]
	private float maxTentacleLength;

	[SerializeField]
	private MeshRenderer tentacleRenderer;

	[SerializeField]
	private Transform tentacleAnchor;

	private Material tentacleMat;

	private ShaderHashId tentacleEnd = "_TentacleEndPos";

	private ShaderHashId tentacleEndDir = "_TentacleEndDir";

	private bool isLeftHanded;

	private Vector3 knownSafePosition;

	private Vector3 clawHoldAdjustment;

	private Vector3 clawAnchorPosition;

	private Vector3 lastRequestedPlayerPosition;

	private Quaternion clawRotationOnGrab;

	private bool isGripBroken;

	private bool hasRigCallback;

	private VRRig rigForCallback;

	private Vector3 clawVisualPos;

	private Quaternion clawVisualRot;

	private long anchoredBit = 4611686018427387904L;
}
