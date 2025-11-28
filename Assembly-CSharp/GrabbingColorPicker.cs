using System;
using GorillaLocomotion;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004CD RID: 1229
public class GrabbingColorPicker : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001FB8 RID: 8120 RVA: 0x000A8EFC File Offset: 0x000A70FC
	private void Start()
	{
		if (!this.setPlayerColor)
		{
			return;
		}
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		this.LoadPlayerColor(@float, float2, float3);
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x000A8F4C File Offset: 0x000A714C
	private void LoadPlayerColor(float r, float g, float b)
	{
		this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, r));
		this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, g));
		this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, b));
		this.R_PushSlider.SetProgress(r);
		this.G_PushSlider.SetProgress(g);
		this.B_PushSlider.SetProgress(b);
		this.UpdateDisplay();
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x000A8FD4 File Offset: 0x000A71D4
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.setPlayerColor)
		{
			return;
		}
		CosmeticsController.OnPlayerColorSet = (Action<float, float, float>)Delegate.Combine(CosmeticsController.OnPlayerColorSet, new Action<float, float, float>(this.LoadPlayerColor));
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged += new Action<Color>(this.HandleLocalColorChanged);
		}
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x000A904C File Offset: 0x000A724C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.setPlayerColor)
		{
			return;
		}
		CosmeticsController.OnPlayerColorSet = (Action<float, float, float>)Delegate.Remove(CosmeticsController.OnPlayerColorSet, new Action<float, float, float>(this.LoadPlayerColor));
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged -= new Action<Color>(this.HandleLocalColorChanged);
		}
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000A90C4 File Offset: 0x000A72C4
	public void SliceUpdate()
	{
		float num = Vector3.Distance(base.transform.position, GTPlayer.Instance.transform.position);
		this.hasUpdated = false;
		if (num < 5f)
		{
			int segment = this.Segment1;
			int segment2 = this.Segment2;
			int segment3 = this.Segment3;
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.R_PushSlider.GetProgress()));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.G_PushSlider.GetProgress()));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.B_PushSlider.GetProgress()));
			if (segment != this.Segment1 || segment2 != this.Segment2 || segment3 != this.Segment3)
			{
				this.hasUpdated = true;
				if (this.setPlayerColor)
				{
					this.SetPlayerColor();
				}
				this.UpdateDisplay();
				this.UpdateColor.Invoke(new Vector3((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f));
				if (segment != this.Segment1)
				{
					this.R_SliderAudio.transform.position = this.R_PushSlider.transform.position;
					this.R_SliderAudio.GTPlay();
				}
				if (segment2 != this.Segment2)
				{
					this.G_SliderAudio.transform.position = this.G_PushSlider.transform.position;
					this.G_SliderAudio.GTPlay();
				}
				if (segment3 != this.Segment3)
				{
					this.B_SliderAudio.transform.position = this.B_PushSlider.transform.position;
					this.B_SliderAudio.GTPlay();
				}
			}
		}
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000A9294 File Offset: 0x000A7494
	private void SetPlayerColor()
	{
		PlayerPrefs.SetFloat("redValue", (float)this.Segment1 / 9f);
		PlayerPrefs.SetFloat("greenValue", (float)this.Segment2 / 9f);
		PlayerPrefs.SetFloat("blueValue", (float)this.Segment3 / 9f);
		GorillaTagger.Instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		GorillaComputer.instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		PlayerPrefs.Save();
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
			{
				(float)this.Segment1 / 9f,
				(float)this.Segment2 / 9f,
				(float)this.Segment3 / 9f
			});
		}
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x000A93B8 File Offset: 0x000A75B8
	private void SetSliderColors(float r, float g, float b)
	{
		if (!this.hasUpdated)
		{
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, r));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, g));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, b));
			this.R_PushSlider.SetProgress(r);
			this.G_PushSlider.SetProgress(g);
			this.B_PushSlider.SetProgress(b);
			this.UpdateDisplay();
		}
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x000A9448 File Offset: 0x000A7648
	private void HandleLocalColorChanged(Color newColor)
	{
		this.SetSliderColors(newColor.r, newColor.g, newColor.b);
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x000A9464 File Offset: 0x000A7664
	private void UpdateDisplay()
	{
		this.textR.text = this.Segment1.ToString();
		this.textG.text = this.Segment2.ToString();
		this.textB.text = this.Segment3.ToString();
		Color color;
		color..ctor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		Renderer[] componentsInChildren = this.ColorSwatch.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].color = color;
			}
		}
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x000A9520 File Offset: 0x000A7720
	public void ResetSliders(Vector3 v)
	{
		this.SetSliderColors(v.x, v.y, v.z);
	}

	// Token: 0x04002A10 RID: 10768
	[SerializeField]
	private bool setPlayerColor = true;

	// Token: 0x04002A11 RID: 10769
	[SerializeField]
	private PushableSlider R_PushSlider;

	// Token: 0x04002A12 RID: 10770
	[SerializeField]
	private PushableSlider G_PushSlider;

	// Token: 0x04002A13 RID: 10771
	[SerializeField]
	private PushableSlider B_PushSlider;

	// Token: 0x04002A14 RID: 10772
	[SerializeField]
	private AudioSource R_SliderAudio;

	// Token: 0x04002A15 RID: 10773
	[SerializeField]
	private AudioSource G_SliderAudio;

	// Token: 0x04002A16 RID: 10774
	[SerializeField]
	private AudioSource B_SliderAudio;

	// Token: 0x04002A17 RID: 10775
	[SerializeField]
	private TextMeshPro textR;

	// Token: 0x04002A18 RID: 10776
	[SerializeField]
	private TextMeshPro textG;

	// Token: 0x04002A19 RID: 10777
	[SerializeField]
	private TextMeshPro textB;

	// Token: 0x04002A1A RID: 10778
	[SerializeField]
	private GameObject ColorSwatch;

	// Token: 0x04002A1B RID: 10779
	[SerializeField]
	private UnityEvent<Vector3> UpdateColor;

	// Token: 0x04002A1C RID: 10780
	private int Segment1;

	// Token: 0x04002A1D RID: 10781
	private int Segment2;

	// Token: 0x04002A1E RID: 10782
	private int Segment3;

	// Token: 0x04002A1F RID: 10783
	private bool hasUpdated;
}
