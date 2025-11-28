using System;
using GorillaLocomotion.Climbing;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000F9D RID: 3997
	public class GorillaZipline : MonoBehaviour
	{
		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x0600645D RID: 25693 RVA: 0x0020BB82 File Offset: 0x00209D82
		// (set) Token: 0x0600645E RID: 25694 RVA: 0x0020BB8A File Offset: 0x00209D8A
		public float currentSpeed { get; private set; }

		// Token: 0x0600645F RID: 25695 RVA: 0x0020BB94 File Offset: 0x00209D94
		protected void FindTFromDistance(ref float t, float distance, int steps = 1000)
		{
			float num = distance / (float)steps;
			Vector3 vector = this.spline.GetPointLocal(t);
			float num2 = 0f;
			for (int i = 0; i < 1000; i++)
			{
				t += num;
				if (t >= 1f || t <= 0f)
				{
					break;
				}
				Vector3 pointLocal = this.spline.GetPointLocal(t);
				num2 += Vector3.Distance(pointLocal, vector);
				if (num2 >= Mathf.Abs(distance))
				{
					break;
				}
				vector = pointLocal;
			}
		}

		// Token: 0x06006460 RID: 25696 RVA: 0x0020BC08 File Offset: 0x00209E08
		private float FindSlideHelperSpot(Vector3 grabPoint)
		{
			int i = 0;
			int num = 200;
			float num2 = 0.001f;
			float num3 = 1f / (float)num;
			float3 @float = base.transform.InverseTransformPoint(grabPoint);
			float result = 0f;
			float num4 = float.PositiveInfinity;
			while (i < num)
			{
				float num5 = math.distancesq(this.spline.GetPointLocal(num2), @float);
				if (num5 < num4)
				{
					num4 = num5;
					result = num2;
				}
				num2 += num3;
				i++;
			}
			return result;
		}

		// Token: 0x06006461 RID: 25697 RVA: 0x0020BC84 File Offset: 0x00209E84
		protected virtual void Start()
		{
			this.spline = base.GetComponent<BezierSpline>();
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06006462 RID: 25698 RVA: 0x0020BCB9 File Offset: 0x00209EB9
		private void OnDestroy()
		{
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Remove(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06006463 RID: 25699 RVA: 0x0020BCE2 File Offset: 0x00209EE2
		public Vector3 GetCurrentDirection()
		{
			return this.spline.GetDirection(this.currentT);
		}

		// Token: 0x06006464 RID: 25700 RVA: 0x0020BCF8 File Offset: 0x00209EF8
		protected void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
		{
			bool flag = this.currentClimber == null;
			this.currentClimber = hand;
			if (climbRef)
			{
				this.climbOffsetHelper.SetParent(climbRef.transform);
				this.climbOffsetHelper.position = hand.transform.position;
				this.climbOffsetHelper.localPosition = new Vector3(0f, 0f, this.climbOffsetHelper.localPosition.z);
			}
			this.currentT = this.FindSlideHelperSpot(this.climbOffsetHelper.position);
			this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
			if (flag)
			{
				Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
				float num = Vector3.Dot(averagedVelocity.normalized, this.spline.GetDirection(this.currentT));
				this.currentSpeed = averagedVelocity.magnitude * num * this.currentInheritVelocityMulti;
			}
		}

		// Token: 0x06006465 RID: 25701 RVA: 0x0020BDEC File Offset: 0x00209FEC
		private void Update()
		{
			if (this.currentClimber)
			{
				Vector3 direction = this.spline.GetDirection(this.currentT);
				float num = Physics.gravity.y * direction.y * this.settings.gravityMulti;
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.settings.maxSpeed, num * Time.deltaTime);
				float num2 = MathUtils.Linear(this.currentSpeed, 0f, this.settings.maxFrictionSpeed, this.settings.friction, this.settings.maxFriction);
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, num2 * Time.deltaTime);
				this.currentSpeed = Mathf.Min(this.currentSpeed, this.settings.maxSpeed);
				this.currentSpeed = Mathf.Max(this.currentSpeed, -this.settings.maxSpeed);
				float value = Mathf.Abs(this.currentSpeed);
				this.FindTFromDistance(ref this.currentT, this.currentSpeed * Time.deltaTime, 1000);
				this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
				if (!this.audioSlide.gameObject.activeSelf)
				{
					this.audioSlide.gameObject.SetActive(true);
				}
				this.audioSlide.volume = MathUtils.Linear(value, 0f, this.settings.maxSpeed, this.settings.minSlideVolume, this.settings.maxSlideVolume);
				this.audioSlide.pitch = MathUtils.Linear(value, 0f, this.settings.maxSpeed, this.settings.minSlidePitch, this.settings.maxSlidePitch);
				if (!this.audioSlide.isPlaying)
				{
					this.audioSlide.GTPlay();
				}
				float num3 = MathUtils.Linear(value, 0f, this.settings.maxSpeed, -0.1f, 0.75f);
				if (num3 > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.currentClimber.xrNode, num3, Time.deltaTime);
				}
				if (!this.spline.Loop)
				{
					if (this.currentT >= 1f || this.currentT <= 0f)
					{
						this.currentClimber.ForceStopClimbing(false, true);
					}
				}
				else if (this.currentT >= 1f)
				{
					this.currentT = 0f;
				}
				else if (this.currentT <= 0f)
				{
					this.currentT = 1f;
				}
				if (!this.slideHelper.isBeingClimbed)
				{
					this.Stop();
				}
			}
			if (this.currentInheritVelocityMulti < 1f)
			{
				this.currentInheritVelocityMulti += Time.deltaTime * 0.2f;
				this.currentInheritVelocityMulti = Mathf.Min(this.currentInheritVelocityMulti, 1f);
			}
		}

		// Token: 0x06006466 RID: 25702 RVA: 0x0020C0DA File Offset: 0x0020A2DA
		private void Stop()
		{
			this.currentClimber = null;
			this.audioSlide.GTStop();
			this.audioSlide.gameObject.SetActive(false);
			this.currentInheritVelocityMulti = 0.55f;
			this.currentSpeed = 0f;
		}

		// Token: 0x04007405 RID: 29701
		[SerializeField]
		protected Transform segmentsRoot;

		// Token: 0x04007406 RID: 29702
		[SerializeField]
		protected GameObject segmentPrefab;

		// Token: 0x04007407 RID: 29703
		[SerializeField]
		protected GorillaClimbable slideHelper;

		// Token: 0x04007408 RID: 29704
		[SerializeField]
		private AudioSource audioSlide;

		// Token: 0x04007409 RID: 29705
		protected BezierSpline spline;

		// Token: 0x0400740A RID: 29706
		[SerializeField]
		private Transform climbOffsetHelper;

		// Token: 0x0400740B RID: 29707
		[SerializeField]
		private GorillaZiplineSettings settings;

		// Token: 0x0400740D RID: 29709
		[SerializeField]
		protected float ziplineDistance = 15f;

		// Token: 0x0400740E RID: 29710
		[SerializeField]
		protected float segmentDistance = 0.9f;

		// Token: 0x0400740F RID: 29711
		private GorillaHandClimber currentClimber;

		// Token: 0x04007410 RID: 29712
		private float currentT;

		// Token: 0x04007411 RID: 29713
		private const float inheritVelocityRechargeRate = 0.2f;

		// Token: 0x04007412 RID: 29714
		private const float inheritVelocityValueOnRelease = 0.55f;

		// Token: 0x04007413 RID: 29715
		private float currentInheritVelocityMulti = 1f;
	}
}
