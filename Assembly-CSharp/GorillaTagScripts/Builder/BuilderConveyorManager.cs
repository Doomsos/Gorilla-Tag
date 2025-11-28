using System;
using Photon.Pun;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E45 RID: 3653
	public class BuilderConveyorManager : MonoBehaviour
	{
		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x06005B01 RID: 23297 RVA: 0x001D2616 File Offset: 0x001D0816
		// (set) Token: 0x06005B02 RID: 23298 RVA: 0x001D261D File Offset: 0x001D081D
		public static BuilderConveyorManager instance { get; private set; }

		// Token: 0x06005B03 RID: 23299 RVA: 0x001D2625 File Offset: 0x001D0825
		private void Awake()
		{
			if (BuilderConveyorManager.instance != null && BuilderConveyorManager.instance != this)
			{
				Object.Destroy(this);
			}
			if (BuilderConveyorManager.instance == null)
			{
				BuilderConveyorManager.instance = this;
			}
		}

		// Token: 0x06005B04 RID: 23300 RVA: 0x001D265C File Offset: 0x001D085C
		public void UpdateManager()
		{
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.UpdateConveyor();
			}
			bool flag = false;
			bool flag2 = this.pieceTransforms.length >= this.pieceTransforms.capacity - 5;
			for (int i = this.jobSplineTimes.Length - 1; i >= 0; i--)
			{
				BuilderConveyor builderConveyor2 = this.table.conveyors[this.conveyorIndices[i]];
				float num = Time.deltaTime * builderConveyor2.GetFrameMovement();
				float num2 = this.jobSplineTimes[i] + num;
				this.jobSplineTimes[i] = Mathf.Clamp(num2, 0f, 1f);
				if (PhotonNetwork.IsMasterClient && (!flag || flag2) && (double)num2 > 0.999)
				{
					builderConveyor2.RemovePieceFromConveyor(this.pieceTransforms[i]);
					this.RemovePieceFromJobAtIndex(i);
					flag = true;
				}
			}
			for (int j = this.shelfSlice; j < this.table.conveyors.Count; j += BuilderTable.SHELF_SLICE_BUCKETS)
			{
				this.table.conveyors[j].UpdateShelfSliced();
			}
			this.shelfSlice = (this.shelfSlice + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
		}

		// Token: 0x06005B05 RID: 23301 RVA: 0x001D27D8 File Offset: 0x001D09D8
		public void Setup(BuilderTable mytable)
		{
			if (this.isSetup)
			{
				return;
			}
			this.table = mytable;
			this.conveyorSplines = new NativeArray<NativeSpline>(this.table.conveyors.Count, 4, 1);
			this.conveyorRotations = new NativeArray<Quaternion>(this.table.conveyors.Count, 4, 1);
			int num = 0;
			for (int i = 0; i < this.table.conveyors.Count; i++)
			{
				this.conveyorSplines[i] = this.table.conveyors[i].nativeSpline;
				this.conveyorRotations[i] = this.table.conveyors[i].GetSpawnTransform().rotation;
				num += this.table.conveyors[i].GetMaxItemsOnConveyor();
			}
			this.maxItemCount = num;
			this.conveyorIndices = new NativeList<int>(this.maxItemCount, 4);
			this.jobSplineTimes = new NativeList<float>(this.maxItemCount, 4);
			this.jobShelfOffsets = new NativeList<Vector3>(this.maxItemCount, 4);
			this.pieceTransforms = new TransformAccessArray(this.maxItemCount, 3);
			this.isSetup = true;
		}

		// Token: 0x06005B06 RID: 23302 RVA: 0x001D2914 File Offset: 0x001D0B14
		public float GetSplineProgressForPiece(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					return this.jobSplineTimes[i];
				}
			}
			return 1f;
		}

		// Token: 0x06005B07 RID: 23303 RVA: 0x001D2964 File Offset: 0x001D0B64
		public int GetPieceCreateTimestamp(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderConveyor builderConveyor = this.table.conveyors[this.conveyorIndices[i]];
					int num = Mathf.RoundToInt(this.jobSplineTimes[i] / builderConveyor.GetFrameMovement() * 1000f);
					return PhotonNetwork.ServerTimestamp - num;
				}
			}
			return PhotonNetwork.ServerTimestamp - 5000;
		}

		// Token: 0x06005B08 RID: 23304 RVA: 0x001D29F0 File Offset: 0x001D0BF0
		public void OnClearTable()
		{
			if (!this.isSetup)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.OnClearTable();
			}
			for (int i = this.pieceTransforms.length - 1; i >= 0; i--)
			{
				this.pieceTransforms.RemoveAtSwapBack(i);
			}
			this.jobSplineTimes.Clear();
			this.jobShelfOffsets.Clear();
			this.conveyorIndices.Clear();
		}

		// Token: 0x06005B09 RID: 23305 RVA: 0x001D2A94 File Offset: 0x001D0C94
		private void OnDestroy()
		{
			this.conveyorSplines.Dispose();
			this.conveyorRotations.Dispose();
			this.conveyorIndices.Dispose();
			this.jobSplineTimes.Dispose();
			this.jobShelfOffsets.Dispose();
			this.pieceTransforms.Dispose();
		}

		// Token: 0x06005B0A RID: 23306 RVA: 0x001D2AE4 File Offset: 0x001D0CE4
		public JobHandle ConstructJobHandle()
		{
			BuilderConveyorManager.EvaluateSplineJob evaluateSplineJob = new BuilderConveyorManager.EvaluateSplineJob
			{
				conveyorRotations = this.conveyorRotations,
				conveyorIndices = this.conveyorIndices,
				shelfOffsets = this.jobShelfOffsets,
				splineTimes = this.jobSplineTimes
			};
			for (int i = 0; i < this.conveyorSplines.Length; i++)
			{
				evaluateSplineJob.SetSplineAt(i, this.conveyorSplines[i]);
			}
			return IJobParallelForTransformExtensions.Schedule<BuilderConveyorManager.EvaluateSplineJob>(evaluateSplineJob, this.pieceTransforms, default(JobHandle));
		}

		// Token: 0x06005B0B RID: 23307 RVA: 0x001D2B70 File Offset: 0x001D0D70
		public void AddPieceToJob(BuilderPiece piece, float splineTime, int conveyorID)
		{
			if (this.pieceTransforms.length >= this.pieceTransforms.capacity)
			{
				Debug.LogError("Too many pieces on conveyor!");
			}
			this.pieceTransforms.Add(piece.transform);
			this.conveyorIndices.Add(ref conveyorID);
			this.jobShelfOffsets.Add(ref piece.desiredShelfOffset);
			this.jobSplineTimes.Add(ref splineTime);
		}

		// Token: 0x06005B0C RID: 23308 RVA: 0x001D2BDB File Offset: 0x001D0DDB
		public void RemovePieceFromJobAtIndex(int index)
		{
			BuilderRenderer.RemoveAt(this.pieceTransforms, index);
			this.jobShelfOffsets.RemoveAt(index);
			this.jobSplineTimes.RemoveAt(index);
			this.conveyorIndices.RemoveAt(index);
		}

		// Token: 0x06005B0D RID: 23309 RVA: 0x001D2C10 File Offset: 0x001D0E10
		public void RemovePieceFromJob(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderRenderer.RemoveAt(this.pieceTransforms, i);
					this.jobShelfOffsets.RemoveAt(i);
					this.jobSplineTimes.RemoveAt(i);
					this.conveyorIndices.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x04006823 RID: 26659
		private NativeArray<NativeSpline> conveyorSplines;

		// Token: 0x04006824 RID: 26660
		private NativeArray<Quaternion> conveyorRotations;

		// Token: 0x04006825 RID: 26661
		private NativeList<int> conveyorIndices;

		// Token: 0x04006826 RID: 26662
		private NativeList<float> jobSplineTimes;

		// Token: 0x04006827 RID: 26663
		private NativeList<Vector3> jobShelfOffsets;

		// Token: 0x04006828 RID: 26664
		private TransformAccessArray pieceTransforms;

		// Token: 0x04006829 RID: 26665
		private BuilderTable table;

		// Token: 0x0400682A RID: 26666
		private bool isSetup;

		// Token: 0x0400682B RID: 26667
		private int maxItemCount;

		// Token: 0x0400682C RID: 26668
		private int shelfSlice;

		// Token: 0x02000E46 RID: 3654
		[BurstCompile]
		public struct EvaluateSplineJob : IJobParallelForTransform
		{
			// Token: 0x06005B0F RID: 23311 RVA: 0x001D2C7D File Offset: 0x001D0E7D
			public NativeSpline GetSplineAt(int index)
			{
				switch (index)
				{
				case 0:
					return this.conveyorSpline0;
				case 1:
					return this.conveyorSpline1;
				case 2:
					return this.conveyorSpline2;
				case 3:
					return this.conveyorSpline3;
				default:
					return this.conveyorSpline0;
				}
			}

			// Token: 0x06005B10 RID: 23312 RVA: 0x001D2CB9 File Offset: 0x001D0EB9
			public void SetSplineAt(int index, NativeSpline s)
			{
				switch (index)
				{
				case 0:
					this.conveyorSpline0 = s;
					return;
				case 1:
					this.conveyorSpline1 = s;
					return;
				case 2:
					this.conveyorSpline2 = s;
					return;
				case 3:
					this.conveyorSpline3 = s;
					return;
				default:
					return;
				}
			}

			// Token: 0x06005B11 RID: 23313 RVA: 0x001D2CF4 File Offset: 0x001D0EF4
			public void Execute(int index, TransformAccess transform)
			{
				float num = this.splineTimes[index];
				Vector3 vector = this.shelfOffsets[index];
				int num2 = this.conveyorIndices[index];
				NativeSpline splineAt = this.GetSplineAt(num2);
				Quaternion quaternion = this.conveyorRotations[num2];
				float num3;
				Vector3 position = CurveUtility.EvaluatePosition(splineAt.GetCurve(SplineUtility.SplineToCurveT<NativeSpline>(splineAt, num, ref num3)), num3) + quaternion * vector;
				transform.position = position;
			}

			// Token: 0x0400682E RID: 26670
			public NativeSpline conveyorSpline0;

			// Token: 0x0400682F RID: 26671
			public NativeSpline conveyorSpline1;

			// Token: 0x04006830 RID: 26672
			public NativeSpline conveyorSpline2;

			// Token: 0x04006831 RID: 26673
			public NativeSpline conveyorSpline3;

			// Token: 0x04006832 RID: 26674
			[ReadOnly]
			public NativeArray<Quaternion> conveyorRotations;

			// Token: 0x04006833 RID: 26675
			[ReadOnly]
			public NativeList<int> conveyorIndices;

			// Token: 0x04006834 RID: 26676
			[ReadOnly]
			public NativeList<float> splineTimes;

			// Token: 0x04006835 RID: 26677
			[ReadOnly]
			public NativeList<Vector3> shelfOffsets;
		}
	}
}
