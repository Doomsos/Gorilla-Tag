using System;
using System.Collections.Generic;
using Fusion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000454 RID: 1108
public class VRRigReliableState : MonoBehaviour, IWrappedSerializable, INetworkStruct
{
	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06001C2D RID: 7213 RVA: 0x000957D2 File Offset: 0x000939D2
	public bool HasBracelet
	{
		get
		{
			return this.braceletBeadColors.Count > 0;
		}
	}

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06001C2E RID: 7214 RVA: 0x000957E2 File Offset: 0x000939E2
	// (set) Token: 0x06001C2F RID: 7215 RVA: 0x000957EA File Offset: 0x000939EA
	public bool isDirty { get; private set; } = true;

	// Token: 0x06001C30 RID: 7216 RVA: 0x000957F3 File Offset: 0x000939F3
	private void Awake()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Combine(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
		RoomSystem.JoinedRoomEvent += new Action(this.SetIsDirty);
	}

	// Token: 0x06001C31 RID: 7217 RVA: 0x00095830 File Offset: 0x00093A30
	private void OnDestroy()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Remove(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x00095852 File Offset: 0x00093A52
	public void SetIsDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x0009585B File Offset: 0x00093A5B
	public void SetIsNotDirty()
	{
		this.isDirty = false;
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x00095864 File Offset: 0x00093A64
	public void SharedStart(bool isOfflineVRRig_, BodyDockPositions bDock_)
	{
		this.isOfflineVRRig = isOfflineVRRig_;
		this.bDock = bDock_;
		this.activeTransferrableObjectIndex = new int[5];
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			this.activeTransferrableObjectIndex[i] = -1;
		}
		this.transferrablePosStates = new TransferrableObject.PositionState[5];
		this.transferrableItemStates = new TransferrableObject.ItemStates[5];
		this.transferableDockPositions = new BodyDockPositions.DropPositions[5];
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x000958CC File Offset: 0x00093ACC
	void IWrappedSerializable.OnSerializeRead(object newData)
	{
		this.Data = (ReliableStateData)newData;
		long header = this.Data.Header;
		int num;
		this.SetHeader(header, out num);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & 1L << (i & 31)) != 0L)
			{
				long num2 = this.Data.TransferrableStates[i];
				this.activeTransferrableObjectIndex[i] = (int)num2;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)(num2 >> 32 & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)(num2 >> 40 & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)(num2 >> 48 & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = this.Data.WearablesPackedState;
		this.lThrowableProjectileIndex = this.Data.LThrowableProjectileIndex;
		this.rThrowableProjectileIndex = this.Data.RThrowableProjectileIndex;
		this.sizeLayerMask = this.Data.SizeLayerMask;
		this.randomThrowableIndex = this.Data.RandomThrowableIndex;
		this.braceletBeadColors.Clear();
		if (num > 0)
		{
			if (num <= 3)
			{
				int num3 = (int)this.Data.PackedBeads;
				this.braceletSelfIndex = num3 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num3, 0, num, this.braceletBeadColors);
			}
			else
			{
				long packedBeads = this.Data.PackedBeads;
				this.braceletSelfIndex = (int)(packedBeads >> 60);
				if (num <= 6)
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, num, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors(this.Data.PackedBeadsMoreThan6, 6, num, this.braceletBeadColors);
				}
			}
		}
		this.bDock.RefreshTransferrableItems();
		this.bDock.myRig.UpdateFriendshipBracelet();
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x00095AA8 File Offset: 0x00093CA8
	object IWrappedSerializable.OnSerializeWrite()
	{
		this.isDirty = false;
		ReliableStateData reliableStateData = default(ReliableStateData);
		long header = this.GetHeader();
		reliableStateData.Header = header;
		long[] array = this.GetTransferrableStates(header).ToArray();
		reliableStateData.TransferrableStates.CopyFrom(array, 0, array.Length);
		reliableStateData.WearablesPackedState = this.wearablesPackedStates;
		reliableStateData.LThrowableProjectileIndex = this.lThrowableProjectileIndex;
		reliableStateData.RThrowableProjectileIndex = this.rThrowableProjectileIndex;
		reliableStateData.SizeLayerMask = this.sizeLayerMask;
		reliableStateData.RandomThrowableIndex = this.randomThrowableIndex;
		if (this.braceletBeadColors.Count > 0)
		{
			long num = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num |= (long)this.braceletSelfIndex << 30;
				reliableStateData.PackedBeads = num;
			}
			else
			{
				num |= (long)this.braceletSelfIndex << 60;
				reliableStateData.PackedBeads = num;
				if (this.braceletBeadColors.Count > 6)
				{
					reliableStateData.PackedBeadsMoreThan6 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6);
				}
			}
		}
		this.Data = reliableStateData;
		return reliableStateData;
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x00095BC0 File Offset: 0x00093DC0
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		long header = this.GetHeader();
		stream.SendNext(header);
		foreach (long num in this.GetTransferrableStates(header))
		{
			stream.SendNext(num);
		}
		stream.SendNext(this.wearablesPackedStates);
		stream.SendNext(this.lThrowableProjectileIndex);
		stream.SendNext(this.rThrowableProjectileIndex);
		stream.SendNext(this.sizeLayerMask);
		stream.SendNext(this.randomThrowableIndex);
		if (this.braceletBeadColors.Count > 0)
		{
			long num2 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num2 |= (long)this.braceletSelfIndex << 30;
				stream.SendNext((int)num2);
				return;
			}
			num2 |= (long)this.braceletSelfIndex << 60;
			stream.SendNext(num2);
			if (this.braceletBeadColors.Count > 6)
			{
				stream.SendNext(VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6));
			}
		}
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x00095D14 File Offset: 0x00093F14
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		long num = (long)stream.ReceiveNext();
		this.isMicEnabled = ((num & 32L) != 0L);
		this.isBraceletLeftHanded = ((num & 64L) != 0L);
		this.isBuilderWatchEnabled = ((num & 128L) != 0L);
		int num2 = (int)(num >> 12) & 15;
		this.lThrowableProjectileColor.r = (byte)(num >> 16);
		this.lThrowableProjectileColor.g = (byte)(num >> 24);
		this.lThrowableProjectileColor.b = (byte)(num >> 32);
		this.rThrowableProjectileColor.r = (byte)(num >> 40);
		this.rThrowableProjectileColor.g = (byte)(num >> 48);
		this.rThrowableProjectileColor.b = (byte)(num >> 56);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((num & 1L << (i & 31)) != 0L)
			{
				long num3 = (long)stream.ReceiveNext();
				this.activeTransferrableObjectIndex[i] = (int)num3;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)(num3 >> 32 & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)(num3 >> 40 & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)(num3 >> 48 & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = (int)stream.ReceiveNext();
		this.lThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.rThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.sizeLayerMask = (int)stream.ReceiveNext();
		this.randomThrowableIndex = (int)stream.ReceiveNext();
		this.braceletBeadColors.Clear();
		if (num2 > 0)
		{
			if (num2 <= 3)
			{
				int num4 = (int)stream.ReceiveNext();
				this.braceletSelfIndex = num4 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num4, 0, num2, this.braceletBeadColors);
			}
			else
			{
				long num5 = (long)stream.ReceiveNext();
				this.braceletSelfIndex = (int)(num5 >> 60);
				if (num2 <= 6)
				{
					VRRigReliableState.UnpackBeadColors(num5, 0, num2, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(num5, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors((long)stream.ReceiveNext(), 6, num2, this.braceletBeadColors);
				}
			}
		}
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.bDock.RefreshTransferrableItems();
		}
		this.bDock.myRig.UpdateFriendshipBracelet();
		this.bDock.myRig.EnableBuilderResizeWatch(this.isBuilderWatchEnabled);
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x00095F84 File Offset: 0x00094184
	private long GetHeader()
	{
		long num = 0L;
		if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
			{
				if (this.activeTransferrableObjectIndex[i] != -1 && (this.transferrablePosStates[i] == TransferrableObject.PositionState.InLeftHand || this.transferrablePosStates[i] == TransferrableObject.PositionState.InRightHand))
				{
					num |= (long)((ulong)((byte)(1 << i)));
				}
			}
		}
		else
		{
			for (int j = 0; j < this.activeTransferrableObjectIndex.Length; j++)
			{
				if (this.activeTransferrableObjectIndex[j] != -1)
				{
					num |= (long)((ulong)((byte)(1 << j)));
				}
			}
		}
		if (this.isBraceletLeftHanded)
		{
			num |= 64L;
		}
		if (this.isMicEnabled)
		{
			num |= 32L;
		}
		if (this.isBuilderWatchEnabled && !CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			num |= 128L;
		}
		num |= ((long)this.braceletBeadColors.Count & 15L) << 12;
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.r) << 16);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.g) << 24);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.b) << 32);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.r) << 40);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.g) << 48);
		return num | (long)((long)((ulong)this.rThrowableProjectileColor.b) << 56);
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x000960CC File Offset: 0x000942CC
	private void SetHeader(long header, out int numBeadsToRead)
	{
		this.isMicEnabled = ((header & 32L) != 0L);
		this.isBraceletLeftHanded = ((header & 64L) != 0L);
		numBeadsToRead = ((int)(header >> 12) & 15);
		this.lThrowableProjectileColor.r = (byte)(header >> 16);
		this.lThrowableProjectileColor.g = (byte)(header >> 24);
		this.lThrowableProjectileColor.b = (byte)(header >> 32);
		this.rThrowableProjectileColor.r = (byte)(header >> 40);
		this.rThrowableProjectileColor.g = (byte)(header >> 48);
		this.rThrowableProjectileColor.b = (byte)(header >> 56);
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x00096164 File Offset: 0x00094364
	private List<long> GetTransferrableStates(long header)
	{
		List<long> list = new List<long>();
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & 1L << (i & 31)) != 0L && this.activeTransferrableObjectIndex[i] != -1)
			{
				long num = (long)((ulong)this.activeTransferrableObjectIndex[i]);
				num |= (long)this.transferrablePosStates[i] << 32;
				num |= (long)this.transferrableItemStates[i] << 40;
				num |= (long)this.transferableDockPositions[i] << 48;
				list.Add(num);
			}
		}
		return list;
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x000961E0 File Offset: 0x000943E0
	private static long PackBeadColors(List<Color> beadColors, int fromIndex)
	{
		long num = 0L;
		int num2 = Mathf.Min(fromIndex + 6, beadColors.Count);
		int num3 = 0;
		for (int i = fromIndex; i < num2; i++)
		{
			long num4 = (long)FriendshipGroupDetection.PackColor(beadColors[i]);
			num |= num4 << num3;
			num3 += 10;
		}
		return num;
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x0009622C File Offset: 0x0009442C
	private static void UnpackBeadColors(long packed, int startIndex, int endIndex, List<Color> beadColorsResult)
	{
		int num = Mathf.Min(startIndex + 6, endIndex);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			short data = (short)(packed >> num2 & 1023L);
			beadColorsResult.Add(FriendshipGroupDetection.UnpackColor(data));
			num2 += 10;
		}
	}

	// Token: 0x04002627 RID: 9767
	[NonSerialized]
	public int[] activeTransferrableObjectIndex;

	// Token: 0x04002628 RID: 9768
	[NonSerialized]
	public TransferrableObject.PositionState[] transferrablePosStates;

	// Token: 0x04002629 RID: 9769
	[NonSerialized]
	public TransferrableObject.ItemStates[] transferrableItemStates;

	// Token: 0x0400262A RID: 9770
	[NonSerialized]
	public BodyDockPositions.DropPositions[] transferableDockPositions;

	// Token: 0x0400262B RID: 9771
	[NonSerialized]
	public int wearablesPackedStates;

	// Token: 0x0400262C RID: 9772
	[NonSerialized]
	public int lThrowableProjectileIndex = -1;

	// Token: 0x0400262D RID: 9773
	[NonSerialized]
	public int rThrowableProjectileIndex = -1;

	// Token: 0x0400262E RID: 9774
	[NonSerialized]
	public Color32 lThrowableProjectileColor = Color.white;

	// Token: 0x0400262F RID: 9775
	[NonSerialized]
	public Color32 rThrowableProjectileColor = Color.white;

	// Token: 0x04002630 RID: 9776
	[NonSerialized]
	public int randomThrowableIndex;

	// Token: 0x04002631 RID: 9777
	[NonSerialized]
	public bool isMicEnabled;

	// Token: 0x04002632 RID: 9778
	private bool isOfflineVRRig;

	// Token: 0x04002633 RID: 9779
	private BodyDockPositions bDock;

	// Token: 0x04002634 RID: 9780
	[NonSerialized]
	public int sizeLayerMask = 1;

	// Token: 0x04002635 RID: 9781
	private const long IS_MIC_ENABLED_BIT = 32L;

	// Token: 0x04002636 RID: 9782
	private const long BRACELET_LEFTHAND_BIT = 64L;

	// Token: 0x04002637 RID: 9783
	private const long BUILDER_WATCH_ENABLED_BIT = 128L;

	// Token: 0x04002638 RID: 9784
	private const int BRACELET_NUM_BEADS_SHIFT = 12;

	// Token: 0x04002639 RID: 9785
	private const int LPROJECTILECOLOR_R_SHIFT = 16;

	// Token: 0x0400263A RID: 9786
	private const int LPROJECTILECOLOR_G_SHIFT = 24;

	// Token: 0x0400263B RID: 9787
	private const int LPROJECTILECOLOR_B_SHIFT = 32;

	// Token: 0x0400263C RID: 9788
	private const int RPROJECTILECOLOR_R_SHIFT = 40;

	// Token: 0x0400263D RID: 9789
	private const int RPROJECTILECOLOR_G_SHIFT = 48;

	// Token: 0x0400263E RID: 9790
	private const int RPROJECTILECOLOR_B_SHIFT = 56;

	// Token: 0x0400263F RID: 9791
	private const int POS_STATES_SHIFT = 32;

	// Token: 0x04002640 RID: 9792
	private const int ITEM_STATES_SHIFT = 40;

	// Token: 0x04002641 RID: 9793
	private const int DOCK_POSITIONS_SHIFT = 48;

	// Token: 0x04002642 RID: 9794
	private const int BRACELET_SELF_INDEX_SHIFT = 60;

	// Token: 0x04002643 RID: 9795
	[NonSerialized]
	public bool isBraceletLeftHanded;

	// Token: 0x04002644 RID: 9796
	[NonSerialized]
	public int braceletSelfIndex;

	// Token: 0x04002645 RID: 9797
	[NonSerialized]
	public List<Color> braceletBeadColors = new List<Color>(10);

	// Token: 0x04002646 RID: 9798
	[NonSerialized]
	public bool isBuilderWatchEnabled;

	// Token: 0x04002648 RID: 9800
	private ReliableStateData Data;
}
