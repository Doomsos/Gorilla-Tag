using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000342 RID: 834
public class ZoneConditionalVisibility : MonoBehaviour
{
	// Token: 0x06001411 RID: 5137 RVA: 0x00073E2F File Offset: 0x0007202F
	private void Awake()
	{
		if (this.renderersOnly)
		{
			this.renderers = new List<Renderer>(32);
			base.GetComponentsInChildren<Renderer>(false, this.renderers);
		}
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x00073E53 File Offset: 0x00072053
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x00073E81 File Offset: 0x00072081
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x00073EAC File Offset: 0x000720AC
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.IsInZone(this.zone);
		if (this.invisibleWhileLoaded)
		{
			if (this.renderersOnly)
			{
				for (int i = 0; i < this.renderers.Count; i++)
				{
					if (this.renderers[i] != null)
					{
						this.renderers[i].enabled = !flag;
					}
				}
				return;
			}
			base.gameObject.SetActive(!flag);
			return;
		}
		else
		{
			if (this.renderersOnly)
			{
				for (int j = 0; j < this.renderers.Count; j++)
				{
					if (this.renderers[j] != null)
					{
						this.renderers[j].enabled = flag;
					}
				}
				return;
			}
			base.gameObject.SetActive(flag);
			return;
		}
	}

	// Token: 0x04001EA2 RID: 7842
	[SerializeField]
	private GTZone zone;

	// Token: 0x04001EA3 RID: 7843
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x04001EA4 RID: 7844
	[SerializeField]
	private bool renderersOnly;

	// Token: 0x04001EA5 RID: 7845
	private List<Renderer> renderers;
}
