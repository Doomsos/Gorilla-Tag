using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000CEB RID: 3307
[Serializable]
public class VoiceLoudnessReactorRendererColorTarget
{
	// Token: 0x06005065 RID: 20581 RVA: 0x0019E218 File Offset: 0x0019C418
	public void Inititialize()
	{
		if (this._materials == null)
		{
			this._materials = new List<Material>(this.renderer.materials);
			this._materials[this.materialIndex].EnableKeyword(this.colorProperty);
			this.renderer.SetMaterials(this._materials);
			this.UpdateMaterialColor(0f);
		}
	}

	// Token: 0x06005066 RID: 20582 RVA: 0x0019E27C File Offset: 0x0019C47C
	public void UpdateMaterialColor(float level)
	{
		Color color = this.gradient.Evaluate(level);
		if (this._lastColor == color)
		{
			return;
		}
		this._materials[this.materialIndex].SetColor(this.colorProperty, color);
		this._lastColor = color;
	}

	// Token: 0x04005FAD RID: 24493
	[SerializeField]
	private string colorProperty = "_BaseColor";

	// Token: 0x04005FAE RID: 24494
	public Renderer renderer;

	// Token: 0x04005FAF RID: 24495
	public int materialIndex;

	// Token: 0x04005FB0 RID: 24496
	public Gradient gradient;

	// Token: 0x04005FB1 RID: 24497
	public bool useSmoothedLoudness;

	// Token: 0x04005FB2 RID: 24498
	public float scale = 1f;

	// Token: 0x04005FB3 RID: 24499
	private List<Material> _materials;

	// Token: 0x04005FB4 RID: 24500
	private Color _lastColor = Color.white;
}
