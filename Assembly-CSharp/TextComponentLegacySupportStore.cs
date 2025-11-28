using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000AEF RID: 2799
public struct TextComponentLegacySupportStore
{
	// Token: 0x060045A7 RID: 17831 RVA: 0x00171024 File Offset: 0x0016F224
	public TextComponentLegacySupportStore(Transform objRef)
	{
		this._objectReference = objRef;
		this._legacyTextReference = null;
		this._legacyTextMeshReference = null;
		this._tmpTextReference = objRef.GetComponent<TMP_Text>();
		if (this._tmpTextReference != null)
		{
			return;
		}
		this._legacyTextReference = objRef.GetComponent<Text>();
		if (this._legacyTextReference)
		{
			return;
		}
		this._legacyTextMeshReference = objRef.GetComponent<TextMesh>();
		if (this._legacyTextMeshReference)
		{
			return;
		}
		Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Could not find either a [TMP_Text], Legacy-[Text], or Legacy-[TextMesh] component on object [" + objRef.name + "]", this._objectReference);
	}

	// Token: 0x1700068F RID: 1679
	// (get) Token: 0x060045A8 RID: 17832 RVA: 0x001710B5 File Offset: 0x0016F2B5
	public bool IsValid
	{
		get
		{
			return this._tmpTextReference || this._legacyTextReference || this._legacyTextMeshReference;
		}
	}

	// Token: 0x17000690 RID: 1680
	// (get) Token: 0x060045A9 RID: 17833 RVA: 0x001710DE File Offset: 0x0016F2DE
	// (set) Token: 0x060045AA RID: 17834 RVA: 0x001710FE File Offset: 0x0016F2FE
	public float characterSpacing
	{
		get
		{
			if (this._tmpTextReference)
			{
				return this._tmpTextReference.characterSpacing;
			}
			return 0f;
		}
		set
		{
			if (this._tmpTextReference)
			{
				this._tmpTextReference.characterSpacing = value;
				return;
			}
		}
	}

	// Token: 0x060045AB RID: 17835 RVA: 0x0017111C File Offset: 0x0016F31C
	public void SetFont(TMP_FontAsset font, Font legacyFont)
	{
		if (font != null && this._tmpTextReference)
		{
			this.SetFont(font);
			return;
		}
		if (legacyFont != null && (this._legacyTextReference || this._legacyTextMeshReference))
		{
			this.SetFont(legacyFont);
			return;
		}
		if (!this.IsValid)
		{
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Trying to change font but both text references are NULL.");
		}
	}

	// Token: 0x060045AC RID: 17836 RVA: 0x00171184 File Offset: 0x0016F384
	public void SetFont(Font font)
	{
		if (this._legacyTextReference)
		{
			this._legacyTextReference.font = font;
			return;
		}
		if (this._legacyTextMeshReference)
		{
			this._legacyTextMeshReference.font = font;
			return;
		}
		Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Trying to change font for non-legacy reference but passed in a legacy font.", font);
	}

	// Token: 0x060045AD RID: 17837 RVA: 0x001711D0 File Offset: 0x0016F3D0
	public void SetFont(TMP_FontAsset font)
	{
		if (this._tmpTextReference == null)
		{
			return;
		}
		this._tmpTextReference.font = font;
	}

	// Token: 0x060045AE RID: 17838 RVA: 0x001711F0 File Offset: 0x0016F3F0
	public void SetFontSize(float fontSize)
	{
		if (!this._tmpTextReference)
		{
			return;
		}
		TMP_Text tmpTextReference = this._tmpTextReference;
		this._tmpTextReference.fontSizeMax = fontSize;
		tmpTextReference.fontSize = fontSize;
	}

	// Token: 0x17000691 RID: 1681
	// (get) Token: 0x060045AF RID: 17839 RVA: 0x00171228 File Offset: 0x0016F428
	// (set) Token: 0x060045B0 RID: 17840 RVA: 0x00171290 File Offset: 0x0016F490
	public string text
	{
		get
		{
			if (this._tmpTextReference)
			{
				return this._tmpTextReference.text;
			}
			if (this._legacyTextReference)
			{
				return this._legacyTextReference.text;
			}
			if (this._legacyTextMeshReference)
			{
				return this._legacyTextMeshReference.text;
			}
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Both Legacy Text ref and TMP text ref are null!");
			return "";
		}
		set
		{
			if (this._tmpTextReference != null)
			{
				this._tmpTextReference.text = value;
				return;
			}
			if (this._legacyTextReference != null)
			{
				this._legacyTextReference.text = value;
				return;
			}
			if (this._legacyTextMeshReference)
			{
				this._legacyTextMeshReference.text = value;
				return;
			}
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Both Legacy Text ref and TMP text ref are null and cannot be set!", this._objectReference);
		}
	}

	// Token: 0x060045B1 RID: 17841 RVA: 0x001712FD File Offset: 0x0016F4FD
	public void SetText(string newText)
	{
		this.text = newText;
	}

	// Token: 0x060045B2 RID: 17842 RVA: 0x00171306 File Offset: 0x0016F506
	public void SetCharSpacing(float spacing)
	{
		this.characterSpacing = spacing;
	}

	// Token: 0x040057AC RID: 22444
	private Transform _objectReference;

	// Token: 0x040057AD RID: 22445
	private TMP_Text _tmpTextReference;

	// Token: 0x040057AE RID: 22446
	private Text _legacyTextReference;

	// Token: 0x040057AF RID: 22447
	private TextMesh _legacyTextMeshReference;
}
