using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200095B RID: 2395
public class CustomMapsAIBehaviourController : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x170005B7 RID: 1463
	// (get) Token: 0x06003D45 RID: 15685 RVA: 0x001454C2 File Offset: 0x001436C2
	// (set) Token: 0x06003D44 RID: 15684 RVA: 0x001454B9 File Offset: 0x001436B9
	public GRPlayer TargetPlayer { get; private set; }

	// Token: 0x06003D46 RID: 15686 RVA: 0x001454CC File Offset: 0x001436CC
	private void Awake()
	{
		this.TargetPlayer = null;
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Default",
			"Gorilla Object"
		});
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviourStateChanged;
	}

	// Token: 0x06003D47 RID: 15687 RVA: 0x0014551D File Offset: 0x0014371D
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviourStateChanged;
	}

	// Token: 0x06003D48 RID: 15688 RVA: 0x00145536 File Offset: 0x00143736
	public void SetTarget(GRPlayer newTarget)
	{
		if (newTarget.IsNull())
		{
			this.ClearTarget();
			return;
		}
		this.TargetPlayer = newTarget;
	}

	// Token: 0x06003D49 RID: 15689 RVA: 0x0014554E File Offset: 0x0014374E
	public void ClearTarget()
	{
		this.TargetPlayer = null;
	}

	// Token: 0x06003D4A RID: 15690 RVA: 0x00145557 File Offset: 0x00143757
	private void Update()
	{
		this.OnThink();
		this.UpdateAnimators();
	}

	// Token: 0x06003D4B RID: 15691 RVA: 0x00145568 File Offset: 0x00143768
	private void OnTriggerEnter(Collider collider)
	{
		CustomMapsBehaviourBase customMapsBehaviourBase = this.behaviourDict[this.currentBehaviour];
		if (customMapsBehaviourBase != null)
		{
			customMapsBehaviourBase.OnTriggerEnter(collider);
		}
	}

	// Token: 0x06003D4C RID: 15692 RVA: 0x00145591 File Offset: 0x00143791
	private void InitAnimators()
	{
		this.animators = base.gameObject.GetComponentsInChildren<Animator>();
	}

	// Token: 0x06003D4D RID: 15693 RVA: 0x001455A4 File Offset: 0x001437A4
	private void UpdateAnimators()
	{
		if (this.animators.IsNullOrEmpty<Animator>())
		{
			return;
		}
		float magnitude = this.agent.navAgent.velocity.magnitude;
		for (int i = 0; i < this.animators.Length; i++)
		{
			this.animators[i].SetFloat(CustomMapsAIBehaviourController.movementSpeedParamIndex, magnitude);
		}
	}

	// Token: 0x06003D4E RID: 15694 RVA: 0x00145600 File Offset: 0x00143800
	public void PlayAnimation(string stateName, float blendTime = 0f)
	{
		for (int i = 0; i < this.animators.Length; i++)
		{
			this.animators[i].CrossFadeInFixedTime(stateName, blendTime);
		}
	}

	// Token: 0x06003D4F RID: 15695 RVA: 0x00145630 File Offset: 0x00143830
	public bool IsAnimationPlaying(string stateName)
	{
		int num = 0;
		if (num >= this.animators.Length)
		{
			return false;
		}
		Animator animator = this.animators[num];
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		return (currentAnimatorStateInfo.IsName(stateName) && currentAnimatorStateInfo.normalizedTime < 1f) || animator.GetNextAnimatorStateInfo(0).IsName(stateName);
	}

	// Token: 0x06003D50 RID: 15696 RVA: 0x00145690 File Offset: 0x00143890
	public void SetupBehaviours(AIAgent aiAgent)
	{
		this.allowTargetingTaggedPlayers = aiAgent.allowTargetingTaggedPlayers;
		for (int i = 0; i < aiAgent.agentBehaviours.Count; i++)
		{
			if (!this.usedBehaviours.Contains(aiAgent.agentBehaviours[i]))
			{
				switch (aiAgent.agentBehaviours[i])
				{
				case 0:
					this.behaviourDict[0] = new CustomMapsSearchBehaviour(this, aiAgent);
					break;
				case 1:
					this.behaviourDict[1] = new CustomMapsChaseBehaviour(this, aiAgent);
					break;
				case 2:
					this.behaviourDict[2] = new CustomMapsAttackBehaviour(this, aiAgent);
					break;
				default:
					goto IL_A1;
				}
				this.usedBehaviours.Add(aiAgent.agentBehaviours[i]);
			}
			IL_A1:;
		}
	}

	// Token: 0x06003D51 RID: 15697 RVA: 0x00145753 File Offset: 0x00143953
	public void StopMoving()
	{
		this.RequestDestination(base.transform.position);
	}

	// Token: 0x06003D52 RID: 15698 RVA: 0x00145766 File Offset: 0x00143966
	public void RequestDestination(Vector3 destination)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.agent.RequestDestination(destination);
	}

	// Token: 0x06003D53 RID: 15699 RVA: 0x00145784 File Offset: 0x00143984
	private void OnThink()
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (this.behaviourDict == null || this.behaviourDict.Count == 0)
		{
			return;
		}
		int num = -1;
		if (this.currentBehaviourIndex != -1 && this.behaviourDict[this.usedBehaviours[this.currentBehaviourIndex]].CanContinueExecuting())
		{
			num = this.currentBehaviourIndex;
		}
		else
		{
			for (int i = 0; i < this.usedBehaviours.Count; i++)
			{
				if (i != this.currentBehaviourIndex && this.behaviourDict[this.usedBehaviours[i]].CanExecute())
				{
					num = i;
					break;
				}
			}
		}
		if (num == -1)
		{
			return;
		}
		if (this.currentBehaviourIndex != num)
		{
			this.currentBehaviourIndex = num;
			this.currentBehaviour = this.usedBehaviours[num];
			this.agent.RequestBehaviorChange(this.currentBehaviour);
		}
		this.behaviourDict[this.currentBehaviour].Execute();
	}

	// Token: 0x06003D54 RID: 15700 RVA: 0x0014587C File Offset: 0x00143A7C
	private void OnNetworkBehaviourStateChanged(byte newstate)
	{
		if (newstate < 0 || newstate >= 3)
		{
			return;
		}
		if (!this.behaviourDict.ContainsKey(newstate))
		{
			return;
		}
		if (this.currentBehaviour != newstate && this.behaviourDict.ContainsKey(this.currentBehaviour))
		{
			this.behaviourDict[this.currentBehaviour].ResetBehavior();
		}
		this.currentBehaviour = newstate;
		this.behaviourDict[this.currentBehaviour].NetExecute();
	}

	// Token: 0x06003D55 RID: 15701 RVA: 0x001458F4 File Offset: 0x00143AF4
	public void OnEntityInit()
	{
		bool flag = AISpawnManager.HasInstance && AISpawnManager.instance != null;
		if (!flag && MapSpawnManager.instance == null)
		{
			return;
		}
		this.entity.transform.parent = (flag ? AISpawnManager.instance.transform : MapSpawnManager.instance.transform);
		byte b;
		AIAgent.UnpackCreateData(this.entity.createData, ref b, ref this.luaAgentID);
		AIAgent newEnemy;
		if (flag && AISpawnManager.instance.SpawnEnemy((int)b, ref newEnemy))
		{
			this.SetupNewEnemy(newEnemy);
			return;
		}
		MapEntity mapEntity;
		if (!flag && MapSpawnManager.instance.SpawnEntity((int)b, ref mapEntity))
		{
			this.SetupNewEnemy((AIAgent)mapEntity);
			return;
		}
		GTDev.LogError<string>("CustomMapsAIBehaviourController::OnEntityInit could not spawn enemy", null);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06003D56 RID: 15702 RVA: 0x001459B8 File Offset: 0x00143BB8
	private void SetupNewEnemy(AIAgent newEnemy)
	{
		newEnemy.gameObject.SetActive(true);
		newEnemy.transform.parent = this.entity.transform;
		newEnemy.transform.localPosition = Vector3.zero;
		newEnemy.transform.localRotation = Quaternion.identity;
		this.InitAnimators();
		NavMeshAgent component = this.entity.gameObject.GetComponent<NavMeshAgent>();
		if (component.IsNull())
		{
			GTDev.LogError<string>("nav mesh agent is null", null);
			Object.Destroy(base.gameObject);
			return;
		}
		component.agentTypeID = this.GetNavAgentType(newEnemy.navAgentType);
		component.speed = newEnemy.movementSpeed;
		component.angularSpeed = newEnemy.turnSpeed;
		component.acceleration = newEnemy.acceleration;
		this.SetupBehaviours(newEnemy);
	}

	// Token: 0x06003D57 RID: 15703 RVA: 0x00145A7C File Offset: 0x00143C7C
	private int GetNavAgentType(NavAgentType navType)
	{
		int settingsCount = NavMesh.GetSettingsCount();
		int agentTypeID = NavMesh.GetSettingsByIndex(0).agentTypeID;
		for (int i = 0; i < settingsCount; i++)
		{
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(i);
			if (NavMesh.GetSettingsNameFromID(settingsByIndex.agentTypeID) == navType.ToString())
			{
				agentTypeID = settingsByIndex.agentTypeID;
				break;
			}
		}
		return agentTypeID;
	}

	// Token: 0x06003D58 RID: 15704 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003D59 RID: 15705 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x06003D5A RID: 15706 RVA: 0x00145AE0 File Offset: 0x00143CE0
	public GRPlayer FindBestTarget(Vector3 sourcePos, float maxRange, float maxRangeSq, float minDotVal)
	{
		float num = 0f;
		GRPlayer result = null;
		this.tempRigs.Clear();
		this.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.tempRigs);
		Vector3 vector = base.transform.rotation * Vector3.forward;
		for (int i = 0; i < this.tempRigs.Count; i++)
		{
			GRPlayer component = this.tempRigs[i].GetComponent<GRPlayer>();
			Vector3 vector2;
			if (this.IsTargetInRange(sourcePos, component, maxRangeSq, out vector2))
			{
				float num2 = 0f;
				if (vector2.sqrMagnitude > 0f)
				{
					num2 = Mathf.Sqrt(vector2.magnitude);
				}
				float num3 = Vector3.Dot(vector2.normalized, vector);
				if (num3 >= minDotVal)
				{
					float num4 = Mathf.Lerp(0f, 0.5f, 1f - num2 / maxRange);
					float num5 = Mathf.Lerp(0f, 0.5f, (1f - minDotVal - (1f - num3)) / (1f - minDotVal));
					if (num4 + num5 > num && this.IsTargetVisible(sourcePos, component, maxRange))
					{
						num = num4 + num5;
						result = component;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06003D5B RID: 15707 RVA: 0x00145C14 File Offset: 0x00143E14
	public bool IsTargetVisible(Vector3 startPos, GRPlayer target, float maxDist)
	{
		if (!this.IsTargetable(target))
		{
			return false;
		}
		int num = Physics.RaycastNonAlloc(new Ray(startPos, target.transform.position - startPos), CustomMapsAIBehaviourController.visibilityHits, Mathf.Min(Vector3.Distance(target.transform.position, startPos), maxDist), this.visibilityLayerMask.value, 1);
		for (int i = 0; i < num; i++)
		{
			if (CustomMapsAIBehaviourController.visibilityHits[i].transform != base.transform && !CustomMapsAIBehaviourController.visibilityHits[i].transform.IsChildOf(base.transform))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003D5C RID: 15708 RVA: 0x00145CBC File Offset: 0x00143EBC
	public bool IsTargetInRange(Vector3 startPos, GRPlayer target, float maxRangeSq, out Vector3 toTarget)
	{
		toTarget = Vector3.zero;
		if (!this.IsTargetable(target))
		{
			return false;
		}
		Vector3 position = target.transform.position;
		toTarget = position - startPos;
		return toTarget.sqrMagnitude <= maxRangeSq;
	}

	// Token: 0x06003D5D RID: 15709 RVA: 0x00145D08 File Offset: 0x00143F08
	public bool IsTargetable(GRPlayer potentialTarget)
	{
		if (potentialTarget.IsNull())
		{
			return false;
		}
		if (potentialTarget.State == GRPlayer.GRPlayerState.Ghost)
		{
			return false;
		}
		if (potentialTarget.MyRig.isLocal)
		{
			if (CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				return false;
			}
		}
		else if (CustomMapManager.IsRemotePlayerInVirtualStump(potentialTarget.MyRig.OwningNetPlayer.UserId))
		{
			return false;
		}
		return this.allowTargetingTaggedPlayers || GameMode.ActiveGameMode.GameType() == GameModeType.Custom || !GameMode.LocalIsTagged(potentialTarget.MyRig.OwningNetPlayer);
	}

	// Token: 0x04004E0B RID: 19979
	private static readonly int movementSpeedParamIndex = Animator.StringToHash("MovementSpeed");

	// Token: 0x04004E0C RID: 19980
	public GameEntity entity;

	// Token: 0x04004E0D RID: 19981
	public GameAgent agent;

	// Token: 0x04004E0E RID: 19982
	public GRAttributes attributes;

	// Token: 0x04004E0F RID: 19983
	private Animator[] animators;

	// Token: 0x04004E10 RID: 19984
	public short luaAgentID;

	// Token: 0x04004E11 RID: 19985
	private List<VRRig> tempRigs = new List<VRRig>(10);

	// Token: 0x04004E12 RID: 19986
	private static RaycastHit[] visibilityHits = new RaycastHit[10];

	// Token: 0x04004E13 RID: 19987
	private LayerMask visibilityLayerMask;

	// Token: 0x04004E14 RID: 19988
	private bool allowTargetingTaggedPlayers;

	// Token: 0x04004E16 RID: 19990
	private Dictionary<AgentBehaviours, CustomMapsBehaviourBase> behaviourDict = new Dictionary<AgentBehaviours, CustomMapsBehaviourBase>(8);

	// Token: 0x04004E17 RID: 19991
	private List<AgentBehaviours> usedBehaviours = new List<AgentBehaviours>(8);

	// Token: 0x04004E18 RID: 19992
	private AgentBehaviours currentBehaviour;

	// Token: 0x04004E19 RID: 19993
	private int currentBehaviourIndex;

	// Token: 0x04004E1A RID: 19994
	private const int BEHAVIOUR_COUNT = 3;

	// Token: 0x0200095C RID: 2396
	public enum CustomMapsAIBehaviour
	{
		// Token: 0x04004E1C RID: 19996
		Search,
		// Token: 0x04004E1D RID: 19997
		Chase,
		// Token: 0x04004E1E RID: 19998
		Attack
	}
}
