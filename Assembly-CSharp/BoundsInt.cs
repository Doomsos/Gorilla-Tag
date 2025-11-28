using System;
using UnityEngine;

// Token: 0x02000CF6 RID: 3318
[Serializable]
public struct BoundsInt
{
	// Token: 0x06005087 RID: 20615 RVA: 0x0019E89D File Offset: 0x0019CA9D
	public BoundsInt(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06005088 RID: 20616 RVA: 0x0019E8B0 File Offset: 0x0019CAB0
	public BoundsInt(Vector3 center, Vector3 size)
	{
		Vector3 vector = size * 0.5f;
		this.min = BoundsInt.FloatToInt(center - vector);
		this.max = BoundsInt.FloatToInt(center + vector);
	}

	// Token: 0x1700077E RID: 1918
	// (get) Token: 0x06005089 RID: 20617 RVA: 0x0019E8ED File Offset: 0x0019CAED
	public Vector3Int center
	{
		get
		{
			return (this.min + this.max) / 2;
		}
	}

	// Token: 0x1700077F RID: 1919
	// (get) Token: 0x0600508A RID: 20618 RVA: 0x0019E906 File Offset: 0x0019CB06
	public Vector3Int size
	{
		get
		{
			return this.max - this.min;
		}
	}

	// Token: 0x17000780 RID: 1920
	// (get) Token: 0x0600508B RID: 20619 RVA: 0x0019E919 File Offset: 0x0019CB19
	public Vector3 centerFloat
	{
		get
		{
			return BoundsInt.IntToFloat(this.center);
		}
	}

	// Token: 0x17000781 RID: 1921
	// (get) Token: 0x0600508C RID: 20620 RVA: 0x0019E926 File Offset: 0x0019CB26
	public Vector3 sizeFloat
	{
		get
		{
			return BoundsInt.IntToFloat(this.size);
		}
	}

	// Token: 0x0600508D RID: 20621 RVA: 0x0019E933 File Offset: 0x0019CB33
	public static Vector3Int FloatToInt(Vector3 v)
	{
		return new Vector3Int(Mathf.RoundToInt(v.x * 1000f), Mathf.RoundToInt(v.y * 1000f), Mathf.RoundToInt(v.z * 1000f));
	}

	// Token: 0x0600508E RID: 20622 RVA: 0x0019E96D File Offset: 0x0019CB6D
	public static Vector3 IntToFloat(Vector3Int v)
	{
		return new Vector3((float)v.x / 1000f, (float)v.y / 1000f, (float)v.z / 1000f);
	}

	// Token: 0x0600508F RID: 20623 RVA: 0x0019E99E File Offset: 0x0019CB9E
	public static BoundsInt FromBounds(Bounds bounds)
	{
		return new BoundsInt(bounds.center, bounds.size);
	}

	// Token: 0x06005090 RID: 20624 RVA: 0x0019E9B3 File Offset: 0x0019CBB3
	public Bounds ToBounds()
	{
		return new Bounds(this.centerFloat, this.sizeFloat);
	}

	// Token: 0x06005091 RID: 20625 RVA: 0x0019E89D File Offset: 0x0019CA9D
	public void SetMinMax(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06005092 RID: 20626 RVA: 0x0019E9C6 File Offset: 0x0019CBC6
	public void SetMinMax(Vector3 min, Vector3 max)
	{
		this.min = BoundsInt.FloatToInt(min);
		this.max = BoundsInt.FloatToInt(max);
	}

	// Token: 0x06005093 RID: 20627 RVA: 0x0019E9E0 File Offset: 0x0019CBE0
	public void Encapsulate(BoundsInt other)
	{
		this.min = new Vector3Int(Mathf.Min(this.min.x, other.min.x), Mathf.Min(this.min.y, other.min.y), Mathf.Min(this.min.z, other.min.z));
		this.max = new Vector3Int(Mathf.Max(this.max.x, other.max.x), Mathf.Max(this.max.y, other.max.y), Mathf.Max(this.max.z, other.max.z));
	}

	// Token: 0x06005094 RID: 20628 RVA: 0x0019EAAC File Offset: 0x0019CCAC
	public void Expand(float amount)
	{
		int num = Mathf.RoundToInt(amount * 1000f);
		Vector3Int vector3Int;
		vector3Int..ctor(num, num, num);
		this.min -= vector3Int;
		this.max += vector3Int;
	}

	// Token: 0x06005095 RID: 20629 RVA: 0x0019EAF4 File Offset: 0x0019CCF4
	public bool Intersects(BoundsInt other)
	{
		return this.min.x < other.max.x && this.max.x > other.min.x && this.min.y < other.max.y && this.max.y > other.min.y && this.min.z < other.max.z && this.max.z > other.min.z;
	}

	// Token: 0x06005096 RID: 20630 RVA: 0x0019EBA0 File Offset: 0x0019CDA0
	public bool Contains(BoundsInt other)
	{
		return this.min.x <= other.min.x && this.min.y <= other.min.y && this.min.z <= other.min.z && this.max.x >= other.max.x && this.max.y >= other.max.y && this.max.z >= other.max.z;
	}

	// Token: 0x06005097 RID: 20631 RVA: 0x0019EC4C File Offset: 0x0019CE4C
	public bool Contains(Vector3 point)
	{
		Vector3Int vector3Int = BoundsInt.FloatToInt(point);
		return vector3Int.x >= this.min.x && vector3Int.x <= this.max.x && vector3Int.y >= this.min.y && vector3Int.y <= this.max.y && vector3Int.z >= this.min.z && vector3Int.z <= this.max.z;
	}

	// Token: 0x06005098 RID: 20632 RVA: 0x0019ECE0 File Offset: 0x0019CEE0
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

	// Token: 0x06005099 RID: 20633 RVA: 0x0019EDEC File Offset: 0x0019CFEC
	public long Volume()
	{
		Vector3Int size = this.size;
		return (long)size.x * (long)size.y * (long)size.z;
	}

	// Token: 0x0600509A RID: 20634 RVA: 0x0019EE1A File Offset: 0x0019D01A
	public float VolumeFloat()
	{
		return (float)this.Volume() / 1E+09f;
	}

	// Token: 0x0600509B RID: 20635 RVA: 0x0019EE29 File Offset: 0x0019D029
	public static bool operator ==(BoundsInt a, BoundsInt b)
	{
		return a.min == b.min && a.max == b.max;
	}

	// Token: 0x0600509C RID: 20636 RVA: 0x0019EE51 File Offset: 0x0019D051
	public static bool operator !=(BoundsInt a, BoundsInt b)
	{
		return !(a == b);
	}

	// Token: 0x0600509D RID: 20637 RVA: 0x0019EE60 File Offset: 0x0019D060
	public override bool Equals(object obj)
	{
		if (obj is BoundsInt)
		{
			BoundsInt b = (BoundsInt)obj;
			return this == b;
		}
		return false;
	}

	// Token: 0x0600509E RID: 20638 RVA: 0x0019EE8A File Offset: 0x0019D08A
	public override int GetHashCode()
	{
		return this.min.GetHashCode() ^ this.max.GetHashCode() << 2;
	}

	// Token: 0x0600509F RID: 20639 RVA: 0x0019EEB1 File Offset: 0x0019D0B1
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
