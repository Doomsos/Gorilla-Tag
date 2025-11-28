using System;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000599 RID: 1433
public class BuilderResourceMeter : MonoBehaviour
{
	// Token: 0x06002434 RID: 9268 RVA: 0x000C2008 File Offset: 0x000C0208
	private void Awake()
	{
		this.fillColor = this.resourceColors.GetColorForType(this._resourceType);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		this.fillCube.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor(ShaderProps._BaseColor, this.fillColor);
		this.fillCube.SetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor(ShaderProps._BaseColor, this.emptyColor);
		this.emptyCube.SetPropertyBlock(materialPropertyBlock);
		this.fillAmount = this.fillTarget;
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x000C2084 File Offset: 0x000C0284
	private void Start()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x000C20B2 File Offset: 0x000C02B2
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x000C20E8 File Offset: 0x000C02E8
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag != this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			if (!flag)
			{
				this.fillCube.enabled = false;
				this.emptyCube.enabled = false;
				return;
			}
			this.fillCube.enabled = true;
			this.emptyCube.enabled = true;
			this.OnAvailableResourcesChange();
		}
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000C214C File Offset: 0x000C034C
	public void OnAvailableResourcesChange()
	{
		if (this.table == null || this.table.maxResources == null)
		{
			return;
		}
		this.resourceMax = this.table.maxResources[(int)this._resourceType];
		int num = this.table.usedResources[(int)this._resourceType];
		if (num != this.usedResource)
		{
			this.usedResource = num;
			this.SetNormalizedFillTarget((float)(this.resourceMax - this.usedResource) / (float)this.resourceMax);
		}
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x000C21CC File Offset: 0x000C03CC
	public void UpdateMeterFill()
	{
		if (this.animatingMeter)
		{
			float newFill = Mathf.MoveTowards(this.fillAmount, this.fillTarget, this.lerpSpeed * Time.deltaTime);
			this.UpdateFill(newFill);
		}
	}

	// Token: 0x0600243A RID: 9274 RVA: 0x000C2208 File Offset: 0x000C0408
	private void UpdateFill(float newFill)
	{
		this.fillAmount = newFill;
		if (Mathf.Approximately(this.fillAmount, this.fillTarget))
		{
			this.fillAmount = this.fillTarget;
			this.animatingMeter = false;
		}
		if (!this.inBuilderZone)
		{
			return;
		}
		if (this.fillAmount <= 1E-45f)
		{
			this.fillCube.enabled = false;
			float num = this.meterHeight / this.meshHeight;
			Vector3 localScale;
			localScale..ctor(this.emptyCube.transform.localScale.x, num, this.emptyCube.transform.localScale.z);
			Vector3 localPosition;
			localPosition..ctor(0f, this.meterHeight / 2f, 0f);
			this.emptyCube.transform.localScale = localScale;
			this.emptyCube.transform.localPosition = localPosition;
			this.emptyCube.enabled = true;
			return;
		}
		if (this.fillAmount >= 1f)
		{
			float num2 = this.meterHeight / this.meshHeight;
			Vector3 localScale2;
			localScale2..ctor(this.fillCube.transform.localScale.x, num2, this.fillCube.transform.localScale.z);
			Vector3 localPosition2;
			localPosition2..ctor(0f, this.meterHeight / 2f, 0f);
			this.fillCube.transform.localScale = localScale2;
			this.fillCube.transform.localPosition = localPosition2;
			this.fillCube.enabled = true;
			this.emptyCube.enabled = false;
			return;
		}
		float num3 = this.meterHeight / this.meshHeight * this.fillAmount;
		Vector3 localScale3;
		localScale3..ctor(this.fillCube.transform.localScale.x, num3, this.fillCube.transform.localScale.z);
		Vector3 localPosition3;
		localPosition3..ctor(0f, num3 * this.meshHeight / 2f, 0f);
		this.fillCube.transform.localScale = localScale3;
		this.fillCube.transform.localPosition = localPosition3;
		this.fillCube.enabled = true;
		float num4 = this.meterHeight / this.meshHeight * (1f - this.fillAmount);
		Vector3 localScale4;
		localScale4..ctor(this.emptyCube.transform.localScale.x, num4, this.emptyCube.transform.localScale.z);
		Vector3 localPosition4;
		localPosition4..ctor(0f, this.meterHeight - num4 * this.meshHeight / 2f, 0f);
		this.emptyCube.transform.localScale = localScale4;
		this.emptyCube.transform.localPosition = localPosition4;
		this.emptyCube.enabled = true;
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x000C24DC File Offset: 0x000C06DC
	public void SetNormalizedFillTarget(float fill)
	{
		this.fillTarget = Mathf.Clamp(fill, 0f, 1f);
		this.animatingMeter = true;
	}

	// Token: 0x04002F8D RID: 12173
	public BuilderResourceColors resourceColors;

	// Token: 0x04002F8E RID: 12174
	public MeshRenderer fillCube;

	// Token: 0x04002F8F RID: 12175
	public MeshRenderer emptyCube;

	// Token: 0x04002F90 RID: 12176
	private Color fillColor = Color.white;

	// Token: 0x04002F91 RID: 12177
	public Color emptyColor = Color.black;

	// Token: 0x04002F92 RID: 12178
	[FormerlySerializedAs("MeterHeight")]
	public float meterHeight = 2f;

	// Token: 0x04002F93 RID: 12179
	public float meshHeight = 1f;

	// Token: 0x04002F94 RID: 12180
	public BuilderResourceType _resourceType;

	// Token: 0x04002F95 RID: 12181
	private float fillAmount;

	// Token: 0x04002F96 RID: 12182
	[Range(0f, 1f)]
	[SerializeField]
	private float fillTarget;

	// Token: 0x04002F97 RID: 12183
	public float lerpSpeed = 0.5f;

	// Token: 0x04002F98 RID: 12184
	private bool animatingMeter;

	// Token: 0x04002F99 RID: 12185
	private int resourceMax = -1;

	// Token: 0x04002F9A RID: 12186
	private int usedResource = -1;

	// Token: 0x04002F9B RID: 12187
	private bool inBuilderZone;

	// Token: 0x04002F9C RID: 12188
	internal BuilderTable table;
}
