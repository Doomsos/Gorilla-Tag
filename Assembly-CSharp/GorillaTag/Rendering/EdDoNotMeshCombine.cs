using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	public class EdDoNotMeshCombine : MonoBehaviour
	{
		protected void Awake()
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
