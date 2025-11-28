using System;
using System.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

// Token: 0x02000C9D RID: 3229
public class GraphicsStateCollectionManager : MonoBehaviour
{
	// Token: 0x06004EE2 RID: 20194 RVA: 0x00198050 File Offset: 0x00196250
	private GraphicsStateCollection FindExistingCollection()
	{
		for (int i = 0; i < this.collections.Length; i++)
		{
			if (this.collections[i] != null && this.collections[i].runtimePlatform == Application.platform && this.collections[i].graphicsDeviceType == SystemInfo.graphicsDeviceType && this.collections[i].qualityLevelName == QualitySettings.names[QualitySettings.GetQualityLevel()])
			{
				return this.collections[i];
			}
		}
		return null;
	}

	// Token: 0x06004EE3 RID: 20195 RVA: 0x001980D4 File Offset: 0x001962D4
	private void Awake()
	{
		if (GraphicsStateCollectionManager.Instance != null && GraphicsStateCollectionManager.Instance != this)
		{
			Debug.LogError("Only one instance of GraphicsStateCollectionManager is allowed!");
			Object.Destroy(base.gameObject);
			return;
		}
		GraphicsStateCollectionManager.Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06004EE4 RID: 20196 RVA: 0x00198124 File Offset: 0x00196324
	private void Start()
	{
		if (this.mode == GraphicsStateCollectionManager.Mode.Tracing)
		{
			this.m_GraphicsStateCollection = this.FindExistingCollection();
			if (this.m_GraphicsStateCollection != null)
			{
				this.m_OutputCollectionName = "SharedAssets/GraphicsStateCollections/" + this.m_GraphicsStateCollection.name;
			}
			else
			{
				int qualityLevel = QualitySettings.GetQualityLevel();
				string text = QualitySettings.names[qualityLevel];
				text = text.Replace(" ", "");
				this.m_OutputCollectionName = string.Concat(new object[]
				{
					"SharedAssets/GraphicsStateCollections/",
					"GfxState_",
					Application.platform,
					"_",
					SystemInfo.graphicsDeviceType.ToString(),
					"_",
					text
				});
				this.m_GraphicsStateCollection = new GraphicsStateCollection();
			}
			Debug.Log("Tracing started for GraphicsStateCollection by Scene '" + SceneManager.GetActiveScene().name + "'.");
			this.m_GraphicsStateCollection.BeginTrace();
			this._autoSaveRoutine = base.StartCoroutine(this.AutoSaveRoutine());
			return;
		}
		GraphicsStateCollection graphicsStateCollection = this.FindExistingCollection();
		if (graphicsStateCollection != null)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Scene '",
				SceneManager.GetActiveScene().name,
				"' started warming up ",
				graphicsStateCollection.totalGraphicsStateCount.ToString(),
				" GraphicsState entries."
			}));
			graphicsStateCollection.WarmUp(default(JobHandle));
		}
	}

	// Token: 0x06004EE5 RID: 20197 RVA: 0x001982A8 File Offset: 0x001964A8
	private void OnApplicationFocus(bool focus)
	{
		if (!focus && this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
		{
			Debug.Log("Focus changed. Sending collection to Editor with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
			this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
		}
	}

	// Token: 0x06004EE6 RID: 20198 RVA: 0x00198308 File Offset: 0x00196508
	private void OnDestroy()
	{
		if (this._autoSaveRoutine != null)
		{
			base.StopCoroutine(this._autoSaveRoutine);
		}
		if (this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
		{
			this.m_GraphicsStateCollection.EndTrace();
			Debug.Log("Sending collection to Editor with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
			this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
		}
	}

	// Token: 0x06004EE7 RID: 20199 RVA: 0x00198383 File Offset: 0x00196583
	private IEnumerator AutoSaveRoutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(5f);
			if (this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
			{
				Debug.Log("Auto-saving collection with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
				this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
			}
		}
		yield break;
	}

	// Token: 0x04005D95 RID: 23957
	public GraphicsStateCollectionManager.Mode mode;

	// Token: 0x04005D96 RID: 23958
	public static GraphicsStateCollectionManager Instance;

	// Token: 0x04005D97 RID: 23959
	public GraphicsStateCollection[] collections;

	// Token: 0x04005D98 RID: 23960
	private const string k_CollectionFolderPath = "SharedAssets/GraphicsStateCollections/";

	// Token: 0x04005D99 RID: 23961
	private string m_OutputCollectionName;

	// Token: 0x04005D9A RID: 23962
	private GraphicsStateCollection m_GraphicsStateCollection;

	// Token: 0x04005D9B RID: 23963
	private Coroutine _autoSaveRoutine;

	// Token: 0x02000C9E RID: 3230
	public enum Mode
	{
		// Token: 0x04005D9D RID: 23965
		Tracing,
		// Token: 0x04005D9E RID: 23966
		WarmUp
	}
}
