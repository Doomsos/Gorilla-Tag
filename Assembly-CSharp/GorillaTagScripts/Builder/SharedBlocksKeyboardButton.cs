using System;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E6D RID: 3693
	public class SharedBlocksKeyboardButton : GorillaKeyButton<SharedBlocksKeyboardBindings>
	{
		// Token: 0x06005C52 RID: 23634 RVA: 0x001DAAA9 File Offset: 0x001D8CA9
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
