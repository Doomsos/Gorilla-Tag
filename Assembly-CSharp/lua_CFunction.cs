using System;
using System.Runtime.InteropServices;

[UnmanagedFunctionPointer(2)]
public unsafe delegate int lua_CFunction(lua_State* L);
