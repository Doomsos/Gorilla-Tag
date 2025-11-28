using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000CD4 RID: 3284
public class TransformOscillation : MonoBehaviour
{
	// Token: 0x0600501E RID: 20510 RVA: 0x0019BAA2 File Offset: 0x00199CA2
	private void Awake()
	{
		if (this.useRigidbodyMotion && !this.targetRigidbody)
		{
			this.targetRigidbody = base.GetComponent<Rigidbody>();
		}
		this.lastRotOffs = Quaternion.identity;
		this.startTime = Time.time;
		this.isRunning = false;
	}

	// Token: 0x0600501F RID: 20511 RVA: 0x0019BAE2 File Offset: 0x00199CE2
	private void OnEnable()
	{
		this.lastPosOffs = Vector3.zero;
		this.lastRotOffs = Quaternion.identity;
		if (this.startOnEnable)
		{
			this.StartOscillation();
			return;
		}
		this.isRunning = false;
	}

	// Token: 0x06005020 RID: 20512 RVA: 0x0019BB10 File Offset: 0x00199D10
	public void StartOscillation()
	{
		this.startTime = Time.time;
		this.isRunning = true;
	}

	// Token: 0x06005021 RID: 20513 RVA: 0x0019BB24 File Offset: 0x00199D24
	private float GetTimeSeconds()
	{
		if (!this.useServerTime)
		{
			return Time.timeSinceLevelLoad;
		}
		if (GorillaComputer.instance == null)
		{
			return Time.timeSinceLevelLoad;
		}
		this.dt = GorillaComputer.instance.GetServerTime();
		return (float)this.dt.Minute * 60f + (float)this.dt.Second + (float)this.dt.Millisecond / 1000f;
	}

	// Token: 0x06005022 RID: 20514 RVA: 0x0019BB98 File Offset: 0x00199D98
	private void ComputeOffsets(float t)
	{
		this.offsPos.x = this.PosAmp.x * Mathf.Sin(t * this.PosFreq.x);
		this.offsPos.y = this.PosAmp.y * Mathf.Sin(t * this.PosFreq.y);
		this.offsPos.z = this.PosAmp.z * Mathf.Sin(t * this.PosFreq.z);
		this.offsRot.x = this.RotAmp.x * Mathf.Sin(t * this.RotFreq.x);
		this.offsRot.y = this.RotAmp.y * Mathf.Sin(t * this.RotFreq.y);
		this.offsRot.z = this.RotAmp.z * Mathf.Sin(t * this.RotFreq.z);
	}

	// Token: 0x06005023 RID: 20515 RVA: 0x0019BC9C File Offset: 0x00199E9C
	private void LateUpdate()
	{
		if (!this.isRunning)
		{
			return;
		}
		if (this.useTimeLimit && Time.time - this.startTime >= this.timer)
		{
			return;
		}
		if (this.useRigidbodyMotion && this.targetRigidbody)
		{
			return;
		}
		float timeSeconds = this.GetTimeSeconds();
		this.ComputeOffsets(timeSeconds);
		Transform transform = base.transform;
		Quaternion quaternion = Quaternion.Euler(this.offsRot);
		Vector3 vector = transform.localPosition - this.lastPosOffs;
		Quaternion quaternion2 = transform.localRotation * Quaternion.Inverse(this.lastRotOffs);
		transform.localPosition = vector + this.offsPos;
		transform.localRotation = quaternion2 * quaternion;
		this.lastPosOffs = this.offsPos;
		this.lastRotOffs = quaternion;
	}

	// Token: 0x06005024 RID: 20516 RVA: 0x0019BD60 File Offset: 0x00199F60
	private void FixedUpdate()
	{
		if (!this.isRunning)
		{
			return;
		}
		if (this.useTimeLimit && Time.time - this.startTime >= this.timer)
		{
			return;
		}
		if (!this.useRigidbodyMotion || !this.targetRigidbody)
		{
			return;
		}
		float timeSeconds = this.GetTimeSeconds();
		this.ComputeOffsets(timeSeconds);
		Transform transform = base.transform;
		Quaternion quaternion = Quaternion.Euler(this.offsRot);
		Transform parent = transform.parent;
		Vector3 vector = parent ? parent.TransformVector(this.lastPosOffs) : this.lastPosOffs;
		Quaternion quaternion2 = parent ? (parent.rotation * this.lastRotOffs * Quaternion.Inverse(parent.rotation)) : this.lastRotOffs;
		Vector3 vector2 = transform.position - vector;
		Quaternion quaternion3 = transform.rotation * Quaternion.Inverse(quaternion2);
		Vector3 vector3 = parent ? parent.TransformVector(this.offsPos) : this.offsPos;
		Quaternion quaternion4 = parent ? (parent.rotation * quaternion * Quaternion.Inverse(parent.rotation)) : quaternion;
		this.targetRigidbody.MovePosition(vector2 + vector3);
		this.targetRigidbody.MoveRotation(quaternion3 * quaternion4);
		this.lastPosOffs = this.offsPos;
		this.lastRotOffs = quaternion;
	}

	// Token: 0x04005EAA RID: 24234
	[SerializeField]
	private Vector3 PosAmp;

	// Token: 0x04005EAB RID: 24235
	[SerializeField]
	private Vector3 PosFreq;

	// Token: 0x04005EAC RID: 24236
	[SerializeField]
	private Vector3 RotAmp;

	// Token: 0x04005EAD RID: 24237
	[SerializeField]
	private Vector3 RotFreq;

	// Token: 0x04005EAE RID: 24238
	[SerializeField]
	private bool useServerTime;

	// Token: 0x04005EAF RID: 24239
	[Header("Rigidbody Motion (optional)")]
	[Tooltip("If true and a Rigidbody is present, applies motion using Rigidbody.MovePosition/MoveRotation in FixedUpdate.")]
	[SerializeField]
	private bool useRigidbodyMotion;

	// Token: 0x04005EB0 RID: 24240
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x04005EB1 RID: 24241
	[Header("Activation Timer (optional)")]
	[Tooltip("If true, oscillation only runs for 'activeDurationSeconds' after OnEnable; otherwise it runs indefinitely.")]
	[SerializeField]
	private bool useTimeLimit;

	// Token: 0x04005EB2 RID: 24242
	[SerializeField]
	private float timer = 2f;

	// Token: 0x04005EB3 RID: 24243
	[Header("Start Behavior (optional)")]
	[Tooltip("If true, oscillation starts automatically on OnEnable(). If false, call StartOscillation() manually.")]
	[SerializeField]
	private bool startOnEnable = true;

	// Token: 0x04005EB4 RID: 24244
	private Vector3 lastPosOffs = Vector3.zero;

	// Token: 0x04005EB5 RID: 24245
	private Quaternion lastRotOffs = Quaternion.identity;

	// Token: 0x04005EB6 RID: 24246
	private Vector3 offsPos;

	// Token: 0x04005EB7 RID: 24247
	private Vector3 offsRot;

	// Token: 0x04005EB8 RID: 24248
	private DateTime dt;

	// Token: 0x04005EB9 RID: 24249
	private float startTime;

	// Token: 0x04005EBA RID: 24250
	private bool isRunning;
}
