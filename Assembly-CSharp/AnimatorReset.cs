using System;
using UnityEngine;

// Token: 0x02000536 RID: 1334
public class AnimatorReset : MonoBehaviour
{
	// Token: 0x0600219B RID: 8603 RVA: 0x000AFF43 File Offset: 0x000AE143
	public void Reset()
	{
		if (!this.target)
		{
			return;
		}
		this.target.Rebind();
		this.target.Update(0f);
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000AFF6E File Offset: 0x000AE16E
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.Reset();
		}
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x000AFF7E File Offset: 0x000AE17E
	private void OnDisable()
	{
		if (this.onDisable)
		{
			this.Reset();
		}
	}

	// Token: 0x04002C4F RID: 11343
	public Animator target;

	// Token: 0x04002C50 RID: 11344
	public bool onEnable;

	// Token: 0x04002C51 RID: 11345
	public bool onDisable = true;
}
