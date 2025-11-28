using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020007F7 RID: 2039
[NetworkBehaviourWeaved(5)]
public class HalloweenGhostChaser : NetworkComponent
{
	// Token: 0x0600359C RID: 13724 RVA: 0x0012283B File Offset: 0x00120A3B
	protected override void Awake()
	{
		base.Awake();
		this.spawnIndex = 0;
		this.targetPlayer = null;
		this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
		this.grabTime = -this.minGrabCooldown;
		this.possibleTarget = new List<NetPlayer>();
	}

	// Token: 0x0600359D RID: 13725 RVA: 0x00122870 File Offset: 0x00120A70
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x0600359E RID: 13726 RVA: 0x001228A0 File Offset: 0x00120AA0
	private void InitializeGhost()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.lastHeadAngleTime = 0f;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Random.value * this.maxTimeToNextHeadAngle;
			this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			this.ghostBody.transform.localPosition = Vector3.zero;
			base.transform.eulerAngles = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x0600359F RID: 13727 RVA: 0x00122940 File Offset: 0x00120B40
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			HalloweenGhostChaser.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case HalloweenGhostChaser.ChaseState.Dormant:
				if (Time.time >= this.nextTimeToChasePlayer)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				if (Time.time >= this.lastSummonCheck + this.summoningDuration)
				{
					this.lastSummonCheck = Time.time;
					this.possibleTarget.Clear();
					int num = 0;
					int i = 0;
					while (i < this.spawnTransforms.Length)
					{
						int num2 = 0;
						for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
						{
							if ((GorillaParent.instance.vrrigs[j].transform.position - this.spawnTransforms[i].position).magnitude < this.summonDistance)
							{
								this.possibleTarget.Add(GorillaParent.instance.vrrigs[j].creator);
								num2++;
								if (num2 >= this.summonCount)
								{
									break;
								}
							}
						}
						if (num2 >= this.summonCount)
						{
							if (!this.wasSurroundedLastCheck)
							{
								this.wasSurroundedLastCheck = true;
								break;
							}
							this.wasSurroundedLastCheck = false;
							this.isSummoned = true;
							this.currentState = HalloweenGhostChaser.ChaseState.Gong;
							break;
						}
						else
						{
							num++;
							i++;
						}
					}
					if (num == this.spawnTransforms.Length)
					{
						this.wasSurroundedLastCheck = false;
					}
				}
				break;
			case HalloweenGhostChaser.ChaseState.InitialRise:
				if (Time.time > this.timeRiseStarted + this.totalTimeToRise)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.Chasing;
				}
				break;
			case (HalloweenGhostChaser.ChaseState)3:
				break;
			case HalloweenGhostChaser.ChaseState.Gong:
				if (Time.time > this.timeGongStarted + this.gongDuration)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				break;
			default:
				if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
				{
					if (chaseState == HalloweenGhostChaser.ChaseState.Grabbing)
					{
						if (Time.time > this.grabTime + this.grabDuration)
						{
							this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
						}
					}
				}
				else
				{
					if (this.followTarget == null || this.targetPlayer == null)
					{
						this.ChooseRandomTarget();
					}
					if (!(this.followTarget == null) && (this.followTarget.position - this.ghostBody.transform.position).magnitude < this.catchDistance)
					{
						this.currentState = HalloweenGhostChaser.ChaseState.Grabbing;
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

	// Token: 0x060035A0 RID: 13728 RVA: 0x00122BD8 File Offset: 0x00120DD8
	public void UpdateState()
	{
		HalloweenGhostChaser.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			this.isSummoned = false;
			if (this.ghostMaterial.color == this.summonedColor)
			{
				this.ghostMaterial.color = this.defaultColor;
				return;
			}
			break;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.RiseHost();
				}
				this.MoveHead();
				return;
			}
			break;
		case (HalloweenGhostChaser.ChaseState)3:
		case HalloweenGhostChaser.ChaseState.Gong:
			break;
		default:
			if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (chaseState != HalloweenGhostChaser.ChaseState.Grabbing)
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
					this.MoveHead();
				}
			}
			else if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				this.MoveHead();
				return;
			}
			break;
		}
	}

	// Token: 0x060035A1 RID: 13729 RVA: 0x00122CBC File Offset: 0x00120EBC
	private void OnChangeState(HalloweenGhostChaser.ChaseState newState)
	{
		switch (newState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			if (this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				this.InitializeGhost();
			}
			else
			{
				this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			}
			this.SetInitialRotations();
			return;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			this.timeRiseStarted = Time.time;
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				if (!this.isSummoned)
				{
					this.currentSpeed = 0f;
					this.ChooseRandomTarget();
					this.SetInitialSpawnPoint();
				}
				else
				{
					this.currentSpeed = 3f;
				}
			}
			if (this.isSummoned)
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				this.ghostMaterial.color = this.summonedColor;
			}
			else
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlay();
				this.ghostMaterial.color = this.defaultColor;
			}
			this.SetInitialRotations();
			return;
		case (HalloweenGhostChaser.ChaseState)3:
			break;
		case HalloweenGhostChaser.ChaseState.Gong:
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				this.ChooseRandomTarget();
				this.SetInitialSpawnPoint();
				base.transform.position = this.spawnTransforms[this.spawnIndex].position;
			}
			this.timeGongStarted = Time.time;
			this.laugh.volume = 1f;
			this.laugh.GTPlayOneShot(this.gong, 1f);
			this.isSummoned = true;
			return;
		default:
			if (newState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (newState != HalloweenGhostChaser.ChaseState.Grabbing)
				{
					return;
				}
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.grabTime = Time.time;
				if (this.isSummoned)
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				}
				else
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlay();
				}
				this.leftArm.localEulerAngles = this.leftArmGrabbingLocal;
				this.rightArm.localEulerAngles = this.rightArmGrabbingLocal;
				this.leftHand.localEulerAngles = this.leftHandGrabbingLocal;
				this.rightHand.localEulerAngles = this.rightHandGrabbingLocal;
				this.ghostBody.transform.localPosition = this.ghostOffsetGrabbingLocal;
				this.ghostBody.transform.localEulerAngles = this.ghostGrabbingEulerRotation;
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				if (vrrig != null)
				{
					this.followTarget = vrrig.transform;
					return;
				}
			}
			else
			{
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.ResetPath();
			}
			break;
		}
	}

	// Token: 0x060035A2 RID: 13730 RVA: 0x00122FB4 File Offset: 0x001211B4
	private void SetInitialSpawnPoint()
	{
		float num = 1000f;
		this.spawnIndex = 0;
		if (this.followTarget == null)
		{
			return;
		}
		for (int i = 0; i < this.spawnTransforms.Length; i++)
		{
			float magnitude = (this.followTarget.position - this.spawnTransformOffsets[i].position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				this.spawnIndex = i;
			}
		}
	}

	// Token: 0x060035A3 RID: 13731 RVA: 0x00123024 File Offset: 0x00121224
	private void ChooseRandomTarget()
	{
		int num = -1;
		if (this.possibleTarget.Count >= this.summonCount)
		{
			int randomTarget = Random.Range(0, this.possibleTarget.Count);
			num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == this.possibleTarget[randomTarget]);
			this.currentSpeed = 3f;
		}
		if (num == -1)
		{
			num = Random.Range(0, GorillaParent.instance.vrrigs.Count);
		}
		this.possibleTarget.Clear();
		if (num < GorillaParent.instance.vrrigs.Count)
		{
			this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
			this.followTarget = GorillaParent.instance.vrrigs[num].head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, ref navMeshHit, 5f, 1);
			return;
		}
		this.targetPlayer = null;
		this.followTarget = null;
	}

	// Token: 0x060035A4 RID: 13732 RVA: 0x0012313C File Offset: 0x0012133C
	private void SetInitialRotations()
	{
		this.leftArm.localEulerAngles = Vector3.zero;
		this.rightArm.localEulerAngles = Vector3.zero;
		this.leftHand.localEulerAngles = this.leftHandStartingLocal;
		this.rightHand.localEulerAngles = this.rightHandStartingLocal;
		this.ghostBody.transform.localPosition = Vector3.zero;
		this.ghostBody.transform.localEulerAngles = this.ghostStartingEulerRotation;
	}

	// Token: 0x060035A5 RID: 13733 RVA: 0x001231B8 File Offset: 0x001213B8
	private void MoveHead()
	{
		if (Time.time > this.nextHeadAngleTime)
		{
			this.skullTransform.localEulerAngles = this.headEulerAngles[Random.Range(0, this.headEulerAngles.Length)];
			this.lastHeadAngleTime = Time.time;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Mathf.Max(Random.value * this.maxTimeToNextHeadAngle, 0.05f);
		}
	}

	// Token: 0x060035A6 RID: 13734 RVA: 0x00123224 File Offset: 0x00121424
	private void RiseHost()
	{
		if (Time.time < this.timeRiseStarted + this.totalTimeToRise)
		{
			if (this.spawnIndex == -1)
			{
				this.spawnIndex = 0;
			}
			base.transform.position = this.spawnTransforms[this.spawnIndex].position + Vector3.up * (Time.time - this.timeRiseStarted) / this.totalTimeToRise * this.riseDistance;
			base.transform.rotation = this.spawnTransforms[this.spawnIndex].rotation;
		}
	}

	// Token: 0x060035A7 RID: 13735 RVA: 0x001232C0 File Offset: 0x001214C0
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTime + this.minGrabCooldown)
		{
			this.grabTime = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTime + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x060035A8 RID: 13736 RVA: 0x00123370 File Offset: 0x00121570
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.points[this.points.Count - 1] = destination;
		Vector3 vector = this.points[this.currentTargetIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentTargetIdx + 1 < this.points.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentTargetIdx++;
		}
	}

	// Token: 0x060035A9 RID: 13737 RVA: 0x00123480 File Offset: 0x00121680
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, ref navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, ref navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.points = new List<Vector3>();
		foreach (Vector3 vector in this.path.corners)
		{
			this.points.Add(vector + Vector3.up * this.heightAboveNavmesh);
		}
		this.points.Add(destination);
		this.currentTargetIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x060035AA RID: 13738 RVA: 0x00123554 File Offset: 0x00121754
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x060035AB RID: 13739 RVA: 0x00123560 File Offset: 0x00121760
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseTime)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(this.followTarget.position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.followTarget.position, this.currentSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.LookRotation(this.followTarget.position - base.transform.position, Vector3.up);
		}
	}

	// Token: 0x060035AC RID: 13740 RVA: 0x00123634 File Offset: 0x00121834
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.childGhost.localPosition = this.noisyOffset;
		this.leftArm.localEulerAngles = this.noisyOffset * 20f;
		this.rightArm.localEulerAngles = this.noisyOffset * -20f;
	}

	// Token: 0x060035AD RID: 13741 RVA: 0x001236D2 File Offset: 0x001218D2
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x170004C7 RID: 1223
	// (get) Token: 0x060035AE RID: 13742 RVA: 0x0012370E File Offset: 0x0012190E
	// (set) Token: 0x060035AF RID: 13743 RVA: 0x00123738 File Offset: 0x00121938
	[Networked]
	[NetworkedWeaved(0, 5)]
	public unsafe HalloweenGhostChaser.GhostData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HalloweenGhostChaser.GhostData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HalloweenGhostChaser.GhostData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060035B0 RID: 13744 RVA: 0x00123764 File Offset: 0x00121964
	public override void WriteDataFusion()
	{
		HalloweenGhostChaser.GhostData data = default(HalloweenGhostChaser.GhostData);
		NetPlayer netPlayer = this.targetPlayer;
		data.TargetActorNumber = ((netPlayer != null) ? netPlayer.ActorNumber : -1);
		data.CurrentState = (int)this.currentState;
		data.SpawnIndex = this.spawnIndex;
		data.CurrentSpeed = this.currentSpeed;
		data.IsSummoned = this.isSummoned;
		this.Data = data;
	}

	// Token: 0x060035B1 RID: 13745 RVA: 0x001237D4 File Offset: 0x001219D4
	public override void ReadDataFusion()
	{
		int targetActorNumber = this.Data.TargetActorNumber;
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		this.currentState = (HalloweenGhostChaser.ChaseState)this.Data.CurrentState;
		this.spawnIndex = this.Data.SpawnIndex;
		float num = this.Data.CurrentSpeed;
		this.isSummoned = this.Data.IsSummoned;
		if (float.IsFinite(num))
		{
			this.currentSpeed = num;
		}
	}

	// Token: 0x060035B2 RID: 13746 RVA: 0x00123854 File Offset: 0x00121A54
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		if (this.targetPlayer == null)
		{
			stream.SendNext(-1);
		}
		else
		{
			stream.SendNext(this.targetPlayer.ActorNumber);
		}
		stream.SendNext(this.currentState);
		stream.SendNext(this.spawnIndex);
		stream.SendNext(this.currentSpeed);
		stream.SendNext(this.isSummoned);
	}

	// Token: 0x060035B3 RID: 13747 RVA: 0x001238F0 File Offset: 0x00121AF0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		int playerID = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		this.currentState = (HalloweenGhostChaser.ChaseState)stream.ReceiveNext();
		this.spawnIndex = (int)stream.ReceiveNext();
		float num = (float)stream.ReceiveNext();
		this.isSummoned = (bool)stream.ReceiveNext();
		if (float.IsFinite(num))
		{
			this.currentSpeed = num;
		}
	}

	// Token: 0x060035B4 RID: 13748 RVA: 0x00123985 File Offset: 0x00121B85
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x060035B5 RID: 13749 RVA: 0x001239A3 File Offset: 0x00121BA3
	public void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeGhost();
			return;
		}
		this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
	}

	// Token: 0x060035B7 RID: 13751 RVA: 0x00123A6B File Offset: 0x00121C6B
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060035B8 RID: 13752 RVA: 0x00123A83 File Offset: 0x00121C83
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040044BB RID: 17595
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x040044BC RID: 17596
	public Transform followTarget;

	// Token: 0x040044BD RID: 17597
	public Transform childGhost;

	// Token: 0x040044BE RID: 17598
	public float velocityStep = 1f;

	// Token: 0x040044BF RID: 17599
	public float currentSpeed;

	// Token: 0x040044C0 RID: 17600
	public float velocityIncreaseTime = 20f;

	// Token: 0x040044C1 RID: 17601
	public float riseDistance = 2f;

	// Token: 0x040044C2 RID: 17602
	public float summonDistance = 5f;

	// Token: 0x040044C3 RID: 17603
	public float timeEncircled;

	// Token: 0x040044C4 RID: 17604
	public float lastSummonCheck;

	// Token: 0x040044C5 RID: 17605
	public float timeGongStarted;

	// Token: 0x040044C6 RID: 17606
	public float summoningDuration = 30f;

	// Token: 0x040044C7 RID: 17607
	public float summoningCheckCountdown = 5f;

	// Token: 0x040044C8 RID: 17608
	public float gongDuration = 5f;

	// Token: 0x040044C9 RID: 17609
	public int summonCount = 5;

	// Token: 0x040044CA RID: 17610
	public bool wasSurroundedLastCheck;

	// Token: 0x040044CB RID: 17611
	public AudioSource laugh;

	// Token: 0x040044CC RID: 17612
	public List<NetPlayer> possibleTarget;

	// Token: 0x040044CD RID: 17613
	public AudioClip defaultLaugh;

	// Token: 0x040044CE RID: 17614
	public AudioClip deepLaugh;

	// Token: 0x040044CF RID: 17615
	public AudioClip gong;

	// Token: 0x040044D0 RID: 17616
	public Vector3 noisyOffset;

	// Token: 0x040044D1 RID: 17617
	public Vector3 leftArmGrabbingLocal;

	// Token: 0x040044D2 RID: 17618
	public Vector3 rightArmGrabbingLocal;

	// Token: 0x040044D3 RID: 17619
	public Vector3 leftHandGrabbingLocal;

	// Token: 0x040044D4 RID: 17620
	public Vector3 rightHandGrabbingLocal;

	// Token: 0x040044D5 RID: 17621
	public Vector3 leftHandStartingLocal;

	// Token: 0x040044D6 RID: 17622
	public Vector3 rightHandStartingLocal;

	// Token: 0x040044D7 RID: 17623
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x040044D8 RID: 17624
	public Vector3 ghostStartingEulerRotation;

	// Token: 0x040044D9 RID: 17625
	public Vector3 ghostGrabbingEulerRotation;

	// Token: 0x040044DA RID: 17626
	public float maxTimeToNextHeadAngle;

	// Token: 0x040044DB RID: 17627
	public float lastHeadAngleTime;

	// Token: 0x040044DC RID: 17628
	public float nextHeadAngleTime;

	// Token: 0x040044DD RID: 17629
	public float nextTimeToChasePlayer;

	// Token: 0x040044DE RID: 17630
	public float maxNextTimeToChasePlayer;

	// Token: 0x040044DF RID: 17631
	public float timeRiseStarted;

	// Token: 0x040044E0 RID: 17632
	public float totalTimeToRise;

	// Token: 0x040044E1 RID: 17633
	public float catchDistance;

	// Token: 0x040044E2 RID: 17634
	public float grabTime;

	// Token: 0x040044E3 RID: 17635
	public float grabDuration;

	// Token: 0x040044E4 RID: 17636
	public float grabSpeed = 1f;

	// Token: 0x040044E5 RID: 17637
	public float minGrabCooldown;

	// Token: 0x040044E6 RID: 17638
	public float lastSpeedIncreased;

	// Token: 0x040044E7 RID: 17639
	public Vector3[] headEulerAngles;

	// Token: 0x040044E8 RID: 17640
	public Transform skullTransform;

	// Token: 0x040044E9 RID: 17641
	public Transform leftArm;

	// Token: 0x040044EA RID: 17642
	public Transform rightArm;

	// Token: 0x040044EB RID: 17643
	public Transform leftHand;

	// Token: 0x040044EC RID: 17644
	public Transform rightHand;

	// Token: 0x040044ED RID: 17645
	public Transform[] spawnTransforms;

	// Token: 0x040044EE RID: 17646
	public Transform[] spawnTransformOffsets;

	// Token: 0x040044EF RID: 17647
	public NetPlayer targetPlayer;

	// Token: 0x040044F0 RID: 17648
	public GameObject ghostBody;

	// Token: 0x040044F1 RID: 17649
	public HalloweenGhostChaser.ChaseState currentState;

	// Token: 0x040044F2 RID: 17650
	public HalloweenGhostChaser.ChaseState lastState;

	// Token: 0x040044F3 RID: 17651
	public int spawnIndex;

	// Token: 0x040044F4 RID: 17652
	public NetPlayer grabbedPlayer;

	// Token: 0x040044F5 RID: 17653
	public Material ghostMaterial;

	// Token: 0x040044F6 RID: 17654
	public Color defaultColor;

	// Token: 0x040044F7 RID: 17655
	public Color summonedColor;

	// Token: 0x040044F8 RID: 17656
	public bool isSummoned;

	// Token: 0x040044F9 RID: 17657
	private bool targetIsOnNavMesh;

	// Token: 0x040044FA RID: 17658
	private const float navMeshSampleRange = 5f;

	// Token: 0x040044FB RID: 17659
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x040044FC RID: 17660
	public float hapticDuration = 1.5f;

	// Token: 0x040044FD RID: 17661
	private NavMeshPath path;

	// Token: 0x040044FE RID: 17662
	public List<Vector3> points;

	// Token: 0x040044FF RID: 17663
	public int currentTargetIdx;

	// Token: 0x04004500 RID: 17664
	private float nextPathTimestamp;

	// Token: 0x04004501 RID: 17665
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 5)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private HalloweenGhostChaser.GhostData _Data;

	// Token: 0x020007F8 RID: 2040
	public enum ChaseState
	{
		// Token: 0x04004503 RID: 17667
		Dormant = 1,
		// Token: 0x04004504 RID: 17668
		InitialRise,
		// Token: 0x04004505 RID: 17669
		Gong = 4,
		// Token: 0x04004506 RID: 17670
		Chasing = 8,
		// Token: 0x04004507 RID: 17671
		Grabbing = 16
	}

	// Token: 0x020007F9 RID: 2041
	[NetworkStructWeaved(5)]
	[StructLayout(2, Size = 20)]
	public struct GhostData : INetworkStruct
	{
		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x060035B9 RID: 13753 RVA: 0x00123A97 File Offset: 0x00121C97
		// (set) Token: 0x060035BA RID: 13754 RVA: 0x00123AA5 File Offset: 0x00121CA5
		[Networked]
		[NetworkedWeaved(3, 1)]
		public unsafe float CurrentSpeed
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed) = value;
			}
		}

		// Token: 0x04004508 RID: 17672
		[FieldOffset(0)]
		public int TargetActorNumber;

		// Token: 0x04004509 RID: 17673
		[FieldOffset(4)]
		public int CurrentState;

		// Token: 0x0400450A RID: 17674
		[FieldOffset(8)]
		public int SpawnIndex;

		// Token: 0x0400450B RID: 17675
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(12)]
		private FixedStorage@1 _CurrentSpeed;

		// Token: 0x0400450C RID: 17676
		[FieldOffset(16)]
		public NetworkBool IsSummoned;
	}
}
