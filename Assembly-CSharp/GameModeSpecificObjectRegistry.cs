using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class GameModeSpecificObjectRegistry : MonoBehaviour
{
	// Token: 0x060003C2 RID: 962 RVA: 0x00016EA2 File Offset: 0x000150A2
	private void OnEnable()
	{
		GameModeSpecificObject.OnAwake += this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed += this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode += this.GameMode_OnStartGameMode;
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00016ED7 File Offset: 0x000150D7
	private void OnDisable()
	{
		GameModeSpecificObject.OnAwake -= this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed -= this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode -= this.GameMode_OnStartGameMode;
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x00016F0C File Offset: 0x0001510C
	private void GameModeSpecificObject_OnAwake(GameModeSpecificObject obj)
	{
		foreach (GameModeType gameModeType in obj.GameModes)
		{
			if (!this.gameModeSpecificObjects.ContainsKey(gameModeType))
			{
				this.gameModeSpecificObjects.Add(gameModeType, new List<GameModeSpecificObject>());
			}
			this.gameModeSpecificObjects[gameModeType].Add(obj);
		}
		if (GameMode.ActiveGameMode == null)
		{
			obj.gameObject.SetActive(obj.Validation == GameModeSpecificObject.ValidationMethod.Exclusion);
			return;
		}
		obj.gameObject.SetActive(obj.CheckValid(GameMode.ActiveGameMode.GameType()));
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x00016FC8 File Offset: 0x000151C8
	private void GameModeSpecificObject_OnDestroyed(GameModeSpecificObject obj)
	{
		foreach (GameModeType gameModeType in obj.GameModes)
		{
			if (this.gameModeSpecificObjects.ContainsKey(gameModeType))
			{
				this.gameModeSpecificObjects[gameModeType].Remove(obj);
			}
		}
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x00017038 File Offset: 0x00015238
	private void GameMode_OnStartGameMode(GameModeType newGameModeType)
	{
		if (this.currentGameType == newGameModeType)
		{
			return;
		}
		if (this.gameModeSpecificObjects.ContainsKey(this.currentGameType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject in this.gameModeSpecificObjects[this.currentGameType])
			{
				gameModeSpecificObject.gameObject.SetActive(gameModeSpecificObject.CheckValid(newGameModeType));
			}
		}
		if (this.gameModeSpecificObjects.ContainsKey(newGameModeType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject2 in this.gameModeSpecificObjects[newGameModeType])
			{
				gameModeSpecificObject2.gameObject.SetActive(gameModeSpecificObject2.CheckValid(newGameModeType));
			}
		}
		this.currentGameType = newGameModeType;
	}

	// Token: 0x04000438 RID: 1080
	private Dictionary<GameModeType, List<GameModeSpecificObject>> gameModeSpecificObjects = new Dictionary<GameModeType, List<GameModeSpecificObject>>();

	// Token: 0x04000439 RID: 1081
	private GameModeType currentGameType = GameModeType.Count;
}
