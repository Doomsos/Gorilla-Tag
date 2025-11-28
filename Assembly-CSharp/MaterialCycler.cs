using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200038C RID: 908
public class MaterialCycler : MonoBehaviour
{
	// Token: 0x060015A7 RID: 5543 RVA: 0x00079DAB File Offset: 0x00077FAB
	private void Awake()
	{
		this.materialCyclerNetworked = base.GetComponent<MaterialCyclerNetworked>();
		this.SetMaterials();
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x00079DBF File Offset: 0x00077FBF
	private void OnEnable()
	{
		if (this.materialCyclerNetworked != null)
		{
			this.materialCyclerNetworked.OnSynchronize += new Action<int, int3>(this.MaterialCyclerNetworked_OnSynchronize);
		}
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x00079DE6 File Offset: 0x00077FE6
	private void OnDisable()
	{
		if (this.materialCyclerNetworked != null)
		{
			this.materialCyclerNetworked.OnSynchronize -= new Action<int, int3>(this.MaterialCyclerNetworked_OnSynchronize);
		}
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x00079E10 File Offset: 0x00078010
	private void MaterialCyclerNetworked_OnSynchronize(int idx, int3 rgb)
	{
		if (idx < 0 || idx >= this.materials.Length)
		{
			return;
		}
		this.index = idx;
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.renderers[i].material = this.materials[this.index].Materials[i];
			this.renderers[i].material.SetColor(this.setColorTarget, new Color((float)rgb.x / 9f, (float)rgb.y / 9f, (float)rgb.z / 9f));
		}
		this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x00079F04 File Offset: 0x00078104
	private void SetMaterials()
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			if (this.materials[this.index].Materials.Length > i)
			{
				this.renderers[i].material = this.materials[this.index].Materials[i];
			}
			else
			{
				this.renderers[i].material = null;
			}
		}
		this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x00079FC3 File Offset: 0x000781C3
	public void NextMaterial()
	{
		this.index = (this.index + 1) % this.materials.Length;
		this.SetMaterials();
		this.SetDirty();
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x00079FE8 File Offset: 0x000781E8
	private void SetDirty()
	{
		if (this.materialCyclerNetworked == null)
		{
			return;
		}
		this.synchTime = Time.time + this.materialCyclerNetworked.SyncTimeOut;
		if (this.crDirty == null)
		{
			this.crDirty = base.StartCoroutine(this.timeOutDirty());
		}
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x0007A035 File Offset: 0x00078235
	private IEnumerator timeOutDirty()
	{
		while (this.synchTime > Time.time)
		{
			yield return null;
		}
		this.synchronize();
		this.crDirty = null;
		yield break;
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x0007A044 File Offset: 0x00078244
	private void synchronize()
	{
		this.materialCyclerNetworked.Synchronize(this.index, this.renderers[0].material.color);
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x0007A06C File Offset: 0x0007826C
	public void SetColor(Vector3 rgb)
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.renderers[i].material.SetColor(this.setColorTarget, new Color(rgb.x, rgb.y, rgb.z));
		}
		this.SetDirty();
	}

	// Token: 0x04002014 RID: 8212
	[SerializeField]
	private MaterialCycler.MaterialPack[] materials;

	// Token: 0x04002015 RID: 8213
	[SerializeField]
	private Renderer[] renderers;

	// Token: 0x04002016 RID: 8214
	private int index;

	// Token: 0x04002017 RID: 8215
	[SerializeField]
	private string setColorTarget = "_BaseColor";

	// Token: 0x04002018 RID: 8216
	[SerializeField]
	private UnityEvent<Vector3> reset;

	// Token: 0x04002019 RID: 8217
	private Coroutine crDirty;

	// Token: 0x0400201A RID: 8218
	private float synchTime;

	// Token: 0x0400201B RID: 8219
	private MaterialCyclerNetworked materialCyclerNetworked;

	// Token: 0x0200038D RID: 909
	[Serializable]
	private class MaterialPack
	{
		// Token: 0x1700021E RID: 542
		// (get) Token: 0x060015B2 RID: 5554 RVA: 0x0007A0D4 File Offset: 0x000782D4
		public Material[] Materials
		{
			get
			{
				return this.materials;
			}
		}

		// Token: 0x0400201C RID: 8220
		[SerializeField]
		private Material[] materials;
	}
}
