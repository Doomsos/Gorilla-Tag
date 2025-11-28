using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000404 RID: 1028
public class GorillaReportButton : MonoBehaviour
{
	// Token: 0x0600191C RID: 6428 RVA: 0x00086622 File Offset: 0x00084822
	public void AssignParentLine(GorillaPlayerScoreboardLine parent)
	{
		this.parentLine = parent;
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x0008662C File Offset: 0x0008482C
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time)
		{
			this.isOn = !this.isOn;
			this.UpdateColor();
			this.selected = !this.selected;
			this.touchTime = Time.time;
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.05f);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
				{
					67,
					false,
					0.05f
				});
			}
		}
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x0008671F File Offset: 0x0008491F
	private void OnTriggerExit(Collider other)
	{
		if (this.metaReportType != GorillaReportButton.MetaReportReason.Cancel)
		{
			other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null;
		}
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x00086737 File Offset: 0x00084937
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
	}

	// Token: 0x0400226B RID: 8811
	public GorillaReportButton.MetaReportReason metaReportType;

	// Token: 0x0400226C RID: 8812
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x0400226D RID: 8813
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x0400226E RID: 8814
	public bool isOn;

	// Token: 0x0400226F RID: 8815
	public Material offMaterial;

	// Token: 0x04002270 RID: 8816
	public Material onMaterial;

	// Token: 0x04002271 RID: 8817
	public string offText;

	// Token: 0x04002272 RID: 8818
	public string onText;

	// Token: 0x04002273 RID: 8819
	public Text myText;

	// Token: 0x04002274 RID: 8820
	public float debounceTime = 0.25f;

	// Token: 0x04002275 RID: 8821
	public float touchTime;

	// Token: 0x04002276 RID: 8822
	public bool testPress;

	// Token: 0x04002277 RID: 8823
	public bool selected;

	// Token: 0x02000405 RID: 1029
	[SerializeField]
	public enum MetaReportReason
	{
		// Token: 0x04002279 RID: 8825
		HateSpeech,
		// Token: 0x0400227A RID: 8826
		Cheating,
		// Token: 0x0400227B RID: 8827
		Toxicity,
		// Token: 0x0400227C RID: 8828
		Bullying,
		// Token: 0x0400227D RID: 8829
		Doxing,
		// Token: 0x0400227E RID: 8830
		Impersonation,
		// Token: 0x0400227F RID: 8831
		Submit,
		// Token: 0x04002280 RID: 8832
		Cancel
	}
}
