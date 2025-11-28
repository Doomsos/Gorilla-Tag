using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000268 RID: 616
[Serializable]
public class HandTapOverrides
{
	// Token: 0x040013B2 RID: 5042
	private const string PREFAB_TOOLTIP = "Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.";

	// Token: 0x040013B3 RID: 5043
	public bool overrideSurfacePrefab;

	// Token: 0x040013B4 RID: 5044
	[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
	public HashWrapper surfaceTapPrefab;

	// Token: 0x040013B5 RID: 5045
	public bool overrideGamemodePrefab;

	// Token: 0x040013B6 RID: 5046
	[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
	public HashWrapper gamemodeTapPrefab;

	// Token: 0x040013B7 RID: 5047
	public bool overrideSound;

	// Token: 0x040013B8 RID: 5048
	public AudioClip tapSound;
}
