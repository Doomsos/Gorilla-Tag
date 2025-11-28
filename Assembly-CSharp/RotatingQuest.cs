using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

// Token: 0x020001FB RID: 507
[Serializable]
public class RotatingQuest
{
	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x00048A7A File Offset: 0x00046C7A
	[JsonIgnore]
	public bool IsMovementQuest
	{
		get
		{
			return this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000DC7 RID: 3527 RVA: 0x00048A91 File Offset: 0x00046C91
	// (set) Token: 0x06000DC8 RID: 3528 RVA: 0x00048A99 File Offset: 0x00046C99
	[JsonIgnore]
	public GTZone RequiredZone { get; private set; } = GTZone.none;

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00048AA2 File Offset: 0x00046CA2
	public void SetRequiredZone()
	{
		this.RequiredZone = ((this.requiredZones.Count > 0) ? this.requiredZones[Random.Range(0, this.requiredZones.Count)] : GTZone.none);
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00048AD8 File Offset: 0x00046CD8
	public void AddEventListener()
	{
		if (this.isQuestComplete)
		{
			return;
		}
		switch (this.questType)
		{
		case QuestType.gameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.gameModeRound:
			PlayerGameEvents.OnGameModeCompleteRound += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.grabObject:
			PlayerGameEvents.OnGrabbedObject += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.dropObject:
			PlayerGameEvents.OnDroppedObject += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.eatObject:
			PlayerGameEvents.OnEatObject += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.tapObject:
			PlayerGameEvents.OnTapObject += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.launchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.moveDistance:
			PlayerGameEvents.OnPlayerMoved += new Action<float, float>(this.OnGameMoveEvent);
			return;
		case QuestType.swimDistance:
			PlayerGameEvents.OnPlayerSwam += new Action<float, float>(this.OnGameMoveEvent);
			return;
		case QuestType.triggerHandEffect:
			PlayerGameEvents.OnTriggerHandEffect += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.enterLocation:
			PlayerGameEvents.OnEnterLocation += new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.misc:
			PlayerGameEvents.OnMiscEvent += new Action<string, int>(this.OnGameEventOccurence);
			return;
		case QuestType.critter:
			PlayerGameEvents.OnCritterEvent += new Action<string>(this.OnGameEventOccurence);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00048C1C File Offset: 0x00046E1C
	public void RemoveEventListener()
	{
		switch (this.questType)
		{
		case QuestType.gameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.gameModeRound:
			PlayerGameEvents.OnGameModeCompleteRound -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.grabObject:
			PlayerGameEvents.OnGrabbedObject -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.dropObject:
			PlayerGameEvents.OnDroppedObject -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.eatObject:
			PlayerGameEvents.OnEatObject -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.tapObject:
			PlayerGameEvents.OnTapObject -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.launchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.moveDistance:
			PlayerGameEvents.OnPlayerMoved -= new Action<float, float>(this.OnGameMoveEvent);
			return;
		case QuestType.swimDistance:
			PlayerGameEvents.OnPlayerSwam -= new Action<float, float>(this.OnGameMoveEvent);
			return;
		case QuestType.triggerHandEffect:
			PlayerGameEvents.OnTriggerHandEffect -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.enterLocation:
			PlayerGameEvents.OnEnterLocation -= new Action<string>(this.OnGameEventOccurence);
			return;
		case QuestType.misc:
			PlayerGameEvents.OnMiscEvent -= new Action<string, int>(this.OnGameEventOccurence);
			return;
		case QuestType.critter:
			PlayerGameEvents.OnCritterEvent -= new Action<string>(this.OnGameEventOccurence);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x00048D58 File Offset: 0x00046F58
	public void ApplySavedProgress(int progress)
	{
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			this.moveDistance = (float)progress;
			this.occurenceCount = Mathf.FloorToInt(this.moveDistance);
			this.isQuestComplete = (this.occurenceCount >= this.requiredOccurenceCount);
			return;
		}
		this.occurenceCount = progress;
		this.isQuestComplete = (this.occurenceCount >= this.requiredOccurenceCount);
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x00048DC7 File Offset: 0x00046FC7
	public int GetProgress()
	{
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			return Mathf.FloorToInt(this.moveDistance);
		}
		return this.occurenceCount;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x00048DEE File Offset: 0x00046FEE
	private void OnGameEventOccurence(string eventName)
	{
		this.OnGameEventOccurence(eventName, 1);
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x00048DF8 File Offset: 0x00046FF8
	private void OnGameEventOccurence(string eventName, int count)
	{
		if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
		{
			return;
		}
		string.IsNullOrEmpty(this.questOccurenceFilter);
		if (eventName.StartsWith(this.questOccurenceFilter))
		{
			this.SetProgress(this.occurenceCount + count);
		}
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x00048E48 File Offset: 0x00047048
	private void OnGameMoveEvent(float distance, float speed)
	{
		if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
		{
			return;
		}
		if (!(this.questOccurenceFilter == "maxSpeed"))
		{
			this.moveDistance += distance;
			this.SetProgress(Mathf.FloorToInt(this.moveDistance));
			return;
		}
		if (speed <= this.moveDistance)
		{
			return;
		}
		this.moveDistance = speed;
		this.SetProgress(Mathf.FloorToInt(this.moveDistance));
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x00048EC4 File Offset: 0x000470C4
	private void SetProgress(int progress)
	{
		if (this.isQuestComplete)
		{
			return;
		}
		if (this.occurenceCount == progress)
		{
			return;
		}
		this.lastChange = Time.frameCount;
		this.occurenceCount = progress;
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			this.moveDistance = (float)progress;
		}
		if (this.occurenceCount >= this.requiredOccurenceCount)
		{
			this.Complete();
		}
		this.questManager.HandleQuestProgressChanged(false);
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x00048F31 File Offset: 0x00047131
	private void Complete()
	{
		if (this.isQuestComplete)
		{
			return;
		}
		this.isQuestComplete = true;
		this.RemoveEventListener();
		this.questManager.HandleQuestCompleted(this.questID);
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x00048F5A File Offset: 0x0004715A
	public string GetTextDescription()
	{
		return this.<GetTextDescription>g__GetActionName|32_0().ToUpper() + this.<GetTextDescription>g__GetLocationText|32_1().ToUpper();
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x00048F77 File Offset: 0x00047177
	public string GetProgressText()
	{
		if (!this.isQuestComplete)
		{
			return string.Format("{0}/{1}", this.occurenceCount, this.requiredOccurenceCount);
		}
		return "[DONE]";
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x00048FD4 File Offset: 0x000471D4
	[CompilerGenerated]
	private string <GetTextDescription>g__GetActionName|32_0()
	{
		switch (this.questType)
		{
		case QuestType.none:
			return "[UNDEFINED]";
		case QuestType.gameModeObjective:
			return this.questName;
		case QuestType.gameModeRound:
			return this.questName;
		case QuestType.grabObject:
			return this.questName;
		case QuestType.dropObject:
			return this.questName;
		case QuestType.eatObject:
			return this.questName;
		case QuestType.launchedProjectile:
			return this.questName;
		case QuestType.moveDistance:
			return this.questName;
		case QuestType.swimDistance:
			return this.questName;
		case QuestType.triggerHandEffect:
			return this.questName;
		case QuestType.enterLocation:
			return this.questName;
		case QuestType.misc:
			return this.questName;
		}
		return this.questName;
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x00049097 File Offset: 0x00047297
	[CompilerGenerated]
	private string <GetTextDescription>g__GetLocationText|32_1()
	{
		if (this.RequiredZone == GTZone.none)
		{
			return "";
		}
		return string.Format(" IN {0}", this.RequiredZone);
	}

	// Token: 0x040010C7 RID: 4295
	public bool disable;

	// Token: 0x040010C8 RID: 4296
	public int questID;

	// Token: 0x040010C9 RID: 4297
	public float weight = 1f;

	// Token: 0x040010CA RID: 4298
	public QuestCategory category;

	// Token: 0x040010CB RID: 4299
	public string questName = "UNNAMED QUEST";

	// Token: 0x040010CC RID: 4300
	public QuestType questType;

	// Token: 0x040010CD RID: 4301
	public string questOccurenceFilter;

	// Token: 0x040010CE RID: 4302
	public int requiredOccurenceCount = 1;

	// Token: 0x040010CF RID: 4303
	[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	public List<GTZone> requiredZones;

	// Token: 0x040010D0 RID: 4304
	[Space]
	[NonSerialized]
	public bool isQuestActive;

	// Token: 0x040010D1 RID: 4305
	[NonSerialized]
	public bool isQuestComplete;

	// Token: 0x040010D2 RID: 4306
	[NonSerialized]
	public bool isDailyQuest;

	// Token: 0x040010D3 RID: 4307
	[NonSerialized]
	public int lastChange;

	// Token: 0x040010D5 RID: 4309
	[NonSerialized]
	public int occurenceCount;

	// Token: 0x040010D6 RID: 4310
	private float moveDistance;

	// Token: 0x040010D7 RID: 4311
	[NonSerialized]
	public GorillaQuestManager questManager;
}
