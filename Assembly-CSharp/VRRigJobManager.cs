using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x020009B7 RID: 2487
[DefaultExecutionOrder(0)]
public class VRRigJobManager : MonoBehaviour
{
	// Token: 0x170005CE RID: 1486
	// (get) Token: 0x06003F9A RID: 16282 RVA: 0x0015516D File Offset: 0x0015336D
	public static VRRigJobManager Instance
	{
		get
		{
			return VRRigJobManager._instance;
		}
	}

	// Token: 0x06003F9B RID: 16283 RVA: 0x00155174 File Offset: 0x00153374
	private void Awake()
	{
		VRRigJobManager._instance = this;
		this.cachedInput = new NativeArray<VRRigJobManager.VRRigTransformInput>(9, 4, 1);
		this.tAA = new TransformAccessArray(9, 2);
		this.job = default(VRRigJobManager.VRRigTransformJob);
	}

	// Token: 0x06003F9C RID: 16284 RVA: 0x001551A5 File Offset: 0x001533A5
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.cachedInput.Dispose();
		this.tAA.Dispose();
	}

	// Token: 0x06003F9D RID: 16285 RVA: 0x001551C8 File Offset: 0x001533C8
	public void RegisterVRRig(VRRig rig)
	{
		this.rigList.Add(rig);
		this.tAA.Add(rig.transform);
		this.actualListSz++;
	}

	// Token: 0x06003F9E RID: 16286 RVA: 0x001551F8 File Offset: 0x001533F8
	public void DeregisterVRRig(VRRig rig)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.rigList.Remove(rig);
		for (int i = this.actualListSz - 1; i >= 0; i--)
		{
			if (this.tAA[i] == rig.transform)
			{
				this.tAA.RemoveAtSwapBack(i);
				break;
			}
		}
		this.actualListSz--;
	}

	// Token: 0x06003F9F RID: 16287 RVA: 0x00155264 File Offset: 0x00153464
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.cachedInput[i] = new VRRigJobManager.VRRigTransformInput
			{
				rigPosition = this.rigList[i].jobPos,
				rigRotaton = this.rigList[i].jobRotation
			};
			this.tAA[i] = this.rigList[i].transform;
		}
	}

	// Token: 0x06003FA0 RID: 16288 RVA: 0x001552E4 File Offset: 0x001534E4
	public void Update()
	{
		this.jobHandle.Complete();
		for (int i = 0; i < this.rigList.Count; i++)
		{
			this.rigList[i].RemoteRigUpdate();
		}
		this.CopyInput();
		this.job.input = this.cachedInput;
		this.jobHandle = IJobParallelForTransformExtensions.Schedule<VRRigJobManager.VRRigTransformJob>(this.job, this.tAA, default(JobHandle));
	}

	// Token: 0x040050C1 RID: 20673
	[OnEnterPlay_SetNull]
	private static VRRigJobManager _instance;

	// Token: 0x040050C2 RID: 20674
	private const int MaxSize = 9;

	// Token: 0x040050C3 RID: 20675
	private const int questJobThreads = 2;

	// Token: 0x040050C4 RID: 20676
	private List<VRRig> rigList = new List<VRRig>(9);

	// Token: 0x040050C5 RID: 20677
	private NativeArray<VRRigJobManager.VRRigTransformInput> cachedInput;

	// Token: 0x040050C6 RID: 20678
	private TransformAccessArray tAA;

	// Token: 0x040050C7 RID: 20679
	private int actualListSz;

	// Token: 0x040050C8 RID: 20680
	private JobHandle jobHandle;

	// Token: 0x040050C9 RID: 20681
	private VRRigJobManager.VRRigTransformJob job;

	// Token: 0x020009B8 RID: 2488
	private struct VRRigTransformInput
	{
		// Token: 0x040050CA RID: 20682
		public Vector3 rigPosition;

		// Token: 0x040050CB RID: 20683
		public Quaternion rigRotaton;
	}

	// Token: 0x020009B9 RID: 2489
	[BurstCompile]
	private struct VRRigTransformJob : IJobParallelForTransform
	{
		// Token: 0x06003FA2 RID: 16290 RVA: 0x0015536F File Offset: 0x0015356F
		public void Execute(int i, TransformAccess tA)
		{
			if (i < this.input.Length)
			{
				tA.position = this.input[i].rigPosition;
				tA.rotation = this.input[i].rigRotaton;
			}
		}

		// Token: 0x040050CC RID: 20684
		[ReadOnly]
		public NativeArray<VRRigJobManager.VRRigTransformInput> input;
	}
}
