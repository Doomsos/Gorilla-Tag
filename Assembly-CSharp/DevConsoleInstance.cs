using System;
using System.Collections.Generic;
using UnityEngine;

public class DevConsoleInstance : MonoBehaviour
{
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	public DevConsoleInstance()
	{
		HashSet<LogType> hashSet = new HashSet<LogType>();
		hashSet.Add(0);
		hashSet.Add(4);
		hashSet.Add(3);
		hashSet.Add(2);
		hashSet.Add(1);
		this.selectedLogTypes = hashSet;
		this.textScale = 0.5;
		this.isEnabled = true;
		base..ctor();
	}

	public GorillaDevButton[] buttons;

	public GameObject[] disableWhileActive;

	public GameObject[] enableWhileActive;

	public float maxHeight;

	public float lineHeight;

	public int targetLogIndex = -1;

	public int currentLogIndex;

	public int expandAmount = 20;

	public int expandedMessageIndex = -1;

	public bool canExpand = true;

	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	public HashSet<LogType> selectedLogTypes;

	[SerializeField]
	private GorillaDevButton[] logTypeButtons;

	[SerializeField]
	private GorillaDevButton BottomButton;

	public float lineStartHeight;

	public float lineStartZ;

	public float textStartHeight;

	public float lineStartTextWidth;

	public double textScale;

	public bool isEnabled;

	[SerializeField]
	private GameObject ConsoleLineExample;
}
