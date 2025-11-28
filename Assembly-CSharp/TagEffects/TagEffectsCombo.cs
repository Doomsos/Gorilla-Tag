using System;

namespace TagEffects
{
	// Token: 0x02000F76 RID: 3958
	[Serializable]
	public class TagEffectsCombo : IEquatable<TagEffectsCombo>
	{
		// Token: 0x060062F9 RID: 25337 RVA: 0x001FE924 File Offset: 0x001FCB24
		bool IEquatable<TagEffectsCombo>.Equals(TagEffectsCombo other)
		{
			return (other.inputA == this.inputA && other.inputB == this.inputB) || (other.inputA == this.inputB && other.inputB == this.inputA);
		}

		// Token: 0x060062FA RID: 25338 RVA: 0x001FE97F File Offset: 0x001FCB7F
		public override bool Equals(object obj)
		{
			return this.Equals((TagEffectsCombo)obj);
		}

		// Token: 0x060062FB RID: 25339 RVA: 0x001FE98D File Offset: 0x001FCB8D
		public override int GetHashCode()
		{
			return this.inputA.GetHashCode() * this.inputB.GetHashCode();
		}

		// Token: 0x040071B3 RID: 29107
		public TagEffectPack inputA;

		// Token: 0x040071B4 RID: 29108
		public TagEffectPack inputB;
	}
}
