using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000154 RID: 340
public class SpawnRegion<TItem, TRegion> : MonoBehaviour where TItem : Object where TRegion : SpawnRegion<TItem, TRegion>
{
	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x060008F3 RID: 2291 RVA: 0x00030748 File Offset: 0x0002E948
	public static List<TRegion> Regions
	{
		get
		{
			return SpawnRegion<TItem, TRegion>._regions;
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x060008F4 RID: 2292 RVA: 0x0003074F File Offset: 0x0002E94F
	// (set) Token: 0x060008F5 RID: 2293 RVA: 0x00030757 File Offset: 0x0002E957
	public int MaxItems { get; private set; } = 10;

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x060008F6 RID: 2294 RVA: 0x00030760 File Offset: 0x0002E960
	private bool HasSpawnOrigins
	{
		get
		{
			Transform[] array = this.spawnOrigins;
			return array != null && array.Length != 0;
		}
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x060008F7 RID: 2295 RVA: 0x00030772 File Offset: 0x0002E972
	public List<TItem> Items
	{
		get
		{
			return this._items;
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x060008F8 RID: 2296 RVA: 0x0003077A File Offset: 0x0002E97A
	public int ItemCount
	{
		get
		{
			return this._items.Count;
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x060008F9 RID: 2297 RVA: 0x00030787 File Offset: 0x0002E987
	// (set) Token: 0x060008FA RID: 2298 RVA: 0x0003078F File Offset: 0x0002E98F
	public int ID { get; private set; }

	// Token: 0x060008FB RID: 2299 RVA: 0x00030798 File Offset: 0x0002E998
	private void OnEnable()
	{
		Transform[] array = this.spawnOrigins;
		this._useSpawnOrigins = (array != null && array.Length != 0);
		this._testAgainstGeo = (!this._useSpawnOrigins && this.geoTestPoint);
		if (this._testAgainstGeo && this._hitTestBuffer == null)
		{
			this._hitTestBuffer = new RaycastHit[20];
		}
		SpawnRegion<TItem, TRegion>.RegisterRegion((TRegion)((object)this));
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00030800 File Offset: 0x0002EA00
	private void OnDisable()
	{
		SpawnRegion<TItem, TRegion>.UnregisterRegion((TRegion)((object)this));
		foreach (TItem titem in this._items)
		{
			if (titem)
			{
				SpawnRegion<TItem, TRegion>._itemRegionLookup.Remove(titem);
			}
		}
		this._items.Clear();
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x0003087C File Offset: 0x0002EA7C
	private static void RegisterRegion(TRegion region)
	{
		SpawnRegion<TItem, TRegion>._regionLookup[region.ID] = region;
		SpawnRegion<TItem, TRegion>._regions.Add(region);
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x0003089F File Offset: 0x0002EA9F
	private static void UnregisterRegion(TRegion region)
	{
		SpawnRegion<TItem, TRegion>._regionLookup.Remove(region.ID);
		SpawnRegion<TItem, TRegion>._regions.Remove(region);
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x000308C4 File Offset: 0x0002EAC4
	public static void AddItemToRegion(TItem item, int regionId)
	{
		TRegion tregion;
		if (SpawnRegion<TItem, TRegion>._regionLookup.TryGetValue(regionId, ref tregion))
		{
			tregion.AddItem(item);
		}
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x000308EC File Offset: 0x0002EAEC
	public static void RemoveItemFromRegion(TItem item)
	{
		int num;
		TRegion tregion;
		if (SpawnRegion<TItem, TRegion>._itemRegionLookup.TryGetValue(item, ref num) && SpawnRegion<TItem, TRegion>._regionLookup.TryGetValue(num, ref tregion))
		{
			tregion.RemoveItem(item);
		}
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00030923 File Offset: 0x0002EB23
	public void AddItem(TItem item)
	{
		this._items.Add(item);
		SpawnRegion<TItem, TRegion>._itemRegionLookup[item] = this.ID;
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00030942 File Offset: 0x0002EB42
	public void RemoveItem(TItem item)
	{
		this._items.Remove(item);
		SpawnRegion<TItem, TRegion>._itemRegionLookup.Remove(item);
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00030960 File Offset: 0x0002EB60
	[return: TupleElementNames(new string[]
	{
		"isOnGround",
		"position",
		"normal"
	})]
	public ValueTuple<bool, Vector3, Vector3> GetSpawnPointWithNormal(int maxTries = 5)
	{
		for (int i = 0; i < maxTries; i++)
		{
			RaycastHit raycastHit;
			if (this.TryGetSpawnPoint(out raycastHit))
			{
				return new ValueTuple<bool, Vector3, Vector3>(true, raycastHit.point, raycastHit.normal);
			}
		}
		float num = this._scale / 2f;
		Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num, num), num, Random.Range(-num, num)));
		return new ValueTuple<bool, Vector3, Vector3>(false, vector, Vector3.up);
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x000309D4 File Offset: 0x0002EBD4
	private bool TryGetSpawnPoint(out RaycastHit spawnPoint)
	{
		float num = base.transform.lossyScale.y * this._scale;
		if (this._useSpawnOrigins)
		{
			Vector3 vector = this.spawnOrigins[Random.Range(0, this.spawnOrigins.Length)].position;
			if (this.TryGetSpawnPoint(vector, Random.onUnitSphere, Mathf.Max(num, 100f), out spawnPoint))
			{
				return spawnPoint.normal.y > 0f || this.TryGetSpawnPoint(spawnPoint.point, Vector3.down, num, out spawnPoint);
			}
			spawnPoint = default(RaycastHit);
			return false;
		}
		else
		{
			float num2 = this._scale / 2f;
			Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num2, num2), num2, Random.Range(-num2, num2)));
			if (this._testAgainstGeo && this.IsInsideGeo(vector))
			{
				spawnPoint = default(RaycastHit);
				return false;
			}
			return this.TryGetSpawnPoint(vector, Vector3.down, num, out spawnPoint);
		}
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00030AC0 File Offset: 0x0002ECC0
	private bool TryGetSpawnPoint(Vector3 origin, Vector3 direction, float distance, out RaycastHit spawnPoint)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, direction, ref raycastHit, distance, -1, 1))
		{
			Debug.DrawLine(origin, raycastHit.point, Color.green, 5f);
			spawnPoint = raycastHit;
			return true;
		}
		Debug.DrawLine(origin, origin + direction * distance, Color.red, 5f);
		spawnPoint = default(RaycastHit);
		return false;
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00030B24 File Offset: 0x0002ED24
	private bool IsInsideGeo(Vector3 point)
	{
		Vector3 position = this.geoTestPoint.position;
		Vector3 vector = position - point;
		int num;
		int num2;
		for (;;)
		{
			num = Physics.RaycastNonAlloc(point, vector, this._hitTestBuffer, vector.magnitude, -1, 1);
			num2 = Physics.RaycastNonAlloc(position, -vector, this._hitTestBuffer, vector.magnitude, -1, 1);
			if (num < this._hitTestBuffer.Length && num2 < this._hitTestBuffer.Length)
			{
				break;
			}
			this._hitTestBuffer = new RaycastHit[this._hitTestBuffer.Length * 2];
		}
		bool flag = (num + num2) % 2 != 0;
		Debug.DrawLine(point, position, flag ? Color.red : Color.green, 5f);
		return flag;
	}

	// Token: 0x04000B0E RID: 2830
	private static List<TRegion> _regions = new List<TRegion>();

	// Token: 0x04000B0F RID: 2831
	private static Dictionary<int, TRegion> _regionLookup = new Dictionary<int, TRegion>();

	// Token: 0x04000B10 RID: 2832
	private static Dictionary<TItem, int> _itemRegionLookup = new Dictionary<TItem, int>();

	// Token: 0x04000B11 RID: 2833
	[SerializeField]
	private float _scale = 10f;

	// Token: 0x04000B13 RID: 2835
	[SerializeField]
	[Tooltip("If set, spawn points will be created via raycasts from one of these points.")]
	private Transform[] spawnOrigins;

	// Token: 0x04000B14 RID: 2836
	[Tooltip("If set, all spawn points will be tested against this transform to see if they're inside geo.  Ignored if spawn origins are configured.")]
	private Transform geoTestPoint;

	// Token: 0x04000B15 RID: 2837
	private List<TItem> _items = new List<TItem>();

	// Token: 0x04000B17 RID: 2839
	private bool _useSpawnOrigins;

	// Token: 0x04000B18 RID: 2840
	private bool _testAgainstGeo;

	// Token: 0x04000B19 RID: 2841
	private RaycastHit[] _hitTestBuffer;
}
