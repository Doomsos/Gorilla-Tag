using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag.Audio;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02001027 RID: 4135
	public class FireManager : ITickSystemPost
	{
		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06006886 RID: 26758 RVA: 0x0022050D File Offset: 0x0021E70D
		// (set) Token: 0x06006887 RID: 26759 RVA: 0x00220514 File Offset: 0x0021E714
		internal static FireManager instance { get; private set; }

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x06006888 RID: 26760 RVA: 0x0022051C File Offset: 0x0021E71C
		// (set) Token: 0x06006889 RID: 26761 RVA: 0x00220523 File Offset: 0x0021E723
		internal static bool hasInstance { get; private set; }

		// Token: 0x0600688A RID: 26762 RVA: 0x0022052B File Offset: 0x0021E72B
		[RuntimeInitializeOnLoadMethod(0)]
		private static void Initialize()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (FireManager.hasInstance)
			{
				return;
			}
			FireManager.instance = new FireManager();
			FireManager.hasInstance = true;
			TickSystem<object>.AddPostTickCallback(FireManager.instance);
		}

		// Token: 0x0600688B RID: 26763 RVA: 0x00220558 File Offset: 0x0021E758
		[MethodImpl(256)]
		internal static void Register(FireInstance f)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			int instanceID = f.gameObject.GetInstanceID();
			if (!FireManager._kGObjInstId_to_fire.TryAdd(instanceID, f))
			{
				if (f == null)
				{
					Debug.LogError("FireManager: You tried to register null!", f);
					return;
				}
				Debug.LogError("FireManager: \"" + f.name + "\" was attempted to be registered more than once!", f);
			}
			f.GetComponentAndSetFieldIfNullElseLogAndDisable(ref f._collider, "_collider", "Collider", "Disabling.", "Register");
			f.GetComponentAndSetFieldIfNullElseLogAndDisable(ref f._thermalVolume, "_thermalVolume", "ThermalSourceVolume", "Disabling.", "Register");
			f.GetComponentAndSetFieldIfNullElseLogAndDisable(ref f._particleSystem, "_particleSystem", "ParticleSystem", "Disabling.", "Register");
			f.GetComponentAndSetFieldIfNullElseLogAndDisable(ref f._loopingAudioSource, "_loopingAudioSource", "AudioSource", "Disabling.", "Register");
			f.DisableIfNull(f._extinguishSound.obj, "_extinguishSound", "AudioClip", "Register");
			f.DisableIfNull(f._igniteSound.obj, "_igniteSound", "AudioClip", "Register");
			f._defaultTemperature = f._thermalVolume.celsius;
			f._timeSinceExtinguished = -f._stayExtinguishedDuration;
			f._psEmissionModule = f._particleSystem.emission;
			f._psDefaultEmissionRate = f._psEmissionModule.rateOverTime.constant;
			f._deathStateDuration = 0f;
			if (f._emissiveRenderers != null)
			{
				f._emiRenderers_matPropBlocks = new MaterialPropertyBlock[f._emissiveRenderers.Length];
				f._emiRenderers_defaultColors = new Color[f._emissiveRenderers.Length];
				for (int i = 0; i < f._emissiveRenderers.Length; i++)
				{
					f._emiRenderers_matPropBlocks[i] = new MaterialPropertyBlock();
					f._emissiveRenderers[i].GetPropertyBlock(f._emiRenderers_matPropBlocks[i]);
					f._emiRenderers_defaultColors[i] = f._emiRenderers_matPropBlocks[i].GetColor(FireManager.shaderProp_EmissionColor);
				}
			}
		}

		// Token: 0x0600688C RID: 26764 RVA: 0x00220754 File Offset: 0x0021E954
		[MethodImpl(256)]
		internal static void Unregister(FireInstance reactable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			int instanceID = reactable.gameObject.GetInstanceID();
			FireManager._kGObjInstId_to_fire.Remove(instanceID);
		}

		// Token: 0x0600688D RID: 26765 RVA: 0x00220784 File Offset: 0x0021E984
		[MethodImpl(256)]
		private static Vector3Int GetSpatialGridPos(Vector3 pos)
		{
			Vector3 vector = pos / 0.2f;
			return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
		}

		// Token: 0x0600688E RID: 26766 RVA: 0x002207B8 File Offset: 0x0021E9B8
		[MethodImpl(256)]
		private static void ResetFireValues(FireInstance f)
		{
			f._timeSinceExtinguished = Mathf.Min(f._timeSinceExtinguished, f._stayExtinguishedDuration);
			f._timeSinceDyingStart = 0f;
			f._isDespawning = false;
			f._timeAlive = 0f;
			f._thermalVolume.celsius = f._defaultTemperature;
		}

		// Token: 0x0600688F RID: 26767 RVA: 0x0022080C File Offset: 0x0021EA0C
		[MethodImpl(256)]
		internal static void SpawnFire(SinglePool pool, Vector3 pos, Vector3 normal, float scale)
		{
			int num;
			if (FireManager._fireSpatialGrid.TryGetValue(FireManager.GetSpatialGridPos(pos), ref num))
			{
				FireManager.ResetFireValues(FireManager._kGObjInstId_to_fire[num]);
				return;
			}
			GameObject gameObject = pool.Instantiate(false);
			gameObject.transform.position = pos;
			gameObject.transform.up = normal;
			gameObject.transform.localScale = Vector3.one * scale;
			gameObject.SetActive(true);
		}

		// Token: 0x06006890 RID: 26768 RVA: 0x0022087C File Offset: 0x0021EA7C
		[MethodImpl(256)]
		internal static void OnEnable(FireInstance f)
		{
			if (ApplicationQuittingState.IsQuitting || ObjectPools.instance == null || !ObjectPools.instance.initialized)
			{
				return;
			}
			FireManager.ResetFireValues(f);
			f._spatialGridPosition = FireManager.GetSpatialGridPos(f.transform.position);
			FireManager._fireSpatialGrid.Add(f._spatialGridPosition, f.gameObject.GetInstanceID());
			FireManager._kEnabledReactions.Add(f);
			if (GTAudioOneShot.isInitialized && Time.realtimeSinceStartup > 10f)
			{
				GTAudioOneShot.Play(f._igniteSound, f.transform.position, f._igniteSoundVolume, 1f);
			}
			if (8 > FireManager._activeAudioSources)
			{
				FireManager._activeAudioSources++;
				f._loopingAudioSource.enabled = true;
				return;
			}
			f._loopingAudioSource.enabled = false;
		}

		// Token: 0x06006891 RID: 26769 RVA: 0x00220954 File Offset: 0x0021EB54
		[MethodImpl(256)]
		internal static void OnDisable(FireInstance f)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			FireManager._kEnabledReactions.Remove(f);
			FireManager._fireSpatialGrid.Remove(f._spatialGridPosition);
			FireManager._activeAudioSources = Mathf.Min(FireManager._activeAudioSources - (f._loopingAudioSource.enabled ? 1 : 0), 0);
		}

		// Token: 0x06006892 RID: 26770 RVA: 0x002209A8 File Offset: 0x0021EBA8
		[MethodImpl(256)]
		internal static void OnTriggerEnter(FireInstance f, Collider other)
		{
			if (f._isDespawning || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (other.gameObject.layer == 4)
			{
				FireManager.Extinguish(f.gameObject, float.MaxValue);
			}
		}

		// Token: 0x06006893 RID: 26771 RVA: 0x002209D8 File Offset: 0x0021EBD8
		[MethodImpl(256)]
		internal static void Extinguish(GameObject gObj, float extinguishAmount)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			FireInstance fireInstance;
			if (!FireManager._kGObjInstId_to_fire.TryGetValue(gObj.GetInstanceID(), ref fireInstance))
			{
				return;
			}
			float num = fireInstance._thermalVolume.celsius - extinguishAmount;
			if (num <= 0f && fireInstance._thermalVolume.celsius > 0.001f)
			{
				fireInstance._thermalVolume.celsius = Mathf.Max(num, 0f);
				fireInstance._timeSinceExtinguished = 0f;
				GTAudioOneShot.Play(fireInstance._extinguishSound, fireInstance.transform.position, fireInstance._extinguishSoundVolume, 1f);
				if (fireInstance._despawnOnExtinguish)
				{
					fireInstance._isDespawning = true;
					fireInstance._timeSinceDyingStart = 0f;
				}
			}
		}

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x06006894 RID: 26772 RVA: 0x00220A8B File Offset: 0x0021EC8B
		// (set) Token: 0x06006895 RID: 26773 RVA: 0x00220A93 File Offset: 0x0021EC93
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06006896 RID: 26774 RVA: 0x00220A9C File Offset: 0x0021EC9C
		void ITickSystemPost.PostTick()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			foreach (FireInstance fireInstance in FireManager._kEnabledReactions)
			{
				fireInstance._timeAlive += Time.unscaledDeltaTime;
				bool flag = fireInstance._timeSinceExtinguished < fireInstance._stayExtinguishedDuration;
				fireInstance._timeSinceExtinguished += Time.unscaledDeltaTime;
				bool flag2 = fireInstance._timeSinceExtinguished < fireInstance._stayExtinguishedDuration;
				if (fireInstance._isDespawning)
				{
					fireInstance._timeSinceDyingStart += Time.unscaledDeltaTime;
					if (fireInstance._timeSinceDyingStart >= fireInstance._deathStateDuration || fireInstance._thermalVolume.celsius < -9999f)
					{
						FireManager._kFiresToDespawn.Add(fireInstance);
					}
				}
				if (!fireInstance._isDespawning && fireInstance._despawnOnExtinguish && fireInstance._timeAlive > fireInstance._maxLifetime)
				{
					fireInstance._isDespawning = true;
					fireInstance._timeSinceDyingStart = 0f;
					GTAudioOneShot.Play(fireInstance._extinguishSound, fireInstance.transform.position, fireInstance._extinguishSoundVolume, 1f);
				}
				if (!fireInstance._isDespawning && flag != flag2)
				{
					if (flag2)
					{
						if (fireInstance._despawnOnExtinguish)
						{
							fireInstance._isDespawning = true;
							fireInstance._timeSinceDyingStart = 0f;
						}
						GTAudioOneShot.Play(fireInstance._extinguishSound, fireInstance.transform.position, fireInstance._extinguishSoundVolume, 1f);
					}
					else
					{
						GTAudioOneShot.Play(fireInstance._igniteSound, fireInstance.transform.position, fireInstance._igniteSoundVolume, 1f);
					}
				}
				float num = fireInstance._thermalVolume.celsius + fireInstance._reheatSpeed * Time.unscaledDeltaTime;
				if (fireInstance._isDespawning)
				{
					if (fireInstance._deathStateDuration <= 0f)
					{
						num = 0f;
					}
					else
					{
						num = Mathf.Lerp(fireInstance._thermalVolume.celsius, 0f, fireInstance._timeSinceDyingStart / fireInstance._deathStateDuration);
					}
				}
				num = ((num > fireInstance._defaultTemperature) ? fireInstance._defaultTemperature : num);
				fireInstance._thermalVolume.celsius = num;
				float num2 = num / fireInstance._defaultTemperature;
				fireInstance._loopingAudioSource.volume = num2;
				for (int i = 0; i < fireInstance._emissiveRenderers.Length; i++)
				{
					fireInstance._emiRenderers_matPropBlocks[i].SetColor(FireManager.shaderProp_EmissionColor, fireInstance._emiRenderers_defaultColors[i] * num2);
				}
			}
			foreach (FireInstance fireInstance2 in FireManager._kFiresToDespawn)
			{
				ObjectPools.instance.Destroy(fireInstance2.gameObject);
			}
			FireManager._kFiresToDespawn.Clear();
		}

		// Token: 0x0400772D RID: 30509
		[OnEnterPlay_Clear]
		private static readonly Dictionary<int, FireInstance> _kGObjInstId_to_fire = new Dictionary<int, FireInstance>(256);

		// Token: 0x0400772E RID: 30510
		[OnEnterPlay_Clear]
		private static readonly List<FireInstance> _kEnabledReactions = new List<FireInstance>(256);

		// Token: 0x0400772F RID: 30511
		[OnEnterPlay_Clear]
		private static readonly List<FireInstance> _kFiresToDespawn = new List<FireInstance>(256);

		// Token: 0x04007730 RID: 30512
		[OnEnterPlay_Clear]
		private static readonly Dictionary<Vector3Int, int> _fireSpatialGrid = new Dictionary<Vector3Int, int>(256);

		// Token: 0x04007731 RID: 30513
		private const float _kSpatialGridCellSize = 0.2f;

		// Token: 0x04007732 RID: 30514
		private const int _kMaxAudioSources = 8;

		// Token: 0x04007733 RID: 30515
		[OnEnterPlay_Set(0)]
		private static int _activeAudioSources;

		// Token: 0x04007734 RID: 30516
		private static readonly int shaderProp_EmissionColor = ShaderProps._EmissionColor;
	}
}
