using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTag.Rendering
{
	// Token: 0x02001077 RID: 4215
	public class WaterBubbleParticleVolumeCollector : MonoBehaviour
	{
		// Token: 0x060069CB RID: 27083 RVA: 0x0022696C File Offset: 0x00224B6C
		protected void Awake()
		{
			List<WaterVolume> componentsInHierarchy = SceneManager.GetActiveScene().GetComponentsInHierarchy(true, 64);
			List<Collider> list = new List<Collider>(componentsInHierarchy.Count * 4);
			foreach (WaterVolume waterVolume in componentsInHierarchy)
			{
				if (!(waterVolume.Parameters != null) || waterVolume.Parameters.allowBubblesInVolume)
				{
					foreach (Collider collider in waterVolume.volumeColliders)
					{
						if (!(collider == null))
						{
							list.Add(collider);
						}
					}
				}
			}
			this.bubbleableVolumeColliders = list.ToArray();
			this.particleTriggerModules = new ParticleSystem.TriggerModule[this.particleSystems.Length];
			this.particleEmissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleTriggerModules[i] = this.particleSystems[i].trigger;
				this.particleEmissionModules[i] = this.particleSystems[i].emission;
			}
			for (int j = 0; j < this.particleSystems.Length; j++)
			{
				ParticleSystem.TriggerModule triggerModule = this.particleTriggerModules[j];
				for (int k = 0; k < list.Count; k++)
				{
					triggerModule.SetCollider(k, this.bubbleableVolumeColliders[k]);
				}
			}
			this.SetEmissionState(false);
		}

		// Token: 0x060069CC RID: 27084 RVA: 0x00226B0C File Offset: 0x00224D0C
		protected void LateUpdate()
		{
			bool headInWater = GTPlayer.Instance.HeadInWater;
			if (headInWater && !this.emissionEnabled)
			{
				this.SetEmissionState(true);
				return;
			}
			if (!headInWater && this.emissionEnabled)
			{
				this.SetEmissionState(false);
			}
		}

		// Token: 0x060069CD RID: 27085 RVA: 0x00226B4C File Offset: 0x00224D4C
		private void SetEmissionState(bool setEnabled)
		{
			float rateOverTimeMultiplier = setEnabled ? 1f : 0f;
			for (int i = 0; i < this.particleEmissionModules.Length; i++)
			{
				this.particleEmissionModules[i].rateOverTimeMultiplier = rateOverTimeMultiplier;
			}
			this.emissionEnabled = setEnabled;
		}

		// Token: 0x0400790F RID: 30991
		public ParticleSystem[] particleSystems;

		// Token: 0x04007910 RID: 30992
		private ParticleSystem.TriggerModule[] particleTriggerModules;

		// Token: 0x04007911 RID: 30993
		private ParticleSystem.EmissionModule[] particleEmissionModules;

		// Token: 0x04007912 RID: 30994
		private Collider[] bubbleableVolumeColliders;

		// Token: 0x04007913 RID: 30995
		private bool emissionEnabled;
	}
}
