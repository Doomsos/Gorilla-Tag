using System;
using System.Reflection;

namespace Viveport.Core
{
	// Token: 0x02000D50 RID: 3408
	public class Logger
	{
		// Token: 0x060052FB RID: 21243 RVA: 0x001A5678 File Offset: 0x001A3878
		public static void Log(string message)
		{
			if (!Logger._hasDetected || Logger._usingUnityLog)
			{
				Logger.UnityLog(message);
				return;
			}
			Logger.ConsoleLog(message);
		}

		// Token: 0x060052FC RID: 21244 RVA: 0x001A5695 File Offset: 0x001A3895
		private static void ConsoleLog(string message)
		{
			Console.WriteLine(message);
			Logger._hasDetected = true;
		}

		// Token: 0x060052FD RID: 21245 RVA: 0x001A56A4 File Offset: 0x001A38A4
		private static void UnityLog(string message)
		{
			try
			{
				if (Logger._unityLogType == null)
				{
					Logger._unityLogType = Logger.GetType("UnityEngine.Debug");
				}
				Logger._unityLogType.GetMethod("Log", new Type[]
				{
					typeof(string)
				}).Invoke(null, new object[]
				{
					message
				});
				Logger._usingUnityLog = true;
			}
			catch (Exception)
			{
				Logger.ConsoleLog(message);
				Logger._usingUnityLog = false;
			}
			Logger._hasDetected = true;
		}

		// Token: 0x060052FE RID: 21246 RVA: 0x001A5730 File Offset: 0x001A3930
		private static Type GetType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		// Token: 0x04006104 RID: 24836
		private const string LoggerTypeNameUnity = "UnityEngine.Debug";

		// Token: 0x04006105 RID: 24837
		private static bool _hasDetected;

		// Token: 0x04006106 RID: 24838
		private static bool _usingUnityLog = true;

		// Token: 0x04006107 RID: 24839
		private static Type _unityLogType;
	}
}
