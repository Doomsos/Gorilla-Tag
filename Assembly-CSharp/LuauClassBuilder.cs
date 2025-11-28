using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000B53 RID: 2899
public class LuauClassBuilder<[IsUnmanaged] T> where T : struct, ValueType
{
	// Token: 0x06004743 RID: 18243 RVA: 0x00178448 File Offset: 0x00176648
	public LuauClassBuilder(string className)
	{
		this._className = className;
		this._classType = typeof(T);
	}

	// Token: 0x06004744 RID: 18244 RVA: 0x001784C0 File Offset: 0x001766C0
	public LuauClassBuilder<T> AddField(string luaName, string fieldName = null)
	{
		if (fieldName == null)
		{
			fieldName = luaName;
		}
		FieldInfo field = typeof(T).GetField(fieldName, 20);
		if (field == null)
		{
			throw new ArgumentException(string.Concat(new string[]
			{
				"Property ",
				fieldName,
				" does not exist on type ",
				typeof(T).Name,
				"."
			}));
		}
		this._classFields.TryAdd(LuaHashing.ByteHash(luaName), field);
		return this;
	}

	// Token: 0x06004745 RID: 18245 RVA: 0x00178542 File Offset: 0x00176742
	public LuauClassBuilder<T> AddStaticFunction(string luaName, lua_CFunction function)
	{
		this._staticFunctions.TryAdd(luaName, function);
		return this;
	}

	// Token: 0x06004746 RID: 18246 RVA: 0x00178553 File Offset: 0x00176753
	public LuauClassBuilder<T> AddStaticFunction(string luaName, FunctionPointer<lua_CFunction> function)
	{
		this._staticFunctionPtrs.TryAdd(luaName, function);
		return this;
	}

	// Token: 0x06004747 RID: 18247 RVA: 0x00178564 File Offset: 0x00176764
	public LuauClassBuilder<T> AddProperty(string luaName, lua_CFunction function)
	{
		this._properties.TryAdd(luaName, function);
		return this;
	}

	// Token: 0x06004748 RID: 18248 RVA: 0x00178575 File Offset: 0x00176775
	public LuauClassBuilder<T> AddProperty(string luaName, FunctionPointer<lua_CFunction> function)
	{
		this._propertyPtrs.TryAdd(luaName, function);
		return this;
	}

	// Token: 0x06004749 RID: 18249 RVA: 0x00178586 File Offset: 0x00176786
	public LuauClassBuilder<T> AddFunction(string luaName, lua_CFunction function)
	{
		if (luaName.StartsWith("__"))
		{
			this._staticFunctions.TryAdd(luaName, function);
		}
		this._functions.TryAdd(LuaHashing.ByteHash(luaName), function);
		return this;
	}

	// Token: 0x0600474A RID: 18250 RVA: 0x001785B7 File Offset: 0x001767B7
	public LuauClassBuilder<T> AddFunction(string luaName, FunctionPointer<lua_CFunction> function)
	{
		if (luaName.StartsWith("__"))
		{
			this._staticFunctionPtrs.TryAdd(luaName, function);
		}
		this._functionPtrs.TryAdd(LuaHashing.ByteHash(luaName), function);
		return this;
	}

	// Token: 0x0600474B RID: 18251 RVA: 0x001785E8 File Offset: 0x001767E8
	public unsafe LuauClassBuilder<T> Build(lua_State* L, bool global)
	{
		BurstClassInfo.NewClass<T>(this._className, this._classFields, this._functions, this._functionPtrs);
		Luau.luaL_newmetatable(L, this._className);
		FunctionPointer<lua_CFunction> fn = BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(BurstClassInfo.Index));
		Luau.lua_pushcfunction(L, fn, null);
		Luau.lua_setfield(L, -2, "__index");
		FunctionPointer<lua_CFunction> fn2 = BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(BurstClassInfo.NameCall));
		Luau.lua_pushcfunction(L, fn2, null);
		Luau.lua_setfield(L, -2, "__namecall");
		FunctionPointer<lua_CFunction> fn3 = BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(BurstClassInfo.NewIndex));
		Luau.lua_pushcfunction(L, fn3, null);
		Luau.lua_setfield(L, -2, "__newindex");
		foreach (KeyValuePair<string, lua_CFunction> keyValuePair in this._staticFunctions)
		{
			Luau.lua_pushcfunction(L, keyValuePair.Value, keyValuePair.Key);
			Luau.lua_setfield(L, -2, keyValuePair.Key);
		}
		foreach (KeyValuePair<string, FunctionPointer<lua_CFunction>> keyValuePair2 in this._staticFunctionPtrs)
		{
			Luau.lua_pushcfunction(L, keyValuePair2.Value, keyValuePair2.Key);
			Luau.lua_setfield(L, -2, keyValuePair2.Key);
		}
		FixedString32Bytes fixedString32Bytes = "metahash";
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2;
		Luau.lua_pushnumber(L, (double)LuaHashing.ByteHash(this._className));
		Luau.lua_setfield(L, -2, k);
		Luau.lua_setreadonly(L, -1, 1);
		Luau.lua_pop(L, 1);
		if (global)
		{
			Luau.lua_createtable(L, 0, 0);
			foreach (KeyValuePair<string, lua_CFunction> keyValuePair3 in this._staticFunctions)
			{
				Luau.lua_pushcfunction(L, keyValuePair3.Value, keyValuePair3.Key);
				Luau.lua_setfield(L, -2, keyValuePair3.Key);
			}
			foreach (KeyValuePair<string, FunctionPointer<lua_CFunction>> keyValuePair4 in this._staticFunctionPtrs)
			{
				Luau.lua_pushcfunction(L, keyValuePair4.Value, keyValuePair4.Key);
				Luau.lua_setfield(L, -2, keyValuePair4.Key);
			}
			Luau.lua_pushnumber(L, (double)LuaHashing.ByteHash(this._className));
			Luau.lua_setfield(L, -2, k);
			Luau.luaL_getmetatable(L, this._className);
			Luau.lua_setmetatable(L, -2);
			Luau.lua_setglobal(L, this._className);
		}
		return this;
	}

	// Token: 0x04005839 RID: 22585
	private string _className;

	// Token: 0x0400583A RID: 22586
	private Type _classType;

	// Token: 0x0400583B RID: 22587
	private Dictionary<string, lua_CFunction> _staticFunctions = new Dictionary<string, lua_CFunction>();

	// Token: 0x0400583C RID: 22588
	private Dictionary<string, FunctionPointer<lua_CFunction>> _staticFunctionPtrs = new Dictionary<string, FunctionPointer<lua_CFunction>>();

	// Token: 0x0400583D RID: 22589
	private Dictionary<int, FieldInfo> _classFields = new Dictionary<int, FieldInfo>();

	// Token: 0x0400583E RID: 22590
	private Dictionary<string, lua_CFunction> _properties = new Dictionary<string, lua_CFunction>();

	// Token: 0x0400583F RID: 22591
	private Dictionary<string, FunctionPointer<lua_CFunction>> _propertyPtrs = new Dictionary<string, FunctionPointer<lua_CFunction>>();

	// Token: 0x04005840 RID: 22592
	private Dictionary<int, lua_CFunction> _functions = new Dictionary<int, lua_CFunction>();

	// Token: 0x04005841 RID: 22593
	private Dictionary<int, FunctionPointer<lua_CFunction>> _functionPtrs = new Dictionary<int, FunctionPointer<lua_CFunction>>();
}
