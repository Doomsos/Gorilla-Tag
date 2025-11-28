using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001E3 RID: 483
public class PlayerGameEventListener : MonoBehaviour
{
	// Token: 0x06000D29 RID: 3369 RVA: 0x000469FA File Offset: 0x00044BFA
	private void OnEnable()
	{
		this.SubscribeToEvents();
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x00046A02 File Offset: 0x00044C02
	private void OnDisable()
	{
		this.UnsubscribeFromEvents();
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00046A0C File Offset: 0x00044C0C
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

	// Token: 0x06000D2C RID: 3372 RVA: 0x00046B40 File Offset: 0x00044D40
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

	// Token: 0x06000D2D RID: 3373 RVA: 0x00046C71 File Offset: 0x00044E71
	private void OnGameMoveEventTriggered(float distance, float speed)
	{
		Debug.LogError("Movement events not supported - please implement");
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00046C7D File Offset: 0x00044E7D
	public void OnGameEventTriggered(string eventName)
	{
		this.OnGameEventTriggered(eventName, 1);
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00046C88 File Offset: 0x00044E88
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

	// Token: 0x0400102D RID: 4141
	[SerializeField]
	private PlayerGameEvents.EventType eventType;

	// Token: 0x0400102E RID: 4142
	[Tooltip("Cooldown in seconds")]
	[SerializeField]
	private string filter;

	// Token: 0x0400102F RID: 4143
	[SerializeField]
	private float cooldown = 1f;

	// Token: 0x04001030 RID: 4144
	[SerializeField]
	private UnityEvent onGameEvent;

	// Token: 0x04001031 RID: 4145
	[SerializeField]
	private UnityEvent<int> onGameEventCounted;

	// Token: 0x04001032 RID: 4146
	private float _cooldownEnd;
}
