using System;
using Liv.Lck;
using Liv.Lck.DependencyInjection;
using UnityEngine;

// Token: 0x02000361 RID: 865
[DefaultExecutionOrder(-950)]
public class GtLckServiceInitializer : MonoBehaviour
{
	// Token: 0x0600148E RID: 5262 RVA: 0x00075B68 File Offset: 0x00073D68
	private void Awake()
	{
		LckDiContainer instance = LckDiContainer.Instance;
		if (instance.HasService<ILckService>())
		{
			Debug.LogWarning("LCK: Service already configured. Skipping custom GT initialisation.");
			return;
		}
		Debug.Log("LCK: Initializing with GT-SPECIFIC overrides.");
		LckServiceInitializer.ConfigureServices(instance, this._qualityConfig, delegate(LckDiContainer container)
		{
			container.AddSingleton<ILckCosmeticsFeatureFlagManager, LckCosmeticsFeatureFlagManagerPlayFab>();
		});
	}

	// Token: 0x04001F2A RID: 7978
	[Header("LCK Configuration")]
	[Tooltip("Assign the LCK Quality Config ScriptableObject here.")]
	[SerializeField]
	private LckQualityConfig _qualityConfig;
}
