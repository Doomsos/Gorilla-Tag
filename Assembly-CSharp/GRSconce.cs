using System;
using UnityEngine;

// Token: 0x020006F4 RID: 1780
public class GRSconce : MonoBehaviour
{
	// Token: 0x06002D9F RID: 11679 RVA: 0x000F67E8 File Offset: 0x000F49E8
	private void Awake()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange += this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged += this.OnStateChange;
		}
		this.state = GRSconce.State.Off;
		this.StopLight();
	}

	// Token: 0x06002DA0 RID: 11680 RVA: 0x000F684C File Offset: 0x000F4A4C
	private bool IsAuthority()
	{
		return this.gameEntity.IsAuthority();
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x000F685C File Offset: 0x000F4A5C
	private void SetState(GRSconce.State newState)
	{
		this.state = newState;
		GRSconce.State state = this.state;
		if (state != GRSconce.State.Off)
		{
			if (state == GRSconce.State.On)
			{
				this.StartLight();
			}
		}
		else
		{
			this.StopLight();
		}
		if (this.IsAuthority())
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x000F68B0 File Offset: 0x000F4AB0
	private void StartLight()
	{
		this.gameLight.gameObject.SetActive(true);
		this.audioSource.volume = this.lightOnSoundVolume;
		this.audioSource.clip = this.lightOnSound;
		this.audioSource.Play();
		this.meshRenderer.material = this.onMaterial;
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x000F690C File Offset: 0x000F4B0C
	private void StopLight()
	{
		this.gameLight.gameObject.SetActive(false);
		this.meshRenderer.material = this.offMaterial;
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x000F6930 File Offset: 0x000F4B30
	private void OnEnergyChange(GRTool tool, int energy, GameEntityId chargingEntityId)
	{
		if (this.IsAuthority() && this.state == GRSconce.State.Off && tool.IsEnergyFull())
		{
			this.SetState(GRSconce.State.On);
		}
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x000F6954 File Offset: 0x000F4B54
	private void OnStateChange(long prevState, long nextState)
	{
		if (!this.IsAuthority())
		{
			GRSconce.State state = (GRSconce.State)nextState;
			this.SetState(state);
		}
	}

	// Token: 0x04003B4B RID: 15179
	public GameEntity gameEntity;

	// Token: 0x04003B4C RID: 15180
	public GameLight gameLight;

	// Token: 0x04003B4D RID: 15181
	public GRTool tool;

	// Token: 0x04003B4E RID: 15182
	public MeshRenderer meshRenderer;

	// Token: 0x04003B4F RID: 15183
	public Material offMaterial;

	// Token: 0x04003B50 RID: 15184
	public Material onMaterial;

	// Token: 0x04003B51 RID: 15185
	public AudioSource audioSource;

	// Token: 0x04003B52 RID: 15186
	public AudioClip lightOnSound;

	// Token: 0x04003B53 RID: 15187
	public float lightOnSoundVolume;

	// Token: 0x04003B54 RID: 15188
	private GRSconce.State state;

	// Token: 0x020006F5 RID: 1781
	private enum State
	{
		// Token: 0x04003B56 RID: 15190
		Off,
		// Token: 0x04003B57 RID: 15191
		On
	}
}
