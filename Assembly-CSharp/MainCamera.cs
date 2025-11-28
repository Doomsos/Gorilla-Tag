using System;
using UnityEngine;

// Token: 0x020009E8 RID: 2536
public class MainCamera : MonoBehaviourStatic<MainCamera>
{
	// Token: 0x0600409D RID: 16541 RVA: 0x00159DC5 File Offset: 0x00157FC5
	public static implicit operator Camera(MainCamera mc)
	{
		return mc.camera;
	}

	// Token: 0x040051ED RID: 20973
	public Camera camera;
}
