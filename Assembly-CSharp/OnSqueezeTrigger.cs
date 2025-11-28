using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000274 RID: 628
public class OnSqueezeTrigger : MonoBehaviour
{
	// Token: 0x06001019 RID: 4121 RVA: 0x00054ACB File Offset: 0x00052CCB
	private void Start()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x00054ADC File Offset: 0x00052CDC
	private void Update()
	{
		bool flag;
		if (this.myHoldable.InLeftHand())
		{
			flag = ((this.indexFinger ? this.myRig.leftIndex.calcT : this.myRig.leftMiddle.calcT) > 0.5f);
		}
		else
		{
			flag = (this.myHoldable.InRightHand() && (this.indexFinger ? this.myRig.rightIndex.calcT : this.myRig.rightMiddle.calcT) > 0.5f);
		}
		if (flag != this.triggerWasDown)
		{
			if (flag)
			{
				this.onPress.Invoke();
				this.updateWhilePressed.Invoke();
			}
			else
			{
				this.onRelease.Invoke();
			}
		}
		else if (flag)
		{
			this.updateWhilePressed.Invoke();
		}
		this.triggerWasDown = flag;
	}

	// Token: 0x0400140E RID: 5134
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x0400140F RID: 5135
	[SerializeField]
	private UnityEvent onPress;

	// Token: 0x04001410 RID: 5136
	[SerializeField]
	private UnityEvent onRelease;

	// Token: 0x04001411 RID: 5137
	[SerializeField]
	private UnityEvent updateWhilePressed;

	// Token: 0x04001412 RID: 5138
	private VRRig myRig;

	// Token: 0x04001413 RID: 5139
	private bool indexFinger = true;

	// Token: 0x04001414 RID: 5140
	private bool triggerWasDown;
}
