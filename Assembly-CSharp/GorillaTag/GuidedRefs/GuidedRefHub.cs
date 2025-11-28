using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs.Internal;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001039 RID: 4153
	[DefaultExecutionOrder(-2147483648)]
	public class GuidedRefHub : MonoBehaviour, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x060068D4 RID: 26836 RVA: 0x00222127 File Offset: 0x00220327
		protected void Awake()
		{
			this.GuidedRefInitialize();
		}

		// Token: 0x060068D5 RID: 26837 RVA: 0x00222130 File Offset: 0x00220330
		protected void OnDestroy()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (this.isRootInstance)
			{
				GuidedRefHub.hasRootInstance = false;
				GuidedRefHub.rootInstance = null;
			}
			List<int> list;
			if (GuidedRefHub.globalLookupRefInstIDsByHub.TryGetValue(this, ref list))
			{
				foreach (int num in list)
				{
					GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId[num].Remove(this);
				}
				GuidedRefHub.globalLookupRefInstIDsByHub.Remove(this);
			}
		}

		// Token: 0x060068D6 RID: 26838 RVA: 0x002221C0 File Offset: 0x002203C0
		public void GuidedRefInitialize()
		{
			if (this.isRootInstance)
			{
				if (GuidedRefHub.hasRootInstance)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"GuidedRefHub: Attempted to assign global instance when one was already assigned:\n- This path: ",
						base.transform.GetPath(),
						"\n- Global instance: ",
						GuidedRefHub.rootInstance.transform.GetPath(),
						"\n"
					}), this);
					Object.Destroy(this);
					return;
				}
				GuidedRefHub.hasRootInstance = true;
				GuidedRefHub.rootInstance = this;
			}
			GuidedRefHub.globalLookupRefInstIDsByHub[this] = new List<int>(2);
		}

		// Token: 0x060068D7 RID: 26839 RVA: 0x00222249 File Offset: 0x00220449
		public static bool IsInstanceIDRegisteredWithAnyHub(int instanceID)
		{
			return GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.ContainsKey(instanceID);
		}

		// Token: 0x060068D8 RID: 26840 RVA: 0x00222258 File Offset: 0x00220458
		private void RegisterTarget_Internal<TIGuidedRefTargetMono>(TIGuidedRefTargetMono targetMono) where TIGuidedRefTargetMono : IGuidedRefTargetMono
		{
			RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetMono.GRefTargetInfo.targetId);
			if (orAddRelayInfoByTargetId == null)
			{
				return;
			}
			IGuidedRefTargetMono guidedRefTargetMono = targetMono;
			if (orAddRelayInfoByTargetId.targetMono != null && orAddRelayInfoByTargetId.targetMono != guidedRefTargetMono)
			{
				if (targetMono.GRefTargetInfo.hackIgnoreDuplicateRegistration)
				{
					return;
				}
				Debug.LogError(string.Concat(new string[]
				{
					"GuidedRefHub: Multiple targets registering with the same Hub. Maybe look at the HubID you are using:- hub=\"",
					base.transform.GetPath(),
					"\"\n- target1=\"",
					orAddRelayInfoByTargetId.targetMono.transform.GetPath(),
					"\",\n- target2=\"",
					targetMono.transform.GetPath(),
					"\""
				}), this);
				return;
			}
			else
			{
				int instanceID = targetMono.GetInstanceID();
				GuidedRefHub.GetHubsThatHaveRegisteredInstId(instanceID).Add(this);
				List<int> list;
				if (!GuidedRefHub.globalLookupRefInstIDsByHub.TryGetValue(this, ref list))
				{
					Debug.LogError(string.Concat(new string[]
					{
						"GuidedRefHub: It appears hub was not registered before `RegisterTarget` was called on it: - hub: \"",
						base.transform.GetPath(),
						"\"\n- target: \"",
						targetMono.transform.GetPath(),
						"\""
					}), this);
					return;
				}
				list.Add(instanceID);
				orAddRelayInfoByTargetId.targetMono = targetMono;
				GuidedRefHub.ResolveReferences(orAddRelayInfoByTargetId);
				return;
			}
		}

		// Token: 0x060068D9 RID: 26841 RVA: 0x002223AC File Offset: 0x002205AC
		public static void RegisterTarget<TIGuidedRefTargetMono>(TIGuidedRefTargetMono targetMono, GuidedRefHubIdSO[] hubIds = null, Component debugCaller = null) where TIGuidedRefTargetMono : IGuidedRefTargetMono
		{
			if (targetMono == null)
			{
				string text = (debugCaller == null) ? "UNSUPPLIED_CALLER_NAME" : debugCaller.name;
				Debug.LogError("GuidedRefHub: Cannot register null target from \"" + text + "\".", debugCaller);
				return;
			}
			if (targetMono.GRefTargetInfo.targetId == null)
			{
				return;
			}
			GuidedRefHub.globalHubsTransientList.Clear();
			targetMono.transform.GetComponentsInParent<GuidedRefHub>(true, GuidedRefHub.globalHubsTransientList);
			if (GuidedRefHub.hasRootInstance)
			{
				GuidedRefHub.globalHubsTransientList.Add(GuidedRefHub.rootInstance);
			}
			bool flag = false;
			foreach (GuidedRefHub guidedRefHub in GuidedRefHub.globalHubsTransientList)
			{
				if (hubIds == null || hubIds.Length <= 0 || Array.IndexOf<GuidedRefHubIdSO>(hubIds, guidedRefHub.hubId) != -1)
				{
					flag = true;
					guidedRefHub.RegisterTarget_Internal<TIGuidedRefTargetMono>(targetMono);
				}
			}
			if (!flag && Application.isPlaying)
			{
				Debug.LogError("GuidedRefHub: Could not find hub for target: \"" + targetMono.transform.GetPath() + "\"", targetMono.transform);
			}
		}

		// Token: 0x060068DA RID: 26842 RVA: 0x002224E0 File Offset: 0x002206E0
		public static void UnregisterTarget<TIGuidedRefTargetMono>(TIGuidedRefTargetMono targetMono, bool destroyed = true) where TIGuidedRefTargetMono : IGuidedRefTargetMono
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (targetMono == null)
			{
				Debug.LogError("GuidedRefHub: Cannot unregister null target.");
				return;
			}
			int instanceID = targetMono.GetInstanceID();
			List<GuidedRefHub> list;
			if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceID, ref list))
			{
				return;
			}
			using (List<GuidedRefHub>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RelayInfo relayInfo;
					if (enumerator.Current.lookupRelayInfoByTargetId.TryGetValue(targetMono.GRefTargetInfo.targetId, ref relayInfo))
					{
						foreach (RegisteredReceiverFieldInfo registeredReceiverFieldInfo in relayInfo.registeredFields)
						{
							if (registeredReceiverFieldInfo.receiverMono != null)
							{
								registeredReceiverFieldInfo.receiverMono.OnGuidedRefTargetDestroyed(registeredReceiverFieldInfo.fieldId);
								GuidedRefHub.kReceiversWaitingToFullyResolve.Remove(registeredReceiverFieldInfo.receiverMono);
								relayInfo.resolvedFields.Remove(registeredReceiverFieldInfo);
								relayInfo.registeredFields.Add(registeredReceiverFieldInfo);
								IGuidedRefReceiverMono receiverMono = registeredReceiverFieldInfo.receiverMono;
								int guidedRefsWaitingToResolveCount = receiverMono.GuidedRefsWaitingToResolveCount;
								receiverMono.GuidedRefsWaitingToResolveCount = guidedRefsWaitingToResolveCount + 1;
							}
						}
					}
				}
			}
			foreach (GuidedRefHub guidedRefHub in list)
			{
				RelayInfo relayInfo2;
				if (guidedRefHub.lookupRelayInfoByTargetId.TryGetValue(targetMono.GRefTargetInfo.targetId, ref relayInfo2))
				{
					relayInfo2.targetMono = null;
				}
				GuidedRefHub.globalLookupRefInstIDsByHub[guidedRefHub].Remove(instanceID);
			}
			GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.Remove(instanceID);
		}

		// Token: 0x060068DB RID: 26843 RVA: 0x002226A8 File Offset: 0x002208A8
		public static void ReceiverFullyRegistered<TIGuidedRefReceiverMono>(TIGuidedRefReceiverMono receiverMono) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			GuidedRefHub.kReceiversFullyRegistered.Add(receiverMono);
			GuidedRefHub.kReceiversWaitingToFullyResolve.Add(receiverMono);
			GuidedRefHub.CheckAndNotifyIfReceiverFullyResolved<TIGuidedRefReceiverMono>(receiverMono);
		}

		// Token: 0x060068DC RID: 26844 RVA: 0x002226D4 File Offset: 0x002208D4
		private static void CheckAndNotifyIfReceiverFullyResolved<TIGuidedRefReceiverMono>(TIGuidedRefReceiverMono receiverMono) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			if (receiverMono.GuidedRefsWaitingToResolveCount == 0 && GuidedRefHub.kReceiversFullyRegistered.Contains(receiverMono))
			{
				GuidedRefHub.kReceiversWaitingToFullyResolve.Remove(receiverMono);
				receiverMono.OnAllGuidedRefsResolved();
			}
		}

		// Token: 0x060068DD RID: 26845 RVA: 0x00222720 File Offset: 0x00220920
		private void RegisterReceiverField(RegisteredReceiverFieldInfo registeredReceiverFieldInfo, GuidedRefTargetIdSO targetId)
		{
			GuidedRefHub.globalLookupRefInstIDsByHub[this].Add(registeredReceiverFieldInfo.receiverMono.GetInstanceID());
			GuidedRefHub.GetHubsThatHaveRegisteredInstId(registeredReceiverFieldInfo.receiverMono.GetInstanceID()).Add(this);
			RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetId);
			orAddRelayInfoByTargetId.registeredFields.Add(registeredReceiverFieldInfo);
			GuidedRefHub.ResolveReferences(orAddRelayInfoByTargetId);
		}

		// Token: 0x060068DE RID: 26846 RVA: 0x00222778 File Offset: 0x00220978
		private static void RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(GuidedRefHubIdSO hubId, TIGuidedRefReceiverMono receiverMono, int fieldId, GuidedRefTargetIdSO targetId, int index) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			if (receiverMono == null)
			{
				Debug.LogError("GuidedRefHub: Cannot register null receiver.");
				return;
			}
			GuidedRefHub.globalHubsTransientList.Clear();
			receiverMono.transform.GetComponentsInParent<GuidedRefHub>(true, GuidedRefHub.globalHubsTransientList);
			if (GuidedRefHub.hasRootInstance)
			{
				GuidedRefHub.globalHubsTransientList.Add(GuidedRefHub.rootInstance);
			}
			RegisteredReceiverFieldInfo registeredReceiverFieldInfo = new RegisteredReceiverFieldInfo
			{
				receiverMono = receiverMono,
				fieldId = fieldId,
				index = index
			};
			bool flag = false;
			foreach (GuidedRefHub guidedRefHub in GuidedRefHub.globalHubsTransientList)
			{
				if (!(hubId != null) || !(guidedRefHub.hubId != hubId))
				{
					flag = true;
					guidedRefHub.RegisterReceiverField(registeredReceiverFieldInfo, targetId);
					break;
				}
			}
			if (flag)
			{
				int guidedRefsWaitingToResolveCount = receiverMono.GuidedRefsWaitingToResolveCount;
				receiverMono.GuidedRefsWaitingToResolveCount = guidedRefsWaitingToResolveCount + 1;
				return;
			}
			Debug.LogError("Could not find matching GuidedRefHub to register with for receiver at: " + receiverMono.transform.GetPath(), receiverMono.transform);
		}

		// Token: 0x060068DF RID: 26847 RVA: 0x002228B0 File Offset: 0x00220AB0
		public static void RegisterReceiverField<TIGuidedRefReceiverMono>(TIGuidedRefReceiverMono receiverMono, string fieldIdName, ref GuidedRefReceiverFieldInfo fieldInfo) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			if (!GRef.ShouldResolveNow(fieldInfo.resolveModes))
			{
				return;
			}
			fieldInfo.fieldId = Shader.PropertyToID(fieldIdName);
			GuidedRefHub.RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(fieldInfo.hubId, receiverMono, fieldInfo.fieldId, fieldInfo.targetId, -1);
		}

		// Token: 0x060068E0 RID: 26848 RVA: 0x002228E8 File Offset: 0x00220AE8
		public static void RegisterReceiverArray<TIGuidedRefReceiverMono, T>(TIGuidedRefReceiverMono receiverMono, string fieldIdName, ref T[] receiverArray, ref GuidedRefReceiverArrayInfo arrayInfo) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono where T : Object
		{
			if (!GRef.ShouldResolveNow(arrayInfo.resolveModes))
			{
				return;
			}
			if (receiverArray == null)
			{
				receiverArray = new T[arrayInfo.targets.Length];
			}
			else if (receiverArray.Length != arrayInfo.targets.Length)
			{
				Array.Resize<T>(ref receiverArray, arrayInfo.targets.Length);
			}
			arrayInfo.fieldId = Shader.PropertyToID(fieldIdName);
			for (int i = 0; i < arrayInfo.targets.Length; i++)
			{
				GuidedRefHub.RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(arrayInfo.hubId, receiverMono, arrayInfo.fieldId, arrayInfo.targets[i], i);
			}
		}

		// Token: 0x060068E1 RID: 26849 RVA: 0x00222970 File Offset: 0x00220B70
		public static void UnregisterReceiver<TIGuidedRefReceiverMono>(TIGuidedRefReceiverMono receiverMono) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			if (receiverMono == null)
			{
				Debug.LogError("GuidedRefHub: Cannot unregister null receiver.");
				return;
			}
			int instanceID = receiverMono.GetInstanceID();
			List<GuidedRefHub> list;
			if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceID, ref list))
			{
				Debug.LogError("Tried to unregister a receiver before it was registered.");
				return;
			}
			IGuidedRefReceiverMono iReceiverMono = receiverMono;
			Predicate<RegisteredReceiverFieldInfo> <>9__0;
			foreach (GuidedRefHub guidedRefHub in list)
			{
				foreach (RelayInfo relayInfo in guidedRefHub.lookupRelayInfoByTargetId.Values)
				{
					List<RegisteredReceiverFieldInfo> registeredFields = relayInfo.registeredFields;
					Predicate<RegisteredReceiverFieldInfo> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((RegisteredReceiverFieldInfo fieldInfo) => fieldInfo.receiverMono == iReceiverMono));
					}
					registeredFields.RemoveAll(predicate);
				}
				GuidedRefHub.globalLookupRefInstIDsByHub[guidedRefHub].Remove(instanceID);
			}
			GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.Remove(instanceID);
			receiverMono.GuidedRefsWaitingToResolveCount = 0;
		}

		// Token: 0x060068E2 RID: 26850 RVA: 0x00222AA4 File Offset: 0x00220CA4
		private RelayInfo GetOrAddRelayInfoByTargetId(GuidedRefTargetIdSO targetId)
		{
			if (targetId == null)
			{
				Debug.LogError("GetOrAddRelayInfoByTargetId cannot register null target id");
				return null;
			}
			RelayInfo relayInfo;
			if (!this.lookupRelayInfoByTargetId.TryGetValue(targetId, ref relayInfo))
			{
				relayInfo = new RelayInfo
				{
					targetMono = null,
					registeredFields = new List<RegisteredReceiverFieldInfo>(1),
					resolvedFields = new List<RegisteredReceiverFieldInfo>(1)
				};
				this.lookupRelayInfoByTargetId[targetId] = relayInfo;
				GuidedRefHub.static_relayInfo_to_targetId[relayInfo] = targetId;
			}
			return relayInfo;
		}

		// Token: 0x060068E3 RID: 26851 RVA: 0x00222B18 File Offset: 0x00220D18
		public static List<GuidedRefHub> GetHubsThatHaveRegisteredInstId(int instanceId)
		{
			List<GuidedRefHub> list;
			if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceId, ref list))
			{
				list = new List<GuidedRefHub>(1);
				GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId[instanceId] = list;
			}
			return list;
		}

		// Token: 0x060068E4 RID: 26852 RVA: 0x00222B48 File Offset: 0x00220D48
		private static void ResolveReferences(RelayInfo relayInfo)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (relayInfo == null)
			{
				Debug.LogError("GuidedRefHub.ResolveReferences: (this should never happen) relayInfo is null.");
				return;
			}
			if (relayInfo.registeredFields == null)
			{
				GuidedRefTargetIdSO guidedRefTargetIdSO = GuidedRefHub.static_relayInfo_to_targetId[relayInfo];
				string text = (guidedRefTargetIdSO != null) ? guidedRefTargetIdSO.name : "NULL";
				Debug.LogError("GuidedRefHub.ResolveReferences: (this should never happen) \"" + text + "\"relayInfo.registeredFields is null.");
				return;
			}
			if (relayInfo.targetMono == null)
			{
				return;
			}
			for (int i = relayInfo.registeredFields.Count - 1; i >= 0; i--)
			{
				RegisteredReceiverFieldInfo registeredReceiverFieldInfo = relayInfo.registeredFields[i];
				if (registeredReceiverFieldInfo.receiverMono.GuidedRefTryResolveReference(new GuidedRefTryResolveInfo
				{
					fieldId = registeredReceiverFieldInfo.fieldId,
					index = registeredReceiverFieldInfo.index,
					targetMono = relayInfo.targetMono
				}))
				{
					relayInfo.registeredFields.RemoveAt(i);
					GuidedRefHub.CheckAndNotifyIfReceiverFullyResolved<IGuidedRefReceiverMono>(registeredReceiverFieldInfo.receiverMono);
					relayInfo.resolvedFields.Add(registeredReceiverFieldInfo);
				}
			}
		}

		// Token: 0x060068E5 RID: 26853 RVA: 0x00222C3C File Offset: 0x00220E3C
		public static bool TryResolveField<TIGuidedRefReceiverMono, T>(TIGuidedRefReceiverMono receiverMono, ref T refReceiverObj, GuidedRefReceiverFieldInfo receiverFieldInfo, GuidedRefTryResolveInfo tryResolveInfo) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono where T : Object
		{
			if (tryResolveInfo.index > -1 || tryResolveInfo.fieldId != receiverFieldInfo.fieldId || refReceiverObj != null)
			{
				return false;
			}
			bool flag = tryResolveInfo.targetMono != null && tryResolveInfo.targetMono.GuidedRefTargetObject != null;
			T t = flag ? (tryResolveInfo.targetMono.GuidedRefTargetObject as T) : default(T);
			if (!flag)
			{
				string fieldNameByID = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveField: Receiver \"",
					receiverMono.transform.name,
					"\" with field \"",
					fieldNameByID,
					"\": was already assigned to something other than matching target id! Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. ",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
			}
			else if (refReceiverObj != null && refReceiverObj != t)
			{
				Debug.LogError("was assigned didn't match assigning anyway");
				string fieldNameByID2 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveField: Receiver \"",
					receiverMono.transform.name,
					"\" with field \"",
					fieldNameByID2,
					"\" was already assigned to something other than matching target id! Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. ",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
			}
			refReceiverObj = t;
			int guidedRefsWaitingToResolveCount = receiverMono.GuidedRefsWaitingToResolveCount;
			receiverMono.GuidedRefsWaitingToResolveCount = guidedRefsWaitingToResolveCount - 1;
			return true;
		}

		// Token: 0x060068E6 RID: 26854 RVA: 0x00222E10 File Offset: 0x00221010
		public static bool TryResolveArrayItem<TIGuidedRefReceiverMono, T>(TIGuidedRefReceiverMono receiverMono, IList<T> receivingArray, GuidedRefReceiverArrayInfo receiverArrayInfo, GuidedRefTryResolveInfo tryResolveInfo) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono where T : Object
		{
			bool flag;
			return GuidedRefHub.TryResolveArrayItem<TIGuidedRefReceiverMono, T>(receiverMono, receivingArray, receiverArrayInfo, tryResolveInfo, out flag);
		}

		// Token: 0x060068E7 RID: 26855 RVA: 0x00222E28 File Offset: 0x00221028
		public static bool TryResolveArrayItem<TIGuidedRefReceiverMono, T>(TIGuidedRefReceiverMono receiverMono, IList<T> receivingArray, GuidedRefReceiverArrayInfo receiverArrayInfo, GuidedRefTryResolveInfo tryResolveInfo, out bool arrayResolved) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono where T : Object
		{
			arrayResolved = false;
			if (tryResolveInfo.index <= -1 && receiverArrayInfo.fieldId != tryResolveInfo.fieldId)
			{
				return false;
			}
			if (receivingArray == null)
			{
				string fieldNameByID = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID,
					"\": Receiving array cannot be null!",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
				return false;
			}
			if (receiverArrayInfo.targets == null)
			{
				string fieldNameByID2 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver component \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID2,
					"\": Targets array is null! It must have been set to null after registering. If this intentional than the you need to unregister first.",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
				return false;
			}
			int num = receiverArrayInfo.targets.Length;
			if (num <= receiverArrayInfo.resolveCount)
			{
				string fieldNameByID3 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver component \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID3,
					"\": Targets array size is equal or smaller than resolve count. Did you change the size of the array before it finished resolving or before unregistering?",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
				return false;
			}
			if (num != receivingArray.Count)
			{
				string fieldNameByID4 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver component \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID4,
					"\": The sizes of `receivingList` and `receiverArrayInfo.fieldInfos` are not equal. They must be the same length before calling.",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
				return false;
			}
			T t = tryResolveInfo.targetMono.GuidedRefTargetObject as T;
			if (t == null)
			{
				string fieldNameByID5 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver \"",
					receiverMono.transform.name,
					"\" with field \"",
					fieldNameByID5,
					"\" found a matching target id but target object was null! Was it destroyed without unregistering? ",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
			}
			if (receivingArray[tryResolveInfo.index] != null && receivingArray[tryResolveInfo.index] != t)
			{
				string fieldNameByID6 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID6,
					"\" ",
					string.Format("at index {0}: Already assigned to something other than matching target id! ", tryResolveInfo.index),
					"Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. ",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
			}
			int num2 = receiverArrayInfo.resolveCount + 1;
			receiverArrayInfo.resolveCount = num2;
			arrayResolved = (num2 >= num);
			num2 = receiverMono.GuidedRefsWaitingToResolveCount;
			receiverMono.GuidedRefsWaitingToResolveCount = num2 - 1;
			receivingArray[tryResolveInfo.index] = t;
			return true;
		}

		// Token: 0x060068E8 RID: 26856 RVA: 0x0022321B File Offset: 0x0022141B
		public static string GetFieldNameByID(int fieldId)
		{
			return "FieldNameOnlyAvailableInEditor";
		}

		// Token: 0x060068EB RID: 26859 RVA: 0x000743A9 File Offset: 0x000725A9
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060068EC RID: 26860 RVA: 0x000178ED File Offset: 0x00015AED
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x04007794 RID: 30612
		[SerializeField]
		private bool isRootInstance;

		// Token: 0x04007795 RID: 30613
		public GuidedRefHubIdSO hubId;

		// Token: 0x04007796 RID: 30614
		[OnEnterPlay_SetNull]
		[NonSerialized]
		public static GuidedRefHub rootInstance;

		// Token: 0x04007797 RID: 30615
		[OnEnterPlay_Set(false)]
		[NonSerialized]
		public static bool hasRootInstance;

		// Token: 0x04007798 RID: 30616
		[DebugReadout]
		private readonly Dictionary<GuidedRefTargetIdSO, RelayInfo> lookupRelayInfoByTargetId = new Dictionary<GuidedRefTargetIdSO, RelayInfo>(256);

		// Token: 0x04007799 RID: 30617
		private static readonly Dictionary<RelayInfo, GuidedRefTargetIdSO> static_relayInfo_to_targetId = new Dictionary<RelayInfo, GuidedRefTargetIdSO>(256);

		// Token: 0x0400779A RID: 30618
		[OnEnterPlay_Clear]
		private static readonly Dictionary<int, List<GuidedRefHub>> globalLookupHubsThatHaveRegisteredInstId = new Dictionary<int, List<GuidedRefHub>>(256);

		// Token: 0x0400779B RID: 30619
		[OnEnterPlay_Clear]
		private static readonly Dictionary<GuidedRefHub, List<int>> globalLookupRefInstIDsByHub = new Dictionary<GuidedRefHub, List<int>>(256);

		// Token: 0x0400779C RID: 30620
		[OnEnterPlay_Clear]
		private static readonly List<GuidedRefHub> globalHubsTransientList = new List<GuidedRefHub>(32);

		// Token: 0x0400779D RID: 30621
		private const string kUnsuppliedCallerName = "UNSUPPLIED_CALLER_NAME";

		// Token: 0x0400779E RID: 30622
		[DebugReadout]
		[OnEnterPlay_Clear]
		internal static readonly HashSet<IGuidedRefReceiverMono> kReceiversWaitingToFullyResolve = new HashSet<IGuidedRefReceiverMono>(256);

		// Token: 0x0400779F RID: 30623
		[DebugReadout]
		[OnEnterPlay_Clear]
		internal static readonly HashSet<IGuidedRefReceiverMono> kReceiversFullyRegistered = new HashSet<IGuidedRefReceiverMono>(256);
	}
}
