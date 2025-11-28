using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000BD5 RID: 3029
[Serializable]
public class CallLimiter
{
	// Token: 0x06004AF9 RID: 19193 RVA: 0x00002050 File Offset: 0x00000250
	public CallLimiter()
	{
	}

	// Token: 0x06004AFA RID: 19194 RVA: 0x001881EC File Offset: 0x001863EC
	public CallLimiter(int historyLength, float coolDown, float latencyMax = 0.5f)
	{
		this.callTimeHistory = new float[historyLength];
		this.callHistoryLength = historyLength;
		for (int i = 0; i < historyLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.timeCooldown = coolDown;
		this.maxLatency = (double)latencyMax;
	}

	// Token: 0x06004AFB RID: 19195 RVA: 0x0018823C File Offset: 0x0018643C
	public bool CheckCallServerTime(double time)
	{
		double currentTime = PhotonNetwork.CurrentTime;
		double num = this.maxLatency;
		double num2 = 4294967.295 - this.maxLatency;
		double num3;
		if (currentTime > num || time < num)
		{
			if (time > currentTime + 0.05)
			{
				return false;
			}
			num3 = currentTime - time;
		}
		else
		{
			double num4 = num2 + currentTime;
			if (time > currentTime + 0.5 && time < num4)
			{
				return false;
			}
			num3 = currentTime + (4294967.295 - time);
		}
		if (num3 > this.maxLatency)
		{
			return false;
		}
		int num5 = (this.oldTimeIndex > 0) ? (this.oldTimeIndex - 1) : (this.callHistoryLength - 1);
		double num6 = (double)this.callTimeHistory[num5];
		if (num6 > num2 && time < num6)
		{
			this.Reset();
		}
		else if (time < num6)
		{
			return false;
		}
		return this.CheckCallTime((float)time);
	}

	// Token: 0x06004AFC RID: 19196 RVA: 0x00188304 File Offset: 0x00186504
	public virtual bool CheckCallTime(float time)
	{
		if (this.callTimeHistory[this.oldTimeIndex] > time)
		{
			this.blockCall = true;
			this.blockStartTime = time;
			return false;
		}
		this.callTimeHistory[this.oldTimeIndex] = time + this.timeCooldown;
		int num = this.oldTimeIndex + 1;
		this.oldTimeIndex = num;
		this.oldTimeIndex = num % this.callHistoryLength;
		this.blockCall = false;
		return true;
	}

	// Token: 0x06004AFD RID: 19197 RVA: 0x0018836C File Offset: 0x0018656C
	public virtual void Reset()
	{
		if (this.callTimeHistory == null)
		{
			return;
		}
		for (int i = 0; i < this.callHistoryLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.oldTimeIndex = 0;
		this.blockStartTime = 0f;
		this.blockCall = false;
	}

	// Token: 0x04005B21 RID: 23329
	protected const double k_serverMaxTime = 4294967.295;

	// Token: 0x04005B22 RID: 23330
	[SerializeField]
	protected float[] callTimeHistory;

	// Token: 0x04005B23 RID: 23331
	[Space]
	[SerializeField]
	protected int callHistoryLength;

	// Token: 0x04005B24 RID: 23332
	[SerializeField]
	protected float timeCooldown;

	// Token: 0x04005B25 RID: 23333
	[SerializeField]
	protected double maxLatency;

	// Token: 0x04005B26 RID: 23334
	private int oldTimeIndex;

	// Token: 0x04005B27 RID: 23335
	protected bool blockCall;

	// Token: 0x04005B28 RID: 23336
	protected float blockStartTime;
}
