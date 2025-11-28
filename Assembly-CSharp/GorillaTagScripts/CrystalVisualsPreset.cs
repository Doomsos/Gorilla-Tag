using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DD7 RID: 3543
	[CreateAssetMenu(fileName = "CrystalVisualsPreset", menuName = "ScriptableObjects/CrystalVisualsPreset", order = 0)]
	public class CrystalVisualsPreset : ScriptableObject
	{
		// Token: 0x060057FE RID: 22526 RVA: 0x001C1FB0 File Offset: 0x001C01B0
		public override int GetHashCode()
		{
			return new ValueTuple<CrystalVisualsPreset.VisualState, CrystalVisualsPreset.VisualState>(this.stateA, this.stateB).GetHashCode();
		}

		// Token: 0x060057FF RID: 22527 RVA: 0x00002789 File Offset: 0x00000989
		[Conditional("UNITY_EDITOR")]
		private void Save()
		{
		}

		// Token: 0x0400654F RID: 25935
		public CrystalVisualsPreset.VisualState stateA;

		// Token: 0x04006550 RID: 25936
		public CrystalVisualsPreset.VisualState stateB;

		// Token: 0x02000DD8 RID: 3544
		[Serializable]
		public struct VisualState
		{
			// Token: 0x06005801 RID: 22529 RVA: 0x001C1FDC File Offset: 0x001C01DC
			public override int GetHashCode()
			{
				int num = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.albedo);
				int num2 = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.emission);
				return new ValueTuple<int, int>(num, num2).GetHashCode();
			}

			// Token: 0x06005802 RID: 22530 RVA: 0x001C2014 File Offset: 0x001C0214
			[CompilerGenerated]
			internal static int <GetHashCode>g__GetColorHash|2_0(Color c)
			{
				return new ValueTuple<float, float, float>(c.r, c.g, c.b).GetHashCode();
			}

			// Token: 0x04006551 RID: 25937
			[ColorUsage(false, false)]
			public Color albedo;

			// Token: 0x04006552 RID: 25938
			[ColorUsage(false, false)]
			public Color emission;
		}
	}
}
