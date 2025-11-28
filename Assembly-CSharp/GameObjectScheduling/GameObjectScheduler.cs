using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02001159 RID: 4441
	public class GameObjectScheduler : MonoBehaviour
	{
		// Token: 0x0600700A RID: 28682 RVA: 0x00247328 File Offset: 0x00245528
		private void Start()
		{
			this.schedule.Validate();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				list.Add(base.transform.GetChild(i).gameObject);
			}
			this.scheduledGameObject = list.ToArray();
			for (int j = 0; j < this.scheduledGameObject.Length; j++)
			{
				this.scheduledGameObject[j].SetActive(false);
			}
			this.dispatcher = base.GetComponent<GameObjectSchedulerEventDispatcher>();
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}

		// Token: 0x0600700B RID: 28683 RVA: 0x002473BE File Offset: 0x002455BE
		private void OnEnable()
		{
			if (this.monitor == null && this.scheduledGameObject != null)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		// Token: 0x0600700C RID: 28684 RVA: 0x002473E2 File Offset: 0x002455E2
		private void OnDisable()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		// Token: 0x0600700D RID: 28685 RVA: 0x002473FF File Offset: 0x002455FF
		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			bool previousState = this.getActiveState();
			for (int i = 0; i < this.scheduledGameObject.Length; i++)
			{
				this.scheduledGameObject[i].SetActive(previousState);
			}
			for (;;)
			{
				yield return new WaitForSeconds(60f);
				bool activeState = this.getActiveState();
				if (previousState != activeState)
				{
					this.changeActiveState(activeState);
					previousState = activeState;
				}
			}
			yield break;
		}

		// Token: 0x0600700E RID: 28686 RVA: 0x00247410 File Offset: 0x00245610
		private bool getActiveState()
		{
			this.currentNodeIndex = this.schedule.GetCurrentNodeIndex(this.getServerTime(), 0);
			bool result;
			if (this.currentNodeIndex == -1)
			{
				result = this.schedule.InitialState;
			}
			else if (this.currentNodeIndex < this.schedule.Nodes.Length)
			{
				result = this.schedule.Nodes[this.currentNodeIndex].ActiveState;
			}
			else
			{
				result = this.schedule.Nodes[this.schedule.Nodes.Length - 1].ActiveState;
			}
			return result;
		}

		// Token: 0x0600700F RID: 28687 RVA: 0x001FA0F7 File Offset: 0x001F82F7
		private DateTime getServerTime()
		{
			return GorillaComputer.instance.GetServerTime();
		}

		// Token: 0x06007010 RID: 28688 RVA: 0x002474A0 File Offset: 0x002456A0
		private void changeActiveState(bool state)
		{
			if (state)
			{
				for (int i = 0; i < this.scheduledGameObject.Length; i++)
				{
					this.scheduledGameObject[i].SetActive(true);
				}
				if (this.dispatcher != null && this.dispatcher.OnScheduledActivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
			}
			else
			{
				if (this.dispatcher != null && this.dispatcher.OnScheduledDeactivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
				for (int j = 0; j < this.scheduledGameObject.Length; j++)
				{
					this.scheduledGameObject[j].SetActive(false);
				}
			}
		}

		// Token: 0x04008063 RID: 32867
		[SerializeField]
		private GameObjectSchedule schedule;

		// Token: 0x04008064 RID: 32868
		private GameObject[] scheduledGameObject;

		// Token: 0x04008065 RID: 32869
		private GameObjectSchedulerEventDispatcher dispatcher;

		// Token: 0x04008066 RID: 32870
		private int currentNodeIndex = -1;

		// Token: 0x04008067 RID: 32871
		private Coroutine monitor;
	}
}
