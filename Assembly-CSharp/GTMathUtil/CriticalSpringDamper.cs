using System;

namespace GTMathUtil
{
	// Token: 0x02000D93 RID: 3475
	internal class CriticalSpringDamper
	{
		// Token: 0x06005549 RID: 21833 RVA: 0x001AB082 File Offset: 0x001A9282
		private static float halflife_to_damping(float halflife, float eps = 1E-05f)
		{
			return 2.7725887f / (halflife + eps);
		}

		// Token: 0x0600554A RID: 21834 RVA: 0x001AADA9 File Offset: 0x001A8FA9
		private static float fast_negexp(float x)
		{
			return 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
		}

		// Token: 0x0600554B RID: 21835 RVA: 0x001AD848 File Offset: 0x001ABA48
		public float Update(float dt)
		{
			float num = CriticalSpringDamper.halflife_to_damping(this.halfLife, 1E-05f) / 2f;
			float num2 = this.x - this.xGoal;
			float num3 = this.curVel + num2 * num;
			float num4 = CriticalSpringDamper.fast_negexp(num * dt);
			this.x = num4 * (num2 + num3 * dt) + this.xGoal;
			this.curVel = num4 * (this.curVel - num3 * num * dt);
			return this.x;
		}

		// Token: 0x04006237 RID: 25143
		public float x;

		// Token: 0x04006238 RID: 25144
		public float xGoal;

		// Token: 0x04006239 RID: 25145
		public float halfLife = 0.1f;

		// Token: 0x0400623A RID: 25146
		private float curVel;
	}
}
