using System;

namespace GorillaNetworking
{
	// Token: 0x02000EF2 RID: 3826
	public class GorillaKeyboardButton : GorillaKeyButton<GorillaKeyboardBindings>
	{
		// Token: 0x06006018 RID: 24600 RVA: 0x001F0022 File Offset: 0x001EE222
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
