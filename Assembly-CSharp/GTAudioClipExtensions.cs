using System;
using UnityEngine;

// Token: 0x020002B5 RID: 693
public static class GTAudioClipExtensions
{
	// Token: 0x0600112C RID: 4396 RVA: 0x0005BD50 File Offset: 0x00059F50
	public static float GetPeakMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = float.NegativeInfinity;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float num2 in array)
		{
			num = Mathf.Max(num, Mathf.Abs(num2));
		}
		return num;
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0005BDAC File Offset: 0x00059FAC
	public static float GetRMSMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = 0f;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float num2 in array)
		{
			num += num2 * num2;
		}
		return Mathf.Sqrt(num / (float)array.Length);
	}
}
