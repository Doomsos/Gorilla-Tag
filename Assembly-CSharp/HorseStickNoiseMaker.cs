using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class HorseStickNoiseMaker : MonoBehaviour
{
	// Token: 0x06000B65 RID: 2917 RVA: 0x0003DD74 File Offset: 0x0003BF74
	protected void OnEnable()
	{
		if (!this.gorillaPlayerXform && !base.transform.TryFindByPath(this.gorillaPlayerXform_path, out this.gorillaPlayerXform, false))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"HorseStickNoiseMaker: DEACTIVATING! Could not find gorillaPlayerXform using path: \"",
				this.gorillaPlayerXform_path,
				"\"\nThis component's transform path: \"",
				base.transform.GetPath(),
				"\""
			}));
			base.gameObject.SetActive(false);
			return;
		}
		this.oldPos = this.gorillaPlayerXform.position;
		this.distElapsed = 0f;
		this.timeSincePlay = 0f;
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0003DE1C File Offset: 0x0003C01C
	protected void LateUpdate()
	{
		Vector3 position = this.gorillaPlayerXform.position;
		Vector3 vector = position - this.oldPos;
		this.distElapsed += vector.magnitude;
		this.timeSincePlay += Time.deltaTime;
		this.oldPos = position;
		if (this.distElapsed >= this.metersPerClip && this.timeSincePlay >= this.minSecBetweenClips)
		{
			this.soundBankPlayer.Play();
			this.distElapsed = 0f;
			this.timeSincePlay = 0f;
			if (this.particleFX != null)
			{
				this.particleFX.Play();
			}
		}
	}

	// Token: 0x04000DF2 RID: 3570
	[Tooltip("Meters the object should traverse between playing a provided audio clip.")]
	public float metersPerClip = 4f;

	// Token: 0x04000DF3 RID: 3571
	[Tooltip("Number of seconds that must elapse before playing another audio clip.")]
	public float minSecBetweenClips = 1.5f;

	// Token: 0x04000DF4 RID: 3572
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000DF5 RID: 3573
	[Tooltip("Transform assigned in Gorilla Player Networked Prefab to the Gorilla Player Networked parent to keep track of distance traveled.")]
	public Transform gorillaPlayerXform;

	// Token: 0x04000DF6 RID: 3574
	[Delayed]
	public string gorillaPlayerXform_path;

	// Token: 0x04000DF7 RID: 3575
	[Tooltip("Optional particle FX to spawn when sound plays")]
	public ParticleSystem particleFX;

	// Token: 0x04000DF8 RID: 3576
	private Vector3 oldPos;

	// Token: 0x04000DF9 RID: 3577
	private float timeSincePlay;

	// Token: 0x04000DFA RID: 3578
	private float distElapsed;
}
