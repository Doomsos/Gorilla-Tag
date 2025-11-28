using System;
using BoingKit;
using GorillaLocomotion.Gameplay;
using GorillaTag.Rendering;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02001205 RID: 4613
[DOTSCompilerGenerated]
internal class __JobReflectionRegistrationOutput__1221673671587648887
{
	// Token: 0x06007334 RID: 29492 RVA: 0x0025AC54 File Offset: 0x00258E54
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForExtensions.EarlyJobInit<HandEffectsTriggerRegistry.HandEffectsJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderRenderer.SetupInstanceDataForMesh>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderRenderer.SetupInstanceDataForMeshStatic>();
			IJobParallelForExtensions.EarlyJobInit<GorillaIKMgr.IKJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<GorillaIKMgr.IKTransformJob>();
			IJobExtensions.EarlyJobInit<DayNightCycle.LerpBakedLightingJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<VRRigJobManager.VRRigTransformJob>();
			IJobParallelForExtensions.EarlyJobInit<BuilderFindPotentialSnaps>();
			IJobParallelForTransformExtensions.EarlyJobInit<FindNearbyPiecesJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderConveyorManager.EvaluateSplineJob>();
			IJobExtensions.EarlyJobInit<SolveRopeJob>();
			IJobExtensions.EarlyJobInit<VectorizedSolveRopeJob>();
			IJobExtensions.EarlyJobInit<EdMeshCombinerPrefab.CopyMeshJob>();
			IJobParallelForExtensions.EarlyJobInit<BoingWorkAsynchronous.BehaviorJob>();
			IJobParallelForExtensions.EarlyJobInit<BoingWorkAsynchronous.ReactorJob>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex);
		}
	}

	// Token: 0x06007335 RID: 29493 RVA: 0x0025ACD0 File Offset: 0x00258ED0
	[RuntimeInitializeOnLoadMethod(2)]
	public static void EarlyInit()
	{
		__JobReflectionRegistrationOutput__1221673671587648887.CreateJobReflectionData();
	}
}
