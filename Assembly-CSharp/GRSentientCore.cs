using System;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x020006FF RID: 1791
public class GRSentientCore : MonoBehaviour, IGRSleepableEntity
{
	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x06002DF0 RID: 11760 RVA: 0x00094550 File Offset: 0x00092750
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x06002DF1 RID: 11761 RVA: 0x000F97E6 File Offset: 0x000F79E6
	public float WakeUpRadius
	{
		get
		{
			return this.wakeupRadius;
		}
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x000F97F0 File Offset: 0x000F79F0
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		GhostReactor.instance.sleepableEntities.Add(this);
		this.gameEntity.OnStateChanged += this.OnStateChanged;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnSnapped = (Action)Delegate.Combine(gameEntity3.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnDetached = (Action)Delegate.Combine(gameEntity4.OnDetached, new Action(this.OnDetached));
		this.Sleep();
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x000F98D4 File Offset: 0x000F7AD4
	private void OnDestroy()
	{
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.sleepableEntities.Remove(this);
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnStateChanged;
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
			GameEntity gameEntity2 = this.gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
			GameEntity gameEntity3 = this.gameEntity;
			gameEntity3.OnSnapped = (Action)Delegate.Remove(gameEntity3.OnSnapped, new Action(this.OnSnapped));
			GameEntity gameEntity4 = this.gameEntity;
			gameEntity4.OnDetached = (Action)Delegate.Remove(gameEntity4.OnDetached, new Action(this.OnDetached));
		}
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x000F99C3 File Offset: 0x000F7BC3
	public bool IsSleeping()
	{
		return this.gameEntity.GetState() == 0L;
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x000F99D4 File Offset: 0x000F7BD4
	public void WakeUp()
	{
		if (this.gameEntity.IsAuthority() && this.IsSleeping())
		{
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
		}
		if (this.localState == GRSentientCore.SentientCoreState.Asleep)
		{
			this.localState = GRSentientCore.SentientCoreState.Awake;
			this.localStateStartTime = Time.time;
		}
		this.sleepRequested = false;
		base.enabled = true;
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x000F9A36 File Offset: 0x000F7C36
	public void Sleep()
	{
		this.sleepRequested = true;
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x000F9A3F File Offset: 0x000F7C3F
	private void OnStateChanged(long prevState, long nextState)
	{
		if ((int)nextState == 0)
		{
			this.sleepRequested = false;
		}
		else if (!base.enabled)
		{
			this.WakeUp();
		}
		this.SetState((GRSentientCore.SentientCoreState)nextState);
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x000F9A64 File Offset: 0x000F7C64
	private void OnGrabbed()
	{
		this.WakeUp();
		this.SetState(GRSentientCore.SentientCoreState.Held);
		this.timeUntilNextAlert = Mathf.Min(this.timeUntilFirstAlert, this.timeUntilNextAlert);
	}

	// Token: 0x06002DF9 RID: 11769 RVA: 0x000F9A8A File Offset: 0x000F7C8A
	private void OnReleased()
	{
		this.SetState(GRSentientCore.SentientCoreState.Dropped);
	}

	// Token: 0x06002DFA RID: 11770 RVA: 0x000F9A93 File Offset: 0x000F7C93
	private void OnSnapped()
	{
		this.SetState(GRSentientCore.SentientCoreState.AttachedToPlayer);
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x000F9A8A File Offset: 0x000F7C8A
	private void OnDetached()
	{
		this.SetState(GRSentientCore.SentientCoreState.Dropped);
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x000F9A9C File Offset: 0x000F7C9C
	private void Update()
	{
		if (this.debugDraw)
		{
			DebugUtil.DrawSphere(base.transform.position, 0.15f, 12, 12, Color.cyan, true, DebugUtil.Style.Wireframe);
		}
		if (this.gameEntity.IsAuthority())
		{
			this.AuthorityUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x000F9AEC File Offset: 0x000F7CEC
	private void AuthorityUpdate()
	{
		if (this.trailFX != null)
		{
			if (this.gameEntity.snappedByActorNumber != -1 || this.gameEntity.heldByActorNumber != -1)
			{
				if (this.trailFX.isPlaying)
				{
					this.trailFX.Stop();
				}
			}
			else if (!this.trailFX.isPlaying)
			{
				this.trailFX.Play();
			}
		}
		switch (this.localState)
		{
		case GRSentientCore.SentientCoreState.Asleep:
		case GRSentientCore.SentientCoreState.JumpAnticipation:
		case GRSentientCore.SentientCoreState.Jumping:
		case GRSentientCore.SentientCoreState.HeldAlert:
		case GRSentientCore.SentientCoreState.Dropped:
			break;
		case GRSentientCore.SentientCoreState.Awake:
			if (this.sleepRequested)
			{
				this.sleepRequested = false;
				this.SetState(GRSentientCore.SentientCoreState.Asleep);
			}
			if (this.gameEntity.heldByActorNumber != -1)
			{
				this.SetState(GRSentientCore.SentientCoreState.Held);
				return;
			}
			if (!this.sleepRequested && Time.time > this.localStateStartTime + this.jumpCooldownTime)
			{
				this.AuthorityInitiateJump();
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.JumpInitiated:
			if (this.sleepRequested)
			{
				this.sleepRequested = false;
				this.SetState(GRSentientCore.SentientCoreState.Asleep);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.Held:
			this.timeUntilNextAlert -= Time.deltaTime;
			if (this.timeUntilNextAlert < 0f)
			{
				this.timeUntilNextAlert = Random.Range(this.timeRangeBetweenAlerts.x, this.timeRangeBetweenAlerts.y);
				this.SetState(GRSentientCore.SentientCoreState.HeldAlert);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.AttachedToPlayer:
			this.timeUntilNextAlert -= Time.deltaTime;
			if (this.timeUntilNextAlert < 0f)
			{
				this.timeUntilNextAlert = Random.Range(this.timeRangeBetweenAlerts.x, this.timeRangeBetweenAlerts.y);
				this.alertEnemiesSound.Play(null);
				GRNoiseEventManager.instance.AddNoiseEvent(base.transform.position, this.alertNoiseEventMagnitude, this.enemyAlertDuration);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x000F9CB0 File Offset: 0x000F7EB0
	private void SharedUpdate()
	{
		switch (this.localState)
		{
		default:
			base.enabled = false;
			return;
		case GRSentientCore.SentientCoreState.Awake:
			if (this.visualCore != null && this.visualCore.transform.localScale != Vector3.one)
			{
				this.visualCore.transform.localScale = Vector3.one;
				this.visualCore.transform.localPosition = Vector3.zero;
				this.visualCore.transform.localRotation = Quaternion.identity;
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.JumpAnticipation:
			if (this.debugDraw)
			{
				this.DrawJumpPath(Color.yellow);
			}
			if (Time.time <= this.jumpStartTime)
			{
				Vector3 normalized = (this.surfaceNormal + this.jumpDirection).normalized;
				float num = (this.jumpStartTime - Time.time) / this.jumpAnticipationTime * 0.25f + 0.75f;
				float num2 = Mathf.Sqrt(1f / num);
				this.visualCore.transform.localScale = new Vector3(num2, num, num2);
				this.visualCore.transform.position = this.visualCore.parent.position - normalized * (1f - num) * this.radius;
				this.visualCore.transform.rotation = Quaternion.FromToRotation(Vector3.up, normalized);
				return;
			}
			this.SetState(GRSentientCore.SentientCoreState.Jumping);
			this.jumpSound.Play(null);
			if (this.visualCore != null)
			{
				this.visualCore.transform.localScale = Vector3.one;
				this.visualCore.transform.localPosition = Vector3.zero;
				this.visualCore.transform.localRotation = Quaternion.identity;
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.Jumping:
		{
			if (this.debugDraw)
			{
				this.DrawJumpPath(Color.yellow);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector = base.transform.position + this.jumpVelocity * deltaTime;
			Vector3 vector2 = this.useSurfaceNormalForGravityDirection ? (-this.surfaceNormal) : Vector3.down;
			this.jumpVelocity += vector2 * (this.jumpGravityAccel * deltaTime);
			float magnitude = this.jumpVelocity.magnitude;
			if (magnitude > this.maxSpeed && this.maxSpeed > 0f)
			{
				this.jumpVelocity *= this.maxSpeed / magnitude;
			}
			float magnitude2 = (vector - base.transform.position).magnitude;
			Vector3 vector3 = (magnitude2 > 0.001f) ? ((vector - base.transform.position) / magnitude2) : Vector3.zero;
			RaycastHit raycastHit;
			if (Physics.SphereCast(new Ray(base.transform.position, vector3), this.radius, ref raycastHit, magnitude2, GTPlayer.Instance.locomotionEnabledLayers.value, 1))
			{
				vector = base.transform.position + vector3 * raycastHit.distance;
				this.surfaceNormal = raycastHit.normal;
				this.SetState(GRSentientCore.SentientCoreState.Awake);
				this.landSound.Play(null);
			}
			base.transform.position = vector;
			return;
		}
		case GRSentientCore.SentientCoreState.Held:
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer != null)
			{
				grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.TimeChaosExposure, Time.deltaTime);
			}
			this.isPlayingAlert = false;
			return;
		}
		case GRSentientCore.SentientCoreState.HeldAlert:
			if (!this.isPlayingAlert)
			{
				this.isPlayingAlert = true;
				this.alertEnemiesSound.Play(null);
				GRNoiseEventManager.instance.AddNoiseEvent(base.transform.position, this.alertNoiseEventMagnitude, this.enemyAlertDuration);
			}
			if (Time.time - this.localStateStartTime > this.enemyAlertDuration)
			{
				this.SetState(GRSentientCore.SentientCoreState.Held);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.AttachedToPlayer:
		{
			GRPlayer grplayer2 = GRPlayer.Get(this.gameEntity.snappedByActorNumber);
			if (grplayer2 != null)
			{
				grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.TimeChaosExposure, Time.deltaTime);
				return;
			}
			break;
		}
		case GRSentientCore.SentientCoreState.Dropped:
		{
			float deltaTime2 = Time.deltaTime;
			Vector3 vector4 = base.transform.position + this.rb.linearVelocity * deltaTime2;
			float magnitude3 = (vector4 - base.transform.position).magnitude;
			Vector3 vector5 = (magnitude3 > 0.001f) ? ((vector4 - base.transform.position) / magnitude3) : Vector3.zero;
			RaycastHit raycastHit2;
			if (Physics.SphereCast(new Ray(base.transform.position, vector5), this.radius, ref raycastHit2, magnitude3, GTPlayer.Instance.locomotionEnabledLayers.value, 1))
			{
				vector4 = base.transform.position + vector5 * raycastHit2.distance;
				this.surfaceNormal = raycastHit2.normal;
				base.transform.position = vector4;
				this.rb.isKinematic = true;
				this.SetState(GRSentientCore.SentientCoreState.Awake);
			}
			break;
		}
		}
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x000FA1CC File Offset: 0x000F83CC
	private void SetState(GRSentientCore.SentientCoreState nextState)
	{
		if (this.localState != nextState)
		{
			this.localState = nextState;
			this.localStateStartTime = Time.time;
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, (long)nextState);
			}
		}
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x000FA21C File Offset: 0x000F841C
	public void PerformJump(Vector3 startPos, Vector3 normal, Vector3 direction, double jumpNetworkTime)
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!base.enabled || this.IsSleeping())
		{
			this.WakeUp();
		}
		base.transform.position = startPos;
		float num = Mathf.Clamp((float)(jumpNetworkTime - PhotonNetwork.Time), 0f, this.jumpAnticipationTime);
		this.jumpStartTime = Time.time + num;
		this.jumpDirection = direction;
		this.jumpDirection.Normalize();
		this.jumpStartPosition = startPos;
		this.surfaceNormal = normal;
		this.jumpVelocity = this.jumpDirection * this.jumpSpeed;
		this.SetState(GRSentientCore.SentientCoreState.JumpAnticipation);
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x000FA2B8 File Offset: 0x000F84B8
	private void DrawJumpPath(Color pathColor)
	{
		DebugUtil.DrawLine(this.jumpStartPosition, this.jumpStartPosition + this.surfaceNormal * 0.15f, Color.cyan, true);
		float num = 0.016666f;
		int num2 = 100;
		Vector3 vector = this.jumpStartPosition;
		Vector3 vector2 = this.jumpDirection * this.jumpSpeed;
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector3 = vector + vector2 * num;
			vector2 += -this.surfaceNormal * (this.jumpGravityAccel * num);
			float magnitude = (vector3 - vector).magnitude;
			Vector3 vector4 = (magnitude > 0.001f) ? ((vector3 - vector) / magnitude) : Vector3.zero;
			RaycastHit raycastHit;
			if (Physics.SphereCast(new Ray(vector, vector4), this.radius, ref raycastHit, magnitude, GTPlayer.Instance.locomotionEnabledLayers.value, 1))
			{
				vector3 = raycastHit.point;
				DebugUtil.DrawLine(vector, vector3, pathColor, true);
				DebugUtil.DrawLine(vector3, vector3 + raycastHit.normal * 0.15f, Color.cyan, true);
				DebugUtil.DrawSphere(raycastHit.point, 0.1f, 12, 12, pathColor, true, DebugUtil.Style.Wireframe);
				return;
			}
			DebugUtil.DrawLine(vector, vector3, pathColor, true);
			vector = vector3;
		}
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x000FA414 File Offset: 0x000F8614
	public void AuthorityInitiateJump()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		if (Vector3.Dot(insideUnitSphere, this.surfaceNormal) > 0.99f)
		{
			insideUnitSphere..ctor(this.surfaceNormal.y, this.surfaceNormal.z, this.surfaceNormal.x);
		}
		float num = Random.Range(this.jumpAngleMinMax.x, this.jumpAngleMinMax.y);
		Vector3 direction = Quaternion.AngleAxis(90f - num, Vector3.Cross(this.surfaceNormal, insideUnitSphere)) * this.surfaceNormal;
		direction.Normalize();
		this.SetState(GRSentientCore.SentientCoreState.JumpInitiated);
		this.gameEntity.manager.ghostReactorManager.RequestSentientCorePerformJump(this.gameEntity, base.transform.position, this.surfaceNormal, direction, this.jumpAnticipationTime);
	}

	// Token: 0x04003BFA RID: 15354
	public GameEntity gameEntity;

	// Token: 0x04003BFB RID: 15355
	public Vector2 jumpAngleMinMax = new Vector2(30f, 60f);

	// Token: 0x04003BFC RID: 15356
	public float jumpSpeed = 3f;

	// Token: 0x04003BFD RID: 15357
	public float jumpGravityAccel = 10f;

	// Token: 0x04003BFE RID: 15358
	public float maxSpeed = 5f;

	// Token: 0x04003BFF RID: 15359
	public float radius = 0.14f;

	// Token: 0x04003C00 RID: 15360
	public float jumpAnticipationTime = 1f;

	// Token: 0x04003C01 RID: 15361
	public float jumpCooldownTime = 2f;

	// Token: 0x04003C02 RID: 15362
	public bool useSurfaceNormalForGravityDirection = true;

	// Token: 0x04003C03 RID: 15363
	public Vector2 timeRangeBetweenAlerts = new Vector2(7f, 12f);

	// Token: 0x04003C04 RID: 15364
	public float timeUntilFirstAlert = 0.5f;

	// Token: 0x04003C05 RID: 15365
	public float alertNoiseEventMagnitude = 1f;

	// Token: 0x04003C06 RID: 15366
	public AbilitySound jumpSound;

	// Token: 0x04003C07 RID: 15367
	public AbilitySound landSound;

	// Token: 0x04003C08 RID: 15368
	public AbilitySound alertEnemiesSound;

	// Token: 0x04003C09 RID: 15369
	public float wakeupRadius = 3f;

	// Token: 0x04003C0A RID: 15370
	public bool debugDraw;

	// Token: 0x04003C0B RID: 15371
	public Transform visualCore;

	// Token: 0x04003C0C RID: 15372
	public ParticleSystem trailFX;

	// Token: 0x04003C0D RID: 15373
	private Vector3 surfaceNormal = Vector3.up;

	// Token: 0x04003C0E RID: 15374
	private Vector3 jumpDirection = Vector3.up;

	// Token: 0x04003C0F RID: 15375
	private Vector3 jumpStartPosition;

	// Token: 0x04003C10 RID: 15376
	private Vector3 jumpVelocity;

	// Token: 0x04003C11 RID: 15377
	private float jumpStartTime;

	// Token: 0x04003C12 RID: 15378
	private Rigidbody rb;

	// Token: 0x04003C13 RID: 15379
	private float timeUntilNextAlert = 7f;

	// Token: 0x04003C14 RID: 15380
	private float enemyAlertDuration = 1f;

	// Token: 0x04003C15 RID: 15381
	private bool isPlayingAlert;

	// Token: 0x04003C16 RID: 15382
	private bool sleepRequested;

	// Token: 0x04003C17 RID: 15383
	[ReadOnly]
	public GRSentientCore.SentientCoreState localState = GRSentientCore.SentientCoreState.Awake;

	// Token: 0x04003C18 RID: 15384
	private float localStateStartTime;

	// Token: 0x02000700 RID: 1792
	public enum SentientCoreState
	{
		// Token: 0x04003C1A RID: 15386
		Asleep,
		// Token: 0x04003C1B RID: 15387
		Awake,
		// Token: 0x04003C1C RID: 15388
		JumpInitiated,
		// Token: 0x04003C1D RID: 15389
		JumpAnticipation,
		// Token: 0x04003C1E RID: 15390
		Jumping,
		// Token: 0x04003C1F RID: 15391
		Held,
		// Token: 0x04003C20 RID: 15392
		HeldAlert,
		// Token: 0x04003C21 RID: 15393
		AttachedToPlayer,
		// Token: 0x04003C22 RID: 15394
		Dropped
	}
}
