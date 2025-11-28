using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Rendering;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x0200093E RID: 2366
public class CMSZoneShaderSettingsTrigger : MonoBehaviour
{
	// Token: 0x06003C6E RID: 15470 RVA: 0x0013F1B1 File Offset: 0x0013D3B1
	public void OnEnable()
	{
		if (this.activateOnEnable)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x06003C6F RID: 15471 RVA: 0x0013F1C4 File Offset: 0x0013D3C4
	public void CopySettings(ZoneShaderTriggerSettings triggerSettings)
	{
		base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
		this.activateOnEnable = triggerSettings.activateOnEnable;
		if (triggerSettings.activationType == 1)
		{
			this.activateCustomMapDefaults = true;
			return;
		}
		GameObject zoneShaderSettingsObject = triggerSettings.zoneShaderSettingsObject;
		if (zoneShaderSettingsObject.IsNotNull())
		{
			this.shaderSettingsObject = zoneShaderSettingsObject;
		}
	}

	// Token: 0x06003C70 RID: 15472 RVA: 0x0013F216 File Offset: 0x0013D416
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.bodyCollider)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x06003C71 RID: 15473 RVA: 0x0013F230 File Offset: 0x0013D430
	private void ActivateShaderSettings()
	{
		if (this.activateCustomMapDefaults)
		{
			CustomMapManager.ActivateDefaultZoneShaderSettings();
			return;
		}
		if (this.shaderSettingsObject.IsNotNull())
		{
			ZoneShaderSettings component = this.shaderSettingsObject.GetComponent<ZoneShaderSettings>();
			if (component.IsNotNull())
			{
				component.BecomeActiveInstance(false);
			}
		}
	}

	// Token: 0x04004D13 RID: 19731
	public GameObject shaderSettingsObject;

	// Token: 0x04004D14 RID: 19732
	public bool activateCustomMapDefaults;

	// Token: 0x04004D15 RID: 19733
	public bool activateOnEnable;
}
