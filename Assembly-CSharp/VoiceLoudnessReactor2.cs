using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020004F9 RID: 1273
public class VoiceLoudnessReactor2 : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000374 RID: 884
	// (get) Token: 0x060020C1 RID: 8385 RVA: 0x000ADAD1 File Offset: 0x000ABCD1
	private float Loudness
	{
		get
		{
			return this.gsl.Loudness * this.sensitivity;
		}
	}

	// Token: 0x060020C2 RID: 8386 RVA: 0x000ADAE8 File Offset: 0x000ABCE8
	private void OnEnable()
	{
		if (this.continuousProperties.Count == 0)
		{
			return;
		}
		if (this.gsl == null)
		{
			this.gsl = base.GetComponentInParent<GorillaSpeakerLoudness>(true);
			if (this.gsl == null)
			{
				GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
				if (componentInParent != null)
				{
					this.gsl = componentInParent.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
					if (this.gsl == null)
					{
						return;
					}
				}
			}
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060020C3 RID: 8387 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x060020C4 RID: 8388 RVA: 0x000ADB62 File Offset: 0x000ABD62
	// (set) Token: 0x060020C5 RID: 8389 RVA: 0x000ADB6A File Offset: 0x000ABD6A
	public bool TickRunning { get; set; }

	// Token: 0x060020C6 RID: 8390 RVA: 0x000ADB73 File Offset: 0x000ABD73
	public void Tick()
	{
		this.continuousProperties.ApplyAll(this.Loudness);
	}

	// Token: 0x04002B57 RID: 11095
	[Tooltip("Multiply the microphone input by this value. A good default is 15.")]
	public float sensitivity = 15f;

	// Token: 0x04002B58 RID: 11096
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x04002B59 RID: 11097
	private GorillaSpeakerLoudness gsl;
}
