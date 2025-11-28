using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200116C RID: 4460
	[ExecuteInEditMode]
	public class LatexFormula : MonoBehaviour
	{
		// Token: 0x040080C0 RID: 32960
		public static readonly string BaseUrl = "http://tex.s2cms.ru/svg/f(x) ";

		// Token: 0x040080C1 RID: 32961
		private int m_hash = LatexFormula.BaseUrl.GetHashCode();

		// Token: 0x040080C2 RID: 32962
		[SerializeField]
		private string m_formula = "";

		// Token: 0x040080C3 RID: 32963
		private Texture m_texture;
	}
}
