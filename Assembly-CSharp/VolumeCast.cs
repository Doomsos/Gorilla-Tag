using System;
using System.Collections.Generic;
using Drawing;
using GorillaTag;
using UnityEngine;

// Token: 0x020009FC RID: 2556
public class VolumeCast : MonoBehaviourGizmos
{
	// Token: 0x06004174 RID: 16756 RVA: 0x0015BC90 File Offset: 0x00159E90
	public bool CheckOverlaps()
	{
		Transform transform = base.transform;
		Vector3 lossyScale = transform.lossyScale;
		Quaternion rotation = transform.rotation;
		int num = (int)this.physicsMask;
		QueryTriggerInteraction queryTriggerInteraction = this.includeTriggers ? 2 : 1;
		Vector3 vector;
		Vector3 vector2;
		float num2;
		VolumeCast.GetEndsAndRadius(transform, this.center, this.height, this.radius, out vector, out vector2, out num2);
		VolumeCast.VolumeShape volumeShape = this.shape;
		Vector3 vector3;
		Vector3 vector4;
		if (volumeShape != VolumeCast.VolumeShape.Box)
		{
			if (volumeShape != VolumeCast.VolumeShape.Cylinder)
			{
				return false;
			}
			vector3 = (vector + vector2) * 0.5f;
			vector4..ctor(num2, Vector3.Distance(vector, vector2) * 0.5f, num2);
		}
		else
		{
			vector3 = transform.TransformPoint(this.center);
			vector4 = Vector3.Scale(lossyScale, this.size * 0.5f).Abs();
		}
		Array.Clear(this._boxOverlaps, 0, 8);
		this._boxHits = Physics.OverlapBoxNonAlloc(vector3, vector4, this._boxOverlaps, rotation, num, queryTriggerInteraction);
		if (this.shape != VolumeCast.VolumeShape.Cylinder)
		{
			return this._colliding = (this._boxHits > 0);
		}
		this._hits = 0;
		Array.Clear(this._capOverlaps, 0, 8);
		Array.Clear(this._overlaps, 0, 8);
		this._capHits = Physics.OverlapCapsuleNonAlloc(vector, vector2, num2, this._capOverlaps, num, queryTriggerInteraction);
		this._set.Clear();
		int num3 = Math.Max(this._capHits, this._boxHits);
		Collider[] array = (this._capHits < this._boxHits) ? this._capOverlaps : this._boxOverlaps;
		Collider[] array2 = (this._capHits < this._boxHits) ? this._boxOverlaps : this._capOverlaps;
		for (int i = 0; i < num3; i++)
		{
			Collider collider = array[i];
			if (collider && !this._set.Add(collider))
			{
				Collider[] overlaps = this._overlaps;
				int hits = this._hits;
				this._hits = hits + 1;
				overlaps[hits] = collider;
			}
			Collider collider2 = array2[i];
			if (collider2 && !this._set.Add(collider2))
			{
				Collider[] overlaps2 = this._overlaps;
				int hits = this._hits;
				this._hits = hits + 1;
				overlaps2[hits] = collider2;
			}
		}
		return this._colliding = (this._hits > 0);
	}

	// Token: 0x06004175 RID: 16757 RVA: 0x0015BED4 File Offset: 0x0015A0D4
	private static void GetEndsAndRadius(Transform t, Vector3 center, float height, float radius, out Vector3 a, out Vector3 b, out float r)
	{
		float num = height * 0.5f;
		Vector3 lossyScale = t.lossyScale;
		a = t.TransformPoint(center + Vector3.down * num);
		b = t.TransformPoint(center + Vector3.up * num);
		r = Math.Max(Math.Abs(lossyScale.x), Math.Abs(lossyScale.z)) * radius;
	}

	// Token: 0x0400523E RID: 21054
	public VolumeCast.VolumeShape shape;

	// Token: 0x0400523F RID: 21055
	[Space]
	public Vector3 center;

	// Token: 0x04005240 RID: 21056
	public Vector3 size = Vector3.one;

	// Token: 0x04005241 RID: 21057
	public float height = 1f;

	// Token: 0x04005242 RID: 21058
	public float radius = 1f;

	// Token: 0x04005243 RID: 21059
	private const int MAX_HITS = 8;

	// Token: 0x04005244 RID: 21060
	[Space]
	public UnityLayerMask physicsMask = UnityLayerMask.Everything;

	// Token: 0x04005245 RID: 21061
	public bool includeTriggers;

	// Token: 0x04005246 RID: 21062
	[Space]
	[SerializeField]
	private bool _simulateInEditMode;

	// Token: 0x04005247 RID: 21063
	[DebugReadout]
	[NonSerialized]
	private int _capHits;

	// Token: 0x04005248 RID: 21064
	[DebugReadout]
	[NonSerialized]
	private Collider[] _capOverlaps = new Collider[8];

	// Token: 0x04005249 RID: 21065
	[DebugReadout]
	[NonSerialized]
	private int _boxHits;

	// Token: 0x0400524A RID: 21066
	[DebugReadout]
	[NonSerialized]
	private Collider[] _boxOverlaps = new Collider[8];

	// Token: 0x0400524B RID: 21067
	[DebugReadout]
	[NonSerialized]
	private int _hits;

	// Token: 0x0400524C RID: 21068
	[DebugReadout]
	[NonSerialized]
	private Collider[] _overlaps = new Collider[8];

	// Token: 0x0400524D RID: 21069
	[DebugReadout]
	[NonSerialized]
	private bool _colliding;

	// Token: 0x0400524E RID: 21070
	[NonSerialized]
	private HashSet<Collider> _set = new HashSet<Collider>(8);

	// Token: 0x020009FD RID: 2557
	public enum VolumeShape
	{
		// Token: 0x04005250 RID: 21072
		Box,
		// Token: 0x04005251 RID: 21073
		Cylinder
	}
}
