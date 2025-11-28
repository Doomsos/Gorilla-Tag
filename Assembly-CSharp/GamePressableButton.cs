using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000630 RID: 1584
public class GamePressableButton : MonoBehaviour, IClickable
{
	// Token: 0x0600286F RID: 10351 RVA: 0x000D7200 File Offset: 0x000D5400
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x000D720C File Offset: 0x000D540C
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = collider.gameObject.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (!component)
		{
			return;
		}
		if (!this.CheckValidEquippedState(component.isLeftHand))
		{
			return;
		}
		this.PressButton(component.isLeftHand);
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x000D7268 File Offset: 0x000D5468
	private bool CheckValidEquippedState(bool pressedHandLeft)
	{
		if (!this.requireEquipped)
		{
			return true;
		}
		int num = -1;
		GamePlayer gamePlayer;
		if (this.gameEntity.IsHeldByLocalPlayer() && this.activeWhileGrabbed && GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		GamePlayer gamePlayer2;
		if (num == -1 && this.gameEntity.IsSnappedByLocalPlayer() && this.activeWhileSnapped && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			num = gamePlayer2.FindSnapIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return false;
		}
		bool flag = GamePlayer.IsLeftHand(num);
		return pressedHandLeft != flag;
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000D7310 File Offset: 0x000D5510
	private void PressButton(bool isLeftHand)
	{
		this.touchTime = Time.time;
		UnityEvent unityEvent = this.onPressButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
			{
				67,
				isLeftHand,
				0.05f
			});
		}
	}

	// Token: 0x040033CE RID: 13262
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x040033CF RID: 13263
	[SerializeField]
	private bool requireEquipped;

	// Token: 0x040033D0 RID: 13264
	[SerializeField]
	private bool activeWhileGrabbed;

	// Token: 0x040033D1 RID: 13265
	[SerializeField]
	private bool activeWhileSnapped;

	// Token: 0x040033D2 RID: 13266
	public UnityEvent onPressButton;

	// Token: 0x040033D3 RID: 13267
	[Header("Button Press")]
	public float debounceTime = 0.25f;

	// Token: 0x040033D4 RID: 13268
	public int pressButtonSoundIndex = 67;

	// Token: 0x040033D5 RID: 13269
	private float touchTime;
}
