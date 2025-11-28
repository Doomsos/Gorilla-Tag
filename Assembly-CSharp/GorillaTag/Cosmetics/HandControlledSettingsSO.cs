using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010FF RID: 4351
	public class HandControlledSettingsSO : ScriptableObject
	{
		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x06006CF7 RID: 27895 RVA: 0x0023CA5C File Offset: 0x0023AC5C
		private bool IsAngle
		{
			get
			{
				return this.rotationControl == HandControlledCosmetic.RotationControl.Angle;
			}
		}

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x06006CF8 RID: 27896 RVA: 0x0023CA67 File Offset: 0x0023AC67
		private bool IsTranslation
		{
			get
			{
				return this.rotationControl == HandControlledCosmetic.RotationControl.Translation;
			}
		}

		// Token: 0x04007E1B RID: 32283
		private const string SENS_TT = "The difference between the current input and cached input is magnified by this number.";

		// Token: 0x04007E1C RID: 32284
		public HandControlledCosmetic.RotationControl rotationControl;

		// Token: 0x04007E1D RID: 32285
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public float inputSensitivity = 2f;

		// Token: 0x04007E1E RID: 32286
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public AnimationCurve verticalSensitivity = AnimationCurve.Constant(0f, 1f, 2f);

		// Token: 0x04007E1F RID: 32287
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public AnimationCurve horizontalSensitivity = AnimationCurve.Constant(0f, 1f, 2f);

		// Token: 0x04007E20 RID: 32288
		[Tooltip("How quickly the cached input approaches the current input. A high value will function more like a mouse, while a low value will function more like a joystick.")]
		public float inputDecaySpeed = 1f;

		// Token: 0x04007E21 RID: 32289
		[Tooltip("How quickly the cached input approaches the current input, as a function of distance. A high value will function more like a mouse, while a low value will function more like a joystick.")]
		public AnimationCurve inputDecayCurve = AnimationCurve.Constant(0f, 2f, 1f);

		// Token: 0x04007E22 RID: 32290
		[Tooltip("How quickly the transform approaches the intended angle (smaller value = more lag).")]
		public float rotationSpeed = 20f;

		// Token: 0x04007E23 RID: 32291
		[Tooltip("The transform's local rotation cannot exceed these euler angles.")]
		public Vector3 angleLimits = new Vector3(45f, 360f, 0f);
	}
}
