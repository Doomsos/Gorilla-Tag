using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000282 RID: 642
[DisallowMultipleComponent]
public class SimpleSpeedTracker : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600107D RID: 4221 RVA: 0x000562BC File Offset: 0x000544BC
	public void OnEnable()
	{
		if (this.target == null)
		{
			this.target = base.transform;
		}
		this.lastPos = this.target.position;
		this.lastSliceTime = Time.time;
		this.lastVelocity = Vector3.zero;
		this.lastRawSpeed = 0f;
		this.lastSpeed = 0f;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00056328 File Offset: 0x00054528
	public void SliceUpdate()
	{
		float num = Mathf.Max(1E-06f, Time.time - this.lastSliceTime);
		Vector3 position = this.target.position;
		Vector3 vector = (position - this.lastPos) / num;
		float magnitude = vector.magnitude;
		this.lastSpeed = (this.useRawSpeed ? magnitude : Mathf.Lerp(this.lastSpeed, magnitude, 1f - Mathf.Exp(-this.responsiveness * num)));
		float num2 = this.postprocessCurve.Evaluate(this.lastSpeed);
		this.continuousProperties.ApplyAll(num2);
		float num3 = this.useRawSpeed ? magnitude : num2;
		UnityEvent<float> unityEvent = this.onSpeedUpdated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(num3);
		}
		this.debugCurrentSpeed = num3;
		bool flag = num3 >= this.eventThreshold;
		if (flag && !this.wasAboveThreshold)
		{
			UnityEvent unityEvent2 = this.onSpeedAboveThreshold;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
		}
		else if (!flag && this.wasAboveThreshold)
		{
			UnityEvent unityEvent3 = this.onSpeedBelowThreshold;
			if (unityEvent3 != null)
			{
				unityEvent3.Invoke();
			}
		}
		this.wasAboveThreshold = flag;
		this.lastVelocity = vector;
		this.lastRawSpeed = magnitude;
		this.lastPos = position;
		this.lastSliceTime = Time.time;
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00056461 File Offset: 0x00054661
	public float GetPostProcessSpeed()
	{
		return this.postprocessCurve.Evaluate(this.lastSpeed);
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x00056474 File Offset: 0x00054674
	public float GetRawSpeed()
	{
		return this.lastRawSpeed;
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x0005647C File Offset: 0x0005467C
	public Vector3 GetWorldVelocity()
	{
		return this.lastVelocity;
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x00056484 File Offset: 0x00054684
	public Vector3 GetLocalVelocity()
	{
		if (this.useWorldAxes)
		{
			return this.lastVelocity;
		}
		if (this.target != null)
		{
			return this.target.InverseTransformDirection(this.lastVelocity);
		}
		return base.transform.InverseTransformDirection(this.lastVelocity);
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x000564D1 File Offset: 0x000546D1
	public float GetSignedSpeedAlongForward(Transform reference)
	{
		if (reference == null)
		{
			return 0f;
		}
		return Vector3.Dot(this.lastVelocity, reference.forward);
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x000564F3 File Offset: 0x000546F3
	public float GetSignedSpeedX()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisRight());
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x00056506 File Offset: 0x00054706
	public float GetSignedSpeedY()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisUp());
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x00056519 File Offset: 0x00054719
	public float GetSignedSpeedZ()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisForward());
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x0005652C File Offset: 0x0005472C
	public Vector3 GetVelocityInAxisSpace()
	{
		Vector3 vector = this.ResolveAxisRight();
		Vector3 vector2 = this.ResolveAxisUp();
		Vector3 vector3 = this.ResolveAxisForward();
		return new Vector3(Vector3.Dot(this.lastVelocity, vector), Vector3.Dot(this.lastVelocity, vector2), Vector3.Dot(this.lastVelocity, vector3));
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x00056578 File Offset: 0x00054778
	private Vector3 ResolveAxisRight()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.right;
			}
			return Vector3.right;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.right;
			}
			return this.target.right;
		}
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x000565D4 File Offset: 0x000547D4
	private Vector3 ResolveAxisUp()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.up;
			}
			return Vector3.up;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.up;
			}
			return this.target.up;
		}
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x00056630 File Offset: 0x00054830
	private Vector3 ResolveAxisForward()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.forward;
			}
			return Vector3.forward;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.forward;
			}
			return this.target.forward;
		}
	}

	// Token: 0x0400147D RID: 5245
	[Header("Settings")]
	[Tooltip("Transform whose movement speed is tracked. If left empty, uses this object’s transform.")]
	[SerializeField]
	private Transform target;

	// Token: 0x0400147E RID: 5246
	[Tooltip("If enabled, speed and direction calculations use world (global) space, otherwise local space.\nUse Local Space when you want speed relative to the object’s facing direction (e.g., how fast a sword swings forward)")]
	[SerializeField]
	private bool useWorldAxes;

	// Token: 0x0400147F RID: 5247
	[Tooltip("Optional transform defining a custom world reference.\nIf set, that transform’s Right/Up/Forward axes are treated as world axes.\nIf left empty, Unity’s global world axes are used.")]
	[SerializeField]
	private Transform worldSpace;

	// Token: 0x04001480 RID: 5248
	[Tooltip("If true, uses raw instantaneous speed without smoothing.\nIf false, smooths speed using the Responsiveness setting below.")]
	[SerializeField]
	private bool useRawSpeed;

	// Token: 0x04001481 RID: 5249
	[SerializeField]
	private float responsiveness = 10f;

	// Token: 0x04001482 RID: 5250
	[SerializeField]
	private AnimationCurve postprocessCurve = AnimationCurve.Linear(0f, 0f, 10f, 10f);

	// Token: 0x04001483 RID: 5251
	[Header("Property Output")]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x04001484 RID: 5252
	[Header("Events")]
	[Tooltip("Speed threshold used to trigger events.")]
	[SerializeField]
	private float eventThreshold = 1f;

	// Token: 0x04001485 RID: 5253
	public UnityEvent<float> onSpeedUpdated;

	// Token: 0x04001486 RID: 5254
	public UnityEvent onSpeedAboveThreshold;

	// Token: 0x04001487 RID: 5255
	public UnityEvent onSpeedBelowThreshold;

	// Token: 0x04001488 RID: 5256
	[Header("Debug")]
	[Tooltip("Current displayed speed value (raw or smoothed).")]
	public float debugCurrentSpeed;

	// Token: 0x04001489 RID: 5257
	private float lastSpeed;

	// Token: 0x0400148A RID: 5258
	private float lastRawSpeed;

	// Token: 0x0400148B RID: 5259
	private Vector3 lastVelocity;

	// Token: 0x0400148C RID: 5260
	private Vector3 lastPos;

	// Token: 0x0400148D RID: 5261
	private float lastSliceTime;

	// Token: 0x0400148E RID: 5262
	private bool wasAboveThreshold;
}
