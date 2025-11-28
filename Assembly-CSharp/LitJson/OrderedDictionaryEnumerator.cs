using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000D54 RID: 3412
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x06005373 RID: 21363 RVA: 0x001A6586 File Offset: 0x001A4786
		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x06005374 RID: 21364 RVA: 0x001A6594 File Offset: 0x001A4794
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x06005375 RID: 21365 RVA: 0x001A65C0 File Offset: 0x001A47C0
		public object Key
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Key;
			}
		}

		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x06005376 RID: 21366 RVA: 0x001A65E0 File Offset: 0x001A47E0
		public object Value
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Value;
			}
		}

		// Token: 0x06005377 RID: 21367 RVA: 0x001A6600 File Offset: 0x001A4800
		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			this.list_enumerator = enumerator;
		}

		// Token: 0x06005378 RID: 21368 RVA: 0x001A660F File Offset: 0x001A480F
		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		// Token: 0x06005379 RID: 21369 RVA: 0x001A661C File Offset: 0x001A481C
		public void Reset()
		{
			this.list_enumerator.Reset();
		}

		// Token: 0x0400611B RID: 24859
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
	}
}
