using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200079B RID: 1947
public class GorillaLevelScreen : MonoBehaviour
{
	// Token: 0x060032F3 RID: 13043 RVA: 0x00113416 File Offset: 0x00111616
	private void Awake()
	{
		if (this.myText != null)
		{
			this.startingText = this.myText.text;
		}
	}

	// Token: 0x060032F4 RID: 13044 RVA: 0x00113438 File Offset: 0x00111638
	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
		Material[] materials = base.GetComponent<MeshRenderer>().materials;
		materials[0] = (setToGoodMaterial ? this.goodMaterial : this.badMaterial);
		base.GetComponent<MeshRenderer>().materials = materials;
	}

	// Token: 0x04004161 RID: 16737
	public string startingText;

	// Token: 0x04004162 RID: 16738
	public Material goodMaterial;

	// Token: 0x04004163 RID: 16739
	public Material badMaterial;

	// Token: 0x04004164 RID: 16740
	public Text myText;
}
