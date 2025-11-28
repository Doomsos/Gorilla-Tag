using System;
using UnityEngine;

// Token: 0x02000318 RID: 792
public class SoundOnCollisionTagSpecific : MonoBehaviour
{
	// Token: 0x06001349 RID: 4937 RVA: 0x0006FA9C File Offset: 0x0006DC9C
	private void OnTriggerEnter(Collider collider)
	{
		if (Time.time > this.nextSound && collider.gameObject.CompareTag(this.tagName))
		{
			this.nextSound = Time.time + this.noiseCooldown;
			this.audioSource.GTPlayOneShot(this.collisionSounds[Random.Range(0, this.collisionSounds.Length)], 0.5f);
		}
	}

	// Token: 0x04001CCC RID: 7372
	public string tagName;

	// Token: 0x04001CCD RID: 7373
	public float noiseCooldown = 1f;

	// Token: 0x04001CCE RID: 7374
	private float nextSound;

	// Token: 0x04001CCF RID: 7375
	public AudioSource audioSource;

	// Token: 0x04001CD0 RID: 7376
	public AudioClip[] collisionSounds;
}
