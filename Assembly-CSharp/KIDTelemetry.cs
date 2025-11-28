using System;
using UnityEngine;

// Token: 0x02000A6B RID: 2667
public static class KIDTelemetry
{
	// Token: 0x17000653 RID: 1619
	// (get) Token: 0x06004317 RID: 17175 RVA: 0x00033213 File Offset: 0x00031413
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x17000654 RID: 1620
	// (get) Token: 0x06004318 RID: 17176 RVA: 0x001643A6 File Offset: 0x001625A6
	public static string Open_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Open";
		}
	}

	// Token: 0x17000655 RID: 1621
	// (get) Token: 0x06004319 RID: 17177 RVA: 0x001643AD File Offset: 0x001625AD
	public static string Updated_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Updated";
		}
	}

	// Token: 0x17000656 RID: 1622
	// (get) Token: 0x0600431A RID: 17178 RVA: 0x001643B4 File Offset: 0x001625B4
	public static string Closed_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Closed";
		}
	}

	// Token: 0x17000657 RID: 1623
	// (get) Token: 0x0600431B RID: 17179 RVA: 0x00033224 File Offset: 0x00031424
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x0600431C RID: 17180 RVA: 0x001643BB File Offset: 0x001625BB
	public static string GetPermissionManagedByBodyData(string permission)
	{
		return "permission_managedby_" + permission.Replace('-', '_');
	}

	// Token: 0x0600431D RID: 17181 RVA: 0x001643D1 File Offset: 0x001625D1
	public static string GetPermissionEnabledBodyData(string permission)
	{
		return "permission_eneabled_" + permission.Replace('-', '_');
	}

	// Token: 0x0400547E RID: 21630
	public const string SCREEN_SHOWN_EVENT_NAME = "kid_screen_shown";

	// Token: 0x0400547F RID: 21631
	public const string PHASE_TWO_IN_COHORT_EVENT_NAME = "kid_phase2_incohort";

	// Token: 0x04005480 RID: 21632
	public const string PHASE_THREE_OPTIONAL_EVENT_NAME = "kid_phase3_optional";

	// Token: 0x04005481 RID: 21633
	public const string AGE_GATE_EVENT_NAME = "kid_age_gate";

	// Token: 0x04005482 RID: 21634
	public const string AGE_GATE_CONFIRM_EVENT_NAME = "kid_age_gate_confirm";

	// Token: 0x04005483 RID: 21635
	public const string AGE_DISCREPENCY_EVENT_NAME = "kid_age_gate_discrepency";

	// Token: 0x04005484 RID: 21636
	public const string GAME_SETTINGS_EVENT_NAME = "kid_game_settings";

	// Token: 0x04005485 RID: 21637
	public const string EMAIL_CONFIRM_EVENT_NAME = "kid_email_confirm";

	// Token: 0x04005486 RID: 21638
	public const string AGE_APPEAL_EVENT_NAME = "kid_age_appeal";

	// Token: 0x04005487 RID: 21639
	public const string APPEAL_AGE_GATE_EVENT_NAME = "kid_age_appeal_age_gate";

	// Token: 0x04005488 RID: 21640
	public const string APPEAL_ENTER_EMAIL_EVENT_NAME = "kid_age_appeal_enter_email";

	// Token: 0x04005489 RID: 21641
	public const string APPEAL_CONFIRM_EMAIL_EVENT_NAME = "kid_age_appeal_confirm_email";

	// Token: 0x0400548A RID: 21642
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x0400548B RID: 21643
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x0400548C RID: 21644
	public const string WARNING_SCREEN_CUSTOM_TAG = "kid_warning_screen";

	// Token: 0x0400548D RID: 21645
	public const string PHASE_TWO = "kid_phase_2";

	// Token: 0x0400548E RID: 21646
	public const string PHASE_THREE = "kid_phase_3";

	// Token: 0x0400548F RID: 21647
	public const string PHASE_FOUR = "kid_phase_4";

	// Token: 0x04005490 RID: 21648
	public const string AGE_GATE_CUSTOM_TAG = "kid_age_gate";

	// Token: 0x04005491 RID: 21649
	public const string SETTINGS_CUSTOM_TAG = "kid_settings";

	// Token: 0x04005492 RID: 21650
	public const string SETUP_CUSTOM_TAG = "kid_setup";

	// Token: 0x04005493 RID: 21651
	public const string APPEAL_CUSTOM_TAG = "kid_age_appeal";

	// Token: 0x04005494 RID: 21652
	public const string SCREEN_TYPE_BODY_DATA = "screen";

	// Token: 0x04005495 RID: 21653
	public const string OPT_IN_CHOICE_BODY_DATA = "opt_in_choice";

	// Token: 0x04005496 RID: 21654
	public const string BUTTON_PRESSED_BODY_DATA = "button_pressed";

	// Token: 0x04005497 RID: 21655
	public const string MISMATCH_EXPECTED_BODY_DATA = "mismatch_expected";

	// Token: 0x04005498 RID: 21656
	public const string MISMATCH_ACTUAL_BODY_DATA = "mismatch_actual";

	// Token: 0x04005499 RID: 21657
	public const string AGE_DECLARED_BODY_DATA = "age_declared";

	// Token: 0x0400549A RID: 21658
	public const string LEARN_MORE_URL_PRESSED_BODY_DATA = "learn_more_url_pressed";

	// Token: 0x0400549B RID: 21659
	public const string SCREEN_SHOWN_REASON_BODY_DATA = "screen_shown_reason";

	// Token: 0x0400549C RID: 21660
	public const string SUBMITTED_AGE_BODY_DATA = "submitted_age";

	// Token: 0x0400549D RID: 21661
	public const string CORRECT_AGE_BODY_DATA = "correct_age";

	// Token: 0x0400549E RID: 21662
	public const string APPEAL_EMAIL_TYPE_BODY_DATA = "email_type";

	// Token: 0x0400549F RID: 21663
	public const string SHOWN_SETTINGS_SCREEN = "saw_game_settings";

	// Token: 0x040054A0 RID: 21664
	public const string KID_STATUS_BODY_DATA = "kid_status";

	// Token: 0x040054A1 RID: 21665
	private const string PERMISSION_MANAGED_BY_BODY_DATA = "permission_managedby_";

	// Token: 0x040054A2 RID: 21666
	private const string PERMISSION_ENABLED_BODY_DATA = "permission_eneabled_";
}
