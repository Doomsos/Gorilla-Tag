using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using JetBrains.Annotations;
using KID.Model;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EventsModels;
using UnityEngine;

// Token: 0x020007E4 RID: 2020
public static class GorillaTelemetry
{
	// Token: 0x0600351A RID: 13594 RVA: 0x0011E1B0 File Offset: 0x0011C3B0
	static GorillaTelemetry()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["User"] = null;
		dictionary["EventType"] = null;
		dictionary["ZoneId"] = null;
		dictionary["SubZoneId"] = null;
		GorillaTelemetry.gZoneEventArgs = dictionary;
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2["User"] = null;
		dictionary2["EventType"] = null;
		GorillaTelemetry.gNotifEventArgs = dictionary2;
		GorillaTelemetry.nextStayTimestamp = 0f;
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary3["User"] = null;
		dictionary3["EventType"] = null;
		dictionary3["game_mode"] = null;
		GorillaTelemetry.gGameModeStartEventArgs = dictionary3;
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		dictionary4["User"] = null;
		dictionary4["EventType"] = null;
		dictionary4["Items"] = null;
		GorillaTelemetry.gShopEventArgs = dictionary4;
		GorillaTelemetry.gSingleItemParam = new CosmeticsController.CosmeticItem[1];
		GorillaTelemetry.gSingleItemBuilderParam = new BuilderSetManager.BuilderSetStoreItem[1];
		Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
		dictionary5["User"] = null;
		dictionary5["EventType"] = null;
		dictionary5["AgeCategory"] = null;
		dictionary5["VoiceChatEnabled"] = null;
		dictionary5["CustomUsernameEnabled"] = null;
		dictionary5["JoinGroups"] = null;
		GorillaTelemetry.gKidEventArgs = dictionary5;
		Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
		dictionary6["User"] = null;
		dictionary6["WamGameId"] = null;
		dictionary6["WamMachineId"] = null;
		GorillaTelemetry.gWamGameStartArgs = dictionary6;
		Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
		dictionary7["User"] = null;
		dictionary7["WamGameId"] = null;
		dictionary7["WamMachineId"] = null;
		dictionary7["WamMLevelNumber"] = null;
		dictionary7["WamGoodMolesShown"] = null;
		dictionary7["WamHazardMolesShown"] = null;
		dictionary7["WamLevelMinScore"] = null;
		dictionary7["WamLevelScore"] = null;
		dictionary7["WamHazardMolesHit"] = null;
		dictionary7["WamGameState"] = null;
		GorillaTelemetry.gWamLevelEndArgs = dictionary7;
		Dictionary<string, object> dictionary8 = new Dictionary<string, object>();
		dictionary8["CustomMapName"] = null;
		dictionary8["CustomMapModId"] = null;
		dictionary8["LowestFPS"] = null;
		dictionary8["LowestFPSDrawCalls"] = null;
		dictionary8["LowestFPSPlayerCount"] = null;
		dictionary8["AverageFPS"] = null;
		dictionary8["AverageDrawCalls"] = null;
		dictionary8["AveragePlayerCount"] = null;
		dictionary8["HighestFPS"] = null;
		dictionary8["HighestFPSDrawCalls"] = null;
		dictionary8["HighestFPSPlayerCount"] = null;
		dictionary8["PlaytimeInSeconds"] = null;
		GorillaTelemetry.gCustomMapPerfArgs = dictionary8;
		Dictionary<string, object> dictionary9 = new Dictionary<string, object>();
		dictionary9["User"] = null;
		dictionary9["CustomMapName"] = null;
		dictionary9["CustomMapModId"] = null;
		dictionary9["CustomMapCreator"] = null;
		dictionary9["MinPlayerCount"] = null;
		dictionary9["MaxPlayerCount"] = null;
		dictionary9["PlaytimeOnMap"] = null;
		dictionary9["PrivateRoom"] = null;
		GorillaTelemetry.gCustomMapTrackingMetrics = dictionary9;
		Dictionary<string, object> dictionary10 = new Dictionary<string, object>();
		dictionary10["User"] = null;
		dictionary10["CustomMapName"] = null;
		dictionary10["CustomMapModId"] = null;
		dictionary10["CustomMapCreator"] = null;
		GorillaTelemetry.gCustomMapDownloadMetrics = dictionary10;
		Dictionary<string, object> dictionary11 = new Dictionary<string, object>();
		dictionary11["User"] = null;
		dictionary11["ghost_game_id"] = null;
		dictionary11["event_timestamp"] = null;
		dictionary11["initial_cores_balance"] = null;
		dictionary11["number_of_players"] = null;
		dictionary11["start_at_beginning"] = null;
		dictionary11["seconds_into_shift_at_join"] = null;
		dictionary11["floor_joined"] = null;
		dictionary11["player_rank"] = null;
		dictionary11["is_private_room"] = null;
		GorillaTelemetry.gGhostReactorShiftStartArgs = dictionary11;
		Dictionary<string, object> dictionary12 = new Dictionary<string, object>();
		dictionary12["User"] = null;
		dictionary12["ghost_game_id"] = null;
		dictionary12["event_timestamp"] = null;
		dictionary12["final_cores_balance"] = null;
		dictionary12["total_cores_collected_by_player"] = null;
		dictionary12["total_cores_collected_by_group"] = null;
		dictionary12["total_cores_spent_by_player"] = null;
		dictionary12["total_cores_spent_by_group"] = null;
		dictionary12["gates_unlocked"] = null;
		dictionary12["died"] = null;
		dictionary12["items_purchased"] = null;
		dictionary12["shift_cut"] = null;
		dictionary12["play_duration"] = null;
		dictionary12["started_late"] = null;
		dictionary12["time_started"] = null;
		dictionary12["reason"] = null;
		dictionary12["max_number_in_game"] = null;
		dictionary12["end_number_in_game"] = null;
		dictionary12["items_picked_up"] = null;
		dictionary12["revives"] = null;
		dictionary12["num_shifts_played"] = null;
		GorillaTelemetry.gGhostReactorShiftEndArgs = dictionary12;
		Dictionary<string, object> dictionary13 = new Dictionary<string, object>();
		dictionary13["User"] = null;
		dictionary13["ghost_game_id"] = null;
		dictionary13["event_timestamp"] = null;
		dictionary13["initial_cores_balance"] = null;
		dictionary13["number_of_players"] = null;
		dictionary13["start_at_beginning"] = null;
		dictionary13["seconds_into_shift_at_join"] = null;
		dictionary13["player_rank"] = null;
		dictionary13["floor"] = null;
		dictionary13["preset"] = null;
		dictionary13["modifier"] = null;
		dictionary13["is_private_room"] = null;
		GorillaTelemetry.gGhostReactorFloorStartArgs = dictionary13;
		Dictionary<string, object> dictionary14 = new Dictionary<string, object>();
		dictionary14["User"] = null;
		dictionary14["ghost_game_id"] = null;
		dictionary14["event_timestamp"] = null;
		dictionary14["final_cores_balance"] = null;
		dictionary14["total_cores_collected_by_player"] = null;
		dictionary14["total_cores_collected_by_group"] = null;
		dictionary14["total_cores_spent_by_player"] = null;
		dictionary14["total_cores_spent_by_group"] = null;
		dictionary14["gates_unlocked"] = null;
		dictionary14["died"] = null;
		dictionary14["items_purchased"] = null;
		dictionary14["shift_cut"] = null;
		dictionary14["play_duration"] = null;
		dictionary14["started_late"] = null;
		dictionary14["time_started"] = null;
		dictionary14["max_number_in_game"] = null;
		dictionary14["end_number_in_game"] = null;
		dictionary14["items_picked_up"] = null;
		dictionary14["revives"] = null;
		dictionary14["floor"] = null;
		dictionary14["preset"] = null;
		dictionary14["modifier"] = null;
		dictionary14["chaos_seeds_collected"] = null;
		dictionary14["objectives_completed"] = null;
		dictionary14["section"] = null;
		dictionary14["xp_gained"] = null;
		GorillaTelemetry.gGhostReactorFloorEndArgs = dictionary14;
		Dictionary<string, object> dictionary15 = new Dictionary<string, object>();
		dictionary15["User"] = null;
		dictionary15["ghost_game_id"] = null;
		dictionary15["event_timestamp"] = null;
		dictionary15["tool"] = null;
		dictionary15["tool_level"] = null;
		dictionary15["cores_spent"] = null;
		dictionary15["shiny_rocks_spent"] = null;
		dictionary15["floor"] = null;
		dictionary15["preset"] = null;
		GorillaTelemetry.gGhostReactorToolPurchasedArgs = dictionary15;
		Dictionary<string, object> dictionary16 = new Dictionary<string, object>();
		dictionary16["User"] = null;
		dictionary16["ghost_game_id"] = null;
		dictionary16["event_timestamp"] = null;
		dictionary16["new_rank"] = null;
		dictionary16["floor"] = null;
		dictionary16["preset"] = null;
		GorillaTelemetry.gGhostReactorRankUpArgs = dictionary16;
		Dictionary<string, object> dictionary17 = new Dictionary<string, object>();
		dictionary17["User"] = null;
		dictionary17["ghost_game_id"] = null;
		dictionary17["event_timestamp"] = null;
		dictionary17["tool"] = null;
		GorillaTelemetry.gGhostReactorToolUnlockArgs = dictionary17;
		Dictionary<string, object> dictionary18 = new Dictionary<string, object>();
		dictionary18["User"] = null;
		dictionary18["ghost_game_id"] = null;
		dictionary18["event_timestamp"] = null;
		dictionary18["tool"] = null;
		dictionary18["new_level"] = null;
		dictionary18["shiny_rocks_spent"] = null;
		dictionary18["juice_spent"] = null;
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs = dictionary18;
		Dictionary<string, object> dictionary19 = new Dictionary<string, object>();
		dictionary19["User"] = null;
		dictionary19["ghost_game_id"] = null;
		dictionary19["event_timestamp"] = null;
		dictionary19["type"] = null;
		dictionary19["tool"] = null;
		dictionary19["new_level"] = null;
		dictionary19["juice_spent"] = null;
		dictionary19["grift_spent"] = null;
		dictionary19["cores_spent"] = null;
		dictionary19["floor"] = null;
		dictionary19["preset"] = null;
		GorillaTelemetry.gGhostReactorToolUpgradeArgs = dictionary19;
		Dictionary<string, object> dictionary20 = new Dictionary<string, object>();
		dictionary20["User"] = null;
		dictionary20["ghost_game_id"] = null;
		dictionary20["event_timestamp"] = null;
		dictionary20["unlock_time"] = null;
		dictionary20["chaos_seeds_in_queue"] = null;
		dictionary20["floor"] = null;
		dictionary20["preset"] = null;
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs = dictionary20;
		Dictionary<string, object> dictionary21 = new Dictionary<string, object>();
		dictionary21["User"] = null;
		dictionary21["ghost_game_id"] = null;
		dictionary21["event_timestamp"] = null;
		dictionary21["juice_collected"] = null;
		dictionary21["cores_processed_by_overdrive"] = null;
		GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs = dictionary21;
		Dictionary<string, object> dictionary22 = new Dictionary<string, object>();
		dictionary22["User"] = null;
		dictionary22["ghost_game_id"] = null;
		dictionary22["event_timestamp"] = null;
		dictionary22["shiny_rocks_used"] = null;
		dictionary22["chaos_seeds_in_queue"] = null;
		dictionary22["floor"] = null;
		dictionary22["preset"] = null;
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs = dictionary22;
		Dictionary<string, object> dictionary23 = new Dictionary<string, object>();
		dictionary23["User"] = null;
		dictionary23["ghost_game_id"] = null;
		dictionary23["event_timestamp"] = null;
		dictionary23["shiny_rocks_spent"] = null;
		dictionary23["final_credits"] = null;
		dictionary23["floor"] = null;
		dictionary23["preset"] = null;
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs = dictionary23;
		Dictionary<string, object> dictionary24 = new Dictionary<string, object>();
		dictionary24["User"] = null;
		dictionary24["total_play_time"] = null;
		dictionary24["room_play_time"] = null;
		dictionary24["session_play_time"] = null;
		dictionary24["interval_play_time"] = null;
		dictionary24["terminal_total_time"] = null;
		dictionary24["terminal_interval_time"] = null;
		dictionary24["time_holding_gadget_type_total"] = null;
		dictionary24["time_holding_gadget_type_interval"] = null;
		dictionary24["tags_holding_gadget_type_total"] = null;
		dictionary24["tags_holding_gadget_type_interval"] = null;
		dictionary24["tags_holding_own_gadgets_total"] = null;
		dictionary24["tags_holding_own_gadgets_interval"] = null;
		dictionary24["tags_holding_others_gadgets_total"] = null;
		dictionary24["tags_holding_others_gadgets_interval"] = null;
		dictionary24["resource_collected_total"] = null;
		dictionary24["resource_collected_interval"] = null;
		dictionary24["rounds_played_total"] = null;
		dictionary24["rounds_played_interval"] = null;
		dictionary24["unlocked_nodes"] = null;
		dictionary24["number_of_players"] = null;
		dictionary24["si_purchase_type"] = null;
		dictionary24["si_shiny_rock_cost"] = null;
		dictionary24["si_tech_points_purchased"] = null;
		GorillaTelemetry.gSuperInfectionArgs = dictionary24;
		GameObject gameObject = new GameObject("GorillaTelemetryBatcher");
		Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<GorillaTelemetry.BatchRunner>();
	}

	// Token: 0x0600351B RID: 13595 RVA: 0x0011ED1C File Offset: 0x0011CF1C
	public static void EnqueueTelemetryEvent(string eventName, object content, [CanBeNull] string[] customTags = null)
	{
		if (content == null || string.IsNullOrWhiteSpace(eventName) || !GorillaServer.Instance.CheckIsMothershipTelemetryEnabled())
		{
			return;
		}
		if (GorillaTelemetry.telemetryEventsQueueMothership.Count > 100)
		{
			Debug.LogError("[Telemetry] Too many telemetry events!  Not enqueueing " + eventName + ": " + content.ToJson(true));
			return;
		}
		GorillaTelemetry.telemetryEventsQueueMothership.Enqueue(new MothershipAnalyticsEvent
		{
			event_name = eventName,
			event_timestamp = DateTime.UtcNow.ToString("O"),
			body = JsonConvert.SerializeObject(content),
			custom_tags = ((customTags != null && customTags.Length != 0) ? GorillaTelemetry.SerializeCustomTags(customTags) : string.Empty)
		});
	}

	// Token: 0x0600351C RID: 13596 RVA: 0x0011EDC2 File Offset: 0x0011CFC2
	[Obsolete("EnqueueTelemetryEventPlayFab is deprecated. Use EnqueueTelemetryEvent instead.")]
	private static void EnqueueTelemetryEventPlayFab(EventContents eventContent)
	{
		if (!GorillaServer.Instance.CheckIsPlayFabTelemetryEnabled())
		{
			return;
		}
		GorillaTelemetry.telemetryEventsQueuePlayFab.Enqueue(eventContent);
	}

	// Token: 0x0600351D RID: 13597 RVA: 0x0011EDE0 File Offset: 0x0011CFE0
	private static void FlushPlayFabTelemetry()
	{
		int count = GorillaTelemetry.telemetryEventsQueuePlayFab.Count;
		if (count == 0)
		{
			return;
		}
		EventContents[] array = ArrayPool<EventContents>.Shared.Rent(count);
		try
		{
			int i;
			for (i = 0; i < count; i++)
			{
				EventContents eventContents;
				array[i] = (GorillaTelemetry.telemetryEventsQueuePlayFab.TryDequeue(ref eventContents) ? eventContents : null);
			}
			if (i == 0)
			{
				ArrayPool<EventContents>.Shared.Return(array, false);
			}
			else
			{
				WriteEventsRequest writeEventsRequest = new WriteEventsRequest();
				writeEventsRequest.Events = GorillaTelemetry.GetEventListForArrayPlayFab(array, i);
				PlayFabEventsAPI.WriteTelemetryEvents(writeEventsRequest, delegate(WriteEventsResponse result)
				{
				}, delegate(PlayFabError error)
				{
				}, null, null);
			}
		}
		finally
		{
			ArrayPool<EventContents>.Shared.Return(array, false);
		}
	}

	// Token: 0x0600351E RID: 13598 RVA: 0x0011EEB0 File Offset: 0x0011D0B0
	private static void FlushMothershipTelemetry()
	{
		int count = GorillaTelemetry.telemetryEventsQueueMothership.Count;
		if (count == 0)
		{
			return;
		}
		MothershipAnalyticsEvent[] array = ArrayPool<MothershipAnalyticsEvent>.Shared.Rent(count);
		try
		{
			int j;
			for (j = 0; j < count; j++)
			{
				MothershipAnalyticsEvent mothershipAnalyticsEvent;
				array[j] = (GorillaTelemetry.telemetryEventsQueueMothership.TryDequeue(ref mothershipAnalyticsEvent) ? mothershipAnalyticsEvent : null);
			}
			if (j == 0)
			{
				ArrayPool<MothershipAnalyticsEvent>.Shared.Return(array, false);
			}
			else
			{
				MothershipWriteEventsRequest mothershipWriteEventsRequest = new MothershipWriteEventsRequest
				{
					title_id = MothershipClientApiUnity.TitleId,
					deployment_id = MothershipClientApiUnity.DeploymentId,
					env_id = MothershipClientApiUnity.EnvironmentId,
					events = new AnalyticsRequestVector(GorillaTelemetry.GetEventListForArrayMothership(array, j))
				};
				MothershipClientApiUnity.WriteEvents(MothershipClientContext.MothershipId, mothershipWriteEventsRequest, delegate(MothershipWriteEventsResponse resp)
				{
				}, delegate(MothershipError err, int i)
				{
				});
			}
		}
		finally
		{
			ArrayPool<MothershipAnalyticsEvent>.Shared.Return(array, false);
		}
	}

	// Token: 0x0600351F RID: 13599 RVA: 0x0011EFB0 File Offset: 0x0011D1B0
	private static List<EventContents> GetEventListForArrayPlayFab(EventContents[] array, int count)
	{
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (array[i] != null)
			{
				num++;
			}
		}
		List<EventContents> list;
		if (!GorillaTelemetry.gListPoolPlayFab.TryGetValue(num, ref list))
		{
			list = new List<EventContents>(num);
			GorillaTelemetry.gListPoolPlayFab.TryAdd(num, list);
		}
		else
		{
			list.Clear();
		}
		for (int j = 0; j < count; j++)
		{
			if (array[j] != null)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	// Token: 0x06003520 RID: 13600 RVA: 0x0011F01C File Offset: 0x0011D21C
	private static List<MothershipAnalyticsEvent> GetEventListForArrayMothership(MothershipAnalyticsEvent[] array, int count)
	{
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (array[i] != null)
			{
				num++;
			}
		}
		List<MothershipAnalyticsEvent> list;
		if (!GorillaTelemetry.gListPoolMothership.TryGetValue(num, ref list))
		{
			list = new List<MothershipAnalyticsEvent>(num);
			GorillaTelemetry.gListPoolMothership.TryAdd(num, list);
		}
		else
		{
			list.Clear();
		}
		string code = LocalisationManager.CurrentLanguage.Identifier.Code;
		for (int j = 0; j < count; j++)
		{
			if (array[j] != null)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	// Token: 0x06003521 RID: 13601 RVA: 0x0011F09F File Offset: 0x0011D29F
	private static bool IsConnected()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return false;
		}
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003522 RID: 13602 RVA: 0x0011F0D2 File Offset: 0x0011D2D2
	private static bool IsConnectedToPlayfab()
	{
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003523 RID: 13603 RVA: 0x0011F0D2 File Offset: 0x0011D2D2
	private static bool IsConnectedIgnoreRoom()
	{
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003524 RID: 13604 RVA: 0x0011F0F7 File Offset: 0x0011D2F7
	private static string PlayFabUserId()
	{
		return GorillaTelemetry.gPlayFabAuth.GetPlayFabPlayerId();
	}

	// Token: 0x06003525 RID: 13605 RVA: 0x0011F104 File Offset: 0x0011D304
	private static string SerializeCustomTags(string[] customTags)
	{
		string result = string.Empty;
		if (customTags != null && customTags.Length != 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < customTags.Length; i++)
			{
				dictionary.Add(string.Format("tag{0}", i + 1), customTags[i]);
			}
			result = JsonConvert.SerializeObject(dictionary);
		}
		return result;
	}

	// Token: 0x06003526 RID: 13606 RVA: 0x0011F158 File Offset: 0x0011D358
	public static void EnqueueZoneEvent(ZoneDef zone, GTZoneEventType zoneEventType)
	{
		if (zoneEventType == GTZoneEventType.zone_stay && Time.realtimeSinceStartup < GorillaTelemetry.nextStayTimestamp)
		{
			return;
		}
		GorillaTelemetry.nextStayTimestamp = Time.realtimeSinceStartup + (float)zone.trackStayIntervalSec;
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!GorillaServer.Instance.CheckIsTZE_Enabled())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = zoneEventType.GetName<GTZoneEventType>();
		string name2 = zone.zoneId.GetName<GTZone>();
		string name3 = zone.subZoneId.GetName<GTSubZone>();
		bool sessionIsPrivate = NetworkSystem.Instance.SessionIsPrivate;
		Dictionary<string, object> dictionary = GorillaTelemetry.gZoneEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["ZoneId"] = name2;
		dictionary["SubZoneId"] = name3;
		dictionary["IsPrivateRoom"] = sessionIsPrivate;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_zone_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_zone_event", dictionary, null);
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x0011F254 File Offset: 0x0011D454
	public static void PostGameModeEvent(GTGameModeEventType gameModeEvent, GameModeType gameMode)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = gameModeEvent.GetName<GTGameModeEventType>();
		string name2 = gameMode.GetName<GameModeType>();
		Dictionary<string, object> dictionary = GorillaTelemetry.gGameModeStartEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["game_mode"] = name2;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "game_mode_played_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.EnqueueTelemetryEvent("game_mode_played_event", dictionary, null);
	}

	// Token: 0x06003528 RID: 13608 RVA: 0x0011F2DA File Offset: 0x0011D4DA
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, CosmeticsController.CosmeticItem item)
	{
		GorillaTelemetry.gSingleItemParam[0] = item;
		GorillaTelemetry.PostShopEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemParam);
		GorillaTelemetry.gSingleItemParam[0] = default(CosmeticsController.CosmeticItem);
	}

	// Token: 0x06003529 RID: 13609 RVA: 0x0011F308 File Offset: 0x0011D508
	private static string[] FetchItemArgs(IList<CosmeticsController.CosmeticItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			CosmeticsController.CosmeticItem cosmeticItem = items[i];
			if (!cosmeticItem.isNullItem)
			{
				string itemName = cosmeticItem.itemName;
				if (!string.IsNullOrWhiteSpace(itemName) && !itemName.Contains("NOTHING", 3) && hashSet.Add(itemName))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x0011F394 File Offset: 0x0011D594
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, IList<CosmeticsController.CosmeticItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] array = GorillaTelemetry.FetchItemArgs(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["Items"] = array;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_shop_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_shop_event", dictionary, null);
	}

	// Token: 0x0600352B RID: 13611 RVA: 0x00002789 File Offset: 0x00000989
	private static void PostShopEvent_OnResult(WriteEventResponse result)
	{
	}

	// Token: 0x0600352C RID: 13612 RVA: 0x00002789 File Offset: 0x00000989
	private static void PostShopEvent_OnError(PlayFabError error)
	{
	}

	// Token: 0x0600352D RID: 13613 RVA: 0x0011F423 File Offset: 0x0011D623
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, BuilderSetManager.BuilderSetStoreItem item)
	{
		GorillaTelemetry.gSingleItemBuilderParam[0] = item;
		GorillaTelemetry.PostBuilderKioskEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemBuilderParam);
		GorillaTelemetry.gSingleItemBuilderParam[0] = default(BuilderSetManager.BuilderSetStoreItem);
	}

	// Token: 0x0600352E RID: 13614 RVA: 0x0011F450 File Offset: 0x0011D650
	private static string[] BuilderItemsToStrings(IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem = items[i];
			if (!builderSetStoreItem.isNullItem)
			{
				string playfabID = builderSetStoreItem.playfabID;
				if (!string.IsNullOrWhiteSpace(playfabID) && !playfabID.Contains("NOTHING", 3) && hashSet.Add(playfabID))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x0600352F RID: 13615 RVA: 0x0011F4DC File Offset: 0x0011D6DC
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] array = GorillaTelemetry.BuilderItemsToStrings(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["Items"] = array;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_shop_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_shop_event", dictionary, null);
	}

	// Token: 0x06003530 RID: 13616 RVA: 0x0011F56C File Offset: 0x0011D76C
	public static void PostKidEvent(bool joinGroupsEnabled, bool voiceChatEnabled, bool customUsernamesEnabled, AgeStatusType ageCategory, GTKidEventType kidEvent)
	{
		if ((double)Random.value < 0.1)
		{
			return;
		}
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = kidEvent.GetName<GTKidEventType>();
		string text2 = (ageCategory == 3) ? "Not_Managed_Account" : "Managed_Account";
		string text3 = joinGroupsEnabled.ToString().ToUpper();
		string text4 = voiceChatEnabled.ToString().ToUpper();
		string text5 = customUsernamesEnabled.ToString().ToUpper();
		Dictionary<string, object> dictionary = GorillaTelemetry.gKidEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["AgeCategory"] = text2;
		dictionary["VoiceChatEnabled"] = text4;
		dictionary["CustomUsernameEnabled"] = text5;
		dictionary["JoinGroups"] = text3;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_kid_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_kid_event", dictionary, null);
	}

	// Token: 0x06003531 RID: 13617 RVA: 0x0011F668 File Offset: 0x0011D868
	public static void WamGameStart(string playerId, string gameId, string machineId)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamGameStartArgs["User"] = playerId;
		GorillaTelemetry.gWamGameStartArgs["WamGameId"] = gameId;
		GorillaTelemetry.gWamGameStartArgs["WamMachineId"] = machineId;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_wam_gameStartEvent",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gWamGameStartArgs
		});
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_wam_gameStartEvent", GorillaTelemetry.gWamGameStartArgs, null);
	}

	// Token: 0x06003532 RID: 13618 RVA: 0x0011F6E8 File Offset: 0x0011D8E8
	public static void WamLevelEnd(string playerId, int gameId, string machineId, int currentLevelNumber, int levelGoodMolesShown, int levelHazardMolesShown, int levelMinScore, int currentScore, int levelHazardMolesHit, string currentGameResult)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamLevelEndArgs["User"] = playerId;
		GorillaTelemetry.gWamLevelEndArgs["WamGameId"] = gameId.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamMachineId"] = machineId;
		GorillaTelemetry.gWamLevelEndArgs["WamMLevelNumber"] = currentLevelNumber.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGoodMolesShown"] = levelGoodMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesShown"] = levelHazardMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelMinScore"] = levelMinScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelScore"] = currentScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesHit"] = levelHazardMolesHit.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGameState"] = currentGameResult;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_wam_levelEndEvent",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gWamLevelEndArgs
		});
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_wam_levelEndEvent", GorillaTelemetry.gWamLevelEndArgs, null);
	}

	// Token: 0x06003533 RID: 13619 RVA: 0x0011F804 File Offset: 0x0011DA04
	public static void PostCustomMapPerformance(string mapName, long mapModId, int lowestFPS, int lowestDC, int lowestPC, int avgFPS, int avgDC, int avgPC, int highestFPS, int highestDC, int highestPC, int playtime)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapPerfArgs;
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["LowestFPS"] = lowestFPS.ToString();
		dictionary["LowestFPSDrawCalls"] = lowestDC.ToString();
		dictionary["LowestFPSPlayerCount"] = lowestPC.ToString();
		dictionary["AverageFPS"] = avgFPS.ToString();
		dictionary["AverageDrawCalls"] = avgDC.ToString();
		dictionary["AveragePlayerCount"] = avgPC.ToString();
		dictionary["HighestFPS"] = highestFPS.ToString();
		dictionary["HighestFPSDrawCalls"] = highestDC.ToString();
		dictionary["HighestFPSPlayerCount"] = highestPC.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "CustomMapPerformance",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.EnqueueTelemetryEvent("CustomMapPerformance", dictionary, null);
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x0011F924 File Offset: 0x0011DB24
	public static void PostCustomMapTracking(string mapName, long mapModId, string mapCreatorUsername, int minPlayers, int maxPlayers, int playtime, bool privateRoom)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		int num = playtime % 60;
		int num2 = (playtime - num) / 60;
		int num3 = num2 % 60;
		int num4 = (num2 - num3) / 60;
		string text = string.Format("{0}.{1}.{2}", num4, num3, num);
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapTrackingMetrics;
		dictionary["User"] = GorillaTelemetry.PlayFabUserId();
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["CustomMapCreator"] = mapCreatorUsername;
		dictionary["MinPlayerCount"] = minPlayers.ToString();
		dictionary["MaxPlayerCount"] = maxPlayers.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		dictionary["PrivateRoom"] = privateRoom.ToString();
		dictionary["PlaytimeOnMap"] = text;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "CustomMapTracking",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.EnqueueTelemetryEvent("CustomMapTracking", dictionary, null);
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x00002789 File Offset: 0x00000989
	public static void PostCustomMapDownloadEvent(string mapName, long mapModId, string mapCreatorUsername)
	{
	}

	// Token: 0x06003536 RID: 13622 RVA: 0x0011FA44 File Offset: 0x0011DC44
	public static void GhostReactorShiftStart(string gameId, int initialCores, float timeIntoShift, bool wasPlayerInAtStart, int numPlayers, int floorJoined, string playerRank)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorShiftStartArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorShiftStartArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorShiftStartArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["initial_cores_balance"] = initialCores.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["number_of_players"] = numPlayers.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["start_at_beginning"] = wasPlayerInAtStart.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["seconds_into_shift_at_join"] = timeIntoShift.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["floor_joined"] = floorJoined.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["player_rank"] = playerRank;
		GorillaTelemetry.gGhostReactorShiftStartArgs["is_private_room"] = NetworkSystem.Instance.SessionIsPrivate.ToString();
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_game_start",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorShiftStartArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_game_start";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("initial_cores_balance", initialCores.ToString());
		dictionary.Add("number_of_players", numPlayers.ToString());
		dictionary.Add("start_at_beginning", wasPlayerInAtStart.ToString());
		dictionary.Add("seconds_into_shift_at_join", timeIntoShift.ToString());
		dictionary.Add("floor_joined", floorJoined.ToString());
		dictionary.Add("player_rank", playerRank);
		dictionary.Add("is_private_room", NetworkSystem.Instance.SessionIsPrivate.ToString());
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003537 RID: 13623 RVA: 0x0011FC64 File Offset: 0x0011DE64
	public static void GhostReactorGameEnd(string gameId, int finalCores, int totalCoresCollectedByPlayer, int totalCoresCollectedByGroup, int totalCoresSpentByPlayer, int totalCoresSpentByGroup, int gatesUnlocked, int deaths, List<string> itemsPurchased, int shiftCut, bool isShiftActuallyEnding, float timeIntoShiftAtJoin, float playDuration, bool wasPlayerInAtStart, ZoneClearReason zoneClearReason, int maxNumberOfPlayersInShift, int endNumberOfPlayers, Dictionary<string, int> itemTypesHeldThisShift, int revives, int numShiftsPlayed)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorShiftEndArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorShiftEndArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorShiftEndArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["final_cores_balance"] = finalCores.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["total_cores_collected_by_player"] = totalCoresCollectedByPlayer.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["total_cores_collected_by_group"] = totalCoresCollectedByGroup.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["total_cores_spent_by_player"] = totalCoresSpentByPlayer.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["total_cores_spent_by_group"] = totalCoresSpentByGroup.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["gates_unlocked"] = gatesUnlocked.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["died"] = deaths.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["items_purchased"] = itemsPurchased.ToJson(true);
		GorillaTelemetry.gGhostReactorShiftEndArgs["shift_cut"] = shiftCut.ToJson(true);
		GorillaTelemetry.gGhostReactorShiftEndArgs["play_duration"] = playDuration.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["started_late"] = (!wasPlayerInAtStart).ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["time_started"] = timeIntoShiftAtJoin.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["revives"] = revives.ToString();
		string text = "shift_ended";
		if (!isShiftActuallyEnding)
		{
			if (zoneClearReason == ZoneClearReason.LeaveZone)
			{
				text = "left_zone";
			}
			else
			{
				text = "disconnect";
			}
		}
		GorillaTelemetry.gGhostReactorShiftEndArgs["reason"] = text;
		GorillaTelemetry.gGhostReactorShiftEndArgs["max_number_in_game"] = maxNumberOfPlayersInShift.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["end_number_in_game"] = endNumberOfPlayers.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["items_picked_up"] = itemTypesHeldThisShift.ToJson(true);
		GorillaTelemetry.gGhostReactorShiftEndArgs["num_shifts_played"] = numShiftsPlayed.ToString();
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_game_end",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorShiftEndArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_game_end";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("final_cores_balance", finalCores.ToString());
		dictionary.Add("total_cores_collected_by_player", totalCoresCollectedByPlayer.ToString());
		dictionary.Add("total_cores_collected_by_group", totalCoresCollectedByGroup.ToString());
		dictionary.Add("total_cores_spent_by_player", totalCoresSpentByPlayer.ToString());
		dictionary.Add("total_cores_spent_by_group", totalCoresSpentByGroup.ToString());
		dictionary.Add("gates_unlocked", gatesUnlocked.ToString());
		dictionary.Add("died", deaths.ToString());
		dictionary.Add("items_purchased", itemsPurchased.ToJson(true));
		dictionary.Add("shift_cut_data", shiftCut.ToJson(true));
		dictionary.Add("play_duration", playDuration.ToString());
		dictionary.Add("started_late", (!wasPlayerInAtStart).ToString());
		dictionary.Add("time_started", timeIntoShiftAtJoin.ToString());
		dictionary.Add("reason", text);
		dictionary.Add("max_number_in_game", maxNumberOfPlayersInShift.ToString());
		dictionary.Add("end_number_in_game", endNumberOfPlayers.ToString());
		dictionary.Add("items_picked_up", itemTypesHeldThisShift.ToJson(true));
		dictionary.Add("revives", revives.ToString());
		dictionary.Add("num_shifts_played", numShiftsPlayed.ToString());
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003538 RID: 13624 RVA: 0x00120054 File Offset: 0x0011E254
	public static void GhostReactorFloorStart(string gameId, int initialCores, float timeIntoShift, bool wasPlayerInAtStart, int numPlayers, string playerRank, int floor, string preset, string modifier)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorFloorStartArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorFloorStartArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorFloorStartArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorFloorStartArgs["initial_cores_balance"] = initialCores.ToString();
		GorillaTelemetry.gGhostReactorFloorStartArgs["number_of_players"] = numPlayers.ToString();
		GorillaTelemetry.gGhostReactorFloorStartArgs["start_at_beginning"] = wasPlayerInAtStart.ToString();
		GorillaTelemetry.gGhostReactorFloorStartArgs["seconds_into_shift_at_join"] = timeIntoShift.ToString();
		GorillaTelemetry.gGhostReactorFloorStartArgs["player_rank"] = playerRank;
		GorillaTelemetry.gGhostReactorFloorStartArgs["floor"] = floor.ToString();
		GorillaTelemetry.gGhostReactorFloorStartArgs["preset"] = preset.ToString();
		GorillaTelemetry.gGhostReactorFloorStartArgs["modifier"] = modifier.ToString();
		GorillaTelemetry.gGhostReactorFloorStartArgs["is_private_room"] = NetworkSystem.Instance.SessionIsPrivate.ToString();
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_floor_start",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorFloorStartArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_floor_start";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("initial_cores_balance", initialCores.ToString());
		dictionary.Add("number_of_players", numPlayers.ToString());
		dictionary.Add("start_at_beginning", wasPlayerInAtStart.ToString());
		dictionary.Add("seconds_into_shift_at_join", timeIntoShift.ToString());
		dictionary.Add("player_rank", playerRank);
		dictionary.Add("floor", floor.ToString());
		dictionary.Add("preset", preset.ToString());
		dictionary.Add("modifier", modifier.ToString());
		dictionary.Add("is_private_room", NetworkSystem.Instance.SessionIsPrivate.ToString());
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003539 RID: 13625 RVA: 0x001202C4 File Offset: 0x0011E4C4
	public static void GhostReactorFloorComplete(string gameId, int finalCores, int totalCoresCollectedByPlayer, int totalCoresCollectedByGroup, int totalCoresSpentByPlayer, int totalCoresSpentByGroup, int gatesUnlocked, int deaths, List<string> itemsPurchased, int shiftCut, bool isShiftActuallyEnding, float timeIntoShiftAtJoin, float playDuration, bool wasPlayerInAtStart, ZoneClearReason zoneClearReason, int maxNumberOfPlayersInShift, int endNumberOfPlayers, Dictionary<string, int> itemTypesHeldThisShift, int revives, int floor, string preset, string modifier, int chaosSeedsCollected, bool objectivesCompleted, string section, int xpGained)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorFloorEndArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorFloorEndArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorFloorEndArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["final_cores_balance"] = finalCores.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["total_cores_collected_by_player"] = totalCoresCollectedByPlayer.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["total_cores_collected_by_group"] = totalCoresCollectedByGroup.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["total_cores_spent_by_player"] = totalCoresSpentByPlayer.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["total_cores_spent_by_group"] = totalCoresSpentByGroup.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["gates_unlocked"] = gatesUnlocked.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["died"] = deaths.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["items_purchased"] = itemsPurchased.ToJson(true);
		GorillaTelemetry.gGhostReactorFloorEndArgs["shift_cut"] = shiftCut.ToJson(true);
		GorillaTelemetry.gGhostReactorFloorEndArgs["play_duration"] = playDuration.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["started_late"] = (!wasPlayerInAtStart).ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["time_started"] = timeIntoShiftAtJoin.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["revives"] = revives.ToString();
		string text = "shift_ended";
		if (!isShiftActuallyEnding)
		{
			if (zoneClearReason == ZoneClearReason.LeaveZone)
			{
				text = "left_zone";
			}
			else
			{
				text = "disconnect";
			}
		}
		GorillaTelemetry.gGhostReactorFloorEndArgs["reason"] = text;
		GorillaTelemetry.gGhostReactorFloorEndArgs["max_number_in_game"] = maxNumberOfPlayersInShift.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["end_number_in_game"] = endNumberOfPlayers.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["items_picked_up"] = itemTypesHeldThisShift.ToJson(true);
		GorillaTelemetry.gGhostReactorFloorEndArgs["floor"] = floor.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["preset"] = preset;
		GorillaTelemetry.gGhostReactorFloorEndArgs["modifier"] = modifier;
		GorillaTelemetry.gGhostReactorFloorEndArgs["section"] = section;
		GorillaTelemetry.gGhostReactorFloorEndArgs["xp_gained"] = xpGained.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["chaos_seeds_collected"] = chaosSeedsCollected.ToString();
		GorillaTelemetry.gGhostReactorFloorEndArgs["objectives_completed"] = objectivesCompleted.ToString();
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_floor_end",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorFloorEndArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_floor_end";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("final_cores_balance", finalCores.ToString());
		dictionary.Add("total_cores_collected_by_player", totalCoresCollectedByPlayer.ToString());
		dictionary.Add("total_cores_collected_by_group", totalCoresCollectedByGroup.ToString());
		dictionary.Add("total_cores_spent_by_player", totalCoresSpentByPlayer.ToString());
		dictionary.Add("total_cores_spent_by_group", totalCoresSpentByGroup.ToString());
		dictionary.Add("gates_unlocked", gatesUnlocked.ToString());
		dictionary.Add("died", deaths.ToString());
		dictionary.Add("items_purchased", itemsPurchased.ToJson(true));
		dictionary.Add("shift_cut_data", shiftCut.ToJson(true));
		dictionary.Add("play_duration", playDuration.ToString());
		dictionary.Add("started_late", (!wasPlayerInAtStart).ToString());
		dictionary.Add("time_started", timeIntoShiftAtJoin.ToString());
		dictionary.Add("reason", text);
		dictionary.Add("max_number_in_game", maxNumberOfPlayersInShift.ToString());
		dictionary.Add("end_number_in_game", endNumberOfPlayers.ToString());
		dictionary.Add("items_picked_up", itemTypesHeldThisShift.ToJson(true));
		dictionary.Add("revives", revives.ToString());
		dictionary.Add("floor", floor.ToString());
		dictionary.Add("preset", preset);
		dictionary.Add("modifier", modifier);
		dictionary.Add("chaos_seeds_collected", chaosSeedsCollected.ToString());
		dictionary.Add("objectives_completed", objectivesCompleted.ToString());
		dictionary.Add("section", section);
		dictionary.Add("xp_gained", xpGained.ToString());
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x0600353A RID: 13626 RVA: 0x00120784 File Offset: 0x0011E984
	public static void GhostReactorToolPurchased(string gameId, string toolName, int toolLevel, int coresSpent, int shinyRocksSpent, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["tool"] = toolName;
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["tool_level"] = toolLevel.ToString();
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["cores_spent"] = coresSpent.ToString();
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["floor"] = floor.ToString();
		GorillaTelemetry.gGhostReactorToolPurchasedArgs["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_tool_purchased",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorToolPurchasedArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_tool_purchased";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("tool", toolName);
		dictionary.Add("tool_level", toolLevel.ToString());
		dictionary.Add("cores_spent", coresSpent.ToString());
		dictionary.Add("shiny_rocks_spent", shinyRocksSpent.ToString());
		dictionary.Add("floor", floor.ToString());
		dictionary.Add("preset", preset);
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x0600353B RID: 13627 RVA: 0x00120958 File Offset: 0x0011EB58
	public static void GhostReactorRankUp(string gameId, string newRank, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorRankUpArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorRankUpArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorRankUpArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorRankUpArgs["new_rank"] = newRank;
		GorillaTelemetry.gGhostReactorRankUpArgs["floor"] = floor.ToString();
		GorillaTelemetry.gGhostReactorRankUpArgs["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_game_rank_up",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorRankUpArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_game_rank_up";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("new_rank", newRank);
		dictionary.Add("floor", floor.ToString());
		dictionary.Add("preset", preset);
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x0600353C RID: 13628 RVA: 0x00120AB4 File Offset: 0x0011ECB4
	public static void GhostReactorToolUnlock(string gameId, string toolName)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorToolUnlockArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorToolUnlockArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorToolUnlockArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorToolUnlockArgs["tool"] = toolName;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_game_tool_unlock",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorToolUnlockArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_game_tool_unlock";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("tool", toolName);
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x0600353D RID: 13629 RVA: 0x00120BCC File Offset: 0x0011EDCC
	public static void GhostReactorPodUpgradePurchased(string gameId, string toolName, int level, int shinyRocksSpent, int juiceSpent)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs["tool"] = toolName;
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs["new_level"] = level.ToString();
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs["juice_spent"] = juiceSpent.ToString();
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_pod_upgrade_purchased",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_pod_upgrade_purchased";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("tool", toolName);
		dictionary.Add("new_level", level.ToString());
		dictionary.Add("shiny_rocks_spent", shinyRocksSpent.ToString());
		dictionary.Add("juice_spent", juiceSpent.ToString());
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x0600353E RID: 13630 RVA: 0x00120D5C File Offset: 0x0011EF5C
	public static void GhostReactorToolUpgrade(string gameId, string upgradeType, string toolName, int newLevel, int juiceSpent, int griftSpent, int coresSpent, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["type"] = upgradeType;
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["tool"] = toolName;
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["new_level"] = newLevel.ToString();
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["juice_spent"] = juiceSpent.ToString();
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["grift_spent"] = griftSpent.ToString();
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["cores_spent"] = coresSpent.ToString();
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["floor"] = floor.ToString();
		GorillaTelemetry.gGhostReactorToolUpgradeArgs["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_game_tool_upgrade",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorToolUpgradeArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_game_tool_upgrade";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("type", upgradeType);
		dictionary.Add("tool", toolName);
		dictionary.Add("new_level", newLevel.ToString());
		dictionary.Add("juice_spent", juiceSpent.ToString());
		dictionary.Add("grift_spent", griftSpent.ToString());
		dictionary.Add("cores_spent", coresSpent.ToString());
		dictionary.Add("floor", floor.ToString());
		dictionary.Add("preset", preset);
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x0600353F RID: 13631 RVA: 0x00120F74 File Offset: 0x0011F174
	public static void GhostReactorChaosSeedStart(string gameId, string unlockTime, int chaosSeedsInQueue, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs["unlock_time"] = unlockTime;
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs["chaos_seeds_in_queue"] = chaosSeedsInQueue.ToString();
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs["floor"] = floor.ToString();
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_chaos_seed_start",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorChaosSeedStartArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_chaos_seed_start";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("unlock_time", unlockTime);
		dictionary.Add("chaos_seeds_in_queue", chaosSeedsInQueue.ToString());
		dictionary.Add("floor", floor.ToString());
		dictionary.Add("preset", preset);
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003540 RID: 13632 RVA: 0x001210F8 File Offset: 0x0011F2F8
	public static void GhostReactorChaosJuiceCollected(string gameId, int juiceCollected, int coresProcessedByOverdrive)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs["juice_collected"] = juiceCollected.ToString();
		GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs["cores_processed_by_overdrive"] = coresProcessedByOverdrive.ToString();
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_chaos_juice_collected",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_chaos_juice_collected";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("juice_collected", juiceCollected.ToString());
		dictionary.Add("cores_processed_by_overdrive", coresProcessedByOverdrive.ToString());
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003541 RID: 13633 RVA: 0x00121244 File Offset: 0x0011F444
	public static void GhostReactorOverdrivePurchased(string gameId, int shinyRocksUsed, int chaosSeedsInQueue, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs["shiny_rocks_used"] = shinyRocksUsed.ToString();
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs["chaos_seeds_in_queue"] = chaosSeedsInQueue.ToString();
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs["floor"] = floor.ToString();
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_overdrive_purchased",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_overdrive_purchased";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("shiny_rocks_used", shinyRocksUsed.ToString());
		dictionary.Add("chaos_seeds_in_queue", chaosSeedsInQueue.ToString());
		dictionary.Add("floor", floor.ToString());
		dictionary.Add("preset", preset);
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003542 RID: 13634 RVA: 0x001213D4 File Offset: 0x0011F5D4
	public static void GhostReactorCreditsRefillPurchased(string gameId, int shinyRocksSpent, int finalCredits, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs["final_credits"] = finalCredits.ToString();
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs["floor"] = floor.ToString();
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_credits_refill_purchased",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "ghost_credits_refill_purchased";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ghost_game_id", gameId);
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("shiny_rocks_spent", shinyRocksSpent.ToString());
		dictionary.Add("final_credits", finalCredits.ToString());
		dictionary.Add("floor", floor.ToString());
		dictionary.Add("preset", preset);
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003543 RID: 13635 RVA: 0x00121564 File Offset: 0x0011F764
	public static void SuperInfectionEvent(bool roomDisconnect, float totalPlayTime, float roomPlayTime, float sessionPlayTime, float intervalPlayTime, float terminalTotalTime, float terminalIntervalTime, Dictionary<SITechTreePageId, float> timeUsingGadgetsTotal, Dictionary<SITechTreePageId, float> timeUsingGadgetsInterval, float timeUsingOwnGadgetsTotal, float timeUsingOwnGadgetsInterval, float timeUsingOthersGadgetsTotal, float timeUsingOthersGadgetsInterval, Dictionary<SITechTreePageId, int> tagsUsingGadgetsTotal, Dictionary<SITechTreePageId, int> tagsUsingGadgetsInterval, int tagsHoldingOwnGadgetsTotal, int tagsHoldingOwnGadgetsInterval, int tagsHoldingOthersGadgetsTotal, int tagsHoldingOthersGadgetsInterval, Dictionary<SIResource.ResourceType, int> resourcesGatheredTotal, Dictionary<SIResource.ResourceType, int> resourcesGatheredInterval, int roundsPlayedTotal, int roundsPlayedInterval, bool[][] unlockedNodes, int numberOfPlayers)
	{
		if (!GorillaTelemetry.IsConnectedIgnoreRoom())
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < unlockedNodes.Length; i++)
		{
			num += unlockedNodes[i].Length;
		}
		char[] array = new char[num];
		num = 0;
		for (int j = 0; j < unlockedNodes.Length; j++)
		{
			for (int k = 0; k < unlockedNodes[j].Length; k++)
			{
				array[num] = (unlockedNodes[j][k] ? '1' : '0');
				num++;
			}
		}
		GorillaTelemetry.gSuperInfectionArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gSuperInfectionArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gSuperInfectionArgs["total_play_time"] = totalPlayTime.ToString();
		GorillaTelemetry.gSuperInfectionArgs["room_play_time"] = roomPlayTime.ToString();
		GorillaTelemetry.gSuperInfectionArgs["session_play_time"] = sessionPlayTime.ToString();
		GorillaTelemetry.gSuperInfectionArgs["interval_play_time"] = intervalPlayTime.ToString();
		GorillaTelemetry.gSuperInfectionArgs["terminal_total_time"] = terminalTotalTime.ToString();
		GorillaTelemetry.gSuperInfectionArgs["terminal_interval_time"] = terminalIntervalTime.ToString();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		for (int l = 0; l < 11; l++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)l;
			float num2;
			timeUsingGadgetsTotal.TryGetValue(sitechTreePageId, ref num2);
			float num3;
			timeUsingGadgetsInterval.TryGetValue(sitechTreePageId, ref num3);
			int num4;
			tagsUsingGadgetsTotal.TryGetValue(sitechTreePageId, ref num4);
			int num5;
			tagsUsingGadgetsInterval.TryGetValue(sitechTreePageId, ref num5);
			string text = sitechTreePageId.ToString();
			dictionary[text] = num2.ToString();
			dictionary2[text] = num3.ToString();
			dictionary3[text] = num4.ToString();
			dictionary4[text] = num5.ToString();
		}
		Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
		Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
		for (int m = 0; m < 6; m++)
		{
			SIResource.ResourceType resourceType = (SIResource.ResourceType)m;
			int num6;
			resourcesGatheredTotal.TryGetValue(resourceType, ref num6);
			int num7;
			resourcesGatheredInterval.TryGetValue(resourceType, ref num7);
			string text2 = resourceType.ToString();
			dictionary5[text2] = num6.ToString();
			dictionary6[text2] = num7.ToString();
		}
		GorillaTelemetry.gSuperInfectionArgs["time_holding_gadget_type_total"] = dictionary;
		GorillaTelemetry.gSuperInfectionArgs["time_holding_gadget_type_interval"] = dictionary2;
		GorillaTelemetry.gSuperInfectionArgs["time_holding_own_gadgets_total"] = timeUsingOwnGadgetsTotal.ToString();
		GorillaTelemetry.gSuperInfectionArgs["time_holding_own_gadgets_interval"] = timeUsingOwnGadgetsInterval.ToString();
		GorillaTelemetry.gSuperInfectionArgs["time_holding_others_gadgets_total"] = timeUsingOthersGadgetsTotal.ToString();
		GorillaTelemetry.gSuperInfectionArgs["time_holding_others_gadgets_interval"] = timeUsingOthersGadgetsInterval.ToString();
		GorillaTelemetry.gSuperInfectionArgs["tags_holding_gadget_type_total"] = dictionary3;
		GorillaTelemetry.gSuperInfectionArgs["tags_holding_gadget_type_interval"] = dictionary4;
		GorillaTelemetry.gSuperInfectionArgs["tags_holding_own_gadgets_total"] = tagsHoldingOwnGadgetsTotal.ToString();
		GorillaTelemetry.gSuperInfectionArgs["tags_holding_own_gadgets_interval"] = tagsHoldingOwnGadgetsInterval.ToString();
		GorillaTelemetry.gSuperInfectionArgs["tags_holding_others_gadgets_total"] = tagsHoldingOthersGadgetsTotal.ToString();
		GorillaTelemetry.gSuperInfectionArgs["tags_holding_others_gadgets_interval"] = tagsHoldingOthersGadgetsInterval.ToString();
		GorillaTelemetry.gSuperInfectionArgs["resource_collected_total"] = dictionary5;
		GorillaTelemetry.gSuperInfectionArgs["resource_collected_interval"] = dictionary6;
		GorillaTelemetry.gSuperInfectionArgs["rounds_played_total"] = roundsPlayedTotal.ToString();
		GorillaTelemetry.gSuperInfectionArgs["rounds_played_interval"] = roundsPlayedInterval.ToString();
		GorillaTelemetry.gSuperInfectionArgs["unlocked_nodes"] = new string(array);
		GorillaTelemetry.gSuperInfectionArgs["number_of_players"] = numberOfPlayers.ToString();
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = (roomDisconnect ? "super_infection_room_disconnect" : "super_infection_interval"),
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gSuperInfectionArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = (roomDisconnect ? "super_infection_room_left" : "super_infection_interval");
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
		dictionary7.Add("event_timestamp", DateTime.Now.ToString());
		dictionary7.Add("total_play_time", totalPlayTime.ToString());
		dictionary7.Add("room_play_time", roomPlayTime.ToString());
		dictionary7.Add("session_play_time", sessionPlayTime.ToString());
		dictionary7.Add("interval_play_time", intervalPlayTime.ToString());
		dictionary7.Add("terminal_total_time", terminalTotalTime.ToString());
		dictionary7.Add("terminal_interval_time", terminalIntervalTime.ToString());
		dictionary7.Add("time_holding_gadget_type_total", timeUsingGadgetsTotal);
		dictionary7.Add("time_holding_gadget_type_interval", timeUsingGadgetsInterval);
		dictionary7.Add("time_holding_own_gadgets_total", timeUsingOwnGadgetsTotal.ToString());
		dictionary7.Add("time_holding_own_gadgets_interval", timeUsingOwnGadgetsInterval.ToString());
		dictionary7.Add("time_holding_others_gadgets_total", timeUsingOthersGadgetsTotal.ToString());
		dictionary7.Add("time_holding_others_gadgets_interval", timeUsingOthersGadgetsInterval.ToString());
		dictionary7.Add("tags_holding_gadget_type_total", dictionary3);
		dictionary7.Add("tags_holding_gadget_type_interval", dictionary4);
		dictionary7.Add("tags_holding_own_gadgets_total", tagsHoldingOwnGadgetsTotal.ToString());
		dictionary7.Add("tags_holding_own_gadgets_interval", tagsHoldingOwnGadgetsInterval.ToString());
		dictionary7.Add("tags_holding_others_gadgets_total", tagsHoldingOthersGadgetsTotal.ToString());
		dictionary7.Add("tags_holding_others_gadgets_interval", tagsHoldingOthersGadgetsInterval.ToString());
		dictionary7.Add("resource_type_collected_total", dictionary5);
		dictionary7.Add("resource_type_collected_interval", dictionary6);
		dictionary7.Add("rounds_played_total", roundsPlayedTotal.ToString());
		dictionary7.Add("rounds_played_interval", roundsPlayedInterval.ToString());
		dictionary7.Add("unlocked_nodes", new string(array));
		dictionary7.Add("player_count", numberOfPlayers.ToString());
		ghostReactorTelemetryData.BodyData = dictionary7;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003544 RID: 13636 RVA: 0x00121B60 File Offset: 0x0011FD60
	public static void SuperInfectionEvent(string purchaseType, int shinyRockCost, int techPointsPurchased, float totalPlayTime, float roomPlayTime, float sessionPlayTime)
	{
		if (!GorillaTelemetry.IsConnectedIgnoreRoom())
		{
			return;
		}
		GorillaTelemetry.gSuperInfectionArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gSuperInfectionArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gSuperInfectionArgs["total_play_time"] = totalPlayTime.ToString();
		GorillaTelemetry.gSuperInfectionArgs["room_play_time"] = roomPlayTime.ToString();
		GorillaTelemetry.gSuperInfectionArgs["session_play_time"] = sessionPlayTime.ToString();
		GorillaTelemetry.gSuperInfectionArgs["si_purchase_type"] = purchaseType;
		GorillaTelemetry.gSuperInfectionArgs["si_shiny_rock_cost"] = shinyRockCost;
		GorillaTelemetry.gSuperInfectionArgs["si_tech_points_purchased"] = techPointsPurchased;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "super_infection_purchase",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gSuperInfectionArgs
		});
		GhostReactorTelemetryData ghostReactorTelemetryData = default(GhostReactorTelemetryData);
		ghostReactorTelemetryData.EventName = "super_infection_purchase";
		ghostReactorTelemetryData.CustomTags = new string[]
		{
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("event_timestamp", DateTime.Now.ToString());
		dictionary.Add("total_play_time", totalPlayTime.ToString());
		dictionary.Add("room_play_time", roomPlayTime.ToString());
		dictionary.Add("session_play_time", sessionPlayTime.ToString());
		dictionary.Add("si_purchase_type", purchaseType.ToString());
		dictionary.Add("si_shiny_rock_cost", shinyRockCost.ToString());
		dictionary.Add("si_tech_points_purchased", techPointsPurchased.ToString());
		ghostReactorTelemetryData.BodyData = dictionary;
		GhostReactorTelemetryData ghostReactorTelemetryData2 = ghostReactorTelemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData2.EventName, ghostReactorTelemetryData2.BodyData, ghostReactorTelemetryData2.CustomTags);
	}

	// Token: 0x06003545 RID: 13637 RVA: 0x00121D28 File Offset: 0x0011FF28
	public static void PostNotificationEvent(string notificationType)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		Dictionary<string, object> dictionary = GorillaTelemetry.gNotifEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = notificationType;
		GorillaTelemetry.EnqueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_ggwp_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_ggwp_event", dictionary, null);
	}

	// Token: 0x040043C9 RID: 17353
	private static readonly float TELEMETRY_FLUSH_SEC = 10f;

	// Token: 0x040043CA RID: 17354
	private static readonly ConcurrentQueue<EventContents> telemetryEventsQueuePlayFab = new ConcurrentQueue<EventContents>();

	// Token: 0x040043CB RID: 17355
	private static readonly ConcurrentQueue<MothershipAnalyticsEvent> telemetryEventsQueueMothership = new ConcurrentQueue<MothershipAnalyticsEvent>();

	// Token: 0x040043CC RID: 17356
	private static readonly Dictionary<int, List<EventContents>> gListPoolPlayFab = new Dictionary<int, List<EventContents>>();

	// Token: 0x040043CD RID: 17357
	private static readonly Dictionary<int, List<MothershipAnalyticsEvent>> gListPoolMothership = new Dictionary<int, List<MothershipAnalyticsEvent>>();

	// Token: 0x040043CE RID: 17358
	private static readonly string namespacePrefix = "custom";

	// Token: 0x040043CF RID: 17359
	private static readonly string EVENT_NAMESPACE = GorillaTelemetry.namespacePrefix + "." + PlayFabAuthenticatorSettings.TitleId;

	// Token: 0x040043D0 RID: 17360
	private static PlayFabAuthenticator gPlayFabAuth;

	// Token: 0x040043D1 RID: 17361
	private static readonly Dictionary<string, object> gZoneEventArgs;

	// Token: 0x040043D2 RID: 17362
	private static readonly Dictionary<string, object> gNotifEventArgs;

	// Token: 0x040043D3 RID: 17363
	public static float nextStayTimestamp;

	// Token: 0x040043D4 RID: 17364
	private static readonly Dictionary<string, object> gGameModeStartEventArgs;

	// Token: 0x040043D5 RID: 17365
	private static readonly Dictionary<string, object> gShopEventArgs;

	// Token: 0x040043D6 RID: 17366
	private static CosmeticsController.CosmeticItem[] gSingleItemParam;

	// Token: 0x040043D7 RID: 17367
	private static BuilderSetManager.BuilderSetStoreItem[] gSingleItemBuilderParam;

	// Token: 0x040043D8 RID: 17368
	private static Dictionary<string, object> gKidEventArgs;

	// Token: 0x040043D9 RID: 17369
	private static readonly Dictionary<string, object> gWamGameStartArgs;

	// Token: 0x040043DA RID: 17370
	private static readonly Dictionary<string, object> gWamLevelEndArgs;

	// Token: 0x040043DB RID: 17371
	private static Dictionary<string, object> gCustomMapPerfArgs;

	// Token: 0x040043DC RID: 17372
	private static Dictionary<string, object> gCustomMapTrackingMetrics;

	// Token: 0x040043DD RID: 17373
	private static Dictionary<string, object> gCustomMapDownloadMetrics;

	// Token: 0x040043DE RID: 17374
	private static readonly Dictionary<string, object> gGhostReactorShiftStartArgs;

	// Token: 0x040043DF RID: 17375
	private static readonly Dictionary<string, object> gGhostReactorShiftEndArgs;

	// Token: 0x040043E0 RID: 17376
	private static readonly Dictionary<string, object> gGhostReactorFloorStartArgs;

	// Token: 0x040043E1 RID: 17377
	private static readonly Dictionary<string, object> gGhostReactorFloorEndArgs;

	// Token: 0x040043E2 RID: 17378
	private static readonly Dictionary<string, object> gGhostReactorToolPurchasedArgs;

	// Token: 0x040043E3 RID: 17379
	private static readonly Dictionary<string, object> gGhostReactorRankUpArgs;

	// Token: 0x040043E4 RID: 17380
	private static readonly Dictionary<string, object> gGhostReactorToolUnlockArgs;

	// Token: 0x040043E5 RID: 17381
	private static readonly Dictionary<string, object> gGhostReactorPodUpgradePurchasedArgs;

	// Token: 0x040043E6 RID: 17382
	private static readonly Dictionary<string, object> gGhostReactorToolUpgradeArgs;

	// Token: 0x040043E7 RID: 17383
	private static readonly Dictionary<string, object> gGhostReactorChaosSeedStartArgs;

	// Token: 0x040043E8 RID: 17384
	private static readonly Dictionary<string, object> gGhostReactorChaosJuiceCollectedArgs;

	// Token: 0x040043E9 RID: 17385
	private static readonly Dictionary<string, object> gGhostReactorOverdrivePurchasedArgs;

	// Token: 0x040043EA RID: 17386
	private static readonly Dictionary<string, object> gGhostReactorCreditsRefillPurchasedArgs;

	// Token: 0x040043EB RID: 17387
	private static readonly Dictionary<string, object> gSuperInfectionArgs;

	// Token: 0x020007E5 RID: 2021
	public static class k
	{
		// Token: 0x040043EC RID: 17388
		public const string User = "User";

		// Token: 0x040043ED RID: 17389
		public const string ZoneId = "ZoneId";

		// Token: 0x040043EE RID: 17390
		public const string SubZoneId = "SubZoneId";

		// Token: 0x040043EF RID: 17391
		public const string EventType = "EventType";

		// Token: 0x040043F0 RID: 17392
		public const string IsPrivateRoom = "IsPrivateRoom";

		// Token: 0x040043F1 RID: 17393
		public const string Items = "Items";

		// Token: 0x040043F2 RID: 17394
		public const string VoiceChatEnabled = "VoiceChatEnabled";

		// Token: 0x040043F3 RID: 17395
		public const string JoinGroups = "JoinGroups";

		// Token: 0x040043F4 RID: 17396
		public const string CustomUsernameEnabled = "CustomUsernameEnabled";

		// Token: 0x040043F5 RID: 17397
		public const string AgeCategory = "AgeCategory";

		// Token: 0x040043F6 RID: 17398
		public const string telemetry_zone_event = "telemetry_zone_event";

		// Token: 0x040043F7 RID: 17399
		public const string telemetry_shop_event = "telemetry_shop_event";

		// Token: 0x040043F8 RID: 17400
		public const string telemetry_kid_event = "telemetry_kid_event";

		// Token: 0x040043F9 RID: 17401
		public const string telemetry_ggwp_event = "telemetry_ggwp_event";

		// Token: 0x040043FA RID: 17402
		public const string NOTHING = "NOTHING";

		// Token: 0x040043FB RID: 17403
		public const string telemetry_wam_gameStartEvent = "telemetry_wam_gameStartEvent";

		// Token: 0x040043FC RID: 17404
		public const string telemetry_wam_levelEndEvent = "telemetry_wam_levelEndEvent";

		// Token: 0x040043FD RID: 17405
		public const string WamMachineId = "WamMachineId";

		// Token: 0x040043FE RID: 17406
		public const string WamGameId = "WamGameId";

		// Token: 0x040043FF RID: 17407
		public const string WamMLevelNumber = "WamMLevelNumber";

		// Token: 0x04004400 RID: 17408
		public const string WamGoodMolesShown = "WamGoodMolesShown";

		// Token: 0x04004401 RID: 17409
		public const string WamHazardMolesShown = "WamHazardMolesShown";

		// Token: 0x04004402 RID: 17410
		public const string WamLevelMinScore = "WamLevelMinScore";

		// Token: 0x04004403 RID: 17411
		public const string WamLevelScore = "WamLevelScore";

		// Token: 0x04004404 RID: 17412
		public const string WamHazardMolesHit = "WamHazardMolesHit";

		// Token: 0x04004405 RID: 17413
		public const string WamGameState = "WamGameState";

		// Token: 0x04004406 RID: 17414
		public const string CustomMapName = "CustomMapName";

		// Token: 0x04004407 RID: 17415
		public const string LowestFPS = "LowestFPS";

		// Token: 0x04004408 RID: 17416
		public const string LowestFPSDrawCalls = "LowestFPSDrawCalls";

		// Token: 0x04004409 RID: 17417
		public const string LowestFPSPlayerCount = "LowestFPSPlayerCount";

		// Token: 0x0400440A RID: 17418
		public const string AverageFPS = "AverageFPS";

		// Token: 0x0400440B RID: 17419
		public const string AverageDrawCalls = "AverageDrawCalls";

		// Token: 0x0400440C RID: 17420
		public const string AveragePlayerCount = "AveragePlayerCount";

		// Token: 0x0400440D RID: 17421
		public const string HighestFPS = "HighestFPS";

		// Token: 0x0400440E RID: 17422
		public const string HighestFPSDrawCalls = "HighestFPSDrawCalls";

		// Token: 0x0400440F RID: 17423
		public const string HighestFPSPlayerCount = "HighestFPSPlayerCount";

		// Token: 0x04004410 RID: 17424
		public const string CustomMapCreator = "CustomMapCreator";

		// Token: 0x04004411 RID: 17425
		public const string CustomMapModId = "CustomMapModId";

		// Token: 0x04004412 RID: 17426
		public const string MinPlayerCount = "MinPlayerCount";

		// Token: 0x04004413 RID: 17427
		public const string MaxPlayerCount = "MaxPlayerCount";

		// Token: 0x04004414 RID: 17428
		public const string PlaytimeOnMap = "PlaytimeOnMap";

		// Token: 0x04004415 RID: 17429
		public const string PlaytimeInSeconds = "PlaytimeInSeconds";

		// Token: 0x04004416 RID: 17430
		public const string PrivateRoom = "PrivateRoom";

		// Token: 0x04004417 RID: 17431
		public const string ghost_game_start = "ghost_game_start";

		// Token: 0x04004418 RID: 17432
		public const string ghost_game_end = "ghost_game_end";

		// Token: 0x04004419 RID: 17433
		public const string ghost_floor_start = "ghost_floor_start";

		// Token: 0x0400441A RID: 17434
		public const string ghost_floor_end = "ghost_floor_end";

		// Token: 0x0400441B RID: 17435
		public const string ghost_tool_purchased = "ghost_tool_purchased";

		// Token: 0x0400441C RID: 17436
		public const string ghost_game_rank_up = "ghost_game_rank_up";

		// Token: 0x0400441D RID: 17437
		public const string ghost_game_tool_unlock = "ghost_game_tool_unlock";

		// Token: 0x0400441E RID: 17438
		public const string ghost_pod_upgrade_purchased = "ghost_pod_upgrade_purchased";

		// Token: 0x0400441F RID: 17439
		public const string ghost_game_tool_upgrade = "ghost_game_tool_upgrade";

		// Token: 0x04004420 RID: 17440
		public const string ghost_chaos_seed_start = "ghost_chaos_seed_start";

		// Token: 0x04004421 RID: 17441
		public const string ghost_chaos_juice_collected = "ghost_chaos_juice_collected";

		// Token: 0x04004422 RID: 17442
		public const string ghost_overdrive_purchased = "ghost_overdrive_purchased";

		// Token: 0x04004423 RID: 17443
		public const string ghost_credits_refill_purchased = "ghost_credits_refill_purchased";

		// Token: 0x04004424 RID: 17444
		public const string ghost_game_id = "ghost_game_id";

		// Token: 0x04004425 RID: 17445
		public const string event_timestamp = "event_timestamp";

		// Token: 0x04004426 RID: 17446
		public const string initial_cores_balance = "initial_cores_balance";

		// Token: 0x04004427 RID: 17447
		public const string final_cores_balance = "final_cores_balance";

		// Token: 0x04004428 RID: 17448
		public const string cores_spent_waiting_in_breakroom = "cores_spent_waiting_in_breakroom";

		// Token: 0x04004429 RID: 17449
		public const string cores_collected = "cores_collected";

		// Token: 0x0400442A RID: 17450
		public const string cores_collected_from_ghosts = "cores_collected_from_ghosts";

		// Token: 0x0400442B RID: 17451
		public const string cores_collected_from_gathering = "cores_collected_from_gathering";

		// Token: 0x0400442C RID: 17452
		public const string cores_spent_on_items = "cores_spent_on_items";

		// Token: 0x0400442D RID: 17453
		public const string cores_spent_on_gates = "cores_spent_on_gates";

		// Token: 0x0400442E RID: 17454
		public const string cores_spent_on_levels = "cores_spent_on_levels";

		// Token: 0x0400442F RID: 17455
		public const string cores_given_to_others = "cores_given_to_others";

		// Token: 0x04004430 RID: 17456
		public const string cores_received_from_others = "cores_received_from_others";

		// Token: 0x04004431 RID: 17457
		public const string gates_unlocked = "gates_unlocked";

		// Token: 0x04004432 RID: 17458
		public const string died = "died";

		// Token: 0x04004433 RID: 17459
		public const string caught_in_anamole = "caught_in_anamole";

		// Token: 0x04004434 RID: 17460
		public const string items_purchased = "items_purchased";

		// Token: 0x04004435 RID: 17461
		public const string levels_unlocked = "levels_unlocked";

		// Token: 0x04004436 RID: 17462
		public const string shift_cut = "shift_cut";

		// Token: 0x04004437 RID: 17463
		public const string number_of_players = "number_of_players";

		// Token: 0x04004438 RID: 17464
		public const string start_at_beginning = "start_at_beginning";

		// Token: 0x04004439 RID: 17465
		public const string seconds_into_shift_at_join = "seconds_into_shift_at_join";

		// Token: 0x0400443A RID: 17466
		public const string reason = "reason";

		// Token: 0x0400443B RID: 17467
		public const string play_duration = "play_duration";

		// Token: 0x0400443C RID: 17468
		public const string started_late = "started_late";

		// Token: 0x0400443D RID: 17469
		public const string time_started = "time_started";

		// Token: 0x0400443E RID: 17470
		public const string max_number_in_game = "max_number_in_game";

		// Token: 0x0400443F RID: 17471
		public const string end_number_in_game = "end_number_in_game";

		// Token: 0x04004440 RID: 17472
		public const string items_picked_up = "items_picked_up";

		// Token: 0x04004441 RID: 17473
		public const string super_infection_room_disconnect = "super_infection_room_disconnect";

		// Token: 0x04004442 RID: 17474
		public const string super_infection_interval = "super_infection_interval";

		// Token: 0x04004443 RID: 17475
		public const string super_infection_purchase = "super_infection_purchase";

		// Token: 0x04004444 RID: 17476
		public const string si_purchase_type = "si_purchase_type";

		// Token: 0x04004445 RID: 17477
		public const string si_shiny_rock_cost = "si_shiny_rock_cost";

		// Token: 0x04004446 RID: 17478
		public const string si_tech_points_purchased = "si_tech_points_purchased";

		// Token: 0x04004447 RID: 17479
		public const string total_play_time = "total_play_time";

		// Token: 0x04004448 RID: 17480
		public const string room_play_time = "room_play_time";

		// Token: 0x04004449 RID: 17481
		public const string session_play_time = "session_play_time";

		// Token: 0x0400444A RID: 17482
		public const string interval_play_time = "interval_play_time";

		// Token: 0x0400444B RID: 17483
		public const string terminal_total_time = "terminal_total_time";

		// Token: 0x0400444C RID: 17484
		public const string terminal_interval_time = "terminal_interval_time";

		// Token: 0x0400444D RID: 17485
		public const string time_holding_gadget_type_total = "time_holding_gadget_type_total";

		// Token: 0x0400444E RID: 17486
		public const string time_holding_gadget_type_interval = "time_holding_gadget_type_interval";

		// Token: 0x0400444F RID: 17487
		public const string time_holding_own_gadgets_total = "time_holding_own_gadgets_total";

		// Token: 0x04004450 RID: 17488
		public const string time_holding_own_gadgets_interval = "time_holding_own_gadgets_interval";

		// Token: 0x04004451 RID: 17489
		public const string time_holding_others_gadgets_total = "time_holding_others_gadgets_total";

		// Token: 0x04004452 RID: 17490
		public const string time_holding_others_gadgets_interval = "time_holding_others_gadgets_interval";

		// Token: 0x04004453 RID: 17491
		public const string tags_holding_gadget_type_total = "tags_holding_gadget_type_total";

		// Token: 0x04004454 RID: 17492
		public const string tags_holding_gadget_type_interval = "tags_holding_gadget_type_interval";

		// Token: 0x04004455 RID: 17493
		public const string tags_holding_own_gadgets_total = "tags_holding_own_gadgets_total";

		// Token: 0x04004456 RID: 17494
		public const string tags_holding_own_gadgets_interval = "tags_holding_own_gadgets_interval";

		// Token: 0x04004457 RID: 17495
		public const string tags_holding_others_gadgets_total = "tags_holding_others_gadgets_total";

		// Token: 0x04004458 RID: 17496
		public const string tags_holding_others_gadgets_interval = "tags_holding_others_gadgets_interval";

		// Token: 0x04004459 RID: 17497
		public const string resource_collected_total = "resource_collected_total";

		// Token: 0x0400445A RID: 17498
		public const string resource_collected_interval = "resource_collected_interval";

		// Token: 0x0400445B RID: 17499
		public const string rounds_played_total = "rounds_played_total";

		// Token: 0x0400445C RID: 17500
		public const string rounds_played_interval = "rounds_played_interval";

		// Token: 0x0400445D RID: 17501
		public const string unlocked_nodes = "unlocked_nodes";

		// Token: 0x0400445E RID: 17502
		public const string floor_joined = "floor_joined";

		// Token: 0x0400445F RID: 17503
		public const string player_rank = "player_rank";

		// Token: 0x04004460 RID: 17504
		public const string total_cores_collected_by_player = "total_cores_collected_by_player";

		// Token: 0x04004461 RID: 17505
		public const string total_cores_collected_by_group = "total_cores_collected_by_group";

		// Token: 0x04004462 RID: 17506
		public const string total_cores_spent_by_player = "total_cores_spent_by_player";

		// Token: 0x04004463 RID: 17507
		public const string total_cores_spent_by_group = "total_cores_spent_by_group";

		// Token: 0x04004464 RID: 17508
		public const string floor = "floor";

		// Token: 0x04004465 RID: 17509
		public const string preset = "preset";

		// Token: 0x04004466 RID: 17510
		public const string modifier = "modifier";

		// Token: 0x04004467 RID: 17511
		public const string section = "section";

		// Token: 0x04004468 RID: 17512
		public const string xp_gained = "xp_gained";

		// Token: 0x04004469 RID: 17513
		public const string chaos_seeds_collected = "chaos_seeds_collected";

		// Token: 0x0400446A RID: 17514
		public const string objectives_completed = "objectives_completed";

		// Token: 0x0400446B RID: 17515
		public const string revives = "revives";

		// Token: 0x0400446C RID: 17516
		public const string tool = "tool";

		// Token: 0x0400446D RID: 17517
		public const string tool_level = "tool_level";

		// Token: 0x0400446E RID: 17518
		public const string cores_spent = "cores_spent";

		// Token: 0x0400446F RID: 17519
		public const string shiny_rocks_spent = "shiny_rocks_spent";

		// Token: 0x04004470 RID: 17520
		public const string new_rank = "new_rank";

		// Token: 0x04004471 RID: 17521
		public const string upgrade = "upgrade";

		// Token: 0x04004472 RID: 17522
		public const string grift_price = "grift_price";

		// Token: 0x04004473 RID: 17523
		public const string type = "type";

		// Token: 0x04004474 RID: 17524
		public const string new_level = "new_level";

		// Token: 0x04004475 RID: 17525
		public const string juice_spent = "juice_spent";

		// Token: 0x04004476 RID: 17526
		public const string grift_spent = "grift_spent";

		// Token: 0x04004477 RID: 17527
		public const string chaos_seeds_in_queue = "chaos_seeds_in_queue";

		// Token: 0x04004478 RID: 17528
		public const string unlock_time = "unlock_time";

		// Token: 0x04004479 RID: 17529
		public const string shiny_rocks_used = "shiny_rocks_used";

		// Token: 0x0400447A RID: 17530
		public const string juice_collected = "juice_collected";

		// Token: 0x0400447B RID: 17531
		public const string cores_processed_by_overdrive = "cores_processed_by_overdrive";

		// Token: 0x0400447C RID: 17532
		public const string final_credits = "final_credits";

		// Token: 0x0400447D RID: 17533
		public const string is_private_room = "is_private_room";

		// Token: 0x0400447E RID: 17534
		public const string num_shifts_played = "num_shifts_played";

		// Token: 0x0400447F RID: 17535
		public const string game_mode_played_event = "game_mode_played_event";

		// Token: 0x04004480 RID: 17536
		public const string game_mode = "game_mode";
	}

	// Token: 0x020007E6 RID: 2022
	private class BatchRunner : MonoBehaviour
	{
		// Token: 0x06003546 RID: 13638 RVA: 0x00121D94 File Offset: 0x0011FF94
		private IEnumerator Start()
		{
			for (;;)
			{
				float start = Time.time;
				while (Time.time < start + GorillaTelemetry.TELEMETRY_FLUSH_SEC)
				{
					yield return null;
				}
				GorillaTelemetry.FlushPlayFabTelemetry();
				GorillaTelemetry.FlushMothershipTelemetry();
			}
			yield break;
		}
	}
}
