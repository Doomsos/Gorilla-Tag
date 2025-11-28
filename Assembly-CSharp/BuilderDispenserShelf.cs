using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000571 RID: 1393
public class BuilderDispenserShelf : MonoBehaviour
{
	// Token: 0x0600231B RID: 8987 RVA: 0x000B795F File Offset: 0x000B5B5F
	private void BuildDispenserPool()
	{
		this.dispenserPool = new List<BuilderDispenser>(12);
		this.activeDispensers = new List<BuilderDispenser>(6);
		this.AddToDispenserPool(6);
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x000B7984 File Offset: 0x000B5B84
	private void AddToDispenserPool(int count)
	{
		if (this.dispenserPrefab == null)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			BuilderDispenser builderDispenser = Object.Instantiate<BuilderDispenser>(this.dispenserPrefab, this.shelfCenter);
			builderDispenser.gameObject.SetActive(false);
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
			this.dispenserPool.Add(builderDispenser);
		}
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x000B79F0 File Offset: 0x000B5BF0
	private void ActivateDispensers()
	{
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentGroup.pieceSubsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		int count = this.piecesInSet.Count;
		if (this.dispenserPool.Count < count)
		{
			this.AddToDispenserPool(count - this.dispenserPool.Count);
		}
		this.activeDispensers.Clear();
		for (int i = 0; i < this.dispenserPool.Count; i++)
		{
			if (i < count)
			{
				BuilderDispenser builderDispenser = this.dispenserPool[i];
				builderDispenser.gameObject.SetActive(true);
				float num = this.shelfWidth / -2f + this.shelfWidth / (float)(count * 2) + this.shelfWidth / (float)count * (float)i;
				builderDispenser.transform.localPosition = new Vector3(num, 0f, 0f);
				builderDispenser.AssignPieceType(this.piecesInSet[i], this.currentGroup.defaultMaterial.GetHashCode());
				this.activeDispensers.Add(builderDispenser);
			}
			else
			{
				this.dispenserPool[i].ClearDispenser();
				this.dispenserPool[i].gameObject.SetActive(false);
			}
		}
		this.dispenserToUpdate = 0;
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x000B7B9C File Offset: 0x000B5D9C
	public void Setup()
	{
		this.InitIfNeeded();
		foreach (BuilderDispenser builderDispenser in this.dispenserPool)
		{
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
		}
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x000B7C04 File Offset: 0x000B5E04
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.setSelector.Setup(this._includedCategories);
		this.currentGroup = this.setSelector.GetSelectedGroup();
		this.setSelector.OnSelectedGroup.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.BuildDispenserPool();
		this.ActivateDispensers();
		this.initialized = true;
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000B7C6B File Offset: 0x000B5E6B
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedGroup.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000B7C97 File Offset: 0x000B5E97
	public void OnSelectedSetChange(int displayGroupID)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, displayGroupID, false);
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000B7CBC File Offset: 0x000B5EBC
	public void SetSelection(int displayGroupID)
	{
		this.setSelector.SetSelection(displayGroupID);
		BuilderPieceSet.BuilderDisplayGroup selectedGroup = this.setSelector.GetSelectedGroup();
		if ((this.initialized && this.currentGroup == null) || selectedGroup.displayName != this.currentGroup.displayName)
		{
			this.currentGroup = selectedGroup;
			if (this.table.GetTableState() == BuilderTable.TableState.Ready)
			{
				if (!this.animatingShelf)
				{
					this.StartShelfSwap();
					return;
				}
			}
			else
			{
				this.animatingShelf = false;
				this.ImmediateShelfSwap();
			}
		}
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000B7D3A File Offset: 0x000B5F3A
	public int GetSelectedDisplayGroupID()
	{
		return this.setSelector.GetSelectedGroup().GetDisplayGroupIdentifier();
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x000B7D4C File Offset: 0x000B5F4C
	private void ImmediateShelfSwap()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
		this.ActivateDispensers();
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000B7DA4 File Offset: 0x000B5FA4
	private void StartShelfSwap()
	{
		this.dispenserToClear = 0;
		this.timeToClearShelf = (double)(Time.time + 0.15f);
		this.resetAnimation.Rewind();
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ParentPieceToShelf(this.resetAnimation.transform);
		}
		this.resetAnimation.Play();
		this.animatingShelf = true;
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000B7E38 File Offset: 0x000B6038
	public void UpdateShelf()
	{
		if (this.animatingShelf && (double)Time.time > this.timeToClearShelf)
		{
			if (this.dispenserToClear < this.activeDispensers.Count)
			{
				if (this.dispenserToClear == 0)
				{
					this.resetSoundBank.Play();
				}
				this.activeDispensers[this.dispenserToClear].ClearDispenser();
				this.dispenserToClear++;
				return;
			}
			if (!this.resetAnimation.isPlaying)
			{
				this.playSpawnSetSound = true;
				this.ActivateDispensers();
				this.animatingShelf = false;
			}
		}
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000B7EC8 File Offset: 0x000B60C8
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (!this.initialized)
		{
			return;
		}
		if (this.animatingShelf)
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
		if (this.activeDispensers.Count > 0)
		{
			this.activeDispensers[this.dispenserToUpdate].UpdateDispenser();
			this.dispenserToUpdate = (this.dispenserToUpdate + 1) % this.activeDispensers.Count;
		}
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000B7F7B File Offset: 0x000B617B
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x000B7F84 File Offset: 0x000B6184
	public void OnShelfPieceCreated(BuilderPiece piece, bool playfx)
	{
		if (this.playSpawnSetSound && playfx)
		{
			this.audioSource.GTPlayOneShot(this.spawnNewSetSound, 1f);
			this.playSpawnSetSound = false;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceCreated(piece, playfx);
		}
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000B8000 File Offset: 0x000B6200
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceRecycled(piece);
		}
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000B8054 File Offset: 0x000B6254
	public void OnClearTable()
	{
		if (!this.initialized)
		{
			return;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.OnClearTable();
		}
		base.StopAllCoroutines();
		if (this.animatingShelf)
		{
			this.resetAnimation.Rewind();
			this.animatingShelf = false;
		}
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000B80D0 File Offset: 0x000B62D0
	public void ClearShelf()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
	}

	// Token: 0x04002DDF RID: 11743
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x04002DE0 RID: 11744
	public List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x04002DE1 RID: 11745
	[Header("Dispenser Shelf Properties")]
	public Transform shelfCenter;

	// Token: 0x04002DE2 RID: 11746
	public float shelfWidth = 1.4f;

	// Token: 0x04002DE3 RID: 11747
	public Animation resetAnimation;

	// Token: 0x04002DE4 RID: 11748
	[SerializeField]
	private SoundBankPlayer resetSoundBank;

	// Token: 0x04002DE5 RID: 11749
	[SerializeField]
	private AudioClip spawnNewSetSound;

	// Token: 0x04002DE6 RID: 11750
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002DE7 RID: 11751
	private bool playSpawnSetSound;

	// Token: 0x04002DE8 RID: 11752
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x04002DE9 RID: 11753
	public int shelfID = -1;

	// Token: 0x04002DEA RID: 11754
	private BuilderPieceSet.BuilderDisplayGroup currentGroup;

	// Token: 0x04002DEB RID: 11755
	private bool initialized;

	// Token: 0x04002DEC RID: 11756
	public BuilderDispenser dispenserPrefab;

	// Token: 0x04002DED RID: 11757
	private List<BuilderDispenser> dispenserPool;

	// Token: 0x04002DEE RID: 11758
	private List<BuilderDispenser> activeDispensers;

	// Token: 0x04002DEF RID: 11759
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x04002DF0 RID: 11760
	private bool animatingShelf;

	// Token: 0x04002DF1 RID: 11761
	private double timeToClearShelf = double.MaxValue;

	// Token: 0x04002DF2 RID: 11762
	private int dispenserToClear;

	// Token: 0x04002DF3 RID: 11763
	private int dispenserToUpdate;

	// Token: 0x04002DF4 RID: 11764
	private bool shouldVerifySetSelection;
}
