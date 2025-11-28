using System;
using UnityEngine;

// Token: 0x0200053D RID: 1341
public class GameBall : MonoBehaviour
{
	// Token: 0x17000392 RID: 914
	// (get) Token: 0x060021BB RID: 8635 RVA: 0x000B0407 File Offset: 0x000AE607
	public bool IsLaunched
	{
		get
		{
			return this._launched;
		}
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x000B0410 File Offset: 0x000AE610
	private void Awake()
	{
		this.id = GameBallId.Invalid;
		if (this.rigidBody == null)
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.disc && this.rigidBody != null)
		{
			this.rigidBody.maxAngularVelocity = 28f;
		}
		this.heldByActorNumber = -1;
		this.lastHeldByTeamId = -1;
		this.onlyGrabTeamId = -1;
		this._monkeBall = base.GetComponent<MonkeBall>();
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000B04A4 File Offset: 0x000AE6A4
	private void FixedUpdate()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		if (this._launched)
		{
			this._launchedTimer += Time.fixedDeltaTime;
			if (this.collider.isTrigger && this._launchedTimer > 1f && this.rigidBody.linearVelocity.y <= 0f)
			{
				this._launched = false;
				this.collider.isTrigger = false;
			}
		}
		Vector3 vector = -Physics.gravity * (1f - this.gravityMult);
		this.rigidBody.AddForce(vector * this.rigidBody.mass, 0);
		this._catchSoundDecay -= Time.deltaTime;
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x000B0569 File Offset: 0x000AE769
	public void WasLaunched()
	{
		this._launched = true;
		this.collider.isTrigger = true;
		this._launchedTimer = 0f;
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x000B0589 File Offset: 0x000AE789
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.linearVelocity;
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x000B05AA File Offset: 0x000AE7AA
	public void SetVelocity(Vector3 velocity)
	{
		this.rigidBody.linearVelocity = velocity;
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x000B05B8 File Offset: 0x000AE7B8
	public void PlayCatchFx()
	{
		if (this.audioSource != null && this._catchSoundDecay <= 0f)
		{
			this.audioSource.clip = this.catchSound;
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.Play();
			this._catchSoundDecay = 0.1f;
		}
	}

	// Token: 0x060021C2 RID: 8642 RVA: 0x000B0618 File Offset: 0x000AE818
	public void PlayThrowFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = this.throwSound;
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.Play();
		}
	}

	// Token: 0x060021C3 RID: 8643 RVA: 0x000B0655 File Offset: 0x000AE855
	public void PlayBounceFX()
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = this.groundSound;
			this.audioSource.volume = this.groundSoundVolume;
			this.audioSource.Play();
		}
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x000B0692 File Offset: 0x000AE892
	public void SetHeldByTeamId(int teamId)
	{
		this.lastHeldByTeamId = teamId;
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x000B069B File Offset: 0x000AE89B
	private bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x000B06AA File Offset: 0x000AE8AA
	public void SetVisualOffset(bool detach)
	{
		if (this._monkeBall != null)
		{
			this._monkeBall.SetVisualOffset(detach);
		}
	}

	// Token: 0x04002C69 RID: 11369
	public GameBallId id;

	// Token: 0x04002C6A RID: 11370
	public float gravityMult = 1f;

	// Token: 0x04002C6B RID: 11371
	public bool disc;

	// Token: 0x04002C6C RID: 11372
	public Vector3 localDiscUp;

	// Token: 0x04002C6D RID: 11373
	public AudioSource audioSource;

	// Token: 0x04002C6E RID: 11374
	public AudioClip catchSound;

	// Token: 0x04002C6F RID: 11375
	public float catchSoundVolume;

	// Token: 0x04002C70 RID: 11376
	private float _catchSoundDecay;

	// Token: 0x04002C71 RID: 11377
	public AudioClip throwSound;

	// Token: 0x04002C72 RID: 11378
	public float throwSoundVolume;

	// Token: 0x04002C73 RID: 11379
	public AudioClip groundSound;

	// Token: 0x04002C74 RID: 11380
	public float groundSoundVolume;

	// Token: 0x04002C75 RID: 11381
	[SerializeField]
	private Rigidbody rigidBody;

	// Token: 0x04002C76 RID: 11382
	[SerializeField]
	private Collider collider;

	// Token: 0x04002C77 RID: 11383
	public int heldByActorNumber;

	// Token: 0x04002C78 RID: 11384
	public int lastHeldByActorNumber;

	// Token: 0x04002C79 RID: 11385
	public int lastHeldByTeamId;

	// Token: 0x04002C7A RID: 11386
	public int onlyGrabTeamId;

	// Token: 0x04002C7B RID: 11387
	private bool _launched;

	// Token: 0x04002C7C RID: 11388
	private float _launchedTimer;

	// Token: 0x04002C7D RID: 11389
	public MonkeBall _monkeBall;
}
