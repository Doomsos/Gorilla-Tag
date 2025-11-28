using System;
using System.Runtime.InteropServices;

// Token: 0x02000B56 RID: 2902
// (Invoke) Token: 0x0600474D RID: 18253
[UnmanagedFunctionPointer(2)]
public unsafe delegate int lua_CFunction(lua_State* L);
