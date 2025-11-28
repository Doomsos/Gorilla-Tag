using System;
using UnityEngine;

// Token: 0x02000C16 RID: 3094
public class KinematicTestMotion : MonoBehaviour
{
	// Token: 0x06004C3A RID: 19514 RVA: 0x0018CF4A File Offset: 0x0018B14A
	private void FixedUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.FixedUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06004C3B RID: 19515 RVA: 0x0018CF61 File Offset: 0x0018B161
	private void Update()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.Update)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06004C3C RID: 19516 RVA: 0x0018CF77 File Offset: 0x0018B177
	private void LateUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.LateUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06004C3D RID: 19517 RVA: 0x0018CF90 File Offset: 0x0018B190
	private void UpdatePosition(float time)
	{
		float num = Mathf.Sin(time * 2f * 3.1415927f * this.period) * 0.5f + 0.5f;
		Vector3 vector = Vector3.Lerp(this.start.position, this.end.position, num);
		if (this.moveType == KinematicTestMotion.MoveType.TransformPosition)
		{
			base.transform.position = vector;
			return;
		}
		if (this.moveType == KinematicTestMotion.MoveType.RigidbodyMovePosition)
		{
			this.rigidbody.MovePosition(vector);
		}
	}

	// Token: 0x04005C29 RID: 23593
	public Transform start;

	// Token: 0x04005C2A RID: 23594
	public Transform end;

	// Token: 0x04005C2B RID: 23595
	public Rigidbody rigidbody;

	// Token: 0x04005C2C RID: 23596
	public KinematicTestMotion.UpdateType updateType;

	// Token: 0x04005C2D RID: 23597
	public KinematicTestMotion.MoveType moveType = KinematicTestMotion.MoveType.RigidbodyMovePosition;

	// Token: 0x04005C2E RID: 23598
	public float period = 4f;

	// Token: 0x02000C17 RID: 3095
	public enum UpdateType
	{
		// Token: 0x04005C30 RID: 23600
		Update,
		// Token: 0x04005C31 RID: 23601
		LateUpdate,
		// Token: 0x04005C32 RID: 23602
		FixedUpdate
	}

	// Token: 0x02000C18 RID: 3096
	public enum MoveType
	{
		// Token: 0x04005C34 RID: 23604
		TransformPosition,
		// Token: 0x04005C35 RID: 23605
		RigidbodyMovePosition
	}
}
