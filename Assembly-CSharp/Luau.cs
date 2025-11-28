using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000B58 RID: 2904
public class Luau
{
	// Token: 0x06004754 RID: 18260
	[DllImport("luau")]
	public unsafe static extern lua_State* luaL_newstate();

	// Token: 0x06004755 RID: 18261
	[DllImport("luau")]
	public unsafe static extern void luaL_openlibs(lua_State* L);

	// Token: 0x06004756 RID: 18262
	[DllImport("luau")]
	public unsafe static extern sbyte* luau_compile([MarshalAs(20)] string source, [NativeInteger] UIntPtr size, lua_CompileOptions* options, [NativeInteger] UIntPtr* outsize);

	// Token: 0x06004757 RID: 18263
	[DllImport("luau")]
	public unsafe static extern int luau_load(lua_State* L, [MarshalAs(20)] string chunkname, sbyte* data, [NativeInteger] UIntPtr size, int env);

	// Token: 0x06004758 RID: 18264
	[DllImport("luau")]
	public unsafe static extern void lua_pushvalue(lua_State* L, int idx);

	// Token: 0x06004759 RID: 18265
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, lua_CFunction fn, [MarshalAs(20)] string debugname, int nup, lua_Continuation cont);

	// Token: 0x0600475A RID: 18266
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, FunctionPointer<lua_CFunction> fn, [MarshalAs(20)] string debugname, int nup, lua_Continuation cont);

	// Token: 0x0600475B RID: 18267
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, FunctionPointer<lua_CFunction> fn, byte* debugname, int nup, int* cont);

	// Token: 0x0600475C RID: 18268 RVA: 0x001788C8 File Offset: 0x00176AC8
	public unsafe static void lua_pushcfunction(lua_State* L, FunctionPointer<lua_CFunction> fn, [MarshalAs(20)] string debugname)
	{
		Luau.lua_pushcclosurek(L, fn, debugname, 0, null);
	}

	// Token: 0x0600475D RID: 18269 RVA: 0x001788D4 File Offset: 0x00176AD4
	public unsafe static void lua_pushcfunction(lua_State* L, lua_CFunction fn, [MarshalAs(20)] string debugname)
	{
		Luau.lua_pushcclosurek(L, fn, debugname, 0, null);
	}

	// Token: 0x0600475E RID: 18270
	[DllImport("luau")]
	public unsafe static extern void lua_settop(lua_State* L, int idx);

	// Token: 0x0600475F RID: 18271
	[DllImport("luau")]
	public unsafe static extern int lua_gettop(lua_State* L);

	// Token: 0x06004760 RID: 18272
	[DllImport("luau")]
	public unsafe static extern sbyte* lua_tolstring(lua_State* L, int idx, int* len);

	// Token: 0x06004761 RID: 18273
	[DllImport("luau")]
	public unsafe static extern int lua_resume(lua_State* L, lua_State* from, int nargs);

	// Token: 0x06004762 RID: 18274
	[DllImport("luau")]
	public unsafe static extern void lua_setfield(lua_State* L, int index, [MarshalAs(20)] string k);

	// Token: 0x06004763 RID: 18275
	[DllImport("luau")]
	public unsafe static extern void lua_setfield(lua_State* L, int index, byte* k);

	// Token: 0x06004764 RID: 18276 RVA: 0x001788E0 File Offset: 0x00176AE0
	public unsafe static void lua_setglobal(lua_State* L, string s)
	{
		Luau.lua_setfield(L, -10002, s);
	}

	// Token: 0x06004765 RID: 18277 RVA: 0x001788F0 File Offset: 0x00176AF0
	public unsafe static void lua_register(lua_State* L, lua_CFunction f, string n)
	{
		lua_Continuation cont = null;
		Luau.lua_pushcclosurek(L, f, n, 0, cont);
		Luau.lua_setglobal(L, n);
	}

	// Token: 0x06004766 RID: 18278 RVA: 0x00178910 File Offset: 0x00176B10
	public unsafe static void lua_pop(lua_State* L, int n)
	{
		Luau.lua_settop(L, -n - 1);
	}

	// Token: 0x06004767 RID: 18279 RVA: 0x0017891C File Offset: 0x00176B1C
	public unsafe static sbyte* lua_tostring(lua_State* L, int idx)
	{
		return Luau.lua_tolstring(L, idx, null);
	}

	// Token: 0x06004768 RID: 18280
	[DllImport("luau")]
	public unsafe static extern int lua_isstring(lua_State* L, int index);

	// Token: 0x06004769 RID: 18281
	[DllImport("luau")]
	public unsafe static extern int lua_type(lua_State* L, int index);

	// Token: 0x0600476A RID: 18282
	[DllImport("luau")]
	public unsafe static extern int lua_pushstring(lua_State* L, [MarshalAs(20)] string s);

	// Token: 0x0600476B RID: 18283
	[DllImport("luau")]
	public unsafe static extern int lua_pushstring(lua_State* L, byte* s);

	// Token: 0x0600476C RID: 18284
	[DllImport("luau")]
	public unsafe static extern int lua_error(lua_State* L);

	// Token: 0x0600476D RID: 18285
	[DllImport("luau")]
	public unsafe static extern void luaL_errorL(lua_State* L, [MarshalAs(20)] string fmt, [MarshalAs(20)] params string[] a);

	// Token: 0x0600476E RID: 18286
	[DllImport("luau")]
	public unsafe static extern void luaL_errorL(lua_State* L, sbyte* fmt);

	// Token: 0x0600476F RID: 18287
	[DllImport("luau")]
	public unsafe static extern int lua_toboolean(lua_State* L, int index);

	// Token: 0x06004770 RID: 18288
	[DllImport("luau")]
	public unsafe static extern byte* lua_debugtrace(lua_State* L);

	// Token: 0x06004771 RID: 18289
	[DllImport("luau")]
	public unsafe static extern void lua_close(lua_State* L);

	// Token: 0x06004772 RID: 18290
	[DllImport("luau")]
	public unsafe static extern int lua_ref(lua_State* L, int idx);

	// Token: 0x06004773 RID: 18291
	[DllImport("luau")]
	public unsafe static extern void lua_unref(lua_State* L, int rid);

	// Token: 0x06004774 RID: 18292 RVA: 0x00178927 File Offset: 0x00176B27
	public unsafe static void lua_getref(lua_State* L, int rid)
	{
		Luau.lua_rawgeti(L, -10000, rid);
	}

	// Token: 0x06004775 RID: 18293
	[DllImport("luau")]
	public unsafe static extern void* lua_touserdatatagged(lua_State* L, int idx, int tag);

	// Token: 0x06004776 RID: 18294
	[DllImport("luau")]
	public unsafe static extern void* lua_touserdata(lua_State* L, int index);

	// Token: 0x06004777 RID: 18295
	[DllImport("luau")]
	public unsafe static extern void* lua_newuserdatatagged(lua_State* L, int sz, int tag);

	// Token: 0x06004778 RID: 18296
	[DllImport("luau")]
	public unsafe static extern void lua_getuserdatametatable(lua_State* L, int tag);

	// Token: 0x06004779 RID: 18297
	[DllImport("luau")]
	public unsafe static extern void lua_setuserdatametatable(lua_State* L, int tag, int idx);

	// Token: 0x0600477A RID: 18298
	[DllImport("luau")]
	public unsafe static extern int lua_setmetatable(lua_State* L, int objindex);

	// Token: 0x0600477B RID: 18299
	[DllImport("luau")]
	public unsafe static extern int luaL_newmetatable(lua_State* L, [MarshalAs(20)] string tname);

	// Token: 0x0600477C RID: 18300
	[DllImport("luau")]
	public unsafe static extern int lua_getfield(lua_State* L, int idx, [MarshalAs(20)] string k);

	// Token: 0x0600477D RID: 18301
	[DllImport("luau")]
	public unsafe static extern int lua_getfield(lua_State* L, int idx, byte* k);

	// Token: 0x0600477E RID: 18302
	[DllImport("luau")]
	public unsafe static extern int luaL_getmetafield(lua_State* L, int idx, byte* k);

	// Token: 0x0600477F RID: 18303
	[DllImport("luau")]
	public unsafe static extern int luaL_getmetafield(lua_State* L, int idx, [MarshalAs(20)] string k);

	// Token: 0x06004780 RID: 18304 RVA: 0x00178935 File Offset: 0x00176B35
	public unsafe static void luaL_getmetatable(lua_State* L, string n)
	{
		Luau.lua_getfield(L, -10000, n);
	}

	// Token: 0x06004781 RID: 18305 RVA: 0x00178944 File Offset: 0x00176B44
	public unsafe static void luaL_getmetatable(lua_State* L, byte* n)
	{
		Luau.lua_getfield(L, -10000, n);
	}

	// Token: 0x06004782 RID: 18306 RVA: 0x00178953 File Offset: 0x00176B53
	public unsafe static void lua_getglobal(lua_State* L, string n)
	{
		Luau.lua_getfield(L, -10002, n);
	}

	// Token: 0x06004783 RID: 18307
	[DllImport("luau")]
	public unsafe static extern int lua_getmetatable(lua_State* L, int objindex);

	// Token: 0x06004784 RID: 18308
	[DllImport("luau")]
	public unsafe static extern byte* lua_namecallatom(lua_State* L, int* atom);

	// Token: 0x06004785 RID: 18309
	[DllImport("luau")]
	public unsafe static extern byte* luaL_checklstring(lua_State* L, int numArg, int* l);

	// Token: 0x06004786 RID: 18310 RVA: 0x00178962 File Offset: 0x00176B62
	public unsafe static byte* luaL_checkstring(lua_State* L, int n)
	{
		return Luau.luaL_checklstring(L, n, null);
	}

	// Token: 0x06004787 RID: 18311
	[DllImport("luau")]
	public unsafe static extern void lua_pushnumber(lua_State* L, double n);

	// Token: 0x06004788 RID: 18312
	[DllImport("luau")]
	public unsafe static extern double luaL_checknumber(lua_State* L, int numArg);

	// Token: 0x06004789 RID: 18313
	[DllImport("luau")]
	public unsafe static extern void lua_setreadonly(lua_State* L, int idx, int enabled);

	// Token: 0x0600478A RID: 18314
	[DllImport("luau")]
	public unsafe static extern double lua_tonumberx(lua_State* L, int index, int* isnum);

	// Token: 0x0600478B RID: 18315
	[DllImport("luau")]
	public unsafe static extern int lua_gc(lua_State* L, int what, int data);

	// Token: 0x0600478C RID: 18316
	[DllImport("luau")]
	public unsafe static extern void lua_call(lua_State* L, int nargs, int nresults);

	// Token: 0x0600478D RID: 18317
	[DllImport("luau")]
	public unsafe static extern int lua_pcall(lua_State* L, int nargs, int nresults, int fn);

	// Token: 0x0600478E RID: 18318
	[DllImport("luau")]
	public unsafe static extern int lua_status(lua_State* L);

	// Token: 0x0600478F RID: 18319
	[DllImport("luau")]
	public unsafe static extern void* luaL_checkudata(lua_State* L, int arg, [MarshalAs(20)] string tname);

	// Token: 0x06004790 RID: 18320
	[DllImport("luau")]
	public unsafe static extern void* luaL_checkudata(lua_State* L, int arg, byte* tname);

	// Token: 0x06004791 RID: 18321
	[DllImport("luau")]
	public unsafe static extern int lua_objlen(lua_State* L, int index);

	// Token: 0x06004792 RID: 18322
	[DllImport("luau")]
	public unsafe static extern double luaL_optnumber(lua_State* L, int narg, double d);

	// Token: 0x06004793 RID: 18323
	[DllImport("luau")]
	public unsafe static extern void lua_createtable(lua_State* L, int narr, int nrec);

	// Token: 0x06004794 RID: 18324
	[DllImport("luau")]
	public unsafe static extern void lua_pushlightuserdatatagged(lua_State* L, void* p, int tag);

	// Token: 0x06004795 RID: 18325
	[DllImport("luau")]
	public unsafe static extern void lua_pushnil(lua_State* L);

	// Token: 0x06004796 RID: 18326
	[DllImport("luau")]
	public unsafe static extern int lua_next(lua_State* L, int index);

	// Token: 0x06004797 RID: 18327
	[DllImport("luau")]
	public unsafe static extern void lua_rawseti(lua_State* L, int idx, int n);

	// Token: 0x06004798 RID: 18328
	[DllImport("luau")]
	public unsafe static extern void lua_rawgeti(lua_State* L, int index, int n);

	// Token: 0x06004799 RID: 18329
	[DllImport("luau")]
	public unsafe static extern void lua_rawget(lua_State* L, int index);

	// Token: 0x0600479A RID: 18330
	[DllImport("luau")]
	public unsafe static extern void lua_rawset(lua_State* L, int index);

	// Token: 0x0600479B RID: 18331
	[DllImport("luau")]
	public unsafe static extern void lua_remove(lua_State* L, int index);

	// Token: 0x0600479C RID: 18332
	[DllImport("luau")]
	public unsafe static extern void lua_pushboolean(lua_State* L, int b);

	// Token: 0x0600479D RID: 18333
	[DllImport("luau")]
	public unsafe static extern int lua_rawequal(lua_State* L, int a, int b);

	// Token: 0x0600479E RID: 18334 RVA: 0x0017896D File Offset: 0x00176B6D
	public unsafe static void* lua_newuserdata(lua_State* L, int size)
	{
		return Luau.lua_newuserdatatagged(L, size, 0);
	}

	// Token: 0x0600479F RID: 18335 RVA: 0x00178977 File Offset: 0x00176B77
	public unsafe static double lua_tonumber(lua_State* L, int index)
	{
		return Luau.lua_tonumberx(L, index, null);
	}

	// Token: 0x060047A0 RID: 18336 RVA: 0x00178984 File Offset: 0x00176B84
	public unsafe static T* lua_class_push<[IsUnmanaged] T>(lua_State* L) where T : struct, ValueType
	{
		T* result = (T*)Luau.lua_newuserdata(L, sizeof(T) + 4);
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
		return result;
	}

	// Token: 0x060047A1 RID: 18337 RVA: 0x001789C0 File Offset: 0x00176BC0
	public unsafe static T* lua_class_push<[IsUnmanaged] T>(lua_State* L, FixedString32Bytes name) where T : struct, ValueType
	{
		T* result = (T*)Luau.lua_newuserdata(L, sizeof(T) + 4);
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
		return result;
	}

	// Token: 0x060047A2 RID: 18338 RVA: 0x001789F8 File Offset: 0x00176BF8
	public unsafe static void lua_class_push(lua_State* L, FixedString32Bytes name, IntPtr ptr)
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		Luau.lua_createtable(L, 0, 0);
		Luau.lua_pushlightuserdatatagged(L, (void*)ptr, 0);
		Luau.lua_setfield(L, -2, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
	}

	// Token: 0x060047A3 RID: 18339 RVA: 0x00178A50 File Offset: 0x00176C50
	public unsafe static T* lua_class_get<[IsUnmanaged] T>(lua_State* L, int idx) where T : struct, ValueType
	{
		int num = Luau.lua_type(L, idx);
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			T* ptr2 = (T*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 2);
			if (flag)
			{
				Luau.lua_getfield(L, idx, "__ptr");
				if (Luau.lua_type(L, -1) == 2)
				{
					T* ptr3 = (T*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
		return null;
	}

	// Token: 0x060047A4 RID: 18340 RVA: 0x00178B08 File Offset: 0x00176D08
	public unsafe static T* lua_class_get<[IsUnmanaged] T>(lua_State* L, int idx, FixedString32Bytes name) where T : struct, ValueType
	{
		int num = Luau.lua_type(L, idx);
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			T* ptr2 = (T*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 1);
			if (flag)
			{
				FixedString32Bytes fixedString32Bytes = "__ptr";
				Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
				if (Luau.lua_type(L, -1) == 2)
				{
					T* ptr3 = (T*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return null;
	}

	// Token: 0x060047A5 RID: 18341 RVA: 0x00178BC8 File Offset: 0x00176DC8
	public unsafe static byte* lua_class_get(lua_State* L, int idx, FixedString32Bytes name)
	{
		int num = Luau.lua_type(L, idx);
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			byte* ptr2 = (byte*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 1);
			if (flag)
			{
				FixedString32Bytes fixedString32Bytes = "__ptr";
				Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
				if (Luau.lua_type(L, -1) == 2)
				{
					byte* ptr3 = (byte*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return null;
	}

	// Token: 0x060047A6 RID: 18342 RVA: 0x00178C88 File Offset: 0x00176E88
	public unsafe static IntPtr lua_light_ptr(lua_State* L, int idx)
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		if (Luau.lua_type(L, -1) == 2)
		{
			void* ptr = Luau.lua_touserdata(L, -1);
			Luau.lua_pop(L, 1);
			if (ptr != null)
			{
				return (IntPtr)ptr;
			}
		}
		return IntPtr.Zero;
	}

	// Token: 0x060047A7 RID: 18343 RVA: 0x00178CDB File Offset: 0x00176EDB
	public unsafe static bool lua_class_check<[IsUnmanaged] T>(lua_State* L, int idx) where T : struct, ValueType
	{
		return Luau.lua_objlen(L, idx) == sizeof(T);
	}

	// Token: 0x060047A8 RID: 18344 RVA: 0x00178CEC File Offset: 0x00176EEC
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int lua_print(lua_State* L)
	{
		string text = "";
		int num = Luau.lua_gettop(L);
		for (int i = 1; i <= num; i++)
		{
			int num2 = Luau.lua_type(L, i);
			if (num2 == 5 || num2 == 3)
			{
				sbyte* ptr = Luau.lua_tostring(L, i);
				text += Marshal.PtrToStringAnsi((IntPtr)((void*)ptr));
			}
			else
			{
				if (num2 != 1)
				{
					Luau.luaL_errorL(L, "Invalid String", Array.Empty<string>());
					return 0;
				}
				int num3 = Luau.lua_toboolean(L, i);
				text += ((num3 == 1) ? "true" : "false");
			}
		}
		LuauHud.Instance.LuauLog(text);
		return 0;
	}

	// Token: 0x04005842 RID: 22594
	public const int LUA_GLOBALSINDEX = -10002;

	// Token: 0x04005843 RID: 22595
	public const int LUA_REGISTRYINDEX = -10000;

	// Token: 0x02000B59 RID: 2905
	public enum lua_Types
	{
		// Token: 0x04005845 RID: 22597
		LUA_TNIL,
		// Token: 0x04005846 RID: 22598
		LUA_TBOOLEAN,
		// Token: 0x04005847 RID: 22599
		LUA_TLIGHTUSERDATA,
		// Token: 0x04005848 RID: 22600
		LUA_TNUMBER,
		// Token: 0x04005849 RID: 22601
		LUA_TVECTOR,
		// Token: 0x0400584A RID: 22602
		LUA_TSTRING,
		// Token: 0x0400584B RID: 22603
		LUA_TTABLE,
		// Token: 0x0400584C RID: 22604
		LUA_TFUNCTION,
		// Token: 0x0400584D RID: 22605
		LUA_TUSERDATA,
		// Token: 0x0400584E RID: 22606
		LUA_TTHREAD,
		// Token: 0x0400584F RID: 22607
		LUA_TBUFFER,
		// Token: 0x04005850 RID: 22608
		LUA_TPROTO,
		// Token: 0x04005851 RID: 22609
		LUA_TUPVAL,
		// Token: 0x04005852 RID: 22610
		LUA_TDEADKEY,
		// Token: 0x04005853 RID: 22611
		LUA_T_COUNT = 11
	}

	// Token: 0x02000B5A RID: 2906
	public enum lua_Status
	{
		// Token: 0x04005855 RID: 22613
		LUA_OK,
		// Token: 0x04005856 RID: 22614
		LUA_YIELD,
		// Token: 0x04005857 RID: 22615
		LUA_ERRRUN,
		// Token: 0x04005858 RID: 22616
		LUA_ERRSYNTAX,
		// Token: 0x04005859 RID: 22617
		LUA_ERRMEM,
		// Token: 0x0400585A RID: 22618
		LUA_ERRERR,
		// Token: 0x0400585B RID: 22619
		LUA_BREAK
	}

	// Token: 0x02000B5B RID: 2907
	public enum gc_status
	{
		// Token: 0x0400585D RID: 22621
		LUA_GCSTOP,
		// Token: 0x0400585E RID: 22622
		LUA_GCRESTART,
		// Token: 0x0400585F RID: 22623
		LUA_GCCOLLECT,
		// Token: 0x04005860 RID: 22624
		LUA_GCCOUNT,
		// Token: 0x04005861 RID: 22625
		LUA_GCISRUNNING,
		// Token: 0x04005862 RID: 22626
		LUA_GCSTEP,
		// Token: 0x04005863 RID: 22627
		LUA_GCSETGOAL,
		// Token: 0x04005864 RID: 22628
		LUA_GCSETSTEPMUL,
		// Token: 0x04005865 RID: 22629
		LUA_GCSETSTEPSIZE
	}

	// Token: 0x02000B5C RID: 2908
	public static class lua_TypeID
	{
		// Token: 0x060047AA RID: 18346 RVA: 0x00178D88 File Offset: 0x00176F88
		public static string get(Type t)
		{
			string result;
			if (Luau.lua_TypeID.names.TryGetValue(t, ref result))
			{
				return result;
			}
			return "";
		}

		// Token: 0x060047AB RID: 18347 RVA: 0x00178DAB File Offset: 0x00176FAB
		public static void push(Type t, string name)
		{
			Luau.lua_TypeID.names.TryAdd(t, name);
		}

		// Token: 0x04005866 RID: 22630
		private static Dictionary<Type, string> names = new Dictionary<Type, string>();
	}

	// Token: 0x02000B5D RID: 2909
	public static class lua_ClassFields<T>
	{
		// Token: 0x060047AD RID: 18349 RVA: 0x00178DC8 File Offset: 0x00176FC8
		public static FieldInfo Get(string name)
		{
			Dictionary<int, FieldInfo> dictionary;
			FieldInfo result;
			if (Luau.lua_ClassFields<T>.classDictionarys.TryGetValue(typeof(T).GetHashCode(), ref dictionary) && dictionary.TryGetValue(name.GetHashCode(), ref result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060047AE RID: 18350 RVA: 0x00178E08 File Offset: 0x00177008
		public static void Add(string name, FieldInfo field)
		{
			Dictionary<int, FieldInfo> dictionary;
			if (Luau.lua_ClassFields<T>.classDictionarys.TryGetValue(typeof(T).GetHashCode(), ref dictionary))
			{
				dictionary.TryAdd(name.GetHashCode(), field);
				return;
			}
			Dictionary<int, FieldInfo> dictionary2 = new Dictionary<int, FieldInfo>();
			dictionary2.TryAdd(name.GetHashCode(), field);
			Luau.lua_ClassFields<T>.classDictionarys.TryAdd(typeof(T).GetHashCode(), dictionary2);
		}

		// Token: 0x04005867 RID: 22631
		private static Dictionary<int, Dictionary<int, FieldInfo>> classDictionarys = new Dictionary<int, Dictionary<int, FieldInfo>>();
	}

	// Token: 0x02000B5E RID: 2910
	public static class lua_ClassProperties<T>
	{
		// Token: 0x060047B0 RID: 18352 RVA: 0x00178E7C File Offset: 0x0017707C
		public static lua_CFunction Get(string name)
		{
			Dictionary<string, lua_CFunction> dictionary;
			lua_CFunction result;
			if (Luau.lua_ClassProperties<T>.classProperties.TryGetValue(typeof(T), ref dictionary) && dictionary.TryGetValue(name, ref result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060047B1 RID: 18353 RVA: 0x00178EB0 File Offset: 0x001770B0
		public static void Add(string name, lua_CFunction field)
		{
			Dictionary<string, lua_CFunction> dictionary;
			if (Luau.lua_ClassProperties<T>.classProperties.TryGetValue(typeof(T), ref dictionary))
			{
				dictionary.TryAdd(name, field);
				return;
			}
			Dictionary<string, lua_CFunction> dictionary2 = new Dictionary<string, lua_CFunction>();
			dictionary2.TryAdd(name, field);
			Luau.lua_ClassProperties<T>.classProperties.TryAdd(typeof(T), dictionary2);
		}

		// Token: 0x04005868 RID: 22632
		private static Dictionary<Type, Dictionary<string, lua_CFunction>> classProperties = new Dictionary<Type, Dictionary<string, lua_CFunction>>();
	}

	// Token: 0x02000B5F RID: 2911
	public static class lua_ClassFunctions<T>
	{
		// Token: 0x060047B3 RID: 18355 RVA: 0x00178F10 File Offset: 0x00177110
		public static lua_CFunction Get(string name)
		{
			Dictionary<string, lua_CFunction> dictionary;
			lua_CFunction result;
			if (Luau.lua_ClassFunctions<T>.classProperties.TryGetValue(typeof(T), ref dictionary) && dictionary.TryGetValue(name, ref result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060047B4 RID: 18356 RVA: 0x00178F44 File Offset: 0x00177144
		public static void Add(string name, lua_CFunction field)
		{
			Dictionary<string, lua_CFunction> dictionary;
			if (Luau.lua_ClassFunctions<T>.classProperties.TryGetValue(typeof(T), ref dictionary))
			{
				dictionary.TryAdd(name, field);
				return;
			}
			Dictionary<string, lua_CFunction> dictionary2 = new Dictionary<string, lua_CFunction>();
			dictionary2.TryAdd(name, field);
			Luau.lua_ClassFunctions<T>.classProperties.TryAdd(typeof(T), dictionary2);
		}

		// Token: 0x04005869 RID: 22633
		private static Dictionary<Type, Dictionary<string, lua_CFunction>> classProperties = new Dictionary<Type, Dictionary<string, lua_CFunction>>();
	}
}
