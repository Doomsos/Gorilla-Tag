using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200062B RID: 1579
public class GamePlayerLocal : MonoBehaviour
{
	// Token: 0x06002851 RID: 10321 RVA: 0x000D670C File Offset: 0x000D490C
	private void Awake()
	{
		GamePlayerLocal.instance = this;
		this.hands = new GamePlayerLocal.HandData[2];
		this.inputData = new GamePlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GamePlayerLocal.InputData(32);
		}
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinRoom);
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x000D6776 File Offset: 0x000D4976
	private void OnJoinRoom()
	{
		this.gamePlayer.MigrateHeldActorNumbers();
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x000D6784 File Offset: 0x000D4984
	public void OnUpdateInteract()
	{
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(this.currGameEntityManager, j);
		}
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x000D67CC File Offset: 0x000D49CC
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GamePlayerLocal.InputDataMotion data = default(GamePlayerLocal.InputDataMotion);
		data.position = ControllerInputPoller.DevicePosition(xrnode);
		data.rotation = ControllerInputPoller.DeviceRotation(xrnode);
		data.velocity = ControllerInputPoller.DeviceVelocity(xrnode);
		data.angVelocity = ControllerInputPoller.DeviceAngularVelocity(xrnode);
		data.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(data);
	}

	// Token: 0x06002855 RID: 10325 RVA: 0x000D6838 File Offset: 0x000D4A38
	private void UpdateHand(GameEntityManager emptyHandManager, int handIndex)
	{
		GameEntityManager gameEntityManager;
		if (!this.gamePlayer.GetGrabbedGameEntityIdAndManager(handIndex, out gameEntityManager).IsValid())
		{
			this.UpdateHandEmpty(emptyHandManager, handIndex);
			return;
		}
		this.UpdateHandHolding(gameEntityManager, handIndex);
	}

	// Token: 0x06002856 RID: 10326 RVA: 0x000D686E File Offset: 0x000D4A6E
	public void MigrateToEntityManager(GameEntityManager newEntityManager)
	{
		if (this.currGameEntityManager == newEntityManager)
		{
			return;
		}
		if (newEntityManager.IsAuthority())
		{
			this.gamePlayer.MigrateToEntityManager(newEntityManager);
		}
		this.currGameEntityManager = newEntityManager;
	}

	// Token: 0x06002857 RID: 10327 RVA: 0x000D689C File Offset: 0x000D4A9C
	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = (gameBallId.IsValid() ? 0.0 : handData.gripPressedTime);
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
		if (handIndex == 0)
		{
			EquipmentInteractor.instance.disableLeftGrab = gameBallId.IsValid();
			return;
		}
		if (handIndex == 1)
		{
			EquipmentInteractor.instance.disableRightGrab = gameBallId.IsValid();
		}
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x000D691C File Offset: 0x000D4B1C
	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x000D6955 File Offset: 0x000D4B55
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex);
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x000D6964 File Offset: 0x000D4B64
	private void UpdateStuckState()
	{
		bool disableMovement = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGrabbedGameEntityId(i).IsValid())
			{
				disableMovement = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = disableMovement;
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x000D69AC File Offset: 0x000D4BAC
	private void UpdateHandEmpty(GameEntityManager gameEntityManager, int handIndex)
	{
		if (this.gamePlayer.IsGrabbingDisabled())
		{
			return;
		}
		if (gameEntityManager == null)
		{
			return;
		}
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		bool flag = GamePlayerLocal.IsLeftHand(handIndex) ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(4)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(5));
		double timeAsDouble = Time.timeAsDouble;
		if (flag && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag;
		bool flag2 = GamePlayerLocal.IsLeftHand(handIndex) ? ControllerInputPoller.GetIndexPressed(4) : ControllerInputPoller.GetIndexPressed(5);
		if (flag2 && !handData.gripWasHeld)
		{
			handData.triggerPressedTime = timeAsDouble;
		}
		double num2 = timeAsDouble - handData.triggerPressedTime;
		handData.triggerWasHeld = flag2;
		this.hands[handIndex] = handData;
		if (flag && num < 0.15000000596046448)
		{
			Transform handTransform = this.GetHandTransform(handIndex);
			Vector3 position = handTransform.position;
			Vector3 vector = position;
			Quaternion rotation = handTransform.rotation;
			bool isLeftHand = GamePlayerLocal.IsLeftHand(handIndex);
			GameEntityId gameEntityId = gameEntityManager.TryGrabLocal(position, isLeftHand, out vector);
			if (gameEntityId.IsValid())
			{
				Transform handTransform2 = this.GetHandTransform(handIndex);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(gameEntityId);
				Vector3 vector2 = gameEntity.transform.position + (position - vector);
				Quaternion rotation2 = gameEntity.transform.rotation;
				GameGrabbable component = gameEntity.GetComponent<GameGrabbable>();
				GameGrab gameGrab;
				if (component && component.GetBestGrabPoint(position, rotation, handIndex, out gameGrab))
				{
					vector2 = gameGrab.position;
					rotation2 = gameGrab.rotation;
				}
				Vector3 localPosition = handTransform2.InverseTransformPoint(vector2);
				Quaternion localRotation = Quaternion.Inverse(handTransform2.rotation) * rotation2;
				gameEntityManager.RequestGrabEntity(gameEntityId, isLeftHand, localPosition, localRotation);
			}
		}
		if (flag2 && num2 < 0.15000000596046448)
		{
			Vector3 position2 = this.GetHandTransform(handIndex).position;
			GameTriggerInteractable gameTriggerInteractable = null;
			float num3 = float.MaxValue;
			int num4 = 0;
			while (num4 < GameTriggerInteractable.LocalInteractableTriggers.Count && !GameTriggerInteractable.LocalInteractableTriggers[num4].triggerInteractionActive)
			{
				if (GameTriggerInteractable.LocalInteractableTriggers[num4].PointWithinInteractableArea(position2))
				{
					float magnitude = (GameTriggerInteractable.LocalInteractableTriggers[num4].interactableCenter.position - position2).magnitude;
					if (magnitude <= num3)
					{
						num3 = magnitude;
						gameTriggerInteractable = GameTriggerInteractable.LocalInteractableTriggers[num4];
					}
				}
				num4++;
			}
			if (gameTriggerInteractable != null)
			{
				gameTriggerInteractable.BeginTriggerInteraction(handIndex);
			}
		}
		if (!flag2)
		{
			this.ClearTriggerInteractables(handIndex);
		}
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x000D6C40 File Offset: 0x000D4E40
	private void UpdateHandHolding(GameEntityManager gameEntityManager, int handIndex)
	{
		if (gameEntityManager == null)
		{
			return;
		}
		XRNode xrnode = this.GetXRNode(handIndex);
		if (!(GamePlayerLocal.IsLeftHand(handIndex) ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(4)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(5))))
		{
			GameEntityId grabbedGameEntityId = this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(grabbedGameEntityId);
			GameSnappable component = gameEntity.GetComponent<GameSnappable>();
			if (component != null)
			{
				SuperInfectionSnapPoint superInfectionSnapPoint = component.BestSnapPoint();
				if (superInfectionSnapPoint != null)
				{
					gameEntityManager.RequestSnapEntity(grabbedGameEntityId, GamePlayerLocal.IsLeftHand(handIndex), superInfectionSnapPoint.jointType);
					return;
				}
			}
			GameDockable component2 = gameEntity.GetComponent<GameDockable>();
			if (component2 != null)
			{
				GameEntityId gameEntityId = component2.BestDock();
				if (gameEntityId != GameEntityId.Invalid)
				{
					Transform dockablePoint = component2.GetDockablePoint();
					Quaternion quaternion = Quaternion.Inverse(Quaternion.Inverse(component2.transform.rotation) * dockablePoint.rotation);
					Vector3 vector = quaternion * -component2.transform.InverseTransformPoint(dockablePoint.position);
					GameEntity gameEntity2 = gameEntityManager.GetGameEntity(gameEntityId);
					if (gameEntity2 != null)
					{
						GameDock component3 = gameEntity2.GetComponent<GameDock>();
						if (component3 != null)
						{
							Transform dockMarker = component3.dockMarker;
							Vector3 vector2 = dockMarker.transform.TransformPoint(vector);
							vector = gameEntity2.transform.InverseTransformPoint(vector2);
							Quaternion quaternion2 = dockMarker.rotation * quaternion;
							quaternion = Quaternion.Inverse(gameEntity2.transform.rotation) * quaternion2;
						}
					}
					gameEntityManager.RequestAttachEntity(grabbedGameEntityId, gameEntityId, 0, vector, quaternion);
					return;
				}
			}
			Vector3 vector3 = ControllerInputPoller.DeviceAngularVelocity(xrnode);
			Quaternion quaternion3 = ControllerInputPoller.DeviceRotation(xrnode);
			Quaternion handRotOffset = GTPlayer.Instance.GetHandRotOffset(GamePlayerLocal.IsLeftHand(handIndex));
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
			GamePlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector4 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector4 = rotation * vector4;
			vector4 *= transform.localScale.x;
			vector3 = rotation * quaternion3 * handRotOffset * vector3;
			this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector4 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			gameEntityManager.RequestThrowEntity(grabbedGameEntityId, GamePlayerLocal.IsLeftHand(handIndex), GTPlayer.Instance.HeadCenterPosition, vector4, vector3);
		}
		this.ClearTriggerInteractables(handIndex);
	}

	// Token: 0x0600285D RID: 10333 RVA: 0x000B22C8 File Offset: 0x000B04C8
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return 5;
		}
		return 4;
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x000D6EF4 File Offset: 0x000D50F4
	public Transform GetHandTransform(int handIndex)
	{
		return GamePlayer.GetHandTransform(GorillaTagger.Instance.offlineVRRig, handIndex);
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000D6F08 File Offset: 0x000D5108
	private Transform GetFingerTransform(int handIndex)
	{
		GorillaTagger gorillaTagger = GorillaTagger.Instance;
		Transform result;
		if (handIndex != 0)
		{
			if (handIndex != 1)
			{
				result = null;
			}
			else
			{
				result = gorillaTagger.rightHandTriggerCollider.transform;
			}
		}
		else
		{
			result = gorillaTagger.leftHandTriggerCollider.transform;
		}
		return result;
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000D6F44 File Offset: 0x000D5144
	public Vector3 GetHandVelocity(int handIndex)
	{
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		GamePlayerLocal.InputData inputData = this.inputData[handIndex];
		Vector3 vector = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
		vector = rotation * vector;
		return vector * base.transform.localScale.x;
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x000D6FBC File Offset: 0x000D51BC
	public Vector3 GetHandAngularVelocity(int handIndex)
	{
		object obj = (handIndex == 0) ? 4 : 5;
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		object node = obj;
		Quaternion quaternion = ControllerInputPoller.DeviceRotation(node);
		Vector3 vector = ControllerInputPoller.DeviceAngularVelocity(node);
		return rotation * -(Quaternion.Inverse(quaternion) * vector);
	}

	// Token: 0x06002862 RID: 10338 RVA: 0x000D700B File Offset: 0x000D520B
	public float GetHandSpeed(int handIndex)
	{
		return this.inputData[handIndex].GetMaxSpeed(0f, 0.05f);
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x000B1C8B File Offset: 0x000AFE8B
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x000B1C91 File Offset: 0x000AFE91
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x000D7024 File Offset: 0x000D5224
	public static bool IsHandHolding(int handIndex)
	{
		GameEntityManager gameEntityManager;
		return GamePlayerLocal.instance.gamePlayer.GetGrabbedGameEntityIdAndManager(handIndex, out gameEntityManager).IsValid();
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x000D704D File Offset: 0x000D524D
	public static bool IsHandHolding(XRNode xrNode)
	{
		return GamePlayerLocal.IsHandHolding((xrNode == 4) ? 0 : 1);
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000B2303 File Offset: 0x000B0503
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x000B231F File Offset: 0x000B051F
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x000D705C File Offset: 0x000D525C
	public void ClearTriggerInteractables(int handIndex)
	{
		for (int i = 0; i < GameTriggerInteractable.LocalInteractableTriggers.Count; i++)
		{
			if (GameTriggerInteractable.LocalInteractableTriggers[i].triggerInteractionActive && GameTriggerInteractable.LocalInteractableTriggers[i].handIndex == handIndex)
			{
				GameTriggerInteractable.LocalInteractableTriggers[i].EndTriggerInteraction();
			}
		}
	}

	// Token: 0x040033B8 RID: 13240
	public GamePlayer gamePlayer;

	// Token: 0x040033B9 RID: 13241
	private GamePlayerLocal.HandData[] hands;

	// Token: 0x040033BA RID: 13242
	public const int MAX_INPUT_HISTORY = 32;

	// Token: 0x040033BB RID: 13243
	private GamePlayerLocal.InputData[] inputData;

	// Token: 0x040033BC RID: 13244
	[OnEnterPlay_SetNull]
	public static volatile GamePlayerLocal instance;

	// Token: 0x040033BD RID: 13245
	[NonSerialized]
	public GameEntityManager currGameEntityManager;

	// Token: 0x0200062C RID: 1580
	private enum HandGrabState
	{
		// Token: 0x040033BF RID: 13247
		Empty,
		// Token: 0x040033C0 RID: 13248
		Holding
	}

	// Token: 0x0200062D RID: 1581
	private struct HandData
	{
		// Token: 0x040033C1 RID: 13249
		public GamePlayerLocal.HandGrabState grabState;

		// Token: 0x040033C2 RID: 13250
		public bool gripWasHeld;

		// Token: 0x040033C3 RID: 13251
		public bool triggerWasHeld;

		// Token: 0x040033C4 RID: 13252
		public double gripPressedTime;

		// Token: 0x040033C5 RID: 13253
		public double triggerPressedTime;

		// Token: 0x040033C6 RID: 13254
		public GameEntityId grabbedGameBallId;
	}

	// Token: 0x0200062E RID: 1582
	public struct InputDataMotion
	{
		// Token: 0x040033C7 RID: 13255
		public double time;

		// Token: 0x040033C8 RID: 13256
		public Vector3 position;

		// Token: 0x040033C9 RID: 13257
		public Quaternion rotation;

		// Token: 0x040033CA RID: 13258
		public Vector3 velocity;

		// Token: 0x040033CB RID: 13259
		public Vector3 angVelocity;
	}

	// Token: 0x0200062F RID: 1583
	public class InputData
	{
		// Token: 0x0600286B RID: 10347 RVA: 0x000D70B3 File Offset: 0x000D52B3
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GamePlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x0600286C RID: 10348 RVA: 0x000D70CE File Offset: 0x000D52CE
		public void AddInput(GamePlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x0600286D RID: 10349 RVA: 0x000D70FC File Offset: 0x000D52FC
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					float sqrMagnitude = inputDataMotion.velocity.sqrMagnitude;
					if (sqrMagnitude > num3)
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return Mathf.Sqrt(num3);
		}

		// Token: 0x0600286E RID: 10350 RVA: 0x000D7178 File Offset: 0x000D5378
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 vector = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					vector += inputDataMotion.velocity;
					num3++;
				}
			}
			if (num3 == 0)
			{
				return Vector3.zero;
			}
			return vector / (float)num3;
		}

		// Token: 0x040033CC RID: 13260
		public int maxInputs;

		// Token: 0x040033CD RID: 13261
		public List<GamePlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
