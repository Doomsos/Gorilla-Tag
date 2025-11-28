using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009B1 RID: 2481
public class VirtualStumpBarrierSFX : MonoBehaviour
{
	// Token: 0x06003F59 RID: 16217 RVA: 0x00153E74 File Offset: 0x00152074
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.PlaySFX();
			return;
		}
		VRRig vrrig;
		if (other.gameObject.TryGetComponent<VRRig>(ref vrrig) && !vrrig.isLocal)
		{
			bool flag = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
			this.trackedGameObjects.Add(other.gameObject, flag);
			this.OnTriggerStay(other);
		}
	}

	// Token: 0x06003F5A RID: 16218 RVA: 0x00153F04 File Offset: 0x00152104
	public void OnTriggerStay(Collider other)
	{
		bool flag;
		if (!this.trackedGameObjects.TryGetValue(other.gameObject, ref flag))
		{
			return;
		}
		bool flag2 = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
		if (flag != flag2)
		{
			this.PlaySFX();
			this.trackedGameObjects.Remove(other.gameObject);
		}
	}

	// Token: 0x06003F5B RID: 16219 RVA: 0x00153F70 File Offset: 0x00152170
	public void OnTriggerExit(Collider other)
	{
		bool flag;
		if (this.trackedGameObjects.TryGetValue(other.gameObject, ref flag))
		{
			bool flag2 = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
			if (flag != flag2)
			{
				this.PlaySFX();
			}
			this.trackedGameObjects.Remove(other.gameObject);
		}
	}

	// Token: 0x06003F5C RID: 16220 RVA: 0x00153FDC File Offset: 0x001521DC
	public void PlaySFX()
	{
		if (this.barrierAudioSource.IsNull())
		{
			return;
		}
		if (this.PassThroughBarrierSoundClips.IsNullOrEmpty<AudioClip>())
		{
			return;
		}
		this.barrierAudioSource.clip = this.PassThroughBarrierSoundClips[Random.Range(0, this.PassThroughBarrierSoundClips.Count)];
		this.barrierAudioSource.Play();
	}

	// Token: 0x04005093 RID: 20627
	[SerializeField]
	private AudioSource barrierAudioSource;

	// Token: 0x04005094 RID: 20628
	[FormerlySerializedAs("teleportingPlayerSoundClips")]
	[SerializeField]
	private List<AudioClip> PassThroughBarrierSoundClips = new List<AudioClip>();

	// Token: 0x04005095 RID: 20629
	private Dictionary<GameObject, bool> trackedGameObjects = new Dictionary<GameObject, bool>();
}
