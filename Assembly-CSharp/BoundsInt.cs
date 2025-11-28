using System;
using UnityEngine;

// Token: 0x02000CF6 RID: 3318
[Serializable]
public struct BoundsInt
{
	// Token: 0x06005087 RID: 20615 RVA: 0x0019E8BD File Offset: 0x0019CABD
	public BoundsInt(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06005088 RID: 20616 RVA: 0x0019E8D0 File Offset: 0x0019CAD0
	public BoundsInt(Vector3 center, Vector3 size)
	{
		Vector3 vector = size * 0.5f;
		this.min = BoundsInt.FloatToInt(center - vector);
		this.max = BoundsInt.FloatToInt(center + vector);
	}

	// Token: 0x1700077E RID: 1918
	// (get) Token: 0x06005089 RID: 20617 RVA: 0x0019E90D File Offset: 0x0019CB0D
	public Vector3Int center
	{
		get
		{
			return (this.min + this.max) / 2;
		}
	}

	// Token: 0x1700077F RID: 1919
	// (get) Token: 0x0600508A RID: 20618 RVA: 0x0019E926 File Offset: 0x0019CB26
	public Vector3Int size
	{
		get
		{
			return this.max - this.min;
		}
	}

	// Token: 0x17000780 RID: 1920
	// (get) Token: 0x0600508B RID: 20619 RVA: 0x0019E939 File Offset: 0x0019CB39
	public Vector3 centerFloat
	{
		get
		{
			return BoundsInt.IntToFloat(this.center);
		}
	}

	// Token: 0x17000781 RID: 1921
	// (get) Token: 0x0600508C RID: 20620 RVA: 0x0019E946 File Offset: 0x0019CB46
	public Vector3 sizeFloat
	{
		get
		{
			return BoundsInt.IntToFloat(this.size);
		}
	}

	// Token: 0x0600508D RID: 20621 RVA: 0x0019E953 File Offset: 0x0019CB53
	public static Vector3Int FloatToInt(Vector3 v)
	{
		return new Vector3Int(Mathf.RoundToInt(v.x * 1000f), Mathf.RoundToInt(v.y * 1000f), Mathf.RoundToInt(v.z * 1000f));
	}

	// Token: 0x0600508E RID: 20622 RVA: 0x0019E98D File Offset: 0x0019CB8D
	public static Vector3 IntToFloat(Vector3Int v)
	{
		return new Vector3((float)v.x / 1000f, (float)v.y / 1000f, (float)v.z / 1000f);
	}

	// Token: 0x0600508F RID: 20623 RVA: 0x0019E9BE File Offset: 0x0019CBBE
	public static BoundsInt FromBounds(Bounds bounds)
	{
		return new BoundsInt(bounds.center, bounds.size);
	}

	// Token: 0x06005090 RID: 20624 RVA: 0x0019E9D3 File Offset: 0x0019CBD3
	public Bounds ToBounds()
	{
		return new Bounds(this.centerFloat, this.sizeFloat);
	}

	// Token: 0x06005091 RID: 20625 RVA: 0x0019E8BD File Offset: 0x0019CABD
	public void SetMinMax(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06005092 RID: 20626 RVA: 0x0019E9E6 File Offset: 0x0019CBE6
	public void SetMinMax(Vector3 min, Vector3 max)
	{
		this.min = BoundsInt.FloatToInt(min);
		this.max = BoundsInt.FloatToInt(max);
	}

	// Token: 0x06005093 RID: 20627 RVA: 0x0019EA00 File Offset: 0x0019CC00
	public void Encapsulate(BoundsInt other)
	{
		this.min = new Vector3Int(Mathf.Min(this.min.x, other.min.x), Mathf.Min(this.min.y, other.min.y), Mathf.Min(this.min.z, other.min.z));
		this.max = new Vector3Int(Mathf.Max(this.max.x, other.max.x), Mathf.Max(this.max.y, other.max.y), Mathf.Max(this.max.z, other.max.z));
	}

	// Token: 0x06005094 RID: 20628 RVA: 0x0019EACC File Offset: 0x0019CCCC
	public void Expand(float amount)
	{
		int num = Mathf.RoundToInt(amount * 1000f);
		Vector3Int vector3Int;
		vector3Int..ctor(num, num, num);
		this.min -= vector3Int;
		this.max += vector3Int;
	}

	// Token: 0x06005095 RID: 20629 RVA: 0x0019EB14 File Offset: 0x0019CD14
	public bool Intersects(BoundsInt other)
	{
		return this.min.x < other.max.x && this.max.x > other.min.x && this.min.y < other.max.y && this.max.y > other.min.y && this.min.z < other.max.z && this.max.z > other.min.z;
	}

	// Token: 0x06005096 RID: 20630 RVA: 0x0019EBC0 File Offset: 0x0019CDC0
	public bool Contains(BoundsInt other)
	{
		return this.min.x <= other.min.x && this.min.y <= other.min.y && this.min.z <= other.min.z && this.max.x >= other.max.x && this.max.y >= other.max.y && this.max.z >= other.max.z;
	}

	// Token: 0x06005097 RID: 20631 RVA: 0x0019EC6C File Offset: 0x0019CE6C
	public bool Contains(Vector3 point)
	{
		Vector3Int vector3Int = BoundsInt.FloatToInt(point);
		return vector3Int.x >= this.min.x && vector3Int.x <= this.max.x && vector3Int.y >= this.min.y && vector3Int.y <= this.max.y && vector3Int.z >= this.min.z && vector3Int.z <= this.max.z;
	}

	// Token: 0x06005098 RID: 20632 RVA: 0x0019ED00 File Offset: 0x0019CF00
	public BoundsInt GetIntersection(BoundsInt other)
	{
		Vector3Int vector3Int;
		vector3Int..ctor(Mathf.Max(this.min.x, other.min.x), Mathf.Max(this.min.y, other.min.y), Mathf.Max(this.min.z, other.min.z));
		Vector3Int vector3Int2;
		vector3Int2..ctor(Mathf.Min(this.max.x, other.max.x), Mathf.Min(this.max.y, other.max.y), Mathf.Min(this.max.z, other.max.z));
		if (vector3Int.x > vector3Int2.x || vector3Int.y > vector3Int2.y || vector3Int.z > vector3Int2.z)
		{
			return new BoundsInt(Vector3Int.zero, Vector3Int.zero);
		}
		return new BoundsInt(vector3Int, vector3Int2);
	}

	// Token: 0x06005099 RID: 20633 RVA: 0x0019EE0C File Offset: 0x0019D00C
	public long Volume()
	{
		Vector3Int size = this.size;
		return (long)size.x * (long)size.y * (long)size.z;
	}

	// Token: 0x0600509A RID: 20634 RVA: 0x0019EE3A File Offset: 0x0019D03A
	public float VolumeFloat()
	{
		return (float)this.Volume() / 1E+09f;
	}

	// Token: 0x0600509B RID: 20635 RVA: 0x0019EE49 File Offset: 0x0019D049
	public static bool operator ==(BoundsInt a, BoundsInt b)
	{
		return a.min == b.min && a.max == b.max;
	}

	// Token: 0x0600509C RID: 20636 RVA: 0x0019EE71 File Offset: 0x0019D071
	public static bool operator !=(BoundsInt a, BoundsInt b)
	{
		return !(a == b);
	}

	// Token: 0x0600509D RID: 20637 RVA: 0x0019EE80 File Offset: 0x0019D080
	public override bool Equals(object obj)
	{
		if (obj is BoundsInt)
		{
			BoundsInt b = (BoundsInt)obj;
			return this == b;
		}
		return false;
	}

	// Token: 0x0600509E RID: 20638 RVA: 0x0019EEAA File Offset: 0x0019D0AA
	public override int GetHashCode()
	{
		return this.min.GetHashCode() ^ this.max.GetHashCode() << 2;
	}

	// Token: 0x0600509F RID: 20639 RVA: 0x0019EED1 File Offset: 0x0019D0D1
	public override string ToString()
	{
		return string.Format("BoundsInt(min: {0}, max: {1})", this.min, this.max);
	}

	// Token: 0x04005FF0 RID: 24560
	private const int SCALE_FACTOR = 1000;

	// Token: 0x04005FF1 RID: 24561
	public Vector3Int min;

	// Token: 0x04005FF2 RID: 24562
	public Vector3Int max;
}
