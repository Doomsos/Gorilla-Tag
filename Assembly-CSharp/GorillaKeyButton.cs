using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000913 RID: 2323
public abstract class GorillaKeyButton<TBinding> : MonoBehaviour where TBinding : Enum
{
	// Token: 0x06003B46 RID: 15174 RVA: 0x001394D2 File Offset: 0x001376D2
	private void Awake()
	{
		if (this.ButtonRenderer == null)
		{
			this.ButtonRenderer = base.GetComponent<Renderer>();
		}
		this.propBlock = new MaterialPropertyBlock();
		this.pressTime = 0f;
	}

	// Token: 0x06003B47 RID: 15175 RVA: 0x00139504 File Offset: 0x00137704
	private void OnEnable()
	{
		for (int i = 0; i < this.linkedObjects.Length; i++)
		{
			if (this.linkedObjects[i].IsNotNull())
			{
				this.linkedObjects[i].SetActive(true);
			}
		}
		this.OnEnableEvents();
	}

	// Token: 0x06003B48 RID: 15176 RVA: 0x00139548 File Offset: 0x00137748
	private void OnDisable()
	{
		for (int i = 0; i < this.linkedObjects.Length; i++)
		{
			if (this.linkedObjects[i].IsNotNull())
			{
				this.linkedObjects[i].SetActive(false);
			}
		}
		this.OnDisableEvents();
	}

	// Token: 0x06003B49 RID: 15177 RVA: 0x0013958C File Offset: 0x0013778C
	private void OnTriggerEnter(Collider collider)
	{
		GorillaTriggerColliderHandIndicator componentInParent = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent)
		{
			this.PressButton(componentInParent.isLeftHand);
		}
	}

	// Token: 0x06003B4A RID: 15178 RVA: 0x001395B4 File Offset: 0x001377B4
	private void PressButton(bool isLeftHand)
	{
		this.OnButtonPressedEvent();
		UnityEvent<TBinding> onKeyButtonPressed = this.OnKeyButtonPressed;
		if (onKeyButtonPressed != null)
		{
			onKeyButtonPressed.Invoke(this.Binding);
		}
		this.PressButtonColourUpdate();
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, isLeftHand, 0.1f);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
			{
				66,
				isLeftHand,
				0.1f
			});
		}
	}

	// Token: 0x06003B4B RID: 15179 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnEnableEvents()
	{
	}

	// Token: 0x06003B4C RID: 15180 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnDisableEvents()
	{
	}

	// Token: 0x06003B4D RID: 15181 RVA: 0x00139679 File Offset: 0x00137879
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x06003B4E RID: 15182 RVA: 0x00139684 File Offset: 0x00137884
	public virtual void PressButtonColourUpdate()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.propBlock.SetColor(ShaderProps._BaseColor, this.ButtonColorSettings.PressedColor);
		this.propBlock.SetColor(ShaderProps._Color, this.ButtonColorSettings.PressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|21_0());
	}

	// Token: 0x06003B4F RID: 15183
	protected abstract void OnButtonPressedEvent();

	// Token: 0x06003B51 RID: 15185 RVA: 0x0013971C File Offset: 0x0013791C
	[CompilerGenerated]
	private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|21_0()
	{
		yield return new WaitForSeconds(this.ButtonColorSettings.PressedTime);
		if (this.pressTime != 0f && Time.time > this.ButtonColorSettings.PressedTime + this.pressTime)
		{
			this.propBlock.SetColor(ShaderProps._BaseColor, this.ButtonColorSettings.UnpressedColor);
			this.propBlock.SetColor(ShaderProps._Color, this.ButtonColorSettings.UnpressedColor);
			this.ButtonRenderer.SetPropertyBlock(this.propBlock);
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x04004BB2 RID: 19378
	public string characterString;

	// Token: 0x04004BB3 RID: 19379
	public TBinding Binding;

	// Token: 0x04004BB4 RID: 19380
	public bool functionKey;

	// Token: 0x04004BB5 RID: 19381
	public Renderer ButtonRenderer;

	// Token: 0x04004BB6 RID: 19382
	public ButtonColorSettings ButtonColorSettings;

	// Token: 0x04004BB7 RID: 19383
	[Tooltip("These GameObjects will be Activated/Deactivated when this button is Activated/Deactivated")]
	public GameObject[] linkedObjects;

	// Token: 0x04004BB8 RID: 19384
	[Tooltip("Intended for use with GorillaKeyWrapper")]
	public UnityEvent<TBinding> OnKeyButtonPressed = new UnityEvent<TBinding>();

	// Token: 0x04004BB9 RID: 19385
	public bool testClick;

	// Token: 0x04004BBA RID: 19386
	public bool repeatTestClick;

	// Token: 0x04004BBB RID: 19387
	public float repeatCooldown = 2f;

	// Token: 0x04004BBC RID: 19388
	private float pressTime;

	// Token: 0x04004BBD RID: 19389
	private float lastTestClick;

	// Token: 0x04004BBE RID: 19390
	protected MaterialPropertyBlock propBlock;
}
