using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DB7 RID: 3511
	public class BuilderPool : MonoBehaviour
	{
		// Token: 0x06005669 RID: 22121 RVA: 0x001B2B6A File Offset: 0x001B0D6A
		private void Awake()
		{
			if (BuilderPool.instance == null)
			{
				BuilderPool.instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x0600566A RID: 22122 RVA: 0x001B2B88 File Offset: 0x001B0D88
		public void Setup()
		{
			if (this.isSetup)
			{
				return;
			}
			this.piecePools = new List<List<BuilderPiece>>(512);
			this.piecePoolLookup = new Dictionary<int, int>(512);
			this.bumpGlowPool = new List<BuilderBumpGlow>(256);
			this.AddToGlowBumpPool(256);
			this.snapOverlapPool = new List<SnapOverlap>(4096);
			this.AddToSnapOverlapPool(4096);
			this.isSetup = true;
		}

		// Token: 0x0600566B RID: 22123 RVA: 0x001B2BFC File Offset: 0x001B0DFC
		public void BuildFromShelves(List<BuilderShelf> shelves)
		{
			for (int i = 0; i < shelves.Count; i++)
			{
				BuilderShelf builderShelf = shelves[i];
				for (int j = 0; j < builderShelf.buildPieceSpawns.Count; j++)
				{
					BuilderShelf.BuildPieceSpawn buildPieceSpawn = builderShelf.buildPieceSpawns[j];
					this.AddToPool(buildPieceSpawn.buildPiecePrefab.name.GetStaticHash(), buildPieceSpawn.count);
				}
			}
		}

		// Token: 0x0600566C RID: 22124 RVA: 0x001B2C61 File Offset: 0x001B0E61
		public IEnumerator BuildFromPieceSets()
		{
			if (this.hasBuiltPieceSets)
			{
				yield break;
			}
			this.hasBuiltPieceSets = true;
			List<BuilderPieceSet> allPieceSets = BuilderSetManager.instance.GetAllPieceSets();
			foreach (BuilderPieceSet builderPieceSet in allPieceSets)
			{
				bool isStarterSet = BuilderSetManager.instance.GetStarterSetsConcat().Contains(builderPieceSet.playfabID);
				bool isFallbackSet = builderPieceSet.SetName.Equals("HIDDEN");
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int pieceType = pieceInfo.piecePrefab.name.GetStaticHash();
						int count;
						if (!this.piecePoolLookup.TryGetValue(pieceType, ref count))
						{
							count = this.piecePools.Count;
							this.piecePools.Add(new List<BuilderPiece>(128));
							this.piecePoolLookup.Add(pieceType, count);
							if (!isFallbackSet)
							{
								int numToCreate = isStarterSet ? 32 : 8;
								int i = 0;
								while (i < numToCreate)
								{
									i += 2;
									this.AddToPool(pieceType, 2);
									yield return null;
								}
							}
						}
						yield return null;
					}
					List<BuilderPieceSet.PieceInfo>.Enumerator enumerator3 = default(List<BuilderPieceSet.PieceInfo>.Enumerator);
				}
				List<BuilderPieceSet.BuilderPieceSubset>.Enumerator enumerator2 = default(List<BuilderPieceSet.BuilderPieceSubset>.Enumerator);
			}
			List<BuilderPieceSet>.Enumerator enumerator = default(List<BuilderPieceSet>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600566D RID: 22125 RVA: 0x001B2C70 File Offset: 0x001B0E70
		private void AddToPool(int pieceType, int count)
		{
			int count2;
			if (!this.piecePoolLookup.TryGetValue(pieceType, ref count2))
			{
				count2 = this.piecePools.Count;
				this.piecePools.Add(new List<BuilderPiece>(count * 8));
				this.piecePoolLookup.Add(pieceType, count2);
				Debug.LogWarningFormat("Creating Pool for piece {0} of size {1}. Is this piece not in a piece set?", new object[]
				{
					pieceType,
					count * 8
				});
			}
			BuilderPiece piecePrefab = BuilderSetManager.instance.GetPiecePrefab(pieceType);
			if (piecePrefab == null)
			{
				return;
			}
			List<BuilderPiece> list = this.piecePools[count2];
			for (int i = 0; i < count; i++)
			{
				BuilderPiece builderPiece = Object.Instantiate<BuilderPiece>(piecePrefab);
				builderPiece.OnCreatedByPool();
				builderPiece.gameObject.SetActive(false);
				list.Add(builderPiece);
			}
		}

		// Token: 0x0600566E RID: 22126 RVA: 0x001B2D34 File Offset: 0x001B0F34
		public BuilderPiece CreatePiece(int pieceType, bool assertNotEmpty)
		{
			int count;
			if (!this.piecePoolLookup.TryGetValue(pieceType, ref count))
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("No Pool Found for {0} Adding 4", new object[]
					{
						pieceType
					});
				}
				count = this.piecePools.Count;
				this.AddToPool(pieceType, 4);
			}
			List<BuilderPiece> list = this.piecePools[count];
			if (list.Count == 0)
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("Pool for {0} is Empty Adding 4", new object[]
					{
						pieceType
					});
				}
				this.AddToPool(pieceType, 4);
			}
			BuilderPiece result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return result;
		}

		// Token: 0x0600566F RID: 22127 RVA: 0x001B2DD8 File Offset: 0x001B0FD8
		public void DestroyPiece(BuilderPiece piece)
		{
			if (piece == null)
			{
				Debug.LogError("Why is a null piece being destroyed");
				return;
			}
			int num;
			if (!this.piecePoolLookup.TryGetValue(piece.pieceType, ref num))
			{
				Debug.LogErrorFormat("No Pool Found for {0} Cannot return to pool", new object[]
				{
					piece.pieceType
				});
				return;
			}
			List<BuilderPiece> list = this.piecePools[num];
			if (list.Count == 128)
			{
				piece.OnReturnToPool();
				Object.Destroy(piece.gameObject);
				return;
			}
			piece.gameObject.SetActive(false);
			piece.transform.SetParent(null);
			piece.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			piece.OnReturnToPool();
			list.Add(piece);
		}

		// Token: 0x06005670 RID: 22128 RVA: 0x001B2EA0 File Offset: 0x001B10A0
		private void AddToGlowBumpPool(int count)
		{
			if (this.bumpGlowPrefab == null)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				BuilderBumpGlow builderBumpGlow = Object.Instantiate<BuilderBumpGlow>(this.bumpGlowPrefab);
				builderBumpGlow.gameObject.SetActive(false);
				this.bumpGlowPool.Add(builderBumpGlow);
			}
		}

		// Token: 0x06005671 RID: 22129 RVA: 0x001B2EEC File Offset: 0x001B10EC
		public BuilderBumpGlow CreateGlowBump()
		{
			if (this.bumpGlowPool.Count == 0)
			{
				this.AddToGlowBumpPool(4);
			}
			BuilderBumpGlow result = this.bumpGlowPool[this.bumpGlowPool.Count - 1];
			this.bumpGlowPool.RemoveAt(this.bumpGlowPool.Count - 1);
			return result;
		}

		// Token: 0x06005672 RID: 22130 RVA: 0x001B2F40 File Offset: 0x001B1140
		public void DestroyBumpGlow(BuilderBumpGlow bump)
		{
			if (bump == null)
			{
				return;
			}
			bump.gameObject.SetActive(false);
			bump.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			this.bumpGlowPool.Add(bump);
		}

		// Token: 0x06005673 RID: 22131 RVA: 0x001B2F90 File Offset: 0x001B1190
		private void AddToSnapOverlapPool(int count)
		{
			this.snapOverlapPool.Capacity = this.snapOverlapPool.Capacity + count;
			for (int i = 0; i < count; i++)
			{
				this.snapOverlapPool.Add(new SnapOverlap());
			}
		}

		// Token: 0x06005674 RID: 22132 RVA: 0x001B2FD4 File Offset: 0x001B11D4
		public SnapOverlap CreateSnapOverlap(BuilderAttachGridPlane otherPlane, SnapBounds bounds)
		{
			if (this.snapOverlapPool.Count == 0)
			{
				this.AddToSnapOverlapPool(1024);
			}
			SnapOverlap snapOverlap = this.snapOverlapPool[this.snapOverlapPool.Count - 1];
			this.snapOverlapPool.RemoveAt(this.snapOverlapPool.Count - 1);
			snapOverlap.otherPlane = otherPlane;
			snapOverlap.bounds = bounds;
			snapOverlap.nextOverlap = null;
			return snapOverlap;
		}

		// Token: 0x06005675 RID: 22133 RVA: 0x001B303E File Offset: 0x001B123E
		public void DestroySnapOverlap(SnapOverlap snapOverlap)
		{
			snapOverlap.otherPlane = null;
			snapOverlap.nextOverlap = null;
			this.snapOverlapPool.Add(snapOverlap);
		}

		// Token: 0x06005676 RID: 22134 RVA: 0x001B305C File Offset: 0x001B125C
		private void OnDestroy()
		{
			for (int i = 0; i < this.piecePools.Count; i++)
			{
				if (this.piecePools[i] != null)
				{
					foreach (BuilderPiece builderPiece in this.piecePools[i])
					{
						if (builderPiece != null)
						{
							Object.Destroy(builderPiece);
						}
					}
					this.piecePools[i].Clear();
				}
			}
			this.piecePoolLookup.Clear();
			foreach (BuilderBumpGlow builderBumpGlow in this.bumpGlowPool)
			{
				Object.Destroy(builderBumpGlow);
			}
			this.bumpGlowPool.Clear();
		}

		// Token: 0x0400638C RID: 25484
		public List<List<BuilderPiece>> piecePools;

		// Token: 0x0400638D RID: 25485
		public Dictionary<int, int> piecePoolLookup;

		// Token: 0x0400638E RID: 25486
		[HideInInspector]
		public List<BuilderBumpGlow> bumpGlowPool;

		// Token: 0x0400638F RID: 25487
		public BuilderBumpGlow bumpGlowPrefab;

		// Token: 0x04006390 RID: 25488
		[HideInInspector]
		public List<SnapOverlap> snapOverlapPool;

		// Token: 0x04006391 RID: 25489
		public static BuilderPool instance;

		// Token: 0x04006392 RID: 25490
		private const int POOl_CAPACITY = 128;

		// Token: 0x04006393 RID: 25491
		private const int INITIAL_INSTANCE_COUNT_STARTER = 32;

		// Token: 0x04006394 RID: 25492
		private const int INITIAL_INSTANCE_COUNT_PREMIUM = 8;

		// Token: 0x04006395 RID: 25493
		private bool isSetup;

		// Token: 0x04006396 RID: 25494
		private bool hasBuiltPieceSets;
	}
}
