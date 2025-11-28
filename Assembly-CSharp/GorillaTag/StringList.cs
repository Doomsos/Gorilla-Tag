using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FF4 RID: 4084
	[CreateAssetMenu(fileName = "New String List", menuName = "String List")]
	public class StringList : ScriptableObject
	{
		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x0600672C RID: 26412 RVA: 0x00218EB6 File Offset: 0x002170B6
		public string[] Strings
		{
			get
			{
				return this.strings;
			}
		}

		// Token: 0x040075BB RID: 30139
		[SerializeField]
		private string[] strings;
	}
}
