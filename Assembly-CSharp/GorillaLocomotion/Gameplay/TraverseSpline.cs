using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000FA8 RID: 4008
	[NetworkBehaviourWeaved(1)]
	public class TraverseSpline : NetworkComponent
	{
		// Token: 0x060064A8 RID: 25768 RVA: 0x0020D225 File Offset: 0x0020B425
		protected override void Awake()
		{
			base.Awake();
			this.progress = this.SplineProgressOffet % 1f;
		}

		// Token: 0x060064A9 RID: 25769 RVA: 0x0020D240 File Offset: 0x0020B440
		protected virtual void FixedUpdate()
		{
			if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f);
			}
			else
			{
				if (this.isHeldByLocalPlayer)
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, this.speedMultiplierWhileHeld, this.acceleration * Time.deltaTime);
				}
				else
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, 1f, this.deceleration * Time.deltaTime);
				}
				if (this.goingForward)
				{
					this.progress += Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress > 1f)
					{
						if (this.mode == SplineWalkerMode.Once)
						{
							this.progress = 1f;
						}
						else if (this.mode == SplineWalkerMode.Loop)
						{
							this.progress %= 1f;
						}
						else
						{
							this.progress = 2f - this.progress;
							this.goingForward = false;
						}
					}
				}
				else
				{
					this.progress -= Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress < 0f)
					{
						this.progress = -this.progress;
						this.goingForward = true;
					}
				}
			}
			Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
			base.transform.position = point;
			if (this.lookForward)
			{
				base.transform.LookAt(base.transform.position + this.spline.GetDirection(this.progress, this.constantVelocity));
			}
		}

		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x060064AA RID: 25770 RVA: 0x0020D409 File Offset: 0x0020B609
		// (set) Token: 0x060064AB RID: 25771 RVA: 0x0020D42F File Offset: 0x0020B62F
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe float Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(float*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(float*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x060064AC RID: 25772 RVA: 0x0020D456 File Offset: 0x0020B656
		public override void WriteDataFusion()
		{
			this.Data = this.progress + this.currentSpeedMultiplier * 1f / this.duration;
		}

		// Token: 0x060064AD RID: 25773 RVA: 0x0020D478 File Offset: 0x0020B678
		public override void ReadDataFusion()
		{
			this.progressLerpEnd = this.Data;
			this.ReadDataShared();
		}

		// Token: 0x060064AE RID: 25774 RVA: 0x0020D48C File Offset: 0x0020B68C
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.progress + this.currentSpeedMultiplier * 1f / this.duration);
		}

		// Token: 0x060064AF RID: 25775 RVA: 0x0020D4B3 File Offset: 0x0020B6B3
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			this.progressLerpEnd = (float)stream.ReceiveNext();
			this.ReadDataShared();
		}

		// Token: 0x060064B0 RID: 25776 RVA: 0x0020D4CC File Offset: 0x0020B6CC
		private void ReadDataShared()
		{
			if (float.IsNaN(this.progressLerpEnd) || float.IsInfinity(this.progressLerpEnd))
			{
				this.progressLerpEnd = 1f;
			}
			else
			{
				this.progressLerpEnd = Mathf.Abs(this.progressLerpEnd);
				if (this.progressLerpEnd > 1f)
				{
					this.progressLerpEnd = (float)((double)this.progressLerpEnd % 1.0);
				}
			}
			this.progressLerpStart = ((Mathf.Abs(this.progressLerpEnd - this.progress) > Mathf.Abs(this.progressLerpEnd - (this.progress - 1f))) ? (this.progress - 1f) : this.progress);
			this.progressLerpStartTime = Time.time;
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x0020D587 File Offset: 0x0020B787
		protected float GetProgress()
		{
			return this.progress;
		}

		// Token: 0x060064B2 RID: 25778 RVA: 0x0020D58F File Offset: 0x0020B78F
		public float GetCurrentSpeed()
		{
			return this.currentSpeedMultiplier;
		}

		// Token: 0x060064B4 RID: 25780 RVA: 0x0020D5E5 File Offset: 0x0020B7E5
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x060064B5 RID: 25781 RVA: 0x0020D5FD File Offset: 0x0020B7FD
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04007453 RID: 29779
		public BezierSpline spline;

		// Token: 0x04007454 RID: 29780
		public float duration = 30f;

		// Token: 0x04007455 RID: 29781
		public float speedMultiplierWhileHeld = 2f;

		// Token: 0x04007456 RID: 29782
		private float currentSpeedMultiplier;

		// Token: 0x04007457 RID: 29783
		public float acceleration = 1f;

		// Token: 0x04007458 RID: 29784
		public float deceleration = 1f;

		// Token: 0x04007459 RID: 29785
		private bool isHeldByLocalPlayer;

		// Token: 0x0400745A RID: 29786
		public bool lookForward = true;

		// Token: 0x0400745B RID: 29787
		public SplineWalkerMode mode;

		// Token: 0x0400745C RID: 29788
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x0400745D RID: 29789
		private float progress;

		// Token: 0x0400745E RID: 29790
		private float progressLerpStart;

		// Token: 0x0400745F RID: 29791
		private float progressLerpEnd;

		// Token: 0x04007460 RID: 29792
		private const float progressLerpDuration = 1f;

		// Token: 0x04007461 RID: 29793
		private float progressLerpStartTime;

		// Token: 0x04007462 RID: 29794
		private bool goingForward = true;

		// Token: 0x04007463 RID: 29795
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x04007464 RID: 29796
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private float _Data;
	}
}
