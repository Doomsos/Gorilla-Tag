using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200018F RID: 399
public class FingerFlagWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000AAF RID: 2735 RVA: 0x0003A071 File Offset: 0x00038271
	// (set) Token: 0x06000AB0 RID: 2736 RVA: 0x0003A079 File Offset: 0x00038279
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x06000AB1 RID: 2737 RVA: 0x0003A082 File Offset: 0x00038282
	// (set) Token: 0x06000AB2 RID: 2738 RVA: 0x0003A08A File Offset: 0x0003828A
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0003A093 File Offset: 0x00038293
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = base.GetComponentInParent<VRRig>(true);
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0003A0BC File Offset: 0x000382BC
	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0003A0F4 File Offset: 0x000382F4
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

	// Token: 0x06000AB7 RID: 2743 RVA: 0x0003A174 File Offset: 0x00038374
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
		}
		bool flag = this.fullyRetracted;
		this.fullyRetracted = (this.extended && this.retractExtendTime <= 0f);
		if (flag != this.fullyRetracted)
		{
			Transform[] array = this.clothRigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(!this.fullyRetracted);
			}
		}
		this.UpdateAnimation();
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x0003A202 File Offset: 0x00038402
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0003A23B File Offset: 0x0003843B
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0003A258 File Offset: 0x00038458
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

	// Token: 0x06000ABB RID: 2747 RVA: 0x0003A278 File Offset: 0x00038478
	private void UpdateAnimation()
	{
		float num = this.extended ? this.extendSpeed : (-this.retractSpeed);
		this.retractExtendTime = Mathf.Clamp01(this.retractExtendTime + Time.deltaTime * num);
		this.animator.SetFloat(this.retractExtendTimeAnimParam, this.retractExtendTime);
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x0003A2D0 File Offset: 0x000384D0
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

	// Token: 0x04000D11 RID: 3345
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x04000D12 RID: 3346
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x04000D13 RID: 3347
	public Transform thumbRingBone;

	// Token: 0x04000D14 RID: 3348
	public Transform[] clothBones;

	// Token: 0x04000D15 RID: 3349
	public Transform[] clothRigidbodies;

	// Token: 0x04000D16 RID: 3350
	[Header("Animation")]
	public Animator animator;

	// Token: 0x04000D17 RID: 3351
	public float extendSpeed = 1.5f;

	// Token: 0x04000D18 RID: 3352
	public float retractSpeed = 2.25f;

	// Token: 0x04000D19 RID: 3353
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x04000D1A RID: 3354
	public AudioClip extendAudioClip;

	// Token: 0x04000D1B RID: 3355
	public AudioClip retractAudioClip;

	// Token: 0x04000D1C RID: 3356
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x04000D1D RID: 3357
	public float extendVibrationStrength = 0.2f;

	// Token: 0x04000D1E RID: 3358
	public float retractVibrationDuration = 0.05f;

	// Token: 0x04000D1F RID: 3359
	public float retractVibrationStrength = 0.2f;

	// Token: 0x04000D20 RID: 3360
	private readonly int retractExtendTimeAnimParam = Animator.StringToHash("retractExtendTime");

	// Token: 0x04000D21 RID: 3361
	private bool networkedExtended;

	// Token: 0x04000D22 RID: 3362
	private bool extended;

	// Token: 0x04000D23 RID: 3363
	private bool fullyRetracted;

	// Token: 0x04000D24 RID: 3364
	private float retractExtendTime;

	// Token: 0x04000D25 RID: 3365
	private InputDevice inputDevice;

	// Token: 0x04000D26 RID: 3366
	private VRRig myRig;

	// Token: 0x04000D27 RID: 3367
	private int stateBitIndex;
}
