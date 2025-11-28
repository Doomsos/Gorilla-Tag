using System;

// Token: 0x020001E5 RID: 485
public class PlayerGameEvents
{
	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06000D33 RID: 3379 RVA: 0x00046D30 File Offset: 0x00044F30
	// (remove) Token: 0x06000D34 RID: 3380 RVA: 0x00046D64 File Offset: 0x00044F64
	public static event Action<string> OnGameModeObjectiveTrigger;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06000D35 RID: 3381 RVA: 0x00046D98 File Offset: 0x00044F98
	// (remove) Token: 0x06000D36 RID: 3382 RVA: 0x00046DCC File Offset: 0x00044FCC
	public static event Action<string> OnGameModeCompleteRound;

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06000D37 RID: 3383 RVA: 0x00046E00 File Offset: 0x00045000
	// (remove) Token: 0x06000D38 RID: 3384 RVA: 0x00046E34 File Offset: 0x00045034
	public static event Action<string> OnGrabbedObject;

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06000D39 RID: 3385 RVA: 0x00046E68 File Offset: 0x00045068
	// (remove) Token: 0x06000D3A RID: 3386 RVA: 0x00046E9C File Offset: 0x0004509C
	public static event Action<string> OnDroppedObject;

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06000D3B RID: 3387 RVA: 0x00046ED0 File Offset: 0x000450D0
	// (remove) Token: 0x06000D3C RID: 3388 RVA: 0x00046F04 File Offset: 0x00045104
	public static event Action<string> OnEatObject;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06000D3D RID: 3389 RVA: 0x00046F38 File Offset: 0x00045138
	// (remove) Token: 0x06000D3E RID: 3390 RVA: 0x00046F6C File Offset: 0x0004516C
	public static event Action<string> OnTapObject;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06000D3F RID: 3391 RVA: 0x00046FA0 File Offset: 0x000451A0
	// (remove) Token: 0x06000D40 RID: 3392 RVA: 0x00046FD4 File Offset: 0x000451D4
	public static event Action<string> OnLaunchedProjectile;

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x06000D41 RID: 3393 RVA: 0x00047008 File Offset: 0x00045208
	// (remove) Token: 0x06000D42 RID: 3394 RVA: 0x0004703C File Offset: 0x0004523C
	public static event Action<float, float> OnPlayerMoved;

	// Token: 0x14000020 RID: 32
	// (add) Token: 0x06000D43 RID: 3395 RVA: 0x00047070 File Offset: 0x00045270
	// (remove) Token: 0x06000D44 RID: 3396 RVA: 0x000470A4 File Offset: 0x000452A4
	public static event Action<float, float> OnPlayerSwam;

	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06000D45 RID: 3397 RVA: 0x000470D8 File Offset: 0x000452D8
	// (remove) Token: 0x06000D46 RID: 3398 RVA: 0x0004710C File Offset: 0x0004530C
	public static event Action<string> OnTriggerHandEffect;

	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06000D47 RID: 3399 RVA: 0x00047140 File Offset: 0x00045340
	// (remove) Token: 0x06000D48 RID: 3400 RVA: 0x00047174 File Offset: 0x00045374
	public static event Action<string> OnEnterLocation;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06000D49 RID: 3401 RVA: 0x000471A8 File Offset: 0x000453A8
	// (remove) Token: 0x06000D4A RID: 3402 RVA: 0x000471DC File Offset: 0x000453DC
	public static event Action<string, int> OnMiscEvent;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06000D4B RID: 3403 RVA: 0x00047210 File Offset: 0x00045410
	// (remove) Token: 0x06000D4C RID: 3404 RVA: 0x00047244 File Offset: 0x00045444
	public static event Action<string> OnCritterEvent;

	// Token: 0x06000D4D RID: 3405 RVA: 0x00047278 File Offset: 0x00045478
	public static void GameModeObjectiveTriggered()
	{
		string text = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeObjectiveTrigger = PlayerGameEvents.OnGameModeObjectiveTrigger;
		if (onGameModeObjectiveTrigger == null)
		{
			return;
		}
		onGameModeObjectiveTrigger.Invoke(text);
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x000472A0 File Offset: 0x000454A0
	public static void GameModeCompleteRound()
	{
		string text = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeCompleteRound = PlayerGameEvents.OnGameModeCompleteRound;
		if (onGameModeCompleteRound == null)
		{
			return;
		}
		onGameModeCompleteRound.Invoke(text);
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x000472C8 File Offset: 0x000454C8
	public static void GrabbedObject(string objectName)
	{
		Action<string> onGrabbedObject = PlayerGameEvents.OnGrabbedObject;
		if (onGrabbedObject == null)
		{
			return;
		}
		onGrabbedObject.Invoke(objectName);
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x000472DA File Offset: 0x000454DA
	public static void DroppedObject(string objectName)
	{
		Action<string> onDroppedObject = PlayerGameEvents.OnDroppedObject;
		if (onDroppedObject == null)
		{
			return;
		}
		onDroppedObject.Invoke(objectName);
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x000472EC File Offset: 0x000454EC
	public static void EatObject(string objectName)
	{
		Action<string> onEatObject = PlayerGameEvents.OnEatObject;
		if (onEatObject == null)
		{
			return;
		}
		onEatObject.Invoke(objectName);
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x000472FE File Offset: 0x000454FE
	public static void TapObject(string objectName)
	{
		Action<string> onTapObject = PlayerGameEvents.OnTapObject;
		if (onTapObject == null)
		{
			return;
		}
		onTapObject.Invoke(objectName);
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00047310 File Offset: 0x00045510
	public static void LaunchedProjectile(string objectName)
	{
		Action<string> onLaunchedProjectile = PlayerGameEvents.OnLaunchedProjectile;
		if (onLaunchedProjectile == null)
		{
			return;
		}
		onLaunchedProjectile.Invoke(objectName);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00047322 File Offset: 0x00045522
	public static void PlayerMoved(float distance, float speed)
	{
		Action<float, float> onPlayerMoved = PlayerGameEvents.OnPlayerMoved;
		if (onPlayerMoved == null)
		{
			return;
		}
		onPlayerMoved.Invoke(distance, speed);
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00047335 File Offset: 0x00045535
	public static void PlayerSwam(float distance, float speed)
	{
		Action<float, float> onPlayerSwam = PlayerGameEvents.OnPlayerSwam;
		if (onPlayerSwam == null)
		{
			return;
		}
		onPlayerSwam.Invoke(distance, speed);
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00047348 File Offset: 0x00045548
	public static void TriggerHandEffect(string effectName)
	{
		Action<string> onTriggerHandEffect = PlayerGameEvents.OnTriggerHandEffect;
		if (onTriggerHandEffect == null)
		{
			return;
		}
		onTriggerHandEffect.Invoke(effectName);
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x0004735A File Offset: 0x0004555A
	public static void TriggerEnterLocation(string locationName)
	{
		Action<string> onEnterLocation = PlayerGameEvents.OnEnterLocation;
		if (onEnterLocation == null)
		{
			return;
		}
		onEnterLocation.Invoke(locationName);
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x0004736C File Offset: 0x0004556C
	public static void MiscEvent(string eventName, int count = 1)
	{
		Action<string, int> onMiscEvent = PlayerGameEvents.OnMiscEvent;
		if (onMiscEvent == null)
		{
			return;
		}
		onMiscEvent.Invoke(eventName, count);
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x0004737F File Offset: 0x0004557F
	public static void CritterEvent(string eventName)
	{
		Action<string> onCritterEvent = PlayerGameEvents.OnCritterEvent;
		if (onCritterEvent == null)
		{
			return;
		}
		onCritterEvent.Invoke(eventName);
	}

	// Token: 0x020001E6 RID: 486
	public enum EventType
	{
		// Token: 0x04001042 RID: 4162
		NONE,
		// Token: 0x04001043 RID: 4163
		GameModeObjective,
		// Token: 0x04001044 RID: 4164
		GameModeCompleteRound,
		// Token: 0x04001045 RID: 4165
		GrabbedObject,
		// Token: 0x04001046 RID: 4166
		DroppedObject,
		// Token: 0x04001047 RID: 4167
		EatObject,
		// Token: 0x04001048 RID: 4168
		TapObject,
		// Token: 0x04001049 RID: 4169
		LaunchedProjectile,
		// Token: 0x0400104A RID: 4170
		PlayerMoved,
		// Token: 0x0400104B RID: 4171
		PlayerSwam,
		// Token: 0x0400104C RID: 4172
		TriggerHandEfffect,
		// Token: 0x0400104D RID: 4173
		EnterLocation,
		// Token: 0x0400104E RID: 4174
		MiscEvent
	}
}
