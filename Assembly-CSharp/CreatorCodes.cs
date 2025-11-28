using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000461 RID: 1121
public static class CreatorCodes
{
	// Token: 0x06001C65 RID: 7269 RVA: 0x00096B9C File Offset: 0x00094D9C
	public static string getCurrentCreatorCode(string id)
	{
		if (id.IsNullOrEmpty())
		{
			return string.Empty;
		}
		if (CreatorCodes.data.currentCreatorCode == null)
		{
			return string.Empty;
		}
		if (!CreatorCodes.data.currentCreatorCode.ContainsKey(id))
		{
			return string.Empty;
		}
		return CreatorCodes.data.currentCreatorCode[id];
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x00096BF1 File Offset: 0x00094DF1
	public static CreatorCodes.CreatorCodeStatus getCurrentCreatorCodeStatus(string id)
	{
		if (id == null)
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		if (CreatorCodes.creatorCodeStatus == null)
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		if (!CreatorCodes.creatorCodeStatus.ContainsKey(id))
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		return CreatorCodes.creatorCodeStatus[id];
	}

	// Token: 0x14000040 RID: 64
	// (add) Token: 0x06001C67 RID: 7271 RVA: 0x00096C1C File Offset: 0x00094E1C
	// (remove) Token: 0x06001C68 RID: 7272 RVA: 0x00096C50 File Offset: 0x00094E50
	public static event Action<string> OnCreatorCodeChangedEvent;

	// Token: 0x14000041 RID: 65
	// (add) Token: 0x06001C69 RID: 7273 RVA: 0x00096C84 File Offset: 0x00094E84
	// (remove) Token: 0x06001C6A RID: 7274 RVA: 0x00096CB8 File Offset: 0x00094EB8
	public static event Action InitializedEvent;

	// Token: 0x14000042 RID: 66
	// (add) Token: 0x06001C6B RID: 7275 RVA: 0x00096CEC File Offset: 0x00094EEC
	// (remove) Token: 0x06001C6C RID: 7276 RVA: 0x00096D20 File Offset: 0x00094F20
	public static event Action<string, string, NexusGroupId> OnCreatorCodeValidEvent;

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x06001C6D RID: 7277 RVA: 0x00096D54 File Offset: 0x00094F54
	// (remove) Token: 0x06001C6E RID: 7278 RVA: 0x00096D88 File Offset: 0x00094F88
	public static event Action<string> OnCreatorCodeFailureEvent;

	// Token: 0x06001C6F RID: 7279 RVA: 0x00096DBB File Offset: 0x00094FBB
	public static void Initialize()
	{
		CreatorCodes.ValidatedCreatorCode = new Dictionary<string, NexusManager.MemberCode>();
		CreatorCodes.creatorCodeStatus = new Dictionary<string, CreatorCodes.CreatorCodeStatus>();
		CreatorCodes.LoadData();
		CreatorCodes.Intialized = true;
		Action initializedEvent = CreatorCodes.InitializedEvent;
		if (initializedEvent == null)
		{
			return;
		}
		initializedEvent.Invoke();
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x00096DEC File Offset: 0x00094FEC
	public static void DeleteCharacter(string id)
	{
		if (CreatorCodes.data.currentCreatorCode.ContainsKey(id) && CreatorCodes.data.currentCreatorCode[id].Length > 0)
		{
			CreatorCodes.data.currentCreatorCode[id] = CreatorCodes.data.currentCreatorCode[id].Substring(0, CreatorCodes.data.currentCreatorCode[id].Length - 1);
			CreatorCodes.ValidatedCreatorCode[id] = null;
			CreatorCodes.creatorCodeStatus[id] = ((CreatorCodes.data.currentCreatorCode[id].Length == 0) ? CreatorCodes.CreatorCodeStatus.Empty : CreatorCodes.CreatorCodeStatus.Unchecked);
			Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
			if (onCreatorCodeChangedEvent == null)
			{
				return;
			}
			onCreatorCodeChangedEvent.Invoke(id);
		}
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x00096EA8 File Offset: 0x000950A8
	public static void AppendKey(string id, string input)
	{
		if (!CreatorCodes.data.currentCreatorCode.ContainsKey(id))
		{
			CreatorCodes.data.currentCreatorCode[id] = string.Empty;
		}
		if (CreatorCodes.data.currentCreatorCode[id].Length < 10)
		{
			Dictionary<string, string> currentCreatorCode = CreatorCodes.data.currentCreatorCode;
			currentCreatorCode[id] += input;
			CreatorCodes.ValidatedCreatorCode[id] = null;
			CreatorCodes.creatorCodeStatus[id] = CreatorCodes.CreatorCodeStatus.Unchecked;
			Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
			if (onCreatorCodeChangedEvent == null)
			{
				return;
			}
			onCreatorCodeChangedEvent.Invoke(id);
		}
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x00096F40 File Offset: 0x00095140
	public static void ResetCreatorCode(string id)
	{
		Debug.Log("Resetting creator code");
		CreatorCodes.data.currentCreatorCode[id] = "";
		CreatorCodes.creatorCodeStatus[id] = CreatorCodes.CreatorCodeStatus.Empty;
		CreatorCodes.supportedMember = default(Member);
		CreatorCodes.ValidatedCreatorCode[id] = null;
		CreatorCodes.SaveData();
		Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
		if (onCreatorCodeChangedEvent == null)
		{
			return;
		}
		onCreatorCodeChangedEvent.Invoke(id);
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x00096FA4 File Offset: 0x000951A4
	public static Task<NexusManager.MemberCode> CheckValidationCoroutineJIT(string terminalId, string code, NexusGroupId[] group)
	{
		CreatorCodes.<CheckValidationCoroutineJIT>d__27 <CheckValidationCoroutineJIT>d__;
		<CheckValidationCoroutineJIT>d__.<>t__builder = AsyncTaskMethodBuilder<NexusManager.MemberCode>.Create();
		<CheckValidationCoroutineJIT>d__.terminalId = terminalId;
		<CheckValidationCoroutineJIT>d__.code = code;
		<CheckValidationCoroutineJIT>d__.group = group;
		<CheckValidationCoroutineJIT>d__.<>1__state = -1;
		<CheckValidationCoroutineJIT>d__.<>t__builder.Start<CreatorCodes.<CheckValidationCoroutineJIT>d__27>(ref <CheckValidationCoroutineJIT>d__);
		return <CheckValidationCoroutineJIT>d__.<>t__builder.Task;
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x00096FF7 File Offset: 0x000951F7
	private static void SaveData()
	{
		PlayerPrefs.SetString("CreatorCodes_Store", JsonConvert.SerializeObject(CreatorCodes.data));
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x00097010 File Offset: 0x00095210
	private static void LoadData()
	{
		string @string = PlayerPrefs.GetString("CreatorCodes_Store", string.Empty);
		if (@string.Length == 0)
		{
			return;
		}
		CreatorCodes.data = JsonConvert.DeserializeObject<CreatorCodes.CreatorCodesData>(@string);
		foreach (string text in CreatorCodes.data.currentCreatorCode.Keys)
		{
			if (CreatorCodes.data.codeFirstUsedTime.ContainsKey(text) && DateTime.UtcNow.Subtract(CreatorCodes.data.codeFirstUsedTime[text]).Days > 14)
			{
				CreatorCodes.data.currentCreatorCode[text] = string.Empty;
			}
		}
	}

	// Token: 0x04002676 RID: 9846
	private const int MAX_CODE_LENGTH = 10;

	// Token: 0x04002677 RID: 9847
	private const string PLAYER_PREF_KEY = "CreatorCodes_Store";

	// Token: 0x04002678 RID: 9848
	private const int DAYS_TO_STORE_CODE = 14;

	// Token: 0x04002679 RID: 9849
	private static CreatorCodes.CreatorCodesData data = new CreatorCodes.CreatorCodesData();

	// Token: 0x0400267A RID: 9850
	private static Dictionary<string, NexusManager.MemberCode> ValidatedCreatorCode;

	// Token: 0x0400267B RID: 9851
	private static Dictionary<string, CreatorCodes.CreatorCodeStatus> creatorCodeStatus;

	// Token: 0x04002680 RID: 9856
	public static bool Intialized = false;

	// Token: 0x04002681 RID: 9857
	public static Member supportedMember;

	// Token: 0x02000462 RID: 1122
	public enum CreatorCodeStatus
	{
		// Token: 0x04002683 RID: 9859
		Empty,
		// Token: 0x04002684 RID: 9860
		Unchecked,
		// Token: 0x04002685 RID: 9861
		Validating,
		// Token: 0x04002686 RID: 9862
		Valid
	}

	// Token: 0x02000463 RID: 1123
	[Serializable]
	private class CreatorCodesData
	{
		// Token: 0x04002687 RID: 9863
		public Dictionary<string, string> currentCreatorCode = new Dictionary<string, string>();

		// Token: 0x04002688 RID: 9864
		public Dictionary<string, DateTime> codeFirstUsedTime = new Dictionary<string, DateTime>();
	}
}
