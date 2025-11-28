using System;
using System.Collections.Generic;
using GorillaLocomotion;
using JetBrains.Annotations;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200016B RID: 363
[RequireComponent(typeof(NetworkView))]
public class MonkeyeAI : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060009AB RID: 2475 RVA: 0x00033EC0 File Offset: 0x000320C0
	private string UserIdFromRig(VRRig rig)
	{
		if (rig == null)
		{
			return "";
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return "-1";
			}
			Debug.Log("Not in a room but not targeting offline rig");
			return null;
		}
		else
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return NetworkSystem.Instance.LocalPlayer.UserId;
			}
			if (rig.creator == null)
			{
				return "";
			}
			return rig.creator.UserId;
		}
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00033F48 File Offset: 0x00032148
	private VRRig GetRig(string userId)
	{
		if (userId == "")
		{
			return null;
		}
		if (NetworkSystem.Instance.InRoom || !(userId != "-1"))
		{
			foreach (VRRig vrrig in this.GetValidChoosableRigs())
			{
				if (!(vrrig == null))
				{
					NetPlayer creator = vrrig.creator;
					if (creator != null && userId == creator.UserId)
					{
						return vrrig;
					}
				}
			}
			return null;
		}
		if (userId == "-1 " && GorillaTagger.Instance != null)
		{
			return GorillaTagger.Instance.offlineVRRig;
		}
		return null;
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x0003400C File Offset: 0x0003220C
	private float Distance2D(Vector3 a, Vector3 b)
	{
		Vector2 vector = new Vector2(a.x, a.z);
		Vector2 vector2;
		vector2..ctor(b.x, b.z);
		return Vector2.Distance(vector, vector2);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00034044 File Offset: 0x00032244
	private Transform PickRandomPatrolPoint()
	{
		int num;
		do
		{
			num = Random.Range(0, this.patrolPts.Count);
		}
		while (num == this.patrolIdx);
		this.patrolIdx = num;
		return this.patrolPts[num];
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x00034084 File Offset: 0x00032284
	private void PickNewPath(bool pathFinished = false)
	{
		if (this.calculatingPath)
		{
			return;
		}
		this.currentWaypoint = 0;
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Patrolling:
			if (this.patrolCount == this.maxPatrols)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount = 0;
			}
			else
			{
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount++;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Chasing:
			if (!this.lockedOn)
			{
				Vector3 position = base.transform.position;
				VRRig vrrig;
				if (this.ClosestPlayer(position, out vrrig) && vrrig != this.targetRig)
				{
					this.SetTargetPlayer(vrrig);
				}
			}
			if (this.targetRig == null)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.sleepPt.position;
			}
			else
			{
				this.targetPosition = this.targetRig.transform.position;
			}
			break;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
			this.targetPosition = this.sleepPt.position;
			break;
		}
		this.calculatingPath = true;
		this.seeker.StartPath(base.transform.position, this.targetPosition, new OnPathDelegate(this.OnPathComplete));
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x000341D4 File Offset: 0x000323D4
	private void Awake()
	{
		this.lazerFx = base.GetComponent<Monkeye_LazerFX>();
		this.animController = base.GetComponent<Animator>();
		this.layerBase = this.animController.GetLayerIndex("Base_Layer");
		this.layerForward = this.animController.GetLayerIndex("MoveFwdAddPose");
		this.layerLeft = this.animController.GetLayerIndex("TurnLAddPose");
		this.layerRight = this.animController.GetLayerIndex("TurnRAddPose");
		this.seeker = base.GetComponent<Seeker>();
		this.renderer = this.portalFx.GetComponent<Renderer>();
		this.portalMatPropBlock = new MaterialPropertyBlock();
		this.monkEyeMatPropBlock = new MaterialPropertyBlock();
		this.layerMask = (UnityLayer.Default.ToLayerMask() | UnityLayer.GorillaObject.ToLayerMask());
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.replStateRequestableOwnershipGaurd = this.replState.GetComponent<RequestableOwnershipGuard>();
		this.myRequestableOwnershipGaurd = base.GetComponent<RequestableOwnershipGuard>();
		if (this.monkEyeColor.a != 0f || this.monkEyeEyeColorNormal.a != 0f)
		{
			if (this.monkEyeColor.a != 0f)
			{
				this.monkEyeMatPropBlock.SetVector(MonkeyeAI.ColorShaderProp, this.monkEyeColor);
			}
			if (this.monkEyeEyeColorNormal.a != 0f)
			{
				this.monkEyeMatPropBlock.SetVector(MonkeyeAI.EyeColorShaderProp, this.monkEyeEyeColorNormal);
			}
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
		base.InvokeRepeating("AntiOverlapAssurance", 0.2f, 0.5f);
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0003436B File Offset: 0x0003256B
	private void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00034380 File Offset: 0x00032580
	private void OnPathComplete(Path path_)
	{
		this.path = path_;
		this.currentWaypoint = 0;
		if (this.path.vectorPath.Count < 1)
		{
			base.transform.position = this.sleepPt.position;
			base.transform.rotation = this.sleepPt.rotation;
			this.path = null;
		}
		this.calculatingPath = false;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x000343E8 File Offset: 0x000325E8
	private void FollowPath()
	{
		if (this.path == null || this.currentWaypoint >= this.path.vectorPath.Count || this.currentWaypoint < 0)
		{
			this.PickNewPath(false);
			if (this.path == null)
			{
				return;
			}
		}
		if (this.Distance2D(base.transform.position, this.path.vectorPath[this.currentWaypoint]) < 0.01f)
		{
			if (this.currentWaypoint + 1 == this.path.vectorPath.Count)
			{
				this.PickNewPath(true);
				return;
			}
			this.currentWaypoint++;
		}
		Vector3 normalized = (this.path.vectorPath[this.currentWaypoint] - base.transform.position).normalized;
		normalized.y = 0f;
		if (this.animController.GetCurrentAnimatorStateInfo(0).IsName("Move"))
		{
			Vector3 vector = normalized * this.speed;
			base.transform.position += vector * this.deltaTime;
		}
		Mathf.Clamp01(Vector3.Dot(base.transform.forward, normalized) / 1.5707964f);
		if (Mathf.Sign(Vector3.Cross(base.transform.forward, normalized).y) > 0f)
		{
			this.animController.SetLayerWeight(this.layerRight, 0f);
		}
		else
		{
			this.animController.SetLayerWeight(this.layerLeft, 0f);
		}
		this.animController.SetLayerWeight(this.layerForward, 0f);
		Vector3 vector2 = Vector3.RotateTowards(base.transform.forward, normalized, this.rotationSpeed * this.deltaTime, 0f);
		base.transform.rotation = Quaternion.LookRotation(vector2);
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x000345CC File Offset: 0x000327CC
	private bool PlayerNear(VRRig rig, float dist, out float playerDist)
	{
		if (rig == null)
		{
			playerDist = float.PositiveInfinity;
			return false;
		}
		playerDist = this.Distance2D(rig.transform.position, base.transform.position);
		return playerDist < dist && Physics.RaycastNonAlloc(new Ray(base.transform.position, rig.transform.position - base.transform.position), this.rayResults, playerDist, this.layerMask) <= 0;
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0003465C File Offset: 0x0003285C
	private void Sleeping()
	{
		this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
		if (this.audioSource.volume == this.sleepLoopVolume)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			this.PickNewPath(false);
		}
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x000346BC File Offset: 0x000328BC
	private bool ClosestPlayer(in Vector3 myPos, out VRRig outRig)
	{
		float num = float.MaxValue;
		outRig = null;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num2 = 0f;
			if (this.PlayerNear(vrrig, this.chaseDistance, out num2) && num2 < num)
			{
				num = num2;
				outRig = vrrig;
			}
		}
		return num != float.MaxValue;
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x0003473C File Offset: 0x0003293C
	private bool CheckForChase()
	{
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num = 0f;
			if (this.PlayerNear(vrrig, this.wakeDistance, out num))
			{
				this.SetTargetPlayer(vrrig);
				this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
				this.PickNewPath(false);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x000347BC File Offset: 0x000329BC
	public void SetChasePlayer(VRRig rig)
	{
		if (!this.GetValidChoosableRigs().Contains(rig))
		{
			return;
		}
		this.SetTargetPlayer(rig);
		this.lockedOn = true;
		this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
		this.PickNewPath(false);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x000347E9 File Offset: 0x000329E9
	public void SetSleep()
	{
		if (this.replState.state == MonkeyeAI_ReplState.EStates.Patrolling || this.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00034810 File Offset: 0x00032A10
	private void Patrolling()
	{
		this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.audioSource.volume == this.patrolLoopVolume)
		{
			this.CheckForChase();
		}
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00034878 File Offset: 0x00032A78
	private void Chasing()
	{
		this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
		this.PickNewPath(false);
		if (this.targetRig == null)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			return;
		}
		if (this.Distance2D(base.transform.position, this.targetRig.transform.position) < this.attackDistance)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.BeginAttack);
			return;
		}
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x00034904 File Offset: 0x00032B04
	private void ReturnToSleepPt()
	{
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.CheckForChase())
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
			return;
		}
		if (this.Distance2D(base.transform.position, this.sleepPt.position) < 0.01f)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x0003495C File Offset: 0x00032B5C
	private void UpdateClientState()
	{
		if (this.wasConnectedToRoom && !NetworkSystem.Instance.InRoom)
		{
			this.SetDefaultState();
			return;
		}
		if (ColliderEnabledManager.instance != null && !this.replState.floorEnabled)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.replState.userId == "-1")
				{
					ColliderEnabledManager.instance.DisableFloorForFrame();
				}
			}
			else if (this.replState.userId == NetworkSystem.Instance.LocalPlayer.UserId)
			{
				ColliderEnabledManager.instance.DisableFloorForFrame();
			}
		}
		if (this.portalFx.activeSelf != this.replState.portalEnabled)
		{
			this.portalFx.SetActive(this.replState.portalEnabled);
		}
		this.portalFx.transform.position = new Vector3(this.replState.attackPos.x, this.portalFx.transform.position.y, this.replState.attackPos.z);
		this.replState.timer -= this.deltaTime;
		if (this.replState.timer < 0f)
		{
			this.replState.timer = 0f;
		}
		VRRig rig = this.GetRig(this.replState.userId);
		if (this.replState.state >= MonkeyeAI_ReplState.EStates.BeginAttack)
		{
			if (rig == null)
			{
				this.lazerFx.DisableLazer();
			}
			else if (this.replState.state < MonkeyeAI_ReplState.EStates.DropPlayer)
			{
				this.lazerFx.EnableLazer(this.eyeBones, rig);
			}
			else
			{
				this.lazerFx.DisableLazer();
			}
		}
		else
		{
			this.lazerFx.DisableLazer();
		}
		if (this.replState.portalEnabled)
		{
			this.portalColor.a = this.replState.alpha;
			this.portalMatPropBlock.SetVector(MonkeyeAI.tintColorShaderProp, this.portalColor);
			this.renderer.SetPropertyBlock(this.portalMatPropBlock);
		}
		if (GorillaTagger.Instance.offlineVRRig == rig && this.replState.freezePlayer)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			Vector3 linearVelocity = rigidbody.linearVelocity;
			rigidbody.linearVelocity = new Vector3(linearVelocity.x * this.deltaTime * 4f, Mathf.Min(linearVelocity.y, 0f), linearVelocity.x * this.deltaTime * 4f);
		}
		if (!this.replState.IsMine)
		{
			this.SetClientState(this.replState.state);
		}
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x00034C01 File Offset: 0x00032E01
	private void SetDefaultState()
	{
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.SetDefaultAttackState();
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00034C10 File Offset: 0x00032E10
	private void SetDefaultAttackState()
	{
		this.replState.floorEnabled = true;
		this.replState.timer = 0f;
		this.replState.userId = "";
		this.replState.attackPos = base.transform.position;
		this.replState.portalEnabled = false;
		this.replState.freezePlayer = false;
		this.replState.alpha = 0f;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00034C87 File Offset: 0x00032E87
	private void ExitAttackState()
	{
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00034C98 File Offset: 0x00032E98
	private void BeginAttack()
	{
		this.path = null;
		this.replState.freezePlayer = true;
		if (this.replState.timer <= 0f)
		{
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlayOneShot(this.attackSound, this.attackVolume);
			}
			this.replState.timer = this.openFloorTime;
			this.replState.portalEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.OpenFloor);
		}
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00034D14 File Offset: 0x00032F14
	private void OpenFloor()
	{
		this.replState.alpha = Mathf.Lerp(0f, 1f, 1f - Mathf.Clamp01(this.replState.timer / this.openFloorTime));
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = false;
			this.SetState(MonkeyeAI_ReplState.EStates.DropPlayer);
		}
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x00034D8E File Offset: 0x00032F8E
	private void DropPlayer()
	{
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.CloseFloor);
		}
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00034DC6 File Offset: 0x00032FC6
	private void CloseFloor()
	{
		if (this.replState.timer <= 0f)
		{
			this.ExitAttackState();
		}
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00034DE0 File Offset: 0x00032FE0
	private void ValidateChasingRig()
	{
		if (this.targetRig == null)
		{
			this.SetTargetPlayer(null);
			return;
		}
		bool flag = false;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			if (vrrig == this.targetRig)
			{
				flag = true;
				this.SetTargetPlayer(vrrig);
				break;
			}
		}
		if (!flag)
		{
			this.SetTargetPlayer(null);
		}
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00034E68 File Offset: 0x00033068
	public void SetState(MonkeyeAI_ReplState.EStates state_)
	{
		if (this.replState.IsMine)
		{
			this.replState.state = state_;
		}
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.lockedOn = false;
			this.audioSource.clip = this.sleepLoopSound;
			this.audioSource.volume = 0f;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
				return;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.lockedOn = false;
			this.audioSource.clip = this.patrolLoopSound;
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
			}
			this.patrolCount = 0;
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			this.audioSource.clip = this.chaseLoopSound;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
				return;
			}
			break;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
		case MonkeyeAI_ReplState.EStates.GoToSleep:
			break;
		case MonkeyeAI_ReplState.EStates.BeginAttack:
			this.setEyeColor(this.monkEyeEyeColorAttacking);
			if (this.replState.IsMine)
			{
				this.replState.attackPos = ((this.targetRig != null) ? this.targetRig.transform.position : base.transform.position);
				this.replState.timer = this.beginAttackTime;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00035038 File Offset: 0x00033238
	public void SetClientState(MonkeyeAI_ReplState.EStates state_)
	{
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		if (this.previousState != this.replState.state)
		{
			this.previousState = this.replState.state;
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.lockedOn = false;
				this.audioSource.clip = this.sleepLoopSound;
				this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.lockedOn = false;
				this.audioSource.clip = this.patrolLoopSound;
				this.audioSource.loop = true;
				this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				this.patrolCount = 0;
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.audioSource.loop = true;
				this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
				this.audioSource.clip = this.chaseLoopSound;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.setEyeColor(this.monkEyeEyeColorAttacking);
				break;
			}
		}
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
			return;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
			return;
		default:
			return;
		}
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x000352D5 File Offset: 0x000334D5
	private void setEyeColor(Color c)
	{
		if (c.a != 0f)
		{
			this.monkEyeMatPropBlock.SetVector(MonkeyeAI.EyeColorShaderProp, c);
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0003530C File Offset: 0x0003350C
	public List<VRRig> GetValidChoosableRigs()
	{
		this.validRigs.Clear();
		foreach (VRRig vrrig in this.playerCollection.containedRigs)
		{
			if ((NetworkSystem.Instance.InRoom || vrrig.isOfflineVRRig) && !(vrrig == null))
			{
				this.validRigs.Add(vrrig);
			}
		}
		return this.validRigs;
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00035398 File Offset: 0x00033598
	public void SliceUpdate()
	{
		this.wasConnectedToRoom = NetworkSystem.Instance.InRoom;
		this.deltaTime = Time.time - this.lastTime;
		this.lastTime = Time.time;
		this.UpdateClientState();
		if (NetworkSystem.Instance.InRoom && !this.replState.IsMine)
		{
			this.path = null;
			return;
		}
		if (!this.playerCollection.gameObject.activeInHierarchy)
		{
			NetPlayer netPlayer = null;
			float num = float.PositiveInfinity;
			foreach (VRRig vrrig in this.playersInRoomCollection.containedRigs)
			{
				if (!(vrrig == null))
				{
					float num2 = Vector3.Distance(base.transform.position, vrrig.transform.position);
					if (num2 < num)
					{
						netPlayer = vrrig.creator;
						num = num2;
					}
				}
			}
			if (num > 6f)
			{
				return;
			}
			this.path = null;
			if (netPlayer == null)
			{
				return;
			}
			this.replStateRequestableOwnershipGaurd.TransferOwnership(netPlayer, "");
			this.myRequestableOwnershipGaurd.TransferOwnership(netPlayer, "");
			return;
		}
		else
		{
			this.ValidateChasingRig();
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.Sleeping();
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.Patrolling();
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.Chasing();
				break;
			case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
				this.ReturnToSleepPt();
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.BeginAttack();
				break;
			case MonkeyeAI_ReplState.EStates.OpenFloor:
				this.OpenFloor();
				break;
			case MonkeyeAI_ReplState.EStates.DropPlayer:
				this.DropPlayer();
				break;
			case MonkeyeAI_ReplState.EStates.CloseFloor:
				this.CloseFloor();
				break;
			}
			if (this.path == null)
			{
				return;
			}
			this.FollowPath();
			this.velocity = base.transform.position - this.prevPosition;
			this.prevPosition = base.transform.position;
			return;
		}
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00035584 File Offset: 0x00033784
	private void AntiOverlapAssurance()
	{
		try
		{
			if ((!NetworkSystem.Instance.InRoom || this.replState.IsMine) && this.playerCollection.gameObject.activeInHierarchy)
			{
				foreach (MonkeyeAI monkeyeAI in this.playerCollection.monkeyeAis)
				{
					if (!(monkeyeAI == this) && Vector3.Distance(base.transform.position, monkeyeAI.transform.position) < this.overlapRadius && (double)Vector3.Dot(base.transform.forward, monkeyeAI.transform.forward) > 0.2)
					{
						MonkeyeAI_ReplState.EStates state = this.replState.state;
						if (state != MonkeyeAI_ReplState.EStates.Patrolling)
						{
							if (state == MonkeyeAI_ReplState.EStates.Chasing)
							{
								if (monkeyeAI.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
								{
									this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
								}
							}
						}
						else
						{
							this.PickNewPath(false);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
		}
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x000356AC File Offset: 0x000338AC
	private void SetTargetPlayer([CanBeNull] VRRig rig)
	{
		if (rig == null)
		{
			this.replState.userId = "";
			this.replState.freezePlayer = false;
			this.replState.floorEnabled = true;
			this.replState.portalEnabled = false;
			this.targetRig = null;
			return;
		}
		this.replState.userId = this.UserIdFromRig(rig);
		this.targetRig = rig;
	}

	// Token: 0x04000BC5 RID: 3013
	public List<Transform> patrolPts;

	// Token: 0x04000BC6 RID: 3014
	public Transform sleepPt;

	// Token: 0x04000BC7 RID: 3015
	private int patrolIdx = -1;

	// Token: 0x04000BC8 RID: 3016
	private int patrolCount;

	// Token: 0x04000BC9 RID: 3017
	private Vector3 targetPosition;

	// Token: 0x04000BCA RID: 3018
	private MaterialPropertyBlock portalMatPropBlock;

	// Token: 0x04000BCB RID: 3019
	private MaterialPropertyBlock monkEyeMatPropBlock;

	// Token: 0x04000BCC RID: 3020
	private Renderer renderer;

	// Token: 0x04000BCD RID: 3021
	private AIDestinationSetter aiDest;

	// Token: 0x04000BCE RID: 3022
	private AIPath aiPath;

	// Token: 0x04000BCF RID: 3023
	private AILerp aiLerp;

	// Token: 0x04000BD0 RID: 3024
	private Seeker seeker;

	// Token: 0x04000BD1 RID: 3025
	private Path path;

	// Token: 0x04000BD2 RID: 3026
	private int currentWaypoint;

	// Token: 0x04000BD3 RID: 3027
	private bool calculatingPath;

	// Token: 0x04000BD4 RID: 3028
	private Monkeye_LazerFX lazerFx;

	// Token: 0x04000BD5 RID: 3029
	private Animator animController;

	// Token: 0x04000BD6 RID: 3030
	private RaycastHit[] rayResults = new RaycastHit[1];

	// Token: 0x04000BD7 RID: 3031
	private LayerMask layerMask;

	// Token: 0x04000BD8 RID: 3032
	private bool wasConnectedToRoom;

	// Token: 0x04000BD9 RID: 3033
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x04000BDA RID: 3034
	public MazePlayerCollection playerCollection;

	// Token: 0x04000BDB RID: 3035
	public PlayerCollection playersInRoomCollection;

	// Token: 0x04000BDC RID: 3036
	private List<VRRig> validRigs = new List<VRRig>();

	// Token: 0x04000BDD RID: 3037
	public GameObject portalFx;

	// Token: 0x04000BDE RID: 3038
	public Transform[] eyeBones;

	// Token: 0x04000BDF RID: 3039
	public float speed = 0.1f;

	// Token: 0x04000BE0 RID: 3040
	public float rotationSpeed = 1f;

	// Token: 0x04000BE1 RID: 3041
	public float wakeDistance = 1f;

	// Token: 0x04000BE2 RID: 3042
	public float chaseDistance = 3f;

	// Token: 0x04000BE3 RID: 3043
	public float sleepDuration = 3f;

	// Token: 0x04000BE4 RID: 3044
	public float attackDistance = 0.1f;

	// Token: 0x04000BE5 RID: 3045
	public float beginAttackTime = 1f;

	// Token: 0x04000BE6 RID: 3046
	public float openFloorTime = 3f;

	// Token: 0x04000BE7 RID: 3047
	public float dropPlayerTime = 1f;

	// Token: 0x04000BE8 RID: 3048
	public float closeFloorTime = 1f;

	// Token: 0x04000BE9 RID: 3049
	public Color portalColor;

	// Token: 0x04000BEA RID: 3050
	public Color gorillaPortalColor;

	// Token: 0x04000BEB RID: 3051
	public Color monkEyeColor;

	// Token: 0x04000BEC RID: 3052
	public Color monkEyeEyeColorNormal;

	// Token: 0x04000BED RID: 3053
	public Color monkEyeEyeColorAttacking;

	// Token: 0x04000BEE RID: 3054
	public int maxPatrols = 4;

	// Token: 0x04000BEF RID: 3055
	private VRRig targetRig;

	// Token: 0x04000BF0 RID: 3056
	private float deltaTime;

	// Token: 0x04000BF1 RID: 3057
	private float lastTime;

	// Token: 0x04000BF2 RID: 3058
	public MonkeyeAI_ReplState replState;

	// Token: 0x04000BF3 RID: 3059
	private MonkeyeAI_ReplState.EStates previousState;

	// Token: 0x04000BF4 RID: 3060
	private RequestableOwnershipGuard replStateRequestableOwnershipGaurd;

	// Token: 0x04000BF5 RID: 3061
	private RequestableOwnershipGuard myRequestableOwnershipGaurd;

	// Token: 0x04000BF6 RID: 3062
	private int layerBase;

	// Token: 0x04000BF7 RID: 3063
	private int layerForward = 1;

	// Token: 0x04000BF8 RID: 3064
	private int layerLeft = 2;

	// Token: 0x04000BF9 RID: 3065
	private int layerRight = 3;

	// Token: 0x04000BFA RID: 3066
	private static readonly int EmissionColorShaderProp = ShaderProps._EmissionColor;

	// Token: 0x04000BFB RID: 3067
	private static readonly int ColorShaderProp = ShaderProps._BaseColor;

	// Token: 0x04000BFC RID: 3068
	private static readonly int EyeColorShaderProp = ShaderProps._GChannelColor;

	// Token: 0x04000BFD RID: 3069
	private static readonly int tintColorShaderProp = ShaderProps._TintColor;

	// Token: 0x04000BFE RID: 3070
	private static readonly int animStateID = Animator.StringToHash("state");

	// Token: 0x04000BFF RID: 3071
	private Vector3 prevPosition;

	// Token: 0x04000C00 RID: 3072
	private Vector3 velocity;

	// Token: 0x04000C01 RID: 3073
	public AudioSource audioSource;

	// Token: 0x04000C02 RID: 3074
	public AudioClip sleepLoopSound;

	// Token: 0x04000C03 RID: 3075
	public float sleepLoopVolume = 0.5f;

	// Token: 0x04000C04 RID: 3076
	[FormerlySerializedAs("moveLoopSound")]
	public AudioClip patrolLoopSound;

	// Token: 0x04000C05 RID: 3077
	public float patrolLoopVolume = 0.5f;

	// Token: 0x04000C06 RID: 3078
	public float patrolLoopFadeInTime = 1f;

	// Token: 0x04000C07 RID: 3079
	public AudioClip chaseLoopSound;

	// Token: 0x04000C08 RID: 3080
	public float chaseLoopVolume = 0.5f;

	// Token: 0x04000C09 RID: 3081
	public float chaseLoopFadeInTime = 0.05f;

	// Token: 0x04000C0A RID: 3082
	public AudioClip attackSound;

	// Token: 0x04000C0B RID: 3083
	public float attackVolume = 0.5f;

	// Token: 0x04000C0C RID: 3084
	public float overlapRadius;

	// Token: 0x04000C0D RID: 3085
	private bool lockedOn;
}
