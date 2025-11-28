using System;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class SuperInfectionTelemetry : MonoBehaviour
{
	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000978 RID: 2424 RVA: 0x00033213 File Offset: 0x00031413
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06000979 RID: 2425 RVA: 0x00033224 File Offset: 0x00031424
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x04000B6C RID: 2924
	public const string ROOM_LEFT_EVENT_NAME = "super_infection_room_left";

	// Token: 0x04000B6D RID: 2925
	public const string INTERVAL_EVENT_NAME = "super_infection_interval";

	// Token: 0x04000B6E RID: 2926
	public const string SI_PURCHASE_EVENT_NAME = "super_infection_purchase";

	// Token: 0x04000B6F RID: 2927
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04000B70 RID: 2928
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x04000B71 RID: 2929
	public const string SUPER_INFECTION_PREPEND = "super_infection_";

	// Token: 0x04000B72 RID: 2930
	public const string SUPER_INFECTION_GAME_ID_BODY_DATA = "super_infection_round_id";

	// Token: 0x04000B73 RID: 2931
	public const string EVENT_TIMESTAMP_BODY_DATA = "event_timestamp";

	// Token: 0x04000B74 RID: 2932
	public const string TOTAL_PLAY_TIME_BODY_DATA = "total_play_time";

	// Token: 0x04000B75 RID: 2933
	public const string ROOM_PLAY_TIME_BODY_DATA = "room_play_time";

	// Token: 0x04000B76 RID: 2934
	public const string SESSION_PLAY_TIME_BODY_DATA = "session_play_time";

	// Token: 0x04000B77 RID: 2935
	public const string INTERVAL_PLAY_TIME_BODY_DATA = "interval_play_time";

	// Token: 0x04000B78 RID: 2936
	public const string TERMINAL_TOTAL_TIME_BODY_DATA = "terminal_total_time";

	// Token: 0x04000B79 RID: 2937
	public const string TERMINAL_INTERVAL_TIME_BODY_DATA = "terminal_interval_time";

	// Token: 0x04000B7A RID: 2938
	public const string TIME_USING_GADGET_TYPE_TOTAL_BODY_DATA = "time_holding_gadget_type_total";

	// Token: 0x04000B7B RID: 2939
	public const string TIME_USING_GADGET_TYPE_INTERVAL_BODY_DATA = "time_holding_gadget_type_interval";

	// Token: 0x04000B7C RID: 2940
	public const string TIME_HOLDING_OWN_GADGETS_TOTAL_BODY_DATA = "time_holding_own_gadgets_total";

	// Token: 0x04000B7D RID: 2941
	public const string TIME_HOLDING_OWN_GADGETS_INTERVAL_BODY_DATA = "time_holding_own_gadgets_interval";

	// Token: 0x04000B7E RID: 2942
	public const string TIME_HOLDING_OTHERS_GADGETS_TOTAL_BODY_DATA = "time_holding_others_gadgets_total";

	// Token: 0x04000B7F RID: 2943
	public const string TIME_HOLDING_OTHERS_GADGETS_INTERVAL_BODY_DATA = "time_holding_others_gadgets_interval";

	// Token: 0x04000B80 RID: 2944
	public const string TAGS_HOLDING_GADGET_TYPE_TOTAL_BODY_DATA = "tags_holding_gadget_type_total";

	// Token: 0x04000B81 RID: 2945
	public const string TAGS_HOLDING_GADGET_TYPE_INTERVAL_BODY_DATA = "tags_holding_gadget_type_interval";

	// Token: 0x04000B82 RID: 2946
	public const string TAGS_HOLDING_OWN_GADGETS_TOTAL_BODY_DATA = "tags_holding_own_gadgets_total";

	// Token: 0x04000B83 RID: 2947
	public const string TAGS_HOLDING_OWN_GADGETS_INTERVAL_BODY_DATA = "tags_holding_own_gadgets_interval";

	// Token: 0x04000B84 RID: 2948
	public const string TAGS_HOLDING_OTHERS_GADGETS_TOTAL_BODY_DATA = "tags_holding_others_gadgets_total";

	// Token: 0x04000B85 RID: 2949
	public const string TAGS_HOLDING_OTHERS_GADGETS_INTERVAL_BODY_DATA = "tags_holding_others_gadgets_interval";

	// Token: 0x04000B86 RID: 2950
	public const string RESOURCE_TYPE_COLLECTED_TOTAL_BODY_DATA = "resource_type_collected_total";

	// Token: 0x04000B87 RID: 2951
	public const string RESOURCE_TYPE_COLLECTED_INTERVAL_BODY_DATA = "resource_type_collected_interval";

	// Token: 0x04000B88 RID: 2952
	public const string ROUNDS_PLAYED_TOTAL_BODY_DATA = "rounds_played_total";

	// Token: 0x04000B89 RID: 2953
	public const string ROUNDS_PLAYED_INTERVAL_BODY_DATA = "rounds_played_interval";

	// Token: 0x04000B8A RID: 2954
	public const string UNLOCKED_NODES_BODY_DATA = "unlocked_nodes";

	// Token: 0x04000B8B RID: 2955
	public const string PLAYER_COUNT_BODY_DATA = "player_count";

	// Token: 0x04000B8C RID: 2956
	public const string SI_SHINY_ROCK_COST = "si_shiny_rock_cost";

	// Token: 0x04000B8D RID: 2957
	public const string SI_PURCHASE_TYPE = "si_purchase_type";

	// Token: 0x04000B8E RID: 2958
	public const string SI_TECH_POINTS_PURCHASED = "si_tech_points_purchased";
}
