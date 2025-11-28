using System;
using UnityEngine;

// Token: 0x02000C21 RID: 3105
[Serializable]
public struct AnimHashId
{
	// Token: 0x17000727 RID: 1831
	// (get) Token: 0x06004C59 RID: 19545 RVA: 0x0018D2B5 File Offset: 0x0018B4B5
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x17000728 RID: 1832
	// (get) Token: 0x06004C5A RID: 19546 RVA: 0x0018D2BD File Offset: 0x0018B4BD
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x06004C5B RID: 19547 RVA: 0x0018D2C5 File Offset: 0x0018B4C5
	public AnimHashId(string text)
	{
		this._text = text;
		this._hash = Animator.StringToHash(text);
	}

	// Token: 0x06004C5C RID: 19548 RVA: 0x0018D2B5 File Offset: 0x0018B4B5
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x06004C5D RID: 19549 RVA: 0x0018D2BD File Offset: 0x0018B4BD
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x06004C5E RID: 19550 RVA: 0x0018D2BD File Offset: 0x0018B4BD
	public static implicit operator int(AnimHashId h)
	{
		return h._hash;
	}

	// Token: 0x06004C5F RID: 19551 RVA: 0x0018D2DA File Offset: 0x0018B4DA
	public static implicit operator AnimHashId(string s)
	{
		return new AnimHashId(s);
	}

	// Token: 0x04005C47 RID: 23623
	[SerializeField]
	private string _text;

	// Token: 0x04005C48 RID: 23624
	[NonSerialized]
	private int _hash;
}
