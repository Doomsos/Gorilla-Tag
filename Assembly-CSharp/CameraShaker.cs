using System;
using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
	private static event Action<float, float> ShakeRequested;

	public static void Shake(float duration, float magnitude)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude);
		}
	}

	private void OnEnable()
	{
		CameraShaker.ShakeRequested += this._ShakeRequested;
	}

	private void _ShakeRequested(float duration, float magnitude)
	{
		this.stopTime = Time.time + duration;
		if (!this.rumbling)
		{
			base.StartCoroutine(this.crRumble(magnitude));
		}
	}

	private void OnDisable()
	{
		CameraShaker.ShakeRequested -= this._ShakeRequested;
	}

	private void OnDestroy()
	{
		CameraShaker.ShakeRequested -= this._ShakeRequested;
	}

	private IEnumerator crRumble(float magnitude)
	{
		this.rumbling = true;
		Vector3 localPosition = base.transform.localPosition;
		while (this.stopTime > Time.time)
		{
			base.transform.localPosition = Random.insideUnitSphere * magnitude * (this.stopTime - Time.time);
			yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));
		}
		base.transform.localPosition = localPosition;
		this.rumbling = false;
		yield break;
	}

	private bool rumbling;

	private float stopTime;
}
