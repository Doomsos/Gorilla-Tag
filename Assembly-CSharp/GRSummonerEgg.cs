using System;
using UnityEngine;

// Token: 0x0200070E RID: 1806
public class GRSummonerEgg : MonoBehaviour
{
	// Token: 0x06002E5E RID: 11870 RVA: 0x000FBFB6 File Offset: 0x000FA1B6
	private void Awake()
	{
		this.summonedEntity = base.GetComponent<GRSummonedEntity>();
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x000FBFC4 File Offset: 0x000FA1C4
	private void Start()
	{
		this.hatchTime = Random.Range(this.minHatchTime, this.maxHatchTime);
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = base.transform.position;
			component.rotation = base.transform.rotation;
			component.linearVelocity = Vector3.up * 2f;
			component.angularVelocity = Vector3.zero;
		}
		base.Invoke("HatchEgg", this.hatchTime);
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x000FC054 File Offset: 0x000FA254
	public void HatchEgg()
	{
		GRBreakable component = base.GetComponent<GRBreakable>();
		if (component)
		{
			component.BreakLocal();
		}
		if (this.entity.IsAuthority())
		{
			Vector3 position = this.entity.transform.position + this.spawnOffset;
			Quaternion identity = Quaternion.identity;
			GhostReactorManager.Get(this.entity).gameEntityManager.RequestCreateItem(this.entityPrefabToSpawn.name.GetStaticHash(), position, identity, (long)((this.summonedEntity != null) ? this.summonedEntity.GetSummonerNetID() : 0));
		}
		base.Invoke("DestroySelf", 2f);
		this.hatchSound.Play(this.hatchAudio);
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x000FC10B File Offset: 0x000FA30B
	public void DestroySelf()
	{
		if (this.entity.IsAuthority())
		{
			this.entity.manager.RequestDestroyItem(this.entity.id);
		}
	}

	// Token: 0x04003C7E RID: 15486
	public GameEntity entity;

	// Token: 0x04003C7F RID: 15487
	public AudioSource hatchAudio;

	// Token: 0x04003C80 RID: 15488
	public AbilitySound hatchSound;

	// Token: 0x04003C81 RID: 15489
	public GameEntity entityPrefabToSpawn;

	// Token: 0x04003C82 RID: 15490
	public Vector3 spawnOffset = new Vector3(0f, 0f, 0.3f);

	// Token: 0x04003C83 RID: 15491
	public float minHatchTime = 3f;

	// Token: 0x04003C84 RID: 15492
	public float maxHatchTime = 6f;

	// Token: 0x04003C85 RID: 15493
	private float hatchTime = 2f;

	// Token: 0x04003C86 RID: 15494
	private GRSummonedEntity summonedEntity;
}
