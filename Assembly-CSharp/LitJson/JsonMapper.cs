using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LitJson
{
	// Token: 0x02000D5E RID: 3422
	public class JsonMapper
	{
		// Token: 0x060053A1 RID: 21409 RVA: 0x001A6740 File Offset: 0x001A4940
		static JsonMapper()
		{
			JsonMapper.max_nesting_depth = 100;
			JsonMapper.array_metadata = new Dictionary<Type, ArrayMetadata>();
			JsonMapper.conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
			JsonMapper.object_metadata = new Dictionary<Type, ObjectMetadata>();
			JsonMapper.type_properties = new Dictionary<Type, IList<PropertyMetadata>>();
			JsonMapper.static_writer = new JsonWriter();
			JsonMapper.datetime_format = DateTimeFormatInfo.InvariantInfo;
			JsonMapper.base_exporters_table = new Dictionary<Type, ExporterFunc>();
			JsonMapper.custom_exporters_table = new Dictionary<Type, ExporterFunc>();
			JsonMapper.base_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
			JsonMapper.custom_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
			JsonMapper.RegisterBaseExporters();
			JsonMapper.RegisterBaseImporters();
		}

		// Token: 0x060053A2 RID: 21410 RVA: 0x001A67F4 File Offset: 0x001A49F4
		private static void AddArrayMetadata(Type type)
		{
			if (JsonMapper.array_metadata.ContainsKey(type))
			{
				return;
			}
			ArrayMetadata arrayMetadata = default(ArrayMetadata);
			arrayMetadata.IsArray = type.IsArray;
			if (type.GetInterface("System.Collections.IList") != null)
			{
				arrayMetadata.IsList = true;
			}
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				if (!(propertyInfo.Name != "Item"))
				{
					ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
					if (indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(int))
					{
						arrayMetadata.ElementType = propertyInfo.PropertyType;
					}
				}
			}
			object obj = JsonMapper.array_metadata_lock;
			lock (obj)
			{
				try
				{
					JsonMapper.array_metadata.Add(type, arrayMetadata);
				}
				catch (ArgumentException)
				{
				}
			}
		}

		// Token: 0x060053A3 RID: 21411 RVA: 0x001A68EC File Offset: 0x001A4AEC
		private static void AddObjectMetadata(Type type)
		{
			if (JsonMapper.object_metadata.ContainsKey(type))
			{
				return;
			}
			ObjectMetadata objectMetadata = default(ObjectMetadata);
			if (type.GetInterface("System.Collections.IDictionary") != null)
			{
				objectMetadata.IsDictionary = true;
			}
			objectMetadata.Properties = new Dictionary<string, PropertyMetadata>();
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				if (propertyInfo.Name == "Item")
				{
					ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
					if (indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(string))
					{
						objectMetadata.ElementType = propertyInfo.PropertyType;
					}
				}
				else
				{
					PropertyMetadata propertyMetadata = default(PropertyMetadata);
					propertyMetadata.Info = propertyInfo;
					propertyMetadata.Type = propertyInfo.PropertyType;
					objectMetadata.Properties.Add(propertyInfo.Name, propertyMetadata);
				}
			}
			foreach (FieldInfo fieldInfo in type.GetFields())
			{
				PropertyMetadata propertyMetadata2 = default(PropertyMetadata);
				propertyMetadata2.Info = fieldInfo;
				propertyMetadata2.IsField = true;
				propertyMetadata2.Type = fieldInfo.FieldType;
				objectMetadata.Properties.Add(fieldInfo.Name, propertyMetadata2);
			}
			object obj = JsonMapper.object_metadata_lock;
			lock (obj)
			{
				try
				{
					JsonMapper.object_metadata.Add(type, objectMetadata);
				}
				catch (ArgumentException)
				{
				}
			}
		}

		// Token: 0x060053A4 RID: 21412 RVA: 0x001A6A78 File Offset: 0x001A4C78
		private static void AddTypeProperties(Type type)
		{
			if (JsonMapper.type_properties.ContainsKey(type))
			{
				return;
			}
			IList<PropertyMetadata> list = new List<PropertyMetadata>();
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				if (!(propertyInfo.Name == "Item"))
				{
					list.Add(new PropertyMetadata
					{
						Info = propertyInfo,
						IsField = false
					});
				}
			}
			foreach (FieldInfo info in type.GetFields())
			{
				list.Add(new PropertyMetadata
				{
					Info = info,
					IsField = true
				});
			}
			object obj = JsonMapper.type_properties_lock;
			lock (obj)
			{
				try
				{
					JsonMapper.type_properties.Add(type, list);
				}
				catch (ArgumentException)
				{
				}
			}
		}

		// Token: 0x060053A5 RID: 21413 RVA: 0x001A6B70 File Offset: 0x001A4D70
		private static MethodInfo GetConvOp(Type t1, Type t2)
		{
			object obj = JsonMapper.conv_ops_lock;
			lock (obj)
			{
				if (!JsonMapper.conv_ops.ContainsKey(t1))
				{
					JsonMapper.conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
				}
			}
			if (JsonMapper.conv_ops[t1].ContainsKey(t2))
			{
				return JsonMapper.conv_ops[t1][t2];
			}
			MethodInfo method = t1.GetMethod("op_Implicit", new Type[]
			{
				t2
			});
			obj = JsonMapper.conv_ops_lock;
			lock (obj)
			{
				try
				{
					JsonMapper.conv_ops[t1].Add(t2, method);
				}
				catch (ArgumentException)
				{
					return JsonMapper.conv_ops[t1][t2];
				}
			}
			return method;
		}

		// Token: 0x060053A6 RID: 21414 RVA: 0x001A6C60 File Offset: 0x001A4E60
		private static object ReadValue(Type inst_type, JsonReader reader)
		{
			reader.Read();
			if (reader.Token == JsonToken.ArrayEnd)
			{
				return null;
			}
			if (reader.Token == JsonToken.Null)
			{
				if (!inst_type.IsClass)
				{
					throw new JsonException(string.Format("Can't assign null to an instance of type {0}", inst_type));
				}
				return null;
			}
			else
			{
				if (reader.Token != JsonToken.Double && reader.Token != JsonToken.Int && reader.Token != JsonToken.Long && reader.Token != JsonToken.String && reader.Token != JsonToken.Boolean)
				{
					object obj = null;
					if (reader.Token == JsonToken.ArrayStart)
					{
						JsonMapper.AddArrayMetadata(inst_type);
						ArrayMetadata arrayMetadata = JsonMapper.array_metadata[inst_type];
						if (!arrayMetadata.IsArray && !arrayMetadata.IsList)
						{
							throw new JsonException(string.Format("Type {0} can't act as an array", inst_type));
						}
						IList list;
						Type elementType;
						if (!arrayMetadata.IsArray)
						{
							list = (IList)Activator.CreateInstance(inst_type);
							elementType = arrayMetadata.ElementType;
						}
						else
						{
							list = new ArrayList();
							elementType = inst_type.GetElementType();
						}
						for (;;)
						{
							object obj2 = JsonMapper.ReadValue(elementType, reader);
							if (reader.Token == JsonToken.ArrayEnd)
							{
								break;
							}
							list.Add(obj2);
						}
						if (arrayMetadata.IsArray)
						{
							int count = list.Count;
							obj = Array.CreateInstance(elementType, count);
							for (int i = 0; i < count; i++)
							{
								((Array)obj).SetValue(list[i], i);
							}
						}
						else
						{
							obj = list;
						}
					}
					else if (reader.Token == JsonToken.ObjectStart)
					{
						JsonMapper.AddObjectMetadata(inst_type);
						ObjectMetadata objectMetadata = JsonMapper.object_metadata[inst_type];
						obj = Activator.CreateInstance(inst_type);
						string text;
						for (;;)
						{
							reader.Read();
							if (reader.Token == JsonToken.ObjectEnd)
							{
								return obj;
							}
							text = (string)reader.Value;
							if (objectMetadata.Properties.ContainsKey(text))
							{
								PropertyMetadata propertyMetadata = objectMetadata.Properties[text];
								if (propertyMetadata.IsField)
								{
									((FieldInfo)propertyMetadata.Info).SetValue(obj, JsonMapper.ReadValue(propertyMetadata.Type, reader));
								}
								else
								{
									PropertyInfo propertyInfo = (PropertyInfo)propertyMetadata.Info;
									if (propertyInfo.CanWrite)
									{
										propertyInfo.SetValue(obj, JsonMapper.ReadValue(propertyMetadata.Type, reader), null);
									}
									else
									{
										JsonMapper.ReadValue(propertyMetadata.Type, reader);
									}
								}
							}
							else
							{
								if (!objectMetadata.IsDictionary)
								{
									break;
								}
								((IDictionary)obj).Add(text, JsonMapper.ReadValue(objectMetadata.ElementType, reader));
							}
						}
						throw new JsonException(string.Format("The type {0} doesn't have the property '{1}'", inst_type, text));
					}
					return obj;
				}
				Type type = reader.Value.GetType();
				if (inst_type.IsAssignableFrom(type))
				{
					return reader.Value;
				}
				if (JsonMapper.custom_importers_table.ContainsKey(type) && JsonMapper.custom_importers_table[type].ContainsKey(inst_type))
				{
					return JsonMapper.custom_importers_table[type][inst_type](reader.Value);
				}
				if (JsonMapper.base_importers_table.ContainsKey(type) && JsonMapper.base_importers_table[type].ContainsKey(inst_type))
				{
					return JsonMapper.base_importers_table[type][inst_type](reader.Value);
				}
				if (inst_type.IsEnum)
				{
					return Enum.ToObject(inst_type, reader.Value);
				}
				MethodInfo convOp = JsonMapper.GetConvOp(inst_type, type);
				if (convOp != null)
				{
					return convOp.Invoke(null, new object[]
					{
						reader.Value
					});
				}
				throw new JsonException(string.Format("Can't assign value '{0}' (type {1}) to type {2}", reader.Value, type, inst_type));
			}
		}

		// Token: 0x060053A7 RID: 21415 RVA: 0x001A6FB4 File Offset: 0x001A51B4
		private static IJsonWrapper ReadValue(WrapperFactory factory, JsonReader reader)
		{
			reader.Read();
			if (reader.Token == JsonToken.ArrayEnd || reader.Token == JsonToken.Null)
			{
				return null;
			}
			IJsonWrapper jsonWrapper = factory();
			if (reader.Token == JsonToken.String)
			{
				jsonWrapper.SetString((string)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Double)
			{
				jsonWrapper.SetDouble((double)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Int)
			{
				jsonWrapper.SetInt((int)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Long)
			{
				jsonWrapper.SetLong((long)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Boolean)
			{
				jsonWrapper.SetBoolean((bool)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.ArrayStart)
			{
				jsonWrapper.SetJsonType(JsonType.Array);
				for (;;)
				{
					IJsonWrapper jsonWrapper2 = JsonMapper.ReadValue(factory, reader);
					if (reader.Token == JsonToken.ArrayEnd)
					{
						break;
					}
					jsonWrapper.Add(jsonWrapper2);
				}
			}
			else if (reader.Token == JsonToken.ObjectStart)
			{
				jsonWrapper.SetJsonType(JsonType.Object);
				for (;;)
				{
					reader.Read();
					if (reader.Token == JsonToken.ObjectEnd)
					{
						break;
					}
					string text = (string)reader.Value;
					jsonWrapper[text] = JsonMapper.ReadValue(factory, reader);
				}
			}
			return jsonWrapper;
		}

		// Token: 0x060053A8 RID: 21416 RVA: 0x001A70DC File Offset: 0x001A52DC
		private static void RegisterBaseExporters()
		{
			JsonMapper.base_exporters_table[typeof(byte)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((byte)obj));
			};
			JsonMapper.base_exporters_table[typeof(char)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToString((char)obj));
			};
			JsonMapper.base_exporters_table[typeof(DateTime)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToString((DateTime)obj, JsonMapper.datetime_format));
			};
			JsonMapper.base_exporters_table[typeof(decimal)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write((decimal)obj);
			};
			JsonMapper.base_exporters_table[typeof(sbyte)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((sbyte)obj));
			};
			JsonMapper.base_exporters_table[typeof(short)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((short)obj));
			};
			JsonMapper.base_exporters_table[typeof(ushort)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((ushort)obj));
			};
			JsonMapper.base_exporters_table[typeof(uint)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToUInt64((uint)obj));
			};
			JsonMapper.base_exporters_table[typeof(ulong)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write((ulong)obj);
			};
			JsonMapper.base_exporters_table[typeof(float)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write((double)((float)obj));
			};
		}

		// Token: 0x060053A9 RID: 21417 RVA: 0x001A72E8 File Offset: 0x001A54E8
		private static void RegisterBaseImporters()
		{
			ImporterFunc importer = (object input) => Convert.ToByte((int)input);
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(byte), importer);
			importer = ((object input) => Convert.ToUInt64((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(ulong), importer);
			importer = ((object input) => Convert.ToSByte((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(sbyte), importer);
			importer = ((object input) => Convert.ToInt16((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(short), importer);
			importer = ((object input) => Convert.ToUInt16((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(ushort), importer);
			importer = ((object input) => Convert.ToUInt32((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(uint), importer);
			importer = ((object input) => Convert.ToSingle((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(float), importer);
			importer = ((object input) => Convert.ToSingle((float)((double)input)));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(double), typeof(float), importer);
			importer = ((object input) => Convert.ToDouble((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(double), importer);
			importer = ((object input) => Convert.ToDecimal((double)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(double), typeof(decimal), importer);
			importer = ((object input) => Convert.ToUInt32((long)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(long), typeof(uint), importer);
			importer = ((object input) => Convert.ToChar((string)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(string), typeof(char), importer);
			importer = ((object input) => Convert.ToDateTime((string)input, JsonMapper.datetime_format));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(string), typeof(DateTime), importer);
		}

		// Token: 0x060053AA RID: 21418 RVA: 0x001A7628 File Offset: 0x001A5828
		private static void RegisterImporter(IDictionary<Type, IDictionary<Type, ImporterFunc>> table, Type json_type, Type value_type, ImporterFunc importer)
		{
			if (!table.ContainsKey(json_type))
			{
				table.Add(json_type, new Dictionary<Type, ImporterFunc>());
			}
			table[json_type][value_type] = importer;
		}

		// Token: 0x060053AB RID: 21419 RVA: 0x001A7650 File Offset: 0x001A5850
		private static void WriteValue(object obj, JsonWriter writer, bool writer_is_private, int depth)
		{
			if (depth > JsonMapper.max_nesting_depth)
			{
				throw new JsonException(string.Format("Max allowed object depth reached while trying to export from type {0}", obj.GetType()));
			}
			if (obj == null)
			{
				writer.Write(null);
				return;
			}
			if (obj is IJsonWrapper)
			{
				if (writer_is_private)
				{
					writer.TextWriter.Write(((IJsonWrapper)obj).ToJson());
					return;
				}
				((IJsonWrapper)obj).ToJson(writer);
				return;
			}
			else
			{
				if (obj is string)
				{
					writer.Write((string)obj);
					return;
				}
				if (obj is double)
				{
					writer.Write((double)obj);
					return;
				}
				if (obj is int)
				{
					writer.Write((int)obj);
					return;
				}
				if (obj is bool)
				{
					writer.Write((bool)obj);
					return;
				}
				if (obj is long)
				{
					writer.Write((long)obj);
					return;
				}
				if (obj is Array)
				{
					writer.WriteArrayStart();
					foreach (object obj2 in ((Array)obj))
					{
						JsonMapper.WriteValue(obj2, writer, writer_is_private, depth + 1);
					}
					writer.WriteArrayEnd();
					return;
				}
				if (obj is IList)
				{
					writer.WriteArrayStart();
					foreach (object obj3 in ((IList)obj))
					{
						JsonMapper.WriteValue(obj3, writer, writer_is_private, depth + 1);
					}
					writer.WriteArrayEnd();
					return;
				}
				if (obj is IDictionary)
				{
					writer.WriteObjectStart();
					foreach (object obj4 in ((IDictionary)obj))
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj4;
						writer.WritePropertyName((string)dictionaryEntry.Key);
						JsonMapper.WriteValue(dictionaryEntry.Value, writer, writer_is_private, depth + 1);
					}
					writer.WriteObjectEnd();
					return;
				}
				Type type = obj.GetType();
				if (JsonMapper.custom_exporters_table.ContainsKey(type))
				{
					JsonMapper.custom_exporters_table[type](obj, writer);
					return;
				}
				if (JsonMapper.base_exporters_table.ContainsKey(type))
				{
					JsonMapper.base_exporters_table[type](obj, writer);
					return;
				}
				if (!(obj is Enum))
				{
					JsonMapper.AddTypeProperties(type);
					IEnumerable<PropertyMetadata> enumerable = JsonMapper.type_properties[type];
					writer.WriteObjectStart();
					foreach (PropertyMetadata propertyMetadata in enumerable)
					{
						if (propertyMetadata.IsField)
						{
							writer.WritePropertyName(propertyMetadata.Info.Name);
							JsonMapper.WriteValue(((FieldInfo)propertyMetadata.Info).GetValue(obj), writer, writer_is_private, depth + 1);
						}
						else
						{
							PropertyInfo propertyInfo = (PropertyInfo)propertyMetadata.Info;
							if (propertyInfo.CanRead)
							{
								writer.WritePropertyName(propertyMetadata.Info.Name);
								JsonMapper.WriteValue(propertyInfo.GetValue(obj, null), writer, writer_is_private, depth + 1);
							}
						}
					}
					writer.WriteObjectEnd();
					return;
				}
				Type underlyingType = Enum.GetUnderlyingType(type);
				if (underlyingType == typeof(long) || underlyingType == typeof(uint) || underlyingType == typeof(ulong))
				{
					writer.Write((ulong)obj);
					return;
				}
				writer.Write((int)obj);
				return;
			}
		}

		// Token: 0x060053AC RID: 21420 RVA: 0x001A79C4 File Offset: 0x001A5BC4
		public static string ToJson(object obj)
		{
			object obj2 = JsonMapper.static_writer_lock;
			string result;
			lock (obj2)
			{
				JsonMapper.static_writer.Reset();
				JsonMapper.WriteValue(obj, JsonMapper.static_writer, true, 0);
				result = JsonMapper.static_writer.ToString();
			}
			return result;
		}

		// Token: 0x060053AD RID: 21421 RVA: 0x001A7A20 File Offset: 0x001A5C20
		public static void ToJson(object obj, JsonWriter writer)
		{
			JsonMapper.WriteValue(obj, writer, false, 0);
		}

		// Token: 0x060053AE RID: 21422 RVA: 0x001A7A2B File Offset: 0x001A5C2B
		public static JsonData ToObject(JsonReader reader)
		{
			return (JsonData)JsonMapper.ToWrapper(() => new JsonData(), reader);
		}

		// Token: 0x060053AF RID: 21423 RVA: 0x001A7A58 File Offset: 0x001A5C58
		public static JsonData ToObject(TextReader reader)
		{
			JsonReader reader2 = new JsonReader(reader);
			return (JsonData)JsonMapper.ToWrapper(() => new JsonData(), reader2);
		}

		// Token: 0x060053B0 RID: 21424 RVA: 0x001A7A96 File Offset: 0x001A5C96
		public static JsonData ToObject(string json)
		{
			return (JsonData)JsonMapper.ToWrapper(() => new JsonData(), json);
		}

		// Token: 0x060053B1 RID: 21425 RVA: 0x001A7AC2 File Offset: 0x001A5CC2
		public static T ToObject<T>(JsonReader reader)
		{
			return (T)((object)JsonMapper.ReadValue(typeof(T), reader));
		}

		// Token: 0x060053B2 RID: 21426 RVA: 0x001A7ADC File Offset: 0x001A5CDC
		public static T ToObject<T>(TextReader reader)
		{
			JsonReader reader2 = new JsonReader(reader);
			return (T)((object)JsonMapper.ReadValue(typeof(T), reader2));
		}

		// Token: 0x060053B3 RID: 21427 RVA: 0x001A7B08 File Offset: 0x001A5D08
		public static T ToObject<T>(string json)
		{
			JsonReader reader = new JsonReader(json);
			return (T)((object)JsonMapper.ReadValue(typeof(T), reader));
		}

		// Token: 0x060053B4 RID: 21428 RVA: 0x001A7B31 File Offset: 0x001A5D31
		public static IJsonWrapper ToWrapper(WrapperFactory factory, JsonReader reader)
		{
			return JsonMapper.ReadValue(factory, reader);
		}

		// Token: 0x060053B5 RID: 21429 RVA: 0x001A7B3C File Offset: 0x001A5D3C
		public static IJsonWrapper ToWrapper(WrapperFactory factory, string json)
		{
			JsonReader reader = new JsonReader(json);
			return JsonMapper.ReadValue(factory, reader);
		}

		// Token: 0x060053B6 RID: 21430 RVA: 0x001A7B58 File Offset: 0x001A5D58
		public static void RegisterExporter<T>(ExporterFunc<T> exporter)
		{
			ExporterFunc exporterFunc = delegate(object obj, JsonWriter writer)
			{
				exporter((T)((object)obj), writer);
			};
			JsonMapper.custom_exporters_table[typeof(T)] = exporterFunc;
		}

		// Token: 0x060053B7 RID: 21431 RVA: 0x001A7B94 File Offset: 0x001A5D94
		public static void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
		{
			ImporterFunc importer2 = (object input) => importer((TJson)((object)input));
			JsonMapper.RegisterImporter(JsonMapper.custom_importers_table, typeof(TJson), typeof(TValue), importer2);
		}

		// Token: 0x060053B8 RID: 21432 RVA: 0x001A7BD8 File Offset: 0x001A5DD8
		public static void UnregisterExporters()
		{
			JsonMapper.custom_exporters_table.Clear();
		}

		// Token: 0x060053B9 RID: 21433 RVA: 0x001A7BE4 File Offset: 0x001A5DE4
		public static void UnregisterImporters()
		{
			JsonMapper.custom_importers_table.Clear();
		}

		// Token: 0x04006125 RID: 24869
		private static int max_nesting_depth;

		// Token: 0x04006126 RID: 24870
		private static IFormatProvider datetime_format;

		// Token: 0x04006127 RID: 24871
		private static IDictionary<Type, ExporterFunc> base_exporters_table;

		// Token: 0x04006128 RID: 24872
		private static IDictionary<Type, ExporterFunc> custom_exporters_table;

		// Token: 0x04006129 RID: 24873
		private static IDictionary<Type, IDictionary<Type, ImporterFunc>> base_importers_table;

		// Token: 0x0400612A RID: 24874
		private static IDictionary<Type, IDictionary<Type, ImporterFunc>> custom_importers_table;

		// Token: 0x0400612B RID: 24875
		private static IDictionary<Type, ArrayMetadata> array_metadata;

		// Token: 0x0400612C RID: 24876
		private static readonly object array_metadata_lock = new object();

		// Token: 0x0400612D RID: 24877
		private static IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops;

		// Token: 0x0400612E RID: 24878
		private static readonly object conv_ops_lock = new object();

		// Token: 0x0400612F RID: 24879
		private static IDictionary<Type, ObjectMetadata> object_metadata;

		// Token: 0x04006130 RID: 24880
		private static readonly object object_metadata_lock = new object();

		// Token: 0x04006131 RID: 24881
		private static IDictionary<Type, IList<PropertyMetadata>> type_properties;

		// Token: 0x04006132 RID: 24882
		private static readonly object type_properties_lock = new object();

		// Token: 0x04006133 RID: 24883
		private static JsonWriter static_writer;

		// Token: 0x04006134 RID: 24884
		private static readonly object static_writer_lock = new object();
	}
}
