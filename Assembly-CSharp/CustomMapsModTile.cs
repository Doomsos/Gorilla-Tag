using System;
using System.Runtime.CompilerServices;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x020009A1 RID: 2465
public class CustomMapsModTile : CustomMapsScreenTouchPoint
{
	// Token: 0x170005C4 RID: 1476
	// (get) Token: 0x06003EE5 RID: 16101 RVA: 0x0015142E File Offset: 0x0014F62E
	public Mod CurrentMod
	{
		get
		{
			return this.currentMod;
		}
	}

	// Token: 0x06003EE6 RID: 16102 RVA: 0x00151436 File Offset: 0x0014F636
	protected override void Awake()
	{
		base.Awake();
		this.defaultLogo = this.touchPointRenderer.sprite;
		this.highlight.SetActive(false);
	}

	// Token: 0x06003EE7 RID: 16103 RVA: 0x0015145C File Offset: 0x0014F65C
	public void ShowTileText(bool show, bool useMapName)
	{
		if (!show)
		{
			this.ratingsText.gameObject.SetActive(false);
			this.mapNameText.gameObject.SetActive(false);
			this.thumsbUp.SetActive(false);
			return;
		}
		if (useMapName)
		{
			this.mapNameText.gameObject.SetActive(true);
			this.ratingsText.gameObject.SetActive(false);
			this.thumsbUp.SetActive(false);
			return;
		}
		this.ratingsText.gameObject.SetActive(true);
		this.thumsbUp.SetActive(true);
		this.mapNameText.gameObject.SetActive(false);
	}

	// Token: 0x06003EE8 RID: 16104 RVA: 0x001514FB File Offset: 0x0014F6FB
	public void ActivateTile(bool useMapName)
	{
		this.isActive = true;
		base.gameObject.SetActive(true);
		this.ShowTileText(true, useMapName);
		CustomMapsScreenTouchPoint.pressTime = Time.time;
	}

	// Token: 0x06003EE9 RID: 16105 RVA: 0x00151522 File Offset: 0x0014F722
	public void DeactivateTile()
	{
		this.isActive = false;
		base.gameObject.SetActive(false);
		this.highlight.SetActive(false);
		this.ShowTileText(false, false);
		this.ResetLogo();
	}

	// Token: 0x06003EEA RID: 16106 RVA: 0x00002789 File Offset: 0x00000989
	public override void PressButtonColourUpdate()
	{
	}

	// Token: 0x06003EEB RID: 16107 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnButtonPressedEvent()
	{
	}

	// Token: 0x06003EEC RID: 16108 RVA: 0x00151554 File Offset: 0x0014F754
	public void SetMod(Mod mod, bool useMapName)
	{
		CustomMapsModTile.<SetMod>d__19 <SetMod>d__;
		<SetMod>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetMod>d__.<>4__this = this;
		<SetMod>d__.mod = mod;
		<SetMod>d__.useMapName = useMapName;
		<SetMod>d__.<>1__state = -1;
		<SetMod>d__.<>t__builder.Start<CustomMapsModTile.<SetMod>d__19>(ref <SetMod>d__);
	}

	// Token: 0x06003EED RID: 16109 RVA: 0x0015159B File Offset: 0x0014F79B
	public void ResetLogo()
	{
		this.touchPointRenderer.sprite = this.defaultLogo;
	}

	// Token: 0x06003EEE RID: 16110 RVA: 0x001515AE File Offset: 0x0014F7AE
	public void ShowDetails()
	{
		CustomMapsTerminal.ShowDetailsScreen(this.currentMod);
	}

	// Token: 0x06003EEF RID: 16111 RVA: 0x001515BB File Offset: 0x0014F7BB
	public void HighlightTile()
	{
		this.highlight.SetActive(true);
	}

	// Token: 0x06003EF0 RID: 16112 RVA: 0x001515C9 File Offset: 0x0014F7C9
	public bool IsCurrentModHidden()
	{
		return this.currentMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMod.IsHidden());
	}

	// Token: 0x04005010 RID: 20496
	[SerializeField]
	private TMP_Text ratingsText;

	// Token: 0x04005011 RID: 20497
	[SerializeField]
	private TMP_Text mapNameText;

	// Token: 0x04005012 RID: 20498
	[SerializeField]
	private GameObject thumsbUp;

	// Token: 0x04005013 RID: 20499
	[SerializeField]
	private GameObject highlight;

	// Token: 0x04005014 RID: 20500
	private const float LOGO_WIDTH = 320f;

	// Token: 0x04005015 RID: 20501
	private const float LOGO_HEIGHT = 180f;

	// Token: 0x04005016 RID: 20502
	private Mod currentMod;

	// Token: 0x04005017 RID: 20503
	private Sprite defaultLogo;

	// Token: 0x04005018 RID: 20504
	private bool isDownloadingThumbnail;

	// Token: 0x04005019 RID: 20505
	private bool newDownloadRequest;

	// Token: 0x0400501A RID: 20506
	private bool isActive;
}
