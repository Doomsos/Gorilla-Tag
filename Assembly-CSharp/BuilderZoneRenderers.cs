using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000568 RID: 1384
public class BuilderZoneRenderers : MonoBehaviour
{
	// Token: 0x060022D8 RID: 8920 RVA: 0x000B60A0 File Offset: 0x000B42A0
	private void Start()
	{
		this.allRenderers.Clear();
		this.allRenderers.AddRange(this.renderers);
		foreach (GameObject gameObject in this.rootObjects)
		{
			this.allRenderers.AddRange(gameObject.GetComponentsInChildren<Renderer>(true));
		}
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x000B6150 File Offset: 0x000B4350
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x000B6188 File Offset: 0x000B4388
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer in this.allRenderers)
			{
				renderer.enabled = true;
			}
			using (List<Canvas>.Enumerator enumerator2 = this.canvases.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Canvas canvas = enumerator2.Current;
					canvas.enabled = true;
				}
				return;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer2 in this.allRenderers)
			{
				renderer2.enabled = false;
			}
			foreach (Canvas canvas2 in this.canvases)
			{
				canvas2.enabled = false;
			}
		}
	}

	// Token: 0x04002D8D RID: 11661
	public List<Renderer> renderers;

	// Token: 0x04002D8E RID: 11662
	public List<Canvas> canvases;

	// Token: 0x04002D8F RID: 11663
	public List<GameObject> rootObjects;

	// Token: 0x04002D90 RID: 11664
	private bool inBuilderZone;

	// Token: 0x04002D91 RID: 11665
	private List<Renderer> allRenderers = new List<Renderer>(200);
}
