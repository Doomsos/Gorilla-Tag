using System;
using UnityEngine;

// Token: 0x020004CE RID: 1230
public class PushableSlider : MonoBehaviour
{
	// Token: 0x06001FC3 RID: 8131 RVA: 0x000A9549 File Offset: 0x000A7749
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x000A9551 File Offset: 0x000A7751
	private void Initialize()
	{
		if (this._initialized)
		{
			return;
		}
		this._initialized = true;
		this._localSpace = base.transform.worldToLocalMatrix;
		this._startingPos = base.transform.localPosition;
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x000A9588 File Offset: 0x000A7788
	private void OnTriggerStay(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 vector = this._localSpace.MultiplyPoint3x4(other.transform.position);
		Vector3 vector2 = base.transform.localPosition - this._startingPos - vector;
		float num = Mathf.Abs(vector2.x);
		if (num < this.farPushDist)
		{
			Vector3 currentVelocity = componentInParent.currentVelocity;
			if (Mathf.Sign(vector2.x) != Mathf.Sign((this._localSpace.rotation * currentVelocity).x))
			{
				return;
			}
			vector2.x = Mathf.Sign(vector2.x) * (this.farPushDist - num);
			vector2.y = 0f;
			vector2.z = 0f;
			Vector3 vector3 = base.transform.localPosition - this._startingPos + vector2;
			vector3.x = Mathf.Clamp(vector3.x, this.minXOffset, this.maxXOffset);
			base.transform.localPosition = this.GetXOffsetVector(vector3.x + this._startingPos.x);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06001FC6 RID: 8134 RVA: 0x000A96EB File Offset: 0x000A78EB
	private Vector3 GetXOffsetVector(float x)
	{
		return new Vector3(x, this._startingPos.y, this._startingPos.z);
	}

	// Token: 0x06001FC7 RID: 8135 RVA: 0x000A970C File Offset: 0x000A790C
	public void SetProgress(float value)
	{
		this.Initialize();
		value = Mathf.Clamp(value, 0f, 1f);
		float num = Mathf.Lerp(this.minXOffset, this.maxXOffset, value);
		base.transform.localPosition = this.GetXOffsetVector(this._startingPos.x + num);
		this._previousLocalPosition = new Vector3(num, 0f, 0f);
		this._cachedProgress = value;
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x000A9780 File Offset: 0x000A7980
	public float GetProgress()
	{
		this.Initialize();
		Vector3 vector = base.transform.localPosition - this._startingPos;
		if (vector == this._previousLocalPosition)
		{
			return this._cachedProgress;
		}
		this._previousLocalPosition = vector;
		this._cachedProgress = (vector.x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
		return this._cachedProgress;
	}

	// Token: 0x04002A20 RID: 10784
	[SerializeField]
	private float farPushDist = 0.015f;

	// Token: 0x04002A21 RID: 10785
	[SerializeField]
	private float maxXOffset;

	// Token: 0x04002A22 RID: 10786
	[SerializeField]
	private float minXOffset;

	// Token: 0x04002A23 RID: 10787
	private Matrix4x4 _localSpace;

	// Token: 0x04002A24 RID: 10788
	private Vector3 _startingPos;

	// Token: 0x04002A25 RID: 10789
	private Vector3 _previousLocalPosition;

	// Token: 0x04002A26 RID: 10790
	private float _cachedProgress;

	// Token: 0x04002A27 RID: 10791
	private bool _initialized;
}
