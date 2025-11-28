using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000531 RID: 1329
public class PrimaryButtonWatcher : MonoBehaviour
{
	// Token: 0x06002181 RID: 8577 RVA: 0x000AFA9B File Offset: 0x000ADC9B
	private void Awake()
	{
		if (this.primaryButtonPress == null)
		{
			this.primaryButtonPress = new PrimaryButtonEvent();
		}
		this.devicesWithPrimaryButton = new List<InputDevice>();
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x000AFABC File Offset: 0x000ADCBC
	private void OnEnable()
	{
		List<InputDevice> list = new List<InputDevice>();
		InputDevices.GetDevices(list);
		foreach (InputDevice device in list)
		{
			this.InputDevices_deviceConnected(device);
		}
		InputDevices.deviceConnected += new Action<InputDevice>(this.InputDevices_deviceConnected);
		InputDevices.deviceDisconnected += new Action<InputDevice>(this.InputDevices_deviceDisconnected);
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x000AFB38 File Offset: 0x000ADD38
	private void OnDisable()
	{
		InputDevices.deviceConnected -= new Action<InputDevice>(this.InputDevices_deviceConnected);
		InputDevices.deviceDisconnected -= new Action<InputDevice>(this.InputDevices_deviceDisconnected);
		this.devicesWithPrimaryButton.Clear();
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000AFB68 File Offset: 0x000ADD68
	private void InputDevices_deviceConnected(InputDevice device)
	{
		bool flag;
		if (device.TryGetFeatureValue(CommonUsages.primaryButton, ref flag))
		{
			this.devicesWithPrimaryButton.Add(device);
		}
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x000AFB91 File Offset: 0x000ADD91
	private void InputDevices_deviceDisconnected(InputDevice device)
	{
		if (this.devicesWithPrimaryButton.Contains(device))
		{
			this.devicesWithPrimaryButton.Remove(device);
		}
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x000AFBB0 File Offset: 0x000ADDB0
	private void Update()
	{
		bool flag = false;
		foreach (InputDevice inputDevice in this.devicesWithPrimaryButton)
		{
			bool flag2 = false;
			flag = ((inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, ref flag2) && flag2) || flag);
		}
		if (flag != this.lastButtonState)
		{
			this.primaryButtonPress.Invoke(flag);
			this.lastButtonState = flag;
		}
	}

	// Token: 0x04002C3D RID: 11325
	public PrimaryButtonEvent primaryButtonPress;

	// Token: 0x04002C3E RID: 11326
	private bool lastButtonState;

	// Token: 0x04002C3F RID: 11327
	private List<InputDevice> devicesWithPrimaryButton;
}
