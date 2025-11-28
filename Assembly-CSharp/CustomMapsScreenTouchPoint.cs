using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaTag;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using UnityEngine;

// Token: 0x020009A4 RID: 2468
public abstract class CustomMapsScreenTouchPoint : MonoBehaviour
{
	// Token: 0x06003EFA RID: 16122 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void Awake()
	{
	}

	// Token: 0x06003EFB RID: 16123 RVA: 0x00151A0C File Offset: 0x0014FC0C
	protected virtual void OnDisable()
	{
		if (this.colorUpdateCoroutine != null)
		{
			base.StopCoroutine(this.colorUpdateCoroutine);
		}
		if (this.buttonColorSettings != null)
		{
			this.touchPointRenderer.color = this.buttonColorSettings.UnpressedColor;
		}
	}

	// Token: 0x06003EFC RID: 16124 RVA: 0x00151A48 File Offset: 0x0014FC48
	private void OnTriggerEnter(Collider collider)
	{
		GTDev.Log<string>(string.Format("trigger {0} pressTime={1} time={2}", base.gameObject.name, CustomMapsScreenTouchPoint.pressTime, Time.time), null);
		if (Time.time < CustomMapsScreenTouchPoint.pressTime + CustomMapsScreenTouchPoint.pressedTime)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			Vector3 vector = this.GetForwardDirection();
			if (Vector3.Dot((collider.transform.position - base.transform.position).normalized, vector) < 0f)
			{
				return;
			}
			GTDev.Log<string>(string.Format("trigger {0} collider {1} postion {2}", base.gameObject.name, collider.gameObject.name, collider.transform.position), null);
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			CustomMapsScreenTouchPoint.pressTime = Time.time;
			this.OnButtonPressedEvent();
			this.PressButtonColourUpdate();
			if (this.screen != null)
			{
				this.screen.PressButton(this.keyBinding);
			}
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x06003EFD RID: 16125 RVA: 0x00151B85 File Offset: 0x0014FD85
	public virtual void PressButtonColourUpdate()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.touchPointRenderer.color = this.buttonColorSettings.PressedColor;
		this.colorUpdateCoroutine = base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|12_0());
	}

	// Token: 0x06003EFE RID: 16126 RVA: 0x00151BC0 File Offset: 0x0014FDC0
	private Vector3 GetForwardDirection()
	{
		switch (this.forwardDirection)
		{
		case CustomMapsScreenTouchPoint.TouchPointDirections.Forward:
			return base.transform.forward;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Backward:
			return -base.transform.forward;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Left:
			return -base.transform.right;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Right:
			return base.transform.right;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Up:
			return base.transform.up;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Down:
			return -base.transform.up;
		default:
			return base.transform.forward;
		}
	}

	// Token: 0x06003EFF RID: 16127
	protected abstract void OnButtonPressedEvent();

	// Token: 0x06003F02 RID: 16130 RVA: 0x00151C62 File Offset: 0x0014FE62
	[CompilerGenerated]
	private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|12_0()
	{
		yield return new WaitForSeconds(CustomMapsScreenTouchPoint.pressedTime);
		if (CustomMapsScreenTouchPoint.pressTime != 0f && Time.time > CustomMapsScreenTouchPoint.pressedTime + CustomMapsScreenTouchPoint.pressTime)
		{
			this.touchPointRenderer.color = this.buttonColorSettings.UnpressedColor;
			CustomMapsScreenTouchPoint.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x04005026 RID: 20518
	[SerializeField]
	private CustomMapsTerminalScreen screen;

	// Token: 0x04005027 RID: 20519
	[SerializeField]
	private CustomMapKeyboardBinding keyBinding;

	// Token: 0x04005028 RID: 20520
	[SerializeField]
	private CustomMapsScreenTouchPoint.TouchPointDirections forwardDirection;

	// Token: 0x04005029 RID: 20521
	[SerializeField]
	protected SpriteRenderer touchPointRenderer;

	// Token: 0x0400502A RID: 20522
	[SerializeField]
	protected ButtonColorSettings buttonColorSettings;

	// Token: 0x0400502B RID: 20523
	private static float pressedTime = 0.25f;

	// Token: 0x0400502C RID: 20524
	protected static float pressTime;

	// Token: 0x0400502D RID: 20525
	private Coroutine colorUpdateCoroutine;

	// Token: 0x020009A5 RID: 2469
	public enum TouchPointDirections
	{
		// Token: 0x0400502F RID: 20527
		Forward,
		// Token: 0x04005030 RID: 20528
		Backward,
		// Token: 0x04005031 RID: 20529
		Left,
		// Token: 0x04005032 RID: 20530
		Right,
		// Token: 0x04005033 RID: 20531
		Up,
		// Token: 0x04005034 RID: 20532
		Down
	}
}
