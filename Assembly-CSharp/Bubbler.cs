using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004EE RID: 1262
public class Bubbler : TransferrableObject
{
	// Token: 0x06002073 RID: 8307 RVA: 0x000AC1A0 File Offset: 0x000AA3A0
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasParticleSystem = (this.bubbleParticleSystem != null);
		if (this.hasParticleSystem)
		{
			this.bubbleParticleArray = new ParticleSystem.Particle[this.bubbleParticleSystem.main.maxParticles];
			this.bubbleParticleSystem.trigger.SetCollider(0, GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<SphereCollider>());
			this.bubbleParticleSystem.trigger.SetCollider(1, GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<SphereCollider>());
		}
		this.initialTriggerDuration = 0.05f;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06002074 RID: 8308 RVA: 0x000AC244 File Offset: 0x000AA444
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasBubblerAudio = (this.bubblerAudio != null && this.bubblerAudio.clip != null);
		this.hasPopBubbleAudio = (this.popBubbleAudio != null && this.popBubbleAudio.clip != null);
		this.hasFan = (this.fan != null);
		this.hasActiveOnlyComponent = (this.gameObjectActiveOnlyWhileTriggerDown != null);
	}

	// Token: 0x06002075 RID: 8309 RVA: 0x000AC2D4 File Offset: 0x000AA4D4
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x000AC328 File Offset: 0x000AA528
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
		this.currentParticles.Clear();
		this.particleInfoDict.Clear();
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x000AC398 File Offset: 0x000AA598
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06002078 RID: 8312 RVA: 0x000AC3A6 File Offset: 0x000AA5A6
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this._worksInWater && GTPlayer.Instance.InWater)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x06002079 RID: 8313 RVA: 0x000AC3CC File Offset: 0x000AA5CC
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		bool forLeftController = this.currentState == TransferrableObject.PositionState.InLeftHand;
		bool enabled = this.itemState != TransferrableObject.ItemStates.State0;
		Behaviour[] array = this.behavioursToEnableWhenTriggerPressed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = enabled;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
			{
				this.bubbleParticleSystem.Stop();
			}
			if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTStop();
			}
			if (this.hasActiveOnlyComponent)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(false);
			}
		}
		else
		{
			if (this.hasParticleSystem && !this.bubbleParticleSystem.isEmitting)
			{
				this.bubbleParticleSystem.Play();
			}
			if (this.hasBubblerAudio && !this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTPlay();
			}
			if (this.hasActiveOnlyComponent && !this.gameObjectActiveOnlyWhileTriggerDown.activeSelf)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(true);
			}
			if (this.IsMyItem())
			{
				this.initialTriggerPull = Time.time;
				GorillaTagger.Instance.StartVibration(forLeftController, this.triggerStrength, this.initialTriggerDuration);
				if (Time.time > this.initialTriggerPull + this.initialTriggerDuration)
				{
					GorillaTagger.Instance.StartVibration(forLeftController, this.ongoingStrength, Time.deltaTime);
				}
			}
			if (this.hasFan)
			{
				if (!this.fanYaxisinstead)
				{
					float num = this.fan.transform.localEulerAngles.z + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, 0f, num);
				}
				else
				{
					float num2 = this.fan.transform.localEulerAngles.y + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, num2, 0f);
				}
			}
		}
		if (this.hasParticleSystem && (!this.allBubblesPopped || this.itemState == TransferrableObject.ItemStates.State1))
		{
			int particles = this.bubbleParticleSystem.GetParticles(this.bubbleParticleArray);
			this.allBubblesPopped = (particles <= 0);
			if (!this.allBubblesPopped)
			{
				for (int j = 0; j < particles; j++)
				{
					if (this.currentParticles.Contains(this.bubbleParticleArray[j].randomSeed))
					{
						this.currentParticles.Remove(this.bubbleParticleArray[j].randomSeed);
					}
				}
				foreach (uint num3 in this.currentParticles)
				{
					if (this.particleInfoDict.TryGetValue(num3, ref this.outPosition))
					{
						if (this.hasPopBubbleAudio)
						{
							GTAudioSourceExtensions.GTPlayClipAtPoint(this.popBubbleAudio.clip, this.outPosition);
						}
						this.particleInfoDict.Remove(num3);
					}
				}
				this.currentParticles.Clear();
				for (int k = 0; k < particles; k++)
				{
					if (this.particleInfoDict.TryGetValue(this.bubbleParticleArray[k].randomSeed, ref this.outPosition))
					{
						this.particleInfoDict[this.bubbleParticleArray[k].randomSeed] = this.bubbleParticleArray[k].position;
					}
					else
					{
						this.particleInfoDict.Add(this.bubbleParticleArray[k].randomSeed, this.bubbleParticleArray[k].position);
					}
					this.currentParticles.Add(this.bubbleParticleArray[k].randomSeed);
				}
			}
		}
	}

	// Token: 0x0600207A RID: 8314 RVA: 0x000AC7D8 File Offset: 0x000AA9D8
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x0600207B RID: 8315 RVA: 0x0003DD3C File Offset: 0x0003BF3C
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x000AC7E7 File Offset: 0x000AA9E7
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x000AC7F2 File Offset: 0x000AA9F2
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002AF7 RID: 10999
	[SerializeField]
	private bool _worksInWater = true;

	// Token: 0x04002AF8 RID: 11000
	public ParticleSystem bubbleParticleSystem;

	// Token: 0x04002AF9 RID: 11001
	private ParticleSystem.Particle[] bubbleParticleArray;

	// Token: 0x04002AFA RID: 11002
	public AudioSource bubblerAudio;

	// Token: 0x04002AFB RID: 11003
	public AudioSource popBubbleAudio;

	// Token: 0x04002AFC RID: 11004
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x04002AFD RID: 11005
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x04002AFE RID: 11006
	private Vector3 outPosition;

	// Token: 0x04002AFF RID: 11007
	private bool allBubblesPopped;

	// Token: 0x04002B00 RID: 11008
	public bool disableActivation;

	// Token: 0x04002B01 RID: 11009
	public bool disableDeactivation;

	// Token: 0x04002B02 RID: 11010
	public float rotationSpeed = 5f;

	// Token: 0x04002B03 RID: 11011
	public GameObject fan;

	// Token: 0x04002B04 RID: 11012
	public bool fanYaxisinstead;

	// Token: 0x04002B05 RID: 11013
	public float ongoingStrength = 0.005f;

	// Token: 0x04002B06 RID: 11014
	public float triggerStrength = 0.2f;

	// Token: 0x04002B07 RID: 11015
	private float initialTriggerPull;

	// Token: 0x04002B08 RID: 11016
	private float initialTriggerDuration;

	// Token: 0x04002B09 RID: 11017
	private bool hasBubblerAudio;

	// Token: 0x04002B0A RID: 11018
	private bool hasPopBubbleAudio;

	// Token: 0x04002B0B RID: 11019
	public GameObject gameObjectActiveOnlyWhileTriggerDown;

	// Token: 0x04002B0C RID: 11020
	public Behaviour[] behavioursToEnableWhenTriggerPressed;

	// Token: 0x04002B0D RID: 11021
	private bool hasParticleSystem;

	// Token: 0x04002B0E RID: 11022
	private bool hasFan;

	// Token: 0x04002B0F RID: 11023
	private bool hasActiveOnlyComponent;

	// Token: 0x020004EF RID: 1263
	private enum BubblerState
	{
		// Token: 0x04002B11 RID: 11025
		None = 1,
		// Token: 0x04002B12 RID: 11026
		Bubbling
	}
}
