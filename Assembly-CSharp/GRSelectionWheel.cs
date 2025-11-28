using System;
using System.Collections.Generic;
using GorillaExtensions;
using TMPro;
using UnityEngine;

// Token: 0x020006FB RID: 1787
public class GRSelectionWheel : MonoBehaviour, ITickSystemTick
{
	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x06002DD4 RID: 11732 RVA: 0x000F9026 File Offset: 0x000F7226
	// (set) Token: 0x06002DD5 RID: 11733 RVA: 0x000F902E File Offset: 0x000F722E
	public bool TickRunning { get; set; }

	// Token: 0x06002DD6 RID: 11734 RVA: 0x000F9037 File Offset: 0x000F7237
	public void Start()
	{
		this.targetPage = 0;
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x0001877F File Offset: 0x0001697F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x000F9040 File Offset: 0x000F7240
	public void ShowText(bool showText)
	{
		foreach (TMP_Text tmp_Text in this.shelfNames)
		{
			tmp_Text.enabled = showText;
		}
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x000F9094 File Offset: 0x000F7294
	public void InitFromNameList(List<string> shelves)
	{
		this.shelfNames.Clear();
		for (int i = 0; i < shelves.Count; i++)
		{
			TMP_Text tmp_Text = Object.Instantiate<TMP_Text>(this.templateText);
			tmp_Text.text = shelves[i];
			this.shelfNames.Add(tmp_Text);
			tmp_Text.transform.SetParent(base.transform, false);
		}
		this.UpdateVisuals();
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x000F90FC File Offset: 0x000F72FC
	public void Tick()
	{
		if (!this.isBeingDrivenRemotely)
		{
			float num = this.deltaAngle * (float)this.shelfNames.Count;
			float num2 = this.currentAngle / this.deltaAngle;
			int num3 = (int)(num2 + 0.5f);
			if (this.rotSpeedMult == 0f)
			{
				float num4 = ((float)num3 - num2) * this.deltaAngle;
				this.currentAngle += num4 * (1f - Mathf.Exp(-20f * Time.deltaTime));
				this.targetPage = num3;
			}
			else
			{
				this.currentAngle += this.rotSpeedMult * Time.deltaTime * this.rotSpeed;
				this.currentAngle = Mathf.Clamp(this.currentAngle, -this.deltaAngle * 0.4f, num - this.deltaAngle + this.deltaAngle * 0.4f);
			}
		}
		int num5 = (int)(this.currentAngle / this.deltaAngle + 0.5f);
		if (this.lastPlayedAudioTickPage != num5)
		{
			this.lastPlayedAudioTickPage = num5;
			this.audioSource.GTPlay();
		}
		float num6 = 0.005f;
		if (Math.Abs(this.lastAngle - this.currentAngle) > num6)
		{
			this.UpdateVisuals();
		}
		this.lastAngle = this.currentAngle;
	}

	// Token: 0x06002DDC RID: 11740 RVA: 0x000F923B File Offset: 0x000F743B
	public void SetRotationSpeed(float speed)
	{
		this.rotSpeedMult = Mathf.Sign(speed) * Mathf.Pow(Mathf.Abs(speed), 2f);
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x000F925A File Offset: 0x000F745A
	public void SetTargetShelf(int shelf)
	{
		this.currentAngle += (float)(shelf - this.targetPage) * this.deltaAngle;
		this.targetPage = shelf;
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x000F9280 File Offset: 0x000F7480
	public void SetTargetAngle(float angle)
	{
		this.currentAngle = angle;
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x000F928C File Offset: 0x000F748C
	public void UpdateVisuals()
	{
		this.rotationWheel.localRotation = Quaternion.Euler(-this.currentAngle + 7.5f, 0f, 0f);
		float num = this.deltaAngle;
		int count = this.shelfNames.Count;
		float num2 = this.currentAngle / this.deltaAngle;
		for (int i = 0; i < this.shelfNames.Count; i++)
		{
			float num3 = ((float)i - num2) * this.deltaAngle + this.pointerOffsetAngle;
			float num4 = num3 * 3.1415927f / 180f;
			float num5 = Mathf.Cos(num4);
			float num6 = Mathf.Sin(num4);
			Quaternion localRotation = Quaternion.Euler(90f - num3, 180f, 0f);
			Vector3 vector;
			vector..ctor(this.textHorizOffset, num5 * this.wheelTextRadius, num6 * this.wheelTextRadius);
			this.shelfNames[i].transform.rotation = base.transform.TransformRotation(localRotation);
			this.shelfNames[i].transform.position = base.transform.TransformPoint(vector);
			this.shelfNames[i].color = ((Math.Abs(num2 - (float)i) < 0.5f) ? Color.green : Color.white);
		}
	}

	// Token: 0x04003BDB RID: 15323
	private List<TMP_Text> shelfNames = new List<TMP_Text>();

	// Token: 0x04003BDC RID: 15324
	public TMP_Text templateText;

	// Token: 0x04003BDD RID: 15325
	public float deltaAngle;

	// Token: 0x04003BDE RID: 15326
	public float pointerOffsetAngle;

	// Token: 0x04003BDF RID: 15327
	public float wheelTextRadius;

	// Token: 0x04003BE0 RID: 15328
	public float textHorizOffset = -0.0375f;

	// Token: 0x04003BE1 RID: 15329
	public float rotSpeed = 60f;

	// Token: 0x04003BE2 RID: 15330
	public bool isBeingDrivenRemotely;

	// Token: 0x04003BE3 RID: 15331
	public AudioSource audioSource;

	// Token: 0x04003BE4 RID: 15332
	public int lastPlayedAudioTickPage = -1;

	// Token: 0x04003BE5 RID: 15333
	public float wheelTextPairOffset = 0.0025f;

	// Token: 0x04003BE6 RID: 15334
	public Transform rotationWheel;

	// Token: 0x04003BE7 RID: 15335
	public float lastAngle = -1000f;

	// Token: 0x04003BE9 RID: 15337
	[NonSerialized]
	public int targetPage;

	// Token: 0x04003BEA RID: 15338
	[NonSerialized]
	public float currentAngle;

	// Token: 0x04003BEB RID: 15339
	private float rotSpeedMult;
}
