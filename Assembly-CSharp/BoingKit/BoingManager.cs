using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001189 RID: 4489
	public static class BoingManager
	{
		// Token: 0x17000A98 RID: 2712
		// (get) Token: 0x06007147 RID: 28999 RVA: 0x00251E14 File Offset: 0x00250014
		public static IEnumerable<BoingBehavior> Behaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Values;
			}
		}

		// Token: 0x17000A99 RID: 2713
		// (get) Token: 0x06007148 RID: 29000 RVA: 0x00251E20 File Offset: 0x00250020
		public static IEnumerable<BoingReactor> Reactors
		{
			get
			{
				return BoingManager.s_reactorMap.Values;
			}
		}

		// Token: 0x17000A9A RID: 2714
		// (get) Token: 0x06007149 RID: 29001 RVA: 0x00251E2C File Offset: 0x0025002C
		public static IEnumerable<BoingEffector> Effectors
		{
			get
			{
				return BoingManager.s_effectorMap.Values;
			}
		}

		// Token: 0x17000A9B RID: 2715
		// (get) Token: 0x0600714A RID: 29002 RVA: 0x00251E38 File Offset: 0x00250038
		public static IEnumerable<BoingReactorField> ReactorFields
		{
			get
			{
				return BoingManager.s_fieldMap.Values;
			}
		}

		// Token: 0x17000A9C RID: 2716
		// (get) Token: 0x0600714B RID: 29003 RVA: 0x00251E44 File Offset: 0x00250044
		public static IEnumerable<BoingReactorFieldCPUSampler> ReactorFieldCPUSamlers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Values;
			}
		}

		// Token: 0x17000A9D RID: 2717
		// (get) Token: 0x0600714C RID: 29004 RVA: 0x00251E50 File Offset: 0x00250050
		public static IEnumerable<BoingReactorFieldGPUSampler> ReactorFieldGPUSampler
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Values;
			}
		}

		// Token: 0x17000A9E RID: 2718
		// (get) Token: 0x0600714D RID: 29005 RVA: 0x00251E5C File Offset: 0x0025005C
		public static float DeltaTime
		{
			get
			{
				return BoingManager.s_deltaTime;
			}
		}

		// Token: 0x17000A9F RID: 2719
		// (get) Token: 0x0600714E RID: 29006 RVA: 0x00251E63 File Offset: 0x00250063
		public static float FixedDeltaTime
		{
			get
			{
				return Time.fixedDeltaTime;
			}
		}

		// Token: 0x17000AA0 RID: 2720
		// (get) Token: 0x0600714F RID: 29007 RVA: 0x00251E6A File Offset: 0x0025006A
		internal static int NumBehaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Count;
			}
		}

		// Token: 0x17000AA1 RID: 2721
		// (get) Token: 0x06007150 RID: 29008 RVA: 0x00251E76 File Offset: 0x00250076
		internal static int NumEffectors
		{
			get
			{
				return BoingManager.s_effectorMap.Count;
			}
		}

		// Token: 0x17000AA2 RID: 2722
		// (get) Token: 0x06007151 RID: 29009 RVA: 0x00251E82 File Offset: 0x00250082
		internal static int NumReactors
		{
			get
			{
				return BoingManager.s_reactorMap.Count;
			}
		}

		// Token: 0x17000AA3 RID: 2723
		// (get) Token: 0x06007152 RID: 29010 RVA: 0x00251E8E File Offset: 0x0025008E
		internal static int NumFields
		{
			get
			{
				return BoingManager.s_fieldMap.Count;
			}
		}

		// Token: 0x17000AA4 RID: 2724
		// (get) Token: 0x06007153 RID: 29011 RVA: 0x00251E9A File Offset: 0x0025009A
		internal static int NumCPUFieldSamplers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Count;
			}
		}

		// Token: 0x17000AA5 RID: 2725
		// (get) Token: 0x06007154 RID: 29012 RVA: 0x00251EA6 File Offset: 0x002500A6
		internal static int NumGPUFieldSamplers
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Count;
			}
		}

		// Token: 0x06007155 RID: 29013 RVA: 0x00251EB4 File Offset: 0x002500B4
		private static void ValidateManager()
		{
			if (BoingManager.s_managerGo != null)
			{
				return;
			}
			BoingManager.s_managerGo = new GameObject("Boing Kit manager (don't delete)");
			BoingManager.s_managerGo.AddComponent<BoingManagerPreUpdatePump>();
			BoingManager.s_managerGo.AddComponent<BoingManagerPostUpdatePump>();
			Object.DontDestroyOnLoad(BoingManager.s_managerGo);
			BoingManager.s_managerGo.AddComponent<SphereCollider>().enabled = false;
		}

		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x06007156 RID: 29014 RVA: 0x00251F0E File Offset: 0x0025010E
		internal static SphereCollider SharedSphereCollider
		{
			get
			{
				if (BoingManager.s_managerGo == null)
				{
					return null;
				}
				return BoingManager.s_managerGo.GetComponent<SphereCollider>();
			}
		}

		// Token: 0x06007157 RID: 29015 RVA: 0x00251F29 File Offset: 0x00250129
		internal static void Register(BoingBehavior behavior)
		{
			BoingManager.PreRegisterBehavior();
			BoingManager.s_behaviorMap.Add(behavior.GetInstanceID(), behavior);
			if (BoingManager.OnBehaviorRegister != null)
			{
				BoingManager.OnBehaviorRegister(behavior);
			}
		}

		// Token: 0x06007158 RID: 29016 RVA: 0x00251F53 File Offset: 0x00250153
		internal static void Unregister(BoingBehavior behavior)
		{
			if (BoingManager.OnBehaviorUnregister != null)
			{
				BoingManager.OnBehaviorUnregister(behavior);
			}
			BoingManager.s_behaviorMap.Remove(behavior.GetInstanceID());
			BoingManager.PostUnregisterBehavior();
		}

		// Token: 0x06007159 RID: 29017 RVA: 0x00251F7D File Offset: 0x0025017D
		internal static void Register(BoingEffector effector)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_effectorMap.Add(effector.GetInstanceID(), effector);
			if (BoingManager.OnEffectorRegister != null)
			{
				BoingManager.OnEffectorRegister(effector);
			}
		}

		// Token: 0x0600715A RID: 29018 RVA: 0x00251FA7 File Offset: 0x002501A7
		internal static void Unregister(BoingEffector effector)
		{
			if (BoingManager.OnEffectorUnregister != null)
			{
				BoingManager.OnEffectorUnregister(effector);
			}
			BoingManager.s_effectorMap.Remove(effector.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x0600715B RID: 29019 RVA: 0x00251FD1 File Offset: 0x002501D1
		internal static void Register(BoingReactor reactor)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_reactorMap.Add(reactor.GetInstanceID(), reactor);
			if (BoingManager.OnReactorRegister != null)
			{
				BoingManager.OnReactorRegister(reactor);
			}
		}

		// Token: 0x0600715C RID: 29020 RVA: 0x00251FFB File Offset: 0x002501FB
		internal static void Unregister(BoingReactor reactor)
		{
			if (BoingManager.OnReactorUnregister != null)
			{
				BoingManager.OnReactorUnregister(reactor);
			}
			BoingManager.s_reactorMap.Remove(reactor.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x0600715D RID: 29021 RVA: 0x00252025 File Offset: 0x00250225
		internal static void Register(BoingReactorField field)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_fieldMap.Add(field.GetInstanceID(), field);
			if (BoingManager.OnReactorFieldRegister != null)
			{
				BoingManager.OnReactorFieldRegister(field);
			}
		}

		// Token: 0x0600715E RID: 29022 RVA: 0x0025204F File Offset: 0x0025024F
		internal static void Unregister(BoingReactorField field)
		{
			if (BoingManager.OnReactorFieldUnregister != null)
			{
				BoingManager.OnReactorFieldUnregister(field);
			}
			BoingManager.s_fieldMap.Remove(field.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x0600715F RID: 29023 RVA: 0x00252079 File Offset: 0x00250279
		internal static void Register(BoingReactorFieldCPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_cpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldCPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
		}

		// Token: 0x06007160 RID: 29024 RVA: 0x002520A3 File Offset: 0x002502A3
		internal static void Unregister(BoingReactorFieldCPUSampler sampler)
		{
			if (BoingManager.OnReactorFieldCPUSamplerUnregister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
			BoingManager.s_cpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06007161 RID: 29025 RVA: 0x002520CD File Offset: 0x002502CD
		internal static void Register(BoingReactorFieldGPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_gpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldGPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldGPUSamplerRegister(sampler);
			}
		}

		// Token: 0x06007162 RID: 29026 RVA: 0x002520F7 File Offset: 0x002502F7
		internal static void Unregister(BoingReactorFieldGPUSampler sampler)
		{
			if (BoingManager.OnFieldGPUSamplerUnregister != null)
			{
				BoingManager.OnFieldGPUSamplerUnregister(sampler);
			}
			BoingManager.s_gpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06007163 RID: 29027 RVA: 0x00252121 File Offset: 0x00250321
		internal static void Register(BoingBones bones)
		{
			BoingManager.PreRegisterBones();
			BoingManager.s_bonesMap.Add(bones.GetInstanceID(), bones);
			if (BoingManager.OnBonesRegister != null)
			{
				BoingManager.OnBonesRegister(bones);
			}
		}

		// Token: 0x06007164 RID: 29028 RVA: 0x0025214B File Offset: 0x0025034B
		internal static void Unregister(BoingBones bones)
		{
			if (BoingManager.OnBonesUnregister != null)
			{
				BoingManager.OnBonesUnregister(bones);
			}
			BoingManager.s_bonesMap.Remove(bones.GetInstanceID());
			BoingManager.PostUnregisterBones();
		}

		// Token: 0x06007165 RID: 29029 RVA: 0x00252175 File Offset: 0x00250375
		private static void PreRegisterBehavior()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06007166 RID: 29030 RVA: 0x0025217C File Offset: 0x0025037C
		private static void PostUnregisterBehavior()
		{
			if (BoingManager.s_behaviorMap.Count > 0)
			{
				return;
			}
			BoingWorkAsynchronous.PostUnregisterBehaviorCleanUp();
		}

		// Token: 0x06007167 RID: 29031 RVA: 0x00252194 File Offset: 0x00250394
		private static void PreRegisterEffectorReactor()
		{
			BoingManager.ValidateManager();
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				BoingManager.s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
			if (BoingManager.s_effectorMap.Count >= BoingManager.s_effectorParamsList.Capacity)
			{
				BoingManager.s_effectorParamsList.Capacity += BoingManager.kEffectorParamsIncrement;
				BoingManager.s_effectorParamsBuffer.Dispose();
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
		}

		// Token: 0x06007168 RID: 29032 RVA: 0x00252224 File Offset: 0x00250424
		private static void PostUnregisterEffectorReactor()
		{
			if (BoingManager.s_effectorMap.Count > 0 || BoingManager.s_reactorMap.Count > 0 || BoingManager.s_fieldMap.Count > 0 || BoingManager.s_cpuSamplerMap.Count > 0 || BoingManager.s_gpuSamplerMap.Count > 0)
			{
				return;
			}
			BoingManager.s_effectorParamsList = null;
			BoingManager.s_effectorParamsBuffer.Dispose();
			BoingManager.s_effectorParamsBuffer = null;
			BoingWorkAsynchronous.PostUnregisterEffectorReactorCleanUp();
		}

		// Token: 0x06007169 RID: 29033 RVA: 0x00252175 File Offset: 0x00250375
		private static void PreRegisterBones()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x0600716A RID: 29034 RVA: 0x00002789 File Offset: 0x00000989
		private static void PostUnregisterBones()
		{
		}

		// Token: 0x0600716B RID: 29035 RVA: 0x0025228E File Offset: 0x0025048E
		internal static void Execute(BoingManager.UpdateMode updateMode)
		{
			if (updateMode == BoingManager.UpdateMode.EarlyUpdate)
			{
				BoingManager.s_deltaTime = Time.deltaTime;
			}
			BoingManager.RefreshEffectorParams();
			BoingManager.ExecuteBones(updateMode);
			BoingManager.ExecuteBehaviors(updateMode);
			BoingManager.ExecuteReactors(updateMode);
		}

		// Token: 0x0600716C RID: 29036 RVA: 0x002522B8 File Offset: 0x002504B8
		internal static void ExecuteBehaviors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_behaviorMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
		}

		// Token: 0x0600716D RID: 29037 RVA: 0x0025234C File Offset: 0x0025054C
		internal static void PullBehaviorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
		}

		// Token: 0x0600716E RID: 29038 RVA: 0x002523B4 File Offset: 0x002505B4
		internal static void RestoreBehaviors()
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x0600716F RID: 29039 RVA: 0x0025240C File Offset: 0x0025060C
		internal static void RefreshEffectorParams()
		{
			if (BoingManager.s_effectorParamsList == null)
			{
				return;
			}
			BoingManager.s_effectorParamsIndexMap.Clear();
			BoingManager.s_effectorParamsList.Clear();
			foreach (KeyValuePair<int, BoingEffector> keyValuePair in BoingManager.s_effectorMap)
			{
				BoingEffector value = keyValuePair.Value;
				BoingManager.s_effectorParamsIndexMap.Add(value.GetInstanceID(), BoingManager.s_effectorParamsList.Count);
				BoingManager.s_effectorParamsList.Add(new BoingEffector.Params(value));
			}
			if (BoingManager.s_aEffectorParams == null || BoingManager.s_aEffectorParams.Length != BoingManager.s_effectorParamsList.Count)
			{
				BoingManager.s_aEffectorParams = BoingManager.s_effectorParamsList.ToArray();
				return;
			}
			BoingManager.s_effectorParamsList.CopyTo(BoingManager.s_aEffectorParams);
		}

		// Token: 0x06007170 RID: 29040 RVA: 0x002524E0 File Offset: 0x002506E0
		internal static void ExecuteReactors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_effectorMap.Count == 0 && BoingManager.s_reactorMap.Count == 0 && BoingManager.s_fieldMap.Count == 0 && BoingManager.s_cpuSamplerMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteReactors(BoingManager.s_effectorMap, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteReactors(BoingManager.s_aEffectorParams, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
		}

		// Token: 0x06007171 RID: 29041 RVA: 0x002525B8 File Offset: 0x002507B8
		internal static void PullReactorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				if (keyValuePair2.Value.UpdateMode == updateMode)
				{
					keyValuePair2.Value.SampleFromField();
				}
			}
		}

		// Token: 0x06007172 RID: 29042 RVA: 0x00252674 File Offset: 0x00250874
		internal static void RestoreReactors()
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				keyValuePair.Value.Restore();
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				keyValuePair2.Value.Restore();
			}
		}

		// Token: 0x06007173 RID: 29043 RVA: 0x00252714 File Offset: 0x00250914
		internal static void DispatchReactorFieldCompute()
		{
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				return;
			}
			BoingManager.s_effectorParamsBuffer.SetData(BoingManager.s_aEffectorParams);
			float deltaTime = Time.deltaTime;
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair in BoingManager.s_fieldMap)
			{
				BoingReactorField value = keyValuePair.Value;
				if (value.HardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					value.ExecuteGpu(deltaTime, BoingManager.s_effectorParamsBuffer, BoingManager.s_effectorParamsIndexMap);
				}
			}
		}

		// Token: 0x06007174 RID: 29044 RVA: 0x002527A0 File Offset: 0x002509A0
		internal static void ExecuteBones(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x06007175 RID: 29045 RVA: 0x00252840 File Offset: 0x00250A40
		internal static void PullBonesResults(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x06007176 RID: 29046 RVA: 0x00252878 File Offset: 0x00250A78
		internal static void RestoreBones()
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x040081C5 RID: 33221
		public static BoingManager.BehaviorRegisterDelegate OnBehaviorRegister;

		// Token: 0x040081C6 RID: 33222
		public static BoingManager.BehaviorUnregisterDelegate OnBehaviorUnregister;

		// Token: 0x040081C7 RID: 33223
		public static BoingManager.EffectorRegisterDelegate OnEffectorRegister;

		// Token: 0x040081C8 RID: 33224
		public static BoingManager.EffectorUnregisterDelegate OnEffectorUnregister;

		// Token: 0x040081C9 RID: 33225
		public static BoingManager.ReactorRegisterDelegate OnReactorRegister;

		// Token: 0x040081CA RID: 33226
		public static BoingManager.ReactorUnregisterDelegate OnReactorUnregister;

		// Token: 0x040081CB RID: 33227
		public static BoingManager.ReactorFieldRegisterDelegate OnReactorFieldRegister;

		// Token: 0x040081CC RID: 33228
		public static BoingManager.ReactorFieldUnregisterDelegate OnReactorFieldUnregister;

		// Token: 0x040081CD RID: 33229
		public static BoingManager.ReactorFieldCPUSamplerRegisterDelegate OnReactorFieldCPUSamplerRegister;

		// Token: 0x040081CE RID: 33230
		public static BoingManager.ReactorFieldCPUSamplerUnregisterDelegate OnReactorFieldCPUSamplerUnregister;

		// Token: 0x040081CF RID: 33231
		public static BoingManager.ReactorFieldGPUSamplerRegisterDelegate OnReactorFieldGPUSamplerRegister;

		// Token: 0x040081D0 RID: 33232
		public static BoingManager.ReactorFieldGPUSamplerUnregisterDelegate OnFieldGPUSamplerUnregister;

		// Token: 0x040081D1 RID: 33233
		public static BoingManager.BonesRegisterDelegate OnBonesRegister;

		// Token: 0x040081D2 RID: 33234
		public static BoingManager.BonesUnregisterDelegate OnBonesUnregister;

		// Token: 0x040081D3 RID: 33235
		private static float s_deltaTime = 0f;

		// Token: 0x040081D4 RID: 33236
		private static Dictionary<int, BoingBehavior> s_behaviorMap = new Dictionary<int, BoingBehavior>();

		// Token: 0x040081D5 RID: 33237
		private static Dictionary<int, BoingEffector> s_effectorMap = new Dictionary<int, BoingEffector>();

		// Token: 0x040081D6 RID: 33238
		private static Dictionary<int, BoingReactor> s_reactorMap = new Dictionary<int, BoingReactor>();

		// Token: 0x040081D7 RID: 33239
		private static Dictionary<int, BoingReactorField> s_fieldMap = new Dictionary<int, BoingReactorField>();

		// Token: 0x040081D8 RID: 33240
		private static Dictionary<int, BoingReactorFieldCPUSampler> s_cpuSamplerMap = new Dictionary<int, BoingReactorFieldCPUSampler>();

		// Token: 0x040081D9 RID: 33241
		private static Dictionary<int, BoingReactorFieldGPUSampler> s_gpuSamplerMap = new Dictionary<int, BoingReactorFieldGPUSampler>();

		// Token: 0x040081DA RID: 33242
		private static Dictionary<int, BoingBones> s_bonesMap = new Dictionary<int, BoingBones>();

		// Token: 0x040081DB RID: 33243
		private static readonly int kEffectorParamsIncrement = 16;

		// Token: 0x040081DC RID: 33244
		private static List<BoingEffector.Params> s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);

		// Token: 0x040081DD RID: 33245
		private static BoingEffector.Params[] s_aEffectorParams;

		// Token: 0x040081DE RID: 33246
		private static ComputeBuffer s_effectorParamsBuffer;

		// Token: 0x040081DF RID: 33247
		private static Dictionary<int, int> s_effectorParamsIndexMap = new Dictionary<int, int>();

		// Token: 0x040081E0 RID: 33248
		internal static readonly bool UseAsynchronousJobs = true;

		// Token: 0x040081E1 RID: 33249
		internal static GameObject s_managerGo;

		// Token: 0x0200118A RID: 4490
		public enum UpdateMode
		{
			// Token: 0x040081E3 RID: 33251
			FixedUpdate,
			// Token: 0x040081E4 RID: 33252
			EarlyUpdate,
			// Token: 0x040081E5 RID: 33253
			LateUpdate
		}

		// Token: 0x0200118B RID: 4491
		public enum TranslationLockSpace
		{
			// Token: 0x040081E7 RID: 33255
			Global,
			// Token: 0x040081E8 RID: 33256
			Local
		}

		// Token: 0x0200118C RID: 4492
		// (Invoke) Token: 0x06007179 RID: 29049
		public delegate void BehaviorRegisterDelegate(BoingBehavior behavior);

		// Token: 0x0200118D RID: 4493
		// (Invoke) Token: 0x0600717D RID: 29053
		public delegate void BehaviorUnregisterDelegate(BoingBehavior behavior);

		// Token: 0x0200118E RID: 4494
		// (Invoke) Token: 0x06007181 RID: 29057
		public delegate void EffectorRegisterDelegate(BoingEffector effector);

		// Token: 0x0200118F RID: 4495
		// (Invoke) Token: 0x06007185 RID: 29061
		public delegate void EffectorUnregisterDelegate(BoingEffector effector);

		// Token: 0x02001190 RID: 4496
		// (Invoke) Token: 0x06007189 RID: 29065
		public delegate void ReactorRegisterDelegate(BoingReactor reactor);

		// Token: 0x02001191 RID: 4497
		// (Invoke) Token: 0x0600718D RID: 29069
		public delegate void ReactorUnregisterDelegate(BoingReactor reactor);

		// Token: 0x02001192 RID: 4498
		// (Invoke) Token: 0x06007191 RID: 29073
		public delegate void ReactorFieldRegisterDelegate(BoingReactorField field);

		// Token: 0x02001193 RID: 4499
		// (Invoke) Token: 0x06007195 RID: 29077
		public delegate void ReactorFieldUnregisterDelegate(BoingReactorField field);

		// Token: 0x02001194 RID: 4500
		// (Invoke) Token: 0x06007199 RID: 29081
		public delegate void ReactorFieldCPUSamplerRegisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02001195 RID: 4501
		// (Invoke) Token: 0x0600719D RID: 29085
		public delegate void ReactorFieldCPUSamplerUnregisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02001196 RID: 4502
		// (Invoke) Token: 0x060071A1 RID: 29089
		public delegate void ReactorFieldGPUSamplerRegisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x02001197 RID: 4503
		// (Invoke) Token: 0x060071A5 RID: 29093
		public delegate void ReactorFieldGPUSamplerUnregisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x02001198 RID: 4504
		// (Invoke) Token: 0x060071A9 RID: 29097
		public delegate void BonesRegisterDelegate(BoingBones bones);

		// Token: 0x02001199 RID: 4505
		// (Invoke) Token: 0x060071AD RID: 29101
		public delegate void BonesUnregisterDelegate(BoingBones bones);
	}
}
