using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class CrankableToyCarHoldable : TransferrableObject
{
	// Token: 0x06000F2A RID: 3882 RVA: 0x0005084A File Offset: 0x0004EA4A
	protected override void Start()
	{
		base.Start();
		this.crank.SetOnCrankedCallback(new Action<float>(this.OnCranked));
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0005086C File Offset: 0x0004EA6C
	internal override void OnEnable()
	{
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
		}
		NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
		if (netPlayer != null && this._events != null)
		{
			this._events.Init(netPlayer);
			this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnDeployRPC);
		}
		else
		{
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x00050957 File Offset: 0x0004EB57
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Dispose();
		}
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x00050978 File Offset: 0x0004EB78
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.deployablePart.activeSelf)
			{
				this.OnCarDeployed();
				return;
			}
		}
		else if (this.deployablePart.activeSelf)
		{
			this.OnCarReturned();
		}
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x000509CC File Offset: 0x0004EBCC
	private void OnCranked(float deltaAngle)
	{
		this.currentCrankStrength += Mathf.Abs(deltaAngle);
		this.currentCrankClickAmount += deltaAngle;
		if (Mathf.Abs(this.currentCrankClickAmount) > this.crankAnglePerClick)
		{
			if (this.currentCrankStrength >= this.maxCrankStrength)
			{
				this.overCrankedSound.Play();
				VRRig ownerRig = this.ownerRig;
				if (ownerRig != null && ownerRig.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.overcrankHapticStrength, this.overcrankHapticDuration);
				}
			}
			else
			{
				float num = Mathf.Lerp(this.minClickPitch, this.maxClickPitch, Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength));
				SoundBankPlayer soundBankPlayer = this.clickSound;
				float? pitchOverride = new float?(num);
				soundBankPlayer.Play(default(float?), pitchOverride);
				VRRig ownerRig2 = this.ownerRig;
				if (ownerRig2 != null && ownerRig2.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.crankHapticStrength, this.crankHapticDuration);
				}
			}
			this.currentCrankClickAmount = 0f;
		}
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x00050AE0 File Offset: 0x0004ECE0
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
		{
			return false;
		}
		if (this.currentCrankStrength == 0f)
		{
			return true;
		}
		bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand);
		Vector3 vector = base.transform.TransformPoint(Vector3.zero);
		Quaternion rotation = base.transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		float num = Mathf.Lerp(this.minLifetime, this.maxLifetime, Mathf.Clamp01(Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength)));
		this.DeployCarLocal(vector, rotation, averageVelocity, num, false);
		if (PhotonNetwork.InRoom)
		{
			this._events.Activate.RaiseOthers(new object[]
			{
				BitPackUtils.PackWorldPosForNetwork(vector),
				BitPackUtils.PackQuaternionForNetwork(rotation),
				BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f),
				num
			});
		}
		this.currentCrankStrength = 0f;
		return true;
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x00050C0F File Offset: 0x0004EE0F
	private void DeployCarLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		if (!this.disabledWhileDeployed.activeSelf)
		{
			return;
		}
		this.deployedCar.Deploy(this, launchPos, launchRot, releaseVel, lifetime, isRemote);
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x00050C34 File Offset: 0x0004EE34
	private void OnDeployRPC(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (!this || sender != receiver || info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnDeployRPC");
		Vector3 launchPos = BitPackUtils.UnpackWorldPosFromNetwork((long)args[0]);
		Quaternion launchRot = BitPackUtils.UnpackQuaternionFromNetwork((int)args[1]);
		Vector3 releaseVel = BitPackUtils.UnpackWorldPosFromNetwork((long)args[2]) / 100f;
		float lifetime = (float)args[3];
		float num = 10000f;
		if (launchPos.IsValid(num) && launchRot.IsValid())
		{
			float num2 = 10000f;
			if (releaseVel.IsValid(num2))
			{
				this.DeployCarLocal(launchPos, launchRot, releaseVel, lifetime, true);
				return;
			}
		}
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x00050CE9 File Offset: 0x0004EEE9
	public void OnCarDeployed()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this.deployablePart.SetActive(true);
		this.disabledWhileDeployed.SetActive(false);
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x00050D11 File Offset: 0x0004EF11
	public void OnCarReturned()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this.deployablePart.SetActive(false);
		this.disabledWhileDeployed.SetActive(true);
		this.clickSound.RestartSequence();
	}

	// Token: 0x040012A4 RID: 4772
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank crank;

	// Token: 0x040012A5 RID: 4773
	[SerializeField]
	private CrankableToyCarDeployed deployedCar;

	// Token: 0x040012A6 RID: 4774
	[SerializeField]
	private GameObject deployablePart;

	// Token: 0x040012A7 RID: 4775
	[SerializeField]
	private GameObject disabledWhileDeployed;

	// Token: 0x040012A8 RID: 4776
	[SerializeField]
	private float crankAnglePerClick;

	// Token: 0x040012A9 RID: 4777
	[SerializeField]
	private float maxCrankStrength;

	// Token: 0x040012AA RID: 4778
	[SerializeField]
	private float minClickPitch;

	// Token: 0x040012AB RID: 4779
	[SerializeField]
	private float maxClickPitch;

	// Token: 0x040012AC RID: 4780
	[SerializeField]
	private float minLifetime;

	// Token: 0x040012AD RID: 4781
	[SerializeField]
	private float maxLifetime;

	// Token: 0x040012AE RID: 4782
	[SerializeField]
	private SoundBankPlayer clickSound;

	// Token: 0x040012AF RID: 4783
	[SerializeField]
	private SoundBankPlayer overCrankedSound;

	// Token: 0x040012B0 RID: 4784
	[SerializeField]
	private float crankHapticStrength = 0.1f;

	// Token: 0x040012B1 RID: 4785
	[SerializeField]
	private float crankHapticDuration = 0.05f;

	// Token: 0x040012B2 RID: 4786
	[SerializeField]
	private float overcrankHapticStrength = 0.8f;

	// Token: 0x040012B3 RID: 4787
	[SerializeField]
	private float overcrankHapticDuration = 0.05f;

	// Token: 0x040012B4 RID: 4788
	private float currentCrankStrength;

	// Token: 0x040012B5 RID: 4789
	private float currentCrankClickAmount;

	// Token: 0x040012B6 RID: 4790
	private RubberDuckEvents _events;
}
