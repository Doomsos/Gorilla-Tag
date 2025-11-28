using System;
using UnityEngine;

// Token: 0x02000843 RID: 2115
[Serializable]
public class RankedMultiplayerStatisticInt : RankedMultiplayerStatistic
{
	// Token: 0x06003798 RID: 14232 RVA: 0x0012B5A9 File Offset: 0x001297A9
	public RankedMultiplayerStatisticInt(string n, int val, int min = 0, int max = 2147483647, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.intValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x06003799 RID: 14233 RVA: 0x0012B5CA File Offset: 0x001297CA
	public static implicit operator int(RankedMultiplayerStatisticInt stat)
	{
		if (stat.IsValid)
		{
			return stat.intValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0;
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x0012B5F1 File Offset: 0x001297F1
	public void Set(int val)
	{
		this.intValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x0012B611 File Offset: 0x00129811
	public int Get()
	{
		return this.intValue;
	}

	// Token: 0x0600379C RID: 14236 RVA: 0x0012B61C File Offset: 0x0012981C
	public override bool TrySetValue(string valAsString)
	{
		int num;
		bool flag = int.TryParse(valAsString, ref num);
		if (flag)
		{
			this.intValue = Mathf.Clamp(num, this.minValue, this.maxValue);
		}
		return flag;
	}

	// Token: 0x0600379D RID: 14237 RVA: 0x0012B64C File Offset: 0x0012984C
	public void Increment()
	{
		this.AddTo(1);
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x0012B655 File Offset: 0x00129855
	public void AddTo(int amount)
	{
		this.intValue += amount;
		this.intValue = Mathf.Clamp(this.intValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x0012B688 File Offset: 0x00129888
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetInt(this.name, this.intValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x0012B6BC File Offset: 0x001298BC
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.intValue = PlayerPrefs.GetInt(this.name, this.intValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x060037A1 RID: 14241 RVA: 0x0012B6FE File Offset: 0x001298FE
	public override string ToString()
	{
		return this.intValue.ToString();
	}

	// Token: 0x040046F7 RID: 18167
	private int intValue;

	// Token: 0x040046F8 RID: 18168
	private int minValue;

	// Token: 0x040046F9 RID: 18169
	private int maxValue;
}
