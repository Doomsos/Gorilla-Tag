using System;
using UnityEngine;

public class DestroyIfNotQA : MonoBehaviour
{
	private void Awake()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
