using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020001AD RID: 429
public class FingerTorch : MonoBehaviour, ISpawnable
{
	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000B68 RID: 2920 RVA: 0x0003DEE3 File Offset: 0x0003C0E3
	// (set) Token: 0x06000B69 RID: 2921 RVA: 0x0003DEEB File Offset: 0x0003C0EB
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000B6A RID: 2922 RVA: 0x0003DEF4 File Offset: 0x0003C0F4
	// (set) Token: 0x06000B6B RID: 2923 RVA: 0x0003DEFC File Offset: 0x0003C0FC
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000B6C RID: 2924 RVA: 0x0003DF05 File Offset: 0x0003C105
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0003DF28 File Offset: 0x0003C128
	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x00002789 File Offset: 0x00000989
	protected void OnDisable()
	{
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0003DF60 File Offset: 0x0003C160
	private void UpdateLocal()
	{
		int node = this.attachedToLeftHand ? 4 : 5;
		bool flag = ControllerInputPoller.GripFloat(node) > 0.25f;
		bool flag2 = ControllerInputPoller.PrimaryButtonPress(node);
		bool flag3 = ControllerInputPoller.SecondaryButtonPress(node);
		bool flag4 = flag && (flag2 || flag3);
		this.networkedExtended = flag4;
		if (PhotonNetwork.InRoom && this.myRig)
		{
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.networkedExtended);
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0003DFE0 File Offset: 0x0003C1E0
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
			this.particleFX.SetActive(this.extended);
		}
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x0003E014 File Offset: 0x0003C214
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x0003E04D File Offset: 0x0003C24D
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x0003E06A File Offset: 0x0003C26A
	protected void LateUpdate()
	{
		if (this.IsMyItem())
		{
			this.UpdateLocal();
		}
		else
		{
			this.UpdateReplicated();
		}
		this.UpdateShared();
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0003E088 File Offset: 0x0003C288
	private void OnExtendStateChanged(bool playAudio)
	{
		this.audioSource.clip = (this.extended ? this.extendAudioClip : this.retractAudioClip);
		if (playAudio)
		{
			this.audioSource.GTPlay();
		}
		if (this.IsMyItem() && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(this.attachedToLeftHand, this.extended ? this.extendVibrationDuration : this.retractVibrationDuration, this.extended ? this.extendVibrationStrength : this.retractVibrationStrength);
		}
	}

	// Token: 0x04000DFB RID: 3579
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x04000DFC RID: 3580
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x04000DFD RID: 3581
	public Transform thumbRingBone;

	// Token: 0x04000DFE RID: 3582
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x04000DFF RID: 3583
	public AudioClip extendAudioClip;

	// Token: 0x04000E00 RID: 3584
	public AudioClip retractAudioClip;

	// Token: 0x04000E01 RID: 3585
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x04000E02 RID: 3586
	public float extendVibrationStrength = 0.2f;

	// Token: 0x04000E03 RID: 3587
	public float retractVibrationDuration = 0.05f;

	// Token: 0x04000E04 RID: 3588
	public float retractVibrationStrength = 0.2f;

	// Token: 0x04000E05 RID: 3589
	[Header("Particle FX")]
	public GameObject particleFX;

	// Token: 0x04000E06 RID: 3590
	private bool networkedExtended;

	// Token: 0x04000E07 RID: 3591
	private bool extended;

	// Token: 0x04000E08 RID: 3592
	private InputDevice inputDevice;

	// Token: 0x04000E09 RID: 3593
	private VRRig myRig;

	// Token: 0x04000E0A RID: 3594
	private int stateBitIndex;
}
