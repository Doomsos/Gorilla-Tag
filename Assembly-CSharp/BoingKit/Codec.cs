using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011B1 RID: 4529
	public class Codec
	{
		// Token: 0x0600721A RID: 29210 RVA: 0x00257D84 File Offset: 0x00255F84
		public static float PackSaturated(float a, float b)
		{
			a = Mathf.Floor(a * 4095f);
			b = Mathf.Floor(b * 4095f);
			return a * 4096f + b;
		}

		// Token: 0x0600721B RID: 29211 RVA: 0x00257DAB File Offset: 0x00255FAB
		public static float PackSaturated(Vector2 v)
		{
			return Codec.PackSaturated(v.x, v.y);
		}

		// Token: 0x0600721C RID: 29212 RVA: 0x00257DBE File Offset: 0x00255FBE
		public static Vector2 UnpackSaturated(float f)
		{
			return new Vector2(Mathf.Floor(f / 4096f), Mathf.Repeat(f, 4096f)) / 4095f;
		}

		// Token: 0x0600721D RID: 29213 RVA: 0x00257DE8 File Offset: 0x00255FE8
		public static Vector2 OctWrap(Vector2 v)
		{
			return (Vector2.one - new Vector2(Mathf.Abs(v.y), Mathf.Abs(v.x))) * new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}

		// Token: 0x0600721E RID: 29214 RVA: 0x00257E3C File Offset: 0x0025603C
		public static float PackNormal(Vector3 n)
		{
			n /= Mathf.Abs(n.x) + Mathf.Abs(n.y) + Mathf.Abs(n.z);
			return Codec.PackSaturated(((n.z >= 0f) ? new Vector2(n.x, n.y) : Codec.OctWrap(new Vector2(n.x, n.y))) * 0.5f + 0.5f * Vector2.one);
		}

		// Token: 0x0600721F RID: 29215 RVA: 0x00257ED0 File Offset: 0x002560D0
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

		// Token: 0x06007220 RID: 29216 RVA: 0x00257F78 File Offset: 0x00256178
		public static uint PackRgb(Color color)
		{
			return (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x06007221 RID: 29217 RVA: 0x00257FA8 File Offset: 0x002561A8
		public static Color UnpackRgb(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f);
		}

		// Token: 0x06007222 RID: 29218 RVA: 0x00257FE4 File Offset: 0x002561E4
		public static uint PackRgba(Color color)
		{
			return (uint)(color.a * 255f) << 24 | (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x06007223 RID: 29219 RVA: 0x00258030 File Offset: 0x00256230
		public static Color UnpackRgba(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f, ((i & 4278190080U) >> 24) / 255f);
		}

		// Token: 0x06007224 RID: 29220 RVA: 0x00258086 File Offset: 0x00256286
		public static uint Pack8888(uint x, uint y, uint z, uint w)
		{
			return (x & 255U) << 24 | (y & 255U) << 16 | (z & 255U) << 8 | (w & 255U);
		}

		// Token: 0x06007225 RID: 29221 RVA: 0x002580AF File Offset: 0x002562AF
		public static void Unpack8888(uint i, out uint x, out uint y, out uint z, out uint w)
		{
			x = (i >> 24 & 255U);
			y = (i >> 16 & 255U);
			z = (i >> 8 & 255U);
			w = (i & 255U);
		}

		// Token: 0x06007226 RID: 29222 RVA: 0x002580E0 File Offset: 0x002562E0
		private static int IntReinterpret(float f)
		{
			return new Codec.IntFloat
			{
				FloatValue = f
			}.IntValue;
		}

		// Token: 0x06007227 RID: 29223 RVA: 0x00258103 File Offset: 0x00256303
		public static int HashConcat(int hash, int i)
		{
			return (hash ^ i) * Codec.FnvPrime;
		}

		// Token: 0x06007228 RID: 29224 RVA: 0x0025810E File Offset: 0x0025630E
		public static int HashConcat(int hash, long i)
		{
			hash = Codec.HashConcat(hash, (int)(i & (long)((ulong)-1)));
			hash = Codec.HashConcat(hash, (int)(i >> 32));
			return hash;
		}

		// Token: 0x06007229 RID: 29225 RVA: 0x0025812B File Offset: 0x0025632B
		public static int HashConcat(int hash, float f)
		{
			return Codec.HashConcat(hash, Codec.IntReinterpret(f));
		}

		// Token: 0x0600722A RID: 29226 RVA: 0x00258139 File Offset: 0x00256339
		public static int HashConcat(int hash, bool b)
		{
			return Codec.HashConcat(hash, b ? 1 : 0);
		}

		// Token: 0x0600722B RID: 29227 RVA: 0x00258148 File Offset: 0x00256348
		public static int HashConcat(int hash, params int[] ints)
		{
			foreach (int i2 in ints)
			{
				hash = Codec.HashConcat(hash, i2);
			}
			return hash;
		}

		// Token: 0x0600722C RID: 29228 RVA: 0x00258174 File Offset: 0x00256374
		public static int HashConcat(int hash, params float[] floats)
		{
			foreach (float f in floats)
			{
				hash = Codec.HashConcat(hash, f);
			}
			return hash;
		}

		// Token: 0x0600722D RID: 29229 RVA: 0x0025819F File Offset: 0x0025639F
		public static int HashConcat(int hash, Vector2 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y
			});
		}

		// Token: 0x0600722E RID: 29230 RVA: 0x002581BF File Offset: 0x002563BF
		public static int HashConcat(int hash, Vector3 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z
			});
		}

		// Token: 0x0600722F RID: 29231 RVA: 0x002581E8 File Offset: 0x002563E8
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

		// Token: 0x06007230 RID: 29232 RVA: 0x0025821A File Offset: 0x0025641A
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

		// Token: 0x06007231 RID: 29233 RVA: 0x0025824C File Offset: 0x0025644C
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

		// Token: 0x06007232 RID: 29234 RVA: 0x0025827E File Offset: 0x0025647E
		public static int HashConcat(int hash, Transform t)
		{
			return Codec.HashConcat(hash, t.GetHashCode());
		}

		// Token: 0x06007233 RID: 29235 RVA: 0x0025828C File Offset: 0x0025648C
		public static int Hash(int i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06007234 RID: 29236 RVA: 0x00258299 File Offset: 0x00256499
		public static int Hash(long i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06007235 RID: 29237 RVA: 0x002582A6 File Offset: 0x002564A6
		public static int Hash(float f)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, f);
		}

		// Token: 0x06007236 RID: 29238 RVA: 0x002582B3 File Offset: 0x002564B3
		public static int Hash(bool b)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, b);
		}

		// Token: 0x06007237 RID: 29239 RVA: 0x002582C0 File Offset: 0x002564C0
		public static int Hash(params int[] ints)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, ints);
		}

		// Token: 0x06007238 RID: 29240 RVA: 0x002582CD File Offset: 0x002564CD
		public static int Hash(params float[] floats)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, floats);
		}

		// Token: 0x06007239 RID: 29241 RVA: 0x002582DA File Offset: 0x002564DA
		public static int Hash(Vector2 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x0600723A RID: 29242 RVA: 0x002582E7 File Offset: 0x002564E7
		public static int Hash(Vector3 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x0600723B RID: 29243 RVA: 0x002582F4 File Offset: 0x002564F4
		public static int Hash(Vector4 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x0600723C RID: 29244 RVA: 0x00258301 File Offset: 0x00256501
		public static int Hash(Quaternion q)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, q);
		}

		// Token: 0x0600723D RID: 29245 RVA: 0x0025830E File Offset: 0x0025650E
		public static int Hash(Color c)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, c);
		}

		// Token: 0x0600723E RID: 29246 RVA: 0x0025831C File Offset: 0x0025651C
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

		// Token: 0x0600723F RID: 29247 RVA: 0x00258361 File Offset: 0x00256561
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
