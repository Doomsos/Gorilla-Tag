using System;
using UnityEngine;

// Token: 0x02000BE6 RID: 3046
internal abstract class RPCNetworkBase : MonoBehaviour
{
	// Token: 0x06004B30 RID: 19248
	public abstract void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler);
}
