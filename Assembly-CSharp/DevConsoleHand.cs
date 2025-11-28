using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A5 RID: 677
public class DevConsoleHand : DevConsoleInstance
{
	// Token: 0x0400157A RID: 5498
	public List<GameObject> otherButtonsList;

	// Token: 0x0400157B RID: 5499
	public bool isStillEnabled = true;

	// Token: 0x0400157C RID: 5500
	public bool isLeftHand;

	// Token: 0x0400157D RID: 5501
	public ConsoleMode mode;

	// Token: 0x0400157E RID: 5502
	public double debugScale;

	// Token: 0x0400157F RID: 5503
	public double inspectorScale;

	// Token: 0x04001580 RID: 5504
	public double componentInspectorScale;

	// Token: 0x04001581 RID: 5505
	public List<GameObject> consoleButtons;

	// Token: 0x04001582 RID: 5506
	public List<GameObject> inspectorButtons;

	// Token: 0x04001583 RID: 5507
	public List<GameObject> componentInspectorButtons;

	// Token: 0x04001584 RID: 5508
	public GorillaDevButton consoleButton;

	// Token: 0x04001585 RID: 5509
	public GorillaDevButton inspectorButton;

	// Token: 0x04001586 RID: 5510
	public GorillaDevButton componentInspectorButton;

	// Token: 0x04001587 RID: 5511
	public GorillaDevButton showNonStarItems;

	// Token: 0x04001588 RID: 5512
	public GorillaDevButton showPrivateItems;

	// Token: 0x04001589 RID: 5513
	public Text componentInspectionText;

	// Token: 0x0400158A RID: 5514
	public DevInspector selectedInspector;
}
