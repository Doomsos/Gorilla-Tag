using System;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class Monkeye_LazerFX : MonoBehaviour
{
	// Token: 0x060009EB RID: 2539 RVA: 0x00035C6E File Offset: 0x00033E6E
	private void Awake()
	{
		base.enabled = false;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x00035C78 File Offset: 0x00033E78
	public void EnableLazer(Transform[] eyes_, VRRig rig_)
	{
		if (rig_ == this.rig)
		{
			return;
		}
		this.eyeBones = eyes_;
		this.rig = rig_;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].positionCount = 2;
		}
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x00035CC8 File Offset: 0x00033EC8
	public void DisableLazer()
	{
		if (base.enabled)
		{
			base.enabled = false;
			LineRenderer[] array = this.lines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].positionCount = 0;
			}
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00035D04 File Offset: 0x00033F04
	private void Update()
	{
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].SetPosition(0, this.eyeBones[i].transform.position);
			this.lines[i].SetPosition(1, this.rig.transform.position);
		}
	}

	// Token: 0x04000C29 RID: 3113
	private Transform[] eyeBones;

	// Token: 0x04000C2A RID: 3114
	private VRRig rig;

	// Token: 0x04000C2B RID: 3115
	public LineRenderer[] lines;
}
