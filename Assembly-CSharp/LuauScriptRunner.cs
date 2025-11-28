using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x02000B63 RID: 2915
public class LuauScriptRunner
{
	// Token: 0x060047C8 RID: 18376 RVA: 0x00179C64 File Offset: 0x00177E64
	public unsafe static bool ErrorCheck(lua_State* L, int status)
	{
		if (status != 0)
		{
			sbyte* ptr = Luau.lua_tostring(L, -1);
			LuauHud.Instance.LuauLog(new string(ptr));
			sbyte* ptr2 = (sbyte*)Luau.lua_debugtrace(L);
			LuauHud.Instance.LuauLog(new string(ptr2));
			LuauHud.Instance.LuauLog("Error code: " + status.ToString());
			Luau.lua_close(L);
			return true;
		}
		return false;
	}

	// Token: 0x060047C9 RID: 18377 RVA: 0x00179CC8 File Offset: 0x00177EC8
	public bool Tick(float deltaTime)
	{
		if (!this.ShouldTick)
		{
			return false;
		}
		this.preTickCallback(this.L);
		LuauVm.ProcessEvents();
		if (!this.ShouldTick)
		{
			return false;
		}
		Luau.lua_settop(this.L, 0);
		Luau.lua_getfield(this.L, -10002, "tick");
		if (Luau.lua_type(this.L, -1) == 7)
		{
			Luau.lua_pushnumber(this.L, (double)deltaTime);
			int status = Luau.lua_pcall(this.L, 1, 0, 0);
			this.ShouldTick = !LuauScriptRunner.ErrorCheck(this.L, status);
			if (this.ShouldTick)
			{
				this.postTickCallback(this.L);
				Luau.lua_settop(this.L, 0);
				int data = Luau.lua_gc(this.L, 3, 0);
				Luau.lua_gc(this.L, 6, data);
			}
			return this.ShouldTick;
		}
		Luau.lua_pop(this.L, 1);
		return false;
	}

	// Token: 0x060047CA RID: 18378 RVA: 0x00179DB8 File Offset: 0x00177FB8
	public unsafe LuauScriptRunner(string script, string name, [CanBeNull] lua_CFunction bindings = null, [CanBeNull] lua_CFunction preTick = null, [CanBeNull] lua_CFunction postTick = null)
	{
		this.Script = script;
		this.ScriptName = name;
		this.L = Luau.luaL_newstate();
		LuauScriptRunner.ScriptRunners.Add(this);
		Luau.luaL_openlibs(this.L);
		Bindings.Vec3Builder(this.L);
		Bindings.QuatBuilder(this.L);
		if (bindings != null)
		{
			bindings(this.L);
		}
		this.postTickCallback = postTick;
		this.preTickCallback = preTick;
		UIntPtr size = (UIntPtr)((IntPtr)0);
		Luau.lua_register(this.L, new lua_CFunction(Luau.lua_print), "print");
		byte[] bytes = Encoding.UTF8.GetBytes(script);
		sbyte* data = Luau.luau_compile(script, (UIntPtr)((IntPtr)bytes.Length), null, &size);
		Luau.luau_load(this.L, name, data, size, 0);
		int status = Luau.lua_resume(this.L, null, 0);
		this.ShouldTick = !LuauScriptRunner.ErrorCheck(this.L, status);
	}

	// Token: 0x060047CB RID: 18379 RVA: 0x00179E9F File Offset: 0x0017809F
	public LuauScriptRunner FromFile(string filePath, [CanBeNull] lua_CFunction bindings = null, [CanBeNull] lua_CFunction tick = null)
	{
		return new LuauScriptRunner(File.ReadAllText(Path.Join(Application.persistentDataPath, "Scripts", filePath)), filePath, bindings, tick, null);
	}

	// Token: 0x060047CC RID: 18380 RVA: 0x00179ED0 File Offset: 0x001780D0
	~LuauScriptRunner()
	{
		LuauVm.ClassBuilders.Clear();
		Bindings.LuauPlayerList.Clear();
		Bindings.LuauGameObjectList.Clear();
		Bindings.LuauGameObjectListReverse.Clear();
		Bindings.LuauGameObjectStates.Clear();
		Bindings.LuauVRRigList.Clear();
		Bindings.LuauAIAgentList.Clear();
		Bindings.Components.ComponentList.Clear();
		ReflectionMetaNames.ReflectedNames.Clear();
		if (BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			BurstClassInfo.ClassList.InfoFields.Data.Clear();
		}
	}

	// Token: 0x0400587E RID: 22654
	public static List<LuauScriptRunner> ScriptRunners = new List<LuauScriptRunner>();

	// Token: 0x0400587F RID: 22655
	public bool ShouldTick;

	// Token: 0x04005880 RID: 22656
	private lua_CFunction postTickCallback;

	// Token: 0x04005881 RID: 22657
	private lua_CFunction preTickCallback;

	// Token: 0x04005882 RID: 22658
	public string ScriptName;

	// Token: 0x04005883 RID: 22659
	public string Script;

	// Token: 0x04005884 RID: 22660
	public unsafe lua_State* L;
}
