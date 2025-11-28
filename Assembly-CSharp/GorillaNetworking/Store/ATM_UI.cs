using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F34 RID: 3892
	public class ATM_UI : MonoBehaviour
	{
		// Token: 0x06006174 RID: 24948 RVA: 0x001F614A File Offset: 0x001F434A
		private void Start()
		{
			if (ATM_Manager.instance != null && !ATM_Manager.instance.atmUIs.Contains(this))
			{
				ATM_Manager.instance.AddATM(this);
			}
		}

		// Token: 0x06006175 RID: 24949 RVA: 0x001F617C File Offset: 0x001F437C
		public void SetCustomMapScene(Scene scene)
		{
			this.customMapScene = scene;
		}

		// Token: 0x06006176 RID: 24950 RVA: 0x001F6185 File Offset: 0x001F4385
		public bool IsFromCustomMapScene(Scene scene)
		{
			return this.customMapScene == scene;
		}

		// Token: 0x0400703F RID: 28735
		public GameObject creatorCodeObject;

		// Token: 0x04007040 RID: 28736
		public TMP_Text atmText;

		// Token: 0x04007041 RID: 28737
		public TMP_Text creatorCodeTitle;

		// Token: 0x04007042 RID: 28738
		public TMP_Text creatorCodeField;

		// Token: 0x04007043 RID: 28739
		public TMP_Text[] ATM_RightColumnButtonText;

		// Token: 0x04007044 RID: 28740
		public TMP_Text[] ATM_RightColumnArrowText;

		// Token: 0x04007045 RID: 28741
		private Scene customMapScene;
	}
}
