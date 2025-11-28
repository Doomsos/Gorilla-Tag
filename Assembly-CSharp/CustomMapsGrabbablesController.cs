using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000961 RID: 2401
public class CustomMapsGrabbablesController : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06003D91 RID: 15761 RVA: 0x0014644C File Offset: 0x0014464C
	private void Awake()
	{
		this.isGrabbed = false;
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x06003D92 RID: 15762 RVA: 0x001464B0 File Offset: 0x001446B0
	private void OnDestroy()
	{
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x06003D93 RID: 15763 RVA: 0x0014650C File Offset: 0x0014470C
	public void OnEntityInit()
	{
		GTDev.Log<string>("CustomMapsGrabbablesController::OnEntityInit", null);
		if (MapSpawnManager.instance == null)
		{
			return;
		}
		base.transform.parent = MapSpawnManager.instance.transform;
		byte b;
		GrabbableEntity.UnpackCreateData(this.entity.createData, ref b, ref this.luaAgentID);
		MapEntity mapEntity;
		if (!MapSpawnManager.instance.SpawnEntity((int)b, ref mapEntity))
		{
			GTDev.LogError<string>("CustomMapsGrabbablesController::OnEntityInit could not spawn grabbable", null);
			Object.Destroy(base.gameObject);
			return;
		}
		GrabbableEntity grabbableEntity = (GrabbableEntity)mapEntity;
		if (grabbableEntity == null)
		{
			return;
		}
		grabbableEntity.gameObject.SetActive(true);
		grabbableEntity.transform.parent = this.entity.transform;
		grabbableEntity.transform.localPosition = Vector3.zero;
		grabbableEntity.transform.localRotation = Quaternion.identity;
		this.returnParent = this.entity.transform.parent;
		this.entity.audioSource = grabbableEntity.audioSource;
		this.entity.catchSound = grabbableEntity.catchSound;
		this.entity.catchSoundVolume = grabbableEntity.catchSoundVolume;
		this.entity.throwSound = grabbableEntity.throwSound;
		this.entity.throwSoundVolume = grabbableEntity.throwSoundVolume;
		Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer("Prop");
		}
	}

	// Token: 0x06003D94 RID: 15764 RVA: 0x00146679 File Offset: 0x00144879
	public int GetGrabbingActor()
	{
		if (!this.isGrabbed)
		{
			return -1;
		}
		return this.entity.heldByActorNumber;
	}

	// Token: 0x06003D95 RID: 15765 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003D96 RID: 15766 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x06003D97 RID: 15767 RVA: 0x00146690 File Offset: 0x00144890
	private void OnGrabbed()
	{
		this.isGrabbed = true;
	}

	// Token: 0x06003D98 RID: 15768 RVA: 0x00146699 File Offset: 0x00144899
	private void OnReleased()
	{
		this.isGrabbed = false;
		if (this.returnParent.IsNotNull())
		{
			this.entity.transform.parent = this.returnParent;
		}
	}

	// Token: 0x04004E2D RID: 20013
	public GameEntity entity;

	// Token: 0x04004E2E RID: 20014
	public short luaAgentID;

	// Token: 0x04004E2F RID: 20015
	private bool isGrabbed;

	// Token: 0x04004E30 RID: 20016
	private Transform returnParent;
}
