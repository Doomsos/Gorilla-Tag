using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class RotationSoundPlayer : MonoBehaviour
{
	// Token: 0x06000B56 RID: 2902 RVA: 0x0003D90C File Offset: 0x0003BB0C
	private void Awake()
	{
		List<Transform> list = new List<Transform>(this.transforms);
		list.RemoveAll((Transform xform) => xform == null);
		this.transforms = list.ToArray();
		this.initialUpAxis = new Vector3[this.transforms.Length];
		this.lastUpAxis = new Vector3[this.transforms.Length];
		this.lastRotationSpeeds = new float[this.transforms.Length];
		for (int i = 0; i < this.transforms.Length; i++)
		{
			this.initialUpAxis[i] = this.transforms[i].localRotation * Vector3.up;
			this.lastUpAxis[i] = this.initialUpAxis[i];
			this.lastRotationSpeeds[i] = 0f;
		}
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0003D9EC File Offset: 0x0003BBEC
	private void Update()
	{
		this.cooldownTimer -= Time.deltaTime;
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Vector3 vector = this.transforms[i].localRotation * Vector3.up;
			float num = Vector3.Angle(vector, this.initialUpAxis[i]);
			float num2 = Vector3.Angle(vector, this.lastUpAxis[i]);
			float deltaTime = Time.deltaTime;
			float num3 = num2 / deltaTime;
			if (this.cooldownTimer <= 0f && num > this.rotationAmountThreshold && num3 > this.rotationSpeedThreshold && !this.soundBankPlayer.isPlaying)
			{
				this.cooldownTimer = this.cooldown;
				this.soundBankPlayer.Play();
			}
			this.lastUpAxis[i] = vector;
			this.lastRotationSpeeds[i] = num3;
		}
	}

	// Token: 0x04000DDD RID: 3549
	[Tooltip("Transforms that will make a noise when they rotate.")]
	[SerializeField]
	private Transform[] transforms;

	// Token: 0x04000DDE RID: 3550
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04000DDF RID: 3551
	[Tooltip("How much the transform must rotate from it's initial rotation before a sound is played.")]
	private float rotationAmountThreshold = 30f;

	// Token: 0x04000DE0 RID: 3552
	[Tooltip("How fast the transform must rotate before a sound is played.")]
	private float rotationSpeedThreshold = 45f;

	// Token: 0x04000DE1 RID: 3553
	private float cooldown = 0.6f;

	// Token: 0x04000DE2 RID: 3554
	private float cooldownTimer;

	// Token: 0x04000DE3 RID: 3555
	private Vector3[] initialUpAxis;

	// Token: 0x04000DE4 RID: 3556
	private Vector3[] lastUpAxis;

	// Token: 0x04000DE5 RID: 3557
	private float[] lastRotationSpeeds;
}
