using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000CA5 RID: 3237
[Serializable]
public class GTScene : IEquatable<GTScene>
{
	// Token: 0x1700074D RID: 1869
	// (get) Token: 0x06004EFB RID: 20219 RVA: 0x00198564 File Offset: 0x00196764
	public string alias
	{
		get
		{
			return this._alias;
		}
	}

	// Token: 0x1700074E RID: 1870
	// (get) Token: 0x06004EFC RID: 20220 RVA: 0x0019856C File Offset: 0x0019676C
	public string name
	{
		get
		{
			return this._name;
		}
	}

	// Token: 0x1700074F RID: 1871
	// (get) Token: 0x06004EFD RID: 20221 RVA: 0x00198574 File Offset: 0x00196774
	public string path
	{
		get
		{
			return this._path;
		}
	}

	// Token: 0x17000750 RID: 1872
	// (get) Token: 0x06004EFE RID: 20222 RVA: 0x0019857C File Offset: 0x0019677C
	public string guid
	{
		get
		{
			return this._guid;
		}
	}

	// Token: 0x17000751 RID: 1873
	// (get) Token: 0x06004EFF RID: 20223 RVA: 0x00198584 File Offset: 0x00196784
	public int buildIndex
	{
		get
		{
			return this._buildIndex;
		}
	}

	// Token: 0x17000752 RID: 1874
	// (get) Token: 0x06004F00 RID: 20224 RVA: 0x0019858C File Offset: 0x0019678C
	public bool includeInBuild
	{
		get
		{
			return this._includeInBuild;
		}
	}

	// Token: 0x17000753 RID: 1875
	// (get) Token: 0x06004F01 RID: 20225 RVA: 0x00198594 File Offset: 0x00196794
	public bool isLoaded
	{
		get
		{
			return SceneManager.GetSceneByBuildIndex(this._buildIndex).isLoaded;
		}
	}

	// Token: 0x17000754 RID: 1876
	// (get) Token: 0x06004F02 RID: 20226 RVA: 0x001985B4 File Offset: 0x001967B4
	public bool hasAlias
	{
		get
		{
			return !string.IsNullOrWhiteSpace(this._alias);
		}
	}

	// Token: 0x06004F03 RID: 20227 RVA: 0x001985C4 File Offset: 0x001967C4
	public GTScene(string name, string path, string guid, int buildIndex, bool includeInBuild)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentNullException("name");
		}
		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ArgumentNullException("path");
		}
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this._name = name;
		this._path = path;
		this._guid = guid;
		this._buildIndex = buildIndex;
		this._includeInBuild = includeInBuild;
	}

	// Token: 0x06004F04 RID: 20228 RVA: 0x00198635 File Offset: 0x00196835
	public override int GetHashCode()
	{
		return this._guid.GetHashCode();
	}

	// Token: 0x06004F05 RID: 20229 RVA: 0x00198642 File Offset: 0x00196842
	public override string ToString()
	{
		return this.ToJson(false);
	}

	// Token: 0x06004F06 RID: 20230 RVA: 0x0019864B File Offset: 0x0019684B
	public bool Equals(GTScene other)
	{
		return this._guid.Equals(other._guid) && this._name == other._name && this._path == other._path;
	}

	// Token: 0x06004F07 RID: 20231 RVA: 0x00198688 File Offset: 0x00196888
	public override bool Equals(object obj)
	{
		GTScene gtscene = obj as GTScene;
		return gtscene != null && this.Equals(gtscene);
	}

	// Token: 0x06004F08 RID: 20232 RVA: 0x001986A8 File Offset: 0x001968A8
	public static bool operator ==(GTScene x, GTScene y)
	{
		return x.Equals(y);
	}

	// Token: 0x06004F09 RID: 20233 RVA: 0x001986B1 File Offset: 0x001968B1
	public static bool operator !=(GTScene x, GTScene y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06004F0A RID: 20234 RVA: 0x001986BD File Offset: 0x001968BD
	public void LoadAsync()
	{
		if (this.isLoaded)
		{
			return;
		}
		SceneManager.LoadSceneAsync(this._buildIndex, 1);
	}

	// Token: 0x06004F0B RID: 20235 RVA: 0x001986D5 File Offset: 0x001968D5
	public void UnloadAsync()
	{
		if (!this.isLoaded)
		{
			return;
		}
		SceneManager.UnloadSceneAsync(this._buildIndex, 1);
	}

	// Token: 0x06004F0C RID: 20236 RVA: 0x000743B1 File Offset: 0x000725B1
	public static GTScene FromAsset(object sceneAsset)
	{
		return null;
	}

	// Token: 0x06004F0D RID: 20237 RVA: 0x000743B1 File Offset: 0x000725B1
	public static GTScene From(object editorBuildSettingsScene)
	{
		return null;
	}

	// Token: 0x04005DAC RID: 23980
	[SerializeField]
	private string _alias;

	// Token: 0x04005DAD RID: 23981
	[SerializeField]
	private string _name;

	// Token: 0x04005DAE RID: 23982
	[SerializeField]
	private string _path;

	// Token: 0x04005DAF RID: 23983
	[SerializeField]
	private string _guid;

	// Token: 0x04005DB0 RID: 23984
	[SerializeField]
	private int _buildIndex;

	// Token: 0x04005DB1 RID: 23985
	[SerializeField]
	private bool _includeInBuild;
}
