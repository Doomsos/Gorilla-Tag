using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000035 RID: 53
public class CosmeticPlaySoundOnColision : MonoBehaviour
{
	// Token: 0x060000C9 RID: 201 RVA: 0x00005668 File Offset: 0x00003868
	private void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.soundLookup = new Dictionary<int, int>();
		this.audioSource = base.GetComponent<AudioSource>();
		for (int i = 0; i < this.soundIdRemappings.Length; i++)
		{
			this.soundLookup.Add(this.soundIdRemappings[i].SoundIn, this.soundIdRemappings[i].SoundOut);
		}
	}

	// Token: 0x060000CA RID: 202 RVA: 0x000056D0 File Offset: 0x000038D0
	private void OnTriggerEnter(Collider other)
	{
		GorillaSurfaceOverride gorillaSurfaceOverride;
		if (this.speed >= this.minSpeed && other.TryGetComponent<GorillaSurfaceOverride>(ref gorillaSurfaceOverride))
		{
			int soundIndex;
			if (this.soundLookup.TryGetValue(gorillaSurfaceOverride.overrideIndex, ref soundIndex))
			{
				this.playSound(soundIndex, this.invokeEventOnOverideSound);
				return;
			}
			this.playSound(this.defaultSound, this.invokeEventOnDefaultSound);
		}
	}

	// Token: 0x060000CB RID: 203 RVA: 0x0000572C File Offset: 0x0000392C
	private void playSound(int soundIndex, bool invokeEvent)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			if (this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
				{
					this.OnStopPlayback.Invoke();
				}
				if (this.crWaitForStopPlayback != null)
				{
					base.StopCoroutine(this.crWaitForStopPlayback);
					this.crWaitForStopPlayback = null;
				}
			}
			this.audioSource.clip = GTPlayer.Instance.materialData[soundIndex].audio;
			this.audioSource.GTPlay();
			if (invokeEvent && (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem()))
			{
				this.OnStartPlayback.Invoke();
				this.crWaitForStopPlayback = base.StartCoroutine(this.waitForStopPlayback());
			}
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x00005808 File Offset: 0x00003A08
	private IEnumerator waitForStopPlayback()
	{
		while (this.audioSource.isPlaying)
		{
			yield return null;
		}
		if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
		{
			this.OnStopPlayback.Invoke();
		}
		this.crWaitForStopPlayback = null;
		yield break;
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00005817 File Offset: 0x00003A17
	private void FixedUpdate()
	{
		this.speed = Vector3.Distance(base.transform.position, this.previousFramePosition) * Time.fixedDeltaTime * 100f;
		this.previousFramePosition = base.transform.position;
	}

	// Token: 0x040000DC RID: 220
	[GorillaSoundLookup]
	[SerializeField]
	private int defaultSound = 1;

	// Token: 0x040000DD RID: 221
	[SerializeField]
	private SoundIdRemapping[] soundIdRemappings;

	// Token: 0x040000DE RID: 222
	[SerializeField]
	private UnityEvent OnStartPlayback;

	// Token: 0x040000DF RID: 223
	[SerializeField]
	private UnityEvent OnStopPlayback;

	// Token: 0x040000E0 RID: 224
	[SerializeField]
	private float minSpeed = 0.1f;

	// Token: 0x040000E1 RID: 225
	private TransferrableObject transferrableObject;

	// Token: 0x040000E2 RID: 226
	private Dictionary<int, int> soundLookup;

	// Token: 0x040000E3 RID: 227
	private AudioSource audioSource;

	// Token: 0x040000E4 RID: 228
	private Coroutine crWaitForStopPlayback;

	// Token: 0x040000E5 RID: 229
	private float speed;

	// Token: 0x040000E6 RID: 230
	private Vector3 previousFramePosition;

	// Token: 0x040000E7 RID: 231
	[SerializeField]
	private bool invokeEventsOnAllClients;

	// Token: 0x040000E8 RID: 232
	[SerializeField]
	private bool invokeEventOnOverideSound = true;

	// Token: 0x040000E9 RID: 233
	[SerializeField]
	private bool invokeEventOnDefaultSound;
}
