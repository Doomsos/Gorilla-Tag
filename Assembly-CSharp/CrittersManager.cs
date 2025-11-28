using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Critters.Scripts;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using UnityEngine;
using Utilities;

// Token: 0x0200005E RID: 94
[NetworkBehaviourWeaved(0)]
public class CrittersManager : NetworkComponent, IRequestableOwnershipGuardCallbacks, IBuildValidation, ITickSystemTick
{
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060001CD RID: 461 RVA: 0x0000AF74 File Offset: 0x00009174
	// (set) Token: 0x060001CE RID: 462 RVA: 0x0000AF7B File Offset: 0x0000917B
	public static bool hasInstance { get; private set; }

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060001CF RID: 463 RVA: 0x0000AF83 File Offset: 0x00009183
	// (set) Token: 0x060001D0 RID: 464 RVA: 0x0000AF8B File Offset: 0x0000918B
	public bool TickRunning { get; set; }

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060001D1 RID: 465 RVA: 0x0000AF94 File Offset: 0x00009194
	public bool allowGrabbingEntireBag
	{
		get
		{
			if (!NetworkSystem.Instance.SessionIsPrivate)
			{
				return (CrittersManager.AllowGrabbingFlags.EntireBag & this.publicRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
			}
			return (CrittersManager.AllowGrabbingFlags.EntireBag & this.privateRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000AFB9 File Offset: 0x000091B9
	public bool allowGrabbingOutOfHands
	{
		get
		{
			if (!NetworkSystem.Instance.SessionIsPrivate)
			{
				return (CrittersManager.AllowGrabbingFlags.OutOfHands & this.publicRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
			}
			return (CrittersManager.AllowGrabbingFlags.OutOfHands & this.privateRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060001D3 RID: 467 RVA: 0x0000AFDE File Offset: 0x000091DE
	public bool allowGrabbingFromBags
	{
		get
		{
			if (!NetworkSystem.Instance.SessionIsPrivate)
			{
				return (CrittersManager.AllowGrabbingFlags.FromBags & this.publicRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
			}
			return (CrittersManager.AllowGrabbingFlags.FromBags & this.privateRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
		}
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000B004 File Offset: 0x00009204
	public void LoadGrabSettings()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("PublicCrittersGrabSettings", delegate(string data)
		{
			int num;
			if (int.TryParse(data, ref num))
			{
				this.publicRoomGrabbingFlags = (CrittersManager.AllowGrabbingFlags)num;
			}
		}, delegate(PlayFabError e)
		{
		}, false);
		PlayFabTitleDataCache.Instance.GetTitleData("PrivateCrittersGrabSettings", delegate(string data)
		{
			int num;
			if (int.TryParse(data, ref num))
			{
				this.privateRoomGrabbingFlags = (CrittersManager.AllowGrabbingFlags)num;
			}
		}, delegate(PlayFabError e)
		{
		}, false);
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060001D5 RID: 469 RVA: 0x0000B087 File Offset: 0x00009287
	public bool LocalInZone
	{
		get
		{
			return this.localInZone;
		}
	}

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060001D6 RID: 470 RVA: 0x0000B090 File Offset: 0x00009290
	// (remove) Token: 0x060001D7 RID: 471 RVA: 0x0000B0C8 File Offset: 0x000092C8
	public event Action<CrittersManager.CritterEvent, int, Vector3, Quaternion> OnCritterEventReceived;

	// Token: 0x060001D8 RID: 472 RVA: 0x0000B100 File Offset: 0x00009300
	public bool BuildValidationCheck()
	{
		if (this.guard == null)
		{
			Debug.LogError("requestable owner guard missing", base.gameObject);
			return false;
		}
		if (this.crittersPool == null)
		{
			Debug.LogError("critters pool missing", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0000B14E File Offset: 0x0000934E
	protected override void Start()
	{
		base.Start();
		CrittersManager.instance.LoadGrabSettings();
		CrittersManager.CheckInitialize();
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000B168 File Offset: 0x00009368
	public static void InitializeCrittersManager()
	{
		if (CrittersManager.hasInstance)
		{
			return;
		}
		CrittersManager.hasInstance = true;
		CrittersManager.instance = Object.FindAnyObjectByType<CrittersManager>();
		CrittersManager.instance.crittersActors = new List<CrittersActor>();
		CrittersManager.instance.crittersPawns = new List<CrittersPawn>();
		CrittersManager.instance.awareOfActors = new Dictionary<CrittersPawn, List<CrittersActor>>();
		CrittersManager.instance.despawnableActors = new List<CrittersActor>();
		CrittersManager.instance.newlyDisabledActors = new List<CrittersActor>();
		CrittersManager.instance.rigActorSetups = new List<CrittersRigActorSetup>();
		CrittersManager.instance.rigSetupByRig = new Dictionary<VRRig, CrittersRigActorSetup>();
		CrittersManager.instance.updatesToSend = new List<int>();
		CrittersManager.instance.objList = new List<object>();
		CrittersManager.instance.lowPriorityPawnsToProcess = new List<CrittersActor>();
		CrittersManager.instance.actorSpawners = Enumerable.ToList<CrittersActorSpawner>(Object.FindObjectsByType<CrittersActorSpawner>(0));
		CrittersManager.instance._spawnRegions = CrittersRegion.Regions;
		CrittersManager.instance.poolCounts = new Dictionary<CrittersActor.CrittersActorType, int>();
		CrittersManager.instance.despawnDecayValue = new Dictionary<CrittersActor.CrittersActorType, float>();
		CrittersManager.instance.actorTypes = (CrittersActor.CrittersActorType[])Enum.GetValues(typeof(CrittersActor.CrittersActorType));
		CrittersManager.instance.poolIndexDict = new Dictionary<CrittersActor.CrittersActorType, int>();
		for (int i = 0; i < CrittersManager.instance.actorTypes.Length; i++)
		{
			CrittersManager.instance.poolCounts[CrittersManager.instance.actorTypes[i]] = 0;
			CrittersManager.instance.despawnDecayValue[CrittersManager.instance.actorTypes[i]] = 0f;
		}
		CrittersManager.instance.PopulatePools();
		List<CrittersRigActorSetup> list = Enumerable.ToList<CrittersRigActorSetup>(Object.FindObjectsByType<CrittersRigActorSetup>(0));
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].enabled)
			{
				CrittersManager.RegisterRigActorSetup(list[j]);
			}
		}
		CrittersActorGrabber[] array = Object.FindObjectsByType<CrittersActorGrabber>(0);
		for (int k = 0; k < array.Length; k++)
		{
			if (array[k].isLeft)
			{
				CrittersManager._leftGrabber = array[k];
			}
			else
			{
				CrittersManager._rightGrabber = array[k];
			}
		}
		if (CrittersManager.instance.guard.IsNotNull())
		{
			CrittersManager.instance.guard.AddCallbackTarget(CrittersManager.instance);
		}
		RoomSystem.JoinedRoomEvent += new Action(CrittersManager.instance.JoinedRoomEvent);
		RoomSystem.LeftRoomEvent += new Action(CrittersManager.instance.LeftRoomEvent);
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000B3F9 File Offset: 0x000095F9
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000B40D File Offset: 0x0000960D
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060001DD RID: 477 RVA: 0x0000B424 File Offset: 0x00009624
	private void ResetRoom()
	{
		this.lastSpawnTime = 0.0;
		for (int i = 0; i < this.allActors.Count; i++)
		{
			CrittersActor crittersActor = this.allActors[i];
			if (crittersActor.gameObject.activeSelf)
			{
				if (this.persistentActors.Contains(this.allActors[i]))
				{
					this.allActors[i].Initialize();
				}
				else
				{
					crittersActor.gameObject.SetActive(false);
				}
			}
		}
		for (int j = 0; j < this.actorSpawners.Count; j++)
		{
			this.actorSpawners[j].DoReset();
		}
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000B4CF File Offset: 0x000096CF
	public void Tick()
	{
		this.HandleZonesAndOwnership();
		if (this.localInZone)
		{
			this.ProcessSpawning();
			this.ProcessActorBinLocations();
			this.ProcessRigSetups();
			this.ProcessCritterAwareness();
			this.ProcessDespawningIdles();
			this.ProcessActors();
		}
		this.ProcessNewlyDisabledActors();
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000B50C File Offset: 0x0000970C
	public void ProcessRigSetups()
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		this.objList.Clear();
		for (int i = 0; i < this.rigActorSetups.Count; i++)
		{
			this.rigActorSetups[i].CheckUpdate(ref this.objList, false);
		}
		if (this.objList.Count > 0 && NetworkSystem.Instance.InRoom)
		{
			CrittersManager.instance.SendRPC("RemoteUpdatePlayerCrittersActorData", 1, new object[]
			{
				this.objList.ToArray()
			});
		}
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000B59C File Offset: 0x0000979C
	private void ProcessCritterAwareness()
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		int num = 0;
		this.lowPriorityPawnsToProcess.Clear();
		int i = 0;
		while (i < this.crittersPawns.Count)
		{
			CrittersPawn crittersPawn = this.crittersPawns[i];
			if (!this.awareOfActors.ContainsKey(crittersPawn))
			{
				this.awareOfActors[crittersPawn] = new List<CrittersActor>();
			}
			else
			{
				this.awareOfActors[crittersPawn].Clear();
			}
			this.nearbyActors.Clear();
			int num2 = this.actorBinIndices[crittersPawn];
			if (this.priorityBins[num2])
			{
				goto IL_D9;
			}
			if (i >= this.lowPriorityIndex && num < this.lowPriorityActorsPerFrame)
			{
				this.lowPriorityPawnsToProcess.Add(this.crittersPawns[i]);
				num++;
				this.lowPriorityIndex++;
				if (this.lowPriorityIndex >= this.crittersPawns.Count)
				{
					this.lowPriorityIndex = 0;
					goto IL_D9;
				}
				goto IL_D9;
			}
			IL_1C4:
			i++;
			continue;
			IL_D9:
			int num3 = Mathf.FloorToInt((float)(num2 / this.binXCount));
			int num4 = num2 % this.binXCount;
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					if (num3 + j < this.binXCount && num3 + j >= 0 && num4 + k < this.binZCount && num4 + k >= 0)
					{
						this.nearbyActors.AddRange(this.actorBins[num4 + k + (num3 + j) * this.binXCount]);
					}
				}
			}
			for (int l = 0; l < this.nearbyActors.Count; l++)
			{
				if (this.crittersPawns[i].AwareOfActor(this.nearbyActors[l]))
				{
					this.awareOfActors[this.crittersPawns[i]].Add(this.nearbyActors[l]);
				}
			}
			goto IL_1C4;
		}
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000B784 File Offset: 0x00009984
	private void ProcessSpawning()
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		if (this.lastSpawnTime + this.spawnDelay <= (NetworkSystem.Instance.InRoom ? PhotonNetwork.Time : ((double)Time.time)))
		{
			int nextSpawnRegion = this.GetNextSpawnRegion();
			if (nextSpawnRegion >= 0)
			{
				this.SpawnCritter(nextSpawnRegion);
			}
			else
			{
				this.lastSpawnTime = (NetworkSystem.Instance.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			}
		}
		if (this.spawnerIndex >= this.actorSpawners.Count)
		{
			this.spawnerIndex = 0;
		}
		if (this.actorSpawners.Count == 0)
		{
			return;
		}
		this.actorSpawners[this.spawnerIndex].ProcessLocal();
		this.spawnerIndex++;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000B844 File Offset: 0x00009A44
	private int GetNextSpawnRegion()
	{
		for (int i = 1; i <= this._spawnRegions.Count; i++)
		{
			int num = (this._currentRegionIndex + i) % this._spawnRegions.Count;
			CrittersRegion crittersRegion = this._spawnRegions[num];
			if (crittersRegion.CritterCount < crittersRegion.maxCritters)
			{
				this._currentRegionIndex = num;
				return this._currentRegionIndex;
			}
		}
		return -1;
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000B8A8 File Offset: 0x00009AA8
	private void ProcessActorBinLocations()
	{
		if (this.LocalAuthority())
		{
			for (int i = 0; i < this.actorBins.Length; i++)
			{
				this.actorBins[i].Clear();
				this.priorityBins[i] = false;
			}
			for (int j = this.crittersActors.Count - 1; j >= 0; j--)
			{
				CrittersActor crittersActor = this.crittersActors[j];
				if (crittersActor == null)
				{
					this.crittersActors.RemoveAt(j);
				}
				else
				{
					Transform transform = crittersActor.transform;
					int num = Mathf.Clamp(Mathf.FloorToInt((transform.position.x - this.binDimensionXMin) / this.individualBinSide), 0, this.binXCount - 1);
					int num2 = Mathf.Clamp(Mathf.FloorToInt((transform.position.z - this.binDimensionZMin) / this.individualBinSide), 0, this.binZCount - 1);
					int num3 = num + num2 * this.binXCount;
					if (this.actorBinIndices.ContainsKey(crittersActor))
					{
						this.actorBinIndices[crittersActor] = num3;
					}
					else
					{
						this.actorBinIndices.Add(crittersActor, num3);
					}
					this.actorBins[num3].Add(crittersActor);
				}
			}
			for (int k = 0; k < RoomSystem.PlayersInRoom.Count; k++)
			{
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[k], out rigContainer))
				{
					Transform transform2 = rigContainer.Rig.transform;
					float num4 = (transform2.position.x - this.binDimensionXMin) / this.individualBinSide;
					float num5 = (transform2.position.z - this.binDimensionZMin) / this.individualBinSide;
					int num6 = Mathf.FloorToInt(num4);
					int num7 = Mathf.FloorToInt(num5);
					int num8 = (num4 % 1f > 0.5f) ? 1 : -1;
					int num9 = (num5 % 1f > 0.5f) ? 1 : -1;
					if (num6 < 0 || num6 >= this.binXCount || num7 < 0 || num7 >= this.binZCount)
					{
						return;
					}
					int num10 = num6 + num7 * this.binXCount;
					this.priorityBins[num10] = true;
					num8 = Mathf.Clamp(num6 + num8, 0, this.binXCount - 1);
					num9 = Mathf.Clamp(num7 + num9, 0, this.binZCount - 1);
					this.priorityBins[num8 + num7 * this.binXCount] = true;
					this.priorityBins[num6 + num9 * this.binXCount] = true;
					this.priorityBins[num8 + num9 * this.binXCount] = true;
				}
			}
		}
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000BB28 File Offset: 0x00009D28
	private void ProcessDespawningIdles()
	{
		for (int i = 0; i < this.actorTypes.Length; i++)
		{
			this.despawnDecayValue[this.actorTypes[i]] = Mathf.Lerp(this.despawnDecayValue[this.actorTypes[i]], (float)this.despawnThreshold, 1f - Mathf.Exp(-this.decayRate * (Time.realtimeSinceStartup - Time.deltaTime)));
		}
		if (!this.LocalAuthority())
		{
			return;
		}
		if (this.despawnableActors.Count == 0)
		{
			return;
		}
		int j = 0;
		while (j <= this.lowPriorityActorsPerFrame)
		{
			this.despawnIndex++;
			if (this.despawnIndex >= this.despawnableActors.Count)
			{
				this.despawnIndex = 0;
			}
			j++;
			CrittersActor crittersActor = this.despawnableActors[this.despawnIndex];
			if (this.despawnDecayValue[crittersActor.crittersActorType] >= (float)this.despawnThreshold && crittersActor.ShouldDespawn())
			{
				this.DespawnActor(crittersActor);
			}
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0000BC24 File Offset: 0x00009E24
	public void DespawnActor(CrittersActor actor)
	{
		int actorId = actor.actorId;
		if (!this.updatesToSend.Contains(actorId))
		{
			this.updatesToSend.Add(actorId);
		}
		actor.gameObject.SetActive(false);
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000BC60 File Offset: 0x00009E60
	public void IncrementPoolCount(CrittersActor.CrittersActorType type)
	{
		int num;
		if (!this.poolCounts.TryGetValue(type, ref num))
		{
			this.poolCounts[type] = 1;
		}
		else
		{
			this.poolCounts[type] = this.poolCounts[type] + 1;
		}
		float num2;
		if (!this.despawnDecayValue.TryGetValue(type, ref num2))
		{
			this.despawnDecayValue[type] = 1f;
			return;
		}
		this.despawnDecayValue[type] = this.despawnDecayValue[type] + 1f;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0000BCE8 File Offset: 0x00009EE8
	public void DecrementPoolCount(CrittersActor.CrittersActorType type)
	{
		int num;
		if (this.poolCounts.TryGetValue(type, ref num))
		{
			this.poolCounts[type] = Mathf.Max(0, num - 1);
			return;
		}
		this.poolCounts[type] = 0;
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000BD28 File Offset: 0x00009F28
	private void ProcessActors()
	{
		if (this.LocalAuthority())
		{
			for (int i = this.crittersActors.Count - 1; i >= 0; i--)
			{
				if (this.crittersActors[i].crittersActorType != CrittersActor.CrittersActorType.Creature || this.priorityBins[this.actorBinIndices[this.crittersActors[i]]] || this.lowPriorityPawnsToProcess.Contains(this.crittersActors[i]))
				{
					int actorId = this.crittersActors[i].actorId;
					if (this.crittersActors[i].ProcessLocal() && !this.updatesToSend.Contains(actorId))
					{
						this.updatesToSend.Add(actorId);
					}
				}
			}
			return;
		}
		for (int j = 0; j < this.crittersActors.Count; j++)
		{
			this.crittersActors[j].ProcessRemote();
		}
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000BE14 File Offset: 0x0000A014
	private void ProcessNewlyDisabledActors()
	{
		for (int i = 0; i < this.newlyDisabledActors.Count; i++)
		{
			CrittersActor crittersActor = this.newlyDisabledActors[i];
			if (CrittersManager.instance.crittersActors.Contains(crittersActor))
			{
				CrittersManager.instance.crittersActors.Remove(crittersActor);
			}
			if (crittersActor.despawnWhenIdle && CrittersManager.instance.despawnableActors.Contains(crittersActor))
			{
				CrittersManager.instance.despawnableActors.Remove(crittersActor);
			}
			CrittersManager.instance.DecrementPoolCount(crittersActor.crittersActorType);
			crittersActor.SetTransformToDefaultParent(true);
		}
		this.newlyDisabledActors.Clear();
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000BEC4 File Offset: 0x0000A0C4
	public static void RegisterCritter(CrittersPawn crittersPawn)
	{
		CrittersManager.CheckInitialize();
		if (!CrittersManager.instance.crittersPawns.Contains(crittersPawn))
		{
			CrittersManager.instance.crittersPawns.Add(crittersPawn);
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000BEF4 File Offset: 0x0000A0F4
	public static void RegisterRigActorSetup(CrittersRigActorSetup setup)
	{
		CrittersManager.CheckInitialize();
		if (!CrittersManager.instance.rigActorSetups.Contains(setup))
		{
			CrittersManager.instance.rigActorSetups.Add(setup);
		}
		CrittersManager.instance.rigSetupByRig.AddOrUpdate(setup.myRig, setup);
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000BF44 File Offset: 0x0000A144
	public static void DeregisterCritter(CrittersPawn crittersPawn)
	{
		CrittersManager.CheckInitialize();
		CrittersManager.instance.SetCritterRegion(crittersPawn, 0);
		if (CrittersManager.instance.crittersPawns.Contains(crittersPawn))
		{
			CrittersManager.instance.crittersPawns.Remove(crittersPawn);
		}
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0000BF80 File Offset: 0x0000A180
	public static void RegisterActor(CrittersActor actor)
	{
		CrittersManager.CheckInitialize();
		if (!CrittersManager.instance.crittersActors.Contains(actor))
		{
			CrittersManager.instance.crittersActors.Add(actor);
		}
		if (actor.despawnWhenIdle && !CrittersManager.instance.despawnableActors.Contains(actor))
		{
			CrittersManager.instance.despawnableActors.Add(actor);
		}
		if (CrittersManager.instance.newlyDisabledActors.Contains(actor))
		{
			CrittersManager.instance.newlyDisabledActors.Remove(actor);
		}
		CrittersManager.instance.IncrementPoolCount(actor.crittersActorType);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000C01F File Offset: 0x0000A21F
	public static void DeregisterActor(CrittersActor actor)
	{
		CrittersManager.CheckInitialize();
		if (!CrittersManager.instance.newlyDisabledActors.Contains(actor))
		{
			CrittersManager.instance.newlyDisabledActors.Add(actor);
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000C04C File Offset: 0x0000A24C
	public static void CheckInitialize()
	{
		if (!CrittersManager.hasInstance)
		{
			CrittersManager.InitializeCrittersManager();
		}
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000C05A File Offset: 0x0000A25A
	public static bool CritterAwareOfAny(CrittersPawn creature)
	{
		return CrittersManager.instance.awareOfActors[creature].Count > 0;
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000C078 File Offset: 0x0000A278
	public static bool AnyFoodNearby(CrittersPawn creature)
	{
		List<CrittersActor> list = CrittersManager.instance.awareOfActors[creature];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].crittersActorType == CrittersActor.CrittersActorType.Food)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000C0BC File Offset: 0x0000A2BC
	public static CrittersFood ClosestFood(CrittersPawn creature)
	{
		float num = float.MaxValue;
		CrittersFood result = null;
		List<CrittersActor> list = CrittersManager.instance.awareOfActors[creature];
		for (int i = 0; i < list.Count; i++)
		{
			CrittersActor crittersActor = list[i];
			if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.Food)
			{
				CrittersFood crittersFood = (CrittersFood)crittersActor;
				float sqrMagnitude = (creature.transform.position - crittersFood.food.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = crittersFood;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000C14B File Offset: 0x0000A34B
	public static void PlayHaptics(AudioClip clip, float strength, bool isLeftHand)
	{
		(isLeftHand ? CrittersManager._leftGrabber : CrittersManager._rightGrabber).PlayHaptics(clip, strength);
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000C163 File Offset: 0x0000A363
	public static void StopHaptics(bool isLeftHand)
	{
		(isLeftHand ? CrittersManager._leftGrabber : CrittersManager._rightGrabber).StopHaptics();
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000C17C File Offset: 0x0000A37C
	public CrittersPawn SpawnCritter(int regionIndex = -1)
	{
		CrittersRegion crittersRegion = (regionIndex >= 0 && regionIndex < this._spawnRegions.Count) ? this._spawnRegions[regionIndex] : null;
		int randomCritterType = this.creatureIndex.GetRandomCritterType(crittersRegion);
		if (randomCritterType < 0)
		{
			return null;
		}
		Vector3 position = crittersRegion ? crittersRegion.GetSpawnPoint() : this._spawnRegions[0].transform.position;
		Quaternion rotation = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
		CrittersPawn crittersPawn = this.SpawnCritter(randomCritterType, position, rotation);
		this.SetCritterRegion(crittersPawn, crittersRegion);
		this.lastSpawnTime = (NetworkSystem.Instance.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		return crittersPawn;
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000C238 File Offset: 0x0000A438
	public CrittersPawn SpawnCritter(int critterType, Vector3 position, Quaternion rotation)
	{
		CrittersPawn crittersPawn = (CrittersPawn)this.SpawnActor(CrittersActor.CrittersActorType.Creature, -1);
		crittersPawn.SetTemplate(critterType);
		crittersPawn.currentState = CrittersPawn.CreatureState.Idle;
		crittersPawn.MoveActor(position, Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f), false, true, true);
		crittersPawn.SetImpulseVelocity(Vector3.zero, Vector3.zero);
		crittersPawn.SetState(CrittersPawn.CreatureState.Spawning);
		if (NetworkSystem.Instance.InRoom && this.LocalAuthority())
		{
			base.SendRPC("RemoteSpawnCreature", 1, new object[]
			{
				crittersPawn.actorId,
				crittersPawn.regionId,
				crittersPawn.visuals.Appearance.WriteToRPCData()
			});
		}
		return crittersPawn;
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000C2F7 File Offset: 0x0000A4F7
	public void DespawnCritter(CrittersPawn crittersPawn)
	{
		this.DeactivateActor(crittersPawn);
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000C300 File Offset: 0x0000A500
	public void QueueDespawnAllCritters()
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		foreach (CrittersPawn crittersPawn in this.crittersPawns)
		{
			crittersPawn.SetState(CrittersPawn.CreatureState.Despawning);
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000C35C File Offset: 0x0000A55C
	private void SetCritterRegion(CrittersPawn critter, CrittersRegion region)
	{
		this.SetCritterRegion(critter, region ? region.ID : 0);
	}

	// Token: 0x060001FA RID: 506 RVA: 0x0000C376 File Offset: 0x0000A576
	private void SetCritterRegion(CrittersPawn critter, int regionId)
	{
		if (critter.regionId != 0)
		{
			CrittersRegion.RemoveCritterFromRegion(critter);
		}
		if (regionId != 0)
		{
			CrittersRegion.AddCritterToRegion(critter, regionId);
		}
		critter.regionId = regionId;
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000C397 File Offset: 0x0000A597
	public void DeactivateActor(CrittersActor actor)
	{
		actor.gameObject.SetActive(false);
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000C3A8 File Offset: 0x0000A5A8
	private void CamCapture()
	{
		Camera component = base.GetComponent<Camera>();
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = component.targetTexture;
		component.Render();
		Texture2D texture2D = new Texture2D(component.targetTexture.width, component.targetTexture.height);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)component.targetTexture.width, (float)component.targetTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		ImageConversion.EncodeToPNG(texture2D);
		Object.Destroy(texture2D);
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000C435 File Offset: 0x0000A635
	private IEnumerator RemoteDataInitialization(NetPlayer player, int actorNumber)
	{
		List<object> nonPlayerActorObjList = new List<object>();
		List<object> playerActorObjList = new List<object>();
		int worldActorDataCount = 0;
		int playerActorDataCount = 0;
		int num;
		for (int i = 0; i < this.allActors.Count; i = num + 1)
		{
			if (!NetworkSystem.Instance.InRoom || !this.LocalAuthority())
			{
				this.RemoveInitializingPlayer(actorNumber);
				yield break;
			}
			if (this.allActors[i].isOnPlayer)
			{
				num = playerActorDataCount;
				playerActorDataCount = num + 1;
				this.allActors[i].AddPlayerCrittersActorDataToList(ref playerActorObjList);
			}
			if (playerActorDataCount >= this.actorsPerInitializationCall || (i == this.allActors.Count - 1 && playerActorDataCount > 0))
			{
				if (!player.InRoom || player.ActorNumber != actorNumber)
				{
					this.RemoveInitializingPlayer(actorNumber);
					yield break;
				}
				if (NetworkSystem.Instance.InRoom && this.LocalAuthority())
				{
					base.SendRPC("RemoteUpdatePlayerCrittersActorData", player, new object[]
					{
						playerActorObjList.ToArray()
					});
				}
				playerActorObjList.Clear();
				playerActorDataCount = 0;
				yield return new WaitForSeconds(this.actorsInitializationCallCooldown);
			}
			num = i;
		}
		if (!player.InRoom || player.ActorNumber != actorNumber)
		{
			this.RemoveInitializingPlayer(actorNumber);
			yield break;
		}
		if (NetworkSystem.Instance.InRoom && this.LocalAuthority() && playerActorDataCount > 0)
		{
			base.SendRPC("RemoteUpdatePlayerCrittersActorData", player, new object[]
			{
				playerActorObjList.ToArray()
			});
		}
		for (int i = 0; i < this.allActors.Count; i = num + 1)
		{
			if (!player.InRoom || player.ActorNumber != actorNumber)
			{
				this.RemoveInitializingPlayer(actorNumber);
				yield break;
			}
			if (!NetworkSystem.Instance.InRoom || !this.LocalAuthority())
			{
				this.RemoveInitializingPlayer(actorNumber);
				yield break;
			}
			CrittersActor crittersActor = this.allActors[i];
			if (crittersActor.gameObject.activeSelf)
			{
				num = worldActorDataCount;
				worldActorDataCount = num + 1;
				if (crittersActor.parentActorId == -1)
				{
					crittersActor.UpdateImpulses(false, false);
					crittersActor.UpdateImpulseVelocity();
				}
				crittersActor.AddActorDataToList(ref nonPlayerActorObjList);
				if (worldActorDataCount >= this.actorsPerInitializationCall)
				{
					if (!player.InRoom || player.ActorNumber != actorNumber)
					{
						this.RemoveInitializingPlayer(actorNumber);
						yield break;
					}
					if (!NetworkSystem.Instance.InRoom || !this.LocalAuthority())
					{
						this.RemoveInitializingPlayer(actorNumber);
						yield break;
					}
					base.SendRPC("RemoteUpdateCritterData", player, new object[]
					{
						nonPlayerActorObjList.ToArray()
					});
					nonPlayerActorObjList.Clear();
					worldActorDataCount = 0;
					yield return new WaitForSeconds(this.actorsInitializationCallCooldown);
				}
			}
			num = i;
		}
		if (NetworkSystem.Instance.InRoom && this.LocalAuthority() && worldActorDataCount > 0)
		{
			base.SendRPC("RemoteUpdateCritterData", player, new object[]
			{
				nonPlayerActorObjList.ToArray()
			});
		}
		this.RemoveInitializingPlayer(actorNumber);
		yield break;
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000C452 File Offset: 0x0000A652
	private IEnumerator DelayedInitialization(NetPlayer player, List<object> nonPlayerActorObjList)
	{
		yield return new WaitForSeconds(30f);
		base.SendRPC("RemoteUpdateCritterData", player, new object[]
		{
			nonPlayerActorObjList.ToArray()
		});
		yield break;
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000C46F File Offset: 0x0000A66F
	public void RemoveInitializingPlayer(int actorNumber)
	{
		if (this.updatingPlayers.Contains(actorNumber))
		{
			this.updatingPlayers.Remove(actorNumber);
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000C48C File Offset: 0x0000A68C
	private void JoinedRoomEvent()
	{
		if (this.localInZone && !this.LocalAuthority())
		{
			this.ResetRoom();
		}
		this.hasNewlyInitialized = false;
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000C4AB File Offset: 0x0000A6AB
	private void LeftRoomEvent()
	{
		this.guard.TransferOwnership(NetworkSystem.Instance.LocalPlayer, "");
		if (this.LocalInZone)
		{
			this.ResetRoom();
		}
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000C4D8 File Offset: 0x0000A6D8
	[PunRPC]
	public void RequestDataInitialization(PhotonMessageInfo info)
	{
		if (!NetworkSystem.Instance.InRoom || !this.LocalAuthority())
		{
			return;
		}
		if (this.updatingPlayers == null)
		{
			this.updatingPlayers = new List<int>();
		}
		if (this.updatingPlayers.Contains(info.Sender.ActorNumber))
		{
			return;
		}
		this.updatingPlayers.Add(info.Sender.ActorNumber);
		base.StartCoroutine(this.RemoteDataInitialization(info.Sender, info.Sender.ActorNumber));
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000C560 File Offset: 0x0000A760
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		if (!this.localInZone)
		{
			return;
		}
		int num;
		if (!CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num))
		{
			return;
		}
		if (num > this.actorsPerInitializationCall)
		{
			return;
		}
		int num2 = 0;
		while (num2 < num && this.UpdateActorByType(stream))
		{
			num2++;
		}
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0000C5B8 File Offset: 0x0000A7B8
	public bool UpdateActorByType(PhotonStream stream)
	{
		int num;
		CrittersActor crittersActor;
		return CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num) && num >= 0 && num < this.universalActorId && this.actorById.TryGetValue(num, ref crittersActor) && crittersActor.UpdateSpecificActor(stream);
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000C600 File Offset: 0x0000A800
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!ZoneManagement.IsInZone(GTZone.critters))
		{
			return;
		}
		using (GTProfiler.BeginSample("WriteDataPUNCrittersManager"))
		{
			int num = Mathf.Min(this.updatesToSend.Count, this.actorsPerInitializationCall);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				this.allActors[this.updatesToSend[i]].SendDataByCrittersActorType(stream);
			}
			this.updatesToSend.RemoveRange(0, num);
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000C698 File Offset: 0x0000A898
	[PunRPC]
	public void RemoteCritterActorReleased(int releasedActorID, bool keepWorldPosition, Quaternion rotation, Vector3 position, Vector3 velocity, Vector3 angularVelocity, PhotonMessageInfo info)
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		if (rotation.IsValid())
		{
			float num = 10000f;
			if (position.IsValid(num))
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angularVelocity.IsValid(num3))
					{
						this.CheckValidRemoteActorRelease(releasedActorID, keepWorldPosition, rotation, position, velocity, angularVelocity, info);
						return;
					}
				}
			}
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000C714 File Offset: 0x0000A914
	[PunRPC]
	public void RemoteSpawnCreature(int actorID, int regionId, object[] spawnData, PhotonMessageInfo info)
	{
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		if (!this.localInZone)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		if (!CritterAppearance.ValidateData(spawnData))
		{
			return;
		}
		CrittersActor crittersActor;
		if (this.actorById.TryGetValue(actorID, ref crittersActor))
		{
			CrittersPawn crittersPawn = (CrittersPawn)crittersActor;
			this.SetCritterRegion(crittersPawn, regionId);
			crittersPawn.SetSpawnData(spawnData);
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000C784 File Offset: 0x0000A984
	[PunRPC]
	public void RemoteCrittersActorGrabbedby(int grabbedActorID, int grabberActorID, Quaternion offsetRotation, Vector3 offsetPosition, bool isGrabDisabled, PhotonMessageInfo info)
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		if (offsetRotation.IsValid())
		{
			float num = 10000f;
			if (offsetPosition.IsValid(num))
			{
				this.CheckValidRemoteActorGrab(grabbedActorID, grabberActorID, offsetRotation, offsetPosition, isGrabDisabled, info);
				return;
			}
		}
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000C7DC File Offset: 0x0000A9DC
	[PunRPC]
	public void RemoteUpdatePlayerCrittersActorData(object[] data, PhotonMessageInfo info)
	{
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		if (!this.localInZone)
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		CrittersActor crittersActor;
		for (int i = 0; i < data.Length; i += crittersActor.UpdatePlayerCrittersActorFromRPC(data, i))
		{
			int num;
			if (!CrittersManager.ValidateDataType<int>(data[i], out num))
			{
				return;
			}
			if (!this.actorById.TryGetValue(num, ref crittersActor))
			{
				return;
			}
		}
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000C83C File Offset: 0x0000AA3C
	[PunRPC]
	public void RemoteUpdateCritterData(object[] data, PhotonMessageInfo info)
	{
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		if (!this.localInZone)
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		CrittersActor crittersActor;
		for (int i = 0; i < data.Length; i += crittersActor.UpdateFromRPC(data, i))
		{
			int num;
			if (!CrittersManager.ValidateDataType<int>(data[i], out num))
			{
				return;
			}
			if (!this.actorById.TryGetValue(num, ref crittersActor))
			{
				return;
			}
		}
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000C8B0 File Offset: 0x0000AAB0
	public CrittersActor SpawnActor(CrittersActor.CrittersActorType type, int subObjectIndex = -1)
	{
		List<CrittersActor> list;
		if (!this.actorPools.TryGetValue(type, ref list))
		{
			return null;
		}
		int num = this.poolIndexDict[type];
		for (int i = 0; i < list.Count; i++)
		{
			if (!list[(i + num) % list.Count].gameObject.activeSelf)
			{
				num = (i + num) % list.Count;
				this.poolIndexDict[type] = num + 1;
				list[num].subObjectIndex = subObjectIndex;
				list[num].gameObject.SetActive(true);
				return list[num];
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			CrittersActor crittersActor = list[j];
			int num2 = this.actorBinIndices[crittersActor];
			if (!this.priorityBins[num2])
			{
				list[j].gameObject.SetActive(false);
				list[j].subObjectIndex = subObjectIndex;
				list[j].gameObject.SetActive(true);
				return list[j];
			}
		}
		return null;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x0600020D RID: 525 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0000C9B8 File Offset: 0x0000ABB8
	public void PopulatePools()
	{
		this.binDimensionXMin = this.crittersRange.position.x - this.crittersRange.localScale.x / 2f;
		this.binDimensionZMin = this.crittersRange.position.z - this.crittersRange.localScale.z / 2f;
		this.xLength = this.crittersRange.localScale.x;
		this.zLength = this.crittersRange.localScale.z;
		float num = this.xLength * this.zLength / (float)this.totalBinsApproximate;
		this.individualBinSide = Mathf.Sqrt(num);
		this.binXCount = Mathf.CeilToInt(this.xLength / this.individualBinSide);
		this.binZCount = Mathf.CeilToInt(this.zLength / this.individualBinSide);
		int num2 = this.binXCount * this.binZCount;
		this.actorBins = new List<CrittersActor>[num2];
		for (int i = 0; i < num2; i++)
		{
			this.actorBins[i] = new List<CrittersActor>();
		}
		this.priorityBins = new bool[num2];
		this.actorBinIndices = new Dictionary<CrittersActor, int>();
		this.nearbyActors = new List<CrittersActor>();
		this.allActors = new List<CrittersActor>();
		this.actorPools = new Dictionary<CrittersActor.CrittersActorType, List<CrittersActor>>();
		this.actorPools.Add(CrittersActor.CrittersActorType.Bag, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.Cage, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.Food, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.Creature, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.LoudNoise, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.Grabber, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.FoodSpawner, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.AttachPoint, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.StunBomb, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.BodyAttachPoint, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.NoiseMaker, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.StickyTrap, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.StickyGoo, new List<CrittersActor>());
		this.actorById = new Dictionary<int, CrittersActor>();
		this.universalActorId = 0;
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = base.transform;
		this.poolParent = gameObject.transform;
		this.poolParent.name = "Critter Actors Pool Parent";
		List<CrittersActor> list;
		this.actorPools.TryGetValue(CrittersActor.CrittersActorType.Food, ref list);
		this.persistentActors = Enumerable.ToList<CrittersActor>(Object.FindObjectsByType<CrittersActor>(1));
		this.persistentActors.Sort((CrittersActor x, CrittersActor y) => x.transform.position.magnitude.CompareTo(y.transform.position.magnitude));
		this.persistentActors.Sort((CrittersActor x, CrittersActor y) => x.gameObject.name.CompareTo(y.gameObject.name));
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.bagPrefab, CrittersActor.CrittersActorType.Bag, gameObject.transform, 80, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.cagePrefab, CrittersActor.CrittersActorType.Cage, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.foodPrefab, CrittersActor.CrittersActorType.Food, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.creaturePrefab, CrittersActor.CrittersActorType.Creature, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.noisePrefab, CrittersActor.CrittersActorType.LoudNoise, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.grabberPrefab, CrittersActor.CrittersActorType.Grabber, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.foodSpawnerPrefab, CrittersActor.CrittersActorType.FoodSpawner, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.bodyAttachPointPrefab, CrittersActor.CrittersActorType.BodyAttachPoint, gameObject.transform, 40, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, null, CrittersActor.CrittersActorType.AttachPoint, gameObject.transform, 0, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.stunBombPrefab, CrittersActor.CrittersActorType.StunBomb, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.noiseMakerPrefab, CrittersActor.CrittersActorType.NoiseMaker, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.stickyTrapPrefab, CrittersActor.CrittersActorType.StickyTrap, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.stickyGooPrefab, CrittersActor.CrittersActorType.StickyGoo, gameObject.transform, this.poolCount, this.persistentActors);
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0000CE88 File Offset: 0x0000B088
	public void UpdatePool<T>(ref Dictionary<CrittersActor.CrittersActorType, List<T>> dict, GameObject prefab, CrittersActor.CrittersActorType crittersActorType, Transform parent, int poolAmount, List<CrittersActor> sceneActors) where T : CrittersActor
	{
		int num = 0;
		for (int i = 0; i < sceneActors.Count; i++)
		{
			if (sceneActors[i].crittersActorType == crittersActorType)
			{
				dict[crittersActorType].Add((T)((object)sceneActors[i]));
				sceneActors[i].actorId = this.universalActorId;
				this.actorById.Add(this.universalActorId, sceneActors[i]);
				this.allActors.Add(sceneActors[i]);
				this.universalActorId++;
				num++;
				if (sceneActors[i].enabled)
				{
					if (crittersActorType == CrittersActor.CrittersActorType.Creature)
					{
						CrittersManager.RegisterCritter(sceneActors[i] as CrittersPawn);
					}
					else
					{
						CrittersManager.RegisterActor(sceneActors[i]);
					}
				}
			}
		}
		for (int j = 0; j < poolAmount - num; j++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(prefab);
			gameObject.transform.parent = parent;
			gameObject.name += j.ToString();
			gameObject.SetActive(false);
			T component = gameObject.GetComponent<T>();
			dict[crittersActorType].Add(component);
			component.actorId = this.universalActorId;
			component.SetDefaultParent(parent);
			this.actorById.Add(this.universalActorId, component);
			this.allActors.Add(component);
			this.universalActorId++;
		}
		this.poolIndexDict[crittersActorType] = 0;
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000D01C File Offset: 0x0000B21C
	public void TriggerEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position, Quaternion rotation)
	{
		Action<CrittersManager.CritterEvent, int, Vector3, Quaternion> onCritterEventReceived = this.OnCritterEventReceived;
		if (onCritterEventReceived != null)
		{
			onCritterEventReceived.Invoke(eventType, sourceActor, position, rotation);
		}
		if (!this.LocalAuthority() || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RemoteReceivedCritterEvent", 1, new object[]
		{
			eventType,
			sourceActor,
			position,
			rotation
		});
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000D08B File Offset: 0x0000B28B
	public void TriggerEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position)
	{
		this.TriggerEvent(eventType, sourceActor, position, Quaternion.identity);
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000D09C File Offset: 0x0000B29C
	[PunRPC]
	public void RemoteReceivedCritterEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position, Quaternion rotation, PhotonMessageInfo info)
	{
		if (!this.localInZone)
		{
			return;
		}
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		float num = 10000f;
		if (!position.IsValid(num) || !rotation.IsValid())
		{
			return;
		}
		if (!this.critterEventCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		Action<CrittersManager.CritterEvent, int, Vector3, Quaternion> onCritterEventReceived = this.OnCritterEventReceived;
		if (onCritterEventReceived == null)
		{
			return;
		}
		onCritterEventReceived.Invoke(eventType, sourceActor, position, rotation);
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000D11E File Offset: 0x0000B31E
	public static bool ValidateDataType<T>(object obj, out T dataAsType)
	{
		if (obj is T)
		{
			dataAsType = (T)((object)obj);
			return true;
		}
		dataAsType = default(T);
		return false;
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000D140 File Offset: 0x0000B340
	public void CheckValidRemoteActorRelease(int releasedActorID, bool keepWorldPosition, Quaternion rotation, Vector3 position, Vector3 velocity, Vector3 angularVelocity, PhotonMessageInfo info)
	{
		CrittersActor crittersActor;
		if (!this.actorById.TryGetValue(releasedActorID, ref crittersActor))
		{
			return;
		}
		CrittersActor crittersActor2 = this.TopLevelCritterGrabber(crittersActor);
		ref rotation.SetValueSafe(rotation);
		ref position.SetValueSafe(position);
		ref velocity.SetValueSafe(velocity);
		ref angularVelocity.SetValueSafe(angularVelocity);
		if (crittersActor2 != null && crittersActor2 is CrittersGrabber && crittersActor2.isOnPlayer && crittersActor2.rigPlayerId == info.Sender.ActorNumber)
		{
			crittersActor.Released(keepWorldPosition, rotation, position, velocity, angularVelocity);
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000D1C8 File Offset: 0x0000B3C8
	private void CheckValidRemoteActorGrab(int actorBeingGrabbedActorID, int grabbingActorID, Quaternion offsetRotation, Vector3 offsetPosition, bool isGrabDisabled, PhotonMessageInfo info)
	{
		CrittersActor crittersActor;
		CrittersActor crittersActor2;
		if (!this.actorById.TryGetValue(actorBeingGrabbedActorID, ref crittersActor) || !this.actorById.TryGetValue(grabbingActorID, ref crittersActor2))
		{
			return;
		}
		ref offsetRotation.SetValueSafe(offsetRotation);
		ref offsetPosition.SetValueSafe(offsetPosition);
		if ((crittersActor.transform.position - crittersActor2.transform.position).magnitude > this.maxGrabDistance || offsetPosition.magnitude > this.maxGrabDistance)
		{
			return;
		}
		if (((crittersActor2.crittersActorType == CrittersActor.CrittersActorType.Grabber && crittersActor2.isOnPlayer && crittersActor2.rigPlayerId == info.Sender.ActorNumber) || crittersActor2.crittersActorType != CrittersActor.CrittersActorType.Grabber) && crittersActor.AllowGrabbingActor(crittersActor2))
		{
			crittersActor.GrabbedBy(crittersActor2, true, offsetRotation, offsetPosition, isGrabDisabled);
		}
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000D28C File Offset: 0x0000B48C
	private CrittersActor TopLevelCritterGrabber(CrittersActor baseActor)
	{
		CrittersActor crittersActor = null;
		this.actorById.TryGetValue(baseActor.parentActorId, ref crittersActor);
		while (crittersActor != null && crittersActor.parentActorId != -1)
		{
			this.actorById.TryGetValue(crittersActor.parentActorId, ref crittersActor);
		}
		return crittersActor;
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000D2D8 File Offset: 0x0000B4D8
	public static CapsuleCollider DuplicateCapsuleCollider(Transform targetTransform, CapsuleCollider sourceCollider)
	{
		if (sourceCollider == null)
		{
			return null;
		}
		CapsuleCollider capsuleCollider = new GameObject().AddComponent<CapsuleCollider>();
		capsuleCollider.transform.rotation = sourceCollider.transform.rotation;
		capsuleCollider.transform.position = sourceCollider.transform.position;
		capsuleCollider.transform.localScale = sourceCollider.transform.lossyScale;
		capsuleCollider.radius = sourceCollider.radius;
		capsuleCollider.height = sourceCollider.height;
		capsuleCollider.center = sourceCollider.center;
		capsuleCollider.gameObject.layer = targetTransform.gameObject.layer;
		capsuleCollider.transform.SetParent(targetTransform.transform);
		return capsuleCollider;
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000D388 File Offset: 0x0000B588
	private void HandleZonesAndOwnership()
	{
		bool flag = this.localInZone;
		this.localInZone = ZoneManagement.IsInZone(GTZone.critters);
		this.CheckOwnership();
		if (!this.LocalAuthority() && this.localInZone && NetworkSystem.Instance.InRoom && this.guard.actualOwner != null && (!this.hasNewlyInitialized || !flag) && Time.time > this.lastRequest + this.initRequestCooldown)
		{
			this.lastRequest = Time.time;
			this.hasNewlyInitialized = true;
			base.SendRPC("RequestDataInitialization", this.guard.actualOwner, Array.Empty<object>());
		}
		if (flag && !this.localInZone)
		{
			this.ResetRoom();
			this.poolParent.gameObject.SetActive(false);
			this.crittersPool.poolParent.gameObject.SetActive(false);
		}
		if (!flag && this.localInZone)
		{
			this.poolParent.gameObject.SetActive(true);
			this.crittersPool.poolParent.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000D490 File Offset: 0x0000B690
	private void CheckOwnership()
	{
		if (!PhotonNetwork.InRoom && base.IsMine)
		{
			if (this.guard.actualOwner == null || !this.guard.actualOwner.Equals(NetworkSystem.Instance.LocalPlayer))
			{
				this.guard.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			}
			return;
		}
		if (this.allRigs == null && !VRRigCache.isInitialized)
		{
			return;
		}
		if (this.allRigs == null)
		{
			this.allRigs = new List<VRRig>(VRRigCache.Instance.GetAllRigs());
		}
		if (!this.LocalAuthority())
		{
			return;
		}
		if (this.localInZone)
		{
			return;
		}
		int num = int.MaxValue;
		NetPlayer netPlayer = null;
		for (int i = 0; i < this.allRigs.Count; i++)
		{
			NetPlayer creator = this.allRigs[i].creator;
			if (creator != null && this.allRigs[i].zoneEntity.currentZone == GTZone.critters && creator.ActorNumber < num)
			{
				netPlayer = creator;
				num = creator.ActorNumber;
			}
		}
		if (netPlayer == null)
		{
			return;
		}
		this.guard.TransferOwnership(netPlayer, "");
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000D5A0 File Offset: 0x0000B7A0
	public bool LocalAuthority()
	{
		return !NetworkSystem.Instance.InRoom || (!(this.guard == null) && ((this.guard.actualOwner != null && this.guard.isTrulyMine) || (base.Owner != null && base.Owner.IsLocal) || this.guard.currentState == NetworkingState.IsOwner));
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000D60C File Offset: 0x0000B80C
	private bool SenderIsOwner(PhotonMessageInfo info)
	{
		return (this.guard.actualOwner != null || base.Owner != null) && info.Sender != null && !this.LocalAuthority() && ((this.guard.actualOwner != null && this.guard.actualOwner.ActorNumber == info.Sender.ActorNumber) || (base.Owner != null && base.Owner.ActorNumber == info.Sender.ActorNumber));
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000D690 File Offset: 0x0000B890
	private void OwnerSentError(PhotonMessageInfo info)
	{
		NetPlayer owner = base.Owner;
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000D699 File Offset: 0x0000B899
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x06000225 RID: 549 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06000226 RID: 550 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04000214 RID: 532
	public CritterIndex creatureIndex;

	// Token: 0x04000215 RID: 533
	public static volatile CrittersManager instance;

	// Token: 0x04000217 RID: 535
	public LayerMask movementLayers;

	// Token: 0x04000218 RID: 536
	public LayerMask objectLayers;

	// Token: 0x04000219 RID: 537
	public LayerMask containerLayer;

	// Token: 0x0400021A RID: 538
	[ReadOnly]
	public List<CrittersActor> crittersActors;

	// Token: 0x0400021B RID: 539
	[ReadOnly]
	public List<CrittersActor> allActors;

	// Token: 0x0400021C RID: 540
	[ReadOnly]
	public List<CrittersPawn> crittersPawns;

	// Token: 0x0400021D RID: 541
	[ReadOnly]
	public List<CrittersActor> despawnableActors;

	// Token: 0x0400021E RID: 542
	[ReadOnly]
	public List<CrittersActor> newlyDisabledActors;

	// Token: 0x0400021F RID: 543
	[ReadOnly]
	public List<CrittersRigActorSetup> rigActorSetups;

	// Token: 0x04000220 RID: 544
	[ReadOnly]
	public List<CrittersActorSpawner> actorSpawners;

	// Token: 0x04000221 RID: 545
	[NonSerialized]
	private List<CrittersActor> persistentActors = new List<CrittersActor>();

	// Token: 0x04000222 RID: 546
	public Dictionary<int, CrittersActor> actorById;

	// Token: 0x04000223 RID: 547
	public Dictionary<CrittersPawn, List<CrittersActor>> awareOfActors;

	// Token: 0x04000224 RID: 548
	public Dictionary<VRRig, CrittersRigActorSetup> rigSetupByRig;

	// Token: 0x04000225 RID: 549
	private int allActorsCount;

	// Token: 0x04000226 RID: 550
	public bool intialized;

	// Token: 0x04000227 RID: 551
	private List<int> updatesToSend;

	// Token: 0x04000228 RID: 552
	public int actorsPerInitializationCall = 5;

	// Token: 0x04000229 RID: 553
	public float actorsInitializationCallCooldown = 0.2f;

	// Token: 0x0400022A RID: 554
	public Transform poolParent;

	// Token: 0x0400022B RID: 555
	public List<object> objList;

	// Token: 0x0400022C RID: 556
	public double spawnDelay;

	// Token: 0x0400022D RID: 557
	private double lastSpawnTime;

	// Token: 0x0400022E RID: 558
	public float softJointGracePeriod = 0.1f;

	// Token: 0x0400022F RID: 559
	private List<CrittersRegion> _spawnRegions;

	// Token: 0x04000230 RID: 560
	private int _currentRegionIndex = -1;

	// Token: 0x04000231 RID: 561
	private static CrittersActorGrabber _rightGrabber;

	// Token: 0x04000232 RID: 562
	private static CrittersActorGrabber _leftGrabber;

	// Token: 0x04000233 RID: 563
	public float springForce = 1000f;

	// Token: 0x04000234 RID: 564
	public float springAngularForce = 100f;

	// Token: 0x04000235 RID: 565
	public float damperForce = 10f;

	// Token: 0x04000236 RID: 566
	public float damperAngularForce = 1f;

	// Token: 0x04000237 RID: 567
	public float lightMass = 0.05f;

	// Token: 0x04000238 RID: 568
	public float heavyMass = 2f;

	// Token: 0x04000239 RID: 569
	public float overlapDistanceMax = 0.01f;

	// Token: 0x0400023A RID: 570
	public float fastThrowThreshold = 3f;

	// Token: 0x0400023B RID: 571
	public float fastThrowMultiplier = 1.5f;

	// Token: 0x0400023C RID: 572
	private Dictionary<CrittersActor.CrittersActorType, int> poolIndexDict;

	// Token: 0x0400023E RID: 574
	public CrittersManager.AllowGrabbingFlags privateRoomGrabbingFlags;

	// Token: 0x0400023F RID: 575
	public CrittersManager.AllowGrabbingFlags publicRoomGrabbingFlags;

	// Token: 0x04000240 RID: 576
	public float MaxAttachSpeed = 0.04f;

	// Token: 0x04000241 RID: 577
	private float binDimensionXMin;

	// Token: 0x04000242 RID: 578
	private float binDimensionZMin;

	// Token: 0x04000243 RID: 579
	public Transform crittersRange;

	// Token: 0x04000244 RID: 580
	public int totalBinsApproximate = 400;

	// Token: 0x04000245 RID: 581
	private float xLength;

	// Token: 0x04000246 RID: 582
	private float zLength;

	// Token: 0x04000247 RID: 583
	private int binXCount;

	// Token: 0x04000248 RID: 584
	private int binZCount;

	// Token: 0x04000249 RID: 585
	private float individualBinSide;

	// Token: 0x0400024A RID: 586
	private List<CrittersActor>[] actorBins;

	// Token: 0x0400024B RID: 587
	private bool[] priorityBins;

	// Token: 0x0400024C RID: 588
	private Dictionary<CrittersActor, int> actorBinIndices;

	// Token: 0x0400024D RID: 589
	private List<CrittersActor> nearbyActors;

	// Token: 0x0400024E RID: 590
	private List<NetPlayer> playersToUpdate;

	// Token: 0x0400024F RID: 591
	public CrittersPool crittersPool;

	// Token: 0x04000250 RID: 592
	private int lowPriorityActorsPerFrame = 5;

	// Token: 0x04000251 RID: 593
	private int lowPriorityIndex;

	// Token: 0x04000252 RID: 594
	private int spawnerIndex;

	// Token: 0x04000253 RID: 595
	private int despawnIndex;

	// Token: 0x04000254 RID: 596
	private List<CrittersActor> lowPriorityPawnsToProcess;

	// Token: 0x04000255 RID: 597
	private Dictionary<CrittersActor.CrittersActorType, float> despawnDecayValue;

	// Token: 0x04000256 RID: 598
	public float decayRate = 60f;

	// Token: 0x04000257 RID: 599
	private CrittersActor.CrittersActorType[] actorTypes;

	// Token: 0x04000258 RID: 600
	public float maxGrabDistance = 25f;

	// Token: 0x04000259 RID: 601
	public RequestableOwnershipGuard guard;

	// Token: 0x0400025A RID: 602
	private List<VRRig> allRigs;

	// Token: 0x0400025B RID: 603
	private bool localInZone;

	// Token: 0x0400025C RID: 604
	private List<int> updatingPlayers;

	// Token: 0x0400025D RID: 605
	private bool hasNewlyInitialized;

	// Token: 0x0400025E RID: 606
	private float initRequestCooldown = 10f;

	// Token: 0x0400025F RID: 607
	private float lastRequest;

	// Token: 0x04000260 RID: 608
	public int poolCount = 100;

	// Token: 0x04000261 RID: 609
	public int despawnThreshold = 20;

	// Token: 0x04000262 RID: 610
	private Dictionary<CrittersActor.CrittersActorType, int> poolCounts;

	// Token: 0x04000263 RID: 611
	private Dictionary<CrittersActor.CrittersActorType, List<CrittersActor>> actorPools;

	// Token: 0x04000264 RID: 612
	public GameObject foodPrefab;

	// Token: 0x04000265 RID: 613
	public GameObject creaturePrefab;

	// Token: 0x04000266 RID: 614
	public GameObject noisePrefab;

	// Token: 0x04000267 RID: 615
	public GameObject grabberPrefab;

	// Token: 0x04000268 RID: 616
	public GameObject cagePrefab;

	// Token: 0x04000269 RID: 617
	public GameObject foodSpawnerPrefab;

	// Token: 0x0400026A RID: 618
	public GameObject stunBombPrefab;

	// Token: 0x0400026B RID: 619
	public GameObject bodyAttachPointPrefab;

	// Token: 0x0400026C RID: 620
	public GameObject bagPrefab;

	// Token: 0x0400026D RID: 621
	public GameObject noiseMakerPrefab;

	// Token: 0x0400026E RID: 622
	public GameObject stickyTrapPrefab;

	// Token: 0x0400026F RID: 623
	public GameObject stickyGooPrefab;

	// Token: 0x04000270 RID: 624
	public int universalActorId;

	// Token: 0x04000271 RID: 625
	public int rigActorId;

	// Token: 0x04000273 RID: 627
	private CallLimiter critterEventCallLimit = new CallLimiter(10, 0.5f, 0.5f);

	// Token: 0x0200005F RID: 95
	[Flags]
	public enum AllowGrabbingFlags
	{
		// Token: 0x04000275 RID: 629
		None = 0,
		// Token: 0x04000276 RID: 630
		OutOfHands = 1,
		// Token: 0x04000277 RID: 631
		FromBags = 2,
		// Token: 0x04000278 RID: 632
		EntireBag = 4
	}

	// Token: 0x02000060 RID: 96
	public enum CritterEvent
	{
		// Token: 0x0400027A RID: 634
		StunExplosion,
		// Token: 0x0400027B RID: 635
		NoiseMakerTriggered,
		// Token: 0x0400027C RID: 636
		StickyDeployed,
		// Token: 0x0400027D RID: 637
		StickyTriggered
	}
}
