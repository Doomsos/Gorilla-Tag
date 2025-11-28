using System;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

// Token: 0x020006ED RID: 1773
public class GRRecycler : MonoBehaviourTick
{
	// Token: 0x06002D69 RID: 11625 RVA: 0x000F58B8 File Offset: 0x000F3AB8
	public override void Tick()
	{
		if (this.closed && !this.anim.isPlaying)
		{
			if (!this.playedAudio)
			{
				this.audioSource.volume = this.recyclerRunningAudioVolume;
				this.audioSource.PlayOneShot(this.recyclerRunningAudio);
				this.playedAudio = true;
			}
			this.timeRemaining -= Time.deltaTime;
			if (this.timeRemaining <= 0f)
			{
				this.anim.PlayQueued("Recycler_Open", 0);
				this.closed = false;
				if (this.closeEffects != null && this.openEffects != null)
				{
					this.closeEffects.Stop();
					this.openEffects.Play();
				}
			}
		}
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x000F597B File Offset: 0x000F3B7B
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x000F5984 File Offset: 0x000F3B84
	public int GetRecycleValue(GRTool.GRToolType type)
	{
		return this.reactor.toolProgression.GetRecycleShiftCredit(type);
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x000F5997 File Offset: 0x000F3B97
	public void ScanItem(GameEntityId id)
	{
		this.scanner.ScanItem(id);
	}

	// Token: 0x06002D6D RID: 11629 RVA: 0x000F59A8 File Offset: 0x000F3BA8
	public void RecycleItem()
	{
		if (this.anim != null)
		{
			this.anim.Play("Recycler_Close");
		}
		if (this.closeEffects != null && this.openEffects != null)
		{
			this.openEffects.Stop();
			this.closeEffects.Play();
		}
		this.closed = true;
		this.playedAudio = false;
		this.timeRemaining = this.closeDuration;
	}

	// Token: 0x06002D6E RID: 11630 RVA: 0x000F5A20 File Offset: 0x000F3C20
	private void OnTriggerEnter(Collider other)
	{
		if (this.reactor == null)
		{
			Debug.LogFormat("GRRecycler reactor is null?", Array.Empty<object>());
			return;
		}
		if (!this.reactor.grManager.IsAuthority())
		{
			Debug.LogFormat("GRRecycler is not authority.", Array.Empty<object>());
			return;
		}
		GRTool componentInParent = other.gameObject.GetComponentInParent<GRTool>();
		if (componentInParent == null)
		{
			Debug.LogFormat("GRRecycler Colliding Object is not a GRTool.", Array.Empty<object>());
			return;
		}
		GRTool.GRToolType toolType = other.gameObject.GetToolType();
		int recycleValue = this.GetRecycleValue(toolType);
		if (this.reactor != null)
		{
			int count = this.reactor.vrRigs.Count;
			for (int i = 0; i < count; i++)
			{
				GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
				if (grplayer != null)
				{
					grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, (float)recycleValue);
				}
			}
		}
		Debug.LogFormat("GRRecycler Recycle Value is {0}", new object[]
		{
			recycleValue
		});
		if (GRPlayer.Get(componentInParent.gameEntity.lastHeldByActorNumber) == null)
		{
			Debug.LogFormat("GRRecycler Tool Not last held by a player (?), can't recycle.", Array.Empty<object>());
			return;
		}
		Debug.LogFormat("GRRecycler Refunding player {0} {1} Currency and Destroying Tool.", new object[]
		{
			componentInParent.gameEntity.lastHeldByActorNumber,
			recycleValue
		});
		if (toolType != GRTool.GRToolType.None)
		{
			this.reactor.grManager.RequestRecycleItem(componentInParent.gameEntity.lastHeldByActorNumber, componentInParent.gameEntity.id, toolType);
		}
	}

	// Token: 0x04003B0B RID: 15115
	private GameEntity gameEntity;

	// Token: 0x04003B0C RID: 15116
	public ParticleSystem closeEffects;

	// Token: 0x04003B0D RID: 15117
	public ParticleSystem openEffects;

	// Token: 0x04003B0E RID: 15118
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x04003B0F RID: 15119
	public GRRecyclerScanner scanner;

	// Token: 0x04003B10 RID: 15120
	public Animation anim;

	// Token: 0x04003B11 RID: 15121
	public float closeDuration = 1f;

	// Token: 0x04003B12 RID: 15122
	private float timeRemaining;

	// Token: 0x04003B13 RID: 15123
	private bool closed;

	// Token: 0x04003B14 RID: 15124
	private bool playedAudio;

	// Token: 0x04003B15 RID: 15125
	public AudioSource audioSource;

	// Token: 0x04003B16 RID: 15126
	public AudioClip recyclerRunningAudio;

	// Token: 0x04003B17 RID: 15127
	public float recyclerRunningAudioVolume = 0.5f;
}
