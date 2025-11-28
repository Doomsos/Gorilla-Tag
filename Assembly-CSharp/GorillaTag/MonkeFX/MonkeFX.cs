using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x02001031 RID: 4145
	public class MonkeFX : ITickSystemPost
	{
		// Token: 0x060068B3 RID: 26803 RVA: 0x00221CA8 File Offset: 0x0021FEA8
		private static void InitBonesArray()
		{
			MonkeFX._rigs = VRRigCache.Instance.GetAllRigs();
			MonkeFX._bones = new Transform[MonkeFX._rigs.Length * MonkeFX._boneNames.Length];
			for (int i = 0; i < MonkeFX._rigs.Length; i++)
			{
				if (MonkeFX._rigs[i] == null)
				{
					MonkeFX._errorLog_nullVRRigFromVRRigCache.AddOccurrence(i.ToString());
				}
				else
				{
					int num = i * MonkeFX._boneNames.Length;
					if (MonkeFX._rigs[i].mainSkin == null)
					{
						MonkeFX._errorLog_nullMainSkin.AddOccurrence(MonkeFX._rigs[i].transform.GetPath());
						Debug.LogError("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene path: \n- \"" + MonkeFX._rigs[i].transform.GetPath() + "\"");
					}
					else
					{
						for (int j = 0; j < MonkeFX._rigs[i].mainSkin.bones.Length; j++)
						{
							Transform transform = MonkeFX._rigs[i].mainSkin.bones[j];
							if (transform == null)
							{
								MonkeFX._errorLog_nullBone.AddOccurrence(j.ToString());
							}
							else
							{
								for (int k = 0; k < MonkeFX._boneNames.Length; k++)
								{
									if (MonkeFX._boneNames[k] == transform.name)
									{
										MonkeFX._bones[num + k] = transform;
									}
								}
							}
						}
					}
				}
			}
			MonkeFX._errorLog_nullVRRigFromVRRigCache.LogOccurrences(VRRigCache.Instance, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 106);
			MonkeFX._errorLog_nullMainSkin.LogOccurrences(null, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 107);
			MonkeFX._errorLog_nullBone.LogOccurrences(null, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 108);
		}

		// Token: 0x060068B4 RID: 26804 RVA: 0x00002789 File Offset: 0x00000989
		private static void UpdateBones()
		{
		}

		// Token: 0x060068B5 RID: 26805 RVA: 0x00002789 File Offset: 0x00000989
		private static void UpdateBone()
		{
		}

		// Token: 0x060068B6 RID: 26806 RVA: 0x00221E50 File Offset: 0x00220050
		public static void Register(MonkeFXSettingsSO settingsSO)
		{
			MonkeFX.EnsureInstance();
			if (settingsSO == null || !MonkeFX.instance._settingsSOs.Add(settingsSO))
			{
				return;
			}
			int num = MonkeFX.instance._srcMeshId_to_sourceMesh.Count;
			for (int i = 0; i < settingsSO.sourceMeshes.Length; i++)
			{
				Mesh obj = settingsSO.sourceMeshes[i].obj;
				if (!(obj == null) && MonkeFX.instance._srcMeshInst_to_meshId.TryAdd(obj.GetInstanceID(), num))
				{
					MonkeFX.instance._srcMeshId_to_sourceMesh.Add(obj);
					num++;
				}
			}
		}

		// Token: 0x060068B7 RID: 26807 RVA: 0x00221EE8 File Offset: 0x002200E8
		[MethodImpl(256)]
		public static float GetScaleToFitInBounds(Mesh mesh)
		{
			Bounds bounds = mesh.bounds;
			float num = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
			if (num <= 0f)
			{
				return 0f;
			}
			return 1f / num;
		}

		// Token: 0x060068B8 RID: 26808 RVA: 0x00221F40 File Offset: 0x00220140
		[MethodImpl(256)]
		public static float Pack0To1Floats(float x, float y)
		{
			return Mathf.Clamp01(x) * 65536f + Mathf.Clamp01(y);
		}

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x060068B9 RID: 26809 RVA: 0x00221F55 File Offset: 0x00220155
		// (set) Token: 0x060068BA RID: 26810 RVA: 0x00221F5C File Offset: 0x0022015C
		public static MonkeFX instance { get; private set; }

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x060068BB RID: 26811 RVA: 0x00221F64 File Offset: 0x00220164
		// (set) Token: 0x060068BC RID: 26812 RVA: 0x00221F6B File Offset: 0x0022016B
		public static bool hasInstance { get; private set; }

		// Token: 0x060068BD RID: 26813 RVA: 0x00221F73 File Offset: 0x00220173
		private static void EnsureInstance()
		{
			if (MonkeFX.hasInstance)
			{
				return;
			}
			MonkeFX.instance = new MonkeFX();
			MonkeFX.hasInstance = true;
		}

		// Token: 0x060068BE RID: 26814 RVA: 0x00221F8D File Offset: 0x0022018D
		[RuntimeInitializeOnLoadMethod(0)]
		private static void OnAfterFirstSceneLoaded()
		{
			MonkeFX.EnsureInstance();
			TickSystem<object>.AddPostTickCallback(MonkeFX.instance);
		}

		// Token: 0x060068BF RID: 26815 RVA: 0x00221F9E File Offset: 0x0022019E
		void ITickSystemPost.PostTick()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			MonkeFX.UpdateBones();
		}

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x060068C0 RID: 26816 RVA: 0x00221FAD File Offset: 0x002201AD
		// (set) Token: 0x060068C1 RID: 26817 RVA: 0x00221FB5 File Offset: 0x002201B5
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x060068C2 RID: 26818 RVA: 0x00221FBE File Offset: 0x002201BE
		private static void PauseTick()
		{
			if (!MonkeFX.hasInstance)
			{
				MonkeFX.instance = new MonkeFX();
			}
			TickSystem<object>.RemovePostTickCallback(MonkeFX.instance);
		}

		// Token: 0x060068C3 RID: 26819 RVA: 0x00221FDB File Offset: 0x002201DB
		private static void ResumeTick()
		{
			if (!MonkeFX.hasInstance)
			{
				MonkeFX.instance = new MonkeFX();
			}
			TickSystem<object>.AddPostTickCallback(MonkeFX.instance);
		}

		// Token: 0x04007777 RID: 30583
		private static readonly string[] _boneNames = new string[]
		{
			"body",
			"hand.L",
			"hand.R"
		};

		// Token: 0x04007778 RID: 30584
		private static VRRig[] _rigs;

		// Token: 0x04007779 RID: 30585
		private static Transform[] _bones;

		// Token: 0x0400777A RID: 30586
		private static int _rigsHash;

		// Token: 0x0400777B RID: 30587
		private static readonly GTLogErrorLimiter _errorLog_nullVRRigFromVRRigCache = new GTLogErrorLimiter("(This should never happen) Skipping null `VRRig` obtained from `VRRigCache`!", 10, "\n- ");

		// Token: 0x0400777C RID: 30588
		private static GTLogErrorLimiter _errorLog_nullMainSkin = new GTLogErrorLimiter("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene paths: \n", 10, "\n- ");

		// Token: 0x0400777D RID: 30589
		private static readonly GTLogErrorLimiter _errorLog_nullBone = new GTLogErrorLimiter("(This should never happen) Skipping null bone obtained from `VRRig.mainSkin.bones`! Index(es): ", 10, "\n- ");

		// Token: 0x0400777E RID: 30590
		private readonly HashSet<MonkeFXSettingsSO> _settingsSOs = new HashSet<MonkeFXSettingsSO>(8);

		// Token: 0x0400777F RID: 30591
		private readonly Dictionary<int, int> _srcMeshInst_to_meshId = new Dictionary<int, int>(8);

		// Token: 0x04007780 RID: 30592
		private readonly List<Mesh> _srcMeshId_to_sourceMesh = new List<Mesh>(8);

		// Token: 0x04007781 RID: 30593
		private readonly List<MonkeFX.ElementsRange> _srcMeshId_to_elemRange = new List<MonkeFX.ElementsRange>(8);

		// Token: 0x04007782 RID: 30594
		private readonly Dictionary<int, List<MonkeFXSettingsSO>> _meshId_to_settingsUsers = new Dictionary<int, List<MonkeFXSettingsSO>>();

		// Token: 0x04007783 RID: 30595
		private const float _k16BitFactor = 65536f;

		// Token: 0x02001032 RID: 4146
		private struct ElementsRange
		{
			// Token: 0x04007787 RID: 30599
			public int min;

			// Token: 0x04007788 RID: 30600
			public int max;
		}
	}
}
