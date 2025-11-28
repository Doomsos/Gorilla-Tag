using System;
using System.Globalization;
using Backtrace.Unity;
using Backtrace.Unity.Model;
using GorillaNetworking;
using PlayFab;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000C27 RID: 3111
public class BacktraceManager : MonoBehaviour
{
	// Token: 0x06004C78 RID: 19576 RVA: 0x0018D6C2 File Offset: 0x0018B8C2
	public virtual void Awake()
	{
		base.GetComponent<BacktraceClient>().BeforeSend = delegate(BacktraceData data)
		{
			if (new Random((uint)(Time.realtimeSinceStartupAsDouble * 1000.0)).NextDouble() > this.backtraceSampleRate)
			{
				return null;
			}
			return data;
		};
	}

	// Token: 0x06004C79 RID: 19577 RVA: 0x0018D6DB File Offset: 0x0018B8DB
	private void Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("BacktraceSampleRate", delegate(string data)
		{
			if (data != null)
			{
				double.TryParse(data.Trim('"'), 511, CultureInfo.InvariantCulture, ref this.backtraceSampleRate);
				Debug.Log(string.Format("Set backtrace sample rate to: {0}", this.backtraceSampleRate));
			}
		}, delegate(PlayFabError e)
		{
			Debug.LogError(string.Format("Error getting Backtrace sample rate: {0}", e));
		}, false);
	}

	// Token: 0x04005C51 RID: 23633
	public double backtraceSampleRate = 0.01;
}
