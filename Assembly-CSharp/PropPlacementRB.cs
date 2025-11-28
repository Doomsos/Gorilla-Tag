using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

// Token: 0x02000215 RID: 533
public class PropPlacementRB : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06000EAC RID: 3756 RVA: 0x0004E14F File Offset: 0x0004C34F
	protected void OnDestroy()
	{
		if (this._placingProp != null)
		{
			Object.Destroy(this._placingProp);
		}
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x0004E16C File Offset: 0x0004C36C
	public void PlaceProp_NoPool(PropHuntPropZone parentZone, GTAssetRef<GameObject> propRef, Vector3 pos, Quaternion rot, CosmeticSO debugCosmeticSO)
	{
		if (this._isInstantiatingAsync)
		{
			Debug.LogError("ERROR!!!  PropPlacementRB: Tried to place (spawn) prop while one was already being placed.");
			return;
		}
		this._parentZone = parentZone;
		MeshCollider[] colliders = this._colliders;
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].gameObject.SetActive(false);
		}
		base.transform.position = pos;
		base.transform.rotation = rot;
		base.gameObject.SetActive(false);
		this._isInstantiatingAsync = true;
		propRef.InstantiateAsync(null, false).Completed += new Action<AsyncOperationHandle<GameObject>>(this.OnPropLoaded_NoPool);
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x0004E200 File Offset: 0x0004C400
	public void OnPropLoaded_NoPool(AsyncOperationHandle<GameObject> handle)
	{
		this._isInstantiatingAsync = false;
		this._placingProp = handle.Result;
		this._placingProp.transform.position = base.transform.position;
		this._placingProp.transform.rotation = base.transform.rotation;
		this.m_rb.linearVelocity = Vector3.zero;
		this.m_rb.angularVelocity = Vector3.zero;
		CosmeticSO debugCosmeticSO = null;
		if (!PropPlacementRB.TryPrepPropTemplate(this, this._placingProp, debugCosmeticSO))
		{
			this.DestroyProp_NoPool();
			return;
		}
		this._placingProp.SetActive(false);
		base.gameObject.SetActive(true);
		GTDelayedExec.Add(this, 2f, 0);
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x0004E2B4 File Offset: 0x0004C4B4
	public static bool TryPrepPropTemplate(PropPlacementRB rb, GameObject rendererGobj, CosmeticSO _debugCosmeticSO)
	{
		rb._isInstantiatingAsync = false;
		rb._placingProp = rendererGobj;
		rb._placingProp.transform.position = rb.transform.position;
		rb._placingProp.transform.rotation = rb.transform.rotation;
		rb.m_rb.linearVelocity = Vector3.zero;
		rb.m_rb.angularVelocity = Vector3.zero;
		bool flag = false;
		MeshFilter[] componentsInChildren = rendererGobj.GetComponentsInChildren<MeshFilter>(true);
		List<MeshCollider> list;
		bool result;
		using (ListPool<MeshCollider>.Get(ref list))
		{
			list.Capacity = math.max(list.Capacity, 8);
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				Mesh sharedMesh = meshFilter.sharedMesh;
				if (!(sharedMesh == null) && sharedMesh.isReadable)
				{
					flag = true;
					MeshCollider meshCollider = new GameObject(meshFilter.name + "__PropHuntDecoy_Collider")
					{
						transform = 
						{
							parent = rb.transform
						},
						layer = 30
					}.AddComponent<MeshCollider>();
					meshCollider.convex = true;
					meshCollider.transform.position = meshFilter.transform.position;
					meshCollider.transform.rotation = meshFilter.transform.rotation;
					meshCollider.sharedMesh = meshFilter.sharedMesh;
					list.Add(meshCollider);
				}
			}
			rb._colliders = list.ToArray();
			if (!flag)
			{
				result = false;
			}
			else
			{
				Transform[] componentsInChildren2 = rendererGobj.GetComponentsInChildren<Transform>(true);
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].gameObject.isStatic = true;
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0004E480 File Offset: 0x0004C680
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		this.OnPropFell();
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0004E488 File Offset: 0x0004C688
	private void OnPropFell()
	{
		if (this._placingProp == null)
		{
			return;
		}
		this._placingProp.transform.position = base.transform.position;
		this._placingProp.transform.rotation = base.transform.rotation;
		this._placingProp.SetActive(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0004E4EC File Offset: 0x0004C6EC
	public void DestroyProp_NoPool()
	{
		if (this._placingProp != null)
		{
			Object.Destroy(this._placingProp);
			this._placingProp = null;
		}
	}

	// Token: 0x040011CF RID: 4559
	[FormerlySerializedAs("rb")]
	[SerializeField]
	private Rigidbody m_rb;

	// Token: 0x040011D0 RID: 4560
	[FormerlySerializedAs("simDurationBeforeFreeze")]
	[SerializeField]
	private float m_simDurationBeforeFreeze;

	// Token: 0x040011D1 RID: 4561
	private PropHuntPropZone _parentZone;

	// Token: 0x040011D2 RID: 4562
	[SerializeField]
	internal GameObject _placingProp;

	// Token: 0x040011D3 RID: 4563
	[SerializeField]
	private MeshCollider[] _colliders;

	// Token: 0x040011D4 RID: 4564
	private bool _isInstantiatingAsync;
}
