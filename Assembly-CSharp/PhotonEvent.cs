using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000BAE RID: 2990
[Serializable]
public class PhotonEvent : IEquatable<PhotonEvent>
{
	// Token: 0x170006E0 RID: 1760
	// (get) Token: 0x060049D9 RID: 18905 RVA: 0x00185059 File Offset: 0x00183259
	// (set) Token: 0x060049DA RID: 18906 RVA: 0x00185061 File Offset: 0x00183261
	public bool reliable
	{
		get
		{
			return this._reliable;
		}
		set
		{
			this._reliable = value;
		}
	}

	// Token: 0x170006E1 RID: 1761
	// (get) Token: 0x060049DB RID: 18907 RVA: 0x0018506A File Offset: 0x0018326A
	// (set) Token: 0x060049DC RID: 18908 RVA: 0x00185072 File Offset: 0x00183272
	public bool failSilent
	{
		get
		{
			return this._failSilent;
		}
		set
		{
			this._failSilent = value;
		}
	}

	// Token: 0x060049DD RID: 18909 RVA: 0x0018507B File Offset: 0x0018327B
	private PhotonEvent()
	{
	}

	// Token: 0x060049DE RID: 18910 RVA: 0x0018508A File Offset: 0x0018328A
	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		this._eventId = eventId;
		this.Enable();
	}

	// Token: 0x060049DF RID: 18911 RVA: 0x001850C5 File Offset: 0x001832C5
	public PhotonEvent(string eventId) : this(StaticHash.Compute(eventId))
	{
	}

	// Token: 0x060049E0 RID: 18912 RVA: 0x001850D3 File Offset: 0x001832D3
	public PhotonEvent(int eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x060049E1 RID: 18913 RVA: 0x001850E3 File Offset: 0x001832E3
	public PhotonEvent(string eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x060049E2 RID: 18914 RVA: 0x001850F4 File Offset: 0x001832F4
	~PhotonEvent()
	{
		this.Dispose();
	}

	// Token: 0x060049E3 RID: 18915 RVA: 0x00185120 File Offset: 0x00183320
	public void AddCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (this._delegate != null)
		{
			foreach (Delegate @delegate in this._delegate.GetInvocationList())
			{
				if (@delegate != null && @delegate.Equals(callback))
				{
					return;
				}
			}
		}
		this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Combine(this._delegate, callback);
	}

	// Token: 0x060049E4 RID: 18916 RVA: 0x0018518E File Offset: 0x0018338E
	public void RemoveCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback != null)
		{
			this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Remove(this._delegate, callback);
		}
	}

	// Token: 0x060049E5 RID: 18917 RVA: 0x001851B3 File Offset: 0x001833B3
	public void Enable()
	{
		if (this._disposed)
		{
			return;
		}
		if (this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonEvent.AddPhotonEvent(this);
		}
		this._enabled = true;
	}

	// Token: 0x060049E6 RID: 18918 RVA: 0x001851DB File Offset: 0x001833DB
	public void Disable()
	{
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonEvent.RemovePhotonEvent(this);
		}
		this._enabled = false;
	}

	// Token: 0x060049E7 RID: 18919 RVA: 0x00185203 File Offset: 0x00183403
	public void Dispose()
	{
		this._delegate = null;
		if (this._enabled)
		{
			this._enabled = false;
			if (Application.isPlaying)
			{
				PhotonEvent.RemovePhotonEvent(this);
			}
		}
		this._eventId = -1;
		this._disposed = true;
	}

	// Token: 0x1400007E RID: 126
	// (add) Token: 0x060049E8 RID: 18920 RVA: 0x00185238 File Offset: 0x00183438
	// (remove) Token: 0x060049E9 RID: 18921 RVA: 0x0018526C File Offset: 0x0018346C
	public static event Action<EventData, Exception> OnError;

	// Token: 0x060049EA RID: 18922 RVA: 0x0018529F File Offset: 0x0018349F
	private void InvokeDelegate(int sender, object[] args, PhotonMessageInfoWrapped info)
	{
		Action<int, int, object[], PhotonMessageInfoWrapped> @delegate = this._delegate;
		if (@delegate == null)
		{
			return;
		}
		@delegate.Invoke(sender, this._eventId, args, info);
	}

	// Token: 0x060049EB RID: 18923 RVA: 0x001852BA File Offset: 0x001834BA
	public void RaiseLocal(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.Local, args);
	}

	// Token: 0x060049EC RID: 18924 RVA: 0x001852C4 File Offset: 0x001834C4
	public void RaiseOthers(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteOthers, args);
	}

	// Token: 0x060049ED RID: 18925 RVA: 0x001852CE File Offset: 0x001834CE
	public void RaiseAll(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteAll, args);
	}

	// Token: 0x060049EE RID: 18926 RVA: 0x001852D8 File Offset: 0x001834D8
	private void Raise(PhotonEvent.RaiseMode mode, params object[] args)
	{
		if (this._disposed)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (args != null && args.Length > 20)
		{
			Debug.LogError(string.Format("{0}: too many event args, max is {1}, trying to send {2}. Stopping!", "PhotonEvent", 20, args.Length));
			return;
		}
		SendOptions sendOptions = this._reliable ? PhotonEvent.gSendReliable : PhotonEvent.gSendUnreliable;
		switch (mode)
		{
		case PhotonEvent.RaiseMode.Local:
			this.InvokeDelegate(this._eventId, args, new PhotonMessageInfoWrapped(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.ServerTimestamp));
			return;
		case PhotonEvent.RaiseMode.RemoteOthers:
		{
			object[] array = Enumerable.ToArray<object>(Enumerable.Prepend<object>(args, this._eventId));
			PhotonNetwork.RaiseEvent(176, array, PhotonEvent.gReceiversOthers, sendOptions);
			return;
		}
		case PhotonEvent.RaiseMode.RemoteAll:
		{
			object[] array2 = Enumerable.ToArray<object>(Enumerable.Prepend<object>(args, this._eventId));
			PhotonNetwork.RaiseEvent(176, array2, PhotonEvent.gReceiversAll, sendOptions);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060049EF RID: 18927 RVA: 0x001853CC File Offset: 0x001835CC
	public bool Equals(PhotonEvent other)
	{
		return !(other == null) && (this._eventId == other._eventId && this._enabled == other._enabled && this._reliable == other._reliable && this._failSilent == other._failSilent) && this._disposed == other._disposed;
	}

	// Token: 0x060049F0 RID: 18928 RVA: 0x0018542C File Offset: 0x0018362C
	public override bool Equals(object obj)
	{
		PhotonEvent photonEvent = obj as PhotonEvent;
		return photonEvent != null && this.Equals(photonEvent);
	}

	// Token: 0x060049F1 RID: 18929 RVA: 0x0018544C File Offset: 0x0018364C
	public override int GetHashCode()
	{
		int staticHash = this._eventId.GetStaticHash();
		int i = StaticHash.Compute(this._enabled, this._reliable, this._failSilent, this._disposed);
		return StaticHash.Compute(staticHash, i);
	}

	// Token: 0x060049F2 RID: 18930 RVA: 0x00185488 File Offset: 0x00183688
	static PhotonEvent()
	{
		PhotonEvent.gReceiversAll = new RaiseEventOptions
		{
			Receivers = 1
		};
		PhotonEvent.gReceiversOthers = new RaiseEventOptions
		{
			Receivers = 0
		};
		PhotonEvent.gSendUnreliable = SendOptions.SendUnreliable;
		PhotonEvent.gSendUnreliable.Encrypt = true;
		PhotonEvent.gSendReliable = SendOptions.SendReliable;
		PhotonEvent.gSendReliable.Encrypt = true;
	}

	// Token: 0x060049F3 RID: 18931 RVA: 0x001854ED File Offset: 0x001836ED
	[RuntimeInitializeOnLoadMethod(3)]
	private static void StaticLoadAfterPhotonNetwork()
	{
		PhotonNetwork.NetworkingClient.EventReceived += new Action<EventData>(PhotonEvent.StaticOnEvent);
	}

	// Token: 0x060049F4 RID: 18932 RVA: 0x00185505 File Offset: 0x00183705
	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x060049F5 RID: 18933 RVA: 0x00185513 File Offset: 0x00183713
	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x060049F6 RID: 18934 RVA: 0x00185524 File Offset: 0x00183724
	private static void StaticOnEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		try
		{
			object[] array = evData.CustomData as object[];
			if (array != null && array.Length != 0 && array.Length <= 21)
			{
				object obj = array[0];
				if (obj is int)
				{
					int sender = (int)obj;
					if (sender != -1)
					{
						ListProcessor<PhotonEvent> listProcessor;
						if (PhotonEvent._photonEvents.TryGetValue(sender, ref listProcessor))
						{
							object[] args;
							if (array.Length > 1)
							{
								args = new object[array.Length - 1];
								Array.Copy(array, 1, args, 0, args.Length);
							}
							else
							{
								args = Array.Empty<object>();
							}
							PhotonMessageInfoWrapped info = new PhotonMessageInfoWrapped(evData.Sender, PhotonNetwork.ServerTimestamp);
							listProcessor.ItemProcessor = delegate(in PhotonEvent pEv)
							{
								if (pEv._eventId == -1 || pEv._disposed || !pEv._enabled)
								{
									return;
								}
								pEv.InvokeDelegate(sender, args, info);
							};
							listProcessor.ProcessList();
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Action<EventData, Exception> onError = PhotonEvent.OnError;
			if (onError != null)
			{
				onError.Invoke(evData, ex);
			}
		}
	}

	// Token: 0x060049F7 RID: 18935 RVA: 0x0018563C File Offset: 0x0018383C
	private static void AddPhotonEvent(PhotonEvent photonEvent)
	{
		int eventId = photonEvent._eventId;
		if (eventId == -1)
		{
			return;
		}
		ListProcessor<PhotonEvent> listProcessor;
		if (!PhotonEvent._photonEvents.TryGetValue(eventId, ref listProcessor))
		{
			listProcessor = new ListProcessor<PhotonEvent>(10, null);
			PhotonEvent._photonEvents.Add(eventId, listProcessor);
		}
		if (listProcessor.Contains(photonEvent))
		{
			return;
		}
		listProcessor.Add(photonEvent);
	}

	// Token: 0x060049F8 RID: 18936 RVA: 0x0018568C File Offset: 0x0018388C
	private static void RemovePhotonEvent(PhotonEvent photonEvent)
	{
		ListProcessor<PhotonEvent> listProcessor;
		if (!PhotonEvent._photonEvents.TryGetValue(photonEvent._eventId, ref listProcessor))
		{
			return;
		}
		listProcessor.Remove(photonEvent);
		if (listProcessor.Count == 0)
		{
			PhotonEvent._photonEvents.Remove(photonEvent._eventId);
		}
	}

	// Token: 0x060049F9 RID: 18937 RVA: 0x001856CF File Offset: 0x001838CF
	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	// Token: 0x060049FA RID: 18938 RVA: 0x001856ED File Offset: 0x001838ED
	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.RemoveCallback(callback);
		return photonEvent;
	}

	// Token: 0x04005AB4 RID: 23220
	private const int MAX_EVENT_ARGS = 20;

	// Token: 0x04005AB5 RID: 23221
	private const int INVALID_ID = -1;

	// Token: 0x04005AB6 RID: 23222
	[SerializeField]
	private int _eventId = -1;

	// Token: 0x04005AB7 RID: 23223
	[SerializeField]
	private bool _enabled;

	// Token: 0x04005AB8 RID: 23224
	[SerializeField]
	private bool _reliable;

	// Token: 0x04005AB9 RID: 23225
	[SerializeField]
	private bool _failSilent;

	// Token: 0x04005ABA RID: 23226
	[NonSerialized]
	private bool _disposed;

	// Token: 0x04005ABB RID: 23227
	private Action<int, int, object[], PhotonMessageInfoWrapped> _delegate;

	// Token: 0x04005ABD RID: 23229
	public const byte PHOTON_EVENT_CODE = 176;

	// Token: 0x04005ABE RID: 23230
	private static readonly RaiseEventOptions gReceiversAll;

	// Token: 0x04005ABF RID: 23231
	private static readonly RaiseEventOptions gReceiversOthers;

	// Token: 0x04005AC0 RID: 23232
	private static readonly SendOptions gSendReliable;

	// Token: 0x04005AC1 RID: 23233
	private static readonly SendOptions gSendUnreliable;

	// Token: 0x04005AC2 RID: 23234
	private static readonly Dictionary<int, ListProcessor<PhotonEvent>> _photonEvents = new Dictionary<int, ListProcessor<PhotonEvent>>(20);

	// Token: 0x02000BAF RID: 2991
	public enum RaiseMode
	{
		// Token: 0x04005AC4 RID: 23236
		Local,
		// Token: 0x04005AC5 RID: 23237
		RemoteOthers,
		// Token: 0x04005AC6 RID: 23238
		RemoteAll
	}
}
