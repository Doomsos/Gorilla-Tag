using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio;
using Modio.Images;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x0200098E RID: 2446
public class NewMapsDisplay : MonoBehaviour
{
	// Token: 0x06003E3D RID: 15933 RVA: 0x0014BC44 File Offset: 0x00149E44
	public void OnEnable()
	{
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
		if (!UGCPermissionManager.IsUGCDisabled)
		{
			if (!ModIOManager.IsInitialized() || !ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
			{
				this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
			}
			else
			{
				if (this.newMapsModId == ModId.Null)
				{
					return;
				}
				this.Initialize();
			}
		}
		this.loadingText.gameObject.SetActive(true);
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x0014BD00 File Offset: 0x00149F00
	public void OnDisable()
	{
		if (this.initCoroutine != null)
		{
			base.StopCoroutine(this.initCoroutine);
			this.initCoroutine = null;
		}
		this.newMapsModProfile = null;
		this.newMapDatas.Clear();
		this.slideshowActive = false;
		this.slideshowIndex = 0;
		this.lastSlideshowUpdate = 0f;
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		this.loadingText.text = this.loadingString;
		this.loadingText.gameObject.SetActive(false);
		UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x0014BDCC File Offset: 0x00149FCC
	private void OnUGCEnabled()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			if (!ModIOManager.IsInitialized() || !ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
			{
				this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
				return;
			}
			if (this.newMapsModId == ModId.Null)
			{
				return;
			}
			this.Initialize();
		}
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x0014BE28 File Offset: 0x0014A028
	private void OnUGCDisabled()
	{
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		this.loadingText.text = this.ugcDisabledString;
		this.loadingText.gameObject.SetActive(true);
	}

	// Token: 0x06003E41 RID: 15937 RVA: 0x0014BE89 File Offset: 0x0014A089
	private IEnumerator DelayedInitialize()
	{
		while (!ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
		{
			yield return new WaitForSecondsRealtime(1f);
		}
		this.initCoroutine = null;
		if (this.newMapsModId == ModId.Null)
		{
			yield break;
		}
		this.Initialize();
		yield break;
	}

	// Token: 0x06003E42 RID: 15938 RVA: 0x0014BE98 File Offset: 0x0014A098
	private Task<Error> Initialize()
	{
		NewMapsDisplay.<Initialize>d__28 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<NewMapsDisplay.<Initialize>d__28>(ref <Initialize>d__);
		return <Initialize>d__.<>t__builder.Task;
	}

	// Token: 0x06003E43 RID: 15939 RVA: 0x0014BEDB File Offset: 0x0014A0DB
	private void StartSlideshow()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			return;
		}
		this.slideshowIndex = 0;
		this.slideshowActive = true;
		this.UpdateSlideshow();
	}

	// Token: 0x06003E44 RID: 15940 RVA: 0x0014BEFF File Offset: 0x0014A0FF
	public void Update()
	{
		if (!this.slideshowActive || Time.time - this.lastSlideshowUpdate < this.slideshowUpdateInterval)
		{
			return;
		}
		this.UpdateSlideshow();
	}

	// Token: 0x06003E45 RID: 15941 RVA: 0x0014BF24 File Offset: 0x0014A124
	private void UpdateSlideshow()
	{
		this.loadingText.gameObject.SetActive(false);
		this.lastSlideshowUpdate = Time.time;
		Texture2D image = this.newMapDatas[this.slideshowIndex].image;
		if (image != null)
		{
			Sprite sprite;
			if (!this.cachedTextures.TryGetValue(image, ref sprite))
			{
				sprite = Sprite.Create(image, new Rect(0f, 0f, (float)image.width, (float)image.height), new Vector2(0.5f, 0.5f));
				this.cachedTextures.Add(image, sprite);
			}
			this.mapImage.sprite = sprite;
			this.mapImage.gameObject.SetActive(true);
		}
		else
		{
			this.mapImage.gameObject.SetActive(false);
		}
		this.mapInfoTMP.text = this.newMapDatas[this.slideshowIndex].info;
		this.mapInfoTMP.gameObject.SetActive(true);
		this.slideshowIndex++;
		if (this.slideshowIndex >= this.newMapDatas.Count)
		{
			this.slideshowIndex = 0;
		}
	}

	// Token: 0x04004F19 RID: 20249
	[SerializeField]
	private SpriteRenderer mapImage;

	// Token: 0x04004F1A RID: 20250
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04004F1B RID: 20251
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x04004F1C RID: 20252
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x04004F1D RID: 20253
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x04004F1E RID: 20254
	[SerializeField]
	private TMP_Text mapInfoTMP;

	// Token: 0x04004F1F RID: 20255
	[SerializeField]
	private float slideshowUpdateInterval = 1f;

	// Token: 0x04004F20 RID: 20256
	[SerializeField]
	private string loadingString = "LOADING...";

	// Token: 0x04004F21 RID: 20257
	[SerializeField]
	private string ugcDisabledString = "UGC DISABLED BY K-ID SETTINGS";

	// Token: 0x04004F22 RID: 20258
	private ModId newMapsModId = ModId.Null;

	// Token: 0x04004F23 RID: 20259
	private Mod newMapsModProfile;

	// Token: 0x04004F24 RID: 20260
	private List<NewMapsDisplay.NewMapData> newMapDatas = new List<NewMapsDisplay.NewMapData>();

	// Token: 0x04004F25 RID: 20261
	private bool slideshowActive;

	// Token: 0x04004F26 RID: 20262
	private int slideshowIndex;

	// Token: 0x04004F27 RID: 20263
	private float lastSlideshowUpdate;

	// Token: 0x04004F28 RID: 20264
	private bool requestingNewMapsModProfile;

	// Token: 0x04004F29 RID: 20265
	private LazyImage<Texture2D> lazyImage;

	// Token: 0x04004F2A RID: 20266
	private bool downloadingImages;

	// Token: 0x04004F2B RID: 20267
	private bool downloadingImage;

	// Token: 0x04004F2C RID: 20268
	private Texture2D lastDownloadedImage;

	// Token: 0x04004F2D RID: 20269
	private Coroutine initCoroutine;

	// Token: 0x04004F2E RID: 20270
	private Dictionary<Texture2D, Sprite> cachedTextures = new Dictionary<Texture2D, Sprite>();

	// Token: 0x0200098F RID: 2447
	private struct NewMapData
	{
		// Token: 0x04004F2F RID: 20271
		public Texture2D image;

		// Token: 0x04004F30 RID: 20272
		public string info;
	}
}
