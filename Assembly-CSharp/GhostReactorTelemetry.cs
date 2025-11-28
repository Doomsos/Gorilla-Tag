using System;
using UnityEngine;

// Token: 0x0200065F RID: 1631
public class GhostReactorTelemetry : MonoBehaviour
{
	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x060029D4 RID: 10708 RVA: 0x00033213 File Offset: 0x00031413
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x060029D5 RID: 10709 RVA: 0x00033224 File Offset: 0x00031424
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x040035B8 RID: 13752
	public const string SHIFT_START_EVENT_NAME = "ghost_game_start";

	// Token: 0x040035B9 RID: 13753
	public const string SHIFT_END_EVENT_NAME = "ghost_game_end";

	// Token: 0x040035BA RID: 13754
	public const string FLOOR_START_EVENT_NAME = "ghost_floor_start";

	// Token: 0x040035BB RID: 13755
	public const string FLOOR_END_EVENT_NAME = "ghost_floor_end";

	// Token: 0x040035BC RID: 13756
	public const string TOOL_PURCHASED_EVENT_NAME = "ghost_tool_purchased";

	// Token: 0x040035BD RID: 13757
	public const string RANK_UP_EVENT_NAME = "ghost_game_rank_up";

	// Token: 0x040035BE RID: 13758
	public const string TOOL_UNLOCK_EVENT_NAME = "ghost_game_tool_unlock";

	// Token: 0x040035BF RID: 13759
	public const string POD_UPGRADE_PURCHASED_EVENT_NAME = "ghost_pod_upgrade_purchased";

	// Token: 0x040035C0 RID: 13760
	public const string TOOL_UPGRADE_EVENT_NAME = "ghost_game_tool_upgrade";

	// Token: 0x040035C1 RID: 13761
	public const string CHAOS_SEED_START_EVENT_NAME = "ghost_chaos_seed_start";

	// Token: 0x040035C2 RID: 13762
	public const string CHAOS_JUICE_COLLECTED_EVENT_NAME = "ghost_chaos_juice_collected";

	// Token: 0x040035C3 RID: 13763
	public const string OVERDRIVE_PURCHASED_EVENT_NAME = "ghost_overdrive_purchased";

	// Token: 0x040035C4 RID: 13764
	public const string CREDITS_REFILL_PURCHASED_EVENT_NAME = "ghost_credits_refill_purchased";

	// Token: 0x040035C5 RID: 13765
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x040035C6 RID: 13766
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x040035C7 RID: 13767
	public const string GHOST_GAME_ID_BODY_DATA = "ghost_game_id";

	// Token: 0x040035C8 RID: 13768
	public const string EVENT_TIMESTAMP_BODY_DATA = "event_timestamp";

	// Token: 0x040035C9 RID: 13769
	public const string INITIAL_CORES_BALANCE_BODY_DATA = "initial_cores_balance";

	// Token: 0x040035CA RID: 13770
	public const string FINAL_CORES_BALANCE_BODY_DATA = "final_cores_balance";

	// Token: 0x040035CB RID: 13771
	public const string CORES_SPENT_WAITING_IN_BREAKROOM_BODY_DATA = "cores_spent_waiting_in_breakroom";

	// Token: 0x040035CC RID: 13772
	public const string CORES_COLLECTED_FROM_GHOSTS_BODY_DATA = "cores_collected_from_ghosts";

	// Token: 0x040035CD RID: 13773
	public const string CORES_COLLECTED_FROM_GATHERING_BODY_DATA = "cores_collected_from_gathering";

	// Token: 0x040035CE RID: 13774
	public const string CORES_SPENT_ON_ITEMS_BODY_DATA = "cores_spent_on_items";

	// Token: 0x040035CF RID: 13775
	public const string CORES_SPENT_ON_GATES_BODY_DATA = "cores_spent_on_gates";

	// Token: 0x040035D0 RID: 13776
	public const string CORES_SPENT_ON_LEVELS_BODY_DATA = "cores_spent_on_levels";

	// Token: 0x040035D1 RID: 13777
	public const string CORES_GIVEN_TO_OTHERS_BODY_DATA = "cores_given_to_others";

	// Token: 0x040035D2 RID: 13778
	public const string CORES_RECEIVED_FROM_OTHERS_BODY_DATA = "cores_received_from_others";

	// Token: 0x040035D3 RID: 13779
	public const string SHIFT_CUT_DATA = "shift_cut_data";

	// Token: 0x040035D4 RID: 13780
	public const string GATES_UNLOCKED_BODY_DATA = "gates_unlocked";

	// Token: 0x040035D5 RID: 13781
	public const string DIED_BODY_DATA = "died";

	// Token: 0x040035D6 RID: 13782
	public const string CAUGHT_IN_ANAMOLE_BODY_DATA = "caught_in_anamole";

	// Token: 0x040035D7 RID: 13783
	public const string ITEMS_PURCHASED_BODY_DATA = "items_purchased";

	// Token: 0x040035D8 RID: 13784
	public const string LEVELS_UNLOCKED_BODY_DATA = "levels_unlocked";

	// Token: 0x040035D9 RID: 13785
	public const string NUMBER_OF_PLAYERS_BODY_DATA = "number_of_players";

	// Token: 0x040035DA RID: 13786
	public const string START_AT_BEGINNING_BODY_DATA = "start_at_beginning";

	// Token: 0x040035DB RID: 13787
	public const string SECONDS_INTO_SHIFT_AT_JOIN_BODY_DATA = "seconds_into_shift_at_join";

	// Token: 0x040035DC RID: 13788
	public const string REASON_BODY_DATA = "reason";

	// Token: 0x040035DD RID: 13789
	public const string PLAY_DURATION_BODY_DATA = "play_duration";

	// Token: 0x040035DE RID: 13790
	public const string STARTED_LATE_BODY_DATA = "started_late";

	// Token: 0x040035DF RID: 13791
	public const string TIME_STARTED_BODY_DATA = "time_started";

	// Token: 0x040035E0 RID: 13792
	public const string CORES_COLLECTED_BODY_DATA = "cores_collected";

	// Token: 0x040035E1 RID: 13793
	public const string MAX_NUMBER_IN_GAME_BODY_DATA = "max_number_in_game";

	// Token: 0x040035E2 RID: 13794
	public const string END_NUMBER_IN_GAME_BODY_DATA = "end_number_in_game";

	// Token: 0x040035E3 RID: 13795
	public const string ITEMS_PICKED_UP_BODY_DATA = "items_picked_up";

	// Token: 0x040035E4 RID: 13796
	public const string FLOOR_JOINED_BODY_DATA = "floor_joined";

	// Token: 0x040035E5 RID: 13797
	public const string PLAYER_RANK_BODY_DATA = "player_rank";

	// Token: 0x040035E6 RID: 13798
	public const string TOTAL_CORES_COLLECTED_BY_PLAYER_BODY_DATA = "total_cores_collected_by_player";

	// Token: 0x040035E7 RID: 13799
	public const string TOTAL_CORES_COLLECTED_BY_GROUP_BODY_DATA = "total_cores_collected_by_group";

	// Token: 0x040035E8 RID: 13800
	public const string TOTAL_CORES_SPENT_BY_PLAYER_BODY_DATA = "total_cores_spent_by_player";

	// Token: 0x040035E9 RID: 13801
	public const string TOTAL_CORES_SPENT_BY_GROUP_BODY_DATA = "total_cores_spent_by_group";

	// Token: 0x040035EA RID: 13802
	public const string FLOOR_BODY_DATA = "floor";

	// Token: 0x040035EB RID: 13803
	public const string PRESET_BODY_DATA = "preset";

	// Token: 0x040035EC RID: 13804
	public const string MODIFIER_BODY_DATA = "modifier";

	// Token: 0x040035ED RID: 13805
	public const string SECTION_BODY_DATA = "section";

	// Token: 0x040035EE RID: 13806
	public const string XP_GAINED_BODY_DATA = "xp_gained";

	// Token: 0x040035EF RID: 13807
	public const string CHAOS_SEEDS_COLLECTED_BODY_DATA = "chaos_seeds_collected";

	// Token: 0x040035F0 RID: 13808
	public const string OBJECTIVES_COMPLETED_BODY_DATA = "objectives_completed";

	// Token: 0x040035F1 RID: 13809
	public const string REVIVES_BODY_DATA = "revives";

	// Token: 0x040035F2 RID: 13810
	public const string TOOL_BODY_DATA = "tool";

	// Token: 0x040035F3 RID: 13811
	public const string TOOL_LEVEL_BODY_DATA = "tool_level";

	// Token: 0x040035F4 RID: 13812
	public const string CORES_SPENT_BODY_DATA = "cores_spent";

	// Token: 0x040035F5 RID: 13813
	public const string SHINY_ROCKS_SPENT_BODY_DATA = "shiny_rocks_spent";

	// Token: 0x040035F6 RID: 13814
	public const string NEW_RANK_BODY_DATA = "new_rank";

	// Token: 0x040035F7 RID: 13815
	public const string UPGRADE_BODY_DATA = "upgrade";

	// Token: 0x040035F8 RID: 13816
	public const string GRIFT_PRICE_BODY_DATA = "grift_price";

	// Token: 0x040035F9 RID: 13817
	public const string TYPE_BODY_DATA = "type";

	// Token: 0x040035FA RID: 13818
	public const string NEW_LEVEL_BODY_DATA = "new_level";

	// Token: 0x040035FB RID: 13819
	public const string JUICE_SPENT_BODY_DATA = "juice_spent";

	// Token: 0x040035FC RID: 13820
	public const string GRIFT_SPENT_BODY_DATA = "grift_spent";

	// Token: 0x040035FD RID: 13821
	public const string CHAOS_SEEDS_IN_QUEUE_BODY_DATA = "chaos_seeds_in_queue";

	// Token: 0x040035FE RID: 13822
	public const string UNLOCK_TIME_BODY_DATA = "unlock_time";

	// Token: 0x040035FF RID: 13823
	public const string SHINY_ROCKS_USED_BODY_DATA = "shiny_rocks_used";

	// Token: 0x04003600 RID: 13824
	public const string JUICE_COLLECTED_BODY_DATA = "juice_collected";

	// Token: 0x04003601 RID: 13825
	public const string CORES_PROCESSED_BY_OVERDRIVE_BODY_DATA = "cores_processed_by_overdrive";

	// Token: 0x04003602 RID: 13826
	public const string FINAL_CREDITS_BODY_DATA = "final_credits";

	// Token: 0x04003603 RID: 13827
	public const string IS_PRIVATE_ROOM_BODY_DATA = "is_private_room";

	// Token: 0x04003604 RID: 13828
	public const string NUM_SHIFTS_PLAYED_BODY_DATA = "num_shifts_played";
}
