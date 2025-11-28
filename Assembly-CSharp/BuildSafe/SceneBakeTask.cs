using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	// Token: 0x02000EB4 RID: 3764
	public abstract class SceneBakeTask : MonoBehaviour
	{
		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x06005DE7 RID: 24039 RVA: 0x001E19F3 File Offset: 0x001DFBF3
		// (set) Token: 0x06005DE8 RID: 24040 RVA: 0x001E19FB File Offset: 0x001DFBFB
		public SceneBakeMode bakeMode
		{
			get
			{
				return this.m_bakeMode;
			}
			set
			{
				this.m_bakeMode = value;
			}
		}

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x06005DE9 RID: 24041 RVA: 0x001E1A04 File Offset: 0x001DFC04
		// (set) Token: 0x06005DEA RID: 24042 RVA: 0x001E1A0C File Offset: 0x001DFC0C
		public virtual int callbackOrder
		{
			get
			{
				return this.m_callbackOrder;
			}
			set
			{
				this.m_callbackOrder = value;
			}
		}

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x06005DEB RID: 24043 RVA: 0x001E1A15 File Offset: 0x001DFC15
		// (set) Token: 0x06005DEC RID: 24044 RVA: 0x001E1A1D File Offset: 0x001DFC1D
		public bool runIfInactive
		{
			get
			{
				return this.m_runIfInactive;
			}
			set
			{
				this.m_runIfInactive = value;
			}
		}

		// Token: 0x06005DED RID: 24045
		[Conditional("UNITY_EDITOR")]
		public abstract void OnSceneBake(Scene scene, SceneBakeMode mode);

		// Token: 0x06005DEE RID: 24046 RVA: 0x00002789 File Offset: 0x00000989
		[Conditional("UNITY_EDITOR")]
		private void ForceRun()
		{
		}

		// Token: 0x04006BD5 RID: 27605
		[SerializeField]
		private SceneBakeMode m_bakeMode;

		// Token: 0x04006BD6 RID: 27606
		[SerializeField]
		private int m_callbackOrder;

		// Token: 0x04006BD7 RID: 27607
		[Space]
		[SerializeField]
		private bool m_runIfInactive = true;
	}
}
