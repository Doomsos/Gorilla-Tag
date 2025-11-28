using System;
using UnityEngine;

// Token: 0x0200030B RID: 779
public struct TexFormatInfo
{
	// Token: 0x060012DF RID: 4831 RVA: 0x0006E138 File Offset: 0x0006C338
	public TexFormatInfo(Texture2D tex2d)
	{
		this.width = tex2d.width;
		this.height = tex2d.height;
		this.format = tex2d.format;
		this.filterMode = tex2d.filterMode;
		this.isLinearColor = !tex2d.isDataSRGB;
		this.mipmapCount = tex2d.mipmapCount;
		this.isValid = true;
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x0006E198 File Offset: 0x0006C398
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"TexFormatInfo(isValid: ",
			this.isValid.ToString(),
			", width: ",
			this.width.ToString(),
			", height: ",
			this.height.ToString(),
			", format: ",
			this.format.ToString(),
			", filterMode: ",
			this.filterMode.ToString(),
			", isLinearColor: ",
			this.isLinearColor.ToString(),
			", mipmapCount: ",
			this.mipmapCount.ToString(),
			")"
		});
	}

	// Token: 0x04001C96 RID: 7318
	public bool isValid;

	// Token: 0x04001C97 RID: 7319
	public int width;

	// Token: 0x04001C98 RID: 7320
	public int height;

	// Token: 0x04001C99 RID: 7321
	public TextureFormat format;

	// Token: 0x04001C9A RID: 7322
	public FilterMode filterMode;

	// Token: 0x04001C9B RID: 7323
	public int mipmapCount;

	// Token: 0x04001C9C RID: 7324
	public bool isLinearColor;
}
