using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000FA1 RID: 4001
	public class NoncontrollableBroomstick : MonoBehaviour, IGorillaGrabable
	{
		// Token: 0x0600646F RID: 25711 RVA: 0x0020C1AC File Offset: 0x0020A3AC
		private void Start()
		{
			this.smoothRotationTrackingRateExp = Mathf.Exp(this.smoothRotationTrackingRate);
			this.progressPerFixedUpdate = Time.fixedDeltaTime / this.duration;
			this.progress = this.SplineProgressOffet;
			this.secondsToCycles = 1.0 / (double)this.duration;
			if (this.unitySpline != null)
			{
				this.nativeSpline = new NativeSpline(this.unitySpline.Spline, this.unitySpline.transform.localToWorldMatrix, 4);
			}
		}

		// Token: 0x06006470 RID: 25712 RVA: 0x0020C23C File Offset: 0x0020A43C
		protected virtual void FixedUpdate()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this.secondsToCycles + (double)this.SplineProgressOffet;
				this.progress = (float)(num % 1.0);
			}
			else
			{
				this.progress = (this.progress + this.progressPerFixedUpdate) % 1f;
			}
			Quaternion quaternion = Quaternion.identity;
			if (this.unitySpline != null)
			{
				float3 @float;
				float3 float2;
				float3 float3;
				SplineUtility.Evaluate<NativeSpline>(this.nativeSpline, this.progress, ref @float, ref float2, ref float3);
				base.transform.position = @float;
				if (this.lookForward)
				{
					quaternion = Quaternion.LookRotation(new Vector3(float2.x, float2.y, float2.z));
				}
			}
			else if (this.spline != null)
			{
				Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
				base.transform.position = point;
				if (this.lookForward)
				{
					quaternion = Quaternion.LookRotation(this.spline.GetDirection(this.progress, this.constantVelocity));
				}
			}
			if (this.lookForward)
			{
				base.transform.rotation = Quaternion.Slerp(quaternion, base.transform.rotation, Mathf.Exp(-this.smoothRotationTrackingRateExp * Time.deltaTime));
			}
		}

		// Token: 0x06006471 RID: 25713 RVA: 0x00027DED File Offset: 0x00025FED
		bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
		{
			return true;
		}

		// Token: 0x06006472 RID: 25714 RVA: 0x0020C385 File Offset: 0x0020A585
		void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosition)
		{
			grabbedObject = base.transform;
			grabbedLocalPosition = base.transform.InverseTransformPoint(g.transform.position);
		}

		// Token: 0x06006473 RID: 25715 RVA: 0x00002789 File Offset: 0x00000989
		void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
		{
		}

		// Token: 0x06006474 RID: 25716 RVA: 0x0020C3AB File Offset: 0x0020A5AB
		private void OnDestroy()
		{
			this.nativeSpline.Dispose();
		}

		// Token: 0x06006475 RID: 25717 RVA: 0x0020C3B8 File Offset: 0x0020A5B8
		public bool MomentaryGrabOnly()
		{
			return this.momentaryGrabOnly;
		}

		// Token: 0x06006477 RID: 25719 RVA: 0x00013E3B File Offset: 0x0001203B
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x0400741E RID: 29726
		public SplineContainer unitySpline;

		// Token: 0x0400741F RID: 29727
		public BezierSpline spline;

		// Token: 0x04007420 RID: 29728
		public float duration = 30f;

		// Token: 0x04007421 RID: 29729
		public float smoothRotationTrackingRate = 0.5f;

		// Token: 0x04007422 RID: 29730
		public bool lookForward = true;

		// Token: 0x04007423 RID: 29731
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x04007424 RID: 29732
		private float progress;

		// Token: 0x04007425 RID: 29733
		private float smoothRotationTrackingRateExp;

		// Token: 0x04007426 RID: 29734
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x04007427 RID: 29735
		private float progressPerFixedUpdate;

		// Token: 0x04007428 RID: 29736
		private double secondsToCycles;

		// Token: 0x04007429 RID: 29737
		private NativeSpline nativeSpline;

		// Token: 0x0400742A RID: 29738
		[SerializeField]
		private bool momentaryGrabOnly = true;
	}
}
