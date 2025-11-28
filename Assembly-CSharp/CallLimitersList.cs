using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000BD7 RID: 3031
[Serializable]
public class CallLimitersList<Titem, Tenum> where Titem : CallLimiter, new() where Tenum : Enum
{
	// Token: 0x06004B01 RID: 19201 RVA: 0x0018840C File Offset: 0x0018660C
	public bool IsSpamming(Tenum index)
	{
		return this.IsSpamming((int)((object)index));
	}

	// Token: 0x06004B02 RID: 19202 RVA: 0x0018841F File Offset: 0x0018661F
	public bool IsSpamming(int index)
	{
		return !this.m_callLimiters[index].CheckCallTime(Time.unscaledTime);
	}

	// Token: 0x06004B03 RID: 19203 RVA: 0x0018843F File Offset: 0x0018663F
	public bool IsSpamming(Tenum index, double serverTime)
	{
		return this.IsSpamming((int)((object)index), serverTime);
	}

	// Token: 0x06004B04 RID: 19204 RVA: 0x00188453 File Offset: 0x00186653
	public bool IsSpamming(int index, double serverTime)
	{
		return !this.m_callLimiters[index].CheckCallServerTime(serverTime);
	}

	// Token: 0x06004B05 RID: 19205 RVA: 0x00188470 File Offset: 0x00186670
	public void Reset()
	{
		Titem[] callLimiters = this.m_callLimiters;
		for (int i = 0; i < callLimiters.Length; i++)
		{
			callLimiters[i].Reset();
		}
	}

	// Token: 0x04005B2A RID: 23338
	[RequiredListLength("GetMaxLength")]
	[SerializeField]
	private Titem[] m_callLimiters;
}
