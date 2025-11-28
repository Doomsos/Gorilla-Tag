using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C25 RID: 3109
public class AverageVector3
{
	// Token: 0x06004C73 RID: 19571 RVA: 0x0018D589 File Offset: 0x0018B789
	public AverageVector3(float averagingWindow = 0.1f)
	{
		this.timeWindow = averagingWindow;
	}

	// Token: 0x06004C74 RID: 19572 RVA: 0x0018D5B0 File Offset: 0x0018B7B0
	public void AddSample(Vector3 sample, float time)
	{
		this.samples.Add(new AverageVector3.Sample
		{
			timeStamp = time,
			value = sample
		});
		this.RefreshSamples();
	}

	// Token: 0x06004C75 RID: 19573 RVA: 0x0018D5E8 File Offset: 0x0018B7E8
	public Vector3 GetAverage()
	{
		this.RefreshSamples();
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.samples.Count; i++)
		{
			vector += this.samples[i].value;
		}
		return vector / (float)this.samples.Count;
	}

	// Token: 0x06004C76 RID: 19574 RVA: 0x0018D643 File Offset: 0x0018B843
	public void Clear()
	{
		this.samples.Clear();
	}

	// Token: 0x06004C77 RID: 19575 RVA: 0x0018D650 File Offset: 0x0018B850
	private void RefreshSamples()
	{
		float num = Time.time - this.timeWindow;
		for (int i = this.samples.Count - 1; i >= 0; i--)
		{
			if (this.samples[i].timeStamp < num)
			{
				this.samples.RemoveAt(i);
			}
		}
	}

	// Token: 0x04005C4D RID: 23629
	private List<AverageVector3.Sample> samples = new List<AverageVector3.Sample>();

	// Token: 0x04005C4E RID: 23630
	private float timeWindow = 0.1f;

	// Token: 0x02000C26 RID: 3110
	public struct Sample
	{
		// Token: 0x04005C4F RID: 23631
		public float timeStamp;

		// Token: 0x04005C50 RID: 23632
		public Vector3 value;
	}
}
