using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011B6 RID: 4534
	public struct Aabb
	{
		// Token: 0x17000AA9 RID: 2729
		// (get) Token: 0x06007243 RID: 29251 RVA: 0x00258397 File Offset: 0x00256597
		// (set) Token: 0x06007244 RID: 29252 RVA: 0x002583A4 File Offset: 0x002565A4
		public float MinX
		{
			get
			{
				return this.Min.x;
			}
			set
			{
				this.Min.x = value;
			}
		}

		// Token: 0x17000AAA RID: 2730
		// (get) Token: 0x06007245 RID: 29253 RVA: 0x002583B2 File Offset: 0x002565B2
		// (set) Token: 0x06007246 RID: 29254 RVA: 0x002583BF File Offset: 0x002565BF
		public float MinY
		{
			get
			{
				return this.Min.y;
			}
			set
			{
				this.Min.y = value;
			}
		}

		// Token: 0x17000AAB RID: 2731
		// (get) Token: 0x06007247 RID: 29255 RVA: 0x002583CD File Offset: 0x002565CD
		// (set) Token: 0x06007248 RID: 29256 RVA: 0x002583DA File Offset: 0x002565DA
		public float MinZ
		{
			get
			{
				return this.Min.z;
			}
			set
			{
				this.Min.z = value;
			}
		}

		// Token: 0x17000AAC RID: 2732
		// (get) Token: 0x06007249 RID: 29257 RVA: 0x002583E8 File Offset: 0x002565E8
		// (set) Token: 0x0600724A RID: 29258 RVA: 0x002583F5 File Offset: 0x002565F5
		public float MaxX
		{
			get
			{
				return this.Max.x;
			}
			set
			{
				this.Max.x = value;
			}
		}

		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x0600724B RID: 29259 RVA: 0x00258403 File Offset: 0x00256603
		// (set) Token: 0x0600724C RID: 29260 RVA: 0x00258410 File Offset: 0x00256610
		public float MaxY
		{
			get
			{
				return this.Max.y;
			}
			set
			{
				this.Max.y = value;
			}
		}

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x0600724D RID: 29261 RVA: 0x0025841E File Offset: 0x0025661E
		// (set) Token: 0x0600724E RID: 29262 RVA: 0x0025842B File Offset: 0x0025662B
		public float MaxZ
		{
			get
			{
				return this.Max.z;
			}
			set
			{
				this.Max.z = value;
			}
		}

		// Token: 0x17000AAF RID: 2735
		// (get) Token: 0x0600724F RID: 29263 RVA: 0x00258439 File Offset: 0x00256639
		public Vector3 Center
		{
			get
			{
				return 0.5f * (this.Min + this.Max);
			}
		}

		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x06007250 RID: 29264 RVA: 0x00258458 File Offset: 0x00256658
		public Vector3 Size
		{
			get
			{
				Vector3 vector = this.Max - this.Min;
				vector.x = Mathf.Max(0f, vector.x);
				vector.y = Mathf.Max(0f, vector.y);
				vector.z = Mathf.Max(0f, vector.z);
				return vector;
			}
		}

		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x06007251 RID: 29265 RVA: 0x002584BD File Offset: 0x002566BD
		public static Aabb Empty
		{
			get
			{
				return new Aabb(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));
			}
		}

		// Token: 0x06007252 RID: 29266 RVA: 0x002584EC File Offset: 0x002566EC
		public static Aabb FromPoint(Vector3 p)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(p);
			return empty;
		}

		// Token: 0x06007253 RID: 29267 RVA: 0x00258508 File Offset: 0x00256708
		public static Aabb FromPoints(Vector3 a, Vector3 b)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(a);
			empty.Include(b);
			return empty;
		}

		// Token: 0x06007254 RID: 29268 RVA: 0x0025852C File Offset: 0x0025672C
		public Aabb(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x06007255 RID: 29269 RVA: 0x0025853C File Offset: 0x0025673C
		public void Include(Vector3 p)
		{
			this.MinX = Mathf.Min(this.MinX, p.x);
			this.MinY = Mathf.Min(this.MinY, p.y);
			this.MinZ = Mathf.Min(this.MinZ, p.z);
			this.MaxX = Mathf.Max(this.MaxX, p.x);
			this.MaxY = Mathf.Max(this.MaxY, p.y);
			this.MaxZ = Mathf.Max(this.MaxZ, p.z);
		}

		// Token: 0x06007256 RID: 29270 RVA: 0x002585D4 File Offset: 0x002567D4
		public bool Contains(Vector3 p)
		{
			return this.MinX <= p.x && this.MinY <= p.y && this.MinZ <= p.z && this.MaxX >= p.x && this.MaxY >= p.y && this.MaxZ >= p.z;
		}

		// Token: 0x06007257 RID: 29271 RVA: 0x0025863A File Offset: 0x0025683A
		public bool ContainsX(Vector3 p)
		{
			return this.MinX <= p.x && this.MaxX >= p.x;
		}

		// Token: 0x06007258 RID: 29272 RVA: 0x0025865D File Offset: 0x0025685D
		public bool ContainsY(Vector3 p)
		{
			return this.MinY <= p.y && this.MaxY >= p.y;
		}

		// Token: 0x06007259 RID: 29273 RVA: 0x00258680 File Offset: 0x00256880
		public bool ContainsZ(Vector3 p)
		{
			return this.MinZ <= p.z && this.MaxZ >= p.z;
		}

		// Token: 0x0600725A RID: 29274 RVA: 0x002586A4 File Offset: 0x002568A4
		public bool Intersects(Aabb rhs)
		{
			return this.MinX <= rhs.MaxX && this.MinY <= rhs.MaxY && this.MinZ <= rhs.MaxZ && this.MaxX >= rhs.MinX && this.MaxY >= rhs.MinY && this.MaxZ >= rhs.MinZ;
		}

		// Token: 0x0600725B RID: 29275 RVA: 0x00258710 File Offset: 0x00256910
		public bool Intersects(ref BoingEffector.Params effector)
		{
			if (!effector.Bits.IsBitSet(0))
			{
				return this.Intersects(Aabb.FromPoint(effector.CurrPosition).Expand(effector.Radius));
			}
			return this.Intersects(Aabb.FromPoints(effector.PrevPosition, effector.CurrPosition).Expand(effector.Radius));
		}

		// Token: 0x0600725C RID: 29276 RVA: 0x00258770 File Offset: 0x00256970
		public Aabb Expand(float amount)
		{
			this.MinX -= amount;
			this.MinY -= amount;
			this.MinZ -= amount;
			this.MaxX += amount;
			this.MaxY += amount;
			this.MaxZ += amount;
			return this;
		}

		// Token: 0x040082C7 RID: 33479
		public Vector3 Min;

		// Token: 0x040082C8 RID: 33480
		public Vector3 Max;
	}
}
