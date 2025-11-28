using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class MicrophoneCosmetic : MonoBehaviour
{
	// Token: 0x06000C05 RID: 3077 RVA: 0x00040C20 File Offset: 0x0003EE20
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (!Application.isEditor && Application.platform == 11 && Microphone.devices.Length != 0)
		{
			this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 16000);
		}
		else
		{
			int sampleRate = AudioSettings.GetConfiguration().sampleRate;
			this.audioSource.clip = Microphone.Start(null, true, 10, sampleRate);
		}
		this.audioSource.loop = true;
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00040CA0 File Offset: 0x0003EEA0
	private void OnEnable()
	{
		int num = (Application.platform == 11 && Microphone.devices.Length != 0) ? Microphone.GetPosition(Microphone.devices[0]) : Microphone.GetPosition(null);
		num -= 10;
		if ((float)num < 0f)
		{
			num = this.audioSource.clip.samples + num - 1;
		}
		this.audioSource.GTPlay();
		this.audioSource.timeSamples = num;
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00040D0D File Offset: 0x0003EF0D
	private void OnDisable()
	{
		this.audioSource.GTStop();
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00040D1C File Offset: 0x0003EF1C
	private void Update()
	{
		Vector3 vector = this.mouthTransform.position - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = 0f;
		if (sqrMagnitude < this.mouthProximityRampRange.x * this.mouthProximityRampRange.x)
		{
			float magnitude = vector.magnitude;
			num = Mathf.InverseLerp(this.mouthProximityRampRange.x, this.mouthProximityRampRange.y, magnitude);
		}
		if (num != this.audioSource.volume)
		{
			this.audioSource.volume = num;
		}
		int num2 = this.audioSource.timeSamples -= 10;
		if ((float)num2 < 0f)
		{
			num2 = this.audioSource.clip.samples + num2 - 1;
		}
		this.audioSource.clip.SetData(this.zero, num2);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x00002789 File Offset: 0x00000989
	private void OnAudioFilterRead(float[] data, int channels)
	{
	}

	// Token: 0x04000EB1 RID: 3761
	[SerializeField]
	private Transform mouthTransform;

	// Token: 0x04000EB2 RID: 3762
	[SerializeField]
	private Vector2 mouthProximityRampRange = new Vector2(0.6f, 0.3f);

	// Token: 0x04000EB3 RID: 3763
	private AudioSource audioSource;

	// Token: 0x04000EB4 RID: 3764
	private float[] zero = new float[1];
}
