using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008D7 RID: 2263
public class RadialBoundsTrigger : MonoBehaviour
{
	// Token: 0x06003A0A RID: 14858 RVA: 0x00133066 File Offset: 0x00131266
	public void TestOverlap()
	{
		this.TestOverlap(this._raiseEvents);
	}

	// Token: 0x06003A0B RID: 14859 RVA: 0x00133074 File Offset: 0x00131274
	public void TestOverlap(bool raiseEvents)
	{
		if (!this.object1 || !this.object2)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			return;
		}
		float time = Time.time;
		float num = this.object1.radius + this.object2.radius;
		bool flag = (this.object2.center - this.object1.center).sqrMagnitude <= num * num;
		if (this._overlapping && flag)
		{
			this._overlapping = true;
			this._timeSpentInOverlap = time - this._timeOverlapStarted;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds, float> onOverlapStay = this.object1.onOverlapStay;
				if (onOverlapStay != null)
				{
					onOverlapStay.Invoke(this.object2, this._timeSpentInOverlap);
				}
				UnityEvent<RadialBounds, float> onOverlapStay2 = this.object2.onOverlapStay;
				if (onOverlapStay2 == null)
				{
					return;
				}
				onOverlapStay2.Invoke(this.object1, this._timeSpentInOverlap);
				return;
			}
		}
		else if (!this._overlapping && flag)
		{
			if (time - this._timeOverlapStopped < this.hysteresis)
			{
				return;
			}
			this._overlapping = true;
			this._timeOverlapStarted = time;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapEnter = this.object1.onOverlapEnter;
				if (onOverlapEnter != null)
				{
					onOverlapEnter.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapEnter2 = this.object2.onOverlapEnter;
				if (onOverlapEnter2 == null)
				{
					return;
				}
				onOverlapEnter2.Invoke(this.object1);
				return;
			}
		}
		else if (!flag && this._overlapping)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = time;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
				if (onOverlapExit != null)
				{
					onOverlapExit.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
				if (onOverlapExit2 == null)
				{
					return;
				}
				onOverlapExit2.Invoke(this.object1);
			}
		}
	}

	// Token: 0x06003A0C RID: 14860 RVA: 0x00133260 File Offset: 0x00131460
	private void FixedUpdate()
	{
		this.TestOverlap();
	}

	// Token: 0x06003A0D RID: 14861 RVA: 0x00133268 File Offset: 0x00131468
	private void OnDisable()
	{
		if (this._raiseEvents && this.object1 && this.object2 && this._overlapping)
		{
			UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
			if (onOverlapExit != null)
			{
				onOverlapExit.Invoke(this.object2);
			}
			UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
			if (onOverlapExit2 != null)
			{
				onOverlapExit2.Invoke(this.object1);
			}
		}
		this._timeOverlapStarted = -1f;
		this._timeSpentInOverlap = 0f;
		this._overlapping = false;
	}

	// Token: 0x04004947 RID: 18759
	[SerializeField]
	private Id32 _triggerID;

	// Token: 0x04004948 RID: 18760
	[Space]
	public RadialBounds object1 = new RadialBounds();

	// Token: 0x04004949 RID: 18761
	[Space]
	public RadialBounds object2 = new RadialBounds();

	// Token: 0x0400494A RID: 18762
	[Space]
	public float hysteresis = 0.5f;

	// Token: 0x0400494B RID: 18763
	[SerializeField]
	private bool _raiseEvents = true;

	// Token: 0x0400494C RID: 18764
	[Space]
	private bool _overlapping;

	// Token: 0x0400494D RID: 18765
	private float _timeSpentInOverlap;

	// Token: 0x0400494E RID: 18766
	[Space]
	private float _timeOverlapStarted;

	// Token: 0x0400494F RID: 18767
	private float _timeOverlapStopped;
}
