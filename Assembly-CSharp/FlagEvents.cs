using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004D8 RID: 1240
[Serializable]
public class FlagEvents<T> where T : Enum
{
	// Token: 0x06001FE7 RID: 8167 RVA: 0x000A9B10 File Offset: 0x000A7D10
	public void InvokeAll(T test, bool isLocal = false)
	{
		int num = Convert.ToInt32(test);
		for (int i = 0; i < this.list.Length; i++)
		{
			if ((num & this.list[i].flagsAsInt) != 0 && (!this.list[i].runOnlyLocally || isLocal))
			{
				UnityEvent anyFlagTrue = this.list[i].anyFlagTrue;
				if (anyFlagTrue != null)
				{
					anyFlagTrue.Invoke();
				}
			}
		}
	}

	// Token: 0x04002A3D RID: 10813
	[SerializeField]
	private FlagEvents<T>.FlagEvent[] list;

	// Token: 0x020004D9 RID: 1241
	[Serializable]
	private class FlagEvent : ISerializationCallbackReceiver
	{
		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06001FE9 RID: 8169 RVA: 0x000A9B79 File Offset: 0x000A7D79
		private string FlagsLabel
		{
			get
			{
				return typeof(T).Name;
			}
		}

		// Token: 0x06001FEA RID: 8170 RVA: 0x000A9B8A File Offset: 0x000A7D8A
		public void OnBeforeSerialize()
		{
			this.flagsAsInt = Convert.ToInt32(this.flags);
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x000A9BA2 File Offset: 0x000A7DA2
		public void OnAfterDeserialize()
		{
			this.flags = (T)((object)this.flagsAsInt);
		}

		// Token: 0x04002A3E RID: 10814
		public string debugName = "Any flag true";

		// Token: 0x04002A3F RID: 10815
		[Tooltip("Check this box if only the local player is supposed to run this event.")]
		public bool runOnlyLocally;

		// Token: 0x04002A40 RID: 10816
		private T flags;

		// Token: 0x04002A41 RID: 10817
		[HideInInspector]
		public int flagsAsInt;

		// Token: 0x04002A42 RID: 10818
		public UnityEvent anyFlagTrue;
	}
}
