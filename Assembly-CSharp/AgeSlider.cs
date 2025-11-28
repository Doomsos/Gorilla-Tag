using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A00 RID: 2560
public class AgeSlider : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000614 RID: 1556
	// (get) Token: 0x06004182 RID: 16770 RVA: 0x0015C2B9 File Offset: 0x0015A4B9
	// (set) Token: 0x06004183 RID: 16771 RVA: 0x0015C2C1 File Offset: 0x0015A4C1
	public AgeSlider.SliderHeldEvent onHoldComplete
	{
		get
		{
			return this.m_OnHoldComplete;
		}
		set
		{
			this.m_OnHoldComplete = value;
		}
	}

	// Token: 0x06004184 RID: 16772 RVA: 0x0015C2CA File Offset: 0x0015A4CA
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004185 RID: 16773 RVA: 0x0015C2EE File Offset: 0x0015A4EE
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06004186 RID: 16774 RVA: 0x0015C314 File Offset: 0x0015A514
	protected void Update()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (ControllerBehaviour.Instance.ButtonDown && this._confirmButton.activeInHierarchy)
		{
			this.progress += Time.deltaTime / this.holdTime;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			if (this.progress >= 1f)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				return;
			}
		}
		else
		{
			this.progress = 0f;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
		}
	}

	// Token: 0x06004187 RID: 16775 RVA: 0x0015C420 File Offset: 0x0015A620
	private void PostUpdate()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (ControllerBehaviour.Instance.IsLeftStick || ControllerBehaviour.Instance.IsUpStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
		if (ControllerBehaviour.Instance.IsRightStick || ControllerBehaviour.Instance.IsDownStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
	}

	// Token: 0x06004188 RID: 16776 RVA: 0x0015C50D File Offset: 0x0015A70D
	public static void ToggleAgeGate(bool state)
	{
		AgeSlider._ageGateActive = state;
	}

	// Token: 0x06004189 RID: 16777 RVA: 0x0015C515 File Offset: 0x0015A715
	public bool BuildValidationCheck()
	{
		if (this._confirmButton == null)
		{
			Debug.LogError("[KID] Object [_confirmButton] is NULL. Must be assigned in editor");
			return false;
		}
		return true;
	}

	// Token: 0x04005261 RID: 21089
	private const int MIN_AGE = 13;

	// Token: 0x04005262 RID: 21090
	[SerializeField]
	private AgeSlider.SliderHeldEvent m_OnHoldComplete = new AgeSlider.SliderHeldEvent();

	// Token: 0x04005263 RID: 21091
	[SerializeField]
	private int _maxAge = 99;

	// Token: 0x04005264 RID: 21092
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04005265 RID: 21093
	[SerializeField]
	private GameObject _confirmButton;

	// Token: 0x04005266 RID: 21094
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x04005267 RID: 21095
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x04005268 RID: 21096
	private int _currentAge;

	// Token: 0x04005269 RID: 21097
	private static bool _ageGateActive;

	// Token: 0x0400526A RID: 21098
	private float progress;

	// Token: 0x02000A01 RID: 2561
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
