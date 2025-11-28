using System;
using UnityEngine;

// Token: 0x020004BA RID: 1210
public class ShoppingCart : MonoBehaviour
{
	// Token: 0x06001F34 RID: 7988 RVA: 0x000A5721 File Offset: 0x000A3921
	public void Awake()
	{
		if (ShoppingCart.instance == null)
		{
			ShoppingCart.instance = this;
			return;
		}
		if (ShoppingCart.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x04002982 RID: 10626
	[OnEnterPlay_SetNull]
	public static volatile ShoppingCart instance;
}
