using System;
using System.Collections.Generic;
using CjLib;
using Unity.Collections;
using UnityEngine;

// Token: 0x020006D8 RID: 1752
public class GRNoiseEventManager : MonoBehaviourTick
{
	// Token: 0x06002CD7 RID: 11479 RVA: 0x000F2EFC File Offset: 0x000F10FC
	public void Awake()
	{
		GRNoiseEventManager.instance = this;
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x000F2F04 File Offset: 0x000F1104
	public override void Tick()
	{
		this.RemoveExpiredEvents();
		if (GhostReactorManager.noiseDebugEnabled)
		{
			this.RenderDebug();
		}
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x000F2F19 File Offset: 0x000F1119
	private int FindUnusedEventEntry()
	{
		return -1;
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x000F2F1C File Offset: 0x000F111C
	public void AddNoiseEvent(Vector3 position, float magnitude = 1f, float duration = 1f)
	{
		GameNoiseEvent gameNoiseEvent = new GameNoiseEvent
		{
			position = position,
			eventTime = Time.timeAsDouble,
			duration = duration,
			magnitude = magnitude
		};
		int num = this.FindUnusedEventEntry();
		if (num == -1)
		{
			this.noiseEvents.Add(gameNoiseEvent);
			return;
		}
		this.noiseEvents[num] = gameNoiseEvent;
	}

	// Token: 0x06002CDC RID: 11484 RVA: 0x000F2F7C File Offset: 0x000F117C
	public List<GameNoiseEvent> GetNoiseEventsInRadius(Vector3 origin, float radius)
	{
		List<GameNoiseEvent> list = new List<GameNoiseEvent>();
		float num = radius * radius;
		foreach (GameNoiseEvent gameNoiseEvent in this.noiseEvents)
		{
			if (gameNoiseEvent.IsValid())
			{
				float sqrMagnitude = (gameNoiseEvent.position - origin).sqrMagnitude;
				float num2 = gameNoiseEvent.magnitude * gameNoiseEvent.magnitude;
				if (sqrMagnitude < num * num2)
				{
					list.Add(gameNoiseEvent);
				}
			}
		}
		return list;
	}

	// Token: 0x06002CDD RID: 11485 RVA: 0x000F3010 File Offset: 0x000F1210
	public bool GetMostRecentNoiseEventInRadius(Vector3 origin, float radius, out GameNoiseEvent outEvent)
	{
		double timeAsDouble = Time.timeAsDouble;
		float num = radius * radius;
		double num2 = -1.0;
		int num3 = -1;
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			GameNoiseEvent gameNoiseEvent = this.noiseEvents[i];
			if (gameNoiseEvent.IsValid())
			{
				float sqrMagnitude = (gameNoiseEvent.position - origin).sqrMagnitude;
				float num4 = gameNoiseEvent.magnitude * gameNoiseEvent.magnitude;
				if (sqrMagnitude < num * num4)
				{
					double num5 = timeAsDouble - gameNoiseEvent.eventTime;
					if (num3 < 0 || num5 < num2)
					{
						num3 = i;
						num2 = num5;
					}
				}
			}
		}
		if (num3 < 0)
		{
			outEvent = default(GameNoiseEvent);
			return false;
		}
		outEvent = this.noiseEvents[num3];
		return true;
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x000F30CC File Offset: 0x000F12CC
	public void RenderDebug()
	{
		int num = 0;
		float num2 = 5f;
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			GameNoiseEvent gameNoiseEvent = this.noiseEvents[i];
			if (gameNoiseEvent.IsValid())
			{
				float radius = this.debugMeshScale * gameNoiseEvent.magnitude * num2;
				DebugUtil.DrawSphere(gameNoiseEvent.position, radius, 8, 6, Color.green, true, DebugUtil.Style.Wireframe);
				num++;
			}
		}
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x000F3138 File Offset: 0x000F1338
	private void RemoveExpiredEvents()
	{
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			if (!this.noiseEvents[i].IsValid())
			{
				ListExtensions.RemoveAtSwapBack<GameNoiseEvent>(this.noiseEvents, i);
				i--;
			}
		}
	}

	// Token: 0x04003A41 RID: 14913
	private List<GameNoiseEvent> noiseEvents = new List<GameNoiseEvent>();

	// Token: 0x04003A42 RID: 14914
	public static GRNoiseEventManager instance;

	// Token: 0x04003A43 RID: 14915
	public float debugMeshScale = 1f;
}
