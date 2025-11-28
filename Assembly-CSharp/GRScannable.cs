using System;
using UnityEngine;

// Token: 0x020006F3 RID: 1779
public class GRScannable : MonoBehaviour
{
	// Token: 0x06002D9A RID: 11674 RVA: 0x000F67B3 File Offset: 0x000F49B3
	public virtual void Start()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
	}

	// Token: 0x06002D9B RID: 11675 RVA: 0x000F67CF File Offset: 0x000F49CF
	public virtual string GetTitleText(GhostReactor reactor)
	{
		return this.titleText;
	}

	// Token: 0x06002D9C RID: 11676 RVA: 0x000F67D7 File Offset: 0x000F49D7
	public virtual string GetBodyText(GhostReactor reactor)
	{
		return this.bodyText;
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x000F67DF File Offset: 0x000F49DF
	public virtual string GetAnnotationText(GhostReactor reactor)
	{
		return this.annotationText;
	}

	// Token: 0x04003B47 RID: 15175
	public GameEntity gameEntity;

	// Token: 0x04003B48 RID: 15176
	[SerializeField]
	protected string titleText;

	// Token: 0x04003B49 RID: 15177
	[SerializeField]
	protected string bodyText;

	// Token: 0x04003B4A RID: 15178
	[SerializeField]
	protected string annotationText;
}
