using System;

// Token: 0x02000841 RID: 2113
public abstract class RankedMultiplayerStatistic
{
	// Token: 0x0600378C RID: 14220 RVA: 0x0004C0FD File Offset: 0x0004A2FD
	public override string ToString()
	{
		return string.Empty;
	}

	// Token: 0x0600378D RID: 14221
	public abstract void Load();

	// Token: 0x0600378E RID: 14222
	protected abstract void Save();

	// Token: 0x0600378F RID: 14223
	public abstract bool TrySetValue(string valAsString);

	// Token: 0x06003790 RID: 14224 RVA: 0x0012B4F7 File Offset: 0x001296F7
	public virtual string WriteToJson()
	{
		return string.Format("{{{0}:\"{1}\"}}", this.name, this.ToString());
	}

	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x06003791 RID: 14225 RVA: 0x0012B50F File Offset: 0x0012970F
	// (set) Token: 0x06003792 RID: 14226 RVA: 0x0012B517 File Offset: 0x00129717
	public bool IsValid { get; protected set; }

	// Token: 0x06003793 RID: 14227 RVA: 0x0012B520 File Offset: 0x00129720
	public RankedMultiplayerStatistic(string n, RankedMultiplayerStatistic.SerializationType sType = RankedMultiplayerStatistic.SerializationType.Mothership)
	{
		this.serializationType = sType;
		this.name = n;
		this.IsValid = (this.serializationType != RankedMultiplayerStatistic.SerializationType.Mothership);
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
	}

	// Token: 0x06003794 RID: 14228 RVA: 0x0012B558 File Offset: 0x00129758
	protected virtual void HandleUserDataSetSuccess(string keyName)
	{
		if (keyName == this.name)
		{
			this.IsValid = true;
		}
	}

	// Token: 0x06003795 RID: 14229 RVA: 0x0012B56F File Offset: 0x0012976F
	protected virtual void HandleUserDataGetSuccess(string keyName, string value)
	{
		if (keyName == this.name)
		{
			if (this.TrySetValue(value))
			{
				this.IsValid = true;
				return;
			}
			this.Save();
		}
	}

	// Token: 0x06003796 RID: 14230 RVA: 0x0012B596 File Offset: 0x00129796
	protected void HandleUserDataGetFailure(string keyName)
	{
		if (keyName == this.name)
		{
			this.Save();
			this.IsValid = true;
		}
	}

	// Token: 0x06003797 RID: 14231 RVA: 0x0012B5B3 File Offset: 0x001297B3
	protected void HandleUserDataSetFailure(string keyName)
	{
		if (keyName == this.name)
		{
			this.Save();
		}
	}

	// Token: 0x040046F0 RID: 18160
	protected RankedMultiplayerStatistic.SerializationType serializationType = RankedMultiplayerStatistic.SerializationType.Mothership;

	// Token: 0x040046F1 RID: 18161
	public string name;

	// Token: 0x02000842 RID: 2114
	public enum SerializationType
	{
		// Token: 0x040046F4 RID: 18164
		None,
		// Token: 0x040046F5 RID: 18165
		Mothership,
		// Token: 0x040046F6 RID: 18166
		PlayerPrefs
	}
}
