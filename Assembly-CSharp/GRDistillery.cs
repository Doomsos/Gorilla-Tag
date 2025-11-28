using System;
using System.Globalization;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200069A RID: 1690
public class GRDistillery : MonoBehaviour
{
	// Token: 0x06002B2F RID: 11055 RVA: 0x000E7978 File Offset: 0x000E5B78
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
		this.sentientCoreDeposit.Init(reactor);
		this.cores = PlayerPrefs.GetInt("_grDistilleryCore", -1);
		if (this.cores == -1)
		{
			this.cores = 0;
		}
		this.RestoreStartTime();
		this.InitializeGauges();
	}

	// Token: 0x06002B30 RID: 11056 RVA: 0x000E79C8 File Offset: 0x000E5BC8
	private void SaveStartTime(DateTime time)
	{
		string text = time.ToString("O");
		PlayerPrefs.SetString("_grDistilleryStartTime", text);
		PlayerPrefs.Save();
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x000E79F4 File Offset: 0x000E5BF4
	private void RestoreStartTime()
	{
		string @string = PlayerPrefs.GetString("_grDistilleryStartTime", string.Empty);
		if (@string != string.Empty)
		{
			this.startTime = DateTime.ParseExact(@string, "O", CultureInfo.InvariantCulture, 128);
		}
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x000E7A39 File Offset: 0x000E5C39
	public void StartResearch()
	{
		if (this.cores > 0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime();
			this.SaveStartTime(this.startTime);
			this.bProcessing = true;
			this.InitializeGauges();
		}
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x000E7A70 File Offset: 0x000E5C70
	public double CalculateRemaining()
	{
		return (double)this.secondsToResearchACore - (GorillaComputer.instance.GetServerTime() - this.startTime).TotalSeconds;
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x000E7AA4 File Offset: 0x000E5CA4
	private void FirstUpdate()
	{
		double num = this.CalculateRemaining();
		while (this.cores > 0 && num < (double)(-(double)this.secondsToResearchACore))
		{
			if (num < (double)(-(double)this.secondsToResearchACore))
			{
				this.CompleteResearchingCore();
				num += (double)this.secondsToResearchACore;
			}
		}
		if (this.cores > 0 && num < 0.0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime().AddSeconds(num);
			num = this.CalculateRemaining();
			this.SaveStartTime(this.startTime);
		}
		if (this.cores > 0)
		{
			this.bProcessing = true;
			this.currentGaugeCore = this.cores - 1;
		}
		else
		{
			this.currentGaugeCore = 0;
		}
		if (this.cores >= 4)
		{
			this.depositDoor.transform.position = this.depositClosePosition.position;
		}
		else
		{
			this.depositDoor.transform.position = this.depositOpenPosition.position;
		}
		this.UpdateGauges();
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x000E7B9C File Offset: 0x000E5D9C
	public void Update()
	{
		if (!this.firstUpdate)
		{
			this.FirstUpdate();
			this.firstUpdate = true;
		}
		this.UpdateDoorPosition();
		this.UpdateGauges();
		if (!this.bProcessing)
		{
			return;
		}
		this.remaingTime = this.CalculateRemaining();
		if (this.remaingTime <= 0.0)
		{
			this.CompleteResearchingCore();
		}
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x000E7BF8 File Offset: 0x000E5DF8
	private void UpdateDoorPosition()
	{
		if (this.cores >= 4)
		{
			this.depositDoor.transform.position = Vector3.MoveTowards(this.depositDoor.transform.position, this.depositClosePosition.transform.position, this.depositDoorCloseSpeed * Time.deltaTime);
			return;
		}
		this.depositDoor.transform.position = Vector3.MoveTowards(this.depositDoor.transform.position, this.depositOpenPosition.transform.position, this.depositDoorCloseSpeed * Time.deltaTime);
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x000E7C94 File Offset: 0x000E5E94
	private void CompleteResearchingCore()
	{
		this.cores = Math.Max(this.cores - 1, 0);
		this.currentGaugeCore = Math.Max(this.cores - 1, 0);
		PlayerPrefs.SetInt("_grDistilleryCore", this.cores);
		PlayerPrefs.Save();
		if (this.cores > 0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime().AddSeconds(this.remaingTime);
			this.SaveStartTime(this.startTime);
			this.remaingTime = this.CalculateRemaining();
		}
		if (this.cores == 0)
		{
			this.bProcessing = false;
		}
		this.UpdateGauges();
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x000E7D34 File Offset: 0x000E5F34
	public void DepositCore()
	{
		if (this.cores < this.maxCores)
		{
			this.cores++;
			if (!this.bFillingGauge)
			{
				this.bFillingGauge = true;
				this.fillTime = 0f;
			}
			PlayerPrefs.SetInt("_grDistilleryCore", this.cores);
			PlayerPrefs.Save();
			if (this.cores == 1)
			{
				this.StartResearch();
			}
		}
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x00002789 File Offset: 0x00000989
	public void DebugFinishDistill()
	{
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x000E7D9C File Offset: 0x000E5F9C
	private void OnEnable()
	{
		if (this._applyMaterialgauge1)
		{
			this._applyMaterialgauge1.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge2)
		{
			this._applyMaterialgauge2.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge3)
		{
			this._applyMaterialgauge3.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge4)
		{
			this._applyMaterialgauge4.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		this.InitializeGauges();
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x000E7E14 File Offset: 0x000E6014
	private void InitializeGauges()
	{
		for (int i = 0; i < this.gaugesFill.Length - 1; i++)
		{
			this.gaugesFill[i] = ((this.cores >= i + 1) ? this.gaugeFullFillAmount : this.gaugeEmptyFillAmount);
		}
		this.researchGaugeFill = this.gaugesFill[0];
		this.currentGaugeFillAmount = this.gaugeEmptyFillAmount;
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x000E7E74 File Offset: 0x000E6074
	private void UpdateGauges()
	{
		for (int i = 0; i < this.gaugesFill.Length; i++)
		{
			if (i + 1 > this.cores)
			{
				this.gaugesFill[i] = this.gaugeEmptyFillAmount;
			}
		}
		if (this.bFillingGauge)
		{
			this.fillTime += Time.deltaTime;
			float num = this.fillTime / this.gaugeDrainTime;
			if (this.currentGaugeCore == this.cores - 1)
			{
				if (num > 1f)
				{
					this.bFillingGauge = false;
				}
				else
				{
					this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.currentGaugeFillAmount, Mathf.Lerp(this.gaugeEmptyFillAmount, this.gaugeFullFillAmount, (float)this.remaingTime / (float)this.secondsToResearchACore), num);
				}
			}
			else
			{
				this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.currentGaugeFillAmount, this.gaugeFullFillAmount, num);
			}
			if (this.bFillingGauge && num > 1f)
			{
				this.currentGaugeCore++;
				this.currentGaugeFillAmount = this.gaugeEmptyFillAmount;
				this.fillTime = 0f;
			}
		}
		else if (this.bProcessing)
		{
			this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.gaugeEmptyFillAmount, this.gaugeFullFillAmount, (float)this.remaingTime / (float)this.secondsToResearchACore);
			this.currentGaugeFillAmount = this.gaugesFill[this.currentGaugeCore];
		}
		this._applyMaterialgauge1.SetFloat("_LiquidFill", this.gaugesFill[0]);
		this._applyMaterialgauge1.Apply();
		this._applyMaterialgauge2.SetFloat("_LiquidFill", this.gaugesFill[1]);
		this._applyMaterialgauge2.Apply();
		this._applyMaterialgauge3.SetFloat("_LiquidFill", this.gaugesFill[2]);
		this._applyMaterialgauge3.Apply();
		this._applyMaterialgauge4.SetFloat("_LiquidFill", this.gaugesFill[3]);
		this._applyMaterialgauge4.Apply();
		this._applyMaterialCurrentResearch.SetFloat("_LiquidFill", this.researchGaugeFill);
		this._applyMaterialCurrentResearch.Apply();
	}

	// Token: 0x04003792 RID: 14226
	[SerializeField]
	private GRCurrencyDepositor sentientCoreDeposit;

	// Token: 0x04003793 RID: 14227
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge1;

	// Token: 0x04003794 RID: 14228
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge2;

	// Token: 0x04003795 RID: 14229
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge3;

	// Token: 0x04003796 RID: 14230
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge4;

	// Token: 0x04003797 RID: 14231
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialCurrentResearch;

	// Token: 0x04003798 RID: 14232
	[FormerlySerializedAs("emptyFillAmount")]
	public float gaugeEmptyFillAmount = 0.44f;

	// Token: 0x04003799 RID: 14233
	[FormerlySerializedAs("fullFillAmount")]
	public float gaugeFullFillAmount = 0.56f;

	// Token: 0x0400379A RID: 14234
	[SerializeField]
	private Transform depositClosePosition;

	// Token: 0x0400379B RID: 14235
	[SerializeField]
	private Transform depositOpenPosition;

	// Token: 0x0400379C RID: 14236
	[SerializeField]
	private GameObject depositDoor;

	// Token: 0x0400379D RID: 14237
	[SerializeField]
	private float depositDoorCloseSpeed = 0.5f;

	// Token: 0x0400379E RID: 14238
	[SerializeField]
	private TextMeshPro currentResearchPoints;

	// Token: 0x0400379F RID: 14239
	public float researchGaugeEmptyFillAmount = 0.44f;

	// Token: 0x040037A0 RID: 14240
	public float researchGaugeFullFillAmount = 0.56f;

	// Token: 0x040037A1 RID: 14241
	public int secondsToResearchACore;

	// Token: 0x040037A2 RID: 14242
	public float gaugeDrainTime = 2f;

	// Token: 0x040037A3 RID: 14243
	public int maxCores = 4;

	// Token: 0x040037A4 RID: 14244
	public AudioSource feedbackSound;

	// Token: 0x040037A5 RID: 14245
	private DateTime startTime;

	// Token: 0x040037A6 RID: 14246
	private bool bProcessing;

	// Token: 0x040037A7 RID: 14247
	private int cores;

	// Token: 0x040037A8 RID: 14248
	private bool bFillingGauge;

	// Token: 0x040037A9 RID: 14249
	private int currentGaugeCore;

	// Token: 0x040037AA RID: 14250
	private float currentGaugeFillAmount;

	// Token: 0x040037AB RID: 14251
	private double remaingTime;

	// Token: 0x040037AC RID: 14252
	private float fillTime;

	// Token: 0x040037AD RID: 14253
	private float[] gaugesFill = new float[4];

	// Token: 0x040037AE RID: 14254
	private float researchGaugeFill;

	// Token: 0x040037AF RID: 14255
	private bool firstUpdate;

	// Token: 0x040037B0 RID: 14256
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x040037B1 RID: 14257
	private const string grDistilleryCorePrefsKey = "_grDistilleryCore";

	// Token: 0x040037B2 RID: 14258
	private const string grDistilleryStartTimePrefsKey = "_grDistilleryStartTime";
}
