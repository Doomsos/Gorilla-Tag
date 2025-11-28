using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
	// Token: 0x02000D53 RID: 3411
	public class JsonData : IJsonWrapper, IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary, IEquatable<JsonData>
	{
		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06005316 RID: 21270 RVA: 0x001A57A6 File Offset: 0x001A39A6
		public int Count
		{
			get
			{
				return this.EnsureCollection().Count;
			}
		}

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x06005317 RID: 21271 RVA: 0x001A57B3 File Offset: 0x001A39B3
		public bool IsArray
		{
			get
			{
				return this.type == JsonType.Array;
			}
		}

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x06005318 RID: 21272 RVA: 0x001A57BE File Offset: 0x001A39BE
		public bool IsBoolean
		{
			get
			{
				return this.type == JsonType.Boolean;
			}
		}

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06005319 RID: 21273 RVA: 0x001A57C9 File Offset: 0x001A39C9
		public bool IsDouble
		{
			get
			{
				return this.type == JsonType.Double;
			}
		}

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x0600531A RID: 21274 RVA: 0x001A57D4 File Offset: 0x001A39D4
		public bool IsInt
		{
			get
			{
				return this.type == JsonType.Int;
			}
		}

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x0600531B RID: 21275 RVA: 0x001A57DF File Offset: 0x001A39DF
		public bool IsLong
		{
			get
			{
				return this.type == JsonType.Long;
			}
		}

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x0600531C RID: 21276 RVA: 0x001A57EA File Offset: 0x001A39EA
		public bool IsObject
		{
			get
			{
				return this.type == JsonType.Object;
			}
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x0600531D RID: 21277 RVA: 0x001A57F5 File Offset: 0x001A39F5
		public bool IsString
		{
			get
			{
				return this.type == JsonType.String;
			}
		}

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x0600531E RID: 21278 RVA: 0x001A5800 File Offset: 0x001A3A00
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x0600531F RID: 21279 RVA: 0x001A5808 File Offset: 0x001A3A08
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.EnsureCollection().IsSynchronized;
			}
		}

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06005320 RID: 21280 RVA: 0x001A5815 File Offset: 0x001A3A15
		object ICollection.SyncRoot
		{
			get
			{
				return this.EnsureCollection().SyncRoot;
			}
		}

		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06005321 RID: 21281 RVA: 0x001A5822 File Offset: 0x001A3A22
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.EnsureDictionary().IsFixedSize;
			}
		}

		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06005322 RID: 21282 RVA: 0x001A582F File Offset: 0x001A3A2F
		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.EnsureDictionary().IsReadOnly;
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06005323 RID: 21283 RVA: 0x001A583C File Offset: 0x001A3A3C
		ICollection IDictionary.Keys
		{
			get
			{
				this.EnsureDictionary();
				IList<string> list = new List<string>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Key);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x06005324 RID: 21284 RVA: 0x001A58A4 File Offset: 0x001A3AA4
		ICollection IDictionary.Values
		{
			get
			{
				this.EnsureDictionary();
				IList<JsonData> list = new List<JsonData>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Value);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x06005325 RID: 21285 RVA: 0x001A590C File Offset: 0x001A3B0C
		bool IJsonWrapper.IsArray
		{
			get
			{
				return this.IsArray;
			}
		}

		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06005326 RID: 21286 RVA: 0x001A5914 File Offset: 0x001A3B14
		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return this.IsBoolean;
			}
		}

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06005327 RID: 21287 RVA: 0x001A591C File Offset: 0x001A3B1C
		bool IJsonWrapper.IsDouble
		{
			get
			{
				return this.IsDouble;
			}
		}

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06005328 RID: 21288 RVA: 0x001A5924 File Offset: 0x001A3B24
		bool IJsonWrapper.IsInt
		{
			get
			{
				return this.IsInt;
			}
		}

		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x06005329 RID: 21289 RVA: 0x001A592C File Offset: 0x001A3B2C
		bool IJsonWrapper.IsLong
		{
			get
			{
				return this.IsLong;
			}
		}

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x0600532A RID: 21290 RVA: 0x001A5934 File Offset: 0x001A3B34
		bool IJsonWrapper.IsObject
		{
			get
			{
				return this.IsObject;
			}
		}

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x0600532B RID: 21291 RVA: 0x001A593C File Offset: 0x001A3B3C
		bool IJsonWrapper.IsString
		{
			get
			{
				return this.IsString;
			}
		}

		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x0600532C RID: 21292 RVA: 0x001A5944 File Offset: 0x001A3B44
		bool IList.IsFixedSize
		{
			get
			{
				return this.EnsureList().IsFixedSize;
			}
		}

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x0600532D RID: 21293 RVA: 0x001A5951 File Offset: 0x001A3B51
		bool IList.IsReadOnly
		{
			get
			{
				return this.EnsureList().IsReadOnly;
			}
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x0600532E RID: 21294 RVA: 0x001A595E File Offset: 0x001A3B5E
		// (set) Token: 0x0600532F RID: 21295 RVA: 0x001A596C File Offset: 0x001A3B6C
		object IDictionary.Item
		{
			get
			{
				return this.EnsureDictionary()[key];
			}
			set
			{
				if (!(key is string))
				{
					throw new ArgumentException("The key has to be a string");
				}
				JsonData value2 = this.ToJsonData(value);
				this[(string)key] = value2;
			}
		}

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x06005330 RID: 21296 RVA: 0x001A59A4 File Offset: 0x001A3BA4
		// (set) Token: 0x06005331 RID: 21297 RVA: 0x001A59CC File Offset: 0x001A3BCC
		object IOrderedDictionary.Item
		{
			get
			{
				this.EnsureDictionary();
				return this.object_list[idx].Value;
			}
			set
			{
				this.EnsureDictionary();
				JsonData jsonData = this.ToJsonData(value);
				KeyValuePair<string, JsonData> keyValuePair = this.object_list[idx];
				this.inst_object[keyValuePair.Key] = jsonData;
				KeyValuePair<string, JsonData> keyValuePair2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, jsonData);
				this.object_list[idx] = keyValuePair2;
			}
		}

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x06005332 RID: 21298 RVA: 0x001A5A24 File Offset: 0x001A3C24
		// (set) Token: 0x06005333 RID: 21299 RVA: 0x001A5A34 File Offset: 0x001A3C34
		object IList.Item
		{
			get
			{
				return this.EnsureList()[index];
			}
			set
			{
				this.EnsureList();
				JsonData value2 = this.ToJsonData(value);
				this[index] = value2;
			}
		}

		// Token: 0x170007E5 RID: 2021
		public JsonData this[string prop_name]
		{
			get
			{
				this.EnsureDictionary();
				return this.inst_object[prop_name];
			}
			set
			{
				this.EnsureDictionary();
				KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
				if (this.inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < this.object_list.Count; i++)
					{
						if (this.object_list[i].Key == prop_name)
						{
							this.object_list[i] = keyValuePair;
							break;
						}
					}
				}
				else
				{
					this.object_list.Add(keyValuePair);
				}
				this.inst_object[prop_name] = value;
				this.json = null;
			}
		}

		// Token: 0x170007E6 RID: 2022
		public JsonData this[int index]
		{
			get
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					return this.inst_array[index];
				}
				return this.object_list[index].Value;
			}
			set
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					this.inst_array[index] = value;
				}
				else
				{
					KeyValuePair<string, JsonData> keyValuePair = this.object_list[index];
					KeyValuePair<string, JsonData> keyValuePair2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
					this.object_list[index] = keyValuePair2;
					this.inst_object[keyValuePair.Key] = value;
				}
				this.json = null;
			}
		}

		// Token: 0x06005338 RID: 21304 RVA: 0x00002050 File Offset: 0x00000250
		public JsonData()
		{
		}

		// Token: 0x06005339 RID: 21305 RVA: 0x001A5BAF File Offset: 0x001A3DAF
		public JsonData(bool boolean)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = boolean;
		}

		// Token: 0x0600533A RID: 21306 RVA: 0x001A5BC5 File Offset: 0x001A3DC5
		public JsonData(double number)
		{
			this.type = JsonType.Double;
			this.inst_double = number;
		}

		// Token: 0x0600533B RID: 21307 RVA: 0x001A5BDB File Offset: 0x001A3DDB
		public JsonData(int number)
		{
			this.type = JsonType.Int;
			this.inst_int = number;
		}

		// Token: 0x0600533C RID: 21308 RVA: 0x001A5BF1 File Offset: 0x001A3DF1
		public JsonData(long number)
		{
			this.type = JsonType.Long;
			this.inst_long = number;
		}

		// Token: 0x0600533D RID: 21309 RVA: 0x001A5C08 File Offset: 0x001A3E08
		public JsonData(object obj)
		{
			if (obj is bool)
			{
				this.type = JsonType.Boolean;
				this.inst_boolean = (bool)obj;
				return;
			}
			if (obj is double)
			{
				this.type = JsonType.Double;
				this.inst_double = (double)obj;
				return;
			}
			if (obj is int)
			{
				this.type = JsonType.Int;
				this.inst_int = (int)obj;
				return;
			}
			if (obj is long)
			{
				this.type = JsonType.Long;
				this.inst_long = (long)obj;
				return;
			}
			if (obj is string)
			{
				this.type = JsonType.String;
				this.inst_string = (string)obj;
				return;
			}
			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		// Token: 0x0600533E RID: 21310 RVA: 0x001A5CB1 File Offset: 0x001A3EB1
		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		// Token: 0x0600533F RID: 21311 RVA: 0x001A5CC7 File Offset: 0x001A3EC7
		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005340 RID: 21312 RVA: 0x001A5CCF File Offset: 0x001A3ECF
		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005341 RID: 21313 RVA: 0x001A5CD7 File Offset: 0x001A3ED7
		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005342 RID: 21314 RVA: 0x001A5CDF File Offset: 0x001A3EDF
		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005343 RID: 21315 RVA: 0x001A5CE7 File Offset: 0x001A3EE7
		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005344 RID: 21316 RVA: 0x001A5CEF File Offset: 0x001A3EEF
		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		// Token: 0x06005345 RID: 21317 RVA: 0x001A5D0B File Offset: 0x001A3F0B
		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		// Token: 0x06005346 RID: 21318 RVA: 0x001A5D27 File Offset: 0x001A3F27
		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		// Token: 0x06005347 RID: 21319 RVA: 0x001A5D43 File Offset: 0x001A3F43
		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		// Token: 0x06005348 RID: 21320 RVA: 0x001A5D5F File Offset: 0x001A3F5F
		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}

		// Token: 0x06005349 RID: 21321 RVA: 0x001A5D7B File Offset: 0x001A3F7B
		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		// Token: 0x0600534A RID: 21322 RVA: 0x001A5D8C File Offset: 0x001A3F8C
		void IDictionary.Add(object key, object value)
		{
			JsonData jsonData = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, jsonData);
			KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>((string)key, jsonData);
			this.object_list.Add(keyValuePair);
			this.json = null;
		}

		// Token: 0x0600534B RID: 21323 RVA: 0x001A5DCF File Offset: 0x001A3FCF
		void IDictionary.Clear()
		{
			this.EnsureDictionary().Clear();
			this.object_list.Clear();
			this.json = null;
		}

		// Token: 0x0600534C RID: 21324 RVA: 0x001A5DEE File Offset: 0x001A3FEE
		bool IDictionary.Contains(object key)
		{
			return this.EnsureDictionary().Contains(key);
		}

		// Token: 0x0600534D RID: 21325 RVA: 0x001A5DFC File Offset: 0x001A3FFC
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600534E RID: 21326 RVA: 0x001A5E04 File Offset: 0x001A4004
		void IDictionary.Remove(object key)
		{
			this.EnsureDictionary().Remove(key);
			for (int i = 0; i < this.object_list.Count; i++)
			{
				if (this.object_list[i].Key == (string)key)
				{
					this.object_list.RemoveAt(i);
					break;
				}
			}
			this.json = null;
		}

		// Token: 0x0600534F RID: 21327 RVA: 0x001A5E69 File Offset: 0x001A4069
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnsureCollection().GetEnumerator();
		}

		// Token: 0x06005350 RID: 21328 RVA: 0x001A5E76 File Offset: 0x001A4076
		bool IJsonWrapper.GetBoolean()
		{
			if (this.type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return this.inst_boolean;
		}

		// Token: 0x06005351 RID: 21329 RVA: 0x001A5E92 File Offset: 0x001A4092
		double IJsonWrapper.GetDouble()
		{
			if (this.type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return this.inst_double;
		}

		// Token: 0x06005352 RID: 21330 RVA: 0x001A5EAE File Offset: 0x001A40AE
		int IJsonWrapper.GetInt()
		{
			if (this.type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return this.inst_int;
		}

		// Token: 0x06005353 RID: 21331 RVA: 0x001A5ECA File Offset: 0x001A40CA
		long IJsonWrapper.GetLong()
		{
			if (this.type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return this.inst_long;
		}

		// Token: 0x06005354 RID: 21332 RVA: 0x001A5EE6 File Offset: 0x001A40E6
		string IJsonWrapper.GetString()
		{
			if (this.type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return this.inst_string;
		}

		// Token: 0x06005355 RID: 21333 RVA: 0x001A5F02 File Offset: 0x001A4102
		void IJsonWrapper.SetBoolean(bool val)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = val;
			this.json = null;
		}

		// Token: 0x06005356 RID: 21334 RVA: 0x001A5F19 File Offset: 0x001A4119
		void IJsonWrapper.SetDouble(double val)
		{
			this.type = JsonType.Double;
			this.inst_double = val;
			this.json = null;
		}

		// Token: 0x06005357 RID: 21335 RVA: 0x001A5F30 File Offset: 0x001A4130
		void IJsonWrapper.SetInt(int val)
		{
			this.type = JsonType.Int;
			this.inst_int = val;
			this.json = null;
		}

		// Token: 0x06005358 RID: 21336 RVA: 0x001A5F47 File Offset: 0x001A4147
		void IJsonWrapper.SetLong(long val)
		{
			this.type = JsonType.Long;
			this.inst_long = val;
			this.json = null;
		}

		// Token: 0x06005359 RID: 21337 RVA: 0x001A5F5E File Offset: 0x001A415E
		void IJsonWrapper.SetString(string val)
		{
			this.type = JsonType.String;
			this.inst_string = val;
			this.json = null;
		}

		// Token: 0x0600535A RID: 21338 RVA: 0x001A5F75 File Offset: 0x001A4175
		string IJsonWrapper.ToJson()
		{
			return this.ToJson();
		}

		// Token: 0x0600535B RID: 21339 RVA: 0x001A5F7D File Offset: 0x001A417D
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			this.ToJson(writer);
		}

		// Token: 0x0600535C RID: 21340 RVA: 0x001A5F86 File Offset: 0x001A4186
		int IList.Add(object value)
		{
			return this.Add(value);
		}

		// Token: 0x0600535D RID: 21341 RVA: 0x001A5F8F File Offset: 0x001A418F
		void IList.Clear()
		{
			this.EnsureList().Clear();
			this.json = null;
		}

		// Token: 0x0600535E RID: 21342 RVA: 0x001A5FA3 File Offset: 0x001A41A3
		bool IList.Contains(object value)
		{
			return this.EnsureList().Contains(value);
		}

		// Token: 0x0600535F RID: 21343 RVA: 0x001A5FB1 File Offset: 0x001A41B1
		int IList.IndexOf(object value)
		{
			return this.EnsureList().IndexOf(value);
		}

		// Token: 0x06005360 RID: 21344 RVA: 0x001A5FBF File Offset: 0x001A41BF
		void IList.Insert(int index, object value)
		{
			this.EnsureList().Insert(index, value);
			this.json = null;
		}

		// Token: 0x06005361 RID: 21345 RVA: 0x001A5FD5 File Offset: 0x001A41D5
		void IList.Remove(object value)
		{
			this.EnsureList().Remove(value);
			this.json = null;
		}

		// Token: 0x06005362 RID: 21346 RVA: 0x001A5FEA File Offset: 0x001A41EA
		void IList.RemoveAt(int index)
		{
			this.EnsureList().RemoveAt(index);
			this.json = null;
		}

		// Token: 0x06005363 RID: 21347 RVA: 0x001A5FFF File Offset: 0x001A41FF
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		// Token: 0x06005364 RID: 21348 RVA: 0x001A6018 File Offset: 0x001A4218
		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData jsonData = this.ToJsonData(value);
			this[text] = jsonData;
			KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(text, jsonData);
			this.object_list.Insert(idx, keyValuePair);
		}

		// Token: 0x06005365 RID: 21349 RVA: 0x001A6054 File Offset: 0x001A4254
		void IOrderedDictionary.RemoveAt(int idx)
		{
			this.EnsureDictionary();
			this.inst_object.Remove(this.object_list[idx].Key);
			this.object_list.RemoveAt(idx);
		}

		// Token: 0x06005366 RID: 21350 RVA: 0x001A6094 File Offset: 0x001A4294
		private ICollection EnsureCollection()
		{
			if (this.type == JsonType.Array)
			{
				return (ICollection)this.inst_array;
			}
			if (this.type == JsonType.Object)
			{
				return (ICollection)this.inst_object;
			}
			throw new InvalidOperationException("The JsonData instance has to be initialized first");
		}

		// Token: 0x06005367 RID: 21351 RVA: 0x001A60CC File Offset: 0x001A42CC
		private IDictionary EnsureDictionary()
		{
			if (this.type == JsonType.Object)
			{
				return (IDictionary)this.inst_object;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a dictionary");
			}
			this.type = JsonType.Object;
			this.inst_object = new Dictionary<string, JsonData>();
			this.object_list = new List<KeyValuePair<string, JsonData>>();
			return (IDictionary)this.inst_object;
		}

		// Token: 0x06005368 RID: 21352 RVA: 0x001A612C File Offset: 0x001A432C
		private IList EnsureList()
		{
			if (this.type == JsonType.Array)
			{
				return (IList)this.inst_array;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a list");
			}
			this.type = JsonType.Array;
			this.inst_array = new List<JsonData>();
			return (IList)this.inst_array;
		}

		// Token: 0x06005369 RID: 21353 RVA: 0x001A617E File Offset: 0x001A437E
		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is JsonData)
			{
				return (JsonData)obj;
			}
			return new JsonData(obj);
		}

		// Token: 0x0600536A RID: 21354 RVA: 0x001A619C File Offset: 0x001A439C
		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}
			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}
			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}
			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}
			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}
			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object obj2 in obj)
				{
					JsonData.WriteJson((JsonData)obj2, writer);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj.IsObject)
			{
				writer.WriteObjectStart();
				foreach (object obj3 in obj)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonData.WriteJson((JsonData)dictionaryEntry.Value, writer);
				}
				writer.WriteObjectEnd();
				return;
			}
		}

		// Token: 0x0600536B RID: 21355 RVA: 0x001A62E4 File Offset: 0x001A44E4
		public int Add(object value)
		{
			JsonData jsonData = this.ToJsonData(value);
			this.json = null;
			return this.EnsureList().Add(jsonData);
		}

		// Token: 0x0600536C RID: 21356 RVA: 0x001A630C File Offset: 0x001A450C
		public void Clear()
		{
			if (this.IsObject)
			{
				this.Clear();
				return;
			}
			if (this.IsArray)
			{
				this.Clear();
				return;
			}
		}

		// Token: 0x0600536D RID: 21357 RVA: 0x001A632C File Offset: 0x001A452C
		public bool Equals(JsonData x)
		{
			if (x == null)
			{
				return false;
			}
			if (x.type != this.type)
			{
				return false;
			}
			switch (this.type)
			{
			case JsonType.None:
				return true;
			case JsonType.Object:
				return this.inst_object.Equals(x.inst_object);
			case JsonType.Array:
				return this.inst_array.Equals(x.inst_array);
			case JsonType.String:
				return this.inst_string.Equals(x.inst_string);
			case JsonType.Int:
				return this.inst_int.Equals(x.inst_int);
			case JsonType.Long:
				return this.inst_long.Equals(x.inst_long);
			case JsonType.Double:
				return this.inst_double.Equals(x.inst_double);
			case JsonType.Boolean:
				return this.inst_boolean.Equals(x.inst_boolean);
			default:
				return false;
			}
		}

		// Token: 0x0600536E RID: 21358 RVA: 0x001A6401 File Offset: 0x001A4601
		public JsonType GetJsonType()
		{
			return this.type;
		}

		// Token: 0x0600536F RID: 21359 RVA: 0x001A640C File Offset: 0x001A460C
		public void SetJsonType(JsonType type)
		{
			if (this.type == type)
			{
				return;
			}
			switch (type)
			{
			case JsonType.Object:
				this.inst_object = new Dictionary<string, JsonData>();
				this.object_list = new List<KeyValuePair<string, JsonData>>();
				break;
			case JsonType.Array:
				this.inst_array = new List<JsonData>();
				break;
			case JsonType.String:
				this.inst_string = null;
				break;
			case JsonType.Int:
				this.inst_int = 0;
				break;
			case JsonType.Long:
				this.inst_long = 0L;
				break;
			case JsonType.Double:
				this.inst_double = 0.0;
				break;
			case JsonType.Boolean:
				this.inst_boolean = false;
				break;
			}
			this.type = type;
		}

		// Token: 0x06005370 RID: 21360 RVA: 0x001A64AC File Offset: 0x001A46AC
		public string ToJson()
		{
			if (this.json != null)
			{
				return this.json;
			}
			StringWriter stringWriter = new StringWriter();
			JsonData.WriteJson(this, new JsonWriter(stringWriter)
			{
				Validate = false
			});
			this.json = stringWriter.ToString();
			return this.json;
		}

		// Token: 0x06005371 RID: 21361 RVA: 0x001A64F8 File Offset: 0x001A46F8
		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			JsonData.WriteJson(this, writer);
			writer.Validate = validate;
		}

		// Token: 0x06005372 RID: 21362 RVA: 0x001A6524 File Offset: 0x001A4724
		public override string ToString()
		{
			switch (this.type)
			{
			case JsonType.Object:
				return "JsonData object";
			case JsonType.Array:
				return "JsonData array";
			case JsonType.String:
				return this.inst_string;
			case JsonType.Int:
				return this.inst_int.ToString();
			case JsonType.Long:
				return this.inst_long.ToString();
			case JsonType.Double:
				return this.inst_double.ToString();
			case JsonType.Boolean:
				return this.inst_boolean.ToString();
			default:
				return "Uninitialized JsonData";
			}
		}

		// Token: 0x04006111 RID: 24849
		private IList<JsonData> inst_array;

		// Token: 0x04006112 RID: 24850
		private bool inst_boolean;

		// Token: 0x04006113 RID: 24851
		private double inst_double;

		// Token: 0x04006114 RID: 24852
		private int inst_int;

		// Token: 0x04006115 RID: 24853
		private long inst_long;

		// Token: 0x04006116 RID: 24854
		private IDictionary<string, JsonData> inst_object;

		// Token: 0x04006117 RID: 24855
		private string inst_string;

		// Token: 0x04006118 RID: 24856
		private string json;

		// Token: 0x04006119 RID: 24857
		private JsonType type;

		// Token: 0x0400611A RID: 24858
		private IList<KeyValuePair<string, JsonData>> object_list;
	}
}
