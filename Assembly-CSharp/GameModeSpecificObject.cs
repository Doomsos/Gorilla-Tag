using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class GameModeSpecificObject : MonoBehaviour
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x060003B2 RID: 946 RVA: 0x00016C64 File Offset: 0x00014E64
	// (remove) Token: 0x060003B3 RID: 947 RVA: 0x00016C98 File Offset: 0x00014E98
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnAwake;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x060003B4 RID: 948 RVA: 0x00016CCC File Offset: 0x00014ECC
	// (remove) Token: 0x060003B5 RID: 949 RVA: 0x00016D00 File Offset: 0x00014F00
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnDestroyed;

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060003B6 RID: 950 RVA: 0x00016D33 File Offset: 0x00014F33
	public GameModeSpecificObject.ValidationMethod Validation
	{
		get
		{
			return this.validationMethod;
		}
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060003B7 RID: 951 RVA: 0x00016D3B File Offset: 0x00014F3B
	public List<GameModeType> GameModes
	{
		get
		{
			return this.gameModes;
		}
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x00016D44 File Offset: 0x00014F44
	private void Awake()
	{
		GameModeSpecificObject.<Awake>d__15 <Awake>d__;
		<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Awake>d__.<>4__this = this;
		<Awake>d__.<>1__state = -1;
		<Awake>d__.<>t__builder.Start<GameModeSpecificObject.<Awake>d__15>(ref <Awake>d__);
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00016D7B File Offset: 0x00014F7B
	private void OnDestroy()
	{
		if (GameModeSpecificObject.OnDestroyed != null)
		{
			GameModeSpecificObject.OnDestroyed(this);
		}
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00016D8F File Offset: 0x00014F8F
	public bool CheckValid(GameModeType gameMode)
	{
		if (this.validationMethod == GameModeSpecificObject.ValidationMethod.Exclusion)
		{
			return !this.gameModes.Contains(gameMode);
		}
		return this.gameModes.Contains(gameMode);
	}

	// Token: 0x0400042E RID: 1070
	[SerializeField]
	private GameModeSpecificObject.ValidationMethod validationMethod;

	// Token: 0x0400042F RID: 1071
	[SerializeField]
	private GameModeType[] _gameModes;

	// Token: 0x04000430 RID: 1072
	private List<GameModeType> gameModes;

	// Token: 0x02000093 RID: 147
	// (Invoke) Token: 0x060003BD RID: 957
	public delegate void GameModeSpecificObjectDelegate(GameModeSpecificObject gameModeSpecificObject);

	// Token: 0x02000094 RID: 148
	[Serializable]
	public enum ValidationMethod
	{
		// Token: 0x04000432 RID: 1074
		Inclusion,
		// Token: 0x04000433 RID: 1075
		Exclusion
	}
}
