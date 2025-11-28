using System;
using System.Collections.Generic;
using Photon.Pun;

// Token: 0x02000047 RID: 71
public class CrittersActorSpawnerPoint : CrittersActor
{
	// Token: 0x14000002 RID: 2
	// (add) Token: 0x0600015C RID: 348 RVA: 0x0000930C File Offset: 0x0000750C
	// (remove) Token: 0x0600015D RID: 349 RVA: 0x00009344 File Offset: 0x00007544
	public event Action<CrittersActor> OnSpawnChanged;

	// Token: 0x0600015E RID: 350 RVA: 0x00009379 File Offset: 0x00007579
	public override void Initialize()
	{
		base.Initialize();
		base.UpdateImpulses(false, false);
	}

	// Token: 0x0600015F RID: 351 RVA: 0x00009389 File Offset: 0x00007589
	public override void OnDisable()
	{
		base.OnDisable();
		this.spawnedActorID = -1;
		this.spawnedActor = null;
	}

	// Token: 0x06000160 RID: 352 RVA: 0x000093A0 File Offset: 0x000075A0
	public void SetSpawnedActor(CrittersActor actor)
	{
		if (this.spawnedActor == actor)
		{
			return;
		}
		this.spawnedActor = actor;
		if (this.spawnedActor != null)
		{
			this.spawnedActorID = this.spawnedActor.actorId;
		}
		else
		{
			this.spawnedActorID = -1;
		}
		Action<CrittersActor> onSpawnChanged = this.OnSpawnChanged;
		if (onSpawnChanged != null)
		{
			onSpawnChanged.Invoke(this.spawnedActor);
		}
		this.updatedSinceLastFrame = true;
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000940C File Offset: 0x0000760C
	private void UpdateSpawnedActor(int newSpawnedActorID)
	{
		if (this.spawnedActorID == newSpawnedActorID)
		{
			return;
		}
		if (newSpawnedActorID == -1)
		{
			this.spawnedActorID = newSpawnedActorID;
			this.spawnedActor = null;
		}
		else
		{
			CrittersActor crittersActor;
			if (!CrittersManager.instance.actorById.TryGetValue(newSpawnedActorID, ref crittersActor))
			{
				return;
			}
			this.spawnedActorID = newSpawnedActorID;
			this.spawnedActor = crittersActor;
		}
		Action<CrittersActor> onSpawnChanged = this.OnSpawnChanged;
		if (onSpawnChanged == null)
		{
			return;
		}
		onSpawnChanged.Invoke(this.spawnedActor);
	}

	// Token: 0x06000162 RID: 354 RVA: 0x00009472 File Offset: 0x00007672
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.spawnedActorID);
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0000948C File Offset: 0x0000768C
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		if (!base.UpdateSpecificActor(stream))
		{
			return false;
		}
		int num;
		if (!CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num))
		{
			return false;
		}
		if (num < -1 || num >= CrittersManager.instance.universalActorId)
		{
			return false;
		}
		this.UpdateSpawnedActor(num);
		return true;
	}

	// Token: 0x06000164 RID: 356 RVA: 0x000094D2 File Offset: 0x000076D2
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.spawnedActorID);
		return this.TotalActorDataLength();
	}

	// Token: 0x06000165 RID: 357 RVA: 0x000094F4 File Offset: 0x000076F4
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x06000166 RID: 358 RVA: 0x00009500 File Offset: 0x00007700
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		if (num >= -1 && num < CrittersManager.instance.universalActorId)
		{
			return this.TotalActorDataLength();
		}
		this.UpdateSpawnedActor(num);
		return this.TotalActorDataLength();
	}

	// Token: 0x04000184 RID: 388
	private CrittersActor spawnedActor;

	// Token: 0x04000185 RID: 389
	private int spawnedActorID = -1;
}
