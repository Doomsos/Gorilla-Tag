using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011AD RID: 4525
	public static class BoingWorkAsynchronous
	{
		// Token: 0x0600720E RID: 29198 RVA: 0x002573BC File Offset: 0x002555BC
		internal static void PostUnregisterBehaviorCleanUp()
		{
			if (BoingWorkAsynchronous.s_behaviorJobNeedsGather)
			{
				BoingWorkAsynchronous.s_hBehaviorJob.Complete();
				BoingWorkAsynchronous.s_aBehaviorParams.Dispose();
				BoingWorkAsynchronous.s_aBehaviorOutput.Dispose();
				BoingWorkAsynchronous.s_behaviorJobNeedsGather = false;
			}
		}

		// Token: 0x0600720F RID: 29199 RVA: 0x002573E9 File Offset: 0x002555E9
		internal static void PostUnregisterEffectorReactorCleanUp()
		{
			if (BoingWorkAsynchronous.s_reactorJobNeedsGather)
			{
				BoingWorkAsynchronous.s_hReactorJob.Complete();
				BoingWorkAsynchronous.s_aEffectors.Dispose();
				BoingWorkAsynchronous.s_aReactorExecParams.Dispose();
				BoingWorkAsynchronous.s_aReactorExecOutput.Dispose();
				BoingWorkAsynchronous.s_reactorJobNeedsGather = false;
			}
		}

		// Token: 0x06007210 RID: 29200 RVA: 0x00257420 File Offset: 0x00255620
		internal static void ExecuteBehaviors(Dictionary<int, BoingBehavior> behaviorMap, BoingManager.UpdateMode updateMode)
		{
			int num = 0;
			BoingWorkAsynchronous.s_aBehaviorParams = new NativeArray<BoingWork.Params>(behaviorMap.Count, 3, 0);
			BoingWorkAsynchronous.s_aBehaviorOutput = new NativeArray<BoingWork.Output>(behaviorMap.Count, 3, 0);
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					BoingWorkAsynchronous.s_aBehaviorParams[num++] = value.Params;
				}
			}
			if (num > 0)
			{
				BoingWorkAsynchronous.BehaviorJob behaviorJob = new BoingWorkAsynchronous.BehaviorJob
				{
					Params = BoingWorkAsynchronous.s_aBehaviorParams,
					Output = BoingWorkAsynchronous.s_aBehaviorOutput,
					DeltaTime = BoingManager.DeltaTime,
					FixedDeltaTime = BoingManager.FixedDeltaTime
				};
				int num2 = (int)Mathf.Ceil((float)num / (float)Environment.ProcessorCount);
				BoingWorkAsynchronous.s_hBehaviorJob = IJobParallelForExtensions.Schedule<BoingWorkAsynchronous.BehaviorJob>(behaviorJob, num, num2, default(JobHandle));
				JobHandle.ScheduleBatchedJobs();
			}
			BoingWorkAsynchronous.s_behaviorJobNeedsGather = true;
			if (BoingWorkAsynchronous.s_behaviorJobNeedsGather)
			{
				if (num > 0)
				{
					BoingWorkAsynchronous.s_hBehaviorJob.Complete();
					for (int i = 0; i < num; i++)
					{
						BoingWorkAsynchronous.s_aBehaviorOutput[i].GatherOutput(behaviorMap, updateMode);
					}
				}
				BoingWorkAsynchronous.s_aBehaviorParams.Dispose();
				BoingWorkAsynchronous.s_aBehaviorOutput.Dispose();
				BoingWorkAsynchronous.s_behaviorJobNeedsGather = false;
			}
		}

		// Token: 0x06007211 RID: 29201 RVA: 0x00257580 File Offset: 0x00255780
		internal static void ExecuteReactors(Dictionary<int, BoingEffector> effectorMap, Dictionary<int, BoingReactor> reactorMap, Dictionary<int, BoingReactorField> fieldMap, Dictionary<int, BoingReactorFieldCPUSampler> cpuSamplerMap, BoingManager.UpdateMode updateMode)
		{
			int num = 0;
			BoingWorkAsynchronous.s_aEffectors = new NativeArray<BoingEffector.Params>(effectorMap.Count, 3, 0);
			BoingWorkAsynchronous.s_aReactorExecParams = new NativeArray<BoingWork.Params>(reactorMap.Count, 3, 0);
			BoingWorkAsynchronous.s_aReactorExecOutput = new NativeArray<BoingWork.Output>(reactorMap.Count, 3, 0);
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					BoingWorkAsynchronous.s_aReactorExecParams[num++] = value.Params;
				}
			}
			if (num > 0)
			{
				int num2 = 0;
				BoingEffector.Params @params = default(BoingEffector.Params);
				foreach (KeyValuePair<int, BoingEffector> keyValuePair2 in effectorMap)
				{
					BoingEffector value2 = keyValuePair2.Value;
					@params.Fill(keyValuePair2.Value);
					BoingWorkAsynchronous.s_aEffectors[num2++] = @params;
				}
			}
			if (num > 0)
			{
				BoingWorkAsynchronous.s_hReactorJob = IJobParallelForExtensions.Schedule<BoingWorkAsynchronous.ReactorJob>(new BoingWorkAsynchronous.ReactorJob
				{
					Effectors = BoingWorkAsynchronous.s_aEffectors,
					Params = BoingWorkAsynchronous.s_aReactorExecParams,
					Output = BoingWorkAsynchronous.s_aReactorExecOutput,
					DeltaTime = BoingManager.DeltaTime,
					FixedDeltaTime = BoingManager.FixedDeltaTime
				}, num, 32, default(JobHandle));
				JobHandle.ScheduleBatchedJobs();
			}
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair3 in fieldMap)
			{
				BoingReactorField value3 = keyValuePair3.Value;
				if (value3.HardwareMode == BoingReactorField.HardwareModeEnum.CPU)
				{
					value3.ExecuteCpu(BoingManager.DeltaTime);
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair4 in cpuSamplerMap)
			{
				BoingReactorFieldCPUSampler value4 = keyValuePair4.Value;
			}
			BoingWorkAsynchronous.s_reactorJobNeedsGather = true;
			if (BoingWorkAsynchronous.s_reactorJobNeedsGather)
			{
				if (num > 0)
				{
					BoingWorkAsynchronous.s_hReactorJob.Complete();
					for (int i = 0; i < num; i++)
					{
						BoingWorkAsynchronous.s_aReactorExecOutput[i].GatherOutput(reactorMap, updateMode);
					}
				}
				BoingWorkAsynchronous.s_aEffectors.Dispose();
				BoingWorkAsynchronous.s_aReactorExecParams.Dispose();
				BoingWorkAsynchronous.s_aReactorExecOutput.Dispose();
				BoingWorkAsynchronous.s_reactorJobNeedsGather = false;
			}
		}

		// Token: 0x06007212 RID: 29202 RVA: 0x00257800 File Offset: 0x00255A00
		internal static void ExecuteBones(BoingEffector.Params[] aEffectorParams, Dictionary<int, BoingBones> bonesMap, BoingManager.UpdateMode updateMode)
		{
			float deltaTime = BoingManager.DeltaTime;
			foreach (KeyValuePair<int, BoingBones> keyValuePair in bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					if (aEffectorParams != null)
					{
						for (int i = 0; i < aEffectorParams.Length; i++)
						{
							value.AccumulateTarget(ref aEffectorParams[i], deltaTime);
						}
					}
					value.EndAccumulateTargets();
					BoingManager.UpdateMode updateMode2 = value.UpdateMode;
					if (updateMode2 != BoingManager.UpdateMode.FixedUpdate)
					{
						if (updateMode2 - BoingManager.UpdateMode.EarlyUpdate <= 1)
						{
							value.Params.Execute(value, BoingManager.DeltaTime);
						}
					}
					else
					{
						value.Params.Execute(value, BoingManager.FixedDeltaTime);
					}
				}
			}
		}

		// Token: 0x06007213 RID: 29203 RVA: 0x002578CC File Offset: 0x00255ACC
		internal static void PullBonesResults(BoingEffector.Params[] aEffectorParams, Dictionary<int, BoingBones> bonesMap, BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.Params.PullResults(value);
				}
			}
		}

		// Token: 0x040082A8 RID: 33448
		private static bool s_behaviorJobNeedsGather;

		// Token: 0x040082A9 RID: 33449
		private static JobHandle s_hBehaviorJob;

		// Token: 0x040082AA RID: 33450
		private static NativeArray<BoingWork.Params> s_aBehaviorParams;

		// Token: 0x040082AB RID: 33451
		private static NativeArray<BoingWork.Output> s_aBehaviorOutput;

		// Token: 0x040082AC RID: 33452
		private static bool s_reactorJobNeedsGather;

		// Token: 0x040082AD RID: 33453
		private static JobHandle s_hReactorJob;

		// Token: 0x040082AE RID: 33454
		private static NativeArray<BoingEffector.Params> s_aEffectors;

		// Token: 0x040082AF RID: 33455
		private static NativeArray<BoingWork.Params> s_aReactorExecParams;

		// Token: 0x040082B0 RID: 33456
		private static NativeArray<BoingWork.Output> s_aReactorExecOutput;

		// Token: 0x020011AE RID: 4526
		private struct BehaviorJob : IJobParallelFor
		{
			// Token: 0x06007214 RID: 29204 RVA: 0x00257930 File Offset: 0x00255B30
			public void Execute(int index)
			{
				BoingWork.Params @params = this.Params[index];
				if (@params.Bits.IsBitSet(9))
				{
					@params.Execute(this.FixedDeltaTime);
				}
				else
				{
					@params.Execute(this.DeltaTime);
				}
				this.Output[index] = new BoingWork.Output(@params.InstanceID, ref @params.Instance.PositionSpring, ref @params.Instance.RotationSpring, ref @params.Instance.ScaleSpring);
			}

			// Token: 0x040082B1 RID: 33457
			public NativeArray<BoingWork.Params> Params;

			// Token: 0x040082B2 RID: 33458
			public NativeArray<BoingWork.Output> Output;

			// Token: 0x040082B3 RID: 33459
			public float DeltaTime;

			// Token: 0x040082B4 RID: 33460
			public float FixedDeltaTime;
		}

		// Token: 0x020011AF RID: 4527
		private struct ReactorJob : IJobParallelFor
		{
			// Token: 0x06007215 RID: 29205 RVA: 0x002579B4 File Offset: 0x00255BB4
			public void Execute(int index)
			{
				BoingWork.Params @params = this.Params[index];
				int i = 0;
				int length = this.Effectors.Length;
				while (i < length)
				{
					BoingEffector.Params params2 = this.Effectors[i];
					@params.AccumulateTarget(ref params2, this.DeltaTime);
					i++;
				}
				@params.EndAccumulateTargets();
				if (@params.Bits.IsBitSet(9))
				{
					@params.Execute(this.FixedDeltaTime);
				}
				else
				{
					@params.Execute(BoingManager.DeltaTime);
				}
				this.Output[index] = new BoingWork.Output(@params.InstanceID, ref @params.Instance.PositionSpring, ref @params.Instance.RotationSpring, ref @params.Instance.ScaleSpring);
			}

			// Token: 0x040082B5 RID: 33461
			[ReadOnly]
			public NativeArray<BoingEffector.Params> Effectors;

			// Token: 0x040082B6 RID: 33462
			public NativeArray<BoingWork.Params> Params;

			// Token: 0x040082B7 RID: 33463
			public NativeArray<BoingWork.Output> Output;

			// Token: 0x040082B8 RID: 33464
			public float DeltaTime;

			// Token: 0x040082B9 RID: 33465
			public float FixedDeltaTime;
		}
	}
}
