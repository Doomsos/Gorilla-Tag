using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class RotatingQuestsManager : MonoBehaviour, ITickSystemTick, GorillaQuestManager
{
	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000DD8 RID: 3544 RVA: 0x000490BE File Offset: 0x000472BE
	// (set) Token: 0x06000DD9 RID: 3545 RVA: 0x000490C6 File Offset: 0x000472C6
	public bool TickRunning { get; set; }

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000DDA RID: 3546 RVA: 0x000490CF File Offset: 0x000472CF
	// (set) Token: 0x06000DDB RID: 3547 RVA: 0x000490D7 File Offset: 0x000472D7
	public DateTime DailyQuestCountdown { get; private set; }

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000DDC RID: 3548 RVA: 0x000490E0 File Offset: 0x000472E0
	// (set) Token: 0x06000DDD RID: 3549 RVA: 0x000490E8 File Offset: 0x000472E8
	public DateTime WeeklyQuestCountdown { get; private set; }

	// Token: 0x06000DDE RID: 3550 RVA: 0x000490F1 File Offset: 0x000472F1
	private void Start()
	{
		this._questAudio = base.GetComponent<AudioSource>();
		this.RequestQuestsFromTitleData();
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x0001877F File Offset: 0x0001697F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00049105 File Offset: 0x00047305
	public void Tick()
	{
		if (this.hasQuest && this.nextQuestUpdateTime < DateTime.UtcNow)
		{
			this.SetupQuests();
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00049128 File Offset: 0x00047328
	private void ProcessAllQuests(Action<RotatingQuest> action)
	{
		RotatingQuestsManager.<>c__DisplayClass29_0 CS$<>8__locals1;
		CS$<>8__locals1.action = action;
		RotatingQuestsManager.<ProcessAllQuests>g__ProcessAllQuestsInList|29_0(this.quests.DailyQuests, ref CS$<>8__locals1);
		RotatingQuestsManager.<ProcessAllQuests>g__ProcessAllQuestsInList|29_0(this.quests.WeeklyQuests, ref CS$<>8__locals1);
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00049161 File Offset: 0x00047361
	private void QuestLoadPostProcess(RotatingQuest quest)
	{
		if (quest.requiredZones.Count == 1 && quest.requiredZones[0] == GTZone.none)
		{
			quest.requiredZones.Clear();
		}
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x000298E4 File Offset: 0x00027AE4
	private void QuestSavePreProcess(RotatingQuest quest)
	{
		if (quest.requiredZones.Count == 0)
		{
			quest.requiredZones.Add(GTZone.none);
		}
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x0004918C File Offset: 0x0004738C
	public void LoadTestQuestsFromFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>(this.localQuestPath);
		this.LoadQuestsFromJson(textAsset.text);
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x000491B1 File Offset: 0x000473B1
	public void RequestQuestsFromTitleData()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("AllActiveQuests", delegate(string data)
		{
			this.LoadQuestsFromJson(data);
		}, delegate(PlayFabError e)
		{
			Debug.LogError(string.Format("Error getting AllActiveQuests data: {0}", e));
		}, false);
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x000491F0 File Offset: 0x000473F0
	public void LoadQuestsFromJson(string jsonString)
	{
		this.quests = JsonConvert.DeserializeObject<RotatingQuestsManager.RotatingQuestList>(jsonString);
		this.ProcessAllQuests(new Action<RotatingQuest>(this.QuestLoadPostProcess));
		if (this.quests == null)
		{
			Debug.LogError("Error: Quests failed to parse!");
			return;
		}
		this.hasQuest = true;
		this.quests.Init();
		if (Application.isPlaying)
		{
			this.SetupQuests();
		}
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00049250 File Offset: 0x00047450
	private void SetupQuests()
	{
		this.ClearAllQuestEventListeners();
		this.SelectActiveQuests();
		this.LoadQuestProgress();
		this.HandleQuestProgressChanged(true);
		this.SetupAllQuestEventListeners();
		this.nextQuestUpdateTime = this.DailyQuestCountdown;
		this.nextQuestUpdateTime = this.nextQuestUpdateTime.AddMinutes(1.0);
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x000492A4 File Offset: 0x000474A4
	private void SelectActiveQuests()
	{
		DateTime dateTime;
		dateTime..ctor(2025, 1, 10, 18, 0, 0, 1);
		TimeSpan timeSpan = TimeSpan.FromHours(-8.0);
		DateTime dateTime2;
		dateTime2..ctor(1, 1, 1, 0, 0, 0);
		DateTime dateTime3;
		dateTime3..ctor(2006, 12, 31, 0, 0, 0);
		TimeSpan timeSpan2 = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 4, 1, 0);
		TimeZoneInfo.TransitionTime transitionTime2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 10, 5, 0);
		DateTime dateTime4;
		dateTime4..ctor(2007, 1, 1, 0, 0, 0);
		DateTime dateTime5;
		dateTime5..ctor(9999, 12, 31, 0, 0, 0);
		TimeSpan timeSpan3 = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime transitionTime3 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, 0);
		TimeZoneInfo.TransitionTime transitionTime4 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, 0);
		TimeZoneInfo timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Pacific Standard Time", timeSpan, "Pacific Standard Time", "Pacific Standard Time", "Pacific Standard Time", new TimeZoneInfo.AdjustmentRule[]
		{
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime2, dateTime3, timeSpan2, transitionTime, transitionTime2),
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime4, dateTime5, timeSpan3, transitionTime3, transitionTime4)
		});
		if (timeZoneInfo != null && timeZoneInfo.IsDaylightSavingTime(DateTime.UtcNow - timeSpan))
		{
			dateTime -= TimeSpan.FromHours(1.0);
		}
		TimeSpan timeSpan4 = DateTime.UtcNow - dateTime;
		this.RemoveDisabledQuests();
		int days = timeSpan4.Days;
		this.dailyQuestSetID = days;
		this.weeklyQuestSetID = days / 7;
		RotatingQuestsManager.LastQuestDailyID = this.dailyQuestSetID;
		this.DailyQuestCountdown = dateTime + TimeSpan.FromDays((double)(this.dailyQuestSetID + 1));
		this.WeeklyQuestCountdown = dateTime + TimeSpan.FromDays((double)((this.weeklyQuestSetID + 1) * 7));
		Random.InitState(this.dailyQuestSetID);
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			int num = Math.Min(rotatingQuestGroup.selectCount, rotatingQuestGroup.quests.Count);
			float num2 = 0f;
			List<ValueTuple<int, float>> list = new List<ValueTuple<int, float>>(rotatingQuestGroup.quests.Count);
			for (int i = 0; i < rotatingQuestGroup.quests.Count; i++)
			{
				rotatingQuestGroup.quests[i].isQuestActive = false;
				num2 += rotatingQuestGroup.quests[i].weight;
				list.Add(new ValueTuple<int, float>(i, rotatingQuestGroup.quests[i].weight));
			}
			for (int j = 0; j < num; j++)
			{
				float num3 = Random.Range(0f, num2);
				for (int k = 0; k < list.Count; k++)
				{
					float item = list[k].Item2;
					if (num3 <= item || k == list.Count - 1)
					{
						num2 -= item;
						int item2 = list[k].Item1;
						list.RemoveAt(k);
						rotatingQuestGroup.quests[item2].isQuestActive = true;
						rotatingQuestGroup.quests[item2].SetRequiredZone();
						break;
					}
					num3 -= item;
				}
			}
		}
		Random.InitState(this.weeklyQuestSetID);
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			int num4 = Math.Min(rotatingQuestGroup2.selectCount, rotatingQuestGroup2.quests.Count);
			float num5 = 0f;
			List<ValueTuple<int, float>> list2 = new List<ValueTuple<int, float>>(rotatingQuestGroup2.quests.Count);
			for (int l = 0; l < rotatingQuestGroup2.quests.Count; l++)
			{
				rotatingQuestGroup2.quests[l].isQuestActive = false;
				num5 += rotatingQuestGroup2.quests[l].weight;
				list2.Add(new ValueTuple<int, float>(l, rotatingQuestGroup2.quests[l].weight));
			}
			for (int m = 0; m < num4; m++)
			{
				float num6 = Random.Range(0f, num5);
				for (int n = 0; n < list2.Count; n++)
				{
					float item3 = list2[n].Item2;
					if (num6 <= item3 || n == list2.Count - 1)
					{
						num5 -= item3;
						int item4 = list2[n].Item1;
						list2.RemoveAt(n);
						rotatingQuestGroup2.quests[item4].isQuestActive = true;
						rotatingQuestGroup2.quests[item4].SetRequiredZone();
						break;
					}
					num6 -= item3;
				}
			}
		}
		ProgressionController.ReportQuestSelectionChanged();
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x000497D8 File Offset: 0x000479D8
	private void RemoveDisabledQuests()
	{
		RotatingQuestsManager.<RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|37_0(this.quests.DailyQuests);
		RotatingQuestsManager.<RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|37_0(this.quests.WeeklyQuests);
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x000497FC File Offset: 0x000479FC
	public void LoadQuestProgress()
	{
		int @int = PlayerPrefs.GetInt("Rotating_Quest_Daily_SetID_Key", -1);
		int int2 = PlayerPrefs.GetInt("Rotating_Quest_Daily_SaveCount_Key", -1);
		if (@int == this.dailyQuestSetID)
		{
			for (int i = 0; i < int2; i++)
			{
				int int3 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_ID_Key", i), -1);
				int int4 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_Progress_Key", i), -1);
				if (int3 != -1)
				{
					for (int j = 0; j < this.quests.DailyQuests.Count; j++)
					{
						for (int k = 0; k < this.quests.DailyQuests[j].quests.Count; k++)
						{
							RotatingQuest rotatingQuest = this.quests.DailyQuests[j].quests[k];
							if (rotatingQuest.questID == int3)
							{
								rotatingQuest.ApplySavedProgress(int4);
								break;
							}
						}
					}
				}
			}
		}
		int int5 = PlayerPrefs.GetInt("Rotating_Quest_Weekly_SetID_Key", -1);
		int int6 = PlayerPrefs.GetInt("Rotating_Quest_Weekly_SaveCount_Key", -1);
		if (int5 == this.weeklyQuestSetID)
		{
			for (int l = 0; l < int6; l++)
			{
				int int7 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_ID_Key", l), -1);
				int int8 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_Progress_Key", l), -1);
				if (int7 != -1)
				{
					for (int m = 0; m < this.quests.WeeklyQuests.Count; m++)
					{
						for (int n = 0; n < this.quests.WeeklyQuests[m].quests.Count; n++)
						{
							RotatingQuest rotatingQuest2 = this.quests.WeeklyQuests[m].quests[n];
							if (rotatingQuest2.questID == int7)
							{
								rotatingQuest2.ApplySavedProgress(int8);
								break;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x000499F8 File Offset: 0x00047BF8
	public void SaveQuestProgress()
	{
		int num = 0;
		for (int i = 0; i < this.quests.DailyQuests.Count; i++)
		{
			for (int j = 0; j < this.quests.DailyQuests[i].quests.Count; j++)
			{
				RotatingQuest rotatingQuest = this.quests.DailyQuests[i].quests[j];
				int progress = rotatingQuest.GetProgress();
				if (progress > 0)
				{
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_ID_Key", num), rotatingQuest.questID);
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_Progress_Key", num), progress);
					num++;
				}
			}
		}
		if (num > 0)
		{
			PlayerPrefs.SetInt("Rotating_Quest_Daily_SetID_Key", this.dailyQuestSetID);
			PlayerPrefs.SetInt("Rotating_Quest_Daily_SaveCount_Key", num);
		}
		int num2 = 0;
		for (int k = 0; k < this.quests.WeeklyQuests.Count; k++)
		{
			for (int l = 0; l < this.quests.WeeklyQuests[k].quests.Count; l++)
			{
				RotatingQuest rotatingQuest2 = this.quests.WeeklyQuests[k].quests[l];
				int progress2 = rotatingQuest2.GetProgress();
				if (progress2 > 0)
				{
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_ID_Key", num2), rotatingQuest2.questID);
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_Progress_Key", num2), progress2);
					num2++;
				}
			}
		}
		if (num2 > 0)
		{
			PlayerPrefs.SetInt("Rotating_Quest_Weekly_SetID_Key", this.weeklyQuestSetID);
			PlayerPrefs.SetInt("Rotating_Quest_Weekly_SaveCount_Key", num2);
		}
		PlayerPrefs.Save();
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00049BC8 File Offset: 0x00047DC8
	public void SetupAllQuestEventListeners()
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			foreach (RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				rotatingQuest.questManager = this;
				if (rotatingQuest.isQuestActive && !rotatingQuest.isQuestComplete)
				{
					rotatingQuest.AddEventListener();
				}
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			foreach (RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				rotatingQuest2.questManager = this;
				if (rotatingQuest2.isQuestActive && !rotatingQuest2.isQuestComplete)
				{
					rotatingQuest2.AddEventListener();
				}
			}
		}
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x00049D08 File Offset: 0x00047F08
	public void ClearAllQuestEventListeners()
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			foreach (RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				rotatingQuest.RemoveEventListener();
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			foreach (RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				rotatingQuest2.RemoveEventListener();
			}
		}
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x00049E14 File Offset: 0x00048014
	public void HandleQuestCompleted(int questID)
	{
		RotatingQuest quest = this.quests.GetQuest(questID);
		if (quest == null)
		{
			return;
		}
		ProgressionController.ReportQuestComplete(questID, quest.isDailyQuest);
		if (this._playQuestSounds)
		{
			AudioSource questAudio = this._questAudio;
			if (questAudio == null)
			{
				return;
			}
			questAudio.GTPlay();
		}
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00049E56 File Offset: 0x00048056
	public void HandleQuestProgressChanged(bool initialLoad)
	{
		if (!initialLoad)
		{
			this.SaveQuestProgress();
		}
		RotatingQuestsManager.LastQuestChange = Time.frameCount;
		ProgressionController.ReportQuestChanged(initialLoad);
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x00049E84 File Offset: 0x00048084
	[CompilerGenerated]
	internal static void <ProcessAllQuests>g__ProcessAllQuestsInList|29_0(List<RotatingQuestsManager.RotatingQuestGroup> questGroups, ref RotatingQuestsManager.<>c__DisplayClass29_0 A_1)
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questGroups)
		{
			foreach (RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				A_1.action.Invoke(rotatingQuest);
			}
		}
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x00049F1C File Offset: 0x0004811C
	[CompilerGenerated]
	internal static void <RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|37_0(List<RotatingQuestsManager.RotatingQuestGroup> questList)
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questList)
		{
			for (int i = rotatingQuestGroup.quests.Count - 1; i >= 0; i--)
			{
				if (rotatingQuestGroup.quests[i].disable)
				{
					rotatingQuestGroup.quests.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x040010D9 RID: 4313
	private bool hasQuest;

	// Token: 0x040010DA RID: 4314
	[SerializeField]
	private bool useTestLocalQuests;

	// Token: 0x040010DB RID: 4315
	[SerializeField]
	private string localQuestPath = "TestingRotatingQuests";

	// Token: 0x040010DC RID: 4316
	public static int LastQuestChange;

	// Token: 0x040010DD RID: 4317
	public static int LastQuestDailyID;

	// Token: 0x040010DE RID: 4318
	public RotatingQuestsManager.RotatingQuestList quests;

	// Token: 0x040010DF RID: 4319
	public int dailyQuestSetID;

	// Token: 0x040010E0 RID: 4320
	public int weeklyQuestSetID;

	// Token: 0x040010E1 RID: 4321
	[SerializeField]
	private bool _playQuestSounds;

	// Token: 0x040010E2 RID: 4322
	private AudioSource _questAudio;

	// Token: 0x040010E5 RID: 4325
	private DateTime nextQuestUpdateTime;

	// Token: 0x040010E6 RID: 4326
	private const string kDailyQuestSetIDKey = "Rotating_Quest_Daily_SetID_Key";

	// Token: 0x040010E7 RID: 4327
	private const string kDailyQuestSaveCountKey = "Rotating_Quest_Daily_SaveCount_Key";

	// Token: 0x040010E8 RID: 4328
	private const string kDailyQuestIDKey = "Rotating_Quest_Daily_ID_Key";

	// Token: 0x040010E9 RID: 4329
	private const string kDailyQuestProgressKey = "Rotating_Quest_Daily_Progress_Key";

	// Token: 0x040010EA RID: 4330
	private const string kWeeklyQuestSetIDKey = "Rotating_Quest_Weekly_SetID_Key";

	// Token: 0x040010EB RID: 4331
	private const string kWeeklyQuestSaveCountKey = "Rotating_Quest_Weekly_SaveCount_Key";

	// Token: 0x040010EC RID: 4332
	private const string kWeeklyQuestIDKey = "Rotating_Quest_Weekly_ID_Key";

	// Token: 0x040010ED RID: 4333
	private const string kWeeklyQuestProgressKey = "Rotating_Quest_Weekly_Progress_Key";

	// Token: 0x020001FD RID: 509
	[Serializable]
	public class RotatingQuestGroup
	{
		// Token: 0x040010EE RID: 4334
		public int selectCount;

		// Token: 0x040010EF RID: 4335
		public string name;

		// Token: 0x040010F0 RID: 4336
		public List<RotatingQuest> quests;
	}

	// Token: 0x020001FE RID: 510
	[Serializable]
	public class RotatingQuestList
	{
		// Token: 0x06000DF6 RID: 3574 RVA: 0x00049F9C File Offset: 0x0004819C
		public void Init()
		{
			RotatingQuestsManager.RotatingQuestList.<Init>g__SetIsDaily|2_0(this.DailyQuests, true);
			RotatingQuestsManager.RotatingQuestList.<Init>g__SetIsDaily|2_0(this.WeeklyQuests, false);
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x00049FB8 File Offset: 0x000481B8
		public RotatingQuest GetQuest(int questID)
		{
			RotatingQuestsManager.RotatingQuestList.<>c__DisplayClass3_0 CS$<>8__locals1;
			CS$<>8__locals1.questID = questID;
			RotatingQuest rotatingQuest = RotatingQuestsManager.RotatingQuestList.<GetQuest>g__GetQuestFrom|3_0(this.DailyQuests, ref CS$<>8__locals1);
			if (rotatingQuest == null)
			{
				rotatingQuest = RotatingQuestsManager.RotatingQuestList.<GetQuest>g__GetQuestFrom|3_0(this.WeeklyQuests, ref CS$<>8__locals1);
			}
			return rotatingQuest;
		}

		// Token: 0x06000DF9 RID: 3577 RVA: 0x00049FF0 File Offset: 0x000481F0
		[CompilerGenerated]
		internal static void <Init>g__SetIsDaily|2_0(List<RotatingQuestsManager.RotatingQuestGroup> questList, bool isDaily)
		{
			foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questList)
			{
				foreach (RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
				{
					rotatingQuest.isDailyQuest = isDaily;
				}
			}
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x0004A078 File Offset: 0x00048278
		[CompilerGenerated]
		internal static RotatingQuest <GetQuest>g__GetQuestFrom|3_0(List<RotatingQuestsManager.RotatingQuestGroup> list, ref RotatingQuestsManager.RotatingQuestList.<>c__DisplayClass3_0 A_1)
		{
			foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in list)
			{
				foreach (RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
				{
					if (rotatingQuest.questID == A_1.questID)
					{
						return rotatingQuest;
					}
				}
			}
			return null;
		}

		// Token: 0x040010F1 RID: 4337
		public List<RotatingQuestsManager.RotatingQuestGroup> DailyQuests;

		// Token: 0x040010F2 RID: 4338
		public List<RotatingQuestsManager.RotatingQuestGroup> WeeklyQuests;
	}
}
