using System;
using GorillaExtensions;
using GorillaLocomotion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000810 RID: 2064
public class HoverboardVisual : MonoBehaviour, ICallBack
{
	// Token: 0x170004D2 RID: 1234
	// (get) Token: 0x0600364B RID: 13899 RVA: 0x001266F6 File Offset: 0x001248F6
	// (set) Token: 0x0600364C RID: 13900 RVA: 0x001266FE File Offset: 0x001248FE
	public Color boardColor { get; private set; }

	// Token: 0x0600364D RID: 13901 RVA: 0x00126708 File Offset: 0x00124908
	private void Awake()
	{
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x170004D3 RID: 1235
	// (get) Token: 0x0600364E RID: 13902 RVA: 0x00126744 File Offset: 0x00124944
	// (set) Token: 0x0600364F RID: 13903 RVA: 0x0012674C File Offset: 0x0012494C
	public bool IsHeld { get; private set; }

	// Token: 0x170004D4 RID: 1236
	// (get) Token: 0x06003650 RID: 13904 RVA: 0x00126755 File Offset: 0x00124955
	// (set) Token: 0x06003651 RID: 13905 RVA: 0x0012675D File Offset: 0x0012495D
	public bool IsLeftHanded { get; private set; }

	// Token: 0x170004D5 RID: 1237
	// (get) Token: 0x06003652 RID: 13906 RVA: 0x00126766 File Offset: 0x00124966
	// (set) Token: 0x06003653 RID: 13907 RVA: 0x0012676E File Offset: 0x0012496E
	public Vector3 NominalLocalPosition { get; private set; }

	// Token: 0x170004D6 RID: 1238
	// (get) Token: 0x06003654 RID: 13908 RVA: 0x00126777 File Offset: 0x00124977
	// (set) Token: 0x06003655 RID: 13909 RVA: 0x0012677F File Offset: 0x0012497F
	public Quaternion NominalLocalRotation { get; private set; }

	// Token: 0x170004D7 RID: 1239
	// (get) Token: 0x06003656 RID: 13910 RVA: 0x00126788 File Offset: 0x00124988
	private Transform NominalParentTransform
	{
		get
		{
			if (!this.IsHeld)
			{
				return base.transform.parent;
			}
			return (this.IsLeftHanded ? this.parentRig.leftHand : this.parentRig.rightHand).rigTarget.transform;
		}
	}

	// Token: 0x06003657 RID: 13911 RVA: 0x001267C8 File Offset: 0x001249C8
	public void SetIsHeld(bool isHeldLeftHanded, Vector3 localPosition, Quaternion localRotation, Color boardColor)
	{
		if (!this.isCallbackActive)
		{
			this.parentRig.AddLateUpdateCallback(this);
			this.isCallbackActive = true;
		}
		this.IsHeld = true;
		base.gameObject.SetActive(true);
		this.IsLeftHanded = isHeldLeftHanded;
		this.NominalLocalPosition = localPosition;
		this.NominalLocalRotation = localRotation;
		Transform nominalParentTransform = this.NominalParentTransform;
		this.interpolatedLocalPosition = nominalParentTransform.InverseTransformPoint(base.transform.position);
		this.interpolatedLocalRotation = nominalParentTransform.InverseTransformRotation(base.transform.rotation);
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(ref num, ref vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(true);
		}
		this.colorMaterial.color = boardColor;
		this.boardColor = boardColor;
	}

	// Token: 0x06003658 RID: 13912 RVA: 0x001268D1 File Offset: 0x00124AD1
	public void SetNotHeld(bool isLeftHanded)
	{
		this.IsLeftHanded = isLeftHanded;
		this.SetNotHeld();
	}

	// Token: 0x06003659 RID: 13913 RVA: 0x001268E0 File Offset: 0x00124AE0
	public void SetNotHeld()
	{
		bool isHeld = this.IsHeld;
		base.gameObject.SetActive(false);
		this.IsHeld = false;
		this.interpolatedLocalPosition = base.transform.localPosition;
		this.interpolatedLocalRotation = base.transform.localRotation;
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(ref num, ref vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (!isHeld)
		{
			base.transform.position = base.transform.parent.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = base.transform.parent.TransformRotation(this.NominalLocalRotation);
		}
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(false);
		}
		this.hoverboardAudio.Stop();
	}

	// Token: 0x0600365A RID: 13914 RVA: 0x001269E8 File Offset: 0x00124BE8
	void ICallBack.CallBack()
	{
		Transform nominalParentTransform = this.NominalParentTransform;
		if ((this.interpolatedLocalPosition - this.NominalLocalPosition).IsShorterThan(0.01f))
		{
			base.transform.position = nominalParentTransform.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.NominalLocalRotation);
			if (!this.IsHeld)
			{
				this.parentRig.RemoveLateUpdateCallback(this);
				this.isCallbackActive = false;
			}
		}
		else
		{
			this.interpolatedLocalPosition = Vector3.MoveTowards(this.interpolatedLocalPosition, this.NominalLocalPosition, this.positionLerpSpeed * Time.deltaTime);
			this.interpolatedLocalRotation = Quaternion.RotateTowards(this.interpolatedLocalRotation, this.NominalLocalRotation, this.rotationLerpSpeed * Time.deltaTime);
			base.transform.position = nominalParentTransform.TransformPoint(this.interpolatedLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.interpolatedLocalRotation);
		}
		if (this.IsHeld)
		{
			if (this.parentRig.isLocal)
			{
				GTPlayer.Instance.SetHoverboardPosRot(base.transform.position, base.transform.rotation);
				return;
			}
			this.hoverboardAudio.UpdateAudioLoop(this.parentRig.LatestVelocity().magnitude, 0f, 0f, 0f);
		}
	}

	// Token: 0x0600365B RID: 13915 RVA: 0x00126B3E File Offset: 0x00124D3E
	public void PlayGrindHaptic()
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, this.grindHapticStrength, this.grindHapticDuration);
		}
	}

	// Token: 0x0600365C RID: 13916 RVA: 0x00126B64 File Offset: 0x00124D64
	public void PlayCarveHaptic(float carveForce)
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, carveForce * this.carveHapticStrength, this.carveHapticDuration);
		}
	}

	// Token: 0x0600365D RID: 13917 RVA: 0x00126B8C File Offset: 0x00124D8C
	public void ProxyGrabHandle(bool isLeftHand)
	{
		EquipmentInteractor.instance.UpdateHandEquipment(this.handlePosition, isLeftHand);
	}

	// Token: 0x0600365E RID: 13918 RVA: 0x00126BA1 File Offset: 0x00124DA1
	public void DropFreeBoard()
	{
		FreeHoverboardManager.instance.SendDropBoardRPC(base.transform.position, base.transform.rotation, this.velocityEstimator.linearVelocity, this.velocityEstimator.angularVelocity, this.boardColor);
	}

	// Token: 0x0600365F RID: 13919 RVA: 0x00126BDF File Offset: 0x00124DDF
	public void SetRaceDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.racePositionReadout.gameObject.SetActive(false);
			return;
		}
		this.racePositionReadout.gameObject.SetActive(true);
		this.racePositionReadout.text = text;
	}

	// Token: 0x06003660 RID: 13920 RVA: 0x00126C18 File Offset: 0x00124E18
	public void SetRaceLapsDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.raceLapsReadout.gameObject.SetActive(false);
			return;
		}
		this.raceLapsReadout.gameObject.SetActive(true);
		this.raceLapsReadout.text = text;
	}

	// Token: 0x040045AD RID: 17837
	[SerializeField]
	private VRRig parentRig;

	// Token: 0x040045AE RID: 17838
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040045AF RID: 17839
	[SerializeField]
	[FormerlySerializedAs("audio")]
	private HoverboardAudio hoverboardAudio;

	// Token: 0x040045B0 RID: 17840
	[SerializeField]
	private HoverboardHandle handlePosition;

	// Token: 0x040045B1 RID: 17841
	[SerializeField]
	private float grindHapticStrength;

	// Token: 0x040045B2 RID: 17842
	[SerializeField]
	private float grindHapticDuration;

	// Token: 0x040045B3 RID: 17843
	[SerializeField]
	private float carveHapticStrength;

	// Token: 0x040045B4 RID: 17844
	[SerializeField]
	private float carveHapticDuration;

	// Token: 0x040045B5 RID: 17845
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x040045B6 RID: 17846
	[SerializeField]
	private InteractionPoint handleInteractionPoint;

	// Token: 0x040045B7 RID: 17847
	[SerializeField]
	private TextMeshPro racePositionReadout;

	// Token: 0x040045B8 RID: 17848
	[SerializeField]
	private TextMeshPro raceLapsReadout;

	// Token: 0x040045B9 RID: 17849
	private Material colorMaterial;

	// Token: 0x040045BF RID: 17855
	private Vector3 interpolatedLocalPosition;

	// Token: 0x040045C0 RID: 17856
	private Quaternion interpolatedLocalRotation;

	// Token: 0x040045C1 RID: 17857
	[SerializeField]
	private float lerpIntoHandDuration;

	// Token: 0x040045C2 RID: 17858
	private float positionLerpSpeed;

	// Token: 0x040045C3 RID: 17859
	private float rotationLerpSpeed;

	// Token: 0x040045C4 RID: 17860
	private bool isCallbackActive;
}
