using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000605 RID: 1541
public class GameEntity : MonoBehaviour
{
	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x060026D3 RID: 9939 RVA: 0x000CE49F File Offset: 0x000CC69F
	// (set) Token: 0x060026D4 RID: 9940 RVA: 0x000CE4A7 File Offset: 0x000CC6A7
	[DebugReadout]
	public GameEntityId id { get; internal set; }

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x060026D5 RID: 9941 RVA: 0x000CE4B0 File Offset: 0x000CC6B0
	// (set) Token: 0x060026D6 RID: 9942 RVA: 0x000CE4B8 File Offset: 0x000CC6B8
	[DebugReadout]
	public int typeId { get; private set; }

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x060026D7 RID: 9943 RVA: 0x000CE4C1 File Offset: 0x000CC6C1
	// (set) Token: 0x060026D8 RID: 9944 RVA: 0x000CE4C9 File Offset: 0x000CC6C9
	[DebugReadout]
	public long createData { get; set; }

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x060026D9 RID: 9945 RVA: 0x000CE4D2 File Offset: 0x000CC6D2
	// (set) Token: 0x060026DA RID: 9946 RVA: 0x000CE4DA File Offset: 0x000CC6DA
	[DebugReadout]
	public int heldByActorNumber { get; internal set; }

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x060026DB RID: 9947 RVA: 0x000CE4E3 File Offset: 0x000CC6E3
	// (set) Token: 0x060026DC RID: 9948 RVA: 0x000CE4EB File Offset: 0x000CC6EB
	[DebugReadout]
	public int snappedByActorNumber { get; internal set; }

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x060026DD RID: 9949 RVA: 0x000CE4F4 File Offset: 0x000CC6F4
	// (set) Token: 0x060026DE RID: 9950 RVA: 0x000CE4FC File Offset: 0x000CC6FC
	[DebugReadout]
	public SnapJointType snappedJoint { get; internal set; }

	// Token: 0x170003DF RID: 991
	// (get) Token: 0x060026DF RID: 9951 RVA: 0x000CE505 File Offset: 0x000CC705
	// (set) Token: 0x060026E0 RID: 9952 RVA: 0x000CE50D File Offset: 0x000CC70D
	[DebugReadout]
	public int heldByHandIndex { get; internal set; }

	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x060026E1 RID: 9953 RVA: 0x000CE516 File Offset: 0x000CC716
	// (set) Token: 0x060026E2 RID: 9954 RVA: 0x000CE51E File Offset: 0x000CC71E
	[DebugReadout]
	public int lastHeldByActorNumber { get; internal set; }

	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x060026E3 RID: 9955 RVA: 0x000CE527 File Offset: 0x000CC727
	// (set) Token: 0x060026E4 RID: 9956 RVA: 0x000CE52F File Offset: 0x000CC72F
	[DebugReadout]
	public int onlyGrabActorNumber { get; internal set; }

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x060026E5 RID: 9957 RVA: 0x000CE538 File Offset: 0x000CC738
	// (set) Token: 0x060026E6 RID: 9958 RVA: 0x000CE540 File Offset: 0x000CC740
	[DebugReadout]
	public GameEntityId attachedToEntityId { get; internal set; }

	// Token: 0x1400004A RID: 74
	// (add) Token: 0x060026E7 RID: 9959 RVA: 0x000CE54C File Offset: 0x000CC74C
	// (remove) Token: 0x060026E8 RID: 9960 RVA: 0x000CE584 File Offset: 0x000CC784
	public event GameEntity.StateChangedEvent OnStateChanged;

	// Token: 0x1400004B RID: 75
	// (add) Token: 0x060026E9 RID: 9961 RVA: 0x000CE5BC File Offset: 0x000CC7BC
	// (remove) Token: 0x060026EA RID: 9962 RVA: 0x000CE5F4 File Offset: 0x000CC7F4
	public event GameEntity.EntityDestroyedEvent onEntityDestroyed;

	// Token: 0x060026EB RID: 9963 RVA: 0x000CE62C File Offset: 0x000CC82C
	private void Awake()
	{
		this.id = GameEntityId.Invalid;
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.heldByActorNumber = -1;
		this.heldByHandIndex = -1;
		this.onlyGrabActorNumber = -1;
		this.snappedByActorNumber = -1;
		this.attachedToEntityId = GameEntityId.Invalid;
		this.entityComponents = new List<IGameEntityComponent>(1);
		base.GetComponentsInChildren<IGameEntityComponent>(this.entityComponents);
		this.entitySerialize = new List<IGameEntitySerialize>(1);
		base.GetComponentsInChildren<IGameEntitySerialize>(this.entitySerialize);
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x000CE6A7 File Offset: 0x000CC8A7
	public void Create(GameEntityManager manager, int typeId)
	{
		this.manager = manager;
		this.typeId = typeId;
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000CE6B8 File Offset: 0x000CC8B8
	public void Init(long createData)
	{
		this.createData = createData;
		for (int i = 0; i < this.entityComponents.Count; i++)
		{
			this.entityComponents[i].OnEntityInit();
		}
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x000CE6F4 File Offset: 0x000CC8F4
	public void OnDestroy()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < this.entityComponents.Count; i++)
		{
			this.entityComponents[i].OnEntityDestroy();
		}
		GameEntity.EntityDestroyedEvent entityDestroyedEvent = this.onEntityDestroyed;
		if (entityDestroyedEvent == null)
		{
			return;
		}
		entityDestroyedEvent(this);
	}

	// Token: 0x060026EF RID: 9967 RVA: 0x000CE741 File Offset: 0x000CC941
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.linearVelocity;
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x000CE762 File Offset: 0x000CC962
	public void PlayCatchFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.GTPlayOneShot(this.catchSound, 1f);
		}
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x000CE799 File Offset: 0x000CC999
	public void PlayThrowFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.GTPlayOneShot(this.throwSound, 1f);
		}
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000CE7D0 File Offset: 0x000CC9D0
	public void PlaySnapFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.snapSoundVolume;
			this.audioSource.GTPlayOneShot(this.snapSound, 1f);
		}
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000CE807 File Offset: 0x000CCA07
	private bool IsGamePlayer(Collider collider)
	{
		return GamePlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x000CE816 File Offset: 0x000CCA16
	public long GetState()
	{
		return this.state;
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000CE81E File Offset: 0x000CCA1E
	public void RequestState(GameEntityId id, long newState)
	{
		this.manager.RequestState(id, newState);
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000CE82D File Offset: 0x000CCA2D
	public bool IsAuthority()
	{
		return this.manager.IsAuthority();
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x000CE83A File Offset: 0x000CCA3A
	public bool IsValidToMigrate()
	{
		return this.manager.IsEntityValidToMigrate(this);
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x000CE848 File Offset: 0x000CCA48
	public void SetState(long newState)
	{
		if (this.state != newState)
		{
			long prevState = this.state;
			this.state = newState;
			GameEntity.StateChangedEvent onStateChanged = this.OnStateChanged;
			if (onStateChanged != null)
			{
				onStateChanged(prevState, newState);
			}
			for (int i = 0; i < this.entityComponents.Count; i++)
			{
				this.entityComponents[i].OnEntityStateChange(prevState, newState);
			}
		}
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x000CE8A8 File Offset: 0x000CCAA8
	public GameEntityId MigrateToEntityManager(GameEntityManager newManager)
	{
		this.manager.RemoveGameEntity(this);
		this.manager = newManager;
		GameEntityId gameEntityId = newManager.AddGameEntity(this);
		this.id = gameEntityId;
		this.manager.InitItemLocal(this, this.createData);
		return gameEntityId;
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x000CE8EA File Offset: 0x000CCAEA
	public void MigrateHeldBy(int actorNumber)
	{
		if (this.heldByActorNumber >= 0)
		{
			this.heldByActorNumber = actorNumber;
		}
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000CE8FC File Offset: 0x000CCAFC
	public void MigrateSnappedBy(int actorNumber)
	{
		if (this.snappedByActorNumber >= 0)
		{
			this.snappedByActorNumber = actorNumber;
		}
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x000CE90E File Offset: 0x000CCB0E
	public int GetNetId(GameEntityId gameEntityId)
	{
		return this.manager.GetNetIdFromEntityId(gameEntityId);
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000CE91C File Offset: 0x000CCB1C
	public int GetNetId()
	{
		return this.manager.GetNetIdFromEntityId(this.id);
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000CE930 File Offset: 0x000CCB30
	public static GameEntity Get(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameEntity component = transform.GetComponent<GameEntity>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000CE974 File Offset: 0x000CCB74
	public bool IsHeldByLocalPlayer()
	{
		return this.heldByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x000CE98D File Offset: 0x000CCB8D
	public bool IsSnappedByLocalPlayer()
	{
		return this.snappedByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x000CE9A6 File Offset: 0x000CCBA6
	public bool IsHeld()
	{
		return this.heldByActorNumber != -1;
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000CE9B4 File Offset: 0x000CCBB4
	public int GetLastHeldByPlayerForEntityID(GameEntityId gameEntityId)
	{
		GameEntity gameEntity = this.manager.GetGameEntity(gameEntityId);
		if (gameEntity != null)
		{
			return gameEntity.lastHeldByActorNumber;
		}
		return 0;
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x000CE9DF File Offset: 0x000CCBDF
	public bool WasLastHeldByLocalPlayer()
	{
		return this.lastHeldByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000CE9F8 File Offset: 0x000CCBF8
	public bool IsAttachedToPlayer(NetPlayer player)
	{
		return player != null && (this.heldByActorNumber == player.ActorNumber || this.snappedByActorNumber == player.ActorNumber);
	}

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x06002705 RID: 9989 RVA: 0x000CEA1D File Offset: 0x000CCC1D
	public EHandedness EquippedHandedness
	{
		get
		{
			if (this.heldByHandIndex == 0 || (this.snappedJoint & SnapJointType.HandL) != SnapJointType.None)
			{
				return EHandedness.Left;
			}
			if (this.heldByHandIndex != 1 && (this.snappedJoint & SnapJointType.HandR) == SnapJointType.None)
			{
				return EHandedness.None;
			}
			return EHandedness.Right;
		}
	}

	// Token: 0x0400329E RID: 12958
	public const int Invalid = -1;

	// Token: 0x040032A2 RID: 12962
	public bool pickupable = true;

	// Token: 0x040032A3 RID: 12963
	public float pickupRangeFromSurface;

	// Token: 0x040032A4 RID: 12964
	public bool canHoldingPlayerUpdateState;

	// Token: 0x040032A5 RID: 12965
	public bool canLastHoldingPlayerUpdateState;

	// Token: 0x040032A6 RID: 12966
	public bool canSnapPlayerUpdateState;

	// Token: 0x040032A7 RID: 12967
	public AudioSource audioSource;

	// Token: 0x040032A8 RID: 12968
	public AudioClip catchSound;

	// Token: 0x040032A9 RID: 12969
	public float catchSoundVolume = 0.5f;

	// Token: 0x040032AA RID: 12970
	public AudioClip throwSound;

	// Token: 0x040032AB RID: 12971
	public float throwSoundVolume = 0.5f;

	// Token: 0x040032AC RID: 12972
	public AudioClip snapSound;

	// Token: 0x040032AD RID: 12973
	public float snapSoundVolume = 0.5f;

	// Token: 0x040032AE RID: 12974
	private Rigidbody rigidBody;

	// Token: 0x040032B6 RID: 12982
	[NonSerialized]
	public GameEntityManager manager;

	// Token: 0x040032B7 RID: 12983
	public Action OnGrabbed;

	// Token: 0x040032B8 RID: 12984
	public Action OnReleased;

	// Token: 0x040032B9 RID: 12985
	public Action OnSnapped;

	// Token: 0x040032BA RID: 12986
	public Action OnUnsnapped;

	// Token: 0x040032BB RID: 12987
	public Action OnAttached;

	// Token: 0x040032BC RID: 12988
	public Action OnDetached;

	// Token: 0x040032BD RID: 12989
	public Action OnTick;

	// Token: 0x040032BE RID: 12990
	public float MinTimeBetweenTicks;

	// Token: 0x040032BF RID: 12991
	[NonSerialized]
	public float LastTickTime;

	// Token: 0x040032C2 RID: 12994
	private long state;

	// Token: 0x040032C3 RID: 12995
	private List<IGameEntityComponent> entityComponents;

	// Token: 0x040032C4 RID: 12996
	public List<IGameEntitySerialize> entitySerialize;

	// Token: 0x02000606 RID: 1542
	// (Invoke) Token: 0x06002708 RID: 9992
	public delegate void StateChangedEvent(long prevState, long nextState);

	// Token: 0x02000607 RID: 1543
	// (Invoke) Token: 0x0600270C RID: 9996
	public delegate void EntityDestroyedEvent(GameEntity entity);
}
