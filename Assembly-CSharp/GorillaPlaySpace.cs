using System;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public class GorillaPlaySpace : MonoBehaviour
{
	// Token: 0x1700038C RID: 908
	// (get) Token: 0x0600215B RID: 8539 RVA: 0x000AF4E8 File Offset: 0x000AD6E8
	public static GorillaPlaySpace Instance
	{
		get
		{
			return GorillaPlaySpace._instance;
		}
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000AF4EF File Offset: 0x000AD6EF
	private void Awake()
	{
		if (GorillaPlaySpace._instance != null && GorillaPlaySpace._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaPlaySpace._instance = this;
	}

	// Token: 0x04002BE8 RID: 11240
	[OnEnterPlay_SetNull]
	private static GorillaPlaySpace _instance;

	// Token: 0x04002BE9 RID: 11241
	public Collider headCollider;

	// Token: 0x04002BEA RID: 11242
	public Collider bodyCollider;

	// Token: 0x04002BEB RID: 11243
	public Transform rightHandTransform;

	// Token: 0x04002BEC RID: 11244
	public Transform leftHandTransform;

	// Token: 0x04002BED RID: 11245
	public Vector3 headColliderOffset;

	// Token: 0x04002BEE RID: 11246
	public Vector3 bodyColliderOffset;

	// Token: 0x04002BEF RID: 11247
	private Vector3 lastLeftHandPosition;

	// Token: 0x04002BF0 RID: 11248
	private Vector3 lastRightHandPosition;

	// Token: 0x04002BF1 RID: 11249
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x04002BF2 RID: 11250
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x04002BF3 RID: 11251
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04002BF4 RID: 11252
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04002BF5 RID: 11253
	private Rigidbody playspaceRigidbody;

	// Token: 0x04002BF6 RID: 11254
	public Transform headsetTransform;

	// Token: 0x04002BF7 RID: 11255
	public Vector3 rightHandOffset;

	// Token: 0x04002BF8 RID: 11256
	public Vector3 leftHandOffset;

	// Token: 0x04002BF9 RID: 11257
	public VRRig vrRig;

	// Token: 0x04002BFA RID: 11258
	public VRRig offlineVRRig;

	// Token: 0x04002BFB RID: 11259
	public float vibrationCooldown = 0.1f;

	// Token: 0x04002BFC RID: 11260
	public float vibrationDuration = 0.05f;

	// Token: 0x04002BFD RID: 11261
	private float leftLastTouchedSurface;

	// Token: 0x04002BFE RID: 11262
	private float rightLastTouchedSurface;

	// Token: 0x04002BFF RID: 11263
	public VRRig myVRRig;

	// Token: 0x04002C00 RID: 11264
	private float bodyHeight;

	// Token: 0x04002C01 RID: 11265
	public float tagCooldown;

	// Token: 0x04002C02 RID: 11266
	public float taggedTime;

	// Token: 0x04002C03 RID: 11267
	public float disconnectTime = 60f;

	// Token: 0x04002C04 RID: 11268
	public float maxStepVelocity = 2f;

	// Token: 0x04002C05 RID: 11269
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x04002C06 RID: 11270
	public float tapHapticDuration = 0.05f;

	// Token: 0x04002C07 RID: 11271
	public float tapHapticStrength = 0.5f;

	// Token: 0x04002C08 RID: 11272
	public float tagHapticDuration = 0.15f;

	// Token: 0x04002C09 RID: 11273
	public float tagHapticStrength = 1f;

	// Token: 0x04002C0A RID: 11274
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04002C0B RID: 11275
	public float taggedHapticStrength = 1f;
}
