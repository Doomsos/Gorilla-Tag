using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class GorillaVelocityEstimator : MonoBehaviour
{
	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000ACC RID: 2764 RVA: 0x0003A959 File Offset: 0x00038B59
	// (set) Token: 0x06000ACD RID: 2765 RVA: 0x0003A961 File Offset: 0x00038B61
	public Vector3 linearVelocity { get; private set; }

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000ACE RID: 2766 RVA: 0x0003A96A File Offset: 0x00038B6A
	// (set) Token: 0x06000ACF RID: 2767 RVA: 0x0003A972 File Offset: 0x00038B72
	public Vector3 angularVelocity { get; private set; }

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x0003A97B File Offset: 0x00038B7B
	// (set) Token: 0x06000AD1 RID: 2769 RVA: 0x0003A983 File Offset: 0x00038B83
	public Vector3 handPos { get; private set; }

	// Token: 0x06000AD2 RID: 2770 RVA: 0x0003A98C File Offset: 0x00038B8C
	private void Awake()
	{
		this.history = new GorillaVelocityEstimator.VelocityHistorySample[this.numFrames];
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0003A9A0 File Offset: 0x00038BA0
	private void OnEnable()
	{
		this.currentFrame = 0;
		for (int i = 0; i < this.history.Length; i++)
		{
			this.history[i] = default(GorillaVelocityEstimator.VelocityHistorySample);
		}
		this.lastPos = base.transform.position;
		this.lastRotation = base.transform.rotation;
		GorillaVelocityEstimatorManager.Register(this);
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x0003AA01 File Offset: 0x00038C01
	private void OnDisable()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0003AA01 File Offset: 0x00038C01
	private void OnDestroy()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0003AA0C File Offset: 0x00038C0C
	public void TriggeredLateUpdate()
	{
		Vector3 vector;
		Quaternion quaternion;
		base.transform.GetPositionAndRotation(ref vector, ref quaternion);
		Vector3 vector2 = Vector3.zero;
		if (!this.useGlobalSpace)
		{
			vector2 = GTPlayer.Instance.InstantaneousVelocity;
		}
		Vector3 vector3 = (vector - this.lastPos) / Time.deltaTime - vector2;
		Vector3 vector4 = (quaternion * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector4.x > 180f)
		{
			vector4.x -= 360f;
		}
		if (vector4.y > 180f)
		{
			vector4.y -= 360f;
		}
		if (vector4.z > 180f)
		{
			vector4.z -= 360f;
		}
		vector4 *= 0.017453292f / Time.fixedDeltaTime;
		this.linearVelocity += (vector3 - this.history[this.currentFrame].linear) / (float)this.numFrames;
		this.angularVelocity += (vector4 - this.history[this.currentFrame].angular) / (float)this.numFrames;
		this.history[this.currentFrame] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = vector3,
			angular = vector4
		};
		this.handPos = vector;
		this.currentFrame = (this.currentFrame + 1) % this.numFrames;
		this.lastPos = vector;
		this.lastRotation = quaternion;
	}

	// Token: 0x04000D40 RID: 3392
	[Min(1f)]
	[SerializeField]
	private int numFrames = 8;

	// Token: 0x04000D44 RID: 3396
	private GorillaVelocityEstimator.VelocityHistorySample[] history;

	// Token: 0x04000D45 RID: 3397
	private int currentFrame;

	// Token: 0x04000D46 RID: 3398
	private Vector3 lastPos;

	// Token: 0x04000D47 RID: 3399
	private Quaternion lastRotation;

	// Token: 0x04000D48 RID: 3400
	private Vector3 lastRotationVec;

	// Token: 0x04000D49 RID: 3401
	public bool useGlobalSpace;

	// Token: 0x02000195 RID: 405
	public struct VelocityHistorySample
	{
		// Token: 0x04000D4A RID: 3402
		public Vector3 linear;

		// Token: 0x04000D4B RID: 3403
		public Vector3 angular;
	}
}
