using System;
using UnityEngine;

// Token: 0x020004A8 RID: 1192
public class TransferrableObjectGripPosition : MonoBehaviour
{
	// Token: 0x06001ED0 RID: 7888 RVA: 0x000A3A4B File Offset: 0x000A1C4B
	private void Awake()
	{
		if (this.parentObject == null)
		{
			this.parentObject = base.transform.parent.GetComponent<TransferrableItemSlotTransformOverride>();
		}
		this.parentObject.AddGripPosition(this.attachmentType, this);
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x000A3A83 File Offset: 0x000A1C83
	public SubGrabPoint CreateSubGrabPoint(SlotTransformOverride overrideContainer)
	{
		return new SubGrabPoint();
	}

	// Token: 0x04002926 RID: 10534
	[SerializeField]
	private TransferrableItemSlotTransformOverride parentObject;

	// Token: 0x04002927 RID: 10535
	[SerializeField]
	private TransferrableObject.PositionState attachmentType;
}
