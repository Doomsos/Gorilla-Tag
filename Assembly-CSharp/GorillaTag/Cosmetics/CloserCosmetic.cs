using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010C3 RID: 4291
	public class CloserCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x06006B9E RID: 27550 RVA: 0x00235160 File Offset: 0x00233360
		// (set) Token: 0x06006B9F RID: 27551 RVA: 0x00235168 File Offset: 0x00233368
		public bool TickRunning { get; set; }

		// Token: 0x06006BA0 RID: 27552 RVA: 0x00235174 File Offset: 0x00233374
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.localRotA = this.sideA.transform.localRotation;
			this.localRotB = this.sideB.transform.localRotation;
			this.fingerValue = 0f;
			this.UpdateState(CloserCosmetic.State.Opening);
		}

		// Token: 0x06006BA1 RID: 27553 RVA: 0x00036897 File Offset: 0x00034A97
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06006BA2 RID: 27554 RVA: 0x002351C8 File Offset: 0x002333C8
		public void Tick()
		{
			switch (this.currentState)
			{
			case CloserCosmetic.State.Closing:
				this.Closing();
				return;
			case CloserCosmetic.State.Opening:
				this.Opening();
				break;
			case CloserCosmetic.State.None:
				break;
			default:
				return;
			}
		}

		// Token: 0x06006BA3 RID: 27555 RVA: 0x002351FC File Offset: 0x002333FC
		public void Close(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Closing);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x06006BA4 RID: 27556 RVA: 0x0023520C File Offset: 0x0023340C
		public void Open(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Opening);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x06006BA5 RID: 27557 RVA: 0x0023521C File Offset: 0x0023341C
		private void Closing()
		{
			float num = this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f;
			Quaternion quaternion = Quaternion.Euler(this.maxRotationB);
			Quaternion quaternion2 = Quaternion.Slerp(this.localRotB, quaternion, num);
			this.sideB.transform.localRotation = quaternion2;
			Quaternion quaternion3 = Quaternion.Euler(this.maxRotationA);
			Quaternion quaternion4 = Quaternion.Slerp(this.localRotA, quaternion3, num);
			this.sideA.transform.localRotation = quaternion4;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion2) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion4) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x06006BA6 RID: 27558 RVA: 0x002352E0 File Offset: 0x002334E0
		private void Opening()
		{
			float num = this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f;
			Quaternion quaternion = Quaternion.Slerp(this.sideB.transform.localRotation, this.localRotB, num);
			this.sideB.transform.localRotation = quaternion;
			Quaternion quaternion2 = Quaternion.Slerp(this.sideA.transform.localRotation, this.localRotA, num);
			this.sideA.transform.localRotation = quaternion2;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion2) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x06006BA7 RID: 27559 RVA: 0x002353A5 File Offset: 0x002335A5
		private void UpdateState(CloserCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04007C15 RID: 31765
		[SerializeField]
		private GameObject sideA;

		// Token: 0x04007C16 RID: 31766
		[SerializeField]
		private GameObject sideB;

		// Token: 0x04007C17 RID: 31767
		[SerializeField]
		private Vector3 maxRotationA;

		// Token: 0x04007C18 RID: 31768
		[SerializeField]
		private Vector3 maxRotationB;

		// Token: 0x04007C19 RID: 31769
		[SerializeField]
		private bool useFingerFlexValueAsStrength;

		// Token: 0x04007C1A RID: 31770
		private Quaternion localRotA;

		// Token: 0x04007C1B RID: 31771
		private Quaternion localRotB;

		// Token: 0x04007C1C RID: 31772
		private CloserCosmetic.State currentState;

		// Token: 0x04007C1D RID: 31773
		private float fingerValue;

		// Token: 0x020010C4 RID: 4292
		private enum State
		{
			// Token: 0x04007C20 RID: 31776
			Closing,
			// Token: 0x04007C21 RID: 31777
			Opening,
			// Token: 0x04007C22 RID: 31778
			None
		}
	}
}
