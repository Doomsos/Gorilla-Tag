using System.Collections;
using System.Threading.Tasks;
using GorillaLocomotion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks;

[RequireComponent(typeof(Collider))]
public sealed class AlarmClock : MonoBehaviour
{
	private const float TouchDebouncePeriod = 1f;

	[SerializeField]
	private string _key;

	[SerializeField]
	private TextMeshPro _readout;

	[SerializeField]
	private float _onTime = 1f;

	[SerializeField]
	private float _offTime = 0.2f;

	public UnityEvent OnActivate;

	public UnityEvent OnDeactivate;

	private float _lastTouchTime = float.MinValue;

	public string Key => _key;

	public bool Initialized { get; private set; }

	private async void Awake()
	{
		while (AlarmClockManager.Instance == null)
		{
			await Task.Yield();
		}
		if (AlarmClockManager.Instance.ActiveKey == _key)
		{
			OnActivateCallback();
		}
		else
		{
			OnDeactivateCallback();
		}
		Initialized = true;
	}

	private void OnEnable()
	{
		OnActivate.AddListener(OnActivateCallback);
		OnDeactivate.AddListener(OnDeactivateCallback);
		StartCoroutine(Blink());
	}

	private void OnDisable()
	{
		OnActivate.RemoveListener(OnActivateCallback);
		OnDeactivate.RemoveListener(OnDeactivateCallback);
		StopAllCoroutines();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (Initialized && !(Time.time < _lastTouchTime + 1f) && !(other.GetComponentInParent<GTPlayer>() == null) && SubscriptionManager.IsLocalSubscribed())
		{
			_lastTouchTime = Time.time;
			AlarmClockManager.ToggleAlarmClock(this);
		}
	}

	private void OnActivateCallback()
	{
		_readout.text = "SET";
		_readout.color = Color.green;
	}

	private void OnDeactivateCallback()
	{
		_readout.text = "88:88";
		_readout.color = Color.red;
	}

	private IEnumerator Blink()
	{
		while (true)
		{
			_readout.enabled = true;
			yield return new WaitForSeconds(_onTime);
			_readout.enabled = false;
			yield return new WaitForSeconds(_offTime);
		}
	}
}
