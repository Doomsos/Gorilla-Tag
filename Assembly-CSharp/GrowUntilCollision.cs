using System;
using UnityEngine;

// Token: 0x02000CB9 RID: 3257
public class GrowUntilCollision : MonoBehaviour
{
	// Token: 0x06004F82 RID: 20354 RVA: 0x00199770 File Offset: 0x00197970
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (this.audioSource != null)
		{
			this.maxVolume = this.audioSource.volume;
			this.maxPitch = this.audioSource.pitch;
		}
		this.zero();
	}

	// Token: 0x06004F83 RID: 20355 RVA: 0x001997C0 File Offset: 0x001979C0
	private void zero()
	{
		base.transform.localScale = Vector3.one * this.initialRadius;
		if (this.audioSource != null)
		{
			this.audioSource.volume = 0f;
			this.audioSource.pitch = 1f;
		}
		this.timeSinceTrigger = 0f;
	}

	// Token: 0x06004F84 RID: 20356 RVA: 0x00199821 File Offset: 0x00197A21
	private void OnTriggerEnter(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x06004F85 RID: 20357 RVA: 0x00199821 File Offset: 0x00197A21
	private void OnTriggerExit(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x06004F86 RID: 20358 RVA: 0x00199840 File Offset: 0x00197A40
	private void OnCollisionEnter(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x06004F87 RID: 20359 RVA: 0x00199870 File Offset: 0x00197A70
	private void OnCollisionExit(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x06004F88 RID: 20360 RVA: 0x0019989D File Offset: 0x00197A9D
	private void tryToTrigger(Vector3 p1, Vector3 p2)
	{
		if (this.timeSinceTrigger > this.minRetriggerTime)
		{
			if (this.colliderFound != null)
			{
				this.colliderFound.Invoke(p1, p2);
			}
			this.zero();
		}
	}

	// Token: 0x06004F89 RID: 20361 RVA: 0x001998C8 File Offset: 0x00197AC8
	private void Update()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		if (base.transform.localScale.x < this.maxSize * num)
		{
			base.transform.localScale += Vector3.one * Time.deltaTime * num;
			if (this.audioSource != null)
			{
				this.audioSource.volume = this.maxVolume * (base.transform.localScale.x / this.maxSize);
				this.audioSource.pitch = 1f + this.maxPitch * (base.transform.localScale.x / this.maxSize);
			}
		}
		this.timeSinceTrigger += Time.deltaTime;
	}

	// Token: 0x04005E01 RID: 24065
	[SerializeField]
	private float maxSize = 10f;

	// Token: 0x04005E02 RID: 24066
	[SerializeField]
	private float initialRadius = 1f;

	// Token: 0x04005E03 RID: 24067
	[SerializeField]
	private float minRetriggerTime = 1f;

	// Token: 0x04005E04 RID: 24068
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04005E05 RID: 24069
	private AudioSource audioSource;

	// Token: 0x04005E06 RID: 24070
	private float maxVolume;

	// Token: 0x04005E07 RID: 24071
	private float maxPitch;

	// Token: 0x04005E08 RID: 24072
	private float timeSinceTrigger;
}
