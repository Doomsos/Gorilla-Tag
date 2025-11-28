using System;

namespace GorillaTagScripts
{
	// Token: 0x02000DB6 RID: 3510
	public class BuilderOptionButton : GorillaPressableButton
	{
		// Token: 0x06005663 RID: 22115 RVA: 0x001B2B27 File Offset: 0x001B0D27
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x06005664 RID: 22116 RVA: 0x00002789 File Offset: 0x00000989
		private void OnDestroy()
		{
		}

		// Token: 0x06005665 RID: 22117 RVA: 0x001B2B2F File Offset: 0x001B0D2F
		public void Setup(Action<BuilderOptionButton, bool> onPressed)
		{
			this.onPressed = onPressed;
		}

		// Token: 0x06005666 RID: 22118 RVA: 0x001B2B38 File Offset: 0x001B0D38
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			Action<BuilderOptionButton, bool> action = this.onPressed;
			if (action == null)
			{
				return;
			}
			action.Invoke(this, isLeftHand);
		}

		// Token: 0x06005667 RID: 22119 RVA: 0x001B2B4C File Offset: 0x001B0D4C
		public void SetPressed(bool pressed)
		{
			this.buttonRenderer.material = (pressed ? this.pressedMaterial : this.unpressedMaterial);
		}

		// Token: 0x0400638B RID: 25483
		private new Action<BuilderOptionButton, bool> onPressed;
	}
}
