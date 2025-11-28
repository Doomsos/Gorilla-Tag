using System;
using System.Collections.Generic;
using Cysharp.Text;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x0200046E RID: 1134
public class HeadModel : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06001CAB RID: 7339 RVA: 0x00097C38 File Offset: 0x00095E38
	protected void Awake()
	{
		this.RefreshRenderer();
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x00097C40 File Offset: 0x00095E40
	protected void RefreshRenderer()
	{
		this._mannequinRenderer = base.GetComponentInChildren<Renderer>(true);
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x00097C4F File Offset: 0x00095E4F
	public void SetCosmeticActive(string playFabId, bool forRightSide = false)
	{
		this._ClearCurrent();
		this._AddPreviewCosmetic(playFabId, forRightSide);
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x00097C60 File Offset: 0x00095E60
	public void SetCosmeticActiveArray(string[] playFabIds, bool[] forRightSideArray)
	{
		this._ClearCurrent();
		for (int i = 0; i < playFabIds.Length; i++)
		{
			this._AddPreviewCosmetic(playFabIds[i], forRightSideArray[i]);
		}
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x00097C90 File Offset: 0x00095E90
	private void _AddPreviewCosmetic(string playFabId, bool forRightSide)
	{
		CosmeticInfoV2 cosmeticInfoV;
		if (!CosmeticsController.instance.TryGetCosmeticInfoV2(playFabId, out cosmeticInfoV))
		{
			if (!(playFabId == "null") && !(playFabId == "NOTHING") && !(playFabId == "Slingshot"))
			{
				Debug.LogError(ZString.Concat<string, string, string>("HeadModel._AddPreviewCosmetic: Cosmetic id \"", playFabId, "\" not found in `CosmeticsController`."), this);
			}
			return;
		}
		if (cosmeticInfoV.hideWardrobeMannequin)
		{
			if (this._mannequinRenderer.IsNull())
			{
				this.RefreshRenderer();
			}
			if (this._mannequinRenderer.IsNotNull())
			{
				this._mannequinRenderer.enabled = false;
			}
		}
		foreach (CosmeticPart cosmeticPart in cosmeticInfoV.wardrobeParts)
		{
			if (!cosmeticPart.prefabAssetRef.RuntimeKeyIsValid())
			{
				GTDev.LogError<string>("Cosmetic " + cosmeticInfoV.displayName + " has missing object reference in wardrobe parts, skipping load", null);
			}
			else
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = playFabId,
							prefabAssetRef = cosmeticPart.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(base.transform, false),
							xform = null
						};
						cosmeticPartLoadInfo.loadOp.Completed += new Action<AsyncOperationHandle<GameObject>>(this._HandleLoadOpOnCompleted);
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x00097E64 File Offset: 0x00096064
	private void _HandleLoadOpOnCompleted(AsyncOperationHandle<GameObject> loadOp)
	{
		int num;
		if (!this._loadOp_to_partInfoIndex.TryGetValue(loadOp, ref num))
		{
			if (loadOp.Status == 1 && loadOp.Result)
			{
				Object.Destroy(loadOp.Result);
			}
			return;
		}
		HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[num];
		if (loadOp.Status == 2)
		{
			Debug.Log("HeadModel: Failed to load a part for cosmetic \"" + cosmeticPartLoadInfo.playFabId + "\"! Waiting for 10 seconds before trying again.", this);
			GTDelayedExec.Add(this, 10f, num);
			return;
		}
		cosmeticPartLoadInfo.xform = loadOp.Result.transform;
		cosmeticPartLoadInfo.xform.localPosition = cosmeticPartLoadInfo.attachInfo.offset.pos;
		cosmeticPartLoadInfo.xform.localRotation = cosmeticPartLoadInfo.attachInfo.offset.rot;
		cosmeticPartLoadInfo.xform.localScale = cosmeticPartLoadInfo.attachInfo.offset.scale;
		cosmeticPartLoadInfo.xform.gameObject.SetActive(true);
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x00097F60 File Offset: 0x00096160
	void IDelayedExecListener.OnDelayedAction(int partLoadInfosIndex)
	{
		if (partLoadInfosIndex < 0 || partLoadInfosIndex >= this._currentPartLoadInfos.Count)
		{
			return;
		}
		HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[partLoadInfosIndex];
		if (cosmeticPartLoadInfo.loadOp.Status != 2)
		{
			return;
		}
		cosmeticPartLoadInfo.loadOp.Completed += new Action<AsyncOperationHandle<GameObject>>(this._HandleLoadOpOnCompleted);
		cosmeticPartLoadInfo.loadOp = cosmeticPartLoadInfo.prefabAssetRef.InstantiateAsync(base.transform, false);
		this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = partLoadInfosIndex;
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x00097FE8 File Offset: 0x000961E8
	protected void _ClearCurrent()
	{
		for (int i = 0; i < this._currentPartLoadInfos.Count; i++)
		{
			Object.Destroy(this._currentPartLoadInfos[i].loadOp.Result);
		}
		this._EnsureCapacityAndClear<AsyncOperationHandle, int>(this._loadOp_to_partInfoIndex);
		this._EnsureCapacityAndClear<HeadModel._CosmeticPartLoadInfo>(this._currentPartLoadInfos);
		if (this._mannequinRenderer.IsNull())
		{
			this.RefreshRenderer();
		}
		this._mannequinRenderer.enabled = true;
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x00098060 File Offset: 0x00096260
	private void _EnsureCapacityAndClear<T>(List<T> list)
	{
		if (list.Count > list.Capacity)
		{
			list.Capacity = list.Count;
		}
		list.Clear();
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x00098082 File Offset: 0x00096282
	private void _EnsureCapacityAndClear<T1, T2>(Dictionary<T1, T2> dict)
	{
		dict.EnsureCapacity(dict.Count);
		dict.Clear();
	}

	// Token: 0x040026B9 RID: 9913
	[DebugReadout]
	protected readonly List<HeadModel._CosmeticPartLoadInfo> _currentPartLoadInfos = new List<HeadModel._CosmeticPartLoadInfo>(1);

	// Token: 0x040026BA RID: 9914
	[DebugReadout]
	private readonly Dictionary<AsyncOperationHandle, int> _loadOp_to_partInfoIndex = new Dictionary<AsyncOperationHandle, int>(1);

	// Token: 0x040026BB RID: 9915
	private Renderer _mannequinRenderer;

	// Token: 0x040026BC RID: 9916
	public GameObject[] cosmetics;

	// Token: 0x0200046F RID: 1135
	protected struct _CosmeticPartLoadInfo
	{
		// Token: 0x040026BD RID: 9917
		public string playFabId;

		// Token: 0x040026BE RID: 9918
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x040026BF RID: 9919
		public CosmeticAttachInfo attachInfo;

		// Token: 0x040026C0 RID: 9920
		public AsyncOperationHandle<GameObject> loadOp;

		// Token: 0x040026C1 RID: 9921
		public Transform xform;
	}
}
