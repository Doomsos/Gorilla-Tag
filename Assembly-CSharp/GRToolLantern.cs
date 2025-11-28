using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000721 RID: 1825
[RequireComponent(typeof(GameEntity))]
public class GRToolLantern : MonoBehaviour, IGRSummoningEntity
{
	// Token: 0x06002EE0 RID: 12000 RVA: 0x000FE9B0 File Offset: 0x000FCBB0
	private void Awake()
	{
		this.trackedEntities = new List<int>();
		this.state = GRToolLantern.State.Off;
		this.gameEntity.OnStateChanged += this.OnStateChanged;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x000FEA65 File Offset: 0x000FCC65
	private void OnEnable()
	{
		this.TurnOff();
		this.state = GRToolLantern.State.Off;
	}

	// Token: 0x06002EE2 RID: 12002 RVA: 0x000FEA74 File Offset: 0x000FCC74
	private void OnDestroy()
	{
		if (this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.DisableXRay();
		}
	}

	// Token: 0x06002EE3 RID: 12003 RVA: 0x000FEA94 File Offset: 0x000FCC94
	private void OnToolUpgraded(GRTool tool)
	{
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity1))
		{
			this.turnOnSound = this.upgrade1TurnOnSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity2))
		{
			this.turnOnSound = this.upgrade2TurnOnSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.turnOnSound = this.upgrade3TurnOnSound;
		}
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x00002789 File Offset: 0x00000989
	public void OnGrabbed()
	{
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x000FEAE5 File Offset: 0x000FCCE5
	public void OnReleased()
	{
		if (this.WasLastHeldLocal())
		{
			this.DisableXRay();
		}
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x000FEAF5 File Offset: 0x000FCCF5
	private void EnableXRay()
	{
		if (!this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			GRPlayer.GetLocal().xRayVisionRefCount++;
			this.providingXRay = true;
		}
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x000FEB27 File Offset: 0x000FCD27
	private void DisableXRay()
	{
		if (this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			GRPlayer.GetLocal().xRayVisionRefCount--;
			this.providingXRay = false;
		}
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x000FEB5C File Offset: 0x000FCD5C
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.tool.energy > 0)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x000FEB94 File Offset: 0x000FCD94
	private void OnUpdateAuthority(float dt)
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			bool isOn = this.IsHeld();
			this.EnableLights(isOn);
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity2))
		{
			this.SetState(GRToolLantern.State.On);
			if (Time.timeAsDouble > this.lastFlareDropTime + this.minFlareDropInterval && this.IsButtonHeld() && this.tool.HasEnoughEnergy() && this.trackedEntities.Count < this.maxSpawnedFlares && this.lanternFlarePrefab != null)
			{
				if (this.gameEntity.IsAuthority())
				{
					Vector3 vector = base.transform.rotation * this.flareSpawnoffset;
					this.gameEntity.manager.RequestCreateItem(this.lanternFlarePrefab.name.GetStaticHash(), base.transform.position + vector, base.transform.rotation * Quaternion.Euler(10f, 0f, 10f), (long)this.gameEntity.GetNetId());
				}
				this.lastFlareDropTime = Time.timeAsDouble;
				this.tool.UseEnergy();
				this.audioSource.PlayOneShot(this.turnOnSound, this.turnOnSoundVolume);
				return;
			}
		}
		else
		{
			GRToolLantern.State state = this.state;
			if (state != GRToolLantern.State.Off)
			{
				if (state != GRToolLantern.State.On)
				{
					return;
				}
				this.timeOnSpentEnergy -= dt;
				if ((!this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f) || this.tool.energy <= 0)
				{
					this.SetState(GRToolLantern.State.Off);
					this.gameEntity.RequestState(this.gameEntity.id, 0L);
					return;
				}
				if (this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f)
				{
					this.TryConsumeEnergy();
				}
			}
			else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
			{
				this.SetState(GRToolLantern.State.On);
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
				return;
			}
		}
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x000FED9C File Offset: 0x000FCF9C
	private void TryConsumeEnergy()
	{
		if (this.tool.HasEnoughEnergy())
		{
			this.tool.UseEnergy();
			this.timeOnSpentEnergy = this.timeOnPerEnergyUseDurationSeconds * 10f * (float)this.tool.GetEnergyUseCost() / (float)this.tool.GetEnergyMax();
		}
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x000FEDF0 File Offset: 0x000FCFF0
	private void OnUpdateRemote(float dt)
	{
		GRToolLantern.State state = (GRToolLantern.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x000FEE1C File Offset: 0x000FD01C
	private void SetState(GRToolLantern.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		GRToolLantern.State state = this.state;
		if (state != GRToolLantern.State.Off)
		{
			if (state == GRToolLantern.State.On)
			{
				this.TurnOn();
				return;
			}
		}
		else
		{
			this.TurnOff();
		}
	}

	// Token: 0x06002EED RID: 12013 RVA: 0x000FEE60 File Offset: 0x000FD060
	private void TurnOn()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.EnableXRay();
		}
		else
		{
			this.EnableLights(true);
		}
		this.audioSource.PlayOneShot(this.turnOnSound, this.turnOnSoundVolume);
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		this.timeLastTurnedOn = Time.time;
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x000FEEC0 File Offset: 0x000FD0C0
	private void EnableLights(bool isOn)
	{
		if (this.gameLight.gameObject.activeSelf == isOn)
		{
			return;
		}
		if (this.attributes.HasBeenInitialized())
		{
			this.gameLight.light.intensity = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.LightIntensity);
		}
		this.gameLight.gameObject.SetActive(isOn);
		for (int i = 0; i < this.meshAndMaterials.Count; i++)
		{
			MaterialUtils.SwapMaterial(this.meshAndMaterials[i], !isOn);
		}
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x000FEF47 File Offset: 0x000FD147
	private void TurnOff()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.DisableXRay();
			return;
		}
		this.EnableLights(false);
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x000FEF66 File Offset: 0x000FD166
	private bool IsHeld()
	{
		return this.gameEntity.IsHeld();
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x000FEF73 File Offset: 0x000FD173
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x000FEF8C File Offset: 0x000FD18C
	private bool WasLastHeldLocal()
	{
		return this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x000FEFA8 File Offset: 0x000FD1A8
	private bool IsButtonHeld()
	{
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return false;
		}
		if (!GamePlayer.IsLeftHand(num))
		{
			return gamePlayer.rig.rightIndex.calcT > 0.25f;
		}
		return gamePlayer.rig.leftIndex.calcT > 0.25f;
	}

	// Token: 0x06002EF4 RID: 12020 RVA: 0x00002789 File Offset: 0x00000989
	private void OnStateChanged(long prevState, long nextState)
	{
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x000FF01C File Offset: 0x000FD21C
	public bool CanChangeState(long newStateIndex)
	{
		if (newStateIndex < 0L || newStateIndex >= 2L)
		{
			return false;
		}
		GRToolLantern.State state = (GRToolLantern.State)newStateIndex;
		if (state != GRToolLantern.State.Off)
		{
			return state == GRToolLantern.State.On && this.tool.energy > 0;
		}
		return Time.time > this.timeLastTurnedOn + this.minOnDuration || this.tool.energy <= 0;
	}

	// Token: 0x06002EF6 RID: 12022 RVA: 0x000FF078 File Offset: 0x000FD278
	public void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x000FF098 File Offset: 0x000FD298
	public void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
	}

	// Token: 0x06002EF8 RID: 12024 RVA: 0x000FF0C7 File Offset: 0x000FD2C7
	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x000FF0D0 File Offset: 0x000FD2D0
	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	// Token: 0x04003D41 RID: 15681
	public GameEntity gameEntity;

	// Token: 0x04003D42 RID: 15682
	public GRTool tool;

	// Token: 0x04003D43 RID: 15683
	public GameLight gameLight;

	// Token: 0x04003D44 RID: 15684
	public GRAttributes attributes;

	// Token: 0x04003D45 RID: 15685
	[SerializeField]
	private float timeOnPerEnergyUseDurationSeconds = 2f;

	// Token: 0x04003D46 RID: 15686
	[SerializeField]
	private int minEnergyPerUse = 1;

	// Token: 0x04003D47 RID: 15687
	[SerializeField]
	private float turnOnSoundVolume;

	// Token: 0x04003D48 RID: 15688
	[SerializeField]
	private AudioClip turnOnSound;

	// Token: 0x04003D49 RID: 15689
	[SerializeField]
	private AudioClip upgrade1TurnOnSound;

	// Token: 0x04003D4A RID: 15690
	[SerializeField]
	private AudioClip upgrade2TurnOnSound;

	// Token: 0x04003D4B RID: 15691
	[SerializeField]
	private AudioClip upgrade3TurnOnSound;

	// Token: 0x04003D4C RID: 15692
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003D4D RID: 15693
	public List<MeshAndMaterials> meshAndMaterials;

	// Token: 0x04003D4E RID: 15694
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x04003D4F RID: 15695
	private float timeOnSpentEnergy;

	// Token: 0x04003D50 RID: 15696
	private float timeLastTurnedOn;

	// Token: 0x04003D51 RID: 15697
	private float minOnDuration = 0.5f;

	// Token: 0x04003D52 RID: 15698
	private GRToolLantern.State state;

	// Token: 0x04003D53 RID: 15699
	private List<int> trackedEntities;

	// Token: 0x04003D54 RID: 15700
	private double lastFlareDropTime;

	// Token: 0x04003D55 RID: 15701
	public double minFlareDropInterval = 1.0;

	// Token: 0x04003D56 RID: 15702
	public GameEntity lanternFlarePrefab;

	// Token: 0x04003D57 RID: 15703
	public int maxSpawnedFlares = 10;

	// Token: 0x04003D58 RID: 15704
	private bool providingXRay;

	// Token: 0x04003D59 RID: 15705
	public Vector3 flareSpawnoffset = Vector3.zero;

	// Token: 0x02000722 RID: 1826
	private enum State
	{
		// Token: 0x04003D5B RID: 15707
		Off,
		// Token: 0x04003D5C RID: 15708
		On,
		// Token: 0x04003D5D RID: 15709
		Count
	}
}
