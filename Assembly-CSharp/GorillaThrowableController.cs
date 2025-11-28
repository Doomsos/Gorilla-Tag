using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x02000924 RID: 2340
public class GorillaThrowableController : MonoBehaviour
{
	// Token: 0x06003BD8 RID: 15320 RVA: 0x0013C0C9 File Offset: 0x0013A2C9
	protected void Awake()
	{
		this.gorillaThrowableLayerMask = LayerMask.GetMask(new string[]
		{
			"GorillaThrowable"
		});
	}

	// Token: 0x06003BD9 RID: 15321 RVA: 0x0013C0E4 File Offset: 0x0013A2E4
	private void LateUpdate()
	{
		if (this.testCanGrab)
		{
			this.testCanGrab = false;
			this.CanGrabAnObject(this.rightHandController, out this.returnCollider);
			Debug.Log(this.returnCollider.gameObject, this.returnCollider.gameObject);
		}
		if (this.leftHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(4))
			{
				if (this.leftHandGrabbedObject != null)
				{
					this.leftHandGrabbedObject.ThrowThisThingo();
					this.leftHandGrabbedObject = null;
				}
				this.leftHandIsGrabbing = false;
			}
		}
		else if (this.CheckIfHandHasGrabbed(4))
		{
			this.leftHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.leftHandController, out this.returnCollider))
			{
				this.leftHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.leftHandGrabbedObject.Grabbed(this.leftHandController);
			}
		}
		if (this.rightHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(5))
			{
				if (this.rightHandGrabbedObject != null)
				{
					this.rightHandGrabbedObject.ThrowThisThingo();
					this.rightHandGrabbedObject = null;
				}
				this.rightHandIsGrabbing = false;
				return;
			}
		}
		else if (this.CheckIfHandHasGrabbed(5))
		{
			this.rightHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.rightHandController, out this.returnCollider))
			{
				this.rightHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.rightHandGrabbedObject.Grabbed(this.rightHandController);
			}
		}
	}

	// Token: 0x06003BDA RID: 15322 RVA: 0x0013C230 File Offset: 0x0013A430
	private bool CheckIfHandHasReleased(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == 4) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(1) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(2));
		if (this.triggerValue < 0.75f)
		{
			this.triggerValue = ((node == 4) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(1) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(2));
			if (this.triggerValue < 0.75f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003BDB RID: 15323 RVA: 0x0013C2AC File Offset: 0x0013A4AC
	private bool CheckIfHandHasGrabbed(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == 4) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(1) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(2));
		if (this.triggerValue > 0.75f)
		{
			return true;
		}
		this.triggerValue = ((node == 4) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(1) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(2));
		return this.triggerValue > 0.75f;
	}

	// Token: 0x06003BDC RID: 15324 RVA: 0x0013C328 File Offset: 0x0013A528
	private bool CanGrabAnObject(Transform handTransform, out Collider returnCollider)
	{
		this.magnitude = 100f;
		returnCollider = null;
		Debug.Log("trying:");
		if (Physics.OverlapSphereNonAlloc(handTransform.position, this.handRadius, this.colliders, this.gorillaThrowableLayerMask) > 0)
		{
			Debug.Log("found something!");
			this.minCollider = this.colliders[0];
			foreach (Collider collider in this.colliders)
			{
				if (collider != null)
				{
					Debug.Log("found this", collider);
					if ((collider.transform.position - handTransform.position).magnitude < this.magnitude)
					{
						this.minCollider = collider;
						this.magnitude = (collider.transform.position - handTransform.position).magnitude;
					}
				}
			}
			returnCollider = this.minCollider;
			return true;
		}
		return false;
	}

	// Token: 0x06003BDD RID: 15325 RVA: 0x0013C411 File Offset: 0x0013A611
	public void GrabbableObjectHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, this.hoverVibrationStrength, this.hoverVibrationDuration);
	}

	// Token: 0x04004C5A RID: 19546
	public Transform leftHandController;

	// Token: 0x04004C5B RID: 19547
	public Transform rightHandController;

	// Token: 0x04004C5C RID: 19548
	public bool leftHandIsGrabbing;

	// Token: 0x04004C5D RID: 19549
	public bool rightHandIsGrabbing;

	// Token: 0x04004C5E RID: 19550
	public GorillaThrowable leftHandGrabbedObject;

	// Token: 0x04004C5F RID: 19551
	public GorillaThrowable rightHandGrabbedObject;

	// Token: 0x04004C60 RID: 19552
	public float hoverVibrationStrength = 0.25f;

	// Token: 0x04004C61 RID: 19553
	public float hoverVibrationDuration = 0.05f;

	// Token: 0x04004C62 RID: 19554
	public float handRadius = 0.05f;

	// Token: 0x04004C63 RID: 19555
	private InputDevice rightDevice;

	// Token: 0x04004C64 RID: 19556
	private InputDevice leftDevice;

	// Token: 0x04004C65 RID: 19557
	private InputDevice inputDevice;

	// Token: 0x04004C66 RID: 19558
	private float triggerValue;

	// Token: 0x04004C67 RID: 19559
	private bool boolVar;

	// Token: 0x04004C68 RID: 19560
	private Collider[] colliders = new Collider[10];

	// Token: 0x04004C69 RID: 19561
	private Collider minCollider;

	// Token: 0x04004C6A RID: 19562
	private Collider returnCollider;

	// Token: 0x04004C6B RID: 19563
	private float magnitude;

	// Token: 0x04004C6C RID: 19564
	public bool testCanGrab;

	// Token: 0x04004C6D RID: 19565
	private int gorillaThrowableLayerMask;
}
