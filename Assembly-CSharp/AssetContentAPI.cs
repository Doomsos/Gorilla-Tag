using System;
using UnityEngine;

public class AssetContentAPI : ScriptableObject
{
	public string bundleName;

	public LazyLoadReference<TextAsset> bundleFile;

	public UnityEngine.Object[] assets = new UnityEngine.Object[0];
}
