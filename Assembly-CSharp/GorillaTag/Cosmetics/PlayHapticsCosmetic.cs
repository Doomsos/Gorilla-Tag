using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200110E RID: 4366
	public class PlayHapticsCosmetic : MonoBehaviour
	{
		// Token: 0x06006D46 RID: 27974 RVA: 0x0023E068 File Offset: 0x0023C268
		private void Awake()
		{
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x06006D47 RID: 27975 RVA: 0x0023E076 File Offset: 0x0023C276
		public void PlayHaptics()
		{
			GorillaTagger.Instance.StartVibration(this.leftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06006D48 RID: 27976 RVA: 0x0023E094 File Offset: 0x0023C294
		public void PlayHapticsTransferableObject()
		{
			if (this.parentTransferable != null && this.parentTransferable.IsMyItem())
			{
				bool forLeftController = this.parentTransferable.InLeftHand();
				GorillaTagger.Instance.StartVibration(forLeftController, this.hapticStrength, this.hapticDuration);
			}
		}

		// Token: 0x06006D49 RID: 27977 RVA: 0x0023E0DF File Offset: 0x0023C2DF
		public void PlayHaptics(bool isLeftHand)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06006D4A RID: 27978 RVA: 0x0023E0F8 File Offset: 0x0023C2F8
		public void PlayHapticsBothHands(bool isLeftHand)
		{
			this.PlayHaptics(false);
			this.PlayHaptics(true);
		}

		// Token: 0x06006D4B RID: 27979 RVA: 0x0023E0DF File Offset: 0x0023C2DF
		public void PlayHaptics(bool isLeftHand, float value)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06006D4C RID: 27980 RVA: 0x0023E108 File Offset: 0x0023C308
		public void PlayHapticsBothHands(bool isLeftHand, float value)
		{
			this.PlayHaptics(false, value);
			this.PlayHaptics(true, value);
		}

		// Token: 0x06006D4D RID: 27981 RVA: 0x0023E0DF File Offset: 0x0023C2DF
		public void PlayHaptics(bool isLeftHand, Collider other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06006D4E RID: 27982 RVA: 0x0023E11A File Offset: 0x0023C31A
		public void PlayHapticsBothHands(bool isLeftHand, Collider other)
		{
			this.PlayHaptics(false, other);
			this.PlayHaptics(true, other);
		}

		// Token: 0x06006D4F RID: 27983 RVA: 0x0023E0DF File Offset: 0x0023C2DF
		public void PlayHaptics(bool isLeftHand, Collision other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06006D50 RID: 27984 RVA: 0x0023E12C File Offset: 0x0023C32C
		public void PlayHapticsBothHands(bool isLeftHand, Collision other)
		{
			this.PlayHaptics(false, other);
			this.PlayHaptics(true, other);
		}

		// Token: 0x06006D51 RID: 27985 RVA: 0x0023E140 File Offset: 0x0023C340
		public void PlayHapticsByButtonValue(bool isLeftHand, float strength)
		{
			float amplitude = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, strength);
			GorillaTagger.Instance.StartVibration(isLeftHand, amplitude, this.hapticDuration);
		}

		// Token: 0x06006D52 RID: 27986 RVA: 0x0023E172 File Offset: 0x0023C372
		public void PlayHapticsByButtonValueBothHands(bool isLeftHand, float strength)
		{
			this.PlayHapticsByButtonValue(false, strength);
			this.PlayHapticsByButtonValue(true, strength);
		}

		// Token: 0x06006D53 RID: 27987 RVA: 0x0023E184 File Offset: 0x0023C384
		public void PlayHapticsByVelocity(bool isLeftHand, float velocity)
		{
			float num = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true, 0.15f, false).magnitude;
			num = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, num);
			GorillaTagger.Instance.StartVibration(isLeftHand, num, this.hapticDuration);
		}

		// Token: 0x06006D54 RID: 27988 RVA: 0x0023E1D6 File Offset: 0x0023C3D6
		public void PlayHapticsByVelocityBothHands(bool isLeftHand, float velocity)
		{
			this.PlayHapticsByVelocity(false, velocity);
			this.PlayHapticsByVelocity(true, velocity);
		}

		// Token: 0x04007E6C RID: 32364
		[SerializeField]
		private float hapticDuration;

		// Token: 0x04007E6D RID: 32365
		[SerializeField]
		private float hapticStrength;

		// Token: 0x04007E6E RID: 32366
		[SerializeField]
		private float minHapticStrengthThreshold;

		// Token: 0x04007E6F RID: 32367
		[SerializeField]
		private float maxHapticStrengthThreshold;

		// Token: 0x04007E70 RID: 32368
		[Tooltip("Only check this box if you are not setting the left/hand right from the subscriber")]
		[SerializeField]
		private bool leftHand;

		// Token: 0x04007E71 RID: 32369
		private TransferrableObject parentTransferable;
	}
}
