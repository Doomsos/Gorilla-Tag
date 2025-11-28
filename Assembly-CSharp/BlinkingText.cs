using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000179 RID: 377
public class BlinkingText : MonoBehaviour
{
	// Token: 0x06000A13 RID: 2579 RVA: 0x000367F0 File Offset: 0x000349F0
	private void Awake()
	{
		this.textComponent = base.GetComponent<Text>();
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00036800 File Offset: 0x00034A00
	private void Update()
	{
		if (this.isOn && Time.time > this.lastTime + this.cycleTime * this.dutyCycle)
		{
			this.isOn = false;
			this.textComponent.enabled = false;
			return;
		}
		if (!this.isOn && Time.time > this.lastTime + this.cycleTime)
		{
			this.lastTime = Time.time;
			this.isOn = true;
			this.textComponent.enabled = true;
		}
	}

	// Token: 0x04000C62 RID: 3170
	public float cycleTime;

	// Token: 0x04000C63 RID: 3171
	public float dutyCycle;

	// Token: 0x04000C64 RID: 3172
	private bool isOn;

	// Token: 0x04000C65 RID: 3173
	private float lastTime;

	// Token: 0x04000C66 RID: 3174
	private Text textComponent;
}
