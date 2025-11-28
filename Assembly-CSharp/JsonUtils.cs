using System;
using Newtonsoft.Json;

// Token: 0x02000C50 RID: 3152
public static class JsonUtils
{
	// Token: 0x06004D1B RID: 19739 RVA: 0x001901F7 File Offset: 0x0018E3F7
	public static string ToJson<T>(this T obj, bool indent = true)
	{
		return JsonConvert.SerializeObject(obj, indent ? 1 : 0);
	}

	// Token: 0x06004D1C RID: 19740 RVA: 0x0019020B File Offset: 0x0018E40B
	public static T FromJson<T>(this string s)
	{
		return JsonConvert.DeserializeObject<T>(s);
	}

	// Token: 0x06004D1D RID: 19741 RVA: 0x00190214 File Offset: 0x0018E414
	public static string JsonSerializeEventData<T>(this T obj)
	{
		JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = 3,
			CheckAdditionalContent = true,
			Formatting = 0
		};
		jsonSerializerSettings.Converters.Add(new Vector3Converter());
		return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
	}

	// Token: 0x06004D1E RID: 19742 RVA: 0x00190258 File Offset: 0x0018E458
	public static T JsonDeserializeEventData<T>(this string s)
	{
		JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = 3
		};
		jsonSerializerSettings.Converters.Add(new Vector3Converter());
		return JsonConvert.DeserializeObject<T>(s, jsonSerializerSettings);
	}
}
