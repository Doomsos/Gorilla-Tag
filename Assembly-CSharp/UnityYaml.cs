using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x02000CAC RID: 3244
public static class UnityYaml
{
	// Token: 0x04005DBC RID: 23996
	private static readonly Assembly EngineAssembly = Assembly.GetAssembly(typeof(MonoBehaviour));

	// Token: 0x04005DBD RID: 23997
	private static readonly Assembly TerrainAssembly = Assembly.GetAssembly(typeof(Tree));

	// Token: 0x04005DBE RID: 23998
	public static Dictionary<int, Type> ClassIDToType = new Dictionary<int, Type>();
}
