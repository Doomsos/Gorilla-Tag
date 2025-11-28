using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000621 RID: 1569
public class GameHitter : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x060027DB RID: 10203 RVA: 0x000D413A File Offset: 0x000D233A
	private void Awake()
	{
		this.components = new List<IGameHitter>(1);
		base.GetComponentsInChildren<IGameHitter>(this.components);
		this.attributes = base.GetComponent<GRAttributes>();
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x000D4160 File Offset: 0x000D2360
	public void OnEntityInit()
	{
		GRTool component = base.GetComponent<GRTool>();
		if (component != null)
		{
			component.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(component);
		}
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x000D4196 File Offset: 0x000D2396
	private void OnToolUpgraded(GRTool tool)
	{
		if (this.attributes.HasValueForAttribute(GRAttributeType.KnockbackMultiplier))
		{
			this.knockbackMultiplier = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.KnockbackMultiplier);
		}
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x000D41BC File Offset: 0x000D23BC
	public void ApplyHit(GameHitData hitData)
	{
		if (this.hitFx.hitSound != null)
		{
			this.hitFx.hitSound.Play(null);
		}
		if (this.hitFx.hitEffect != null)
		{
			this.hitFx.hitEffect.Stop();
			this.hitFx.hitEffect.Play();
		}
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnSuccessfulHit(hitData);
		}
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, 0.2f);
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
			if (gamePlayer != null)
			{
				int num = gamePlayer.FindHandIndex(this.gameEntity.id);
				if (num != -1)
				{
					GTPlayer.Instance.TempFreezeHand(GamePlayer.IsLeftHand(num), 0.15f);
				}
			}
		}
		if (GRNoiseEventManager.instance != null)
		{
			GRNoiseEventManager.instance.AddNoiseEvent(hitData.hitPosition, 1f, 1f);
		}
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x000D42D0 File Offset: 0x000D24D0
	public void ApplyHitToPlayer(GRPlayer player, Vector3 hitPosition)
	{
		this.hitFx.hitSound.Play(null);
		if (this.hitFx.hitEffect != null)
		{
			this.hitFx.hitEffect.Play();
		}
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnSuccessfulHitPlayer(player, hitPosition);
		}
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x000D433C File Offset: 0x000D253C
	private void PlayVibration(float strength, float duration)
	{
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x000D439C File Offset: 0x000D259C
	private T GetParentEnemy<T>(Collider collider) where T : MonoBehaviour
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x000D43E4 File Offset: 0x000D25E4
	public int CalcHitAmount(GameHitType hitType, GameHittable hittable, GameEntity hitByEntity)
	{
		int result = 0;
		if (hitByEntity != null)
		{
			GRAttributes component = hitByEntity.GetComponent<GRAttributes>();
			if (component != null)
			{
				switch (hitType)
				{
				case GameHitType.Club:
					result = component.CalculateFinalValueForAttribute(this.damageAttribute);
					break;
				case GameHitType.Flash:
					result = component.CalculateFinalValueForAttribute(this.flashDamageAttribute);
					break;
				case GameHitType.Shield:
					result = component.CalculateFinalValueForAttribute(this.shieldDamageAttribute);
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x000D444C File Offset: 0x000D264C
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.hitOnCollision)
		{
			return;
		}
		float num = this.gameEntity.GetVelocity().sqrMagnitude;
		if (this.gameEntity.lastHeldByActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			return;
		}
		bool flag = false;
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer != null)
		{
			float handSpeed = GamePlayerLocal.instance.GetHandSpeed(gamePlayer.FindHandIndex(this.gameEntity.id));
			num = handSpeed * handSpeed;
		}
		if (num < this.minSwingSpeed * this.minSwingSpeed)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.hitCooldownEnd)
		{
			return;
		}
		Collider collider = collision.collider;
		GameHittable parentEnemy = this.GetParentEnemy<GameHittable>(collider);
		if (parentEnemy != null)
		{
			Vector3 vector = parentEnemy.transform.position - base.transform.position;
			vector.Normalize();
			if (!flag && gamePlayer != null)
			{
				vector = GamePlayerLocal.instance.GetHandVelocity(gamePlayer.FindHandIndex(this.gameEntity.id)).normalized;
			}
			float num2 = Mathf.Sqrt(num);
			num2 = Mathf.Min(num2, this.maxImpulseSpeed);
			vector *= num2;
			Vector3 position = parentEnemy.transform.position;
			GameHitData hitData = new GameHitData
			{
				hitTypeId = (int)this.hitType,
				hitEntityId = parentEnemy.gameEntity.id,
				hitByEntityId = this.gameEntity.id,
				hitEntityPosition = position,
				hitImpulse = vector * this.knockbackMultiplier,
				hitPosition = collision.GetContact(0).point,
				hitAmount = this.CalcHitAmount(this.hitType, parentEnemy, this.gameEntity)
			};
			if (parentEnemy.IsHitValid(hitData))
			{
				parentEnemy.RequestHit(hitData);
				this.hitCooldownEnd = timeAsDouble + 0.10000000149011612;
			}
		}
	}

	// Token: 0x04003352 RID: 13138
	public GameEntity gameEntity;

	// Token: 0x04003353 RID: 13139
	public GameHitType hitType;

	// Token: 0x04003354 RID: 13140
	public GRAttributeType damageAttribute = GRAttributeType.BatonDamage;

	// Token: 0x04003355 RID: 13141
	public GRAttributeType flashDamageAttribute = GRAttributeType.FlashDamage;

	// Token: 0x04003356 RID: 13142
	public GRAttributeType shieldDamageAttribute = GRAttributeType.BatonDamage;

	// Token: 0x04003357 RID: 13143
	public float minSwingSpeed = 1.5f;

	// Token: 0x04003358 RID: 13144
	public GameHitFx hitFx;

	// Token: 0x04003359 RID: 13145
	private GRAttributes attributes;

	// Token: 0x0400335A RID: 13146
	public float knockbackMultiplier = 1f;

	// Token: 0x0400335B RID: 13147
	public float maxImpulseSpeed = 4.5f;

	// Token: 0x0400335C RID: 13148
	private List<IGameHitter> components;

	// Token: 0x0400335D RID: 13149
	private double hitCooldownEnd;

	// Token: 0x0400335E RID: 13150
	public bool hitOnCollision = true;
}
