using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009F1 RID: 2545
public class ProximityReactor : MonoBehaviour
{
	// Token: 0x17000603 RID: 1539
	// (get) Token: 0x060040D3 RID: 16595 RVA: 0x0015A745 File Offset: 0x00158945
	public float proximityRange
	{
		get
		{
			return this.proximityMax - this.proximityMin;
		}
	}

	// Token: 0x17000604 RID: 1540
	// (get) Token: 0x060040D4 RID: 16596 RVA: 0x0015A754 File Offset: 0x00158954
	public float distance
	{
		get
		{
			return this._distance;
		}
	}

	// Token: 0x17000605 RID: 1541
	// (get) Token: 0x060040D5 RID: 16597 RVA: 0x0015A75C File Offset: 0x0015895C
	public float distanceLinear
	{
		get
		{
			return this._distanceLinear;
		}
	}

	// Token: 0x060040D6 RID: 16598 RVA: 0x0015A764 File Offset: 0x00158964
	public void SetRigFrom()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.from = componentInParent.transform;
		}
	}

	// Token: 0x060040D7 RID: 16599 RVA: 0x0015A790 File Offset: 0x00158990
	public void SetRigTo()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.to = componentInParent.transform;
		}
	}

	// Token: 0x060040D8 RID: 16600 RVA: 0x0015A7BA File Offset: 0x001589BA
	public void SetTransformFrom(Transform t)
	{
		this.from = t;
	}

	// Token: 0x060040D9 RID: 16601 RVA: 0x0015A7C3 File Offset: 0x001589C3
	public void SetTransformTo(Transform t)
	{
		this.to = t;
	}

	// Token: 0x060040DA RID: 16602 RVA: 0x0015A7CC File Offset: 0x001589CC
	private void Setup()
	{
		this._distance = 0f;
		this._distanceLinear = 0f;
	}

	// Token: 0x060040DB RID: 16603 RVA: 0x0015A7E4 File Offset: 0x001589E4
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x060040DC RID: 16604 RVA: 0x0015A7EC File Offset: 0x001589EC
	private void Update()
	{
		if (!this.from || !this.to)
		{
			this._distance = 0f;
			this._distanceLinear = 0f;
			return;
		}
		Vector3 position = this.from.position;
		float magnitude = (this.to.position - position).magnitude;
		if (!this._distance.Approx(magnitude, 1E-06f))
		{
			UnityEvent<float> unityEvent = this.onProximityChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(magnitude);
			}
		}
		this._distance = magnitude;
		float num = this.proximityRange.Approx0(1E-06f) ? 0f : MathUtils.LinearUnclamped(magnitude, this.proximityMin, this.proximityMax, 0f, 1f);
		if (!this._distanceLinear.Approx(num, 1E-06f))
		{
			UnityEvent<float> unityEvent2 = this.onProximityChangedLinear;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(num);
			}
		}
		this._distanceLinear = num;
		if (this._distanceLinear < 0f)
		{
			UnityEvent<float> unityEvent3 = this.onBelowMinProximity;
			if (unityEvent3 != null)
			{
				unityEvent3.Invoke(magnitude);
			}
		}
		if (this._distanceLinear > 1f)
		{
			UnityEvent<float> unityEvent4 = this.onAboveMaxProximity;
			if (unityEvent4 == null)
			{
				return;
			}
			unityEvent4.Invoke(magnitude);
		}
	}

	// Token: 0x04005209 RID: 21001
	public Transform from;

	// Token: 0x0400520A RID: 21002
	public Transform to;

	// Token: 0x0400520B RID: 21003
	[Space]
	public float proximityMin;

	// Token: 0x0400520C RID: 21004
	public float proximityMax = 1f;

	// Token: 0x0400520D RID: 21005
	[Space]
	[NonSerialized]
	private float _distance;

	// Token: 0x0400520E RID: 21006
	[NonSerialized]
	private float _distanceLinear;

	// Token: 0x0400520F RID: 21007
	[Space]
	public UnityEvent<float> onProximityChanged;

	// Token: 0x04005210 RID: 21008
	public UnityEvent<float> onProximityChangedLinear;

	// Token: 0x04005211 RID: 21009
	[Space]
	public UnityEvent<float> onBelowMinProximity;

	// Token: 0x04005212 RID: 21010
	public UnityEvent<float> onAboveMaxProximity;
}
