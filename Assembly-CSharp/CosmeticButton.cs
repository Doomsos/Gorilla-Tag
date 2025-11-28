using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020004C6 RID: 1222
public class CosmeticButton : GorillaPressableButton
{
	// Token: 0x1700035E RID: 862
	// (get) Token: 0x06001F94 RID: 8084 RVA: 0x000A7DDB File Offset: 0x000A5FDB
	// (set) Token: 0x06001F95 RID: 8085 RVA: 0x000A7DE3 File Offset: 0x000A5FE3
	public bool Initialized { get; private set; }

	// Token: 0x06001F96 RID: 8086 RVA: 0x000A7DEC File Offset: 0x000A5FEC
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
		this.Initialized = true;
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x000A7E08 File Offset: 0x000A6008
	public override void UpdateColor()
	{
		if (!base.enabled)
		{
			this.buttonRenderer.material = this.disabledMaterial;
			this.SetOffText(this.myText != null, false, false);
		}
		else if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			this.SetOnText(this.myText.IsNotNull(), false, false);
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			this.SetOffText(this.myText != null, false, false);
		}
		this.UpdatePosition();
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x000A7EA0 File Offset: 0x000A60A0
	public virtual void UpdatePosition()
	{
		Vector3 vector = this.startingPos;
		if (!base.enabled)
		{
			vector += this.disabledOffset;
		}
		else if (this.isOn)
		{
			vector += this.pressedOffset;
		}
		this.posOffset = base.transform.position;
		base.transform.localPosition = vector;
		this.posOffset = base.transform.position - this.posOffset;
		if (this.myText != null)
		{
			this.myText.transform.position += this.posOffset;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.transform.position += this.posOffset;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.transform.position += this.posOffset;
		}
	}

	// Token: 0x040029E5 RID: 10725
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x040029E6 RID: 10726
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x040029E7 RID: 10727
	[SerializeField]
	private Vector3 disabledOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x040029E8 RID: 10728
	private Vector3 startingPos;

	// Token: 0x040029E9 RID: 10729
	protected Vector3 posOffset;
}
