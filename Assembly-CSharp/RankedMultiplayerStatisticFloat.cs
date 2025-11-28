using System;
using UnityEngine;

// Token: 0x02000844 RID: 2116
[Serializable]
public class RankedMultiplayerStatisticFloat : RankedMultiplayerStatistic
{
	// Token: 0x060037A2 RID: 14242 RVA: 0x0012B70B File Offset: 0x0012990B
	public RankedMultiplayerStatisticFloat(string n, float val, float min = 0f, float max = 3.4028235E+38f, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.floatValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x060037A3 RID: 14243 RVA: 0x0012B72C File Offset: 0x0012992C
	public static implicit operator float(RankedMultiplayerStatisticFloat stat)
	{
		if (stat.IsValid)
		{
			return stat.floatValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0f;
	}

	// Token: 0x060037A4 RID: 14244 RVA: 0x0012B757 File Offset: 0x00129957
	public void Set(float val)
	{
		this.floatValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x060037A5 RID: 14245 RVA: 0x0012B777 File Offset: 0x00129977
	public float Get()
	{
		return this.floatValue;
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x0012B780 File Offset: 0x00129980
	public override bool TrySetValue(string valAsString)
	{
		float num;
		bool flag = float.TryParse(valAsString, ref num);
		if (flag)
		{
			this.floatValue = Mathf.Clamp(num, this.minValue, this.maxValue);
		}
		return flag;
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x0012B7B0 File Offset: 0x001299B0
	public void Increment()
	{
		this.AddTo(1f);
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x0012B7BD File Offset: 0x001299BD
	public void AddTo(float amount)
	{
		this.floatValue += amount;
		this.floatValue = Mathf.Clamp(this.floatValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x060037A9 RID: 14249 RVA: 0x0012B7F0 File Offset: 0x001299F0
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetFloat(this.name, this.floatValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x060037AA RID: 14250 RVA: 0x0012B824 File Offset: 0x00129A24
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.floatValue = PlayerPrefs.GetFloat(this.name, this.floatValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x060037AB RID: 14251 RVA: 0x0012B866 File Offset: 0x00129A66
	public override string ToString()
	{
		return this.floatValue.ToString();
	}

	// Token: 0x040046FA RID: 18170
	private float floatValue;

	// Token: 0x040046FB RID: 18171
	private float minValue;

	// Token: 0x040046FC RID: 18172
	private float maxValue;
}
