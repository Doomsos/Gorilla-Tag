using System;
using System.Collections;
using System.Collections.Specialized;

namespace LitJson
{
	// Token: 0x02000D52 RID: 3410
	public interface IJsonWrapper : IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary
	{
		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06005301 RID: 21249
		bool IsArray { get; }

		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x06005302 RID: 21250
		bool IsBoolean { get; }

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06005303 RID: 21251
		bool IsDouble { get; }

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x06005304 RID: 21252
		bool IsInt { get; }

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06005305 RID: 21253
		bool IsLong { get; }

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06005306 RID: 21254
		bool IsObject { get; }

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06005307 RID: 21255
		bool IsString { get; }

		// Token: 0x06005308 RID: 21256
		bool GetBoolean();

		// Token: 0x06005309 RID: 21257
		double GetDouble();

		// Token: 0x0600530A RID: 21258
		int GetInt();

		// Token: 0x0600530B RID: 21259
		JsonType GetJsonType();

		// Token: 0x0600530C RID: 21260
		long GetLong();

		// Token: 0x0600530D RID: 21261
		string GetString();

		// Token: 0x0600530E RID: 21262
		void SetBoolean(bool val);

		// Token: 0x0600530F RID: 21263
		void SetDouble(double val);

		// Token: 0x06005310 RID: 21264
		void SetInt(int val);

		// Token: 0x06005311 RID: 21265
		void SetJsonType(JsonType type);

		// Token: 0x06005312 RID: 21266
		void SetLong(long val);

		// Token: 0x06005313 RID: 21267
		void SetString(string val);

		// Token: 0x06005314 RID: 21268
		string ToJson();

		// Token: 0x06005315 RID: 21269
		void ToJson(JsonWriter writer);
	}
}
