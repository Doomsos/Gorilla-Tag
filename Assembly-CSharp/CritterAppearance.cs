using System;
using GorillaExtensions;
using Photon.Pun;

// Token: 0x02000078 RID: 120
public struct CritterAppearance
{
	// Token: 0x060002F0 RID: 752 RVA: 0x00012663 File Offset: 0x00010863
	public CritterAppearance(string hatName, float size = 1f)
	{
		this.hatName = hatName;
		this.size = size;
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00012674 File Offset: 0x00010874
	public object[] WriteToRPCData()
	{
		object[] array = new object[]
		{
			this.hatName,
			this.size
		};
		if (this.hatName == null)
		{
			array[0] = string.Empty;
		}
		if (this.size != 0f)
		{
			array[1] = this.size;
		}
		return array;
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x000126CB File Offset: 0x000108CB
	public static int DataLength()
	{
		return 2;
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x000126D0 File Offset: 0x000108D0
	public static bool ValidateData(object[] data)
	{
		float num;
		return data != null && data.Length == CritterAppearance.DataLength() && CrittersManager.ValidateDataType<float>(data[1], out num) && num >= 0f && !float.IsNaN(num) && !float.IsInfinity(num);
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00012718 File Offset: 0x00010918
	public static CritterAppearance ReadFromRPCData(object[] data)
	{
		string text;
		if (!CrittersManager.ValidateDataType<string>(data[0], out text))
		{
			return new CritterAppearance(string.Empty, 1f);
		}
		float value;
		if (!CrittersManager.ValidateDataType<float>(data[1], out value))
		{
			return new CritterAppearance(string.Empty, 1f);
		}
		return new CritterAppearance((string)data[0], value.GetFinite());
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00012770 File Offset: 0x00010970
	public static CritterAppearance ReadFromPhotonStream(PhotonStream data)
	{
		string text = (string)data.ReceiveNext();
		float num = (float)data.ReceiveNext();
		return new CritterAppearance(text, num);
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x0001279A File Offset: 0x0001099A
	public override string ToString()
	{
		return string.Format("Size: {0} Hat: {1}", this.size, this.hatName);
	}

	// Token: 0x04000393 RID: 915
	public float size;

	// Token: 0x04000394 RID: 916
	public string hatName;
}
