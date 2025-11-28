using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000F2 RID: 242
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetPlatformDeployer : SIGadget, I_SIDisruptable
{
	// Token: 0x060005D9 RID: 1497 RVA: 0x00020F9C File Offset: 0x0001F19C
	private void Start()
	{
		this.previewPlatform.SetActive(false);
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnReleased = (Action)Delegate.Combine(gameEntity.OnReleased, new Action(this.HandleStopInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Combine(gameEntity2.OnUnsnapped, new Action(this.HandleStopInteraction));
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00021004 File Offset: 0x0001F204
	private void OnDestroy()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnReleased = (Action)Delegate.Remove(gameEntity.OnReleased, new Action(this.HandleStopInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Remove(gameEntity2.OnUnsnapped, new Action(this.HandleStopInteraction));
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0002105F File Offset: 0x0001F25F
	private void HandleStopInteraction()
	{
		this.SetState(SIGadgetPlatformDeployer.State.Idle);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00021068 File Offset: 0x0001F268
	protected override void Update()
	{
		base.Update();
		if (this.remainingRechargeTime > 0f)
		{
			int num = Mathf.CeilToInt(this.remainingRechargeTime / this.chargeRecoveryTime);
			this.remainingRechargeTime = Mathf.Max(this.remainingRechargeTime - Time.deltaTime, 0f);
			int num2 = Mathf.CeilToInt(this.remainingRechargeTime / this.chargeRecoveryTime);
			this.chargeDisplay.UpdateDisplay(this.maxCharges - num2);
			if (num2 != num && this.IsEquippedLocal())
			{
				this.rechargeSFX.Play();
				bool forLeftController;
				if (base.FindAttachedHand(out forLeftController, true, true))
				{
					GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				}
			}
		}
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x0002112E File Offset: 0x0001F32E
	protected override bool IsEquippedLocal()
	{
		return (this.canActivateWhileHeld && this.gameEntity.IsHeldByLocalPlayer()) || this.gameEntity.IsSnappedByLocalPlayer();
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00021154 File Offset: 0x0001F354
	protected override void OnUpdateAuthority(float dt)
	{
		SIGadgetPlatformDeployer.State state = this.state;
		if (state != SIGadgetPlatformDeployer.State.Idle)
		{
			if (state != SIGadgetPlatformDeployer.State.Deploying)
			{
				return;
			}
			if (this.CheckReleaseInputs())
			{
				if (this.IsChargeAvailable())
				{
					this.TryDeployPlatform();
				}
				this.SetStateAuthority(SIGadgetPlatformDeployer.State.Idle);
				return;
			}
			this.UpdatePreview();
			return;
		}
		else
		{
			if (this.CheckInitInputs())
			{
				if (this.IsChargeAvailable())
				{
					if (this.isInstancePlace)
					{
						if (!this.wasInputPressed)
						{
							this.TryDeployInstantPlatform();
						}
					}
					else
					{
						this.SetStateAuthority(SIGadgetPlatformDeployer.State.Deploying);
					}
				}
				this.wasInputPressed = true;
				return;
			}
			this.wasInputPressed = false;
			return;
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x000211D4 File Offset: 0x0001F3D4
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetPlatformDeployer.State state = (SIGadgetPlatformDeployer.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
		SIGadgetPlatformDeployer.State state2 = this.state;
		if (state2 != SIGadgetPlatformDeployer.State.Idle && state2 == SIGadgetPlatformDeployer.State.Deploying)
		{
			this.UpdatePreview();
		}
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00021214 File Offset: 0x0001F414
	private bool CheckInitInputs()
	{
		if (!this.buttonActivatable.CheckInput(this.canActivateWhileHeld, true, this.inputSensitivity, true, true))
		{
			return false;
		}
		if (this.isInstancePlace)
		{
			return true;
		}
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		Vector3 position = gamePlayer.leftHand.position;
		Vector3 position2 = gamePlayer.rightHand.position;
		return Vector3.Distance(position, position2) <= this.activationHandDistance;
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x0002127E File Offset: 0x0001F47E
	private bool CheckReleaseInputs()
	{
		return !this.buttonActivatable.CheckInput(this.canActivateWhileHeld, true, this.inputSensitivity, true, true);
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0002129D File Offset: 0x0001F49D
	private bool IsChargeAvailable()
	{
		return (float)this.maxCharges * this.chargeRecoveryTime - this.remainingRechargeTime > this.chargeRecoveryTime;
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000212BF File Offset: 0x0001F4BF
	private void SpendCharge()
	{
		this.remainingRechargeTime += this.chargeRecoveryTime;
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x000212D4 File Offset: 0x0001F4D4
	private void TryDeployInstantPlatform()
	{
		if (base.IsBlocked())
		{
			this.blockedSFX.Play();
			return;
		}
		GamePlayer gamePlayer;
		if (!this.TryGetGamePlayer(out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindSnapIndex(this.gameEntity.id);
		if (num == -1 && this.canActivateWhileHeld)
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			vector = base.transform.position - base.transform.up * this.handDepthOffset;
			quaternion = base.transform.rotation;
			Debug.DrawRay(base.transform.position, -base.transform.up * 0.3f, Color.blue, 10f);
			Debug.DrawRay(base.transform.position, base.transform.forward * 0.3f, Color.blue, 10f);
			Debug.DrawRay(vector, quaternion * Vector3.forward * 0.3f, Color.green, 10f);
		}
		else
		{
			Transform transform = GamePlayer.IsLeftHand(num) ? gamePlayer.leftHand : gamePlayer.rightHand;
			vector = transform.position;
			Vector3 up = transform.up;
			Vector3 right = transform.right;
			Debug.DrawRay(vector, right * 0.3f, Color.red, 10f);
			Debug.DrawRay(vector, up * 0.3f, Color.red, 10f);
			quaternion = Quaternion.LookRotation(up, right);
			vector += right * this.handDepthOffset;
			Debug.DrawRay(vector, quaternion * Vector3.forward * 0.3f, Color.green, 10f);
		}
		this.DeployPlatform(vector, quaternion);
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x000214B4 File Offset: 0x0001F6B4
	private void TryDeployPlatform()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		Vector3 position = gamePlayer.leftHand.position;
		Vector3 position2 = gamePlayer.rightHand.position;
		if (Vector3.Distance(position, position2) > this.deployMinRequiredHandDistance)
		{
			if (base.IsBlocked())
			{
				this.blockedSFX.Play();
				return;
			}
			Vector3 pos;
			Quaternion rot;
			Vector3 vector;
			if (this.TryGetPlatformPosRotScale(out pos, out rot, out vector))
			{
				this.DeployPlatform(pos, rot);
				return;
			}
		}
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00021520 File Offset: 0x0001F720
	private void DeployPlatform(Vector3 pos, Quaternion rot)
	{
		this.SpendCharge();
		this.CreateLocalPlatformInstance(pos, rot);
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		if (this.gameEntity.IsAuthority())
		{
			base.SendAuthorityToClientRPC(0, new object[]
			{
				actorNumber,
				pos,
				rot
			});
			return;
		}
		base.SendClientToAuthorityRPC(0, new object[]
		{
			actorNumber,
			pos,
			rot
		});
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x000215AC File Offset: 0x0001F7AC
	public override void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 3)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			Vector3 vector;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
			{
				return;
			}
			Quaternion rot;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out rot))
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			if (Vector3.Distance(base.transform.position, vector) > 2f)
			{
				return;
			}
			this.CreateLocalPlatformInstance(vector, rot);
			base.SendAuthorityToClientRPC(0, data);
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00021630 File Offset: 0x0001F830
	public override void ProcessAuthorityToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 3)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			Vector3 pos;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out pos))
			{
				return;
			}
			Quaternion rot;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out rot))
			{
				return;
			}
			if (num != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.CreateLocalPlatformInstance(pos, rot);
			}
		}
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x0002168C File Offset: 0x0001F88C
	private void CreateLocalPlatformInstance(Vector3 pos, Quaternion rot)
	{
		if (this.deployedPlatformCount >= this.maxCharges)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this.platformPrefab, true);
		if (gameObject != null)
		{
			SIGadgetPlatformDeployerPlatform component = gameObject.GetComponent<SIGadgetPlatformDeployerPlatform>();
			if (component != null)
			{
				this.deployedPlatformCount++;
				SIGadgetPlatformDeployerPlatform sigadgetPlatformDeployerPlatform = component;
				sigadgetPlatformDeployerPlatform.OnDisabled = (Action)Delegate.Combine(sigadgetPlatformDeployerPlatform.OnDisabled, delegate()
				{
					this.deployedPlatformCount--;
				});
			}
			gameObject.transform.SetPositionAndRotation(pos, rot);
			ISIGameDeployable isigameDeployable;
			if (gameObject.TryGetComponent<ISIGameDeployable>(ref isigameDeployable))
			{
				isigameDeployable.ApplyUpgrades(this.instanceUpgrades);
			}
		}
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00021726 File Offset: 0x0001F926
	private void SetStateAuthority(SIGadgetPlatformDeployer.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00021748 File Offset: 0x0001F948
	private void SetState(SIGadgetPlatformDeployer.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		SIGadgetPlatformDeployer.State state = this.state;
		if (state == SIGadgetPlatformDeployer.State.Idle)
		{
			this.SetPreviewVisibility(false);
			return;
		}
		if (state != SIGadgetPlatformDeployer.State.Deploying)
		{
			return;
		}
		this.SetPreviewVisibility(true);
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0002178E File Offset: 0x0001F98E
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 2L;
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x0002179D File Offset: 0x0001F99D
	private void SetPreviewVisibility(bool enabled)
	{
		this.previewPlatform.SetActive(enabled);
		if (enabled)
		{
			this.UpdatePreview();
		}
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x000217B4 File Offset: 0x0001F9B4
	private void UpdatePreview()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 localScale;
		if (this.TryGetPlatformPosRotScale(out vector, out quaternion, out localScale))
		{
			this.previewPlatform.transform.SetPositionAndRotation(vector, quaternion);
			this.previewPlatform.transform.localScale = localScale;
			GamePlayer gamePlayer;
			if (this.TryGetGamePlayer(out gamePlayer))
			{
				Vector3 position = gamePlayer.leftHand.position;
				Vector3 position2 = gamePlayer.rightHand.position;
				if (Vector3.Distance(position, position2) > this.deployMinRequiredHandDistance)
				{
					this.previewMesh.material = this.validPreviewMaterial;
					return;
				}
				this.previewMesh.material = this.invalidPreviewMaterial;
			}
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00021848 File Offset: 0x0001FA48
	private bool TryGetPlatformPosRotScale(out Vector3 pos, out Quaternion rot, out Vector3 scale)
	{
		pos = Vector3.zero;
		rot = Quaternion.identity;
		scale = Vector3.one;
		GamePlayer gamePlayer;
		if (this.TryGetGamePlayer(out gamePlayer))
		{
			Vector3 position = gamePlayer.leftHand.position;
			Vector3 position2 = gamePlayer.rightHand.position;
			Vector3 position3 = gamePlayer.rig.head.rigTarget.position;
			Vector3 vector = (position + position2) / 2f;
			Vector3 normalized = (position3 - vector).normalized;
			Vector3 vector2 = Vector3.ProjectOnPlane((position - position2).normalized, normalized);
			pos = vector + -normalized * this.handDepthOffset;
			rot = Quaternion.LookRotation(vector2, normalized);
			return true;
		}
		return false;
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x0002191C File Offset: 0x0001FB1C
	private bool TryGetGamePlayer(out GamePlayer player)
	{
		player = null;
		return GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out player) || (this.canActivateWhileHeld && GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out player));
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x00021954 File Offset: 0x0001FB54
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.instanceUpgrades = withUpgrades;
		bool flag = withUpgrades.Contains(SIUpgradeType.Platform_Capacity);
		this.maxCharges = (flag ? this.maxChargesHighCapacity : this.maxChargesDefault);
		this.chargeDisplay = (flag ? this.chargeDisplayHighCapacity : this.chargeDisplayDefault);
		this.chargeRecoveryTime = (withUpgrades.Contains(SIUpgradeType.Platform_Cooldown) ? this.chargeRecoveryTimeFast : this.chargeRecoveryTimeDefault);
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x000219C5 File Offset: 0x0001FBC5
	public void Disrupt(float disruptTime)
	{
		this.remainingRechargeTime = (float)this.maxCharges * this.chargeRecoveryTime + disruptTime;
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x000219DD File Offset: 0x0001FBDD
	protected override void HandleBlockedActionChanged(bool isBlocked)
	{
		this.blockedDisplayMesh.material = (isBlocked ? this.blockedMat : this.unblockedMat);
	}

	// Token: 0x04000737 RID: 1847
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x04000738 RID: 1848
	[SerializeField]
	private SoundBankPlayer rechargeSFX;

	// Token: 0x04000739 RID: 1849
	[SerializeField]
	private SoundBankPlayer blockedSFX;

	// Token: 0x0400073A RID: 1850
	[SerializeField]
	private MeshRenderer blockedDisplayMesh;

	// Token: 0x0400073B RID: 1851
	[SerializeField]
	private Material unblockedMat;

	// Token: 0x0400073C RID: 1852
	[SerializeField]
	private Material blockedMat;

	// Token: 0x0400073D RID: 1853
	[SerializeField]
	private GameObject platformPrefab;

	// Token: 0x0400073E RID: 1854
	[Header("Activation")]
	[SerializeField]
	private bool canActivateWhileHeld = true;

	// Token: 0x0400073F RID: 1855
	[SerializeField]
	private bool isInstancePlace;

	// Token: 0x04000740 RID: 1856
	[SerializeField]
	private float activationHandDistance = 0.2f;

	// Token: 0x04000741 RID: 1857
	[SerializeField]
	private float inputSensitivity = 0.25f;

	// Token: 0x04000742 RID: 1858
	[Header("Deploy")]
	[SerializeField]
	private float deployMinRequiredHandDistance = 0.2f;

	// Token: 0x04000743 RID: 1859
	[SerializeField]
	private GameObject previewPlatform;

	// Token: 0x04000744 RID: 1860
	[SerializeField]
	private float handInset = 0.1f;

	// Token: 0x04000745 RID: 1861
	[SerializeField]
	private float handDepthOffset = 0.3f;

	// Token: 0x04000746 RID: 1862
	[SerializeField]
	private MeshRenderer previewMesh;

	// Token: 0x04000747 RID: 1863
	[SerializeField]
	private Material validPreviewMaterial;

	// Token: 0x04000748 RID: 1864
	[SerializeField]
	private Material invalidPreviewMaterial;

	// Token: 0x04000749 RID: 1865
	[Header("Charges")]
	private int maxCharges = 3;

	// Token: 0x0400074A RID: 1866
	private float chargeRecoveryTime = 10f;

	// Token: 0x0400074B RID: 1867
	private SIChargeDisplay chargeDisplay;

	// Token: 0x0400074C RID: 1868
	[SerializeField]
	private int maxChargesDefault = 3;

	// Token: 0x0400074D RID: 1869
	[SerializeField]
	private int maxChargesHighCapacity = 5;

	// Token: 0x0400074E RID: 1870
	[SerializeField]
	private SIChargeDisplay chargeDisplayDefault;

	// Token: 0x0400074F RID: 1871
	[SerializeField]
	private SIChargeDisplay chargeDisplayHighCapacity;

	// Token: 0x04000750 RID: 1872
	[SerializeField]
	private float chargeRecoveryTimeDefault = 10f;

	// Token: 0x04000751 RID: 1873
	[SerializeField]
	private float chargeRecoveryTimeFast = 5f;

	// Token: 0x04000752 RID: 1874
	private SIGadgetPlatformDeployer.State state;

	// Token: 0x04000753 RID: 1875
	private bool wasInputPressed;

	// Token: 0x04000754 RID: 1876
	private float remainingRechargeTime;

	// Token: 0x04000755 RID: 1877
	private SIUpgradeSet instanceUpgrades;

	// Token: 0x04000756 RID: 1878
	private const float MAX_DEPLOY_DIST = 2f;

	// Token: 0x04000757 RID: 1879
	private int deployedPlatformCount;

	// Token: 0x020000F3 RID: 243
	private enum State
	{
		// Token: 0x04000759 RID: 1881
		Idle,
		// Token: 0x0400075A RID: 1882
		Deploying,
		// Token: 0x0400075B RID: 1883
		Count
	}
}
