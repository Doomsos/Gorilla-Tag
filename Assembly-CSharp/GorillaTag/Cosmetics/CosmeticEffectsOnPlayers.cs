using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010A8 RID: 4264
	public class CosmeticEffectsOnPlayers : MonoBehaviour, ISpawnable
	{
		// Token: 0x06006AA2 RID: 27298 RVA: 0x0022F6E8 File Offset: 0x0022D8E8
		private bool ShouldAffectRig(VRRig rig, CosmeticEffectsOnPlayers.TargetType target)
		{
			bool flag = rig == this.myRig;
			bool result;
			switch (target)
			{
			case CosmeticEffectsOnPlayers.TargetType.Owner:
				result = flag;
				break;
			case CosmeticEffectsOnPlayers.TargetType.Others:
				result = !flag;
				break;
			case CosmeticEffectsOnPlayers.TargetType.All:
				result = true;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		// Token: 0x06006AA3 RID: 27299 RVA: 0x0022F728 File Offset: 0x0022D928
		private void Awake()
		{
			foreach (CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect in this.allEffects)
			{
				this.allEffectsDict.TryAdd(cosmeticEffect.effectType, cosmeticEffect);
			}
		}

		// Token: 0x06006AA4 RID: 27300 RVA: 0x0022F764 File Offset: 0x0022D964
		public void SetKnockbackStrengthMultiplier(float value)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
			{
				keyValuePair.Value.knockbackStrengthMultiplier = value;
			}
		}

		// Token: 0x06006AA5 RID: 27301 RVA: 0x0022F7C0 File Offset: 0x0022D9C0
		public void ApplyAllEffects()
		{
			this.ApplyAllEffectsByDistance(base.transform.position);
		}

		// Token: 0x06006AA6 RID: 27302 RVA: 0x0022F7D3 File Offset: 0x0022D9D3
		public void ApplyAllEffectsByDistance(Transform _transform)
		{
			this.ApplyAllEffectsByDistance(_transform.position);
		}

		// Token: 0x06006AA7 RID: 27303 RVA: 0x0022F7E4 File Offset: 0x0022D9E4
		public void ApplyAllEffectsByDistance(Vector3 position)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.allEffectsDict)
			{
				switch (effect.Key)
				{
				case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
					this.ApplySkinByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
					this.ApplyTagWithKnockbackByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
					this.ApplyInstantKnockbackByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
					this.PlaySfxByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VFX:
					this.PlayVFXByDistance(effect, position);
					break;
				}
			}
		}

		// Token: 0x06006AA8 RID: 27304 RVA: 0x0022F88C File Offset: 0x0022DA8C
		public void ApplyAllEffectsForRig(VRRig rig)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.allEffectsDict)
			{
				switch (effect.Key)
				{
				case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
					this.ApplySkinForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
					this.ApplyTagWithKnockbackForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
					this.ApplyInstantKnockbackForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride:
					this.ApplyVOForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
					this.PlaySfxForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VFX:
					this.PlayVFXForRig(effect, rig);
					break;
				}
			}
		}

		// Token: 0x06006AA9 RID: 27305 RVA: 0x0022F940 File Offset: 0x0022DB40
		private void ApplySkinByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable2;
			if (!PhotonNetwork.InRoom)
			{
				IEnumerable<VRRig> enumerable = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<VRRig> enumerable = GorillaParent.instance.vrrigs;
				enumerable2 = enumerable;
			}
			foreach (VRRig vrrig in enumerable2)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.SpawnSkinEffects(effect);
				}
			}
		}

		// Token: 0x06006AAA RID: 27306 RVA: 0x0022FA34 File Offset: 0x0022DC34
		private void ApplySkinForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.SpawnSkinEffects(effect);
		}

		// Token: 0x06006AAB RID: 27307 RVA: 0x0022FAA4 File Offset: 0x0022DCA4
		private void ApplyTagWithKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.EnableHitWithKnockBack(effect);
		}

		// Token: 0x06006AAC RID: 27308 RVA: 0x0022FB14 File Offset: 0x0022DD14
		private void ApplyTagWithKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable2;
			if (!PhotonNetwork.InRoom)
			{
				IEnumerable<VRRig> enumerable = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<VRRig> enumerable = GorillaParent.instance.vrrigs;
				enumerable2 = enumerable;
			}
			foreach (VRRig vrrig in enumerable2)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.EnableHitWithKnockBack(effect);
				}
			}
		}

		// Token: 0x06006AAD RID: 27309 RVA: 0x0022FC08 File Offset: 0x0022DE08
		private void ApplyInstantKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			Vector3 vector = vrRig.transform.position - base.transform.position;
			float num = (1f / vector.magnitude * effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
			if (effect.Value.applyScaleToKnockbackStrength)
			{
				num *= vrRig.scaleFactor;
			}
			RoomSystem.HitPlayer(vrRig.creator, vector.normalized, num);
			vrRig.ApplyInstanceKnockBack(effect);
		}

		// Token: 0x06006AAE RID: 27310 RVA: 0x0022FD04 File Offset: 0x0022DF04
		private void ApplyInstantKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(GorillaTagger.Instance.offlineVRRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (GorillaTagger.Instance.offlineVRRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			Vector3 vector = GorillaTagger.Instance.offlineVRRig.transform.position - position;
			if (vector.IsShorterThan(effect.Value.effectDistanceRadius))
			{
				float magnitude = vector.magnitude;
				GTPlayer instance = GTPlayer.Instance;
				if (effect.Value.specialVerticalForce && (instance.IsHandTouching(true) || instance.IsHandTouching(false) || instance.BodyOnGround))
				{
					Vector3 vector2 = -Physics.gravity.normalized;
					Vector3 vector3 = Vector3.ProjectOnPlane(vector, vector2);
					vector = ((Vector3.Dot(vector / magnitude, vector2) > 0f) ? vector : vector3) + vector3.magnitude * vector2;
				}
				float num = (effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier / magnitude).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
				if (effect.Value.applyScaleToKnockbackStrength)
				{
					num *= instance.scale;
				}
				instance.ApplyKnockback(vector.normalized, num, effect.Value.forceOffTheGround);
				GorillaTagger.Instance.offlineVRRig.ApplyInstanceKnockBack(effect);
			}
		}

		// Token: 0x06006AAF RID: 27311 RVA: 0x0022FEB0 File Offset: 0x0022E0B0
		private void ApplyVOForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig rig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(rig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (rig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			rig.ActivateVOEffect(effect);
		}

		// Token: 0x06006AB0 RID: 27312 RVA: 0x0022FF20 File Offset: 0x0022E120
		private void PlaySfxForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.PlayCosmeticEffectSFX(effect);
		}

		// Token: 0x06006AB1 RID: 27313 RVA: 0x0022FF90 File Offset: 0x0022E190
		private void PlaySfxByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable2;
			if (!PhotonNetwork.InRoom)
			{
				IEnumerable<VRRig> enumerable = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<VRRig> enumerable = GorillaParent.instance.vrrigs;
				enumerable2 = enumerable;
			}
			foreach (VRRig vrrig in enumerable2)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.PlayCosmeticEffectSFX(effect);
				}
			}
		}

		// Token: 0x06006AB2 RID: 27314 RVA: 0x00230084 File Offset: 0x0022E284
		private void PlayVFXForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.SpawnVFXEffect(effect);
		}

		// Token: 0x06006AB3 RID: 27315 RVA: 0x002300F4 File Offset: 0x0022E2F4
		private void PlayVFXByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable2;
			if (!PhotonNetwork.InRoom)
			{
				IEnumerable<VRRig> enumerable = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<VRRig> enumerable = GorillaParent.instance.vrrigs;
				enumerable2 = enumerable;
			}
			foreach (VRRig vrrig in enumerable2)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.SpawnVFXEffect(effect);
				}
			}
		}

		// Token: 0x17000A04 RID: 2564
		// (get) Token: 0x06006AB4 RID: 27316 RVA: 0x002301E8 File Offset: 0x0022E3E8
		// (set) Token: 0x06006AB5 RID: 27317 RVA: 0x002301F0 File Offset: 0x0022E3F0
		public bool IsSpawned { get; set; }

		// Token: 0x17000A05 RID: 2565
		// (get) Token: 0x06006AB6 RID: 27318 RVA: 0x002301F9 File Offset: 0x0022E3F9
		// (set) Token: 0x06006AB7 RID: 27319 RVA: 0x00230201 File Offset: 0x0022E401
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006AB8 RID: 27320 RVA: 0x0023020A File Offset: 0x0022E40A
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06006AB9 RID: 27321 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x04007ACE RID: 31438
		public CosmeticEffectsOnPlayers.CosmeticEffect[] allEffects = new CosmeticEffectsOnPlayers.CosmeticEffect[0];

		// Token: 0x04007ACF RID: 31439
		private VRRig myRig;

		// Token: 0x04007AD0 RID: 31440
		private Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> allEffectsDict = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

		// Token: 0x020010A9 RID: 4265
		[Serializable]
		public enum TargetType
		{
			// Token: 0x04007AD4 RID: 31444
			Owner,
			// Token: 0x04007AD5 RID: 31445
			Others,
			// Token: 0x04007AD6 RID: 31446
			All
		}

		// Token: 0x020010AA RID: 4266
		[Serializable]
		public class CosmeticEffect
		{
			// Token: 0x17000A06 RID: 2566
			// (get) Token: 0x06006ABB RID: 27323 RVA: 0x00230232 File Offset: 0x0022E432
			// (set) Token: 0x06006ABC RID: 27324 RVA: 0x0023023A File Offset: 0x0022E43A
			public float knockbackStrengthMultiplier { get; set; }

			// Token: 0x06006ABD RID: 27325 RVA: 0x00230244 File Offset: 0x0022E444
			public bool IsGameModeAllowed()
			{
				GameModeType gameModeType = (GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual;
				return !Enumerable.Contains<GameModeType>(this.excludeForGameModes, gameModeType);
			}

			// Token: 0x17000A07 RID: 2567
			// (get) Token: 0x06006ABE RID: 27326 RVA: 0x0023027D File Offset: 0x0022E47D
			// (set) Token: 0x06006ABF RID: 27327 RVA: 0x00230285 File Offset: 0x0022E485
			public float EffectDuration
			{
				get
				{
					return this.effectDurationOthers;
				}
				set
				{
					this.effectDurationOthers = value;
				}
			}

			// Token: 0x17000A08 RID: 2568
			// (get) Token: 0x06006AC0 RID: 27328 RVA: 0x0023028E File Offset: 0x0022E48E
			// (set) Token: 0x06006AC1 RID: 27329 RVA: 0x00230296 File Offset: 0x0022E496
			public float EffectStartedTime { get; set; }

			// Token: 0x06006AC2 RID: 27330 RVA: 0x0023029F File Offset: 0x0022E49F
			private bool IsSkin()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin;
			}

			// Token: 0x06006AC3 RID: 27331 RVA: 0x002302AA File Offset: 0x0022E4AA
			private bool IsTagKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback;
			}

			// Token: 0x06006AC4 RID: 27332 RVA: 0x002302B5 File Offset: 0x0022E4B5
			private bool IsInstantKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
			}

			// Token: 0x06006AC5 RID: 27333 RVA: 0x002302C0 File Offset: 0x0022E4C0
			private bool HasKnockback()
			{
				CosmeticEffectsOnPlayers.EFFECTTYPE effecttype = this.effectType;
				return effecttype == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback || effecttype == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
			}

			// Token: 0x06006AC6 RID: 27334 RVA: 0x002302E5 File Offset: 0x0022E4E5
			private bool IsVO()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride;
			}

			// Token: 0x06006AC7 RID: 27335 RVA: 0x002302F0 File Offset: 0x0022E4F0
			private bool IsSFX()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.SFX;
			}

			// Token: 0x06006AC8 RID: 27336 RVA: 0x002302FB File Offset: 0x0022E4FB
			private bool IsVFX()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VFX;
			}

			// Token: 0x17000A09 RID: 2569
			// (get) Token: 0x06006AC9 RID: 27337 RVA: 0x00230306 File Offset: 0x0022E506
			private HashSet<GameModeType> Modes
			{
				get
				{
					if (this.modesHash == null)
					{
						this.modesHash = new HashSet<GameModeType>(this.excludeForGameModes);
					}
					return this.modesHash;
				}
			}

			// Token: 0x04007AD7 RID: 31447
			public GameModeType[] excludeForGameModes;

			// Token: 0x04007AD8 RID: 31448
			public CosmeticEffectsOnPlayers.EFFECTTYPE effectType;

			// Token: 0x04007AD9 RID: 31449
			public float effectDistanceRadius;

			// Token: 0x04007ADA RID: 31450
			public CosmeticEffectsOnPlayers.TargetType target = CosmeticEffectsOnPlayers.TargetType.All;

			// Token: 0x04007ADB RID: 31451
			public float effectDurationOthers;

			// Token: 0x04007ADC RID: 31452
			public float effectDurationOwner;

			// Token: 0x04007ADD RID: 31453
			public GorillaSkin newSkin;

			// Token: 0x04007ADE RID: 31454
			[Tooltip("Use object pools")]
			public GameObject knockbackVFX;

			// Token: 0x04007ADF RID: 31455
			[FormerlySerializedAs("knockbackStrengthMultiplier")]
			public float knockbackStrength;

			// Token: 0x04007AE0 RID: 31456
			public bool applyScaleToKnockbackStrength;

			// Token: 0x04007AE1 RID: 31457
			[Tooltip("force pushing players with hands on the ground")]
			public bool forceOffTheGround;

			// Token: 0x04007AE2 RID: 31458
			[Tooltip("Take the horizontal magnitude of the knockback, and add it opposite gravity. For example, being hit sideways will also impart a large upwards force. Breaks conservation of energy, but feels better to the player.")]
			public bool specialVerticalForce;

			// Token: 0x04007AE3 RID: 31459
			[FormerlySerializedAs("minStrengthClamp")]
			public float minKnockbackStrength = 0.5f;

			// Token: 0x04007AE4 RID: 31460
			[FormerlySerializedAs("maxStrengthClamp")]
			public float maxKnockbackStrength = 6f;

			// Token: 0x04007AE6 RID: 31462
			public AudioClip[] voiceOverrideNormalClips;

			// Token: 0x04007AE7 RID: 31463
			public AudioClip[] voiceOverrideLoudClips;

			// Token: 0x04007AE8 RID: 31464
			public float voiceOverrideNormalVolume = 0.5f;

			// Token: 0x04007AE9 RID: 31465
			public float voiceOverrideLoudVolume = 0.8f;

			// Token: 0x04007AEA RID: 31466
			public float voiceOverrideLoudThreshold = 0.175f;

			// Token: 0x04007AEB RID: 31467
			[Tooltip("plays sfx on player")]
			public List<AudioClip> sfxAudioClip;

			// Token: 0x04007AEC RID: 31468
			[Tooltip("plays vfx on player, must be in the global object pool and have a tag.")]
			public GameObject VFXGameObject;

			// Token: 0x04007AED RID: 31469
			private HashSet<GameModeType> modesHash;
		}

		// Token: 0x020010AB RID: 4267
		public enum EFFECTTYPE
		{
			// Token: 0x04007AF0 RID: 31472
			Skin,
			// Token: 0x04007AF1 RID: 31473
			[Obsolete("FPV has been removed, do not use, use Stick Object To Player instead")]
			TagWithKnockback = 2,
			// Token: 0x04007AF2 RID: 31474
			InstantKnockback,
			// Token: 0x04007AF3 RID: 31475
			VoiceOverride,
			// Token: 0x04007AF4 RID: 31476
			SFX,
			// Token: 0x04007AF5 RID: 31477
			VFX
		}
	}
}
