using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F46 RID: 3910
	public class StandImport
	{
		// Token: 0x06006207 RID: 25095 RVA: 0x001F9718 File Offset: 0x001F7918
		public void DecomposeFromTitleDataString(string data)
		{
			string[] array = data.Split("\\n", 0);
			for (int i = 0; i < array.Length; i++)
			{
				this.DecomposeStandDataTitleData(array[i]);
			}
		}

		// Token: 0x06006208 RID: 25096 RVA: 0x001F974C File Offset: 0x001F794C
		public void DecomposeStandDataTitleData(string dataString)
		{
			string[] array = dataString.Split("\\t", 0);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string text2 in array)
			{
				text = text + text2 + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x06006209 RID: 25097 RVA: 0x001F97CF File Offset: 0x001F79CF
		public void DeserializeFromJSON(string JSONString)
		{
			this.standData = JsonConvert.DeserializeObject<List<StandTypeData>>(JSONString);
		}

		// Token: 0x0600620A RID: 25098 RVA: 0x001F97E0 File Offset: 0x001F79E0
		public void DecomposeStandData(string dataString)
		{
			string[] array = dataString.Split('\t', 0);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string text2 in array)
			{
				text = text + text2 + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x040070B1 RID: 28849
		public List<StandTypeData> standData = new List<StandTypeData>();
	}
}
