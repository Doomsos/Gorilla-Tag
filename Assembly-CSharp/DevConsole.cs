using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200029C RID: 668
public class DevConsole : MonoBehaviour, IDebugObject
{
	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x060010FE RID: 4350 RVA: 0x0005B63A File Offset: 0x0005983A
	public static DevConsole instance
	{
		get
		{
			if (DevConsole._instance == null)
			{
				DevConsole._instance = Object.FindAnyObjectByType<DevConsole>();
			}
			return DevConsole._instance;
		}
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x060010FF RID: 4351 RVA: 0x0005B658 File Offset: 0x00059858
	public static List<DevConsole.LogEntry> logEntries
	{
		get
		{
			return DevConsole.instance._logEntries;
		}
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x0005B664 File Offset: 0x00059864
	public void OnDestroyDebugObject()
	{
		Debug.Log("Destroying debug instances now");
		foreach (DevConsoleInstance devConsoleInstance in this.instances)
		{
			Object.DestroyImmediate(devConsoleInstance.gameObject);
		}
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x000396A0 File Offset: 0x000378A0
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001546 RID: 5446
	private static DevConsole _instance;

	// Token: 0x04001547 RID: 5447
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x04001548 RID: 5448
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001549 RID: 5449
	[SerializeField]
	private float maxHeight;

	// Token: 0x0400154A RID: 5450
	public static readonly string[] tracebackScrubbing = new string[]
	{
		"ExitGames.Client.Photon",
		"Photon.Realtime.LoadBalancingClient",
		"Photon.Pun.PhotonHandler"
	};

	// Token: 0x0400154B RID: 5451
	private const int kLogEntriesCapacityIncrementAmount = 1024;

	// Token: 0x0400154C RID: 5452
	[SerializeReference]
	[SerializeField]
	private readonly List<DevConsole.LogEntry> _logEntries = new List<DevConsole.LogEntry>(1024);

	// Token: 0x0400154D RID: 5453
	public int targetLogIndex = -1;

	// Token: 0x0400154E RID: 5454
	public int currentLogIndex;

	// Token: 0x0400154F RID: 5455
	public bool isMuted;

	// Token: 0x04001550 RID: 5456
	public float currentZoomLevel = 1f;

	// Token: 0x04001551 RID: 5457
	public List<GameObject> disableWhileActive;

	// Token: 0x04001552 RID: 5458
	public List<GameObject> enableWhileActive;

	// Token: 0x04001553 RID: 5459
	public int expandAmount = 20;

	// Token: 0x04001554 RID: 5460
	public int expandedMessageIndex = -1;

	// Token: 0x04001555 RID: 5461
	public bool canExpand = true;

	// Token: 0x04001556 RID: 5462
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x04001557 RID: 5463
	public float lineStartHeight;

	// Token: 0x04001558 RID: 5464
	public float textStartHeight;

	// Token: 0x04001559 RID: 5465
	public float lineStartTextWidth;

	// Token: 0x0400155A RID: 5466
	public double textScale = 0.5;

	// Token: 0x0400155B RID: 5467
	public List<DevConsoleInstance> instances;

	// Token: 0x0200029D RID: 669
	[Serializable]
	public class LogEntry
	{
		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06001104 RID: 4356 RVA: 0x0005B74E File Offset: 0x0005994E
		public string Message
		{
			get
			{
				if (this.repeatCount > 1)
				{
					return string.Format("({0}) {1}", this.repeatCount, this._Message);
				}
				return this._Message;
			}
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0005B77C File Offset: 0x0005997C
		public LogEntry(string message, LogType type, string trace)
		{
			this._Message = message;
			this.Type = type;
			this.Trace = trace;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = trace.Split("\n".ToCharArray(), 0);
			for (int i = 0; i < array.Length; i++)
			{
				string line = array[i];
				if (!Enumerable.Any<string>(DevConsole.tracebackScrubbing, (string scrubString) => line.Contains(scrubString)))
				{
					stringBuilder.AppendLine(line);
				}
			}
			this.Trace = stringBuilder.ToString();
			DevConsole.LogEntry.TotalIndex++;
			this.index = DevConsole.LogEntry.TotalIndex;
		}

		// Token: 0x0400155C RID: 5468
		private static int TotalIndex;

		// Token: 0x0400155D RID: 5469
		[SerializeReference]
		[SerializeField]
		public readonly string _Message;

		// Token: 0x0400155E RID: 5470
		[SerializeField]
		[SerializeReference]
		public readonly LogType Type;

		// Token: 0x0400155F RID: 5471
		public readonly string Trace;

		// Token: 0x04001560 RID: 5472
		public bool forwarded;

		// Token: 0x04001561 RID: 5473
		public int repeatCount = 1;

		// Token: 0x04001562 RID: 5474
		public bool filtered;

		// Token: 0x04001563 RID: 5475
		public int index;
	}

	// Token: 0x0200029F RID: 671
	[Serializable]
	public class DisplayedLogLine
	{
		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06001108 RID: 4360 RVA: 0x0005B836 File Offset: 0x00059A36
		// (set) Token: 0x06001109 RID: 4361 RVA: 0x0005B83E File Offset: 0x00059A3E
		public Type data { get; set; }

		// Token: 0x0600110A RID: 4362 RVA: 0x0005B848 File Offset: 0x00059A48
		public DisplayedLogLine(GameObject obj)
		{
			this.lineText = obj.GetComponentInChildren<Text>();
			this.buttons = obj.GetComponentsInChildren<GorillaDevButton>();
			this.transform = obj.GetComponent<RectTransform>();
			this.backdrop = obj.GetComponentInChildren<SpriteRenderer>();
			foreach (GorillaDevButton gorillaDevButton in this.buttons)
			{
				if (gorillaDevButton.Type == DevButtonType.LineExpand)
				{
					this.maximizeButton = gorillaDevButton;
				}
				if (gorillaDevButton.Type == DevButtonType.LineForward)
				{
					this.forwardButton = gorillaDevButton;
				}
			}
		}

		// Token: 0x04001565 RID: 5477
		public GorillaDevButton[] buttons;

		// Token: 0x04001566 RID: 5478
		public Text lineText;

		// Token: 0x04001567 RID: 5479
		public RectTransform transform;

		// Token: 0x04001568 RID: 5480
		public int targetMessage;

		// Token: 0x04001569 RID: 5481
		public GorillaDevButton maximizeButton;

		// Token: 0x0400156A RID: 5482
		public GorillaDevButton forwardButton;

		// Token: 0x0400156B RID: 5483
		public SpriteRenderer backdrop;

		// Token: 0x0400156C RID: 5484
		private bool expanded;

		// Token: 0x0400156D RID: 5485
		public DevInspector inspector;
	}

	// Token: 0x020002A0 RID: 672
	[Serializable]
	public class MessagePayload
	{
		// Token: 0x0600110B RID: 4363 RVA: 0x0005B8C4 File Offset: 0x00059AC4
		public static List<DevConsole.MessagePayload> GeneratePayloads(string username, List<DevConsole.LogEntry> entries)
		{
			List<DevConsole.MessagePayload> list = new List<DevConsole.MessagePayload>();
			List<DevConsole.MessagePayload.Block> list2 = new List<DevConsole.MessagePayload.Block>();
			entries.Sort((DevConsole.LogEntry e1, DevConsole.LogEntry e2) => e1.index.CompareTo(e2.index));
			string text = "";
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block("User `" + username + "` Forwarded some errors"));
			foreach (DevConsole.LogEntry logEntry in entries)
			{
				string[] array = logEntry.Trace.Split("\n".ToCharArray());
				string text2 = "";
				foreach (string text3 in array)
				{
					text2 = text2 + "    " + text3 + "\n";
				}
				string text4 = string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
				if (text.Length + text4.Length > 3000)
				{
					text += "```";
					list2.Add(new DevConsole.MessagePayload.Block(text));
					list.Add(new DevConsole.MessagePayload
					{
						blocks = list2.ToArray()
					});
					list2 = new List<DevConsole.MessagePayload.Block>();
					text = "```";
				}
				text += string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
			}
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block(text));
			list.Add(new DevConsole.MessagePayload
			{
				blocks = list2.ToArray()
			});
			return list;
		}

		// Token: 0x0400156F RID: 5487
		public DevConsole.MessagePayload.Block[] blocks;

		// Token: 0x020002A1 RID: 673
		[Serializable]
		public class Block
		{
			// Token: 0x0600110D RID: 4365 RVA: 0x0005BA94 File Offset: 0x00059C94
			public Block(string markdownText)
			{
				this.text = new DevConsole.MessagePayload.TextBlock
				{
					text = markdownText,
					type = "mrkdwn"
				};
				this.type = "section";
			}

			// Token: 0x04001570 RID: 5488
			public string type;

			// Token: 0x04001571 RID: 5489
			public DevConsole.MessagePayload.TextBlock text;
		}

		// Token: 0x020002A2 RID: 674
		[Serializable]
		public class TextBlock
		{
			// Token: 0x04001572 RID: 5490
			public string type;

			// Token: 0x04001573 RID: 5491
			public string text;
		}
	}
}
