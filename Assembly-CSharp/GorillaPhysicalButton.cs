using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000916 RID: 2326
public class GorillaPhysicalButton : MonoBehaviour
{
	// Token: 0x14000075 RID: 117
	// (add) Token: 0x06003B59 RID: 15193 RVA: 0x001397F0 File Offset: 0x001379F0
	// (remove) Token: 0x06003B5A RID: 15194 RVA: 0x00139828 File Offset: 0x00137A28
	public event Action<GorillaPhysicalButton, bool> onPressedOn;

	// Token: 0x14000076 RID: 118
	// (add) Token: 0x06003B5B RID: 15195 RVA: 0x00139860 File Offset: 0x00137A60
	// (remove) Token: 0x06003B5C RID: 15196 RVA: 0x00139898 File Offset: 0x00137A98
	public event Action<GorillaPhysicalButton, bool> onToggledOff;

	// Token: 0x06003B5D RID: 15197 RVA: 0x001398D0 File Offset: 0x00137AD0
	public virtual void Start()
	{
		if (this.moveableChildren != null)
		{
			this.moveableChildrenStartPositions = new List<Vector3>(this.moveableChildren.Count);
			for (int i = 0; i < this.moveableChildren.Count; i++)
			{
				this.moveableChildrenStartPositions.Add(this.moveableChildren[i].position);
			}
		}
		this.startButtonPosition = base.transform.position;
		base.enabled = true;
	}

	// Token: 0x06003B5E RID: 15198 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x06003B5F RID: 15199 RVA: 0x00002789 File Offset: 0x00000989
	private void OnDisable()
	{
	}

	// Token: 0x06003B60 RID: 15200 RVA: 0x00139948 File Offset: 0x00137B48
	private float GetSurfaceDistanceFromKeyToCollider(Collider collider)
	{
		if (collider == null)
		{
			return 1f;
		}
		SphereCollider sphereCollider = collider as SphereCollider;
		float num = sphereCollider ? sphereCollider.radius : 0f;
		float num2 = base.transform.localScale.z * 0.5f;
		if (Vector3.Distance(collider.transform.position, base.transform.position) > (base.transform.localScale.magnitude * 0.5f + num) * 1.5f)
		{
			return 1f;
		}
		return Vector3.Dot(base.transform.position - collider.transform.position, -base.transform.forward) - num - num2;
	}

	// Token: 0x06003B61 RID: 15201 RVA: 0x00139A10 File Offset: 0x00137C10
	protected void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.recentFingerCollider = other;
		if (this.buttonTestCoroutine == null)
		{
			this.buttonTestCoroutine = base.StartCoroutine(this.ButtonUpdate());
		}
	}

	// Token: 0x06003B62 RID: 15202 RVA: 0x00139A4B File Offset: 0x00137C4B
	protected IEnumerator ButtonUpdate()
	{
		for (;;)
		{
			this.UpdateButtonFromCollider();
			if (!base.enabled || this.recentFingerCollider == null)
			{
				break;
			}
			yield return null;
		}
		this.buttonTestCoroutine = null;
		yield break;
	}

	// Token: 0x06003B63 RID: 15203 RVA: 0x00139A5C File Offset: 0x00137C5C
	protected void UpdateButtonFromCollider()
	{
		if (this.recentFingerCollider != null)
		{
			float surfaceDistanceFromKeyToCollider = this.GetSurfaceDistanceFromKeyToCollider(this.recentFingerCollider);
			this.currentButtonDepthFromPressing -= surfaceDistanceFromKeyToCollider;
			this.currentButtonDepthFromPressing = Mathf.Clamp(this.currentButtonDepthFromPressing, 0f, this.buttonPushDepth);
		}
		else
		{
			this.currentButtonDepthFromPressing = 0f;
		}
		if (this.currentButtonDepthFromPressing == 0f)
		{
			if (!this.canToggleOn && !this.canToggleOff)
			{
				this.isOn = false;
			}
			this.recentFingerCollider = null;
			this.waitingForReleaseAfterStateChange = false;
		}
		this.TestForButtonStateChange();
		this.UpdateButtonVisuals();
	}

	// Token: 0x06003B64 RID: 15204 RVA: 0x00139AFC File Offset: 0x00137CFC
	protected void TestForButtonStateChange()
	{
		if (this.waitingForReleaseAfterStateChange)
		{
			return;
		}
		if (this.currentButtonDepthFromPressing > this.buttonDepthForTrigger && !this.isOn && this.recentFingerCollider != null)
		{
			this.isOn = true;
			this.waitingForReleaseAfterStateChange = true;
			GorillaTriggerColliderHandIndicator component = this.recentFingerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component == null)
			{
				return;
			}
			UnityEvent unityEvent = this.onPressButtonOn;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			Action<GorillaPhysicalButton, bool> action = this.onPressedOn;
			if (action != null)
			{
				action.Invoke(this, component.isLeftHand);
			}
			this.ButtonPressedOn();
			this.ButtonPressedOnWithHand(component.isLeftHand);
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
				return;
			}
		}
		else if (this.currentButtonDepthFromPressing > this.buttonDepthForTrigger && this.canToggleOff && this.isOn && this.recentFingerCollider != null)
		{
			this.isOn = false;
			this.waitingForReleaseAfterStateChange = true;
			GorillaTriggerColliderHandIndicator component2 = this.recentFingerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component2 == null)
			{
				return;
			}
			UnityEvent unityEvent2 = this.onPressButtonToggleOff;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			Action<GorillaPhysicalButton, bool> action2 = this.onToggledOff;
			if (action2 != null)
			{
				action2.Invoke(this, component2.isLeftHand);
			}
			this.ButtonToggledOff();
			this.ButtonToggledOffWithHand(component2.isLeftHand);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component2.isLeftHand, 0.05f);
			GorillaTagger.Instance.StartVibration(component2.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
				{
					67,
					component2.isLeftHand,
					0.05f
				});
			}
		}
	}

	// Token: 0x06003B65 RID: 15205 RVA: 0x00139D8C File Offset: 0x00137F8C
	protected void UpdateButtonVisuals()
	{
		float num = this.currentButtonDepthFromPressing;
		if ((this.canToggleOff || this.canToggleOn) && this.isOn)
		{
			num = Mathf.Max(this.buttonDepthForTrigger, num);
		}
		base.transform.position = this.startButtonPosition - base.transform.forward * num;
		if (this.moveableChildren != null)
		{
			for (int i = 0; i < this.moveableChildren.Count; i++)
			{
				this.moveableChildren[i].position = this.moveableChildrenStartPositions[i] - base.transform.forward * num;
			}
		}
		this.UpdateColorWithState(this.isOn);
	}

	// Token: 0x06003B66 RID: 15206 RVA: 0x00139E4C File Offset: 0x0013804C
	protected void UpdateColorWithState(bool state)
	{
		if (state)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if ((!string.IsNullOrEmpty(this.onText) || !string.IsNullOrEmpty(this.offText)) && this.textField != null)
			{
				this.textField.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if ((!string.IsNullOrEmpty(this.offText) || !string.IsNullOrEmpty(this.onText)) && this.textField != null)
			{
				this.textField.text = this.offText;
			}
		}
	}

	// Token: 0x06003B67 RID: 15207 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonPressedOn()
	{
	}

	// Token: 0x06003B68 RID: 15208 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonPressedOnWithHand(bool isLeftHand)
	{
	}

	// Token: 0x06003B69 RID: 15209 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonToggledOff()
	{
	}

	// Token: 0x06003B6A RID: 15210 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonToggledOffWithHand(bool isLeftHand)
	{
	}

	// Token: 0x06003B6B RID: 15211 RVA: 0x00139EF1 File Offset: 0x001380F1
	public virtual void ResetState()
	{
		this.isOn = false;
		this.currentButtonDepthFromPressing = 0f;
		this.waitingForReleaseAfterStateChange = false;
		this.UpdateButtonVisuals();
	}

	// Token: 0x06003B6C RID: 15212 RVA: 0x00139F12 File Offset: 0x00138112
	public void SetText(string newText)
	{
		if (this.textField != null)
		{
			this.textField.text = this.offText;
		}
	}

	// Token: 0x06003B6D RID: 15213 RVA: 0x00139F34 File Offset: 0x00138134
	public virtual void SetButtonState(bool setToOn)
	{
		if (this.canToggleOn || this.canToggleOff)
		{
			if (this.isOn != setToOn)
			{
				this.isOn = setToOn;
				if (this.isOn)
				{
					UnityEvent unityEvent = this.onPressButtonOn;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.ButtonPressedOn();
				}
				else
				{
					UnityEvent unityEvent2 = this.onPressButtonToggleOff;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.ButtonToggledOff();
				}
			}
			this.UpdateButtonVisuals();
		}
	}

	// Token: 0x04004BC2 RID: 19394
	public Material pressedMaterial;

	// Token: 0x04004BC3 RID: 19395
	public Material unpressedMaterial;

	// Token: 0x04004BC4 RID: 19396
	public MeshRenderer buttonRenderer;

	// Token: 0x04004BC5 RID: 19397
	public int pressButtonSoundIndex = 67;

	// Token: 0x04004BC6 RID: 19398
	[SerializeField]
	public bool canToggleOn;

	// Token: 0x04004BC7 RID: 19399
	public bool canToggleOff;

	// Token: 0x04004BC8 RID: 19400
	private bool waitingForReleaseAfterStateChange;

	// Token: 0x04004BC9 RID: 19401
	public bool isOn;

	// Token: 0x04004BCA RID: 19402
	public bool testPress;

	// Token: 0x04004BCB RID: 19403
	public bool testHandLeft;

	// Token: 0x04004BCC RID: 19404
	[SerializeField]
	protected float buttonPushDepth = 0.0125f;

	// Token: 0x04004BCD RID: 19405
	[SerializeField]
	protected float buttonDepthForTrigger = 0.01f;

	// Token: 0x04004BCE RID: 19406
	[SerializeField]
	public List<Transform> moveableChildren;

	// Token: 0x04004BCF RID: 19407
	[NonSerialized]
	public List<Vector3> moveableChildrenStartPositions;

	// Token: 0x04004BD0 RID: 19408
	private Vector3 startButtonPosition;

	// Token: 0x04004BD1 RID: 19409
	[TextArea]
	public string offText = "OFF";

	// Token: 0x04004BD2 RID: 19410
	[TextArea]
	public string onText = "ON";

	// Token: 0x04004BD3 RID: 19411
	[SerializeField]
	public TMP_Text textField;

	// Token: 0x04004BD4 RID: 19412
	[Space]
	public UnityEvent onPressButtonOn;

	// Token: 0x04004BD5 RID: 19413
	public UnityEvent onPressButtonToggleOff;

	// Token: 0x04004BD8 RID: 19416
	private Collider recentFingerCollider;

	// Token: 0x04004BD9 RID: 19417
	protected float currentButtonDepthFromPressing;

	// Token: 0x04004BDA RID: 19418
	private Coroutine buttonTestCoroutine;
}
