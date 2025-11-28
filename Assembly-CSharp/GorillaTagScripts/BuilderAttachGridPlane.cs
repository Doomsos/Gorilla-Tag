using System;
using GorillaTagScripts.Builder;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DB0 RID: 3504
	public class BuilderAttachGridPlane : MonoBehaviour
	{
		// Token: 0x06005624 RID: 22052 RVA: 0x001B1026 File Offset: 0x001AF226
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x06005625 RID: 22053 RVA: 0x001B1044 File Offset: 0x001AF244
		public void Setup(BuilderPiece piece, int attachIndex, float gridSize)
		{
			this.piece = piece;
			this.attachIndex = attachIndex;
			this.pieceToGridPosition = piece.transform.InverseTransformPoint(base.transform.position);
			this.pieceToGridRotation = Quaternion.Inverse(piece.transform.rotation) * base.transform.rotation;
			float num = (float)(this.width + 2) * gridSize;
			float num2 = (float)(this.length + 2) * gridSize;
			this.boundingRadius = Mathf.Sqrt(num * num + num2 * num2);
			this.connected = new bool[this.width * this.length];
			this.widthOffset = ((this.width % 2 == 0) ? (gridSize / 2f) : 0f);
			this.lengthOffset = ((this.length % 2 == 0) ? (gridSize / 2f) : 0f);
			this.gridPlaneDataIndex = -1;
			this.childPieceCount = 0;
		}

		// Token: 0x06005626 RID: 22054 RVA: 0x001B1130 File Offset: 0x001AF330
		public void OnReturnToPool(BuilderPool pool)
		{
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				SnapOverlap snapOverlap = nextOverlap;
				nextOverlap = nextOverlap.nextOverlap;
				if (snapOverlap.otherPlane != null)
				{
					snapOverlap.otherPlane.RemoveSnapsWithPiece(this.piece, pool);
				}
				this.SetConnected(snapOverlap.bounds, false);
				pool.DestroySnapOverlap(snapOverlap);
			}
			int num = this.width * this.length;
			for (int i = 0; i < num; i++)
			{
				this.connected[i] = false;
			}
			this.childPieceCount = 0;
		}

		// Token: 0x06005627 RID: 22055 RVA: 0x001B11B0 File Offset: 0x001AF3B0
		public Vector3 GetGridPosition(int x, int z, float gridSize)
		{
			float num = (this.width % 2 == 0) ? (gridSize / 2f) : 0f;
			float num2 = (this.length % 2 == 0) ? (gridSize / 2f) : 0f;
			return this.center.position + this.center.rotation * new Vector3((float)x * gridSize - num, (this.male ? 0.002f : -0.002f) * gridSize, (float)z * gridSize - num2);
		}

		// Token: 0x06005628 RID: 22056 RVA: 0x001B1236 File Offset: 0x001AF436
		public int GetChildCount()
		{
			return this.childPieceCount;
		}

		// Token: 0x06005629 RID: 22057 RVA: 0x001B1240 File Offset: 0x001AF440
		public void ChangeChildPieceCount(int delta)
		{
			this.childPieceCount += delta;
			if (this.piece.parentPiece == null)
			{
				return;
			}
			if (this.piece.parentAttachIndex < 0 || this.piece.parentAttachIndex >= this.piece.parentPiece.gridPlanes.Count)
			{
				return;
			}
			this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].ChangeChildPieceCount(delta);
		}

		// Token: 0x0600562A RID: 22058 RVA: 0x001B12C6 File Offset: 0x001AF4C6
		public void AddSnapOverlap(SnapOverlap newOverlap)
		{
			if (this.firstOverlap == null)
			{
				this.firstOverlap = newOverlap;
			}
			else
			{
				newOverlap.nextOverlap = this.firstOverlap;
				this.firstOverlap = newOverlap;
			}
			this.SetConnected(newOverlap.bounds, true);
		}

		// Token: 0x0600562B RID: 22059 RVA: 0x001B12FC File Offset: 0x001AF4FC
		public void RemoveSnapsWithDifferentRoot(BuilderPiece root, BuilderPool pool)
		{
			if (this.firstOverlap == null)
			{
				return;
			}
			if (pool == null)
			{
				return;
			}
			SnapOverlap snapOverlap = null;
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				if (nextOverlap.otherPlane == null || nextOverlap.otherPlane.piece == null)
				{
					SnapOverlap snapOverlap2 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap2.bounds, false);
					pool.DestroySnapOverlap(snapOverlap2);
				}
				else if (root == null || nextOverlap.otherPlane.piece.GetRootPiece() != root)
				{
					SnapOverlap snapOverlap3 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap3.bounds, false);
					snapOverlap3.otherPlane.RemoveSnapsWithPiece(this.piece, pool);
					pool.DestroySnapOverlap(snapOverlap3);
				}
				else
				{
					snapOverlap = nextOverlap;
					nextOverlap = nextOverlap.nextOverlap;
				}
			}
		}

		// Token: 0x0600562C RID: 22060 RVA: 0x001B1414 File Offset: 0x001AF614
		public void RemoveSnapsWithPiece(BuilderPiece piece, BuilderPool pool)
		{
			if (this.firstOverlap == null)
			{
				return;
			}
			if (piece == null || pool == null)
			{
				return;
			}
			SnapOverlap snapOverlap = null;
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				if (nextOverlap.otherPlane == null || nextOverlap.otherPlane.piece == null)
				{
					SnapOverlap snapOverlap2 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap2.bounds, false);
					pool.DestroySnapOverlap(snapOverlap2);
				}
				else if (nextOverlap.otherPlane.piece == piece)
				{
					SnapOverlap snapOverlap3 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap3.bounds, false);
					pool.DestroySnapOverlap(snapOverlap3);
				}
				else
				{
					snapOverlap = nextOverlap;
					nextOverlap = nextOverlap.nextOverlap;
				}
			}
		}

		// Token: 0x0600562D RID: 22061 RVA: 0x001B1514 File Offset: 0x001AF714
		private void SetConnected(SnapBounds bounds, bool connect)
		{
			int num = this.width / 2 - ((this.width % 2 == 0) ? 1 : 0);
			int num2 = this.length / 2 - ((this.length % 2 == 0) ? 1 : 0);
			int num3 = this.connected.Length;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					int num4 = (num + i) * this.length + (j + num2);
					if (num4 >= num3 || num4 < 0)
					{
						if (this.piece != null)
						{
							int pieceId = this.piece.pieceId;
						}
						return;
					}
					this.connected[num4] = connect;
				}
			}
		}

		// Token: 0x0600562E RID: 22062 RVA: 0x001B15E4 File Offset: 0x001AF7E4
		public bool IsConnected(SnapBounds bounds)
		{
			int num = this.width / 2 - ((this.width % 2 == 0) ? 1 : 0);
			int num2 = this.length / 2 - ((this.length % 2 == 0) ? 1 : 0);
			int num3 = this.connected.Length;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					int num4 = (num + i) * this.length + (j + num2);
					if (num4 < 0 || num4 >= num3)
					{
						if (this.piece != null)
						{
							int pieceId = this.piece.pieceId;
						}
						return false;
					}
					if (this.connected[num4])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600562F RID: 22063 RVA: 0x001B16B8 File Offset: 0x001AF8B8
		public void CalcGridOverlap(BuilderAttachGridPlane otherGridPlane, Vector3 otherPieceLocalPos, Quaternion otherPieceLocalRot, float gridSize, out Vector2Int min, out Vector2Int max)
		{
			int num = otherGridPlane.width;
			int num2 = otherGridPlane.length;
			Quaternion quaternion = otherPieceLocalRot * otherGridPlane.pieceToGridRotation;
			Vector3 lossyScale = base.transform.lossyScale;
			otherPieceLocalPos.Scale(base.transform.lossyScale);
			Vector3 vector = otherPieceLocalPos + otherPieceLocalRot * otherGridPlane.pieceToGridPosition;
			if (Mathf.Abs(Vector3.Dot(quaternion * Vector3.forward, Vector3.forward)) < 0.707f)
			{
				num = otherGridPlane.length;
				num2 = otherGridPlane.width;
			}
			float num3 = (num % 2 == 0) ? (gridSize / 2f) : 0f;
			float num4 = (num2 % 2 == 0) ? (gridSize / 2f) : 0f;
			float num5 = (this.width % 2 == 0) ? (gridSize / 2f) : 0f;
			float num6 = (this.length % 2 == 0) ? (gridSize / 2f) : 0f;
			float num7 = num3 - num5;
			float num8 = num4 - num6;
			int num9 = Mathf.RoundToInt((vector.x - num7) / gridSize);
			int num10 = Mathf.RoundToInt((vector.z - num8) / gridSize);
			int num11 = num9 + Mathf.FloorToInt((float)num / 2f);
			int num12 = num10 + Mathf.FloorToInt((float)num2 / 2f);
			int num13 = num11 - (num - 1);
			int num14 = num12 - (num2 - 1);
			int num15 = Mathf.FloorToInt((float)this.width / 2f);
			int num16 = Mathf.FloorToInt((float)this.length / 2f);
			int num17 = num15 - (this.width - 1);
			int num18 = num16 - (this.length - 1);
			min = new Vector2Int(Mathf.Max(num13, num17), Mathf.Max(num14, num18));
			max = new Vector2Int(Mathf.Min(num11, num15), Mathf.Min(num12, num16));
		}

		// Token: 0x06005630 RID: 22064 RVA: 0x001B187C File Offset: 0x001AFA7C
		public bool IsAttachedToMovingGrid()
		{
			return this.piece.state == BuilderPiece.State.AttachedAndPlaced && !this.piece.isBuiltIntoTable && (this.isMoving || (!(this.piece.parentPiece == null) && this.piece.parentAttachIndex >= 0 && this.piece.parentAttachIndex < this.piece.parentPiece.gridPlanes.Count && this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].IsAttachedToMovingGrid()));
		}

		// Token: 0x06005631 RID: 22065 RVA: 0x001B1920 File Offset: 0x001AFB20
		public BuilderAttachGridPlane GetMovingParentGrid()
		{
			if (this.piece.isBuiltIntoTable)
			{
				return null;
			}
			if (this.movesOnPlace && this.movingPart != null && !this.movingPart.IsAnchoredToTable())
			{
				return this;
			}
			if (this.piece.parentPiece == null)
			{
				return null;
			}
			if (this.piece.parentAttachIndex < 0 || this.piece.parentAttachIndex >= this.piece.parentPiece.gridPlanes.Count)
			{
				return null;
			}
			return this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].GetMovingParentGrid();
		}

		// Token: 0x04006342 RID: 25410
		[Tooltip("Are the snap points in this grid \"outies\"")]
		public bool male;

		// Token: 0x04006343 RID: 25411
		[Tooltip("(Optional) midpoint of the grid")]
		public Transform center;

		// Token: 0x04006344 RID: 25412
		[Tooltip("number of snap points wide (local X-axis)")]
		public int width;

		// Token: 0x04006345 RID: 25413
		[Tooltip("number of snap points long (local z-axis)")]
		public int length;

		// Token: 0x04006346 RID: 25414
		[NonSerialized]
		public int gridPlaneDataIndex;

		// Token: 0x04006347 RID: 25415
		[NonSerialized]
		public BuilderItem item;

		// Token: 0x04006348 RID: 25416
		[NonSerialized]
		public BuilderPiece piece;

		// Token: 0x04006349 RID: 25417
		[NonSerialized]
		public int attachIndex;

		// Token: 0x0400634A RID: 25418
		[NonSerialized]
		public float boundingRadius;

		// Token: 0x0400634B RID: 25419
		[NonSerialized]
		public Vector3 pieceToGridPosition;

		// Token: 0x0400634C RID: 25420
		[NonSerialized]
		public Quaternion pieceToGridRotation;

		// Token: 0x0400634D RID: 25421
		[NonSerialized]
		public bool[] connected;

		// Token: 0x0400634E RID: 25422
		[NonSerialized]
		public SnapOverlap firstOverlap;

		// Token: 0x0400634F RID: 25423
		[NonSerialized]
		public float widthOffset;

		// Token: 0x04006350 RID: 25424
		[NonSerialized]
		public float lengthOffset;

		// Token: 0x04006351 RID: 25425
		private int childPieceCount;

		// Token: 0x04006352 RID: 25426
		[HideInInspector]
		public bool isMoving;

		// Token: 0x04006353 RID: 25427
		[HideInInspector]
		public bool movesOnPlace;

		// Token: 0x04006354 RID: 25428
		[HideInInspector]
		public BuilderMovingPart movingPart;
	}
}
