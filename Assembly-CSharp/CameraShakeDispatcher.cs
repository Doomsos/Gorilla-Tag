using System;
using UnityEngine;

public class CameraShakeDispatcher : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.shakeOnEnable)
		{
			this.Shake();
		}
	}

	public void Shake()
	{
		CameraShaker.Shake(this.magnitude, this.duration);
	}

	[SerializeField]
	private float magnitude = 1f;

	[SerializeField]
	private float duration = 0.5f;

	[SerializeField]
	private bool shakeOnEnable;
}
