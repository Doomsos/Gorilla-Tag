using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000428 RID: 1064
public class SlingshotProjectileTrail : MonoBehaviour
{
	// Token: 0x06001A44 RID: 6724 RVA: 0x0008BDCC File Offset: 0x00089FCC
	private void Awake()
	{
		this.initialWidthMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x0008BDE0 File Offset: 0x00089FE0
	public void AttachTrail(GameObject obj, bool blueTeam, bool redTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		this.followObject = obj;
		this.followXform = this.followObject.transform;
		Transform transform = base.transform;
		transform.position = this.followXform.position;
		this.initialScale = transform.localScale.x;
		transform.localScale = this.followXform.localScale;
		this.trailRenderer.widthMultiplier = this.initialWidthMultiplier * this.followXform.localScale.x;
		this.trailRenderer.Clear();
		if (shouldOverrideColor)
		{
			this.SetColor(overrideColor);
		}
		else if (blueTeam)
		{
			this.SetColor(this.blueColor);
		}
		else if (redTeam)
		{
			this.SetColor(this.orangeColor);
		}
		else
		{
			this.SetColor(this.defaultColor);
		}
		this.timeToDie = -1f;
	}

	// Token: 0x06001A46 RID: 6726 RVA: 0x0008BEB4 File Offset: 0x0008A0B4
	protected void LateUpdate()
	{
		if (this.followObject.IsNull())
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		base.gameObject.transform.position = this.followXform.position;
		if (!this.followObject.activeSelf && this.timeToDie < 0f)
		{
			this.timeToDie = Time.time + this.trailRenderer.time;
		}
		if (this.timeToDie > 0f && Time.time > this.timeToDie)
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001A47 RID: 6727 RVA: 0x0008BF70 File Offset: 0x0008A170
	public void SetColor(Color color)
	{
		TrailRenderer trailRenderer = this.trailRenderer;
		this.trailRenderer.endColor = color;
		trailRenderer.startColor = color;
	}

	// Token: 0x040023C7 RID: 9159
	public TrailRenderer trailRenderer;

	// Token: 0x040023C8 RID: 9160
	public Color defaultColor = Color.white;

	// Token: 0x040023C9 RID: 9161
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x040023CA RID: 9162
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x040023CB RID: 9163
	private GameObject followObject;

	// Token: 0x040023CC RID: 9164
	private Transform followXform;

	// Token: 0x040023CD RID: 9165
	private float timeToDie = -1f;

	// Token: 0x040023CE RID: 9166
	private float initialScale;

	// Token: 0x040023CF RID: 9167
	private float initialWidthMultiplier;
}
