using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004AA RID: 1194
public class TransferrableObjectSyncedBool : TransferrableObject
{
	// Token: 0x06001EDC RID: 7900 RVA: 0x000A3BB7 File Offset: 0x000A1DB7
	internal override void OnEnable()
	{
		base.OnEnable();
		this.OnItemStateBoolFalse.AddListener(new UnityAction(this.OnItemStateChanged));
		this.OnItemStateBoolTrue.AddListener(new UnityAction(this.OnItemStateChanged));
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x000A3BED File Offset: 0x000A1DED
	internal override void OnDisable()
	{
		base.OnDisable();
		this.OnItemStateBoolFalse.RemoveListener(new UnityAction(this.OnItemStateChanged));
		this.OnItemStateBoolTrue.RemoveListener(new UnityAction(this.OnItemStateChanged));
	}

	// Token: 0x06001EDE RID: 7902 RVA: 0x000A3C24 File Offset: 0x000A1E24
	public void SetItemState(bool state)
	{
		base.SetItemStateBool(state);
	}

	// Token: 0x06001EDF RID: 7903 RVA: 0x000A3C38 File Offset: 0x000A1E38
	public void ToggleItemState()
	{
		base.ToggleNetworkedItemStateBool();
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x000A3C4B File Offset: 0x000A1E4B
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			UnityEvent onItemStateSetFalse = this.OnItemStateSetFalse;
			if (onItemStateSetFalse == null)
			{
				return;
			}
			onItemStateSetFalse.Invoke();
			return;
		}
		else
		{
			UnityEvent onItemStateSetTrue = this.OnItemStateSetTrue;
			if (onItemStateSetTrue == null)
			{
				return;
			}
			onItemStateSetTrue.Invoke();
			return;
		}
	}

	// Token: 0x0400292B RID: 10539
	[SerializeField]
	private bool deprecatedWarning = true;

	// Token: 0x0400292C RID: 10540
	[SerializeField]
	private UnityEvent OnItemStateSetTrue;

	// Token: 0x0400292D RID: 10541
	[SerializeField]
	private UnityEvent OnItemStateSetFalse;
}
