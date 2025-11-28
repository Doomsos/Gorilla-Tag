using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class GorillaStatusToThermalTemperatureMono : MonoBehaviour, ISpawnable
{
	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06001160 RID: 4448 RVA: 0x0005C382 File Offset: 0x0005A582
	// (set) Token: 0x06001161 RID: 4449 RVA: 0x0005C38A File Offset: 0x0005A58A
	public bool hasRig { get; private set; }

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06001162 RID: 4450 RVA: 0x0005C393 File Offset: 0x0005A593
	public VRRig rig
	{
		get
		{
			return this.m_rig;
		}
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x0005C39C File Offset: 0x0005A59C
	public void SetRig(VRRig newRig)
	{
		if (newRig == this.m_rig)
		{
			return;
		}
		if (this.hasRig)
		{
			VRRig rig = this.m_rig;
			rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		}
		this.m_rig = newRig;
		this.hasRig = (newRig != null);
		if (!this.hasRig || !base.isActiveAndEnabled)
		{
			return;
		}
		VRRig rig2 = this.m_rig;
		rig2.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(rig2.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		this._InitRuntimeArray();
		this._OnMatChanged(-1, this.m_rig.setMatIndex);
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x0005C44B File Offset: 0x0005A64B
	protected void Awake()
	{
		this.hasRig = (this.m_rig != null);
		this._InitRuntimeArray();
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0005C468 File Offset: 0x0005A668
	private void _InitRuntimeArray()
	{
		if (!this.hasRig || this._runtimeMatIndexes_to_temperatures != null)
		{
			return;
		}
		int num = VRRig.LocalRig.materialsToChangeTo.Length;
		this._runtimeMatIndexes_to_temperatures = new float[num];
		for (int i = 0; i < this._runtimeMatIndexes_to_temperatures.Length; i++)
		{
			this._runtimeMatIndexes_to_temperatures[i] = -32768f;
		}
		foreach (GorillaStatusToThermalTemperatureMono._MaterialIndexToTemperature materialIndexToTemperature in this.m_materialIndexesToTemperatures)
		{
			foreach (int num2 in materialIndexToTemperature.matIndexes)
			{
				if (num2 >= 0 && num2 < num)
				{
					this._runtimeMatIndexes_to_temperatures[num2] = materialIndexToTemperature.temperature;
				}
			}
		}
		if (!Application.isEditor)
		{
			this.m_materialIndexesToTemperatures = null;
		}
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0005C528 File Offset: 0x0005A728
	protected void OnEnable()
	{
		if (!this.hasRig || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.m_thermalSourceVolume == null)
		{
			GTDev.LogError<string>("[GorillaStatusToThermalTemperatureMono]  ERROR!!!  Disabling because thermal source is not assigned. Path=" + base.transform.GetPathQ(), this, null);
			base.enabled = false;
			return;
		}
		VRRig rig = this.m_rig;
		rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		this._OnMatChanged(-1, this.m_rig.setMatIndex);
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0005C5B0 File Offset: 0x0005A7B0
	protected void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting || !this.hasRig)
		{
			return;
		}
		VRRig rig = this.m_rig;
		rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x0005C5EC File Offset: 0x0005A7EC
	private void _OnMatChanged(int oldIndex, int newIndex)
	{
		float num = this._runtimeMatIndexes_to_temperatures[newIndex];
		this.m_thermalSourceVolume.celsius = num;
		this.m_thermalSourceVolume.enabled = (num > -32767.99f);
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06001169 RID: 4457 RVA: 0x0005C621 File Offset: 0x0005A821
	// (set) Token: 0x0600116A RID: 4458 RVA: 0x0005C629 File Offset: 0x0005A829
	public bool IsSpawned { get; set; }

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x0600116B RID: 4459 RVA: 0x0005C632 File Offset: 0x0005A832
	// (set) Token: 0x0600116C RID: 4460 RVA: 0x0005C63A File Offset: 0x0005A83A
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x0600116D RID: 4461 RVA: 0x0005C643 File Offset: 0x0005A843
	public void OnSpawn(VRRig newRig)
	{
		this.SetRig(newRig);
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x0005C64C File Offset: 0x0005A84C
	public void OnDespawn()
	{
		this.SetRig(null);
	}

	// Token: 0x040015FA RID: 5626
	private const string preLog = "[GorillaStatusToThermalTemperatureMono]  ";

	// Token: 0x040015FB RID: 5627
	private const string preErr = "[GorillaStatusToThermalTemperatureMono]  ERROR!!!  ";

	// Token: 0x040015FC RID: 5628
	[Tooltip("Should either be assigned here or via another script.")]
	[SerializeField]
	private VRRig m_rig;

	// Token: 0x040015FE RID: 5630
	[SerializeField]
	private ThermalSourceVolume m_thermalSourceVolume;

	// Token: 0x040015FF RID: 5631
	[SerializeField]
	private GorillaStatusToThermalTemperatureMono._MaterialIndexToTemperature[] m_materialIndexesToTemperatures;

	// Token: 0x04001600 RID: 5632
	[DebugReadout]
	private float[] _runtimeMatIndexes_to_temperatures;

	// Token: 0x04001601 RID: 5633
	private const float _k_invalidTemperature = -32768f;

	// Token: 0x020002C5 RID: 709
	[Serializable]
	private struct _MaterialIndexToTemperature
	{
		// Token: 0x04001604 RID: 5636
		public int[] matIndexes;

		// Token: 0x04001605 RID: 5637
		public float temperature;
	}
}
