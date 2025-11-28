using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000918 RID: 2328
public class GorillaPlayerLineButton : MonoBehaviour
{
	// Token: 0x06003B75 RID: 15221 RVA: 0x0013A05C File Offset: 0x0013825C
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	// Token: 0x06003B76 RID: 15222 RVA: 0x0013A072 File Offset: 0x00138272
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	// Token: 0x06003B77 RID: 15223 RVA: 0x0013A081 File Offset: 0x00138281
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
				{
					this.isOn = !this.isOn;
				}
				this.parentLine.PressButton(this.isOn, this.buttonType);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06003B78 RID: 15224 RVA: 0x0013A090 File Offset: 0x00138290
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
			{
				if (this.isAutoOn)
				{
					this.isOn = false;
				}
				else
				{
					this.isOn = !this.isOn;
				}
			}
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute || this.buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech || this.buttonType == GorillaPlayerLineButton.ButtonType.Cheating || this.buttonType == GorillaPlayerLineButton.ButtonType.Cancel || this.parentLine.canPressNextReportButton)
			{
				this.parentLine.PressButton(this.isOn, this.buttonType);
				if (component != null)
				{
					GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
					if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
						{
							67,
							component.isLeftHand,
							0.05f
						});
					}
				}
			}
		}
	}

	// Token: 0x06003B79 RID: 15225 RVA: 0x0013A200 File Offset: 0x00138400
	private void OnTriggerExit(Collider other)
	{
		if (this.buttonType != GorillaPlayerLineButton.ButtonType.Mute && other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.parentLine.canPressNextReportButton = true;
		}
	}

	// Token: 0x06003B7A RID: 15226 RVA: 0x0013A228 File Offset: 0x00138428
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		if (this.isAutoOn)
		{
			base.GetComponent<MeshRenderer>().material = this.autoOnMaterial;
			this.myText.text = this.autoOnText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x04004BDE RID: 19422
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x04004BDF RID: 19423
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x04004BE0 RID: 19424
	public bool isOn;

	// Token: 0x04004BE1 RID: 19425
	public bool isAutoOn;

	// Token: 0x04004BE2 RID: 19426
	public Material offMaterial;

	// Token: 0x04004BE3 RID: 19427
	public Material onMaterial;

	// Token: 0x04004BE4 RID: 19428
	public Material autoOnMaterial;

	// Token: 0x04004BE5 RID: 19429
	public string offText;

	// Token: 0x04004BE6 RID: 19430
	public string onText;

	// Token: 0x04004BE7 RID: 19431
	public string autoOnText;

	// Token: 0x04004BE8 RID: 19432
	public Text myText;

	// Token: 0x04004BE9 RID: 19433
	public float debounceTime = 0.25f;

	// Token: 0x04004BEA RID: 19434
	public float touchTime;

	// Token: 0x04004BEB RID: 19435
	public bool testPress;

	// Token: 0x02000919 RID: 2329
	public enum ButtonType
	{
		// Token: 0x04004BED RID: 19437
		HateSpeech,
		// Token: 0x04004BEE RID: 19438
		Cheating,
		// Token: 0x04004BEF RID: 19439
		Toxicity,
		// Token: 0x04004BF0 RID: 19440
		Mute,
		// Token: 0x04004BF1 RID: 19441
		Report,
		// Token: 0x04004BF2 RID: 19442
		Cancel
	}
}
