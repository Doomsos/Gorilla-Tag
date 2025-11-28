using System;
using UnityEngine;

// Token: 0x02000C13 RID: 3091
public class FPSController : MonoBehaviour
{
	// Token: 0x14000085 RID: 133
	// (add) Token: 0x06004C30 RID: 19504 RVA: 0x0018CDB8 File Offset: 0x0018AFB8
	// (remove) Token: 0x06004C31 RID: 19505 RVA: 0x0018CDF0 File Offset: 0x0018AFF0
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent;

	// Token: 0x14000086 RID: 134
	// (add) Token: 0x06004C32 RID: 19506 RVA: 0x0018CE28 File Offset: 0x0018B028
	// (remove) Token: 0x06004C33 RID: 19507 RVA: 0x0018CE60 File Offset: 0x0018B060
	public event FPSController.OnStateChangeEventHandler OnStopEvent;

	// Token: 0x04005C1A RID: 23578
	public float baseMoveSpeed = 4f;

	// Token: 0x04005C1B RID: 23579
	public float shiftMoveSpeed = 8f;

	// Token: 0x04005C1C RID: 23580
	public float ctrlMoveSpeed = 1f;

	// Token: 0x04005C1D RID: 23581
	public float lookHorizontal = 0.4f;

	// Token: 0x04005C1E RID: 23582
	public float lookVertical = 0.25f;

	// Token: 0x04005C1F RID: 23583
	[SerializeField]
	private Vector3 leftControllerPosOffset = new Vector3(-0.2f, -0.25f, 0.3f);

	// Token: 0x04005C20 RID: 23584
	[SerializeField]
	private Vector3 leftControllerRotationOffset = new Vector3(265f, -82f, 28f);

	// Token: 0x04005C21 RID: 23585
	[SerializeField]
	private Vector3 rightControllerPosOffset = new Vector3(0.2f, -0.25f, 0.3f);

	// Token: 0x04005C22 RID: 23586
	[SerializeField]
	private Vector3 rightControllerRotationOffset = new Vector3(263f, 318f, 485f);

	// Token: 0x04005C23 RID: 23587
	[SerializeField]
	private bool toggleGrab;

	// Token: 0x04005C24 RID: 23588
	[SerializeField]
	private bool clampGrab;

	// Token: 0x04005C27 RID: 23591
	private bool controlRightHand;

	// Token: 0x04005C28 RID: 23592
	public LayerMask HandMask;

	// Token: 0x02000C14 RID: 3092
	// (Invoke) Token: 0x06004C36 RID: 19510
	public delegate void OnStateChangeEventHandler();
}
