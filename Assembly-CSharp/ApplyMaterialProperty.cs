using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200022F RID: 559
public class ApplyMaterialProperty : MonoBehaviour
{
	// Token: 0x06000EE7 RID: 3815 RVA: 0x0004F170 File Offset: 0x0004D370
	private void Start()
	{
		this.UpdateShaderPropertyIds();
		if (this.applyOnStart)
		{
			this.Apply();
		}
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0004F188 File Offset: 0x0004D388
	public void Apply()
	{
		if (!this._renderer)
		{
			this._renderer = base.GetComponent<Renderer>();
		}
		ApplyMaterialProperty.ApplyMode applyMode = this.mode;
		if (applyMode == ApplyMaterialProperty.ApplyMode.MaterialInstance)
		{
			this.ApplyMaterialInstance();
			return;
		}
		if (applyMode != ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock)
		{
			return;
		}
		this.ApplyMaterialPropertyBlock();
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x0004F1CA File Offset: 0x0004D3CA
	public void SetColor(string propertyName, Color color)
	{
		this.SetColor(Shader.PropertyToID(propertyName), color);
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0004F1D9 File Offset: 0x0004D3D9
	public void SetColor(int propertyId, Color color)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyId, null);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Color;
		orCreateData.color = color;
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0004F1F0 File Offset: 0x0004D3F0
	public void SetFloat(string propertyName, float value)
	{
		this.SetFloat(Shader.PropertyToID(propertyName), value);
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0004F1FF File Offset: 0x0004D3FF
	public void SetFloat(int propertyId, float value)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyId, null);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Float;
		orCreateData.@float = value;
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0004F218 File Offset: 0x0004D418
	private ApplyMaterialProperty.CustomMaterialData GetOrCreateData(int id, string propertyName)
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			if (this.customData[i].id == id)
			{
				return this.customData[i];
			}
		}
		ApplyMaterialProperty.CustomMaterialData customMaterialData = new ApplyMaterialProperty.CustomMaterialData(id, propertyName);
		this.customData.Add(customMaterialData);
		return customMaterialData;
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0004F274 File Offset: 0x0004D474
	private void ApplyMaterialInstance()
	{
		if (!this._instance)
		{
			this._instance = base.GetComponent<MaterialInstance>();
			if (this._instance == null)
			{
				this._instance = base.gameObject.AddComponent<MaterialInstance>();
			}
		}
		Material material = this.targetMaterial = this._instance.Material;
		for (int i = 0; i < this.customData.Count; i++)
		{
			switch (this.customData[i].dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				material.SetColor(this.customData[i].id, this.customData[i].color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				material.SetFloat(this.customData[i].id, this.customData[i].@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				material.SetVector(this.customData[i].id, this.customData[i].vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				material.SetVector(this.customData[i].id, this.customData[i].vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				material.SetVector(this.customData[i].id, this.customData[i].vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				material.SetTexture(this.customData[i].id, this.customData[i].texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x0004F440 File Offset: 0x0004D640
	private void ApplyMaterialPropertyBlock()
	{
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		for (int i = 0; i < this.customData.Count; i++)
		{
			switch (this.customData[i].dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				this._block.SetColor(this.customData[i].id, this.customData[i].color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				this._block.SetFloat(this.customData[i].id, this.customData[i].@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				this._block.SetTexture(this.customData[i].id, this.customData[i].texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x0004F600 File Offset: 0x0004D800
	private void UpdateShaderPropertyIds()
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			if (this.customData[i] != null && !string.IsNullOrEmpty(this.customData[i].name))
			{
				this.customData[i].id = Shader.PropertyToID(this.customData[i].name);
			}
		}
	}

	// Token: 0x04001229 RID: 4649
	public ApplyMaterialProperty.ApplyMode mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;

	// Token: 0x0400122A RID: 4650
	[FormerlySerializedAs("materialToApplyBlock")]
	public Material targetMaterial;

	// Token: 0x0400122B RID: 4651
	[SerializeField]
	private MaterialInstance _instance;

	// Token: 0x0400122C RID: 4652
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x0400122D RID: 4653
	public List<ApplyMaterialProperty.CustomMaterialData> customData;

	// Token: 0x0400122E RID: 4654
	[SerializeField]
	private bool applyOnStart;

	// Token: 0x0400122F RID: 4655
	[NonSerialized]
	private MaterialPropertyBlock _block;

	// Token: 0x02000230 RID: 560
	public enum ApplyMode
	{
		// Token: 0x04001231 RID: 4657
		MaterialInstance,
		// Token: 0x04001232 RID: 4658
		MaterialPropertyBlock
	}

	// Token: 0x02000231 RID: 561
	public enum SuportedTypes
	{
		// Token: 0x04001234 RID: 4660
		Color,
		// Token: 0x04001235 RID: 4661
		Float,
		// Token: 0x04001236 RID: 4662
		Vector2,
		// Token: 0x04001237 RID: 4663
		Vector3,
		// Token: 0x04001238 RID: 4664
		Vector4,
		// Token: 0x04001239 RID: 4665
		Texture2D
	}

	// Token: 0x02000232 RID: 562
	[Serializable]
	public class CustomMaterialData
	{
		// Token: 0x06000EF2 RID: 3826 RVA: 0x0004F680 File Offset: 0x0004D880
		public CustomMaterialData(string propertyName)
		{
			this.name = propertyName;
			this.id = Shader.PropertyToID(propertyName);
			this.dataType = ApplyMaterialProperty.SuportedTypes.Color;
			this.color = default(Color);
			this.@float = 0f;
			this.vector2 = default(Vector2);
			this.vector3 = default(Vector3);
			this.vector4 = default(Vector4);
			this.texture2D = null;
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x0004F6F0 File Offset: 0x0004D8F0
		public CustomMaterialData(int propertyId, string propertyName)
		{
			this.name = propertyName;
			this.id = propertyId;
			this.dataType = ApplyMaterialProperty.SuportedTypes.Color;
			this.color = default(Color);
			this.@float = 0f;
			this.vector2 = default(Vector2);
			this.vector3 = default(Vector3);
			this.vector4 = default(Vector4);
			this.texture2D = null;
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x0004F75C File Offset: 0x0004D95C
		public override int GetHashCode()
		{
			return new ValueTuple<int, ApplyMaterialProperty.SuportedTypes, Color, float, Vector2, Vector3, Vector4, ValueTuple<Texture2D>>(this.id, this.dataType, this.color, this.@float, this.vector2, this.vector3, this.vector4, new ValueTuple<Texture2D>(this.texture2D)).GetHashCode();
		}

		// Token: 0x0400123A RID: 4666
		public string name;

		// Token: 0x0400123B RID: 4667
		public int id;

		// Token: 0x0400123C RID: 4668
		public ApplyMaterialProperty.SuportedTypes dataType;

		// Token: 0x0400123D RID: 4669
		public Color color;

		// Token: 0x0400123E RID: 4670
		public float @float;

		// Token: 0x0400123F RID: 4671
		public Vector2 vector2;

		// Token: 0x04001240 RID: 4672
		public Vector3 vector3;

		// Token: 0x04001241 RID: 4673
		public Vector4 vector4;

		// Token: 0x04001242 RID: 4674
		public Texture2D texture2D;
	}
}
