using System;

namespace GorillaTag
{
	// Token: 0x02000FCB RID: 4043
	[AttributeUsage(4, AllowMultiple = false, Inherited = false)]
	public class GTStripGameObjectFromBuildAttribute : Attribute
	{
		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x0600667D RID: 26237 RVA: 0x002163C7 File Offset: 0x002145C7
		public string Condition { get; }

		// Token: 0x0600667E RID: 26238 RVA: 0x002163CF File Offset: 0x002145CF
		public GTStripGameObjectFromBuildAttribute(string condition = "")
		{
			this.Condition = condition;
		}
	}
}
