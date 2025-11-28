using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200031C RID: 796
public class SpinParametricAnimation : MonoBehaviour
{
	// Token: 0x06001352 RID: 4946 RVA: 0x0006FCC9 File Offset: 0x0006DEC9
	protected void OnEnable()
	{
		this.axis = this.axis.normalized;
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0006FCDC File Offset: 0x0006DEDC
	protected void LateUpdate()
	{
		Transform transform = base.transform;
		this._animationProgress = (this._animationProgress + Time.deltaTime * this.revolutionsPerSecond) % 1f;
		float num = this.timeCurve.Evaluate(this._animationProgress) * 360f;
		float num2 = num - this._oldAngle;
		this._oldAngle = num;
		if (this.WorldSpaceRotation)
		{
			transform.rotation = Quaternion.AngleAxis(num2, this.axis) * transform.rotation;
			return;
		}
		transform.localRotation = Quaternion.AngleAxis(num2, this.axis) * transform.localRotation;
	}

	// Token: 0x04001CD7 RID: 7383
	[Tooltip("Axis to rotate around.")]
	public Vector3 axis = Vector3.up;

	// Token: 0x04001CD8 RID: 7384
	[Tooltip("Whether rotation is in World Space or Local Space")]
	public bool WorldSpaceRotation = true;

	// Token: 0x04001CD9 RID: 7385
	[FormerlySerializedAs("speed")]
	[Tooltip("Speed of rotation.")]
	public float revolutionsPerSecond = 0.25f;

	// Token: 0x04001CDA RID: 7386
	[Tooltip("Affects the progress of the animation over time.")]
	public AnimationCurve timeCurve;

	// Token: 0x04001CDB RID: 7387
	private float _animationProgress;

	// Token: 0x04001CDC RID: 7388
	private float _oldAngle;
}
