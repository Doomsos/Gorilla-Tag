using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FD2 RID: 4050
	public class DestroyOnAwake : MonoBehaviour
	{
		// Token: 0x060066AE RID: 26286 RVA: 0x00217088 File Offset: 0x00215288
		protected void Awake()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x060066AF RID: 26287 RVA: 0x002170B8 File Offset: 0x002152B8
		protected void OnEnable()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x060066B0 RID: 26288 RVA: 0x002170E8 File Offset: 0x002152E8
		protected void Update()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}
	}
}
