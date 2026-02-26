using System;
using UnityEngine;

public class DestroyOnDisabled : MonoBehaviour
{
	private void OnDisable()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
