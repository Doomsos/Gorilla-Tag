using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010D8 RID: 4312
	public class ControllerButtonEvent : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06006C1C RID: 27676 RVA: 0x002375A6 File Offset: 0x002357A6
		// (set) Token: 0x06006C1D RID: 27677 RVA: 0x002375AE File Offset: 0x002357AE
		public bool IsSpawned { get; set; }

		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06006C1E RID: 27678 RVA: 0x002375B7 File Offset: 0x002357B7
		// (set) Token: 0x06006C1F RID: 27679 RVA: 0x002375BF File Offset: 0x002357BF
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006C20 RID: 27680 RVA: 0x002375C8 File Offset: 0x002357C8
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06006C21 RID: 27681 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x06006C22 RID: 27682 RVA: 0x002375D1 File Offset: 0x002357D1
		private bool IsMyItem()
		{
			return this.myRig != null && this.myRig.isOfflineVRRig;
		}

		// Token: 0x06006C23 RID: 27683 RVA: 0x002375EE File Offset: 0x002357EE
		private void Awake()
		{
			this.triggerLastValue = 0f;
			this.gripLastValue = 0f;
			this.primaryLastValue = false;
			this.secondaryLastValue = false;
			this.frameCounter = 0;
		}

		// Token: 0x06006C24 RID: 27684 RVA: 0x0023761C File Offset: 0x0023581C
		public void LateUpdate()
		{
			if (!this.IsMyItem())
			{
				return;
			}
			XRNode node = this.inLeftHand ? 4 : 5;
			switch (this.buttonType)
			{
			case ControllerButtonEvent.ButtonType.trigger:
			{
				float num = ControllerInputPoller.TriggerFloat(node);
				if (num > this.triggerValue)
				{
					this.frameCounter++;
				}
				if (num > this.triggerValue && this.triggerLastValue < this.triggerValue)
				{
					UnityEvent<bool, float> unityEvent = this.onButtonPressed;
					if (unityEvent != null)
					{
						unityEvent.Invoke(this.inLeftHand, num);
					}
				}
				else if (num <= this.triggerReleaseValue && this.triggerLastValue > this.triggerReleaseValue)
				{
					UnityEvent<bool, float> unityEvent2 = this.onButtonReleased;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				else if (num > this.triggerValue && this.triggerLastValue >= this.triggerValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent3 = this.onButtonPressStayed;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				this.triggerLastValue = num;
				return;
			}
			case ControllerButtonEvent.ButtonType.primary:
			{
				bool flag = ControllerInputPoller.PrimaryButtonPress(node);
				if (flag)
				{
					this.frameCounter++;
				}
				if (flag && !this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent4 = this.onButtonPressed;
					if (unityEvent4 != null)
					{
						unityEvent4.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag && this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent5 = this.onButtonReleased;
					if (unityEvent5 != null)
					{
						unityEvent5.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag && this.primaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent6 = this.onButtonPressStayed;
					if (unityEvent6 != null)
					{
						unityEvent6.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.primaryLastValue = flag;
				return;
			}
			case ControllerButtonEvent.ButtonType.secondary:
			{
				bool flag2 = ControllerInputPoller.SecondaryButtonPress(node);
				if (flag2)
				{
					this.frameCounter++;
				}
				if (flag2 && !this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent7 = this.onButtonPressed;
					if (unityEvent7 != null)
					{
						unityEvent7.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag2 && this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent8 = this.onButtonReleased;
					if (unityEvent8 != null)
					{
						unityEvent8.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag2 && this.secondaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent9 = this.onButtonPressStayed;
					if (unityEvent9 != null)
					{
						unityEvent9.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.secondaryLastValue = flag2;
				return;
			}
			case ControllerButtonEvent.ButtonType.grip:
			{
				float num2 = ControllerInputPoller.GripFloat(node);
				if (num2 > this.gripValue)
				{
					this.frameCounter++;
				}
				if (num2 > this.gripValue && this.gripLastValue < this.gripValue)
				{
					UnityEvent<bool, float> unityEvent10 = this.onButtonPressed;
					if (unityEvent10 != null)
					{
						unityEvent10.Invoke(this.inLeftHand, num2);
					}
				}
				else if (num2 <= this.gripReleaseValue && this.gripLastValue > this.gripReleaseValue)
				{
					UnityEvent<bool, float> unityEvent11 = this.onButtonReleased;
					if (unityEvent11 != null)
					{
						unityEvent11.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				else if (num2 > this.gripValue && this.gripLastValue >= this.gripValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent12 = this.onButtonPressStayed;
					if (unityEvent12 != null)
					{
						unityEvent12.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				this.gripLastValue = num2;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x04007CB5 RID: 31925
		[SerializeField]
		private float gripValue = 0.75f;

		// Token: 0x04007CB6 RID: 31926
		[SerializeField]
		private float gripReleaseValue = 0.01f;

		// Token: 0x04007CB7 RID: 31927
		[SerializeField]
		private float triggerValue = 0.75f;

		// Token: 0x04007CB8 RID: 31928
		[SerializeField]
		private float triggerReleaseValue = 0.01f;

		// Token: 0x04007CB9 RID: 31929
		[SerializeField]
		private ControllerButtonEvent.ButtonType buttonType;

		// Token: 0x04007CBA RID: 31930
		[Tooltip("How many frames should pass to trigger a press stayed button")]
		[SerializeField]
		private int frameInterval = 20;

		// Token: 0x04007CBB RID: 31931
		public UnityEvent<bool, float> onButtonPressed;

		// Token: 0x04007CBC RID: 31932
		public UnityEvent<bool, float> onButtonReleased;

		// Token: 0x04007CBD RID: 31933
		public UnityEvent<bool, float> onButtonPressStayed;

		// Token: 0x04007CBE RID: 31934
		private float triggerLastValue;

		// Token: 0x04007CBF RID: 31935
		private float gripLastValue;

		// Token: 0x04007CC0 RID: 31936
		private bool primaryLastValue;

		// Token: 0x04007CC1 RID: 31937
		private bool secondaryLastValue;

		// Token: 0x04007CC2 RID: 31938
		private int frameCounter;

		// Token: 0x04007CC3 RID: 31939
		private bool inLeftHand;

		// Token: 0x04007CC4 RID: 31940
		private VRRig myRig;

		// Token: 0x020010D9 RID: 4313
		private enum ButtonType
		{
			// Token: 0x04007CC8 RID: 31944
			trigger,
			// Token: 0x04007CC9 RID: 31945
			primary,
			// Token: 0x04007CCA RID: 31946
			secondary,
			// Token: 0x04007CCB RID: 31947
			grip
		}
	}
}
