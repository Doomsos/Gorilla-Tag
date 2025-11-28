using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200100C RID: 4108
	[Serializable]
	public class CoolDownHelper
	{
		// Token: 0x060067F5 RID: 26613 RVA: 0x0021F10B File Offset: 0x0021D30B
		public CoolDownHelper()
		{
			this.coolDown = 1f;
			this.checkTime = 0f;
		}

		// Token: 0x060067F6 RID: 26614 RVA: 0x0021F129 File Offset: 0x0021D329
		public CoolDownHelper(float cd)
		{
			this.coolDown = cd;
			this.checkTime = 0f;
		}

		// Token: 0x060067F7 RID: 26615 RVA: 0x0021F144 File Offset: 0x0021D344
		[MethodImpl(256)]
		public bool CheckCooldown()
		{
			float unscaledTime = Time.unscaledTime;
			if (unscaledTime < this.checkTime)
			{
				return false;
			}
			this.OnCheckPass();
			this.checkTime = unscaledTime + this.coolDown;
			return true;
		}

		// Token: 0x060067F8 RID: 26616 RVA: 0x0021F177 File Offset: 0x0021D377
		public virtual void Start()
		{
			this.checkTime = Time.unscaledTime + this.coolDown;
		}

		// Token: 0x060067F9 RID: 26617 RVA: 0x0021F18B File Offset: 0x0021D38B
		public virtual void Stop()
		{
			this.checkTime = float.MaxValue;
		}

		// Token: 0x060067FA RID: 26618 RVA: 0x00002789 File Offset: 0x00000989
		[MethodImpl(256)]
		public virtual void OnCheckPass()
		{
		}

		// Token: 0x040076E5 RID: 30437
		public float coolDown;

		// Token: 0x040076E6 RID: 30438
		[NonSerialized]
		public float checkTime;
	}
}
