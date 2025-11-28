using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000C1 RID: 193
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
[RequireComponent(typeof(SIGadgetBlasterType))]
public class SIGadgetBlaster : SIGadget, ITickSystemTick
{
	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060004AF RID: 1199 RVA: 0x0001B24F File Offset: 0x0001944F
	public bool LocalEquippedOrActivated
	{
		get
		{
			return this.IsEquippedLocal() || this.activatedLocally;
		}
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060004B0 RID: 1200 RVA: 0x0001B261 File Offset: 0x00019461
	// (set) Token: 0x060004B1 RID: 1201 RVA: 0x0001B269 File Offset: 0x00019469
	public bool TickRunning { get; set; }

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001B274 File Offset: 0x00019474
	protected override void OnEnable()
	{
		base.OnEnable();
		this.blasterType = base.GetComponent<SIGadgetBlasterType>();
		this.lastFired = 0f;
		this.environmentLayerMask = GTPlayer.Instance.locomotionEnabledLayers;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartGrabbing));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.StartGrabbing));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.StopGrabbing));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.StopGrabbing));
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001B350 File Offset: 0x00019550
	private new void OnDisable()
	{
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001B360 File Offset: 0x00019560
	public void Tick()
	{
		if (SIGadgetBlaster.projectilesToDespawn.Count <= 0)
		{
			return;
		}
		if (Time.time < SIGadgetBlaster.projectilesToDespawnTimes.Peek() + 1f)
		{
			return;
		}
		SIGadgetBlasterProjectile sigadgetBlasterProjectile = SIGadgetBlaster.projectilesToDespawn.Dequeue();
		SIGadgetBlaster.activeProjectiles.RemoveIfContains(sigadgetBlasterProjectile);
		if (sigadgetBlasterProjectile == null || sigadgetBlasterProjectile.gameObject == null)
		{
			return;
		}
		SIGadgetBlaster.blasterProjectilePools[sigadgetBlasterProjectile.poolId].Add(sigadgetBlasterProjectile.gameObject);
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001B3DC File Offset: 0x000195DC
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		this.blasterType.OnUpdateAuthority(dt);
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0001B3F4 File Offset: 0x000195F4
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetBlasterState sigadgetBlasterState = (SIGadgetBlasterState)this.gameEntity.GetState();
		if (sigadgetBlasterState != this.currentState)
		{
			this.SetStateShared(sigadgetBlasterState);
		}
		this.blasterType.OnUpdateRemote(dt);
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0001B431 File Offset: 0x00019631
	public void SetStateAuthority(SIGadgetBlasterState newState)
	{
		this.SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0001B452 File Offset: 0x00019652
	private void SetStateShared(SIGadgetBlasterState newState)
	{
		if (newState == this.currentState || !SIGadgetBlaster.CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetBlasterState sigadgetBlasterState = this.currentState;
		this.currentState = newState;
		this.blasterType.SetStateShared();
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001B480 File Offset: 0x00019680
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.blasterType.ApplyUpgradeNodes(withUpgrades);
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0001B48E File Offset: 0x0001968E
	public static bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L;
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0001B49C File Offset: 0x0001969C
	public bool CheckInput()
	{
		float sensitivity = this.wasActivated ? this.inputActivateThreshold : this.inputDeactivateThreshold;
		this.wasActivated = this.buttonActivatable.CheckInput(true, true, sensitivity, true, true);
		return this.wasActivated;
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001B4DC File Offset: 0x000196DC
	public int NextFireId()
	{
		int num = this.projectileId;
		this.projectileId = num + 1;
		return num;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001B4FC File Offset: 0x000196FC
	public override void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID != 0)
		{
			if (rpcID != 1)
			{
				return;
			}
			if (data == null || data.Length < 2)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			SIGadgetBlasterProjectile sigadgetBlasterProjectile = null;
			for (int i = 0; i < SIGadgetBlaster.activeProjectiles.Count; i++)
			{
				if (SIGadgetBlaster.activeProjectiles[i].projectileId == num)
				{
					sigadgetBlasterProjectile = SIGadgetBlaster.activeProjectiles[i];
					break;
				}
			}
			if (sigadgetBlasterProjectile == null)
			{
				return;
			}
			if (sigadgetBlasterProjectile.firedByPlayer != SIPlayer.Get(info.Sender.ActorNumber))
			{
				return;
			}
			sigadgetBlasterProjectile.GetComponent<SIGadgetProjectileType>().NetworkedProjectileHit(data);
			return;
		}
		else
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			this.blasterType.NetworkFireProjectile(data);
			return;
		}
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001B5C4 File Offset: 0x000197C4
	public void StartGrabbing()
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			this.SetStateAuthority(SIGadgetBlasterState.Idle);
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001B5DD File Offset: 0x000197DD
	public void StopGrabbing()
	{
		this.SetStateShared(SIGadgetBlasterState.Idle);
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001B5E6 File Offset: 0x000197E6
	public void DespawnProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.gameObject.SetActive(false);
		if (!SIGadgetBlaster.projectilesToDespawn.Contains(projectile))
		{
			SIGadgetBlaster.projectilesToDespawn.Enqueue(projectile);
			SIGadgetBlaster.projectilesToDespawnTimes.Enqueue(Time.time);
		}
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001B61C File Offset: 0x0001981C
	public GameObject InstantiateProjectile(SIGadgetBlasterProjectile projectilePrefab, Vector3 position, Quaternion rotation, int thisFireId)
	{
		if (SIGadgetBlaster.blasterProjectilePools == null)
		{
			SIGadgetBlaster.blasterProjectilePools = new Dictionary<int, List<GameObject>>();
		}
		int instanceID = projectilePrefab.GetInstanceID();
		if (!SIGadgetBlaster.blasterProjectilePools.ContainsKey(instanceID))
		{
			SIGadgetBlaster.blasterProjectilePools.Add(instanceID, new List<GameObject>());
		}
		List<GameObject> list = SIGadgetBlaster.blasterProjectilePools[instanceID];
		GameObject gameObject;
		if (list.Count <= 0)
		{
			gameObject = Object.Instantiate<GameObject>(projectilePrefab.gameObject, position, rotation);
		}
		else
		{
			gameObject = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			gameObject.SetActive(true);
		}
		SIGadgetBlasterProjectile component = gameObject.GetComponent<SIGadgetBlasterProjectile>();
		component.transform.position = position;
		component.transform.rotation = rotation;
		component.parentBlaster = this;
		component.projectileId = thisFireId;
		component.firedByPlayer = (this.gameEntity.IsHeld() ? SIPlayer.Get(this.gameEntity.heldByActorNumber) : SIPlayer.Get(this.gameEntity.snappedByActorNumber));
		component.poolId = instanceID;
		SIGadgetBlaster.activeProjectiles.Add(component);
		this.lastFired = Time.time;
		component.InitializeProjectile();
		return gameObject;
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001B72E File Offset: 0x0001992E
	public void FireProjectileHaptics(float strength, float duration)
	{
		GorillaTagger.Instance.StartVibration(this.gameEntity.EquippedHandedness == EHandedness.Left, strength, duration);
	}

	// Token: 0x0400057D RID: 1405
	[OnEnterPlay_SetNull]
	public static Dictionary<int, List<GameObject>> blasterProjectilePools;

	// Token: 0x0400057E RID: 1406
	[NonSerialized]
	public const float PROJECTILE_MAX_LATENCY = 1f;

	// Token: 0x0400057F RID: 1407
	private SIGadgetBlasterType blasterType;

	// Token: 0x04000580 RID: 1408
	[NonSerialized]
	public SIGadgetBlasterState currentState;

	// Token: 0x04000582 RID: 1410
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x04000583 RID: 1411
	[SerializeField]
	private float inputActivateThreshold = 0.35f;

	// Token: 0x04000584 RID: 1412
	[SerializeField]
	private float inputDeactivateThreshold = 0.25f;

	// Token: 0x04000585 RID: 1413
	public int maxProjectileCount = 10;

	// Token: 0x04000586 RID: 1414
	public float maxLagDistance = 5f;

	// Token: 0x04000587 RID: 1415
	private bool wasActivated;

	// Token: 0x04000588 RID: 1416
	[NonSerialized]
	public float lastFired;

	// Token: 0x04000589 RID: 1417
	[NonSerialized]
	public int projectileCount;

	// Token: 0x0400058A RID: 1418
	private int projectileId;

	// Token: 0x0400058B RID: 1419
	[NonSerialized]
	public static List<SIGadgetBlasterProjectile> activeProjectiles = new List<SIGadgetBlasterProjectile>();

	// Token: 0x0400058C RID: 1420
	[NonSerialized]
	public static Queue<SIGadgetBlasterProjectile> projectilesToDespawn = new Queue<SIGadgetBlasterProjectile>();

	// Token: 0x0400058D RID: 1421
	[NonSerialized]
	public static Queue<float> projectilesToDespawnTimes = new Queue<float>();

	// Token: 0x0400058E RID: 1422
	public Transform firingPosition;

	// Token: 0x0400058F RID: 1423
	public AudioSource firingSource;

	// Token: 0x04000590 RID: 1424
	public AudioSource blasterSource;

	// Token: 0x04000591 RID: 1425
	[NonSerialized]
	public LayerMask environmentLayerMask;

	// Token: 0x020000C2 RID: 194
	public enum RPCCalls
	{
		// Token: 0x04000593 RID: 1427
		FireProjectile,
		// Token: 0x04000594 RID: 1428
		ProjectileHitPlayer
	}
}
