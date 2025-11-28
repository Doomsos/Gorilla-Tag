using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022C RID: 556
[Serializable]
public class GTEnumValueMap<T> : ISerializationCallbackReceiver
{
	// Token: 0x06000EDD RID: 3805 RVA: 0x0004F04D File Offset: 0x0004D24D
	public bool TryGet(long i, out T o)
	{
		return this._enumValue_to_unityObject.TryGetValue(i, ref o);
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0004F05C File Offset: 0x0004D25C
	public IEnumerable<T> Values
	{
		get
		{
			return this._enumValue_to_unityObject.Values;
		}
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x00002789 File Offset: 0x00000989
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0004F069 File Offset: 0x0004D269
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		this.Init();
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x0004F074 File Offset: 0x0004D274
	public void Init()
	{
		if (this.m_enumValueAndUnityObjectPairs == null)
		{
			return;
		}
		if (this._enumValue_to_unityObject == null)
		{
			this._enumValue_to_unityObject = new Dictionary<long, T>();
		}
		this._enumValue_to_unityObject.Clear();
		foreach (GTEnumValueMap<T>.EnumValueToUnityObject enumValueToUnityObject in this.m_enumValueAndUnityObjectPairs)
		{
			if (enumValueToUnityObject.enabled && enumValueToUnityObject.value != null)
			{
				this._enumValue_to_unityObject[enumValueToUnityObject.enumKey] = enumValueToUnityObject.value;
			}
		}
		if (!Application.isEditor)
		{
			this.m_enumScriptGuid = null;
			this.m_enumValueAndUnityObjectPairs = null;
		}
	}

	// Token: 0x04001221 RID: 4641
	private Dictionary<long, T> _enumValue_to_unityObject = new Dictionary<long, T>();

	// Token: 0x04001222 RID: 4642
	[Tooltip("The GUID to the Enum script asset which is what is serialized in editor (not used at runtime). This is exposed and editable as a precaution but shouldn't be necessary to have to use.")]
	[SerializeField]
	private string m_enumScriptGuid;

	// Token: 0x04001223 RID: 4643
	[SerializeField]
	private List<GTEnumValueMap<T>.EnumValueToUnityObject> m_enumValueAndUnityObjectPairs = new List<GTEnumValueMap<T>.EnumValueToUnityObject>();

	// Token: 0x0200022D RID: 557
	[Serializable]
	private struct EnumValueToUnityObject
	{
		// Token: 0x04001224 RID: 4644
		public bool enabled;

		// Token: 0x04001225 RID: 4645
		public long enumKey;

		// Token: 0x04001226 RID: 4646
		public string enumName;

		// Token: 0x04001227 RID: 4647
		public T value;
	}
}
