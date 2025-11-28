using System;
using MathGeoLib;
using UnityEngine;

// Token: 0x02000CF4 RID: 3316
[Serializable]
public struct BoundsInfo
{
	// Token: 0x1700077B RID: 1915
	// (get) Token: 0x0600507F RID: 20607 RVA: 0x0019E5D3 File Offset: 0x0019C7D3
	public Vector3 sizeComputed
	{
		get
		{
			return Vector3.Scale(this.size, this.scale) * this.inflate;
		}
	}

	// Token: 0x1700077C RID: 1916
	// (get) Token: 0x06005080 RID: 20608 RVA: 0x0019E5F1 File Offset: 0x0019C7F1
	public Vector3 sizeComputedAA
	{
		get
		{
			return Vector3.Scale(this.sizeAA, this.scaleAA) * this.inflateAA;
		}
	}

	// Token: 0x06005081 RID: 20609 RVA: 0x0019E610 File Offset: 0x0019C810
	public static BoundsInfo ComputeBounds(Vector3[] vertices)
	{
		if (vertices.Length == 0)
		{
			return default(BoundsInfo);
		}
		OrientedBoundingBox orientedBoundingBox = OrientedBoundingBox.BruteEnclosing(vertices);
		Vector4 vector = orientedBoundingBox.Axis1;
		Vector4 vector2 = orientedBoundingBox.Axis2;
		Vector4 vector3 = orientedBoundingBox.Axis3;
		Vector4 vector4;
		vector4..ctor(0f, 0f, 0f, 1f);
		BoundsInfo result = default(BoundsInfo);
		result.center = orientedBoundingBox.Center;
		result.size = orientedBoundingBox.Extent * 2f;
		result.rotation = new Matrix4x4(vector, vector2, vector3, vector4).rotation;
		result.scale = Vector3.one;
		result.inflate = 1f;
		Bounds bounds = GeometryUtility.CalculateBounds(vertices, Matrix4x4.identity);
		result.centerAA = bounds.center;
		result.sizeAA = bounds.size;
		result.scaleAA = Vector3.one;
		result.inflateAA = 1f;
		return result;
	}

	// Token: 0x06005082 RID: 20610 RVA: 0x0019E714 File Offset: 0x0019C914
	public static BoxCollider CreateBoxCollider(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int hashCode3 = bounds.rotation.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2, hashCode3);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.center;
		transform.rotation = bounds.rotation;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputed;
		return boxCollider;
	}

	// Token: 0x06005083 RID: 20611 RVA: 0x0019E7C0 File Offset: 0x0019C9C0
	public static BoxCollider CreateBoxColliderAA(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.centerAA;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputedAA;
		return boxCollider;
	}

	// Token: 0x04005FDF RID: 24543
	public Vector3 center;

	// Token: 0x04005FE0 RID: 24544
	public Vector3 size;

	// Token: 0x04005FE1 RID: 24545
	public Quaternion rotation;

	// Token: 0x04005FE2 RID: 24546
	public Vector3 scale;

	// Token: 0x04005FE3 RID: 24547
	public float inflate;

	// Token: 0x04005FE4 RID: 24548
	[Space]
	public Vector3 centerAA;

	// Token: 0x04005FE5 RID: 24549
	public Vector3 sizeAA;

	// Token: 0x04005FE6 RID: 24550
	public Vector3 scaleAA;

	// Token: 0x04005FE7 RID: 24551
	public float inflateAA;
}
