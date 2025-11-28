using System;
using UnityEngine;

// Token: 0x02000BD6 RID: 3030
[Serializable]
public class CallLimiterWithCooldown : CallLimiter
{
	// Token: 0x06004AFE RID: 19198 RVA: 0x001883B9 File Offset: 0x001865B9
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown) : base(historyLength, coolDown, 0.5f)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06004AFF RID: 19199 RVA: 0x001883CF File Offset: 0x001865CF
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown, float latencyMax) : base(historyLength, coolDown, latencyMax)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06004B00 RID: 19200 RVA: 0x001883E2 File Offset: 0x001865E2
	public override bool CheckCallTime(float time)
	{
		if (this.blockCall && time < this.blockStartTime + this.spamCoolDown)
		{
			this.blockStartTime = time;
			return false;
		}
		return base.CheckCallTime(time);
	}

	// Token: 0x04005B29 RID: 23337
	[SerializeField]
	private float spamCoolDown;
}
