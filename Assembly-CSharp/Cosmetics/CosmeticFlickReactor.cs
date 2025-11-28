using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x02000FC3 RID: 4035
	public class CosmeticFlickReactor : MonoBehaviour
	{
		// Token: 0x0600664F RID: 26191 RVA: 0x0021581F File Offset: 0x00213A1F
		private void Reset()
		{
			if (this.speedTracker == null)
			{
				this.speedTracker = base.GetComponent<SimpleSpeedTracker>();
			}
			if (this.rb == null)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
		}

		// Token: 0x06006650 RID: 26192 RVA: 0x00215858 File Offset: 0x00213A58
		private void Awake()
		{
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = (this.rig != null && this.rig.isLocal);
			this.ResetState();
			this.blockUntilTime = 0f;
			this.hasLastPosition = false;
		}

		// Token: 0x06006651 RID: 26193 RVA: 0x002158DC File Offset: 0x00213ADC
		private void Update()
		{
			Vector3 axis = this.ResolveAxisDirection();
			if (axis.sqrMagnitude < 0.5f)
			{
				return;
			}
			float signedSpeedAlong = this.GetSignedSpeedAlong(axis);
			if (Mathf.Abs(signedSpeedAlong) >= this.minSpeedThreshold)
			{
				int num = (signedSpeedAlong > 0f) ? 1 : -1;
				if (num != this.lastPeakSign || Mathf.Abs(signedSpeedAlong) > Mathf.Abs(this.lastPeakSpeed))
				{
					if (this.lastPeakSign == 0 || num != -this.lastPeakSign)
					{
						this.lastPeakSign = num;
						this.lastPeakSpeed = signedSpeedAlong;
						this.lastPeakTime = Time.time;
						return;
					}
					float num2 = Time.time - this.lastPeakTime;
					float num3 = Mathf.Abs(this.lastPeakSpeed) + Mathf.Abs(signedSpeedAlong);
					bool flag = num2 <= this.flickWindowSeconds;
					bool flag2 = num3 >= this.directionChangeRequired;
					bool flag3 = Time.time >= this.blockUntilTime;
					if (flag && flag2 && flag3)
					{
						this.FireEvents(Mathf.Abs(signedSpeedAlong));
						this.blockUntilTime = Time.time + this.retriggerBufferSeconds;
						this.ResetState();
						return;
					}
					this.lastPeakSign = num;
					this.lastPeakSpeed = signedSpeedAlong;
					this.lastPeakTime = Time.time;
					return;
				}
			}
			else if (Time.time - this.lastPeakTime > this.flickWindowSeconds)
			{
				this.ResetState();
			}
		}

		// Token: 0x06006652 RID: 26194 RVA: 0x00215A28 File Offset: 0x00213C28
		private Vector3 ResolveAxisDirection()
		{
			switch (this.axisMode)
			{
			case CosmeticFlickReactor.AxisMode.X:
				if (!this.useWorldAxes)
				{
					return base.transform.right;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.right;
				}
				return this.worldSpace.right;
			case CosmeticFlickReactor.AxisMode.Y:
				if (!this.useWorldAxes)
				{
					return base.transform.up;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.up;
				}
				return this.worldSpace.up;
			case CosmeticFlickReactor.AxisMode.Z:
				if (!this.useWorldAxes)
				{
					return base.transform.forward;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.forward;
				}
				return this.worldSpace.forward;
			case CosmeticFlickReactor.AxisMode.CustomForward:
				if (!(this.axisReference != null))
				{
					return Vector3.zero;
				}
				return this.axisReference.forward;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x06006653 RID: 26195 RVA: 0x00215B18 File Offset: 0x00213D18
		private float GetSignedSpeedAlong(Vector3 axis)
		{
			Vector3 vector;
			if (this.speedTracker != null)
			{
				vector = this.speedTracker.GetWorldVelocity();
			}
			else if (this.rb != null)
			{
				vector = this.rb.linearVelocity;
			}
			else
			{
				if (!this.hasLastPosition)
				{
					this.lastPosition = base.transform.position;
					this.hasLastPosition = true;
					return 0f;
				}
				Vector3 vector2 = base.transform.position - this.lastPosition;
				float num = (Time.deltaTime > Mathf.Epsilon) ? (1f / Time.deltaTime) : 0f;
				vector = vector2 * num;
				this.lastPosition = base.transform.position;
			}
			return Vector3.Dot(vector, axis.normalized);
		}

		// Token: 0x06006654 RID: 26196 RVA: 0x00215BE0 File Offset: 0x00213DE0
		private void FireEvents(float currentAbsSpeed)
		{
			if (this.isLocal)
			{
				UnityEvent onFlickLocal = this.OnFlickLocal;
				if (onFlickLocal != null)
				{
					onFlickLocal.Invoke();
				}
			}
			UnityEvent onFlickShared = this.OnFlickShared;
			if (onFlickShared != null)
			{
				onFlickShared.Invoke();
			}
			if (this.maxSpeedThreshold > 0f)
			{
				float num = Mathf.InverseLerp(this.minSpeedThreshold, this.maxSpeedThreshold, currentAbsSpeed);
				UnityEvent<float> unityEvent = this.onFlickStrength;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(Mathf.Clamp01(num));
			}
		}

		// Token: 0x06006655 RID: 26197 RVA: 0x00215C4D File Offset: 0x00213E4D
		private void ResetState()
		{
			this.lastPeakSign = 0;
			this.lastPeakSpeed = 0f;
			this.lastPeakTime = -9999f;
		}

		// Token: 0x040074E9 RID: 29929
		[Header("Axis")]
		[Tooltip("Which single axis/direction to use for flick detection.\n- X/Y/Z use the axes defined by the Space settings below (Local vs World).\n- CustomForward uses axisReference.forward (ignores Space).")]
		[SerializeField]
		private CosmeticFlickReactor.AxisMode axisMode = CosmeticFlickReactor.AxisMode.Z;

		// Token: 0x040074EA RID: 29930
		[Tooltip("Used only when AxisMode = CustomForward. The forward/back of this transform defines the direction.")]
		[SerializeField]
		private Transform axisReference;

		// Token: 0x040074EB RID: 29931
		[Header("Space")]
		[Tooltip("If enabled, X/Y/Z use world axes, otherwise local axes.\nUse Local for movement relative to the object’s facing.\nUse World for absolute directions independent of rotation.")]
		[SerializeField]
		private bool useWorldAxes;

		// Token: 0x040074EC RID: 29932
		[Tooltip("Optional transform to define a custom world frame for X/Y/Z.\nIf assigned and Space is World, this transform’s Right/Up/Forward act as the world axes.\nIf not assigned, Unity’s global axes are used.")]
		[SerializeField]
		private Transform worldSpace;

		// Token: 0x040074ED RID: 29933
		[Header("Velocity Source")]
		[Tooltip("Primary velocity tracker.")]
		[SerializeField]
		private SimpleSpeedTracker speedTracker;

		// Token: 0x040074EE RID: 29934
		[Tooltip("Fallback velocity source if speedTracker is missing.")]
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x040074EF RID: 29935
		[Header("Thresholds")]
		[Tooltip("Minimum absolute signed speed along the chosen axis required to consider a object movement (m/s).")]
		[SerializeField]
		private float minSpeedThreshold = 2f;

		// Token: 0x040074F0 RID: 29936
		[Tooltip("Optional upper bound for mapping flick strength to 0–1.\nSet <= 0 to disable onFlickStrength.")]
		[SerializeField]
		private float maxSpeedThreshold;

		// Token: 0x040074F1 RID: 29937
		[Tooltip("How much back-and-forth reversal is required to register a flick.\nExample: 2.5 means => +1.3 then -1.2 within the window (|1.3| + |1.2| = 2.5).")]
		[SerializeField]
		private float directionChangeRequired = 2f;

		// Token: 0x040074F2 RID: 29938
		[Header("Timing")]
		[Tooltip("Max time allowed between the initial peak and its reversal (seconds).")]
		[SerializeField]
		private float flickWindowSeconds = 0.2f;

		// Token: 0x040074F3 RID: 29939
		[Tooltip("Buffer time after a successful flick during which no new flicks are allowed (seconds).")]
		[SerializeField]
		private float retriggerBufferSeconds = 0.15f;

		// Token: 0x040074F4 RID: 29940
		[Header("Events")]
		public UnityEvent OnFlickShared;

		// Token: 0x040074F5 RID: 29941
		public UnityEvent OnFlickLocal;

		// Token: 0x040074F6 RID: 29942
		public UnityEvent<float> onFlickStrength;

		// Token: 0x040074F7 RID: 29943
		private Vector3 lastPosition;

		// Token: 0x040074F8 RID: 29944
		private bool hasLastPosition;

		// Token: 0x040074F9 RID: 29945
		private float lastPeakSpeed;

		// Token: 0x040074FA RID: 29946
		private float lastPeakTime = -999f;

		// Token: 0x040074FB RID: 29947
		private int lastPeakSign;

		// Token: 0x040074FC RID: 29948
		private float blockUntilTime;

		// Token: 0x040074FD RID: 29949
		private VRRig rig;

		// Token: 0x040074FE RID: 29950
		private bool isLocal;

		// Token: 0x02000FC4 RID: 4036
		private enum AxisMode
		{
			// Token: 0x04007500 RID: 29952
			X,
			// Token: 0x04007501 RID: 29953
			Y,
			// Token: 0x04007502 RID: 29954
			Z,
			// Token: 0x04007503 RID: 29955
			CustomForward
		}
	}
}
