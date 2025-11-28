using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200117C RID: 4476
	public class BoingBase : MonoBehaviour
	{
		// Token: 0x17000A88 RID: 2696
		// (get) Token: 0x060070F6 RID: 28918 RVA: 0x0024FBE2 File Offset: 0x0024DDE2
		public Version CurrentVersion
		{
			get
			{
				return this.m_currentVersion;
			}
		}

		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x060070F7 RID: 28919 RVA: 0x0024FBEA File Offset: 0x0024DDEA
		public Version PreviousVersion
		{
			get
			{
				return this.m_previousVersion;
			}
		}

		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x060070F8 RID: 28920 RVA: 0x0024FBF2 File Offset: 0x0024DDF2
		public Version InitialVersion
		{
			get
			{
				return this.m_initialVersion;
			}
		}

		// Token: 0x060070F9 RID: 28921 RVA: 0x0024FBFA File Offset: 0x0024DDFA
		protected virtual void OnUpgrade(Version oldVersion, Version newVersion)
		{
			this.m_previousVersion = this.m_currentVersion;
			if (this.m_currentVersion.Revision < 33)
			{
				this.m_initialVersion = Version.Invalid;
				this.m_previousVersion = Version.Invalid;
			}
			this.m_currentVersion = newVersion;
		}

		// Token: 0x04008123 RID: 33059
		[SerializeField]
		private Version m_currentVersion;

		// Token: 0x04008124 RID: 33060
		[SerializeField]
		private Version m_previousVersion;

		// Token: 0x04008125 RID: 33061
		[SerializeField]
		private Version m_initialVersion = BoingKit.Version;
	}
}
