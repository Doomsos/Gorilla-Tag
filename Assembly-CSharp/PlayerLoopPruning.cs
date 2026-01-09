using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public class PlayerLoopPruning : MonoBehaviour
{
	private void Start()
	{
		this.isAndroid = (Application.platform == RuntimePlatform.Android);
		PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
		PlayerLoop.SetPlayerLoop(this.RemoveSystem<PreLateUpdate>(currentPlayerLoop));
	}

	private PlayerLoopSystem RemoveSystem<T>(in PlayerLoopSystem loopSystem) where T : struct
	{
		PlayerLoopSystem result = new PlayerLoopSystem
		{
			loopConditionFunction = loopSystem.loopConditionFunction,
			type = loopSystem.type,
			updateDelegate = loopSystem.updateDelegate,
			updateFunction = loopSystem.updateFunction
		};
		List<PlayerLoopSystem> list = new List<PlayerLoopSystem>();
		if (loopSystem.subSystemList != null)
		{
			for (int i = 0; i < loopSystem.subSystemList.Length; i++)
			{
				PlayerLoopSystem playerLoopSystem = loopSystem.subSystemList[i];
				PlayerLoopSystem item = new PlayerLoopSystem
				{
					loopConditionFunction = playerLoopSystem.loopConditionFunction,
					type = playerLoopSystem.type,
					updateDelegate = playerLoopSystem.updateDelegate,
					updateFunction = playerLoopSystem.updateFunction
				};
				if (playerLoopSystem.subSystemList != null)
				{
					List<PlayerLoopSystem> list2 = new List<PlayerLoopSystem>();
					for (int j = 0; j < playerLoopSystem.subSystemList.Length; j++)
					{
						if (!this.removeSubsystemList.Contains(playerLoopSystem.subSystemList[j].type.Name) && (!this.isAndroid || !this.androidSubsystemExtras.Contains(playerLoopSystem.subSystemList[j].type.Name)))
						{
							list2.Add(playerLoopSystem.subSystemList[j]);
						}
					}
					item.subSystemList = list2.ToArray();
				}
				list.Add(item);
			}
		}
		result.subSystemList = list.ToArray();
		return result;
	}

	public List<string> removeSubsystemList;

	public List<string> androidSubsystemExtras;

	private bool isAndroid;
}
