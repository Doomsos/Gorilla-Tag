using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000695 RID: 1685
public class GRCurrencyDepositor : MonoBehaviour
{
	// Token: 0x06002AFD RID: 11005 RVA: 0x000E74DC File Offset: 0x000E56DC
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x000E74E8 File Offset: 0x000E56E8
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			GRCollectible component = other.attachedRigidbody.GetComponent<GRCollectible>();
			if (component != null)
			{
				if ((component.type == ProgressionManager.CoreType.ChaosSeed && !this.collectSentientCores) || (component.type != ProgressionManager.CoreType.ChaosSeed && this.collectSentientCores))
				{
					return;
				}
				if (this.reactor.grManager.IsAuthority())
				{
					this.reactor.grManager.RequestDepositCollectible(component.entity.id);
				}
				this.collectibleDepositedEffect.Play();
				this.audioSource.volume = this.collectibleDepositedClipVolume;
				this.audioSource.PlayOneShot(this.collectibleDepositedClip);
				if (component.entity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					if (GamePlayerLocal.instance.gamePlayer.GetGrabbedGameEntityId(0) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(true, 0.5f, 0.15f);
						return;
					}
					if (GamePlayerLocal.instance.gamePlayer.GetGrabbedGameEntityId(1) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(false, 0.5f, 0.15f);
					}
				}
			}
		}
	}

	// Token: 0x04003782 RID: 14210
	public Transform depositingChargePoint;

	// Token: 0x04003783 RID: 14211
	[SerializeField]
	private ParticleSystem collectibleDepositedEffect;

	// Token: 0x04003784 RID: 14212
	[SerializeField]
	private AudioClip collectibleDepositedClip;

	// Token: 0x04003785 RID: 14213
	[SerializeField]
	private float collectibleDepositedClipVolume;

	// Token: 0x04003786 RID: 14214
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003787 RID: 14215
	[SerializeField]
	private bool collectSentientCores;

	// Token: 0x04003788 RID: 14216
	private const float hapticStrength = 0.5f;

	// Token: 0x04003789 RID: 14217
	private const float hapticDuration = 0.15f;

	// Token: 0x0400378A RID: 14218
	[NonSerialized]
	public GhostReactor reactor;
}
