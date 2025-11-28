using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AE RID: 686
public class DevInspector : MonoBehaviour
{
	// Token: 0x06001121 RID: 4385 RVA: 0x0005BBF3 File Offset: 0x00059DF3
	private void OnEnable()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x040015AC RID: 5548
	public GameObject pivot;

	// Token: 0x040015AD RID: 5549
	public Text outputInfo;

	// Token: 0x040015AE RID: 5550
	public Component[] componentToInspect;

	// Token: 0x040015AF RID: 5551
	public bool isEnabled;

	// Token: 0x040015B0 RID: 5552
	public bool autoFind = true;

	// Token: 0x040015B1 RID: 5553
	public GameObject canvas;

	// Token: 0x040015B2 RID: 5554
	public int sidewaysOffset;
}
