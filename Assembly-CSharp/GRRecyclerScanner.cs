using System;
using GorillaTagScripts.GhostReactor;
using TMPro;
using UnityEngine;

// Token: 0x020006EE RID: 1774
public class GRRecyclerScanner : MonoBehaviour
{
	// Token: 0x06002D70 RID: 11632 RVA: 0x000F5BDC File Offset: 0x000F3DDC
	private void Awake()
	{
		this.titleText.text = "";
		this.descriptionText.text = "";
		this.annotationText.text = "";
		this.recycleValueText.text = "";
	}

	// Token: 0x06002D71 RID: 11633 RVA: 0x000F5C2C File Offset: 0x000F3E2C
	public void ScanItem(GameEntityId id)
	{
		if (this.recycler != null && this.recycler.reactor != null && this.recycler.reactor.grManager != null && this.recycler.reactor.grManager.gameEntityManager != null)
		{
			GameEntity gameEntity = this.recycler.reactor.grManager.gameEntityManager.GetGameEntity(id);
			if (gameEntity == null)
			{
				return;
			}
			GRScannable component = gameEntity.GetComponent<GRScannable>();
			if (component == null)
			{
				return;
			}
			this.titleText.text = component.GetTitleText(this.recycler.reactor);
			this.descriptionText.text = component.GetBodyText(this.recycler.reactor);
			this.annotationText.text = component.GetAnnotationText(this.recycler.reactor);
			this.recycleValueText.text = string.Format("Recycle value: {0}", this.recycler.GetRecycleValue(gameEntity.gameObject.GetToolType()));
			this.audioSource.volume = this.recyclerBarcodeAudioVolume;
			this.audioSource.PlayOneShot(this.recyclerBarcodeAudio);
		}
	}

	// Token: 0x06002D72 RID: 11634 RVA: 0x000F5D78 File Offset: 0x000F3F78
	private void OnTriggerEnter(Collider other)
	{
		if (this.recycler.reactor == null)
		{
			return;
		}
		if (!this.recycler.reactor.grManager.IsAuthority())
		{
			return;
		}
		GRScannable componentInParent = other.gameObject.GetComponentInParent<GRScannable>();
		if (componentInParent == null)
		{
			return;
		}
		this.recycler.reactor.grManager.RequestRecycleScanItem(componentInParent.gameEntity.id);
	}

	// Token: 0x04003B18 RID: 15128
	public GRRecycler recycler;

	// Token: 0x04003B19 RID: 15129
	[SerializeField]
	private TextMeshPro titleText;

	// Token: 0x04003B1A RID: 15130
	[SerializeField]
	private TextMeshPro descriptionText;

	// Token: 0x04003B1B RID: 15131
	[SerializeField]
	private TextMeshPro annotationText;

	// Token: 0x04003B1C RID: 15132
	[SerializeField]
	private TextMeshPro recycleValueText;

	// Token: 0x04003B1D RID: 15133
	public AudioSource audioSource;

	// Token: 0x04003B1E RID: 15134
	public AudioClip recyclerBarcodeAudio;

	// Token: 0x04003B1F RID: 15135
	public float recyclerBarcodeAudioVolume = 0.5f;
}
