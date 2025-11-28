using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public class GorillaThrowableController : MonoBehaviour
{
	protected void Awake()
	{
		this.gorillaThrowableLayerMask = LayerMask.GetMask(new string[]
		{
			"GorillaThrowable"
		});
	}

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

	public void GrabbableObjectHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, this.hoverVibrationStrength, this.hoverVibrationDuration);
	}

	public Transform leftHandController;

	public Transform rightHandController;

	public bool leftHandIsGrabbing;

	public bool rightHandIsGrabbing;

	public GorillaThrowable leftHandGrabbedObject;

	public GorillaThrowable rightHandGrabbedObject;

	public float hoverVibrationStrength = 0.25f;

	public float hoverVibrationDuration = 0.05f;

	public float handRadius = 0.05f;

	private InputDevice rightDevice;

	private InputDevice leftDevice;

	private InputDevice inputDevice;

	private float triggerValue;

	private bool boolVar;

	private Collider[] colliders = new Collider[10];

	private Collider minCollider;

	private Collider returnCollider;

	private float magnitude;

	public bool testCanGrab;

	private int gorillaThrowableLayerMask;
}
