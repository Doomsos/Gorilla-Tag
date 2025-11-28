using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008E9 RID: 2281
public class StopwatchFace : MonoBehaviour
{
	// Token: 0x17000562 RID: 1378
	// (get) Token: 0x06003A61 RID: 14945 RVA: 0x00134171 File Offset: 0x00132371
	public bool watchActive
	{
		get
		{
			return this._watchActive;
		}
	}

	// Token: 0x17000563 RID: 1379
	// (get) Token: 0x06003A62 RID: 14946 RVA: 0x00134179 File Offset: 0x00132379
	public int millisElapsed
	{
		get
		{
			return this._millisElapsed;
		}
	}

	// Token: 0x17000564 RID: 1380
	// (get) Token: 0x06003A63 RID: 14947 RVA: 0x00134181 File Offset: 0x00132381
	public Vector3Int digitsMmSsMs
	{
		get
		{
			return StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		}
	}

	// Token: 0x06003A64 RID: 14948 RVA: 0x00134194 File Offset: 0x00132394
	public void SetMillisElapsed(int millis, bool updateFace = true)
	{
		this._millisElapsed = millis;
		if (!updateFace)
		{
			return;
		}
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06003A65 RID: 14949 RVA: 0x001341AD File Offset: 0x001323AD
	private void Awake()
	{
		this._lerpToZero = new LerpTask<int>();
		this._lerpToZero.onLerp = new Action<int, int, float>(this.OnLerpToZero);
		this._lerpToZero.onLerpEnd = new Action(this.OnLerpEnd);
	}

	// Token: 0x06003A66 RID: 14950 RVA: 0x001341E8 File Offset: 0x001323E8
	private void OnLerpToZero(int a, int b, float t)
	{
		this._millisElapsed = Mathf.FloorToInt(Mathf.Lerp((float)a, (float)b, t * t));
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x0013420D File Offset: 0x0013240D
	private void OnLerpEnd()
	{
		this.WatchReset(false);
	}

	// Token: 0x06003A68 RID: 14952 RVA: 0x0013420D File Offset: 0x0013240D
	private void OnEnable()
	{
		this.WatchReset(false);
	}

	// Token: 0x06003A69 RID: 14953 RVA: 0x0013420D File Offset: 0x0013240D
	private void OnDisable()
	{
		this.WatchReset(false);
	}

	// Token: 0x06003A6A RID: 14954 RVA: 0x00134218 File Offset: 0x00132418
	private void Update()
	{
		if (this._lerpToZero.active)
		{
			this._lerpToZero.Update();
			return;
		}
		if (this._watchActive)
		{
			this._millisElapsed += Mathf.FloorToInt(Time.deltaTime * 1000f);
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x06003A6B RID: 14955 RVA: 0x00134270 File Offset: 0x00132470
	private static Vector3Int ParseDigits(TimeSpan time)
	{
		int num = (int)time.TotalMinutes % 100;
		double num2 = 60.0 * (time.TotalMinutes - (double)num);
		int num3 = (int)num2;
		int num4 = (int)(100.0 * (num2 - (double)num3));
		num = Math.Clamp(num, 0, 99);
		num3 = Math.Clamp(num3, 0, 59);
		num4 = Math.Clamp(num4, 0, 99);
		return new Vector3Int(num, num3, num4);
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x001342D8 File Offset: 0x001324D8
	private void UpdateText()
	{
		Vector3Int vector3Int = StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		string text = vector3Int.x.ToString("D2");
		string text2 = vector3Int.y.ToString("D2");
		string text3 = vector3Int.z.ToString("D2");
		this._text.text = string.Concat(new string[]
		{
			text,
			":",
			text2,
			":",
			text3
		});
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x0013436C File Offset: 0x0013256C
	private void UpdateHand()
	{
		float num = (float)(this._millisElapsed % 60000) / 60000f * 360f;
		this._hand.localEulerAngles = new Vector3(0f, 0f, num);
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x001343AE File Offset: 0x001325AE
	public void WatchToggle()
	{
		if (!this._watchActive)
		{
			this.WatchStart();
			return;
		}
		this.WatchStop();
	}

	// Token: 0x06003A6F RID: 14959 RVA: 0x001343C5 File Offset: 0x001325C5
	public void WatchStart()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = true;
	}

	// Token: 0x06003A70 RID: 14960 RVA: 0x001343DC File Offset: 0x001325DC
	public void WatchStop()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = false;
	}

	// Token: 0x06003A71 RID: 14961 RVA: 0x001343F3 File Offset: 0x001325F3
	public void WatchReset()
	{
		this.WatchReset(true);
	}

	// Token: 0x06003A72 RID: 14962 RVA: 0x001343FC File Offset: 0x001325FC
	public void WatchReset(bool doLerp)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (doLerp)
		{
			if (!this._lerpToZero.active)
			{
				this._lerpToZero.Start(this._millisElapsed % 60000, 0, 0.36f);
				return;
			}
		}
		else
		{
			this._watchActive = false;
			this._millisElapsed = 0;
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x040049AA RID: 18858
	[SerializeField]
	private Transform _hand;

	// Token: 0x040049AB RID: 18859
	[SerializeField]
	private Text _text;

	// Token: 0x040049AC RID: 18860
	[Space]
	[SerializeField]
	private StopwatchCosmetic _cosmetic;

	// Token: 0x040049AD RID: 18861
	[Space]
	[SerializeField]
	private AudioClip _audioClick;

	// Token: 0x040049AE RID: 18862
	[SerializeField]
	private AudioClip _audioReset;

	// Token: 0x040049AF RID: 18863
	[SerializeField]
	private AudioClip _audioTick;

	// Token: 0x040049B0 RID: 18864
	[Space]
	[NonSerialized]
	private int _millisElapsed;

	// Token: 0x040049B1 RID: 18865
	[NonSerialized]
	private bool _watchActive;

	// Token: 0x040049B2 RID: 18866
	[NonSerialized]
	private LerpTask<int> _lerpToZero;
}
