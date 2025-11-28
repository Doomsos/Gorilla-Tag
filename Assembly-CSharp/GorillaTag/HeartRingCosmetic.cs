using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FC8 RID: 4040
	[DefaultExecutionOrder(1250)]
	public class HeartRingCosmetic : MonoBehaviour
	{
		// Token: 0x0600666D RID: 26221 RVA: 0x002160A7 File Offset: 0x002142A7
		protected void Awake()
		{
			Application.quitting += delegate()
			{
				base.enabled = false;
			};
		}

		// Token: 0x0600666E RID: 26222 RVA: 0x002160BC File Offset: 0x002142BC
		protected void OnEnable()
		{
			this.particleSystem = this.effects.GetComponentInChildren<ParticleSystem>(true);
			this.audioSource = this.effects.GetComponentInChildren<AudioSource>(true);
			this.ownerRig = base.GetComponentInParent<VRRig>();
			bool flag = this.ownerRig != null && this.ownerRig.head != null && this.ownerRig.head.rigTarget != null;
			base.enabled = flag;
			this.effects.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("Disabling HeartRingCosmetic. Could not find owner head. Scene path: " + base.transform.GetPath(), this);
				return;
			}
			this.ownerHead = ((this.ownerRig != null) ? this.ownerRig.head.rigTarget.transform : base.transform);
			this.maxEmissionRate = this.particleSystem.emission.rateOverTime.constant;
			this.maxVolume = this.audioSource.volume;
		}

		// Token: 0x0600666F RID: 26223 RVA: 0x002161C4 File Offset: 0x002143C4
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num = this.effectActivationRadius * this.effectActivationRadius * x * x;
			bool flag = (this.ownerHead.TransformPoint(this.headToMouthOffset) - position).sqrMagnitude < num;
			ParticleSystem.EmissionModule emission = this.particleSystem.emission;
			emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, flag ? this.maxEmissionRate : 0f, Time.deltaTime / 0.1f);
			this.audioSource.volume = Mathf.Lerp(this.audioSource.volume, flag ? this.maxVolume : 0f, Time.deltaTime / 2f);
			this.ownerRig.UsingHauntedRing = (this.isHauntedVoiceChanger && flag);
			if (this.ownerRig.UsingHauntedRing)
			{
				this.ownerRig.HauntedRingVoicePitch = this.hauntedVoicePitch;
			}
		}

		// Token: 0x04007512 RID: 29970
		public GameObject effects;

		// Token: 0x04007513 RID: 29971
		[SerializeField]
		private bool isHauntedVoiceChanger;

		// Token: 0x04007514 RID: 29972
		[SerializeField]
		private float hauntedVoicePitch = 0.75f;

		// Token: 0x04007515 RID: 29973
		[AssignInCorePrefab]
		public float effectActivationRadius = 0.15f;

		// Token: 0x04007516 RID: 29974
		private readonly Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04007517 RID: 29975
		private VRRig ownerRig;

		// Token: 0x04007518 RID: 29976
		private Transform ownerHead;

		// Token: 0x04007519 RID: 29977
		private ParticleSystem particleSystem;

		// Token: 0x0400751A RID: 29978
		private AudioSource audioSource;

		// Token: 0x0400751B RID: 29979
		private float maxEmissionRate;

		// Token: 0x0400751C RID: 29980
		private float maxVolume;

		// Token: 0x0400751D RID: 29981
		private const float emissionFadeTime = 0.1f;

		// Token: 0x0400751E RID: 29982
		private const float volumeFadeTime = 2f;
	}
}
