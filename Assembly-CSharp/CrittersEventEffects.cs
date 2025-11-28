using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class CrittersEventEffects : MonoBehaviour
{
	// Token: 0x060001A4 RID: 420 RVA: 0x0000A504 File Offset: 0x00008704
	private void Awake()
	{
		if (this.manager == null)
		{
			GTDev.LogError<string>("CrittersEventEffects missing reference to CrittersManager", null);
			return;
		}
		this.effectResponse = new Dictionary<CrittersManager.CritterEvent, GameObject>();
		for (int i = 0; i < this.eventEffects.Length; i++)
		{
			if (this.eventEffects[i].effect != null)
			{
				this.effectResponse.Add(this.eventEffects[i].eventType, this.eventEffects[i].effect);
			}
		}
		this.manager.OnCritterEventReceived += new Action<CrittersManager.CritterEvent, int, Vector3, Quaternion>(this.HandleReceivedEvent);
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000A59C File Offset: 0x0000879C
	private void HandleReceivedEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position, Quaternion rotation)
	{
		GameObject prefab;
		if (this.effectResponse.TryGetValue(eventType, ref prefab))
		{
			GameObject pooled = CrittersPool.GetPooled(prefab);
			if (pooled.IsNotNull())
			{
				pooled.transform.position = position;
				pooled.transform.rotation = rotation;
			}
		}
	}

	// Token: 0x040001EF RID: 495
	public CrittersManager manager;

	// Token: 0x040001F0 RID: 496
	public CrittersEventEffects.CrittersEventResponse[] eventEffects;

	// Token: 0x040001F1 RID: 497
	private Dictionary<CrittersManager.CritterEvent, GameObject> effectResponse;

	// Token: 0x02000057 RID: 87
	[Serializable]
	public class CrittersEventResponse
	{
		// Token: 0x040001F2 RID: 498
		public CrittersManager.CritterEvent eventType;

		// Token: 0x040001F3 RID: 499
		public GameObject effect;
	}
}
