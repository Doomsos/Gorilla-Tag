using System;
using System.Collections;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200056F RID: 1391
public class BuilderDispenser : MonoBehaviour
{
	// Token: 0x06002309 RID: 8969 RVA: 0x000B7140 File Offset: 0x000B5340
	private void Awake()
	{
		this.nullPiece = new BuilderPieceSet.PieceInfo
		{
			piecePrefab = null,
			overrideSetMaterial = false
		};
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x000B716C File Offset: 0x000B536C
	public void UpdateDispenser()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.hasPiece && Time.timeAsDouble > this.nextSpawnTime && this.pieceToSpawn.piecePrefab != null)
		{
			this.TrySpawnPiece();
			this.nextSpawnTime = Time.timeAsDouble + (double)this.spawnRetryDelay;
			return;
		}
		if (this.hasPiece && (this.spawnedPieceInstance == null || (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)))
		{
			base.StopAllCoroutines();
			if (this.spawnedPieceInstance != null)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
			}
			this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
			this.spawnedPieceInstance = null;
			this.hasPiece = false;
		}
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000B7238 File Offset: 0x000B5438
	public bool DoesPieceMatchSpawnInfo(BuilderPiece piece)
	{
		if (piece == null || this.pieceToSpawn.piecePrefab == null)
		{
			return false;
		}
		if (piece.pieceType != this.pieceToSpawn.piecePrefab.name.GetStaticHash())
		{
			return false;
		}
		if (!(piece.materialOptions != null))
		{
			return true;
		}
		int num = piece.materialType;
		int num2;
		Material material;
		int num3;
		piece.materialOptions.GetDefaultMaterial(out num2, out material, out num3);
		if (this.pieceToSpawn.overrideSetMaterial)
		{
			for (int i = 0; i < this.pieceToSpawn.pieceMaterialTypes.Length; i++)
			{
				string text = this.pieceToSpawn.pieceMaterialTypes[i];
				if (!string.IsNullOrEmpty(text))
				{
					int hashCode = text.GetHashCode();
					if (hashCode == num)
					{
						return true;
					}
					if (hashCode == num2 && num == -1)
					{
						return true;
					}
				}
				else if (num == -1 || num == num2)
				{
					return true;
				}
			}
		}
		else if (num == this.materialType || (this.materialType == num2 && num == -1) || (num == num2 && this.materialType == -1))
		{
			return true;
		}
		return false;
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x000B733C File Offset: 0x000B553C
	public void ShelfPieceCreated(BuilderPiece piece, bool playAnimation)
	{
		if (this.DoesPieceMatchSpawnInfo(piece))
		{
			if (this.hasPiece && this.spawnedPieceInstance != null)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
			}
			this.spawnedPieceInstance = piece;
			this.hasPiece = true;
			this.spawnCount++;
			this.spawnCount = Mathf.Max(0, this.spawnCount);
			if (this.table.GetTableState() == BuilderTable.TableState.Ready && playAnimation)
			{
				base.StartCoroutine(this.PlayAnimation());
				if (this.playFX)
				{
					ObjectPools.instance.Instantiate(this.dispenserFX, this.spawnTransform.position, this.spawnTransform.rotation, true);
					return;
				}
				this.playFX = true;
				return;
			}
			else
			{
				Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
				Vector3 vector = this.displayTransform.position + this.displayTransform.rotation * desiredShelfOffset;
				Quaternion quaternion = this.displayTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
				this.spawnedPieceInstance.transform.SetPositionAndRotation(vector, quaternion);
				this.spawnedPieceInstance.SetState(BuilderPiece.State.OnShelf, false);
				this.playFX = true;
			}
		}
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000B747C File Offset: 0x000B567C
	private IEnumerator PlayAnimation()
	{
		this.spawnedPieceInstance.SetState(BuilderPiece.State.Displayed, false);
		this.animateParent.Rewind();
		this.spawnedPieceInstance.transform.SetParent(this.animateParent.transform);
		this.spawnedPieceInstance.transform.SetLocalPositionAndRotation(this.pieceToSpawn.piecePrefab.desiredShelfOffset, Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset));
		this.animateParent.Play();
		yield return new WaitForSeconds(this.animateParent.clip.length);
		if (this.spawnedPieceInstance != null && this.spawnedPieceInstance.state == BuilderPiece.State.Displayed)
		{
			this.spawnedPieceInstance.transform.SetParent(null);
			Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
			Vector3 vector = this.displayTransform.position + this.displayTransform.rotation * desiredShelfOffset;
			Quaternion quaternion = this.displayTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
			this.spawnedPieceInstance.transform.SetPositionAndRotation(vector, quaternion);
			this.spawnedPieceInstance.SetState(BuilderPiece.State.OnShelf, false);
		}
		yield break;
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000B748C File Offset: 0x000B568C
	public void ShelfPieceRecycled(BuilderPiece piece)
	{
		if (piece != null && this.spawnedPieceInstance != null && piece.Equals(this.spawnedPieceInstance))
		{
			piece.shelfOwner = -1;
			this.spawnedPieceInstance = null;
			this.hasPiece = false;
			this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
		}
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000B74E8 File Offset: 0x000B56E8
	public void AssignPieceType(BuilderPieceSet.PieceInfo piece, int inMaterialType)
	{
		this.playFX = false;
		this.pieceToSpawn = piece;
		this.materialType = inMaterialType;
		this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
		this.currentAnimation = this.dispenseDefaultAnimation;
		this.animateParent.clip = this.currentAnimation;
		this.spawnCount = 0;
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000B7544 File Offset: 0x000B5744
	private void TrySpawnPiece()
	{
		if (this.spawnedPieceInstance != null && this.spawnedPieceInstance.state == BuilderPiece.State.OnShelf)
		{
			return;
		}
		if (this.pieceToSpawn.piecePrefab == null)
		{
			return;
		}
		if (this.table.HasEnoughResources(this.pieceToSpawn.piecePrefab))
		{
			Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
			Vector3 position = this.spawnTransform.position + this.spawnTransform.rotation * desiredShelfOffset;
			Quaternion rotation = this.spawnTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
			int num = this.materialType;
			if (this.pieceToSpawn.overrideSetMaterial && this.pieceToSpawn.pieceMaterialTypes.Length != 0)
			{
				int num2 = this.spawnCount % this.pieceToSpawn.pieceMaterialTypes.Length;
				string text = this.pieceToSpawn.pieceMaterialTypes[num2];
				if (string.IsNullOrEmpty(text))
				{
					num = -1;
				}
				else
				{
					num = text.GetHashCode();
				}
			}
			this.table.RequestCreateDispenserShelfPiece(this.pieceToSpawn.piecePrefab.name.GetStaticHash(), position, rotation, num, this.shelfID);
		}
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x000B767C File Offset: 0x000B587C
	public void ParentPieceToShelf(Transform shelfTransform)
	{
		if (this.spawnedPieceInstance != null)
		{
			if (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)
			{
				base.StopAllCoroutines();
				if (this.spawnedPieceInstance != null)
				{
					this.spawnedPieceInstance.shelfOwner = -1;
				}
				this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
				this.spawnedPieceInstance = null;
				this.hasPiece = false;
				return;
			}
			this.spawnedPieceInstance.SetState(BuilderPiece.State.Displayed, false);
			this.spawnedPieceInstance.transform.SetParent(shelfTransform);
		}
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000B7714 File Offset: 0x000B5914
	public void ClearDispenser()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.pieceToSpawn = this.nullPiece;
		this.hasPiece = false;
		if (this.spawnedPieceInstance != null)
		{
			if (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
				this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
				this.spawnedPieceInstance = null;
				return;
			}
			this.table.RequestRecyclePiece(this.spawnedPieceInstance, false, -1);
		}
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000B77A0 File Offset: 0x000B59A0
	public void OnClearTable()
	{
		this.playFX = false;
		this.nextSpawnTime = 0.0;
		this.hasPiece = false;
		this.spawnedPieceInstance = null;
	}

	// Token: 0x04002DCA RID: 11722
	public Transform displayTransform;

	// Token: 0x04002DCB RID: 11723
	public Transform spawnTransform;

	// Token: 0x04002DCC RID: 11724
	public Animation animateParent;

	// Token: 0x04002DCD RID: 11725
	public AnimationClip dispenseDefaultAnimation;

	// Token: 0x04002DCE RID: 11726
	public GameObject dispenserFX;

	// Token: 0x04002DCF RID: 11727
	private AnimationClip currentAnimation;

	// Token: 0x04002DD0 RID: 11728
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x04002DD1 RID: 11729
	[HideInInspector]
	public int shelfID;

	// Token: 0x04002DD2 RID: 11730
	private BuilderPieceSet.PieceInfo pieceToSpawn;

	// Token: 0x04002DD3 RID: 11731
	private BuilderPiece spawnedPieceInstance;

	// Token: 0x04002DD4 RID: 11732
	private int materialType = -1;

	// Token: 0x04002DD5 RID: 11733
	private BuilderPieceSet.PieceInfo nullPiece;

	// Token: 0x04002DD6 RID: 11734
	private int spawnCount;

	// Token: 0x04002DD7 RID: 11735
	private double nextSpawnTime;

	// Token: 0x04002DD8 RID: 11736
	private bool hasPiece;

	// Token: 0x04002DD9 RID: 11737
	private float OnGrabSpawnDelay = 0.5f;

	// Token: 0x04002DDA RID: 11738
	private float spawnRetryDelay = 2f;

	// Token: 0x04002DDB RID: 11739
	private bool playFX;
}
