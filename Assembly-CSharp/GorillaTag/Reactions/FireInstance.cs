using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Reactions
{
	// Token: 0x02001026 RID: 4134
	public class FireInstance : MonoBehaviour
	{
		// Token: 0x06006880 RID: 26752 RVA: 0x002204B1 File Offset: 0x0021E6B1
		protected void Awake()
		{
			FireManager.Register(this);
		}

		// Token: 0x06006881 RID: 26753 RVA: 0x002204B9 File Offset: 0x0021E6B9
		protected void OnDestroy()
		{
			FireManager.Unregister(this);
		}

		// Token: 0x06006882 RID: 26754 RVA: 0x002204C1 File Offset: 0x0021E6C1
		protected void OnEnable()
		{
			FireManager.OnEnable(this);
		}

		// Token: 0x06006883 RID: 26755 RVA: 0x002204C9 File Offset: 0x0021E6C9
		protected void OnDisable()
		{
			FireManager.OnDisable(this);
		}

		// Token: 0x06006884 RID: 26756 RVA: 0x002204D1 File Offset: 0x0021E6D1
		protected void OnTriggerEnter(Collider other)
		{
			FireManager.OnTriggerEnter(this, other);
		}

		// Token: 0x04007713 RID: 30483
		[Header("Scene References")]
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[SerializeField]
		internal Collider _collider;

		// Token: 0x04007714 RID: 30484
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[FormerlySerializedAs("_thermalSourceVolume")]
		[SerializeField]
		internal ThermalSourceVolume _thermalVolume;

		// Token: 0x04007715 RID: 30485
		[SerializeField]
		internal ParticleSystem _particleSystem;

		// Token: 0x04007716 RID: 30486
		[FormerlySerializedAs("_audioSource")]
		[SerializeField]
		internal AudioSource _loopingAudioSource;

		// Token: 0x04007717 RID: 30487
		[Tooltip("The emissive color will be darkened on the materials of these renderers as the fire is extinguished.")]
		[SerializeField]
		internal Renderer[] _emissiveRenderers;

		// Token: 0x04007718 RID: 30488
		[Header("Asset References")]
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _extinguishSound;

		// Token: 0x04007719 RID: 30489
		[SerializeField]
		internal float _extinguishSoundVolume = 1f;

		// Token: 0x0400771A RID: 30490
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _igniteSound;

		// Token: 0x0400771B RID: 30491
		[SerializeField]
		internal float _igniteSoundVolume = 1f;

		// Token: 0x0400771C RID: 30492
		[Header("Values")]
		[SerializeField]
		internal bool _despawnOnExtinguish = true;

		// Token: 0x0400771D RID: 30493
		[SerializeField]
		internal float _maxLifetime = 10f;

		// Token: 0x0400771E RID: 30494
		[Tooltip("How long it should take to reheat to it's default temperature.")]
		[SerializeField]
		internal float _reheatSpeed = 1f;

		// Token: 0x0400771F RID: 30495
		[Tooltip("If you completely extinguish the object, how long should it stay extinguished?")]
		[SerializeField]
		internal float _stayExtinguishedDuration = 1f;

		// Token: 0x04007720 RID: 30496
		internal float _defaultTemperature;

		// Token: 0x04007721 RID: 30497
		internal float _timeSinceExtinguished;

		// Token: 0x04007722 RID: 30498
		internal float _timeSinceDyingStart;

		// Token: 0x04007723 RID: 30499
		internal float _timeAlive;

		// Token: 0x04007724 RID: 30500
		internal float _psDefaultEmissionRate;

		// Token: 0x04007725 RID: 30501
		internal ParticleSystem.EmissionModule _psEmissionModule;

		// Token: 0x04007726 RID: 30502
		internal Vector3Int _spatialGridPosition;

		// Token: 0x04007727 RID: 30503
		internal bool _isDespawning;

		// Token: 0x04007728 RID: 30504
		internal float _deathStateDuration;

		// Token: 0x04007729 RID: 30505
		internal MaterialPropertyBlock[] _emiRenderers_matPropBlocks;

		// Token: 0x0400772A RID: 30506
		internal Color[] _emiRenderers_defaultColors;
	}
}
