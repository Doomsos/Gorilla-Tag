using System;
using GorillaLocomotion;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x0200051B RID: 1307
public class GorillaCameraFollow : MonoBehaviour
{
	// Token: 0x06002148 RID: 8520 RVA: 0x000AF258 File Offset: 0x000AD458
	private void Start()
	{
		if (Application.platform == 11)
		{
			this.cameraParent.SetActive(false);
		}
		if (this.cinemachineCamera != null)
		{
			this.cinemachineFollow = this.cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
			this.baseCameraRadius = this.cinemachineFollow.CameraRadius;
			this.baseFollowDistance = this.cinemachineFollow.CameraDistance;
			this.baseVerticalArmLength = this.cinemachineFollow.VerticalArmLength;
			this.baseShoulderOffset = this.cinemachineFollow.ShoulderOffset;
		}
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000AF2E0 File Offset: 0x000AD4E0
	private void LateUpdate()
	{
		if (this.cinemachineFollow != null)
		{
			float scale = GTPlayer.Instance.scale;
			this.cinemachineFollow.CameraRadius = this.baseCameraRadius * scale;
			this.cinemachineFollow.CameraDistance = this.baseFollowDistance * scale;
			this.cinemachineFollow.VerticalArmLength = this.baseVerticalArmLength * scale;
			this.cinemachineFollow.ShoulderOffset = this.baseShoulderOffset * scale;
		}
	}

	// Token: 0x04002BD1 RID: 11217
	public Transform playerHead;

	// Token: 0x04002BD2 RID: 11218
	public GameObject cameraParent;

	// Token: 0x04002BD3 RID: 11219
	public Vector3 headOffset;

	// Token: 0x04002BD4 RID: 11220
	public Vector3 eulerRotationOffset;

	// Token: 0x04002BD5 RID: 11221
	public CinemachineVirtualCamera cinemachineCamera;

	// Token: 0x04002BD6 RID: 11222
	private Cinemachine3rdPersonFollow cinemachineFollow;

	// Token: 0x04002BD7 RID: 11223
	private float baseCameraRadius = 0.2f;

	// Token: 0x04002BD8 RID: 11224
	private float baseFollowDistance = 2f;

	// Token: 0x04002BD9 RID: 11225
	private float baseVerticalArmLength = 0.4f;

	// Token: 0x04002BDA RID: 11226
	private Vector3 baseShoulderOffset = new Vector3(0.5f, -0.4f, 0f);
}
