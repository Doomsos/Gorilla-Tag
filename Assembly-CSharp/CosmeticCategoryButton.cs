using System;
using UnityEngine;

// Token: 0x020004C7 RID: 1223
public class CosmeticCategoryButton : CosmeticButton
{
	// Token: 0x06001F9A RID: 8090 RVA: 0x000A8002 File Offset: 0x000A6202
	public void SetIcon(Sprite sprite)
	{
		this.equippedLeftIcon.enabled = false;
		this.equippedRightIcon.enabled = false;
		this.equippedIcon.enabled = (sprite != null);
		this.equippedIcon.sprite = sprite;
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x000A803C File Offset: 0x000A623C
	public void SetDualIcon(Sprite leftSprite, Sprite rightSprite)
	{
		this.equippedLeftIcon.enabled = (leftSprite != null);
		this.equippedRightIcon.enabled = (rightSprite != null);
		this.equippedIcon.enabled = false;
		this.equippedLeftIcon.sprite = leftSprite;
		this.equippedRightIcon.sprite = rightSprite;
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x000A8094 File Offset: 0x000A6294
	public override void UpdatePosition()
	{
		base.UpdatePosition();
		if (this.equippedIcon != null)
		{
			this.equippedIcon.transform.position += this.posOffset;
		}
		if (this.equippedLeftIcon != null)
		{
			this.equippedLeftIcon.transform.position += this.posOffset;
		}
		if (this.equippedRightIcon != null)
		{
			this.equippedRightIcon.transform.position += this.posOffset;
		}
	}

	// Token: 0x040029EB RID: 10731
	[SerializeField]
	private SpriteRenderer equippedIcon;

	// Token: 0x040029EC RID: 10732
	[SerializeField]
	private SpriteRenderer equippedLeftIcon;

	// Token: 0x040029ED RID: 10733
	[SerializeField]
	private SpriteRenderer equippedRightIcon;
}
