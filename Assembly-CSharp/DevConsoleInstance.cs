using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A6 RID: 678
public class DevConsoleInstance : MonoBehaviour
{
	// Token: 0x06001113 RID: 4371 RVA: 0x000396A0 File Offset: 0x000378A0
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x0005BAF4 File Offset: 0x00059CF4
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

	// Token: 0x0400158B RID: 5515
	public GorillaDevButton[] buttons;

	// Token: 0x0400158C RID: 5516
	public GameObject[] disableWhileActive;

	// Token: 0x0400158D RID: 5517
	public GameObject[] enableWhileActive;

	// Token: 0x0400158E RID: 5518
	public float maxHeight;

	// Token: 0x0400158F RID: 5519
	public float lineHeight;

	// Token: 0x04001590 RID: 5520
	public int targetLogIndex = -1;

	// Token: 0x04001591 RID: 5521
	public int currentLogIndex;

	// Token: 0x04001592 RID: 5522
	public int expandAmount = 20;

	// Token: 0x04001593 RID: 5523
	public int expandedMessageIndex = -1;

	// Token: 0x04001594 RID: 5524
	public bool canExpand = true;

	// Token: 0x04001595 RID: 5525
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x04001596 RID: 5526
	public HashSet<LogType> selectedLogTypes;

	// Token: 0x04001597 RID: 5527
	[SerializeField]
	private GorillaDevButton[] logTypeButtons;

	// Token: 0x04001598 RID: 5528
	[SerializeField]
	private GorillaDevButton BottomButton;

	// Token: 0x04001599 RID: 5529
	public float lineStartHeight;

	// Token: 0x0400159A RID: 5530
	public float lineStartZ;

	// Token: 0x0400159B RID: 5531
	public float textStartHeight;

	// Token: 0x0400159C RID: 5532
	public float lineStartTextWidth;

	// Token: 0x0400159D RID: 5533
	public double textScale;

	// Token: 0x0400159E RID: 5534
	public bool isEnabled;

	// Token: 0x0400159F RID: 5535
	[SerializeField]
	private GameObject ConsoleLineExample;
}
