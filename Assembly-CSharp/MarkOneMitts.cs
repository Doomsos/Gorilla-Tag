using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class MarkOneMitts : HandTapBehaviour, ITickSystemTick, IProximityEffectReceiver
{
	// Token: 0x0600043C RID: 1084 RVA: 0x00018728 File Offset: 0x00016928
	private void Awake()
	{
		this.leftMitt.Init();
		this.rightMitt.Init();
		this.rig = base.GetComponentInParent<VRRig>();
		this.vibrateController = (this.vibrateController && this.rig.isOfflineVRRig);
		this.proximityEffect.AddReceiver(this);
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x0001877F File Offset: 0x0001697F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00018790 File Offset: 0x00016990
	public void OnProximityCalculated(float distance, float alignment, float parallel)
	{
		float num = distance * alignment * parallel;
		if (num > 0.1f)
		{
			float speed = this.proximitySpeedCurve.Evaluate(num);
			float num2 = this.proximitySpreadCurve.Evaluate(num);
			ParticleSystem.MinMaxCurve xy;
			xy..ctor(-num2, num2);
			this.StartFlame(this.leftMitt, num, speed, xy);
			this.StartFlame(this.rightMitt, num, speed, xy);
			if (this.vibrateController && this.vibrationStrengthMult > 0f)
			{
				GorillaTagger.Instance.StartVibration(true, this.vibrationStrengthMult * 0.5f * num, Time.deltaTime);
				GorillaTagger.Instance.StartVibration(false, this.vibrationStrengthMult * 0.5f * num, Time.deltaTime);
			}
			this.SetInterferenceAudio(true);
			float num3 = 1f - Mathf.Exp(-this.proximityAudioReactionSpeed * Time.deltaTime);
			this.proximityAudioSource.pitch = Mathf.Lerp(this.proximityAudioSource.pitch, this.proximityAudioPitch.Evaluate(num), num3);
			this.proximityAudioSource.volume = Mathf.Lerp(this.proximityAudioSource.volume, this.proximityAudioVolume.Evaluate(num), num3);
			return;
		}
		if (this.leftMitt.thermalSource.enabled || this.rightMitt.thermalSource.enabled)
		{
			this.leftMitt.flame.Stop();
			this.leftMitt.thermalSource.enabled = false;
			this.rightMitt.flame.Stop();
			this.rightMitt.thermalSource.enabled = false;
			this.SetInterferenceAudio(false);
		}
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00018924 File Offset: 0x00016B24
	private void StartFlame(MarkOneMitts.Mitt mitt, float scale, float speed, ParticleSystem.MinMaxCurve xy)
	{
		if (!mitt.thermalSource.enabled)
		{
			mitt.flame.Play();
			mitt.thermalSource.enabled = true;
		}
		mitt.flameTransform.localScale = this.flameScale * scale * Vector3.one;
		mitt.flameMain.startSpeed = speed;
		mitt.flameForce.x = xy;
		mitt.flameForce.y = xy;
		mitt.thermalSource.celsius = this.heatMultiplier * scale;
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x000189B0 File Offset: 0x00016BB0
	private void RunTimer(MarkOneMitts.Mitt mitt, bool isLeftHand)
	{
		if (mitt.timer <= 0f)
		{
			return;
		}
		mitt.timer -= Time.deltaTime;
		if (mitt.timer <= 0f)
		{
			mitt.timer = 0f;
			mitt.flame.Stop();
			mitt.thermalSource.enabled = false;
			if (this.leftMitt.timer <= 0f && this.rightMitt.timer <= 0f)
			{
				this.proximityEffect.enabled = true;
				return;
			}
		}
		else
		{
			float num = mitt.lastTapStrength * mitt.timer;
			mitt.flameTransform.localScale = this.flameScale * num * Vector3.one;
			mitt.thermalSource.celsius = this.heatMultiplier * num;
			if (this.vibrateController)
			{
				GorillaTagger.Instance.StartVibration(isLeftHand, this.vibrationStrengthMult * num, 0.1f);
			}
		}
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x00018A9B File Offset: 0x00016C9B
	private void TryPlayProximityStartStopAudio(AudioClip clip, float volume)
	{
		if (this.proximityStartStopAudioSource.isPlaying)
		{
			return;
		}
		this.proximityStartStopAudioSource.clip = clip;
		this.proximityStartStopAudioSource.volume = volume;
		this.proximityStartStopAudioSource.Play();
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00018AD0 File Offset: 0x00016CD0
	private void SetInterferenceAudio(bool active)
	{
		if (this.proximityAudioSource.isPlaying == active)
		{
			return;
		}
		if (active)
		{
			this.TryPlayProximityStartStopAudio(this.proximityStartAudioClip, this.proximityStartAudioVolume);
			this.proximityAudioSource.Play();
			return;
		}
		this.TryPlayProximityStartStopAudio(this.proximityStopAudioClip, this.proximityStopAudioVolume);
		this.proximityAudioSource.Stop();
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000444 RID: 1092 RVA: 0x00018B2A File Offset: 0x00016D2A
	// (set) Token: 0x06000445 RID: 1093 RVA: 0x00018B32 File Offset: 0x00016D32
	public bool TickRunning { get; set; }

	// Token: 0x06000446 RID: 1094 RVA: 0x00018B3C File Offset: 0x00016D3C
	public void Tick()
	{
		if (this.leftMitt.timer <= 0f && this.rightMitt.timer <= 0f)
		{
			TickSystem<object>.RemoveTickCallback(this);
			return;
		}
		this.RunTimer(this.leftMitt, true);
		this.RunTimer(this.rightMitt, false);
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00018B90 File Offset: 0x00016D90
	internal override void OnTap(HandEffectContext handContext)
	{
		float num = this.handSpeedToEffectStrength.Evaluate(handContext.Speed);
		if (num >= this.minEffectStrength)
		{
			TickSystem<object>.AddTickCallback(this);
			MarkOneMitts.Mitt mitt = handContext.isLeftHand ? this.leftMitt : this.rightMitt;
			mitt.lastTapStrength = num;
			mitt.timer = this.flameTime;
			mitt.bursts[0].count = num * 10f;
			mitt.bursts[1].count = num * 5f;
			mitt.burst.emission.SetBursts(mitt.bursts);
			mitt.burstTransform.localScale = num * Vector3.one;
			this.StartFlame(mitt, num * this.flameScale * this.flameTime, this.flameSpeed, this.emptyParticleCurve);
			mitt.burst.Play();
			Keyframe[] keys = this.handSpeedToEffectStrength.keys;
			float value = keys[keys.Length - 1].value;
			handContext.soundPitch = Mathf.Clamp(value / num, 1f, 3f);
			this.proximityEffect.enabled = false;
			return;
		}
		handContext.soundFX = null;
	}

	// Token: 0x040004B7 RID: 1207
	[SerializeField]
	private MarkOneMitts.Mitt leftMitt;

	// Token: 0x040004B8 RID: 1208
	[SerializeField]
	private MarkOneMitts.Mitt rightMitt;

	// Token: 0x040004B9 RID: 1209
	[SerializeField]
	private ProximityEffect proximityEffect;

	// Token: 0x040004BA RID: 1210
	[SerializeField]
	private AnimationCurve handSpeedToEffectStrength;

	// Token: 0x040004BB RID: 1211
	[SerializeField]
	private float minEffectStrength = 0.5f;

	// Token: 0x040004BC RID: 1212
	[SerializeField]
	private float flameScale = 3f;

	// Token: 0x040004BD RID: 1213
	[SerializeField]
	private float flameTime = 0.5f;

	// Token: 0x040004BE RID: 1214
	[SerializeField]
	private float flameSpeed = 5f;

	// Token: 0x040004BF RID: 1215
	[SerializeField]
	private float heatMultiplier = 100f;

	// Token: 0x040004C0 RID: 1216
	[SerializeField]
	private AnimationCurve proximitySpeedCurve;

	// Token: 0x040004C1 RID: 1217
	[SerializeField]
	private AnimationCurve proximitySpreadCurve;

	// Token: 0x040004C2 RID: 1218
	[Space]
	[SerializeField]
	private bool vibrateController;

	// Token: 0x040004C3 RID: 1219
	[SerializeField]
	private float vibrationStrengthMult = 1f;

	// Token: 0x040004C4 RID: 1220
	[Space]
	[SerializeField]
	private AudioSource proximityAudioSource;

	// Token: 0x040004C5 RID: 1221
	[SerializeField]
	private AnimationCurve proximityAudioPitch;

	// Token: 0x040004C6 RID: 1222
	[SerializeField]
	private AnimationCurve proximityAudioVolume;

	// Token: 0x040004C7 RID: 1223
	[SerializeField]
	private float proximityAudioReactionSpeed = 0.2f;

	// Token: 0x040004C8 RID: 1224
	[Space]
	[SerializeField]
	private AudioSource proximityStartStopAudioSource;

	// Token: 0x040004C9 RID: 1225
	[SerializeField]
	private AudioClip proximityStartAudioClip;

	// Token: 0x040004CA RID: 1226
	[SerializeField]
	private float proximityStartAudioVolume = 0.5f;

	// Token: 0x040004CB RID: 1227
	[SerializeField]
	private AudioClip proximityStopAudioClip;

	// Token: 0x040004CC RID: 1228
	[SerializeField]
	private float proximityStopAudioVolume = 0.5f;

	// Token: 0x040004CD RID: 1229
	private VRRig rig;

	// Token: 0x040004CE RID: 1230
	private ParticleSystem.MinMaxCurve emptyParticleCurve = new ParticleSystem.MinMaxCurve(0f);

	// Token: 0x020000A8 RID: 168
	[Serializable]
	private class Mitt
	{
		// Token: 0x06000449 RID: 1097 RVA: 0x00018D50 File Offset: 0x00016F50
		public void Init()
		{
			this.bursts = new ParticleSystem.Burst[2];
			this.burst.emission.GetBursts(this.bursts);
			this.burstTransform = this.burst.transform;
			this.flameTransform = this.flame.transform;
			this.flameMain = this.flame.main;
			this.flameForce = this.flame.forceOverLifetime;
		}

		// Token: 0x040004D0 RID: 1232
		public ParticleSystem burst;

		// Token: 0x040004D1 RID: 1233
		public ParticleSystem flame;

		// Token: 0x040004D2 RID: 1234
		public ThermalSourceVolume thermalSource;

		// Token: 0x040004D3 RID: 1235
		[NonSerialized]
		public float lastTapStrength;

		// Token: 0x040004D4 RID: 1236
		[NonSerialized]
		public ParticleSystem.Burst[] bursts;

		// Token: 0x040004D5 RID: 1237
		[NonSerialized]
		public Transform burstTransform;

		// Token: 0x040004D6 RID: 1238
		[NonSerialized]
		public Transform flameTransform;

		// Token: 0x040004D7 RID: 1239
		[NonSerialized]
		public float timer;

		// Token: 0x040004D8 RID: 1240
		[NonSerialized]
		public ParticleSystem.MainModule flameMain;

		// Token: 0x040004D9 RID: 1241
		[NonSerialized]
		public ParticleSystem.ForceOverLifetimeModule flameForce;
	}
}
