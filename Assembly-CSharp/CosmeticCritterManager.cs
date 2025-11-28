using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005C7 RID: 1479
public class CosmeticCritterManager : NetworkSceneObject, ITickSystemTick
{
	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x0600256A RID: 9578 RVA: 0x000C831B File Offset: 0x000C651B
	// (set) Token: 0x0600256B RID: 9579 RVA: 0x000C8322 File Offset: 0x000C6522
	public static CosmeticCritterManager Instance { get; private set; }

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x0600256C RID: 9580 RVA: 0x000C832A File Offset: 0x000C652A
	// (set) Token: 0x0600256D RID: 9581 RVA: 0x000C8332 File Offset: 0x000C6532
	public bool TickRunning { get; set; }

	// Token: 0x0600256E RID: 9582 RVA: 0x000C833B File Offset: 0x000C653B
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x000C834F File Offset: 0x000C654F
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x000C8363 File Offset: 0x000C6563
	public void RegisterLocalHoldable(CosmeticCritterHoldable holdable)
	{
		this.localHoldables.Add(holdable);
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x000C8371 File Offset: 0x000C6571
	public void RegisterIndependentSpawner(CosmeticCritterSpawnerIndependent spawner)
	{
		if (spawner.IsLocal)
		{
			this.localCritterSpawners.AddIfNew(spawner);
			return;
		}
		this.remoteCritterSpawners.AddIfNew(spawner);
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x000C8394 File Offset: 0x000C6594
	public void UnregisterIndependentSpawner(CosmeticCritterSpawnerIndependent spawner)
	{
		if (spawner.IsLocal)
		{
			this.localCritterSpawners.Remove(spawner);
			return;
		}
		this.remoteCritterSpawners.Remove(spawner);
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x000C83B9 File Offset: 0x000C65B9
	public void RegisterCatcher(CosmeticCritterCatcher catcher)
	{
		if (catcher.IsLocal)
		{
			this.localCritterCatchers.AddIfNew(catcher);
			return;
		}
		this.remoteCritterCatchers.AddIfNew(catcher);
	}

	// Token: 0x06002574 RID: 9588 RVA: 0x000C83DC File Offset: 0x000C65DC
	public void UnregisterCatcher(CosmeticCritterCatcher catcher)
	{
		if (catcher.IsLocal)
		{
			this.localCritterCatchers.Remove(catcher);
			return;
		}
		this.remoteCritterCatchers.Remove(catcher);
	}

	// Token: 0x06002575 RID: 9589 RVA: 0x000C8404 File Offset: 0x000C6604
	public void RegisterTickForEachCritter(Type type, ICosmeticCritterTickForEach target)
	{
		List<ICosmeticCritterTickForEach> list;
		if (!this.tickForEachCritterOfType.TryGetValue(type, ref list) || list == null)
		{
			list = new List<ICosmeticCritterTickForEach>();
			this.tickForEachCritterOfType.Add(type, list);
		}
		list.AddIfNew(target);
	}

	// Token: 0x06002576 RID: 9590 RVA: 0x000C8440 File Offset: 0x000C6640
	public void UnregisterTickForEachCritter(Type type, ICosmeticCritterTickForEach target)
	{
		List<ICosmeticCritterTickForEach> list;
		if (this.tickForEachCritterOfType.TryGetValue(type, ref list) && list != null)
		{
			list.Remove(target);
		}
	}

	// Token: 0x06002577 RID: 9591 RVA: 0x000C8468 File Offset: 0x000C6668
	private void ResetLocalCallLimiters()
	{
		int i = 0;
		while (i < this.localHoldables.Count)
		{
			if (this.localHoldables[i] == null)
			{
				this.localHoldables.RemoveAt(i);
			}
			else
			{
				this.localHoldables[i].ResetCallLimiter();
				i++;
			}
		}
	}

	// Token: 0x06002578 RID: 9592 RVA: 0x000C84C0 File Offset: 0x000C66C0
	private void ResetCosmeticCritters(NetPlayer player)
	{
		if (NetworkSystem.Instance.LocalPlayer != player)
		{
			return;
		}
		this.ResetLocalCallLimiters();
		for (int i = 0; i < this.activeCritters.Count; i++)
		{
			this.FreeCritter(this.activeCritters[i]);
		}
	}

	// Token: 0x06002579 RID: 9593 RVA: 0x000C850C File Offset: 0x000C670C
	private void Awake()
	{
		if (CosmeticCritterManager.Instance != null && CosmeticCritterManager.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		CosmeticCritterManager.Instance = this;
		this.localHoldables = new List<CosmeticCritterHoldable>();
		this.localCritterSpawners = new List<CosmeticCritterSpawnerIndependent>();
		this.remoteCritterSpawners = new List<CosmeticCritterSpawnerIndependent>();
		this.localCritterCatchers = new List<CosmeticCritterCatcher>();
		this.remoteCritterCatchers = new List<CosmeticCritterCatcher>();
		this.activeCritters = new List<CosmeticCritter>();
		this.activeCrittersPerType = new Dictionary<Type, int>();
		this.activeCrittersBySeed = new Dictionary<int, CosmeticCritter>();
		this.inactiveCrittersByType = new Dictionary<Type, Stack<CosmeticCritter>>();
		this.tickForEachCritterOfType = new Dictionary<Type, List<ICosmeticCritterTickForEach>>();
		NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(this.ResetCosmeticCritters);
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.ResetCosmeticCritters);
	}

	// Token: 0x0600257A RID: 9594 RVA: 0x000C85F0 File Offset: 0x000C67F0
	private void ReuseOrSpawnNewCritter(CosmeticCritterSpawner spawner, int seed, double time)
	{
		Type critterType = spawner.GetCritterType();
		Stack<CosmeticCritter> stack;
		CosmeticCritter component;
		if (!this.inactiveCrittersByType.TryGetValue(critterType, ref stack))
		{
			stack = new Stack<CosmeticCritter>();
			this.inactiveCrittersByType.Add(critterType, stack);
			component = Object.Instantiate<GameObject>(spawner.GetCritterPrefab(), base.transform).GetComponent<CosmeticCritter>();
		}
		else if (stack.TryPop(ref component))
		{
			component.gameObject.SetActive(true);
		}
		else
		{
			component = Object.Instantiate<GameObject>(spawner.GetCritterPrefab(), base.transform).GetComponent<CosmeticCritter>();
		}
		component.SetSeedSpawnerTypeAndTime(seed, spawner, critterType, time);
		this.activeCritters.Add(component);
		if (!this.activeCrittersPerType.ContainsKey(critterType))
		{
			this.activeCrittersPerType.Add(critterType, 1);
		}
		else
		{
			Dictionary<Type, int> dictionary = this.activeCrittersPerType;
			Type type = critterType;
			dictionary[type]++;
		}
		this.activeCrittersBySeed.Add(seed, component);
		Random.State state = Random.state;
		Random.InitState(seed);
		spawner.SetRandomVariables(component);
		component.SetRandomVariables();
		Random.state = state;
		spawner.OnSpawn(component);
		component.OnSpawn();
	}

	// Token: 0x0600257B RID: 9595 RVA: 0x000C86F8 File Offset: 0x000C68F8
	private void FreeCritter(CosmeticCritter critter)
	{
		critter.OnDespawn();
		if (critter.Spawner != null)
		{
			critter.Spawner.OnDespawn(critter);
		}
		critter.gameObject.SetActive(false);
		Type cachedType = critter.CachedType;
		Stack<CosmeticCritter> stack;
		if (!this.inactiveCrittersByType.TryGetValue(cachedType, ref stack))
		{
			stack = new Stack<CosmeticCritter>();
			this.inactiveCrittersByType.Add(cachedType, stack);
		}
		stack.Push(critter);
		this.activeCritters.Remove(critter);
		int num;
		if (this.activeCrittersPerType.TryGetValue(cachedType, ref num))
		{
			this.activeCrittersPerType[cachedType] = Math.Max(num - 1, 0);
		}
		this.activeCrittersBySeed.Remove(critter.Seed);
	}

	// Token: 0x0600257C RID: 9596 RVA: 0x000C87A8 File Offset: 0x000C69A8
	public void Tick()
	{
		for (int i = 0; i < this.activeCritters.Count; i++)
		{
			CosmeticCritter cosmeticCritter = this.activeCritters[i];
			if (cosmeticCritter.Expired())
			{
				this.FreeCritter(cosmeticCritter);
			}
			else
			{
				cosmeticCritter.Tick();
				List<ICosmeticCritterTickForEach> list;
				if (this.tickForEachCritterOfType.TryGetValue(cosmeticCritter.CachedType, ref list))
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].TickForEachCritter(cosmeticCritter);
					}
				}
				int k = 0;
				while (k < this.localCritterCatchers.Count)
				{
					CosmeticCritterCatcher cosmeticCritterCatcher = this.localCritterCatchers[k];
					CosmeticCritterAction localCatchAction = cosmeticCritterCatcher.GetLocalCatchAction(cosmeticCritter);
					if (localCatchAction != CosmeticCritterAction.None)
					{
						double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.timeAsDouble;
						cosmeticCritterCatcher.OnCatch(cosmeticCritter, localCatchAction, num);
						if ((localCatchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
						{
							this.FreeCritter(cosmeticCritter);
							i--;
						}
						if ((localCatchAction & CosmeticCritterAction.SpawnLinked) != CosmeticCritterAction.None && cosmeticCritterCatcher.GetLinkedSpawner() != null)
						{
							this.ReuseOrSpawnNewCritter(cosmeticCritterCatcher.GetLinkedSpawner(), cosmeticCritter.Seed + 1, num);
						}
						if (PhotonNetwork.InRoom && (localCatchAction & CosmeticCritterAction.RPC) != CosmeticCritterAction.None)
						{
							this.photonView.RPC("CosmeticCritterRPC", 1, new object[]
							{
								localCatchAction,
								cosmeticCritterCatcher.OwnerID,
								cosmeticCritter.Seed
							});
							break;
						}
						break;
					}
					else
					{
						k++;
					}
				}
			}
		}
		for (int l = 0; l < this.localCritterSpawners.Count; l++)
		{
			CosmeticCritterSpawnerIndependent cosmeticCritterSpawnerIndependent = this.localCritterSpawners[l];
			int num2;
			if ((!this.activeCrittersPerType.TryGetValue(cosmeticCritterSpawnerIndependent.GetCritterType(), ref num2) || num2 < cosmeticCritterSpawnerIndependent.GetCritter().GetGlobalMaxCritters()) && cosmeticCritterSpawnerIndependent.CanSpawnLocal())
			{
				int num3 = Random.Range(0, int.MaxValue);
				if (!this.activeCrittersBySeed.ContainsKey(num3))
				{
					this.ReuseOrSpawnNewCritter(cosmeticCritterSpawnerIndependent, num3, PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.timeAsDouble);
					if (PhotonNetwork.InRoom)
					{
						this.photonView.RPC("CosmeticCritterRPC", 1, new object[]
						{
							CosmeticCritterAction.RPC | CosmeticCritterAction.Spawn,
							cosmeticCritterSpawnerIndependent.OwnerID,
							num3
						});
					}
				}
			}
		}
	}

	// Token: 0x0600257D RID: 9597 RVA: 0x000C89F0 File Offset: 0x000C6BF0
	[PunRPC]
	private void CosmeticCritterRPC(CosmeticCritterAction action, int holdableID, int seed, PhotonMessageInfo info)
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		GorillaNot.IncrementRPCCall(photonMessageInfoWrapped, "CosmeticCritterRPC");
		if ((action & CosmeticCritterAction.RPC) == CosmeticCritterAction.None)
		{
			return;
		}
		if (action == (CosmeticCritterAction.RPC | CosmeticCritterAction.Spawn))
		{
			this.SpawnCosmeticCritterRPC(holdableID, seed, photonMessageInfoWrapped);
			return;
		}
		this.CatchCosmeticCritterRPC(action, holdableID, seed, photonMessageInfoWrapped);
	}

	// Token: 0x0600257E RID: 9598 RVA: 0x000C8A30 File Offset: 0x000C6C30
	private void CatchCosmeticCritterRPC(CosmeticCritterAction catchAction, int catcherID, int seed, PhotonMessageInfoWrapped info)
	{
		CosmeticCritter critter;
		if (!this.activeCrittersBySeed.TryGetValue(seed, ref critter))
		{
			return;
		}
		int i = 0;
		while (i < this.remoteCritterCatchers.Count)
		{
			CosmeticCritterCatcher cosmeticCritterCatcher = this.remoteCritterCatchers[i];
			if (cosmeticCritterCatcher.OwnerID == catcherID)
			{
				if (!cosmeticCritterCatcher.OwningPlayerMatches(info))
				{
					return;
				}
				if (cosmeticCritterCatcher.ValidateRemoteCatchAction(critter, catchAction, info.SentServerTime))
				{
					cosmeticCritterCatcher.OnCatch(critter, catchAction, info.SentServerTime);
					if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
					{
						this.FreeCritter(critter);
					}
					int num;
					if ((catchAction & CosmeticCritterAction.SpawnLinked) != CosmeticCritterAction.None && cosmeticCritterCatcher.GetLinkedSpawner() != null && (!this.activeCrittersPerType.TryGetValue(cosmeticCritterCatcher.GetLinkedSpawner().GetCritterType(), ref num) || num < cosmeticCritterCatcher.GetLinkedSpawner().GetCritter().GetGlobalMaxCritters() + 1))
					{
						this.ReuseOrSpawnNewCritter(cosmeticCritterCatcher.GetLinkedSpawner(), seed + 1, info.SentServerTime);
					}
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x0600257F RID: 9599 RVA: 0x000C8B14 File Offset: 0x000C6D14
	private void SpawnCosmeticCritterRPC(int spawnerID, int seed, PhotonMessageInfoWrapped info)
	{
		if (this.activeCrittersBySeed.ContainsKey(seed))
		{
			return;
		}
		int i = 0;
		while (i < this.remoteCritterSpawners.Count)
		{
			CosmeticCritterSpawnerIndependent cosmeticCritterSpawnerIndependent = this.remoteCritterSpawners[i];
			if (cosmeticCritterSpawnerIndependent.OwnerID == spawnerID)
			{
				if (!cosmeticCritterSpawnerIndependent.OwningPlayerMatches(info))
				{
					return;
				}
				int num;
				if ((!this.activeCrittersPerType.TryGetValue(cosmeticCritterSpawnerIndependent.GetCritterType(), ref num) || num < cosmeticCritterSpawnerIndependent.GetCritter().GetGlobalMaxCritters()) && cosmeticCritterSpawnerIndependent.CanSpawnRemote(info.SentServerTime))
				{
					this.ReuseOrSpawnNewCritter(cosmeticCritterSpawnerIndependent, seed, info.SentServerTime);
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x04003107 RID: 12551
	private List<CosmeticCritterHoldable> localHoldables;

	// Token: 0x04003108 RID: 12552
	private List<CosmeticCritterSpawnerIndependent> localCritterSpawners;

	// Token: 0x04003109 RID: 12553
	private List<CosmeticCritterSpawnerIndependent> remoteCritterSpawners;

	// Token: 0x0400310A RID: 12554
	private List<CosmeticCritterCatcher> localCritterCatchers;

	// Token: 0x0400310B RID: 12555
	private List<CosmeticCritterCatcher> remoteCritterCatchers;

	// Token: 0x0400310C RID: 12556
	private List<CosmeticCritter> activeCritters;

	// Token: 0x0400310D RID: 12557
	private Dictionary<Type, int> activeCrittersPerType;

	// Token: 0x0400310E RID: 12558
	private Dictionary<int, CosmeticCritter> activeCrittersBySeed;

	// Token: 0x0400310F RID: 12559
	private Dictionary<Type, Stack<CosmeticCritter>> inactiveCrittersByType;

	// Token: 0x04003110 RID: 12560
	private Dictionary<Type, List<ICosmeticCritterTickForEach>> tickForEachCritterOfType;
}
