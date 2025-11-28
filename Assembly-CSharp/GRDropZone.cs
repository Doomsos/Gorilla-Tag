using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200069E RID: 1694
public class GRDropZone : MonoBehaviour
{
	// Token: 0x06002B44 RID: 11076 RVA: 0x000E8167 File Offset: 0x000E6367
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x000E8188 File Offset: 0x000E6388
	private void OnTriggerEnter(Collider other)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null && component.manager.ghostReactorManager != null)
		{
			GhostReactorManager.Get(component).EntityEnteredDropZone(component);
		}
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x000E81D1 File Offset: 0x000E63D1
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x000E81DC File Offset: 0x000E63DC
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x000E8255 File Offset: 0x000E6455
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x040037B9 RID: 14265
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x040037BA RID: 14266
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x040037BB RID: 14267
	public float effectDuration = 1f;

	// Token: 0x040037BC RID: 14268
	private bool playingEffect;

	// Token: 0x040037BD RID: 14269
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x040037BE RID: 14270
	private Vector3 repelDirectionWorld = Vector3.up;
}
