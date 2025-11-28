using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000825 RID: 2085
[DisallowMultipleComponent]
public abstract class GTSystem<T> : MonoBehaviour, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : MonoBehaviour
{
	// Token: 0x170004EC RID: 1260
	// (get) Token: 0x060036C2 RID: 14018 RVA: 0x00128142 File Offset: 0x00126342
	public PhotonView photonView
	{
		get
		{
			return this._photonView;
		}
	}

	// Token: 0x060036C3 RID: 14019 RVA: 0x0012814A File Offset: 0x0012634A
	protected virtual void Awake()
	{
		GTSystem<T>.SetSingleton(this);
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x00128154 File Offset: 0x00126354
	protected virtual void Tick()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < this._instances.Count; i++)
		{
			T t = this._instances[i];
			if (t)
			{
				this.OnTick(deltaTime, t);
			}
		}
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x0012819F File Offset: 0x0012639F
	protected virtual void OnApplicationQuit()
	{
		GTSystem<T>.gAppQuitting = true;
	}

	// Token: 0x060036C6 RID: 14022 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnTick(float dt, T instance)
	{
	}

	// Token: 0x060036C7 RID: 14023 RVA: 0x001281A8 File Offset: 0x001263A8
	private bool RegisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Register] Instance is null.", null);
			return false;
		}
		if (this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Add(instance);
		this.OnRegister(instance);
		return true;
	}

	// Token: 0x060036C8 RID: 14024 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnRegister(T instance)
	{
	}

	// Token: 0x060036C9 RID: 14025 RVA: 0x0012820C File Offset: 0x0012640C
	private bool UnregisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Unregister] Instance is null.", null);
			return false;
		}
		if (!this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Remove(instance);
		this.OnUnregister(instance);
		return true;
	}

	// Token: 0x060036CA RID: 14026 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnUnregister(T instance)
	{
	}

	// Token: 0x060036CB RID: 14027 RVA: 0x0012826E File Offset: 0x0012646E
	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return this._instances.GetEnumerator();
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x0012826E File Offset: 0x0012646E
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this._instances.GetEnumerator();
	}

	// Token: 0x170004ED RID: 1261
	// (get) Token: 0x060036CD RID: 14029 RVA: 0x0012827B File Offset: 0x0012647B
	int IReadOnlyCollection<!0>.Count
	{
		get
		{
			return this._instances.Count;
		}
	}

	// Token: 0x170004EE RID: 1262
	// (get) Token: 0x060036CE RID: 14030 RVA: 0x00128288 File Offset: 0x00126488
	T IReadOnlyList<!0>.Item
	{
		get
		{
			return this._instances[index];
		}
	}

	// Token: 0x170004EF RID: 1263
	// (get) Token: 0x060036CF RID: 14031 RVA: 0x00128296 File Offset: 0x00126496
	public static PhotonView PhotonView
	{
		get
		{
			return GTSystem<T>.gSingleton._photonView;
		}
	}

	// Token: 0x060036D0 RID: 14032 RVA: 0x001282A4 File Offset: 0x001264A4
	protected static void SetSingleton(GTSystem<T> system)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (GTSystem<T>.gSingleton != null && GTSystem<T>.gSingleton != system)
		{
			Object.Destroy(system);
			GTDev.LogWarning<string>("Singleton of type " + GTSystem<T>.gSingleton.GetType().Name + " already exists.", null);
			return;
		}
		GTSystem<T>.gSingleton = system;
		if (!GTSystem<T>.gInitializing)
		{
			return;
		}
		GTSystem<T>.gSingleton._instances.Clear();
		T[] array = Enumerable.ToArray<T>(Enumerable.Where<T>(GTSystem<T>.gQueueRegister, (T x) => x != null));
		GTSystem<T>.gSingleton._instances.AddRange(array);
		GTSystem<T>.gQueueRegister.Clear();
		PhotonView component = GTSystem<T>.gSingleton.GetComponent<PhotonView>();
		if (component != null)
		{
			GTSystem<T>.gSingleton._photonView = component;
			GTSystem<T>.gSingleton._networked = true;
		}
		GTSystem<T>.gInitializing = false;
	}

	// Token: 0x060036D1 RID: 14033 RVA: 0x00128394 File Offset: 0x00126594
	public static void Register(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		GTSystem<T>.gSingleton.RegisterInstance(instance);
	}

	// Token: 0x060036D2 RID: 14034 RVA: 0x00128400 File Offset: 0x00126600
	public static void Unregister(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		GTSystem<T>.gSingleton.UnregisterInstance(instance);
	}

	// Token: 0x04004629 RID: 17961
	[SerializeField]
	protected List<T> _instances = new List<T>();

	// Token: 0x0400462A RID: 17962
	[SerializeField]
	private bool _networked;

	// Token: 0x0400462B RID: 17963
	[SerializeField]
	private PhotonView _photonView;

	// Token: 0x0400462C RID: 17964
	private static GTSystem<T> gSingleton;

	// Token: 0x0400462D RID: 17965
	private static bool gInitializing = false;

	// Token: 0x0400462E RID: 17966
	private static bool gAppQuitting = false;

	// Token: 0x0400462F RID: 17967
	private static HashSet<T> gQueueRegister = new HashSet<T>();
}
