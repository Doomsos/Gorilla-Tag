using System;
using GorillaTagScripts.Builder;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DAD RID: 3501
	public class SurfaceMover : MonoBehaviour
	{
		// Token: 0x06005611 RID: 22033 RVA: 0x001B0A41 File Offset: 0x001AEC41
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterSurfaceMover(this);
		}

		// Token: 0x06005612 RID: 22034 RVA: 0x001B0A5A File Offset: 0x001AEC5A
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterSurfaceMover(this);
			}
		}

		// Token: 0x06005613 RID: 22035 RVA: 0x001B0A74 File Offset: 0x001AEC74
		public void InitMovingSurface()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / this.velocity;
				this.cycleDuration = num + this.cycleDelay;
			}
			else
			{
				if (this.rotationRelativeToStarting)
				{
					this.startingRotation = base.transform.localRotation.eulerAngles;
				}
				this.cycleDuration = this.rotationAmount / 360f / this.velocity;
				this.cycleDuration += this.cycleDelay;
			}
			float num2 = this.cycleDelay / this.cycleDuration;
			Vector2 vector;
			vector..ctor(num2 / 2f, 0f);
			Vector2 vector2;
			vector2..ctor(1f - num2 / 2f, 1f);
			float num3 = (vector2.y - vector.y) / (vector2.x - vector.x);
			this.lerpAlpha = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(num2 / 2f, 0f, 0f, num3),
				new Keyframe(1f - num2 / 2f, 1f, num3, 0f)
			});
			this.currT = this.startPercentage;
			uint num4 = (uint)(this.cycleDuration * 1000f);
			if (num4 == 0U)
			{
				num4 = 1U;
			}
			uint num5 = 2147483648U % num4;
			uint num6 = (uint)(this.startPercentage * num4);
			if (num6 >= num5)
			{
				this.startPercentageCycleOffset = num6 - num5;
				return;
			}
			this.startPercentageCycleOffset = num6 + num4 + num4 - num5;
		}

		// Token: 0x06005614 RID: 22036 RVA: 0x001B0C1E File Offset: 0x001AEE1E
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06005615 RID: 22037 RVA: 0x001B0C47 File Offset: 0x001AEE47
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x06005616 RID: 22038 RVA: 0x001B0C58 File Offset: 0x001AEE58
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06005617 RID: 22039 RVA: 0x001B0C83 File Offset: 0x001AEE83
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06005618 RID: 22040 RVA: 0x001B0C93 File Offset: 0x001AEE93
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06005619 RID: 22041 RVA: 0x001B0CB3 File Offset: 0x001AEEB3
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x0600561A RID: 22042 RVA: 0x001B0CC0 File Offset: 0x001AEEC0
		public void Move()
		{
			this.Progress();
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(this.percent);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x0600561B RID: 22043 RVA: 0x001B0D08 File Offset: 0x001AEF08
		private Vector3 UpdatePointToPoint(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, num);
		}

		// Token: 0x0600561C RID: 22044 RVA: 0x001B0D40 File Offset: 0x001AEF40
		private void UpdateRotation(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc) * this.rotationAmount;
			if (this.rotationRelativeToStarting)
			{
				Vector3 vector = this.startingRotation;
				switch (this.rotationAxis)
				{
				case RotationAxis.X:
					vector.x += num;
					break;
				case RotationAxis.Y:
					vector.y += num;
					break;
				case RotationAxis.Z:
					vector.z += num;
					break;
				}
				base.transform.localRotation = Quaternion.Euler(vector);
				return;
			}
			switch (this.rotationAxis)
			{
			case RotationAxis.X:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.right);
				return;
			case RotationAxis.Y:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.up);
				return;
			case RotationAxis.Z:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.forward);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600561D RID: 22045 RVA: 0x001B0E24 File Offset: 0x001AF024
		private void Progress()
		{
			this.currT = this.CycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			this.percent = this.currT;
			if (this.reverseDirOnCycle)
			{
				this.percent = (this.currForward ? this.currT : (1f - this.currT));
			}
			if (this.reverseDir)
			{
				this.percent = 1f - this.percent;
			}
		}

		// Token: 0x0600561E RID: 22046 RVA: 0x001B0E9C File Offset: 0x001AF09C
		public void CopySettings(SurfaceMoverSettings settings)
		{
			this.moveType = settings.moveType;
			this.startPercentage = 0f;
			this.velocity = Math.Clamp(settings.velocity, 0.001f, Math.Abs(settings.velocity));
			this.reverseDirOnCycle = settings.reverseDirOnCycle;
			this.reverseDir = settings.reverseDir;
			this.cycleDelay = Math.Clamp(settings.cycleDelay, 0f, Math.Abs(settings.cycleDelay));
			this.startXf = settings.start;
			this.endXf = settings.end;
			this.rotationAxis = settings.rotationAxis;
			this.rotationAmount = Math.Clamp(settings.rotationAmount, 0.001f, Math.Abs(settings.rotationAmount));
			this.rotationRelativeToStarting = settings.rotationRelativeToStarting;
		}

		// Token: 0x04006326 RID: 25382
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x04006327 RID: 25383
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x04006328 RID: 25384
		[SerializeField]
		private float velocity;

		// Token: 0x04006329 RID: 25385
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x0400632A RID: 25386
		[SerializeField]
		private bool reverseDir;

		// Token: 0x0400632B RID: 25387
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x0400632C RID: 25388
		[SerializeField]
		protected Transform startXf;

		// Token: 0x0400632D RID: 25389
		[SerializeField]
		protected Transform endXf;

		// Token: 0x0400632E RID: 25390
		[SerializeField]
		public RotationAxis rotationAxis = RotationAxis.Y;

		// Token: 0x0400632F RID: 25391
		[SerializeField]
		public float rotationAmount = 360f;

		// Token: 0x04006330 RID: 25392
		[SerializeField]
		public bool rotationRelativeToStarting;

		// Token: 0x04006331 RID: 25393
		private AnimationCurve lerpAlpha;

		// Token: 0x04006332 RID: 25394
		private float cycleDuration;

		// Token: 0x04006333 RID: 25395
		private float distance;

		// Token: 0x04006334 RID: 25396
		private Vector3 startingRotation;

		// Token: 0x04006335 RID: 25397
		private float currT;

		// Token: 0x04006336 RID: 25398
		private float percent;

		// Token: 0x04006337 RID: 25399
		private bool currForward;

		// Token: 0x04006338 RID: 25400
		private float dtSinceServerUpdate;

		// Token: 0x04006339 RID: 25401
		private int lastServerTimeStamp;

		// Token: 0x0400633A RID: 25402
		private float rotateStartAmt;

		// Token: 0x0400633B RID: 25403
		private float rotateAmt;

		// Token: 0x0400633C RID: 25404
		private uint startPercentageCycleOffset;
	}
}
