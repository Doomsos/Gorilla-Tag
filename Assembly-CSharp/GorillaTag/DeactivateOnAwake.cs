using System;
using UnityEngine;

namespace GorillaTag
{
	public class DeactivateOnAwake : MonoBehaviour
	{
		private void Awake()
		{
			base.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(this);
		}
	}
}
