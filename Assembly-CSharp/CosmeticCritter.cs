using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005C3 RID: 1475
public abstract class CosmeticCritter : MonoBehaviour
{
	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x06002549 RID: 9545 RVA: 0x000C80DB File Offset: 0x000C62DB
	// (set) Token: 0x0600254A RID: 9546 RVA: 0x000C80E3 File Offset: 0x000C62E3
	public int Seed { get; protected set; }

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x0600254B RID: 9547 RVA: 0x000C80EC File Offset: 0x000C62EC
	// (set) Token: 0x0600254C RID: 9548 RVA: 0x000C80F4 File Offset: 0x000C62F4
	public CosmeticCritterSpawner Spawner { get; protected set; }

	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x0600254D RID: 9549 RVA: 0x000C80FD File Offset: 0x000C62FD
	// (set) Token: 0x0600254E RID: 9550 RVA: 0x000C8105 File Offset: 0x000C6305
	public Type CachedType { get; private set; }

	// Token: 0x0600254F RID: 9551 RVA: 0x000C810E File Offset: 0x000C630E
	public int GetGlobalMaxCritters()
	{
		return this.globalMaxCritters;
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000C8116 File Offset: 0x000C6316
	public void SetSeedSpawnerTypeAndTime(int seed, CosmeticCritterSpawner spawner, Type type, double time)
	{
		this.Seed = seed;
		this.Spawner = spawner;
		this.CachedType = type;
		this.startTime = time;
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnSpawn()
	{
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnDespawn()
	{
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void SetRandomVariables()
	{
	}

	// Token: 0x06002554 RID: 9556
	public abstract void Tick();

	// Token: 0x06002555 RID: 9557 RVA: 0x000C8135 File Offset: 0x000C6335
	protected double GetAliveTime()
	{
		if (!PhotonNetwork.InRoom)
		{
			return Time.timeAsDouble - this.startTime;
		}
		return PhotonNetwork.Time - this.startTime;
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x000C8157 File Offset: 0x000C6357
	public virtual bool Expired()
	{
		return this.GetAliveTime() > (double)this.lifetime || this.GetAliveTime() < 0.0;
	}

	// Token: 0x040030F5 RID: 12533
	[Tooltip("After this many seconds the critter will forcibly despawn.")]
	[SerializeField]
	protected float lifetime;

	// Token: 0x040030F6 RID: 12534
	[Tooltip("The maximum number of this kind of critter that can be in the room at any given time.")]
	[SerializeField]
	private int globalMaxCritters;

	// Token: 0x040030FA RID: 12538
	protected double startTime;
}
