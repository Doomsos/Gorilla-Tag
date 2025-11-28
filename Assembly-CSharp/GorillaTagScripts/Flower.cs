using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DDE RID: 3550
	public class Flower : MonoBehaviour
	{
		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x06005830 RID: 22576 RVA: 0x001C2E53 File Offset: 0x001C1053
		// (set) Token: 0x06005831 RID: 22577 RVA: 0x001C2E5B File Offset: 0x001C105B
		public bool IsWatered { get; private set; }

		// Token: 0x06005832 RID: 22578 RVA: 0x001C2E64 File Offset: 0x001C1064
		private void Awake()
		{
			this.shouldUpdateVisuals = true;
			this.anim = base.GetComponent<Animator>();
			this.timer = base.GetComponent<GorillaTimer>();
			this.perchPoint = base.GetComponent<BeePerchPoint>();
			this.timer.onTimerStopped.AddListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
			this.currentState = Flower.FlowerState.None;
			this.wateredFx = this.wateredFx.GetComponent<ParticleSystem>();
			this.IsWatered = false;
			this.meshRenderer = base.GetComponent<SkinnedMeshRenderer>();
			this.meshRenderer.enabled = false;
			this.anim.enabled = false;
		}

		// Token: 0x06005833 RID: 22579 RVA: 0x001C2EFB File Offset: 0x001C10FB
		private void OnDestroy()
		{
			this.timer.onTimerStopped.RemoveListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
		}

		// Token: 0x06005834 RID: 22580 RVA: 0x001C2F1C File Offset: 0x001C111C
		public void WaterFlower(bool isWatered = false)
		{
			this.IsWatered = isWatered;
			switch (this.currentState)
			{
			case Flower.FlowerState.None:
				this.UpdateFlowerState(Flower.FlowerState.Healthy, false, true);
				return;
			case Flower.FlowerState.Healthy:
				if (!isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, false, true);
					return;
				}
				break;
			case Flower.FlowerState.Middle:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Healthy, true, true);
					return;
				}
				this.UpdateFlowerState(Flower.FlowerState.Wilted, false, true);
				return;
			case Flower.FlowerState.Wilted:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, true, true);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005835 RID: 22581 RVA: 0x001C2F8C File Offset: 0x001C118C
		public void UpdateFlowerState(Flower.FlowerState newState, bool isWatered = false, bool updateVisual = true)
		{
			if (FlowersManager.Instance.IsMine)
			{
				this.timer.RestartTimer();
			}
			this.ChangeState(newState);
			if (this.perchPoint)
			{
				this.perchPoint.enabled = (this.currentState == Flower.FlowerState.Healthy);
			}
			if (updateVisual)
			{
				this.LocalUpdateFlowers(newState, isWatered);
			}
		}

		// Token: 0x06005836 RID: 22582 RVA: 0x001C2FE4 File Offset: 0x001C11E4
		private void LocalUpdateFlowers(Flower.FlowerState state, bool isWatered = false)
		{
			GameObject[] array = this.meshStates;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (!this.shouldUpdateVisuals)
			{
				this.meshStates[(int)this.currentState].SetActive(true);
				return;
			}
			if (isWatered && this.wateredFx)
			{
				this.wateredFx.Play();
			}
			this.meshRenderer.enabled = true;
			this.anim.enabled = true;
			switch (state)
			{
			case Flower.FlowerState.Healthy:
				this.anim.SetTrigger(Flower.middle_to_healthy);
				return;
			case Flower.FlowerState.Middle:
				if (this.lastState == Flower.FlowerState.Wilted)
				{
					this.anim.SetTrigger(Flower.wilted_to_middle);
					return;
				}
				this.anim.SetTrigger(Flower.healthy_to_middle);
				return;
			case Flower.FlowerState.Wilted:
				this.anim.SetTrigger(Flower.middle_to_wilted);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005837 RID: 22583 RVA: 0x001C30BD File Offset: 0x001C12BD
		private void HandleOnFlowerTimerEnded(GorillaTimer _timer)
		{
			if (!FlowersManager.Instance.IsMine)
			{
				return;
			}
			if (this.timer == _timer)
			{
				this.WaterFlower(false);
			}
		}

		// Token: 0x06005838 RID: 22584 RVA: 0x001C30E1 File Offset: 0x001C12E1
		private void ChangeState(Flower.FlowerState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
		}

		// Token: 0x06005839 RID: 22585 RVA: 0x001C30F6 File Offset: 0x001C12F6
		public Flower.FlowerState GetCurrentState()
		{
			return this.currentState;
		}

		// Token: 0x0600583A RID: 22586 RVA: 0x001C3100 File Offset: 0x001C1300
		public void OnAnimationIsDone(int state)
		{
			if (this.meshRenderer.enabled)
			{
				for (int i = 0; i < this.meshStates.Length; i++)
				{
					bool active = i == (int)this.currentState;
					this.meshStates[i].SetActive(active);
				}
				this.anim.enabled = false;
				this.meshRenderer.enabled = false;
			}
		}

		// Token: 0x0600583B RID: 22587 RVA: 0x001C315D File Offset: 0x001C135D
		public void UpdateVisuals(bool enable)
		{
			this.shouldUpdateVisuals = enable;
			this.meshStatesGameObject.SetActive(enable);
		}

		// Token: 0x0600583C RID: 22588 RVA: 0x001C3174 File Offset: 0x001C1374
		public void AnimCatch()
		{
			if (this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				this.OnAnimationIsDone(0);
			}
		}

		// Token: 0x04006578 RID: 25976
		private Animator anim;

		// Token: 0x04006579 RID: 25977
		private SkinnedMeshRenderer meshRenderer;

		// Token: 0x0400657A RID: 25978
		[HideInInspector]
		public GorillaTimer timer;

		// Token: 0x0400657B RID: 25979
		private BeePerchPoint perchPoint;

		// Token: 0x0400657C RID: 25980
		public ParticleSystem wateredFx;

		// Token: 0x0400657D RID: 25981
		public ParticleSystem sparkleFx;

		// Token: 0x0400657E RID: 25982
		public GameObject meshStatesGameObject;

		// Token: 0x0400657F RID: 25983
		public GameObject[] meshStates;

		// Token: 0x04006580 RID: 25984
		private static readonly int healthy_to_middle = Animator.StringToHash("healthy_to_middle");

		// Token: 0x04006581 RID: 25985
		private static readonly int middle_to_healthy = Animator.StringToHash("middle_to_healthy");

		// Token: 0x04006582 RID: 25986
		private static readonly int wilted_to_middle = Animator.StringToHash("wilted_to_middle");

		// Token: 0x04006583 RID: 25987
		private static readonly int middle_to_wilted = Animator.StringToHash("middle_to_wilted");

		// Token: 0x04006584 RID: 25988
		private Flower.FlowerState currentState;

		// Token: 0x04006585 RID: 25989
		private string id;

		// Token: 0x04006586 RID: 25990
		private bool shouldUpdateVisuals;

		// Token: 0x04006587 RID: 25991
		private Flower.FlowerState lastState;

		// Token: 0x02000DDF RID: 3551
		public enum FlowerState
		{
			// Token: 0x0400658A RID: 25994
			None = -1,
			// Token: 0x0400658B RID: 25995
			Healthy,
			// Token: 0x0400658C RID: 25996
			Middle,
			// Token: 0x0400658D RID: 25997
			Wilted
		}
	}
}
