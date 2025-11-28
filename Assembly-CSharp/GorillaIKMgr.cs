using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000793 RID: 1939
public class GorillaIKMgr : MonoBehaviour
{
	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x060032E1 RID: 13025 RVA: 0x00112AAC File Offset: 0x00110CAC
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	// Token: 0x060032E2 RID: 13026 RVA: 0x00112AB4 File Offset: 0x00110CB4
	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.firstFrame = true;
		this.tAA = new TransformAccessArray(0, -1);
		this.transformList = new List<Transform>();
		this.job = new GorillaIKMgr.IKJob
		{
			constantInput = new NativeArray<GorillaIKMgr.IKConstantInput>(40, 4, 1),
			input = new NativeArray<GorillaIKMgr.IKInput>(40, 4, 1),
			output = new NativeArray<GorillaIKMgr.IKOutput>(40, 4, 1)
		};
		this.jobXform = new GorillaIKMgr.IKTransformJob
		{
			transformRotations = new NativeArray<Quaternion>(140, 4, 1)
		};
	}

	// Token: 0x060032E3 RID: 13027 RVA: 0x00112B48 File Offset: 0x00110D48
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.jobXformHandle.Complete();
		this.jobXform.transformRotations.Dispose();
		this.tAA.Dispose();
		this.job.input.Dispose();
		this.job.constantInput.Dispose();
		this.job.output.Dispose();
	}

	// Token: 0x060032E4 RID: 13028 RVA: 0x00112BB8 File Offset: 0x00110DB8
	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
		this.updatedSinceLastRun = true;
		if (this.job.constantInput.IsCreated)
		{
			this.SetConstantData(ik, this.actualListSz - 2);
		}
	}

	// Token: 0x060032E5 RID: 13029 RVA: 0x00112C08 File Offset: 0x00110E08
	public void DeregisterIK(GorillaIK ik)
	{
		int num = this.ikList.FindIndex((GorillaIK curr) => curr == ik);
		this.updatedSinceLastRun = true;
		this.ikList.RemoveAt(num);
		this.actualListSz -= 2;
		if (this.job.constantInput.IsCreated)
		{
			for (int i = num; i < this.actualListSz; i++)
			{
				this.job.constantInput[i] = this.job.constantInput[i + 2];
			}
		}
	}

	// Token: 0x060032E6 RID: 13030 RVA: 0x00112CA4 File Offset: 0x00110EA4
	private void SetConstantData(GorillaIK ik, int index)
	{
		this.job.constantInput[index] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerLeft,
			initRotUpper = ik.initialUpperLeft
		};
		this.job.constantInput[index + 1] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerRight,
			initRotUpper = ik.initialUpperRight
		};
	}

	// Token: 0x060032E7 RID: 13031 RVA: 0x00112D1C File Offset: 0x00110F1C
	private void CopyInput()
	{
		int num = 0;
		int i = 0;
		while (i < this.actualListSz)
		{
			GorillaIK gorillaIK = this.ikList[i / 2];
			this.job.input[i] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Left()
			};
			this.job.input[i + 1] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Right()
			};
			gorillaIK.ClearOverrides();
			i += 2;
			num++;
		}
	}

	// Token: 0x060032E8 RID: 13032 RVA: 0x00112DA8 File Offset: 0x00110FA8
	private void CopyOutput()
	{
		bool flag = false;
		if (this.updatedSinceLastRun || this.tAA.length != this.ikList.Count * 7)
		{
			flag = true;
			this.tAA.Dispose();
			this.transformList.Clear();
		}
		for (int i = 0; i < this.ikList.Count; i++)
		{
			GorillaIK gorillaIK = this.ikList[i];
			if (flag || this.updatedSinceLastRun)
			{
				this.transformList.Add(gorillaIK.leftUpperArm);
				this.transformList.Add(gorillaIK.leftLowerArm);
				this.transformList.Add(gorillaIK.rightUpperArm);
				this.transformList.Add(gorillaIK.rightLowerArm);
				this.transformList.Add(gorillaIK.headBone);
				this.transformList.Add(gorillaIK.leftHand);
				this.transformList.Add(gorillaIK.rightHand);
			}
			this.jobXform.transformRotations[this.tFormCount * i] = this.job.output[i * 2].upperArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 1] = this.job.output[i * 2].lowerArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 2] = this.job.output[i * 2 + 1].upperArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 3] = this.job.output[i * 2 + 1].lowerArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 4] = gorillaIK.targetHead.rotation;
			this.jobXform.transformRotations[this.tFormCount * i + 5] = gorillaIK.targetLeft.rotation;
			this.jobXform.transformRotations[this.tFormCount * i + 6] = gorillaIK.targetRight.rotation;
		}
		if (flag)
		{
			this.tAA = new TransformAccessArray(this.transformList.ToArray(), -1);
		}
		this.updatedSinceLastRun = false;
	}

	// Token: 0x060032E9 RID: 13033 RVA: 0x00112FF8 File Offset: 0x001111F8
	public void LateUpdate()
	{
		if (!this.firstFrame)
		{
			this.jobXformHandle.Complete();
		}
		this.CopyInput();
		this.jobHandle = IJobParallelForExtensions.Schedule<GorillaIKMgr.IKJob>(this.job, this.actualListSz, 20, default(JobHandle));
		this.jobHandle.Complete();
		this.CopyOutput();
		this.jobXformHandle = IJobParallelForTransformExtensions.Schedule<GorillaIKMgr.IKTransformJob>(this.jobXform, this.tAA, default(JobHandle));
		this.firstFrame = false;
	}

	// Token: 0x04004146 RID: 16710
	[OnEnterPlay_SetNull]
	private static GorillaIKMgr _instance;

	// Token: 0x04004147 RID: 16711
	private const int MaxSize = 20;

	// Token: 0x04004148 RID: 16712
	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	// Token: 0x04004149 RID: 16713
	private int actualListSz;

	// Token: 0x0400414A RID: 16714
	private JobHandle jobHandle;

	// Token: 0x0400414B RID: 16715
	private JobHandle jobXformHandle;

	// Token: 0x0400414C RID: 16716
	private bool firstFrame = true;

	// Token: 0x0400414D RID: 16717
	private TransformAccessArray tAA;

	// Token: 0x0400414E RID: 16718
	private List<Transform> transformList;

	// Token: 0x0400414F RID: 16719
	private bool updatedSinceLastRun;

	// Token: 0x04004150 RID: 16720
	private int tFormCount = 7;

	// Token: 0x04004151 RID: 16721
	private GorillaIKMgr.IKJob job;

	// Token: 0x04004152 RID: 16722
	private GorillaIKMgr.IKTransformJob jobXform;

	// Token: 0x02000794 RID: 1940
	private struct IKConstantInput
	{
		// Token: 0x04004153 RID: 16723
		public Quaternion initRotLower;

		// Token: 0x04004154 RID: 16724
		public Quaternion initRotUpper;
	}

	// Token: 0x02000795 RID: 1941
	private struct IKInput
	{
		// Token: 0x04004155 RID: 16725
		public Vector3 targetPos;
	}

	// Token: 0x02000796 RID: 1942
	private struct IKOutput
	{
		// Token: 0x060032EB RID: 13035 RVA: 0x0011309B File Offset: 0x0011129B
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
		}

		// Token: 0x04004156 RID: 16726
		public Quaternion upperArmLocalRot;

		// Token: 0x04004157 RID: 16727
		public Quaternion lowerArmLocalRot;
	}

	// Token: 0x02000797 RID: 1943
	[BurstCompile]
	private struct IKJob : IJobParallelFor
	{
		// Token: 0x060032EC RID: 13036 RVA: 0x001130AC File Offset: 0x001112AC
		public void Execute(int i)
		{
			Quaternion initRotUpper = this.constantInput[i].initRotUpper;
			Vector3 vector = GorillaIKMgr.IKJob.upperArmLocalPos;
			Quaternion quaternion = initRotUpper * this.constantInput[i].initRotLower;
			Vector3 vector2 = vector + initRotUpper * GorillaIKMgr.IKJob.forearmLocalPos;
			Vector3 vector3 = vector2 + quaternion * GorillaIKMgr.IKJob.handLocalPos;
			float num = 0f;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (vector2 - vector3).magnitude;
			float num2 = magnitude + magnitude2 - num;
			Vector3 normalized = (vector3 - vector).normalized;
			Vector3 normalized2 = (vector2 - vector).normalized;
			Vector3 normalized3 = (vector3 - vector2).normalized;
			Vector3 normalized4 = (this.input[i].targetPos - vector).normalized;
			float num3 = Mathf.Clamp((this.input[i].targetPos - vector).magnitude, num, num2);
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized2), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(-normalized2, normalized3), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized4), -1f, 1f));
			float num7 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num3 * num3) / (-2f * magnitude * num3), -1f, 1f));
			float num8 = Mathf.Acos(Mathf.Clamp((num3 * num3 - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized5 = Vector3.Cross(normalized, normalized2).normalized;
			Vector3 normalized6 = Vector3.Cross(normalized, normalized4).normalized;
			Quaternion quaternion2 = Quaternion.AngleAxis((num7 - num4) * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized5);
			Quaternion quaternion3 = Quaternion.AngleAxis((num8 - num5) * 57.29578f, Quaternion.Inverse(quaternion) * normalized5);
			Quaternion quaternion4 = Quaternion.AngleAxis(num6 * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized6);
			Quaternion upperArmLocalRot_ = this.constantInput[i].initRotUpper * quaternion4 * quaternion2;
			Quaternion lowerArmLocalRot_ = this.constantInput[i].initRotLower * quaternion3;
			this.output[i] = new GorillaIKMgr.IKOutput(upperArmLocalRot_, lowerArmLocalRot_);
		}

		// Token: 0x04004158 RID: 16728
		public NativeArray<GorillaIKMgr.IKConstantInput> constantInput;

		// Token: 0x04004159 RID: 16729
		public NativeArray<GorillaIKMgr.IKInput> input;

		// Token: 0x0400415A RID: 16730
		public NativeArray<GorillaIKMgr.IKOutput> output;

		// Token: 0x0400415B RID: 16731
		private static readonly Vector3 upperArmLocalPos = new Vector3(-0.0002577677f, 0.1454885f, -0.02598158f);

		// Token: 0x0400415C RID: 16732
		private static readonly Vector3 forearmLocalPos = new Vector3(4.204223E-06f, 0.4061671f, -1.043081E-06f);

		// Token: 0x0400415D RID: 16733
		private static readonly Vector3 handLocalPos = new Vector3(3.073364E-08f, 0.3816895f, 1.117587E-08f);
	}

	// Token: 0x02000798 RID: 1944
	[BurstCompile]
	private struct IKTransformJob : IJobParallelForTransform
	{
		// Token: 0x060032EE RID: 13038 RVA: 0x001133B4 File Offset: 0x001115B4
		public void Execute(int index, TransformAccess xform)
		{
			if (index % 7 <= 3)
			{
				xform.localRotation = this.transformRotations[index];
				return;
			}
			xform.rotation = this.transformRotations[index];
		}

		// Token: 0x0400415E RID: 16734
		public NativeArray<Quaternion> transformRotations;
	}
}
