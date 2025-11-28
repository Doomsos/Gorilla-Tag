using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001095 RID: 4245
	public class DicePhysics : MonoBehaviour
	{
		// Token: 0x06006A39 RID: 27193 RVA: 0x0022A990 File Offset: 0x00228B90
		public int GetRandomSide()
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				if (this.forceLandingSide)
				{
					return Mathf.Clamp(this.forcedLandingSide, 1, 20);
				}
				int num;
				if (this.CheckCosmeticRollOverride(out num))
				{
					return Mathf.Clamp(num, 1, 20);
				}
				return Random.Range(1, 21);
			}
			else
			{
				if (this.forceLandingSide)
				{
					return Mathf.Clamp(this.forcedLandingSide, 1, 6);
				}
				int num2;
				if (this.CheckCosmeticRollOverride(out num2))
				{
					return Mathf.Clamp(num2, 1, 6);
				}
				return Random.Range(1, 7);
			}
		}

		// Token: 0x06006A3A RID: 27194 RVA: 0x0022AA10 File Offset: 0x00228C10
		public Vector3 GetSideDirection(int side)
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				int num = Mathf.Clamp(side - 1, 0, 19);
				return this.d20SideDirections[num];
			}
			int num2 = Mathf.Clamp(side - 1, 0, 5);
			return this.d6SideDirections[num2];
		}

		// Token: 0x06006A3B RID: 27195 RVA: 0x0022AA5C File Offset: 0x00228C5C
		public void StartThrow(DiceHoldable holdable, Vector3 startPosition, Vector3 velocity, float playerScale, int side, double startTime)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.linearVelocity = velocity;
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.throwStartTime = ((startTime > 0.0) ? startTime : ((double)Time.time));
			this.throwSettledTime = -1.0;
			this.scale = playerScale;
			this.landingSide = Mathf.Clamp(side, 1, 20);
			this.prevVelocity = Vector3.zero;
			velocity = Vector3.zero;
			base.enabled = true;
		}

		// Token: 0x06006A3C RID: 27196 RVA: 0x0022AB44 File Offset: 0x00228D44
		public void EndThrow()
		{
			this.rb.isKinematic = true;
			this.rb.linearVelocity = Vector3.zero;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.throwStartTime = -1.0;
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			this.onRollFinished.Invoke();
			base.enabled = false;
		}

		// Token: 0x06006A3D RID: 27197 RVA: 0x0022AC08 File Offset: 0x00228E08
		private void FixedUpdate()
		{
			double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			float num2 = (float)(num - this.throwStartTime);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, ref raycastHit, 0.1f * this.scale, this.surfaceLayers.value, 1))
			{
				Vector3 normal = raycastHit.normal;
				Vector3 sideDirection = this.GetSideDirection(this.landingSide);
				Vector3 vector = base.transform.rotation * sideDirection;
				Vector3 normalized = Vector3.Cross(vector, normal).normalized;
				float num3 = Vector3.SignedAngle(vector, normal, normalized);
				float num4 = Mathf.Sign(num3) * this.angleDeltaVsStrengthCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(num3) / 180f));
				float num5 = this.landingTimeVsStrengthCurve.Evaluate(Mathf.Clamp01(num2 / this.landingTime));
				float magnitude = this.rb.linearVelocity.magnitude;
				float num6 = Mathf.Clamp01(1f - Mathf.Min(magnitude, 1f));
				float num7 = Mathf.Max(num5, num6);
				Vector3 vector2 = this.strength * (num7 * num4 * normalized) - this.damping * this.rb.angularVelocity;
				this.rb.AddTorque(vector2, 5);
				if (!this.rb.isKinematic && magnitude < 0.01f && num3 < 2f)
				{
					this.rb.isKinematic = true;
					this.throwSettledTime = num;
					this.InvokeLandingEffects(this.landingSide);
				}
				else if (!this.rb.isKinematic && num2 > this.landingTime)
				{
					this.rb.isKinematic = true;
					this.throwSettledTime = num;
					base.transform.rotation = Quaternion.FromToRotation(vector, normal) * base.transform.rotation;
					this.InvokeLandingEffects(this.landingSide);
				}
			}
			if (num2 > this.landingTime + this.postLandingTime || (this.rb.isKinematic && (float)(num - this.throwSettledTime) > this.postLandingTime))
			{
				this.EndThrow();
			}
			this.prevVelocity = this.velocity;
			this.velocity = this.rb.linearVelocity;
		}

		// Token: 0x06006A3E RID: 27198 RVA: 0x0022AE54 File Offset: 0x00229054
		private void OnCollisionEnter(Collision collision)
		{
			float magnitude = collision.impulse.magnitude;
			if (magnitude > 0.001f)
			{
				Vector3 vector = Vector3.Reflect(this.prevVelocity, collision.impulse / magnitude);
				this.rb.linearVelocity = vector * this.bounceAmplification;
			}
		}

		// Token: 0x06006A3F RID: 27199 RVA: 0x0022AEA8 File Offset: 0x002290A8
		private void InvokeLandingEffects(int side)
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				if (side == 20)
				{
					this.onBestRoll.Invoke();
					return;
				}
				if (side == 1)
				{
					this.onWorstRoll.Invoke();
					return;
				}
			}
			else
			{
				if (side == 6)
				{
					this.onBestRoll.Invoke();
					return;
				}
				if (side == 1)
				{
					this.onWorstRoll.Invoke();
				}
			}
		}

		// Token: 0x06006A40 RID: 27200 RVA: 0x0022AF04 File Offset: 0x00229104
		private bool CheckCosmeticRollOverride(out int rollSide)
		{
			if (this.cosmeticRollOverrides.Length != 0)
			{
				if (this.cachedLocalRig == null)
				{
					RigContainer rigContainer;
					if (PhotonNetwork.InRoom && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer.Rig != null)
					{
						this.cachedLocalRig = rigContainer.Rig;
					}
					else
					{
						this.cachedLocalRig = GorillaTagger.Instance.offlineVRRig;
					}
				}
				if (this.cachedLocalRig != null)
				{
					int num = -1;
					for (int i = 0; i < this.cosmeticRollOverrides.Length; i++)
					{
						if (this.cosmeticRollOverrides[i].cosmeticName != null && this.cachedLocalRig.cosmeticSet != null && this.cachedLocalRig.cosmeticSet.HasItem(this.cosmeticRollOverrides[i].cosmeticName) && (!this.cosmeticRollOverrides[i].requireHolding || (EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.name == this.cosmeticRollOverrides[i].cosmeticName) || (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.name == this.cosmeticRollOverrides[i].cosmeticName)) && this.cosmeticRollOverrides[i].landingSide > num)
						{
							num = this.cosmeticRollOverrides[i].landingSide;
						}
					}
					if (num > 0)
					{
						rollSide = num;
						return true;
					}
				}
			}
			rollSide = 0;
			return false;
		}

		// Token: 0x040079BB RID: 31163
		[SerializeField]
		private DicePhysics.DiceType diceType = DicePhysics.DiceType.D20;

		// Token: 0x040079BC RID: 31164
		[SerializeField]
		private float landingTime = 5f;

		// Token: 0x040079BD RID: 31165
		[SerializeField]
		private float postLandingTime = 2f;

		// Token: 0x040079BE RID: 31166
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x040079BF RID: 31167
		[SerializeField]
		private AnimationCurve angleDeltaVsStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040079C0 RID: 31168
		[SerializeField]
		private AnimationCurve landingTimeVsStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040079C1 RID: 31169
		[SerializeField]
		private float strength = 1f;

		// Token: 0x040079C2 RID: 31170
		[SerializeField]
		private float damping = 0.5f;

		// Token: 0x040079C3 RID: 31171
		[SerializeField]
		private bool forceLandingSide;

		// Token: 0x040079C4 RID: 31172
		[SerializeField]
		private int forcedLandingSide = 20;

		// Token: 0x040079C5 RID: 31173
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x040079C6 RID: 31174
		[SerializeField]
		private float bounceAmplification = 1f;

		// Token: 0x040079C7 RID: 31175
		[SerializeField]
		private DicePhysics.CosmeticRollOverride[] cosmeticRollOverrides;

		// Token: 0x040079C8 RID: 31176
		[SerializeField]
		private UnityEvent onBestRoll;

		// Token: 0x040079C9 RID: 31177
		[SerializeField]
		private UnityEvent onWorstRoll;

		// Token: 0x040079CA RID: 31178
		[SerializeField]
		private UnityEvent onRollFinished;

		// Token: 0x040079CB RID: 31179
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x040079CC RID: 31180
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x040079CD RID: 31181
		private VRRig cachedLocalRig;

		// Token: 0x040079CE RID: 31182
		private DiceHoldable holdableParent;

		// Token: 0x040079CF RID: 31183
		private double throwStartTime = -1.0;

		// Token: 0x040079D0 RID: 31184
		private double throwSettledTime = -1.0;

		// Token: 0x040079D1 RID: 31185
		private int landingSide;

		// Token: 0x040079D2 RID: 31186
		private float scale;

		// Token: 0x040079D3 RID: 31187
		private Vector3 prevVelocity = Vector3.zero;

		// Token: 0x040079D4 RID: 31188
		private Vector3 velocity = Vector3.zero;

		// Token: 0x040079D5 RID: 31189
		private const float a = 38.833332f;

		// Token: 0x040079D6 RID: 31190
		private const float b = 77.66666f;

		// Token: 0x040079D7 RID: 31191
		private Vector3[] d20SideDirections = new Vector3[]
		{
			Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(0f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(0f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up
		};

		// Token: 0x040079D8 RID: 31192
		private Vector3[] d6SideDirections = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 1f, 0f)
		};

		// Token: 0x02001096 RID: 4246
		private enum DiceType
		{
			// Token: 0x040079DA RID: 31194
			D6,
			// Token: 0x040079DB RID: 31195
			D20
		}

		// Token: 0x02001097 RID: 4247
		[Serializable]
		private struct CosmeticRollOverride
		{
			// Token: 0x040079DC RID: 31196
			public string cosmeticName;

			// Token: 0x040079DD RID: 31197
			public int landingSide;

			// Token: 0x040079DE RID: 31198
			public bool requireHolding;
		}
	}
}
