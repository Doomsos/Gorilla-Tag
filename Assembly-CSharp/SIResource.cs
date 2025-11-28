using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class SIResource : MonoBehaviour
{
	// Token: 0x060007F6 RID: 2038 RVA: 0x0002B79C File Offset: 0x0002999C
	private void Awake()
	{
		if (this.myGameEntity == null)
		{
			this.myGameEntity = base.GetComponent<GameEntity>();
		}
		if (this.myGameEntity == null)
		{
			Debug.LogError("missing gameentity reference! bad!", base.gameObject);
			return;
		}
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.SetLastGrabbed));
		this._rb = base.GetComponent<Rigidbody>();
		this.myGameEntity.onEntityDestroyed += this.HandleOnDestroyed;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0002B82C File Offset: 0x00029A2C
	public void Update()
	{
		if (this.isSleeping || !this.shouldSleep)
		{
			return;
		}
		if (Time.time < this.timeReleased + this.sleepTime)
		{
			return;
		}
		this._rb.isKinematic = true;
		this.isSleeping = true;
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002B867 File Offset: 0x00029A67
	public void SetLastGrabbed()
	{
		this.lastPlayerHeld = SIPlayer.Get(this.myGameEntity.lastHeldByActorNumber);
		if (this.lastPlayerHeld == SIPlayer.LocalPlayer)
		{
			this.localEverGrabbed = true;
		}
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002B898 File Offset: 0x00029A98
	protected virtual void OnEnable()
	{
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.myGameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Combine(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.myGameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.myGameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.timeReleased = Time.time;
		this._rb.isKinematic = true;
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0002B958 File Offset: 0x00029B58
	private void OnDisable()
	{
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Remove(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.myGameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Remove(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.myGameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.myGameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		SpawnRegion<GameEntity, SIResourceRegion>.RemoveItemFromRegion(this.myGameEntity);
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002BA0C File Offset: 0x00029C0C
	public void GrabInitialization()
	{
		this.isSleeping = false;
		this.shouldSleep = false;
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0002BA1C File Offset: 0x00029C1C
	public void ReleaseInitialization()
	{
		this.shouldSleep = true;
		this.isSleeping = false;
		this.timeReleased = Time.time;
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0002BA37 File Offset: 0x00029C37
	public virtual bool CanDeposit()
	{
		return this.lastPlayerHeld != null && this.lastPlayerHeld.gamePlayer.IsLocal() && !this.localDeposited && SIPlayer.LocalPlayer.CanLimitedResourceBeDeposited(this.limitedDepositType);
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0002BA73 File Offset: 0x00029C73
	public virtual void HandleDepositLocal(SIPlayer depositingPlayer)
	{
		this.localDeposited = true;
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void HandleDepositAuth(SIPlayer depositingPlayer)
	{
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0002BA7C File Offset: 0x00029C7C
	private void HandleOnDestroyed(GameEntity entity)
	{
		if (!this.localEverGrabbed || this.localDeposited || !entity.manager.IsZoneActive() || !PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.type == SIResource.ResourceType.StrangeWood)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectStrangeWood", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.WeirdGear)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectWeirdGears", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.FloppyMetal)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectFloppyMetal", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.BouncySand)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectBouncySand", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.VibratingSpring)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectVibratingSpring", 1);
		}
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0002BB18 File Offset: 0x00029D18
	public static List<SIResource.ResourceCost> GetSum(params IList<SIResource.ResourceCost>[] costs)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>();
		if (costs == null)
		{
			return list;
		}
		foreach (IList<SIResource.ResourceCost> list2 in costs)
		{
			if (list2 != null)
			{
				foreach (SIResource.ResourceCost additiveCost in list2)
				{
					list.AddResourceCost(additiveCost);
				}
			}
		}
		return list;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0002BB8C File Offset: 0x00029D8C
	public static List<SIResource.ResourceCost> GetMax(params IList<SIResource.ResourceCost>[] costs)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>();
		if (costs == null)
		{
			return list;
		}
		for (int i = 0; i < costs.Length; i++)
		{
			foreach (SIResource.ResourceCost resourceCost in costs[i])
			{
				int amount = Mathf.Max(list.GetAmount(resourceCost.type), resourceCost.amount);
				list.SetAmount(resourceCost.type, amount);
			}
		}
		return list;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0002BC18 File Offset: 0x00029E18
	public static bool CategoryCostsMatch(IList<SIResource.ResourceCost> cost1, IList<SIResource.ResourceCost> cost2)
	{
		return cost1.GetCategoryCosts() == cost2.GetCategoryCosts();
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0002BC2C File Offset: 0x00029E2C
	public static bool CostsAreEqual(IList<SIResource.ResourceCost> cost1, IList<SIResource.ResourceCost> cost2, bool matchOrder = true)
	{
		if (cost1.Count != cost2.Count)
		{
			return false;
		}
		if (!matchOrder)
		{
			foreach (SIResource.ResourceCost resourceCost in cost1)
			{
				if (cost2.GetAmount(resourceCost.type) != resourceCost.amount)
				{
					return false;
				}
			}
			return true;
		}
		for (int i = 0; i < cost1.Count; i++)
		{
			if (!cost1[i].Equals(cost2[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040009D5 RID: 2517
	public SIPlayer lastPlayerHeld;

	// Token: 0x040009D6 RID: 2518
	public GameEntity myGameEntity;

	// Token: 0x040009D7 RID: 2519
	public SIResource.ResourceType type;

	// Token: 0x040009D8 RID: 2520
	public SIResource.LimitedDepositType limitedDepositType;

	// Token: 0x040009D9 RID: 2521
	public bool localDeposited;

	// Token: 0x040009DA RID: 2522
	public bool localEverGrabbed;

	// Token: 0x040009DB RID: 2523
	[Tooltip("The amount of pitch offset allowed during spawn, in degrees.  With this set to 0, item will always spawn aligned with surface.")]
	public float spawnPitchVariance;

	// Token: 0x040009DC RID: 2524
	public float sleepTime = 10f;

	// Token: 0x040009DD RID: 2525
	private bool shouldSleep = true;

	// Token: 0x040009DE RID: 2526
	private bool isSleeping;

	// Token: 0x040009DF RID: 2527
	private float timeReleased;

	// Token: 0x040009E0 RID: 2528
	private Rigidbody _rb;

	// Token: 0x0200012B RID: 299
	[Serializable]
	public struct ResourceCost : IComparable<SIResource.ResourceCost>, IEquatable<SIResource.ResourceCost>
	{
		// Token: 0x06000806 RID: 2054 RVA: 0x0002BCE6 File Offset: 0x00029EE6
		public ResourceCost(SIResource.ResourceType type, int amount)
		{
			this.type = type;
			this.amount = amount;
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0002BCF8 File Offset: 0x00029EF8
		public int CompareTo(SIResource.ResourceCost other)
		{
			int num = this.type.CompareTo(other.type);
			if (num != 0)
			{
				return num;
			}
			return this.amount.CompareTo(other.amount);
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0002BD38 File Offset: 0x00029F38
		public bool Equals(SIResource.ResourceCost other)
		{
			return this.type == other.type && this.amount == other.amount;
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0002BD58 File Offset: 0x00029F58
		public override bool Equals(object obj)
		{
			if (obj is SIResource.ResourceCost)
			{
				SIResource.ResourceCost other = (SIResource.ResourceCost)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0002BD7D File Offset: 0x00029F7D
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>((int)this.type, this.amount);
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0002BD90 File Offset: 0x00029F90
		public override string ToString()
		{
			return string.Format("{0}: {1}", this.type.ToString(), this.amount);
		}

		// Token: 0x040009E1 RID: 2529
		public SIResource.ResourceType type;

		// Token: 0x040009E2 RID: 2530
		public int amount;
	}

	// Token: 0x0200012C RID: 300
	public struct ResourceCategoryCost : IComparable<SIResource.ResourceCategoryCost>, IEquatable<SIResource.ResourceCategoryCost>
	{
		// Token: 0x0600080C RID: 2060 RVA: 0x0002BDB8 File Offset: 0x00029FB8
		public ResourceCategoryCost(int techPoints, int misc)
		{
			this.techPoints = techPoints;
			this.misc = misc;
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0002BDC8 File Offset: 0x00029FC8
		public int CompareTo(SIResource.ResourceCategoryCost other)
		{
			int num = this.techPoints.CompareTo(other.techPoints);
			if (num != 0)
			{
				return num;
			}
			return this.misc.CompareTo(other.misc);
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x0002BDFD File Offset: 0x00029FFD
		public bool Equals(SIResource.ResourceCategoryCost other)
		{
			return this.techPoints == other.techPoints && this.misc == other.misc;
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0002BE1D File Offset: 0x0002A01D
		public static bool operator ==(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0002BE27 File Offset: 0x0002A027
		public static bool operator !=(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0002BE34 File Offset: 0x0002A034
		public static SIResource.ResourceCategoryCost operator +(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(left.techPoints + right.techPoints, left.misc + right.misc);
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x0002BE55 File Offset: 0x0002A055
		public static SIResource.ResourceCategoryCost operator -(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(left.techPoints - right.techPoints, left.misc - right.misc);
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0002BE76 File Offset: 0x0002A076
		public static SIResource.ResourceCategoryCost operator *(SIResource.ResourceCategoryCost cost, int multiple)
		{
			return new SIResource.ResourceCategoryCost(cost.techPoints * multiple, cost.misc * multiple);
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x0002BE8D File Offset: 0x0002A08D
		public static SIResource.ResourceCategoryCost operator *(int multiple, SIResource.ResourceCategoryCost cost)
		{
			return new SIResource.ResourceCategoryCost(cost.techPoints * multiple, cost.misc * multiple);
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x0002BEA4 File Offset: 0x0002A0A4
		public static SIResource.ResourceCategoryCost Max(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(Mathf.Max(left.techPoints, right.techPoints), Mathf.Max(left.misc, right.misc));
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0002BECD File Offset: 0x0002A0CD
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>(this.techPoints, this.misc);
		}

		// Token: 0x040009E3 RID: 2531
		public int techPoints;

		// Token: 0x040009E4 RID: 2532
		public int misc;
	}

	// Token: 0x0200012D RID: 301
	public enum ResourceType
	{
		// Token: 0x040009E6 RID: 2534
		TechPoint,
		// Token: 0x040009E7 RID: 2535
		StrangeWood,
		// Token: 0x040009E8 RID: 2536
		WeirdGear,
		// Token: 0x040009E9 RID: 2537
		VibratingSpring,
		// Token: 0x040009EA RID: 2538
		BouncySand,
		// Token: 0x040009EB RID: 2539
		FloppyMetal,
		// Token: 0x040009EC RID: 2540
		Count
	}

	// Token: 0x0200012E RID: 302
	public enum LimitedDepositType
	{
		// Token: 0x040009EE RID: 2542
		None,
		// Token: 0x040009EF RID: 2543
		MonkeIdol,
		// Token: 0x040009F0 RID: 2544
		Count
	}
}
