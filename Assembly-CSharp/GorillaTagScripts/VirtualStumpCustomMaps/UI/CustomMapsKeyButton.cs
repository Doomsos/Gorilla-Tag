using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000E28 RID: 3624
	public class CustomMapsKeyButton : GorillaKeyButton<CustomMapKeyboardBinding>
	{
		// Token: 0x06005A78 RID: 23160 RVA: 0x001CF987 File Offset: 0x001CDB87
		protected override void OnEnableEvents()
		{
			base.OnEnableEvents();
			if (!this._isLocalized)
			{
				return;
			}
			this.OnLanguageChanged();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x06005A79 RID: 23161 RVA: 0x001CF9AF File Offset: 0x001CDBAF
		protected override void OnDisableEvents()
		{
			base.OnDisableEvents();
			if (!this._isLocalized)
			{
				return;
			}
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x06005A7A RID: 23162 RVA: 0x001CF9D4 File Offset: 0x001CDBD4
		public static string BindingToString(CustomMapKeyboardBinding binding)
		{
			if (binding < CustomMapKeyboardBinding.up || (binding > CustomMapKeyboardBinding.option3 && binding < CustomMapKeyboardBinding.at))
			{
				if (binding >= CustomMapKeyboardBinding.up)
				{
					return binding.ToString();
				}
				int num = (int)binding;
				return num.ToString();
			}
			else
			{
				switch (binding)
				{
				case CustomMapKeyboardBinding.at:
					return "@";
				case CustomMapKeyboardBinding.dash:
					return "-";
				case CustomMapKeyboardBinding.period:
					return ".";
				case CustomMapKeyboardBinding.underscore:
					return "_";
				case CustomMapKeyboardBinding.plus:
					return "+";
				case CustomMapKeyboardBinding.space:
					return " ";
				default:
					return "";
				}
			}
		}

		// Token: 0x06005A7B RID: 23163 RVA: 0x00002789 File Offset: 0x00000989
		protected override void OnButtonPressedEvent()
		{
		}

		// Token: 0x06005A7C RID: 23164 RVA: 0x001CFA5C File Offset: 0x001CDC5C
		private void OnLanguageChanged()
		{
			if (!this._isLocalized)
			{
				return;
			}
			if (this._buttonDisplayNameTxt == null)
			{
				Debug.LogError("[LOCALIZATION::CUSTOM_MAPS_KEY_BUTTON] [_buttonDisplayNameTxt] has not been assigned and is NULL", this);
				return;
			}
			if (this._localizedName == null || this._localizedName.IsEmpty)
			{
				Debug.LogError("[LOCALIZATION::CUSTOM_MAPS_KEY_BUTTON] [_localizedName] has not been assigned", this);
				return;
			}
			this._buttonDisplayNameTxt.text = this._localizedName.GetLocalizedString();
		}

		// Token: 0x040067AF RID: 26543
		[SerializeField]
		private bool _isLocalized;

		// Token: 0x040067B0 RID: 26544
		[SerializeField]
		private LocalizedString _localizedName;

		// Token: 0x040067B1 RID: 26545
		[SerializeField]
		private TMP_Text _buttonDisplayNameTxt;
	}
}
