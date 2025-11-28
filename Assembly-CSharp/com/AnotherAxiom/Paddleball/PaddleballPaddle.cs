using System;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02000F6B RID: 3947
	public class PaddleballPaddle : MonoBehaviour
	{
		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x060062B1 RID: 25265 RVA: 0x001FD80A File Offset: 0x001FBA0A
		public bool Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x0400717D RID: 29053
		[SerializeField]
		private bool right;
	}
}
