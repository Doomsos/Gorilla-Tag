using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000764 RID: 1892
internal class RPCUtil
{
	// Token: 0x06003111 RID: 12561 RVA: 0x0010B1D4 File Offset: 0x001093D4
	public static bool NotSpam(string id, PhotonMessageInfoWrapped info, float delay)
	{
		RPCUtil.RPCCallID rpccallID = new RPCUtil.RPCCallID(id, info.senderID);
		if (!RPCUtil.RPCCallLog.ContainsKey(rpccallID))
		{
			RPCUtil.RPCCallLog.Add(rpccallID, Time.time);
			return true;
		}
		if (Time.time - RPCUtil.RPCCallLog[rpccallID] > delay)
		{
			RPCUtil.RPCCallLog[rpccallID] = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x06003112 RID: 12562 RVA: 0x0010B235 File Offset: 0x00109435
	public static bool SafeValue(float v)
	{
		return !float.IsNaN(v) && float.IsFinite(v);
	}

	// Token: 0x06003113 RID: 12563 RVA: 0x0010B247 File Offset: 0x00109447
	public static bool SafeValue(float v, float min, float max)
	{
		return RPCUtil.SafeValue(v) && v <= max && v >= min;
	}

	// Token: 0x04003FDE RID: 16350
	private static Dictionary<RPCUtil.RPCCallID, float> RPCCallLog = new Dictionary<RPCUtil.RPCCallID, float>();

	// Token: 0x02000765 RID: 1893
	private struct RPCCallID : IEquatable<RPCUtil.RPCCallID>
	{
		// Token: 0x06003116 RID: 12566 RVA: 0x0010B26C File Offset: 0x0010946C
		public RPCCallID(string nameOfFunction, int senderId)
		{
			this._senderID = senderId;
			this._nameOfFunction = nameOfFunction;
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06003117 RID: 12567 RVA: 0x0010B27C File Offset: 0x0010947C
		public readonly int SenderID
		{
			get
			{
				return this._senderID;
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06003118 RID: 12568 RVA: 0x0010B284 File Offset: 0x00109484
		public readonly string NameOfFunction
		{
			get
			{
				return this._nameOfFunction;
			}
		}

		// Token: 0x06003119 RID: 12569 RVA: 0x0010B28C File Offset: 0x0010948C
		bool IEquatable<RPCUtil.RPCCallID>.Equals(RPCUtil.RPCCallID other)
		{
			return other.NameOfFunction.Equals(this.NameOfFunction) && other.SenderID.Equals(this.SenderID);
		}

		// Token: 0x04003FDF RID: 16351
		private int _senderID;

		// Token: 0x04003FE0 RID: 16352
		private string _nameOfFunction;
	}
}
