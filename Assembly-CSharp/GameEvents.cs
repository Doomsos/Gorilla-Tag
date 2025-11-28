using System;
using GorillaNetworking;
using GorillaTagScripts.Builder;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200050B RID: 1291
public class GameEvents
{
	// Token: 0x04002BA5 RID: 11173
	public static UnityEvent<GorillaKeyboardBindings> OnGorrillaKeyboardButtonPressedEvent = new UnityEvent<GorillaKeyboardBindings>();

	// Token: 0x04002BA6 RID: 11174
	public static UnityEvent<GorillaATMKeyBindings> OnGorrillaATMKeyButtonPressedEvent = new UnityEvent<GorillaATMKeyBindings>();

	// Token: 0x04002BA7 RID: 11175
	internal static UnityEvent<string> ScreenTextChangedEvent = new UnityEvent<string>();

	// Token: 0x04002BA8 RID: 11176
	internal static UnityEvent<Material[]> ScreenTextMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x04002BA9 RID: 11177
	internal static UnityEvent<string> FunctionSelectTextChangedEvent = new UnityEvent<string>();

	// Token: 0x04002BAA RID: 11178
	internal static UnityEvent<Material[]> FunctionTextMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x04002BAB RID: 11179
	internal static UnityEvent LanguageEvent = new UnityEvent();

	// Token: 0x04002BAC RID: 11180
	internal static UnityEvent<string> ScoreboardTextChangedEvent = new UnityEvent<string>();

	// Token: 0x04002BAD RID: 11181
	internal static UnityEvent<Material[]> ScoreboardMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x04002BAE RID: 11182
	public static UnityEvent<SharedBlocksKeyboardBindings> OnSharedBlocksKeyboardButtonPressedEvent = new UnityEvent<SharedBlocksKeyboardBindings>();
}
