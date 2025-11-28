using System;

namespace GorillaNetworking
{
	// Token: 0x02000EE2 RID: 3810
	public class GorillaATMKeyButton : GorillaKeyButton<GorillaATMKeyBindings>
	{
		// Token: 0x06005F36 RID: 24374 RVA: 0x001E9929 File Offset: 0x001E7B29
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaATMKeyButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
