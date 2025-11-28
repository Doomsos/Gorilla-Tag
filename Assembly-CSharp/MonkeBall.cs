using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000549 RID: 1353
public class MonkeBall : MonoBehaviourTick
{
	// Token: 0x06002218 RID: 8728 RVA: 0x000B24B0 File Offset: 0x000B06B0
	private void Start()
	{
		this.Refresh();
	}

	// Token: 0x06002219 RID: 8729 RVA: 0x000B24B8 File Offset: 0x000B06B8
	public override void Tick()
	{
		this.UpdateVisualOffset();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._resyncPosition)
			{
				this._resyncDelay -= Time.deltaTime;
				if (this._resyncDelay <= 0f)
				{
					this._resyncPosition = false;
					GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
				}
			}
			if (this._positionFailsafe)
			{
				if (base.transform.position.y < -500f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
				{
					if (PhotonNetwork.IsConnected)
					{
						GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
					}
					else
					{
						base.transform.position = GameBallManager.Instance.transform.position;
					}
					this._positionFailsafe = false;
					this._positionFailsafeTimer = 3f;
					return;
				}
			}
			else
			{
				this._positionFailsafeTimer -= Time.deltaTime;
				if (this._positionFailsafeTimer <= 0f)
				{
					this._positionFailsafe = true;
				}
			}
			return;
		}
		if (this.gameBall.onlyGrabTeamId != -1 && Time.timeAsDouble >= this.restrictTeamGrabEndTime)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, -1);
		}
		if (this.AlreadyDropped())
		{
			this._droppedTimer += Time.deltaTime;
			if (this._droppedTimer >= 7.5f)
			{
				this._droppedTimer = 0f;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.linearVelocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._justGrabbed)
		{
			this._justGrabbedTimer -= Time.deltaTime;
			if (this._justGrabbedTimer <= 0f)
			{
				this._justGrabbed = false;
			}
		}
		if (this._resyncPosition)
		{
			this._resyncDelay -= Time.deltaTime;
			if (this._resyncDelay <= 0f)
			{
				this._resyncPosition = false;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.linearVelocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._positionFailsafe)
		{
			if (base.transform.position.y < -250f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
			{
				MonkeBallGame.Instance.LaunchBallNeutral(this.gameBall.id);
				this._positionFailsafe = false;
				this._positionFailsafeTimer = 3f;
				return;
			}
		}
		else
		{
			this._positionFailsafeTimer -= Time.deltaTime;
			if (this._positionFailsafeTimer <= 0f)
			{
				this._positionFailsafe = true;
			}
		}
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x000B27C4 File Offset: 0x000B09C4
	public void OnCollisionEnter(Collision collision)
	{
		if (this.AlreadyDropped() || this._justGrabbed)
		{
			return;
		}
		if (MonkeBall.IsGamePlayer(collision.collider))
		{
			return;
		}
		this.alreadyDropped = true;
		this._droppedTimer = 0f;
		this.gameBall.PlayBounceFX();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._rigidBody.linearVelocity.sqrMagnitude > 1f)
			{
				this._resyncPosition = true;
				this._resyncDelay = 1.5f;
			}
			int lastHeldByActorNumber = this.gameBall.lastHeldByActorNumber;
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			return;
		}
		if (this._rigidBody.linearVelocity.sqrMagnitude > 1f)
		{
			this._resyncPosition = true;
			this._resyncDelay = 0.5f;
		}
		if (this._launchAfterScore)
		{
			this._launchAfterScore = false;
			MonkeBallGame.Instance.RequestRestrictBallToTeamOnScore(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
			return;
		}
		MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000B28EB File Offset: 0x000B0AEB
	public void TriggerDelayedResync()
	{
		this._resyncPosition = true;
		if (PhotonNetwork.IsMasterClient)
		{
			this._resyncDelay = 0.5f;
			return;
		}
		this._resyncDelay = 1.5f;
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x000B2912 File Offset: 0x000B0B12
	public void SetRigidbodyDiscrete()
	{
		this._rigidBody.collisionDetectionMode = 0;
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x000B2920 File Offset: 0x000B0B20
	public void SetRigidbodyContinuous()
	{
		this._rigidBody.collisionDetectionMode = 3;
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x000B292E File Offset: 0x000B0B2E
	public static MonkeBall Get(GameBall ball)
	{
		if (ball == null)
		{
			return null;
		}
		return ball.GetComponent<MonkeBall>();
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x000B2941 File Offset: 0x000B0B41
	public bool AlreadyDropped()
	{
		return this.alreadyDropped;
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x000B2949 File Offset: 0x000B0B49
	public void OnGrabbed()
	{
		this.alreadyDropped = false;
		this._justGrabbed = true;
		this._justGrabbedTimer = 0.1f;
		this._resyncPosition = false;
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x000B296B File Offset: 0x000B0B6B
	public void OnSwitchHeldByTeam(int teamId)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, teamId);
		}
	}

	// Token: 0x06002222 RID: 8738 RVA: 0x000B298A File Offset: 0x000B0B8A
	public void ClearCannotGrabTeamId()
	{
		this.gameBall.onlyGrabTeamId = -1;
		this.restrictTeamGrabEndTime = -1.0;
		this.Refresh();
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x000B29B0 File Offset: 0x000B0BB0
	public bool RestrictBallToTeam(int teamId, float duration)
	{
		if (teamId == this.gameBall.onlyGrabTeamId && Time.timeAsDouble + (double)duration < this.restrictTeamGrabEndTime)
		{
			return false;
		}
		this.gameBall.onlyGrabTeamId = teamId;
		this.restrictTeamGrabEndTime = Time.timeAsDouble + (double)duration;
		this.Refresh();
		return true;
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x000B29FE File Offset: 0x000B0BFE
	private void Refresh()
	{
		if (this.gameBall.onlyGrabTeamId == -1)
		{
			this.mainRenderer.material = this.defaultMaterial;
			return;
		}
		this.mainRenderer.material = this.teamMaterial[this.gameBall.onlyGrabTeamId];
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x000B2A3D File Offset: 0x000B0C3D
	private static bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06002226 RID: 8742 RVA: 0x000B2A4C File Offset: 0x000B0C4C
	public void SetVisualOffset(bool detach)
	{
		if (detach)
		{
			this.lastVisiblePosition = this.mainRenderer.transform.position;
			this._visualOffset = true;
			this._timeOffset = Time.time;
			this.mainRenderer.transform.SetParent(null, true);
			return;
		}
		this.ReattachVisuals();
	}

	// Token: 0x06002227 RID: 8743 RVA: 0x000B2AA0 File Offset: 0x000B0CA0
	private void ReattachVisuals()
	{
		if (!this._visualOffset)
		{
			return;
		}
		this.mainRenderer.transform.SetParent(base.transform);
		this.mainRenderer.transform.localPosition = Vector3.zero;
		this.mainRenderer.transform.localRotation = Quaternion.identity;
		this._visualOffset = false;
	}

	// Token: 0x06002228 RID: 8744 RVA: 0x000B2B00 File Offset: 0x000B0D00
	private void UpdateVisualOffset()
	{
		if (this._visualOffset)
		{
			this.mainRenderer.transform.position = Vector3.Lerp(this.mainRenderer.transform.position, this._rigidBody.position, Mathf.Clamp((Time.time - this._timeOffset) / this.maxLerpTime, this.offsetLerp, 1f));
			if ((this.mainRenderer.transform.position - this._rigidBody.position).sqrMagnitude < this._offsetThreshold)
			{
				this.ReattachVisuals();
			}
		}
	}

	// Token: 0x04002CB4 RID: 11444
	public GameBall gameBall;

	// Token: 0x04002CB5 RID: 11445
	public MeshRenderer mainRenderer;

	// Token: 0x04002CB6 RID: 11446
	public Material defaultMaterial;

	// Token: 0x04002CB7 RID: 11447
	public Material[] teamMaterial;

	// Token: 0x04002CB8 RID: 11448
	public double restrictTeamGrabEndTime;

	// Token: 0x04002CB9 RID: 11449
	public bool alreadyDropped;

	// Token: 0x04002CBA RID: 11450
	private bool _justGrabbed;

	// Token: 0x04002CBB RID: 11451
	private float _justGrabbedTimer;

	// Token: 0x04002CBC RID: 11452
	private bool _launchAfterScore;

	// Token: 0x04002CBD RID: 11453
	private float _droppedTimer;

	// Token: 0x04002CBE RID: 11454
	private bool _resyncPosition;

	// Token: 0x04002CBF RID: 11455
	private float _resyncDelay;

	// Token: 0x04002CC0 RID: 11456
	private bool _visualOffset;

	// Token: 0x04002CC1 RID: 11457
	private float _offsetThreshold = 0.05f;

	// Token: 0x04002CC2 RID: 11458
	private float _timeOffset;

	// Token: 0x04002CC3 RID: 11459
	public float maxLerpTime = 0.5f;

	// Token: 0x04002CC4 RID: 11460
	public float offsetLerp = 0.2f;

	// Token: 0x04002CC5 RID: 11461
	private bool _positionFailsafe = true;

	// Token: 0x04002CC6 RID: 11462
	private float _positionFailsafeTimer;

	// Token: 0x04002CC7 RID: 11463
	public Vector3 lastVisiblePosition;

	// Token: 0x04002CC8 RID: 11464
	[SerializeField]
	private Rigidbody _rigidBody;
}
