using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009E3 RID: 2531
public class LifeCycleEventTrigger : MonoBehaviour
{
	// Token: 0x06004088 RID: 16520 RVA: 0x001595F7 File Offset: 0x001577F7
	private void Awake()
	{
		UnityEvent onAwake = this._onAwake;
		if (onAwake == null)
		{
			return;
		}
		onAwake.Invoke();
	}

	// Token: 0x06004089 RID: 16521 RVA: 0x00159609 File Offset: 0x00157809
	private void Start()
	{
		UnityEvent onStart = this._onStart;
		if (onStart == null)
		{
			return;
		}
		onStart.Invoke();
	}

	// Token: 0x0600408A RID: 16522 RVA: 0x0015961B File Offset: 0x0015781B
	private void OnEnable()
	{
		UnityEvent onEnable = this._onEnable;
		if (onEnable == null)
		{
			return;
		}
		onEnable.Invoke();
	}

	// Token: 0x0600408B RID: 16523 RVA: 0x0015962D File Offset: 0x0015782D
	private void OnDisable()
	{
		UnityEvent onDisable = this._onDisable;
		if (onDisable == null)
		{
			return;
		}
		onDisable.Invoke();
	}

	// Token: 0x0600408C RID: 16524 RVA: 0x0015963F File Offset: 0x0015783F
	private void OnDestroy()
	{
		UnityEvent onDestroy = this._onDestroy;
		if (onDestroy == null)
		{
			return;
		}
		onDestroy.Invoke();
	}

	// Token: 0x0400519E RID: 20894
	[SerializeField]
	private UnityEvent _onAwake;

	// Token: 0x0400519F RID: 20895
	[SerializeField]
	private UnityEvent _onStart;

	// Token: 0x040051A0 RID: 20896
	[SerializeField]
	private UnityEvent _onEnable;

	// Token: 0x040051A1 RID: 20897
	[SerializeField]
	private UnityEvent _onDisable;

	// Token: 0x040051A2 RID: 20898
	[SerializeField]
	private UnityEvent _onDestroy;
}
