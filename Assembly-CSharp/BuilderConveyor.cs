using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

// Token: 0x0200056E RID: 1390
public class BuilderConveyor : MonoBehaviour
{
	// Token: 0x060022F1 RID: 8945 RVA: 0x000B67AC File Offset: 0x000B49AC
	private void Start()
	{
		this.InitIfNeeded();
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000B67AC File Offset: 0x000B49AC
	public void Setup()
	{
		this.InitIfNeeded();
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000B67B4 File Offset: 0x000B49B4
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.nextPieceToSpawn = 0;
		this.grabbedPieceTypes = new Queue<int>(10);
		this.grabbedPieceMaterials = new Queue<int>(10);
		this.setSelector.Setup(this._includeCategories);
		this.currentDisplayGroup = this.setSelector.GetSelectedGroup();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentDisplayGroup.pieceSubsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		double timeAsDouble = Time.timeAsDouble;
		this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		this.setSelector.OnSelectedGroup.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.initialized = true;
		this.splineLength = this.spline.Splines[0].GetLength();
		this.maxItemsOnSpline = Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
		this.nativeSpline = new NativeSpline(this.spline.Splines[0], this.spline.transform.localToWorldMatrix, 4);
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000B6928 File Offset: 0x000B4B28
	public int GetMaxItemsOnConveyor()
	{
		return Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000B6945 File Offset: 0x000B4B45
	public float GetFrameMovement()
	{
		return this.conveyorMoveSpeed / this.splineLength;
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000B6954 File Offset: 0x000B4B54
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedGroup.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
		this.nativeSpline.Dispose();
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000B698B File Offset: 0x000B4B8B
	public void OnSelectedSetChange(int displayGroupID)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, displayGroupID, true);
	}

	// Token: 0x060022F8 RID: 8952 RVA: 0x000B69B0 File Offset: 0x000B4BB0
	public void SetSelection(int displayGroupID)
	{
		this.setSelector.SetSelection(displayGroupID);
		this.currentDisplayGroup = this.setSelector.GetSelectedGroup();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentDisplayGroup.pieceSubsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		this.nextPieceToSpawn = 0;
		this.loopCount = 0;
	}

	// Token: 0x060022F9 RID: 8953 RVA: 0x000B6A5C File Offset: 0x000B4C5C
	public int GetSelectedDisplayGroupID()
	{
		return this.setSelector.GetSelectedGroup().GetDisplayGroupIdentifier();
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000B6A70 File Offset: 0x000B4C70
	public void UpdateConveyor()
	{
		if (!this.initialized)
		{
			this.Setup();
		}
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (builderPiece.state != BuilderPiece.State.OnConveyor)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient && builderPiece.state != BuilderPiece.State.None)
				{
					this.grabbedPieceTypes.Enqueue(builderPiece.pieceType);
					this.grabbedPieceMaterials.Enqueue(builderPiece.materialType);
				}
				builderPiece.shelfOwner = -1;
				this.piecesOnConveyor.RemoveAt(i);
				this.table.conveyorManager.RemovePieceFromJob(builderPiece);
			}
		}
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000B6B14 File Offset: 0x000B4D14
	public void RemovePieceFromConveyor(Transform pieceTransform)
	{
		foreach (BuilderPiece builderPiece in this.piecesOnConveyor)
		{
			if (builderPiece.transform == pieceTransform)
			{
				this.piecesOnConveyor.Remove(builderPiece);
				builderPiece.shelfOwner = -1;
				this.table.RequestRecyclePiece(builderPiece, false, -1);
				break;
			}
		}
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000B6B94 File Offset: 0x000B4D94
	private Vector3 EvaluateSpline(float t)
	{
		float num;
		this._evaluateCurve = this.nativeSpline.GetCurve(SplineUtility.SplineToCurveT<NativeSpline>(this.nativeSpline, t, ref num));
		return CurveUtility.EvaluatePosition(this._evaluateCurve, num);
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x000B6BD4 File Offset: 0x000B4DD4
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (this.shouldVerifySetSelection)
		{
			BuilderPieceSet.BuilderDisplayGroup selectedGroup = this.setSelector.GetSelectedGroup();
			if (selectedGroup == null || !BuilderSetManager.instance.DoesAnyPlayerInRoomOwnPieceSet(selectedGroup.setID))
			{
				int defaultGroupID = this.setSelector.GetDefaultGroupID();
				if (defaultGroupID != -1)
				{
					this.OnSelectedSetChange(defaultGroupID);
				}
			}
			this.shouldVerifySetSelection = false;
		}
		if (this.waitForResourceChange)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble >= this.nextSpawnTime)
		{
			this.SpawnNextPiece();
			this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		}
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x000B6C64 File Offset: 0x000B4E64
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x000B6C6D File Offset: 0x000B4E6D
	public void OnAvailableResourcesChange()
	{
		this.waitForResourceChange = false;
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x000B6C76 File Offset: 0x000B4E76
	public Transform GetSpawnTransform()
	{
		return this.spawnTransform;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000B6C80 File Offset: 0x000B4E80
	public void OnShelfPieceCreated(BuilderPiece piece, float timeOffset)
	{
		float num = timeOffset * this.conveyorMoveSpeed / this.splineLength;
		if (num > 1f)
		{
			Debug.LogWarningFormat("Piece {0} add to shelf time {1}", new object[]
			{
				piece.pieceId,
				num
			});
		}
		int count = this.piecesOnConveyor.Count;
		this.piecesOnConveyor.Add(piece);
		float num2 = Mathf.Clamp(num, 0f, 1f);
		Vector3 vector = this.EvaluateSpline(num2);
		Quaternion quaternion = this.spawnTransform.rotation * Quaternion.Euler(piece.desiredShelfRotationOffset);
		Vector3 vector2 = vector + this.spawnTransform.rotation * piece.desiredShelfOffset;
		piece.transform.SetPositionAndRotation(vector2, quaternion);
		if (num <= 1f)
		{
			this.table.conveyorManager.AddPieceToJob(piece, num2, this.shelfID);
		}
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000B6D61 File Offset: 0x000B4F61
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		this.piecesOnConveyor.Remove(piece);
		if (piece != null)
		{
			this.table.conveyorManager.RemovePieceFromJob(piece);
		}
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000B6D8A File Offset: 0x000B4F8A
	public void OnClearTable()
	{
		this.piecesOnConveyor.Clear();
		this.grabbedPieceTypes.Clear();
		this.grabbedPieceMaterials.Clear();
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000B6DB0 File Offset: 0x000B4FB0
	public void ResetConveyorState()
	{
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (!(builderPiece == null))
			{
				BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
				{
					type = BuilderTable.BuilderCommandType.Recycle,
					pieceId = builderPiece.pieceId,
					localPosition = builderPiece.transform.position,
					localRotation = builderPiece.transform.rotation,
					player = NetworkSystem.Instance.LocalPlayer,
					isLeft = false,
					parentPieceId = -1
				};
				this.table.ExecutePieceRecycled(cmd);
			}
		}
		this.OnClearTable();
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000B6E68 File Offset: 0x000B5068
	private void SpawnNextPiece()
	{
		int num;
		int materialType;
		this.FindNextAffordablePieceType(out num, out materialType);
		if (num == -1)
		{
			return;
		}
		this.table.RequestCreateConveyorPiece(num, materialType, this.shelfID);
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x000B6E98 File Offset: 0x000B5098
	private void FindNextAffordablePieceType(out int pieceType, out int materialType)
	{
		if (this.grabbedPieceTypes.Count > 0)
		{
			pieceType = this.grabbedPieceTypes.Dequeue();
			materialType = this.grabbedPieceMaterials.Dequeue();
			return;
		}
		pieceType = -1;
		materialType = -1;
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		for (int i = this.nextPieceToSpawn; i < this.piecesInSet.Count; i++)
		{
			BuilderPiece piecePrefab = this.piecesInSet[i].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab))
			{
				if (i + 1 >= this.piecesInSet.Count)
				{
					this.loopCount++;
					this.loopCount = Mathf.Max(0, this.loopCount);
				}
				this.nextPieceToSpawn = (i + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[i]);
				return;
			}
		}
		this.loopCount++;
		this.loopCount = Mathf.Max(0, this.loopCount);
		for (int j = 0; j < this.nextPieceToSpawn; j++)
		{
			BuilderPiece piecePrefab2 = this.piecesInSet[j].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab2))
			{
				this.nextPieceToSpawn = (j + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab2.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[j]);
				return;
			}
		}
		this.waitForResourceChange = true;
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x000B701C File Offset: 0x000B521C
	private int GetMaterialType(BuilderPieceSet.PieceInfo info)
	{
		if (info.piecePrefab.materialOptions != null && info.overrideSetMaterial && info.pieceMaterialTypes.Length != 0)
		{
			int num = this.loopCount % info.pieceMaterialTypes.Length;
			string text = info.pieceMaterialTypes[num];
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogErrorFormat("Empty Material Override for piece {0} in set {1}", new object[]
				{
					info.piecePrefab.name,
					this.currentDisplayGroup.displayName
				});
				return -1;
			}
			return text.GetHashCode();
		}
		else
		{
			if (string.IsNullOrEmpty(this.currentDisplayGroup.defaultMaterial))
			{
				return -1;
			}
			return this.currentDisplayGroup.defaultMaterial.GetHashCode();
		}
	}

	// Token: 0x04002DB2 RID: 11698
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x04002DB3 RID: 11699
	public List<BuilderPieceSet.BuilderPieceCategory> _includeCategories;

	// Token: 0x04002DB4 RID: 11700
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x04002DB5 RID: 11701
	public int shelfID = -1;

	// Token: 0x04002DB6 RID: 11702
	[Header("Conveyor Properties")]
	[SerializeField]
	private Transform spawnTransform;

	// Token: 0x04002DB7 RID: 11703
	[SerializeField]
	private SplineContainer spline;

	// Token: 0x04002DB8 RID: 11704
	private float conveyorMoveSpeed = 0.2f;

	// Token: 0x04002DB9 RID: 11705
	private float spawnDelay = 1.5f;

	// Token: 0x04002DBA RID: 11706
	private double nextSpawnTime;

	// Token: 0x04002DBB RID: 11707
	private int nextPieceToSpawn;

	// Token: 0x04002DBC RID: 11708
	private BuilderPieceSet.BuilderDisplayGroup currentDisplayGroup;

	// Token: 0x04002DBD RID: 11709
	private int loopCount;

	// Token: 0x04002DBE RID: 11710
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x04002DBF RID: 11711
	private Queue<int> grabbedPieceTypes;

	// Token: 0x04002DC0 RID: 11712
	private Queue<int> grabbedPieceMaterials;

	// Token: 0x04002DC1 RID: 11713
	private List<BuilderPiece> piecesOnConveyor = new List<BuilderPiece>(10);

	// Token: 0x04002DC2 RID: 11714
	private Vector3 moveDirection;

	// Token: 0x04002DC3 RID: 11715
	private bool waitForResourceChange;

	// Token: 0x04002DC4 RID: 11716
	private bool initialized;

	// Token: 0x04002DC5 RID: 11717
	private float splineLength = 1f;

	// Token: 0x04002DC6 RID: 11718
	private int maxItemsOnSpline;

	// Token: 0x04002DC7 RID: 11719
	private BezierCurve _evaluateCurve;

	// Token: 0x04002DC8 RID: 11720
	public NativeSpline nativeSpline;

	// Token: 0x04002DC9 RID: 11721
	private bool shouldVerifySetSelection;
}
