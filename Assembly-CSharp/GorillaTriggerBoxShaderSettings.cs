using System;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x0200052A RID: 1322
public class GorillaTriggerBoxShaderSettings : GorillaTriggerBox
{
	// Token: 0x0600216F RID: 8559 RVA: 0x000AF6FD File Offset: 0x000AD8FD
	private void Awake()
	{
		if (this.sameSceneSettingsRef != null)
		{
			this.settings = this.sameSceneSettingsRef;
			return;
		}
		this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x000AF72C File Offset: 0x000AD92C
	public override void OnBoxTriggered()
	{
		if (this.settings == null)
		{
			if (this.sameSceneSettingsRef != null)
			{
				this.settings = this.sameSceneSettingsRef;
			}
			else
			{
				this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
			}
		}
		if (this.settings != null)
		{
			this.settings.BecomeActiveInstance(false);
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
	}

	// Token: 0x04002C27 RID: 11303
	[SerializeField]
	private XSceneRef settingsRef;

	// Token: 0x04002C28 RID: 11304
	[SerializeField]
	private ZoneShaderSettings sameSceneSettingsRef;

	// Token: 0x04002C29 RID: 11305
	private ZoneShaderSettings settings;
}
