using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020008FD RID: 2301
[Serializable]
public class UberShaderProperty
{
	// Token: 0x06003AD4 RID: 15060 RVA: 0x00136F80 File Offset: 0x00135180
	public T GetValue<T>(Material target)
	{
		switch (this.type)
		{
		case 0:
			return UberShaderProperty.ValueAs<Color, T>(target.GetColor(this.nameID));
		case 1:
			return UberShaderProperty.ValueAs<Vector4, T>(target.GetVector(this.nameID));
		case 2:
		case 3:
			return UberShaderProperty.ValueAs<float, T>(target.GetFloat(this.nameID));
		case 4:
			return UberShaderProperty.ValueAs<Texture, T>(target.GetTexture(this.nameID));
		case 5:
			return UberShaderProperty.ValueAs<int, T>(target.GetInt(this.nameID));
		default:
			return default(T);
		}
	}

	// Token: 0x06003AD5 RID: 15061 RVA: 0x00137018 File Offset: 0x00135218
	public void SetValue<T>(Material target, T value)
	{
		switch (this.type)
		{
		case 0:
			target.SetColor(this.nameID, UberShaderProperty.ValueAs<T, Color>(value));
			break;
		case 1:
			target.SetVector(this.nameID, UberShaderProperty.ValueAs<T, Vector4>(value));
			break;
		case 2:
		case 3:
			target.SetFloat(this.nameID, UberShaderProperty.ValueAs<T, float>(value));
			break;
		case 4:
			target.SetTexture(this.nameID, UberShaderProperty.ValueAs<T, Texture>(value));
			break;
		case 5:
			target.SetInt(this.nameID, UberShaderProperty.ValueAs<T, int>(value));
			break;
		}
		if (!this.isKeywordToggle)
		{
			return;
		}
		bool flag = false;
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != 2)
		{
			if (shaderPropertyType == 5)
			{
				flag = (UberShaderProperty.ValueAs<T, int>(value) >= 1);
			}
		}
		else
		{
			flag = (UberShaderProperty.ValueAs<T, float>(value) >= 0.5f);
		}
		if (flag)
		{
			target.EnableKeyword(this.keyword);
			return;
		}
		target.DisableKeyword(this.keyword);
	}

	// Token: 0x06003AD6 RID: 15062 RVA: 0x00137104 File Offset: 0x00135304
	public void Enable(Material target)
	{
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != 2)
		{
			if (shaderPropertyType == 5)
			{
				target.SetInt(this.nameID, 1);
			}
		}
		else
		{
			target.SetFloat(this.nameID, 1f);
		}
		if (this.isKeywordToggle)
		{
			target.EnableKeyword(this.keyword);
		}
	}

	// Token: 0x06003AD7 RID: 15063 RVA: 0x00137154 File Offset: 0x00135354
	public void Disable(Material target)
	{
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != 2)
		{
			if (shaderPropertyType == 5)
			{
				target.SetInt(this.nameID, 0);
			}
		}
		else
		{
			target.SetFloat(this.nameID, 0f);
		}
		if (this.isKeywordToggle)
		{
			target.DisableKeyword(this.keyword);
		}
	}

	// Token: 0x06003AD8 RID: 15064 RVA: 0x001371A4 File Offset: 0x001353A4
	public bool TryGetKeywordState(Material target, out bool enabled)
	{
		enabled = false;
		if (!this.isKeywordToggle)
		{
			return false;
		}
		enabled = target.IsKeywordEnabled(this.keyword);
		return true;
	}

	// Token: 0x06003AD9 RID: 15065 RVA: 0x001371C2 File Offset: 0x001353C2
	[MethodImpl(256)]
	private unsafe static TOut ValueAs<TIn, TOut>(TIn value)
	{
		return *Unsafe.As<TIn, TOut>(ref value);
	}

	// Token: 0x04004ADB RID: 19163
	public int index;

	// Token: 0x04004ADC RID: 19164
	public int nameID;

	// Token: 0x04004ADD RID: 19165
	public string name;

	// Token: 0x04004ADE RID: 19166
	public ShaderPropertyType type;

	// Token: 0x04004ADF RID: 19167
	public ShaderPropertyFlags flags;

	// Token: 0x04004AE0 RID: 19168
	public Vector2 rangeLimits;

	// Token: 0x04004AE1 RID: 19169
	public string[] attributes;

	// Token: 0x04004AE2 RID: 19170
	public bool isKeywordToggle;

	// Token: 0x04004AE3 RID: 19171
	public string keyword;
}
