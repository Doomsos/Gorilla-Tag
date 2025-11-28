using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020005F0 RID: 1520
public class FortuneTellerButton : GorillaPressableButton
{
	// Token: 0x06002646 RID: 9798 RVA: 0x000CC7B9 File Offset: 0x000CA9B9
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x000CC7CC File Offset: 0x000CA9CC
	public override void ButtonActivation()
	{
		this.PressButtonUpdate();
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x000CC7D4 File Offset: 0x000CA9D4
	public void PressButtonUpdate()
	{
		if (this.pressTime != 0f)
		{
			return;
		}
		base.transform.localPosition = this.startingPos + this.pressedOffset;
		this.buttonRenderer.material = this.pressedMaterial;
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonUpdate>g__ButtonColorUpdate_Local|6_0());
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x000CC861 File Offset: 0x000CAA61
	[CompilerGenerated]
	private IEnumerator <PressButtonUpdate>g__ButtonColorUpdate_Local|6_0()
	{
		yield return new WaitForSeconds(this.durationPressed);
		if (this.pressTime != 0f && Time.time > this.durationPressed + this.pressTime)
		{
			base.transform.localPosition = this.startingPos;
			this.buttonRenderer.material = this.unpressedMaterial;
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x04003227 RID: 12839
	[SerializeField]
	private float durationPressed = 0.25f;

	// Token: 0x04003228 RID: 12840
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04003229 RID: 12841
	private float pressTime;

	// Token: 0x0400322A RID: 12842
	private Vector3 startingPos;
}
