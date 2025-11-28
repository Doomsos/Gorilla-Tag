using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaGameModes;
using TMPro;
using UnityEngine;

// Token: 0x02000B60 RID: 2912
public class LuauHud : MonoBehaviour
{
	// Token: 0x17000692 RID: 1682
	// (get) Token: 0x060047B6 RID: 18358 RVA: 0x00178F84 File Offset: 0x00177184
	public static LuauHud Instance
	{
		get
		{
			return LuauHud._instance;
		}
	}

	// Token: 0x060047B7 RID: 18359 RVA: 0x00178F8C File Offset: 0x0017718C
	private void Awake()
	{
		if (LuauHud._instance != null && LuauHud._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			LuauHud._instance = this;
		}
		this.path = Path.Combine(Application.persistentDataPath, "script.luau");
	}

	// Token: 0x060047B8 RID: 18360 RVA: 0x00178FDB File Offset: 0x001771DB
	private void OnDestroy()
	{
		if (LuauHud._instance == this)
		{
			LuauHud._instance = null;
		}
	}

	// Token: 0x060047B9 RID: 18361 RVA: 0x00178FF0 File Offset: 0x001771F0
	private void Start()
	{
		this.useLuauHud = true;
		DebugHudStats instance = DebugHudStats.Instance;
		instance.enabled = false;
		this.debugHud = instance.gameObject;
		this.text = instance.text;
		this.text.gameObject.SetActive(false);
		this.builder = new StringBuilder(50);
	}

	// Token: 0x060047BA RID: 18362 RVA: 0x00179048 File Offset: 0x00177248
	private void Update()
	{
		if (!CustomMapLoader.IsDevModeEnabled())
		{
			if (this.showLog && this.useLuauHud)
			{
				this.showLog = false;
				DebugHudStats instance = DebugHudStats.Instance;
				if (instance != null)
				{
					instance.gameObject.SetActive(false);
				}
				this.text.gameObject.SetActive(false);
			}
			return;
		}
		GorillaGameManager instance2 = GorillaGameManager.instance;
		if (instance2 == null || instance2.GameType() != GameModeType.Custom)
		{
			return;
		}
		bool flag = ControllerInputPoller.SecondaryButtonPress(4);
		bool flag2 = ControllerInputPoller.SecondaryButtonPress(5);
		if (flag != this.buttonDown && this.useLuauHud)
		{
			this.buttonDown = flag;
			if (!this.buttonDown)
			{
				if (!this.text.gameObject.activeInHierarchy)
				{
					DebugHudStats instance3 = DebugHudStats.Instance;
					if (instance3 != null)
					{
						instance3.gameObject.SetActive(true);
					}
					this.text.gameObject.SetActive(true);
					this.showLog = true;
				}
				else
				{
					DebugHudStats instance4 = DebugHudStats.Instance;
					if (instance4 != null)
					{
						instance4.gameObject.SetActive(false);
					}
					this.text.gameObject.SetActive(false);
					this.showLog = false;
				}
			}
		}
		if (!flag || !flag2)
		{
			this.resetTimer = Time.time;
		}
		if (Time.time - this.resetTimer > 2f && CustomGameMode.GameModeInitialized)
		{
			this.RestartLuauScript();
			this.resetTimer = Time.time;
		}
		if (this.useLuauHud && this.showLog)
		{
			this.builder.Clear();
			this.builder.AppendLine();
			for (int i = 0; i < this.luauLogs.Count; i++)
			{
				this.builder.AppendLine(this.luauLogs[i]);
			}
			this.text.text = this.builder.ToString();
		}
	}

	// Token: 0x060047BB RID: 18363 RVA: 0x00179200 File Offset: 0x00177400
	public void RestartLuauScript()
	{
		this.LuauLog("Restarting Luau Script");
		LuauScriptRunner gameScriptRunner = CustomGameMode.gameScriptRunner;
		if (gameScriptRunner != null && gameScriptRunner.ShouldTick)
		{
			CustomGameMode.StopScript();
		}
		this.script = this.LoadLocalScript();
		if (this.script != "")
		{
			this.LuauLog("Loaded script from: " + this.path);
			this.LuauLog("Loaded Script Text: \n" + this.script);
			CustomGameMode.LuaScript = this.script;
		}
		CustomGameMode.LuaStart();
	}

	// Token: 0x060047BC RID: 18364 RVA: 0x0017928C File Offset: 0x0017748C
	public string LoadLocalScript()
	{
		string result = "";
		if (File.Exists(this.path))
		{
			result = File.ReadAllText(this.path);
		}
		return result;
	}

	// Token: 0x060047BD RID: 18365 RVA: 0x001792B9 File Offset: 0x001774B9
	public void LuauLog(string log)
	{
		Debug.Log(log);
		this.luauLogs.Add(log);
		if (this.luauLogs.Count > 6)
		{
			this.luauLogs.RemoveAt(0);
		}
	}

	// Token: 0x0400586A RID: 22634
	private bool useLuauHud;

	// Token: 0x0400586B RID: 22635
	private bool buttonDown;

	// Token: 0x0400586C RID: 22636
	private bool showLog;

	// Token: 0x0400586D RID: 22637
	private GameObject debugHud;

	// Token: 0x0400586E RID: 22638
	private TMP_Text text;

	// Token: 0x0400586F RID: 22639
	private StringBuilder builder;

	// Token: 0x04005870 RID: 22640
	private float resetTimer;

	// Token: 0x04005871 RID: 22641
	private string path = "";

	// Token: 0x04005872 RID: 22642
	private string script = "";

	// Token: 0x04005873 RID: 22643
	private static LuauHud _instance;

	// Token: 0x04005874 RID: 22644
	private List<string> luauLogs = new List<string>();
}
