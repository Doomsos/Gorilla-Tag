using System;
using UnityEngine;

// Token: 0x02000ABB RID: 2747
public class KIDUI_TooYoungToPlay : MonoBehaviour
{
	// Token: 0x060044DD RID: 17629 RVA: 0x00166492 File Offset: 0x00164692
	public void ShowTooYoungToPlayScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x060044DE RID: 17630 RVA: 0x0016895E File Offset: 0x00166B5E
	public void OnQuitPressed()
	{
		Application.Quit();
	}
}
