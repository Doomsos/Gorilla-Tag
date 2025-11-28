using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000248 RID: 584
public class DeployableObject : TransferrableObject
{
	// Token: 0x06000F3E RID: 3902 RVA: 0x00051103 File Offset: 0x0004F303
	protected override void Awake()
	{
		this._deploySignal.OnSignal += this.DeployRPC;
		base.Awake();
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x00051124 File Offset: 0x0004F324
	internal override void OnEnable()
	{
		this._deploySignal.Enable();
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		for (int i = 0; i < this._rigAwareObjects.Length; i++)
		{
			IRigAware rigAware = this._rigAwareObjects[i] as IRigAware;
			if (rigAware != null)
			{
				rigAware.SetRig(componentInParent);
			}
		}
		this.m_VRRig = componentInParent;
		ListProcessor<Action<RigContainer>> disableEvent = this.m_VRRig.rigContainer.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.OnRigPreDisable);
		disableEvent.Add(action);
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x000511C1 File Offset: 0x0004F3C1
	internal override void OnDisable()
	{
		this.m_VRRig = null;
		this._deploySignal.Disable();
		if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
		base.OnDisable();
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x000511F0 File Offset: 0x0004F3F0
	private void OnRigPreDisable(RigContainer rc)
	{
		this.m_spamChecker.Reset();
		ListProcessor<Action<RigContainer>> disableEvent = rc.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.OnRigPreDisable);
		disableEvent.Remove(action);
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x00051227 File Offset: 0x0004F427
	protected override void OnDestroy()
	{
		this._deploySignal.Dispose();
		base.OnDestroy();
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0005123C File Offset: 0x0004F43C
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this._objectToDeploy.activeSelf)
			{
				this.DeployChild();
				return;
			}
		}
		else if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x00051290 File Offset: 0x0004F490
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRig.LocalRig != this.ownerRig)
		{
			return false;
		}
		bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand);
		Transform transform = base.transform;
		Vector3 vector = transform.TransformPoint(Vector3.zero);
		Quaternion rotation = transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		this.DeployLocal(vector, rotation, averageVelocity, false);
		this._deploySignal.Raise(0, BitPackUtils.PackWorldPosForNetwork(vector), BitPackUtils.PackQuaternionForNetwork(rotation), BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f));
		return true;
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x00051333 File Offset: 0x0004F533
	protected virtual void DeployLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this.DisableWhileDeployed(true);
		this._child.Deploy(this, launchPos, launchRot, releaseVel, isRemote);
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x00051350 File Offset: 0x0004F550
	private void DeployRPC(long packedPos, int packedRot, long packedVel, PhotonSignalInfo info)
	{
		if (info.sender != base.OwningPlayer())
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "DeployRPC");
		if (!this.m_spamChecker.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos);
		Quaternion launchRot = BitPackUtils.UnpackQuaternionFromNetwork(packedRot);
		Vector3 inVel = BitPackUtils.UnpackWorldPosFromNetwork(packedVel) / 100f;
		float num = 10000f;
		if (!vector.IsValid(num) || !launchRot.IsValid() || !this.m_VRRig.IsPositionInRange(vector, this._maxDeployDistance))
		{
			return;
		}
		this.DeployLocal(vector, launchRot, this.m_VRRig.ClampVelocityRelativeToPlayerSafe(inVel, this._maxThrowVelocity, 100f), true);
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x00051400 File Offset: 0x0004F600
	private void DisableWhileDeployed(bool active)
	{
		if (this._disabledWhileDeployed.IsNullOrEmpty<GameObject>())
		{
			return;
		}
		for (int i = 0; i < this._disabledWhileDeployed.Length; i++)
		{
			this._disabledWhileDeployed[i].SetActive(!active);
		}
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0005143F File Offset: 0x0004F63F
	public void DeployChild()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this._objectToDeploy.SetActive(true);
		this.DisableWhileDeployed(true);
		UnityEvent onDeploy = this._onDeploy;
		if (onDeploy == null)
		{
			return;
		}
		onDeploy.Invoke();
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x00051472 File Offset: 0x0004F672
	public void ReturnChild()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this._objectToDeploy.SetActive(false);
		this.DisableWhileDeployed(false);
		UnityEvent onReturn = this._onReturn;
		if (onReturn == null)
		{
			return;
		}
		onReturn.Invoke();
	}

	// Token: 0x040012C8 RID: 4808
	[SerializeField]
	private GameObject _objectToDeploy;

	// Token: 0x040012C9 RID: 4809
	[SerializeField]
	private DeployedChild _child;

	// Token: 0x040012CA RID: 4810
	[SerializeField]
	private GameObject[] _disabledWhileDeployed = new GameObject[0];

	// Token: 0x040012CB RID: 4811
	[SerializeField]
	private SoundBankPlayer deploySound;

	// Token: 0x040012CC RID: 4812
	[SerializeField]
	private PhotonSignal<long, int, long> _deploySignal = "_deploySignal";

	// Token: 0x040012CD RID: 4813
	[SerializeField]
	private float _maxDeployDistance = 4f;

	// Token: 0x040012CE RID: 4814
	[SerializeField]
	private float _maxThrowVelocity = 50f;

	// Token: 0x040012CF RID: 4815
	[SerializeField]
	private UnityEvent _onDeploy;

	// Token: 0x040012D0 RID: 4816
	[SerializeField]
	private UnityEvent _onReturn;

	// Token: 0x040012D1 RID: 4817
	[SerializeField]
	private Component[] _rigAwareObjects = new Component[0];

	// Token: 0x040012D2 RID: 4818
	[SerializeField]
	private CallLimiter m_spamChecker = new CallLimiter(2, 1f, 0.5f);

	// Token: 0x040012D3 RID: 4819
	private VRRig m_VRRig;
}
