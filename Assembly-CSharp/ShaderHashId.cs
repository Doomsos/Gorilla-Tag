using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000C72 RID: 3186
[Serializable]
public struct ShaderHashId : IEquatable<ShaderHashId>
{
	// Token: 0x17000741 RID: 1857
	// (get) Token: 0x06004DC4 RID: 19908 RVA: 0x00192683 File Offset: 0x00190883
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x17000742 RID: 1858
	// (get) Token: 0x06004DC5 RID: 19909 RVA: 0x0019268B File Offset: 0x0019088B
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x06004DC6 RID: 19910 RVA: 0x00192693 File Offset: 0x00190893
	public ShaderHashId(string text)
	{
		this._text = text;
		this._hash = Shader.PropertyToID(text);
	}

	// Token: 0x06004DC7 RID: 19911 RVA: 0x00192683 File Offset: 0x00190883
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x06004DC8 RID: 19912 RVA: 0x0019268B File Offset: 0x0019088B
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x06004DC9 RID: 19913 RVA: 0x0019268B File Offset: 0x0019088B
	public static implicit operator int(ShaderHashId h)
	{
		return h._hash;
	}

	// Token: 0x06004DCA RID: 19914 RVA: 0x001926A8 File Offset: 0x001908A8
	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	// Token: 0x06004DCB RID: 19915 RVA: 0x001926B0 File Offset: 0x001908B0
	public bool Equals(ShaderHashId other)
	{
		return this._hash == other._hash;
	}

	// Token: 0x06004DCC RID: 19916 RVA: 0x001926C0 File Offset: 0x001908C0
	public override bool Equals(object obj)
	{
		if (obj is ShaderHashId)
		{
			ShaderHashId other = (ShaderHashId)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06004DCD RID: 19917 RVA: 0x001926E5 File Offset: 0x001908E5
	public static bool operator ==(ShaderHashId x, ShaderHashId y)
	{
		return x.Equals(y);
	}

	// Token: 0x06004DCE RID: 19918 RVA: 0x001926EF File Offset: 0x001908EF
	public static bool operator !=(ShaderHashId x, ShaderHashId y)
	{
		return !x.Equals(y);
	}

	// Token: 0x04005CFA RID: 23802
	[FormerlySerializedAs("_hashText")]
	[SerializeField]
	private string _text;

	// Token: 0x04005CFB RID: 23803
	[NonSerialized]
	private int _hash;
}
