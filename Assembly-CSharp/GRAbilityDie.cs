using System;
using System.Collections.Generic;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

// Token: 0x02000676 RID: 1654
[Serializable]
public class GRAbilityDie : GRAbilityBase
{
	// Token: 0x06002A49 RID: 10825 RVA: 0x000E4190 File Offset: 0x000E2390
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.disableAllCollidersWhenDead)
		{
			agent.GetComponentsInChildren<Collider>(this.disableCollidersWhenDead);
		}
		if (this.disableAllRenderersWhenDead)
		{
			agent.GetComponentsInChildren<Renderer>(this.hideWhenDead);
		}
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
		this.staggerMovement.Setup(root);
	}

	// Token: 0x06002A4A RID: 10826 RVA: 0x000E41F0 File Offset: 0x000E23F0
	public override void Start()
	{
		base.Start();
		if (this.animData.Count > 0)
		{
			int num = Random.Range(0, this.animData.Count);
			this.delayDeath = this.animData[num].duration;
			this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.delayDeath);
			this.PlayAnim(this.animData[num].animName, 0.1f, this.animData[num].speed);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.isDead = false;
		if (this.doKnockback)
		{
			this.staggerMovement.Start();
		}
		this.soundDeath.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundOnHide.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundDeath.Play(null);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
	}

	// Token: 0x06002A4B RID: 10827 RVA: 0x000E42E6 File Offset: 0x000E24E6
	public override void Stop()
	{
		this.staggerMovement.Stop();
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		GRAbilityDie.Hide(this.hideWhenDead, false);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x000E4324 File Offset: 0x000E2524
	public void SetStaggerVelocity(Vector3 vel)
	{
		float magnitude = vel.magnitude;
		if (magnitude > 0f)
		{
			Vector3 vector = vel / magnitude;
			vector.y = 0f;
			vel = vector * magnitude;
		}
		this.staggerMovement.InitFromVelocityAndDuration(vel, this.delayDeath);
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x000E4370 File Offset: 0x000E2570
	public void SetInstigatingPlayerIndex(int actorNumber)
	{
		this.instigatingActorNumber = actorNumber;
	}

	// Token: 0x06002A4E RID: 10830 RVA: 0x000E437C File Offset: 0x000E257C
	private void Die()
	{
		this.soundOnHide.Play(null);
		if (this.fxDeath != null)
		{
			this.fxDeath.SetActive(false);
			this.fxDeath.SetActive(true);
		}
		GRAbilityDie.Hide(this.hideWhenDead, true);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
		GameEntity entity = this.agent.entity;
		GameEntity gameEntity;
		if (this.lootTable != null && entity.IsAuthority() && this.lootTable.TryForRandomItem(entity, out gameEntity, 0))
		{
			Transform transform = this.lootSpawnMarker;
			if (transform == null)
			{
				transform = this.agent.transform;
			}
			Vector3 position = transform.position;
			if (transform == null)
			{
				position.y += 0.33f;
			}
			entity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), position, transform.rotation, 0L);
		}
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x000E4468 File Offset: 0x000E2668
	public void DestroySelf()
	{
		GameEntity entity = this.agent.entity;
		GRPlayer grplayer = GRPlayer.Get(this.instigatingActorNumber);
		if (grplayer != null)
		{
			grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Kills, 1f);
		}
		GREnemyType? enemyType = entity.GetEnemyType();
		if (enemyType != null)
		{
			GREnemyType valueOrDefault = enemyType.GetValueOrDefault();
			GhostReactor.instance.shiftManager.shiftStats.IncrementEnemyKills(valueOrDefault);
		}
		if (entity.IsAuthority())
		{
			entity.manager.RequestDestroyItem(entity.id);
		}
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x000E44E8 File Offset: 0x000E26E8
	protected override void UpdateShared(float dt)
	{
		if (this.startTime >= 0.0)
		{
			if (this.doKnockback)
			{
				this.staggerMovement.Update(dt);
			}
			double num = Time.timeAsDouble - this.startTime;
			if (!this.isDead && num > (double)this.delayDeath)
			{
				this.isDead = true;
				this.Die();
				return;
			}
			if (this.isDead && num > (double)(this.delayDeath + this.destroyDelay))
			{
				GhostReactorManager.Get(this.entity).OnAbilityDie(this.entity);
				this.DestroySelf();
				this.startTime = -1.0;
			}
		}
	}

	// Token: 0x06002A52 RID: 10834 RVA: 0x000E4590 File Offset: 0x000E2790
	public static void Hide(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002A53 RID: 10835 RVA: 0x000E45D4 File Offset: 0x000E27D4
	public static void Disable(List<Collider> colliders, bool disable)
	{
		if (colliders == null)
		{
			return;
		}
		for (int i = 0; i < colliders.Count; i++)
		{
			if (colliders[i] != null)
			{
				colliders[i].enabled = !disable;
			}
		}
	}

	// Token: 0x04003697 RID: 13975
	public float delayDeath;

	// Token: 0x04003698 RID: 13976
	public List<Renderer> hideWhenDead;

	// Token: 0x04003699 RID: 13977
	public List<Collider> disableCollidersWhenDead;

	// Token: 0x0400369A RID: 13978
	public bool disableAllCollidersWhenDead;

	// Token: 0x0400369B RID: 13979
	public bool disableAllRenderersWhenDead;

	// Token: 0x0400369C RID: 13980
	public GameObject fxDeath;

	// Token: 0x0400369D RID: 13981
	public AbilitySound soundDeath;

	// Token: 0x0400369E RID: 13982
	public AbilitySound soundOnHide;

	// Token: 0x0400369F RID: 13983
	public float destroyDelay = 3f;

	// Token: 0x040036A0 RID: 13984
	public bool doKnockback = true;

	// Token: 0x040036A1 RID: 13985
	public GRBreakableItemSpawnConfig lootTable;

	// Token: 0x040036A2 RID: 13986
	public Transform lootSpawnMarker;

	// Token: 0x040036A3 RID: 13987
	public List<AnimationData> animData;

	// Token: 0x040036A4 RID: 13988
	private int instigatingActorNumber;

	// Token: 0x040036A5 RID: 13989
	private bool isDead;

	// Token: 0x040036A6 RID: 13990
	public GRAbilityInterpolatedMovement staggerMovement;
}
