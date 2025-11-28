using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200112A RID: 4394
	public class SimpleTransformAnimatorCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06006DEA RID: 28138 RVA: 0x0024189C File Offset: 0x0023FA9C
		private void DebugToggle()
		{
			this.Toggle();
		}

		// Token: 0x06006DEB RID: 28139 RVA: 0x002418A4 File Offset: 0x0023FAA4
		private void DebugA()
		{
			this.TogglePoseA();
		}

		// Token: 0x06006DEC RID: 28140 RVA: 0x002418AC File Offset: 0x0023FAAC
		private void DebugB()
		{
			this.TogglePoseB();
		}

		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x06006DED RID: 28141 RVA: 0x002418B4 File Offset: 0x0023FAB4
		// (set) Token: 0x06006DEE RID: 28142 RVA: 0x002418BC File Offset: 0x0023FABC
		public bool TickRunning { get; set; }

		// Token: 0x06006DEF RID: 28143 RVA: 0x002418C5 File Offset: 0x0023FAC5
		private void OnEnable()
		{
			this.posBlendCurrent = this.posBlendTarget;
			this.UpdateTransform();
		}

		// Token: 0x06006DF0 RID: 28144 RVA: 0x002418D9 File Offset: 0x0023FAD9
		private void OnDisable()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
				this.TickRunning = false;
			}
		}

		// Token: 0x06006DF1 RID: 28145 RVA: 0x002418F0 File Offset: 0x0023FAF0
		private void CheckAnimationNeeded()
		{
			bool flag = false;
			bool flag2 = Mathf.Approximately(this.posBlendCurrent, this.posBlendTarget);
			switch (this.animMode)
			{
			case SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos:
				flag = !flag2;
				break;
			case SimpleTransformAnimatorCosmetic.animModes.animateOneshot:
				flag = (this.loopAnim || !flag2);
				break;
			}
			if (flag && !this.TickRunning)
			{
				TickSystem<object>.AddCallbackTarget(this);
				this.TickRunning = true;
				this.isAnimating = true;
				return;
			}
			if (!flag && this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
				this.TickRunning = false;
				this.isAnimating = false;
			}
		}

		// Token: 0x06006DF2 RID: 28146 RVA: 0x00241984 File Offset: 0x0023FB84
		public void Tick()
		{
			float num = 1f / this.animationDuration;
			this.posBlendCurrent = Mathf.MoveTowards(this.posBlendCurrent, this.posBlendTarget, Time.deltaTime * num);
			switch (this.animMode)
			{
			default:
				this.UpdateTransform();
				this.CheckAnimationNeeded();
				return;
			}
		}

		// Token: 0x06006DF3 RID: 28147 RVA: 0x002419E4 File Offset: 0x0023FBE4
		private void UpdateTransform()
		{
			Vector3 vector = this.targetTransform.position;
			Quaternion quaternion = this.targetTransform.rotation;
			float num = this.InterpolationCurve.Evaluate(this.posBlendCurrent);
			if (this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.Position || this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation)
			{
				vector = Vector3.Lerp(this.poseA.position, this.poseB.position, num);
			}
			if (this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.Rotation || this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation)
			{
				quaternion = Quaternion.Slerp(this.poseA.rotation, this.poseB.rotation, num);
			}
			this.targetTransform.SetPositionAndRotation(vector, quaternion);
		}

		// Token: 0x06006DF4 RID: 28148 RVA: 0x00241A85 File Offset: 0x0023FC85
		public void Toggle()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = ((this.posBlendTarget < 0.5f) ? 1f : 0f);
			this.CheckAnimationNeeded();
		}

		// Token: 0x06006DF5 RID: 28149 RVA: 0x00241AB3 File Offset: 0x0023FCB3
		public void TogglePoseA()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = 0f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x06006DF6 RID: 28150 RVA: 0x00241ACD File Offset: 0x0023FCCD
		public void TogglePoseB()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = 1f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x06006DF7 RID: 28151 RVA: 0x00241AE7 File Offset: 0x0023FCE7
		public void playAnimationOneshot()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.animateOneshot;
			this.posBlendCurrent = 0f;
			this.posBlendTarget = 1f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x06006DF8 RID: 28152 RVA: 0x00241B0C File Offset: 0x0023FD0C
		private void DebugPlayAnimationOneShot()
		{
			this.playAnimationOneshot();
		}

		// Token: 0x04007FA6 RID: 32678
		private SimpleTransformAnimatorCosmetic.animModes animMode;

		// Token: 0x04007FA7 RID: 32679
		[Tooltip("Shapes how the transform will interpolate over the course of the animation.")]
		public AnimationCurve InterpolationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007FA8 RID: 32680
		[SerializeField]
		[Tooltip("The object that will animate (blend) between the poses.")]
		private Transform targetTransform;

		// Token: 0x04007FA9 RID: 32681
		[SerializeField]
		[Tooltip("Start pose (blend value 0).")]
		private Transform poseA;

		// Token: 0x04007FAA RID: 32682
		[SerializeField]
		[Tooltip("End pose (blend value 1).")]
		private Transform poseB;

		// Token: 0x04007FAB RID: 32683
		[FormerlySerializedAs("transitionTime")]
		[SerializeField]
		[Tooltip("Total time (in seconds) to animate fully between poses.")]
		private float animationDuration = 1f;

		// Token: 0x04007FAC RID: 32684
		[SerializeField]
		[Tooltip("Controls what aspect of the transform is affected by the blend.")]
		private SimpleTransformAnimatorCosmetic.animatedPropertyChoices animatedProperties = SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation;

		// Token: 0x04007FAD RID: 32685
		private bool loopAnim;

		// Token: 0x04007FAE RID: 32686
		private float posBlendCurrent;

		// Token: 0x04007FAF RID: 32687
		private float posBlendTarget;

		// Token: 0x04007FB0 RID: 32688
		private bool isAnimating;

		// Token: 0x0200112B RID: 4395
		public enum animatedPropertyChoices
		{
			// Token: 0x04007FB3 RID: 32691
			Position,
			// Token: 0x04007FB4 RID: 32692
			Rotation,
			// Token: 0x04007FB5 RID: 32693
			PositionAndRotation
		}

		// Token: 0x0200112C RID: 4396
		public enum animModes
		{
			// Token: 0x04007FB7 RID: 32695
			stepToTargetPos,
			// Token: 0x04007FB8 RID: 32696
			animateBounce,
			// Token: 0x04007FB9 RID: 32697
			animateOneshot
		}
	}
}
