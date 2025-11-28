using System;
using System.Collections.Generic;
using Fusion;
using GorillaTag.Rendering;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

// Token: 0x020001AF RID: 431
[NetworkBehaviourWeaved(3)]
public class AngryBeeSwarm : NetworkComponent
{
	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000B7B RID: 2939 RVA: 0x0003E380 File Offset: 0x0003C580
	public bool isDormant
	{
		get
		{
			return this.currentState == AngryBeeSwarm.ChaseState.Dormant;
		}
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x0003E38C File Offset: 0x0003C58C
	protected override void Awake()
	{
		base.Awake();
		AngryBeeSwarm.instance = this;
		this.targetPlayer = null;
		this.currentState = AngryBeeSwarm.ChaseState.Dormant;
		this.grabTimestamp = -this.minGrabCooldown;
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x0003E3DC File Offset: 0x0003C5DC
	private void InitializeSwarm()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.beeAnimator.transform.localPosition = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x0003E428 File Offset: 0x0003C628
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = AngryBeeSwarm.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			AngryBeeSwarm.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case AngryBeeSwarm.ChaseState.Dormant:
				if (Application.isEditor && Keyboard.current[1].wasPressedThisFrame)
				{
					this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
				}
				break;
			case AngryBeeSwarm.ChaseState.InitialEmerge:
				if (Time.time > this.emergeStartedTimestamp + this.totalTimeToEmerge)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Chasing;
				}
				break;
			case (AngryBeeSwarm.ChaseState)3:
				break;
			case AngryBeeSwarm.ChaseState.Chasing:
				if (this.followTarget == null || this.targetPlayer == null || Time.time > this.NextRefreshClosestPlayerTimestamp)
				{
					this.ChooseClosestTarget();
					if (this.followTarget != null)
					{
						this.BoredToDeathAtTimestamp = -1f;
					}
					else if (this.BoredToDeathAtTimestamp < 0f)
					{
						this.BoredToDeathAtTimestamp = Time.time + this.boredAfterDuration;
					}
				}
				if (this.BoredToDeathAtTimestamp >= 0f && Time.time > this.BoredToDeathAtTimestamp)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Dormant;
				}
				else if (!(this.followTarget == null) && (this.followTarget.position - this.beeAnimator.transform.position).magnitude < this.catchDistance)
				{
					float num = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
					if (this.followTarget.position.y > num)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Grabbing;
					}
				}
				break;
			default:
				if (chaseState == AngryBeeSwarm.ChaseState.Grabbing)
				{
					if (Time.time > this.grabTimestamp + this.grabDuration)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Dormant;
					}
				}
				break;
			}
		}
		if (this.lastState != this.currentState)
		{
			this.OnChangeState(this.currentState);
			this.lastState = this.currentState;
		}
		this.UpdateState();
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x0003E60C File Offset: 0x0003C80C
	public void UpdateState()
	{
		AngryBeeSwarm.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			if (NetworkSystem.Instance.InRoom)
			{
				this.SwarmEmergeUpdateShared();
				return;
			}
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				return;
			}
			break;
		default:
			if (chaseState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.RiseGrabbedLocalPlayer();
				}
				this.GrabBodyShared();
			}
			break;
		}
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0003E69B File Offset: 0x0003C89B
	public void Emerge(Vector3 fromPosition, Vector3 toPosition)
	{
		base.transform.position = fromPosition;
		this.emergeFromPosition = fromPosition;
		this.emergeToPosition = toPosition;
		this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
		this.emergeStartedTimestamp = Time.time;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x0003E6CC File Offset: 0x0003C8CC
	private void OnChangeState(AngryBeeSwarm.ChaseState newState)
	{
		switch (newState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
			if (this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				base.transform.position = new Vector3(0f, -9999f, 0f);
				this.InitializeSwarm();
			}
			this.SetInitialRotations();
			return;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			this.emergeStartedTimestamp = Time.time;
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(0f);
			if (base.IsMine)
			{
				this.currentSpeed = 0f;
				this.ChooseClosestTarget();
			}
			this.SetInitialRotations();
			return;
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(1f);
			this.ResetPath();
			this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
			this.BoredToDeathAtTimestamp = -1f;
			return;
		default:
		{
			if (newState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.grabTimestamp = Time.time;
			this.beeAnimator.transform.localPosition = this.ghostOffsetGrabbingLocal;
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
			if (vrrig != null)
			{
				this.followTarget = vrrig.transform;
			}
			break;
		}
		}
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x0003E874 File Offset: 0x0003CA74
	private void ChooseClosestTarget()
	{
		float num = Mathf.Lerp(this.initialRangeLimit, this.finalRangeLimit, (Time.time + this.totalTimeToEmerge - this.emergeStartedTimestamp) / this.rangeLimitBlendDuration);
		float num2 = num * num;
		VRRig vrrig = null;
		float num3 = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
		foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
		{
			if (vrrig2.head != null && !(vrrig2.head.rigTarget == null) && vrrig2.head.rigTarget.position.y > num3)
			{
				float sqrMagnitude = (base.transform.position - vrrig2.head.rigTarget.transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					vrrig = vrrig2;
				}
			}
		}
		if (vrrig != null)
		{
			this.targetPlayer = vrrig.creator;
			this.followTarget = vrrig.head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, ref navMeshHit, 5f, 1);
		}
		else
		{
			this.targetPlayer = null;
			this.followTarget = null;
		}
		this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0003E9E0 File Offset: 0x0003CBE0
	private void SetInitialRotations()
	{
		this.beeAnimator.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0003E9F8 File Offset: 0x0003CBF8
	private void SwarmEmergeUpdateShared()
	{
		if (Time.time < this.emergeStartedTimestamp + this.totalTimeToEmerge)
		{
			float emergeFraction = (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge;
			if (base.IsMine)
			{
				base.transform.position = Vector3.Lerp(this.emergeFromPosition, this.emergeToPosition, (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge);
			}
			this.beeAnimator.SetEmergeFraction(emergeFraction);
		}
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0003EA70 File Offset: 0x0003CC70
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTimestamp + this.minGrabCooldown)
		{
			this.grabTimestamp = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTimestamp + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0003EB20 File Offset: 0x0003CD20
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.pathPoints[this.pathPoints.Count - 1] = destination;
		Vector3 vector = this.pathPoints[this.currentPathPointIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentPathPointIdx + 1 < this.pathPoints.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentPathPointIdx++;
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0003EC30 File Offset: 0x0003CE30
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, ref navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, ref navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.pathPoints = new List<Vector3>();
		foreach (Vector3 vector in this.path.corners)
		{
			this.pathPoints.Add(vector + Vector3.up * this.heightAboveNavmesh);
		}
		this.pathPoints.Add(destination);
		this.currentPathPointIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0003ED04 File Offset: 0x0003CF04
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0003ED10 File Offset: 0x0003CF10
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseInterval)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			float num = ZoneShaderSettings.GetWaterY() + this.MinHeightAboveWater;
			Vector3 position = this.followTarget.position;
			if (position.y < num)
			{
				position.y = num;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, position, this.currentSpeed * Time.deltaTime);
		}
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x0003EDC8 File Offset: 0x0003CFC8
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.beeAnimator.transform.localPosition = this.noisyOffset;
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x0003EE35 File Offset: 0x0003D035
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000B8C RID: 2956 RVA: 0x0003EE71 File Offset: 0x0003D071
	// (set) Token: 0x06000B8D RID: 2957 RVA: 0x0003EE9B File Offset: 0x0003D09B
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe BeeSwarmData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BeeSwarmData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BeeSwarmData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x0003EEC6 File Offset: 0x0003D0C6
	public override void WriteDataFusion()
	{
		this.Data = new BeeSwarmData(this.targetPlayer.ActorNumber, (int)this.currentState, this.currentSpeed);
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x0003EEEC File Offset: 0x0003D0EC
	public override void ReadDataFusion()
	{
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(this.Data.TargetActorNumber);
		this.currentState = (AngryBeeSwarm.ChaseState)this.Data.CurrentState;
		if (float.IsFinite(this.Data.CurrentSpeed))
		{
			this.currentSpeed = this.Data.CurrentSpeed;
		}
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x0003EF54 File Offset: 0x0003D154
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender == null || !info.Sender.Equals(PhotonNetwork.MasterClient))
		{
			return;
		}
		NetPlayer netPlayer = this.targetPlayer;
		stream.SendNext((netPlayer != null) ? netPlayer.ActorNumber : -1);
		stream.SendNext(this.currentState);
		stream.SendNext(this.currentSpeed);
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x0003EFBC File Offset: 0x0003D1BC
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int playerID = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		this.currentState = (AngryBeeSwarm.ChaseState)stream.ReceiveNext();
		float num = (float)stream.ReceiveNext();
		if (float.IsFinite(num))
		{
			this.currentSpeed = num;
		}
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x0003F020 File Offset: 0x0003D220
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x0003F03E File Offset: 0x0003D23E
	public void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeSwarm();
		}
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x0003F052 File Offset: 0x0003D252
	private void TestEmerge()
	{
		this.Emerge(this.testEmergeFrom.transform.position, this.testEmergeTo.transform.position);
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x0003F108 File Offset: 0x0003D308
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x0003F120 File Offset: 0x0003D320
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000E1A RID: 3610
	public static AngryBeeSwarm instance;

	// Token: 0x04000E1B RID: 3611
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x04000E1C RID: 3612
	public Transform followTarget;

	// Token: 0x04000E1D RID: 3613
	[SerializeField]
	private float velocityStep = 1f;

	// Token: 0x04000E1E RID: 3614
	private float currentSpeed;

	// Token: 0x04000E1F RID: 3615
	[SerializeField]
	private float velocityIncreaseInterval = 20f;

	// Token: 0x04000E20 RID: 3616
	public Vector3 noisyOffset;

	// Token: 0x04000E21 RID: 3617
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x04000E22 RID: 3618
	private float emergeStartedTimestamp;

	// Token: 0x04000E23 RID: 3619
	private float grabTimestamp;

	// Token: 0x04000E24 RID: 3620
	private float lastSpeedIncreased;

	// Token: 0x04000E25 RID: 3621
	[SerializeField]
	private float totalTimeToEmerge;

	// Token: 0x04000E26 RID: 3622
	[SerializeField]
	private float catchDistance;

	// Token: 0x04000E27 RID: 3623
	[SerializeField]
	private float grabDuration;

	// Token: 0x04000E28 RID: 3624
	[SerializeField]
	private float grabSpeed = 1f;

	// Token: 0x04000E29 RID: 3625
	[SerializeField]
	private float minGrabCooldown;

	// Token: 0x04000E2A RID: 3626
	[SerializeField]
	private float initialRangeLimit;

	// Token: 0x04000E2B RID: 3627
	[SerializeField]
	private float finalRangeLimit;

	// Token: 0x04000E2C RID: 3628
	[SerializeField]
	private float rangeLimitBlendDuration;

	// Token: 0x04000E2D RID: 3629
	[SerializeField]
	private float boredAfterDuration;

	// Token: 0x04000E2E RID: 3630
	public NetPlayer targetPlayer;

	// Token: 0x04000E2F RID: 3631
	public AngryBeeAnimator beeAnimator;

	// Token: 0x04000E30 RID: 3632
	public AngryBeeSwarm.ChaseState currentState;

	// Token: 0x04000E31 RID: 3633
	public AngryBeeSwarm.ChaseState lastState;

	// Token: 0x04000E32 RID: 3634
	public NetPlayer grabbedPlayer;

	// Token: 0x04000E33 RID: 3635
	private bool targetIsOnNavMesh;

	// Token: 0x04000E34 RID: 3636
	private const float navMeshSampleRange = 5f;

	// Token: 0x04000E35 RID: 3637
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x04000E36 RID: 3638
	public float hapticDuration = 1.5f;

	// Token: 0x04000E37 RID: 3639
	public float MinHeightAboveWater = 0.5f;

	// Token: 0x04000E38 RID: 3640
	public float PlayerMinHeightAboveWater = 0.5f;

	// Token: 0x04000E39 RID: 3641
	public float RefreshClosestPlayerInterval = 1f;

	// Token: 0x04000E3A RID: 3642
	private float NextRefreshClosestPlayerTimestamp = 1f;

	// Token: 0x04000E3B RID: 3643
	private float BoredToDeathAtTimestamp = -1f;

	// Token: 0x04000E3C RID: 3644
	[SerializeField]
	private Transform testEmergeFrom;

	// Token: 0x04000E3D RID: 3645
	[SerializeField]
	private Transform testEmergeTo;

	// Token: 0x04000E3E RID: 3646
	private Vector3 emergeFromPosition;

	// Token: 0x04000E3F RID: 3647
	private Vector3 emergeToPosition;

	// Token: 0x04000E40 RID: 3648
	private NavMeshPath path;

	// Token: 0x04000E41 RID: 3649
	public List<Vector3> pathPoints;

	// Token: 0x04000E42 RID: 3650
	public int currentPathPointIdx;

	// Token: 0x04000E43 RID: 3651
	private float nextPathTimestamp;

	// Token: 0x04000E44 RID: 3652
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private BeeSwarmData _Data;

	// Token: 0x020001B0 RID: 432
	public enum ChaseState
	{
		// Token: 0x04000E46 RID: 3654
		Dormant = 1,
		// Token: 0x04000E47 RID: 3655
		InitialEmerge,
		// Token: 0x04000E48 RID: 3656
		Chasing = 4,
		// Token: 0x04000E49 RID: 3657
		Grabbing = 8
	}
}
