using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag
{
	[Serializable]
	public class CoolDownHelper
	{
		public CoolDownHelper()
		{
			this.coolDown = 1f;
			this.checkTime = 0f;
		}

		public CoolDownHelper(float cd)
		{
			this.coolDown = cd;
			this.checkTime = 0f;
		}

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

		public virtual void Start()
		{
			this.checkTime = Time.unscaledTime + this.coolDown;
		}

		public virtual void Stop()
		{
			this.checkTime = float.MaxValue;
		}

		[MethodImpl(256)]
		public virtual void OnCheckPass()
		{
		}

		public float coolDown;

		[NonSerialized]
		public float checkTime;
	}
}
