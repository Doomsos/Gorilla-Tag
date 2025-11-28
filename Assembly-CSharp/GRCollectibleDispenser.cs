using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000693 RID: 1683
public class GRCollectibleDispenser : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x06002AF1 RID: 10993 RVA: 0x000E7072 File Offset: 0x000E5272
	public bool CollectibleAlreadySpawned
	{
		get
		{
			return this.currentCollectible != null;
		}
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x06002AF2 RID: 10994 RVA: 0x000E7080 File Offset: 0x000E5280
	public bool ReadyToDispenseNewCollectible
	{
		get
		{
			double num = (double)this.collectibleRespawnTimeMinutes * 60.0;
			bool flag = (ulong)this.collectiblesDispensed < (ulong)((long)this.maxDispenseCount);
			return !this.CollectibleAlreadySpawned && flag && Time.timeAsDouble - this.collectibleDispenseRequestTime > num && Time.timeAsDouble - this.collectibleDispenseTime > num && Time.timeAsDouble - this.collectibleCollectedTime > num;
		}
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x000E70EC File Offset: 0x000E52EC
	public void OnEntityInit()
	{
		GhostReactor reactor = GhostReactorManager.Get(this.gameEntity).reactor;
		if (reactor != null)
		{
			reactor.collectibleDispensers.Add(this);
		}
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x000E7120 File Offset: 0x000E5320
	public void OnEntityDestroy()
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null && ghostReactorManager.reactor != null)
		{
			ghostReactorManager.reactor.collectibleDispensers.Remove(this);
		}
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x000E7164 File Offset: 0x000E5364
	public void OnEntityStateChange(long prevState, long nextState)
	{
		uint num = this.collectiblesDispensed;
		uint num2 = this.collectiblesCollected;
		this.collectiblesDispensed = (uint)(nextState >> 32);
		this.collectiblesCollected = (uint)(nextState & (long)((ulong)-1));
		if (num != this.collectiblesDispensed)
		{
			this.collectibleDispenseTime = Time.timeAsDouble;
		}
		if (num2 != this.collectiblesCollected)
		{
			this.collectibleCollectedTime = Time.timeAsDouble;
		}
		if ((ulong)this.collectiblesCollected >= (ulong)((long)this.maxDispenseCount))
		{
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000E71F0 File Offset: 0x000E53F0
	public void RequestDispenseCollectible()
	{
		if (this.ReadyToDispenseNewCollectible && this.gameEntity.IsAuthority())
		{
			this.gameEntity.manager.RequestCreateItem(this.collectiblePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)this.gameEntity.manager.GetNetIdFromEntityId(this.gameEntity.id));
			this.collectiblesDispensed += 1U;
			this.collectibleDispenseTime = Time.timeAsDouble;
			long num = (long)((ulong)this.collectiblesDispensed);
			long num2 = (long)((ulong)this.collectiblesCollected);
			long newState = num << 32 | num2;
			this.gameEntity.RequestState(this.gameEntity.id, newState);
		}
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x000E72B0 File Offset: 0x000E54B0
	public void OnCollectibleConsumed()
	{
		if (this.currentCollectible != null && this.currentCollectible.IsNotNull())
		{
			GRCollectible grcollectible = this.currentCollectible;
			grcollectible.OnCollected = (Action)Delegate.Remove(grcollectible.OnCollected, new Action(this.OnCollectibleConsumed));
			GameEntity entity = this.currentCollectible.entity;
			entity.OnGrabbed = (Action)Delegate.Remove(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
			this.currentCollectible = null;
		}
		this.collectiblesCollected += 1U;
		this.collectibleCollectedTime = Time.timeAsDouble;
		if (this.gameEntity.IsAuthority())
		{
			long num = (long)((ulong)this.collectiblesDispensed);
			long num2 = (long)((ulong)this.collectiblesCollected);
			long newState = num << 32 | num2;
			this.gameEntity.RequestState(this.gameEntity.id, newState);
		}
		if ((ulong)this.collectiblesCollected >= (ulong)((long)this.maxDispenseCount))
		{
			this.dispenserExhaustedEffect.Play();
			this.audioSource.PlayOneShot(this.dispenserExhaustedClip, this.dispenserExhaustedVolume);
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
			return;
		}
		this.collectibleTakenEffect.Play();
		this.audioSource.PlayOneShot(this.collectibleTakenClip, this.collectibleTakenVolume);
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x000E73FC File Offset: 0x000E55FC
	public void GetSpawnedCollectible(GRCollectible collectible)
	{
		this.currentCollectible = collectible;
		collectible.OnCollected = (Action)Delegate.Combine(collectible.OnCollected, new Action(this.OnCollectibleConsumed));
		GameEntity entity = collectible.entity;
		entity.OnGrabbed = (Action)Delegate.Combine(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
	}

	// Token: 0x0400376B RID: 14187
	public GameEntity gameEntity;

	// Token: 0x0400376C RID: 14188
	public GameEntity collectiblePrefab;

	// Token: 0x0400376D RID: 14189
	public Transform spawnLocation;

	// Token: 0x0400376E RID: 14190
	public LayerMask collectibleLayerMask;

	// Token: 0x0400376F RID: 14191
	public float collectibleRespawnTimeMinutes = 1.5f;

	// Token: 0x04003770 RID: 14192
	public int maxDispenseCount = 3;

	// Token: 0x04003771 RID: 14193
	public AudioSource audioSource;

	// Token: 0x04003772 RID: 14194
	public Transform stillDispensingModel;

	// Token: 0x04003773 RID: 14195
	public Transform fullyConsumedModel;

	// Token: 0x04003774 RID: 14196
	public ParticleSystem collectibleTakenEffect;

	// Token: 0x04003775 RID: 14197
	public AudioClip collectibleTakenClip;

	// Token: 0x04003776 RID: 14198
	public float collectibleTakenVolume;

	// Token: 0x04003777 RID: 14199
	public ParticleSystem dispenserExhaustedEffect;

	// Token: 0x04003778 RID: 14200
	public AudioClip dispenserExhaustedClip;

	// Token: 0x04003779 RID: 14201
	public float dispenserExhaustedVolume;

	// Token: 0x0400377A RID: 14202
	private GRCollectible currentCollectible;

	// Token: 0x0400377B RID: 14203
	private Coroutine getSpawnedCollectibleCoroutine;

	// Token: 0x0400377C RID: 14204
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x0400377D RID: 14205
	private uint collectiblesDispensed;

	// Token: 0x0400377E RID: 14206
	private uint collectiblesCollected;

	// Token: 0x0400377F RID: 14207
	private double collectibleDispenseRequestTime = -10000.0;

	// Token: 0x04003780 RID: 14208
	private double collectibleDispenseTime = -10000.0;

	// Token: 0x04003781 RID: 14209
	private double collectibleCollectedTime = -10000.0;
}
