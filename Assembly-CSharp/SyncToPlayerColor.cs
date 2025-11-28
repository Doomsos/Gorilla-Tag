using System;
using UnityEngine;

// Token: 0x020004F5 RID: 1269
public class SyncToPlayerColor : MonoBehaviour
{
	// Token: 0x060020A3 RID: 8355 RVA: 0x000AD641 File Offset: 0x000AB841
	protected virtual void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this._colorFunc = new Action<Color>(this.UpdateColor);
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x000AD662 File Offset: 0x000AB862
	protected virtual void Start()
	{
		this.UpdateColor(this.rig.playerColor);
		this.rig.OnColorInitialized(this._colorFunc);
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x000AD686 File Offset: 0x000AB886
	protected virtual void OnEnable()
	{
		this.rig.OnColorChanged += this._colorFunc;
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x000AD699 File Offset: 0x000AB899
	protected virtual void OnDisable()
	{
		this.rig.OnColorChanged -= this._colorFunc;
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x000AD6AC File Offset: 0x000AB8AC
	public virtual void UpdateColor(Color color)
	{
		if (!this.target)
		{
			return;
		}
		if (this.colorPropertiesToSync == null)
		{
			return;
		}
		for (int i = 0; i < this.colorPropertiesToSync.Length; i++)
		{
			ShaderHashId h = this.colorPropertiesToSync[i];
			this.target.SetColor(h, color);
		}
	}

	// Token: 0x04002B40 RID: 11072
	public VRRig rig;

	// Token: 0x04002B41 RID: 11073
	public Material target;

	// Token: 0x04002B42 RID: 11074
	public ShaderHashId[] colorPropertiesToSync = new ShaderHashId[]
	{
		"_BaseColor"
	};

	// Token: 0x04002B43 RID: 11075
	private Action<Color> _colorFunc;
}
