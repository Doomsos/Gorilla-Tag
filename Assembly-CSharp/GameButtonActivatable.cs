using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005FE RID: 1534
public class GameButtonActivatable : MonoBehaviour, IGameActivatable
{
	// Token: 0x060026BB RID: 9915 RVA: 0x000CDED4 File Offset: 0x000CC0D4
	public bool CheckInput(XRNode xrNode, float sensitivity = 0.25f)
	{
		switch (this.inputButton)
		{
		case GameButtonActivatable.InputButton.Trigger:
			return ControllerInputPoller.TriggerFloat(xrNode) > sensitivity;
		case GameButtonActivatable.InputButton.ButtonA:
			return ControllerInputPoller.PrimaryButtonPress(xrNode);
		case GameButtonActivatable.InputButton.ButtonB:
			return ControllerInputPoller.SecondaryButtonPress(xrNode);
		case GameButtonActivatable.InputButton.Grip:
			return ControllerInputPoller.GripFloat(xrNode) > sensitivity;
		case GameButtonActivatable.InputButton.Joystick:
			return ControllerInputPoller.TriggerFloat(xrNode) > sensitivity;
		default:
			return false;
		}
	}

	// Token: 0x060026BC RID: 9916 RVA: 0x000CDF34 File Offset: 0x000CC134
	public bool CheckInput(bool checkHeld = true, bool checkSnapped = true, float sensitivity = 0.25f, bool checkHeldActivatable = true, bool checkTriggerInteractable = true)
	{
		int num = -1;
		GamePlayer gamePlayer;
		if (checkHeld && GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		GamePlayer gamePlayer2;
		if (num == -1 && checkSnapped && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			num = gamePlayer2.FindSnapIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return false;
		}
		if (this.gameEntity.IsSnappedByLocalPlayer() && (checkHeldActivatable || checkTriggerInteractable))
		{
			GamePlayer gamePlayer3;
			bool flag = GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer3);
			if (flag && checkHeldActivatable)
			{
				GameEntity grabbedGameEntity = gamePlayer3.GetGrabbedGameEntity(num);
				if (grabbedGameEntity != null && grabbedGameEntity.GetComponent<IGameActivatable>() != null)
				{
					return false;
				}
			}
			if (flag && checkTriggerInteractable && this.inputButton == GameButtonActivatable.InputButton.Trigger && GameTriggerInteractable.LocalInteractableTriggers.Count > 0)
			{
				Vector3 position = GamePlayerLocal.instance.GetHandTransform(num).position;
				for (int i = 0; i < GameTriggerInteractable.LocalInteractableTriggers.Count; i++)
				{
					if (GameTriggerInteractable.LocalInteractableTriggers[i].PointWithinInteractableArea(position))
					{
						return false;
					}
				}
			}
		}
		XRNode xrNode = GamePlayer.IsLeftHand(num) ? 4 : 5;
		return this.CheckInput(xrNode, sensitivity);
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000CE06C File Offset: 0x000CC26C
	private float GetFloatInput(XRNode xrNode, float sensitivity = 0.25f)
	{
		float num;
		switch (this.inputButton)
		{
		case GameButtonActivatable.InputButton.Trigger:
			num = ControllerInputPoller.TriggerFloat(xrNode);
			break;
		case GameButtonActivatable.InputButton.ButtonA:
			num = (float)(ControllerInputPoller.PrimaryButtonPress(xrNode) ? 1 : 0);
			break;
		case GameButtonActivatable.InputButton.ButtonB:
			num = (float)(ControllerInputPoller.SecondaryButtonPress(xrNode) ? 1 : 0);
			break;
		case GameButtonActivatable.InputButton.Grip:
			num = ControllerInputPoller.GripFloat(xrNode);
			break;
		case GameButtonActivatable.InputButton.Joystick:
			num = ControllerInputPoller.TriggerFloat(xrNode);
			break;
		default:
			num = 0f;
			break;
		}
		float num2 = num;
		if (num2 < sensitivity)
		{
			return 0f;
		}
		return num2;
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000CE0EC File Offset: 0x000CC2EC
	public float GetFloatInput(bool checkHeld = true, bool checkSnapped = true, float sensitivity = 0.25f, bool checkHeldActivatable = true)
	{
		int num = -1;
		GamePlayer gamePlayer;
		if (checkHeld && GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		GamePlayer gamePlayer2;
		if (num == -1 && checkSnapped && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			num = gamePlayer2.FindSnapIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return 0f;
		}
		GamePlayer gamePlayer3;
		if (checkHeldActivatable && this.gameEntity.IsSnappedByLocalPlayer() && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer3))
		{
			GameEntity grabbedGameEntity = gamePlayer3.GetGrabbedGameEntity(num);
			if (grabbedGameEntity != null && grabbedGameEntity.GetComponent<IGameActivatable>() != null)
			{
				return 0f;
			}
		}
		XRNode xrNode = GamePlayer.IsLeftHand(num) ? 4 : 5;
		return this.GetFloatInput(xrNode, sensitivity);
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000CE1B9 File Offset: 0x000CC3B9
	protected bool IsEquippedLocal()
	{
		return this.gameEntity.IsHeldByLocalPlayer() || this.gameEntity.IsSnappedByLocalPlayer();
	}

	// Token: 0x04003289 RID: 12937
	[SerializeField]
	public GameButtonActivatable.InputButton inputButton;

	// Token: 0x0400328A RID: 12938
	public GameEntity gameEntity;

	// Token: 0x020005FF RID: 1535
	public enum InputButton
	{
		// Token: 0x0400328C RID: 12940
		Trigger,
		// Token: 0x0400328D RID: 12941
		ButtonA,
		// Token: 0x0400328E RID: 12942
		ButtonB,
		// Token: 0x0400328F RID: 12943
		Grip,
		// Token: 0x04003290 RID: 12944
		Joystick
	}
}
