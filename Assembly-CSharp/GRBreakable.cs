using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068E RID: 1678
public class GRBreakable : MonoBehaviour, IGameHittable
{
	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x06002ADC RID: 10972 RVA: 0x000E6B06 File Offset: 0x000E4D06
	public bool BrokenLocal
	{
		get
		{
			return this.brokenLocal;
		}
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x000E6B0E File Offset: 0x000E4D0E
	private void OnEnable()
	{
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x000E6B27 File Offset: 0x000E4D27
	private void OnDisable()
	{
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x06002ADF RID: 10975 RVA: 0x000E6B50 File Offset: 0x000E4D50
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		GRBreakable.BreakableState breakableState = (GRBreakable.BreakableState)nextState;
		if (breakableState == GRBreakable.BreakableState.Broken)
		{
			this.BreakLocal();
			return;
		}
		if (breakableState == GRBreakable.BreakableState.Unbroken)
		{
			this.RestoreLocal();
		}
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x000E6B74 File Offset: 0x000E4D74
	public void BreakLocal()
	{
		if (!this.brokenLocal)
		{
			this.brokenLocal = true;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = false;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(true);
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.breakSound, this.breakSoundVolume);
			}
			GameEntity gameEntity;
			if (this.gameEntity.IsAuthority() && this.holdsRandomItem && this.itemSpawnProbability.TryForRandomItem(this.gameEntity, out gameEntity, 0))
			{
				this.gameEntity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), this.itemSpawnLocation.position, this.itemSpawnLocation.rotation, 0L);
			}
		}
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x000E6C90 File Offset: 0x000E4E90
	public void RestoreLocal()
	{
		if (this.brokenLocal)
		{
			this.brokenLocal = false;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = true;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(true);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x000E6D20 File Offset: 0x000E4F20
	public bool IsHitValid(GameHitData hit)
	{
		return !this.brokenLocal && hit.hitTypeId == 0;
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x000E6D38 File Offset: 0x000E4F38
	public void OnHit(GameHitData hit)
	{
		if (hit.hitTypeId == 0 && (int)this.gameEntity.GetState() != 1)
		{
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
			GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(hit.hitByEntityId);
			if (gameEntity != null && gameEntity.IsHeldByLocalPlayer())
			{
				PlayerGameEvents.MiscEvent("GRSmashBreakable", 1);
			}
		}
	}

	// Token: 0x04003754 RID: 14164
	public GameEntity gameEntity;

	// Token: 0x04003755 RID: 14165
	public List<Transform> enableWhenBroken;

	// Token: 0x04003756 RID: 14166
	public List<Transform> disableWhenBroken;

	// Token: 0x04003757 RID: 14167
	public Collider breakableCollider;

	// Token: 0x04003758 RID: 14168
	public bool holdsRandomItem = true;

	// Token: 0x04003759 RID: 14169
	public Transform itemSpawnLocation;

	// Token: 0x0400375A RID: 14170
	public GRBreakableItemSpawnConfig itemSpawnProbability;

	// Token: 0x0400375B RID: 14171
	public AudioSource audioSource;

	// Token: 0x0400375C RID: 14172
	public AudioClip breakSound;

	// Token: 0x0400375D RID: 14173
	public float breakSoundVolume;

	// Token: 0x0400375E RID: 14174
	private bool brokenLocal;

	// Token: 0x0200068F RID: 1679
	public enum BreakableState
	{
		// Token: 0x04003760 RID: 14176
		Unbroken,
		// Token: 0x04003761 RID: 14177
		Broken
	}
}
