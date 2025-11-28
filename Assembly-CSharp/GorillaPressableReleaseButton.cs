using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200091E RID: 2334
public class GorillaPressableReleaseButton : GorillaPressableButton
{
	// Token: 0x06003BA7 RID: 15271 RVA: 0x0013B448 File Offset: 0x00139648
	private new void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (this.touchingCollider)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component == null)
		{
			return;
		}
		this.touchTime = Time.time;
		this.touchingCollider = other;
		UnityEvent onPressButton = this.onPressButton;
		if (onPressButton != null)
		{
			onPressButton.Invoke();
		}
		this.ButtonActivation();
		this.ButtonActivationWithHand(component.isLeftHand);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component.isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
			{
				67,
				component.isLeftHand,
				0.05f
			});
		}
	}

	// Token: 0x06003BA8 RID: 15272 RVA: 0x0013B570 File Offset: 0x00139770
	private void OnTriggerExit(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (other != this.touchingCollider)
		{
			return;
		}
		this.touchingCollider = null;
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component == null)
		{
			return;
		}
		UnityEvent unityEvent = this.onReleaseButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this.ButtonDeactivation();
		this.ButtonDeactivationWithHand(component.isLeftHand);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component.isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
			{
				67,
				component.isLeftHand,
				0.05f
			});
		}
	}

	// Token: 0x06003BA9 RID: 15273 RVA: 0x0013B678 File Offset: 0x00139878
	public override void ResetState()
	{
		base.ResetState();
		this.touchingCollider = null;
	}

	// Token: 0x06003BAA RID: 15274 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonDeactivation()
	{
	}

	// Token: 0x06003BAB RID: 15275 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonDeactivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x04004C37 RID: 19511
	public UnityEvent onReleaseButton;

	// Token: 0x04004C38 RID: 19512
	private Collider touchingCollider;
}
