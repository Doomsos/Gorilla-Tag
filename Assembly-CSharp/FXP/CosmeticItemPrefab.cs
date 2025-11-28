using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;

namespace FXP
{
	// Token: 0x02000E9C RID: 3740
	public class CosmeticItemPrefab : MonoBehaviour
	{
		// Token: 0x06005D80 RID: 23936 RVA: 0x001E02C7 File Offset: 0x001DE4C7
		private void Awake()
		{
			this.JonsAwakeCode();
		}

		// Token: 0x06005D81 RID: 23937 RVA: 0x001E02D0 File Offset: 0x001DE4D0
		private void JonsAwakeCode()
		{
			this.lastUpdated = -this.updateClock;
			this.isValid = (this.goPedestal && this.goMannequin && this.goCosmeticItem && this.goCosmeticItemNameplate && this.goClock && this.goPreviewMode && this.goAttractMode && this.goPurchaseMode);
			this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			this.goAttractModeSFX = OVRExtensions.FindChildRecursive(this.goAttractMode.transform, "SFXAttractMode").GetComponent<AudioSource>();
			this.goPurchaseModeSFX = OVRExtensions.FindChildRecursive(this.goPurchaseMode.transform, "SFXPurchaseMode").GetComponent<AudioSource>();
			this.goAttractModeVFX = OVRExtensions.FindChildRecursive(this.goAttractMode.transform, "VFXAttractMode").GetComponent<ParticleSystem>();
			this.goPurchaseModeVFX = OVRExtensions.FindChildRecursive(this.goPurchaseMode.transform, "VFXPurchaseMode").GetComponent<ParticleSystem>();
			this.clockTextMesh = this.goClock.GetComponent<TextMeshPro>();
			this.clockTextMeshIsValid = (this.clockTextMesh != null);
			if (this.clockTextMeshIsValid)
			{
				this.defaultCountdownTextTemplate = this.clockTextMesh.text;
			}
			this.isValid = (this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX);
		}

		// Token: 0x06005D82 RID: 23938 RVA: 0x001E0459 File Offset: 0x001DE659
		private void OnDisable()
		{
			if (StoreUpdater.instance != null)
			{
				this.countdownTimerCoRoutine = null;
				this.StopCountdownCoroutine();
				StoreUpdater.instance.PedestalAsleep(this);
			}
		}

		// Token: 0x06005D83 RID: 23939 RVA: 0x001E0484 File Offset: 0x001DE684
		private void OnEnable()
		{
			if (this.goPreviewModeSFX == null)
			{
				this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goAttractModeSFX == null)
			{
				this.goAttractModeSFX = this.goAttractMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goPurchaseModeSFX == null)
			{
				this.goPurchaseModeSFX = this.goPurchaseMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			this.isValid = (this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX);
			if (StoreUpdater.instance != null)
			{
				StoreUpdater.instance.PedestalAwakened(this);
			}
		}

		// Token: 0x06005D84 RID: 23940 RVA: 0x001E0554 File Offset: 0x001DE754
		public void SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode NewDisplayMode)
		{
			if (!this.isValid)
			{
				return;
			}
			if (NewDisplayMode.Equals(CosmeticItemPrefab.EDisplayMode.NULL))
			{
				return;
			}
			if (NewDisplayMode == this.currentDisplayMode)
			{
				return;
			}
			switch (NewDisplayMode)
			{
			case CosmeticItemPrefab.EDisplayMode.HIDDEN:
			{
				this.goPedestal.SetActive(false);
				this.goMannequin.SetActive(false);
				this.goCosmeticItem.SetActive(false);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				AudioSource audioSource = this.goPreviewModeSFX;
				if (audioSource != null)
				{
					audioSource.GTStop();
				}
				this.goAttractMode.SetActive(false);
				AudioSource audioSource2 = this.goAttractModeSFX;
				if (audioSource2 != null)
				{
					audioSource2.GTStop();
				}
				this.goPurchaseMode.SetActive(false);
				AudioSource audioSource3 = this.goPurchaseModeSFX;
				if (audioSource3 != null)
				{
					audioSource3.GTStop();
				}
				this.StopPreviewTimer();
				this.StopAttractTimer();
				break;
			}
			case CosmeticItemPrefab.EDisplayMode.PREVIEW:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(true);
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goPreviewMode.SetActive(true);
				this.goPreviewModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.ATTRACT:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(true);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goAttractMode.SetActive(true);
				this.goAttractModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartAttractTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.PURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(true);
				this.goPurchaseModeSFX.GTPlay();
				this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = "Purchased!";
				this.StopPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.POSTPURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.StopPreviewTimer();
				break;
			}
			this.currentDisplayMode = NewDisplayMode;
		}

		// Token: 0x06005D85 RID: 23941 RVA: 0x001E08A2 File Offset: 0x001DEAA2
		private void Update()
		{
			if (Time.time > this.lastUpdated + this.updateClock)
			{
				this.lastUpdated = Time.time;
				this.UpdateClock();
			}
		}

		// Token: 0x06005D86 RID: 23942 RVA: 0x001E08CC File Offset: 0x001DEACC
		private void UpdateClock()
		{
			if (this.currentUpdateEvent != null && this.clockTextMeshIsValid && this.clockTextMesh.isActiveAndEnabled)
			{
				TimeSpan ts = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				this.clockTextMesh.text = CountdownText.GetTimeDisplay(ts, this.defaultCountdownTextTemplate);
			}
		}

		// Token: 0x06005D87 RID: 23943 RVA: 0x001E0930 File Offset: 0x001DEB30
		public void SetDefaultProperties()
		{
			if (!this.isValid)
			{
				return;
			}
			this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.defaultPedestalMesh;
			this.goPedestal.GetComponent<MeshRenderer>().sharedMaterial = this.defaultPedestalMaterial;
			this.goMannequin.GetComponent<MeshFilter>().sharedMesh = this.defaultMannequinMesh;
			this.goMannequin.GetComponent<MeshRenderer>().sharedMaterial = this.defaultMannequinMaterial;
			this.goCosmeticItem.GetComponent<MeshFilter>().sharedMesh = this.defaultCosmeticMesh;
			this.goCosmeticItem.GetComponent<MeshRenderer>().sharedMaterial = this.defaultCosmeticMaterial;
			this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = this.defaultItemText;
			this.goPreviewModeSFX.clip = this.defaultSFXPreviewMode;
			this.goAttractModeSFX.clip = this.defaultSFXAttractMode;
			this.goPurchaseModeSFX.clip = this.defaultSFXPurchaseMode;
		}

		// Token: 0x06005D88 RID: 23944 RVA: 0x001E0A13 File Offset: 0x001DEC13
		private void ClearCosmeticMesh()
		{
			Object.Destroy(this.goCosmeticItemGameObject);
		}

		// Token: 0x06005D89 RID: 23945 RVA: 0x001E0A20 File Offset: 0x001DEC20
		private void ClearCosmeticAtlas()
		{
			if (this.goCosmeticItemMeshAtlas.IsNotNull())
			{
				Object.Destroy(this.goCosmeticItemMeshAtlas);
			}
		}

		// Token: 0x06005D8A RID: 23946 RVA: 0x001E0A3C File Offset: 0x001DEC3C
		public void SetCosmeticItemFromCosmeticController(CosmeticsController.CosmeticItem item)
		{
			if (!this.isValid)
			{
				return;
			}
			this.ClearCosmeticAtlas();
			this.ClearCosmeticMesh();
			this.oldItemID = this.itemID;
			this.itemID = item.itemName;
			this.itemName = item.displayName;
			if (item.overrideDisplayName != string.Empty)
			{
				this.itemName = item.overrideDisplayName;
			}
			this.HeadModel.SetCosmeticActive(this.itemID, false);
			this.SetCosmeticStand();
		}

		// Token: 0x06005D8B RID: 23947 RVA: 0x001E0AB8 File Offset: 0x001DECB8
		public void SetCosmeticStand()
		{
			this.cosmeticStand.thisCosmeticName = this.itemID;
			this.cosmeticStand.InitializeCosmetic();
			if (this.oldItemID.Length > 0)
			{
				if (this.oldItemID != this.itemID)
				{
					this.cosmeticStand.isOn = false;
				}
				this.cosmeticStand.UpdateColor();
			}
		}

		// Token: 0x06005D8C RID: 23948 RVA: 0x001E0B1C File Offset: 0x001DED1C
		public void SetStoreUpdateEvent(StoreUpdateEvent storeUpdateEvent, bool playFX)
		{
			if (!this.isValid)
			{
				return;
			}
			if (playFX)
			{
				this.goAttractMode.SetActive(true);
				this.goAttractModeVFX.Play();
			}
			this.currentUpdateEvent = storeUpdateEvent;
			this.SetCosmeticItemFromCosmeticController(CosmeticsController.instance.GetItemFromDict(storeUpdateEvent.ItemName));
			if (base.isActiveAndEnabled)
			{
				this.countdownTimerCoRoutine = base.StartCoroutine(this.PlayCountdownTimer());
			}
			this.UpdateClock();
		}

		// Token: 0x06005D8D RID: 23949 RVA: 0x001E0B8B File Offset: 0x001DED8B
		private IEnumerator PlayCountdownTimer()
		{
			yield return new WaitForSeconds(Mathf.Clamp((float)((this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted).TotalSeconds - 10.0), 0f, float.MaxValue));
			this.PlaySFX();
			yield break;
		}

		// Token: 0x06005D8E RID: 23950 RVA: 0x001E0B9A File Offset: 0x001DED9A
		public void StopCountdownCoroutine()
		{
			this.CountdownSFX.GTStop();
			this.goAttractModeVFX.Stop();
			if (this.countdownTimerCoRoutine != null)
			{
				base.StopCoroutine(this.countdownTimerCoRoutine);
				this.countdownTimerCoRoutine = null;
			}
		}

		// Token: 0x06005D8F RID: 23951 RVA: 0x001E0BD0 File Offset: 0x001DEDD0
		private void PlaySFX()
		{
			if (this.currentUpdateEvent != null)
			{
				TimeSpan timeSpan = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				if (timeSpan.TotalSeconds >= 10.0)
				{
					this.CountdownSFX.time = 0f;
					this.CountdownSFX.GTPlay();
					return;
				}
				this.CountdownSFX.time = 10f - (float)timeSpan.TotalSeconds;
				this.CountdownSFX.GTPlay();
			}
		}

		// Token: 0x06005D90 RID: 23952 RVA: 0x001E0C5C File Offset: 0x001DEE5C
		public void SetCosmeticItemProperties(string WhichGUID, string Name, List<Transform> SocketsList, int Socket, string PedestalMesh = null, string MannequinMesh = null)
		{
			if (!this.isValid)
			{
				return;
			}
			Guid guid;
			if (!Guid.TryParse(WhichGUID, ref guid))
			{
				return;
			}
			this.itemName = Name;
			this.itemSocket = Socket;
			if (this.pedestalMesh != null)
			{
				this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.pedestalMesh;
			}
		}

		// Token: 0x06005D91 RID: 23953 RVA: 0x001E0CB0 File Offset: 0x001DEEB0
		private void StartPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.coroutinePreviewTimer = this.DoPreviewTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInPreviewMode ?? this.defaultHoursInPreviewMode) * 60 * 60)));
			base.StartCoroutine(this.coroutinePreviewTimer);
		}

		// Token: 0x06005D92 RID: 23954 RVA: 0x001E0D2F File Offset: 0x001DEF2F
		private void StopPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.clockTextMesh.text = "Clock";
		}

		// Token: 0x06005D93 RID: 23955 RVA: 0x001E0D65 File Offset: 0x001DEF65
		private IEnumerator DoPreviewTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.clockTextMesh.text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.ATTRACT);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x06005D94 RID: 23956 RVA: 0x001E0D7C File Offset: 0x001DEF7C
		public void StartAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.coroutineAttractTimer = this.DoAttractTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInAttractMode ?? this.defaultHoursInAttractMode) * 60 * 60)));
			base.StartCoroutine(this.coroutineAttractTimer);
		}

		// Token: 0x06005D95 RID: 23957 RVA: 0x001E0DFB File Offset: 0x001DEFFB
		private void StopAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.goClock.GetComponent<TextMesh>().text = "Clock";
		}

		// Token: 0x06005D96 RID: 23958 RVA: 0x001E0E36 File Offset: 0x001DF036
		private IEnumerator DoAttractTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.goClock.GetComponent<TextMesh>().text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.HIDDEN);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x04006B5B RID: 27483
		public string PedestalID = "";

		// Token: 0x04006B5C RID: 27484
		public HeadModel HeadModel;

		// Token: 0x04006B5D RID: 27485
		[SerializeField]
		private Guid? itemGUID;

		// Token: 0x04006B5E RID: 27486
		[SerializeField]
		private string itemName = string.Empty;

		// Token: 0x04006B5F RID: 27487
		[SerializeField]
		private List<Transform> sockets = new List<Transform>();

		// Token: 0x04006B60 RID: 27488
		[SerializeField]
		private int itemSocket = int.MinValue;

		// Token: 0x04006B61 RID: 27489
		[SerializeField]
		private int? hoursInPreviewMode;

		// Token: 0x04006B62 RID: 27490
		[SerializeField]
		private int? hoursInAttractMode;

		// Token: 0x04006B63 RID: 27491
		[SerializeField]
		private Mesh pedestalMesh;

		// Token: 0x04006B64 RID: 27492
		[SerializeField]
		private Mesh mannequinMesh;

		// Token: 0x04006B65 RID: 27493
		[SerializeField]
		private Mesh cosmeticMesh;

		// Token: 0x04006B66 RID: 27494
		[SerializeField]
		private AudioClip sfxPreviewMode;

		// Token: 0x04006B67 RID: 27495
		[SerializeField]
		private AudioClip sfxAttractMode;

		// Token: 0x04006B68 RID: 27496
		[SerializeField]
		private AudioClip sfxPurchaseMode;

		// Token: 0x04006B69 RID: 27497
		[SerializeField]
		private ParticleSystem vfxPreviewMode;

		// Token: 0x04006B6A RID: 27498
		[SerializeField]
		private ParticleSystem vfxAttractMode;

		// Token: 0x04006B6B RID: 27499
		[SerializeField]
		private ParticleSystem vfxPurchaseMode;

		// Token: 0x04006B6C RID: 27500
		[SerializeField]
		private GameObject goPedestal;

		// Token: 0x04006B6D RID: 27501
		[SerializeField]
		private GameObject goMannequin;

		// Token: 0x04006B6E RID: 27502
		[SerializeField]
		private GameObject goCosmeticItem;

		// Token: 0x04006B6F RID: 27503
		[SerializeField]
		private GameObject goCosmeticItemGameObject;

		// Token: 0x04006B70 RID: 27504
		[SerializeField]
		private GameObject goCosmeticItemNameplate;

		// Token: 0x04006B71 RID: 27505
		[SerializeField]
		private GameObject goClock;

		// Token: 0x04006B72 RID: 27506
		[SerializeField]
		private GameObject goPreviewMode;

		// Token: 0x04006B73 RID: 27507
		[SerializeField]
		private GameObject goAttractMode;

		// Token: 0x04006B74 RID: 27508
		[SerializeField]
		private GameObject goPurchaseMode;

		// Token: 0x04006B75 RID: 27509
		[SerializeField]
		private Mesh defaultPedestalMesh;

		// Token: 0x04006B76 RID: 27510
		[SerializeField]
		private Material defaultPedestalMaterial;

		// Token: 0x04006B77 RID: 27511
		[SerializeField]
		private Mesh defaultMannequinMesh;

		// Token: 0x04006B78 RID: 27512
		[SerializeField]
		private Material defaultMannequinMaterial;

		// Token: 0x04006B79 RID: 27513
		[SerializeField]
		private Mesh defaultCosmeticMesh;

		// Token: 0x04006B7A RID: 27514
		[SerializeField]
		private Material defaultCosmeticMaterial;

		// Token: 0x04006B7B RID: 27515
		[SerializeField]
		private string defaultItemText;

		// Token: 0x04006B7C RID: 27516
		[SerializeField]
		private int defaultHoursInPreviewMode;

		// Token: 0x04006B7D RID: 27517
		[SerializeField]
		private int defaultHoursInAttractMode;

		// Token: 0x04006B7E RID: 27518
		[SerializeField]
		private AudioClip defaultSFXPreviewMode;

		// Token: 0x04006B7F RID: 27519
		[SerializeField]
		private AudioClip defaultSFXAttractMode;

		// Token: 0x04006B80 RID: 27520
		[SerializeField]
		private AudioClip defaultSFXPurchaseMode;

		// Token: 0x04006B81 RID: 27521
		private GameObject goCosmeticItemMeshAtlas;

		// Token: 0x04006B82 RID: 27522
		public AudioSource CountdownSFX;

		// Token: 0x04006B83 RID: 27523
		private CosmeticItemPrefab.EDisplayMode currentDisplayMode;

		// Token: 0x04006B84 RID: 27524
		private bool isValid;

		// Token: 0x04006B85 RID: 27525
		[Nullable(2)]
		private AudioSource goPreviewModeSFX;

		// Token: 0x04006B86 RID: 27526
		[Nullable(2)]
		private AudioSource goAttractModeSFX;

		// Token: 0x04006B87 RID: 27527
		[Nullable(2)]
		private AudioSource goPurchaseModeSFX;

		// Token: 0x04006B88 RID: 27528
		[Nullable(2)]
		private ParticleSystem goAttractModeVFX;

		// Token: 0x04006B89 RID: 27529
		[Nullable(2)]
		private ParticleSystem goPurchaseModeVFX;

		// Token: 0x04006B8A RID: 27530
		private IEnumerator coroutinePreviewTimer;

		// Token: 0x04006B8B RID: 27531
		private IEnumerator coroutineAttractTimer;

		// Token: 0x04006B8C RID: 27532
		private DateTime startTime;

		// Token: 0x04006B8D RID: 27533
		private TextMeshPro clockTextMesh;

		// Token: 0x04006B8E RID: 27534
		private bool clockTextMeshIsValid;

		// Token: 0x04006B8F RID: 27535
		private StoreUpdateEvent currentUpdateEvent;

		// Token: 0x04006B90 RID: 27536
		private string defaultCountdownTextTemplate = "";

		// Token: 0x04006B91 RID: 27537
		public CosmeticStand cosmeticStand;

		// Token: 0x04006B92 RID: 27538
		public string itemID = "";

		// Token: 0x04006B93 RID: 27539
		public string oldItemID = "";

		// Token: 0x04006B94 RID: 27540
		private Coroutine countdownTimerCoRoutine;

		// Token: 0x04006B95 RID: 27541
		private float updateClock = 60f;

		// Token: 0x04006B96 RID: 27542
		private float lastUpdated;

		// Token: 0x02000E9D RID: 3741
		[SerializeField]
		public enum EDisplayMode
		{
			// Token: 0x04006B98 RID: 27544
			NULL,
			// Token: 0x04006B99 RID: 27545
			HIDDEN,
			// Token: 0x04006B9A RID: 27546
			PREVIEW,
			// Token: 0x04006B9B RID: 27547
			ATTRACT,
			// Token: 0x04006B9C RID: 27548
			PURCHASE,
			// Token: 0x04006B9D RID: 27549
			POSTPURCHASE
		}
	}
}
