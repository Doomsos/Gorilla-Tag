using System;
using System.Collections;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000572 RID: 1394
public class BuilderDropZone : MonoBehaviour
{
	// Token: 0x0600232E RID: 9006 RVA: 0x000B8176 File Offset: 0x000B6376
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000B8194 File Offset: 0x000B6394
	private void OnTriggerEnter(Collider other)
	{
		if (!this.onEnter)
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BuilderPieceCollider component = other.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if (this.table != null && this.table.builderNetworking != null)
			{
				if (piece == null)
				{
					return;
				}
				if (this.dropType == BuilderDropZone.DropType.Recycle)
				{
					bool flag = piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.OnShelf && piece.state > BuilderPiece.State.AttachedAndPlaced;
					if (!piece.isBuiltIntoTable && flag)
					{
						this.table.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, true, -1);
						return;
					}
				}
				else
				{
					this.table.builderNetworking.PieceEnteredDropZone(piece, this.dropType, this.dropZoneID);
				}
			}
		}
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000B827E File Offset: 0x000B647E
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000B8288 File Offset: 0x000B6488
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000B8301 File Offset: 0x000B6501
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x000B8310 File Offset: 0x000B6510
	private void OnTriggerExit(Collider other)
	{
		if (this.onEnter)
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BuilderPieceCollider component = other.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if (this.table != null && this.table.builderNetworking != null)
			{
				if (piece == null)
				{
					return;
				}
				if (this.dropType == BuilderDropZone.DropType.Recycle)
				{
					bool flag = piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.OnShelf && piece.state > BuilderPiece.State.AttachedAndPlaced;
					if (!piece.isBuiltIntoTable && flag)
					{
						this.table.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, true, -1);
						return;
					}
				}
				else
				{
					this.table.builderNetworking.PieceEnteredDropZone(piece, this.dropType, this.dropZoneID);
				}
			}
		}
	}

	// Token: 0x04002DF5 RID: 11765
	[SerializeField]
	private BuilderDropZone.DropType dropType;

	// Token: 0x04002DF6 RID: 11766
	[SerializeField]
	private bool onEnter = true;

	// Token: 0x04002DF7 RID: 11767
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x04002DF8 RID: 11768
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x04002DF9 RID: 11769
	public float effectDuration = 1f;

	// Token: 0x04002DFA RID: 11770
	private bool playingEffect;

	// Token: 0x04002DFB RID: 11771
	public bool overrideDirection;

	// Token: 0x04002DFC RID: 11772
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x04002DFD RID: 11773
	private Vector3 repelDirectionWorld = Vector3.up;

	// Token: 0x04002DFE RID: 11774
	[HideInInspector]
	public int dropZoneID = -1;

	// Token: 0x04002DFF RID: 11775
	internal BuilderTable table;

	// Token: 0x02000573 RID: 1395
	public enum DropType
	{
		// Token: 0x04002E01 RID: 11777
		Invalid = -1,
		// Token: 0x04002E02 RID: 11778
		Repel,
		// Token: 0x04002E03 RID: 11779
		ReturnToShelf,
		// Token: 0x04002E04 RID: 11780
		BreakApart,
		// Token: 0x04002E05 RID: 11781
		Recycle
	}
}
