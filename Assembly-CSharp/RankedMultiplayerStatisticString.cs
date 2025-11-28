using System;
using UnityEngine;

// Token: 0x02000845 RID: 2117
[Serializable]
public class RankedMultiplayerStatisticString : RankedMultiplayerStatistic
{
	// Token: 0x060037AC RID: 14252 RVA: 0x0012B893 File Offset: 0x00129A93
	public RankedMultiplayerStatisticString(string n, string val, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.stringValue = val;
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x0012B8A4 File Offset: 0x00129AA4
	public static implicit operator string(RankedMultiplayerStatisticString stat)
	{
		if (stat.IsValid)
		{
			return stat.stringValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return string.Empty;
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x0012B8CF File Offset: 0x00129ACF
	public void Set(string val)
	{
		this.stringValue = val;
		this.Save();
	}

	// Token: 0x060037AF RID: 14255 RVA: 0x0012B8DE File Offset: 0x00129ADE
	public string Get()
	{
		return this.stringValue;
	}

	// Token: 0x060037B0 RID: 14256 RVA: 0x0012B8E6 File Offset: 0x00129AE6
	public override bool TrySetValue(string valAsString)
	{
		this.stringValue = valAsString;
		return true;
	}

	// Token: 0x060037B1 RID: 14257 RVA: 0x0012B8F0 File Offset: 0x00129AF0
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetString(this.name, this.stringValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x060037B2 RID: 14258 RVA: 0x0012B924 File Offset: 0x00129B24
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.stringValue = PlayerPrefs.GetString(this.name, this.stringValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x0012B8DE File Offset: 0x00129ADE
	public override string ToString()
	{
		return this.stringValue;
	}

	// Token: 0x040046FD RID: 18173
	private string stringValue;
}
