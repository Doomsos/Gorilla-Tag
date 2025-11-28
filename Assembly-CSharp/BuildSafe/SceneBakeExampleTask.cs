using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	// Token: 0x02000EB2 RID: 3762
	public class SceneBakeExampleTask : SceneBakeTask
	{
		// Token: 0x06005DE4 RID: 24036 RVA: 0x001E1944 File Offset: 0x001DFB44
		public override void OnSceneBake(Scene scene, SceneBakeMode mode)
		{
			for (int i = 0; i < 10; i++)
			{
				SceneBakeExampleTask.DuplicateAndRecolor(base.gameObject);
			}
			if (mode != SceneBakeMode.OnBuildPlayer)
			{
			}
		}

		// Token: 0x06005DE5 RID: 24037 RVA: 0x001E1974 File Offset: 0x001DFB74
		private static void DuplicateAndRecolor(GameObject target)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(target);
			gameObject.transform.position = Random.insideUnitSphere * 4f;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.material = new Material(component.sharedMaterial)
			{
				color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f)
			};
		}
	}
}
