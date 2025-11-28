using System;
using UnityEngine;

// Token: 0x020002CD RID: 717
[Serializable]
public struct GTOption<T>
{
	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060011B1 RID: 4529 RVA: 0x0005D55E File Offset: 0x0005B75E
	public T ResolvedValue
	{
		get
		{
			if (!this.enabled)
			{
				return this.defaultValue;
			}
			return this.value;
		}
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x0005D575 File Offset: 0x0005B775
	public GTOption(T defaultValue)
	{
		this.enabled = false;
		this.value = defaultValue;
		this.defaultValue = defaultValue;
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x0005D58C File Offset: 0x0005B78C
	public void ResetValue()
	{
		this.value = this.defaultValue;
	}

	// Token: 0x0400162E RID: 5678
	[Tooltip("When checked, the filter is applied; when unchecked (default), it is ignored.")]
	[SerializeField]
	public bool enabled;

	// Token: 0x0400162F RID: 5679
	[SerializeField]
	public T value;

	// Token: 0x04001630 RID: 5680
	[NonSerialized]
	public readonly T defaultValue;
}
