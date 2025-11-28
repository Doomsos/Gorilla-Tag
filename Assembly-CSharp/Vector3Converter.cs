using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Token: 0x02000C51 RID: 3153
public class Vector3Converter : JsonConverter
{
	// Token: 0x06004D1F RID: 19743 RVA: 0x0019028C File Offset: 0x0018E48C
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Vector3 vector = (Vector3)value;
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(vector.x);
		writer.WritePropertyName("y");
		writer.WriteValue(vector.y);
		writer.WritePropertyName("z");
		writer.WriteValue(vector.z);
		writer.WriteEndObject();
	}

	// Token: 0x06004D20 RID: 19744 RVA: 0x001902F4 File Offset: 0x0018E4F4
	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JObject jobject = JObject.Load(reader);
		return new Vector3((float)jobject["x"], (float)jobject["y"], (float)jobject["z"]);
	}

	// Token: 0x06004D21 RID: 19745 RVA: 0x00190345 File Offset: 0x0018E545
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector3);
	}
}
