using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class PropHuntPropZone : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06000EA1 RID: 3745 RVA: 0x0004DD9C File Offset: 0x0004BF9C
	private void Awake()
	{
		this.hasBoxCollider = base.TryGetComponent<BoxCollider>(ref this.boxCollider);
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x0004DDBB File Offset: 0x0004BFBB
	private void OnEnable()
	{
		GorillaPropHuntGameManager.RegisterPropZone(this);
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x0004DDC3 File Offset: 0x0004BFC3
	private void OnDisable()
	{
		this.DestroyDecoys();
		GorillaPropHuntGameManager.UnregisterPropZone(this);
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0004DDD4 File Offset: 0x0004BFD4
	public void DestroyDecoys()
	{
		foreach (PropPlacementRB propPlacementRB in this.propPlacementRBs)
		{
			if (propPlacementRB != null)
			{
				PropHuntPools.ReturnDecoyProp(propPlacementRB);
			}
		}
		this.propPlacementRBs.Clear();
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0004DE3C File Offset: 0x0004C03C
	public void OnRoundStart()
	{
		if (!PropHuntPools.IsReady)
		{
			Debug.LogError("ERROR!!!  PropHuntPropZone: (this should never happen) props not ready to be spawned so aborting. you should only be calling this if `PropHuntPools.IsReady` is true or from the callback `PropHuntPools.OnReady`.");
		}
		this.CreateDecoys(GorillaPropHuntGameManager.instance.GetSeed());
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0004DE60 File Offset: 0x0004C060
	public void CreateDecoys(int seed)
	{
		this.DestroyDecoys();
		SRand srand = new SRand(seed + this.seedOffset);
		for (int i = 0; i < this.numProps; i++)
		{
			PropPlacementRB propPlacementRB;
			if (!PropHuntPools.TryGetDecoyProp(GorillaPropHuntGameManager.instance.GetCosmeticId(srand.NextUInt()), out propPlacementRB))
			{
				return;
			}
			Vector3 position;
			if (this.hasBoxCollider)
			{
				Vector3 vector;
				vector..ctor(srand.NextFloat(-this.boxCollider.size.x, this.boxCollider.size.x) / 2f, srand.NextFloat(-this.boxCollider.size.y, this.boxCollider.size.y) / 2f, srand.NextFloat(-this.boxCollider.size.z, this.boxCollider.size.z) / 2f);
				position = base.transform.TransformPoint(vector);
			}
			else
			{
				position = base.transform.position + srand.NextPointInsideSphere(this.radius);
			}
			propPlacementRB.gameObject.SetActive(false);
			propPlacementRB.transform.SetParent(null, false);
			propPlacementRB.transform.position = position;
			propPlacementRB.transform.rotation = Quaternion.Euler(srand.NextFloat(360f), srand.NextFloat(360f), srand.NextFloat(360f));
			propPlacementRB._placingProp.SetActive(false);
			propPlacementRB._placingProp.transform.SetParent(null, false);
			this.propPlacementRBs.Add(propPlacementRB);
		}
		for (int j = 0; j < this.propPlacementRBs.Count; j++)
		{
			this.propPlacementRBs[j].gameObject.SetActive(true);
		}
		GTDelayedExec.Add(this, this.m_simDurationBeforeFreeze, 0);
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0004E044 File Offset: 0x0004C244
	public void OnDelayedAction(int contextId)
	{
		for (int i = 0; i < this.propPlacementRBs.Count; i++)
		{
			PropPlacementRB propPlacementRB = this.propPlacementRBs[i];
			propPlacementRB.gameObject.SetActive(false);
			Transform transform = propPlacementRB.transform;
			GameObject placingProp = propPlacementRB._placingProp;
			placingProp.transform.SetPositionAndRotation(transform.position, transform.rotation);
			placingProp.SetActive(true);
		}
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0004E0A8 File Offset: 0x0004C2A8
	private PropPlacementRB _GetOrCreatePropPlacementObj_NoPool()
	{
		PropPlacementRB propPlacementRB;
		if (this.nextUnusedPropPlacement < this.propPlacementRBs.Count)
		{
			propPlacementRB = this.propPlacementRBs[this.nextUnusedPropPlacement];
		}
		else
		{
			propPlacementRB = Object.Instantiate<PropPlacementRB>(this.propPlacementPrefab, base.transform);
			this.propPlacementRBs.Add(propPlacementRB);
		}
		this.nextUnusedPropPlacement++;
		return propPlacementRB;
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x0004E109 File Offset: 0x0004C309
	private void SpawnProp_NoPool(GTAssetRef<GameObject> item, Vector3 pos, Quaternion rot, CosmeticSO debugCosmeticSO)
	{
		this._GetOrCreatePropPlacementObj_NoPool().PlaceProp_NoPool(this, item, pos, rot, debugCosmeticSO);
	}

	// Token: 0x040011BD RID: 4541
	private const string preLog = "PropHuntPropZone: ";

	// Token: 0x040011BE RID: 4542
	private const string preLogEd = "(editor only log) PropHuntPropZone: ";

	// Token: 0x040011BF RID: 4543
	private const string preLogBeta = "(beta only log) PropHuntPropZone: ";

	// Token: 0x040011C0 RID: 4544
	private const string preErr = "ERROR!!!  PropHuntPropZone: ";

	// Token: 0x040011C1 RID: 4545
	private const string preErrEd = "ERROR!!!  (editor only log) PropHuntPropZone: ";

	// Token: 0x040011C2 RID: 4546
	private const string preErrBeta = "ERROR!!!  (beta only log) PropHuntPropZone: ";

	// Token: 0x040011C3 RID: 4547
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x040011C4 RID: 4548
	[SerializeField]
	private PropPlacementRB propPlacementPrefab;

	// Token: 0x040011C5 RID: 4549
	[SerializeField]
	private int seedOffset;

	// Token: 0x040011C6 RID: 4550
	[SerializeField]
	private float radius = 1f;

	// Token: 0x040011C7 RID: 4551
	[SerializeField]
	private int numProps = 10;

	// Token: 0x040011C8 RID: 4552
	[SerializeField]
	private float m_simDurationBeforeFreeze = 2f;

	// Token: 0x040011C9 RID: 4553
	private BoxCollider boxCollider;

	// Token: 0x040011CA RID: 4554
	private bool hasBoxCollider;

	// Token: 0x040011CB RID: 4555
	private int nextUnusedPropPlacement;

	// Token: 0x040011CC RID: 4556
	private readonly List<PropPlacementRB> propPlacementRBs = new List<PropPlacementRB>(64);
}
