using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x02000AF2 RID: 2802
[BurstCompile]
public static class Bindings
{
	// Token: 0x060045D2 RID: 17874 RVA: 0x0017261C File Offset: 0x0017081C
	public unsafe static void GameObjectBuilder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.LuauGameObject>("GameObject").AddField("position", "Position").AddField("rotation", "Rotation").AddField("scale", "Scale").AddStaticFunction("findGameObject", new lua_CFunction(Bindings.GameObjectFunctions.FindGameObject)).AddFunction("setCollision", new lua_CFunction(Bindings.GameObjectFunctions.SetCollision)).AddFunction("setVisibility", new lua_CFunction(Bindings.GameObjectFunctions.SetVisibility)).AddFunction("setActive", new lua_CFunction(Bindings.GameObjectFunctions.SetActive)).AddFunction("setText", new lua_CFunction(Bindings.GameObjectFunctions.SetText)).AddFunction("onTouched", new lua_CFunction(Bindings.GameObjectFunctions.OnTouched)).AddFunction("setVelocity", new lua_CFunction(Bindings.GameObjectFunctions.SetVelocity)).AddFunction("getVelocity", new lua_CFunction(Bindings.GameObjectFunctions.GetVelocity)).AddFunction("setColor", new lua_CFunction(Bindings.GameObjectFunctions.SetColor)).AddFunction("findChild", new lua_CFunction(Bindings.GameObjectFunctions.FindChildGameObject)).AddFunction("clone", new lua_CFunction(Bindings.GameObjectFunctions.CloneGameObject)).AddFunction("destroy", new lua_CFunction(Bindings.GameObjectFunctions.DestroyGameObject)).AddFunction("findComponent", new lua_CFunction(Bindings.GameObjectFunctions.FindComponent)).AddFunction("equals", new lua_CFunction(Bindings.GameObjectFunctions.Equals)).Build(L, true));
	}

	// Token: 0x060045D3 RID: 17875 RVA: 0x001727A8 File Offset: 0x001709A8
	public unsafe static void GorillaLocomotionSettingsBuilder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.GorillaLocomotionSettings>("PSettings").AddField("velocityLimit", null).AddField("slideVelocityLimit", null).AddField("maxJumpSpeed", null).AddField("jumpMultiplier", null).Build(L, false));
		Bindings.LocomotionSettings = Luau.lua_class_push<Bindings.GorillaLocomotionSettings>(L);
		Bindings.LocomotionSettings->velocityLimit = GTPlayer.Instance.velocityLimit;
		Bindings.LocomotionSettings->slideVelocityLimit = GTPlayer.Instance.slideVelocityLimit;
		Bindings.LocomotionSettings->maxJumpSpeed = 6.5f;
		Bindings.LocomotionSettings->jumpMultiplier = 1.1f;
		Luau.lua_setglobal(L, "PlayerSettings");
	}

	// Token: 0x060045D4 RID: 17876 RVA: 0x0017285C File Offset: 0x00170A5C
	public unsafe static void PlayerInputBuilder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.PlayerInput>("PInput").AddField("leftXAxis", null).AddField("rightXAxis", null).AddField("leftYAxis", null).AddField("rightYAxis", null).AddField("leftTrigger", null).AddField("rightTrigger", null).AddField("leftGrip", null).AddField("rightGrip", null).AddField("leftPrimaryButton", null).AddField("rightPrimaryButton", null).AddField("leftSecondaryButton", null).AddField("rightSecondaryButton", null).Build(L, false));
		Bindings.LocalPlayerInput = Luau.lua_class_push<Bindings.PlayerInput>(L);
		Bindings.UpdateInputs();
		Luau.lua_setglobal(L, "PlayerInput");
	}

	// Token: 0x060045D5 RID: 17877 RVA: 0x00172924 File Offset: 0x00170B24
	public unsafe static void UpdateInputs()
	{
		if (Bindings.LocalPlayerInput != null)
		{
			Bindings.LocalPlayerInput->leftPrimaryButton = ControllerInputPoller.PrimaryButtonPress(4);
			Bindings.LocalPlayerInput->rightPrimaryButton = ControllerInputPoller.PrimaryButtonPress(5);
			Bindings.LocalPlayerInput->leftSecondaryButton = ControllerInputPoller.SecondaryButtonPress(4);
			Bindings.LocalPlayerInput->rightSecondaryButton = ControllerInputPoller.SecondaryButtonPress(5);
			Bindings.LocalPlayerInput->leftGrip = ControllerInputPoller.GripFloat(4);
			Bindings.LocalPlayerInput->rightGrip = ControllerInputPoller.GripFloat(5);
			Bindings.LocalPlayerInput->leftTrigger = ControllerInputPoller.TriggerFloat(4);
			Bindings.LocalPlayerInput->rightTrigger = ControllerInputPoller.TriggerFloat(5);
			Vector2 vector = ControllerInputPoller.Primary2DAxis(4);
			Vector2 vector2 = ControllerInputPoller.Primary2DAxis(5);
			Bindings.LocalPlayerInput->leftXAxis = vector.x;
			Bindings.LocalPlayerInput->leftYAxis = vector.y;
			Bindings.LocalPlayerInput->rightXAxis = vector2.x;
			Bindings.LocalPlayerInput->rightYAxis = vector2.y;
		}
	}

	// Token: 0x060045D6 RID: 17878 RVA: 0x00172A0C File Offset: 0x00170C0C
	public unsafe static void Vec3Builder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Vector3>("Vec3").AddField("x", null).AddField("y", null).AddField("z", null).AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.New))).AddFunction("__add", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Add))).AddFunction("__sub", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Sub))).AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Mul))).AddFunction("__div", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Div))).AddFunction("__unm", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Unm))).AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Eq))).AddFunction("__tostring", new lua_CFunction(Bindings.Vec3Functions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.Vec3Functions.ToString)).AddFunction("dot", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Dot))).AddFunction("cross", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Cross))).AddFunction("projectOnTo", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Project))).AddFunction("length", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Length))).AddFunction("normalize", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Normalize))).AddFunction("getSafeNormal", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.SafeNormal))).AddStaticFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate))).AddFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate))).AddStaticFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance))).AddFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance))).AddStaticFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp))).AddFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp))).AddProperty("zeroVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.ZeroVector))).AddProperty("oneVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.OneVector))).AddStaticFunction("nearlyEqual", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.NearlyEqual))).Build(L, true));
	}

	// Token: 0x060045D7 RID: 17879 RVA: 0x00172CD4 File Offset: 0x00170ED4
	public unsafe static void QuatBuilder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Quaternion>("Quat").AddField("x", null).AddField("y", null).AddField("z", null).AddField("w", null).AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.New))).AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Mul))).AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Eq))).AddFunction("__tostring", new lua_CFunction(Bindings.QuatFunctions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.QuatFunctions.ToString)).AddStaticFunction("fromEuler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromEuler))).AddStaticFunction("fromDirection", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromDirection))).AddFunction("getUpVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.GetUpVector))).AddFunction("euler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Euler))).Build(L, true));
	}

	// Token: 0x060045D8 RID: 17880 RVA: 0x00172E14 File Offset: 0x00171014
	public unsafe static void PlayerBuilder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.LuauPlayer>("Player").AddField("playerID", "PlayerID").AddField("playerName", "PlayerName").AddField("playerMaterial", "PlayerMaterial").AddField("isMasterClient", "IsMasterClient").AddField("bodyPosition", "BodyPosition").AddField("velocity", "Velocity").AddField("isPCVR", "IsPCVR").AddField("leftHandPosition", "LeftHandPosition").AddField("rightHandPosition", "RightHandPosition").AddField("headRotation", "HeadRotation").AddField("leftHandRotation", "LeftHandRotation").AddField("rightHandRotation", "RightHandRotation").AddField("isInVStump", "IsInVStump").AddField("isEntityAuthority", "IsEntityAuthority").AddStaticFunction("getPlayerByID", new lua_CFunction(Bindings.PlayerFunctions.GetPlayerByID)).Build(L, true));
	}

	// Token: 0x060045D9 RID: 17881 RVA: 0x00172F28 File Offset: 0x00171128
	public unsafe static void AIAgentBuilder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.LuauAIAgent>("AIAgent").AddField("entityID", "EntityID").AddField("agentPosition", "EntityPosition").AddField("agentRotation", "EntityRotation").AddFunction("__tostring", new lua_CFunction(Bindings.AIAgentFunctions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.AIAgentFunctions.ToString)).AddFunction("setDestination", new lua_CFunction(Bindings.AIAgentFunctions.SetDestination)).AddFunction("destroyAgent", new lua_CFunction(Bindings.AIAgentFunctions.DestroyEntity)).AddFunction("playAgentAnimation", new lua_CFunction(Bindings.AIAgentFunctions.PlayAgentAnimation)).AddFunction("getTargetPlayer", new lua_CFunction(Bindings.AIAgentFunctions.GetTarget)).AddFunction("setTargetPlayer", new lua_CFunction(Bindings.AIAgentFunctions.SetTarget)).AddStaticFunction("findPrePlacedAIAgentByID", new lua_CFunction(Bindings.AIAgentFunctions.FindPrePlacedAIAgentByID)).AddStaticFunction("getAIAgentByEntityID", new lua_CFunction(Bindings.AIAgentFunctions.GetAIAgentByEntityID)).AddStaticFunction("spawnAIAgent", new lua_CFunction(Bindings.AIAgentFunctions.SpawnAIAgent)).Build(L, true));
	}

	// Token: 0x060045DA RID: 17882 RVA: 0x0017305C File Offset: 0x0017125C
	public unsafe static void GrabbableEntityBuilder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.LuauGrabbableEntity>("GrabbableEntity").AddField("entityID", "EntityID").AddField("entityPosition", "EntityPosition").AddField("entityRotation", "EntityRotation").AddFunction("__tostring", new lua_CFunction(Bindings.GrabbableEntityFunctions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.GrabbableEntityFunctions.ToString)).AddFunction("destroyGrabbable", new lua_CFunction(Bindings.GrabbableEntityFunctions.DestroyEntity)).AddStaticFunction("findPrePlacedGrabbableEntityByID", new lua_CFunction(Bindings.GrabbableEntityFunctions.FindPrePlacedGrabbableEntityByID)).AddStaticFunction("getGrabbableEntityByEntityID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetGrabbableEntityByEntityID)).AddStaticFunction("getHoldingActorNumberByEntityID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetHoldingActorNumberByEntityID)).AddStaticFunction("getHoldingActorNumberByLuauID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetHoldingActorNumberByLuauID)).AddStaticFunction("spawnGrabbableEntity", new lua_CFunction(Bindings.GrabbableEntityFunctions.SpawnGrabbableEntity)).Build(L, true));
	}

	// Token: 0x060045DB RID: 17883 RVA: 0x00173164 File Offset: 0x00171364
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaStartVibration(lua_State* L)
	{
		bool forLeftController = Luau.lua_toboolean(L, 1) == 1;
		float amplitude = (float)Luau.luaL_checknumber(L, 2);
		float duration = (float)Luau.luaL_checknumber(L, 3);
		GorillaTagger.Instance.StartVibration(forLeftController, amplitude, duration);
		return 0;
	}

	// Token: 0x060045DC RID: 17884 RVA: 0x0017319C File Offset: 0x0017139C
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaPlaySound(lua_State* L)
	{
		int num = (int)Luau.luaL_checknumber(L, 1);
		Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
		float num2 = (float)Luau.luaL_checknumber(L, 3);
		if (num < 0 || num >= VRRig.LocalRig.clipToPlay.Length)
		{
			return 0;
		}
		AudioSource.PlayClipAtPoint(VRRig.LocalRig.clipToPlay[num], vector, num2);
		return 0;
	}

	// Token: 0x060045DD RID: 17885 RVA: 0x001731FC File Offset: 0x001713FC
	public unsafe static void RoomStateBuilder(lua_State* L)
	{
		Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.LuauRoomState>("RState").AddField("isQuest", "IsQuest").AddField("fps", "FPS").AddField("isPrivate", "IsPrivate").AddField("code", "RoomCode").Build(L, false));
		Bindings.RoomState = Luau.lua_class_push<Bindings.LuauRoomState>(L);
		Bindings.UpdateRoomState();
		Bindings.RoomState->IsQuest = false;
		Bindings.RoomState->IsPrivate = !PhotonNetwork.CurrentRoom.IsVisible;
		Bindings.RoomState->RoomCode = PhotonNetwork.CurrentRoom.Name;
		Luau.lua_setglobal(L, "Room");
	}

	// Token: 0x060045DE RID: 17886 RVA: 0x001732B7 File Offset: 0x001714B7
	public unsafe static void UpdateRoomState()
	{
		Bindings.RoomState->FPS = 1f / Time.smoothDeltaTime;
	}

	// Token: 0x040057BA RID: 22458
	public static Dictionary<GameObject, IntPtr> LuauGameObjectList = new Dictionary<GameObject, IntPtr>();

	// Token: 0x040057BB RID: 22459
	public static List<KeyValuePair<GameObject, IntPtr>> LuauGameObjectDepthList = new List<KeyValuePair<GameObject, IntPtr>>();

	// Token: 0x040057BC RID: 22460
	public static Dictionary<IntPtr, GameObject> LuauGameObjectListReverse = new Dictionary<IntPtr, GameObject>();

	// Token: 0x040057BD RID: 22461
	public static Dictionary<GameObject, Bindings.LuauGameObjectInitialState> LuauGameObjectStates = new Dictionary<GameObject, Bindings.LuauGameObjectInitialState>();

	// Token: 0x040057BE RID: 22462
	public static Dictionary<GameObject, int> LuauTriggerCallbacks = new Dictionary<GameObject, int>();

	// Token: 0x040057BF RID: 22463
	public static Dictionary<int, IntPtr> LuauPlayerList = new Dictionary<int, IntPtr>();

	// Token: 0x040057C0 RID: 22464
	public static Dictionary<int, VRRig> LuauVRRigList = new Dictionary<int, VRRig>();

	// Token: 0x040057C1 RID: 22465
	public unsafe static Bindings.GorillaLocomotionSettings* LocomotionSettings;

	// Token: 0x040057C2 RID: 22466
	public unsafe static Bindings.PlayerInput* LocalPlayerInput;

	// Token: 0x040057C3 RID: 22467
	public unsafe static Bindings.LuauRoomState* RoomState;

	// Token: 0x040057C4 RID: 22468
	public static Dictionary<int, IntPtr> LuauAIAgentList = new Dictionary<int, IntPtr>();

	// Token: 0x040057C5 RID: 22469
	public static Dictionary<int, IntPtr> LuauGrabbablesList = new Dictionary<int, IntPtr>();

	// Token: 0x02000AF3 RID: 2803
	public static class LuaEmit
	{
		// Token: 0x060045E0 RID: 17888 RVA: 0x00173338 File Offset: 0x00171538
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Emit(lua_State* L)
		{
			if (Bindings.LuaEmit.callTime < Time.time - 1f)
			{
				Bindings.LuaEmit.callTime = Time.time - 1f;
			}
			Bindings.LuaEmit.callTime += 1f / Bindings.LuaEmit.callCount;
			if (Bindings.LuaEmit.callTime > Time.time)
			{
				LuauHud.Instance.LuauLog("Emit rate limit reached, event not sent");
				return 0;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = 0
			};
			if (Luau.lua_type(L, 2) != 6)
			{
				Luau.luaL_errorL(L, "Argument 2 must be a table", Array.Empty<string>());
				return 0;
			}
			Luau.lua_pushnil(L);
			int num = 0;
			List<object> list = new List<object>();
			list.Add(Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 1))));
			while (Luau.lua_next(L, 2) != 0 && num++ < 10)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				if (lua_Types <= Luau.lua_Types.LUA_TNUMBER)
				{
					if (lua_Types == Luau.lua_Types.LUA_TBOOLEAN)
					{
						list.Add(Luau.lua_toboolean(L, -1) == 1);
						Luau.lua_pop(L, 1);
						continue;
					}
					if (lua_Types == Luau.lua_Types.LUA_TNUMBER)
					{
						list.Add(Luau.luaL_checknumber(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
				}
				else if (lua_Types == Luau.lua_Types.LUA_TTABLE || lua_Types == Luau.lua_Types.LUA_TUSERDATA)
				{
					Luau.luaL_getmetafield(L, -1, "metahash");
					BurstClassInfo.ClassInfo classInfo;
					if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), ref classInfo))
					{
						FixedString64Bytes fixedString64Bytes = "\"Internal Class Info Error No Metatable Found\"";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return 0;
					}
					Luau.lua_pop(L, 1);
					FixedString32Bytes fixedString32Bytes = "Vec3";
					if (ref classInfo.Name == ref fixedString32Bytes)
					{
						list.Add(*Luau.lua_class_get<Vector3>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Quat";
					if (ref classInfo.Name == ref fixedString32Bytes)
					{
						list.Add(*Luau.lua_class_get<Quaternion>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Player";
					if (ref classInfo.Name == ref fixedString32Bytes)
					{
						int playerID = Luau.lua_class_get<Bindings.LuauPlayer>(L, -1)->PlayerID;
						NetPlayer netPlayer = null;
						foreach (NetPlayer netPlayer2 in RoomSystem.PlayersInRoom)
						{
							if (netPlayer2.ActorNumber == playerID)
							{
								netPlayer = netPlayer2;
							}
						}
						if (netPlayer == null)
						{
							list.Add(null);
						}
						else
						{
							list.Add(netPlayer.GetPlayerRef());
						}
						Luau.lua_pop(L, 1);
						continue;
					}
					FixedString32Bytes fixedString32Bytes2 = "\"Unknown Type in table\"";
					Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
					continue;
				}
				FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type in table\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return 0;
			}
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.RaiseEvent(180, list.ToArray(), raiseEventOptions, SendOptions.SendReliable);
			}
			return 0;
		}

		// Token: 0x040057C6 RID: 22470
		private static float callTime = 0f;

		// Token: 0x040057C7 RID: 22471
		private static float callCount = 20f;
	}

	// Token: 0x02000AF4 RID: 2804
	[BurstCompile]
	public struct LuauGameObject
	{
		// Token: 0x040057C8 RID: 22472
		public Vector3 Position;

		// Token: 0x040057C9 RID: 22473
		public Quaternion Rotation;

		// Token: 0x040057CA RID: 22474
		public Vector3 Scale;
	}

	// Token: 0x02000AF5 RID: 2805
	[BurstCompile]
	public struct LuauGameObjectInitialState
	{
		// Token: 0x040057CB RID: 22475
		public Vector3 Position;

		// Token: 0x040057CC RID: 22476
		public Quaternion Rotation;

		// Token: 0x040057CD RID: 22477
		public Vector3 Scale;

		// Token: 0x040057CE RID: 22478
		public bool Visible;

		// Token: 0x040057CF RID: 22479
		public bool Collidable;

		// Token: 0x040057D0 RID: 22480
		public bool Created;
	}

	// Token: 0x02000AF6 RID: 2806
	[BurstCompile]
	public static class GameObjectFunctions
	{
		// Token: 0x060045E2 RID: 17890 RVA: 0x00173644 File Offset: 0x00171844
		public static int GetDepth(GameObject gameObject)
		{
			int num = 0;
			Transform transform = gameObject.transform;
			while (transform.parent != null)
			{
				num++;
				transform = transform.parent;
			}
			return num;
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x00173676 File Offset: 0x00171876
		public static void UpdateDepthList()
		{
			Bindings.LuauGameObjectDepthList.Clear();
			Bindings.LuauGameObjectDepthList = Enumerable.ToList<KeyValuePair<GameObject, IntPtr>>(Enumerable.OrderByDescending<KeyValuePair<GameObject, IntPtr>, int>(Bindings.LuauGameObjectList, (KeyValuePair<GameObject, IntPtr> kv) => Bindings.GameObjectFunctions.GetDepth(kv.Key)));
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x001736B8 File Offset: 0x001718B8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			GameObject gameObject = GameObject.CreatePrimitive(3);
			Bindings.LuauGameObject* ptr = Luau.lua_class_push<Bindings.LuauGameObject>(L);
			ptr->Position = gameObject.transform.position;
			ptr->Rotation = gameObject.transform.rotation;
			ptr->Scale = gameObject.transform.localScale;
			Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr));
			Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr), gameObject);
			return 1;
		}

		// Token: 0x060045E5 RID: 17893 RVA: 0x0017372C File Offset: 0x0017192C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindGameObject(lua_State* L)
		{
			GameObject gameObject = GameObject.Find(new string((sbyte*)Luau.luaL_checkstring(L, 1)));
			if (!(gameObject != null))
			{
				return 0;
			}
			if (!CustomMapLoader.IsCustomScene(gameObject.scene.name))
			{
				return 0;
			}
			IntPtr ptr;
			if (Bindings.LuauGameObjectList.TryGetValue(gameObject, ref ptr))
			{
				Luau.lua_class_push(L, "GameObject", ptr);
			}
			else
			{
				Bindings.LuauGameObject* ptr2 = Luau.lua_class_push<Bindings.LuauGameObject>(L);
				ptr2->Position = gameObject.transform.position;
				ptr2->Rotation = gameObject.transform.rotation;
				ptr2->Scale = gameObject.transform.localScale;
				Bindings.LuauGameObjectInitialState luauGameObjectInitialState = default(Bindings.LuauGameObjectInitialState);
				luauGameObjectInitialState.Position = gameObject.transform.localPosition;
				luauGameObjectInitialState.Rotation = gameObject.transform.localRotation;
				luauGameObjectInitialState.Scale = gameObject.transform.localScale;
				luauGameObjectInitialState.Visible = true;
				luauGameObjectInitialState.Collidable = true;
				luauGameObjectInitialState.Created = false;
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				Collider component2 = gameObject.GetComponent<Collider>();
				if (component2.IsNotNull())
				{
					luauGameObjectInitialState.Collidable = component2.enabled;
				}
				if (component.IsNotNull())
				{
					luauGameObjectInitialState.Visible = component.enabled;
				}
				Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr2));
				Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr2), gameObject);
				Bindings.LuauGameObjectStates.TryAdd(gameObject, luauGameObjectInitialState);
				Bindings.GameObjectFunctions.UpdateDepthList();
			}
			return 1;
		}

		// Token: 0x060045E6 RID: 17894 RVA: 0x0017389C File Offset: 0x00171A9C
		public static Transform FindChild(Transform parent, string name)
		{
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (transform.name == name)
				{
					return transform;
				}
				Transform transform2 = Bindings.GameObjectFunctions.FindChild(transform, name);
				if (transform2 != null)
				{
					return transform2;
				}
			}
			return null;
		}

		// Token: 0x060045E7 RID: 17895 RVA: 0x00173918 File Offset: 0x00171B18
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindChildGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				string name = new string((sbyte*)Luau.luaL_checkstring(L, 2));
				Transform transform = Bindings.GameObjectFunctions.FindChild(gameObject.transform, name);
				GameObject gameObject2 = (transform != null) ? transform.gameObject : null;
				if (gameObject2.IsNotNull())
				{
					IntPtr ptr2;
					if (Bindings.LuauGameObjectList.TryGetValue(gameObject2, ref ptr2))
					{
						Luau.lua_class_push(L, "GameObject", ptr2);
					}
					else
					{
						Bindings.LuauGameObject* ptr3 = Luau.lua_class_push<Bindings.LuauGameObject>(L);
						ptr3->Position = gameObject2.transform.position;
						ptr3->Rotation = gameObject2.transform.rotation;
						ptr3->Scale = gameObject2.transform.localScale;
						Bindings.LuauGameObjectInitialState luauGameObjectInitialState = default(Bindings.LuauGameObjectInitialState);
						luauGameObjectInitialState.Position = gameObject2.transform.localPosition;
						luauGameObjectInitialState.Rotation = gameObject2.transform.localRotation;
						luauGameObjectInitialState.Scale = gameObject2.transform.localScale;
						luauGameObjectInitialState.Visible = true;
						luauGameObjectInitialState.Collidable = true;
						luauGameObjectInitialState.Created = false;
						MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
						Collider component2 = gameObject2.GetComponent<Collider>();
						if (component2.IsNotNull())
						{
							luauGameObjectInitialState.Collidable = component2.enabled;
						}
						if (component.IsNotNull())
						{
							luauGameObjectInitialState.Visible = component.enabled;
						}
						Bindings.LuauGameObjectList.TryAdd(gameObject2, (IntPtr)((void*)ptr3));
						Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr3), gameObject2);
						Bindings.LuauGameObjectStates.TryAdd(gameObject2, luauGameObjectInitialState);
						Bindings.GameObjectFunctions.UpdateDepthList();
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x060045E8 RID: 17896 RVA: 0x00173AB4 File Offset: 0x00171CB4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindComponent(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				if (gameObject == null)
				{
					return 0;
				}
				string text = new string((sbyte*)Luau.luaL_checkstring(L, 2));
				if (text == "ParticleSystem")
				{
					ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
					if (component == null)
					{
						return 0;
					}
					Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem* ptr2 = Luau.lua_class_push<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)ptr2), component);
					return 1;
				}
				else if (text == "AudioSource")
				{
					AudioSource component2 = gameObject.GetComponent<AudioSource>();
					if (component2 == null)
					{
						return 0;
					}
					Bindings.Components.LuauAudioSourceBindings.LuauAudioSource* ptr3 = Luau.lua_class_push<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)ptr3), component2);
					return 1;
				}
				else if (text == "Light")
				{
					Light component3 = gameObject.GetComponent<Light>();
					if (component3 == null)
					{
						return 0;
					}
					Bindings.Components.LuauLightBindings.LuauLight* ptr4 = Luau.lua_class_push<Bindings.Components.LuauLightBindings.LuauLight>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)ptr4), component3);
					return 1;
				}
				else if (text == "Animator")
				{
					Animator component4 = gameObject.GetComponent<Animator>();
					if (component4 == null)
					{
						return 0;
					}
					Bindings.Components.LuauAnimatorBindings.LuauAnimator* ptr5 = Luau.lua_class_push<Bindings.Components.LuauAnimatorBindings.LuauAnimator>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)ptr5), component4);
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x060045E9 RID: 17897 RVA: 0x00173BFC File Offset: 0x00171DFC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int CloneGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, gameObject.transform.parent, false);
				Bindings.LuauGameObject* ptr2 = Luau.lua_class_push<Bindings.LuauGameObject>(L);
				ptr2->Position = gameObject2.transform.position;
				ptr2->Rotation = gameObject2.transform.rotation;
				ptr2->Scale = gameObject2.transform.localScale;
				Bindings.LuauGameObjectInitialState luauGameObjectInitialState = default(Bindings.LuauGameObjectInitialState);
				luauGameObjectInitialState.Position = gameObject2.transform.localPosition;
				luauGameObjectInitialState.Rotation = gameObject2.transform.localRotation;
				luauGameObjectInitialState.Scale = gameObject2.transform.localScale;
				luauGameObjectInitialState.Visible = true;
				luauGameObjectInitialState.Collidable = true;
				luauGameObjectInitialState.Created = true;
				MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
				Collider component2 = gameObject2.GetComponent<Collider>();
				if (component2.IsNotNull())
				{
					luauGameObjectInitialState.Collidable = component2.enabled;
				}
				if (component.IsNotNull())
				{
					luauGameObjectInitialState.Visible = component.enabled;
				}
				Bindings.LuauGameObjectList.TryAdd(gameObject2, (IntPtr)((void*)ptr2));
				Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr2), gameObject2);
				Bindings.LuauGameObjectStates.TryAdd(gameObject2, luauGameObjectInitialState);
				Bindings.GameObjectFunctions.UpdateDepthList();
				return 1;
			}
			return 0;
		}

		// Token: 0x060045EA RID: 17898 RVA: 0x00173D50 File Offset: 0x00171F50
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			Bindings.LuauGameObjectInitialState luauGameObjectInitialState;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject) && Bindings.LuauGameObjectStates.TryGetValue(gameObject, ref luauGameObjectInitialState))
			{
				if (!luauGameObjectInitialState.Created)
				{
					Luau.luaL_errorL(L, "Cannot destroy a non-instantiated GameObject.", Array.Empty<string>());
					return 0;
				}
				Queue<GameObject> queue = new Queue<GameObject>();
				queue.Enqueue(gameObject);
				while (queue.Count != 0)
				{
					GameObject gameObject2 = queue.Dequeue();
					IntPtr intPtr;
					if (Bindings.LuauGameObjectList.TryGetValue(gameObject2, ref intPtr))
					{
						Bindings.LuauGameObjectList.Remove(gameObject2);
						Bindings.LuauGameObjectListReverse.Remove(intPtr);
						Bindings.LuauGameObjectStates.Remove(gameObject2);
						foreach (object obj in gameObject2.transform)
						{
							Transform transform = (Transform)obj;
							queue.Enqueue(transform.gameObject);
						}
					}
				}
				Bindings.GameObjectFunctions.UpdateDepthList();
				gameObject.Destroy();
			}
			return 0;
		}

		// Token: 0x060045EB RID: 17899 RVA: 0x00173E7C File Offset: 0x0017207C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetCollision(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				Collider component = gameObject.GetComponent<Collider>();
				if (component.IsNotNull())
				{
					component.enabled = (Luau.lua_toboolean(L, 2) == 1);
				}
			}
			return 0;
		}

		// Token: 0x060045EC RID: 17900 RVA: 0x00173ED0 File Offset: 0x001720D0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVisibility(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component.IsNotNull())
				{
					component.enabled = (Luau.lua_toboolean(L, 2) == 1);
				}
			}
			return 0;
		}

		// Token: 0x060045ED RID: 17901 RVA: 0x00173F24 File Offset: 0x00172124
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetActive(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				gameObject.SetActive(Luau.lua_toboolean(L, 2) == 1);
			}
			return 0;
		}

		// Token: 0x060045EE RID: 17902 RVA: 0x00173F68 File Offset: 0x00172168
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetText(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				string text = new string(Luau.lua_tostring(L, 2));
				TextMeshPro component = gameObject.GetComponent<TextMeshPro>();
				if (component.IsNotNull())
				{
					component.text = text;
				}
				else
				{
					TextMesh component2 = gameObject.GetComponent<TextMesh>();
					if (component2.IsNotNull())
					{
						component2.text = text;
					}
				}
			}
			return 0;
		}

		// Token: 0x060045EF RID: 17903 RVA: 0x00173FDC File Offset: 0x001721DC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int OnTouched(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				int rid;
				if (Bindings.LuauTriggerCallbacks.TryGetValue(gameObject, ref rid))
				{
					Luau.lua_unref(L, rid);
					Bindings.LuauTriggerCallbacks.Remove(gameObject);
				}
				if (Luau.lua_type(L, 2) == 7)
				{
					int num = Luau.lua_ref(L, 2);
					Bindings.LuauTriggerCallbacks.TryAdd(gameObject, num);
				}
				else
				{
					FixedString32Bytes fixedString32Bytes = "Callback must be a function";
					Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
				}
			}
			return 0;
		}

		// Token: 0x060045F0 RID: 17904 RVA: 0x00174070 File Offset: 0x00172270
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVelocity(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				Vector3 linearVelocity = *Luau.lua_class_get<Vector3>(L, 2);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component.IsNotNull())
				{
					component.linearVelocity = linearVelocity;
				}
			}
			return 0;
		}

		// Token: 0x060045F1 RID: 17905 RVA: 0x001740C8 File Offset: 0x001722C8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetVelocity(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				if (gameObject.IsNull())
				{
					return 0;
				}
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				Vector3* ptr2 = Luau.lua_class_push<Vector3>(L, "Vec3");
				if (component.IsNotNull())
				{
					*ptr2 = component.linearVelocity;
				}
				else
				{
					*ptr2 = Vector3.zero;
				}
			}
			return 1;
		}

		// Token: 0x060045F2 RID: 17906 RVA: 0x00174140 File Offset: 0x00172340
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetColor(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2);
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				Color color;
				color..ctor(Mathf.Clamp01(vector.x / 255f), Mathf.Clamp01(vector.y / 255f), Mathf.Clamp01(vector.z / 255f), 1f);
				TextMeshPro component = gameObject.GetComponent<TextMeshPro>();
				if (component != null)
				{
					component.color = color;
					return 0;
				}
				TextMesh component2 = gameObject.GetComponent<TextMesh>();
				if (component2 != null)
				{
					component2.color = color;
					return 0;
				}
				Renderer component3 = gameObject.GetComponent<Renderer>();
				if (component3 != null)
				{
					component3.material.color = color;
				}
			}
			return 0;
		}

		// Token: 0x060045F3 RID: 17907 RVA: 0x0017421C File Offset: 0x0017241C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Equals(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), ref gameObject))
			{
				Bindings.LuauGameObject* ptr2 = Luau.lua_class_get<Bindings.LuauGameObject>(L, 2, "GameObject");
				GameObject gameObject2;
				if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr2), ref gameObject2) && gameObject == gameObject2)
				{
					Luau.lua_pushboolean(L, 1);
					return 1;
				}
			}
			Luau.lua_pushboolean(L, 0);
			return 1;
		}
	}

	// Token: 0x02000AF8 RID: 2808
	[BurstCompile]
	public struct LuauPlayer
	{
		// Token: 0x040057D3 RID: 22483
		public int PlayerID;

		// Token: 0x040057D4 RID: 22484
		public FixedString32Bytes PlayerName;

		// Token: 0x040057D5 RID: 22485
		public int PlayerMaterial;

		// Token: 0x040057D6 RID: 22486
		[MarshalAs(4)]
		public bool IsMasterClient;

		// Token: 0x040057D7 RID: 22487
		public Vector3 BodyPosition;

		// Token: 0x040057D8 RID: 22488
		public Vector3 Velocity;

		// Token: 0x040057D9 RID: 22489
		[MarshalAs(4)]
		public bool IsPCVR;

		// Token: 0x040057DA RID: 22490
		public Vector3 LeftHandPosition;

		// Token: 0x040057DB RID: 22491
		public Vector3 RightHandPosition;

		// Token: 0x040057DC RID: 22492
		[MarshalAs(4)]
		public bool IsEntityAuthority;

		// Token: 0x040057DD RID: 22493
		public Quaternion HeadRotation;

		// Token: 0x040057DE RID: 22494
		public Quaternion LeftHandRotation;

		// Token: 0x040057DF RID: 22495
		public Quaternion RightHandRotation;

		// Token: 0x040057E0 RID: 22496
		[MarshalAs(4)]
		public bool IsInVStump;
	}

	// Token: 0x02000AF9 RID: 2809
	[BurstCompile]
	public static class PlayerFunctions
	{
		// Token: 0x060045F7 RID: 17911 RVA: 0x001742AC File Offset: 0x001724AC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetPlayerByID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (netPlayer.ActorNumber == num)
				{
					IntPtr ptr;
					if (Bindings.LuauPlayerList.TryGetValue(netPlayer.ActorNumber, ref ptr))
					{
						Luau.lua_class_push(L, "Player", ptr);
					}
					else
					{
						Bindings.LuauPlayer* ptr2 = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr2->PlayerID = netPlayer.ActorNumber;
						ptr2->PlayerMaterial = 0;
						ptr2->IsMasterClient = netPlayer.IsMasterClient;
						Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr2);
						GorillaGameManager instance = GorillaGameManager.instance;
						VRRig vrrig = (instance != null) ? instance.FindPlayerVRRig(netPlayer) : null;
						if (vrrig != null)
						{
							ptr2->PlayerName = vrrig.playerNameVisible;
							Bindings.LuauVRRigList[netPlayer.ActorNumber] = vrrig;
							Bindings.PlayerFunctions.UpdatePlayer(L, vrrig, ptr2);
							Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr2);
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x060045F8 RID: 17912 RVA: 0x001743E4 File Offset: 0x001725E4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdatePlayer(lua_State* L, VRRig p, Bindings.LuauPlayer* data)
		{
			data->BodyPosition = p.transform.position;
			data->Velocity = p.LatestVelocity();
			data->LeftHandPosition = p.leftHandTransform.position;
			data->RightHandPosition = p.rightHandTransform.position;
			data->HeadRotation = p.head.rigTarget.rotation;
			data->LeftHandRotation = p.leftHandTransform.rotation;
			data->RightHandRotation = p.rightHandTransform.rotation;
			if (p.isLocal)
			{
				data->IsInVStump = CustomMapManager.IsLocalPlayerInVirtualStump();
			}
			else if (p.creator != null)
			{
				data->IsInVStump = CustomMapManager.IsRemotePlayerInVirtualStump(p.creator.UserId);
			}
			else
			{
				data->IsInVStump = false;
			}
			data->IsEntityAuthority = (CustomMapsGameManager.instance.IsNotNull() && CustomMapsGameManager.instance.gameEntityManager.IsNotNull() && CustomMapsGameManager.instance.gameEntityManager.IsZoneAuthority());
		}
	}

	// Token: 0x02000AFA RID: 2810
	[BurstCompile]
	public struct LuauAIAgent
	{
		// Token: 0x040057E1 RID: 22497
		public int EntityID;

		// Token: 0x040057E2 RID: 22498
		public Vector3 EntityPosition;

		// Token: 0x040057E3 RID: 22499
		public Quaternion EntityRotation;
	}

	// Token: 0x02000AFB RID: 2811
	[BurstCompile]
	public struct LuauGrabbableEntity
	{
		// Token: 0x040057E4 RID: 22500
		public int EntityID;

		// Token: 0x040057E5 RID: 22501
		public Vector3 EntityPosition;

		// Token: 0x040057E6 RID: 22502
		public Quaternion EntityRotation;
	}

	// Token: 0x02000AFC RID: 2812
	[BurstCompile]
	public static class GrabbableEntityFunctions
	{
		// Token: 0x060045F9 RID: 17913 RVA: 0x001744DC File Offset: 0x001726DC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			string s = "NULL";
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_get<Bindings.LuauGrabbableEntity>(L, 1);
			if (ptr != null)
			{
				s = string.Concat(new string[]
				{
					"ID: ",
					ptr->EntityID.ToString(),
					" | Pos: ",
					ptr->EntityPosition.ToString(),
					" | Rot: ",
					ptr->EntityRotation.ToString()
				});
			}
			Luau.lua_pushstring(L, s);
			return 1;
		}

		// Token: 0x060045FA RID: 17914 RVA: 0x00174560 File Offset: 0x00172760
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetGrabbableEntityByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetGrabbableEntityByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
				if (gameEntity.IsNotNull())
				{
					if (gameEntity.gameObject.IsNull())
					{
						return 0;
					}
					Debug.Log("[LuauBindings::GetGrabbableEntityByEntityID] Found agent: " + gameEntity.gameObject.name);
					IntPtr intPtr;
					if (Bindings.LuauGrabbablesList.TryGetValue(num, ref intPtr))
					{
						Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, (Bindings.LuauGrabbableEntity*)((void*)intPtr));
						Luau.lua_class_push(L, "GrabbableEntity", intPtr);
					}
					else
					{
						Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
						Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, ptr);
						Bindings.LuauGrabbablesList[num] = (IntPtr)((void*)ptr);
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x060045FB RID: 17915 RVA: 0x00174638 File Offset: 0x00172838
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetHoldingActorNumberByLuauID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetHoldingActorNumberByLuauID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNull())
			{
				return 0;
			}
			List<GameEntity> gameEntities = gameEntityManager.GetGameEntities();
			for (int i = 0; i < gameEntities.Count; i++)
			{
				if (!gameEntities[i].gameObject.IsNull())
				{
					CustomMapsGrabbablesController component = gameEntities[i].gameObject.GetComponent<CustomMapsGrabbablesController>();
					if (!component.IsNull())
					{
						Debug.Log("[LuauBindings::GetHoldingActorNumberByLuauID] checking GrabbableController on " + string.Format("{0}, id: {1}", component.gameObject.name, component.luaAgentID));
						if (component.luaAgentID == num)
						{
							Luau.lua_pushnumber(L, (double)component.GetGrabbingActor());
							return 1;
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x060045FC RID: 17916 RVA: 0x00174710 File Offset: 0x00172910
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetHoldingActorNumberByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetHoldingActorNumberByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNull())
			{
				return 0;
			}
			GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity.IsNotNull() || gameEntity.gameObject.IsNull())
			{
				return 0;
			}
			Debug.Log("[LuauBindings::GetHoldingActorNumberByEntityID] Found agent: " + gameEntity.gameObject.name);
			CustomMapsGrabbablesController component = gameEntity.gameObject.GetComponent<CustomMapsGrabbablesController>();
			if (component.IsNull())
			{
				return 0;
			}
			Luau.lua_pushnumber(L, (double)component.GetGrabbingActor());
			return 1;
		}

		// Token: 0x060045FD RID: 17917 RVA: 0x001747B8 File Offset: 0x001729B8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindPrePlacedGrabbableEntityByID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::FindPrePlacedGrabbableEntityByID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				List<GameEntity> gameEntities = gameEntityManager.GetGameEntities();
				for (int i = 0; i < gameEntities.Count; i++)
				{
					if (!gameEntities[i].gameObject.IsNull())
					{
						CustomMapsGrabbablesController component = gameEntities[i].gameObject.GetComponent<CustomMapsGrabbablesController>();
						if (!component.IsNull())
						{
							Debug.Log("[LuauBindings::FindPrePlacedGrabbableEntityByID] checking GrabbableController on " + string.Format("{0}, id: {1}", component.gameObject.name, component.luaAgentID));
							if (component.luaAgentID == num)
							{
								IntPtr intPtr;
								if (Bindings.LuauGrabbablesList.TryGetValue(gameEntities[i].GetNetId(), ref intPtr))
								{
									Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntities[i], (Bindings.LuauGrabbableEntity*)((void*)intPtr));
									Luau.lua_class_push(L, "GrabbableEntity", intPtr);
								}
								else
								{
									Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
									Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntities[i], ptr);
									Bindings.LuauGrabbablesList[gameEntities[i].GetNetId()] = (IntPtr)((void*)ptr);
								}
								return 1;
							}
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x060045FE RID: 17918 RVA: 0x00174900 File Offset: 0x00172B00
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SpawnGrabbableEntity(lua_State* L)
		{
			Debug.Log("[LuauBindings::SpawnGrabbableEntity]");
			CustomMapsGameManager instance = CustomMapsGameManager.instance;
			GameEntityManager gameEntityManager = instance.IsNotNull() ? instance.gameEntityManager : null;
			if (gameEntityManager.IsNull())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed. EntityManager is null.");
				return 0;
			}
			if (!gameEntityManager.IsZoneAuthority())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed. Local Player doesn't have Entity Authority.");
				return 0;
			}
			if (Bindings.LuauAIAgentList.Count + Bindings.LuauGrabbablesList.Count == Constants.aiAgentLimit)
			{
				LuauHud.Instance.LuauLog(string.Format("SpawnGrabbableEntity failed, EntityLimit of {0}", Constants.aiAgentLimit) + " has already been reached.");
				return 0;
			}
			int enemyTypeId = (int)Luau.luaL_checknumber(L, 1);
			Vector3 position = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 3, "Quat");
			GameEntityId id = instance.SpawnGrabbableAtLocation(enemyTypeId, position, rotation);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] spawnedGrabbable");
			if (!id.IsValid())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed to create entity.");
				return 0;
			}
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] spawnedGrabbable ID valid");
			GameEntity gameEntity = gameEntityManager.GetGameEntity(id);
			IntPtr intPtr;
			if (Bindings.LuauGrabbablesList.TryGetValue(gameEntity.GetNetId(), ref intPtr))
			{
				Debug.Log("[LuauBindings::SpawnGrabbableEntity] fround grabbable");
				Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, (Bindings.LuauGrabbableEntity*)((void*)intPtr));
				Luau.lua_class_push(L, "GrabbableEntity", intPtr);
				return 1;
			}
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] grabbable not found");
			Luau.lua_getglobal(L, "GrabbableEntities");
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
			Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, ptr);
			Bindings.LuauGrabbablesList[gameEntity.GetNetId()] = (IntPtr)((void*)ptr);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] created new grabbable");
			Luau.lua_rawseti(L, -2, Bindings.LuauGrabbablesList.Count);
			Luau.lua_pop(L, 1);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] pushing new grabbable");
			Luau.lua_class_push(L, "GrabbableEntity", (IntPtr)((void*)ptr));
			return 1;
		}

		// Token: 0x060045FF RID: 17919 RVA: 0x00174AEA File Offset: 0x00172CEA
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdateEntity(GameEntity entity, Bindings.LuauGrabbableEntity* luaAgent)
		{
			luaAgent->EntityID = entity.GetNetId();
			luaAgent->EntityPosition = entity.transform.position;
			luaAgent->EntityRotation = entity.transform.rotation;
		}

		// Token: 0x06004600 RID: 17920 RVA: 0x00174B1C File Offset: 0x00172D1C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyEntity(lua_State* L)
		{
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_get<Bindings.LuauGrabbableEntity>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(ptr->EntityID);
					entityManager.RequestDestroyItem(entityIdFromNetId);
				}
			}
			return 0;
		}
	}

	// Token: 0x02000AFD RID: 2813
	[BurstCompile]
	public static class AIAgentFunctions
	{
		// Token: 0x06004601 RID: 17921 RVA: 0x00174B5C File Offset: 0x00172D5C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			string s = "NULL";
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				s = string.Concat(new string[]
				{
					"ID: ",
					ptr->EntityID.ToString(),
					" | Pos: ",
					ptr->EntityPosition.ToString(),
					" | Rot: ",
					ptr->EntityRotation.ToString()
				});
			}
			Luau.lua_pushstring(L, s);
			return 1;
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x00174BE0 File Offset: 0x00172DE0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetAIAgentByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetAIAgentByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
				if (gameEntity.IsNotNull())
				{
					if (gameEntity.gameObject.IsNull())
					{
						return 0;
					}
					if (gameEntity.gameObject.GetComponent<GameAgent>().IsNotNull())
					{
						Debug.Log("[LuauBindings::GetAIAgentByEntityID] Found agent: " + gameEntity.gameObject.name);
						IntPtr intPtr;
						if (Bindings.LuauAIAgentList.TryGetValue(num, ref intPtr))
						{
							Bindings.AIAgentFunctions.UpdateEntity(gameEntity, (Bindings.LuauAIAgent*)((void*)intPtr));
							Luau.lua_class_push(L, "AIAgent", intPtr);
						}
						else
						{
							Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
							Bindings.AIAgentFunctions.UpdateEntity(gameEntity, ptr);
							Bindings.LuauAIAgentList[num] = (IntPtr)((void*)ptr);
						}
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004603 RID: 17923 RVA: 0x00174CCC File Offset: 0x00172ECC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindPrePlacedAIAgentByID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			GameAgentManager gameAgentManager = CustomMapsGameManager.instance.gameAgentManager;
			if (gameAgentManager.IsNotNull())
			{
				List<GameAgent> agents = gameAgentManager.GetAgents();
				for (int i = 0; i < agents.Count; i++)
				{
					if (!agents[i].gameObject.IsNull())
					{
						CustomMapsAIBehaviourController component = agents[i].gameObject.GetComponent<CustomMapsAIBehaviourController>();
						if (!component.IsNull() && component.luaAgentID == num)
						{
							IntPtr intPtr;
							if (Bindings.LuauAIAgentList.TryGetValue(agents[i].entity.GetNetId(), ref intPtr))
							{
								Bindings.AIAgentFunctions.UpdateEntity(agents[i].entity, (Bindings.LuauAIAgent*)((void*)intPtr));
								Luau.lua_class_push(L, "AIAgent", intPtr);
							}
							else
							{
								Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
								Bindings.AIAgentFunctions.UpdateEntity(agents[i].entity, ptr);
								Bindings.LuauAIAgentList[agents[i].entity.GetNetId()] = (IntPtr)((void*)ptr);
							}
							return 1;
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004604 RID: 17924 RVA: 0x00174DE4 File Offset: 0x00172FE4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SpawnAIAgent(lua_State* L)
		{
			CustomMapsGameManager instance = CustomMapsGameManager.instance;
			GameEntityManager gameEntityManager = instance.IsNotNull() ? instance.gameEntityManager : null;
			if (gameEntityManager.IsNull())
			{
				LuauHud.Instance.LuauLog("SpawnAIAgent failed. EntityManager is null.");
				return 0;
			}
			if (!gameEntityManager.IsZoneAuthority())
			{
				LuauHud.Instance.LuauLog("SpawnAIAgent failed. Local Player doesn't have Entity Authority.");
				return 0;
			}
			if (Bindings.LuauAIAgentList.Count + Bindings.LuauGrabbablesList.Count == Constants.aiAgentLimit)
			{
				LuauHud.Instance.LuauLog(string.Format("SpawnAIAgent failed, AIAgentLimit of {0}", Constants.aiAgentLimit) + " has already been reached.");
				return 0;
			}
			int enemyTypeId = (int)Luau.luaL_checknumber(L, 1);
			Vector3 position = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 3, "Quat");
			GameEntityId id = instance.SpawnEnemyAtLocation(enemyTypeId, position, rotation);
			if (id.IsValid())
			{
				GameEntity gameEntity = gameEntityManager.GetGameEntity(id);
				if ((gameEntity.IsNotNull() ? gameEntity.gameObject.GetComponent<GameAgent>() : null).IsNotNull())
				{
					IntPtr intPtr;
					if (Bindings.LuauAIAgentList.TryGetValue(gameEntity.GetNetId(), ref intPtr))
					{
						Bindings.AIAgentFunctions.UpdateEntity(gameEntity, (Bindings.LuauAIAgent*)((void*)intPtr));
						Luau.lua_class_push(L, "AIAgent", intPtr);
						return 1;
					}
					Luau.lua_getglobal(L, "AIAgents");
					Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
					Bindings.AIAgentFunctions.UpdateEntity(gameEntity, ptr);
					Bindings.LuauAIAgentList[gameEntity.GetNetId()] = (IntPtr)((void*)ptr);
					Luau.lua_rawseti(L, -2, Bindings.LuauAIAgentList.Count);
					Luau.lua_pop(L, 1);
					Luau.lua_class_push(L, "AIAgent", (IntPtr)((void*)ptr));
					return 1;
				}
			}
			LuauHud.Instance.LuauLog("SpawnAIAgent failed to create entity.");
			return 0;
		}

		// Token: 0x06004605 RID: 17925 RVA: 0x00174FAC File Offset: 0x001731AC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetDestination(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			Vector3* ptr2 = Luau.lua_class_get<Vector3>(L, 2);
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				CustomMapsAIBehaviourController component = gameEntityManager.GetGameEntity(gameEntityManager.GetEntityIdFromNetId(ptr->EntityID)).gameObject.GetComponent<CustomMapsAIBehaviourController>();
				if (component.IsNotNull())
				{
					component.RequestDestination(*ptr2);
				}
			}
			return 0;
		}

		// Token: 0x06004606 RID: 17926 RVA: 0x00175010 File Offset: 0x00173210
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int PlayAgentAnimation(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			string stateName = Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 2)));
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
					if (behaviorControllerForEntity.IsNotNull())
					{
						behaviorControllerForEntity.PlayAnimation(stateName, 0f);
					}
				}
			}
			return 0;
		}

		// Token: 0x06004607 RID: 17927 RVA: 0x00175074 File Offset: 0x00173274
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetTarget(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr == null)
			{
				return 0;
			}
			GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
			if (entityManager.IsNull() || !entityManager.IsAuthority())
			{
				return 0;
			}
			int num = (int)Luau.luaL_checknumber(L, 2);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(num, out rigContainer))
			{
				num = -1;
			}
			CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
			if (behaviorControllerForEntity.IsNull())
			{
				return 0;
			}
			if (num == -1)
			{
				behaviorControllerForEntity.ClearTarget();
			}
			else
			{
				GRPlayer component = rigContainer.Rig.GetComponent<GRPlayer>();
				behaviorControllerForEntity.SetTarget(component);
			}
			return 0;
		}

		// Token: 0x06004608 RID: 17928 RVA: 0x00175104 File Offset: 0x00173304
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetTarget(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull() && entityManager.IsAuthority())
				{
					CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
					if (behaviorControllerForEntity.IsNotNull() && behaviorControllerForEntity.TargetPlayer.IsNotNull() && behaviorControllerForEntity.TargetPlayer.MyRig.IsNotNull() && !behaviorControllerForEntity.TargetPlayer.MyRig.OwningNetPlayer.IsNull)
					{
						Luau.lua_pushnumber(L, (double)behaviorControllerForEntity.TargetPlayer.MyRig.OwningNetPlayer.ActorNumber);
						return 1;
					}
				}
			}
			Luau.lua_pushnumber(L, -1.0);
			return 1;
		}

		// Token: 0x06004609 RID: 17929 RVA: 0x001751B5 File Offset: 0x001733B5
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdateEntity(GameEntity entity, Bindings.LuauAIAgent* luaAgent)
		{
			luaAgent->EntityID = entity.GetNetId();
			luaAgent->EntityPosition = entity.transform.position;
			luaAgent->EntityRotation = entity.transform.rotation;
		}

		// Token: 0x0600460A RID: 17930 RVA: 0x001751E8 File Offset: 0x001733E8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyEntity(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(ptr->EntityID);
					entityManager.RequestDestroyItem(entityIdFromNetId);
				}
			}
			return 0;
		}
	}

	// Token: 0x02000AFE RID: 2814
	[BurstCompile]
	public static class Vec3Functions
	{
		// Token: 0x0600460B RID: 17931 RVA: 0x00175225 File Offset: 0x00173425
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.Vec3Functions.New_0000460B$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600460C RID: 17932 RVA: 0x0017522D File Offset: 0x0017342D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Add(lua_State* L)
		{
			return Bindings.Vec3Functions.Add_0000460C$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600460D RID: 17933 RVA: 0x00175235 File Offset: 0x00173435
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Sub(lua_State* L)
		{
			return Bindings.Vec3Functions.Sub_0000460D$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600460E RID: 17934 RVA: 0x0017523D File Offset: 0x0017343D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.Vec3Functions.Mul_0000460E$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600460F RID: 17935 RVA: 0x00175245 File Offset: 0x00173445
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Div(lua_State* L)
		{
			return Bindings.Vec3Functions.Div_0000460F$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004610 RID: 17936 RVA: 0x0017524D File Offset: 0x0017344D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Unm(lua_State* L)
		{
			return Bindings.Vec3Functions.Unm_00004610$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004611 RID: 17937 RVA: 0x00175255 File Offset: 0x00173455
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.Vec3Functions.Eq_00004611$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004612 RID: 17938 RVA: 0x00175260 File Offset: 0x00173460
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushstring(L, vector.ToString());
			return 1;
		}

		// Token: 0x06004613 RID: 17939 RVA: 0x00175299 File Offset: 0x00173499
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Dot(lua_State* L)
		{
			return Bindings.Vec3Functions.Dot_00004613$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004614 RID: 17940 RVA: 0x001752A1 File Offset: 0x001734A1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Cross(lua_State* L)
		{
			return Bindings.Vec3Functions.Cross_00004614$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004615 RID: 17941 RVA: 0x001752A9 File Offset: 0x001734A9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Project(lua_State* L)
		{
			return Bindings.Vec3Functions.Project_00004615$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004616 RID: 17942 RVA: 0x001752B1 File Offset: 0x001734B1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Length(lua_State* L)
		{
			return Bindings.Vec3Functions.Length_00004616$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004617 RID: 17943 RVA: 0x001752B9 File Offset: 0x001734B9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Normalize(lua_State* L)
		{
			return Bindings.Vec3Functions.Normalize_00004617$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004618 RID: 17944 RVA: 0x001752C1 File Offset: 0x001734C1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SafeNormal(lua_State* L)
		{
			return Bindings.Vec3Functions.SafeNormal_00004618$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004619 RID: 17945 RVA: 0x001752C9 File Offset: 0x001734C9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Distance(lua_State* L)
		{
			return Bindings.Vec3Functions.Distance_00004619$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600461A RID: 17946 RVA: 0x001752D1 File Offset: 0x001734D1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Lerp(lua_State* L)
		{
			return Bindings.Vec3Functions.Lerp_0000461A$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600461B RID: 17947 RVA: 0x001752D9 File Offset: 0x001734D9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Rotate(lua_State* L)
		{
			return Bindings.Vec3Functions.Rotate_0000461B$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600461C RID: 17948 RVA: 0x001752E1 File Offset: 0x001734E1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ZeroVector(lua_State* L)
		{
			return Bindings.Vec3Functions.ZeroVector_0000461C$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600461D RID: 17949 RVA: 0x001752E9 File Offset: 0x001734E9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int OneVector(lua_State* L)
		{
			return Bindings.Vec3Functions.OneVector_0000461D$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600461E RID: 17950 RVA: 0x001752F1 File Offset: 0x001734F1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int NearlyEqual(lua_State* L)
		{
			return Bindings.Vec3Functions.NearlyEqual_0000461E$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600461F RID: 17951 RVA: 0x001752FC File Offset: 0x001734FC
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int New$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr.x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr.y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr.z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			return 1;
		}

		// Token: 0x06004620 RID: 17952 RVA: 0x00175360 File Offset: 0x00173560
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Add$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector + vector2;
			return 1;
		}

		// Token: 0x06004621 RID: 17953 RVA: 0x001753B8 File Offset: 0x001735B8
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Sub$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector - vector2;
			return 1;
		}

		// Token: 0x06004622 RID: 17954 RVA: 0x00175410 File Offset: 0x00173610
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float num = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector * num;
			return 1;
		}

		// Token: 0x06004623 RID: 17955 RVA: 0x0017545C File Offset: 0x0017365C
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Div$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float num = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector / num;
			return 1;
		}

		// Token: 0x06004624 RID: 17956 RVA: 0x001754A8 File Offset: 0x001736A8
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Unm$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = -vector;
			return 1;
		}

		// Token: 0x06004625 RID: 17957 RVA: 0x001754E8 File Offset: 0x001736E8
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			int num = (vector == vector2) ? 1 : 0;
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x06004626 RID: 17958 RVA: 0x00175538 File Offset: 0x00173738
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Dot$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double n = (double)Vector3.Dot(vector, vector2);
			Luau.lua_pushnumber(L, n);
			return 1;
		}

		// Token: 0x06004627 RID: 17959 RVA: 0x00175584 File Offset: 0x00173784
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Cross$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Cross(vector, vector2);
			return 1;
		}

		// Token: 0x06004628 RID: 17960 RVA: 0x001755DC File Offset: 0x001737DC
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Project$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Project(vector, vector2);
			return 1;
		}

		// Token: 0x06004629 RID: 17961 RVA: 0x00175634 File Offset: 0x00173834
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Length$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Magnitude(vector));
			return 1;
		}

		// Token: 0x0600462A RID: 17962 RVA: 0x00175666 File Offset: 0x00173866
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Normalize$BurstManaged(lua_State* L)
		{
			Luau.lua_class_get<Vector3>(L, 1, "Vec3").Normalize();
			return 0;
		}

		// Token: 0x0600462B RID: 17963 RVA: 0x00175680 File Offset: 0x00173880
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int SafeNormal$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector.normalized;
			return 1;
		}

		// Token: 0x0600462C RID: 17964 RVA: 0x001756C4 File Offset: 0x001738C4
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Distance$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Distance(vector, vector2));
			return 1;
		}

		// Token: 0x0600462D RID: 17965 RVA: 0x00175710 File Offset: 0x00173910
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Lerp$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double num = Luau.luaL_checknumber(L, 3);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Lerp(vector, vector2, (float)num);
			return 1;
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x00175774 File Offset: 0x00173974
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Rotate$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion * vector;
			return 1;
		}

		// Token: 0x0600462F RID: 17967 RVA: 0x001757CC File Offset: 0x001739CC
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int ZeroVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr.x = 0f;
			ptr.y = 0f;
			ptr.z = 0f;
			return 1;
		}

		// Token: 0x06004630 RID: 17968 RVA: 0x001757FF File Offset: 0x001739FF
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int OneVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr.x = 1f;
			ptr.y = 1f;
			ptr.z = 1f;
			return 1;
		}

		// Token: 0x06004631 RID: 17969 RVA: 0x00175834 File Offset: 0x00173A34
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int NearlyEqual$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			float num = (float)Luau.luaL_optnumber(L, 3, 0.0001);
			bool flag = Math.Abs(vector.x - vector2.x) <= num;
			if (flag && Math.Abs(vector.y - vector2.y) > num)
			{
				flag = false;
			}
			if (flag && Math.Abs(vector.z - vector2.z) > num)
			{
				flag = false;
			}
			Luau.lua_pushboolean(L, flag ? 1 : 0);
			return 1;
		}

		// Token: 0x02000AFF RID: 2815
		// (Invoke) Token: 0x06004633 RID: 17971
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int New_0000460B$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B00 RID: 2816
		internal static class New_0000460B$BurstDirectCall
		{
			// Token: 0x06004636 RID: 17974 RVA: 0x001758DC File Offset: 0x00173ADC
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.New_0000460B$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.New_0000460B$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.New_0000460B$PostfixBurstDelegate>(new Bindings.Vec3Functions.New_0000460B$PostfixBurstDelegate(Bindings.Vec3Functions.New)).Value;
				}
				A_0 = Bindings.Vec3Functions.New_0000460B$BurstDirectCall.Pointer;
			}

			// Token: 0x06004637 RID: 17975 RVA: 0x0017591C File Offset: 0x00173B1C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.New_0000460B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004638 RID: 17976 RVA: 0x00175934 File Offset: 0x00173B34
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.New_0000460B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.New$BurstManaged(L);
			}

			// Token: 0x040057E7 RID: 22503
			private static IntPtr Pointer;
		}

		// Token: 0x02000B01 RID: 2817
		// (Invoke) Token: 0x0600463A RID: 17978
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Add_0000460C$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B02 RID: 2818
		internal static class Add_0000460C$BurstDirectCall
		{
			// Token: 0x0600463D RID: 17981 RVA: 0x00175968 File Offset: 0x00173B68
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Add_0000460C$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Add_0000460C$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Add_0000460C$PostfixBurstDelegate>(new Bindings.Vec3Functions.Add_0000460C$PostfixBurstDelegate(Bindings.Vec3Functions.Add)).Value;
				}
				A_0 = Bindings.Vec3Functions.Add_0000460C$BurstDirectCall.Pointer;
			}

			// Token: 0x0600463E RID: 17982 RVA: 0x001759A8 File Offset: 0x00173BA8
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Add_0000460C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600463F RID: 17983 RVA: 0x001759C0 File Offset: 0x00173BC0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Add_0000460C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Add$BurstManaged(L);
			}

			// Token: 0x040057E8 RID: 22504
			private static IntPtr Pointer;
		}

		// Token: 0x02000B03 RID: 2819
		// (Invoke) Token: 0x06004641 RID: 17985
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Sub_0000460D$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B04 RID: 2820
		internal static class Sub_0000460D$BurstDirectCall
		{
			// Token: 0x06004644 RID: 17988 RVA: 0x001759F4 File Offset: 0x00173BF4
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Sub_0000460D$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Sub_0000460D$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Sub_0000460D$PostfixBurstDelegate>(new Bindings.Vec3Functions.Sub_0000460D$PostfixBurstDelegate(Bindings.Vec3Functions.Sub)).Value;
				}
				A_0 = Bindings.Vec3Functions.Sub_0000460D$BurstDirectCall.Pointer;
			}

			// Token: 0x06004645 RID: 17989 RVA: 0x00175A34 File Offset: 0x00173C34
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Sub_0000460D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004646 RID: 17990 RVA: 0x00175A4C File Offset: 0x00173C4C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Sub_0000460D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Sub$BurstManaged(L);
			}

			// Token: 0x040057E9 RID: 22505
			private static IntPtr Pointer;
		}

		// Token: 0x02000B05 RID: 2821
		// (Invoke) Token: 0x06004648 RID: 17992
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Mul_0000460E$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B06 RID: 2822
		internal static class Mul_0000460E$BurstDirectCall
		{
			// Token: 0x0600464B RID: 17995 RVA: 0x00175A80 File Offset: 0x00173C80
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Mul_0000460E$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Mul_0000460E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Mul_0000460E$PostfixBurstDelegate>(new Bindings.Vec3Functions.Mul_0000460E$PostfixBurstDelegate(Bindings.Vec3Functions.Mul)).Value;
				}
				A_0 = Bindings.Vec3Functions.Mul_0000460E$BurstDirectCall.Pointer;
			}

			// Token: 0x0600464C RID: 17996 RVA: 0x00175AC0 File Offset: 0x00173CC0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Mul_0000460E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600464D RID: 17997 RVA: 0x00175AD8 File Offset: 0x00173CD8
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Mul_0000460E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Mul$BurstManaged(L);
			}

			// Token: 0x040057EA RID: 22506
			private static IntPtr Pointer;
		}

		// Token: 0x02000B07 RID: 2823
		// (Invoke) Token: 0x0600464F RID: 17999
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Div_0000460F$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B08 RID: 2824
		internal static class Div_0000460F$BurstDirectCall
		{
			// Token: 0x06004652 RID: 18002 RVA: 0x00175B0C File Offset: 0x00173D0C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Div_0000460F$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Div_0000460F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Div_0000460F$PostfixBurstDelegate>(new Bindings.Vec3Functions.Div_0000460F$PostfixBurstDelegate(Bindings.Vec3Functions.Div)).Value;
				}
				A_0 = Bindings.Vec3Functions.Div_0000460F$BurstDirectCall.Pointer;
			}

			// Token: 0x06004653 RID: 18003 RVA: 0x00175B4C File Offset: 0x00173D4C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Div_0000460F$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004654 RID: 18004 RVA: 0x00175B64 File Offset: 0x00173D64
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Div_0000460F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Div$BurstManaged(L);
			}

			// Token: 0x040057EB RID: 22507
			private static IntPtr Pointer;
		}

		// Token: 0x02000B09 RID: 2825
		// (Invoke) Token: 0x06004656 RID: 18006
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Unm_00004610$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B0A RID: 2826
		internal static class Unm_00004610$BurstDirectCall
		{
			// Token: 0x06004659 RID: 18009 RVA: 0x00175B98 File Offset: 0x00173D98
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Unm_00004610$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Unm_00004610$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Unm_00004610$PostfixBurstDelegate>(new Bindings.Vec3Functions.Unm_00004610$PostfixBurstDelegate(Bindings.Vec3Functions.Unm)).Value;
				}
				A_0 = Bindings.Vec3Functions.Unm_00004610$BurstDirectCall.Pointer;
			}

			// Token: 0x0600465A RID: 18010 RVA: 0x00175BD8 File Offset: 0x00173DD8
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Unm_00004610$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600465B RID: 18011 RVA: 0x00175BF0 File Offset: 0x00173DF0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Unm_00004610$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Unm$BurstManaged(L);
			}

			// Token: 0x040057EC RID: 22508
			private static IntPtr Pointer;
		}

		// Token: 0x02000B0B RID: 2827
		// (Invoke) Token: 0x0600465D RID: 18013
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Eq_00004611$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B0C RID: 2828
		internal static class Eq_00004611$BurstDirectCall
		{
			// Token: 0x06004660 RID: 18016 RVA: 0x00175C24 File Offset: 0x00173E24
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Eq_00004611$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Eq_00004611$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Eq_00004611$PostfixBurstDelegate>(new Bindings.Vec3Functions.Eq_00004611$PostfixBurstDelegate(Bindings.Vec3Functions.Eq)).Value;
				}
				A_0 = Bindings.Vec3Functions.Eq_00004611$BurstDirectCall.Pointer;
			}

			// Token: 0x06004661 RID: 18017 RVA: 0x00175C64 File Offset: 0x00173E64
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Eq_00004611$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004662 RID: 18018 RVA: 0x00175C7C File Offset: 0x00173E7C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Eq_00004611$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Eq$BurstManaged(L);
			}

			// Token: 0x040057ED RID: 22509
			private static IntPtr Pointer;
		}

		// Token: 0x02000B0D RID: 2829
		// (Invoke) Token: 0x06004664 RID: 18020
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Dot_00004613$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B0E RID: 2830
		internal static class Dot_00004613$BurstDirectCall
		{
			// Token: 0x06004667 RID: 18023 RVA: 0x00175CB0 File Offset: 0x00173EB0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Dot_00004613$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Dot_00004613$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Dot_00004613$PostfixBurstDelegate>(new Bindings.Vec3Functions.Dot_00004613$PostfixBurstDelegate(Bindings.Vec3Functions.Dot)).Value;
				}
				A_0 = Bindings.Vec3Functions.Dot_00004613$BurstDirectCall.Pointer;
			}

			// Token: 0x06004668 RID: 18024 RVA: 0x00175CF0 File Offset: 0x00173EF0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Dot_00004613$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004669 RID: 18025 RVA: 0x00175D08 File Offset: 0x00173F08
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Dot_00004613$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Dot$BurstManaged(L);
			}

			// Token: 0x040057EE RID: 22510
			private static IntPtr Pointer;
		}

		// Token: 0x02000B0F RID: 2831
		// (Invoke) Token: 0x0600466B RID: 18027
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Cross_00004614$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B10 RID: 2832
		internal static class Cross_00004614$BurstDirectCall
		{
			// Token: 0x0600466E RID: 18030 RVA: 0x00175D3C File Offset: 0x00173F3C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Cross_00004614$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Cross_00004614$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Cross_00004614$PostfixBurstDelegate>(new Bindings.Vec3Functions.Cross_00004614$PostfixBurstDelegate(Bindings.Vec3Functions.Cross)).Value;
				}
				A_0 = Bindings.Vec3Functions.Cross_00004614$BurstDirectCall.Pointer;
			}

			// Token: 0x0600466F RID: 18031 RVA: 0x00175D7C File Offset: 0x00173F7C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Cross_00004614$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004670 RID: 18032 RVA: 0x00175D94 File Offset: 0x00173F94
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Cross_00004614$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Cross$BurstManaged(L);
			}

			// Token: 0x040057EF RID: 22511
			private static IntPtr Pointer;
		}

		// Token: 0x02000B11 RID: 2833
		// (Invoke) Token: 0x06004672 RID: 18034
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Project_00004615$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B12 RID: 2834
		internal static class Project_00004615$BurstDirectCall
		{
			// Token: 0x06004675 RID: 18037 RVA: 0x00175DC8 File Offset: 0x00173FC8
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Project_00004615$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Project_00004615$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Project_00004615$PostfixBurstDelegate>(new Bindings.Vec3Functions.Project_00004615$PostfixBurstDelegate(Bindings.Vec3Functions.Project)).Value;
				}
				A_0 = Bindings.Vec3Functions.Project_00004615$BurstDirectCall.Pointer;
			}

			// Token: 0x06004676 RID: 18038 RVA: 0x00175E08 File Offset: 0x00174008
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Project_00004615$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004677 RID: 18039 RVA: 0x00175E20 File Offset: 0x00174020
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Project_00004615$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Project$BurstManaged(L);
			}

			// Token: 0x040057F0 RID: 22512
			private static IntPtr Pointer;
		}

		// Token: 0x02000B13 RID: 2835
		// (Invoke) Token: 0x06004679 RID: 18041
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Length_00004616$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B14 RID: 2836
		internal static class Length_00004616$BurstDirectCall
		{
			// Token: 0x0600467C RID: 18044 RVA: 0x00175E54 File Offset: 0x00174054
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Length_00004616$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Length_00004616$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Length_00004616$PostfixBurstDelegate>(new Bindings.Vec3Functions.Length_00004616$PostfixBurstDelegate(Bindings.Vec3Functions.Length)).Value;
				}
				A_0 = Bindings.Vec3Functions.Length_00004616$BurstDirectCall.Pointer;
			}

			// Token: 0x0600467D RID: 18045 RVA: 0x00175E94 File Offset: 0x00174094
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Length_00004616$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600467E RID: 18046 RVA: 0x00175EAC File Offset: 0x001740AC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Length_00004616$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Length$BurstManaged(L);
			}

			// Token: 0x040057F1 RID: 22513
			private static IntPtr Pointer;
		}

		// Token: 0x02000B15 RID: 2837
		// (Invoke) Token: 0x06004680 RID: 18048
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Normalize_00004617$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B16 RID: 2838
		internal static class Normalize_00004617$BurstDirectCall
		{
			// Token: 0x06004683 RID: 18051 RVA: 0x00175EE0 File Offset: 0x001740E0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Normalize_00004617$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Normalize_00004617$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Normalize_00004617$PostfixBurstDelegate>(new Bindings.Vec3Functions.Normalize_00004617$PostfixBurstDelegate(Bindings.Vec3Functions.Normalize)).Value;
				}
				A_0 = Bindings.Vec3Functions.Normalize_00004617$BurstDirectCall.Pointer;
			}

			// Token: 0x06004684 RID: 18052 RVA: 0x00175F20 File Offset: 0x00174120
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Normalize_00004617$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004685 RID: 18053 RVA: 0x00175F38 File Offset: 0x00174138
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Normalize_00004617$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Normalize$BurstManaged(L);
			}

			// Token: 0x040057F2 RID: 22514
			private static IntPtr Pointer;
		}

		// Token: 0x02000B17 RID: 2839
		// (Invoke) Token: 0x06004687 RID: 18055
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int SafeNormal_00004618$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B18 RID: 2840
		internal static class SafeNormal_00004618$BurstDirectCall
		{
			// Token: 0x0600468A RID: 18058 RVA: 0x00175F6C File Offset: 0x0017416C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.SafeNormal_00004618$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.SafeNormal_00004618$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.SafeNormal_00004618$PostfixBurstDelegate>(new Bindings.Vec3Functions.SafeNormal_00004618$PostfixBurstDelegate(Bindings.Vec3Functions.SafeNormal)).Value;
				}
				A_0 = Bindings.Vec3Functions.SafeNormal_00004618$BurstDirectCall.Pointer;
			}

			// Token: 0x0600468B RID: 18059 RVA: 0x00175FAC File Offset: 0x001741AC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.SafeNormal_00004618$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600468C RID: 18060 RVA: 0x00175FC4 File Offset: 0x001741C4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.SafeNormal_00004618$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.SafeNormal$BurstManaged(L);
			}

			// Token: 0x040057F3 RID: 22515
			private static IntPtr Pointer;
		}

		// Token: 0x02000B19 RID: 2841
		// (Invoke) Token: 0x0600468E RID: 18062
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Distance_00004619$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B1A RID: 2842
		internal static class Distance_00004619$BurstDirectCall
		{
			// Token: 0x06004691 RID: 18065 RVA: 0x00175FF8 File Offset: 0x001741F8
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Distance_00004619$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Distance_00004619$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Distance_00004619$PostfixBurstDelegate>(new Bindings.Vec3Functions.Distance_00004619$PostfixBurstDelegate(Bindings.Vec3Functions.Distance)).Value;
				}
				A_0 = Bindings.Vec3Functions.Distance_00004619$BurstDirectCall.Pointer;
			}

			// Token: 0x06004692 RID: 18066 RVA: 0x00176038 File Offset: 0x00174238
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Distance_00004619$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06004693 RID: 18067 RVA: 0x00176050 File Offset: 0x00174250
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Distance_00004619$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Distance$BurstManaged(L);
			}

			// Token: 0x040057F4 RID: 22516
			private static IntPtr Pointer;
		}

		// Token: 0x02000B1B RID: 2843
		// (Invoke) Token: 0x06004695 RID: 18069
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Lerp_0000461A$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B1C RID: 2844
		internal static class Lerp_0000461A$BurstDirectCall
		{
			// Token: 0x06004698 RID: 18072 RVA: 0x00176084 File Offset: 0x00174284
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Lerp_0000461A$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Lerp_0000461A$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Lerp_0000461A$PostfixBurstDelegate>(new Bindings.Vec3Functions.Lerp_0000461A$PostfixBurstDelegate(Bindings.Vec3Functions.Lerp)).Value;
				}
				A_0 = Bindings.Vec3Functions.Lerp_0000461A$BurstDirectCall.Pointer;
			}

			// Token: 0x06004699 RID: 18073 RVA: 0x001760C4 File Offset: 0x001742C4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Lerp_0000461A$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600469A RID: 18074 RVA: 0x001760DC File Offset: 0x001742DC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Lerp_0000461A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Lerp$BurstManaged(L);
			}

			// Token: 0x040057F5 RID: 22517
			private static IntPtr Pointer;
		}

		// Token: 0x02000B1D RID: 2845
		// (Invoke) Token: 0x0600469C RID: 18076
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Rotate_0000461B$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B1E RID: 2846
		internal static class Rotate_0000461B$BurstDirectCall
		{
			// Token: 0x0600469F RID: 18079 RVA: 0x00176110 File Offset: 0x00174310
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Rotate_0000461B$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Rotate_0000461B$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Rotate_0000461B$PostfixBurstDelegate>(new Bindings.Vec3Functions.Rotate_0000461B$PostfixBurstDelegate(Bindings.Vec3Functions.Rotate)).Value;
				}
				A_0 = Bindings.Vec3Functions.Rotate_0000461B$BurstDirectCall.Pointer;
			}

			// Token: 0x060046A0 RID: 18080 RVA: 0x00176150 File Offset: 0x00174350
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Rotate_0000461B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046A1 RID: 18081 RVA: 0x00176168 File Offset: 0x00174368
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Rotate_0000461B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Rotate$BurstManaged(L);
			}

			// Token: 0x040057F6 RID: 22518
			private static IntPtr Pointer;
		}

		// Token: 0x02000B1F RID: 2847
		// (Invoke) Token: 0x060046A3 RID: 18083
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int ZeroVector_0000461C$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B20 RID: 2848
		internal static class ZeroVector_0000461C$BurstDirectCall
		{
			// Token: 0x060046A6 RID: 18086 RVA: 0x0017619C File Offset: 0x0017439C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.ZeroVector_0000461C$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.ZeroVector_0000461C$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.ZeroVector_0000461C$PostfixBurstDelegate>(new Bindings.Vec3Functions.ZeroVector_0000461C$PostfixBurstDelegate(Bindings.Vec3Functions.ZeroVector)).Value;
				}
				A_0 = Bindings.Vec3Functions.ZeroVector_0000461C$BurstDirectCall.Pointer;
			}

			// Token: 0x060046A7 RID: 18087 RVA: 0x001761DC File Offset: 0x001743DC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.ZeroVector_0000461C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046A8 RID: 18088 RVA: 0x001761F4 File Offset: 0x001743F4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.ZeroVector_0000461C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.ZeroVector$BurstManaged(L);
			}

			// Token: 0x040057F7 RID: 22519
			private static IntPtr Pointer;
		}

		// Token: 0x02000B21 RID: 2849
		// (Invoke) Token: 0x060046AA RID: 18090
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int OneVector_0000461D$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B22 RID: 2850
		internal static class OneVector_0000461D$BurstDirectCall
		{
			// Token: 0x060046AD RID: 18093 RVA: 0x00176228 File Offset: 0x00174428
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.OneVector_0000461D$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.OneVector_0000461D$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.OneVector_0000461D$PostfixBurstDelegate>(new Bindings.Vec3Functions.OneVector_0000461D$PostfixBurstDelegate(Bindings.Vec3Functions.OneVector)).Value;
				}
				A_0 = Bindings.Vec3Functions.OneVector_0000461D$BurstDirectCall.Pointer;
			}

			// Token: 0x060046AE RID: 18094 RVA: 0x00176268 File Offset: 0x00174468
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.OneVector_0000461D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046AF RID: 18095 RVA: 0x00176280 File Offset: 0x00174480
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.OneVector_0000461D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.OneVector$BurstManaged(L);
			}

			// Token: 0x040057F8 RID: 22520
			private static IntPtr Pointer;
		}

		// Token: 0x02000B23 RID: 2851
		// (Invoke) Token: 0x060046B1 RID: 18097
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int NearlyEqual_0000461E$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B24 RID: 2852
		internal static class NearlyEqual_0000461E$BurstDirectCall
		{
			// Token: 0x060046B4 RID: 18100 RVA: 0x001762B4 File Offset: 0x001744B4
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.NearlyEqual_0000461E$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.NearlyEqual_0000461E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.NearlyEqual_0000461E$PostfixBurstDelegate>(new Bindings.Vec3Functions.NearlyEqual_0000461E$PostfixBurstDelegate(Bindings.Vec3Functions.NearlyEqual)).Value;
				}
				A_0 = Bindings.Vec3Functions.NearlyEqual_0000461E$BurstDirectCall.Pointer;
			}

			// Token: 0x060046B5 RID: 18101 RVA: 0x001762F4 File Offset: 0x001744F4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.NearlyEqual_0000461E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046B6 RID: 18102 RVA: 0x0017630C File Offset: 0x0017450C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.NearlyEqual_0000461E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.NearlyEqual$BurstManaged(L);
			}

			// Token: 0x040057F9 RID: 22521
			private static IntPtr Pointer;
		}
	}

	// Token: 0x02000B25 RID: 2853
	[BurstCompile]
	public static class QuatFunctions
	{
		// Token: 0x060046B7 RID: 18103 RVA: 0x0017633D File Offset: 0x0017453D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.QuatFunctions.New_0000461F$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060046B8 RID: 18104 RVA: 0x00176345 File Offset: 0x00174545
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.QuatFunctions.Mul_00004620$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060046B9 RID: 18105 RVA: 0x0017634D File Offset: 0x0017454D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.QuatFunctions.Eq_00004621$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060046BA RID: 18106 RVA: 0x00176358 File Offset: 0x00174558
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Luau.lua_pushstring(L, quaternion.ToString());
			return 1;
		}

		// Token: 0x060046BB RID: 18107 RVA: 0x00176391 File Offset: 0x00174591
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromEuler(lua_State* L)
		{
			return Bindings.QuatFunctions.FromEuler_00004623$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060046BC RID: 18108 RVA: 0x00176399 File Offset: 0x00174599
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromDirection(lua_State* L)
		{
			return Bindings.QuatFunctions.FromDirection_00004624$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060046BD RID: 18109 RVA: 0x001763A1 File Offset: 0x001745A1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetUpVector(lua_State* L)
		{
			return Bindings.QuatFunctions.GetUpVector_00004625$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060046BE RID: 18110 RVA: 0x001763A9 File Offset: 0x001745A9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Euler(lua_State* L)
		{
			return Bindings.QuatFunctions.Euler_00004626$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060046BF RID: 18111 RVA: 0x001763B4 File Offset: 0x001745B4
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int New$BurstManaged(lua_State* L)
		{
			Quaternion* ptr = Luau.lua_class_push<Quaternion>(L, "Quat");
			ptr.x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr.y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr.z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			ptr.w = (float)Luau.luaL_optnumber(L, 4, 0.0);
			return 1;
		}

		// Token: 0x060046C0 RID: 18112 RVA: 0x00176430 File Offset: 0x00174630
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion quaternion2 = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Quaternion>(L, "Quat") = quaternion * quaternion2;
			return 1;
		}

		// Token: 0x060046C1 RID: 18113 RVA: 0x00176488 File Offset: 0x00174688
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion quaternion2 = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			int num = (quaternion == quaternion2) ? 1 : 0;
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x060046C2 RID: 18114 RVA: 0x001764D8 File Offset: 0x001746D8
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int FromEuler$BurstManaged(lua_State* L)
		{
			float num = (float)Luau.luaL_optnumber(L, 1, 0.0);
			float num2 = (float)Luau.luaL_optnumber(L, 2, 0.0);
			float num3 = (float)Luau.luaL_optnumber(L, 3, 0.0);
			Luau.lua_class_push<Quaternion>(L, "Quat").eulerAngles = new Vector3(num, num2, num3);
			return 1;
		}

		// Token: 0x060046C3 RID: 18115 RVA: 0x0017653C File Offset: 0x0017473C
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int FromDirection$BurstManaged(lua_State* L)
		{
			Vector3 lookRotation = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_class_push<Quaternion>(L, "Quat").SetLookRotation(lookRotation);
			return 1;
		}

		// Token: 0x060046C4 RID: 18116 RVA: 0x00176578 File Offset: 0x00174778
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int GetUpVector$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion * Vector3.up;
			return 1;
		}

		// Token: 0x060046C5 RID: 18117 RVA: 0x001765C0 File Offset: 0x001747C0
		[BurstCompile]
		[MethodImpl(256)]
		internal unsafe static int Euler$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion.eulerAngles;
			return 1;
		}

		// Token: 0x02000B26 RID: 2854
		// (Invoke) Token: 0x060046C7 RID: 18119
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int New_0000461F$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B27 RID: 2855
		internal static class New_0000461F$BurstDirectCall
		{
			// Token: 0x060046CA RID: 18122 RVA: 0x00176604 File Offset: 0x00174804
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.New_0000461F$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.New_0000461F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.New_0000461F$PostfixBurstDelegate>(new Bindings.QuatFunctions.New_0000461F$PostfixBurstDelegate(Bindings.QuatFunctions.New)).Value;
				}
				A_0 = Bindings.QuatFunctions.New_0000461F$BurstDirectCall.Pointer;
			}

			// Token: 0x060046CB RID: 18123 RVA: 0x00176644 File Offset: 0x00174844
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.New_0000461F$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046CC RID: 18124 RVA: 0x0017665C File Offset: 0x0017485C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.New_0000461F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.New$BurstManaged(L);
			}

			// Token: 0x040057FA RID: 22522
			private static IntPtr Pointer;
		}

		// Token: 0x02000B28 RID: 2856
		// (Invoke) Token: 0x060046CE RID: 18126
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Mul_00004620$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B29 RID: 2857
		internal static class Mul_00004620$BurstDirectCall
		{
			// Token: 0x060046D1 RID: 18129 RVA: 0x00176690 File Offset: 0x00174890
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Mul_00004620$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Mul_00004620$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Mul_00004620$PostfixBurstDelegate>(new Bindings.QuatFunctions.Mul_00004620$PostfixBurstDelegate(Bindings.QuatFunctions.Mul)).Value;
				}
				A_0 = Bindings.QuatFunctions.Mul_00004620$BurstDirectCall.Pointer;
			}

			// Token: 0x060046D2 RID: 18130 RVA: 0x001766D0 File Offset: 0x001748D0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Mul_00004620$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046D3 RID: 18131 RVA: 0x001766E8 File Offset: 0x001748E8
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Mul_00004620$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Mul$BurstManaged(L);
			}

			// Token: 0x040057FB RID: 22523
			private static IntPtr Pointer;
		}

		// Token: 0x02000B2A RID: 2858
		// (Invoke) Token: 0x060046D5 RID: 18133
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Eq_00004621$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B2B RID: 2859
		internal static class Eq_00004621$BurstDirectCall
		{
			// Token: 0x060046D8 RID: 18136 RVA: 0x0017671C File Offset: 0x0017491C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Eq_00004621$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Eq_00004621$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Eq_00004621$PostfixBurstDelegate>(new Bindings.QuatFunctions.Eq_00004621$PostfixBurstDelegate(Bindings.QuatFunctions.Eq)).Value;
				}
				A_0 = Bindings.QuatFunctions.Eq_00004621$BurstDirectCall.Pointer;
			}

			// Token: 0x060046D9 RID: 18137 RVA: 0x0017675C File Offset: 0x0017495C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Eq_00004621$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046DA RID: 18138 RVA: 0x00176774 File Offset: 0x00174974
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Eq_00004621$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Eq$BurstManaged(L);
			}

			// Token: 0x040057FC RID: 22524
			private static IntPtr Pointer;
		}

		// Token: 0x02000B2C RID: 2860
		// (Invoke) Token: 0x060046DC RID: 18140
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int FromEuler_00004623$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B2D RID: 2861
		internal static class FromEuler_00004623$BurstDirectCall
		{
			// Token: 0x060046DF RID: 18143 RVA: 0x001767A8 File Offset: 0x001749A8
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromEuler_00004623$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromEuler_00004623$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.FromEuler_00004623$PostfixBurstDelegate>(new Bindings.QuatFunctions.FromEuler_00004623$PostfixBurstDelegate(Bindings.QuatFunctions.FromEuler)).Value;
				}
				A_0 = Bindings.QuatFunctions.FromEuler_00004623$BurstDirectCall.Pointer;
			}

			// Token: 0x060046E0 RID: 18144 RVA: 0x001767E8 File Offset: 0x001749E8
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.FromEuler_00004623$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046E1 RID: 18145 RVA: 0x00176800 File Offset: 0x00174A00
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromEuler_00004623$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromEuler$BurstManaged(L);
			}

			// Token: 0x040057FD RID: 22525
			private static IntPtr Pointer;
		}

		// Token: 0x02000B2E RID: 2862
		// (Invoke) Token: 0x060046E3 RID: 18147
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int FromDirection_00004624$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B2F RID: 2863
		internal static class FromDirection_00004624$BurstDirectCall
		{
			// Token: 0x060046E6 RID: 18150 RVA: 0x00176834 File Offset: 0x00174A34
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromDirection_00004624$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromDirection_00004624$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.FromDirection_00004624$PostfixBurstDelegate>(new Bindings.QuatFunctions.FromDirection_00004624$PostfixBurstDelegate(Bindings.QuatFunctions.FromDirection)).Value;
				}
				A_0 = Bindings.QuatFunctions.FromDirection_00004624$BurstDirectCall.Pointer;
			}

			// Token: 0x060046E7 RID: 18151 RVA: 0x00176874 File Offset: 0x00174A74
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.FromDirection_00004624$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046E8 RID: 18152 RVA: 0x0017688C File Offset: 0x00174A8C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromDirection_00004624$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromDirection$BurstManaged(L);
			}

			// Token: 0x040057FE RID: 22526
			private static IntPtr Pointer;
		}

		// Token: 0x02000B30 RID: 2864
		// (Invoke) Token: 0x060046EA RID: 18154
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int GetUpVector_00004625$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B31 RID: 2865
		internal static class GetUpVector_00004625$BurstDirectCall
		{
			// Token: 0x060046ED RID: 18157 RVA: 0x001768C0 File Offset: 0x00174AC0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.GetUpVector_00004625$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.GetUpVector_00004625$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.GetUpVector_00004625$PostfixBurstDelegate>(new Bindings.QuatFunctions.GetUpVector_00004625$PostfixBurstDelegate(Bindings.QuatFunctions.GetUpVector)).Value;
				}
				A_0 = Bindings.QuatFunctions.GetUpVector_00004625$BurstDirectCall.Pointer;
			}

			// Token: 0x060046EE RID: 18158 RVA: 0x00176900 File Offset: 0x00174B00
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.GetUpVector_00004625$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046EF RID: 18159 RVA: 0x00176918 File Offset: 0x00174B18
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.GetUpVector_00004625$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.GetUpVector$BurstManaged(L);
			}

			// Token: 0x040057FF RID: 22527
			private static IntPtr Pointer;
		}

		// Token: 0x02000B32 RID: 2866
		// (Invoke) Token: 0x060046F1 RID: 18161
		[UnmanagedFunctionPointer(2)]
		internal unsafe delegate int Euler_00004626$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000B33 RID: 2867
		internal static class Euler_00004626$BurstDirectCall
		{
			// Token: 0x060046F4 RID: 18164 RVA: 0x0017694C File Offset: 0x00174B4C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Euler_00004626$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Euler_00004626$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Euler_00004626$PostfixBurstDelegate>(new Bindings.QuatFunctions.Euler_00004626$PostfixBurstDelegate(Bindings.QuatFunctions.Euler)).Value;
				}
				A_0 = Bindings.QuatFunctions.Euler_00004626$BurstDirectCall.Pointer;
			}

			// Token: 0x060046F5 RID: 18165 RVA: 0x0017698C File Offset: 0x00174B8C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Euler_00004626$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x060046F6 RID: 18166 RVA: 0x001769A4 File Offset: 0x00174BA4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Euler_00004626$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Euler$BurstManaged(L);
			}

			// Token: 0x04005800 RID: 22528
			private static IntPtr Pointer;
		}
	}

	// Token: 0x02000B34 RID: 2868
	public struct GorillaLocomotionSettings
	{
		// Token: 0x04005801 RID: 22529
		public float velocityLimit;

		// Token: 0x04005802 RID: 22530
		public float slideVelocityLimit;

		// Token: 0x04005803 RID: 22531
		public float maxJumpSpeed;

		// Token: 0x04005804 RID: 22532
		public float jumpMultiplier;
	}

	// Token: 0x02000B35 RID: 2869
	[BurstCompile]
	public struct PlayerInput
	{
		// Token: 0x04005805 RID: 22533
		public float leftXAxis;

		// Token: 0x04005806 RID: 22534
		[MarshalAs(4)]
		public bool leftPrimaryButton;

		// Token: 0x04005807 RID: 22535
		public float rightXAxis;

		// Token: 0x04005808 RID: 22536
		[MarshalAs(4)]
		public bool rightPrimaryButton;

		// Token: 0x04005809 RID: 22537
		public float leftYAxis;

		// Token: 0x0400580A RID: 22538
		[MarshalAs(4)]
		public bool leftSecondaryButton;

		// Token: 0x0400580B RID: 22539
		public float rightYAxis;

		// Token: 0x0400580C RID: 22540
		[MarshalAs(4)]
		public bool rightSecondaryButton;

		// Token: 0x0400580D RID: 22541
		public float leftTrigger;

		// Token: 0x0400580E RID: 22542
		public float rightTrigger;

		// Token: 0x0400580F RID: 22543
		public float leftGrip;

		// Token: 0x04005810 RID: 22544
		public float rightGrip;
	}

	// Token: 0x02000B36 RID: 2870
	public static class JSON
	{
		// Token: 0x060046F7 RID: 18167 RVA: 0x001769D8 File Offset: 0x00174BD8
		public unsafe static Dictionary<object, object> ConsumeTable(lua_State* L, int tableIndex)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			Luau.lua_pushnil(L);
			if (tableIndex < 0)
			{
				tableIndex--;
			}
			while (Luau.lua_next(L, tableIndex) != 0)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				Luau.lua_Types lua_Types2 = (Luau.lua_Types)Luau.lua_type(L, -2);
				object obj;
				if (lua_Types2 == Luau.lua_Types.LUA_TSTRING)
				{
					obj = new string(Luau.lua_tostring(L, -2));
				}
				else
				{
					if (lua_Types2 != Luau.lua_Types.LUA_TNUMBER)
					{
						FixedString64Bytes fixedString64Bytes = "Invalid key in table, key must be a string or a number";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return null;
					}
					obj = Luau.lua_tonumber(L, -2);
				}
				switch (lua_Types)
				{
				case Luau.lua_Types.LUA_TBOOLEAN:
					dictionary.Add(obj, Luau.lua_toboolean(L, -1) == 1);
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TNUMBER:
					dictionary.Add(obj, Luau.luaL_checknumber(L, -1));
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TSTRING:
					dictionary.Add(obj, new string(Luau.lua_tostring(L, -1)));
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TTABLE:
				case Luau.lua_Types.LUA_TUSERDATA:
					if (Luau.luaL_getmetafield(L, -1, "metahash") == 1)
					{
						BurstClassInfo.ClassInfo classInfo;
						if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), ref classInfo))
						{
							FixedString64Bytes fixedString64Bytes2 = "\"Internal Class Info Error No Metatable Found\"";
							Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes2) + 2));
							return null;
						}
						Luau.lua_pop(L, 1);
						FixedString32Bytes fixedString32Bytes = "Vec3";
						if (ref classInfo.Name == ref fixedString32Bytes)
						{
							dictionary.Add(obj, *Luau.lua_class_get<Vector3>(L, -1));
							Luau.lua_pop(L, 1);
							continue;
						}
						fixedString32Bytes = "Quat";
						if (ref classInfo.Name == ref fixedString32Bytes)
						{
							dictionary.Add(obj, *Luau.lua_class_get<Quaternion>(L, -1));
							Luau.lua_pop(L, 1);
							continue;
						}
						FixedString32Bytes fixedString32Bytes2 = "Invalid type in table";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
						return null;
					}
					else
					{
						object obj2 = Bindings.JSON.ConsumeTable(L, -1);
						Luau.lua_pop(L, 1);
						if (obj2 != null)
						{
							dictionary.Add(obj, obj2);
							continue;
						}
						return null;
					}
					break;
				}
				FixedString32Bytes fixedString32Bytes3 = "Unknown type in table";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return null;
			}
			return dictionary;
		}

		// Token: 0x060046F8 RID: 18168 RVA: 0x00176C20 File Offset: 0x00174E20
		private static int ParseStrictInt(string input)
		{
			if (string.IsNullOrEmpty(input) || input != input.Trim())
			{
				return -1;
			}
			int result;
			if (!int.TryParse(input, ref result))
			{
				return -1;
			}
			return result;
		}

		// Token: 0x060046F9 RID: 18169 RVA: 0x00176C54 File Offset: 0x00174E54
		private static bool CompareKeys(JObject obj, HashSet<string> set)
		{
			HashSet<string> equals = new HashSet<string>(Enumerable.Select<JProperty, string>(obj.Properties(), (JProperty p) => p.Name));
			return set.SetEquals(equals);
		}

		// Token: 0x060046FA RID: 18170 RVA: 0x00176C98 File Offset: 0x00174E98
		public unsafe static bool PushTable(lua_State* L, JObject table)
		{
			Luau.lua_createtable(L, 0, 0);
			foreach (KeyValuePair<string, JToken> keyValuePair in table)
			{
				if (keyValuePair.Key != null && keyValuePair.Value != null)
				{
					int num = Bindings.JSON.ParseStrictInt(keyValuePair.Key);
					if (num == -1)
					{
						Luau.lua_pushstring(L, keyValuePair.Key);
					}
					if (keyValuePair.Value is JObject)
					{
						JObject obj = (JObject)keyValuePair.Value;
						HashSet<string> hashSet = new HashSet<string>();
						hashSet.Add("x");
						hashSet.Add("y");
						hashSet.Add("z");
						if (Bindings.JSON.CompareKeys(obj, hashSet))
						{
							JObject jobject = keyValuePair.Value as JObject;
							float num2 = jobject["x"].ToObject<float>();
							float num3 = jobject["y"].ToObject<float>();
							float num4 = jobject["z"].ToObject<float>();
							Vector3 vector;
							vector..ctor(num2, num3, num4);
							*Luau.lua_class_push<Vector3>(L) = vector;
						}
						else
						{
							JObject obj2 = (JObject)keyValuePair.Value;
							HashSet<string> hashSet2 = new HashSet<string>();
							hashSet2.Add("x");
							hashSet2.Add("y");
							hashSet2.Add("z");
							hashSet2.Add("w");
							if (Bindings.JSON.CompareKeys(obj2, hashSet2))
							{
								JObject jobject2 = keyValuePair.Value as JObject;
								float num5 = jobject2["x"].ToObject<float>();
								float num6 = jobject2["y"].ToObject<float>();
								float num7 = jobject2["z"].ToObject<float>();
								float num8 = jobject2["w"].ToObject<float>();
								Quaternion quaternion;
								quaternion..ctor(num5, num6, num7, num8);
								*Luau.lua_class_push<Quaternion>(L) = quaternion;
							}
							else
							{
								Bindings.JSON.PushTable(L, (JObject)keyValuePair.Value);
							}
						}
					}
					else if (keyValuePair.Value is JValue)
					{
						JTokenType type = keyValuePair.Value.Type;
						if (type == 6)
						{
							Luau.lua_pushnumber(L, (double)keyValuePair.Value.ToObject<int>());
						}
						else if (type == 9)
						{
							Luau.lua_pushboolean(L, keyValuePair.Value.ToObject<bool>() ? 1 : 0);
						}
						else if (type == 7)
						{
							Luau.lua_pushnumber(L, keyValuePair.Value.ToObject<double>());
						}
						else
						{
							if (type != 8)
							{
								continue;
							}
							Luau.lua_pushstring(L, keyValuePair.Value.ToString());
						}
					}
					if (num == -1)
					{
						Luau.lua_rawset(L, -3);
					}
					else
					{
						Luau.lua_rawseti(L, -2, num);
					}
				}
			}
			return true;
		}

		// Token: 0x060046FB RID: 18171 RVA: 0x00176F48 File Offset: 0x00175148
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DataSave(lua_State* L)
		{
			int result;
			try
			{
				string text = JsonConvert.SerializeObject(Bindings.JSON.ConsumeTable(L, 1), 1);
				if (text.Length > 10000)
				{
					Luau.luaL_errorL(L, "Save exceeds 10000 bytes", Array.Empty<string>());
					result = 0;
				}
				else
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(Bindings.JSON.ModIODirectory, "saves", CustomMapLoader.LoadedMapModId.ToString()));
					if (!directoryInfo.Exists)
					{
						directoryInfo.Create();
					}
					File.WriteAllText(Path.Join(directoryInfo.FullName, "luau.json"), text);
					result = 0;
				}
			}
			catch
			{
				Luau.luaL_errorL(L, "Argument 2 must be a table", Array.Empty<string>());
				result = 0;
			}
			return result;
		}

		// Token: 0x060046FC RID: 18172 RVA: 0x00177014 File Offset: 0x00175214
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DataLoad(lua_State* L)
		{
			int result;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(Bindings.JSON.ModIODirectory, "saves", CustomMapLoader.LoadedMapModId.ToString()));
				if (!directoryInfo.Exists)
				{
					Luau.lua_createtable(L, 0, 0);
					result = 1;
				}
				else
				{
					FileInfo[] files = directoryInfo.GetFiles("luau.json");
					if (files.Length == 0)
					{
						Luau.lua_createtable(L, 0, 0);
						result = 1;
					}
					else
					{
						JObject table = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(files[0].FullName));
						if (Bindings.JSON.PushTable(L, table))
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
					}
				}
			}
			catch
			{
				Luau.luaL_errorL(L, "Error while loading data", Array.Empty<string>());
				result = 0;
			}
			return result;
		}

		// Token: 0x04005811 RID: 22545
		private static string ModIODirectory = Path.Join(Path.Join(Application.persistentDataPath, "mod.io", "06657"), "data");
	}

	// Token: 0x02000B38 RID: 2872
	[BurstCompile]
	public struct LuauRoomState
	{
		// Token: 0x04005814 RID: 22548
		[MarshalAs(4)]
		public bool IsQuest;

		// Token: 0x04005815 RID: 22549
		public float FPS;

		// Token: 0x04005816 RID: 22550
		[MarshalAs(4)]
		public bool IsPrivate;

		// Token: 0x04005817 RID: 22551
		public FixedString32Bytes RoomCode;
	}

	// Token: 0x02000B39 RID: 2873
	public static class PlayerUtils
	{
		// Token: 0x06004701 RID: 18177 RVA: 0x0017712C File Offset: 0x0017532C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int TeleportPlayer(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			bool keepVelocity = Luau.lua_toboolean(L, 2) == 1;
			if (GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				Vector3 position = instance.transform.position;
				Vector3 vector2 = instance.mainCamera.transform.position - position;
				Vector3 position2 = vector - vector2;
				instance.TeleportTo(position2, instance.transform.rotation, keepVelocity, false);
			}
			return 0;
		}

		// Token: 0x06004702 RID: 18178 RVA: 0x001771AC File Offset: 0x001753AC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVelocity(lua_State* L)
		{
			Vector3 velocity = *Luau.lua_class_get<Vector3>(L, 1);
			if (GTPlayer.hasInstance)
			{
				GTPlayer.Instance.SetVelocity(velocity);
			}
			return 0;
		}
	}

	// Token: 0x02000B3A RID: 2874
	public static class RayCastUtils
	{
		// Token: 0x06004703 RID: 18179 RVA: 0x001771DC File Offset: 0x001753DC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int RayCast(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			if (!Physics.Raycast(vector, vector2, ref Bindings.RayCastUtils.rayHit))
			{
				return 0;
			}
			Luau.lua_createtable(L, 0, 0);
			Luau.lua_pushstring(L, "distance");
			Luau.lua_pushnumber(L, (double)Bindings.RayCastUtils.rayHit.distance);
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "point");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Bindings.RayCastUtils.rayHit.point;
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "normal");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Bindings.RayCastUtils.rayHit.normal;
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "object");
			IntPtr ptr;
			if (Bindings.LuauGameObjectList.TryGetValue(Bindings.RayCastUtils.rayHit.transform.gameObject, ref ptr))
			{
				Luau.lua_class_push(L, "GameObject", ptr);
			}
			else
			{
				Luau.lua_pushnil(L);
			}
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "player");
			Collider collider = Bindings.RayCastUtils.rayHit.collider;
			VRRig vrrig = (collider != null) ? collider.GetComponentInParent<VRRig>() : null;
			if (vrrig != null)
			{
				NetPlayer creator = vrrig.creator;
				if (creator != null)
				{
					IntPtr ptr2;
					if (Bindings.LuauPlayerList.TryGetValue(creator.ActorNumber, ref ptr2))
					{
						Luau.lua_class_push(L, "Player", ptr2);
					}
					else
					{
						Luau.lua_pushnil(L);
					}
				}
				else
				{
					Luau.lua_pushnil(L);
				}
			}
			else
			{
				Luau.lua_pushnil(L);
			}
			Luau.lua_rawset(L, -3);
			return 1;
		}

		// Token: 0x04005818 RID: 22552
		public static RaycastHit rayHit;
	}

	// Token: 0x02000B3B RID: 2875
	public static class Components
	{
		// Token: 0x06004704 RID: 18180 RVA: 0x0017737F File Offset: 0x0017557F
		public unsafe static void Build(lua_State* L)
		{
			Bindings.Components.LuauParticleSystemBindings.Builder(L);
			Bindings.Components.LuauAudioSourceBindings.Builder(L);
			Bindings.Components.LuauLightBindings.Builder(L);
			Bindings.Components.LuauAnimatorBindings.Builder(L);
		}

		// Token: 0x04005819 RID: 22553
		public static Dictionary<IntPtr, object> ComponentList = new Dictionary<IntPtr, object>();

		// Token: 0x02000B3C RID: 2876
		public static class LuauParticleSystemBindings
		{
			// Token: 0x06004706 RID: 18182 RVA: 0x001773A8 File Offset: 0x001755A8
			public unsafe static void Builder(lua_State* L)
			{
				Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>("ParticleSystem").AddFunction("play", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.play)).AddFunction("stop", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.stop)).AddFunction("clear", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.clear)).Build(L, false));
			}

			// Token: 0x06004707 RID: 18183 RVA: 0x00177414 File Offset: 0x00175614
			public unsafe static ParticleSystem GetParticleSystem(lua_State* L)
			{
				Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem* ptr = Luau.lua_class_get<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)ptr), ref obj))
				{
					ParticleSystem particleSystem = obj as ParticleSystem;
					if (particleSystem != null)
					{
						return particleSystem;
					}
				}
				return null;
			}

			// Token: 0x06004708 RID: 18184 RVA: 0x0017744C File Offset: 0x0017564C
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int play(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Play();
				}
				return 0;
			}

			// Token: 0x06004709 RID: 18185 RVA: 0x00177470 File Offset: 0x00175670
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int stop(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Stop();
				}
				return 0;
			}

			// Token: 0x0600470A RID: 18186 RVA: 0x00177494 File Offset: 0x00175694
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int clear(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Clear();
				}
				return 0;
			}

			// Token: 0x02000B3D RID: 2877
			public struct LuauParticleSystem
			{
				// Token: 0x0400581A RID: 22554
				public int x;
			}
		}

		// Token: 0x02000B3E RID: 2878
		public static class LuauAudioSourceBindings
		{
			// Token: 0x0600470B RID: 18187 RVA: 0x001774B8 File Offset: 0x001756B8
			public unsafe static void Builder(lua_State* L)
			{
				Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>("AudioSource").AddFunction("play", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.play)).AddFunction("setVolume", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setVolume)).AddFunction("setLoop", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setLoop)).AddFunction("setPitch", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setPitch)).AddFunction("setMinDistance", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setMinDistance)).AddFunction("setMaxDistance", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setMaxDistance)).Build(L, false));
			}

			// Token: 0x0600470C RID: 18188 RVA: 0x00177568 File Offset: 0x00175768
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static AudioSource GetAudioSource(lua_State* L)
			{
				Bindings.Components.LuauAudioSourceBindings.LuauAudioSource* ptr = Luau.lua_class_get<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)ptr), ref obj))
				{
					AudioSource audioSource = obj as AudioSource;
					if (audioSource != null)
					{
						return audioSource;
					}
				}
				return null;
			}

			// Token: 0x0600470D RID: 18189 RVA: 0x001775A0 File Offset: 0x001757A0
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int play(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				if (audioSource != null)
				{
					audioSource.Play();
				}
				return 0;
			}

			// Token: 0x0600470E RID: 18190 RVA: 0x001775C4 File Offset: 0x001757C4
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setVolume(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.volume = (float)num;
				}
				return 0;
			}

			// Token: 0x0600470F RID: 18191 RVA: 0x001775F4 File Offset: 0x001757F4
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setLoop(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				bool loop = Luau.lua_toboolean(L, 2) == 1;
				if (audioSource != null)
				{
					audioSource.loop = loop;
				}
				return 0;
			}

			// Token: 0x06004710 RID: 18192 RVA: 0x00177624 File Offset: 0x00175824
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setPitch(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.pitch = (float)num;
				}
				return 0;
			}

			// Token: 0x06004711 RID: 18193 RVA: 0x00177654 File Offset: 0x00175854
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setMinDistance(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.minDistance = (float)num;
				}
				return 0;
			}

			// Token: 0x06004712 RID: 18194 RVA: 0x00177684 File Offset: 0x00175884
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setMaxDistance(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.maxDistance = (float)num;
				}
				return 0;
			}

			// Token: 0x02000B3F RID: 2879
			public struct LuauAudioSource
			{
				// Token: 0x0400581B RID: 22555
				public int x;
			}
		}

		// Token: 0x02000B40 RID: 2880
		public static class LuauLightBindings
		{
			// Token: 0x06004713 RID: 18195 RVA: 0x001776B4 File Offset: 0x001758B4
			public unsafe static void Builder(lua_State* L)
			{
				Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.Components.LuauLightBindings.LuauLight>("Light").AddFunction("setColor", new lua_CFunction(Bindings.Components.LuauLightBindings.setColor)).AddFunction("setIntensity", new lua_CFunction(Bindings.Components.LuauLightBindings.setIntensity)).AddFunction("setRange", new lua_CFunction(Bindings.Components.LuauLightBindings.setRange)).Build(L, false));
			}

			// Token: 0x06004714 RID: 18196 RVA: 0x00177720 File Offset: 0x00175920
			public unsafe static Light GetLight(lua_State* L)
			{
				Bindings.Components.LuauLightBindings.LuauLight* ptr = Luau.lua_class_get<Bindings.Components.LuauLightBindings.LuauLight>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)ptr), ref obj))
				{
					Light light = obj as Light;
					if (light != null)
					{
						return light;
					}
				}
				return null;
			}

			// Token: 0x06004715 RID: 18197 RVA: 0x00177758 File Offset: 0x00175958
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setColor(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2);
				if (light != null)
				{
					light.color = new Color(vector.x, vector.y, vector.z);
				}
				return 0;
			}

			// Token: 0x06004716 RID: 18198 RVA: 0x001777A0 File Offset: 0x001759A0
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setIntensity(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (light != null)
				{
					light.intensity = (float)num;
				}
				return 0;
			}

			// Token: 0x06004717 RID: 18199 RVA: 0x001777D0 File Offset: 0x001759D0
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setRange(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (light != null)
				{
					light.range = (float)num;
				}
				return 0;
			}

			// Token: 0x02000B41 RID: 2881
			public struct LuauLight
			{
				// Token: 0x0400581C RID: 22556
				public int x;
			}
		}

		// Token: 0x02000B42 RID: 2882
		public static class LuauAnimatorBindings
		{
			// Token: 0x06004718 RID: 18200 RVA: 0x00177800 File Offset: 0x00175A00
			public unsafe static void Builder(lua_State* L)
			{
				Enumerable.Append<object>(LuauVm.ClassBuilders, new LuauClassBuilder<Bindings.Components.LuauAnimatorBindings.LuauAnimator>("Animator").AddFunction("setSpeed", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.setSpeed)).AddFunction("startPlayback", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.startPlayback)).AddFunction("stopPlayback", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.stopPlayback)).AddFunction("reset", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.reset)).Build(L, false));
			}

			// Token: 0x06004719 RID: 18201 RVA: 0x00177884 File Offset: 0x00175A84
			public unsafe static Animator GetAnimator(lua_State* L)
			{
				Bindings.Components.LuauAnimatorBindings.LuauAnimator* ptr = Luau.lua_class_get<Bindings.Components.LuauAnimatorBindings.LuauAnimator>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)ptr), ref obj))
				{
					Animator animator = obj as Animator;
					if (animator != null)
					{
						return animator;
					}
				}
				return null;
			}

			// Token: 0x0600471A RID: 18202 RVA: 0x001778BC File Offset: 0x00175ABC
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setSpeed(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (animator != null)
				{
					animator.speed = (float)num;
				}
				return 0;
			}

			// Token: 0x0600471B RID: 18203 RVA: 0x001778EC File Offset: 0x00175AEC
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int startPlayback(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.StartPlayback();
				}
				return 0;
			}

			// Token: 0x0600471C RID: 18204 RVA: 0x00177910 File Offset: 0x00175B10
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int stopPlayback(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.StopPlayback();
				}
				return 0;
			}

			// Token: 0x0600471D RID: 18205 RVA: 0x00177934 File Offset: 0x00175B34
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int reset(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.ResetToEntryState();
				}
				return 0;
			}

			// Token: 0x02000B43 RID: 2883
			public struct LuauAnimator
			{
				// Token: 0x0400581D RID: 22557
				public int x;
			}
		}
	}
}
