using System;
using Liv.Lck.GorillaTag;

namespace Docking
{
	// Token: 0x02001148 RID: 4424
	public class LivCameraDock : Dock
	{
		// Token: 0x06006FB7 RID: 28599 RVA: 0x002462C3 File Offset: 0x002444C3
		private void Reset()
		{
			this.cameraSettings.fov = 80f;
		}

		// Token: 0x06006FB8 RID: 28600 RVA: 0x002462D8 File Offset: 0x002444D8
		private void OnValidate()
		{
			if (this.cameraSettings.forceFov && (this.cameraSettings.fov < 30f || this.cameraSettings.fov > 110f))
			{
				this.cameraSettings.fov = 80f;
			}
		}

		// Token: 0x04008027 RID: 32807
		public GtCameraDockSettings cameraSettings;
	}
}
