using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E1 RID: 225
[RequireComponent(typeof(GameEntity))]
public abstract class SIGadget : MonoBehaviour, IGameEntityComponent, IPrefabRequirements, IGameActivatable, IGameStateProvider
{
	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000563 RID: 1379 RVA: 0x0001F89F File Offset: 0x0001DA9F
	// (set) Token: 0x06000564 RID: 1380 RVA: 0x0001F8A7 File Offset: 0x0001DAA7
	public SITechTreePageId PageId
	{
		get
		{
			return this.pageId;
		}
		set
		{
			this.pageId = value;
		}
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x06000565 RID: 1381 RVA: 0x0001F8B0 File Offset: 0x0001DAB0
	public IEnumerable<GameEntity> RequiredPrefabs
	{
		get
		{
			return this.additionalRequiredPrefabs;
		}
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001F8B8 File Offset: 0x0001DAB8
	protected virtual void Update()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0001F8F2 File Offset: 0x0001DAF2
	protected virtual void OnUpdateAuthority(float dt)
	{
		this.SleepAfterDelay();
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0001F8F2 File Offset: 0x0001DAF2
	protected virtual void OnUpdateRemote(float dt)
	{
		this.SleepAfterDelay();
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0001F8FA File Offset: 0x0001DAFA
	protected virtual bool IsEquippedLocal()
	{
		return this.gameEntity.IsHeldByLocalPlayer() || this.gameEntity.IsSnappedByLocalPlayer();
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0001F918 File Offset: 0x0001DB18
	protected Vector2 GetJoystickInput()
	{
		if (!this.ShouldProcessInput())
		{
			return default(Vector2);
		}
		return ControllerInputPoller.Primary2DAxis((this.gameEntity.heldByHandIndex == 0 || this.gameEntity.snappedJoint == SnapJointType.HandL) ? 4 : 5);
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0001F960 File Offset: 0x0001DB60
	protected bool ShouldProcessInput()
	{
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			return true;
		}
		GamePlayer gamePlayer;
		if (this.gameEntity.IsSnappedByLocalPlayer() && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer))
		{
			SnapJointType snappedJoint = this.gameEntity.snappedJoint;
			GameEntity gameEntity;
			if (snappedJoint != SnapJointType.HandL)
			{
				if (snappedJoint != SnapJointType.HandR)
				{
					gameEntity = null;
				}
				else
				{
					gameEntity = gamePlayer.GetGrabbedGameEntity(1);
				}
			}
			else
			{
				gameEntity = gamePlayer.GetGrabbedGameEntity(0);
			}
			GameEntity gameEntity2 = gameEntity;
			return !gameEntity2 || gameEntity2.GetComponent<IGameActivatable>() == null;
		}
		return false;
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001F9E0 File Offset: 0x0001DBE0
	public void SleepAfterDelay()
	{
		if (this.isSleeping || !this.shouldSleep)
		{
			return;
		}
		if (Time.time < this.timeReleased + this.sleepTime)
		{
			return;
		}
		base.GetComponent<Rigidbody>().isKinematic = true;
		this.isSleeping = true;
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0001FA1B File Offset: 0x0001DC1B
	public virtual SIUpgradeSet FilterUpgradeNodes(SIUpgradeSet upgrades)
	{
		return upgrades;
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0001FA20 File Offset: 0x0001DC20
	public virtual void RefreshUpgradeVisuals(SIUpgradeSet withUpgrades)
	{
		foreach (SIGadget.UpgradeVisual upgradeVisual in this.UpgradeBasedVisuals)
		{
			upgradeVisual.Update(withUpgrades);
		}
		Action<SIUpgradeSet> onPostRefreshVisuals = this.OnPostRefreshVisuals;
		if (onPostRefreshVisuals == null)
		{
			return;
		}
		onPostRefreshVisuals.Invoke(withUpgrades);
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0001FA64 File Offset: 0x0001DC64
	protected virtual void OnEnable()
	{
		if (!this.didApplyId)
		{
			GameObject gameObject = base.gameObject;
			gameObject.name = gameObject.name + "[" + SIGadget.uniqueId.ToString() + "]";
			this.didApplyId = true;
			SIGadget.uniqueId++;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Combine(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.timeReleased = Time.time;
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0001FB60 File Offset: 0x0001DD60
	protected virtual void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Remove(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Remove(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.LeaveAllExclusionZones();
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0001FC10 File Offset: 0x0001DE10
	public void GrabInitialization()
	{
		this.isSleeping = false;
		this.shouldSleep = false;
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		SuperInfectionManager component = this.gameEntity.manager.GetComponent<SuperInfectionManager>();
		if (((component != null) ? component.zoneSuperInfection : null) == null)
		{
			return;
		}
		bool isMine = SIPlayer.LocalPlayer.activePlayerGadgets.Contains(this.gameEntity.GetNetId());
		SIProgression.Instance.UpdateHeldGadgetsTelemetry(this.PageId, isMine, 1);
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0001FC8C File Offset: 0x0001DE8C
	public void ReleaseInitialization()
	{
		this.shouldSleep = true;
		this.isSleeping = false;
		this.timeReleased = Time.time;
		if (!this.gameEntity.WasLastHeldByLocalPlayer())
		{
			return;
		}
		SuperInfectionManager component = this.gameEntity.manager.GetComponent<SuperInfectionManager>();
		if (((component != null) ? component.zoneSuperInfection : null) == null)
		{
			return;
		}
		bool isMine = SIPlayer.LocalPlayer.activePlayerGadgets.Contains(this.gameEntity.GetNetId());
		SIProgression.Instance.UpdateHeldGadgetsTelemetry(this.PageId, isMine, -1);
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x0001FD14 File Offset: 0x0001DF14
	public bool FindAttachedHand(out bool isLeft, bool checkHeld = true, bool checkSnapped = true)
	{
		isLeft = false;
		int num = -1;
		GamePlayer gamePlayer;
		if (checkHeld && GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		GamePlayer gamePlayer2;
		if (num == -1 && checkSnapped && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			num = gamePlayer2.FindSnapIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return false;
		}
		isLeft = GamePlayer.IsLeftHand(num);
		return true;
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001FD8C File Offset: 0x0001DF8C
	public int GetAttachedPlayerActorNumber()
	{
		if (this.gameEntity.heldByActorNumber == -1)
		{
			return this.gameEntity.snappedByActorNumber;
		}
		return this.gameEntity.heldByActorNumber;
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnEntityInit()
	{
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnEntityDestroy()
	{
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001FDB4 File Offset: 0x0001DFB4
	public virtual void OnEntityStateChange(long prevState, long newState)
	{
		foreach (IGameStateReceiver gameStateReceiver in this._gameStateReceivers)
		{
			gameStateReceiver.GameStateReceiverOnStateChanged(prevState, newState);
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ProcessAuthorityToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0001FE08 File Offset: 0x0001E008
	public void SendClientToAuthorityRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0001FE60 File Offset: 0x0001E060
	public void SendClientToAuthorityRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0001FEBC File Offset: 0x0001E0BC
	public void SendAuthorityToClientRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.AuthorityToClientRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0001FF14 File Offset: 0x0001E114
	public void SendAuthorityToClientRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.AuthorityToClientRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0001FF70 File Offset: 0x0001E170
	public void SendClientToClientRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0001FFC8 File Offset: 0x0001E1C8
	public void SendClientToClientRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x00020021 File Offset: 0x0001E221
	public void ApplyExclusionZone(SIExclusionZone exclusionZone)
	{
		if (!this.appliedExclusionZones.Contains(exclusionZone))
		{
			if (this.appliedExclusionZones.Count == 0)
			{
				this.appliedExclusionZones.Add(exclusionZone);
				this.HandleBlockedActionChanged(true);
				return;
			}
			this.appliedExclusionZones.Add(exclusionZone);
		}
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0002005E File Offset: 0x0001E25E
	public void LeaveExclusionZone(SIExclusionZone exclusionZone)
	{
		if (this.appliedExclusionZones.Contains(exclusionZone))
		{
			this.appliedExclusionZones.Remove(exclusionZone);
			if (this.appliedExclusionZones.Count == 0)
			{
				this.HandleBlockedActionChanged(false);
			}
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00020090 File Offset: 0x0001E290
	private void LeaveAllExclusionZones()
	{
		foreach (SIExclusionZone siexclusionZone in this.appliedExclusionZones)
		{
			if (siexclusionZone != null)
			{
				siexclusionZone.ClearGadget(this);
			}
		}
		this.appliedExclusionZones.Clear();
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x000200F8 File Offset: 0x0001E2F8
	protected bool IsBlocked()
	{
		return this.appliedExclusionZones.Count > 0;
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void HandleBlockedActionChanged(bool isBlocked)
	{
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00020108 File Offset: 0x0001E308
	void IGameStateProvider.GameStateReceiverRegister(IGameStateReceiver receiver)
	{
		this._gameStateReceivers.Add(receiver);
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00020116 File Offset: 0x0001E316
	void IGameStateProvider.GameStateReceiverUnregister(IGameStateReceiver receiver)
	{
		this._gameStateReceivers.Remove(receiver);
	}

	// Token: 0x040006E0 RID: 1760
	public GameEntity gameEntity;

	// Token: 0x040006E1 RID: 1761
	[Tooltip("Add additional required prefabs here.  These will be automatically added to the GameEntityManager factory.")]
	public GameEntity[] additionalRequiredPrefabs;

	// Token: 0x040006E2 RID: 1762
	public float sleepTime = 10f;

	// Token: 0x040006E3 RID: 1763
	private bool shouldSleep = true;

	// Token: 0x040006E4 RID: 1764
	private bool isSleeping;

	// Token: 0x040006E5 RID: 1765
	private float timeReleased;

	// Token: 0x040006E6 RID: 1766
	protected bool activatedLocally;

	// Token: 0x040006E7 RID: 1767
	[SerializeField]
	private SITechTreePageId pageId;

	// Token: 0x040006E8 RID: 1768
	public Action<SIUpgradeSet> OnPostRefreshVisuals;

	// Token: 0x040006E9 RID: 1769
	private static int uniqueId = 101;

	// Token: 0x040006EA RID: 1770
	private bool didApplyId;

	// Token: 0x040006EB RID: 1771
	[SerializeField]
	private SIGadget.UpgradeVisual[] UpgradeBasedVisuals;

	// Token: 0x040006EC RID: 1772
	private readonly List<SIExclusionZone> appliedExclusionZones = new List<SIExclusionZone>();

	// Token: 0x040006ED RID: 1773
	private List<IGameStateReceiver> _gameStateReceivers = new List<IGameStateReceiver>();

	// Token: 0x020000E2 RID: 226
	[Serializable]
	private struct UpgradeVisual
	{
		// Token: 0x0600058B RID: 1419 RVA: 0x00020160 File Offset: 0x0001E360
		public void Update(SIUpgradeSet withUpgrades)
		{
			bool flag = true;
			if (this.appearRequirements.Length != 0)
			{
				flag = false;
				foreach (SIUpgradeType upgrade in this.appearRequirements)
				{
					if (withUpgrades.Contains(upgrade))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				foreach (SIUpgradeType upgrade2 in this.disappearRequirements)
				{
					if (withUpgrades.Contains(upgrade2))
					{
						flag = false;
						break;
					}
				}
			}
			GameObject[] array2 = this.objects;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].SetActive(flag);
			}
		}

		// Token: 0x040006EE RID: 1774
		public GameObject[] objects;

		// Token: 0x040006EF RID: 1775
		[Tooltip("For the objects to become activated, you must match AT LEAST ONE appearRequirement (if there are any), and not match any disappearRequirements.")]
		public SIUpgradeType[] appearRequirements;

		// Token: 0x040006F0 RID: 1776
		[Tooltip("For the objects to become deactivated, you must match AT LEAST ONE disappearRequirement (if there are any).")]
		public SIUpgradeType[] disappearRequirements;
	}
}
