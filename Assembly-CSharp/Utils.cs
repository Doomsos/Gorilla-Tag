using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000C99 RID: 3225
public static class Utils
{
	// Token: 0x06004EB5 RID: 20149 RVA: 0x00197480 File Offset: 0x00195680
	public static void Disable(this GameObject target)
	{
		if (!target.activeSelf)
		{
			return;
		}
		PooledList<IPreDisable> pooledList = Utils.g_listPool.Take();
		List<IPreDisable> list = pooledList.List;
		target.GetComponents<IPreDisable>(list);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			try
			{
				list[i].PreDisable();
			}
			catch (Exception)
			{
			}
		}
		target.SetActive(false);
		Utils.g_listPool.Return(pooledList);
	}

	// Token: 0x06004EB6 RID: 20150 RVA: 0x001974F8 File Offset: 0x001956F8
	public static void AddIfNew<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
		}
	}

	// Token: 0x06004EB7 RID: 20151 RVA: 0x0019750A File Offset: 0x0019570A
	public static void RemoveIfContains<T>(this List<T> list, T item)
	{
		if (list.Contains(item))
		{
			list.Remove(item);
		}
	}

	// Token: 0x06004EB8 RID: 20152 RVA: 0x0019751D File Offset: 0x0019571D
	public static bool InRoom(this NetPlayer player)
	{
		return NetworkSystem.Instance.InRoom && Enumerable.Contains<NetPlayer>(NetworkSystem.Instance.AllNetPlayers, player);
	}

	// Token: 0x06004EB9 RID: 20153 RVA: 0x00197540 File Offset: 0x00195740
	public static bool PlayerInRoom(int actorNumber)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			for (int i = 0; i < allNetPlayers.Length; i++)
			{
				if (allNetPlayers[i].ActorNumber == actorNumber)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004EBA RID: 20154 RVA: 0x00197580 File Offset: 0x00195780
	public static bool PlayerInRoom(int actorNumer, out Player photonPlayer)
	{
		photonPlayer = null;
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumer, ref photonPlayer);
	}

	// Token: 0x06004EBB RID: 20155 RVA: 0x0019759F File Offset: 0x0019579F
	public static bool PlayerInRoom(int actorNumber, out NetPlayer player)
	{
		if (NetworkSystem.Instance == null)
		{
			player = null;
			return false;
		}
		player = NetworkSystem.Instance.GetPlayer(actorNumber);
		return NetworkSystem.Instance.InRoom && player != null;
	}

	// Token: 0x06004EBC RID: 20156 RVA: 0x001975D4 File Offset: 0x001957D4
	public static long PackVector3ToLong(Vector3 vector)
	{
		long num = (long)Mathf.Clamp(Mathf.RoundToInt(vector.x * 1024f) + 1048576, 0, 2097151);
		long num2 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.y * 1024f) + 1048576, 0, 2097151);
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.z * 1024f) + 1048576, 0, 2097151);
		return num + (num2 << 21) + (num3 << 42);
	}

	// Token: 0x06004EBD RID: 20157 RVA: 0x00197658 File Offset: 0x00195858
	public static Vector3 UnpackVector3FromLong(long data)
	{
		float num = (float)(data & 2097151L);
		long num2 = data >> 21 & 2097151L;
		long num3 = data >> 42 & 2097151L;
		return new Vector3((float)((long)num - 1048576L) * 0.0009765625f, (float)(num2 - 1048576L) * 0.0009765625f, (float)(num3 - 1048576L) * 0.0009765625f);
	}

	// Token: 0x06004EBE RID: 20158 RVA: 0x001976B6 File Offset: 0x001958B6
	public static bool IsASCIILetterOrDigit(char c)
	{
		return (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z');
	}

	// Token: 0x06004EBF RID: 20159 RVA: 0x00002789 File Offset: 0x00000989
	public static void Log(object message)
	{
	}

	// Token: 0x06004EC0 RID: 20160 RVA: 0x00002789 File Offset: 0x00000989
	public static void Log(object message, Object context)
	{
	}

	// Token: 0x06004EC1 RID: 20161 RVA: 0x001976E0 File Offset: 0x001958E0
	public static bool ValidateServerTime(double time, double maximumLatency)
	{
		double currentTime = PhotonNetwork.CurrentTime;
		double num = 4294967.295 - maximumLatency;
		double num2;
		if (currentTime > maximumLatency || time < maximumLatency)
		{
			if (time > currentTime + 0.5)
			{
				return false;
			}
			num2 = currentTime - time;
		}
		else
		{
			double num3 = num + currentTime;
			if (time > currentTime + 0.5 && time < num3)
			{
				return false;
			}
			num2 = currentTime + (4294967.295 - time);
		}
		return num2 <= maximumLatency;
	}

	// Token: 0x06004EC2 RID: 20162 RVA: 0x00197750 File Offset: 0x00195950
	public static double CalculateNetworkDeltaTime(double prevTime, double newTime)
	{
		if (newTime >= prevTime)
		{
			return newTime - prevTime;
		}
		double num = 4294967.295 - prevTime;
		return newTime + num;
	}

	// Token: 0x04005D89 RID: 23945
	private static ObjectPool<PooledList<IPreDisable>> g_listPool = new ObjectPool<PooledList<IPreDisable>>(2, 10);

	// Token: 0x04005D8A RID: 23946
	private static StringBuilder reusableSB = new StringBuilder();
}
