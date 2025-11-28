using System;
using System.Collections.Generic;
using System.IO;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class SICombinedTerminal : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x060006F5 RID: 1781 RVA: 0x000263BF File Offset: 0x000245BF
	public bool IsAuthority
	{
		get
		{
			return this.superInfection.siManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x060006F6 RID: 1782 RVA: 0x000263D6 File Offset: 0x000245D6
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.superInfection.siManager;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x060006F7 RID: 1783 RVA: 0x000263E3 File Offset: 0x000245E3
	public int ActivePage
	{
		get
		{
			return this._activePage;
		}
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x000263EC File Offset: 0x000245EC
	public void SliceUpdate()
	{
		this.wasOccupied = this.isOccupied;
		this.isOccupied = false;
		this.isOccupiedByActivePlayer = false;
		VRRigCache.Instance.GetActiveRigs(this.rigs);
		for (int i = 0; i < this.rigs.Count; i++)
		{
			if (this.activeUserBounds.bounds.Contains(this.rigs[i].transform.position))
			{
				this.isOccupied = true;
				if (this.rigs[i].OwningNetPlayer.IsLocal)
				{
					this.isOccupiedByActivePlayer = true;
					break;
				}
			}
		}
		if (this.isOccupied)
		{
			float num = Time.time - SIProgression.Instance.timeTelemetryLastChecked;
			if (this.activePlayer != null && this.activePlayer.ActorNr == SIPlayer.LocalPlayer.ActorNr && this.isOccupiedByLocalPlayer)
			{
				SIProgression.Instance.activeTerminalTimeInterval += num;
				SIProgression.Instance.activeTerminalTimeTotal += num;
			}
			if (!this.wasOccupied && this.state == EKioskAnimState.Closing)
			{
				this.AnimQueueState(EKioskAnimState.Opening);
			}
			this.foldupTimeStart = Time.time;
			return;
		}
		if (this.state == EKioskAnimState.Opening && Time.time > this.foldupTimeStart + this.foldupDelay && !this.isOccupied)
		{
			this.AnimQueueState(EKioskAnimState.Closing);
		}
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00026548 File Offset: 0x00024748
	public void Reset()
	{
		this.SetActivePage(0);
		this.dispenser.Initialize();
		this.techTree.Initialize();
		this.resourceCollection.Initialize();
		this.dispenser.Reset();
		this.techTree.Reset();
		this.resourceCollection.Reset();
		this.AnimQueueState(EKioskAnimState.Closing);
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x000265A8 File Offset: 0x000247A8
	public void Awake()
	{
		if (this.superInfection == null)
		{
			this.superInfection = base.GetComponentInParent<SuperInfection>();
		}
		this.dispenser.Initialize();
		this.techTree.Initialize();
		this.resourceCollection.Initialize();
		this.Reset();
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x000265F8 File Offset: 0x000247F8
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.activePlayer != null)
		{
			stream.SendNext(this.activePlayer.ActorNr);
		}
		else
		{
			stream.SendNext(-1);
		}
		stream.SendNext(this._activePage);
		this.dispenser.WriteDataPUN(stream, info);
		this.techTree.WriteDataPUN(stream, info);
		this.resourceCollection.WriteDataPUN(stream, info);
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00026670 File Offset: 0x00024870
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.activePlayer = SIPlayer.Get((int)stream.ReceiveNext());
		this._activePage = (int)stream.ReceiveNext();
		this.dispenser.ReadDataPUN(stream, info);
		this.techTree.ReadDataPUN(stream, info);
		this.resourceCollection.ReadDataPUN(stream, info);
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x000266CB File Offset: 0x000248CB
	public void SerializeZoneData(BinaryWriter writer)
	{
		writer.Write(this._activePage);
		this.dispenser.ZoneDataSerializeWrite(writer);
		this.techTree.ZoneDataSerializeWrite(writer);
		this.resourceCollection.ZoneDataSerializeWrite(writer);
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x000266FD File Offset: 0x000248FD
	public void DeserializeZoneData(BinaryReader reader)
	{
		this._activePage = reader.ReadInt32();
		this.SetActivePage(this._activePage);
		this.dispenser.ZoneDataSerializeRead(reader);
		this.techTree.ZoneDataSerializeRead(reader);
		this.resourceCollection.ZoneDataSerializeRead(reader);
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x0002673C File Offset: 0x0002493C
	public void PlayerHandScanned(int actorNr)
	{
		if (!this.IsAuthority)
		{
			this.superInfection.siManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CombinedTerminalHandScan, new object[]
			{
				this.index
			});
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(actorNr);
		if (this.activePlayer != null && this.activePlayer.isActiveAndEnabled && siplayer != this.activePlayer && this.activeUserBounds.bounds.Contains(this.activePlayer.transform.position))
		{
			return;
		}
		this.activePlayer = siplayer;
		this.dispenser.PlayerHandScanned(actorNr);
		this.techTree.PlayerHandScanned(actorNr);
		this.resourceCollection.PlayerHandScanned(actorNr);
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x000267F8 File Offset: 0x000249F8
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, SICombinedTerminal.TerminalSubFunction subFunction)
	{
		if (!this.IsAuthority)
		{
			this.SIManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CombinedTerminalButtonPress, new object[]
			{
				(int)buttonType,
				data,
				(int)subFunction,
				this.index
			});
			return;
		}
		switch (subFunction)
		{
		case SICombinedTerminal.TerminalSubFunction.TechTree:
			this.techTree.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		case SICombinedTerminal.TerminalSubFunction.GadgetDispenser:
			this.dispenser.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		case SICombinedTerminal.TerminalSubFunction.ResourceCollection:
			this.resourceCollection.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0002688A File Offset: 0x00024A8A
	public void SetActivePage(int pageId)
	{
		this._activePage = pageId;
		if (this.techTree.IsValidPage(pageId))
		{
			this.techTree.SetActivePage();
		}
		if (this.dispenser.IsValidPage(pageId))
		{
			this.dispenser.SetActivePage();
		}
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x000268C8 File Offset: 0x00024AC8
	private void AnimQueueState(EKioskAnimState newState)
	{
		this.state = newState;
		for (int i = 0; i < this.m_gtAnimators.Length; i++)
		{
			if (!(this.m_gtAnimators[i] == null))
			{
				this.m_gtAnimators[i].QueueState((long)newState);
			}
		}
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0002690E File Offset: 0x00024B0E
	public void PlayWrongPlayerBuzz(Transform xForm)
	{
		this.wrongPlayerBuzz.transform.position = xForm.position;
		this.wrongPlayerBuzz.PlayOneShot(this.wrongPlayerBuzz.clip);
	}

	// Token: 0x040008C8 RID: 2248
	[DebugReadout]
	internal int index;

	// Token: 0x040008C9 RID: 2249
	[DebugReadout]
	internal SIPlayer activePlayer;

	// Token: 0x040008CA RID: 2250
	[DebugReadout]
	internal bool isOccupiedByActivePlayer;

	// Token: 0x040008CB RID: 2251
	[DebugReadout]
	internal bool isOccupiedByLocalPlayer;

	// Token: 0x040008CC RID: 2252
	[DebugReadout]
	internal bool isOccupied;

	// Token: 0x040008CD RID: 2253
	[DebugReadout]
	internal bool wasOccupied;

	// Token: 0x040008CE RID: 2254
	[DebugReadout]
	internal SuperInfection superInfection;

	// Token: 0x040008CF RID: 2255
	public SIGadgetDispenser dispenser;

	// Token: 0x040008D0 RID: 2256
	public SITechTreeStation techTree;

	// Token: 0x040008D1 RID: 2257
	public SIResourceCollection resourceCollection;

	// Token: 0x040008D2 RID: 2258
	[SerializeField]
	private GTAnimator[] m_gtAnimators;

	// Token: 0x040008D3 RID: 2259
	public Collider activeUserBounds;

	// Token: 0x040008D4 RID: 2260
	public float foldupDelay = 20f;

	// Token: 0x040008D5 RID: 2261
	private float foldupTimeStart;

	// Token: 0x040008D6 RID: 2262
	private EKioskAnimState state;

	// Token: 0x040008D7 RID: 2263
	[DebugReadout]
	private int _activePage;

	// Token: 0x040008D8 RID: 2264
	[Header("Flattener")]
	public Transform zeroZeroImage;

	// Token: 0x040008D9 RID: 2265
	public Transform onePointTwoText;

	// Token: 0x040008DA RID: 2266
	private List<VRRig> rigs = new List<VRRig>();

	// Token: 0x040008DB RID: 2267
	public AudioSource wrongPlayerBuzz;

	// Token: 0x02000112 RID: 274
	public enum TerminalSubFunction
	{
		// Token: 0x040008DD RID: 2269
		TechTree,
		// Token: 0x040008DE RID: 2270
		GadgetDispenser,
		// Token: 0x040008DF RID: 2271
		ResourceCollection
	}
}
