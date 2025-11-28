using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011BA RID: 4538
	[AttributeUsage(256)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x06007274 RID: 29300 RVA: 0x00258DB7 File Offset: 0x00256FB7
		public bool ShowRange
		{
			get
			{
				return this.Min != this.Max;
			}
		}

		// Token: 0x06007275 RID: 29301 RVA: 0x00258DCC File Offset: 0x00256FCC
		public ConditionalFieldAttribute(string propertyToCheck = null, object compareValue = null, object compareValue2 = null, object compareValue3 = null, object compareValue4 = null, object compareValue5 = null, object compareValue6 = null)
		{
			this.PropertyToCheck = propertyToCheck;
			this.CompareValue = compareValue;
			this.CompareValue2 = compareValue2;
			this.CompareValue3 = compareValue3;
			this.CompareValue4 = compareValue4;
			this.CompareValue5 = compareValue5;
			this.CompareValue6 = compareValue6;
			this.Label = "";
			this.Tooltip = "";
			this.Min = 0f;
			this.Max = 0f;
		}

		// Token: 0x040082CB RID: 33483
		public string PropertyToCheck;

		// Token: 0x040082CC RID: 33484
		public object CompareValue;

		// Token: 0x040082CD RID: 33485
		public object CompareValue2;

		// Token: 0x040082CE RID: 33486
		public object CompareValue3;

		// Token: 0x040082CF RID: 33487
		public object CompareValue4;

		// Token: 0x040082D0 RID: 33488
		public object CompareValue5;

		// Token: 0x040082D1 RID: 33489
		public object CompareValue6;

		// Token: 0x040082D2 RID: 33490
		public string Label;

		// Token: 0x040082D3 RID: 33491
		public string Tooltip;

		// Token: 0x040082D4 RID: 33492
		public float Min;

		// Token: 0x040082D5 RID: 33493
		public float Max;
	}
}
