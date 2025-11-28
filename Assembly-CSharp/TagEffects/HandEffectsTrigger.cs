using System;
using GorillaExtensions;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000F6E RID: 3950
	public class HandEffectsTrigger : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x060062C0 RID: 25280 RVA: 0x001FDB33 File Offset: 0x001FBD33
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x060062C1 RID: 25281 RVA: 0x001FDB3C File Offset: 0x001FBD3C
		public bool FingersDown
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFistRight()) || (!this.rightHand && this.rig.IsMakingFistLeft()));
			}
		}

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x060062C2 RID: 25282 RVA: 0x001FDB88 File Offset: 0x001FBD88
		public bool FingersUp
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFiveRight()) || (!this.rightHand && this.rig.IsMakingFiveLeft()));
			}
		}

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x060062C3 RID: 25283 RVA: 0x001FDBD4 File Offset: 0x001FBDD4
		public Vector3 Velocity
		{
			get
			{
				if (this.velocityEstimator != null && this.rig != null && this.rig.scaleFactor > 0.001f)
				{
					return this.velocityEstimator.linearVelocity / this.rig.scaleFactor;
				}
				return Vector3.zero;
			}
		}

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x060062C4 RID: 25284 RVA: 0x001FDC30 File Offset: 0x001FBE30
		bool IHandEffectsTrigger.RightHand
		{
			get
			{
				return this.rightHand;
			}
		}

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x060062C5 RID: 25285 RVA: 0x001FDC38 File Offset: 0x001FBE38
		// (set) Token: 0x060062C6 RID: 25286 RVA: 0x001FDC40 File Offset: 0x001FBE40
		public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x060062C7 RID: 25287 RVA: 0x001FDC49 File Offset: 0x001FBE49
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x060062C8 RID: 25288 RVA: 0x000743A9 File Offset: 0x000725A9
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x060062C9 RID: 25289 RVA: 0x001FDC51 File Offset: 0x001FBE51
		public VRRig Rig
		{
			get
			{
				return this.rig;
			}
		}

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x060062CA RID: 25290 RVA: 0x001FDC59 File Offset: 0x001FBE59
		public TagEffectPack CosmeticEffectPack
		{
			get
			{
				if (this.rig == null)
				{
					return null;
				}
				return this.rig.CosmeticEffectPack;
			}
		}

		// Token: 0x060062CB RID: 25291 RVA: 0x001FDC78 File Offset: 0x001FBE78
		private void Awake()
		{
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.velocityEstimator == null)
			{
				this.velocityEstimator = base.GetComponentInParent<GorillaVelocityEstimator>();
			}
			for (int i = 0; i < this.debugVisuals.Length; i++)
			{
				this.debugVisuals[i].SetActive(TagEffectsLibrary.DebugMode);
			}
		}

		// Token: 0x060062CC RID: 25292 RVA: 0x00074410 File Offset: 0x00072610
		private void OnEnable()
		{
			if (!HandEffectsTriggerRegistry.HasInstance)
			{
				HandEffectsTriggerRegistry.FindInstance();
			}
			HandEffectsTriggerRegistry.Instance.Register(this);
		}

		// Token: 0x060062CD RID: 25293 RVA: 0x00074429 File Offset: 0x00072629
		private void OnDisable()
		{
			HandEffectsTriggerRegistry.Instance.Unregister(this);
		}

		// Token: 0x060062CE RID: 25294 RVA: 0x001FDCD0 File Offset: 0x001FBED0
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
			if (this.rig == other.Rig)
			{
				return;
			}
			if (this.FingersDown && other.FingersDown && (other.Static || (Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.up) * base.transform.up - Vector3.Dot(other.Velocity, other.Transform.up) * other.Transform.up, -other.Transform.up) > TagEffectsLibrary.FistBumpSpeedThreshold && Vector3.Dot(base.transform.up, other.Transform.up) < -0.01f)))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.FIST_BUMP, other);
			}
			if (this.FingersUp && other.FingersUp && (other.Static || Mathf.Abs(Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.right) * base.transform.right - Vector3.Dot(other.Velocity, other.Transform.right) * other.Transform.right, other.Transform.right)) > TagEffectsLibrary.HighFiveSpeedThreshold))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.HIGH_FIVE, other);
			}
		}

		// Token: 0x060062CF RID: 25295 RVA: 0x001FDE3C File Offset: 0x001FC03C
		private void PlayHandEffects(TagEffectsLibrary.EffectType effectType, IHandEffectsTrigger other)
		{
			if (this.rig.IsNull())
			{
				return;
			}
			bool flag = false;
			if (this.rig.isOfflineVRRig)
			{
				PlayerGameEvents.TriggerHandEffect(effectType.ToString());
			}
			if (this.OnTrigger != null || (other != null && other.OnTrigger != null))
			{
				switch (effectType)
				{
				case TagEffectsLibrary.EffectType.FIRST_PERSON:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger = this.OnTrigger;
					if (onTrigger != null)
					{
						onTrigger.Invoke(IHandEffectsTrigger.Mode.Tag1P);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger2 = other.OnTrigger;
						if (onTrigger2 != null)
						{
							onTrigger2.Invoke(IHandEffectsTrigger.Mode.Tag1P);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.THIRD_PERSON:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger3 = this.OnTrigger;
					if (onTrigger3 != null)
					{
						onTrigger3.Invoke(IHandEffectsTrigger.Mode.Tag3P);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger4 = other.OnTrigger;
						if (onTrigger4 != null)
						{
							onTrigger4.Invoke(IHandEffectsTrigger.Mode.Tag3P);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.HIGH_FIVE:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger5 = this.OnTrigger;
					if (onTrigger5 != null)
					{
						onTrigger5.Invoke(IHandEffectsTrigger.Mode.HighFive);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger6 = other.OnTrigger;
						if (onTrigger6 != null)
						{
							onTrigger6.Invoke(IHandEffectsTrigger.Mode.HighFive);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.FIST_BUMP:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger7 = this.OnTrigger;
					if (onTrigger7 != null)
					{
						onTrigger7.Invoke(IHandEffectsTrigger.Mode.FistBump);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger8 = other.OnTrigger;
						if (onTrigger8 != null)
						{
							onTrigger8.Invoke(IHandEffectsTrigger.Mode.FistBump);
						}
					}
					break;
				}
				}
			}
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic = null;
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic2 = null;
			foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic3 in (this.rightHand ? this.rig.CosmeticHandEffectsOverride_Right : this.rig.CosmeticHandEffectsOverride_Left))
			{
				if (handEffectsOverrideCosmetic3.handEffectType == this.MapEnum(effectType))
				{
					handEffectsOverrideCosmetic2 = handEffectsOverrideCosmetic3;
					break;
				}
			}
			if (this.rig.isOfflineVRRig && GorillaTagger.Instance != null)
			{
				if (other.Rig)
				{
					foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic4 in ((other.Rig.CosmeticHandEffectsOverride_Right != null) ? other.Rig.CosmeticHandEffectsOverride_Right : other.Rig.CosmeticHandEffectsOverride_Left))
					{
						if (handEffectsOverrideCosmetic4.handEffectType == this.MapEnum(effectType))
						{
							handEffectsOverrideCosmetic = handEffectsOverrideCosmetic4;
							break;
						}
					}
					if (handEffectsOverrideCosmetic && handEffectsOverrideCosmetic.handEffectType == this.MapEnum(effectType) && ((!handEffectsOverrideCosmetic.isLeftHand && other.RightHand) || (handEffectsOverrideCosmetic.isLeftHand && !other.RightHand)))
					{
						if (handEffectsOverrideCosmetic.thirdPerson.playHaptics)
						{
							GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic.thirdPerson.hapticStrength, handEffectsOverrideCosmetic.thirdPerson.hapticDuration);
						}
						TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic.thirdPerson.effectVFX, base.transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic.thirdPerson.parentEffect, base.transform.rotation);
						flag = true;
					}
				}
				if (handEffectsOverrideCosmetic2 && handEffectsOverrideCosmetic2.handEffectType == this.MapEnum(effectType) && ((handEffectsOverrideCosmetic2.isLeftHand && !this.rightHand) || (!handEffectsOverrideCosmetic2.isLeftHand && this.rightHand)))
				{
					if (handEffectsOverrideCosmetic2.firstPerson.playHaptics)
					{
						GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic2.firstPerson.hapticStrength, handEffectsOverrideCosmetic2.firstPerson.hapticDuration);
					}
					TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic2.firstPerson.effectVFX, other.Transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic2.firstPerson.parentEffect, other.Transform.rotation);
					flag = true;
				}
			}
			if (!flag)
			{
				if (this.rig.isOfflineVRRig)
				{
					GorillaTagger.Instance.StartVibration(!this.rightHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
				}
				TagEffectsLibrary.PlayEffect(base.transform, !this.rightHand, this.rig.scaleFactor, effectType, this.CosmeticEffectPack, other.CosmeticEffectPack, base.transform.rotation);
			}
		}

		// Token: 0x060062D0 RID: 25296 RVA: 0x001FE240 File Offset: 0x001FC440
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return (base.transform.position - t.Transform.position).IsShorterThan(this.triggerRadius * this.rig.scaleFactor);
		}

		// Token: 0x060062D1 RID: 25297 RVA: 0x001FE274 File Offset: 0x001FC474
		private HandEffectsOverrideCosmetic.HandEffectType MapEnum(TagEffectsLibrary.EffectType oldEnum)
		{
			return HandEffectsTrigger.mappingArray[(int)oldEnum];
		}

		// Token: 0x04007183 RID: 29059
		[SerializeField]
		private float triggerRadius = 0.07f;

		// Token: 0x04007184 RID: 29060
		[SerializeField]
		private bool rightHand;

		// Token: 0x04007185 RID: 29061
		[SerializeField]
		private bool isStatic;

		// Token: 0x04007186 RID: 29062
		private VRRig rig;

		// Token: 0x04007187 RID: 29063
		public GorillaVelocityEstimator velocityEstimator;

		// Token: 0x04007188 RID: 29064
		[SerializeField]
		private GameObject[] debugVisuals;

		// Token: 0x0400718B RID: 29067
		private static HandEffectsOverrideCosmetic.HandEffectType[] mappingArray = new HandEffectsOverrideCosmetic.HandEffectType[]
		{
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.HighFive,
			HandEffectsOverrideCosmetic.HandEffectType.FistBump
		};
	}
}
