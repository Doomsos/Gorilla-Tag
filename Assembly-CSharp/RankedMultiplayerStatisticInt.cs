using System;
using UnityEngine;

// Token: 0x02000843 RID: 2115
[Serializable]
public class RankedMultiplayerStatisticInt : RankedMultiplayerStatistic
{
	// Token: 0x06003798 RID: 14232 RVA: 0x0012B5C9 File Offset: 0x001297C9
	public RankedMultiplayerStatisticInt(string n, int val, int min = 0, int max = 2147483647, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.intValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x06003799 RID: 14233 RVA: 0x0012B5EA File Offset: 0x001297EA
	public static implicit operator int(RankedMultiplayerStatisticInt stat)
	{
		if (stat.IsValid)
		{
			return stat.intValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0;
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x0012B611 File Offset: 0x00129811
	public void Set(int val)
	{
		this.intValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x0012B631 File Offset: 0x00129831
	public int Get()
	{
		return this.intValue;
	}

	// Token: 0x0600379C RID: 14236 RVA: 0x0012B63C File Offset: 0x0012983C
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

	// Token: 0x0600379D RID: 14237 RVA: 0x0012B66C File Offset: 0x0012986C
	public void Increment()
	{
		this.AddTo(1);
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x0012B675 File Offset: 0x00129875
	public void AddTo(int amount)
	{
		this.intValue += amount;
		this.intValue = Mathf.Clamp(this.intValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x0012B6A8 File Offset: 0x001298A8
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetInt(this.name, this.intValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x0012B6DC File Offset: 0x001298DC
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

	// Token: 0x060037A1 RID: 14241 RVA: 0x0012B71E File Offset: 0x0012991E
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
