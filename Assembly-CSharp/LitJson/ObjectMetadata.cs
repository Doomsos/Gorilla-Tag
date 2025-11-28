using System;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000D58 RID: 3416
	internal struct ObjectMetadata
	{
		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x06005387 RID: 21383 RVA: 0x001A66F4 File Offset: 0x001A48F4
		// (set) Token: 0x06005388 RID: 21384 RVA: 0x001A6715 File Offset: 0x001A4915
		public Type ElementType
		{
			get
			{
				if (this.element_type == null)
				{
					return typeof(JsonData);
				}
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x06005389 RID: 21385 RVA: 0x001A671E File Offset: 0x001A491E
		// (set) Token: 0x0600538A RID: 21386 RVA: 0x001A6726 File Offset: 0x001A4926
		public bool IsDictionary
		{
			get
			{
				return this.is_dictionary;
			}
			set
			{
				this.is_dictionary = value;
			}
		}

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x0600538B RID: 21387 RVA: 0x001A672F File Offset: 0x001A492F
		// (set) Token: 0x0600538C RID: 21388 RVA: 0x001A6737 File Offset: 0x001A4937
		public IDictionary<string, PropertyMetadata> Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		// Token: 0x04006122 RID: 24866
		private Type element_type;

		// Token: 0x04006123 RID: 24867
		private bool is_dictionary;

		// Token: 0x04006124 RID: 24868
		private IDictionary<string, PropertyMetadata> properties;
	}
}
