using System;
using GorillaTagScripts.UI;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000E26 RID: 3622
	public class CustomMapsKeyboard : GorillaKeyWrapper<CustomMapKeyboardBinding>
	{
		// Token: 0x06005A76 RID: 23158 RVA: 0x001CF977 File Offset: 0x001CDB77
		public static string BindingToString(CustomMapKeyboardBinding binding)
		{
			return CustomMapsKeyButton.BindingToString(binding);
		}
	}
}
