using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viveport
{
	// Token: 0x02000D07 RID: 3335
	public class MainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x060050F1 RID: 20721 RVA: 0x001A1E81 File Offset: 0x001A0081
		private void Awake()
		{
			if (MainThreadDispatcher.instance == null)
			{
				MainThreadDispatcher.instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x060050F2 RID: 20722 RVA: 0x001A1EA4 File Offset: 0x001A00A4
		public void Update()
		{
			Queue<Action> queue = MainThreadDispatcher.actions;
			lock (queue)
			{
				while (MainThreadDispatcher.actions.Count > 0)
				{
					MainThreadDispatcher.actions.Dequeue().Invoke();
				}
			}
		}

		// Token: 0x060050F3 RID: 20723 RVA: 0x001A1EFC File Offset: 0x001A00FC
		public static MainThreadDispatcher Instance()
		{
			if (MainThreadDispatcher.instance == null)
			{
				throw new Exception("Could not find the MainThreadDispatcher GameObject. Please ensure you have added this script to an empty GameObject in your scene.");
			}
			return MainThreadDispatcher.instance;
		}

		// Token: 0x060050F4 RID: 20724 RVA: 0x001A1F1B File Offset: 0x001A011B
		private void OnDestroy()
		{
			MainThreadDispatcher.instance = null;
		}

		// Token: 0x060050F5 RID: 20725 RVA: 0x001A1F24 File Offset: 0x001A0124
		public void Enqueue(IEnumerator action)
		{
			Queue<Action> queue = MainThreadDispatcher.actions;
			lock (queue)
			{
				MainThreadDispatcher.actions.Enqueue(delegate()
				{
					this.StartCoroutine(action);
				});
			}
		}

		// Token: 0x060050F6 RID: 20726 RVA: 0x001A1F88 File Offset: 0x001A0188
		public void Enqueue(Action action)
		{
			this.Enqueue(this.ActionWrapper(action));
		}

		// Token: 0x060050F7 RID: 20727 RVA: 0x001A1F97 File Offset: 0x001A0197
		public void Enqueue<T1>(Action<T1> action, T1 param1)
		{
			this.Enqueue(this.ActionWrapper<T1>(action, param1));
		}

		// Token: 0x060050F8 RID: 20728 RVA: 0x001A1FA7 File Offset: 0x001A01A7
		public void Enqueue<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			this.Enqueue(this.ActionWrapper<T1, T2>(action, param1, param2));
		}

		// Token: 0x060050F9 RID: 20729 RVA: 0x001A1FB8 File Offset: 0x001A01B8
		public void Enqueue<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3>(action, param1, param2, param3));
		}

		// Token: 0x060050FA RID: 20730 RVA: 0x001A1FCB File Offset: 0x001A01CB
		public void Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3, T4>(action, param1, param2, param3, param4));
		}

		// Token: 0x060050FB RID: 20731 RVA: 0x001A1FE0 File Offset: 0x001A01E0
		private IEnumerator ActionWrapper(Action action)
		{
			action.Invoke();
			yield return null;
			yield break;
		}

		// Token: 0x060050FC RID: 20732 RVA: 0x001A1FEF File Offset: 0x001A01EF
		private IEnumerator ActionWrapper<T1>(Action<T1> action, T1 param1)
		{
			action.Invoke(param1);
			yield return null;
			yield break;
		}

		// Token: 0x060050FD RID: 20733 RVA: 0x001A2005 File Offset: 0x001A0205
		private IEnumerator ActionWrapper<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			action.Invoke(param1, param2);
			yield return null;
			yield break;
		}

		// Token: 0x060050FE RID: 20734 RVA: 0x001A2022 File Offset: 0x001A0222
		private IEnumerator ActionWrapper<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			action.Invoke(param1, param2, param3);
			yield return null;
			yield break;
		}

		// Token: 0x060050FF RID: 20735 RVA: 0x001A2047 File Offset: 0x001A0247
		private IEnumerator ActionWrapper<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			action.Invoke(param1, param2, param3, param4);
			yield return null;
			yield break;
		}

		// Token: 0x04006029 RID: 24617
		private static readonly Queue<Action> actions = new Queue<Action>();

		// Token: 0x0400602A RID: 24618
		private static MainThreadDispatcher instance = null;
	}
}
