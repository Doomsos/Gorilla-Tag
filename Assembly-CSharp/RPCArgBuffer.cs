using System;
using System.Runtime.InteropServices;

// Token: 0x020003C0 RID: 960
public struct RPCArgBuffer<T> where T : struct
{
	// Token: 0x0600171A RID: 5914 RVA: 0x00080462 File Offset: 0x0007E662
	public RPCArgBuffer(T argStruct)
	{
		this.DataLength = Marshal.SizeOf(typeof(T));
		this.Data = new byte[this.DataLength];
		this.Args = argStruct;
	}

	// Token: 0x04002110 RID: 8464
	public T Args;

	// Token: 0x04002111 RID: 8465
	public byte[] Data;

	// Token: 0x04002112 RID: 8466
	public int DataLength;
}
