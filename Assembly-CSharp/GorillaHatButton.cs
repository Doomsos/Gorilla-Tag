using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200078A RID: 1930
[Obsolete("This class is obsolete and will be removed in a future version. (MattO 2024-02-26) It doesn't appear to be used anywhere.")]
public class GorillaHatButton : MonoBehaviour
{
	// Token: 0x06003296 RID: 12950 RVA: 0x00110868 File Offset: 0x0010EA68
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			if (this.touchTime + this.debounceTime < Time.time)
			{
				this.touchTime = Time.time;
				this.isOn = !this.isOn;
				this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			}
		}
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x001108D0 File Offset: 0x0010EAD0
	private void OnTriggerEnter(Collider collider)
	{
		if (this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			this.isOn = !this.isOn;
			this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x06003298 RID: 12952 RVA: 0x00110970 File Offset: 0x0010EB70
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x040040E6 RID: 16614
	public GorillaHatButtonParent buttonParent;

	// Token: 0x040040E7 RID: 16615
	public GorillaHatButton.HatButtonType buttonType;

	// Token: 0x040040E8 RID: 16616
	public bool isOn;

	// Token: 0x040040E9 RID: 16617
	public Material offMaterial;

	// Token: 0x040040EA RID: 16618
	public Material onMaterial;

	// Token: 0x040040EB RID: 16619
	public string offText;

	// Token: 0x040040EC RID: 16620
	public string onText;

	// Token: 0x040040ED RID: 16621
	public Text myText;

	// Token: 0x040040EE RID: 16622
	public float debounceTime = 0.25f;

	// Token: 0x040040EF RID: 16623
	public float touchTime;

	// Token: 0x040040F0 RID: 16624
	public string cosmeticName;

	// Token: 0x040040F1 RID: 16625
	public bool testPress;

	// Token: 0x0200078B RID: 1931
	public enum HatButtonType
	{
		// Token: 0x040040F3 RID: 16627
		Hat,
		// Token: 0x040040F4 RID: 16628
		Face,
		// Token: 0x040040F5 RID: 16629
		Badge
	}
}
