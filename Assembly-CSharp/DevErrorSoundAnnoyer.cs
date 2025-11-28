using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A7 RID: 679
public class DevErrorSoundAnnoyer : MonoBehaviour
{
	// Token: 0x040015A0 RID: 5536
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x040015A1 RID: 5537
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040015A2 RID: 5538
	[SerializeField]
	private Text errorUIText;

	// Token: 0x040015A3 RID: 5539
	[SerializeField]
	private Font errorFont;

	// Token: 0x040015A4 RID: 5540
	public string displayedText;
}
