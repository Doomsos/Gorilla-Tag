using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E47 RID: 3655
	public class BuilderMovingPart : MonoBehaviour
	{
		// Token: 0x06005B12 RID: 23314 RVA: 0x001D2D94 File Offset: 0x001D0F94
		private void Awake()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				builderAttachGridPlane.movesOnPlace = true;
				builderAttachGridPlane.movingPart = this;
			}
			this.initLocalPos = base.transform.localPosition;
			this.initLocalRotation = base.transform.localRotation;
		}

		// Token: 0x06005B13 RID: 23315 RVA: 0x001D2DE8 File Offset: 0x001D0FE8
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06005B14 RID: 23316 RVA: 0x001D2E1D File Offset: 0x001D101D
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x06005B15 RID: 23317 RVA: 0x001D2E2C File Offset: 0x001D102C
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06005B16 RID: 23318 RVA: 0x001D2E57 File Offset: 0x001D1057
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06005B17 RID: 23319 RVA: 0x001D2E67 File Offset: 0x001D1067
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06005B18 RID: 23320 RVA: 0x001D2E87 File Offset: 0x001D1087
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x06005B19 RID: 23321 RVA: 0x001D2E94 File Offset: 0x001D1094
		public void ActivateAtNode(byte node, int timestamp)
		{
			float num = (float)node;
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (num >= this.startPercentage)
			{
				int num2 = (int)((num - this.startPercentage) * (float)this.CycleLengthMs());
				int num3 = timestamp - num2;
				if (flag)
				{
					num3 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = num3;
			}
			else
			{
				int num4 = (int)((num + 2f - this.startPercentage) * (float)this.CycleLengthMs());
				if (flag)
				{
					num4 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = timestamp - num4;
			}
			this.SetMoving(true);
		}

		// Token: 0x06005B1A RID: 23322 RVA: 0x001D2F4C File Offset: 0x001D114C
		public int GetTimeOffsetMS()
		{
			int num = PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp;
			uint num2 = (uint)(this.CycleLengthMs() * 2L);
			return num % (int)num2;
		}

		// Token: 0x06005B1B RID: 23323 RVA: 0x001D2F78 File Offset: 0x001D1178
		public byte GetNearestNode()
		{
			int num = Mathf.RoundToInt(this.currT * (float)BuilderMovingPart.NUM_PAUSE_NODES);
			if (!this.IsEvenCycle())
			{
				num += BuilderMovingPart.NUM_PAUSE_NODES;
			}
			return (byte)num;
		}

		// Token: 0x06005B1C RID: 23324 RVA: 0x001D2FAA File Offset: 0x001D11AA
		public byte GetStartNode()
		{
			return (byte)Mathf.RoundToInt(this.startPercentage * (float)BuilderMovingPart.NUM_PAUSE_NODES);
		}

		// Token: 0x06005B1D RID: 23325 RVA: 0x001D2FC0 File Offset: 0x001D11C0
		public void PauseMovement(byte node)
		{
			this.SetMoving(false);
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			float num = (float)node;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (this.reverseDirOnCycle)
			{
				num = (flag ? (1f - num) : num);
			}
			if (this.reverseDir)
			{
				num = 1f - num;
			}
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(num);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(num);
		}

		// Token: 0x06005B1E RID: 23326 RVA: 0x001D3058 File Offset: 0x001D1258
		public void SetMoving(bool isMoving)
		{
			this.isMoving = isMoving;
			BuilderAttachGridPlane[] array = this.myGridPlanes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].isMoving = isMoving;
			}
			if (!isMoving)
			{
				this.ResetMovingGrid();
			}
		}

		// Token: 0x06005B1F RID: 23327 RVA: 0x001D3094 File Offset: 0x001D1294
		public void InitMovingGrid()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / (this.velocity * this.myPiece.GetScale());
				this.cycleDuration = num + this.cycleDelay;
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
			}
			else
			{
				this.cycleDuration = 1f / this.velocity;
			}
			this.currT = this.startPercentage;
			uint num4 = (uint)(this.cycleDuration * 1000f);
			uint num5 = 2147483648U % num4;
			uint num6 = (uint)(this.startPercentage * num4);
			if (num6 >= num5)
			{
				this.startPercentageCycleOffset = num6 - num5;
				return;
			}
			this.startPercentageCycleOffset = num6 + num4 + num4 - num5;
		}

		// Token: 0x06005B20 RID: 23328 RVA: 0x001D3208 File Offset: 0x001D1408
		public void UpdateMovingGrid()
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
				throw new ArgumentOutOfRangeException();
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x06005B21 RID: 23329 RVA: 0x001D3258 File Offset: 0x001D1458
		private Vector3 UpdatePointToPoint(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, num);
		}

		// Token: 0x06005B22 RID: 23330 RVA: 0x001D3290 File Offset: 0x001D1490
		private void UpdateRotation(float perc)
		{
			Quaternion localRotation = Quaternion.AngleAxis(perc * 360f, Vector3.up);
			base.transform.localRotation = localRotation;
		}

		// Token: 0x06005B23 RID: 23331 RVA: 0x001D32BB File Offset: 0x001D14BB
		private void ResetMovingGrid()
		{
			base.transform.SetLocalPositionAndRotation(this.initLocalPos, this.initLocalRotation);
		}

		// Token: 0x06005B24 RID: 23332 RVA: 0x001D32D4 File Offset: 0x001D14D4
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

		// Token: 0x06005B25 RID: 23333 RVA: 0x001D334C File Offset: 0x001D154C
		public bool IsAnchoredToTable()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				if (builderAttachGridPlane.attachIndex == builderAttachGridPlane.piece.attachIndex)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005B26 RID: 23334 RVA: 0x001D3388 File Offset: 0x001D1588
		public void OnPieceDestroy()
		{
			this.ResetMovingGrid();
		}

		// Token: 0x04006836 RID: 26678
		public BuilderPiece myPiece;

		// Token: 0x04006837 RID: 26679
		public BuilderAttachGridPlane[] myGridPlanes;

		// Token: 0x04006838 RID: 26680
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x04006839 RID: 26681
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x0400683A RID: 26682
		[SerializeField]
		private float velocity;

		// Token: 0x0400683B RID: 26683
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x0400683C RID: 26684
		[SerializeField]
		private bool reverseDir;

		// Token: 0x0400683D RID: 26685
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x0400683E RID: 26686
		[SerializeField]
		protected Transform startXf;

		// Token: 0x0400683F RID: 26687
		[SerializeField]
		protected Transform endXf;

		// Token: 0x04006840 RID: 26688
		public static int NUM_PAUSE_NODES = 32;

		// Token: 0x04006841 RID: 26689
		private AnimationCurve lerpAlpha;

		// Token: 0x04006842 RID: 26690
		public bool isMoving;

		// Token: 0x04006843 RID: 26691
		private Quaternion initLocalRotation = Quaternion.identity;

		// Token: 0x04006844 RID: 26692
		private Vector3 initLocalPos = Vector3.zero;

		// Token: 0x04006845 RID: 26693
		private float cycleDuration;

		// Token: 0x04006846 RID: 26694
		private float distance;

		// Token: 0x04006847 RID: 26695
		private float currT;

		// Token: 0x04006848 RID: 26696
		private float percent;

		// Token: 0x04006849 RID: 26697
		private bool currForward;

		// Token: 0x0400684A RID: 26698
		private float dtSinceServerUpdate;

		// Token: 0x0400684B RID: 26699
		private int lastServerTimeStamp;

		// Token: 0x0400684C RID: 26700
		private float rotateStartAmt;

		// Token: 0x0400684D RID: 26701
		private float rotateAmt;

		// Token: 0x0400684E RID: 26702
		private uint startPercentageCycleOffset;

		// Token: 0x02000E48 RID: 3656
		public enum BuilderMovingPartType
		{
			// Token: 0x04006850 RID: 26704
			Translation,
			// Token: 0x04006851 RID: 26705
			Rotation
		}
	}
}
