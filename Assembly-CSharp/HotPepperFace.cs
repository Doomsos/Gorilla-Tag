using System;
using UnityEngine;

// Token: 0x020004DE RID: 1246
public class HotPepperFace : MonoBehaviour
{
	// Token: 0x06002006 RID: 8198 RVA: 0x000AA0BB File Offset: 0x000A82BB
	public void PlayFX(float delay)
	{
		if (delay < 0f)
		{
			this.PlayFX();
			return;
		}
		base.Invoke("PlayFX", delay);
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x000AA0D8 File Offset: 0x000A82D8
	public void PlayFX()
	{
		this._faceMesh.SetActive(true);
		this._thermalSourceVolume.SetActive(true);
		this._fireFX.Play();
		this._flameSpeaker.GTPlay();
		this._breathSpeaker.GTPlay();
		base.Invoke("StopFX", this._effectLength);
	}

	// Token: 0x06002008 RID: 8200 RVA: 0x000AA12F File Offset: 0x000A832F
	public void StopFX()
	{
		this._faceMesh.SetActive(false);
		this._thermalSourceVolume.SetActive(false);
		this._fireFX.Stop();
		this._flameSpeaker.GTStop();
		this._breathSpeaker.GTStop();
	}

	// Token: 0x04002A5C RID: 10844
	[SerializeField]
	private GameObject _faceMesh;

	// Token: 0x04002A5D RID: 10845
	[SerializeField]
	private ParticleSystem _fireFX;

	// Token: 0x04002A5E RID: 10846
	[SerializeField]
	private AudioSource _flameSpeaker;

	// Token: 0x04002A5F RID: 10847
	[SerializeField]
	private AudioSource _breathSpeaker;

	// Token: 0x04002A60 RID: 10848
	[SerializeField]
	private float _effectLength = 1.5f;

	// Token: 0x04002A61 RID: 10849
	[SerializeField]
	private GameObject _thermalSourceVolume;
}
