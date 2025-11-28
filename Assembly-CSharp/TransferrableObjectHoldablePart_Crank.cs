using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000432 RID: 1074
public class TransferrableObjectHoldablePart_Crank : TransferrableObjectHoldablePart
{
	// Token: 0x06001A5C RID: 6748 RVA: 0x0008C22F File Offset: 0x0008A42F
	public void SetOnCrankedCallback(Action<float> onCrankedCallback)
	{
		this.onCrankedCallback = onCrankedCallback;
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x0008C238 File Offset: 0x0008A438
	private void Awake()
	{
		if (this.rotatingPart == null)
		{
			this.rotatingPart = base.transform;
		}
		Vector3 vector = this.rotatingPart.parent.InverseTransformPoint(this.rotatingPart.TransformPoint(Vector3.right));
		this.lastAngle = Mathf.Atan2(vector.y, vector.x);
		this.baseLocalAngle = this.rotatingPart.localRotation;
		this.baseLocalAngleInverse = Quaternion.Inverse(this.baseLocalAngle);
		this.crankRadius = new Vector2(this.crankHandleX, this.crankHandleY).magnitude;
		this.crankAngleOffset = Mathf.Atan2(this.crankHandleY, this.crankHandleX) * 57.29578f;
		if (this.crankHandleMaxZ < this.crankHandleMinZ)
		{
			float num = this.crankHandleMaxZ;
			float num2 = this.crankHandleMinZ;
			this.crankHandleMinZ = num;
			this.crankHandleMaxZ = num2;
		}
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x0008C320 File Offset: 0x0008A520
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		Vector3 vector4;
		if (rig.isOfflineVRRig)
		{
			Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(isHeldLeftHand);
			Vector3 vector = this.rotatingPart.InverseTransformPoint(controllerTransform.position);
			Vector3 vector2 = (vector.xy().normalized * this.crankRadius).WithZ(Mathf.Clamp(vector.z, this.crankHandleMinZ, this.crankHandleMaxZ));
			Vector3 vector3 = this.rotatingPart.TransformPoint(vector2);
			if (this.maxHandSnapDistance > 0f && (controllerTransform.position - vector3).IsLongerThan(this.maxHandSnapDistance))
			{
				this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
				return;
			}
			controllerTransform.position = vector3;
			vector4 = controllerTransform.position;
		}
		else
		{
			VRMap vrmap = isHeldLeftHand ? rig.leftHand : rig.rightHand;
			vector4 = vrmap.GetExtrapolatedControllerPosition();
			vector4 -= vrmap.rigTarget.rotation * GTPlayer.Instance.GetHandOffset(isHeldLeftHand) * rig.scaleFactor;
		}
		Vector3 vector5 = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (vector4 - this.rotatingPart.position);
		float num = Mathf.Atan2(vector5.y, vector5.x) * 57.29578f;
		float num2 = Mathf.DeltaAngle(this.lastAngle, num);
		this.lastAngle = num;
		if (num2 != 0f)
		{
			if (this.onCrankedCallback != null)
			{
				this.onCrankedCallback.Invoke(num2);
			}
			for (int i = 0; i < this.thresholds.Length; i++)
			{
				this.thresholds[i].OnCranked(num2);
			}
		}
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(num - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x0008C520 File Offset: 0x0008A720
	private void OnDrawGizmosSelected()
	{
		Transform transform = (this.rotatingPart != null) ? this.rotatingPart : base.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x040023E3 RID: 9187
	[SerializeField]
	private float crankHandleX;

	// Token: 0x040023E4 RID: 9188
	[SerializeField]
	private float crankHandleY;

	// Token: 0x040023E5 RID: 9189
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x040023E6 RID: 9190
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x040023E7 RID: 9191
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x040023E8 RID: 9192
	private float crankAngleOffset;

	// Token: 0x040023E9 RID: 9193
	private float crankRadius;

	// Token: 0x040023EA RID: 9194
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x040023EB RID: 9195
	private float lastAngle;

	// Token: 0x040023EC RID: 9196
	private Quaternion baseLocalAngle;

	// Token: 0x040023ED RID: 9197
	private Quaternion baseLocalAngleInverse;

	// Token: 0x040023EE RID: 9198
	private Action<float> onCrankedCallback;

	// Token: 0x040023EF RID: 9199
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank.CrankThreshold[] thresholds;

	// Token: 0x02000433 RID: 1075
	[Serializable]
	private struct CrankThreshold
	{
		// Token: 0x06001A61 RID: 6753 RVA: 0x0008C59B File Offset: 0x0008A79B
		public void OnCranked(float deltaAngle)
		{
			this.currentAngle += deltaAngle;
			if (Mathf.Abs(this.currentAngle) > this.angleThreshold)
			{
				this.currentAngle = 0f;
				this.onReached.Invoke();
			}
		}

		// Token: 0x040023F0 RID: 9200
		public float angleThreshold;

		// Token: 0x040023F1 RID: 9201
		public UnityEvent onReached;

		// Token: 0x040023F2 RID: 9202
		[HideInInspector]
		public float currentAngle;
	}
}
