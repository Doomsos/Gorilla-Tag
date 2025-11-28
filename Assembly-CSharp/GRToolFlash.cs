using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200071E RID: 1822
public class GRToolFlash : MonoBehaviour, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x06002ECB RID: 11979 RVA: 0x000FE2EF File Offset: 0x000FC4EF
	private void Awake()
	{
		this.state = GRToolFlash.State.Idle;
		this.stateTimeRemaining = -1f;
		this.gameHitter = base.GetComponent<GameHitter>();
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x000FE30F File Offset: 0x000FC50F
	private void OnEnable()
	{
		this.StopFlash();
		this.SetState(GRToolFlash.State.Idle);
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x000FE31E File Offset: 0x000FC51E
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002ED0 RID: 11984 RVA: 0x000FE354 File Offset: 0x000FC554
	private void OnToolUpgraded(GRTool tool)
	{
		this.stunDuration = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.FlashStunDuration);
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage1))
		{
			this.flashSound = this.upgrade1FlashSound;
			this.flash = this.upgrade1FlashCone;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage2))
		{
			this.flashSound = this.upgrade2FlashSound;
			this.flash = this.upgrade2FlashCone;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
		{
			this.flashSound = this.upgrade3FlashSound;
			this.flash = this.upgrade3FlashCone;
		}
	}

	// Token: 0x06002ED1 RID: 11985 RVA: 0x000FE3D9 File Offset: 0x000FC5D9
	private bool IsHeldLocal()
	{
		return this.item.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002ED2 RID: 11986 RVA: 0x000FE3F2 File Offset: 0x000FC5F2
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x000FE40C File Offset: 0x000FC60C
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x000FE440 File Offset: 0x000FC640
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolFlash.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Flash);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolFlash.State.Flash:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Cooldown);
				return;
			}
			break;
		case GRToolFlash.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x000FE528 File Offset: 0x000FC728
	private void OnUpdateRemote(float dt)
	{
		GRToolFlash.State state = (GRToolFlash.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			if (this.state == GRToolFlash.State.Charging && state == GRToolFlash.State.Cooldown)
			{
				this.SetState(GRToolFlash.State.Flash);
				return;
			}
			if (this.state == GRToolFlash.State.Flash && state == GRToolFlash.State.Cooldown)
			{
				if (Time.time > this.timeLastFlashed + this.flashDuration)
				{
					this.SetState(GRToolFlash.State.Cooldown);
					return;
				}
			}
			else
			{
				this.SetState(state);
			}
		}
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x000FE590 File Offset: 0x000FC790
	private void SetStateAuthority(GRToolFlash.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x000FE5B4 File Offset: 0x000FC7B4
	private void SetState(GRToolFlash.State newState)
	{
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolFlash.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolFlash.State.Flash:
			this.StartFlash();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolFlash.State.Cooldown:
			this.StopFlash();
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x06002ED8 RID: 11992 RVA: 0x000FE638 File Offset: 0x000FC838
	private void StartCharge()
	{
		this.audioSource.volume = this.chargeSoundVolume;
		this.audioSource.clip = this.chargeSound;
		this.audioSource.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x06002ED9 RID: 11993 RVA: 0x000FE690 File Offset: 0x000FC890
	private void StartFlash()
	{
		this.flash.SetActive(true);
		this.audioSource.volume = this.flashSoundVolume;
		this.audioSource.clip = this.flashSound;
		this.audioSource.Play();
		this.tool.UseEnergy();
		this.timeLastFlashed = Time.time;
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 1f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 5f, this.enemyLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					GameHittable component = attachedRigidbody.GetComponent<GameHittable>();
					if (component != null && this.gameHitter != null)
					{
						GameHitData hitData = new GameHitData
						{
							hitTypeId = 1,
							hitEntityId = component.gameEntity.id,
							hitByEntityId = this.gameEntity.id,
							hitEntityPosition = component.gameEntity.transform.position,
							hitPosition = ((raycastHit.distance == 0f) ? this.shootFrom.position : raycastHit.point),
							hitImpulse = Vector3.zero,
							hitAmount = this.gameHitter.CalcHitAmount(GameHitType.Flash, component, this.gameEntity)
						};
						component.RequestHit(hitData);
					}
				}
			}
		}
	}

	// Token: 0x06002EDA RID: 11994 RVA: 0x000FE840 File Offset: 0x000FCA40
	private void StopFlash()
	{
		this.flash.SetActive(false);
	}

	// Token: 0x06002EDB RID: 11995 RVA: 0x000FE850 File Offset: 0x000FCA50
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.item.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? 4 : 5) > 0.25f;
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x000FE8B0 File Offset: 0x000FCAB0
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.item.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x000FE904 File Offset: 0x000FCB04
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFlashed + this.cooldownMinimum);
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x000FE92D File Offset: 0x000FCB2D
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Stun Duration: <color=\"yellow\">{0}<color=\"white\">", this.stunDuration));
	}

	// Token: 0x04003D18 RID: 15640
	public GameEntity gameEntity;

	// Token: 0x04003D19 RID: 15641
	public GRTool tool;

	// Token: 0x04003D1A RID: 15642
	public GRAttributes attributes;

	// Token: 0x04003D1B RID: 15643
	public GameObject flash;

	// Token: 0x04003D1C RID: 15644
	public Transform shootFrom;

	// Token: 0x04003D1D RID: 15645
	public LayerMask enemyLayerMask;

	// Token: 0x04003D1E RID: 15646
	public AudioSource audioSource;

	// Token: 0x04003D1F RID: 15647
	public AudioClip chargeSound;

	// Token: 0x04003D20 RID: 15648
	public float chargeSoundVolume = 0.2f;

	// Token: 0x04003D21 RID: 15649
	public AudioClip flashSound;

	// Token: 0x04003D22 RID: 15650
	public AudioClip upgrade1FlashSound;

	// Token: 0x04003D23 RID: 15651
	public AudioClip upgrade2FlashSound;

	// Token: 0x04003D24 RID: 15652
	public AudioClip upgrade3FlashSound;

	// Token: 0x04003D25 RID: 15653
	public GameObject upgrade1FlashCone;

	// Token: 0x04003D26 RID: 15654
	public GameObject upgrade2FlashCone;

	// Token: 0x04003D27 RID: 15655
	public GameObject upgrade3FlashCone;

	// Token: 0x04003D28 RID: 15656
	public float flashSoundVolume = 1f;

	// Token: 0x04003D29 RID: 15657
	public float stunDuration;

	// Token: 0x04003D2A RID: 15658
	public GRToolFlash.UpgradeTypes upgradesApplied;

	// Token: 0x04003D2B RID: 15659
	public float chargeDuration = 0.75f;

	// Token: 0x04003D2C RID: 15660
	public float flashDuration = 0.1f;

	// Token: 0x04003D2D RID: 15661
	public float cooldownDuration;

	// Token: 0x04003D2E RID: 15662
	private float timeLastFlashed;

	// Token: 0x04003D2F RID: 15663
	private float cooldownMinimum = 0.35f;

	// Token: 0x04003D30 RID: 15664
	private bool activatedLocally;

	// Token: 0x04003D31 RID: 15665
	public GameEntity item;

	// Token: 0x04003D32 RID: 15666
	private GameHitter gameHitter;

	// Token: 0x04003D33 RID: 15667
	private GRToolFlash.State state;

	// Token: 0x04003D34 RID: 15668
	private float stateTimeRemaining;

	// Token: 0x04003D35 RID: 15669
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x0200071F RID: 1823
	[Flags]
	public enum UpgradeTypes
	{
		// Token: 0x04003D37 RID: 15671
		None = 1,
		// Token: 0x04003D38 RID: 15672
		UpagredA = 2,
		// Token: 0x04003D39 RID: 15673
		UpagredB = 4,
		// Token: 0x04003D3A RID: 15674
		UpagredC = 8
	}

	// Token: 0x02000720 RID: 1824
	private enum State
	{
		// Token: 0x04003D3C RID: 15676
		Idle,
		// Token: 0x04003D3D RID: 15677
		Charging,
		// Token: 0x04003D3E RID: 15678
		Flash,
		// Token: 0x04003D3F RID: 15679
		Cooldown,
		// Token: 0x04003D40 RID: 15680
		Count
	}
}
