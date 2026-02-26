using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	public class SceneBakeExampleTask : SceneBakeTask
	{
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

		private static void DuplicateAndRecolor(GameObject target)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(target);
			gameObject.transform.position = UnityEngine.Random.insideUnitSphere * 4f;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.material = new Material(component.sharedMaterial)
			{
				color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f)
			};
		}
	}
}
