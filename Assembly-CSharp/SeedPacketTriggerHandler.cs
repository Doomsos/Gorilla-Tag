using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004E9 RID: 1257
[RequireComponent(typeof(OnTriggerEventsCosmetic))]
public class SeedPacketTriggerHandler : MonoBehaviour
{
	// Token: 0x06002053 RID: 8275 RVA: 0x000AB68B File Offset: 0x000A988B
	public void OnTriggerEntered()
	{
		if (this.toggleOnceOnly && this.triggerEntered)
		{
			return;
		}
		this.triggerEntered = true;
		UnityEvent<SeedPacketTriggerHandler> unityEvent = this.onTriggerEntered;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		this.ToggleEffects();
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x000AB6C0 File Offset: 0x000A98C0
	public void ToggleEffects()
	{
		if (this.particleToPlay)
		{
			this.particleToPlay.Play();
		}
		if (this.soundBankPlayer)
		{
			this.soundBankPlayer.Play();
		}
		if (this.destroyOnTriggerEnter)
		{
			if (this.destroyDelay > 0f)
			{
				base.Invoke("Destroy", this.destroyDelay);
				return;
			}
			this.Destroy();
		}
	}

	// Token: 0x06002055 RID: 8277 RVA: 0x000AB72A File Offset: 0x000A992A
	private void Destroy()
	{
		this.triggerEntered = false;
		if (ObjectPools.instance.DoesPoolExist(base.gameObject))
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04002AC5 RID: 10949
	[SerializeField]
	private ParticleSystem particleToPlay;

	// Token: 0x04002AC6 RID: 10950
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04002AC7 RID: 10951
	[SerializeField]
	private bool destroyOnTriggerEnter;

	// Token: 0x04002AC8 RID: 10952
	[SerializeField]
	private float destroyDelay = 1f;

	// Token: 0x04002AC9 RID: 10953
	[SerializeField]
	private bool toggleOnceOnly;

	// Token: 0x04002ACA RID: 10954
	[HideInInspector]
	public UnityEvent<SeedPacketTriggerHandler> onTriggerEntered;

	// Token: 0x04002ACB RID: 10955
	private bool triggerEntered;
}
