using System;
using UnityEngine;

// Token: 0x02000443 RID: 1091
public class BitmapFontText : MonoBehaviour
{
	// Token: 0x06001ACA RID: 6858 RVA: 0x0008D717 File Offset: 0x0008B917
	private void Awake()
	{
		this.Init();
		this.Render();
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x0008D725 File Offset: 0x0008B925
	public void Render()
	{
		this.font.RenderToTexture(this.texture, this.uppercaseOnly ? this.text.ToUpperInvariant() : this.text);
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x0008D754 File Offset: 0x0008B954
	public void Init()
	{
		this.texture = new Texture2D(this.textArea.x, this.textArea.y, this.font.fontImage.format, false);
		this.texture.filterMode = 0;
		this.material = new Material(this.renderer.sharedMaterial);
		this.material.mainTexture = this.texture;
		this.renderer.sharedMaterial = this.material;
	}

	// Token: 0x04002449 RID: 9289
	public string text;

	// Token: 0x0400244A RID: 9290
	public bool uppercaseOnly;

	// Token: 0x0400244B RID: 9291
	public Vector2Int textArea;

	// Token: 0x0400244C RID: 9292
	[Space]
	public Renderer renderer;

	// Token: 0x0400244D RID: 9293
	public Texture2D texture;

	// Token: 0x0400244E RID: 9294
	public Material material;

	// Token: 0x0400244F RID: 9295
	public BitmapFont font;
}
