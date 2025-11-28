using System;

namespace GorillaTagScripts
{
	// Token: 0x02000DB6 RID: 3510
	public class BuilderOptionButton : GorillaPressableButton
	{
		// Token: 0x06005663 RID: 22115 RVA: 0x001B2B47 File Offset: 0x001B0D47
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x06005664 RID: 22116 RVA: 0x00002789 File Offset: 0x00000989
		private void OnDestroy()
		{
		}

		// Token: 0x06005665 RID: 22117 RVA: 0x001B2B4F File Offset: 0x001B0D4F
		public void Setup(Action<BuilderOptionButton, bool> onPressed)
		{
			this.onPressed = onPressed;
		}

		// Token: 0x06005666 RID: 22118 RVA: 0x001B2B58 File Offset: 0x001B0D58
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			Action<BuilderOptionButton, bool> action = this.onPressed;
			if (action == null)
			{
				return;
			}
			action.Invoke(this, isLeftHand);
		}

		// Token: 0x06005667 RID: 22119 RVA: 0x001B2B6C File Offset: 0x001B0D6C
		public void SetPressed(bool pressed)
		{
			this.buttonRenderer.material = (pressed ? this.pressedMaterial : this.unpressedMaterial);
		}

		// Token: 0x0400638B RID: 25483
		private new Action<BuilderOptionButton, bool> onPressed;
	}
}
