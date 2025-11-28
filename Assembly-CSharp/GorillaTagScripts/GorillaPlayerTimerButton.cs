using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DEE RID: 3566
	public class GorillaPlayerTimerButton : MonoBehaviour
	{
		// Token: 0x060058EB RID: 22763 RVA: 0x001C78C4 File Offset: 0x001C5AC4
		private void Awake()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x060058EC RID: 22764 RVA: 0x001C78D1 File Offset: 0x001C5AD1
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x060058ED RID: 22765 RVA: 0x001C78D1 File Offset: 0x001C5AD1
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x060058EE RID: 22766 RVA: 0x001C78DC File Offset: 0x001C5ADC
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBothStartAndStop)
			{
				this.isStartButton = !PlayerTimerManager.instance.IsLocalTimerStarted();
			}
			this.isInitialized = true;
		}

		// Token: 0x060058EF RID: 22767 RVA: 0x001C7958 File Offset: 0x001C5B58
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
		}

		// Token: 0x060058F0 RID: 22768 RVA: 0x001C79AF File Offset: 0x001C5BAF
		private void OnLocalTimerStarted()
		{
			if (this.isBothStartAndStop)
			{
				this.isStartButton = false;
			}
		}

		// Token: 0x060058F1 RID: 22769 RVA: 0x001C79C0 File Offset: 0x001C5BC0
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isBothStartAndStop && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStartButton = true;
			}
		}

		// Token: 0x060058F2 RID: 22770 RVA: 0x001C79E4 File Offset: 0x001C5BE4
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (Time.time < this.lastTriggeredTime + this.debounceTime)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, this.pressColor);
			this.mesh.SetPropertyBlock(this.materialProps);
			PlayerTimerManager.instance.RequestTimerToggle(this.isStartButton);
			this.lastTriggeredTime = Time.time;
		}

		// Token: 0x060058F3 RID: 22771 RVA: 0x001C7AA4 File Offset: 0x001C5CA4
		private void OnTriggerExit(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			if (other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
			{
				return;
			}
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, this.notPressedColor);
			this.mesh.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x04006613 RID: 26131
		private float lastTriggeredTime;

		// Token: 0x04006614 RID: 26132
		[SerializeField]
		private bool isStartButton;

		// Token: 0x04006615 RID: 26133
		[SerializeField]
		private bool isBothStartAndStop;

		// Token: 0x04006616 RID: 26134
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x04006617 RID: 26135
		[SerializeField]
		private MeshRenderer mesh;

		// Token: 0x04006618 RID: 26136
		[SerializeField]
		private Color pressColor;

		// Token: 0x04006619 RID: 26137
		[SerializeField]
		private Color notPressedColor;

		// Token: 0x0400661A RID: 26138
		private MaterialPropertyBlock materialProps;

		// Token: 0x0400661B RID: 26139
		private bool isInitialized;
	}
}
