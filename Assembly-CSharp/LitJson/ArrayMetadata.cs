using System;

namespace LitJson
{
	// Token: 0x02000D57 RID: 3415
	internal struct ArrayMetadata
	{
		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x06005381 RID: 21377 RVA: 0x001A66A8 File Offset: 0x001A48A8
		// (set) Token: 0x06005382 RID: 21378 RVA: 0x001A66C9 File Offset: 0x001A48C9
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

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x06005383 RID: 21379 RVA: 0x001A66D2 File Offset: 0x001A48D2
		// (set) Token: 0x06005384 RID: 21380 RVA: 0x001A66DA File Offset: 0x001A48DA
		public bool IsArray
		{
			get
			{
				return this.is_array;
			}
			set
			{
				this.is_array = value;
			}
		}

		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x06005385 RID: 21381 RVA: 0x001A66E3 File Offset: 0x001A48E3
		// (set) Token: 0x06005386 RID: 21382 RVA: 0x001A66EB File Offset: 0x001A48EB
		public bool IsList
		{
			get
			{
				return this.is_list;
			}
			set
			{
				this.is_list = value;
			}
		}

		// Token: 0x0400611F RID: 24863
		private Type element_type;

		// Token: 0x04006120 RID: 24864
		private bool is_array;

		// Token: 0x04006121 RID: 24865
		private bool is_list;
	}
}
