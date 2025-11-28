using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011B1 RID: 4529
	public class Codec
	{
		// Token: 0x0600721A RID: 29210 RVA: 0x00257DA4 File Offset: 0x00255FA4
		public static float PackSaturated(float a, float b)
		{
			a = Mathf.Floor(a * 4095f);
			b = Mathf.Floor(b * 4095f);
			return a * 4096f + b;
		}

		// Token: 0x0600721B RID: 29211 RVA: 0x00257DCB File Offset: 0x00255FCB
		public static float PackSaturated(Vector2 v)
		{
			return Codec.PackSaturated(v.x, v.y);
		}

		// Token: 0x0600721C RID: 29212 RVA: 0x00257DDE File Offset: 0x00255FDE
		public static Vector2 UnpackSaturated(float f)
		{
			return new Vector2(Mathf.Floor(f / 4096f), Mathf.Repeat(f, 4096f)) / 4095f;
		}

		// Token: 0x0600721D RID: 29213 RVA: 0x00257E08 File Offset: 0x00256008
		public static Vector2 OctWrap(Vector2 v)
		{
			return (Vector2.one - new Vector2(Mathf.Abs(v.y), Mathf.Abs(v.x))) * new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}

		// Token: 0x0600721E RID: 29214 RVA: 0x00257E5C File Offset: 0x0025605C
		public static float PackNormal(Vector3 n)
		{
			n /= Mathf.Abs(n.x) + Mathf.Abs(n.y) + Mathf.Abs(n.z);
			return Codec.PackSaturated(((n.z >= 0f) ? new Vector2(n.x, n.y) : Codec.OctWrap(new Vector2(n.x, n.y))) * 0.5f + 0.5f * Vector2.one);
		}

		// Token: 0x0600721F RID: 29215 RVA: 0x00257EF0 File Offset: 0x002560F0
		public static Vector3 UnpackNormal(float f)
		{
			Vector2 vector = Codec.UnpackSaturated(f);
			vector = vector * 2f - Vector2.one;
			Vector3 vector2;
			vector2..ctor(vector.x, vector.y, 1f - Mathf.Abs(vector.x) - Mathf.Abs(vector.y));
			float num = Mathf.Clamp01(-vector2.z);
			vector2.x += ((vector2.x >= 0f) ? (-num) : num);
			vector2.y += ((vector2.y >= 0f) ? (-num) : num);
			return vector2.normalized;
		}

		// Token: 0x06007220 RID: 29216 RVA: 0x00257F98 File Offset: 0x00256198
		public static uint PackRgb(Color color)
		{
			return (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x06007221 RID: 29217 RVA: 0x00257FC8 File Offset: 0x002561C8
		public static Color UnpackRgb(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f);
		}

		// Token: 0x06007222 RID: 29218 RVA: 0x00258004 File Offset: 0x00256204
		public static uint PackRgba(Color color)
		{
			return (uint)(color.a * 255f) << 24 | (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x06007223 RID: 29219 RVA: 0x00258050 File Offset: 0x00256250
		public static Color UnpackRgba(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f, ((i & 4278190080U) >> 24) / 255f);
		}

		// Token: 0x06007224 RID: 29220 RVA: 0x002580A6 File Offset: 0x002562A6
		public static uint Pack8888(uint x, uint y, uint z, uint w)
		{
			return (x & 255U) << 24 | (y & 255U) << 16 | (z & 255U) << 8 | (w & 255U);
		}

		// Token: 0x06007225 RID: 29221 RVA: 0x002580CF File Offset: 0x002562CF
		public static void Unpack8888(uint i, out uint x, out uint y, out uint z, out uint w)
		{
			x = (i >> 24 & 255U);
			y = (i >> 16 & 255U);
			z = (i >> 8 & 255U);
			w = (i & 255U);
		}

		// Token: 0x06007226 RID: 29222 RVA: 0x00258100 File Offset: 0x00256300
		private static int IntReinterpret(float f)
		{
			return new Codec.IntFloat
			{
				FloatValue = f
			}.IntValue;
		}

		// Token: 0x06007227 RID: 29223 RVA: 0x00258123 File Offset: 0x00256323
		public static int HashConcat(int hash, int i)
		{
			return (hash ^ i) * Codec.FnvPrime;
		}

		// Token: 0x06007228 RID: 29224 RVA: 0x0025812E File Offset: 0x0025632E
		public static int HashConcat(int hash, long i)
		{
			hash = Codec.HashConcat(hash, (int)(i & (long)((ulong)-1)));
			hash = Codec.HashConcat(hash, (int)(i >> 32));
			return hash;
		}

		// Token: 0x06007229 RID: 29225 RVA: 0x0025814B File Offset: 0x0025634B
		public static int HashConcat(int hash, float f)
		{
			return Codec.HashConcat(hash, Codec.IntReinterpret(f));
		}

		// Token: 0x0600722A RID: 29226 RVA: 0x00258159 File Offset: 0x00256359
		public static int HashConcat(int hash, bool b)
		{
			return Codec.HashConcat(hash, b ? 1 : 0);
		}

		// Token: 0x0600722B RID: 29227 RVA: 0x00258168 File Offset: 0x00256368
		public static int HashConcat(int hash, params int[] ints)
		{
			foreach (int i2 in ints)
			{
				hash = Codec.HashConcat(hash, i2);
			}
			return hash;
		}

		// Token: 0x0600722C RID: 29228 RVA: 0x00258194 File Offset: 0x00256394
		public static int HashConcat(int hash, params float[] floats)
		{
			foreach (float f in floats)
			{
				hash = Codec.HashConcat(hash, f);
			}
			return hash;
		}

		// Token: 0x0600722D RID: 29229 RVA: 0x002581BF File Offset: 0x002563BF
		public static int HashConcat(int hash, Vector2 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y
			});
		}

		// Token: 0x0600722E RID: 29230 RVA: 0x002581DF File Offset: 0x002563DF
		public static int HashConcat(int hash, Vector3 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z
			});
		}

		// Token: 0x0600722F RID: 29231 RVA: 0x00258208 File Offset: 0x00256408
		public static int HashConcat(int hash, Vector4 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z,
				v.w
			});
		}

		// Token: 0x06007230 RID: 29232 RVA: 0x0025823A File Offset: 0x0025643A
		public static int HashConcat(int hash, Quaternion q)
		{
			return Codec.HashConcat(hash, new float[]
			{
				q.x,
				q.y,
				q.z,
				q.w
			});
		}

		// Token: 0x06007231 RID: 29233 RVA: 0x0025826C File Offset: 0x0025646C
		public static int HashConcat(int hash, Color c)
		{
			return Codec.HashConcat(hash, new float[]
			{
				c.r,
				c.g,
				c.b,
				c.a
			});
		}

		// Token: 0x06007232 RID: 29234 RVA: 0x0025829E File Offset: 0x0025649E
		public static int HashConcat(int hash, Transform t)
		{
			return Codec.HashConcat(hash, t.GetHashCode());
		}

		// Token: 0x06007233 RID: 29235 RVA: 0x002582AC File Offset: 0x002564AC
		public static int Hash(int i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06007234 RID: 29236 RVA: 0x002582B9 File Offset: 0x002564B9
		public static int Hash(long i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06007235 RID: 29237 RVA: 0x002582C6 File Offset: 0x002564C6
		public static int Hash(float f)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, f);
		}

		// Token: 0x06007236 RID: 29238 RVA: 0x002582D3 File Offset: 0x002564D3
		public static int Hash(bool b)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, b);
		}

		// Token: 0x06007237 RID: 29239 RVA: 0x002582E0 File Offset: 0x002564E0
		public static int Hash(params int[] ints)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, ints);
		}

		// Token: 0x06007238 RID: 29240 RVA: 0x002582ED File Offset: 0x002564ED
		public static int Hash(params float[] floats)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, floats);
		}

		// Token: 0x06007239 RID: 29241 RVA: 0x002582FA File Offset: 0x002564FA
		public static int Hash(Vector2 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x0600723A RID: 29242 RVA: 0x00258307 File Offset: 0x00256507
		public static int Hash(Vector3 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x0600723B RID: 29243 RVA: 0x00258314 File Offset: 0x00256514
		public static int Hash(Vector4 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x0600723C RID: 29244 RVA: 0x00258321 File Offset: 0x00256521
		public static int Hash(Quaternion q)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, q);
		}

		// Token: 0x0600723D RID: 29245 RVA: 0x0025832E File Offset: 0x0025652E
		public static int Hash(Color c)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, c);
		}

		// Token: 0x0600723E RID: 29246 RVA: 0x0025833C File Offset: 0x0025653C
		private static int HashTransformHierarchyRecurvsive(int hash, Transform t)
		{
			hash = Codec.HashConcat(hash, t);
			hash = Codec.HashConcat(hash, t.childCount);
			for (int i = 0; i < t.childCount; i++)
			{
				hash = Codec.HashTransformHierarchyRecurvsive(hash, t.GetChild(i));
			}
			return hash;
		}

		// Token: 0x0600723F RID: 29247 RVA: 0x00258381 File Offset: 0x00256581
		public static int HashTransformHierarchy(Transform t)
		{
			return Codec.HashTransformHierarchyRecurvsive(Codec.FnvDefaultBasis, t);
		}

		// Token: 0x040082BA RID: 33466
		public static readonly int FnvDefaultBasis = -2128831035;

		// Token: 0x040082BB RID: 33467
		public static readonly int FnvPrime = 16777619;

		// Token: 0x020011B2 RID: 4530
		[StructLayout(2)]
		private struct IntFloat
		{
			// Token: 0x040082BC RID: 33468
			[FieldOffset(0)]
			public int IntValue;

			// Token: 0x040082BD RID: 33469
			[FieldOffset(0)]
			public float FloatValue;
		}
	}
}
