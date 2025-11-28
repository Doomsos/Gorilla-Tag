using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x0200092F RID: 2351
public class GorillaFireballControllerManager : MonoBehaviour
{
	// Token: 0x06003C1B RID: 15387 RVA: 0x0013D2F8 File Offset: 0x0013B4F8
	private void Update()
	{
		if (!this.hasInitialized)
		{
			this.hasInitialized = true;
			List<InputDevice> list = new List<InputDevice>();
			List<InputDevice> list2 = new List<InputDevice>();
			InputDevices.GetDevicesAtXRNode(4, list);
			InputDevices.GetDevicesAtXRNode(5, list2);
			if (list.Count == 1)
			{
				this.leftHand = list[0];
			}
			if (list2.Count == 1)
			{
				this.rightHand = list2[0];
			}
		}
		float axis = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(1);
		if (this.leftHandLastState <= this.throwingThreshold && axis > this.throwingThreshold)
		{
			this.CreateFireball(true);
		}
		else if (this.leftHandLastState >= this.throwingThreshold && axis < this.throwingThreshold)
		{
			this.TryThrowFireball(true);
		}
		this.leftHandLastState = axis;
		axis = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(2);
		if (this.rightHandLastState <= this.throwingThreshold && axis > this.throwingThreshold)
		{
			this.CreateFireball(false);
		}
		else if (this.rightHandLastState >= this.throwingThreshold && axis < this.throwingThreshold)
		{
			this.TryThrowFireball(false);
		}
		this.rightHandLastState = axis;
	}

	// Token: 0x06003C1C RID: 15388 RVA: 0x0013D3FC File Offset: 0x0013B5FC
	public void TryThrowFireball(bool isLeftHand)
	{
		if (isLeftHand && GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
			return;
		}
		if (!isLeftHand && GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
		}
	}

	// Token: 0x06003C1D RID: 15389 RVA: 0x0013D47C File Offset: 0x0013B67C
	public void CreateFireball(bool isLeftHand)
	{
		object[] array = new object[1];
		Vector3 position;
		if (isLeftHand)
		{
			array[0] = true;
			position = GorillaPlaySpace.Instance.myVRRig.leftHandTransform.position;
		}
		else
		{
			array[0] = false;
			position = GorillaPlaySpace.Instance.myVRRig.rightHandTransform.position;
		}
		PhotonNetwork.Instantiate("GorillaPrefabs/GorillaFireball", position, Quaternion.identity, 0, array);
	}

	// Token: 0x04004C9D RID: 19613
	public InputDevice leftHand;

	// Token: 0x04004C9E RID: 19614
	public InputDevice rightHand;

	// Token: 0x04004C9F RID: 19615
	public bool hasInitialized;

	// Token: 0x04004CA0 RID: 19616
	public float leftHandLastState;

	// Token: 0x04004CA1 RID: 19617
	public float rightHandLastState;

	// Token: 0x04004CA2 RID: 19618
	public float throwingThreshold = 0.9f;
}
