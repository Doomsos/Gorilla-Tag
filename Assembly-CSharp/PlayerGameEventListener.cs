using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGameEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		this.SubscribeToEvents();
	}

	private void OnDisable()
	{
		this.UnsubscribeFromEvents();
	}

	private void SubscribeToEvents()
	{
		switch (this.eventType)
		{
		case PlayerGameEvents.EventType.NONE:
			return;
		case PlayerGameEvents.EventType.GameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.GameModeCompleteRound:
			PlayerGameEvents.OnGameModeCompleteRound += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.GrabbedObject:
			PlayerGameEvents.OnGrabbedObject += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.DroppedObject:
			PlayerGameEvents.OnDroppedObject += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.EatObject:
			PlayerGameEvents.OnEatObject += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.TapObject:
			PlayerGameEvents.OnTapObject += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.LaunchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.PlayerMoved:
			PlayerGameEvents.OnPlayerMoved += new Action<float, float>(this.OnGameMoveEventTriggered);
			return;
		case PlayerGameEvents.EventType.PlayerSwam:
			PlayerGameEvents.OnPlayerSwam += new Action<float, float>(this.OnGameMoveEventTriggered);
			return;
		case PlayerGameEvents.EventType.TriggerHandEfffect:
			PlayerGameEvents.OnTriggerHandEffect += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.EnterLocation:
			PlayerGameEvents.OnEnterLocation += new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.MiscEvent:
			PlayerGameEvents.OnMiscEvent += new Action<string, int>(this.OnGameEventTriggered);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void UnsubscribeFromEvents()
	{
		switch (this.eventType)
		{
		case PlayerGameEvents.EventType.NONE:
			return;
		case PlayerGameEvents.EventType.GameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.GameModeCompleteRound:
			PlayerGameEvents.OnGameModeCompleteRound -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.GrabbedObject:
			PlayerGameEvents.OnGrabbedObject -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.DroppedObject:
			PlayerGameEvents.OnDroppedObject -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.EatObject:
			PlayerGameEvents.OnEatObject -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.TapObject:
			PlayerGameEvents.OnTapObject -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.LaunchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.PlayerMoved:
			PlayerGameEvents.OnPlayerMoved -= new Action<float, float>(this.OnGameMoveEventTriggered);
			return;
		case PlayerGameEvents.EventType.PlayerSwam:
			PlayerGameEvents.OnPlayerSwam -= new Action<float, float>(this.OnGameMoveEventTriggered);
			return;
		case PlayerGameEvents.EventType.TriggerHandEfffect:
			PlayerGameEvents.OnTriggerHandEffect -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.EnterLocation:
			PlayerGameEvents.OnEnterLocation -= new Action<string>(this.OnGameEventTriggered);
			return;
		case PlayerGameEvents.EventType.MiscEvent:
			PlayerGameEvents.OnMiscEvent -= new Action<string, int>(this.OnGameEventTriggered);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void OnGameMoveEventTriggered(float distance, float speed)
	{
		Debug.LogError("Movement events not supported - please implement");
	}

	public void OnGameEventTriggered(string eventName)
	{
		this.OnGameEventTriggered(eventName, 1);
	}

	public void OnGameEventTriggered(string eventName, int count)
	{
		if (!string.IsNullOrEmpty(this.filter) && !eventName.StartsWith(this.filter))
		{
			return;
		}
		if (this._cooldownEnd > Time.time)
		{
			return;
		}
		this._cooldownEnd = Time.time + this.cooldown;
		UnityEvent unityEvent = this.onGameEvent;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		UnityEvent<int> unityEvent2 = this.onGameEventCounted;
		if (unityEvent2 == null)
		{
			return;
		}
		unityEvent2.Invoke(count);
	}

	[SerializeField]
	private PlayerGameEvents.EventType eventType;

	[Tooltip("Cooldown in seconds")]
	[SerializeField]
	private string filter;

	[SerializeField]
	private float cooldown = 1f;

	[SerializeField]
	private UnityEvent onGameEvent;

	[SerializeField]
	private UnityEvent<int> onGameEventCounted;

	private float _cooldownEnd;
}
