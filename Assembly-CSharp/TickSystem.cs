using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;

// Token: 0x02000C02 RID: 3074
internal abstract class TickSystem<T> : MonoBehaviour
{
	// Token: 0x06004BED RID: 19437 RVA: 0x0018C528 File Offset: 0x0018A728
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x06004BEE RID: 19438 RVA: 0x0018C53D File Offset: 0x0018A73D
	private void Update()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.TryRunCallbacks();
		TickSystem<T>.tickCallbacks.TryRunCallbacks();
	}

	// Token: 0x06004BEF RID: 19439 RVA: 0x0018C55B File Offset: 0x0018A75B
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.TryRunCallbacks();
	}

	// Token: 0x06004BF0 RID: 19440 RVA: 0x0018C570 File Offset: 0x0018A770
	static TickSystem()
	{
		TickSystem<T>.preTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPre>(100);
		TickSystem<T>.tickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperTick>(100);
		TickSystem<T>.postTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPost>(100);
	}

	// Token: 0x06004BF1 RID: 19441 RVA: 0x0018C5E3 File Offset: 0x0018A7E3
	private static void OnEnterPlay()
	{
		TickSystem<T>.preTickCallbacks.Clear();
		TickSystem<T>.preTickWrapperTable.Clear();
		TickSystem<T>.tickCallbacks.Clear();
		TickSystem<T>.tickWrapperTable.Clear();
		TickSystem<T>.postTickCallbacks.Clear();
		TickSystem<T>.postTickWrapperTable.Clear();
	}

	// Token: 0x06004BF2 RID: 19442 RVA: 0x0018C624 File Offset: 0x0018A824
	[MethodImpl(256)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		if (callback.PreTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPre tickCallbackWrapperPre = TickSystem<T>.preTickWrapperPool.Take();
		tickCallbackWrapperPre.target = callback;
		TickSystem<T>.preTickWrapperTable[callback] = tickCallbackWrapperPre;
		TickSystem<T>.preTickCallbacks.Add(tickCallbackWrapperPre);
		callback.PreTickRunning = true;
	}

	// Token: 0x06004BF3 RID: 19443 RVA: 0x0018C66C File Offset: 0x0018A86C
	[MethodImpl(256)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		if (callback.TickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperTick tickCallbackWrapperTick = TickSystem<T>.tickWrapperPool.Take();
		tickCallbackWrapperTick.target = callback;
		TickSystem<T>.tickWrapperTable[callback] = tickCallbackWrapperTick;
		TickSystem<T>.tickCallbacks.Add(tickCallbackWrapperTick);
		callback.TickRunning = true;
	}

	// Token: 0x06004BF4 RID: 19444 RVA: 0x0018C6B4 File Offset: 0x0018A8B4
	[MethodImpl(256)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		if (callback.PostTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPost tickCallbackWrapperPost = TickSystem<T>.postTickWrapperPool.Take();
		tickCallbackWrapperPost.target = callback;
		TickSystem<T>.postTickWrapperTable[callback] = tickCallbackWrapperPost;
		TickSystem<T>.postTickCallbacks.Add(tickCallbackWrapperPost);
		callback.PostTickRunning = true;
	}

	// Token: 0x06004BF5 RID: 19445 RVA: 0x0018C6FB File Offset: 0x0018A8FB
	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem<T>.AddPreTickCallback(callback);
		TickSystem<T>.AddTickCallback(callback);
		TickSystem<T>.AddPostTickCallback(callback);
	}

	// Token: 0x06004BF6 RID: 19446 RVA: 0x0018C710 File Offset: 0x0018A910
	public static void AddCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.AddTickSystemCallBack(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.AddPreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.AddTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.AddPostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x06004BF7 RID: 19447 RVA: 0x0018C760 File Offset: 0x0018A960
	[MethodImpl(256)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem<T>.TickCallbackWrapperPre instance;
		if (!callback.PreTickRunning || !TickSystem<T>.preTickWrapperTable.TryGetValue(callback, ref instance))
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.Remove(instance);
		callback.PreTickRunning = false;
		TickSystem<T>.preTickWrapperPool.Return(instance);
	}

	// Token: 0x06004BF8 RID: 19448 RVA: 0x0018C7A4 File Offset: 0x0018A9A4
	[MethodImpl(256)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem<T>.TickCallbackWrapperTick instance;
		if (!callback.TickRunning || !TickSystem<T>.tickWrapperTable.TryGetValue(callback, ref instance))
		{
			return;
		}
		TickSystem<T>.tickCallbacks.Remove(instance);
		callback.TickRunning = false;
		TickSystem<T>.tickWrapperPool.Return(instance);
	}

	// Token: 0x06004BF9 RID: 19449 RVA: 0x0018C7E8 File Offset: 0x0018A9E8
	[MethodImpl(256)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem<T>.TickCallbackWrapperPost instance;
		if (!callback.PostTickRunning || !TickSystem<T>.postTickWrapperTable.TryGetValue(callback, ref instance))
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.Remove(instance);
		callback.PostTickRunning = false;
		TickSystem<T>.postTickWrapperPool.Return(instance);
	}

	// Token: 0x06004BFA RID: 19450 RVA: 0x0018C82B File Offset: 0x0018AA2B
	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem<T>.RemovePreTickCallback(callback);
		TickSystem<T>.RemoveTickCallback(callback);
		TickSystem<T>.RemovePostTickCallback(callback);
	}

	// Token: 0x06004BFB RID: 19451 RVA: 0x0018C840 File Offset: 0x0018AA40
	public static void RemoveCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.RemoveTickSystemCallback(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.RemovePreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.RemoveTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.RemovePostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x04005BE8 RID: 23528
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPre> preTickWrapperPool;

	// Token: 0x04005BE9 RID: 23529
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPre> preTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPre>();

	// Token: 0x04005BEA RID: 23530
	private static readonly Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre> preTickWrapperTable = new Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre>(100);

	// Token: 0x04005BEB RID: 23531
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperTick> tickWrapperPool;

	// Token: 0x04005BEC RID: 23532
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperTick> tickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperTick>();

	// Token: 0x04005BED RID: 23533
	private static readonly Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick> tickWrapperTable = new Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick>(100);

	// Token: 0x04005BEE RID: 23534
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPost> postTickWrapperPool;

	// Token: 0x04005BEF RID: 23535
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPost> postTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPost>();

	// Token: 0x04005BF0 RID: 23536
	private static readonly Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost> postTickWrapperTable = new Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost>(100);

	// Token: 0x02000C03 RID: 3075
	private abstract class TickCallbackWrapper<U> : ObjectPoolEvents, ICallBack where U : class
	{
		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x06004BFD RID: 19453 RVA: 0x0018C88E File Offset: 0x0018AA8E
		// (set) Token: 0x06004BFE RID: 19454 RVA: 0x0018C896 File Offset: 0x0018AA96
		public U target
		{
			get
			{
				return this.m_target;
			}
			set
			{
				this.m_target = value;
			}
		}

		// Token: 0x06004BFF RID: 19455
		public abstract void CallBack();

		// Token: 0x06004C00 RID: 19456 RVA: 0x00002789 File Offset: 0x00000989
		public void OnTaken()
		{
		}

		// Token: 0x06004C01 RID: 19457 RVA: 0x0018C8A0 File Offset: 0x0018AAA0
		public void OnReturned()
		{
			this.target = default(U);
		}

		// Token: 0x04005BF1 RID: 23537
		protected U m_target;
	}

	// Token: 0x02000C04 RID: 3076
	private class TickCallbackWrapperPre : TickSystem<T>.TickCallbackWrapper<ITickSystemPre>
	{
		// Token: 0x06004C03 RID: 19459 RVA: 0x0018C8BC File Offset: 0x0018AABC
		public override void CallBack()
		{
			this.m_target.PreTick();
		}
	}

	// Token: 0x02000C05 RID: 3077
	private class TickCallbackWrapperTick : TickSystem<T>.TickCallbackWrapper<ITickSystemTick>
	{
		// Token: 0x06004C05 RID: 19461 RVA: 0x0018C8D1 File Offset: 0x0018AAD1
		public override void CallBack()
		{
			this.m_target.Tick();
		}
	}

	// Token: 0x02000C06 RID: 3078
	private class TickCallbackWrapperPost : TickSystem<T>.TickCallbackWrapper<ITickSystemPost>
	{
		// Token: 0x06004C07 RID: 19463 RVA: 0x0018C8E6 File Offset: 0x0018AAE6
		public override void CallBack()
		{
			this.m_target.PostTick();
		}
	}
}
