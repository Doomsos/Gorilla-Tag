using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000689 RID: 1673
public class GRBarrierSpectral : MonoBehaviour, IGameEntityComponent, IGameHittable
{
	// Token: 0x06002AC1 RID: 10945 RVA: 0x000E6198 File Offset: 0x000E4398
	public void Awake()
	{
		this.hitFx.SetActive(false);
		this.destroyedFx.SetActive(false);
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x000E61B4 File Offset: 0x000E43B4
	public void OnEntityInit()
	{
		this.entity.SetState((long)this.health);
		Vector3 localScale = BitPackUtils.UnpackWorldPosFromNetwork(this.entity.createData);
		base.transform.localScale = localScale;
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x000E61F0 File Offset: 0x000E43F0
	public void OnEntityStateChange(long prevState, long newState)
	{
		int nextHealth = (int)newState;
		this.ChangeHealth(nextHealth);
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x000E6208 File Offset: 0x000E4408
	public void OnImpact(GameHitType hitType)
	{
		if (hitType == GameHitType.Flash)
		{
			int nextHealth = Mathf.Max(this.health - 1, 0);
			this.ChangeHealth(nextHealth);
			if (this.entity.IsAuthority())
			{
				this.entity.RequestState(this.entity.id, (long)this.health);
			}
		}
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x000E625C File Offset: 0x000E445C
	private void ChangeHealth(int nextHealth)
	{
		if (this.health != nextHealth)
		{
			this.health = nextHealth;
			if (this.health == 0)
			{
				this.collider.enabled = false;
				this.visualMesh.enabled = false;
				this.audioSource.PlayOneShot(this.onDestroyedClip, this.onDestroyedVolume);
				this.destroyedFx.SetActive(false);
				this.destroyedFx.SetActive(true);
			}
			else
			{
				this.audioSource.PlayOneShot(this.onDamageClip, this.onDamageVolume);
				this.hitFx.SetActive(false);
				this.hitFx.SetActive(true);
			}
			this.RefreshVisuals();
		}
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x00027DED File Offset: 0x00025FED
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x000E6304 File Offset: 0x000E4504
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		if (this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId) != null)
		{
			this.OnImpact(hitTypeId);
		}
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x000E6340 File Offset: 0x000E4540
	public void RefreshVisuals()
	{
		if (this.lastVisualUpdateHealth != this.health)
		{
			this.lastVisualUpdateHealth = this.health;
			Color color = this.visualMesh.material.GetColor("_BaseColor");
			color.a = (float)this.health / (float)this.maxHealth;
			this.visualMesh.material.SetColor("_BaseColor", color);
		}
	}

	// Token: 0x0400372C RID: 14124
	public GameEntity entity;

	// Token: 0x0400372D RID: 14125
	public MeshRenderer visualMesh;

	// Token: 0x0400372E RID: 14126
	public Collider collider;

	// Token: 0x0400372F RID: 14127
	public AudioSource audioSource;

	// Token: 0x04003730 RID: 14128
	public AudioClip onDamageClip;

	// Token: 0x04003731 RID: 14129
	public float onDamageVolume;

	// Token: 0x04003732 RID: 14130
	public AudioClip onDestroyedClip;

	// Token: 0x04003733 RID: 14131
	public float onDestroyedVolume;

	// Token: 0x04003734 RID: 14132
	[SerializeField]
	private GameObject hitFx;

	// Token: 0x04003735 RID: 14133
	[SerializeField]
	private GameObject destroyedFx;

	// Token: 0x04003736 RID: 14134
	public int maxHealth = 3;

	// Token: 0x04003737 RID: 14135
	[ReadOnly]
	public int health = 3;

	// Token: 0x04003738 RID: 14136
	private int lastVisualUpdateHealth = -1;
}
