using System;
using UnityEngine;

// Token: 0x020009EA RID: 2538
public class MonoBehaviourStatic<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x170005FE RID: 1534
	// (get) Token: 0x060040B6 RID: 16566 RVA: 0x0015A219 File Offset: 0x00158419
	public static T Instance
	{
		get
		{
			return MonoBehaviourStatic<T>.gInstance;
		}
	}

	// Token: 0x060040B7 RID: 16567 RVA: 0x0015A220 File Offset: 0x00158420
	protected void Awake()
	{
		if (MonoBehaviourStatic<T>.gInstance && MonoBehaviourStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
		}
		MonoBehaviourStatic<T>.gInstance = (this as T);
		this.OnAwake();
	}

	// Token: 0x060040B8 RID: 16568 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnAwake()
	{
	}

	// Token: 0x040051F7 RID: 20983
	protected static T gInstance;
}
