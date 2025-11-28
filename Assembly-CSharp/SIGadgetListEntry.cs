using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200011A RID: 282
public class SIGadgetListEntry : MonoBehaviour
{
	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000735 RID: 1845 RVA: 0x00027B28 File Offset: 0x00025D28
	// (set) Token: 0x06000736 RID: 1846 RVA: 0x00027B30 File Offset: 0x00025D30
	public int Id { get; private set; } = -1;

	// Token: 0x06000737 RID: 1847 RVA: 0x00027B3C File Offset: 0x00025D3C
	public void Configure(ITouchScreenStation station, SITechTreePage page, Transform imageTarget, Transform textTarget, SITouchscreenButton.SITouchscreenButtonType buttonType = SITouchscreenButton.SITouchscreenButtonType.Select, int index = 0, float verticalOffset = 0f)
	{
		base.name = (this.gadgetText.text = page.nickName);
		SITouchscreenButton button = this.buttonContainer.button;
		button.buttonType = buttonType;
		this.Id = (button.data = (int)page.pageId);
		button.buttonPressed.RemoveAllListeners();
		button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		station.AddButton(button, false);
		base.transform.localPosition += new Vector3(0f, (float)index * verticalOffset, 0f);
		this.imageFlattener.overrideParentTransform = imageTarget;
		this.textFlattener.overrideParentTransform = textTarget;
		this.imageFlattener.enabled = true;
		this.textFlattener.enabled = true;
	}

	// Token: 0x04000914 RID: 2324
	[SerializeField]
	private TextMeshProUGUI gadgetText;

	// Token: 0x04000915 RID: 2325
	[SerializeField]
	private SITouchscreenButtonContainer buttonContainer;

	// Token: 0x04000916 RID: 2326
	public ObjectHierarchyFlattener imageFlattener;

	// Token: 0x04000917 RID: 2327
	public ObjectHierarchyFlattener textFlattener;

	// Token: 0x04000918 RID: 2328
	public GameObject selectionIndicator;
}
