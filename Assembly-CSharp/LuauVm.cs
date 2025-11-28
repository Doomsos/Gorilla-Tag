using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000B62 RID: 2914
public class LuauVm : MonoBehaviourPunCallbacks, IOnEventCallback
{
	// Token: 0x060047BF RID: 18367 RVA: 0x00179310 File Offset: 0x00177510
	private void LateUpdate()
	{
		foreach (LuauScriptRunner luauScriptRunner in LuauScriptRunner.ScriptRunners)
		{
			if (!luauScriptRunner.Tick(Time.deltaTime))
			{
				LuauHud.Instance.LuauLog(luauScriptRunner.ScriptName + " errored out");
				LuauScriptRunner.ScriptRunners.Remove(luauScriptRunner);
				break;
			}
		}
	}

	// Token: 0x060047C0 RID: 18368 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x060047C1 RID: 18369 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x060047C2 RID: 18370 RVA: 0x00179390 File Offset: 0x00177590
	public void OnEvent(EventData eventData)
	{
		if (eventData.Code != 180)
		{
			return;
		}
		float num = 0f;
		LuauVm.callTimers.TryGetValue(eventData.Sender, ref num);
		if (num < Time.time - 1f)
		{
			num = Time.time - 1f;
		}
		num += 1f / LuauVm.callCount;
		LuauVm.callTimers[eventData.Sender] = num;
		if (num > Time.time)
		{
			return;
		}
		object[] array = new object[]
		{
			NetworkSystem.Instance.GetPlayer(eventData.Sender),
			(object[])eventData.CustomData
		};
		if (array.Length > 20)
		{
			return;
		}
		LuauVm.eventQueue.Enqueue(array);
	}

	// Token: 0x060047C3 RID: 18371 RVA: 0x00179444 File Offset: 0x00177644
	public unsafe static int SendEvent(lua_State* L, object[] args, bool useTable = true)
	{
		try
		{
			NetPlayer netPlayer = null;
			if (args[0] is NetPlayer)
			{
				netPlayer = (NetPlayer)args[0];
				args = (object[])args[1];
			}
			if (GorillaGameManager.instance.GameType() != GameModeType.Custom)
			{
				return -1;
			}
			Luau.lua_getfield(L, -10002, "onEvent");
			if (Luau.lua_type(L, -1) != 7)
			{
				Luau.lua_pop(L, 1);
				return 0;
			}
			string text = args[0] as string;
			if (text == null)
			{
				Luau.lua_pop(L, 1);
				return 0;
			}
			if (string.IsNullOrEmpty(text))
			{
				Luau.lua_pop(L, 1);
				return 0;
			}
			if (text.Length > 30)
			{
				Luau.lua_pop(L, 1);
				return 0;
			}
			Luau.lua_pushstring(L, (string)args[0]);
			if (useTable)
			{
				Luau.lua_createtable(L, args.Length, 0);
			}
			int i = 1;
			while (i < args.Length)
			{
				object obj = args[i];
				if (StructWrapperUtility.IsType<double>(obj))
				{
					if (double.IsFinite((double)obj))
					{
						Luau.lua_pushnumber(L, (double)obj);
						goto IL_399;
					}
				}
				else
				{
					if (StructWrapperUtility.IsType<bool>(obj))
					{
						Luau.lua_pushboolean(L, (int)obj);
						goto IL_399;
					}
					if (StructWrapperUtility.IsType<Vector3>(obj))
					{
						Vector3 vector = (Vector3)obj;
						vector.ClampMagnitudeSafe(10000000f);
						*Luau.lua_class_push<Vector3>(L, "Vec3") = vector;
						goto IL_399;
					}
					if (StructWrapperUtility.IsType<Quaternion>(obj))
					{
						Quaternion quaternion = (Quaternion)obj;
						if (float.IsFinite(quaternion.x) && float.IsFinite(quaternion.y) && float.IsFinite(quaternion.z) && float.IsFinite(quaternion.w))
						{
							*Luau.lua_class_push<Quaternion>(L, "Quat") = quaternion;
							goto IL_399;
						}
					}
					else if (StructWrapperUtility.IsType<Player>(obj))
					{
						int actorNumber = ((Player)obj).ActorNumber;
						IntPtr ptr;
						if (Bindings.LuauPlayerList.TryGetValue(actorNumber, ref ptr))
						{
							Luau.lua_class_push(L, "Player", ptr);
							goto IL_399;
						}
						NetPlayer netPlayer2 = (NetPlayer)obj;
						if (netPlayer2 == null)
						{
							Luau.lua_pushnil(L);
							goto IL_399;
						}
						Bindings.LuauPlayer* ptr2 = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr2->PlayerID = netPlayer2.ActorNumber;
						ptr2->PlayerName = netPlayer2.SanitizedNickName;
						ptr2->PlayerMaterial = 0;
						ptr2->IsMasterClient = netPlayer2.IsMasterClient;
						RigContainer rigContainer;
						VRRigCache.Instance.TryGetVrrig(netPlayer2, out rigContainer);
						VRRig rig = rigContainer.Rig;
						Bindings.LuauVRRigList[netPlayer2.ActorNumber] = rig;
						Bindings.PlayerFunctions.UpdatePlayer(L, rig, ptr2);
						Bindings.LuauPlayerList[netPlayer2.ActorNumber] = (IntPtr)((void*)ptr2);
						goto IL_399;
					}
					else if (StructWrapperUtility.IsType<Bindings.LuauAIAgent>(obj))
					{
						int entityID = ((Bindings.LuauAIAgent)obj).EntityID;
						IntPtr ptr3;
						if (Bindings.LuauAIAgentList.TryGetValue(entityID, ref ptr3))
						{
							Luau.lua_class_push(L, "AIAgent", ptr3);
							goto IL_399;
						}
						bool flag = false;
						if (Bindings.LuauAIAgentList.Count + Bindings.LuauGrabbablesList.Count == Constants.aiAgentLimit)
						{
							Debug.Log("[LuauVM::OnEvent] Custom Map AI Agent limit has already been reached!");
						}
						else
						{
							GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
							if (entityManager.IsNotNull())
							{
								GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(entityID);
								GameEntity gameEntity = entityManager.GetGameEntity(entityIdFromNetId);
								if (gameEntity.IsNotNull() && gameEntity.gameObject.IsNotNull() && gameEntity.gameObject.GetComponent<GameAgent>() != null)
								{
									Bindings.LuauAIAgent* ptr4 = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
									Bindings.AIAgentFunctions.UpdateEntity(gameEntity, ptr4);
									Bindings.LuauAIAgentList[entityID] = (IntPtr)((void*)ptr4);
									flag = true;
								}
							}
						}
						if (!flag)
						{
							Luau.lua_pushnil(L);
							goto IL_399;
						}
						goto IL_399;
					}
					else
					{
						if (obj == null)
						{
							Luau.lua_pushnil(L);
							goto IL_399;
						}
						goto IL_399;
					}
				}
				IL_3A5:
				i++;
				continue;
				IL_399:
				if (useTable)
				{
					Luau.lua_rawseti(L, -2, i);
					goto IL_3A5;
				}
				goto IL_3A5;
			}
			if (netPlayer != null)
			{
				int actorNumber2 = netPlayer.ActorNumber;
				IntPtr ptr5;
				if (Bindings.LuauPlayerList.TryGetValue(actorNumber2, ref ptr5))
				{
					Luau.lua_class_push(L, "Player", ptr5);
				}
				else
				{
					NetPlayer netPlayer3 = netPlayer;
					if (netPlayer3 == null)
					{
						Luau.lua_pushnil(L);
					}
					else
					{
						Bindings.LuauPlayer* ptr6 = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr6->PlayerID = netPlayer3.ActorNumber;
						ptr6->PlayerName = netPlayer3.SanitizedNickName;
						ptr6->PlayerMaterial = 0;
						ptr6->IsMasterClient = netPlayer3.IsMasterClient;
						RigContainer rigContainer2;
						VRRigCache.Instance.TryGetVrrig(netPlayer3, out rigContainer2);
						VRRig rig2 = rigContainer2.Rig;
						Bindings.LuauVRRigList[netPlayer3.ActorNumber] = rig2;
						Bindings.PlayerFunctions.UpdatePlayer(L, rig2, ptr6);
						Bindings.LuauPlayerList[netPlayer3.ActorNumber] = (IntPtr)((void*)ptr6);
					}
				}
				return Luau.lua_pcall(L, 3, 0, 0);
			}
			return Luau.lua_pcall(L, 2, 0, 0);
		}
		catch (Exception)
		{
		}
		return 0;
	}

	// Token: 0x060047C4 RID: 18372 RVA: 0x0017991C File Offset: 0x00177B1C
	public static void ProcessEvents()
	{
		while (LuauVm.eventQueue.Count > 0)
		{
			object[] args = LuauVm.eventQueue.Dequeue();
			foreach (LuauScriptRunner luauScriptRunner in LuauScriptRunner.ScriptRunners)
			{
				if (luauScriptRunner.ShouldTick)
				{
					int status = LuauVm.SendEvent(luauScriptRunner.L, args, true);
					luauScriptRunner.ShouldTick = !LuauScriptRunner.ErrorCheck(luauScriptRunner.L, status);
				}
			}
		}
		while (LuauVm.localEventQueue.Count > 0)
		{
			object[] args2 = LuauVm.localEventQueue.Dequeue();
			foreach (LuauScriptRunner luauScriptRunner2 in LuauScriptRunner.ScriptRunners)
			{
				if (luauScriptRunner2.ShouldTick)
				{
					int status2 = LuauVm.SendEvent(luauScriptRunner2.L, args2, false);
					luauScriptRunner2.ShouldTick = !LuauScriptRunner.ErrorCheck(luauScriptRunner2.L, status2);
				}
			}
		}
		while (LuauVm.touchEventsQueue.Count > 0)
		{
			GameObject gameObject = LuauVm.touchEventsQueue.Dequeue();
			foreach (LuauScriptRunner luauScriptRunner3 in LuauScriptRunner.ScriptRunners)
			{
				int rid;
				if (luauScriptRunner3.ShouldTick && Bindings.LuauTriggerCallbacks.TryGetValue(gameObject, ref rid))
				{
					Luau.lua_getref(luauScriptRunner3.L, rid);
					if (Luau.lua_type(luauScriptRunner3.L, -1) == 7)
					{
						int status3 = Luau.lua_pcall(luauScriptRunner3.L, 0, 0, 0);
						luauScriptRunner3.ShouldTick = !LuauScriptRunner.ErrorCheck(luauScriptRunner3.L, status3);
					}
				}
			}
		}
	}

	// Token: 0x060047C5 RID: 18373 RVA: 0x00179AF0 File Offset: 0x00177CF0
	protected override void Finalize()
	{
		try
		{
			foreach (GCHandle gchandle in LuauVm.Handles)
			{
				gchandle.Free();
			}
			if (BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
			{
				foreach (KVPair<int, BurstClassInfo.ClassInfo> kvpair in BurstClassInfo.ClassList.InfoFields.Data)
				{
					if (kvpair.Value.FieldList.IsCreated)
					{
						kvpair.Value.FieldList.Dispose();
					}
				}
				BurstClassInfo.ClassList.InfoFields.Data.Dispose();
			}
		}
		catch (ObjectDisposedException ex)
		{
			Debug.Log(ex);
		}
		finally
		{
			base.Finalize();
		}
	}

	// Token: 0x04005877 RID: 22647
	public static List<object> ClassBuilders = new List<object>();

	// Token: 0x04005878 RID: 22648
	public static List<GCHandle> Handles = new List<GCHandle>();

	// Token: 0x04005879 RID: 22649
	private static Dictionary<int, float> callTimers = new Dictionary<int, float>();

	// Token: 0x0400587A RID: 22650
	private static float callCount = 25f;

	// Token: 0x0400587B RID: 22651
	public static Queue<object[]> eventQueue = new Queue<object[]>();

	// Token: 0x0400587C RID: 22652
	public static Queue<object[]> localEventQueue = new Queue<object[]>();

	// Token: 0x0400587D RID: 22653
	public static Queue<GameObject> touchEventsQueue = new Queue<GameObject>();
}
