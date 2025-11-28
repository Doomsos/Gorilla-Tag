using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000735 RID: 1845
public class GRToolUpgradePiece : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002F81 RID: 12161 RVA: 0x00102540 File Offset: 0x00100740
	private void Start()
	{
		MeshFilter componentInChildren = base.GetComponentInChildren<MeshFilter>();
		if (componentInChildren != null)
		{
			this.meshCollider.sharedMesh = componentInChildren.sharedMesh;
		}
	}

	// Token: 0x06002F82 RID: 12162 RVA: 0x00102570 File Offset: 0x00100770
	private void EnableProcAnimLoop()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.Tick));
		if (!this.humAudioSource.isPlaying)
		{
			this.humAudioSource.volume = 0f;
			this.humAudioSource.GTPlay();
		}
	}

	// Token: 0x06002F83 RID: 12163 RVA: 0x001025CC File Offset: 0x001007CC
	private void DisableProcAnimLoop()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.Tick));
		this.SwitchMagnetizedTarget(null);
		this.childVisualTransform.localPosition = Vector3.zero;
		this.childVisualTransform.localRotation = Quaternion.identity;
		this.childVisualTransform.localScale = Vector3.one;
		this.humAudioSource.Stop();
		if (this.attractParticleSystem != null)
		{
			this.attractParticleSystem.Stop();
		}
	}

	// Token: 0x06002F84 RID: 12164 RVA: 0x0010265B File Offset: 0x0010085B
	private void SwitchMagnetizedTarget(GameEntity entity)
	{
		this.currentMagnetizingTool = entity;
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x00102664 File Offset: 0x00100864
	private void Tick()
	{
		Vector3 position = base.transform.position;
		List<GameEntity> gameEntities = this.gameEntity.manager.GetGameEntities();
		int num = this.gameEntityListCheckIndex;
		int num2 = (this.toolSearchesPerFrame < gameEntities.Count) ? this.toolSearchesPerFrame : gameEntities.Count;
		GRTool grtool = (this.currentMagnetizingTool != null) ? this.currentMagnetizingTool.GetComponent<GRTool>() : null;
		GRTool.Upgrade upgrade = (grtool != null) ? grtool.FindMatchingUpgrade(this.matchingUpgrade) : null;
		float num3 = (grtool != null) ? grtool.GetPointDistanceToUpgrade(position, upgrade) : 1E+10f;
		if (num3 > this.minDistToStartMagnetize)
		{
			this.SwitchMagnetizedTarget(null);
			grtool = null;
			upgrade = null;
			num3 = 1E+10f;
		}
		for (int i = 0; i < num2; i++)
		{
			num = (num + 1) % gameEntities.Count;
			GameEntity gameEntity = gameEntities[num];
			if (!(gameEntity == null))
			{
				GRTool component = gameEntity.GetComponent<GRTool>();
				if (component != null && gameEntity.heldByActorNumber != -1)
				{
					GRTool.Upgrade upgrade2 = component.FindMatchingUpgrade(this.matchingUpgrade);
					if (upgrade2 != null)
					{
						float pointDistanceToUpgrade = component.GetPointDistanceToUpgrade(position, upgrade2);
						if (pointDistanceToUpgrade > 0f && pointDistanceToUpgrade < num3 && pointDistanceToUpgrade < this.minDistToStartMagnetize)
						{
							this.SwitchMagnetizedTarget(gameEntity);
							grtool = component;
							upgrade = upgrade2;
							num3 = pointDistanceToUpgrade;
						}
					}
				}
			}
		}
		this.gameEntityListCheckIndex = num;
		if (grtool != null)
		{
			Transform upgradeAttachTransform = grtool.GetUpgradeAttachTransform(upgrade);
			if (num3 >= this.minDistToSnap)
			{
				float num4 = Mathf.Clamp01(num3 / this.minDistToStartMagnetize);
				this.humAudioSource.volume = Mathf.Lerp(this.magnetizingLoopMaxVolume, this.magnetizingLoopMinVolume, num4);
				float num5 = this.shakeMaxAmount * (1f - num4);
				float num6 = Mathf.Clamp01((this.visualDistanceCurve != null) ? this.visualDistanceCurve.Evaluate(num4) : num4);
				this.shakePhase += Time.deltaTime * this.shakeFrequency;
				if (this.shakePhase > 6.2831855f)
				{
					this.shakePhase -= 6.2831855f;
				}
				Transform transform = base.transform;
				if (this.childVisualTransform != null)
				{
					Vector3 position2 = Vector3.Lerp(upgradeAttachTransform.position, transform.position, num6);
					Quaternion quaternion = Quaternion.Slerp(upgradeAttachTransform.rotation, transform.rotation, num6);
					Vector3 localScale = Vector3.Lerp(upgradeAttachTransform.localScale, transform.localScale, num6);
					localScale.x /= transform.localScale.x;
					localScale.y /= transform.localScale.y;
					localScale.z /= transform.localScale.y;
					quaternion *= Quaternion.Euler(new Vector3(num5 * Mathf.Sin(this.shakePhase), num5 * Mathf.Cos(this.shakePhase), 0f));
					this.childVisualTransform.position = position2;
					this.childVisualTransform.rotation = quaternion;
					this.childVisualTransform.localScale = localScale;
				}
				if (this.attractParticleSystem != null)
				{
					if (!this.attractParticleSystem.isPlaying)
					{
						this.attractParticleSystem.Play();
					}
					this.attractParticleSystem.emission.enabled = true;
				}
				this.forceField.transform.position = upgradeAttachTransform.position;
				return;
			}
			this.humAudioSource.volume = 0f;
			if (this.attractParticleSystem != null)
			{
				this.attractParticleSystem.Stop();
			}
			this.childVisualTransform.position = upgradeAttachTransform.position;
			this.childVisualTransform.rotation = upgradeAttachTransform.rotation;
			this.childVisualTransform.localScale = new Vector3(upgradeAttachTransform.localScale.x / base.transform.localScale.x, upgradeAttachTransform.localScale.y / base.transform.localScale.y, upgradeAttachTransform.localScale.z / base.transform.localScale.z);
			if (this.currentMagnetizingTool != null)
			{
				GhostReactor instance = GhostReactor.instance;
				if (instance != null)
				{
					instance.grManager.ToolSnapRequestUpgrade(this.gameEntity.GetNetId(), this.matchingUpgrade, this.currentMagnetizingTool.GetComponent<GameEntity>().GetNetId());
					return;
				}
			}
		}
		else
		{
			if (this.attractParticleSystem != null)
			{
				this.attractParticleSystem.emission.enabled = false;
			}
			this.humAudioSource.volume = 0f;
		}
	}

	// Token: 0x06002F86 RID: 12166 RVA: 0x00102B14 File Offset: 0x00100D14
	private void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.ReleasedByPlayer));
	}

	// Token: 0x06002F87 RID: 12167 RVA: 0x00102B70 File Offset: 0x00100D70
	private void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.ReleasedByPlayer));
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x00102BCC File Offset: 0x00100DCC
	public void GrabbedByPlayer()
	{
		if (this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer)
			{
				grplayer.GrabbedItem(this.gameEntity.id, base.gameObject.name);
			}
		}
		this.EnableProcAnimLoop();
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x00102C2D File Offset: 0x00100E2D
	public void ReleasedByPlayer()
	{
		this.DisableProcAnimLoop();
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x00102C38 File Offset: 0x00100E38
	public void OnEntityInit()
	{
		GhostReactor.ToolEntityCreateData toolEntityCreateData = GhostReactor.ToolEntityCreateData.Unpack(this.gameEntity.createData);
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null)
		{
			GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = ghostReactorManager.GetToolUpgradeStationFullForIndex(toolEntityCreateData.stationIndex);
			if (toolUpgradeStationFullForIndex != null)
			{
				toolUpgradeStationFullForIndex.InitLinkedEntity(this.gameEntity);
			}
		}
	}

	// Token: 0x06002F8B RID: 12171 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002F8C RID: 12172 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x04003E31 RID: 15921
	public GameEntity gameEntity;

	// Token: 0x04003E32 RID: 15922
	public GRToolProgressionManager.ToolParts matchingUpgrade;

	// Token: 0x04003E33 RID: 15923
	private int gameEntityListCheckIndex;

	// Token: 0x04003E34 RID: 15924
	private GameEntity currentMagnetizingTool;

	// Token: 0x04003E35 RID: 15925
	public AnimationCurve visualDistanceCurve;

	// Token: 0x04003E36 RID: 15926
	public float shakeMaxAmount = 10f;

	// Token: 0x04003E37 RID: 15927
	public float shakeFrequency = 100f;

	// Token: 0x04003E38 RID: 15928
	public Transform childVisualTransform;

	// Token: 0x04003E39 RID: 15929
	public AudioSource humAudioSource;

	// Token: 0x04003E3A RID: 15930
	public AudioSource audioSource;

	// Token: 0x04003E3B RID: 15931
	public AudioClip snapAudioClip;

	// Token: 0x04003E3C RID: 15932
	public MeshCollider meshCollider;

	// Token: 0x04003E3D RID: 15933
	public ParticleSystem attractParticleSystem;

	// Token: 0x04003E3E RID: 15934
	public ParticleSystemForceField forceField;

	// Token: 0x04003E3F RID: 15935
	public float minDistToStartMagnetize = 0.5f;

	// Token: 0x04003E40 RID: 15936
	public float minDistToSnap;

	// Token: 0x04003E41 RID: 15937
	public float magnetizingLoopMinVolume = 0.2f;

	// Token: 0x04003E42 RID: 15938
	public float magnetizingLoopMaxVolume = 1f;

	// Token: 0x04003E43 RID: 15939
	public float snapAudioVolume = 1f;

	// Token: 0x04003E44 RID: 15940
	private int toolSearchesPerFrame = 5;

	// Token: 0x04003E45 RID: 15941
	private float shakePhase;
}
