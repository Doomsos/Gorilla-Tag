using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class SIGadgetTapTeleporter : SIGadget
{
	// Token: 0x17000061 RID: 97
	// (get) Token: 0x0600061C RID: 1564 RVA: 0x00022C3C File Offset: 0x00020E3C
	// (set) Token: 0x0600061D RID: 1565 RVA: 0x00022C44 File Offset: 0x00020E44
	public Color identifierColor { get; private set; }

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x0600061E RID: 1566 RVA: 0x00022C4D File Offset: 0x00020E4D
	// (set) Token: 0x0600061F RID: 1567 RVA: 0x00022C55 File Offset: 0x00020E55
	public bool useStealthTeleporters { get; private set; }

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000620 RID: 1568 RVA: 0x00022C5E File Offset: 0x00020E5E
	// (set) Token: 0x06000621 RID: 1569 RVA: 0x00022C66 File Offset: 0x00020E66
	public bool isVelocityPreserved { get; private set; }

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000622 RID: 1570 RVA: 0x00022C6F File Offset: 0x00020E6F
	// (set) Token: 0x06000623 RID: 1571 RVA: 0x00022C77 File Offset: 0x00020E77
	public bool hasInfiniteDuration { get; private set; }

	// Token: 0x06000624 RID: 1572 RVA: 0x00022C80 File Offset: 0x00020E80
	public override void OnEntityInit()
	{
		this.gameEntity.OnStateChanged += this.HandleStateChanged;
		this.gameEntity.onEntityDestroyed += this.HandleOnDestroyed;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.HandleHandAttached));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.HandleHandAttached));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.HandleHandDetach));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.HandleHandDetach));
		this.identifierColor = this.GenerateColor(this.gameEntity.GetNetId());
		this.ApplyIdentifierColor();
		this.UpdateNextSelectionDisplay();
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00022D7C File Offset: 0x00020F7C
	private void HandleOnDestroyed(GameEntity entity)
	{
		if (this.gameEntity.IsAuthority())
		{
			if (this._selection1Teleport)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection1Teleport.gameEntity.id);
			}
			if (this._selection2Teleport)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection2Teleport.gameEntity.id);
			}
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00022DF0 File Offset: 0x00020FF0
	private new void OnDisable()
	{
		this.HandleHandDetach();
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00022DF8 File Offset: 0x00020FF8
	private void HandleHandAttached()
	{
		if (this.IsEquippedLocal())
		{
			this.isHandTapSetup = true;
			GorillaTagger.Instance.OnHandTap += new Action<bool, Vector3, Vector3>(this.HandleOnHandTap);
		}
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00022E1F File Offset: 0x0002101F
	private void HandleHandDetach()
	{
		if (this.isHandTapSetup)
		{
			this.isHandTapSetup = false;
			GorillaTagger.Instance.OnHandTap -= new Action<bool, Vector3, Vector3>(this.HandleOnHandTap);
		}
		this.isActivated = false;
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00022E50 File Offset: 0x00021050
	private void HandleOnHandTap(bool isLeft, Vector3 position, Vector3 normal)
	{
		bool flag;
		if (base.FindAttachedHand(out flag, true, true) && isLeft == flag && this.isActivated)
		{
			this.PlaceTapTeleporter(position, normal);
		}
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x00022E80 File Offset: 0x00021080
	private Color GenerateColor(int seed)
	{
		Random.InitState(seed);
		float num = Mathf.Lerp(this.maxBrightness, this.minBrightness, Random.value);
		float num2 = Mathf.Lerp(this.maxBrightness, this.minBrightness, Random.value);
		Color black = Color.black;
		switch (Random.Range(0, 3))
		{
		case 0:
			black.r = num;
			black.g = num2;
			break;
		case 1:
			black.g = num;
			black.b = num2;
			break;
		case 2:
			black.b = num;
			black.r = num2;
			break;
		}
		return black;
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x00022F18 File Offset: 0x00021118
	protected override void OnUpdateAuthority(float dt)
	{
		this.isActivated = this.buttonActivatable.CheckInput(true, true, 0.25f, true, true);
		if (this.nextPlacementDelay > 0f)
		{
			this.nextPlacementDelay -= dt;
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x00022F50 File Offset: 0x00021150
	private void PlaceTapTeleporter(Vector3 position, Vector3 normal)
	{
		if (this.nextPlacementDelay > 0f)
		{
			return;
		}
		if (!this.CheckValidTeleporterPlacement(position, normal))
		{
			return;
		}
		if (base.IsBlocked())
		{
			this.blockedSFX.Play();
			return;
		}
		base.SendClientToAuthorityRPC(0, new object[]
		{
			position,
			Quaternion.LookRotation(normal, base.transform.forward),
			this.nextSelectionId,
			this.hasInfiniteDuration ? -1f : this.portalDefaultDuration
		});
		this.CycleSelection();
		this.nextPlacementDelay = this.placementDelay;
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00022FF8 File Offset: 0x000211F8
	private bool CheckValidTeleporterPlacement(Vector3 position, Vector3 direction)
	{
		Vector3 vector = position + direction * this.nearOffset;
		Vector3 vector2 = position + direction * this.farOffset;
		return Physics.OverlapCapsuleNonAlloc(vector, vector2, this.overlapCheckRadius, this.overlapCheckResults, this.overlapCheckLayers) == 0;
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x0002304A File Offset: 0x0002124A
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.instanceUpgrades = withUpgrades;
		this.useStealthTeleporters = withUpgrades.Contains(SIUpgradeType.Tapteleport_Stealth);
		this.isVelocityPreserved = withUpgrades.Contains(SIUpgradeType.Tapteleport_Keep_Velocity);
		this.hasInfiniteDuration = withUpgrades.Contains(SIUpgradeType.Tapteleport_Infinite_Use);
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0002308C File Offset: 0x0002128C
	public override void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 4)
			{
				return;
			}
			Vector3 vector;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[0], out vector))
			{
				return;
			}
			Quaternion quaternion;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[1], out quaternion))
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[2], out num))
			{
				return;
			}
			if (num < 0 || num > 100)
			{
				return;
			}
			float duration;
			if (!GameEntityManager.ValidateDataType<float>(data[3], out duration))
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			if (Vector3.Distance(vector, base.transform.position) > this.placementCheckDistance)
			{
				return;
			}
			if (!this.CheckValidTeleporterPlacement(vector, quaternion * Vector3.forward))
			{
				return;
			}
			this.RemoveTeleporter(num);
			this.PlaceNewTapTeleporter(vector, quaternion, num, duration);
		}
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x00023140 File Offset: 0x00021340
	public override void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 1)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			if (num < 0 || num > 1)
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			this.nextSelectionId = num;
			this.UpdateNextSelectionDisplay();
		}
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00023198 File Offset: 0x00021398
	private void RemoveTeleporter(int selectId)
	{
		if (selectId == 0)
		{
			if (this._selection1Teleport != null && this._selection1Teleport.gameObject.activeSelf)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection1Teleport.gameEntity.id);
				this._selection1Teleport = null;
				return;
			}
		}
		else if (selectId == 1 && this._selection2Teleport != null && this._selection2Teleport.gameObject.activeSelf)
		{
			this.gameEntity.manager.RequestDestroyItem(this._selection2Teleport.gameEntity.id);
			this._selection2Teleport = null;
		}
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00023240 File Offset: 0x00021440
	private void PlaceNewTapTeleporter(Vector3 position, Quaternion rotation, int selectionId, float duration)
	{
		GameEntityId gameEntityId = this.gameEntity.manager.RequestCreateItem(this.teleportPointPrefab.gameObject.name.GetStaticHash(), position, rotation, BitPackUtils.PackIntsIntoLong(selectionId, (int)duration));
		if (gameEntityId != GameEntityId.Invalid)
		{
			SIGadgetTapTeleporterDeployable component = this.gameEntity.manager.GetGameEntity(gameEntityId).GetComponent<SIGadgetTapTeleporterDeployable>();
			if (selectionId == 0)
			{
				if (this._selection2Teleport != null)
				{
					this._selection2Teleport.SetLink(this, component);
				}
				component.SetLink(this, this._selection2Teleport);
				this._selection1Teleport = component;
			}
			else if (selectionId == 1)
			{
				if (this._selection1Teleport != null)
				{
					this._selection1Teleport.SetLink(this, component);
				}
				component.SetLink(this, this._selection1Teleport);
				this._selection2Teleport = component;
			}
			this.UpdateNewTeleporters();
		}
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00023314 File Offset: 0x00021514
	private void UpdateNewTeleporters()
	{
		int value;
		if (this._selection1Teleport)
		{
			value = this._selection1Teleport.gameEntity.GetNetId();
		}
		else
		{
			value = 0;
		}
		int value2;
		if (this._selection2Teleport)
		{
			value2 = this._selection2Teleport.gameEntity.GetNetId();
		}
		else
		{
			value2 = 0;
		}
		long newState = BitPackUtils.PackIntsIntoLong(value, value2);
		this.gameEntity.RequestState(this.gameEntity.id, newState);
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00023384 File Offset: 0x00021584
	private void HandleStateChanged(long oldState, long newState)
	{
		if (this.gameEntity.IsAuthority())
		{
			return;
		}
		int netId;
		int netId2;
		BitPackUtils.UnpackIntsFromLong(newState, out netId, out netId2);
		GameEntity gameEntityFromNetId = this.gameEntity.manager.GetGameEntityFromNetId(netId);
		if (gameEntityFromNetId != null)
		{
			this._selection1Teleport = gameEntityFromNetId.GetComponent<SIGadgetTapTeleporterDeployable>();
		}
		else
		{
			this._selection1Teleport = null;
		}
		GameEntity gameEntityFromNetId2 = this.gameEntity.manager.GetGameEntityFromNetId(netId2);
		if (gameEntityFromNetId2 != null)
		{
			this._selection2Teleport = gameEntityFromNetId2.GetComponent<SIGadgetTapTeleporterDeployable>();
			return;
		}
		this._selection2Teleport = null;
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00023408 File Offset: 0x00021608
	private void ApplyIdentifierColor()
	{
		this.identifierColorDisplay.material.color = this.identifierColor;
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00023420 File Offset: 0x00021620
	private void UpdateNextSelectionDisplay()
	{
		if (this.nextSelectionId == 0)
		{
			this.selectionColorDisplay.material = this.selectionColor1;
			return;
		}
		if (this.nextSelectionId == 1)
		{
			this.selectionColorDisplay.material = this.selectionColor2;
		}
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00023456 File Offset: 0x00021656
	public void CycleSelection()
	{
		this.nextSelectionId = (this.nextSelectionId + 1) % 2;
		this.UpdateNextSelectionDisplay();
		base.SendClientToClientRPC(0, new object[]
		{
			this.nextSelectionId
		});
	}

	// Token: 0x0400079D RID: 1949
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x0400079E RID: 1950
	[SerializeField]
	private GameObject teleportPointPrefab;

	// Token: 0x0400079F RID: 1951
	[SerializeField]
	private SoundBankPlayer blockedSFX;

	// Token: 0x040007A0 RID: 1952
	[SerializeField]
	private float placementDelay = 0.5f;

	// Token: 0x040007A1 RID: 1953
	[SerializeField]
	private Renderer identifierColorDisplay;

	// Token: 0x040007A2 RID: 1954
	[SerializeField]
	private Renderer selectionColorDisplay;

	// Token: 0x040007A3 RID: 1955
	[SerializeField]
	private Material selectionColor1;

	// Token: 0x040007A4 RID: 1956
	[SerializeField]
	private Material selectionColor2;

	// Token: 0x040007A5 RID: 1957
	[SerializeField]
	private float portalDefaultDuration = 30f;

	// Token: 0x040007A6 RID: 1958
	private float placementCheckDistance = 0.3f;

	// Token: 0x040007AB RID: 1963
	private SIGadgetTapTeleporterDeployable _selection1Teleport;

	// Token: 0x040007AC RID: 1964
	private SIGadgetTapTeleporterDeployable _selection2Teleport;

	// Token: 0x040007AD RID: 1965
	private bool isHandTapSetup;

	// Token: 0x040007AE RID: 1966
	private bool isActivated;

	// Token: 0x040007AF RID: 1967
	private float nextPlacementDelay;

	// Token: 0x040007B0 RID: 1968
	private int nextSelectionId;

	// Token: 0x040007B1 RID: 1969
	private SIUpgradeSet instanceUpgrades;

	// Token: 0x040007B2 RID: 1970
	private float minBrightness = 0.3f;

	// Token: 0x040007B3 RID: 1971
	private float maxBrightness = 1f;

	// Token: 0x040007B4 RID: 1972
	[SerializeField]
	private LayerMask overlapCheckLayers;

	// Token: 0x040007B5 RID: 1973
	[SerializeField]
	private float nearOffset = 0.11f;

	// Token: 0x040007B6 RID: 1974
	[SerializeField]
	private float farOffset = 0.664f;

	// Token: 0x040007B7 RID: 1975
	[SerializeField]
	private float overlapCheckRadius = 0.1f;

	// Token: 0x040007B8 RID: 1976
	private Collider[] overlapCheckResults = new Collider[1];
}
