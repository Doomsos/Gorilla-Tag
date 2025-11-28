using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000806 RID: 2054
public class HeldButton : MonoBehaviour
{
	// Token: 0x0600360C RID: 13836 RVA: 0x00125384 File Offset: 0x00123584
	private void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		if ((componentInParent.isLeftHand && !this.leftHandPressable) || (!componentInParent.isLeftHand && !this.rightHandPressable))
		{
			return;
		}
		if (!this.pendingPress || other != this.pendingPressCollider)
		{
			UnityEvent unityEvent = this.onStartPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.touchTime = Time.time;
			this.pendingPressCollider = other;
			this.pressingHand = componentInParent;
			this.pendingPress = true;
			this.SetOn(true);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x00125444 File Offset: 0x00123644
	private void LateUpdate()
	{
		if (!this.pendingPress)
		{
			return;
		}
		if (this.touchTime < this.releaseTime && this.releaseTime + this.debounceTime < Time.time)
		{
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.SetOn(false);
			return;
		}
		if (this.touchTime + this.pressDuration < Time.time)
		{
			this.onPressButton.Invoke();
			if (this.pressingHand != null)
			{
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, this.pressingHand.isLeftHand, 0.1f);
				GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			UnityEvent unityEvent2 = this.onStopPressingButton;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.releaseTime = Time.time;
			this.SetOn(false);
			return;
		}
		if (this.touchTime > this.releaseTime && this.pressingHand != null)
		{
			GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, Time.fixedDeltaTime);
		}
	}

	// Token: 0x0600360E RID: 13838 RVA: 0x001255A3 File Offset: 0x001237A3
	private void OnTriggerExit(Collider other)
	{
		if (this.pendingPress && this.pendingPressCollider == other)
		{
			this.releaseTime = Time.time;
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x0600360F RID: 13839 RVA: 0x001255D8 File Offset: 0x001237D8
	public void SetOn(bool inOn)
	{
		if (inOn == this.isOn)
		{
			return;
		}
		this.isOn = inOn;
		if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
	}

	// Token: 0x04004565 RID: 17765
	public Material pressedMaterial;

	// Token: 0x04004566 RID: 17766
	public Material unpressedMaterial;

	// Token: 0x04004567 RID: 17767
	public MeshRenderer buttonRenderer;

	// Token: 0x04004568 RID: 17768
	private bool isOn;

	// Token: 0x04004569 RID: 17769
	public float debounceTime = 0.25f;

	// Token: 0x0400456A RID: 17770
	public bool leftHandPressable;

	// Token: 0x0400456B RID: 17771
	public bool rightHandPressable = true;

	// Token: 0x0400456C RID: 17772
	public float pressDuration = 0.5f;

	// Token: 0x0400456D RID: 17773
	public UnityEvent onStartPressingButton;

	// Token: 0x0400456E RID: 17774
	public UnityEvent onStopPressingButton;

	// Token: 0x0400456F RID: 17775
	public UnityEvent onPressButton;

	// Token: 0x04004570 RID: 17776
	[TextArea]
	public string offText;

	// Token: 0x04004571 RID: 17777
	[TextArea]
	public string onText;

	// Token: 0x04004572 RID: 17778
	public Text myText;

	// Token: 0x04004573 RID: 17779
	private float touchTime;

	// Token: 0x04004574 RID: 17780
	private float releaseTime;

	// Token: 0x04004575 RID: 17781
	private bool pendingPress;

	// Token: 0x04004576 RID: 17782
	private Collider pendingPressCollider;

	// Token: 0x04004577 RID: 17783
	private GorillaTriggerColliderHandIndicator pressingHand;
}
