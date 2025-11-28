using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008D6 RID: 2262
public class RadialBounds : MonoBehaviour
{
	// Token: 0x17000550 RID: 1360
	// (get) Token: 0x06003A03 RID: 14851 RVA: 0x00132FE6 File Offset: 0x001311E6
	// (set) Token: 0x06003A04 RID: 14852 RVA: 0x00132FEE File Offset: 0x001311EE
	public Vector3 localCenter
	{
		get
		{
			return this._localCenter;
		}
		set
		{
			this._localCenter = value;
		}
	}

	// Token: 0x17000551 RID: 1361
	// (get) Token: 0x06003A05 RID: 14853 RVA: 0x00132FF7 File Offset: 0x001311F7
	// (set) Token: 0x06003A06 RID: 14854 RVA: 0x00132FFF File Offset: 0x001311FF
	public float localRadius
	{
		get
		{
			return this._localRadius;
		}
		set
		{
			this._localRadius = value;
		}
	}

	// Token: 0x17000552 RID: 1362
	// (get) Token: 0x06003A07 RID: 14855 RVA: 0x00133008 File Offset: 0x00131208
	public Vector3 center
	{
		get
		{
			return base.transform.TransformPoint(this._localCenter);
		}
	}

	// Token: 0x17000553 RID: 1363
	// (get) Token: 0x06003A08 RID: 14856 RVA: 0x0013301B File Offset: 0x0013121B
	public float radius
	{
		get
		{
			return MathUtils.GetScaledRadius(this._localRadius, base.transform.lossyScale);
		}
	}

	// Token: 0x04004942 RID: 18754
	[SerializeField]
	private Vector3 _localCenter;

	// Token: 0x04004943 RID: 18755
	[SerializeField]
	private float _localRadius = 1f;

	// Token: 0x04004944 RID: 18756
	[Space]
	public UnityEvent<RadialBounds> onOverlapEnter;

	// Token: 0x04004945 RID: 18757
	public UnityEvent<RadialBounds> onOverlapExit;

	// Token: 0x04004946 RID: 18758
	public UnityEvent<RadialBounds, float> onOverlapStay;
}
