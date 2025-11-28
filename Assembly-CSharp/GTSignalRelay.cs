using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007F6 RID: 2038
public class GTSignalRelay : MonoBehaviourStatic<GTSignalRelay>, IOnEventCallback
{
	// Token: 0x170004C6 RID: 1222
	// (get) Token: 0x06003593 RID: 13715 RVA: 0x00122606 File Offset: 0x00120806
	public static IReadOnlyList<GTSignalListener> ActiveListeners
	{
		get
		{
			return GTSignalRelay.gActiveListeners;
		}
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x0012260D File Offset: 0x0012080D
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
	}

	// Token: 0x06003595 RID: 13717 RVA: 0x0012261C File Offset: 0x0012081C
	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06003596 RID: 13718 RVA: 0x0012262C File Offset: 0x0012082C
	public static void Register(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		int num = listener.signal;
		if (num == 0)
		{
			return;
		}
		if (!GTSignalRelay.gListenerSet.Add(listener))
		{
			return;
		}
		GTSignalRelay.gActiveListeners.Add(listener);
		List<GTSignalListener> list;
		if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(num, ref list))
		{
			list = new List<GTSignalListener>(64);
			GTSignalRelay.gSignalIdToListeners.Add(num, list);
		}
		list.Add(listener);
	}

	// Token: 0x06003597 RID: 13719 RVA: 0x00122698 File Offset: 0x00120898
	public static void Unregister(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		GTSignalRelay.gListenerSet.Remove(listener);
		GTSignalRelay.gActiveListeners.Remove(listener);
		List<GTSignalListener> list;
		if (GTSignalRelay.gSignalIdToListeners.TryGetValue(listener.signal, ref list))
		{
			list.Remove(listener);
		}
	}

	// Token: 0x06003598 RID: 13720 RVA: 0x001226E8 File Offset: 0x001208E8
	[RuntimeInitializeOnLoadMethod(1)]
	private static void InitializeOnLoad()
	{
		Object.DontDestroyOnLoad(new GameObject("GTSignalRelay").AddComponent<GTSignalRelay>());
	}

	// Token: 0x06003599 RID: 13721 RVA: 0x00122700 File Offset: 0x00120900
	void IOnEventCallback.OnEvent(EventData eventData)
	{
		if (eventData.Code != 186)
		{
			return;
		}
		object[] array = (object[])eventData.CustomData;
		int num = (int)array[0];
		List<GTSignalListener> list;
		if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(num, ref list))
		{
			return;
		}
		int sender = eventData.Sender;
		for (int i = 0; i < list.Count; i++)
		{
			try
			{
				GTSignalListener gtsignalListener = list[i];
				if (!gtsignalListener.deafen)
				{
					if (gtsignalListener.IsReady())
					{
						if (!gtsignalListener.ignoreSelf || sender != gtsignalListener.rigActorID)
						{
							if (!gtsignalListener.listenToSelfOnly || sender == gtsignalListener.rigActorID)
							{
								gtsignalListener.HandleSignalReceived(sender, array);
								if (gtsignalListener.callUnityEvent)
								{
									UnityEvent onSignalReceived = gtsignalListener.onSignalReceived;
									if (onSignalReceived != null)
									{
										onSignalReceived.Invoke();
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x040044B8 RID: 17592
	private static List<GTSignalListener> gActiveListeners = new List<GTSignalListener>(128);

	// Token: 0x040044B9 RID: 17593
	private static HashSet<GTSignalListener> gListenerSet = new HashSet<GTSignalListener>(128);

	// Token: 0x040044BA RID: 17594
	private static Dictionary<int, List<GTSignalListener>> gSignalIdToListeners = new Dictionary<int, List<GTSignalListener>>(128);
}
