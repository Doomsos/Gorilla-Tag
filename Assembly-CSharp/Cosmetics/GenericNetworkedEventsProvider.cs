using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics;

public class GenericNetworkedEventsProvider : MonoBehaviour
{
	private enum EventType : byte
	{
		None,
		Int,
		Float,
		Bool,
		Vector3,
		String,
		Long,
		Quaternion
	}

	private RubberDuckEvents _events;

	private VRRig myRig;

	private CallLimiter callLimiter = new CallLimiter(10, 1f);

	public UnityEvent sharedEvent;

	public UnityEvent<int> sharedEvent_int;

	public UnityEvent<float> sharedEvent_float;

	public UnityEvent<bool> sharedEvent_bool;

	public UnityEvent<Vector3> sharedEvent_vector3;

	public UnityEvent<string> sharedEvent_string;

	public UnityEvent<long> sharedEvent_long;

	public UnityEvent<Quaternion> sharedEvent_quaternion;

	private void OnEnable()
	{
		if (myRig == null)
		{
			myRig = GetComponentInParent<VRRig>();
		}
		if (_events == null)
		{
			_events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
		}
		NetPlayer netPlayer = ((myRig != null) ? (myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : NetworkSystem.Instance.LocalPlayer);
		if (netPlayer != null)
		{
			_events.Init(netPlayer);
		}
		if (_events != null)
		{
			_events.Activate.reliable = true;
			_events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(TriggerSharedEvents);
		}
	}

	private void OnDisable()
	{
		if (_events != null)
		{
			_events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(TriggerSharedEvents);
			_events.Dispose();
			_events = null;
		}
	}

	private void TriggerSharedEvents(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target || info.senderID != myRig.creator.ActorNumber)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "TriggerSharedEvents");
		if (!callLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		if (args == null || args.Length == 0)
		{
			sharedEvent?.Invoke();
			return;
		}
		switch ((EventType)(byte)args[0])
		{
		case EventType.None:
			sharedEvent?.Invoke();
			break;
		case EventType.Int:
			sharedEvent_int?.Invoke((int)args[1]);
			break;
		case EventType.Float:
			sharedEvent_float?.Invoke((float)args[1]);
			break;
		case EventType.Bool:
			sharedEvent_bool?.Invoke((bool)args[1]);
			break;
		case EventType.Vector3:
			sharedEvent_vector3?.Invoke((Vector3)args[1]);
			break;
		case EventType.String:
			sharedEvent_string?.Invoke((string)args[1]);
			break;
		case EventType.Long:
			sharedEvent_long?.Invoke((long)args[1]);
			break;
		case EventType.Quaternion:
			sharedEvent_quaternion?.Invoke((Quaternion)args[1]);
			break;
		}
	}

	private void Raise(object[] args)
	{
		if (PhotonNetwork.InRoom && _events?.Activate != null)
		{
			_events.Activate.RaiseOthers(args);
		}
	}

	public void TriggerSharedEvent()
	{
		Raise(new object[1] { (byte)0 });
	}

	public void TriggerSharedEvent_Int(int value)
	{
		Raise(new object[2]
		{
			(byte)1,
			value
		});
	}

	public void TriggerSharedEvent_Float(float value)
	{
		Raise(new object[2]
		{
			(byte)2,
			value
		});
	}

	public void TriggerSharedEvent_Bool(bool value)
	{
		Raise(new object[2]
		{
			(byte)3,
			value
		});
	}

	public void TriggerSharedEvent_Vector3(Vector3 value)
	{
		Raise(new object[2]
		{
			(byte)4,
			value
		});
	}

	public void TriggerSharedEvent_String(string value)
	{
		Raise(new object[2]
		{
			(byte)5,
			value
		});
	}

	public void TriggerSharedEvent_Long(long value)
	{
		Raise(new object[2]
		{
			(byte)6,
			value
		});
	}

	public void TriggerSharedEvent_Quaternion(Quaternion value)
	{
		Raise(new object[2]
		{
			(byte)7,
			value
		});
	}
}
