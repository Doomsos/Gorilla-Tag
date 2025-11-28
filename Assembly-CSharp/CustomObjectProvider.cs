using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using UnityEngine;

// Token: 0x0200039C RID: 924
public class CustomObjectProvider : NetworkObjectProviderDefault
{
	// Token: 0x17000225 RID: 549
	// (get) Token: 0x0600160B RID: 5643 RVA: 0x0007AC78 File Offset: 0x00078E78
	private static NetworkObjectBaker Baker
	{
		get
		{
			NetworkObjectBaker result;
			if ((result = CustomObjectProvider.baker) == null)
			{
				result = (CustomObjectProvider.baker = new NetworkObjectBaker());
			}
			return result;
		}
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x0007AC8E File Offset: 0x00078E8E
	public override NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
	{
		NetworkObjectAcquireResult networkObjectAcquireResult = base.AcquirePrefabInstance(runner, ref context, ref instance);
		if (networkObjectAcquireResult == null)
		{
			this.IsGameMode(instance);
			return networkObjectAcquireResult;
		}
		instance = null;
		return networkObjectAcquireResult;
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x0007ACA8 File Offset: 0x00078EA8
	private void IsGameMode(NetworkObject instance)
	{
		if (instance.gameObject.GetComponent<GameModeSerializer>() != null)
		{
			GameMode.GetGameModeInstance(GameMode.GetGameModeKeyFromRoomProp()).AddFusionDataBehaviour(instance);
			CustomObjectProvider.Baker.Bake(instance.gameObject);
		}
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x0007ACDE File Offset: 0x00078EDE
	protected override void DestroySceneObject(NetworkRunner runner, NetworkSceneObjectId sceneObjectId, NetworkObject instance)
	{
		if (this.SceneObjects != null && this.SceneObjects.Contains(instance.gameObject))
		{
			return;
		}
		base.DestroySceneObject(runner, sceneObjectId, instance);
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x0007AD05 File Offset: 0x00078F05
	protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
	{
		base.DestroyPrefabInstance(runner, prefabId, instance);
	}

	// Token: 0x04002055 RID: 8277
	public const int GameModeFlag = 1;

	// Token: 0x04002056 RID: 8278
	public const int PlayerFlag = 2;

	// Token: 0x04002057 RID: 8279
	private static NetworkObjectBaker baker;

	// Token: 0x04002058 RID: 8280
	internal List<GameObject> SceneObjects;
}
