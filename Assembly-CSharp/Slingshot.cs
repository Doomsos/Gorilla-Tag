using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200041B RID: 1051
public class Slingshot : ProjectileWeapon
{
	// Token: 0x060019E3 RID: 6627 RVA: 0x0008A248 File Offset: 0x00088448
	private void DestroyDummyProjectile()
	{
		if (this.hasDummyProjectile)
		{
			this.dummyProjectile.transform.localScale = Vector3.one * this.dummyProjectileInitialScale;
			this.dummyProjectile.GetComponent<SphereCollider>().enabled = true;
			ObjectPools.instance.Destroy(this.dummyProjectile);
			this.dummyProjectile = null;
			this.hasDummyProjectile = false;
		}
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x0008A2AC File Offset: 0x000884AC
	protected override void Awake()
	{
		base.Awake();
		if (this.elasticLeft)
		{
			this._elasticIntialWidthMultiplier = this.elasticLeft.widthMultiplier;
		}
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x0008A2D2 File Offset: 0x000884D2
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.myRig = rig;
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x0008A2E4 File Offset: 0x000884E4
	internal override void OnEnable()
	{
		this.leftHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapLeft).transform;
		this.rightHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapRight).transform;
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.elasticLeft)
		{
			this.elasticLeft.positionCount = 2;
		}
		if (this.elasticRight)
		{
			this.elasticRight.positionCount = 2;
		}
		this.dummyProjectile = null;
		base.OnEnable();
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x0008A377 File Offset: 0x00088577
	internal override void OnDisable()
	{
		this.DestroyDummyProjectile();
		base.OnDisable();
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x0008A388 File Offset: 0x00088588
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector;
		if (this.InDrawingState())
		{
			if (!this.hasDummyProjectile)
			{
				this.dummyProjectile = ObjectPools.instance.Instantiate(this.projectilePrefab, true);
				this.hasDummyProjectile = true;
				SphereCollider component = this.dummyProjectile.GetComponent<SphereCollider>();
				component.enabled = false;
				this.dummyProjectileColliderRadius = component.radius;
				this.dummyProjectileInitialScale = this.dummyProjectile.transform.localScale.x;
				bool blueTeam;
				bool orangeTeam;
				bool flag;
				base.GetIsOnTeams(out blueTeam, out orangeTeam, out flag);
				this.dummyProjectile.GetComponent<SlingshotProjectile>().ApplyTeamModelAndColor(blueTeam, orangeTeam, flag && this.targetRig, this.targetRig ? this.targetRig.playerColor : default(Color));
			}
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(false);
			}
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(false);
			}
			float num2 = this.dummyProjectileInitialScale * num;
			this.dummyProjectile.transform.localScale = Vector3.one * num2;
			Vector3 position = this.drawingHand.transform.position;
			Vector3 position2 = this.centerOrigin.position;
			Vector3 normalized = (position2 - position).normalized;
			float num3 = (EquipmentInteractor.instance.grabRadius - this.dummyProjectileColliderRadius) * num;
			vector = position + normalized * num3;
			this.dummyProjectile.transform.position = vector;
			this.dummyProjectile.transform.rotation = Quaternion.LookRotation(position2 - vector, Vector3.up);
			if (!this.wasStretching)
			{
				UnityEvent<bool> stretchStartShared = this.StretchStartShared;
				if (stretchStartShared != null)
				{
					stretchStartShared.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretching = true;
			}
		}
		else
		{
			this.DestroyDummyProjectile();
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(true);
			}
			vector = this.centerOrigin.position;
			if (this.wasStretching)
			{
				UnityEvent<bool> stretchEndShared = this.StretchEndShared;
				if (stretchEndShared != null)
				{
					stretchEndShared.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretching = false;
			}
		}
		this.center.position = vector;
		if (!this.disableLineRenderer)
		{
			this.elasticLeftPoints[0] = this.leftArm.position;
			this.elasticLeftPoints[1] = (this.elasticRightPoints[1] = vector);
			this.elasticRightPoints[0] = this.rightArm.position;
			this.elasticLeft.SetPositions(this.elasticLeftPoints);
			this.elasticRight.SetPositions(this.elasticRightPoints);
			this.elasticLeft.widthMultiplier = this._elasticIntialWidthMultiplier * num;
			this.elasticRight.widthMultiplier = this._elasticIntialWidthMultiplier * num;
		}
		if (!NetworkSystem.Instance.InRoom && this.disableWhenNotInRoom)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x0008A6A8 File Offset: 0x000888A8
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = EquipmentInteractor.instance.rightHand;
			}
			else
			{
				this.drawingHand = EquipmentInteractor.instance.leftHand;
			}
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), this.hapticsStrength, this.hapticsLength);
			if (!this.wasStretchingLocal)
			{
				UnityEvent<bool> stretchStartLocal = this.StretchStartLocal;
				if (stretchStartLocal != null)
				{
					stretchStartLocal.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretchingLocal = true;
				return;
			}
		}
		else if (this.wasStretchingLocal)
		{
			UnityEvent<bool> stretchEndLocal = this.StretchEndLocal;
			if (stretchEndLocal != null)
			{
				stretchEndLocal.Invoke(!this.ForLeftHandSlingshot());
			}
			this.wasStretchingLocal = false;
		}
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x0008A763 File Offset: 0x00088963
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = this.rightHandSnap.gameObject;
				return;
			}
			this.drawingHand = this.leftHandSnap.gameObject;
		}
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x0008A79E File Offset: 0x0008899E
	public static bool IsSlingShotEnabled()
	{
		return !(GorillaTagger.Instance == null) && !(GorillaTagger.Instance.offlineVRRig == null) && GorillaTagger.Instance.offlineVRRig.cosmeticSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x0008A7D8 File Offset: 0x000889D8
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		bool flag = pointGrabbed == this.nock;
		if (flag && !base.InHand())
		{
			return;
		}
		base.OnGrab(pointGrabbed, grabbingHand);
		if (this.InDrawingState() || base.OnChest())
		{
			return;
		}
		if (flag)
		{
			if (grabbingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = true;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = true;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.itemState = TransferrableObject.ItemStates.State2;
			}
			else
			{
				this.itemState = TransferrableObject.ItemStates.State3;
			}
			this.minTimeToLaunch = Time.time + this.delayLaunchTime;
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
		}
	}

	// Token: 0x060019ED RID: 6637 RVA: 0x0008A8B4 File Offset: 0x00088AB4
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (this.InDrawingState() && releasingHand == this.drawingHand)
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = false;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = false;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.currentState = TransferrableObject.PositionState.InLeftHand;
			}
			else
			{
				this.currentState = TransferrableObject.PositionState.InRightHand;
			}
			this.itemState = TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			if (Time.time > this.minTimeToLaunch && (releasingHand.transform.position - this.centerOrigin.transform.position).sqrMagnitude > this.minDrawDistanceToRelease * this.minDrawDistanceToRelease)
			{
				base.LaunchProjectile();
			}
		}
		else
		{
			EquipmentInteractor.instance.disableLeftGrab = false;
			EquipmentInteractor.instance.disableRightGrab = false;
		}
		return true;
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x0008A9CC File Offset: 0x00088BCC
	public override void DropItemCleanup()
	{
		base.DropItemCleanup();
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060019EF RID: 6639 RVA: 0x00027DED File Offset: 0x00025FED
	public override bool AutoGrabTrue(bool leftGrabbingHand)
	{
		return true;
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x0008A9E3 File Offset: 0x00088BE3
	private bool ForLeftHandSlingshot()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x0008A9F9 File Offset: 0x00088BF9
	private bool InDrawingState()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State3;
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x0008AA0F File Offset: 0x00088C0F
	protected override Vector3 GetLaunchPosition()
	{
		return this.dummyProjectile.transform.position;
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x0008AA24 File Offset: 0x00088C24
	protected override Vector3 GetLaunchVelocity()
	{
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector = this.centerOrigin.position - this.center.position;
		vector /= num;
		Vector3 vector2 = Mathf.Min(this.springConstant * this.maxDraw, vector.magnitude * this.springConstant) * vector.normalized * num;
		Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
		return vector2 + averagedVelocity;
	}

	// Token: 0x04002356 RID: 9046
	[SerializeField]
	private bool disableLineRenderer;

	// Token: 0x04002357 RID: 9047
	[FormerlySerializedAs("elastic")]
	public LineRenderer elasticLeft;

	// Token: 0x04002358 RID: 9048
	public LineRenderer elasticRight;

	// Token: 0x04002359 RID: 9049
	public Transform leftArm;

	// Token: 0x0400235A RID: 9050
	public Transform rightArm;

	// Token: 0x0400235B RID: 9051
	public Transform center;

	// Token: 0x0400235C RID: 9052
	public Transform centerOrigin;

	// Token: 0x0400235D RID: 9053
	private GameObject dummyProjectile;

	// Token: 0x0400235E RID: 9054
	public GameObject drawingHand;

	// Token: 0x0400235F RID: 9055
	public InteractionPoint nock;

	// Token: 0x04002360 RID: 9056
	public InteractionPoint grip;

	// Token: 0x04002361 RID: 9057
	public float springConstant;

	// Token: 0x04002362 RID: 9058
	public float maxDraw;

	// Token: 0x04002363 RID: 9059
	[SerializeField]
	private GameObject disableInDraw;

	// Token: 0x04002364 RID: 9060
	[SerializeField]
	private float minDrawDistanceToRelease;

	// Token: 0x04002365 RID: 9061
	[Header("Stretching Haptics")]
	[Space]
	[SerializeField]
	private bool playStretchingHaptics;

	// Token: 0x04002366 RID: 9062
	[SerializeField]
	private float hapticsStrength = 0.1f;

	// Token: 0x04002367 RID: 9063
	[SerializeField]
	private float hapticsLength = 0.1f;

	// Token: 0x04002368 RID: 9064
	[Header("Stretching Events")]
	[Space]
	public UnityEvent<bool> StretchStartShared;

	// Token: 0x04002369 RID: 9065
	public UnityEvent<bool> StretchEndShared;

	// Token: 0x0400236A RID: 9066
	[Space]
	public UnityEvent<bool> StretchStartLocal;

	// Token: 0x0400236B RID: 9067
	public UnityEvent<bool> StretchEndLocal;

	// Token: 0x0400236C RID: 9068
	private bool wasStretching;

	// Token: 0x0400236D RID: 9069
	private bool wasStretchingLocal;

	// Token: 0x0400236E RID: 9070
	private Transform leftHandSnap;

	// Token: 0x0400236F RID: 9071
	private Transform rightHandSnap;

	// Token: 0x04002370 RID: 9072
	public bool disableWhenNotInRoom;

	// Token: 0x04002371 RID: 9073
	private bool hasDummyProjectile;

	// Token: 0x04002372 RID: 9074
	private float delayLaunchTime = 0.07f;

	// Token: 0x04002373 RID: 9075
	private float minTimeToLaunch = -1f;

	// Token: 0x04002374 RID: 9076
	private float dummyProjectileColliderRadius;

	// Token: 0x04002375 RID: 9077
	private float dummyProjectileInitialScale;

	// Token: 0x04002376 RID: 9078
	private int projectileCount;

	// Token: 0x04002377 RID: 9079
	private Vector3[] elasticLeftPoints = new Vector3[2];

	// Token: 0x04002378 RID: 9080
	private Vector3[] elasticRightPoints = new Vector3[2];

	// Token: 0x04002379 RID: 9081
	private float _elasticIntialWidthMultiplier;

	// Token: 0x0400237A RID: 9082
	private new VRRig myRig;

	// Token: 0x0200041C RID: 1052
	public enum SlingshotState
	{
		// Token: 0x0400237C RID: 9084
		NoState = 1,
		// Token: 0x0400237D RID: 9085
		OnChest,
		// Token: 0x0400237E RID: 9086
		LeftHandDrawing = 4,
		// Token: 0x0400237F RID: 9087
		RightHandDrawing = 8
	}

	// Token: 0x0200041D RID: 1053
	public enum SlingshotActions
	{
		// Token: 0x04002381 RID: 9089
		Grab,
		// Token: 0x04002382 RID: 9090
		Release
	}
}
