using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000912 RID: 2322
public class GorillaHeldItemPressableButton : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x14000073 RID: 115
	// (add) Token: 0x06003B3B RID: 15163 RVA: 0x00139064 File Offset: 0x00137264
	// (remove) Token: 0x06003B3C RID: 15164 RVA: 0x0013909C File Offset: 0x0013729C
	public event Action<GorillaHeldItemPressableButton, TransferrableObject, bool> onPressed;

	// Token: 0x14000074 RID: 116
	// (add) Token: 0x06003B3D RID: 15165 RVA: 0x001390D4 File Offset: 0x001372D4
	// (remove) Token: 0x06003B3E RID: 15166 RVA: 0x0013910C File Offset: 0x0013730C
	public event Action<GorillaHeldItemPressableButton, TransferrableObject, bool> onReleased;

	// Token: 0x06003B3F RID: 15167 RVA: 0x00139144 File Offset: 0x00137344
	private void Start()
	{
		if (this.acceptAnyHoldableThatMatchesType)
		{
			this.acceptedTypes = new List<Type>();
			foreach (TransferrableObject transferrableObject in this.acceptedHoldables)
			{
				this.acceptedTypes.Add(transferrableObject.GetType());
			}
		}
	}

	// Token: 0x06003B40 RID: 15168 RVA: 0x001391B4 File Offset: 0x001373B4
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.delayBetweenSuccessfulPresses >= Time.time)
		{
			return;
		}
		TransferrableObject componentInParent = collider.GetComponentInParent<TransferrableObject>();
		if (componentInParent == null)
		{
			componentInParent = collider.transform.parent.GetComponentInParent<TransferrableObject>();
		}
		if (componentInParent == null || !componentInParent.InHand())
		{
			return;
		}
		if (this.acceptAnyHoldableThatMatchesType)
		{
			if (!this.acceptedTypes.Contains(componentInParent.GetType()))
			{
				return;
			}
		}
		else if (!this.acceptedHoldables.Contains(componentInParent))
		{
			return;
		}
		this.touchTime = Time.time;
		switch (this.mode)
		{
		case HeldItemButtonMode.OneShot:
		{
			UnityEvent<TransferrableObject> unityEvent = this.onPressButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke(componentInParent);
			}
			Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action = this.onPressed;
			if (action != null)
			{
				action.Invoke(this, componentInParent, componentInParent.InLeftHand());
			}
			this.ButtonActivation(componentInParent);
			this.ButtonActivationWithHand(componentInParent, componentInParent.InLeftHand());
			break;
		}
		case HeldItemButtonMode.ResetAfterDelay:
		{
			this.isOn = true;
			UnityEvent<TransferrableObject> unityEvent2 = this.onPressButton;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(componentInParent);
			}
			Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action2 = this.onPressed;
			if (action2 != null)
			{
				action2.Invoke(this, componentInParent, componentInParent.InLeftHand());
			}
			this.ButtonActivation(componentInParent);
			this.ButtonActivationWithHand(componentInParent, componentInParent.InLeftHand());
			GTDelayedExec.Add(this, this.delayBetweenSuccessfulPresses, 0);
			break;
		}
		case HeldItemButtonMode.Toggle:
			this.isOn = !this.isOn;
			if (this.isOn)
			{
				UnityEvent<TransferrableObject> unityEvent3 = this.onPressButton;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(componentInParent);
				}
				Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action3 = this.onPressed;
				if (action3 != null)
				{
					action3.Invoke(this, componentInParent, componentInParent.InLeftHand());
				}
				this.ButtonActivation(componentInParent);
				this.ButtonActivationWithHand(componentInParent, componentInParent.InLeftHand());
			}
			else
			{
				UnityEvent<TransferrableObject> unityEvent4 = this.onReleaseButton;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke(componentInParent);
				}
				Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action4 = this.onReleased;
				if (action4 != null)
				{
					action4.Invoke(this, componentInParent, componentInParent.InLeftHand());
				}
			}
			break;
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, componentInParent.InLeftHand(), 0.05f);
		GorillaTagger.Instance.StartVibration(componentInParent.InLeftHand(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
			{
				67,
				componentInParent.InLeftHand(),
				0.05f
			});
		}
		switch (this.consumeItem)
		{
		case HeldItemButtonConsumeMode.None:
			break;
		case HeldItemButtonConsumeMode.Destroy:
			componentInParent.OnMyCreatorLeft();
			return;
		case HeldItemButtonConsumeMode.Disable:
			componentInParent.gameObject.SetActive(false);
			break;
		default:
			return;
		}
	}

	// Token: 0x06003B41 RID: 15169 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonActivation(TransferrableObject holdable)
	{
	}

	// Token: 0x06003B42 RID: 15170 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonActivationWithHand(TransferrableObject holdable, bool isLeftHand)
	{
	}

	// Token: 0x06003B43 RID: 15171 RVA: 0x0013945A File Offset: 0x0013765A
	public virtual void ResetState()
	{
		this.isOn = false;
		UnityEvent<TransferrableObject> unityEvent = this.onReleaseButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke(null);
		}
		Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action = this.onReleased;
		if (action == null)
		{
			return;
		}
		action.Invoke(this, null, false);
	}

	// Token: 0x06003B44 RID: 15172 RVA: 0x00139488 File Offset: 0x00137688
	public void OnDelayedAction(int contextIndex)
	{
		this.ResetState();
	}

	// Token: 0x04004BA5 RID: 19365
	public int pressButtonSoundIndex = 67;

	// Token: 0x04004BA6 RID: 19366
	public bool isOn;

	// Token: 0x04004BA7 RID: 19367
	public float delayBetweenSuccessfulPresses = 0.25f;

	// Token: 0x04004BA8 RID: 19368
	private float touchTime;

	// Token: 0x04004BA9 RID: 19369
	public HeldItemButtonMode mode;

	// Token: 0x04004BAA RID: 19370
	public List<TransferrableObject> acceptedHoldables;

	// Token: 0x04004BAB RID: 19371
	private List<Type> acceptedTypes;

	// Token: 0x04004BAC RID: 19372
	public bool acceptAnyHoldableThatMatchesType = true;

	// Token: 0x04004BAD RID: 19373
	public HeldItemButtonConsumeMode consumeItem;

	// Token: 0x04004BAE RID: 19374
	[Space]
	public UnityEvent<TransferrableObject> onPressButton;

	// Token: 0x04004BB0 RID: 19376
	[Space]
	public UnityEvent<TransferrableObject> onReleaseButton;
}
