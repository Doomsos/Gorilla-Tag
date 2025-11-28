using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000518 RID: 1304
public class GameModePages : BasePageHandler
{
	// Token: 0x1700038A RID: 906
	// (get) Token: 0x06002137 RID: 8503 RVA: 0x000AEF59 File Offset: 0x000AD159
	protected override int pageSize
	{
		get
		{
			return this.buttons.Length;
		}
	}

	// Token: 0x1700038B RID: 907
	// (get) Token: 0x06002138 RID: 8504 RVA: 0x000AEF63 File Offset: 0x000AD163
	protected override int entriesCount
	{
		get
		{
			return GameMode.gameModeNames.Count;
		}
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x000AEF70 File Offset: 0x000AD170
	private void Awake()
	{
		GameModePages.gameModeSelectorInstances.Add(this);
		this.buttons = base.GetComponentsInChildren<GameModeSelectButton>();
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].buttonIndex = i;
			this.buttons[i].selector = this;
		}
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000AEFC3 File Offset: 0x000AD1C3
	protected override void Start()
	{
		base.Start();
		base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		this.initialized = true;
	}

	// Token: 0x0600213B RID: 8507 RVA: 0x000AEFDD File Offset: 0x000AD1DD
	private void OnEnable()
	{
		if (this.initialized)
		{
			base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x000AEFF2 File Offset: 0x000AD1F2
	private void OnDestroy()
	{
		GameModePages.gameModeSelectorInstances.Remove(this);
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x000AF000 File Offset: 0x000AD200
	protected override void ShowPage(int selectedPage, int startIndex, int endIndex)
	{
		GameModePages.textBuilder.Clear();
		for (int i = startIndex; i < endIndex; i++)
		{
			GameModePages.textBuilder.AppendLine(GameMode.gameModeNames[i]);
		}
		this.gameModeText.text = GameModePages.textBuilder.ToString();
		if (base.selectedIndex >= startIndex && base.selectedIndex <= endIndex)
		{
			this.UpdateAllButtons(this.currentButtonIndex);
		}
		else
		{
			this.UpdateAllButtons(-1);
		}
		int buttonsMissing = (selectedPage == base.pages - 1 && base.maxEntires > endIndex) ? (base.maxEntires - endIndex) : 0;
		this.EnableEntryButtons(buttonsMissing);
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x000AF09D File Offset: 0x000AD29D
	protected override void PageEntrySelected(int pageEntry, int selectionIndex)
	{
		if (selectionIndex >= this.entriesCount)
		{
			return;
		}
		GameModePages.sharedSelectedIndex = selectionIndex;
		this.UpdateAllButtons(pageEntry);
		this.currentButtonIndex = pageEntry;
		GorillaComputer.instance.OnModeSelectButtonPress(GameMode.gameModeNames[selectionIndex], false);
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x000AF0D8 File Offset: 0x000AD2D8
	private void UpdateAllButtons(int onButton)
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			if (i == onButton)
			{
				this.buttons[onButton].isOn = true;
				this.buttons[onButton].UpdateColor();
			}
			else if (this.buttons[i].isOn)
			{
				this.buttons[i].isOn = false;
				this.buttons[i].UpdateColor();
			}
		}
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x000AF144 File Offset: 0x000AD344
	private void EnableEntryButtons(int buttonsMissing)
	{
		int num = this.buttons.Length - buttonsMissing;
		int i;
		for (i = 0; i < num; i++)
		{
			this.buttons[i].gameObject.SetActive(true);
		}
		while (i < this.buttons.Length)
		{
			this.buttons[i].gameObject.SetActive(false);
			i++;
		}
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x000AF1A0 File Offset: 0x000AD3A0
	public static void SetSelectedGameModeShared(string gameMode)
	{
		GameModePages.sharedSelectedIndex = GameMode.gameModeNames.IndexOf(gameMode);
		if (GameModePages.sharedSelectedIndex < 0)
		{
			return;
		}
		for (int i = 0; i < GameModePages.gameModeSelectorInstances.Count; i++)
		{
			GameModePages.gameModeSelectorInstances[i].SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x04002BC5 RID: 11205
	private int currentButtonIndex;

	// Token: 0x04002BC6 RID: 11206
	[SerializeField]
	private Text gameModeText;

	// Token: 0x04002BC7 RID: 11207
	[SerializeField]
	private GameModeSelectButton[] buttons;

	// Token: 0x04002BC8 RID: 11208
	private bool initialized;

	// Token: 0x04002BC9 RID: 11209
	private static int sharedSelectedIndex = 0;

	// Token: 0x04002BCA RID: 11210
	private static StringBuilder textBuilder = new StringBuilder(50);

	// Token: 0x04002BCB RID: 11211
	[OnEnterPlay_Clear]
	private static List<GameModePages> gameModeSelectorInstances = new List<GameModePages>(7);
}
