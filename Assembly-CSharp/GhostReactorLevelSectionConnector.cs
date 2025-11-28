using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200064B RID: 1611
public class GhostReactorLevelSectionConnector : MonoBehaviour
{
	// Token: 0x060028F7 RID: 10487 RVA: 0x000DB808 File Offset: 0x000D9A08
	private void Awake()
	{
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int i = 0; i < this.prePlacedGameEntities.Count; i++)
		{
			this.prePlacedGameEntities[i].gameObject.SetActive(false);
		}
		this.renderers = new List<Renderer>(512);
		this.hidden = false;
		base.GetComponentsInChildren<Renderer>(this.renderers);
		if (this.boundingCollider == null)
		{
			Debug.LogWarningFormat("Missing Bounding Collider for section {0}", new object[]
			{
				base.gameObject.name
			});
		}
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x000DB8B0 File Offset: 0x000D9AB0
	public void Init(GhostReactorManager grManager)
	{
		if (grManager.IsAuthority())
		{
			if (this.gateEntity != null)
			{
				grManager.gameEntityManager.RequestCreateItem(this.gateEntity.name.GetStaticHash(), this.gateSpawnPoint.position, this.gateSpawnPoint.rotation, 0L);
			}
			for (int i = 0; i < this.prePlacedGameEntities.Count; i++)
			{
				int staticHash = this.prePlacedGameEntities[i].gameObject.name.GetStaticHash();
				if (!grManager.gameEntityManager.FactoryHasEntity(staticHash))
				{
					Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1}", new object[]
					{
						this.prePlacedGameEntities[i].gameObject.name,
						staticHash
					});
				}
				else
				{
					GameEntityCreateData gameEntityCreateData = new GameEntityCreateData
					{
						entityTypeId = staticHash,
						position = this.prePlacedGameEntities[i].transform.position,
						rotation = this.prePlacedGameEntities[i].transform.rotation,
						createData = 0L
					};
					GhostReactorLevelSection.tempCreateEntitiesList.Add(gameEntityCreateData);
				}
			}
			grManager.gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x000DBA00 File Offset: 0x000D9C00
	public void Hide(bool hide)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				this.renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x000DBA4C File Offset: 0x000D9C4C
	public void UpdateDisable(Vector3 playerPos)
	{
		if (this.boundingCollider == null)
		{
			return;
		}
		float sqrMagnitude = (this.boundingCollider.ClosestPoint(playerPos) - playerPos).sqrMagnitude;
		float num = 324f;
		float num2 = 484f;
		if (this.hidden && sqrMagnitude < num)
		{
			this.hidden = false;
			this.Hide(false);
			return;
		}
		if (!this.hidden && sqrMagnitude > num2)
		{
			this.hidden = true;
			this.Hide(true);
		}
	}

	// Token: 0x040034C2 RID: 13506
	public Transform hubAnchor;

	// Token: 0x040034C3 RID: 13507
	public Transform sectionAnchor;

	// Token: 0x040034C4 RID: 13508
	public Transform gateSpawnPoint;

	// Token: 0x040034C5 RID: 13509
	public GameEntity gateEntity;

	// Token: 0x040034C6 RID: 13510
	public GhostReactorLevelSectionConnector.Direction direction;

	// Token: 0x040034C7 RID: 13511
	public BoxCollider boundingCollider;

	// Token: 0x040034C8 RID: 13512
	public List<Transform> pathNodes;

	// Token: 0x040034C9 RID: 13513
	private const float SHOW_DIST = 18f;

	// Token: 0x040034CA RID: 13514
	private const float HIDE_DIST = 22f;

	// Token: 0x040034CB RID: 13515
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x040034CC RID: 13516
	private List<Renderer> renderers;

	// Token: 0x040034CD RID: 13517
	private bool hidden;

	// Token: 0x0200064C RID: 1612
	public enum Direction
	{
		// Token: 0x040034CF RID: 13519
		Down = -1,
		// Token: 0x040034D0 RID: 13520
		Forward,
		// Token: 0x040034D1 RID: 13521
		Up
	}
}
