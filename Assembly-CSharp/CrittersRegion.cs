using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006E RID: 110
public class CrittersRegion : MonoBehaviour
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060002AA RID: 682 RVA: 0x000109BB File Offset: 0x0000EBBB
	public static List<CrittersRegion> Regions
	{
		get
		{
			return CrittersRegion._regions;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x060002AB RID: 683 RVA: 0x000109C2 File Offset: 0x0000EBC2
	public int CritterCount
	{
		get
		{
			return this._critters.Count;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x060002AC RID: 684 RVA: 0x000109CF File Offset: 0x0000EBCF
	// (set) Token: 0x060002AD RID: 685 RVA: 0x000109D7 File Offset: 0x0000EBD7
	public int ID { get; private set; }

	// Token: 0x060002AE RID: 686 RVA: 0x000109E0 File Offset: 0x0000EBE0
	private void OnEnable()
	{
		CrittersRegion.RegisterRegion(this);
	}

	// Token: 0x060002AF RID: 687 RVA: 0x000109E8 File Offset: 0x0000EBE8
	private void OnDisable()
	{
		CrittersRegion.UnregisterRegion(this);
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x000109F0 File Offset: 0x0000EBF0
	private static void RegisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup[region.ID] = region;
		CrittersRegion._regions.Add(region);
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x00010A0E File Offset: 0x0000EC0E
	private static void UnregisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup.Remove(region.ID);
		CrittersRegion._regions.Remove(region);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00010A30 File Offset: 0x0000EC30
	public static void AddCritterToRegion(CrittersPawn critter, int regionId)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(regionId, ref crittersRegion))
		{
			crittersRegion.AddCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Attempted to add critter to non-existing region {0}.", regionId), null);
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00010A6C File Offset: 0x0000EC6C
	public static void RemoveCritterFromRegion(CrittersPawn critter)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(critter.regionId, ref crittersRegion))
		{
			crittersRegion.RemoveCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Couldn't find region with id {0}", critter.regionId), null);
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00010AB0 File Offset: 0x0000ECB0
	public void AddCritter(CrittersPawn pawn)
	{
		this._critters.Add(pawn);
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00010ABE File Offset: 0x0000ECBE
	public void RemoveCritter(CrittersPawn pawn)
	{
		this._critters.Remove(pawn);
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00010AD0 File Offset: 0x0000ECD0
	public Vector3 GetSpawnPoint()
	{
		float num = this.scale / 2f;
		float num2 = base.transform.lossyScale.y * this.scale;
		Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num, num), num, Random.Range(-num, num)));
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, -base.transform.up, ref raycastHit, num2, -1, 1))
		{
			Debug.DrawLine(vector, raycastHit.point, Color.green, 5f);
			return raycastHit.point;
		}
		Debug.DrawLine(vector, vector - base.transform.up * num2, Color.red, 5f);
		return vector;
	}

	// Token: 0x04000323 RID: 803
	private static List<CrittersRegion> _regions = new List<CrittersRegion>();

	// Token: 0x04000324 RID: 804
	private static Dictionary<int, CrittersRegion> _regionLookup = new Dictionary<int, CrittersRegion>();

	// Token: 0x04000325 RID: 805
	public CrittersBiome Biome = CrittersBiome.Any;

	// Token: 0x04000326 RID: 806
	public int maxCritters = 10;

	// Token: 0x04000327 RID: 807
	public float scale = 10f;

	// Token: 0x04000328 RID: 808
	public List<CrittersPawn> _critters = new List<CrittersPawn>();
}
