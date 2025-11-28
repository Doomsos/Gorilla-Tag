using System;
using UnityEngine;

// Token: 0x0200072D RID: 1837
[RequireComponent(typeof(GameEntity))]
public class GRToolRevive : MonoBehaviour
{
	// Token: 0x06002F5C RID: 12124 RVA: 0x0010198F File Offset: 0x000FFB8F
	private void Awake()
	{
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x00101998 File Offset: 0x000FFB98
	private void OnEnable()
	{
		this.StopRevive();
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x00002789 File Offset: 0x00000989
	private void OnDestroy()
	{
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x001019A8 File Offset: 0x000FFBA8
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x001019D8 File Offset: 0x000FFBD8
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolRevive.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Reviving);
				return;
			}
			break;
		case GRToolRevive.State.Reviving:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolRevive.State.Cooldown);
				return;
			}
			break;
		case GRToolRevive.State.Cooldown:
			if (!this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x00101A50 File Offset: 0x000FFC50
	private void OnUpdateRemote(float dt)
	{
		GRToolRevive.State state = (GRToolRevive.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06002F62 RID: 12130 RVA: 0x00101A7A File Offset: 0x000FFC7A
	private void SetStateAuthority(GRToolRevive.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06002F63 RID: 12131 RVA: 0x00101A9C File Offset: 0x000FFC9C
	private void SetState(GRToolRevive.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (this.state == GRToolRevive.State.Reviving)
		{
			this.StopRevive();
		}
		this.state = newState;
		GRToolRevive.State state = this.state;
		if (state != GRToolRevive.State.Idle)
		{
			if (state == GRToolRevive.State.Reviving)
			{
				this.StartRevive();
				this.stateTimeRemaining = this.reviveDuration;
				return;
			}
		}
		else
		{
			this.stateTimeRemaining = -1f;
		}
	}

	// Token: 0x06002F64 RID: 12132 RVA: 0x00101AF8 File Offset: 0x000FFCF8
	private void StartRevive()
	{
		this.reviveFx.SetActive(true);
		this.audioSource.volume = this.reviveSoundVolume;
		this.audioSource.clip = this.reviveSound;
		this.audioSource.Play();
		this.tool.UseEnergy();
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		if (this.gameEntity.IsAuthority())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.5f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, this.reviveDistance, this.playerLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
					if (component != null && component.State != GRPlayer.GRPlayerState.Alive)
					{
						GhostReactorManager.Get(this.gameEntity).RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Alive);
						return;
					}
				}
			}
		}
	}

	// Token: 0x06002F65 RID: 12133 RVA: 0x00101C0A File Offset: 0x000FFE0A
	private void StopRevive()
	{
		this.reviveFx.SetActive(false);
		this.audioSource.Stop();
	}

	// Token: 0x06002F66 RID: 12134 RVA: 0x00101C24 File Offset: 0x000FFE24
	private bool IsButtonHeld()
	{
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? 4 : 5) > 0.25f;
	}

	// Token: 0x04003DEC RID: 15852
	public GameEntity gameEntity;

	// Token: 0x04003DED RID: 15853
	public GRTool tool;

	// Token: 0x04003DEE RID: 15854
	[SerializeField]
	private Transform shootFrom;

	// Token: 0x04003DEF RID: 15855
	[SerializeField]
	private LayerMask playerLayerMask;

	// Token: 0x04003DF0 RID: 15856
	[SerializeField]
	private float reviveDistance = 1.5f;

	// Token: 0x04003DF1 RID: 15857
	[SerializeField]
	private GameObject reviveFx;

	// Token: 0x04003DF2 RID: 15858
	[SerializeField]
	private float reviveSoundVolume;

	// Token: 0x04003DF3 RID: 15859
	[SerializeField]
	private AudioClip reviveSound;

	// Token: 0x04003DF4 RID: 15860
	[SerializeField]
	private float reviveDuration = 0.75f;

	// Token: 0x04003DF5 RID: 15861
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003DF6 RID: 15862
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x04003DF7 RID: 15863
	private GRToolRevive.State state;

	// Token: 0x04003DF8 RID: 15864
	private float stateTimeRemaining;

	// Token: 0x04003DF9 RID: 15865
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x0200072E RID: 1838
	private enum State
	{
		// Token: 0x04003DFB RID: 15867
		Idle,
		// Token: 0x04003DFC RID: 15868
		Reviving,
		// Token: 0x04003DFD RID: 15869
		Cooldown
	}
}
