using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks;

public sealed class AlarmClockManager : MonoBehaviour
{
	[Serializable]
	public class AlarmClockData
	{
		public string Key;

		public GTZone[] Zones;

		public Transform SpawnPoint;
	}

	public const string SaveDataKey = "AlarmClock";

	[SerializeField]
	private string _loadingMessage = "";

	[SerializeField]
	private AlarmClockData[] _clockData = new AlarmClockData[0];

	[SerializeField]
	private Transform _defaultSpawn;

	public UnityEvent OnWakeUp;

	private Transform _teleportTarget;

	private AlarmClockData _activeClockData;

	[CanBeNull]
	private AlarmClock _activeClock;

	public static AlarmClockManager Instance { get; private set; }

	public string ActiveKey { get; private set; } = "";

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Duplicate instance of singleton class AlarmClockManager.");
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (_defaultSpawn == null)
		{
			Debug.LogError("No default spawn set in AlarmClockManager.");
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		ActiveKey = PlayerPrefs.GetString("AlarmClock");
		if (!string.IsNullOrEmpty(ActiveKey))
		{
			StartCoroutine(ConnectToPlayerSpawned());
		}
	}

	private IEnumerator ConnectToPlayerSpawned()
	{
		while (GorillaTagger.Instance == null || GorillaTagger.Instance.mainCamera == null)
		{
			yield return null;
		}
		PrivateUIRoom.ForceStartOverlay(_loadingMessage);
		yield return new WaitForSeconds(1f);
		GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
	}

	public static void ToggleAlarmClock(AlarmClock clock)
	{
		Instance.ToggleAlarmClockInternal(clock);
	}

	private void ToggleAlarmClockInternal(AlarmClock clock)
	{
		if (ActiveKey == clock.Key)
		{
			_activeClock?.OnDeactivate?.Invoke();
			_activeClock = null;
			ActiveKey = "";
		}
		else
		{
			_activeClock?.OnDeactivate?.Invoke();
			_activeClock = clock;
			_activeClock?.OnActivate?.Invoke();
			ActiveKey = clock.Key;
		}
		PlayerPrefs.SetString("AlarmClock", ActiveKey);
		Debug.Log("Alarm clock data set to \"" + ActiveKey + "\".");
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	[UsedImplicitly]
	private bool AllUniqueClockKeys()
	{
		HashSet<string> hashSet = new HashSet<string>();
		AlarmClockData[] clockData = _clockData;
		foreach (AlarmClockData alarmClockData in clockData)
		{
			if (!hashSet.Add(alarmClockData.Key))
			{
				return false;
			}
		}
		return true;
	}

	private void OnPlayerSpawned()
	{
		Debug.Log("Loaded alarm clock value " + ActiveKey);
		if (!string.IsNullOrEmpty(ActiveKey))
		{
			AlarmClockData alarmClockData = _clockData.FirstOrDefault((AlarmClockData c) => c.Key == ActiveKey);
			if (alarmClockData != null)
			{
				_activeClockData = alarmClockData;
				_teleportTarget = alarmClockData.SpawnPoint;
				PrivateUIRoom.ForceStartOverlay(_loadingMessage);
				ZoneManagement.SetActiveZones(alarmClockData.Zones);
				StartCoroutine(OnZoneLoaded());
				StartCoroutine(ClearUnsubPlayerData());
			}
		}
	}

	private IEnumerator ClearUnsubPlayerData()
	{
		while (!MothershipClientApiUnity.IsClientLoggedIn())
		{
			yield return null;
		}
		if (SubscriptionManager.LocalSubscriptionStatus() != SubscriptionManager.SubscriptionStatus.Active)
		{
			PlayerPrefs.SetString("AlarmClock", "");
			GTPlayer.Instance.TeleportTo(_defaultSpawn.position, _defaultSpawn.rotation);
		}
	}

	private IEnumerator OnZoneLoaded()
	{
		yield return null;
		if (_activeClockData.Zones.Length > 1 || _activeClockData.Zones[0] != GTZone.forest)
		{
			while (ZoneManagement.instance.ZonesSetting)
			{
				yield return null;
			}
		}
		if (GTPlayer.hasInstance)
		{
			GTPlayer.Instance.TeleportTo(_teleportTarget.position, _teleportTarget.rotation);
		}
		PrivateUIRoom.StopForcedOverlay();
		OnWakeUp?.Invoke();
	}
}
