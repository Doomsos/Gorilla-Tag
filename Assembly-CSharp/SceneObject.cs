using System;
using UnityEngine;

// Token: 0x02000CAB RID: 3243
[Serializable]
public class SceneObject : IEquatable<SceneObject>
{
	// Token: 0x06004F45 RID: 20293 RVA: 0x00198ABD File Offset: 0x00196CBD
	public Type GetObjectType()
	{
		if (string.IsNullOrWhiteSpace(this.typeString))
		{
			return null;
		}
		if (this.typeString.Contains("ProxyType"))
		{
			return ProxyType.Parse(this.typeString);
		}
		return Type.GetType(this.typeString);
	}

	// Token: 0x06004F46 RID: 20294 RVA: 0x00198AF7 File Offset: 0x00196CF7
	public SceneObject(int classID, ulong fileID)
	{
		this.classID = classID;
		this.fileID = fileID;
		this.typeString = UnityYaml.ClassIDToType[classID].AssemblyQualifiedName;
	}

	// Token: 0x06004F47 RID: 20295 RVA: 0x00198B23 File Offset: 0x00196D23
	public bool Equals(SceneObject other)
	{
		return this.fileID == other.fileID && this.classID == other.classID;
	}

	// Token: 0x06004F48 RID: 20296 RVA: 0x00198B44 File Offset: 0x00196D44
	public override bool Equals(object obj)
	{
		SceneObject sceneObject = obj as SceneObject;
		return sceneObject != null && this.Equals(sceneObject);
	}

	// Token: 0x06004F49 RID: 20297 RVA: 0x00198B64 File Offset: 0x00196D64
	public override int GetHashCode()
	{
		int i = this.classID;
		int i2 = StaticHash.Compute((long)this.fileID);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004F4A RID: 20298 RVA: 0x00198B89 File Offset: 0x00196D89
	public static bool operator ==(SceneObject x, SceneObject y)
	{
		return x.Equals(y);
	}

	// Token: 0x06004F4B RID: 20299 RVA: 0x00198B92 File Offset: 0x00196D92
	public static bool operator !=(SceneObject x, SceneObject y)
	{
		return !x.Equals(y);
	}

	// Token: 0x04005DB8 RID: 23992
	public int classID;

	// Token: 0x04005DB9 RID: 23993
	public ulong fileID;

	// Token: 0x04005DBA RID: 23994
	[SerializeField]
	public string typeString;

	// Token: 0x04005DBB RID: 23995
	public string json;
}
