using System;
using UnityEngine;

namespace GorillaTag
{
	public class DestroyOnAwake : MonoBehaviour
	{
		protected void Awake()
		{
			try
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		protected void OnEnable()
		{
			try
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		protected void Update()
		{
			try
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}
	}
}
