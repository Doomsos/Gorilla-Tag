using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000440 RID: 1088
public class BitmapFont : ScriptableObject
{
	// Token: 0x06001AC3 RID: 6851 RVA: 0x0008D58C File Offset: 0x0008B78C
	private void OnEnable()
	{
		this._charToSymbol = Enumerable.ToDictionary<BitmapFont.SymbolData, char, BitmapFont.SymbolData>(this.symbols, (BitmapFont.SymbolData s) => s.character, (BitmapFont.SymbolData s) => s);
	}

	// Token: 0x06001AC4 RID: 6852 RVA: 0x0008D5E8 File Offset: 0x0008B7E8
	public void RenderToTexture(Texture2D target, string text)
	{
		if (text == null)
		{
			text = string.Empty;
		}
		int num = target.width * target.height;
		if (this._empty.Length != num)
		{
			this._empty = new Color[num];
			for (int i = 0; i < this._empty.Length; i++)
			{
				this._empty[i] = Color.black;
			}
		}
		target.SetPixels(this._empty);
		int length = text.Length;
		int num2 = 1;
		int width = this.fontImage.width;
		int height = this.fontImage.height;
		for (int j = 0; j < length; j++)
		{
			char c = text.get_Chars(j);
			BitmapFont.SymbolData symbolData = this._charToSymbol[c];
			int width2 = symbolData.width;
			int height2 = symbolData.height;
			int x = symbolData.x;
			int y = symbolData.y;
			Graphics.CopyTexture(this.fontImage, 0, 0, x, height - (y + height2), width2, height2, target, 0, 0, num2, 2 + symbolData.yoffset);
			num2 += width2 + 1;
		}
		target.Apply(false);
	}

	// Token: 0x04002437 RID: 9271
	public Texture2D fontImage;

	// Token: 0x04002438 RID: 9272
	public TextAsset fontJson;

	// Token: 0x04002439 RID: 9273
	public int symbolPixelsPerUnit = 1;

	// Token: 0x0400243A RID: 9274
	public string characterMap;

	// Token: 0x0400243B RID: 9275
	[Space]
	public BitmapFont.SymbolData[] symbols = new BitmapFont.SymbolData[0];

	// Token: 0x0400243C RID: 9276
	private Dictionary<char, BitmapFont.SymbolData> _charToSymbol;

	// Token: 0x0400243D RID: 9277
	private Color[] _empty = new Color[0];

	// Token: 0x02000441 RID: 1089
	[Serializable]
	public struct SymbolData
	{
		// Token: 0x0400243E RID: 9278
		public char character;

		// Token: 0x0400243F RID: 9279
		[Space]
		public int id;

		// Token: 0x04002440 RID: 9280
		public int width;

		// Token: 0x04002441 RID: 9281
		public int height;

		// Token: 0x04002442 RID: 9282
		public int x;

		// Token: 0x04002443 RID: 9283
		public int y;

		// Token: 0x04002444 RID: 9284
		public int xadvance;

		// Token: 0x04002445 RID: 9285
		public int yoffset;
	}
}
