using System;
using UnityEngine;

namespace GorillaTag
{
	public static class GTAppState
	{
		public static bool isQuitting { get; private set; }

		[RuntimeInitializeOnLoadMethod(4)]
		private static void HandleOnSubsystemRegistration()
		{
			GTAppState.isQuitting = false;
			Application.quitting += delegate()
			{
				GTAppState.isQuitting = true;
			};
			Debug.Log(string.Concat(new string[]
			{
				"GTAppState:\n- SystemInfo.operatingSystem=",
				SystemInfo.operatingSystem,
				"\n- SystemInfo.maxTextureArraySlices=",
				SystemInfo.maxTextureArraySlices.ToString(),
				"\n"
			}));
		}

		[RuntimeInitializeOnLoadMethod(0)]
		private static void HandleOnAfterSceneLoad()
		{
		}
	}
}
