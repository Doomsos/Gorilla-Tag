using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020009F3 RID: 2547
[Serializable]
public struct SRand
{
	// Token: 0x060040EC RID: 16620 RVA: 0x0015AB4B File Offset: 0x00158D4B
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x060040ED RID: 16621 RVA: 0x0015AB4B File Offset: 0x00158D4B
	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x060040EE RID: 16622 RVA: 0x0015AB60 File Offset: 0x00158D60
	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060040EF RID: 16623 RVA: 0x0015AB7A File Offset: 0x00158D7A
	public SRand(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060040F0 RID: 16624 RVA: 0x0015AB94 File Offset: 0x00158D94
	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060040F1 RID: 16625 RVA: 0x0015ABC6 File Offset: 0x00158DC6
	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060040F2 RID: 16626 RVA: 0x0015ABF7 File Offset: 0x00158DF7
	public double NextDouble()
	{
		return this.NextState() % 268435457U * 3.725290298461914E-09;
	}

	// Token: 0x060040F3 RID: 16627 RVA: 0x0015AC11 File Offset: 0x00158E11
	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	// Token: 0x060040F4 RID: 16628 RVA: 0x0015AC34 File Offset: 0x00158E34
	public double NextDouble(double min, double max)
	{
		double num = max - min;
		if (num <= 0.0)
		{
			return min;
		}
		double num2 = this.NextDouble() * num;
		return min + num2;
	}

	// Token: 0x060040F5 RID: 16629 RVA: 0x0015AC5F File Offset: 0x00158E5F
	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	// Token: 0x060040F6 RID: 16630 RVA: 0x0015AC68 File Offset: 0x00158E68
	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	// Token: 0x060040F7 RID: 16631 RVA: 0x0015AC73 File Offset: 0x00158E73
	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	// Token: 0x060040F8 RID: 16632 RVA: 0x0015AC80 File Offset: 0x00158E80
	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	// Token: 0x060040F9 RID: 16633 RVA: 0x0015AC8D File Offset: 0x00158E8D
	public uint NextUInt()
	{
		return this.NextState();
	}

	// Token: 0x060040FA RID: 16634 RVA: 0x0015AC8D File Offset: 0x00158E8D
	public int NextInt()
	{
		return (int)this.NextState();
	}

	// Token: 0x060040FB RID: 16635 RVA: 0x0015AC95 File Offset: 0x00158E95
	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	// Token: 0x060040FC RID: 16636 RVA: 0x0015ACA8 File Offset: 0x00158EA8
	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	// Token: 0x060040FD RID: 16637 RVA: 0x0015ACC8 File Offset: 0x00158EC8
	public int NextIntWithExclusion(int min, int max, int exclude)
	{
		int num = max - min - 1;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 1 + this.NextInt(num);
		if (num2 > exclude)
		{
			return num2;
		}
		return num2 - 1;
	}

	// Token: 0x060040FE RID: 16638 RVA: 0x0015ACF8 File Offset: 0x00158EF8
	public int NextIntWithExclusion2(int min, int max, int exclude, int exclude2)
	{
		if (exclude == exclude2)
		{
			return this.NextIntWithExclusion(min, max, exclude);
		}
		int num = max - min - 2;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 2 + this.NextInt(num);
		int num3;
		int num4;
		if (exclude >= exclude2)
		{
			num3 = exclude2 + 1;
			num4 = exclude;
		}
		else
		{
			num3 = exclude + 1;
			num4 = exclude2;
		}
		if (num2 <= num3)
		{
			return num2 - 2;
		}
		if (num2 <= num4)
		{
			return num2 - 1;
		}
		return num2;
	}

	// Token: 0x060040FF RID: 16639 RVA: 0x0015AD5E File Offset: 0x00158F5E
	public byte NextByte()
	{
		return (byte)(this.NextState() & 255U);
	}

	// Token: 0x06004100 RID: 16640 RVA: 0x0015AD70 File Offset: 0x00158F70
	public Color32 NextColor32()
	{
		byte b = this.NextByte();
		byte b2 = this.NextByte();
		byte b3 = this.NextByte();
		return new Color32(b, b2, b3, byte.MaxValue);
	}

	// Token: 0x06004101 RID: 16641 RVA: 0x0015ADA0 File Offset: 0x00158FA0
	[MethodImpl(256)]
	public Vector3 NextPointInsideSphere(float radius)
	{
		float num = this.NextFloat() * 2f - 1f;
		float num2 = this.NextFloat() * 2f - 1f;
		float num3 = this.NextFloat() * 2f - 1f;
		float num4 = MathF.Pow(this.NextFloat(), 0.33333334f);
		float num5 = 1f / MathF.Sqrt(num * num + num2 * num2 + num3 * num3);
		return new Vector3(num * num5 * num4 * radius, num2 * num5 * num4 * radius, num3 * num5 * num4 * radius);
	}

	// Token: 0x06004102 RID: 16642 RVA: 0x0015AE2C File Offset: 0x0015902C
	[MethodImpl(256)]
	public Vector3 NextPointOnSphere(float radius)
	{
		float num = this.NextFloat() * 2f - 1f;
		float num2 = this.NextFloat() * 2f - 1f;
		float num3 = this.NextFloat() * 2f - 1f;
		float num4 = 1f / MathF.Sqrt(num * num + num2 * num2 + num3 * num3);
		return new Vector3(num * num4 * radius, num2 * num4 * radius, num3 * num4 * radius);
	}

	// Token: 0x06004103 RID: 16643 RVA: 0x0015AEA0 File Offset: 0x001590A0
	[MethodImpl(256)]
	public Vector3 NextPointInsideBox(Vector3 extents)
	{
		float num = this.NextFloat() - 0.5f;
		float num2 = this.NextFloat() - 0.5f;
		float num3 = this.NextFloat() - 0.5f;
		return new Vector3(num * extents.x, num2 * extents.y, num3 * extents.z);
	}

	// Token: 0x06004104 RID: 16644 RVA: 0x0015AEF0 File Offset: 0x001590F0
	public Color NextColor()
	{
		float num = this.NextFloat();
		float num2 = this.NextFloat();
		float num3 = this.NextFloat();
		return new Color(num, num2, num3, 1f);
	}

	// Token: 0x06004105 RID: 16645 RVA: 0x0015AF20 File Offset: 0x00159120
	public void Shuffle<T>(T[] array)
	{
		int i = array.Length;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int num2 = i;
			int num3 = num;
			T t = array[num];
			T t2 = array[i];
			array[num2] = t;
			array[num3] = t2;
		}
	}

	// Token: 0x06004106 RID: 16646 RVA: 0x0015AF70 File Offset: 0x00159170
	public void Shuffle<T>(List<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int num2 = i;
			int num3 = num;
			T t = list[num];
			T t2 = list[i];
			list[num2] = t;
			list[num3] = t2;
		}
	}

	// Token: 0x06004107 RID: 16647 RVA: 0x0015AFC8 File Offset: 0x001591C8
	public void Reset()
	{
		this._state = this._seed;
	}

	// Token: 0x06004108 RID: 16648 RVA: 0x0015AB4B File Offset: 0x00158D4B
	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06004109 RID: 16649 RVA: 0x0015AB4B File Offset: 0x00158D4B
	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x0600410A RID: 16650 RVA: 0x0015AB60 File Offset: 0x00158D60
	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x0600410B RID: 16651 RVA: 0x0015AB7A File Offset: 0x00158D7A
	public void Reset(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x0600410C RID: 16652 RVA: 0x0015AB94 File Offset: 0x00158D94
	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x0600410D RID: 16653 RVA: 0x0015ABC6 File Offset: 0x00158DC6
	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x0600410E RID: 16654 RVA: 0x0015AFD8 File Offset: 0x001591D8
	[MethodImpl(256)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	// Token: 0x0600410F RID: 16655 RVA: 0x0015B000 File Offset: 0x00159200
	[MethodImpl(256)]
	private uint Mix(uint x)
	{
		x = (x >> 17 ^ x) * 3982152891U;
		x = (x >> 11 ^ x) * 2890668881U;
		x = (x >> 15 ^ x) * 830770091U;
		x = (x >> 14 ^ x);
		return x;
	}

	// Token: 0x06004110 RID: 16656 RVA: 0x0015B035 File Offset: 0x00159235
	public override int GetHashCode()
	{
		return StaticHash.Compute((int)this._seed, (int)this._state);
	}

	// Token: 0x06004111 RID: 16657 RVA: 0x0015B048 File Offset: 0x00159248
	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", new object[]
		{
			"SRand",
			"_seed",
			this._seed,
			"_state",
			this._state
		});
	}

	// Token: 0x06004112 RID: 16658 RVA: 0x0015B099 File Offset: 0x00159299
	public static SRand New()
	{
		return new SRand(DateTime.UtcNow);
	}

	// Token: 0x06004113 RID: 16659 RVA: 0x0015B0A5 File Offset: 0x001592A5
	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06004114 RID: 16660 RVA: 0x0015B0AD File Offset: 0x001592AD
	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06004115 RID: 16661 RVA: 0x0015B0B5 File Offset: 0x001592B5
	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06004116 RID: 16662 RVA: 0x0015B0BD File Offset: 0x001592BD
	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06004117 RID: 16663 RVA: 0x0015B0C5 File Offset: 0x001592C5
	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06004118 RID: 16664 RVA: 0x0015B0CD File Offset: 0x001592CD
	public static explicit operator SRand(DateTime seed)
	{
		return new SRand(seed);
	}

	// Token: 0x04005218 RID: 21016
	[SerializeField]
	private uint _seed;

	// Token: 0x04005219 RID: 21017
	[SerializeField]
	private uint _state;

	// Token: 0x0400521A RID: 21018
	private const double MAX_AS_DOUBLE = 268435456.0;

	// Token: 0x0400521B RID: 21019
	private const uint MAX_PLUS_ONE = 268435457U;

	// Token: 0x0400521C RID: 21020
	private const double STEP_SIZE = 3.725290298461914E-09;

	// Token: 0x0400521D RID: 21021
	private const float ONE_THIRD = 0.33333334f;
}
