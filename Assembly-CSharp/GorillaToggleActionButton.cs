using System;
using UnityEngine.Events;

// Token: 0x02000928 RID: 2344
public class GorillaToggleActionButton : GorillaPressableButton
{
	// Token: 0x06003BED RID: 15341 RVA: 0x0013C897 File Offset: 0x0013AA97
	public override void Start()
	{
		this.BindToggleAction();
	}

	// Token: 0x06003BEE RID: 15342 RVA: 0x0013C8A0 File Offset: 0x0013AAA0
	private void BindToggleAction()
	{
		if (this.ToggleAction == null || !this.ToggleAction.IsValid)
		{
			return;
		}
		this.ToggleAction.Cache();
		this.onPressButton = new UnityEvent();
		this.onPressButton.AddListener(new UnityAction(this.ExecuteToggleAction));
	}

	// Token: 0x06003BEF RID: 15343 RVA: 0x0013C8F0 File Offset: 0x0013AAF0
	private void ExecuteToggleAction()
	{
		ComponentFunctionReference<bool> toggleAction = this.ToggleAction;
		this.isOn = (toggleAction != null && toggleAction.Invoke());
		this.UpdateColor();
	}

	// Token: 0x04004C7E RID: 19582
	public ComponentFunctionReference<bool> ToggleAction;

	// Token: 0x04004C7F RID: 19583
	private Func<bool> toggleFunc;
}
