using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class UberShaderProperty
{
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

	[MethodImpl(256)]
	private unsafe static TOut ValueAs<TIn, TOut>(TIn value)
	{
		return *Unsafe.As<TIn, TOut>(ref value);
	}

	public int index;

	public int nameID;

	public string name;

	public ShaderPropertyType type;

	public ShaderPropertyFlags flags;

	public Vector2 rangeLimits;

	public string[] attributes;

	public bool isKeywordToggle;

	public string keyword;
}
