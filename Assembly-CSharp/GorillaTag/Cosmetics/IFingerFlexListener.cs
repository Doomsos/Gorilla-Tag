using System;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001100 RID: 4352
	public interface IFingerFlexListener
	{
		// Token: 0x06006CFA RID: 27898 RVA: 0x00027DED File Offset: 0x00025FED
		bool FingerFlexValidation(bool isLeftHand)
		{
			return true;
		}

		// Token: 0x06006CFB RID: 27899
		void OnButtonPressed(bool isLeftHand, float value);

		// Token: 0x06006CFC RID: 27900
		void OnButtonReleased(bool isLeftHand, float value);

		// Token: 0x06006CFD RID: 27901
		void OnButtonPressStayed(bool isLeftHand, float value);

		// Token: 0x02001101 RID: 4353
		public enum ComponentActivator
		{
			// Token: 0x04007E25 RID: 32293
			FingerReleased,
			// Token: 0x04007E26 RID: 32294
			FingerFlexed,
			// Token: 0x04007E27 RID: 32295
			FingerStayed
		}
	}
}
