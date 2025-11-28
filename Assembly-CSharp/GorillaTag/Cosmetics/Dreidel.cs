using System;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010E7 RID: 4327
	public class Dreidel : MonoBehaviour
	{
		// Token: 0x06006C84 RID: 27780 RVA: 0x00239740 File Offset: 0x00237940
		public bool TrySetIdle()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface || this.state == Dreidel.State.Fallen)
			{
				this.StartIdle();
				return true;
			}
			return false;
		}

		// Token: 0x06006C85 RID: 27781 RVA: 0x00239765 File Offset: 0x00237965
		public bool TryCheckForSurfaces()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface)
			{
				this.StartFindingSurfaces();
				return true;
			}
			return false;
		}

		// Token: 0x06006C86 RID: 27782 RVA: 0x00239781 File Offset: 0x00237981
		public void Spin()
		{
			this.StartSpin();
		}

		// Token: 0x06006C87 RID: 27783 RVA: 0x0023978C File Offset: 0x0023798C
		public bool TryGetSpinStartData(out Vector3 surfacePoint, out Vector3 surfaceNormal, out float randomDuration, out Dreidel.Side randomSide, out Dreidel.Variation randomVariation, out double startTime)
		{
			if (this.canStartSpin)
			{
				surfacePoint = this.surfacePlanePoint;
				surfaceNormal = this.surfacePlaneNormal;
				randomDuration = Random.Range(this.spinTimeRange.x, this.spinTimeRange.y);
				randomSide = (Dreidel.Side)Random.Range(0, 4);
				randomVariation = (Dreidel.Variation)Random.Range(0, 5);
				startTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0);
				return true;
			}
			surfacePoint = Vector3.zero;
			surfaceNormal = Vector3.zero;
			randomDuration = 0f;
			randomSide = Dreidel.Side.Shin;
			randomVariation = Dreidel.Variation.Tumble;
			startTime = -1.0;
			return false;
		}

		// Token: 0x06006C88 RID: 27784 RVA: 0x00239838 File Offset: 0x00237A38
		public void SetSpinStartData(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			this.surfacePlanePoint = surfacePoint;
			this.surfacePlaneNormal = surfaceNormal;
			this.spinTime = duration;
			this.spinStartTime = startTime;
			this.spinCounterClockwise = counterClockwise;
			this.landingSide = side;
			this.landingVariation = variation;
		}

		// Token: 0x06006C89 RID: 27785 RVA: 0x00239870 File Offset: 0x00237A70
		private void LateUpdate()
		{
			float deltaTime = Time.deltaTime;
			double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			this.canStartSpin = false;
			switch (this.state)
			{
			default:
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			case Dreidel.State.FindingSurface:
			{
				float num2 = (GTPlayer.Instance != null) ? GTPlayer.Instance.scale : 1f;
				Vector3 down = Vector3.down;
				Vector3 vector = base.transform.parent.position - down * 2f * this.surfaceCheckDistance * num2;
				float num3 = (3f * this.surfaceCheckDistance + -this.bottomPointOffset.y) * num2;
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, down, ref raycastHit, num3, this.surfaceLayers.value, 1) && Vector3.Dot(raycastHit.normal, Vector3.up) > this.surfaceUprightThreshold && Vector3.Dot(raycastHit.normal, this.spinTransform.up) > this.surfaceDreidelAngleThreshold)
				{
					this.canStartSpin = true;
					this.surfacePlanePoint = raycastHit.point;
					this.surfacePlaneNormal = raycastHit.normal;
					this.AlignToSurfacePlane();
					this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
					this.UpdateSpinTransform();
					return;
				}
				this.canStartSpin = false;
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			}
			case Dreidel.State.Spinning:
			{
				float num4 = Mathf.Clamp01((float)(num - this.stateStartTime) / this.spinTime);
				this.spinSpeed = Mathf.Lerp(this.spinSpeedStart, this.spinSpeedEnd, num4);
				float num5 = this.spinCounterClockwise ? -1f : 1f;
				this.spinAngle += num5 * this.spinSpeed * 360f * deltaTime;
				float num6 = this.tiltWobble;
				float num7 = Mathf.Sin(this.spinWobbleFrequency * 2f * 3.1415927f * (float)(num - this.stateStartTime));
				float num8 = 0.5f * num7 + 0.5f;
				this.tiltWobble = Mathf.Lerp(this.spinWobbleAmplitudeEndMin * num4, this.spinWobbleAmplitude * num4, num8);
				if (this.landingTiltTarget.y == 0f)
				{
					if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltTarget.x) * this.tiltWobble;
					}
					else
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltLeadingTarget.x) * this.tiltWobble;
					}
				}
				else if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltTarget.y) * this.tiltWobble;
				}
				else
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltLeadingTarget.y) * this.tiltWobble;
				}
				float num9 = Mathf.Lerp(this.pathStartTurnRate, this.pathEndTurnRate, num4) + num7 * this.pathTurnRateSinOffset;
				if (this.spinCounterClockwise)
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num9 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				else
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num9 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				this.pathOffset += this.pathDir * this.pathMoveSpeed * deltaTime;
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				if (num4 - Mathf.Epsilon >= 1f && this.tiltWobble > 0.9f * this.spinWobbleAmplitude && num6 < this.tiltWobble)
				{
					this.StartFall();
					return;
				}
				break;
			}
			case Dreidel.State.Falling:
			{
				float num10 = this.fallTimeTumble;
				Dreidel.Variation variation = this.landingVariation;
				if (variation <= Dreidel.Variation.Smooth || variation - Dreidel.Variation.Bounce > 2)
				{
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num11 = this.spinCounterClockwise ? -1f : 1f;
					this.spinAngle += num11 * this.spinSpeed * 360f * deltaTime;
					float angularFrequency = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrontBackFrequency;
					float dampingRatio = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallFrontBackDampingRatio;
					float angularFrequency2 = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrequency;
					float dampingRatio2 = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallDampingRatio;
					this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, angularFrequency, dampingRatio, deltaTime);
					this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, angularFrequency2, dampingRatio2, deltaTime);
				}
				else
				{
					bool flag = this.landingVariation != Dreidel.Variation.Bounce;
					bool flag2 = this.landingVariation == Dreidel.Variation.FalseSlowTurn;
					float num12 = flag ? this.slowTurnSwitchTime : this.bounceFallSwitchTime;
					if (flag)
					{
						num10 = this.fallTimeSlowTurn;
					}
					if (num - this.stateStartTime < (double)num12)
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltLeadingTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
					}
					else
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						if (flag2)
						{
							if (!this.falseTargetReached && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.49f)
							{
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
							}
							else
							{
								this.falseTargetReached = true;
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
							}
						}
						else if (flag && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.45f)
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
						}
						else
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
						}
					}
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num13 = this.spinCounterClockwise ? -1f : 1f;
					this.spinAngle += num13 * this.spinSpeed * 360f * deltaTime;
				}
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				float num14 = (float)(num - this.stateStartTime);
				if (num14 > num10)
				{
					if (!this.hasLanded)
					{
						this.hasLanded = true;
						if (this.landingSide == Dreidel.Side.Gimel)
						{
							this.gimelConfetti.transform.position = this.spinTransform.position + Vector3.up * this.confettiHeight;
							this.gimelConfetti.gameObject.SetActive(true);
							this.audioSource.GTPlayOneShot(this.gimelConfettiSound, 1f);
						}
					}
					if (num14 > num10 + this.respawnTimeAfterLanding)
					{
						this.StartIdle();
					}
				}
				break;
			}
			case Dreidel.State.Fallen:
				break;
			}
		}

		// Token: 0x06006C8A RID: 27786 RVA: 0x0023A09C File Offset: 0x0023829C
		private void StartIdle()
		{
			this.state = Dreidel.State.Idle;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x06006C8B RID: 27787 RVA: 0x0023A178 File Offset: 0x00238378
		private void StartFindingSurfaces()
		{
			this.state = Dreidel.State.FindingSurface;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x06006C8C RID: 27788 RVA: 0x0023A254 File Offset: 0x00238454
		private void StartSpin()
		{
			this.state = Dreidel.State.Spinning;
			this.stateStartTime = ((this.spinStartTime > 0.0) ? this.spinStartTime : ((double)Time.time));
			this.canStartSpin = false;
			this.spinSpeed = this.spinSpeedStart;
			this.tiltWobble = 0f;
			this.audioSource.loop = true;
			this.audioSource.clip = this.spinLoopAudio;
			this.audioSource.GTPlay();
			this.gimelConfetti.gameObject.SetActive(false);
			this.AlignToSurfacePlane();
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
		}

		// Token: 0x06006C8D RID: 27789 RVA: 0x0023A31C File Offset: 0x0023851C
		private void StartFall()
		{
			this.state = Dreidel.State.Falling;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.falseTargetReached = false;
			this.hasLanded = false;
			if (this.landingVariation == Dreidel.Variation.FalseSlowTurn)
			{
				if (this.spinCounterClockwise)
				{
					this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
				else
				{
					this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
			}
			else if (this.spinCounterClockwise)
			{
				this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			else
			{
				this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			this.spinSpeedSpring.Reset(this.spinSpeed, 0f);
			this.tiltFrontBackSpring.Reset(this.tiltFrontBack, 0f);
			this.tiltLeftRightSpring.Reset(this.tiltLeftRight, 0f);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.audioSource.loop = false;
			this.audioSource.GTPlayOneShot(this.fallSound, 1f);
			this.gimelConfetti.gameObject.SetActive(false);
		}

		// Token: 0x06006C8E RID: 27790 RVA: 0x0023A46C File Offset: 0x0023866C
		private Vector3 GetGroundContactPoint()
		{
			Vector3 position = this.spinTransform.position;
			this.dreidelCollider.enabled = true;
			Vector3 vector = this.dreidelCollider.ClosestPoint(position - base.transform.up);
			this.dreidelCollider.enabled = false;
			float num = Vector3.Dot(vector - position, this.spinTransform.up);
			if (num > 0f)
			{
				vector -= num * this.spinTransform.up;
			}
			return this.spinTransform.InverseTransformPoint(vector);
		}

		// Token: 0x06006C8F RID: 27791 RVA: 0x0023A500 File Offset: 0x00238700
		private void GetTiltVectorsForSideWithPrev(Dreidel.Side side, out Vector2 sideTilt, out Vector2 prevSideTilt)
		{
			int num = (side <= Dreidel.Side.Shin) ? 3 : (side - Dreidel.Side.Hey);
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				prevSideTilt = this.landingTiltValues[num];
				prevSideTilt.x = sideTilt.x;
				return;
			}
			prevSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = prevSideTilt.x;
		}

		// Token: 0x06006C90 RID: 27792 RVA: 0x0023A584 File Offset: 0x00238784
		private void GetTiltVectorsForSideWithNext(Dreidel.Side side, out Vector2 sideTilt, out Vector2 nextSideTilt)
		{
			int num = (int)((side + 1) % Dreidel.Side.Count);
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				nextSideTilt = this.landingTiltValues[num];
				nextSideTilt.x = sideTilt.x;
				return;
			}
			nextSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = nextSideTilt.x;
		}

		// Token: 0x06006C91 RID: 27793 RVA: 0x0023A600 File Offset: 0x00238800
		private void AlignToSurfacePlane()
		{
			Vector3 vector = Vector3.forward;
			if (Vector3.Dot(Vector3.up, this.surfacePlaneNormal) < 0.9999f)
			{
				Vector3 vector2 = Vector3.Cross(this.surfacePlaneNormal, Vector3.up);
				vector = Quaternion.AngleAxis(90f, vector2) * this.surfacePlaneNormal;
			}
			Quaternion rotation = Quaternion.LookRotation(vector, this.surfacePlaneNormal);
			base.transform.position = this.surfacePlanePoint;
			base.transform.rotation = rotation;
		}

		// Token: 0x06006C92 RID: 27794 RVA: 0x0023A67C File Offset: 0x0023887C
		private void UpdateSpinTransform()
		{
			Vector3 position = this.spinTransform.position;
			Vector3 groundContactPoint = this.GetGroundContactPoint();
			Vector3 vector = this.groundPointSpring.TrackDampingRatio(groundContactPoint, this.groundTrackingFrequency, this.groundTrackingDampingRatio, Time.deltaTime);
			Vector3 vector2 = this.spinTransform.TransformPoint(vector);
			Quaternion quaternion = Quaternion.AngleAxis(90f * this.tiltLeftRight, Vector3.forward) * Quaternion.AngleAxis(90f * this.tiltFrontBack, Vector3.right);
			this.spinAxis = base.transform.InverseTransformDirection(base.transform.up);
			Quaternion quaternion2 = Quaternion.AngleAxis(this.spinAngle, this.spinAxis);
			this.spinTransform.localRotation = quaternion2 * quaternion;
			Vector3 vector3 = base.transform.InverseTransformVector(Vector3.Dot(position - vector2, base.transform.up) * base.transform.up);
			this.spinTransform.localPosition = vector3 + this.pathOffset;
			this.spinTransform.TransformPoint(this.bottomPointOffset);
		}

		// Token: 0x04007D27 RID: 32039
		[Header("References")]
		[SerializeField]
		private Transform spinTransform;

		// Token: 0x04007D28 RID: 32040
		[SerializeField]
		private MeshCollider dreidelCollider;

		// Token: 0x04007D29 RID: 32041
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007D2A RID: 32042
		[SerializeField]
		private AudioClip spinLoopAudio;

		// Token: 0x04007D2B RID: 32043
		[SerializeField]
		private AudioClip fallSound;

		// Token: 0x04007D2C RID: 32044
		[SerializeField]
		private AudioClip gimelConfettiSound;

		// Token: 0x04007D2D RID: 32045
		[SerializeField]
		private ParticleSystem gimelConfetti;

		// Token: 0x04007D2E RID: 32046
		[Header("Offsets")]
		[SerializeField]
		private Vector3 centerOfMassOffset = Vector3.zero;

		// Token: 0x04007D2F RID: 32047
		[SerializeField]
		private Vector3 bottomPointOffset = Vector3.zero;

		// Token: 0x04007D30 RID: 32048
		[SerializeField]
		private Vector2 bodyRect = Vector2.one;

		// Token: 0x04007D31 RID: 32049
		[SerializeField]
		private float confettiHeight = 0.125f;

		// Token: 0x04007D32 RID: 32050
		[Header("Surface Detection")]
		[SerializeField]
		private float surfaceCheckDistance = 0.15f;

		// Token: 0x04007D33 RID: 32051
		[SerializeField]
		private float surfaceUprightThreshold = 0.5f;

		// Token: 0x04007D34 RID: 32052
		[SerializeField]
		private float surfaceDreidelAngleThreshold = 0.9f;

		// Token: 0x04007D35 RID: 32053
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x04007D36 RID: 32054
		[Header("Spin Paramss")]
		[SerializeField]
		private float spinSpeedStart = 2f;

		// Token: 0x04007D37 RID: 32055
		[SerializeField]
		private float spinSpeedEnd = 1f;

		// Token: 0x04007D38 RID: 32056
		[SerializeField]
		private float spinTime = 10f;

		// Token: 0x04007D39 RID: 32057
		[SerializeField]
		private Vector2 spinTimeRange = new Vector2(7f, 12f);

		// Token: 0x04007D3A RID: 32058
		[SerializeField]
		private float spinWobbleFrequency = 0.1f;

		// Token: 0x04007D3B RID: 32059
		[SerializeField]
		private float spinWobbleAmplitude = 0.01f;

		// Token: 0x04007D3C RID: 32060
		[SerializeField]
		private float spinWobbleAmplitudeEndMin = 0.01f;

		// Token: 0x04007D3D RID: 32061
		[SerializeField]
		private float tiltFrontBack;

		// Token: 0x04007D3E RID: 32062
		[SerializeField]
		private float tiltLeftRight;

		// Token: 0x04007D3F RID: 32063
		[SerializeField]
		private float groundTrackingDampingRatio = 0.9f;

		// Token: 0x04007D40 RID: 32064
		[SerializeField]
		private float groundTrackingFrequency = 1f;

		// Token: 0x04007D41 RID: 32065
		[Header("Motion Path")]
		[SerializeField]
		private float pathMoveSpeed = 0.1f;

		// Token: 0x04007D42 RID: 32066
		[SerializeField]
		private float pathStartTurnRate = 360f;

		// Token: 0x04007D43 RID: 32067
		[SerializeField]
		private float pathEndTurnRate = 90f;

		// Token: 0x04007D44 RID: 32068
		[SerializeField]
		private float pathTurnRateSinOffset = 180f;

		// Token: 0x04007D45 RID: 32069
		[Header("Falling Params")]
		[SerializeField]
		private float spinSpeedStopRate = 1f;

		// Token: 0x04007D46 RID: 32070
		[SerializeField]
		private float tumbleFallDampingRatio = 0.4f;

		// Token: 0x04007D47 RID: 32071
		[SerializeField]
		private float tumbleFallFrequency = 6f;

		// Token: 0x04007D48 RID: 32072
		[SerializeField]
		private float tumbleFallFrontBackDampingRatio = 0.4f;

		// Token: 0x04007D49 RID: 32073
		[SerializeField]
		private float tumbleFallFrontBackFrequency = 6f;

		// Token: 0x04007D4A RID: 32074
		[SerializeField]
		private float smoothFallDampingRatio = 0.9f;

		// Token: 0x04007D4B RID: 32075
		[SerializeField]
		private float smoothFallFrequency = 2f;

		// Token: 0x04007D4C RID: 32076
		[SerializeField]
		private float slowTurnDampingRatio = 0.9f;

		// Token: 0x04007D4D RID: 32077
		[SerializeField]
		private float slowTurnFrequency = 2f;

		// Token: 0x04007D4E RID: 32078
		[SerializeField]
		private float bounceFallSwitchTime = 0.5f;

		// Token: 0x04007D4F RID: 32079
		[SerializeField]
		private float slowTurnSwitchTime = 0.5f;

		// Token: 0x04007D50 RID: 32080
		[SerializeField]
		private float respawnTimeAfterLanding = 3f;

		// Token: 0x04007D51 RID: 32081
		[SerializeField]
		private float fallTimeTumble = 3f;

		// Token: 0x04007D52 RID: 32082
		[SerializeField]
		private float fallTimeSlowTurn = 5f;

		// Token: 0x04007D53 RID: 32083
		private Dreidel.State state;

		// Token: 0x04007D54 RID: 32084
		private double stateStartTime;

		// Token: 0x04007D55 RID: 32085
		private float spinSpeed;

		// Token: 0x04007D56 RID: 32086
		private float spinAngle;

		// Token: 0x04007D57 RID: 32087
		private Vector3 spinAxis = Vector3.up;

		// Token: 0x04007D58 RID: 32088
		private bool canStartSpin;

		// Token: 0x04007D59 RID: 32089
		private double spinStartTime = -1.0;

		// Token: 0x04007D5A RID: 32090
		private float tiltWobble;

		// Token: 0x04007D5B RID: 32091
		private bool falseTargetReached;

		// Token: 0x04007D5C RID: 32092
		private bool hasLanded;

		// Token: 0x04007D5D RID: 32093
		private Vector3 pathOffset = Vector3.zero;

		// Token: 0x04007D5E RID: 32094
		private Vector3 pathDir = Vector3.forward;

		// Token: 0x04007D5F RID: 32095
		private Vector3 surfacePlanePoint;

		// Token: 0x04007D60 RID: 32096
		private Vector3 surfacePlaneNormal;

		// Token: 0x04007D61 RID: 32097
		private FloatSpring tiltFrontBackSpring;

		// Token: 0x04007D62 RID: 32098
		private FloatSpring tiltLeftRightSpring;

		// Token: 0x04007D63 RID: 32099
		private FloatSpring spinSpeedSpring;

		// Token: 0x04007D64 RID: 32100
		private Vector3Spring groundPointSpring;

		// Token: 0x04007D65 RID: 32101
		private Vector2[] landingTiltValues = new Vector2[]
		{
			new Vector2(1f, -1f),
			new Vector2(1f, 0f),
			new Vector2(-1f, 1f),
			new Vector2(-1f, 0f)
		};

		// Token: 0x04007D66 RID: 32102
		private Vector2 landingTiltLeadingTarget = Vector2.zero;

		// Token: 0x04007D67 RID: 32103
		private Vector2 landingTiltTarget = Vector2.zero;

		// Token: 0x04007D68 RID: 32104
		[Header("Debug Params")]
		[SerializeField]
		private Dreidel.Side landingSide;

		// Token: 0x04007D69 RID: 32105
		[SerializeField]
		private Dreidel.Variation landingVariation;

		// Token: 0x04007D6A RID: 32106
		[SerializeField]
		private bool spinCounterClockwise;

		// Token: 0x04007D6B RID: 32107
		[SerializeField]
		private bool debugDraw;

		// Token: 0x020010E8 RID: 4328
		private enum State
		{
			// Token: 0x04007D6D RID: 32109
			Idle,
			// Token: 0x04007D6E RID: 32110
			FindingSurface,
			// Token: 0x04007D6F RID: 32111
			Spinning,
			// Token: 0x04007D70 RID: 32112
			Falling,
			// Token: 0x04007D71 RID: 32113
			Fallen
		}

		// Token: 0x020010E9 RID: 4329
		public enum Side
		{
			// Token: 0x04007D73 RID: 32115
			Shin,
			// Token: 0x04007D74 RID: 32116
			Hey,
			// Token: 0x04007D75 RID: 32117
			Gimel,
			// Token: 0x04007D76 RID: 32118
			Nun,
			// Token: 0x04007D77 RID: 32119
			Count
		}

		// Token: 0x020010EA RID: 4330
		public enum Variation
		{
			// Token: 0x04007D79 RID: 32121
			Tumble,
			// Token: 0x04007D7A RID: 32122
			Smooth,
			// Token: 0x04007D7B RID: 32123
			Bounce,
			// Token: 0x04007D7C RID: 32124
			SlowTurn,
			// Token: 0x04007D7D RID: 32125
			FalseSlowTurn,
			// Token: 0x04007D7E RID: 32126
			Count
		}
	}
}
