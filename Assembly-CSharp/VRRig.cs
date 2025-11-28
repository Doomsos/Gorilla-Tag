using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using TagEffects;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000448 RID: 1096
public class VRRig : MonoBehaviour, IWrappedSerializable, INetworkStruct, IPreDisable, IUserCosmeticsCallback, IGorillaSliceableSimple, ITickSystemPost, IEyeScannable
{
	// Token: 0x06001AE4 RID: 6884 RVA: 0x0008DC64 File Offset: 0x0008BE64
	private void CosmeticsV2_Awake()
	{
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
			return;
		}
		if (!this._isListeningFor_OnPostInstantiateAllPrefabs)
		{
			this._isListeningFor_OnPostInstantiateAllPrefabs = true;
			CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
		}
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x0008DCA3 File Offset: 0x0008BEA3
	private void CosmeticsV2_OnDestroy()
	{
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
			return;
		}
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x0008DCD3 File Offset: 0x0008BED3
	internal void Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
		this.CheckForEarlyAccess();
		this.BuildInitialize_AfterCosmeticsV2Instantiated();
		this.SetCosmeticsActive(false);
	}

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06001AE7 RID: 6887 RVA: 0x0008DD08 File Offset: 0x0008BF08
	// (set) Token: 0x06001AE8 RID: 6888 RVA: 0x0008DD15 File Offset: 0x0008BF15
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x06001AE9 RID: 6889 RVA: 0x0008DD23 File Offset: 0x0008BF23
	public Material myDefaultSkinMaterialInstance
	{
		get
		{
			return this.bodyRenderer.myDefaultSkinMaterialInstance;
		}
	}

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06001AEA RID: 6890 RVA: 0x0008DD30 File Offset: 0x0008BF30
	// (set) Token: 0x06001AEB RID: 6891 RVA: 0x0008DD38 File Offset: 0x0008BF38
	public GameObject[] cosmetics
	{
		get
		{
			return this._cosmetics;
		}
		set
		{
			this._cosmetics = value;
		}
	}

	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06001AEC RID: 6892 RVA: 0x0008DD41 File Offset: 0x0008BF41
	// (set) Token: 0x06001AED RID: 6893 RVA: 0x0008DD49 File Offset: 0x0008BF49
	public GameObject[] overrideCosmetics
	{
		get
		{
			return this._overrideCosmetics;
		}
		set
		{
			this._overrideCosmetics = value;
		}
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x0008DD52 File Offset: 0x0008BF52
	internal void SetTaggedBy(VRRig taggingRig)
	{
		this.taggedById = taggingRig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06001AEF RID: 6895 RVA: 0x0008DD65 File Offset: 0x0008BF65
	public HashSet<string> TemporaryCosmetics
	{
		get
		{
			return this._temporaryCosmetics;
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06001AF0 RID: 6896 RVA: 0x0008DD6D File Offset: 0x0008BF6D
	// (set) Token: 0x06001AF1 RID: 6897 RVA: 0x0008DD75 File Offset: 0x0008BF75
	internal bool InitializedCosmetics
	{
		get
		{
			return this.initializedCosmetics;
		}
		set
		{
			this.initializedCosmetics = value;
		}
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x06001AF2 RID: 6898 RVA: 0x0008DD7E File Offset: 0x0008BF7E
	// (set) Token: 0x06001AF3 RID: 6899 RVA: 0x0008DD86 File Offset: 0x0008BF86
	public CosmeticRefRegistry cosmeticReferences { get; private set; }

	// Token: 0x06001AF4 RID: 6900 RVA: 0x0008DD8F File Offset: 0x0008BF8F
	public void SetPitchShiftCosmeticsDirty()
	{
		this.pitchShiftCosmeticsDirty = true;
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x0008DD98 File Offset: 0x0008BF98
	public void BreakHandLinks()
	{
		this.leftHandLink.BreakLink();
		this.rightHandLink.BreakLink();
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x0008DDB0 File Offset: 0x0008BFB0
	public bool IsInHandHoldChainWithOtherPlayer(int otherPlayer)
	{
		return HandLink.IsHandInChainWithOtherPlayer(this.leftHandLink, otherPlayer) || HandLink.IsHandInChainWithOtherPlayer(this.rightHandLink, otherPlayer);
	}

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06001AF7 RID: 6903 RVA: 0x0008DDCE File Offset: 0x0008BFCE
	// (set) Token: 0x06001AF8 RID: 6904 RVA: 0x0008DDD6 File Offset: 0x0008BFD6
	public float LastTouchedGroundAtNetworkTime { get; private set; }

	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06001AF9 RID: 6905 RVA: 0x0008DDDF File Offset: 0x0008BFDF
	// (set) Token: 0x06001AFA RID: 6906 RVA: 0x0008DDE7 File Offset: 0x0008BFE7
	public float LastHandTouchedGroundAtNetworkTime { get; private set; }

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06001AFB RID: 6907 RVA: 0x0008DDF0 File Offset: 0x0008BFF0
	public bool HasBracelet
	{
		get
		{
			return this.reliableState.HasBracelet;
		}
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x0008DDFD File Offset: 0x0008BFFD
	public Vector3 GetMouthPosition()
	{
		return this.MouthPosition.position;
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06001AFD RID: 6909 RVA: 0x0008DE0A File Offset: 0x0008C00A
	// (set) Token: 0x06001AFE RID: 6910 RVA: 0x0008DE12 File Offset: 0x0008C012
	public GorillaSkin CurrentCosmeticSkin { get; set; }

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06001AFF RID: 6911 RVA: 0x0008DE1B File Offset: 0x0008C01B
	// (set) Token: 0x06001B00 RID: 6912 RVA: 0x0008DE23 File Offset: 0x0008C023
	public GorillaSkin CurrentModeSkin { get; set; }

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06001B01 RID: 6913 RVA: 0x0008DE2C File Offset: 0x0008C02C
	// (set) Token: 0x06001B02 RID: 6914 RVA: 0x0008DE34 File Offset: 0x0008C034
	public GorillaSkin TemporaryEffectSkin { get; set; }

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06001B03 RID: 6915 RVA: 0x0008DE3D File Offset: 0x0008C03D
	// (set) Token: 0x06001B04 RID: 6916 RVA: 0x0008DE45 File Offset: 0x0008C045
	public bool PostTickRunning { get; set; }

	// Token: 0x06001B05 RID: 6917 RVA: 0x0008DE4E File Offset: 0x0008C04E
	public VRRig.PartyMemberStatus GetPartyMemberStatus()
	{
		if (this.partyMemberStatus == VRRig.PartyMemberStatus.NeedsUpdate)
		{
			this.partyMemberStatus = (FriendshipGroupDetection.Instance.IsInMyGroup(this.creator.UserId) ? VRRig.PartyMemberStatus.InLocalParty : VRRig.PartyMemberStatus.NotInLocalParty);
		}
		return this.partyMemberStatus;
	}

	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06001B06 RID: 6918 RVA: 0x0008DE7F File Offset: 0x0008C07F
	public bool IsLocalPartyMember
	{
		get
		{
			return this.GetPartyMemberStatus() != VRRig.PartyMemberStatus.NotInLocalParty;
		}
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x0008DE8D File Offset: 0x0008C08D
	public void ClearPartyMemberStatus()
	{
		this.partyMemberStatus = VRRig.PartyMemberStatus.NeedsUpdate;
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x0008DE96 File Offset: 0x0008C096
	public int ActiveTransferrableObjectIndex(int idx)
	{
		return this.reliableState.activeTransferrableObjectIndex[idx];
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x0008DEA5 File Offset: 0x0008C0A5
	public int ActiveTransferrableObjectIndexLength()
	{
		return this.reliableState.activeTransferrableObjectIndex.Length;
	}

	// Token: 0x06001B0A RID: 6922 RVA: 0x0008DEB4 File Offset: 0x0008C0B4
	public void SetActiveTransferrableObjectIndex(int idx, int v)
	{
		if (this.reliableState.activeTransferrableObjectIndex[idx] != v)
		{
			this.reliableState.activeTransferrableObjectIndex[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001B0B RID: 6923 RVA: 0x0008DEDF File Offset: 0x0008C0DF
	public TransferrableObject.PositionState TransferrablePosStates(int idx)
	{
		return this.reliableState.transferrablePosStates[idx];
	}

	// Token: 0x06001B0C RID: 6924 RVA: 0x0008DEEE File Offset: 0x0008C0EE
	public void SetTransferrablePosStates(int idx, TransferrableObject.PositionState v)
	{
		if (this.reliableState.transferrablePosStates[idx] != v)
		{
			this.reliableState.transferrablePosStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x0008DF19 File Offset: 0x0008C119
	public TransferrableObject.ItemStates TransferrableItemStates(int idx)
	{
		return this.reliableState.transferrableItemStates[idx];
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x0008DF28 File Offset: 0x0008C128
	public void SetTransferrableItemStates(int idx, TransferrableObject.ItemStates v)
	{
		if (this.reliableState.transferrableItemStates[idx] != v)
		{
			this.reliableState.transferrableItemStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x0008DF53 File Offset: 0x0008C153
	public void SetTransferrableDockPosition(int idx, BodyDockPositions.DropPositions v)
	{
		if (this.reliableState.transferableDockPositions[idx] != v)
		{
			this.reliableState.transferableDockPositions[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x0008DF7E File Offset: 0x0008C17E
	public BodyDockPositions.DropPositions TransferrableDockPosition(int idx)
	{
		return this.reliableState.transferableDockPositions[idx];
	}

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06001B11 RID: 6929 RVA: 0x0008DF8D File Offset: 0x0008C18D
	// (set) Token: 0x06001B12 RID: 6930 RVA: 0x0008DF9A File Offset: 0x0008C19A
	public int WearablePackedStates
	{
		get
		{
			return this.reliableState.wearablesPackedStates;
		}
		set
		{
			if (this.reliableState.wearablesPackedStates != value)
			{
				this.reliableState.wearablesPackedStates = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06001B13 RID: 6931 RVA: 0x0008DFC1 File Offset: 0x0008C1C1
	// (set) Token: 0x06001B14 RID: 6932 RVA: 0x0008DFCE File Offset: 0x0008C1CE
	public int LeftThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.lThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.lThrowableProjectileIndex != value)
			{
				this.reliableState.lThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06001B15 RID: 6933 RVA: 0x0008DFF5 File Offset: 0x0008C1F5
	// (set) Token: 0x06001B16 RID: 6934 RVA: 0x0008E002 File Offset: 0x0008C202
	public int RightThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.rThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.rThrowableProjectileIndex != value)
			{
				this.reliableState.rThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06001B17 RID: 6935 RVA: 0x0008E029 File Offset: 0x0008C229
	// (set) Token: 0x06001B18 RID: 6936 RVA: 0x0008E036 File Offset: 0x0008C236
	public Color32 LeftThrowableProjectileColor
	{
		get
		{
			return this.reliableState.lThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.lThrowableProjectileColor.Equals(value))
			{
				this.reliableState.lThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06001B19 RID: 6937 RVA: 0x0008E062 File Offset: 0x0008C262
	// (set) Token: 0x06001B1A RID: 6938 RVA: 0x0008E06F File Offset: 0x0008C26F
	public Color32 RightThrowableProjectileColor
	{
		get
		{
			return this.reliableState.rThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.rThrowableProjectileColor.Equals(value))
			{
				this.reliableState.rThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x06001B1B RID: 6939 RVA: 0x0008E09B File Offset: 0x0008C29B
	public Color32 GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightThrowableProjectileColor;
		}
		return this.LeftThrowableProjectileColor;
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x0008E0AD File Offset: 0x0008C2AD
	public void SetThrowableProjectileColor(bool isLeftHand, Color32 color)
	{
		if (isLeftHand)
		{
			this.LeftThrowableProjectileColor = color;
			return;
		}
		this.RightThrowableProjectileColor = color;
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x0008E0C1 File Offset: 0x0008C2C1
	public void SetRandomThrowableModelIndex(int randModelIndex)
	{
		this.RandomThrowableIndex = randModelIndex;
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x0008E0CA File Offset: 0x0008C2CA
	public int GetRandomThrowableModelIndex()
	{
		return this.RandomThrowableIndex;
	}

	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06001B1F RID: 6943 RVA: 0x0008E0D2 File Offset: 0x0008C2D2
	// (set) Token: 0x06001B20 RID: 6944 RVA: 0x0008E0DF File Offset: 0x0008C2DF
	private int RandomThrowableIndex
	{
		get
		{
			return this.reliableState.randomThrowableIndex;
		}
		set
		{
			if (this.reliableState.randomThrowableIndex != value)
			{
				this.reliableState.randomThrowableIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06001B21 RID: 6945 RVA: 0x0008E106 File Offset: 0x0008C306
	// (set) Token: 0x06001B22 RID: 6946 RVA: 0x0008E113 File Offset: 0x0008C313
	public bool IsMicEnabled
	{
		get
		{
			return this.reliableState.isMicEnabled;
		}
		set
		{
			if (this.reliableState.isMicEnabled != value)
			{
				this.reliableState.isMicEnabled = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06001B23 RID: 6947 RVA: 0x0008E13A File Offset: 0x0008C33A
	// (set) Token: 0x06001B24 RID: 6948 RVA: 0x0008E147 File Offset: 0x0008C347
	public int SizeLayerMask
	{
		get
		{
			return this.reliableState.sizeLayerMask;
		}
		set
		{
			if (this.reliableState.sizeLayerMask != value)
			{
				this.reliableState.sizeLayerMask = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06001B25 RID: 6949 RVA: 0x0008E16E File Offset: 0x0008C36E
	public float scaleFactor
	{
		get
		{
			return this.scaleMultiplier * this.nativeScale;
		}
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06001B26 RID: 6950 RVA: 0x0008E17D File Offset: 0x0008C37D
	// (set) Token: 0x06001B27 RID: 6951 RVA: 0x0008E185 File Offset: 0x0008C385
	public float ScaleMultiplier
	{
		get
		{
			return this.scaleMultiplier;
		}
		set
		{
			this.scaleMultiplier = value;
		}
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001B28 RID: 6952 RVA: 0x0008E18E File Offset: 0x0008C38E
	// (set) Token: 0x06001B29 RID: 6953 RVA: 0x0008E196 File Offset: 0x0008C396
	public float NativeScale
	{
		get
		{
			return this.nativeScale;
		}
		set
		{
			this.nativeScale = value;
		}
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001B2A RID: 6954 RVA: 0x0008E19F File Offset: 0x0008C39F
	public NetPlayer Creator
	{
		get
		{
			return this.creator;
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001B2B RID: 6955 RVA: 0x0008E1A7 File Offset: 0x0008C3A7
	internal bool Initialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001B2C RID: 6956 RVA: 0x0008E1AF File Offset: 0x0008C3AF
	// (set) Token: 0x06001B2D RID: 6957 RVA: 0x0008E1B7 File Offset: 0x0008C3B7
	public float SpeakingLoudness
	{
		get
		{
			return this.speakingLoudness;
		}
		set
		{
			this.speakingLoudness = value;
		}
	}

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001B2E RID: 6958 RVA: 0x0008E1C0 File Offset: 0x0008C3C0
	internal HandEffectContext LeftHandEffect
	{
		get
		{
			return this._leftHandEffect;
		}
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06001B2F RID: 6959 RVA: 0x0008E1C8 File Offset: 0x0008C3C8
	internal HandEffectContext RightHandEffect
	{
		get
		{
			return this._rightHandEffect;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06001B30 RID: 6960 RVA: 0x0008E1D0 File Offset: 0x0008C3D0
	internal HandEffectContext ExtraLeftHandEffect
	{
		get
		{
			return this._extraLeftHandEffect;
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06001B31 RID: 6961 RVA: 0x0008E1D8 File Offset: 0x0008C3D8
	internal HandEffectContext ExtraRightHandEffect
	{
		get
		{
			return this._extraRightHandEffect;
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06001B32 RID: 6962 RVA: 0x0008E1E0 File Offset: 0x0008C3E0
	public bool RigBuildFullyInitialized
	{
		get
		{
			return this._rigBuildFullyInitialized;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06001B33 RID: 6963 RVA: 0x0008E1E8 File Offset: 0x0008C3E8
	public GamePlayer GamePlayerRef
	{
		get
		{
			if (this._gamePlayerRef == null)
			{
				this._gamePlayerRef = base.GetComponent<GamePlayer>();
			}
			return this._gamePlayerRef;
		}
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x0008E20C File Offset: 0x0008C40C
	public void BuildInitialize()
	{
		this.fxSettings = Object.Instantiate<FXSystemSettings>(this.sharedFXSettings);
		this.fxSettings.forLocalRig = this.isOfflineVRRig;
		this.lastPosition = base.transform.position;
		if (!this.isOfflineVRRig)
		{
			base.transform.parent = null;
		}
		SizeManager component = base.GetComponent<SizeManager>();
		if (component != null)
		{
			component.BuildInitialize();
		}
		this.myMouthFlap = base.GetComponent<GorillaMouthFlap>();
		this.mySpeakerLoudness = base.GetComponent<GorillaSpeakerLoudness>();
		if (this.myReplacementVoice == null)
		{
			this.myReplacementVoice = base.GetComponentInChildren<ReplacementVoice>();
		}
		this.myEyeExpressions = base.GetComponent<GorillaEyeExpressions>();
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x0008E2B0 File Offset: 0x0008C4B0
	public void BuildInitialize_AfterCosmeticsV2Instantiated()
	{
		if (!this._rigBuildFullyInitialized)
		{
			Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
			GameObject gameObject2;
			foreach (GameObject gameObject in this.cosmetics)
			{
				if (!dictionary.TryGetValue(gameObject.name, ref gameObject2))
				{
					dictionary.Add(gameObject.name, gameObject);
				}
			}
			foreach (GameObject gameObject3 in this.overrideCosmetics)
			{
				if (dictionary.TryGetValue(gameObject3.name, ref gameObject2) && gameObject2.name == gameObject3.name)
				{
					gameObject2.name = "OVERRIDDEN";
				}
			}
			this.cosmetics = Enumerable.ToArray<GameObject>(Enumerable.Concat<GameObject>(this.cosmetics, this.overrideCosmetics));
		}
		this.cosmeticsObjectRegistry.Initialize(this.cosmetics);
		this._rigBuildFullyInitialized = true;
	}

	// Token: 0x06001B36 RID: 6966 RVA: 0x0008E388 File Offset: 0x0008C588
	private void Awake()
	{
		this.CosmeticsV2_Awake();
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.UpdateName));
		if (this.isOfflineVRRig)
		{
			VRRig.gLocalRig = this;
			this.BuildInitialize();
		}
		this.SharedStart();
	}

	// Token: 0x06001B37 RID: 6967 RVA: 0x0008E3E0 File Offset: 0x0008C5E0
	private void ApplyColorCode()
	{
		float num = 0f;
		float @float = PlayerPrefs.GetFloat("redValue", num);
		float float2 = PlayerPrefs.GetFloat("greenValue", num);
		float float3 = PlayerPrefs.GetFloat("blueValue", num);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
	}

	// Token: 0x06001B38 RID: 6968 RVA: 0x0008E424 File Offset: 0x0008C624
	private void SharedStart()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.lastScaleFactor = this.scaleFactor;
		this.isInitialized = true;
		this.myBodyDockPositions = base.GetComponent<BodyDockPositions>();
		this.reliableState.SharedStart(this.isOfflineVRRig, this.myBodyDockPositions);
		this.concatStringOfCosmeticsAllowed = "";
		this.bodyRenderer.SharedStart();
		this.initialized = false;
		if (this.isOfflineVRRig)
		{
			if (CosmeticsController.hasInstance && CosmeticsController.instance.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				CosmeticsController.instance.currentWornSet.LoadFromPlayerPreferences(CosmeticsController.instance);
			}
			if (Application.platform == 11 && this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.initialized = true;
		}
		else if (!this.isOfflineVRRig)
		{
			if (this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.head.syncPos = -this.headBodyOffset;
		}
		GorillaSkin.ShowActiveSkin(this);
		base.Invoke("ApplyColorCode", 1f);
		List<Material> list = new List<Material>();
		this.mainSkin.GetSharedMaterials(list);
		this.layerChanger = base.GetComponent<LayerChanger>();
		if (this.layerChanger != null)
		{
			this.layerChanger.InitializeLayers(base.transform);
		}
		this.frozenEffectMinY = this.frozenEffect.transform.localScale.y;
		this.frozenEffectMinHorizontalScale = this.frozenEffect.transform.localScale.x;
		this.rightIndex.Initialize();
		this.rightMiddle.Initialize();
		this.rightThumb.Initialize();
		this.leftIndex.Initialize();
		this.leftMiddle.Initialize();
		this.leftThumb.Initialize();
	}

	// Token: 0x06001B39 RID: 6969 RVA: 0x0008E5F0 File Offset: 0x0008C7F0
	public void SliceUpdate()
	{
		float time = Time.time;
		if (this._nextUpdateTime < 0f)
		{
			this._nextUpdateTime = time + 1f;
			return;
		}
		if (time < this._nextUpdateTime)
		{
			return;
		}
		this._nextUpdateTime = time + 1f;
		if (RoomSystem.JoinedRoom && NetworkSystem.Instance.IsMasterClient && GameMode.ActiveNetworkHandler.IsNull())
		{
			GameMode.LoadGameModeFromProperty();
		}
	}

	// Token: 0x06001B3A RID: 6970 RVA: 0x0008E65C File Offset: 0x0008C85C
	public bool IsItemAllowed(string itemName)
	{
		if (itemName == "Slingshot")
		{
			return NetworkSystem.Instance.InRoom && GorillaGameManager.instance is GorillaPaintbrawlManager;
		}
		if (BuilderSetManager.instance.GetStarterSetsConcat().Contains(itemName))
		{
			return true;
		}
		if (this.concatStringOfCosmeticsAllowed == null)
		{
			return false;
		}
		if (this.concatStringOfCosmeticsAllowed.Contains(itemName) || PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(this, itemName))
		{
			return true;
		}
		bool canTryOn = CosmeticsController.instance.GetItemFromDict(itemName).canTryOn;
		return this.inTryOnRoom && canTryOn;
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x0008E6EB File Offset: 0x0008C8EB
	public void ApplyLocalTrajectoryOverride(Vector3 overrideVelocity)
	{
		this.LocalTrajectoryOverrideBlend = 1f;
		this.LocalTrajectoryOverridePosition = base.transform.position;
		this.LocalTrajectoryOverrideVelocity = overrideVelocity;
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x0008E710 File Offset: 0x0008C910
	public bool IsLocalTrajectoryOverrideActive()
	{
		return this.LocalTrajectoryOverrideBlend > 0f;
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x0008E71F File Offset: 0x0008C91F
	public void ApplyLocalGrabOverride(bool isBody, bool isLeftHand, Transform grabbingHand)
	{
		this.localOverrideIsBody = isBody;
		this.localOverrideIsLeftHand = isLeftHand;
		this.localOverrideGrabbingHand = grabbingHand;
		this.localGrabOverrideBlend = 1f;
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x0008E741 File Offset: 0x0008C941
	public void ClearLocalGrabOverride()
	{
		this.localGrabOverrideBlend = -1f;
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x0008E750 File Offset: 0x0008C950
	public void RemoteRigUpdate()
	{
		if (this.scaleFactor != this.lastScaleFactor)
		{
			this.ScaleUpdate();
		}
		if (this.voiceAudio != null)
		{
			float num = 1f;
			if (this.IsHaunted)
			{
				num = this.HauntedVoicePitch;
			}
			else if (this.UsingHauntedRing)
			{
				num = this.HauntedRingVoicePitch;
			}
			else if (this.PitchShiftCosmetics.Count > 0)
			{
				if (this.pitchShiftCosmeticsDirty)
				{
					this.cosmeticPitchShift = 0f;
					for (int i = 0; i < this.PitchShiftCosmetics.Count; i++)
					{
						this.cosmeticPitchShift += this.PitchShiftCosmetics[i].Pitch;
					}
					this.cosmeticPitchShift /= (float)this.PitchShiftCosmetics.Count;
					this.pitchShiftCosmeticsDirty = false;
				}
				num = this.cosmeticPitchShift;
			}
			else
			{
				float num2 = GorillaTagger.Instance.offlineVRRig.scaleFactor / this.scaleFactor;
				float num3 = this.voicePitchForRelativeScale.Evaluate(num2);
				if (float.IsNaN(num3) || num3 <= 0f)
				{
					Debug.LogError("Voice pitch curve is invalid, please fix!");
				}
				else
				{
					num = num3;
				}
			}
			if (!Mathf.Approximately(this.voiceAudio.pitch, num))
			{
				this.voiceAudio.pitch = num;
			}
		}
		this.jobPos = base.transform.position;
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobPos = Vector3.Lerp(base.transform.position, this.SanitizeVector3(this.syncPos), this.lerpValueBody * 0.66f);
			if (this.currentRopeSwing && this.currentRopeSwingTarget)
			{
				Vector3 vector;
				if (this.grabbedRopeIsLeft)
				{
					vector = this.currentRopeSwingTarget.position - this.leftHandTransform.position;
				}
				else
				{
					vector = this.currentRopeSwingTarget.position - this.rightHandTransform.position;
				}
				if (this.shouldLerpToRope)
				{
					this.jobPos += Vector3.Lerp(Vector3.zero, vector, this.lastRopeGrabTimer * 4f);
					if (this.lastRopeGrabTimer < 1f)
					{
						this.lastRopeGrabTimer += Time.deltaTime;
					}
				}
				else
				{
					this.jobPos += vector;
				}
			}
			else if (this.currentHoldParent)
			{
				Transform transform;
				if (this.grabbedRopeIsBody)
				{
					transform = this.bodyTransform;
				}
				else if (this.grabbedRopeIsLeft)
				{
					transform = this.leftHandTransform;
				}
				else
				{
					transform = this.rightHandTransform;
				}
				this.jobPos += this.currentHoldParent.TransformPoint(this.grabbedRopeOffset) - transform.position;
			}
			else if (this.mountedMonkeBlock || this.mountedMovingSurface)
			{
				Transform transform2 = this.movingSurfaceIsMonkeBlock ? this.mountedMonkeBlock.transform : this.mountedMovingSurface.transform;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = this.jobPos - base.transform.position;
				Transform transform3;
				if (this.mountedMovingSurfaceIsBody)
				{
					transform3 = this.bodyTransform;
				}
				else if (this.mountedMovingSurfaceIsLeft)
				{
					transform3 = this.leftHandTransform;
				}
				else
				{
					transform3 = this.rightHandTransform;
				}
				vector2 = transform2.TransformPoint(this.mountedMonkeBlockOffset) - (transform3.position + vector3);
				if (this.shouldLerpToMovingSurface)
				{
					this.lastMountedSurfaceTimer += Time.deltaTime;
					this.jobPos += Vector3.Lerp(Vector3.zero, vector2, this.lastMountedSurfaceTimer * 4f);
					if (this.lastMountedSurfaceTimer * 4f >= 1f)
					{
						this.shouldLerpToMovingSurface = false;
					}
				}
				else
				{
					this.jobPos += vector2;
				}
			}
		}
		else
		{
			this.jobPos = this.SanitizeVector3(this.syncPos);
		}
		if (this.LocalTrajectoryOverrideBlend > 0f)
		{
			this.LocalTrajectoryOverrideBlend -= Time.deltaTime / this.LocalTrajectoryOverrideDuration;
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			Vector3 localTrajectoryOverrideVelocity;
			Vector3 localTrajectoryOverridePosition;
			if (this.LocalTestMovementCollision(this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideVelocity, out localTrajectoryOverrideVelocity, out localTrajectoryOverridePosition))
			{
				this.LocalTrajectoryOverrideVelocity = localTrajectoryOverrideVelocity;
				this.LocalTrajectoryOverridePosition = localTrajectoryOverridePosition;
			}
			else
			{
				this.LocalTrajectoryOverridePosition += this.LocalTrajectoryOverrideVelocity * Time.deltaTime;
			}
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			this.jobPos = Vector3.Lerp(this.jobPos, this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideBlend);
		}
		else if (this.localGrabOverrideBlend > 0f)
		{
			this.localGrabOverrideBlend -= Time.deltaTime / this.LocalGrabOverrideDuration;
			if (this.localOverrideGrabbingHand != null)
			{
				Transform transform4;
				if (this.localOverrideIsBody)
				{
					transform4 = this.bodyTransform;
				}
				else if (this.localOverrideIsLeftHand)
				{
					transform4 = this.leftHandTransform;
				}
				else
				{
					transform4 = this.rightHandTransform;
				}
				this.jobPos += this.localOverrideGrabbingHand.TransformPoint(this.grabbedRopeOffset) - transform4.position;
			}
		}
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobRotation = Quaternion.Lerp(base.transform.rotation, this.SanitizeQuaternion(this.syncRotation), this.lerpValueBody);
		}
		else
		{
			this.jobRotation = this.SanitizeQuaternion(this.syncRotation);
		}
		this.head.syncPos = base.transform.rotation * -this.headBodyOffset * this.scaleFactor;
		this.head.MapOther(this.lerpValueBody);
		this.rightHand.MapOther(this.lerpValueBody);
		this.leftHand.MapOther(this.lerpValueBody);
		this.rightIndex.MapOtherFinger((float)(this.handSync % 10) / 10f, this.lerpValueFingers);
		this.rightMiddle.MapOtherFinger((float)(this.handSync % 100) / 100f, this.lerpValueFingers);
		this.rightThumb.MapOtherFinger((float)(this.handSync % 1000) / 1000f, this.lerpValueFingers);
		this.leftIndex.MapOtherFinger((float)(this.handSync % 10000) / 10000f, this.lerpValueFingers);
		this.leftMiddle.MapOtherFinger((float)(this.handSync % 100000) / 100000f, this.lerpValueFingers);
		this.leftThumb.MapOtherFinger((float)(this.handSync % 1000000) / 1000000f, this.lerpValueFingers);
		this.leftHandHoldableStatus = this.handSync % 10000000 / 1000000;
		this.rightHandHoldableStatus = this.handSync % 100000000 / 10000000;
	}

	// Token: 0x06001B40 RID: 6976 RVA: 0x0008EE84 File Offset: 0x0008D084
	private void ScaleUpdate()
	{
		this.frameScale = Mathf.MoveTowards(this.lastScaleFactor, this.scaleFactor, Time.deltaTime * 4f);
		base.transform.localScale = Vector3.one * this.frameScale;
		this.lastScaleFactor = this.frameScale;
	}

	// Token: 0x06001B41 RID: 6977 RVA: 0x0008EEDA File Offset: 0x0008D0DA
	public void AddLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Add(action);
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x0008EEE9 File Offset: 0x0008D0E9
	public void RemoveLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Remove(action);
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x0008EEF8 File Offset: 0x0008D0F8
	public void PostTick()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.isOfflineVRRig)
		{
			if (GorillaGameManager.instance != null)
			{
				this.speedArray = GorillaGameManager.instance.LocalPlayerSpeed();
				instance.jumpMultiplier = this.speedArray[1];
				instance.maxJumpSpeed = this.speedArray[0];
			}
			else
			{
				instance.jumpMultiplier = 1.1f;
				instance.maxJumpSpeed = 6.5f;
			}
			this.nativeScale = instance.NativeScale;
			this.scaleMultiplier = instance.ScaleMultiplier;
			if (this.scaleFactor != this.lastScaleFactor)
			{
				this.ScaleUpdate();
			}
			base.transform.eulerAngles = new Vector3(0f, this.mainCamera.transform.rotation.eulerAngles.y, 0f);
			this.syncPos = this.mainCamera.transform.position + this.headConstraint.rotation * this.head.trackingPositionOffset * this.lastScaleFactor + base.transform.rotation * this.headBodyOffset * this.lastScaleFactor;
			base.transform.position = this.syncPos;
			this.head.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.leftHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightIndex.MapMyFinger(this.lerpValueFingers);
			this.rightMiddle.MapMyFinger(this.lerpValueFingers);
			this.rightThumb.MapMyFinger(this.lerpValueFingers);
			this.leftIndex.MapMyFinger(this.lerpValueFingers);
			this.leftMiddle.MapMyFinger(this.lerpValueFingers);
			this.leftThumb.MapMyFinger(this.lerpValueFingers);
			bool isGroundedHand = instance.IsGroundedHand || instance.IsThrusterActive;
			bool isGroundedButt = instance.IsGroundedButt;
			bool isLeftGrabbing = EquipmentInteractor.instance.isLeftGrabbing;
			bool canBeGrabbed = isLeftGrabbing && EquipmentInteractor.instance.CanGrabLeft();
			bool isRightGrabbing = EquipmentInteractor.instance.isRightGrabbing;
			bool canBeGrabbed2 = isRightGrabbing && EquipmentInteractor.instance.CanGrabRight();
			this.LastTouchedGroundAtNetworkTime = instance.LastTouchedGroundAtNetworkTime;
			this.LastHandTouchedGroundAtNetworkTime = instance.LastHandTouchedGroundAtNetworkTime;
			HandLink handLink = this.leftHandLink;
			if (handLink != null)
			{
				handLink.LocalUpdate(isGroundedHand, isGroundedButt, isLeftGrabbing, canBeGrabbed);
			}
			HandLink handLink2 = this.rightHandLink;
			if (handLink2 != null)
			{
				handLink2.LocalUpdate(isGroundedHand, isGroundedButt, isRightGrabbing, canBeGrabbed2);
			}
			if (GorillaTagger.Instance.loadedDeviceName == "Oculus")
			{
				this.mainSkin.enabled = OVRManager.hasInputFocus;
			}
			this.bodyRenderer.ActiveBody.enabled = !instance.inOverlay;
			int i = this.loudnessCheckFrame - 1;
			this.loudnessCheckFrame = i;
			if (i < 0)
			{
				this.SpeakingLoudness = 0f;
				if (this.shouldSendSpeakingLoudness && this.netView)
				{
					PhotonVoiceView component = this.netView.GetComponent<PhotonVoiceView>();
					if (component && component.RecorderInUse)
					{
						MicWrapper micWrapper = component.RecorderInUse.InputSource as MicWrapper;
						if (micWrapper != null)
						{
							int num = this.replacementVoiceDetectionDelay;
							if (num > this.voiceSampleBuffer.Length)
							{
								Array.Resize<float>(ref this.voiceSampleBuffer, num);
							}
							float[] array = this.voiceSampleBuffer;
							if (micWrapper != null && micWrapper.Mic != null && micWrapper.Mic.samples >= num && micWrapper.Mic.GetData(array, micWrapper.Mic.samples - num))
							{
								float num2 = 0f;
								for (int j = 0; j < num; j++)
								{
									float num3 = Mathf.Sqrt(array[j]);
									if (num3 > num2)
									{
										num2 = num3;
									}
								}
								this.SpeakingLoudness = num2;
							}
						}
					}
				}
				this.loudnessCheckFrame = 10;
			}
			if (PhotonNetwork.InRoom && Time.time > this.nextLocalVelocityStoreTimestamp)
			{
				this.AddVelocityToQueue(base.transform.position, PhotonNetwork.Time);
				this.nextLocalVelocityStoreTimestamp = Time.time + 0.1f;
			}
		}
		if (this.leftHandLink.IsLinkActive())
		{
			VRRig myRig = this.leftHandLink.grabbedLink.myRig;
			if (this.isLocal && myRig.inDuplicationZone && myRig.duplicationZone.IsApplyingDisplacement)
			{
				this.leftHandLink.BreakLink();
			}
			else
			{
				this.leftHandLink.SnapHandsTogether();
			}
		}
		if (this.rightHandLink.IsLinkActive())
		{
			VRRig myRig2 = this.rightHandLink.grabbedLink.myRig;
			if (this.isLocal && myRig2.inDuplicationZone && myRig2.duplicationZone.IsApplyingDisplacement)
			{
				this.rightHandLink.BreakLink();
			}
			else
			{
				this.rightHandLink.SnapHandsTogether();
			}
		}
		if (this.creator != null)
		{
			if (GorillaGameManager.instance != null)
			{
				GorillaGameManager.instance.UpdatePlayerAppearance(this);
			}
			else if (this.setMatIndex != 0)
			{
				this.ChangeMaterialLocal(0);
				this.ForceResetFrozenEffect();
			}
		}
		if (this.inDuplicationZone)
		{
			this.renderTransform.position = base.transform.position + this.duplicationZone.VisualOffsetForRigs;
		}
		if (this.frozenEffect.activeSelf)
		{
			GorillaFreezeTagManager gorillaFreezeTagManager = GorillaGameManager.instance as GorillaFreezeTagManager;
			if (gorillaFreezeTagManager != null)
			{
				this.UpdateFrozen(Time.deltaTime, gorillaFreezeTagManager.freezeDuration);
			}
		}
		if (this.TemporaryCosmeticEffects.Count > 0)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in Enumerable.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>(this.TemporaryCosmeticEffects))
			{
				if (Time.time - effect.Value.EffectStartedTime >= effect.Value.EffectDuration)
				{
					this.RemoveTemporaryCosmeticEffects(effect);
				}
			}
		}
		this.lateUpdateCallbacks.TryRunCallbacks();
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x0008F4EC File Offset: 0x0008D6EC
	public void UpdateFrozen(float dt, float freezeDuration)
	{
		Vector3 localScale = this.frozenEffect.transform.localScale;
		Vector3 vector = localScale;
		vector.y = Mathf.Lerp(this.frozenEffectMinY, this.frozenEffectMaxY, this.frozenTimeElapsed / freezeDuration);
		localScale..ctor(localScale.x, vector.y, localScale.z);
		this.frozenEffect.transform.localScale = localScale;
		this.frozenTimeElapsed += dt;
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x0008F564 File Offset: 0x0008D764
	private void RemoveTemporaryCosmeticEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin)
		{
			bool flag;
			if (effect.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == effect.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
			}
		}
		else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback)
		{
			this.DisableHitWithKnockBack(effect);
		}
		this.TemporaryCosmeticEffects.Remove(effect.Key);
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x0008F5D7 File Offset: 0x0008D7D7
	public void SpawnSkinEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		GorillaSkin.ApplyToRig(this, effect.Value.newSkin, GorillaSkin.SkinType.temporaryEffect);
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001B47 RID: 6983 RVA: 0x0008F606 File Offset: 0x0008D806
	public void EnableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x0008F624 File Offset: 0x0008D824
	private void DisableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (this.TemporaryCosmeticEffects.ContainsKey(effect.Key) && effect.Value.knockbackVFX)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.knockbackVFX, base.transform.position, true);
			if (gameObject != null)
			{
				gameObject.gameObject.transform.SetParent(base.transform);
				gameObject.gameObject.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x06001B49 RID: 6985 RVA: 0x0008F6B0 File Offset: 0x0008D8B0
	public void DisableHitWithKnockBack()
	{
		foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in Enumerable.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>(this.TemporaryCosmeticEffects))
		{
			bool flag;
			if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback)
			{
				this.DisableHitWithKnockBack(effect);
				this.TemporaryCosmeticEffects.Remove(effect.Key);
			}
			else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin && effect.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == effect.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
				this.TemporaryCosmeticEffects.Remove(effect.Key);
			}
		}
	}

	// Token: 0x06001B4A RID: 6986 RVA: 0x0008F606 File Offset: 0x0008D806
	public void ApplyInstanceKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x0008F606 File Offset: 0x0008D806
	public void ActivateVOEffect(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001B4C RID: 6988 RVA: 0x0008F75E File Offset: 0x0008D95E
	public bool TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE key, out CosmeticEffectsOnPlayers.CosmeticEffect value)
	{
		if (this.TemporaryCosmeticEffects == null)
		{
			value = null;
			return false;
		}
		return this.TemporaryCosmeticEffects.TryGetValue(key, ref value);
	}

	// Token: 0x06001B4D RID: 6989 RVA: 0x0008F77C File Offset: 0x0008D97C
	public void PlayCosmeticEffectSFX(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
		int num = Random.Range(0, effect.Value.sfxAudioClip.Count);
		this.tagSound.PlayOneShot(effect.Value.sfxAudioClip[num]);
	}

	// Token: 0x06001B4E RID: 6990 RVA: 0x0008F7D8 File Offset: 0x0008D9D8
	public void SpawnVFXEffect(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.VFXGameObject, base.transform.position, true);
		if (gameObject != null)
		{
			gameObject.gameObject.transform.SetParent(base.transform);
			gameObject.gameObject.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001B4F RID: 6991 RVA: 0x0008F83C File Offset: 0x0008DA3C
	public bool IsPlayerMeshHidden
	{
		get
		{
			return !this.mainSkin.enabled;
		}
	}

	// Token: 0x06001B50 RID: 6992 RVA: 0x0008F84C File Offset: 0x0008DA4C
	public void SetPlayerMeshHidden(bool hide)
	{
		this.mainSkin.enabled = !hide;
		this.faceSkin.enabled = !hide;
		this.nameTagAnchor.SetActive(!hide);
		this.UpdateMatParticles(-1);
	}

	// Token: 0x06001B51 RID: 6993 RVA: 0x0008F882 File Offset: 0x0008DA82
	public void SetInvisibleToLocalPlayer(bool invisible)
	{
		if (this.IsInvisibleToLocalPlayer == invisible)
		{
			return;
		}
		this.IsInvisibleToLocalPlayer = invisible;
		this.nameTagAnchor.SetActive(!invisible);
		this.UpdateFriendshipBracelet();
	}

	// Token: 0x06001B52 RID: 6994 RVA: 0x0008F8AA File Offset: 0x0008DAAA
	public void ChangeLayer(string layerName)
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.ChangeLayer(base.transform.parent, layerName);
		}
		GTPlayer.Instance.ChangeLayer(layerName);
	}

	// Token: 0x06001B53 RID: 6995 RVA: 0x0008F8DC File Offset: 0x0008DADC
	public void RestoreLayer()
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.RestoreOriginalLayers();
		}
		GTPlayer.Instance.RestoreLayer();
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x00002789 File Offset: 0x00000989
	public void SetHeadBodyOffset()
	{
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x0008F901 File Offset: 0x0008DB01
	public void VRRigResize(float ratioVar)
	{
		this.ratio *= ratioVar;
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x0008F914 File Offset: 0x0008DB14
	public int ReturnHandPosition()
	{
		return 0 + Mathf.FloorToInt(this.rightIndex.calcT * 9.99f) + Mathf.FloorToInt(this.rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(this.rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(this.leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(this.leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(this.leftThumb.calcT * 9.99f) * 100000 + this.leftHandHoldableStatus * 1000000 + this.rightHandHoldableStatus * 10000000;
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x0008F9DE File Offset: 0x0008DBDE
	public void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.currentRopeSwingTarget && this.currentRopeSwingTarget.gameObject)
		{
			Object.Destroy(this.currentRopeSwingTarget.gameObject);
		}
		this.ClearRopeData();
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x0008FA20 File Offset: 0x0008DC20
	private InputStruct SerializeWriteShared()
	{
		InputStruct result = default(InputStruct);
		result.headRotation = BitPackUtils.PackQuaternionForNetwork(this.head.rigTarget.localRotation);
		result.rightHandLong = BitPackUtils.PackHandPosRotForNetwork(this.rightHand.rigTarget.localPosition, this.rightHand.rigTarget.localRotation);
		result.leftHandLong = BitPackUtils.PackHandPosRotForNetwork(this.leftHand.rigTarget.localPosition, this.leftHand.rigTarget.localRotation);
		result.position = BitPackUtils.PackWorldPosForNetwork(base.transform.position);
		result.handPosition = this.ReturnHandPosition();
		result.taggedById = (short)this.taggedById;
		int num = Mathf.Clamp(Mathf.RoundToInt(base.transform.rotation.eulerAngles.y + 360f) % 360, 0, 360);
		int num2 = Mathf.RoundToInt(Mathf.Clamp01(this.SpeakingLoudness) * 255f);
		bool flag = this.leftHandLink.IsLinkActive() || this.rightHandLink.IsLinkActive();
		GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
		bool flag2 = activeGameMode != null && activeGameMode.GameType() == GameModeType.PropHunt;
		int packedFields = num + (this.remoteUseReplacementVoice ? 512 : 0) + ((this.grabbedRopeIndex != -1) ? 1024 : 0) + (this.grabbedRopeIsPhotonView ? 2048 : 0) + (flag ? 4096 : 0) + (this.hoverboardVisual.IsHeld ? 8192 : 0) + (this.hoverboardVisual.IsLeftHanded ? 16384 : 0) + ((this.mountedMovingSurfaceId != -1) ? 32768 : 0) + (flag2 ? 65536 : 0) + (this.propHuntHandFollower.IsLeftHand ? 131072 : 0) + (this.leftHandLink.CanBeGrabbed() ? 262144 : 0) + (this.rightHandLink.CanBeGrabbed() ? 524288 : 0) + (num2 << 24);
		result.packedFields = packedFields;
		result.packedCompetitiveData = this.PackCompetitiveData();
		if (this.grabbedRopeIndex != -1)
		{
			result.grabbedRopeIndex = this.grabbedRopeIndex;
			result.ropeBoneIndex = this.grabbedRopeBoneIndex;
			result.ropeGrabIsLeft = this.grabbedRopeIsLeft;
			result.ropeGrabIsBody = this.grabbedRopeIsBody;
			result.ropeGrabOffset = this.grabbedRopeOffset;
		}
		if (this.grabbedRopeIndex == -1 && this.mountedMovingSurfaceId != -1)
		{
			result.grabbedRopeIndex = this.mountedMovingSurfaceId;
			result.ropeGrabIsLeft = this.mountedMovingSurfaceIsLeft;
			result.ropeGrabIsBody = this.mountedMovingSurfaceIsBody;
			result.ropeGrabOffset = this.mountedMonkeBlockOffset;
		}
		if (this.hoverboardVisual.IsHeld)
		{
			result.hoverboardPosRot = BitPackUtils.PackHandPosRotForNetwork(this.hoverboardVisual.NominalLocalPosition, this.hoverboardVisual.NominalLocalRotation);
			result.hoverboardColor = BitPackUtils.PackColorForNetwork(this.hoverboardVisual.boardColor);
		}
		if (flag2)
		{
			result.propHuntPosRot = this.propHuntHandFollower.GetRelativePosRotLong();
		}
		if (flag)
		{
			this.leftHandLink.Write(out result.isGroundedHand, out result.isGroundedButt, out result.leftHandGrabbedActorNumber, out result.leftGrabbedHandIsLeft);
			this.rightHandLink.Write(out result.isGroundedHand, out result.isGroundedButt, out result.rightHandGrabbedActorNumber, out result.rightGrabbedHandIsLeft);
			result.lastTouchedGroundAtTime = this.LastTouchedGroundAtNetworkTime;
			result.lastHandTouchedGroundAtTime = this.LastHandTouchedGroundAtNetworkTime;
		}
		return result;
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x0008FD9C File Offset: 0x0008DF9C
	private void SerializeReadShared(InputStruct data)
	{
		VRMap vrmap = this.head;
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.headRotation);
		ref vrmap.syncRotation.SetValueSafe(quaternion);
		BitPackUtils.UnpackHandPosRotFromNetwork(data.rightHandLong, out this.tempVec, out this.tempQuat);
		this.rightHand.syncPos = this.tempVec;
		ref this.rightHand.syncRotation.SetValueSafe(this.tempQuat);
		BitPackUtils.UnpackHandPosRotFromNetwork(data.leftHandLong, out this.tempVec, out this.tempQuat);
		this.leftHand.syncPos = this.tempVec;
		ref this.leftHand.syncRotation.SetValueSafe(this.tempQuat);
		this.syncPos = BitPackUtils.UnpackWorldPosFromNetwork(data.position);
		this.handSync = data.handPosition;
		int packedFields = data.packedFields;
		int num = packedFields & 511;
		this.syncRotation.eulerAngles = this.SanitizeVector3(new Vector3(0f, (float)num, 0f));
		this.remoteUseReplacementVoice = ((packedFields & 512) != 0);
		int num2 = packedFields >> 24 & 255;
		this.SpeakingLoudness = (float)num2 / 255f;
		this.UpdateReplacementVoice();
		this.UnpackCompetitiveData(data.packedCompetitiveData);
		this.taggedById = (int)data.taggedById;
		bool flag = (packedFields & 1024) != 0;
		this.grabbedRopeIsPhotonView = ((packedFields & 2048) != 0);
		if (flag)
		{
			this.grabbedRopeIndex = data.grabbedRopeIndex;
			this.grabbedRopeBoneIndex = data.ropeBoneIndex;
			this.grabbedRopeIsLeft = data.ropeGrabIsLeft;
			this.grabbedRopeIsBody = data.ropeGrabIsBody;
			ref this.grabbedRopeOffset.SetValueSafe(data.ropeGrabOffset);
		}
		else
		{
			this.grabbedRopeIndex = -1;
		}
		bool flag2 = (packedFields & 32768) != 0;
		if (!flag && flag2)
		{
			this.mountedMovingSurfaceId = data.grabbedRopeIndex;
			this.mountedMovingSurfaceIsLeft = data.ropeGrabIsLeft;
			this.mountedMovingSurfaceIsBody = data.ropeGrabIsBody;
			ref this.mountedMonkeBlockOffset.SetValueSafe(data.ropeGrabOffset);
			this.movingSurfaceIsMonkeBlock = data.movingSurfaceIsMonkeBlock;
		}
		else
		{
			this.mountedMovingSurfaceId = -1;
		}
		bool flag3 = (packedFields & 8192) != 0;
		bool isHeldLeftHanded = (packedFields & 16384) != 0;
		if (flag3)
		{
			Vector3 v;
			Quaternion localRotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.hoverboardPosRot, out v, out localRotation);
			Color boardColor = BitPackUtils.UnpackColorFromNetwork(data.hoverboardColor);
			if (localRotation.IsValid())
			{
				this.hoverboardVisual.SetIsHeld(isHeldLeftHanded, v.ClampMagnitudeSafe(1f), localRotation, boardColor);
			}
		}
		else if (this.hoverboardVisual.gameObject.activeSelf)
		{
			this.hoverboardVisual.SetNotHeld();
		}
		if ((packedFields & 65536) != 0)
		{
			bool isLeftHand = (packedFields & 131072) != 0;
			Vector3 propPos;
			Quaternion propRot;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.propHuntPosRot, out propPos, out propRot);
			this.propHuntHandFollower.SetProp(isLeftHand, propPos, propRot);
		}
		if (this.grabbedRopeIsPhotonView)
		{
			this.localGrabOverrideBlend = -1f;
		}
		Vector3 position = base.transform.position;
		this.leftHandLink.Read(this.leftHand.syncPos, this.syncRotation, position, data.isGroundedHand, data.isGroundedButt, (packedFields & 262144) != 0, data.leftHandGrabbedActorNumber, data.leftGrabbedHandIsLeft);
		this.rightHandLink.Read(this.rightHand.syncPos, this.syncRotation, position, data.isGroundedHand, data.isGroundedButt, (packedFields & 524288) != 0, data.rightHandGrabbedActorNumber, data.rightGrabbedHandIsLeft);
		this.LastTouchedGroundAtNetworkTime = data.lastTouchedGroundAtTime;
		this.LastHandTouchedGroundAtNetworkTime = data.lastHandTouchedGroundAtTime;
		this.UpdateRopeData();
		this.UpdateMovingMonkeBlockData();
		this.AddVelocityToQueue(this.syncPos, data.serverTimeStamp);
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x00090124 File Offset: 0x0008E324
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		stream.SendNext(inputStruct.headRotation);
		stream.SendNext(inputStruct.rightHandLong);
		stream.SendNext(inputStruct.leftHandLong);
		stream.SendNext(inputStruct.position);
		stream.SendNext(inputStruct.handPosition);
		stream.SendNext(inputStruct.packedFields);
		stream.SendNext(inputStruct.packedCompetitiveData);
		if (this.grabbedRopeIndex != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeBoneIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
		}
		else if (this.mountedMovingSurfaceId != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
			stream.SendNext(inputStruct.movingSurfaceIsMonkeBlock);
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			stream.SendNext(inputStruct.hoverboardPosRot);
			stream.SendNext(inputStruct.hoverboardColor);
		}
		if ((inputStruct.packedFields & 4096) != 0)
		{
			stream.SendNext(inputStruct.isGroundedHand);
			stream.SendNext(inputStruct.isGroundedButt);
			stream.SendNext(inputStruct.leftHandGrabbedActorNumber);
			stream.SendNext(inputStruct.leftGrabbedHandIsLeft);
			stream.SendNext(inputStruct.rightHandGrabbedActorNumber);
			stream.SendNext(inputStruct.rightGrabbedHandIsLeft);
			stream.SendNext(inputStruct.lastTouchedGroundAtTime);
			stream.SendNext(inputStruct.lastHandTouchedGroundAtTime);
		}
		if ((inputStruct.packedFields & 65536) != 0)
		{
			stream.SendNext(inputStruct.propHuntPosRot);
		}
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x00090358 File Offset: 0x0008E558
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		double sentServerTime = info.SentServerTime;
		InputStruct inputStruct = new InputStruct
		{
			headRotation = (int)stream.ReceiveNext(),
			rightHandLong = (long)stream.ReceiveNext(),
			leftHandLong = (long)stream.ReceiveNext(),
			position = (long)stream.ReceiveNext(),
			handPosition = (int)stream.ReceiveNext(),
			packedFields = (int)stream.ReceiveNext(),
			packedCompetitiveData = (short)stream.ReceiveNext()
		};
		bool flag = (inputStruct.packedFields & 1024) != 0;
		bool flag2 = (inputStruct.packedFields & 32768) != 0;
		if (flag)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeBoneIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		else if (flag2)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			inputStruct.hoverboardPosRot = (long)stream.ReceiveNext();
			inputStruct.hoverboardColor = (short)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 4096) != 0)
		{
			inputStruct.isGroundedHand = (bool)stream.ReceiveNext();
			inputStruct.isGroundedButt = (bool)stream.ReceiveNext();
			inputStruct.leftHandGrabbedActorNumber = (int)stream.ReceiveNext();
			inputStruct.leftGrabbedHandIsLeft = (bool)stream.ReceiveNext();
			inputStruct.rightHandGrabbedActorNumber = (int)stream.ReceiveNext();
			inputStruct.rightGrabbedHandIsLeft = (bool)stream.ReceiveNext();
			inputStruct.lastTouchedGroundAtTime = (float)stream.ReceiveNext();
			inputStruct.lastHandTouchedGroundAtTime = (float)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 65536) != 0)
		{
			inputStruct.propHuntPosRot = (long)stream.ReceiveNext();
		}
		inputStruct.serverTimeStamp = info.SentServerTime;
		this.SerializeReadShared(inputStruct);
	}

	// Token: 0x06001B5C RID: 7004 RVA: 0x000905C8 File Offset: 0x0008E7C8
	public object OnSerializeWrite()
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		double serverTimeStamp = NetworkSystem.Instance.SimTick / 1000.0;
		inputStruct.serverTimeStamp = serverTimeStamp;
		return inputStruct;
	}

	// Token: 0x06001B5D RID: 7005 RVA: 0x00090604 File Offset: 0x0008E804
	public void OnSerializeRead(object objectData)
	{
		InputStruct data = (InputStruct)objectData;
		this.SerializeReadShared(data);
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x00090620 File Offset: 0x0008E820
	private void UpdateExtrapolationTarget()
	{
		float num = (float)(NetworkSystem.Instance.SimTime - this.remoteLatestTimestamp);
		num -= 0.15f;
		num = Mathf.Clamp(num, -0.5f, 0.5f);
		this.syncPos += this.remoteVelocity * num;
		this.remoteCorrectionNeeded = this.syncPos - base.transform.position;
		if (this.remoteCorrectionNeeded.magnitude > 1.5f && this.grabbedRopeIndex <= 0)
		{
			base.transform.position = this.syncPos;
			this.remoteCorrectionNeeded = Vector3.zero;
		}
	}

	// Token: 0x06001B5F RID: 7007 RVA: 0x000906CC File Offset: 0x0008E8CC
	private void UpdateRopeData()
	{
		if (this.previousGrabbedRope == this.grabbedRopeIndex && this.previousGrabbedRopeBoneIndex == this.grabbedRopeBoneIndex && this.previousGrabbedRopeWasLeft == this.grabbedRopeIsLeft && this.previousGrabbedRopeWasBody == this.grabbedRopeIsBody)
		{
			return;
		}
		this.ClearRopeData();
		if (this.grabbedRopeIndex != -1)
		{
			GorillaRopeSwing gorillaRopeSwing;
			if (this.grabbedRopeIsPhotonView)
			{
				PhotonView photonView = PhotonView.Find(this.grabbedRopeIndex);
				GorillaClimbable gorillaClimbable;
				HandHoldXSceneRef handHoldXSceneRef;
				VRRigSerializer vrrigSerializer;
				if (photonView.TryGetComponent<GorillaClimbable>(ref gorillaClimbable))
				{
					this.currentHoldParent = photonView.transform;
				}
				else if (photonView.TryGetComponent<HandHoldXSceneRef>(ref handHoldXSceneRef))
				{
					GameObject targetObject = handHoldXSceneRef.targetObject;
					this.currentHoldParent = ((targetObject != null) ? targetObject.transform : null);
				}
				else if (photonView && photonView.TryGetComponent<VRRigSerializer>(ref vrrigSerializer))
				{
					this.currentHoldParent = ((this.grabbedRopeBoneIndex == 1) ? vrrigSerializer.VRRig.leftHandHoldsPlayer.transform : vrrigSerializer.VRRig.rightHandHoldsPlayer.transform);
				}
			}
			else if (RopeSwingManager.instance.TryGetRope(this.grabbedRopeIndex, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				if (this.currentRopeSwingTarget == null || this.currentRopeSwingTarget.gameObject == null)
				{
					this.currentRopeSwingTarget = new GameObject("RopeSwingTarget").transform;
				}
				if (gorillaRopeSwing.AttachRemotePlayer(this.creator.ActorNumber, this.grabbedRopeBoneIndex, this.currentRopeSwingTarget, this.grabbedRopeOffset))
				{
					this.currentRopeSwing = gorillaRopeSwing;
				}
				this.lastRopeGrabTimer = 0f;
			}
		}
		else if (this.previousGrabbedRope != -1)
		{
			PhotonView photonView2 = PhotonView.Find(this.previousGrabbedRope);
			VRRigSerializer vrrigSerializer2;
			if (photonView2 && photonView2.TryGetComponent<VRRigSerializer>(ref vrrigSerializer2) && vrrigSerializer2.VRRig == VRRig.LocalRig)
			{
				EquipmentInteractor.instance.ForceDropEquipment(this.bodyHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.leftHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.rightHolds);
			}
		}
		this.shouldLerpToRope = true;
		this.previousGrabbedRope = this.grabbedRopeIndex;
		this.previousGrabbedRopeBoneIndex = this.grabbedRopeBoneIndex;
		this.previousGrabbedRopeWasLeft = this.grabbedRopeIsLeft;
		this.previousGrabbedRopeWasBody = this.grabbedRopeIsBody;
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x0009090C File Offset: 0x0008EB0C
	private void UpdateMovingMonkeBlockData()
	{
		if (this.mountedMonkeBlockOffset.sqrMagnitude > 2f)
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.prevMovingSurfaceID == this.mountedMovingSurfaceId && this.movingSurfaceWasBody == this.mountedMovingSurfaceIsBody && this.movingSurfaceWasLeft == this.mountedMovingSurfaceIsLeft && this.movingSurfaceWasMonkeBlock == this.movingSurfaceIsMonkeBlock)
		{
			return;
		}
		if (this.mountedMovingSurfaceId == -1)
		{
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		else if (this.movingSurfaceIsMonkeBlock)
		{
			this.mountedMonkeBlock = null;
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.zoneEntity.currentZone, out builderTable))
			{
				this.mountedMonkeBlock = builderTable.GetPiece(this.mountedMovingSurfaceId);
			}
			if (this.mountedMonkeBlock == null)
			{
				this.mountedMovingSurfaceId = -1;
				this.mountedMovingSurfaceIsLeft = false;
				this.mountedMovingSurfaceIsBody = false;
				this.mountedMonkeBlock = null;
				this.mountedMovingSurface = null;
			}
		}
		else if (MovingSurfaceManager.instance == null || !MovingSurfaceManager.instance.TryGetMovingSurface(this.mountedMovingSurfaceId, out this.mountedMovingSurface))
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.mountedMovingSurfaceId != -1 && this.prevMovingSurfaceID == -1)
		{
			this.shouldLerpToMovingSurface = true;
			this.lastMountedSurfaceTimer = 0f;
		}
		this.prevMovingSurfaceID = this.mountedMovingSurfaceId;
		this.movingSurfaceWasLeft = this.mountedMovingSurfaceIsLeft;
		this.movingSurfaceWasBody = this.mountedMovingSurfaceIsBody;
		this.movingSurfaceWasMonkeBlock = this.movingSurfaceIsMonkeBlock;
	}

	// Token: 0x06001B61 RID: 7009 RVA: 0x00090AB8 File Offset: 0x0008ECB8
	public static void AttachLocalPlayerToMovingSurface(int blockId, bool isLeft, bool isBody, Vector3 offset, bool isMonkeBlock)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = blockId;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsLeft = isLeft;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsBody = isBody;
			GorillaTagger.Instance.offlineVRRig.movingSurfaceIsMonkeBlock = isMonkeBlock;
			GorillaTagger.Instance.offlineVRRig.mountedMonkeBlockOffset = offset;
		}
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x00090B2E File Offset: 0x0008ED2E
	public static void DetachLocalPlayerFromMovingSurface()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = -1;
		}
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x00090B58 File Offset: 0x0008ED58
	public static void AttachLocalPlayerToPhotonView(PhotonView view, XRNode xrNode, Vector3 offset, Vector3 velocity)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = view.ViewID;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == 4);
			GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = true;
		}
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x00090BC5 File Offset: 0x0008EDC5
	public static void DetachLocalPlayerFromPhotonView()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
		}
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x00090BF0 File Offset: 0x0008EDF0
	private void ClearRopeData()
	{
		if (this.currentRopeSwing)
		{
			this.currentRopeSwing.DetachRemotePlayer(this.creator.ActorNumber);
		}
		if (this.currentRopeSwingTarget)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		this.currentRopeSwing = null;
		this.currentHoldParent = null;
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x00090C47 File Offset: 0x0008EE47
	public void ChangeMaterial(int materialIndex, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			this.ChangeMaterialLocal(materialIndex);
		}
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x00090C60 File Offset: 0x0008EE60
	public void UpdateFrozenEffect(bool enable)
	{
		if (this.frozenEffect != null && ((!this.frozenEffect.activeSelf && enable) || (this.frozenEffect.activeSelf && !enable)))
		{
			this.frozenEffect.SetActive(enable);
			if (enable)
			{
				this.frozenTimeElapsed = 0f;
			}
			else
			{
				Vector3 localScale = this.frozenEffect.transform.localScale;
				localScale..ctor(localScale.x, this.frozenEffectMinY, localScale.z);
				this.frozenEffect.transform.localScale = localScale;
			}
		}
		if (this.iceCubeLeft != null && ((!this.iceCubeLeft.activeSelf && enable) || (this.iceCubeLeft.activeSelf && !enable)))
		{
			this.iceCubeLeft.SetActive(enable);
		}
		if (this.iceCubeRight != null && ((!this.iceCubeRight.activeSelf && enable) || (this.iceCubeRight.activeSelf && !enable)))
		{
			this.iceCubeRight.SetActive(enable);
		}
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x00090D6C File Offset: 0x0008EF6C
	public void ForceResetFrozenEffect()
	{
		this.frozenEffect.SetActive(false);
		this.iceCubeRight.SetActive(false);
		this.iceCubeLeft.SetActive(false);
	}

	// Token: 0x06001B69 RID: 7017 RVA: 0x00090D94 File Offset: 0x0008EF94
	public void ChangeMaterialLocal(int materialIndex)
	{
		if (this.setMatIndex == materialIndex)
		{
			return;
		}
		int num = this.setMatIndex;
		this.setMatIndex = materialIndex;
		if (this.setMatIndex > -1 && this.setMatIndex < this.materialsToChangeTo.Length)
		{
			this.bodyRenderer.SetMaterialIndex(materialIndex);
		}
		this.UpdateMatParticles(materialIndex);
		if (materialIndex > 0 && VRRig.LocalRig != this)
		{
			this.PlayTaggedEffect();
		}
		Action<int, int> onMaterialIndexChanged = this.OnMaterialIndexChanged;
		if (onMaterialIndexChanged == null)
		{
			return;
		}
		onMaterialIndexChanged.Invoke(num, this.setMatIndex);
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x00090E14 File Offset: 0x0008F014
	public void PlayTaggedEffect()
	{
		TagEffectPack tagEffectPack = null;
		quaternion quaternion = base.transform.rotation;
		TagEffectsLibrary.EffectType effectType = (VRRig.LocalRig == this) ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON;
		if (GorillaGameManager.instance != null && this.OwningNetPlayer == null)
		{
			GorillaGameManager.instance.lastTaggedActorNr.TryGetValue(this.OwningNetPlayer.ActorNumber, ref this.taggedById);
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.taggedById);
		RigContainer rigContainer;
		if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			tagEffectPack = rigContainer.Rig.CosmeticEffectPack;
			if (tagEffectPack && tagEffectPack.shouldFaceTagger && effectType == TagEffectsLibrary.EffectType.THIRD_PERSON)
			{
				quaternion = Quaternion.LookRotation((rigContainer.Rig.transform.position - base.transform.position).normalized);
			}
		}
		TagEffectsLibrary.PlayEffect(base.transform, false, this.scaleFactor, effectType, this.CosmeticEffectPack, tagEffectPack, quaternion);
	}

	// Token: 0x06001B6B RID: 7019 RVA: 0x00090F18 File Offset: 0x0008F118
	public void UpdateMatParticles(int materialIndex)
	{
		if (this.lavaParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 2 && this.lavaParticleSystem.isStopped)
			{
				this.lavaParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.lavaParticleSystem.isPlaying)
			{
				this.lavaParticleSystem.Stop();
			}
		}
		if (this.rockParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 1 && this.rockParticleSystem.isStopped)
			{
				this.rockParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.rockParticleSystem.isPlaying)
			{
				this.rockParticleSystem.Stop();
			}
		}
		if (this.iceParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 3 && this.rockParticleSystem.isStopped)
			{
				this.iceParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.iceParticleSystem.isPlaying)
			{
				this.iceParticleSystem.Stop();
			}
		}
		if (this.snowFlakeParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 14 && this.snowFlakeParticleSystem.isStopped)
			{
				this.snowFlakeParticleSystem.Play();
				return;
			}
			if (!this.isOfflineVRRig && this.snowFlakeParticleSystem.isPlaying)
			{
				this.snowFlakeParticleSystem.Stop();
			}
		}
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x00091078 File Offset: 0x0008F278
	public void InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_InitializeNoobMaterial");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		string userID = NetworkSystem.Instance.GetUserID(info.senderID);
		if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject) && (!this.initialized || (this.initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(userID)) || (this.initialized && CosmeticWardrobeProximityDetector.IsUserNearWardrobe(userID))))
		{
			this.initialized = true;
			blue = blue.ClampSafe(0f, 1f);
			red = red.ClampSafe(0f, 1f);
			green = green.ClampSafe(0f, 1f);
			this.InitializeNoobMaterialLocal(red, green, blue);
		}
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x00091154 File Offset: 0x0008F354
	public void InitializeNoobMaterialLocal(float red, float green, float blue)
	{
		Color color;
		color..ctor(red, green, blue);
		color.r = Mathf.Clamp(color.r, 0f, 1f);
		color.g = Mathf.Clamp(color.g, 0f, 1f);
		color.b = Mathf.Clamp(color.b, 0f, 1f);
		this.bodyRenderer.UpdateColor(color);
		this.SetColor(color);
		bool isNamePermissionEnabled = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		this.UpdateName(isNamePermissionEnabled);
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x000911E0 File Offset: 0x0008F3E0
	public void UpdateName(bool isNamePermissionEnabled)
	{
		if (!this.isOfflineVRRig && this.creator != null)
		{
			string text = (isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? this.creator.NickName : this.creator.DefaultName;
			this.playerNameVisible = this.NormalizeName(true, text);
		}
		else if (this.showName && NetworkSystem.Instance != null)
		{
			this.playerNameVisible = ((isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? NetworkSystem.Instance.GetMyNickName() : NetworkSystem.Instance.GetMyDefaultName());
		}
		this.SetNameTagText(this.playerNameVisible);
		if (this.creator != null)
		{
			this.creator.SanitizedNickName = this.playerNameVisible;
		}
		Action onPlayerNameVisibleChanged = this.OnPlayerNameVisibleChanged;
		if (onPlayerNameVisibleChanged == null)
		{
			return;
		}
		onPlayerNameVisibleChanged.Invoke();
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x000912AE File Offset: 0x0008F4AE
	public void SetNameTagText(string name)
	{
		this.playerNameVisible = name;
		this.playerText1.text = name;
		Action<RigContainer> onNameChanged = this.OnNameChanged;
		if (onNameChanged == null)
		{
			return;
		}
		onNameChanged.Invoke(this.rigContainer);
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x000912DC File Offset: 0x0008F4DC
	public void UpdateName()
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
		bool isNamePermissionEnabled = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == 1) && permissionDataByFeature.ManagedBy != 3;
		this.UpdateName(isNamePermissionEnabled);
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x00091318 File Offset: 0x0008F518
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			int length = text.Length;
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			int length2 = text.Length;
			if (length2 > 0 && length == length2 && GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
			}
		}
		return text;
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x000913A5 File Offset: 0x0008F5A5
	public void SetJumpLimitLocal(float maxJumpSpeed)
	{
		GTPlayer.Instance.maxJumpSpeed = maxJumpSpeed;
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000913B2 File Offset: 0x0008F5B2
	public void SetJumpMultiplierLocal(float jumpMultiplier)
	{
		GTPlayer.Instance.jumpMultiplier = jumpMultiplier;
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x000913C0 File Offset: 0x0008F5C0
	public void RequestMaterialColor(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RequestMaterialColor");
		Player playerRef = ((PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.senderID)).PlayerRef;
		if (this.netView.IsMine)
		{
			this.netView.GetView.RPC("RPC_InitializeNoobMaterial", playerRef, new object[]
			{
				this.myDefaultSkinMaterialInstance.color.r,
				this.myDefaultSkinMaterialInstance.color.g,
				this.myDefaultSkinMaterialInstance.color.b
			});
		}
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x00091468 File Offset: 0x0008F668
	public void RequestCosmetics(PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (this.netView.IsMine && CosmeticsController.hasInstance)
		{
			if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
			{
				this.netView.SendRPC("RPC_HideAllCosmetics", info.Sender, Array.Empty<object>());
				return;
			}
			int[] array = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
			int[] array2 = CosmeticsController.instance.tryOnSet.ToPackedIDArray();
			this.netView.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", player, new object[]
			{
				array,
				array2,
				false
			});
		}
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x00091510 File Offset: 0x0008F710
	public void PlayTagSoundLocal(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		if (soundIndex < 0 || soundIndex >= this.clipToPlay.Length)
		{
			return;
		}
		this.tagSound.volume = Mathf.Min(0.25f, soundVolume);
		if (stopCurrentAudio)
		{
			this.tagSound.Stop();
		}
		this.tagSound.GTPlayOneShot(this.clipToPlay[soundIndex], 1f);
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x00091569 File Offset: 0x0008F769
	public void AssignDrumToMusicDrums(int drumIndex, AudioSource drum)
	{
		if (drumIndex >= 0 && drumIndex < this.musicDrums.Length && drum != null)
		{
			this.musicDrums[drumIndex] = drum;
		}
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x0009158C File Offset: 0x0008F78C
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlayDrum");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			this.senderRig = rigContainer.Rig;
		}
		if (this.senderRig == null || this.senderRig.muted)
		{
			return;
		}
		if (drumIndex < 0 || drumIndex >= this.musicDrums.Length || (this.senderRig.transform.position - base.transform.position).sqrMagnitude > 9f || !float.IsFinite(drumVolume))
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent drum", player.UserId, player.NickName);
			return;
		}
		AudioSource audioSource = this.netView.IsMine ? GorillaTagger.Instance.offlineVRRig.musicDrums[drumIndex] : this.musicDrums[drumIndex];
		if (!audioSource.gameObject.activeInHierarchy)
		{
			return;
		}
		float instrumentVolume = GorillaComputer.instance.instrumentVolume;
		audioSource.time = 0f;
		audioSource.volume = Mathf.Max(Mathf.Min(instrumentVolume, drumVolume * instrumentVolume), 0f);
		audioSource.GTPlay();
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x000916C0 File Offset: 0x0008F8C0
	public int AssignInstrumentToInstrumentSelfOnly(TransferrableObject instrument)
	{
		if (instrument == null)
		{
			return -1;
		}
		if (!this.instrumentSelfOnly.Contains(instrument))
		{
			this.instrumentSelfOnly.Add(instrument);
		}
		return this.instrumentSelfOnly.IndexOf(instrument);
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x000916F4 File Offset: 0x0008F8F4
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySelfOnlyInstrument");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner && !this.muted)
		{
			if (selfOnlyIndex >= 0 && selfOnlyIndex < this.instrumentSelfOnly.Count && float.IsFinite(instrumentVol))
			{
				if (this.instrumentSelfOnly[selfOnlyIndex].gameObject.activeSelf)
				{
					this.instrumentSelfOnly[selfOnlyIndex].PlayNote(noteIndex, Mathf.Max(Mathf.Min(GorillaComputer.instance.instrumentVolume, instrumentVol * GorillaComputer.instance.instrumentVolume), 0f) / 2f);
					return;
				}
			}
			else
			{
				GorillaNot.instance.SendReport("inappropriate tag data being sent self only instrument", player.UserId, player.NickName);
			}
		}
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x000917D0 File Offset: 0x0008F9D0
	public void PlayHandTapLocal(int audioClipIndex, bool isLeftHand, float tapVolume)
	{
		if (audioClipIndex > -1 && audioClipIndex < GTPlayer.Instance.materialData.Count)
		{
			GTPlayer.MaterialData materialData = GTPlayer.Instance.materialData[audioClipIndex];
			AudioSource audioSource = isLeftHand ? this.leftHandPlayer : this.rightHandPlayer;
			audioSource.volume = tapVolume;
			AudioClip clip = materialData.overrideAudio ? materialData.audio : GTPlayer.Instance.materialData[0].audio;
			audioSource.GTPlayOneShot(clip, 1f);
		}
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x0009184D File Offset: 0x0008FA4D
	internal HandEffectContext GetHandEffect(bool isLeftHand, StiltID stiltID)
	{
		if (stiltID == StiltID.None)
		{
			if (!isLeftHand)
			{
				return this.RightHandEffect;
			}
			return this.LeftHandEffect;
		}
		else
		{
			if (!isLeftHand)
			{
				return this.ExtraRightHandEffect;
			}
			return this.ExtraLeftHandEffect;
		}
	}

	// Token: 0x06001B7D RID: 7037 RVA: 0x00091874 File Offset: 0x0008FA74
	internal void SetHandEffectData(HandEffectContext effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
	{
		VRMap vrmap = isLeftHand ? this.leftHand : this.rightHand;
		Vector3 vector = dirFromHitToHand * this.tapPointDistance * this.scaleFactor;
		if (this.isOfflineVRRig)
		{
			Vector3 vector2 = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * this.scaleFactor;
			Vector3 position = (stiltID != StiltID.None) ? GTPlayer.Instance.GetHandPosition(isLeftHand, stiltID) : (vrmap.rigTarget.position - vector2 + vector);
			effectContext.position = position;
			effectContext.handSoundSource.transform.position = position;
		}
		else
		{
			Quaternion quaternion = vrmap.rigTarget.parent.rotation * vrmap.syncRotation;
			Vector3 vector3 = this.netSyncPos.GetPredictedFuture() - base.transform.position;
			Vector3 vector2 = quaternion * vrmap.trackingPositionOffset * this.scaleFactor;
			effectContext.position = vrmap.rigTarget.parent.TransformPoint(vrmap.netSyncPos.GetPredictedFuture()) - vector2 + vector + vector3;
		}
		GTPlayer.MaterialData handSurfaceData = this.GetHandSurfaceData(audioClipIndex);
		HandTapOverrides handTapOverrides = isDownTap ? effectContext.DownTapOverrides : effectContext.UpTapOverrides;
		List<int> prefabHashes = effectContext.prefabHashes;
		int num = 0;
		HashWrapper hashWrapper = handTapOverrides.overrideSurfacePrefab ? handTapOverrides.surfaceTapPrefab : GTPlayer.Instance.materialDatasSO.surfaceEffects[handSurfaceData.surfaceEffectIndex];
		prefabHashes[num] = hashWrapper;
		effectContext.prefabHashes[1] = (ref handTapOverrides.overrideGamemodePrefab ? handTapOverrides.gamemodeTapPrefab : ((RoomSystem.JoinedRoom && GameMode.ActiveGameMode.IsNotNull()) ? GameMode.ActiveGameMode.SpecialHandFX(this.creator, this.rigContainer) : -1));
		effectContext.soundFX = (handTapOverrides.overrideSound ? handTapOverrides.tapSound : handSurfaceData.audio);
		effectContext.isDownTap = isDownTap;
		effectContext.isLeftHand = isLeftHand;
		effectContext.soundVolume = handTapVolume * this.handSpeedToVolumeModifier;
		effectContext.soundPitch = 1f;
		effectContext.speed = handTapSpeed;
		effectContext.color = this.playerColor;
	}

	// Token: 0x06001B7E RID: 7038 RVA: 0x00091AB0 File Offset: 0x0008FCB0
	internal GTPlayer.MaterialData GetHandSurfaceData(int index)
	{
		List<GTPlayer.MaterialData> materialData = GTPlayer.Instance.materialData;
		GTPlayer.MaterialData materialData2;
		if (index >= 0 && index < materialData.Count)
		{
			materialData2 = materialData[index];
		}
		else
		{
			materialData2 = materialData[0];
		}
		if (!materialData2.overrideAudio)
		{
			materialData2 = materialData[0];
		}
		return materialData2;
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x00091AF8 File Offset: 0x0008FCF8
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySplashEffect");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner)
		{
			float num = 10000f;
			if (splashPosition.IsValid(num) && splashRotation.IsValid() && float.IsFinite(splashScale) && float.IsFinite(boundingRadius))
			{
				if ((base.transform.position - splashPosition).sqrMagnitude >= 9f)
				{
					return;
				}
				float time = Time.time;
				int num2 = -1;
				float num3 = time + 10f;
				for (int i = 0; i < this.splashEffectTimes.Length; i++)
				{
					if (this.splashEffectTimes[i] < num3)
					{
						num3 = this.splashEffectTimes[i];
						num2 = i;
					}
				}
				if (time - 0.5f > num3)
				{
					this.splashEffectTimes[num2] = time;
					boundingRadius = Mathf.Clamp(boundingRadius, 0.0001f, 0.5f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);
					splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale, null);
					return;
				}
				return;
			}
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent splash effect", player.UserId, player.NickName);
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x00091C94 File Offset: 0x0008FE94
	[Rpc(1, 7)]
	public void RPC_EnableNonCosmeticHandItem(bool enable, bool isLeftHand, RpcInfo info = default(RpcInfo))
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.IncrementRPC(photonMessageInfoWrapped, "EnableNonCosmeticHandItem");
		if (photonMessageInfoWrapped.Sender == this.creator)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(photonMessageInfoWrapped.Sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
		}
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x00091D50 File Offset: 0x0008FF50
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		NetPlayer sender = info.Sender;
		this.IncrementRPC(info, "EnableNonCosmeticHandItem");
		if (sender == this.netView.Owner)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", info.Sender.UserId, info.Sender.NickName);
		}
	}

	// Token: 0x06001B82 RID: 7042 RVA: 0x00091E04 File Offset: 0x00090004
	public bool IsMakingFistLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(4) > 0.25f && ControllerInputPoller.TriggerFloat(4) > 0.25f;
		}
		return this.leftIndex.calcT > 0.25f && this.leftMiddle.calcT > 0.25f;
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x00091E5C File Offset: 0x0009005C
	public bool IsMakingFistRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(5) > 0.25f && ControllerInputPoller.TriggerFloat(5) > 0.25f;
		}
		return this.rightIndex.calcT > 0.25f && this.rightMiddle.calcT > 0.25f;
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x00091EB4 File Offset: 0x000900B4
	public bool IsMakingFiveLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(4) < 0.25f && ControllerInputPoller.TriggerFloat(4) < 0.25f;
		}
		return this.leftIndex.calcT < 0.25f && this.leftMiddle.calcT < 0.25f;
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x00091F0C File Offset: 0x0009010C
	public bool IsMakingFiveRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(5) < 0.25f && ControllerInputPoller.TriggerFloat(5) < 0.25f;
		}
		return this.rightIndex.calcT < 0.25f && this.rightMiddle.calcT < 0.25f;
	}

	// Token: 0x06001B86 RID: 7046 RVA: 0x00091F64 File Offset: 0x00090164
	public VRMap GetMakingFist(bool debug, out bool isLeftHand)
	{
		if (this.IsMakingFistRight())
		{
			isLeftHand = false;
			return this.rightHand;
		}
		if (this.IsMakingFistLeft())
		{
			isLeftHand = true;
			return this.leftHand;
		}
		isLeftHand = false;
		return null;
	}

	// Token: 0x06001B87 RID: 7047 RVA: 0x00091F90 File Offset: 0x00090190
	public void PlayGeodeEffect(Vector3 hitPosition)
	{
		if ((base.transform.position - hitPosition).sqrMagnitude < 9f && this.geodeCrackingSound)
		{
			this.geodeCrackingSound.GTPlay();
		}
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x00091FD8 File Offset: 0x000901D8
	public void PlayClimbSound(AudioClip clip, bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.leftHandPlayer.volume = 0.1f;
			this.leftHandPlayer.clip = clip;
			this.leftHandPlayer.GTPlayOneShot(this.leftHandPlayer.clip, 1f);
			return;
		}
		this.rightHandPlayer.volume = 0.1f;
		this.rightHandPlayer.clip = clip;
		this.rightHandPlayer.GTPlayOneShot(this.rightHandPlayer.clip, 1f);
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x00092058 File Offset: 0x00090258
	public void HideAllCosmetics(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "HideAllCosmetics");
		if (NetworkSystem.Instance.GetPlayer(info.Sender) == this.netView.Owner)
		{
			this.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet, false);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x000920C8 File Offset: 0x000902C8
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, bool playfx, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && currentItems.Length == 16 && tryOnItems.Length == 16)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet, playfx);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x0009215C File Offset: 0x0009035C
	public void UpdateCosmeticsWithTryon(int[] currentItemsPacked, int[] tryOnItemsPacked, bool playfx, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && CosmeticsController.instance.ValidatePackedItems(currentItemsPacked) && CosmeticsController.instance.ValidatePackedItems(tryOnItemsPacked))
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItemsPacked, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItemsPacked, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet, playfx);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x000921FD File Offset: 0x000903FD
	public void LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet newSet, CosmeticsController.CosmeticSet newTryOnSet, bool playfx)
	{
		this.cosmeticSet = newSet;
		this.tryOnSet = newTryOnSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive(playfx);
		}
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x0009221C File Offset: 0x0009041C
	private void CheckForEarlyAccess()
	{
		if (this.concatStringOfCosmeticsAllowed.Contains("Early Access Supporter Pack"))
		{
			this.concatStringOfCosmeticsAllowed += "LBAAE.LFAAM.LFAAN.LHAAA.LHAAK.LHAAL.LHAAM.LHAAN.LHAAO.LHAAP.LHABA.LHABB.";
		}
		this.InitializedCosmetics = true;
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x00092250 File Offset: 0x00090450
	public void SetCosmeticsActive(bool playfx)
	{
		if (CosmeticsController.instance == null || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		this.prevSet.CopyItems(this.mergedSet);
		this.mergedSet.MergeSets(this.inTryOnRoom ? this.tryOnSet : null, this.cosmeticSet);
		BodyDockPositions component = base.GetComponent<BodyDockPositions>();
		this.mergedSet.ActivateCosmetics(this.prevSet, this, component, this.cosmeticsObjectRegistry);
		if (!playfx)
		{
			return;
		}
		if (this.cosmeticsActivationPS != null)
		{
			this.cosmeticsActivationPS.Play();
		}
		if (this.cosmeticsActivationSBP != null)
		{
			this.cosmeticsActivationSBP.Play();
		}
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x000922FD File Offset: 0x000904FD
	public void RefreshCosmetics()
	{
		this.mergedSet.ActivateCosmetics(this.mergedSet, this, this.myBodyDockPositions, this.cosmeticsObjectRegistry);
		this.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x00092328 File Offset: 0x00090528
	public void GetCosmeticsPlayFabCatalogData()
	{
		if (CosmeticsController.instance != null)
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (ItemInstance itemInstance in result.Inventory)
				{
					if (!dictionary.ContainsKey(itemInstance.ItemId))
					{
						dictionary[itemInstance.ItemId] = itemInstance.ItemId;
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog)
						{
							this.concatStringOfCosmeticsAllowed += itemInstance.ItemId;
						}
					}
				}
				if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
				{
					this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
				}
			}, delegate(PlayFabError error)
			{
				this.initializedCosmetics = true;
				if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
				{
					this.SetCosmeticsActive(false);
				}
			}, null, null);
		}
		this.concatStringOfCosmeticsAllowed += "Slingshot";
		this.concatStringOfCosmeticsAllowed += BuilderSetManager.instance.GetStarterSetsConcat();
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x0009239C File Offset: 0x0009059C
	public void GenerateFingerAngleLookupTables()
	{
		this.GenerateTableIndex(ref this.leftIndex);
		this.GenerateTableIndex(ref this.rightIndex);
		this.GenerateTableMiddle(ref this.leftMiddle);
		this.GenerateTableMiddle(ref this.rightMiddle);
		this.GenerateTableThumb(ref this.leftThumb);
		this.GenerateTableThumb(ref this.rightThumb);
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x000923F4 File Offset: 0x000905F4
	private void GenerateTableThumb(ref VRMapThumb thumb)
	{
		thumb.angle1Table = new Quaternion[11];
		thumb.angle2Table = new Quaternion[11];
		for (int i = 0; i < thumb.angle1Table.Length; i++)
		{
			thumb.angle1Table[i] = Quaternion.Lerp(thumb.startingAngle1Quat, thumb.closedAngle1Quat, (float)i / 10f);
			thumb.angle2Table[i] = Quaternion.Lerp(thumb.startingAngle2Quat, thumb.closedAngle2Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x00092484 File Offset: 0x00090684
	private void GenerateTableIndex(ref VRMapIndex index)
	{
		index.angle1Table = new Quaternion[11];
		index.angle2Table = new Quaternion[11];
		index.angle3Table = new Quaternion[11];
		for (int i = 0; i < index.angle1Table.Length; i++)
		{
			index.angle1Table[i] = Quaternion.Lerp(index.startingAngle1Quat, index.closedAngle1Quat, (float)i / 10f);
			index.angle2Table[i] = Quaternion.Lerp(index.startingAngle2Quat, index.closedAngle2Quat, (float)i / 10f);
			index.angle3Table[i] = Quaternion.Lerp(index.startingAngle3Quat, index.closedAngle3Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x0009254C File Offset: 0x0009074C
	private void GenerateTableMiddle(ref VRMapMiddle middle)
	{
		middle.angle1Table = new Quaternion[11];
		middle.angle2Table = new Quaternion[11];
		middle.angle3Table = new Quaternion[11];
		for (int i = 0; i < middle.angle1Table.Length; i++)
		{
			middle.angle1Table[i] = Quaternion.Lerp(middle.startingAngle1Quat, middle.closedAngle1Quat, (float)i / 10f);
			middle.angle2Table[i] = Quaternion.Lerp(middle.startingAngle2Quat, middle.closedAngle2Quat, (float)i / 10f);
			middle.angle3Table[i] = Quaternion.Lerp(middle.startingAngle3Quat, middle.closedAngle3Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x00092614 File Offset: 0x00090814
	private Quaternion SanitizeQuaternion(Quaternion quat)
	{
		if (float.IsNaN(quat.w) || float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsInfinity(quat.w) || float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z))
		{
			return Quaternion.identity;
		}
		return quat;
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x00092690 File Offset: 0x00090890
	private Vector3 SanitizeVector3(Vector3 vec)
	{
		if (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z) || float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z))
		{
			return Vector3.zero;
		}
		return Vector3.ClampMagnitude(vec, 5000f);
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x000926FC File Offset: 0x000908FC
	private void IncrementRPC(PhotonMessageInfoWrapped info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x00092712 File Offset: 0x00090912
	private void IncrementRPC(PhotonMessageInfo info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x06001B99 RID: 7065 RVA: 0x00092728 File Offset: 0x00090928
	private void AddVelocityToQueue(Vector3 position, double serverTime)
	{
		Vector3 velocity = Vector3.zero;
		if (this.velocityHistoryList.Count > 0)
		{
			double num = Utils.CalculateNetworkDeltaTime(this.velocityHistoryList[0].time, serverTime);
			if (num == 0.0)
			{
				return;
			}
			velocity = (position - this.lastPosition) / (float)num;
		}
		this.velocityHistoryList.Add(new VRRig.VelocityTime(velocity, serverTime));
		this.lastPosition = position;
	}

	// Token: 0x06001B9A RID: 7066 RVA: 0x0009279C File Offset: 0x0009099C
	private Vector3 ReturnVelocityAtTime(double timeToReturn)
	{
		if (this.velocityHistoryList.Count <= 1)
		{
			return Vector3.zero;
		}
		int num = 0;
		int num2 = this.velocityHistoryList.Count - 1;
		int num3 = 0;
		if (num2 == num)
		{
			return this.velocityHistoryList[num].vel;
		}
		while (num2 - num > 1 && num3 < 1000)
		{
			num3++;
			int num4 = (num2 - num) / 2;
			if (this.velocityHistoryList[num4].time > timeToReturn)
			{
				num2 = num4;
			}
			else
			{
				num = num4;
			}
		}
		float num5 = (float)(this.velocityHistoryList[num].time - timeToReturn);
		double num6 = this.velocityHistoryList[num].time - this.velocityHistoryList[num2].time;
		if (num6 == 0.0)
		{
			num6 = 0.001;
		}
		num5 /= (float)num6;
		num5 = Mathf.Clamp(num5, 0f, 1f);
		return Vector3.Lerp(this.velocityHistoryList[num].vel, this.velocityHistoryList[num2].vel, num5);
	}

	// Token: 0x06001B9B RID: 7067 RVA: 0x000928AE File Offset: 0x00090AAE
	public Vector3 LatestVelocity()
	{
		if (this.velocityHistoryList.Count > 0)
		{
			return this.velocityHistoryList[0].vel;
		}
		return Vector3.zero;
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x000928D5 File Offset: 0x00090AD5
	public bool IsPositionInRange(Vector3 position, float range)
	{
		return (this.syncPos - position).IsShorterThan(range * this.scaleFactor);
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x000928F0 File Offset: 0x00090AF0
	public bool CheckTagDistanceRollback(VRRig otherRig, float max, float timeInterval)
	{
		Vector3 vector;
		Vector3 vector2;
		GorillaMath.LineSegClosestPoints(this.syncPos, -this.LatestVelocity() * timeInterval, otherRig.syncPos, -otherRig.LatestVelocity() * timeInterval, out vector, out vector2);
		return Vector3.SqrMagnitude(vector - vector2) < max * max * this.scaleFactor;
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x0009294C File Offset: 0x00090B4C
	public Vector3 ClampVelocityRelativeToPlayerSafe(Vector3 inVel, float max, float teleportSpeedThreshold = 100f)
	{
		max *= this.scaleFactor;
		Vector3 vector = Vector3.zero;
		ref vector.SetValueSafe(inVel);
		Vector3 vector2 = (this.velocityHistoryList.Count > 0) ? this.velocityHistoryList[0].vel : Vector3.zero;
		if (vector2.sqrMagnitude > teleportSpeedThreshold * teleportSpeedThreshold)
		{
			vector2 = Vector3.zero;
		}
		Vector3 vector3 = vector - vector2;
		vector3 = Vector3.ClampMagnitude(vector3, max);
		vector = vector2 + vector3;
		return vector;
	}

	// Token: 0x14000039 RID: 57
	// (add) Token: 0x06001B9F RID: 7071 RVA: 0x000929C4 File Offset: 0x00090BC4
	// (remove) Token: 0x06001BA0 RID: 7072 RVA: 0x000929FC File Offset: 0x00090BFC
	public event Action<Color> OnColorChanged;

	// Token: 0x1400003A RID: 58
	// (add) Token: 0x06001BA1 RID: 7073 RVA: 0x00092A34 File Offset: 0x00090C34
	// (remove) Token: 0x06001BA2 RID: 7074 RVA: 0x00092A6C File Offset: 0x00090C6C
	public event Action OnPlayerNameVisibleChanged;

	// Token: 0x06001BA3 RID: 7075 RVA: 0x00092AA4 File Offset: 0x00090CA4
	public void SetColor(Color color)
	{
		Action<Color> onColorChanged = this.OnColorChanged;
		if (onColorChanged != null)
		{
			onColorChanged.Invoke(color);
		}
		Action<Color> action = this.onColorInitialized;
		if (action != null)
		{
			action.Invoke(color);
		}
		this.onColorInitialized = delegate(Color color1)
		{
		};
		this.colorInitialized = true;
		this.playerColor = color;
		if (this.OnDataChange != null)
		{
			this.OnDataChange.Invoke();
		}
	}

	// Token: 0x06001BA4 RID: 7076 RVA: 0x00092B1B File Offset: 0x00090D1B
	public void OnColorInitialized(Action<Color> action)
	{
		if (this.colorInitialized)
		{
			action.Invoke(this.playerColor);
			return;
		}
		this.onColorInitialized = (Action<Color>)Delegate.Combine(this.onColorInitialized, action);
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x00092B49 File Offset: 0x00090D49
	private void SendScoresToRoom()
	{
		if (this.netView != null && this._scoreUpdated)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", 1, new object[]
			{
				this.currentQuestScore
			});
		}
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x00092B88 File Offset: 0x00090D88
	private void SendScoresToGameModeRoom(GameModeType newGameModeType)
	{
		if (this.netView != null && this._rankedInfoUpdated && newGameModeType != GameModeType.InfectionCompetitive && !this.m_sentRankedScore)
		{
			this.m_sentRankedScore = true;
			this.netView.SendRPC("RPC_UpdateRankedInfo", 1, new object[]
			{
				this.currentRankedELO,
				this.currentRankedSubTierQuest,
				this.currentRankedSubTierPC
			});
		}
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x00092C00 File Offset: 0x00090E00
	private void SendScoresToNewPlayer(NetPlayer player)
	{
		if (this.netView != null)
		{
			if (this._scoreUpdated)
			{
				this.netView.SendRPC("RPC_UpdateQuestScore", player, new object[]
				{
					this.currentQuestScore
				});
			}
			if (this._rankedInfoUpdated && !this.IsInRankedMode())
			{
				this.netView.SendRPC("RPC_UpdateRankedInfo", player, new object[]
				{
					this.currentRankedELO,
					this.currentRankedSubTierQuest,
					this.currentRankedSubTierPC
				});
			}
		}
	}

	// Token: 0x1400003B RID: 59
	// (add) Token: 0x06001BA8 RID: 7080 RVA: 0x00092C9C File Offset: 0x00090E9C
	// (remove) Token: 0x06001BA9 RID: 7081 RVA: 0x00092CD4 File Offset: 0x00090ED4
	public event Action<int> OnQuestScoreChanged;

	// Token: 0x06001BAA RID: 7082 RVA: 0x00092D0C File Offset: 0x00090F0C
	public void SetQuestScore(int score)
	{
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged != null)
		{
			onQuestScoreChanged.Invoke(this.currentQuestScore);
		}
		if (this.netView != null)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", 1, new object[]
			{
				this.currentQuestScore
			});
		}
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x00092D6A File Offset: 0x00090F6A
	public int GetCurrentQuestScore()
	{
		if (!this._scoreUpdated)
		{
			this.SetQuestScoreLocal(ProgressionController.TotalPoints);
		}
		return this.currentQuestScore;
	}

	// Token: 0x06001BAC RID: 7084 RVA: 0x00092D85 File Offset: 0x00090F85
	private void SetQuestScoreLocal(int score)
	{
		this.currentQuestScore = score;
		this._scoreUpdated = true;
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x00092D98 File Offset: 0x00090F98
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateQuestScore");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.senderID != this.creator.ActorNumber)
		{
			return;
		}
		if (!this.updateQuestCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		if (score < this.currentQuestScore)
		{
			return;
		}
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged == null)
		{
			return;
		}
		onQuestScoreChanged.Invoke(this.currentQuestScore);
	}

	// Token: 0x1400003C RID: 60
	// (add) Token: 0x06001BAE RID: 7086 RVA: 0x00092E10 File Offset: 0x00091010
	// (remove) Token: 0x06001BAF RID: 7087 RVA: 0x00092E48 File Offset: 0x00091048
	public event Action<int, int> OnRankedSubtierChanged;

	// Token: 0x06001BB0 RID: 7088 RVA: 0x00092E80 File Offset: 0x00091080
	public void SetRankedInfo(float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, bool broadcastToOtherClients = true)
	{
		this.SetRankedInfoLocal(rankedELO, rankedSubtierQuest, rankedSubtierPC);
		Action<int, int> onRankedSubtierChanged = this.OnRankedSubtierChanged;
		if (onRankedSubtierChanged != null)
		{
			onRankedSubtierChanged.Invoke(rankedSubtierQuest, rankedSubtierPC);
		}
		if (this.netView != null && broadcastToOtherClients)
		{
			this.netView.SendRPC("RPC_UpdateRankedInfo", 1, new object[]
			{
				this.currentRankedELO,
				this.currentRankedSubTierQuest,
				this.currentRankedSubTierPC
			});
		}
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x00092EFB File Offset: 0x000910FB
	public int GetCurrentRankedSubTier(bool getPC)
	{
		if (!this._rankedInfoUpdated)
		{
			return -1;
		}
		if (!getPC)
		{
			return this.currentRankedSubTierQuest;
		}
		return this.currentRankedSubTierPC;
	}

	// Token: 0x06001BB2 RID: 7090 RVA: 0x00092F17 File Offset: 0x00091117
	private void SetRankedInfoLocal(float rankedELO, int rankedSubTierQuest, int rankedSubTierPC)
	{
		this.currentRankedELO = rankedELO;
		this.currentRankedSubTierQuest = rankedSubTierQuest;
		this.currentRankedSubTierPC = rankedSubTierPC;
		this._rankedInfoUpdated = true;
	}

	// Token: 0x06001BB3 RID: 7091 RVA: 0x00092F35 File Offset: 0x00091135
	private bool IsInRankedMode()
	{
		return GameMode.ActiveGameMode != null && GameMode.ActiveGameMode.GameType() == GameModeType.InfectionCompetitive;
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x00092F54 File Offset: 0x00091154
	public void UpdateRankedInfo(float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateRankedInfo");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return;
		}
		if (!rigContainer.Rig.updateRankedInfoCallLimit.CheckCallTime(Time.time) || info.senderID != this.creator.ActorNumber || !float.IsFinite(rankedELO))
		{
			return;
		}
		if (this.IsInRankedMode())
		{
			return;
		}
		if (RankedProgressionManager.Instance == null || !RankedProgressionManager.Instance.AreValuesValid(rankedELO, rankedSubtierQuest, rankedSubtierPC))
		{
			return;
		}
		this.SetRankedInfoLocal(rankedELO, rankedSubtierQuest, rankedSubtierPC);
		Action<int, int> onRankedSubtierChanged = this.OnRankedSubtierChanged;
		if (onRankedSubtierChanged != null)
		{
			onRankedSubtierChanged.Invoke(rankedSubtierQuest, rankedSubtierPC);
		}
		RankedProgressionManager.Instance.HandlePlayerRankedInfoReceived(this.creator.ActorNumber, rankedELO, rankedSubtierPC);
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x00093024 File Offset: 0x00091224
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
		GorillaComputer.RegisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
		if (this.currentRopeSwingTarget != null)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		if (!this.isOfflineVRRig)
		{
			PlayerCosmeticsSystem.RegisterCosmeticCallback(this.creator.ActorNumber, this);
		}
		this.bodyRenderer.SetDefaults();
		this.SetInvisibleToLocalPlayer(false);
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride += this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride += this.HandHold_HandPositionReleaseOverride;
			GameMode.OnStartGameMode += this.SendScoresToGameModeRoom;
			RoomSystem.JoinedRoomEvent += new Action(this.SendScoresToRoom);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.SendScoresToNewPlayer);
		}
		else
		{
			VRRigJobManager.Instance.RegisterVRRig(this);
		}
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06001BB6 RID: 7094 RVA: 0x00093118 File Offset: 0x00091318
	void IPreDisable.PreDisable()
	{
		try
		{
			this.ClearRopeData();
			if (this.currentRopeSwingTarget)
			{
				this.currentRopeSwingTarget.SetParent(base.transform);
			}
			this.EnableHuntWatch(false);
			this.EnablePaintbrawlCosmetics(false);
			this.EnableSuperInfectionHands(false);
			this.ClearPartyMemberStatus();
			this.concatStringOfCosmeticsAllowed = "";
			this.rawCosmeticString = "";
			if (this.cosmeticSet != null)
			{
				this.mergedSet.DeactivateAllCosmetcs(this.myBodyDockPositions, CosmeticsController.instance.nullItem, this.cosmeticsObjectRegistry);
				this.mergedSet.ClearSet(CosmeticsController.instance.nullItem);
				this.prevSet.ClearSet(CosmeticsController.instance.nullItem);
				this.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
				this.cosmeticSet.ClearSet(CosmeticsController.instance.nullItem);
			}
			if (!this.isOfflineVRRig)
			{
				PlayerCosmeticsSystem.RemoveCosmeticCallback(this.creator.ActorNumber);
				this.pendingCosmeticUpdate = true;
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this.leftHandLink);
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this.rightHandLink);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this.leftHandLink);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this.rightHandLink);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06001BB7 RID: 7095 RVA: 0x00093298 File Offset: 0x00091498
	public void OnDisable()
	{
		try
		{
			GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.gameMode);
			this.ChangeMaterialLocal(0);
			GorillaComputer.UnregisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
			this.netView = null;
			this.voiceAudio = null;
			this.muted = false;
			this.initialized = false;
			this.initializedCosmetics = false;
			this.inTryOnRoom = false;
			this.timeSpawned = 0f;
			this.setMatIndex = 0;
			this.currentCosmeticTries = 0;
			this.velocityHistoryList.Clear();
			this.netSyncPos.Reset();
			this.rightHand.netSyncPos.Reset();
			this.leftHand.netSyncPos.Reset();
			this.ForceResetFrozenEffect();
			this.nativeScale = (this.frameScale = (this.lastScaleFactor = 1f));
			base.transform.localScale = Vector3.one;
			this.currentQuestScore = 0;
			this._scoreUpdated = false;
			this.currentRankedELO = 0f;
			this.currentRankedSubTierQuest = 0;
			this.currentRankedSubTierPC = 0;
			this._rankedInfoUpdated = false;
			this.TemporaryCosmeticEffects.Clear();
			this.m_sentRankedScore = false;
			try
			{
				CallLimitType<CallLimiter>[] callSettings = this.fxSettings.callSettings;
				for (int i = 0; i < callSettings.Length; i++)
				{
					callSettings[i].CallLimitSettings.Reset();
				}
			}
			catch
			{
				Debug.LogError("fxtype missing in fxSettings, please fix or remove this");
			}
		}
		catch (Exception)
		{
		}
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride -= this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride -= this.HandHold_HandPositionReleaseOverride;
			GameMode.OnStartGameMode -= this.SendScoresToGameModeRoom;
			RoomSystem.JoinedRoomEvent -= new Action(this.SendScoresToRoom);
			RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.SendScoresToNewPlayer);
		}
		else
		{
			VRRigJobManager.Instance.DeregisterVRRig(this);
		}
		EyeScannerMono.Unregister(this);
		this.creator = null;
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06001BB8 RID: 7096 RVA: 0x000934B8 File Offset: 0x000916B8
	private void HandHold_HandPositionReleaseOverride(HandHold hh, bool leftHand)
	{
		if (leftHand)
		{
			this.leftHand.handholdOverrideTarget = null;
			return;
		}
		this.rightHand.handholdOverrideTarget = null;
	}

	// Token: 0x06001BB9 RID: 7097 RVA: 0x000934D6 File Offset: 0x000916D6
	private void HandHold_HandPositionRequestOverride(HandHold hh, bool leftHand, Vector3 pos)
	{
		if (leftHand)
		{
			this.leftHand.handholdOverrideTarget = hh.transform;
			this.leftHand.handholdOverrideTargetOffset = pos;
			return;
		}
		this.rightHand.handholdOverrideTarget = hh.transform;
		this.rightHand.handholdOverrideTargetOffset = pos;
	}

	// Token: 0x06001BBA RID: 7098 RVA: 0x00093518 File Offset: 0x00091718
	public void NetInitialize()
	{
		this.timeSpawned = Time.time;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null)
			{
				if (instance is GorillaHuntManager || instance.GameModeName() == "HUNT")
				{
					this.EnableHuntWatch(true);
				}
				else if (instance is GorillaPaintbrawlManager || instance.GameModeName() == "PAINTBRAWL")
				{
					this.EnablePaintbrawlCosmetics(true);
				}
			}
			else
			{
				string gameModeString = NetworkSystem.Instance.GameModeString;
				if (!gameModeString.IsNullOrEmpty())
				{
					string text = gameModeString;
					if (text.Contains("HUNT"))
					{
						this.EnableHuntWatch(true);
					}
					else if (text.Contains("PAINTBRAWL"))
					{
						this.EnablePaintbrawlCosmetics(true);
					}
				}
			}
			this.UpdateFriendshipBracelet();
			if (this.IsLocalPartyMember && !this.isOfflineVRRig)
			{
				FriendshipGroupDetection.Instance.SendVerifyPartyMember(this.creator);
			}
		}
		if (this.netView != null)
		{
			base.transform.position = this.netView.gameObject.transform.position;
			base.transform.rotation = this.netView.gameObject.transform.rotation;
		}
		try
		{
			Action action = VRRig.newPlayerJoined;
			if (action != null)
			{
				action.Invoke();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x00093674 File Offset: 0x00091874
	public void GrabbedByPlayer(VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand)
	{
		GorillaClimbable climbable = grabbedWithLeftHand ? grabbedByRig.leftHandHoldsPlayer : grabbedByRig.rightHandHoldsPlayer;
		GorillaHandClimber gorillaHandClimber;
		if (grabbedBody)
		{
			gorillaHandClimber = EquipmentInteractor.instance.BodyClimber;
		}
		else if (grabbedLeftHand)
		{
			gorillaHandClimber = EquipmentInteractor.instance.LeftClimber;
		}
		else
		{
			gorillaHandClimber = EquipmentInteractor.instance.RightClimber;
		}
		gorillaHandClimber.SetCanRelease(false);
		GTPlayer.Instance.BeginClimbing(climbable, gorillaHandClimber, null);
		this.grabbedRopeIsBody = grabbedBody;
		this.grabbedRopeIsLeft = grabbedLeftHand;
		this.grabbedRopeIndex = grabbedByRig.netView.ViewID;
		this.grabbedRopeBoneIndex = (grabbedWithLeftHand ? 1 : 0);
		this.grabbedRopeOffset = Vector3.zero;
		this.grabbedRopeIsPhotonView = true;
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x00093718 File Offset: 0x00091918
	public void DroppedByPlayer(VRRig grabbedByRig, Vector3 throwVelocity)
	{
		GorillaClimbable currentClimbable = GTPlayer.Instance.CurrentClimbable;
		if (GTPlayer.Instance.isClimbing && (currentClimbable == grabbedByRig.leftHandHoldsPlayer || currentClimbable == grabbedByRig.rightHandHoldsPlayer))
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 20f);
			GorillaHandClimber currentClimber = GTPlayer.Instance.CurrentClimber;
			GTPlayer.Instance.EndClimbing(currentClimber, false, false);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			this.grabbedRopeIsBody = false;
			this.grabbedRopeIsLeft = false;
			this.grabbedRopeIndex = -1;
			this.grabbedRopeBoneIndex = 0;
			this.grabbedRopeOffset = Vector3.zero;
			this.grabbedRopeIsPhotonView = false;
			return;
		}
		if (VRRig.LocalRig.leftHandLink.IsLinkActive() && VRRig.LocalRig.leftHandLink.grabbedLink.myRig == grabbedByRig)
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 3f);
			VRRig.LocalRig.leftHandLink.BreakLink();
			VRRig.LocalRig.leftHandLink.RejectGrabsFor(1f);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			return;
		}
		if (VRRig.LocalRig.rightHandLink.IsLinkActive() && VRRig.LocalRig.rightHandLink.grabbedLink.myRig == grabbedByRig)
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 3f);
			VRRig.LocalRig.rightHandLink.BreakLink();
			VRRig.LocalRig.rightHandLink.RejectGrabsFor(1f);
			GTPlayer.Instance.SetVelocity(throwVelocity);
		}
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x00093888 File Offset: 0x00091A88
	public bool IsOnGround(float headCheckDistance, float handCheckDistance, out Vector3 groundNormal)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(position, Vector3.down * headCheckDistance * this.scaleFactor, instance.headCollider.radius * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		if (this.LocalCheckCollision(position2, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		if (this.LocalCheckCollision(position3, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		groundNormal = Vector3.up;
		return false;
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x0009399C File Offset: 0x00091B9C
	private bool LocalTestMovementCollision(Vector3 startPosition, Vector3 startVelocity, out Vector3 modifiedVelocity, out Vector3 finalPosition)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 vector = startVelocity * Time.deltaTime;
		finalPosition = startPosition + vector;
		modifiedVelocity = startVelocity;
		Vector3 vector2;
		RaycastHit raycastHit;
		bool flag = this.LocalCheckCollision(startPosition, vector, instance.headCollider.radius * this.scaleFactor, out vector2, out raycastHit);
		if (flag)
		{
			finalPosition = vector2 - vector.normalized * 0.01f;
			modifiedVelocity = startVelocity - raycastHit.normal * Vector3.Dot(raycastHit.normal, startVelocity);
		}
		Vector3 position = this.leftHand.rigTarget.position;
		Vector3 vector3;
		RaycastHit raycastHit2;
		bool flag2 = this.LocalCheckCollision(position, vector, instance.minimumRaycastDistance * this.scaleFactor, out vector3, out raycastHit2);
		if (flag2)
		{
			finalPosition = vector3 - (this.leftHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		Vector3 position2 = this.rightHand.rigTarget.position;
		Vector3 vector4;
		RaycastHit raycastHit3;
		bool flag3 = this.LocalCheckCollision(position2, vector, instance.minimumRaycastDistance * this.scaleFactor, out vector4, out raycastHit3);
		if (flag3)
		{
			finalPosition = vector4 - (this.rightHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		return flag || flag2 || flag3;
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x00093B2C File Offset: 0x00091D2C
	public void TrySweptMoveTo(Vector3 targetPosition, out bool handCollided, out bool buttCollided)
	{
		Vector3 position = base.transform.position;
		this.TrySweptOffsetMove(targetPosition - position, out handCollided, out buttCollided);
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x00093B54 File Offset: 0x00091D54
	public void TrySweptOffsetMove(Vector3 movement, out bool handCollided, out bool buttCollided)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector = position + movement;
		Vector3 startPosition = position;
		handCollided = false;
		buttCollided = false;
		Vector3 vector2;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(startPosition, movement, instance.headCollider.radius * this.scaleFactor, out vector2, out raycastHit))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = vector2 - movement.normalized * 0.01f;
			}
			movement = vector - position;
			buttCollided = true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		Vector3 vector3;
		RaycastHit raycastHit2;
		if (this.LocalCheckCollision(position2, movement, instance.minimumRaycastDistance * this.scaleFactor, out vector3, out raycastHit2))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = vector3 - (this.leftHand.rigTarget.position - position) - movement.normalized * 0.01f;
			}
			movement = vector - position;
			handCollided = true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		Vector3 vector4;
		RaycastHit raycastHit3;
		if (this.LocalCheckCollision(position3, movement, instance.minimumRaycastDistance * this.scaleFactor, out vector4, out raycastHit3))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = vector4 - (this.rightHand.rigTarget.position - position) - movement.normalized * 0.01f;
			}
			movement = vector - position;
			handCollided = true;
		}
		base.transform.position = vector;
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x00093CE4 File Offset: 0x00091EE4
	private bool LocalCheckCollision(Vector3 startPosition, Vector3 movement, float radius, out Vector3 finalPosition, out RaycastHit hit)
	{
		GTPlayer instance = GTPlayer.Instance;
		finalPosition = startPosition + movement;
		RaycastHit raycastHit = default(RaycastHit);
		bool flag = false;
		Vector3 normalized = movement.normalized;
		int num = Physics.SphereCastNonAlloc(startPosition, radius, normalized, this.rayCastNonAllocColliders, movement.magnitude, instance.locomotionEnabledLayers.value);
		if (num > 0)
		{
			raycastHit = this.rayCastNonAllocColliders[0];
			for (int i = 0; i < num; i++)
			{
				if (raycastHit.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < raycastHit.distance))
				{
					flag = true;
					raycastHit = this.rayCastNonAllocColliders[i];
				}
			}
		}
		hit = raycastHit;
		if (flag)
		{
			finalPosition = startPosition + normalized * (raycastHit.distance - 0.01f);
			return true;
		}
		return false;
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x00093DC8 File Offset: 0x00091FC8
	public void UpdateFriendshipBracelet()
	{
		bool flag = false;
		if (this.isOfflineVRRig)
		{
			bool flag2 = false;
			VRRig.PartyMemberStatus partyMemberStatus = this.GetPartyMemberStatus();
			if (partyMemberStatus != VRRig.PartyMemberStatus.InLocalParty)
			{
				if (partyMemberStatus == VRRig.PartyMemberStatus.NotInLocalParty)
				{
					flag2 = false;
					this.reliableState.isBraceletLeftHanded = false;
				}
			}
			else
			{
				flag2 = true;
				this.reliableState.isBraceletLeftHanded = (FriendshipGroupDetection.Instance.DidJoinLeftHanded && !this.huntComputer.activeSelf);
			}
			if (this.reliableState.HasBracelet != flag2 || this.reliableState.braceletBeadColors.Count != FriendshipGroupDetection.Instance.myBeadColors.Count)
			{
				this.reliableState.SetIsDirty();
				flag = (this.reliableState.HasBracelet == flag2);
			}
			this.reliableState.braceletBeadColors.Clear();
			if (flag2)
			{
				this.reliableState.braceletBeadColors.AddRange(FriendshipGroupDetection.Instance.myBeadColors);
			}
			this.reliableState.braceletSelfIndex = FriendshipGroupDetection.Instance.MyBraceletSelfIndex;
		}
		if (this.nonCosmeticLeftHandItem != null)
		{
			bool flag3 = this.reliableState.HasBracelet && this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticLeftHandItem.EnableItem(flag3);
			if (flag3)
			{
				this.friendshipBraceletLeftHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletLeftHand.PlayAppearEffects();
				}
			}
		}
		if (this.nonCosmeticRightHandItem != null)
		{
			bool flag4 = this.reliableState.HasBracelet && !this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticRightHandItem.EnableItem(flag4);
			if (flag4)
			{
				this.friendshipBraceletRightHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletRightHand.PlayAppearEffects();
				}
			}
		}
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x00093F98 File Offset: 0x00092198
	public void EnableHuntWatch(bool on)
	{
		this.huntComputer.SetActive(on);
		if (this.builderResizeWatch != null)
		{
			MeshRenderer component = this.builderResizeWatch.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = !on;
			}
		}
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x00093FDE File Offset: 0x000921DE
	public void EnablePaintbrawlCosmetics(bool on)
	{
		this.paintbrawlBalloons.gameObject.SetActive(on);
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x00093FF4 File Offset: 0x000921F4
	public void EnableBuilderResizeWatch(bool on)
	{
		if (this.builderResizeWatch != null && this.builderResizeWatch.activeSelf != on)
		{
			this.builderResizeWatch.SetActive(on);
			if (this.builderArmShelfLeft != null)
			{
				this.builderArmShelfLeft.gameObject.SetActive(on);
			}
			if (this.builderArmShelfRight != null)
			{
				this.builderArmShelfRight.gameObject.SetActive(on);
			}
		}
		if (this.isOfflineVRRig)
		{
			bool flag = this.reliableState.isBuilderWatchEnabled != on;
			this.reliableState.isBuilderWatchEnabled = on;
			if (flag)
			{
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x00094099 File Offset: 0x00092299
	public void EnableGuardianEjectWatch(bool on)
	{
		if (this.guardianEjectWatch != null && this.guardianEjectWatch.activeSelf != on)
		{
			this.guardianEjectWatch.SetActive(on);
		}
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x000940C3 File Offset: 0x000922C3
	public void EnableVStumpReturnWatch(bool on)
	{
		if (this.vStumpReturnWatch != null && this.vStumpReturnWatch.activeSelf != on)
		{
			this.vStumpReturnWatch.SetActive(on);
		}
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x000940ED File Offset: 0x000922ED
	public void EnableRankedTimerWatch(bool on)
	{
		if (this.rankedTimerWatch != null && this.rankedTimerWatch.activeSelf != on)
		{
			this.rankedTimerWatch.SetActive(on);
		}
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x00094117 File Offset: 0x00092317
	public void EnableSuperInfectionHands(bool on)
	{
		if (this.superInfectionHand != null)
		{
			this.superInfectionHand.EnableHands(on);
		}
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x00094134 File Offset: 0x00092334
	private void UpdateReplacementVoice()
	{
		if (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn != "TRUE")
		{
			this.voiceAudio.mute = true;
			return;
		}
		this.voiceAudio.mute = false;
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x00094184 File Offset: 0x00092384
	public bool ShouldPlayReplacementVoice()
	{
		return this.netView && !this.netView.IsMine && !(GorillaComputer.instance.voiceChatOn == "OFF") && (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && this.SpeakingLoudness > this.replacementVoiceLoudnessThreshold;
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x000941FF File Offset: 0x000923FF
	public void SetDuplicationZone(RigDuplicationZone duplicationZone)
	{
		this.duplicationZone = duplicationZone;
		this.inDuplicationZone = (duplicationZone != null);
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x00094215 File Offset: 0x00092415
	public void ClearDuplicationZone(RigDuplicationZone duplicationZone)
	{
		if (this.duplicationZone == duplicationZone)
		{
			this.SetDuplicationZone(null);
			this.renderTransform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x0009423C File Offset: 0x0009243C
	public void ResetTimeSpawned()
	{
		this.timeSpawned = Time.time;
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x0009424C File Offset: 0x0009244C
	public void SetGooParticleSystemStatus(bool isLeftHand, bool isEnabled)
	{
		if (isLeftHand)
		{
			if (this.leftHandGooParticleSystem.gameObject.activeSelf != isEnabled)
			{
				this.leftHandGooParticleSystem.gameObject.SetActive(isEnabled);
				return;
			}
		}
		else if (this.rightHandGooParticleSystem.gameObject.activeSelf != isEnabled)
		{
			this.rightHandGooParticleSystem.gameObject.SetActive(isEnabled);
		}
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001BD0 RID: 7120 RVA: 0x000942A5 File Offset: 0x000924A5
	// (set) Token: 0x06001BD1 RID: 7121 RVA: 0x000942AD File Offset: 0x000924AD
	bool IUserCosmeticsCallback.PendingUpdate
	{
		get
		{
			return this.pendingCosmeticUpdate;
		}
		set
		{
			this.pendingCosmeticUpdate = value;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001BD2 RID: 7122 RVA: 0x000942B6 File Offset: 0x000924B6
	// (set) Token: 0x06001BD3 RID: 7123 RVA: 0x000942BE File Offset: 0x000924BE
	public bool IsFrozen { get; set; }

	// Token: 0x06001BD4 RID: 7124 RVA: 0x000942C8 File Offset: 0x000924C8
	bool IUserCosmeticsCallback.OnGetUserCosmetics(string cosmetics)
	{
		if (cosmetics == this.rawCosmeticString && this.currentCosmeticTries < this.cosmeticRetries)
		{
			this.currentCosmeticTries++;
			return false;
		}
		this.rawCosmeticString = (cosmetics ?? "");
		this.concatStringOfCosmeticsAllowed = this.rawCosmeticString;
		this.concatStringOfCosmeticsAllowed += "LHAJJ.LHAJK.LHAJL.";
		this.InitializedCosmetics = true;
		this.currentCosmeticTries = 0;
		this.CheckForEarlyAccess();
		this.SetCosmeticsActive(false);
		this.myBodyDockPositions.RefreshTransferrableItems();
		NetworkView networkView = this.netView;
		if (networkView != null)
		{
			networkView.SendRPC("RPC_RequestCosmetics", this.creator, Array.Empty<object>());
		}
		return true;
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x0009437C File Offset: 0x0009257C
	private short PackCompetitiveData()
	{
		if (!this.turningCompInitialized)
		{
			this.GorillaSnapTurningComp = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			this.turningCompInitialized = true;
		}
		this.fps = Mathf.Min(Mathf.RoundToInt(1f / Time.smoothDeltaTime), 255);
		int num = 0;
		if (this.GorillaSnapTurningComp != null)
		{
			this.turnFactor = this.GorillaSnapTurningComp.turnFactor;
			this.turnType = this.GorillaSnapTurningComp.turnType;
			string text = this.turnType;
			if (!(text == "SNAP"))
			{
				if (text == "SMOOTH")
				{
					num = 2;
				}
			}
			else
			{
				num = 1;
			}
			num *= 10;
			num += this.turnFactor;
		}
		return (short)(this.fps + (num << 8));
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x0009443C File Offset: 0x0009263C
	private void UnpackCompetitiveData(short packed)
	{
		int num = 255;
		this.fps = ((int)packed & num);
		int num2 = 31;
		int num3 = packed >> 8 & num2;
		this.turnFactor = num3 % 10;
		int num4 = num3 / 10;
		if (num4 == 1)
		{
			this.turnType = "SNAP";
			return;
		}
		if (num4 != 2)
		{
			this.turnType = "NONE";
			return;
		}
		this.turnType = "SMOOTH";
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x000944A0 File Offset: 0x000926A0
	private void OnKIDSessionUpdated(bool showCustomNames, Permission.ManagedByEnum managedBy)
	{
		bool flag = (showCustomNames || managedBy == 1) && managedBy != 3;
		GorillaComputer.instance.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[]
		{
			GorillaComputer.ComputerState.Name
		}, false);
		bool flag2 = PlayerPrefs.GetInt("nameTagsOn", -1) > 0;
		switch (managedBy)
		{
		case 1:
			flag = GorillaComputer.instance.NametagsEnabled;
			break;
		case 2:
			flag = (showCustomNames && flag2);
			break;
		case 3:
			flag = false;
			break;
		}
		this.UpdateName(flag);
		Debug.Log("[KID] On Session Update - Custom Names Permission changed - Has enabled customNames? [" + flag.ToString() + "]");
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001BD8 RID: 7128 RVA: 0x0009453C File Offset: 0x0009273C
	public static VRRig LocalRig
	{
		get
		{
			return VRRig.gLocalRig;
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001BD9 RID: 7129 RVA: 0x00094543 File Offset: 0x00092743
	public bool isLocal
	{
		get
		{
			return VRRig.gLocalRig == this;
		}
	}

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001BDA RID: 7130 RVA: 0x00010327 File Offset: 0x0000E527
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001BDB RID: 7131 RVA: 0x00094550 File Offset: 0x00092750
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06001BDC RID: 7132 RVA: 0x00094560 File Offset: 0x00092760
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return default(Bounds);
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001BDD RID: 7133 RVA: 0x00094576 File Offset: 0x00092776
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.buildEntries();
		}
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x00094580 File Offset: 0x00092780
	private IList<KeyValueStringPair> buildEntries()
	{
		return new KeyValueStringPair[]
		{
			new KeyValueStringPair("Name", this.playerNameVisible),
			new KeyValueStringPair("Color", string.Format("{0}, {1}, {2}", Mathf.RoundToInt(this.playerColor.r * 9f), Mathf.RoundToInt(this.playerColor.g * 9f), Mathf.RoundToInt(this.playerColor.b * 9f)))
		};
	}

	// Token: 0x1400003D RID: 61
	// (add) Token: 0x06001BDF RID: 7135 RVA: 0x00094618 File Offset: 0x00092818
	// (remove) Token: 0x06001BE0 RID: 7136 RVA: 0x00094650 File Offset: 0x00092850
	public event Action OnDataChange;

	// Token: 0x04002468 RID: 9320
	private bool _isListeningFor_OnPostInstantiateAllPrefabs;

	// Token: 0x04002469 RID: 9321
	[OnEnterPlay_SetNull]
	public static Action newPlayerJoined;

	// Token: 0x0400246A RID: 9322
	public VRMap head;

	// Token: 0x0400246B RID: 9323
	public VRMap rightHand;

	// Token: 0x0400246C RID: 9324
	public VRMap leftHand;

	// Token: 0x0400246D RID: 9325
	public VRMapThumb leftThumb;

	// Token: 0x0400246E RID: 9326
	public VRMapIndex leftIndex;

	// Token: 0x0400246F RID: 9327
	public VRMapMiddle leftMiddle;

	// Token: 0x04002470 RID: 9328
	public VRMapThumb rightThumb;

	// Token: 0x04002471 RID: 9329
	public VRMapIndex rightIndex;

	// Token: 0x04002472 RID: 9330
	public VRMapMiddle rightMiddle;

	// Token: 0x04002473 RID: 9331
	public CrittersLoudNoise leftHandNoise;

	// Token: 0x04002474 RID: 9332
	public CrittersLoudNoise rightHandNoise;

	// Token: 0x04002475 RID: 9333
	public CrittersLoudNoise speakingNoise;

	// Token: 0x04002476 RID: 9334
	private int previousGrabbedRope = -1;

	// Token: 0x04002477 RID: 9335
	private int previousGrabbedRopeBoneIndex;

	// Token: 0x04002478 RID: 9336
	private bool previousGrabbedRopeWasLeft;

	// Token: 0x04002479 RID: 9337
	private bool previousGrabbedRopeWasBody;

	// Token: 0x0400247A RID: 9338
	private GorillaRopeSwing currentRopeSwing;

	// Token: 0x0400247B RID: 9339
	private Transform currentHoldParent;

	// Token: 0x0400247C RID: 9340
	private Transform currentRopeSwingTarget;

	// Token: 0x0400247D RID: 9341
	private float lastRopeGrabTimer;

	// Token: 0x0400247E RID: 9342
	private bool shouldLerpToRope;

	// Token: 0x0400247F RID: 9343
	[NonSerialized]
	public int grabbedRopeIndex = -1;

	// Token: 0x04002480 RID: 9344
	[NonSerialized]
	public int grabbedRopeBoneIndex;

	// Token: 0x04002481 RID: 9345
	[NonSerialized]
	public bool grabbedRopeIsLeft;

	// Token: 0x04002482 RID: 9346
	[NonSerialized]
	public bool grabbedRopeIsBody;

	// Token: 0x04002483 RID: 9347
	[NonSerialized]
	public bool grabbedRopeIsPhotonView;

	// Token: 0x04002484 RID: 9348
	[NonSerialized]
	public Vector3 grabbedRopeOffset = Vector3.zero;

	// Token: 0x04002485 RID: 9349
	private int prevMovingSurfaceID = -1;

	// Token: 0x04002486 RID: 9350
	private bool movingSurfaceWasLeft;

	// Token: 0x04002487 RID: 9351
	private bool movingSurfaceWasBody;

	// Token: 0x04002488 RID: 9352
	private bool movingSurfaceWasMonkeBlock;

	// Token: 0x04002489 RID: 9353
	[NonSerialized]
	public int mountedMovingSurfaceId = -1;

	// Token: 0x0400248A RID: 9354
	[NonSerialized]
	private BuilderPiece mountedMonkeBlock;

	// Token: 0x0400248B RID: 9355
	[NonSerialized]
	private MovingSurface mountedMovingSurface;

	// Token: 0x0400248C RID: 9356
	[NonSerialized]
	public bool mountedMovingSurfaceIsLeft;

	// Token: 0x0400248D RID: 9357
	[NonSerialized]
	public bool mountedMovingSurfaceIsBody;

	// Token: 0x0400248E RID: 9358
	[NonSerialized]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x0400248F RID: 9359
	[NonSerialized]
	public Vector3 mountedMonkeBlockOffset = Vector3.zero;

	// Token: 0x04002490 RID: 9360
	private float lastMountedSurfaceTimer;

	// Token: 0x04002491 RID: 9361
	private bool shouldLerpToMovingSurface;

	// Token: 0x04002492 RID: 9362
	[Tooltip("- False in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool isOfflineVRRig;

	// Token: 0x04002493 RID: 9363
	public GameObject mainCamera;

	// Token: 0x04002494 RID: 9364
	public Transform playerOffsetTransform;

	// Token: 0x04002495 RID: 9365
	public int SDKIndex;

	// Token: 0x04002496 RID: 9366
	public bool isMyPlayer;

	// Token: 0x04002497 RID: 9367
	public AudioSource leftHandPlayer;

	// Token: 0x04002498 RID: 9368
	public AudioSource rightHandPlayer;

	// Token: 0x04002499 RID: 9369
	public AudioSource tagSound;

	// Token: 0x0400249A RID: 9370
	[SerializeField]
	private float ratio;

	// Token: 0x0400249B RID: 9371
	public Transform headConstraint;

	// Token: 0x0400249C RID: 9372
	public Vector3 headBodyOffset = Vector3.zero;

	// Token: 0x0400249D RID: 9373
	public GameObject headMesh;

	// Token: 0x0400249E RID: 9374
	private NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x0400249F RID: 9375
	public Vector3 jobPos;

	// Token: 0x040024A0 RID: 9376
	public Quaternion syncRotation;

	// Token: 0x040024A1 RID: 9377
	public Quaternion jobRotation;

	// Token: 0x040024A2 RID: 9378
	public AudioClip[] clipToPlay;

	// Token: 0x040024A3 RID: 9379
	public AudioClip[] handTapSound;

	// Token: 0x040024A4 RID: 9380
	public int setMatIndex;

	// Token: 0x040024A5 RID: 9381
	public float lerpValueFingers;

	// Token: 0x040024A6 RID: 9382
	public float lerpValueBody;

	// Token: 0x040024A7 RID: 9383
	public GameObject backpack;

	// Token: 0x040024A8 RID: 9384
	public Transform leftHandTransform;

	// Token: 0x040024A9 RID: 9385
	public Transform rightHandTransform;

	// Token: 0x040024AA RID: 9386
	public Transform bodyTransform;

	// Token: 0x040024AB RID: 9387
	public SkinnedMeshRenderer mainSkin;

	// Token: 0x040024AC RID: 9388
	public GorillaSkin defaultSkin;

	// Token: 0x040024AD RID: 9389
	public MeshRenderer faceSkin;

	// Token: 0x040024AE RID: 9390
	public XRaySkeleton skeleton;

	// Token: 0x040024AF RID: 9391
	public GorillaBodyRenderer bodyRenderer;

	// Token: 0x040024B0 RID: 9392
	public ZoneEntityBSP zoneEntity;

	// Token: 0x040024B1 RID: 9393
	public Material scoreboardMaterial;

	// Token: 0x040024B2 RID: 9394
	public GameObject spectatorSkin;

	// Token: 0x040024B3 RID: 9395
	public int handSync;

	// Token: 0x040024B4 RID: 9396
	public Material[] materialsToChangeTo;

	// Token: 0x040024B5 RID: 9397
	public float red;

	// Token: 0x040024B6 RID: 9398
	public float green;

	// Token: 0x040024B7 RID: 9399
	public float blue;

	// Token: 0x040024B8 RID: 9400
	public TextMeshPro playerText1;

	// Token: 0x040024B9 RID: 9401
	public string playerNameVisible;

	// Token: 0x040024BA RID: 9402
	[Tooltip("- True in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool showName;

	// Token: 0x040024BB RID: 9403
	public CosmeticItemRegistry cosmeticsObjectRegistry = new CosmeticItemRegistry();

	// Token: 0x040024BC RID: 9404
	[NonSerialized]
	public PropHuntHandFollower propHuntHandFollower;

	// Token: 0x040024BD RID: 9405
	[FormerlySerializedAs("cosmetics")]
	public GameObject[] _cosmetics;

	// Token: 0x040024BE RID: 9406
	[FormerlySerializedAs("overrideCosmetics")]
	public GameObject[] _overrideCosmetics;

	// Token: 0x040024BF RID: 9407
	private int taggedById;

	// Token: 0x040024C0 RID: 9408
	public string concatStringOfCosmeticsAllowed = "";

	// Token: 0x040024C1 RID: 9409
	private bool initializedCosmetics;

	// Token: 0x040024C2 RID: 9410
	private readonly HashSet<string> _temporaryCosmetics = new HashSet<string>();

	// Token: 0x040024C3 RID: 9411
	public CosmeticsController.CosmeticSet cosmeticSet;

	// Token: 0x040024C4 RID: 9412
	public CosmeticsController.CosmeticSet tryOnSet;

	// Token: 0x040024C5 RID: 9413
	public CosmeticsController.CosmeticSet mergedSet;

	// Token: 0x040024C6 RID: 9414
	public CosmeticsController.CosmeticSet prevSet;

	// Token: 0x040024C7 RID: 9415
	[NonSerialized]
	public readonly List<GameObject> activeCosmetics = new List<GameObject>(16);

	// Token: 0x040024C8 RID: 9416
	private int cosmeticRetries = 2;

	// Token: 0x040024C9 RID: 9417
	private int currentCosmeticTries;

	// Token: 0x040024CB RID: 9419
	public SizeManager sizeManager;

	// Token: 0x040024CC RID: 9420
	public float pitchScale = 0.3f;

	// Token: 0x040024CD RID: 9421
	public float pitchOffset = 1f;

	// Token: 0x040024CE RID: 9422
	[NonSerialized]
	public bool IsHaunted;

	// Token: 0x040024CF RID: 9423
	public float HauntedVoicePitch = 0.5f;

	// Token: 0x040024D0 RID: 9424
	public float HauntedHearingVolume = 0.15f;

	// Token: 0x040024D1 RID: 9425
	[NonSerialized]
	public bool UsingHauntedRing;

	// Token: 0x040024D2 RID: 9426
	[NonSerialized]
	public float HauntedRingVoicePitch;

	// Token: 0x040024D3 RID: 9427
	private float cosmeticPitchShift = 1f;

	// Token: 0x040024D4 RID: 9428
	private bool pitchShiftCosmeticsDirty;

	// Token: 0x040024D5 RID: 9429
	[NonSerialized]
	public List<VoicePitchShiftCosmetic> PitchShiftCosmetics = new List<VoicePitchShiftCosmetic>();

	// Token: 0x040024D6 RID: 9430
	public FriendshipBracelet friendshipBraceletLeftHand;

	// Token: 0x040024D7 RID: 9431
	public NonCosmeticHandItem nonCosmeticLeftHandItem;

	// Token: 0x040024D8 RID: 9432
	public FriendshipBracelet friendshipBraceletRightHand;

	// Token: 0x040024D9 RID: 9433
	public NonCosmeticHandItem nonCosmeticRightHandItem;

	// Token: 0x040024DA RID: 9434
	public HoverboardVisual hoverboardVisual;

	// Token: 0x040024DB RID: 9435
	private int hoverboardEnabledCount;

	// Token: 0x040024DC RID: 9436
	public HoldableHand bodyHolds;

	// Token: 0x040024DD RID: 9437
	public HoldableHand leftHolds;

	// Token: 0x040024DE RID: 9438
	public HoldableHand rightHolds;

	// Token: 0x040024DF RID: 9439
	public GorillaClimbable leftHandHoldsPlayer;

	// Token: 0x040024E0 RID: 9440
	public GorillaClimbable rightHandHoldsPlayer;

	// Token: 0x040024E1 RID: 9441
	public HandLink leftHandLink;

	// Token: 0x040024E2 RID: 9442
	public HandLink rightHandLink;

	// Token: 0x040024E5 RID: 9445
	public GameObject nameTagAnchor;

	// Token: 0x040024E6 RID: 9446
	public GameObject frozenEffect;

	// Token: 0x040024E7 RID: 9447
	public GameObject iceCubeLeft;

	// Token: 0x040024E8 RID: 9448
	public GameObject iceCubeRight;

	// Token: 0x040024E9 RID: 9449
	public float frozenEffectMaxY;

	// Token: 0x040024EA RID: 9450
	public float frozenEffectMaxHorizontalScale = 0.8f;

	// Token: 0x040024EB RID: 9451
	public GameObject FPVEffectsParent;

	// Token: 0x040024EC RID: 9452
	public Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> TemporaryCosmeticEffects = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

	// Token: 0x040024ED RID: 9453
	private float _nextUpdateTime = -1f;

	// Token: 0x040024EE RID: 9454
	public VRRigReliableState reliableState;

	// Token: 0x040024EF RID: 9455
	[SerializeField]
	private Transform MouthPosition;

	// Token: 0x040024F3 RID: 9459
	internal RigContainer rigContainer;

	// Token: 0x040024F4 RID: 9460
	public Action<RigContainer> OnNameChanged;

	// Token: 0x040024F5 RID: 9461
	private Vector3 remoteVelocity;

	// Token: 0x040024F6 RID: 9462
	private double remoteLatestTimestamp;

	// Token: 0x040024F7 RID: 9463
	private Vector3 remoteCorrectionNeeded;

	// Token: 0x040024F8 RID: 9464
	private const float REMOTE_CORRECTION_RATE = 5f;

	// Token: 0x040024F9 RID: 9465
	private const bool USE_NEW_NETCODE = false;

	// Token: 0x040024FA RID: 9466
	private float stealthTimer;

	// Token: 0x040024FB RID: 9467
	private GorillaAmbushManager stealthManager;

	// Token: 0x040024FC RID: 9468
	private LayerChanger layerChanger;

	// Token: 0x040024FD RID: 9469
	private float frozenEffectMinY;

	// Token: 0x040024FE RID: 9470
	private float frozenEffectMinHorizontalScale;

	// Token: 0x040024FF RID: 9471
	private float frozenTimeElapsed;

	// Token: 0x04002500 RID: 9472
	public TagEffectPack CosmeticEffectPack;

	// Token: 0x04002501 RID: 9473
	private GorillaSnapTurn GorillaSnapTurningComp;

	// Token: 0x04002502 RID: 9474
	private bool turningCompInitialized;

	// Token: 0x04002503 RID: 9475
	private string turnType = "NONE";

	// Token: 0x04002504 RID: 9476
	private int turnFactor;

	// Token: 0x04002505 RID: 9477
	private int fps;

	// Token: 0x04002507 RID: 9479
	private VRRig.PartyMemberStatus partyMemberStatus;

	// Token: 0x04002508 RID: 9480
	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[]
	{
		new GTBitOps.BitWriteInfo(0, 1),
		new GTBitOps.BitWriteInfo(1, 2),
		new GTBitOps.BitWriteInfo(3, 2),
		new GTBitOps.BitWriteInfo(5, 2),
		new GTBitOps.BitWriteInfo(7, 2),
		new GTBitOps.BitWriteInfo(9, 2),
		new GTBitOps.BitWriteInfo(11, 1),
		new GTBitOps.BitWriteInfo(12, 1),
		new GTBitOps.BitWriteInfo(13, 1)
	};

	// Token: 0x04002509 RID: 9481
	public bool inTryOnRoom;

	// Token: 0x0400250A RID: 9482
	public bool muted;

	// Token: 0x0400250B RID: 9483
	private float lastScaleFactor = 1f;

	// Token: 0x0400250C RID: 9484
	private float scaleMultiplier = 1f;

	// Token: 0x0400250D RID: 9485
	private float nativeScale = 1f;

	// Token: 0x0400250E RID: 9486
	private float timeSpawned;

	// Token: 0x0400250F RID: 9487
	public float doNotLerpConstant = 1f;

	// Token: 0x04002510 RID: 9488
	public string tempString;

	// Token: 0x04002511 RID: 9489
	private Player tempPlayer;

	// Token: 0x04002512 RID: 9490
	internal NetPlayer creator;

	// Token: 0x04002513 RID: 9491
	private float[] speedArray;

	// Token: 0x04002514 RID: 9492
	private double handLerpValues;

	// Token: 0x04002515 RID: 9493
	private bool initialized;

	// Token: 0x04002516 RID: 9494
	[FormerlySerializedAs("battleBalloons")]
	public PaintbrawlBalloons paintbrawlBalloons;

	// Token: 0x04002517 RID: 9495
	private int tempInt;

	// Token: 0x04002518 RID: 9496
	public BodyDockPositions myBodyDockPositions;

	// Token: 0x04002519 RID: 9497
	public ParticleSystem lavaParticleSystem;

	// Token: 0x0400251A RID: 9498
	public ParticleSystem rockParticleSystem;

	// Token: 0x0400251B RID: 9499
	public ParticleSystem iceParticleSystem;

	// Token: 0x0400251C RID: 9500
	public ParticleSystem snowFlakeParticleSystem;

	// Token: 0x0400251D RID: 9501
	public ParticleSystem leftHandGooParticleSystem;

	// Token: 0x0400251E RID: 9502
	public ParticleSystem rightHandGooParticleSystem;

	// Token: 0x0400251F RID: 9503
	public string tempItemName;

	// Token: 0x04002520 RID: 9504
	public CosmeticsController.CosmeticItem tempItem;

	// Token: 0x04002521 RID: 9505
	public string tempItemId;

	// Token: 0x04002522 RID: 9506
	public int tempItemCost;

	// Token: 0x04002523 RID: 9507
	public int leftHandHoldableStatus;

	// Token: 0x04002524 RID: 9508
	public int rightHandHoldableStatus;

	// Token: 0x04002525 RID: 9509
	[Tooltip("This has to match the drumsAS array in DrumsItem.cs.")]
	[SerializeReference]
	public AudioSource[] musicDrums;

	// Token: 0x04002526 RID: 9510
	private List<TransferrableObject> instrumentSelfOnly = new List<TransferrableObject>();

	// Token: 0x04002527 RID: 9511
	public AudioSource geodeCrackingSound;

	// Token: 0x04002528 RID: 9512
	public float bonkTime;

	// Token: 0x04002529 RID: 9513
	public float bonkCooldown = 2f;

	// Token: 0x0400252A RID: 9514
	private VRRig tempVRRig;

	// Token: 0x0400252B RID: 9515
	public GameObject huntComputer;

	// Token: 0x0400252C RID: 9516
	public GameObject builderResizeWatch;

	// Token: 0x0400252D RID: 9517
	public BuilderArmShelf builderArmShelfLeft;

	// Token: 0x0400252E RID: 9518
	public BuilderArmShelf builderArmShelfRight;

	// Token: 0x0400252F RID: 9519
	public GameObject guardianEjectWatch;

	// Token: 0x04002530 RID: 9520
	public GameObject vStumpReturnWatch;

	// Token: 0x04002531 RID: 9521
	public GameObject rankedTimerWatch;

	// Token: 0x04002532 RID: 9522
	public SuperInfectionHandDisplay superInfectionHand;

	// Token: 0x04002533 RID: 9523
	public ProjectileWeapon projectileWeapon;

	// Token: 0x04002534 RID: 9524
	private PhotonVoiceView myPhotonVoiceView;

	// Token: 0x04002535 RID: 9525
	private VRRig senderRig;

	// Token: 0x04002536 RID: 9526
	private bool isInitialized;

	// Token: 0x04002537 RID: 9527
	private CircularBuffer<VRRig.VelocityTime> velocityHistoryList = new CircularBuffer<VRRig.VelocityTime>(200);

	// Token: 0x04002538 RID: 9528
	public int velocityHistoryMaxLength = 200;

	// Token: 0x04002539 RID: 9529
	private Vector3 lastPosition;

	// Token: 0x0400253A RID: 9530
	public const int splashLimitCount = 4;

	// Token: 0x0400253B RID: 9531
	public const float splashLimitCooldown = 0.5f;

	// Token: 0x0400253C RID: 9532
	private float[] splashEffectTimes = new float[4];

	// Token: 0x0400253D RID: 9533
	internal AudioSource voiceAudio;

	// Token: 0x0400253E RID: 9534
	public bool remoteUseReplacementVoice;

	// Token: 0x0400253F RID: 9535
	public bool localUseReplacementVoice;

	// Token: 0x04002540 RID: 9536
	private MicWrapper currentMicWrapper;

	// Token: 0x04002541 RID: 9537
	private IAudioDesc audioDesc;

	// Token: 0x04002542 RID: 9538
	private float speakingLoudness;

	// Token: 0x04002543 RID: 9539
	public bool shouldSendSpeakingLoudness = true;

	// Token: 0x04002544 RID: 9540
	public float replacementVoiceLoudnessThreshold = 0.05f;

	// Token: 0x04002545 RID: 9541
	public int replacementVoiceDetectionDelay = 128;

	// Token: 0x04002546 RID: 9542
	private GorillaMouthFlap myMouthFlap;

	// Token: 0x04002547 RID: 9543
	private GorillaSpeakerLoudness mySpeakerLoudness;

	// Token: 0x04002548 RID: 9544
	public ReplacementVoice myReplacementVoice;

	// Token: 0x04002549 RID: 9545
	private GorillaEyeExpressions myEyeExpressions;

	// Token: 0x0400254A RID: 9546
	[SerializeField]
	internal NetworkView netView;

	// Token: 0x0400254B RID: 9547
	[SerializeField]
	internal VRRigSerializer rigSerializer;

	// Token: 0x0400254C RID: 9548
	public NetPlayer OwningNetPlayer;

	// Token: 0x0400254D RID: 9549
	[SerializeField]
	private FXSystemSettings sharedFXSettings;

	// Token: 0x0400254E RID: 9550
	[NonSerialized]
	public FXSystemSettings fxSettings;

	// Token: 0x0400254F RID: 9551
	[SerializeField]
	private float tapPointDistance = 0.035f;

	// Token: 0x04002550 RID: 9552
	[SerializeField]
	private float handSpeedToVolumeModifier = 0.05f;

	// Token: 0x04002551 RID: 9553
	[SerializeField]
	private HandEffectContext _leftHandEffect;

	// Token: 0x04002552 RID: 9554
	[SerializeField]
	private HandEffectContext _rightHandEffect;

	// Token: 0x04002553 RID: 9555
	[SerializeField]
	private HandEffectContext _extraLeftHandEffect;

	// Token: 0x04002554 RID: 9556
	[SerializeField]
	private HandEffectContext _extraRightHandEffect;

	// Token: 0x04002555 RID: 9557
	private bool _rigBuildFullyInitialized;

	// Token: 0x04002556 RID: 9558
	[SerializeField]
	private Transform renderTransform;

	// Token: 0x04002557 RID: 9559
	private GamePlayer _gamePlayerRef;

	// Token: 0x04002558 RID: 9560
	private bool playerWasHaunted;

	// Token: 0x04002559 RID: 9561
	private float nonHauntedVolume;

	// Token: 0x0400255A RID: 9562
	[SerializeField]
	private AnimationCurve voicePitchForRelativeScale;

	// Token: 0x0400255B RID: 9563
	private Vector3 LocalTrajectoryOverridePosition;

	// Token: 0x0400255C RID: 9564
	private Vector3 LocalTrajectoryOverrideVelocity;

	// Token: 0x0400255D RID: 9565
	private float LocalTrajectoryOverrideBlend;

	// Token: 0x0400255E RID: 9566
	[SerializeField]
	private float LocalTrajectoryOverrideDuration = 1f;

	// Token: 0x0400255F RID: 9567
	private bool localOverrideIsBody;

	// Token: 0x04002560 RID: 9568
	private bool localOverrideIsLeftHand;

	// Token: 0x04002561 RID: 9569
	private Transform localOverrideGrabbingHand;

	// Token: 0x04002562 RID: 9570
	private float localGrabOverrideBlend;

	// Token: 0x04002563 RID: 9571
	[SerializeField]
	private float LocalGrabOverrideDuration = 0.25f;

	// Token: 0x04002564 RID: 9572
	private float[] voiceSampleBuffer = new float[128];

	// Token: 0x04002565 RID: 9573
	private const int CHECK_LOUDNESS_FREQ_FRAMES = 10;

	// Token: 0x04002566 RID: 9574
	private CallbackContainer<ICallBack> lateUpdateCallbacks = new CallbackContainer<ICallBack>(5);

	// Token: 0x04002567 RID: 9575
	private float nextLocalVelocityStoreTimestamp;

	// Token: 0x04002568 RID: 9576
	private bool IsInvisibleToLocalPlayer;

	// Token: 0x04002569 RID: 9577
	private const int remoteUseReplacementVoice_BIT = 512;

	// Token: 0x0400256A RID: 9578
	private const int grabbedRope_BIT = 1024;

	// Token: 0x0400256B RID: 9579
	private const int grabbedRopeIsPhotonView_BIT = 2048;

	// Token: 0x0400256C RID: 9580
	private const int isHoldingHandsWithPlayer_BIT = 4096;

	// Token: 0x0400256D RID: 9581
	private const int isHoldingHoverboard_BIT = 8192;

	// Token: 0x0400256E RID: 9582
	private const int isHoverboardLeftHanded_BIT = 16384;

	// Token: 0x0400256F RID: 9583
	private const int isOnMovingSurface_BIT = 32768;

	// Token: 0x04002570 RID: 9584
	private const int isPropHunt_BIT = 65536;

	// Token: 0x04002571 RID: 9585
	private const int propHuntLeftHand_BIT = 131072;

	// Token: 0x04002572 RID: 9586
	private const int isLeftHandGrabbable_BIT = 262144;

	// Token: 0x04002573 RID: 9587
	private const int isRightHandGrabbable_BIT = 524288;

	// Token: 0x04002574 RID: 9588
	private const int speakingLoudnessVal_BITSHIFT = 24;

	// Token: 0x04002575 RID: 9589
	private Vector3 tempVec;

	// Token: 0x04002576 RID: 9590
	private Quaternion tempQuat;

	// Token: 0x04002577 RID: 9591
	public Action<int, int> OnMaterialIndexChanged;

	// Token: 0x04002578 RID: 9592
	[SerializeField]
	private ParticleSystem cosmeticsActivationPS;

	// Token: 0x04002579 RID: 9593
	[SerializeField]
	private SoundBankPlayer cosmeticsActivationSBP;

	// Token: 0x0400257A RID: 9594
	public Color playerColor;

	// Token: 0x0400257B RID: 9595
	public bool colorInitialized;

	// Token: 0x0400257C RID: 9596
	private Action<Color> onColorInitialized;

	// Token: 0x0400257F RID: 9599
	private bool m_sentRankedScore;

	// Token: 0x04002581 RID: 9601
	private int currentQuestScore;

	// Token: 0x04002582 RID: 9602
	private bool _scoreUpdated;

	// Token: 0x04002583 RID: 9603
	private CallLimiter updateQuestCallLimit = new CallLimiter(1, 0.5f, 0.5f);

	// Token: 0x04002585 RID: 9605
	private float currentRankedELO;

	// Token: 0x04002586 RID: 9606
	private int currentRankedSubTierQuest;

	// Token: 0x04002587 RID: 9607
	private int currentRankedSubTierPC;

	// Token: 0x04002588 RID: 9608
	private bool _rankedInfoUpdated;

	// Token: 0x04002589 RID: 9609
	internal CallLimiter updateRankedInfoCallLimit = new CallLimiter(2, 60f, 0.5f);

	// Token: 0x0400258A RID: 9610
	public const float maxGuardianThrowVelocity = 20f;

	// Token: 0x0400258B RID: 9611
	public const float maxRegularThrowVelocity = 3f;

	// Token: 0x0400258C RID: 9612
	private RaycastHit[] rayCastNonAllocColliders = new RaycastHit[5];

	// Token: 0x0400258D RID: 9613
	private bool inDuplicationZone;

	// Token: 0x0400258E RID: 9614
	private RigDuplicationZone duplicationZone;

	// Token: 0x0400258F RID: 9615
	private bool pendingCosmeticUpdate = true;

	// Token: 0x04002590 RID: 9616
	private string rawCosmeticString = "";

	// Token: 0x04002592 RID: 9618
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Right = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04002593 RID: 9619
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Left = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04002594 RID: 9620
	private int loudnessCheckFrame;

	// Token: 0x04002595 RID: 9621
	private float frameScale;

	// Token: 0x04002596 RID: 9622
	private const bool SHOW_SCREENS = false;

	// Token: 0x04002597 RID: 9623
	[OnEnterPlay_SetNull]
	private static VRRig gLocalRig;

	// Token: 0x02000449 RID: 1097
	public enum PartyMemberStatus
	{
		// Token: 0x0400259A RID: 9626
		NeedsUpdate,
		// Token: 0x0400259B RID: 9627
		InLocalParty,
		// Token: 0x0400259C RID: 9628
		NotInLocalParty
	}

	// Token: 0x0200044A RID: 1098
	public enum WearablePackedStateSlots
	{
		// Token: 0x0400259E RID: 9630
		Hat,
		// Token: 0x0400259F RID: 9631
		LeftHand,
		// Token: 0x040025A0 RID: 9632
		RightHand,
		// Token: 0x040025A1 RID: 9633
		Face,
		// Token: 0x040025A2 RID: 9634
		Pants1,
		// Token: 0x040025A3 RID: 9635
		Pants2,
		// Token: 0x040025A4 RID: 9636
		Badge,
		// Token: 0x040025A5 RID: 9637
		Fur,
		// Token: 0x040025A6 RID: 9638
		Shirt
	}

	// Token: 0x0200044B RID: 1099
	public struct VelocityTime
	{
		// Token: 0x06001BE5 RID: 7141 RVA: 0x00094A17 File Offset: 0x00092C17
		public VelocityTime(Vector3 velocity, double velTime)
		{
			this.vel = velocity;
			this.time = velTime;
		}

		// Token: 0x040025A7 RID: 9639
		public Vector3 vel;

		// Token: 0x040025A8 RID: 9640
		public double time;
	}
}
