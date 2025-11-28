using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000325 RID: 805
public class ToggleableWearable : MonoBehaviour
{
	// Token: 0x0600138C RID: 5004 RVA: 0x00070E68 File Offset: 0x0006F068
	protected void Awake()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
		if (this.ownerRig == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.ownerRig = componentInParent.offlineVRRig;
				this.ownerIsLocal = (this.ownerRig != null);
			}
		}
		if (this.ownerRig == null)
		{
			Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
			base.enabled = false;
			return;
		}
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer == null)
			{
				Debug.LogError("TriggerToggler: Disabling because a renderer is null.");
				base.enabled = false;
				break;
			}
			renderer.enabled = this.startOn;
		}
		this.hasAudioSource = (this.audioSource != null);
		this.assignedSlotBitIndex = (int)this.assignedSlot;
		if (this.oneShot)
		{
			this.toggleCooldownRange.x = this.toggleCooldownRange.x + this.animationTransitionDuration;
			this.toggleCooldownRange.y = this.toggleCooldownRange.y + this.animationTransitionDuration;
		}
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x00070F70 File Offset: 0x0006F170
	protected void LateUpdate()
	{
		if (this.ownerIsLocal)
		{
			this.toggleCooldownTimer -= Time.deltaTime;
			Transform transform = base.transform;
			if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(this.triggerOffset), this.triggerRadius * transform.lossyScale.x, this.colliders, this.layerMask) > 0 && this.toggleCooldownTimer < 0f)
			{
				XRController componentInParent = this.colliders[0].GetComponentInParent<XRController>();
				if (componentInParent != null)
				{
					this.LocalToggle(componentInParent.controllerNode == 4, true, true);
				}
				this.toggleCooldownTimer = Random.Range(this.toggleCooldownRange.x, this.toggleCooldownRange.y);
				this.toggleTimer = 0f;
			}
			if (this.resetTimer > 0f)
			{
				this.toggleTimer += Time.deltaTime;
				if (this.toggleTimer > this.resetTimer && this.startOn != this.isOn)
				{
					this.LocalToggle(false, true, false);
					this.toggleTimer = 0f;
				}
			}
		}
		else
		{
			bool flag = (this.ownerRig.WearablePackedStates & 1 << this.assignedSlotBitIndex) != 0;
			if (this.isOn != flag)
			{
				this.SharedSetState(flag, true);
			}
		}
		if (this.oneShot)
		{
			if (this.isOn)
			{
				this.progress = Mathf.MoveTowards(this.progress, 1f, Time.deltaTime / this.animationTransitionDuration);
				if (this.progress == 1f)
				{
					if (this.ownerIsLocal)
					{
						this.LocalToggle(false, false, false);
					}
					else
					{
						this.SharedSetState(false, false);
					}
					this.progress = 0f;
				}
			}
		}
		else
		{
			this.progress = Mathf.MoveTowards(this.progress, this.isOn ? 1f : 0f, Time.deltaTime / this.animationTransitionDuration);
		}
		Animator[] array = this.animators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(ToggleableWearable.animParam_Progress, this.progress);
		}
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x00071180 File Offset: 0x0006F380
	private void LocalToggle(bool isLeftHand, bool playAudio, bool playHaptics)
	{
		this.ownerRig.WearablePackedStates ^= 1 << this.assignedSlotBitIndex;
		this.SharedSetState((this.ownerRig.WearablePackedStates & 1 << this.assignedSlotBitIndex) != 0, playAudio);
		if (playHaptics && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.isOn ? this.turnOnVibrationDuration : this.turnOffVibrationDuration, this.isOn ? this.turnOnVibrationStrength : this.turnOffVibrationStrength);
		}
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x00071214 File Offset: 0x0006F414
	private void SharedSetState(bool state, bool playAudio)
	{
		this.isOn = state;
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.isOn;
		}
		if (!playAudio || !this.hasAudioSource)
		{
			return;
		}
		AudioClip audioClip = this.isOn ? this.toggleOnSound : this.toggleOffSound;
		if (audioClip == null)
		{
			return;
		}
		if (this.oneShot)
		{
			this.audioSource.clip = audioClip;
			this.audioSource.GTPlay();
			return;
		}
		this.audioSource.GTPlayOneShot(audioClip, 1f);
	}

	// Token: 0x04001D0E RID: 7438
	public Renderer[] renderers;

	// Token: 0x04001D0F RID: 7439
	public Animator[] animators;

	// Token: 0x04001D10 RID: 7440
	public float animationTransitionDuration = 1f;

	// Token: 0x04001D11 RID: 7441
	[Tooltip("Whether the wearable state is toggled on by default.")]
	public bool startOn;

	// Token: 0x04001D12 RID: 7442
	[Tooltip("AudioSource to play toggle sounds.")]
	public AudioSource audioSource;

	// Token: 0x04001D13 RID: 7443
	[Tooltip("Sound to play when toggled on.")]
	public AudioClip toggleOnSound;

	// Token: 0x04001D14 RID: 7444
	[Tooltip("Sound to play when toggled off.")]
	public AudioClip toggleOffSound;

	// Token: 0x04001D15 RID: 7445
	[Tooltip("Layer to check for trigger sphere collisions.")]
	public LayerMask layerMask;

	// Token: 0x04001D16 RID: 7446
	[Tooltip("Radius of the trigger sphere.")]
	public float triggerRadius = 0.2f;

	// Token: 0x04001D17 RID: 7447
	[Tooltip("Position in local space to move the trigger sphere.")]
	public Vector3 triggerOffset = Vector3.zero;

	// Token: 0x04001D18 RID: 7448
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	public VRRig.WearablePackedStateSlots assignedSlot;

	// Token: 0x04001D19 RID: 7449
	[Header("Vibration")]
	public float turnOnVibrationDuration = 0.05f;

	// Token: 0x04001D1A RID: 7450
	public float turnOnVibrationStrength = 0.2f;

	// Token: 0x04001D1B RID: 7451
	public float turnOffVibrationDuration = 0.05f;

	// Token: 0x04001D1C RID: 7452
	public float turnOffVibrationStrength = 0.2f;

	// Token: 0x04001D1D RID: 7453
	private VRRig ownerRig;

	// Token: 0x04001D1E RID: 7454
	private bool ownerIsLocal;

	// Token: 0x04001D1F RID: 7455
	private bool isOn;

	// Token: 0x04001D20 RID: 7456
	[SerializeField]
	private Vector2 toggleCooldownRange = new Vector2(0.2f, 0.2f);

	// Token: 0x04001D21 RID: 7457
	private bool hasAudioSource;

	// Token: 0x04001D22 RID: 7458
	private readonly Collider[] colliders = new Collider[1];

	// Token: 0x04001D23 RID: 7459
	private int framesSinceCooldownAndExitingVolume;

	// Token: 0x04001D24 RID: 7460
	private float toggleCooldownTimer;

	// Token: 0x04001D25 RID: 7461
	private int assignedSlotBitIndex;

	// Token: 0x04001D26 RID: 7462
	private static readonly int animParam_Progress = Animator.StringToHash("Progress");

	// Token: 0x04001D27 RID: 7463
	private float progress;

	// Token: 0x04001D28 RID: 7464
	[SerializeField]
	private bool oneShot;

	// Token: 0x04001D29 RID: 7465
	[SerializeField]
	[Tooltip("Seconds before reverting to its default state, as defined by 'Start On.' A value of 0 or less means never.")]
	private float resetTimer;

	// Token: 0x04001D2A RID: 7466
	private float toggleTimer;
}
