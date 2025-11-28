using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000CAF RID: 3247
public class FireworksController : MonoBehaviour
{
	// Token: 0x06004F55 RID: 20309 RVA: 0x00198CF2 File Offset: 0x00196EF2
	private void Awake()
	{
		this._launchOrder = Enumerable.ToArray<Firework>(this.fireworks);
		this._rnd = new SRand(this.seed);
	}

	// Token: 0x06004F56 RID: 20310 RVA: 0x00198D18 File Offset: 0x00196F18
	public void LaunchVolley()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this._rnd.Shuffle<Firework>(this._launchOrder);
		for (int i = 0; i < this._launchOrder.Length; i++)
		{
			MonoBehaviour monoBehaviour = this._launchOrder[i];
			float num = this._rnd.NextFloat() * this.roundLength;
			monoBehaviour.Invoke("Launch", num);
		}
	}

	// Token: 0x06004F57 RID: 20311 RVA: 0x00198D7C File Offset: 0x00196F7C
	public void LaunchVolleyRound()
	{
		int num = 0;
		while ((long)num < (long)((ulong)this.roundNumVolleys))
		{
			float num2 = this._rnd.NextFloat() * this.roundLength;
			base.Invoke("LaunchVolley", num2);
			num++;
		}
	}

	// Token: 0x06004F58 RID: 20312 RVA: 0x00198DC0 File Offset: 0x00196FC0
	public void Launch(Firework fw)
	{
		if (!fw)
		{
			return;
		}
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		AudioSource sourceOrigin = fw.sourceOrigin;
		int num = this._rnd.NextInt(this.bursts.Length);
		AudioClip audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		AudioClip audioClip2 = this.bursts[num];
		while (this._lastWhistle == audioClip)
		{
			audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		}
		while (this._lastBurst == audioClip2)
		{
			num = this._rnd.NextInt(this.bursts.Length);
			audioClip2 = this.bursts[num];
		}
		this._lastWhistle = audioClip;
		this._lastBurst = audioClip2;
		int num2 = this._rnd.NextInt(fw.explosions.Length);
		ParticleSystem particleSystem = fw.explosions[num2];
		if (fw.doTrail)
		{
			ParticleSystem trail = fw.trail;
			trail.startColor = fw.colorOrigin;
			trail.subEmitters.GetSubEmitterSystem(0).colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fw.colorOrigin, fw.colorTarget);
			trail.Stop();
			trail.Play();
		}
		sourceOrigin.pitch = this._rnd.NextFloat(0.92f, 1f);
		fw.doTrailAudio = this._rnd.NextBool();
		FireworksController.ExplosionEvent ev = new FireworksController.ExplosionEvent
		{
			firework = fw,
			timeSince = TimeSince.Now(),
			burstIndex = num,
			explosionIndex = num2,
			delay = (double)(fw.doTrail ? audioClip.length : 0f),
			active = true
		};
		if (fw.doExplosion)
		{
			this.PostExplosionEvent(ev);
		}
		if (fw.doTrailAudio && this._timeSinceLastWhistle > this.minWhistleDelay)
		{
			this._timeSinceLastWhistle = TimeSince.Now();
			sourceOrigin.PlayOneShot(audioClip, this._rnd.NextFloat(this.whistleVolumeMin, this.whistleVolumeMax));
		}
		particleSystem.Stop();
		particleSystem.transform.position = position2;
	}

	// Token: 0x06004F59 RID: 20313 RVA: 0x00198FF0 File Offset: 0x001971F0
	private void PostExplosionEvent(FireworksController.ExplosionEvent ev)
	{
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			if (!this._explosionQueue[i].active)
			{
				this._explosionQueue[i] = ev;
				return;
			}
		}
	}

	// Token: 0x06004F5A RID: 20314 RVA: 0x00199031 File Offset: 0x00197231
	private void Update()
	{
		this.ProcessEvents();
	}

	// Token: 0x06004F5B RID: 20315 RVA: 0x0019903C File Offset: 0x0019723C
	private void ProcessEvents()
	{
		if (this._explosionQueue == null || this._explosionQueue.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			FireworksController.ExplosionEvent explosionEvent = this._explosionQueue[i];
			if (explosionEvent.active && explosionEvent.timeSince >= explosionEvent.delay)
			{
				this.DoExplosion(explosionEvent);
				this._explosionQueue[i] = default(FireworksController.ExplosionEvent);
			}
		}
	}

	// Token: 0x06004F5C RID: 20316 RVA: 0x001990B0 File Offset: 0x001972B0
	private void DoExplosion(FireworksController.ExplosionEvent ev)
	{
		Firework firework = ev.firework;
		ParticleSystem particleSystem = firework.explosions[ev.explosionIndex];
		ParticleSystem.MinMaxGradient color;
		color..ctor(firework.colorOrigin, firework.colorTarget);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = particleSystem.subEmitters.GetSubEmitterSystem(0).colorOverLifetime;
		colorOverLifetime.color = color;
		colorOverLifetime2.color = color;
		ParticleSystem particleSystem2 = firework.explosions[ev.explosionIndex];
		particleSystem2.Stop();
		particleSystem2.Play();
		firework.sourceTarget.PlayOneShot(this.bursts[ev.burstIndex]);
	}

	// Token: 0x06004F5D RID: 20317 RVA: 0x00199140 File Offset: 0x00197340
	public void RenderGizmo(Firework fw, Color c)
	{
		if (!fw)
		{
			return;
		}
		if (!fw.origin || !fw.target)
		{
			return;
		}
		Gizmos.color = c;
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		Gizmos.DrawLine(position, position2);
		Gizmos.DrawWireCube(position, Vector3.one * 0.5f);
		Gizmos.DrawWireCube(position2, Vector3.one * 0.5f);
	}

	// Token: 0x04005DCD RID: 24013
	public Firework[] fireworks;

	// Token: 0x04005DCE RID: 24014
	public AudioClip[] whistles;

	// Token: 0x04005DCF RID: 24015
	public AudioClip[] bursts;

	// Token: 0x04005DD0 RID: 24016
	[Space]
	[Range(0f, 1f)]
	public float whistleVolumeMin = 0.1f;

	// Token: 0x04005DD1 RID: 24017
	[Range(0f, 1f)]
	public float whistleVolumeMax = 0.15f;

	// Token: 0x04005DD2 RID: 24018
	public float minWhistleDelay = 1f;

	// Token: 0x04005DD3 RID: 24019
	[Space]
	[NonSerialized]
	private AudioClip _lastWhistle;

	// Token: 0x04005DD4 RID: 24020
	[NonSerialized]
	private AudioClip _lastBurst;

	// Token: 0x04005DD5 RID: 24021
	[NonSerialized]
	private Firework[] _launchOrder;

	// Token: 0x04005DD6 RID: 24022
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x04005DD7 RID: 24023
	[NonSerialized]
	private FireworksController.ExplosionEvent[] _explosionQueue = new FireworksController.ExplosionEvent[8];

	// Token: 0x04005DD8 RID: 24024
	[NonSerialized]
	private TimeSince _timeSinceLastWhistle = 10f;

	// Token: 0x04005DD9 RID: 24025
	[Space]
	public string seed = "Fireworks.Summer23";

	// Token: 0x04005DDA RID: 24026
	[Space]
	public uint roundNumVolleys = 6U;

	// Token: 0x04005DDB RID: 24027
	public uint roundLength = 6U;

	// Token: 0x04005DDC RID: 24028
	[FormerlySerializedAs("_timeOfDayEvent")]
	[FormerlySerializedAs("_timeOfDay")]
	[Space]
	[SerializeField]
	private TimeEvent _fireworksEvent;

	// Token: 0x02000CB0 RID: 3248
	[Serializable]
	public struct ExplosionEvent
	{
		// Token: 0x04005DDD RID: 24029
		public TimeSince timeSince;

		// Token: 0x04005DDE RID: 24030
		public double delay;

		// Token: 0x04005DDF RID: 24031
		public int explosionIndex;

		// Token: 0x04005DE0 RID: 24032
		public int burstIndex;

		// Token: 0x04005DE1 RID: 24033
		public bool active;

		// Token: 0x04005DE2 RID: 24034
		public Firework firework;
	}
}
