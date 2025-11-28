using System;
using UnityEngine;

// Token: 0x02000BE8 RID: 3048
public class GTDelayedExec : ITickSystemTick
{
	// Token: 0x170006FE RID: 1790
	// (get) Token: 0x06004B33 RID: 19251 RVA: 0x00188CB2 File Offset: 0x00186EB2
	// (set) Token: 0x06004B34 RID: 19252 RVA: 0x00188CB9 File Offset: 0x00186EB9
	public static GTDelayedExec instance { get; private set; }

	// Token: 0x170006FF RID: 1791
	// (get) Token: 0x06004B35 RID: 19253 RVA: 0x00188CC1 File Offset: 0x00186EC1
	// (set) Token: 0x06004B36 RID: 19254 RVA: 0x00188CC8 File Offset: 0x00186EC8
	public static int listenerCount { get; private set; }

	// Token: 0x06004B37 RID: 19255 RVA: 0x00188CD0 File Offset: 0x00186ED0
	[OnEnterPlay_Run]
	private static void EdReInit()
	{
		GTDelayedExec._listenerDelays = new float[1024];
		GTDelayedExec._listeners = new GTDelayedExec.Listener[1024];
	}

	// Token: 0x06004B38 RID: 19256 RVA: 0x00188CF0 File Offset: 0x00186EF0
	[RuntimeInitializeOnLoadMethod(2)]
	private static void InitializeAfterAssemblies()
	{
		GTDelayedExec.listenerCount = 0;
		GTDelayedExec.instance = new GTDelayedExec();
		TickSystem<object>.AddTickCallback(GTDelayedExec.instance);
	}

	// Token: 0x06004B39 RID: 19257 RVA: 0x00188D0C File Offset: 0x00186F0C
	internal static void Add(IDelayedExecListener listener, float delay, int contextId)
	{
		if (GTDelayedExec.listenerCount >= GTDelayedExec.maxListenersCount)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ERROR!!!  GTDelayedExec: Recovering from default maximum number of delayed listeners ",
				1024.ToString(),
				" reached. Please set the k_defaultMaxListenersCount value to ",
				(GTDelayedExec.maxListenersCount * 2).ToString(),
				"."
			}));
			GTDelayedExec.maxListenersCount *= 2;
			Array.Resize<float>(ref GTDelayedExec._listenerDelays, GTDelayedExec.maxListenersCount);
			Array.Resize<GTDelayedExec.Listener>(ref GTDelayedExec._listeners, GTDelayedExec.maxListenersCount);
		}
		GTDelayedExec._listenerDelays[GTDelayedExec.listenerCount] = Time.unscaledTime + delay;
		GTDelayedExec._listeners[GTDelayedExec.listenerCount] = new GTDelayedExec.Listener(listener, contextId);
		GTDelayedExec.listenerCount++;
	}

	// Token: 0x17000700 RID: 1792
	// (get) Token: 0x06004B3A RID: 19258 RVA: 0x00188DCD File Offset: 0x00186FCD
	// (set) Token: 0x06004B3B RID: 19259 RVA: 0x00188DD5 File Offset: 0x00186FD5
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06004B3C RID: 19260 RVA: 0x00188DE0 File Offset: 0x00186FE0
	void ITickSystemTick.Tick()
	{
		for (int i = 0; i < GTDelayedExec.listenerCount; i++)
		{
			if (Time.unscaledTime >= GTDelayedExec._listenerDelays[i])
			{
				try
				{
					GTDelayedExec._listeners[i].listener.OnDelayedAction(GTDelayedExec._listeners[i].contextId);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				GTDelayedExec.listenerCount--;
				GTDelayedExec._listenerDelays[i] = GTDelayedExec._listenerDelays[GTDelayedExec.listenerCount];
				GTDelayedExec._listeners[i] = GTDelayedExec._listeners[GTDelayedExec.listenerCount];
				i--;
			}
		}
	}

	// Token: 0x04005B58 RID: 23384
	public const int k_defaultMaxListenersCount = 1024;

	// Token: 0x04005B59 RID: 23385
	public static int maxListenersCount = 1024;

	// Token: 0x04005B5B RID: 23387
	private static float[] _listenerDelays = new float[1024];

	// Token: 0x04005B5C RID: 23388
	private static GTDelayedExec.Listener[] _listeners = new GTDelayedExec.Listener[1024];

	// Token: 0x02000BE9 RID: 3049
	private struct Listener
	{
		// Token: 0x06004B3F RID: 19263 RVA: 0x00188EB6 File Offset: 0x001870B6
		public Listener(IDelayedExecListener listener, int contextId)
		{
			this.listener = listener;
			this.contextId = contextId;
		}

		// Token: 0x04005B5E RID: 23390
		public readonly IDelayedExecListener listener;

		// Token: 0x04005B5F RID: 23391
		public readonly int contextId;
	}
}
