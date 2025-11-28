using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000113 RID: 275
public class SIDispenserGadgetListEntry : MonoBehaviour
{
	// Token: 0x06000707 RID: 1799 RVA: 0x0002695C File Offset: 0x00024B5C
	public void SetStation(ITouchScreenStation station, Transform imageTarget, Transform textTarget)
	{
		this.dispenseButton.button.buttonPressed.RemoveAllListeners();
		this.dispenseButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		this.infoButton.button.buttonPressed.RemoveAllListeners();
		this.infoButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		station.AddButton(this.dispenseButton.button, false);
		station.AddButton(this.infoButton.button, false);
		this.image1.overrideParentTransform = imageTarget;
		this.image2.overrideParentTransform = imageTarget;
		this.text1.overrideParentTransform = textTarget;
		this.text2.overrideParentTransform = textTarget;
		this.image1.enabled = true;
		this.image2.enabled = true;
		this.text1.enabled = true;
		this.text2.enabled = true;
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x00026A5C File Offset: 0x00024C5C
	public void SetTechTreeNode(SITechTreeNode node)
	{
		base.name = (this.gadgetText.text = node.nickName);
		int nodeId = node.upgradeType.GetNodeId();
		SIDispenserGadgetListEntry.<SetTechTreeNode>g__ConfigureButton|8_0(this.dispenseButton.button, SITouchscreenButton.SITouchscreenButtonType.Dispense, nodeId);
		SIDispenserGadgetListEntry.<SetTechTreeNode>g__ConfigureButton|8_0(this.infoButton.button, SITouchscreenButton.SITouchscreenButtonType.Select, nodeId);
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00026AB3 File Offset: 0x00024CB3
	[CompilerGenerated]
	internal static void <SetTechTreeNode>g__ConfigureButton|8_0(SITouchscreenButton button, SITouchscreenButton.SITouchscreenButtonType type, int data)
	{
		button.buttonType = type;
		button.data = data;
	}

	// Token: 0x040008E0 RID: 2272
	[SerializeField]
	private TextMeshProUGUI gadgetText;

	// Token: 0x040008E1 RID: 2273
	[SerializeField]
	private SITouchscreenButtonContainer dispenseButton;

	// Token: 0x040008E2 RID: 2274
	[SerializeField]
	private SITouchscreenButtonContainer infoButton;

	// Token: 0x040008E3 RID: 2275
	public ObjectHierarchyFlattener image1;

	// Token: 0x040008E4 RID: 2276
	public ObjectHierarchyFlattener image2;

	// Token: 0x040008E5 RID: 2277
	public ObjectHierarchyFlattener text1;

	// Token: 0x040008E6 RID: 2278
	public ObjectHierarchyFlattener text2;
}
