using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200090B RID: 2315
public class GorillaComputerTerminal : MonoBehaviour, IBuildValidation
{
	// Token: 0x06003B24 RID: 15140 RVA: 0x00138D54 File Offset: 0x00136F54
	public bool BuildValidationCheck()
	{
		if (this.myScreenText == null || this.myFunctionText == null || this.monitorMesh == null)
		{
			Debug.LogErrorFormat(base.gameObject, "gorilla computer terminal {0} is missing screen text, function text, or monitor mesh. this will break lots of computer stuff", new object[]
			{
				base.gameObject.name
			});
			return false;
		}
		return true;
	}

	// Token: 0x06003B25 RID: 15141 RVA: 0x00138DB2 File Offset: 0x00136FB2
	private void OnEnable()
	{
		if (GorillaComputer.instance == null)
		{
			base.StartCoroutine(this.<OnEnable>g__OnEnable_Local|4_0());
			return;
		}
		this.Init();
	}

	// Token: 0x06003B26 RID: 15142 RVA: 0x00138DD8 File Offset: 0x00136FD8
	private void Init()
	{
		GameEvents.ScreenTextChangedEvent.AddListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.AddListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.AddListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
		GameEvents.LanguageEvent.AddListener(new UnityAction(this.OnLanguageChanged));
		this.myScreenText.text = GorillaComputer.instance.screenText.Text;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.Text;
		if (GorillaComputer.instance.screenText.currentMaterials != null)
		{
			this.monitorMesh.materials = GorillaComputer.instance.screenText.currentMaterials;
		}
	}

	// Token: 0x06003B27 RID: 15143 RVA: 0x00138EA4 File Offset: 0x001370A4
	private void OnDisable()
	{
		GameEvents.ScreenTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.RemoveListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
	}

	// Token: 0x06003B28 RID: 15144 RVA: 0x00138EF3 File Offset: 0x001370F3
	public void OnScreenTextChanged(string text)
	{
		this.myScreenText.text = text;
	}

	// Token: 0x06003B29 RID: 15145 RVA: 0x00138F01 File Offset: 0x00137101
	public void OnFunctionTextChanged(string text)
	{
		this.myFunctionText.text = text;
	}

	// Token: 0x06003B2A RID: 15146 RVA: 0x00138F0F File Offset: 0x0013710F
	private void OnMaterialsChanged(Material[] materials)
	{
		this.monitorMesh.materials = materials;
	}

	// Token: 0x06003B2B RID: 15147 RVA: 0x00138F20 File Offset: 0x00137120
	private void OnLanguageChanged()
	{
		LocalisationFontPair localisationFontPair;
		if (LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair))
		{
			this.myScreenText.font = localisationFontPair.fontAsset;
			this.myFunctionText.font = localisationFontPair.fontAsset;
		}
		this.myScreenText.characterSpacing = localisationFontPair.charSpacing;
	}

	// Token: 0x06003B2D RID: 15149 RVA: 0x00138F69 File Offset: 0x00137169
	[CompilerGenerated]
	private IEnumerator <OnEnable>g__OnEnable_Local|4_0()
	{
		yield return new WaitUntil(() => GorillaComputer.instance != null);
		yield return null;
		this.Init();
		yield break;
	}

	// Token: 0x04004B7D RID: 19325
	public TextMeshPro myScreenText;

	// Token: 0x04004B7E RID: 19326
	public TextMeshPro myFunctionText;

	// Token: 0x04004B7F RID: 19327
	public MeshRenderer monitorMesh;
}
