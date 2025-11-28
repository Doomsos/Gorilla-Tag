using System;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E6D RID: 3693
	public class SharedBlocksKeyboardButton : GorillaKeyButton<SharedBlocksKeyboardBindings>
	{
		// Token: 0x06005C52 RID: 23634 RVA: 0x001DAA89 File Offset: 0x001D8C89
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
