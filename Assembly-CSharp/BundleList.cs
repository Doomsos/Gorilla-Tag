using System;

// Token: 0x0200045D RID: 1117
internal class BundleList
{
	// Token: 0x06001C59 RID: 7257 RVA: 0x000967F4 File Offset: 0x000949F4
	public void FromJson(string jsonString)
	{
		this.data = JSonHelper.FromJson<BundleData>(jsonString);
		if (this.data.Length == 0)
		{
			return;
		}
		this.activeBundleIdx = 0;
		int majorVersion = this.data[0].majorVersion;
		int minorVersion = this.data[0].minorVersion;
		int minorVersion2 = this.data[0].minorVersion2;
		int gameMajorVersion = NetworkSystemConfig.GameMajorVersion;
		int gameMinorVersion = NetworkSystemConfig.GameMinorVersion;
		int gameMinorVersion2 = NetworkSystemConfig.GameMinorVersion2;
		for (int i = 1; i < this.data.Length; i++)
		{
			this.data[i].isActive = false;
			int num = gameMajorVersion * 1000000 + gameMinorVersion * 1000 + gameMinorVersion2;
			int num2 = this.data[i].majorVersion * 1000000 + this.data[i].minorVersion * 1000 + this.data[i].minorVersion2;
			if (num >= num2 && this.data[i].majorVersion >= majorVersion && this.data[i].minorVersion >= minorVersion && this.data[i].minorVersion2 >= minorVersion2)
			{
				this.activeBundleIdx = i;
				majorVersion = this.data[i].majorVersion;
				minorVersion = this.data[i].minorVersion;
				minorVersion2 = this.data[i].minorVersion2;
				break;
			}
		}
		this.data[this.activeBundleIdx].isActive = true;
	}

	// Token: 0x06001C5A RID: 7258 RVA: 0x00096994 File Offset: 0x00094B94
	public bool HasSku(string skuName, out int idx)
	{
		if (this.data == null)
		{
			idx = -1;
			return false;
		}
		for (int i = 0; i < this.data.Length; i++)
		{
			if (this.data[i].skuName == skuName)
			{
				idx = i;
				return true;
			}
		}
		idx = -1;
		return false;
	}

	// Token: 0x06001C5B RID: 7259 RVA: 0x000969E3 File Offset: 0x00094BE3
	public BundleData ActiveBundle()
	{
		return this.data[this.activeBundleIdx];
	}

	// Token: 0x04002660 RID: 9824
	private int activeBundleIdx;

	// Token: 0x04002661 RID: 9825
	public BundleData[] data;
}
