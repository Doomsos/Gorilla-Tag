using System;
using UnityEngine;

namespace GorillaTag
{
	public class InspectorNote : MonoBehaviour
	{
		protected void Awake()
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
