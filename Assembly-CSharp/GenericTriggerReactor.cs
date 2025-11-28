using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000099 RID: 153
public class GenericTriggerReactor : MonoBehaviour, IBuildValidation
{
	// Token: 0x060003CA RID: 970 RVA: 0x000171FA File Offset: 0x000153FA
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.ComponentName.Length == 0)
		{
			return true;
		}
		if (Type.GetType(this.ComponentName) == null)
		{
			Debug.LogError("GenericTriggerReactor :: ComponentName must specify a valid Component or be empty.");
			return false;
		}
		return true;
	}

	// Token: 0x060003CB RID: 971 RVA: 0x0001722B File Offset: 0x0001542B
	private void Awake()
	{
		this.componentType = Type.GetType(this.ComponentName);
		base.TryGetComponent<GorillaVelocityEstimator>(ref this.gorillaVelocityEstimator);
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0001724B File Offset: 0x0001544B
	private void OnTriggerEnter(Collider other)
	{
		this.OnTriggerTest(other, this.speedRangeEnter, this.GTOnTriggerEnter, this.idealMotionPlayRangeEnter);
	}

	// Token: 0x060003CD RID: 973 RVA: 0x00017266 File Offset: 0x00015466
	private void OnTriggerExit(Collider other)
	{
		this.OnTriggerTest(other, this.speedRangeExit, this.GTOnTriggerExit, this.idealMotionPlayRangeExit);
	}

	// Token: 0x060003CE RID: 974 RVA: 0x00017284 File Offset: 0x00015484
	private void OnTriggerTest(Collider other, Vector2 speedRange, UnityEvent unityEvent, Vector2 idealMotionPlay)
	{
		Component component;
		if (unityEvent != null && (this.componentType == null || other.TryGetComponent(this.componentType, ref component)))
		{
			if (this.gorillaVelocityEstimator != null)
			{
				float magnitude = this.gorillaVelocityEstimator.linearVelocity.magnitude;
				if (magnitude < speedRange.x || magnitude > speedRange.y)
				{
					return;
				}
				if (this.idealMotion != null)
				{
					float num = Vector3.Dot(this.gorillaVelocityEstimator.linearVelocity.normalized, this.idealMotion.forward);
					if (num < idealMotionPlay.x || num > idealMotionPlay.y)
					{
						return;
					}
				}
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x04000440 RID: 1088
	[SerializeField]
	private string ComponentName = string.Empty;

	// Token: 0x04000441 RID: 1089
	[Space]
	[SerializeField]
	private Vector2 speedRangeEnter;

	// Token: 0x04000442 RID: 1090
	[SerializeField]
	private Vector2 speedRangeExit;

	// Token: 0x04000443 RID: 1091
	[Space]
	[SerializeField]
	private Transform idealMotion;

	// Token: 0x04000444 RID: 1092
	[SerializeField]
	private Vector2 idealMotionPlayRangeEnter;

	// Token: 0x04000445 RID: 1093
	[SerializeField]
	private Vector2 idealMotionPlayRangeExit;

	// Token: 0x04000446 RID: 1094
	[Space]
	[SerializeField]
	private UnityEvent GTOnTriggerEnter;

	// Token: 0x04000447 RID: 1095
	[SerializeField]
	private UnityEvent GTOnTriggerExit;

	// Token: 0x04000448 RID: 1096
	private Type componentType;

	// Token: 0x04000449 RID: 1097
	private GorillaVelocityEstimator gorillaVelocityEstimator;
}
