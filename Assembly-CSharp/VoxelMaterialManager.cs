using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMaterialManager : MonoBehaviour
{
	[Serializable]
	public struct LightingProfile
	{
		public Color color;

		public Vector3 direction;
	}

	public Material[] voxelMats;

	public List<string> lightmapNames;

	public List<LightingProfile> lightingProfiles;

	private int _timeOfDayIndex = -1;

	private void Update()
	{
		if (_timeOfDayIndex != BetterDayNightManager.instance.currentTimeIndex)
		{
			UpdateMaterial();
		}
	}

	private void UpdateMaterial()
	{
		string currentTimeOfDay = BetterDayNightManager.instance.currentTimeOfDay;
		if (!string.IsNullOrEmpty(currentTimeOfDay))
		{
			int num = lightmapNames.IndexOf(currentTimeOfDay);
			if (num >= 0 && num < lightingProfiles.Count)
			{
				Debug.Log($"[V] Setting lighting profile {num} ({currentTimeOfDay})");
				LightingProfile lightingProfile = lightingProfiles[num];
				Shader.SetGlobalVector("_Light_Direction", lightingProfile.direction);
				Shader.SetGlobalColor("_Light_Color", lightingProfile.color);
				Shader.SetGlobalColor("_Shadow_Color", lightingProfile.color * 0.32f);
				Shader.SetGlobalColor("_Rim_Color", lightingProfile.color * 0.1f);
				Shader.SetGlobalColor("_Backlight_Color", lightingProfile.color * 0.07f);
				_timeOfDayIndex = BetterDayNightManager.instance.currentTimeIndex;
			}
		}
	}
}
