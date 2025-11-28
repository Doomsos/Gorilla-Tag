using System;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class AnimationEventController : MonoBehaviour
{
	// Token: 0x060009F0 RID: 2544 RVA: 0x00035D62 File Offset: 0x00033F62
	public void TriggerAttackVFX()
	{
		this.fxAttack.SetActive(false);
		this.fxAttack.SetActive(true);
	}

	// Token: 0x04000C2C RID: 3116
	public GameObject fxAttack;
}
