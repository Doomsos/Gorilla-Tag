using System;
using UnityEngine;

namespace GorillaTagScripts
{
	public class BuilderTestTools : MonoBehaviour
	{
		public void Awake()
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
