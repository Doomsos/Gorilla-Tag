using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000544 RID: 1348
public class GameBallPlayerLocal : MonoBehaviour
{
	// Token: 0x06002200 RID: 8704 RVA: 0x000B1D60 File Offset: 0x000AFF60
	private void Awake()
	{
		GameBallPlayerLocal.instance = this;
		this.hands = new GameBallPlayerLocal.HandData[2];
		this.inputData = new GameBallPlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GameBallPlayerLocal.InputData(32);
		}
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x000B1DAF File Offset: 0x000AFFAF
	private void OnApplicationQuit()
	{
		MonkeBallGame.Instance.OnPlayerDestroy();
	}

	// Token: 0x06002202 RID: 8706 RVA: 0x000B1DBB File Offset: 0x000AFFBB
	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x000B1DAF File Offset: 0x000AFFAF
	private void OnDestroy()
	{
		MonkeBallGame.Instance.OnPlayerDestroy();
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x000B1DCC File Offset: 0x000AFFCC
	public void OnUpdateInteract()
	{
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(j);
		}
	}

	// Token: 0x06002205 RID: 8709 RVA: 0x000B1E18 File Offset: 0x000B0018
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GameBallPlayerLocal.InputDataMotion data = default(GameBallPlayerLocal.InputDataMotion);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.devicePosition, ref data.position);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, ref data.rotation);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceVelocity, ref data.velocity);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, ref data.angVelocity);
		data.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(data);
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x000B1EA4 File Offset: 0x000B00A4
	private void UpdateHand(int handIndex)
	{
		if (GameBallManager.Instance == null)
		{
			return;
		}
		if (!this.gamePlayer.GetGameBallId(handIndex).IsValid())
		{
			this.UpdateHandEmpty(handIndex);
			return;
		}
		this.UpdateHandHolding(handIndex);
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x000B1EE8 File Offset: 0x000B00E8
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = 0.0;
		this.hands[handIndex] = handData;
		this.UpdateStuckState();
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x000B1F25 File Offset: 0x000B0125
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x000B1F34 File Offset: 0x000B0134
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x000B1F5C File Offset: 0x000B015C
	private void UpdateStuckState()
	{
		bool disableMovement = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGameBallId(i).IsValid())
			{
				disableMovement = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = disableMovement;
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x000B1FA4 File Offset: 0x000B01A4
	private void UpdateHandEmpty(int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		bool flag = ControllerInputPoller.GripFloat(this.GetXRNode(handIndex)) > 0.7f;
		double timeAsDouble = Time.timeAsDouble;
		if (flag && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag;
		this.hands[handIndex] = handData;
		if (flag && num < 0.15000000596046448)
		{
			Vector3 position = this.GetHandTransform(handIndex).position;
			GameBallId gameBallId = GameBallManager.Instance.TryGrabLocal(position, this.gamePlayer.teamId);
			float num2 = 0.15f;
			if (gameBallId.IsValid())
			{
				bool flag2 = GameBallPlayerLocal.IsLeftHand(handIndex);
				BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
				object obj = flag2 ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
				GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
				Vector3 vector = gameBall.transform.position;
				Vector3 vector2 = gameBall.transform.position - position;
				if (vector2.sqrMagnitude > num2 * num2)
				{
					vector = position + vector2.normalized * num2;
				}
				object obj2 = obj;
				Vector3 localPosition = obj2.InverseTransformPoint(vector);
				Quaternion localRotation = Quaternion.Inverse(obj2.rotation) * gameBall.transform.rotation;
				obj2.InverseTransformPoint(gameBall.transform.position);
				GameBallManager.Instance.RequestGrabBall(gameBallId, flag2, localPosition, localRotation);
			}
		}
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x000B2130 File Offset: 0x000B0330
	private void UpdateHandHolding(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		if (ControllerInputPoller.GripFloat(xrnode) <= 0.7f)
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
			Vector3 vector;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, ref vector);
			Quaternion quaternion;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, ref quaternion);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
			GameBallPlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector2 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector2 = rotation * vector2;
			vector2 *= transform.localScale.x;
			vector = rotation * -(Quaternion.Inverse(quaternion) * vector);
			GameBallId gameBallId = this.gamePlayer.GetGameBallId(handIndex);
			GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
			if (gameBall == null)
			{
				return;
			}
			if (gameBall.IsLaunched)
			{
				return;
			}
			if (gameBall.disc)
			{
				Vector3 vector3 = gameBall.transform.rotation * gameBall.localDiscUp;
				vector3.Normalize();
				float num = Vector3.Dot(vector3, vector);
				vector = vector3 * num;
				vector *= 1.25f;
				vector2 *= 1.25f;
			}
			else
			{
				vector2 *= 1.5f;
			}
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector2 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			GameBallManager.Instance.RequestThrowBall(gameBallId, GameBallPlayerLocal.IsLeftHand(handIndex), vector2, vector);
		}
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x000B22E8 File Offset: 0x000B04E8
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return 5;
		}
		return 4;
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x000B22F0 File Offset: 0x000B04F0
	private Transform GetHandTransform(int handIndex)
	{
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		return ((handIndex == 0) ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform).parent;
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x000B1CAB File Offset: 0x000AFEAB
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x000B1CB1 File Offset: 0x000AFEB1
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x000B2323 File Offset: 0x000B0523
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x000B233F File Offset: 0x000B053F
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x04002CA1 RID: 11425
	public GameBallPlayer gamePlayer;

	// Token: 0x04002CA2 RID: 11426
	private const int MAX_INPUT_HISTORY = 32;

	// Token: 0x04002CA3 RID: 11427
	private GameBallPlayerLocal.HandData[] hands;

	// Token: 0x04002CA4 RID: 11428
	private GameBallPlayerLocal.InputData[] inputData;

	// Token: 0x04002CA5 RID: 11429
	[OnEnterPlay_SetNull]
	public static volatile GameBallPlayerLocal instance;

	// Token: 0x02000545 RID: 1349
	private enum HandGrabState
	{
		// Token: 0x04002CA7 RID: 11431
		Empty,
		// Token: 0x04002CA8 RID: 11432
		Holding
	}

	// Token: 0x02000546 RID: 1350
	private struct HandData
	{
		// Token: 0x04002CA9 RID: 11433
		public GameBallPlayerLocal.HandGrabState grabState;

		// Token: 0x04002CAA RID: 11434
		public bool gripWasHeld;

		// Token: 0x04002CAB RID: 11435
		public double gripPressedTime;

		// Token: 0x04002CAC RID: 11436
		public GameBallId grabbedGameBallId;
	}

	// Token: 0x02000547 RID: 1351
	public struct InputDataMotion
	{
		// Token: 0x04002CAD RID: 11437
		public double time;

		// Token: 0x04002CAE RID: 11438
		public Vector3 position;

		// Token: 0x04002CAF RID: 11439
		public Quaternion rotation;

		// Token: 0x04002CB0 RID: 11440
		public Vector3 velocity;

		// Token: 0x04002CB1 RID: 11441
		public Vector3 angVelocity;
	}

	// Token: 0x02000548 RID: 1352
	public class InputData
	{
		// Token: 0x06002214 RID: 8724 RVA: 0x000B2361 File Offset: 0x000B0561
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GameBallPlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x000B237C File Offset: 0x000B057C
		public void AddInput(GameBallPlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x000B23AC File Offset: 0x000B05AC
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
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

		// Token: 0x06002217 RID: 8727 RVA: 0x000B2428 File Offset: 0x000B0628
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 vector = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
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

		// Token: 0x04002CB2 RID: 11442
		public int maxInputs;

		// Token: 0x04002CB3 RID: 11443
		public List<GameBallPlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
