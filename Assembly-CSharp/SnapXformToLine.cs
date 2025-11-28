using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008E4 RID: 2276
public class SnapXformToLine : MonoBehaviour
{
	// Token: 0x17000560 RID: 1376
	// (get) Token: 0x06003A45 RID: 14917 RVA: 0x00133ABB File Offset: 0x00131CBB
	public Vector3 linePoint
	{
		get
		{
			return this._closest;
		}
	}

	// Token: 0x17000561 RID: 1377
	// (get) Token: 0x06003A46 RID: 14918 RVA: 0x00133AC3 File Offset: 0x00131CC3
	public float linearDistance
	{
		get
		{
			return this._linear;
		}
	}

	// Token: 0x06003A47 RID: 14919 RVA: 0x00133ACB File Offset: 0x00131CCB
	public void SnapTarget(bool applyToXform = true)
	{
		this.Snap(this.target, true);
	}

	// Token: 0x06003A48 RID: 14920 RVA: 0x00133ADA File Offset: 0x00131CDA
	public void SnapTarget(Vector3 point)
	{
		if (this.target)
		{
			this.target.position = this.GetSnappedPoint(this.target.position);
		}
	}

	// Token: 0x06003A49 RID: 14921 RVA: 0x00133B08 File Offset: 0x00131D08
	public void SnapTargetLinear(float t)
	{
		if (this.target && this.from && this.to)
		{
			this.target.position = Vector3.Lerp(this.from.position, this.to.position, t);
		}
	}

	// Token: 0x06003A4A RID: 14922 RVA: 0x00133B63 File Offset: 0x00131D63
	public Vector3 GetSnappedPoint(Transform t)
	{
		return this.GetSnappedPoint(t.position);
	}

	// Token: 0x06003A4B RID: 14923 RVA: 0x00133B74 File Offset: 0x00131D74
	public Vector3 GetSnappedPoint(Vector3 point)
	{
		if (!this.apply)
		{
			return point;
		}
		if (!this.from || !this.to)
		{
			return point;
		}
		return SnapXformToLine.GetClosestPointOnLine(point, this.from.position, this.to.position);
	}

	// Token: 0x06003A4C RID: 14924 RVA: 0x00133BC4 File Offset: 0x00131DC4
	public void Snap(Transform xform, bool applyToXform = true)
	{
		if (!this.apply || !xform || !this.from || !this.to)
		{
			return;
		}
		Vector3 position = xform.position;
		Vector3 position2 = this.from.position;
		Vector3 position3 = this.to.position;
		Vector3 closestPointOnLine = SnapXformToLine.GetClosestPointOnLine(position, position2, position3);
		float num = Vector3.Distance(position2, position3);
		float num2 = Vector3.Distance(closestPointOnLine, position2);
		Vector3 closest = this._closest;
		Vector3 vector = closestPointOnLine;
		float linear = this._linear;
		float num3 = Mathf.Approximately(num, 0f) ? 0f : (num2 / (num + Mathf.Epsilon));
		this._closest = vector;
		this._linear = num3;
		if (this.output)
		{
			IRangedVariable<float> asT = this.output.AsT;
			asT.Set(asT.Min + this._linear * asT.Range);
		}
		if (applyToXform)
		{
			xform.position = this._closest;
			if (!Mathf.Approximately(closest.x, vector.x) || !Mathf.Approximately(closest.y, vector.y) || !Mathf.Approximately(closest.z, vector.z))
			{
				UnityEvent<Vector3> unityEvent = this.onPositionChanged;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this._closest);
				}
			}
			if (!Mathf.Approximately(linear, num3))
			{
				UnityEvent<float> unityEvent2 = this.onLinearDistanceChanged;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this._linear);
				}
			}
			if (this.snapOrientation)
			{
				xform.forward = (position3 - position2).normalized;
				xform.up = Vector3.Lerp(this.from.up.normalized, this.to.up.normalized, this._linear);
			}
		}
	}

	// Token: 0x06003A4D RID: 14925 RVA: 0x00133D8A File Offset: 0x00131F8A
	private void OnDisable()
	{
		if (this.resetOnDisable)
		{
			this.SnapTargetLinear(0f);
		}
	}

	// Token: 0x06003A4E RID: 14926 RVA: 0x00133D9F File Offset: 0x00131F9F
	private void LateUpdate()
	{
		this.SnapTarget(true);
	}

	// Token: 0x06003A4F RID: 14927 RVA: 0x00133DA8 File Offset: 0x00131FA8
	private static Vector3 GetClosestPointOnLine(Vector3 p, Vector3 a, Vector3 b)
	{
		Vector3 vector = p - a;
		Vector3 vector2 = b - a;
		float sqrMagnitude = vector2.sqrMagnitude;
		float num = Mathf.Clamp(Vector3.Dot(vector, vector2) / sqrMagnitude, 0f, 1f);
		return a + vector2 * num;
	}

	// Token: 0x04004980 RID: 18816
	public bool apply = true;

	// Token: 0x04004981 RID: 18817
	public bool snapOrientation = true;

	// Token: 0x04004982 RID: 18818
	public bool resetOnDisable = true;

	// Token: 0x04004983 RID: 18819
	[Space]
	public Transform target;

	// Token: 0x04004984 RID: 18820
	[Space]
	public Transform from;

	// Token: 0x04004985 RID: 18821
	public Transform to;

	// Token: 0x04004986 RID: 18822
	private Vector3 _closest;

	// Token: 0x04004987 RID: 18823
	private float _linear;

	// Token: 0x04004988 RID: 18824
	public Ref<IRangedVariable<float>> output;

	// Token: 0x04004989 RID: 18825
	public UnityEvent<float> onLinearDistanceChanged;

	// Token: 0x0400498A RID: 18826
	public UnityEvent<Vector3> onPositionChanged;
}
