using System;
using System.Collections.Generic;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F4F RID: 3919
	public class StoreUpdateEvent
	{
		// Token: 0x06006232 RID: 25138 RVA: 0x00002050 File Offset: 0x00000250
		public StoreUpdateEvent()
		{
		}

		// Token: 0x06006233 RID: 25139 RVA: 0x001FA03C File Offset: 0x001F823C
		public StoreUpdateEvent(string pedestalID, string itemName, DateTime startTimeUTC, DateTime endTimeUTC)
		{
			this.PedestalID = pedestalID;
			this.ItemName = itemName;
			this.StartTimeUTC = startTimeUTC;
			this.EndTimeUTC = endTimeUTC;
		}

		// Token: 0x06006234 RID: 25140 RVA: 0x001FA061 File Offset: 0x001F8261
		public static string SerializeAsJSon(StoreUpdateEvent storeEvent)
		{
			return JsonUtility.ToJson(storeEvent);
		}

		// Token: 0x06006235 RID: 25141 RVA: 0x001FA069 File Offset: 0x001F8269
		public static string SerializeArrayAsJSon(StoreUpdateEvent[] storeEvents)
		{
			return JsonConvert.SerializeObject(storeEvents);
		}

		// Token: 0x06006236 RID: 25142 RVA: 0x001FA071 File Offset: 0x001F8271
		public static StoreUpdateEvent DeserializeFromJSon(string json)
		{
			return JsonUtility.FromJson<StoreUpdateEvent>(json);
		}

		// Token: 0x06006237 RID: 25143 RVA: 0x001FA079 File Offset: 0x001F8279
		public static StoreUpdateEvent[] DeserializeFromJSonArray(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list.ToArray();
		}

		// Token: 0x06006238 RID: 25144 RVA: 0x001FA0AB File Offset: 0x001F82AB
		public static List<StoreUpdateEvent> DeserializeFromJSonList(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list;
		}

		// Token: 0x040070E0 RID: 28896
		public string PedestalID;

		// Token: 0x040070E1 RID: 28897
		public string ItemName;

		// Token: 0x040070E2 RID: 28898
		public DateTime StartTimeUTC;

		// Token: 0x040070E3 RID: 28899
		public DateTime EndTimeUTC;
	}
}
