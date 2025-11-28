using System;
using UnityEngine;

// Token: 0x02000340 RID: 832
public class ZoneConditionalComponentEnabling : MonoBehaviour
{
	// Token: 0x06001409 RID: 5129 RVA: 0x00073C27 File Offset: 0x00071E27
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x00073C55 File Offset: 0x00071E55
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x00073C80 File Offset: 0x00071E80
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.IsInZone(this.zone);
		bool enabled = this.invisibleWhileLoaded ? (!flag) : flag;
		if (this.components != null)
		{
			for (int i = 0; i < this.components.Length; i++)
			{
				if (this.components[i] != null)
				{
					this.components[i].enabled = enabled;
				}
			}
		}
		if (this.m_renderers != null)
		{
			for (int j = 0; j < this.m_renderers.Length; j++)
			{
				if (this.m_renderers[j] != null)
				{
					this.m_renderers[j].enabled = enabled;
				}
			}
		}
		if (this.m_colliders != null)
		{
			for (int k = 0; k < this.m_colliders.Length; k++)
			{
				if (this.m_colliders[k] != null)
				{
					this.m_colliders[k].enabled = enabled;
				}
			}
		}
	}

	// Token: 0x04001E9A RID: 7834
	[SerializeField]
	private GTZone zone;

	// Token: 0x04001E9B RID: 7835
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x04001E9C RID: 7836
	[SerializeField]
	private Behaviour[] components;

	// Token: 0x04001E9D RID: 7837
	[SerializeField]
	private Renderer[] m_renderers;

	// Token: 0x04001E9E RID: 7838
	[SerializeField]
	private Collider[] m_colliders;
}
