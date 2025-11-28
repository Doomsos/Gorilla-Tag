using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x020009A7 RID: 2471
public class CustomMapsSearchScreen : CustomMapsTerminalScreen
{
	// Token: 0x06003F09 RID: 16137 RVA: 0x00151D14 File Offset: 0x0014FF14
	public override void Show()
	{
		base.Show();
		this.searchPhraseText.gameObject.SetActive(true);
		this.customMapsGalleryView.gameObject.SetActive(false);
		this.searchMessageText.gameObject.SetActive(false);
		this.leftPageButton.gameObject.SetActive(false);
		this.rightPageButton.gameObject.SetActive(false);
		this.searchedMods.Clear();
		this.filteredSearchedMods.Clear();
		this.displayedMods.Clear();
		this.searchPhraseText.text = this.defaultSearchString;
		this.searchPhrase = string.Empty;
		this.currentSearchModsRequestPage = 0;
		this.currentModPage = 0;
	}

	// Token: 0x06003F0A RID: 16138 RVA: 0x00151DC7 File Offset: 0x0014FFC7
	public override void Hide()
	{
		base.Hide();
		this.customMapsGalleryView.ShowTileText(false, true);
	}

	// Token: 0x06003F0B RID: 16139 RVA: 0x00002789 File Offset: 0x00000989
	public override void Initialize()
	{
	}

	// Token: 0x06003F0C RID: 16140 RVA: 0x00151DDC File Offset: 0x0014FFDC
	public void ReturnFromDetailsScreen()
	{
		base.Show();
		this.customMapsGalleryView.ShowTileText(true, true);
	}

	// Token: 0x06003F0D RID: 16141 RVA: 0x00151DF4 File Offset: 0x0014FFF4
	public override void PressButton(CustomMapKeyboardBinding pressedButton)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (CustomMapKeyboardBinding.tile1 <= pressedButton && pressedButton <= CustomMapKeyboardBinding.tile6 && !this.customMapsGalleryView.IsNull())
		{
			this.customMapsGalleryView.ShowDetailsForEntry(pressedButton - CustomMapKeyboardBinding.tile1);
		}
		if (pressedButton < CustomMapKeyboardBinding.up)
		{
			string text = this.searchPhrase;
			int num = (int)pressedButton;
			this.searchPhrase = text + num.ToString();
			this.RefreshSearchText();
			return;
		}
		if (pressedButton > CustomMapKeyboardBinding.option3 && pressedButton < CustomMapKeyboardBinding.at)
		{
			this.searchPhrase += pressedButton.ToString();
			this.RefreshSearchText();
			return;
		}
		if (pressedButton != CustomMapKeyboardBinding.delete)
		{
			if (pressedButton != CustomMapKeyboardBinding.enter)
			{
				switch (pressedButton)
				{
				case CustomMapKeyboardBinding.goback:
					if (this.loadingSearchMods)
					{
						return;
					}
					CustomMapsTerminal.ReturnFromSearchScreen();
					break;
				case CustomMapKeyboardBinding.left:
					this.currentModPage--;
					this.RefreshScreenState();
					break;
				case CustomMapKeyboardBinding.right:
					this.currentModPage++;
					this.RefreshScreenState();
					break;
				}
			}
			else
			{
				if (this.loadingSearchMods)
				{
					return;
				}
				this.searchedMods.Clear();
				this.filteredSearchedMods.Clear();
				this.currentSearchModsRequestPage = 0;
				this.searchMessageText.gameObject.SetActive(true);
				this.searchMessageText.text = this.searchingString;
				this.RetrieveMods();
			}
		}
		else if (!this.searchPhrase.IsNullOrEmpty())
		{
			this.searchPhrase = this.searchPhrase.Remove(this.searchPhrase.Length - 1);
		}
		this.RefreshSearchText();
	}

	// Token: 0x06003F0E RID: 16142 RVA: 0x00151F81 File Offset: 0x00150181
	private void RefreshSearchText()
	{
		if (this.searchPhrase.IsNullOrEmpty())
		{
			this.searchPhraseText.text = this.defaultSearchString;
			return;
		}
		this.searchPhraseText.text = this.searchPhrase;
	}

	// Token: 0x06003F0F RID: 16143 RVA: 0x00151FB4 File Offset: 0x001501B4
	private Task RetrieveMods()
	{
		CustomMapsSearchScreen.<RetrieveMods>d__26 <RetrieveMods>d__;
		<RetrieveMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveMods>d__.<>4__this = this;
		<RetrieveMods>d__.<>1__state = -1;
		<RetrieveMods>d__.<>t__builder.Start<CustomMapsSearchScreen.<RetrieveMods>d__26>(ref <RetrieveMods>d__);
		return <RetrieveMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003F10 RID: 16144 RVA: 0x00151FF8 File Offset: 0x001501F8
	private void FilterSearchMods()
	{
		if (this.searchedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		this.filteredSearchedMods.Clear();
		foreach (Mod mod in this.searchedMods)
		{
			ModId modId;
			if (ModIOManager.TryGetNewMapsModId(out modId) && mod.Id == modId)
			{
				this.totalSearchMods = Mathf.Max(0, this.totalSearchMods - 1);
			}
			else
			{
				this.filteredSearchedMods.Add(mod);
			}
		}
	}

	// Token: 0x06003F11 RID: 16145 RVA: 0x00152098 File Offset: 0x00150298
	private void RefreshScreenState()
	{
		this.searchMessageText.gameObject.SetActive(false);
		this.customMapsGalleryView.ResetGallery();
		this.customMapsGalleryView.gameObject.SetActive(false);
		this.displayedMods.Clear();
		if (this.errorLoadingSearchMods)
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = this.errorMessage;
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
			return;
		}
		if (this.filteredSearchedMods.IsNullOrEmpty<Mod>())
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = this.noMapsFoundString;
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
			return;
		}
		int num = 0;
		int num2 = this.modsPerPage - 1;
		if (!this.IsOnFirstPage())
		{
			num = this.currentModPage * this.modsPerPage;
			num2 = num + this.modsPerPage - 1;
			this.leftPageButton.gameObject.SetActive(true);
		}
		else
		{
			this.leftPageButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.rightPageButton.gameObject.SetActive(true);
		}
		else
		{
			this.rightPageButton.gameObject.SetActive(false);
		}
		if (this.filteredSearchedMods.Count <= num2 && this.totalSearchMods > this.searchedMods.Count)
		{
			this.RetrieveMods();
			return;
		}
		int num3 = num;
		while (num3 <= num2 && this.filteredSearchedMods.Count > num3)
		{
			this.displayedMods.Add(this.filteredSearchedMods[num3]);
			num3++;
		}
		this.customMapsGalleryView.gameObject.SetActive(true);
		string text;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedMods, true, out text))
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = text;
			this.customMapsGalleryView.gameObject.SetActive(false);
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
		}
	}

	// Token: 0x06003F12 RID: 16146 RVA: 0x001522AC File Offset: 0x001504AC
	private int GetNumPages()
	{
		int num = this.totalSearchMods % this.modsPerPage;
		int num2 = this.totalSearchMods / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06003F13 RID: 16147 RVA: 0x001522DC File Offset: 0x001504DC
	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	// Token: 0x06003F14 RID: 16148 RVA: 0x001522E8 File Offset: 0x001504E8
	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	// Token: 0x04005038 RID: 20536
	[SerializeField]
	private TMP_Text searchPhraseText;

	// Token: 0x04005039 RID: 20537
	[SerializeField]
	private TMP_Text searchMessageText;

	// Token: 0x0400503A RID: 20538
	[SerializeField]
	private CustomMapsGalleryView customMapsGalleryView;

	// Token: 0x0400503B RID: 20539
	[SerializeField]
	private GameObject leftPageButton;

	// Token: 0x0400503C RID: 20540
	[SerializeField]
	private GameObject rightPageButton;

	// Token: 0x0400503D RID: 20541
	[SerializeField]
	private string defaultSearchString = "SEARCH PHRASE";

	// Token: 0x0400503E RID: 20542
	[SerializeField]
	private string noMapsFoundString = "NO RESULTS FOUND";

	// Token: 0x0400503F RID: 20543
	[SerializeField]
	private string searchingString = "SEARCHING";

	// Token: 0x04005040 RID: 20544
	[SerializeField]
	private int numModsPerRequest = 60;

	// Token: 0x04005041 RID: 20545
	[SerializeField]
	private int modsPerPage = 6;

	// Token: 0x04005042 RID: 20546
	private string searchPhrase = "";

	// Token: 0x04005043 RID: 20547
	private List<Mod> searchedMods = new List<Mod>();

	// Token: 0x04005044 RID: 20548
	private List<Mod> filteredSearchedMods = new List<Mod>();

	// Token: 0x04005045 RID: 20549
	private List<Mod> displayedMods = new List<Mod>();

	// Token: 0x04005046 RID: 20550
	private int currentSearchModsRequestPage;

	// Token: 0x04005047 RID: 20551
	private bool loadingSearchMods;

	// Token: 0x04005048 RID: 20552
	private bool errorLoadingSearchMods;

	// Token: 0x04005049 RID: 20553
	private int totalSearchMods;

	// Token: 0x0400504A RID: 20554
	private int currentModPage;

	// Token: 0x0400504B RID: 20555
	private string errorMessage = "";
}
