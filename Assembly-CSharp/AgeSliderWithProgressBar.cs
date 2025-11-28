using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000A02 RID: 2562
public class AgeSliderWithProgressBar : MonoBehaviourTick
{
	// Token: 0x17000615 RID: 1557
	// (get) Token: 0x0600418C RID: 16780 RVA: 0x0015C540 File Offset: 0x0015A740
	// (set) Token: 0x0600418D RID: 16781 RVA: 0x0015C548 File Offset: 0x0015A748
	public AgeSliderWithProgressBar.SliderHeldEvent onHoldComplete
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

	// Token: 0x17000616 RID: 1558
	// (get) Token: 0x0600418E RID: 16782 RVA: 0x0015C551 File Offset: 0x0015A751
	public bool AdjustAge
	{
		get
		{
			return this._adjustAge;
		}
	}

	// Token: 0x17000617 RID: 1559
	// (get) Token: 0x0600418F RID: 16783 RVA: 0x0015C559 File Offset: 0x0015A759
	// (set) Token: 0x06004190 RID: 16784 RVA: 0x0015C561 File Offset: 0x0015A761
	public bool ControllerActive
	{
		get
		{
			return this.controllerActive;
		}
		set
		{
			if (value)
			{
				ControllerBehaviour.Instance.OnAction += this.PostUpdate;
			}
			else
			{
				ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
			}
			this.controllerActive = value;
		}
	}

	// Token: 0x17000618 RID: 1560
	// (get) Token: 0x06004191 RID: 16785 RVA: 0x0015C59B File Offset: 0x0015A79B
	// (set) Token: 0x06004192 RID: 16786 RVA: 0x0015C5A3 File Offset: 0x0015A7A3
	public string LockMessage
	{
		get
		{
			return this._lockMessage;
		}
		set
		{
			this._lockMessage = value;
		}
	}

	// Token: 0x17000619 RID: 1561
	// (get) Token: 0x06004193 RID: 16787 RVA: 0x0015C5AC File Offset: 0x0015A7AC
	public int CurrentAge
	{
		get
		{
			return this._currentAge;
		}
	}

	// Token: 0x06004194 RID: 16788 RVA: 0x0015C5B4 File Offset: 0x0015A7B4
	private void Awake()
	{
		if (this._messageText)
		{
			this._originalText = this._messageText.text;
		}
	}

	// Token: 0x06004195 RID: 16789 RVA: 0x0015C5D4 File Offset: 0x0015A7D4
	public void SetOriginalText(string text)
	{
		this._originalText = text;
	}

	// Token: 0x06004196 RID: 16790 RVA: 0x0015C5E0 File Offset: 0x0015A7E0
	private new void OnEnable()
	{
		base.OnEnable();
		if (this._progressBarContainer != null && this.progressBarFill != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
		}
	}

	// Token: 0x06004197 RID: 16791 RVA: 0x0015C668 File Offset: 0x0015A868
	public override void Tick()
	{
		if (!this._progressBarContainer)
		{
			return;
		}
		if (!this.ControllerActive)
		{
			return;
		}
		if (!this._lockMessage.IsNullOrEmpty())
		{
			this.progress = 0f;
			if (this._messageText)
			{
				this._messageText.text = this.LockMessage;
			}
		}
		else
		{
			if (this._messageText)
			{
				this._messageText.text = this._originalText;
			}
			if ((double)this.progress == 1.0)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				this.progress = 0f;
			}
			if (ControllerBehaviour.Instance.ButtonDown && this._progressBarContainer != null && (this._currentAge > 0 || !this.AdjustAge))
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progress = Mathf.Clamp01(this.progress);
			}
			else
			{
				this.progress = 0f;
			}
		}
		if (this._progressBarContainer != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(this.progress, 1f, 1f);
		}
	}

	// Token: 0x06004198 RID: 16792 RVA: 0x0015C7AC File Offset: 0x0015A9AC
	private void PostUpdate()
	{
		if (this.ControllerActive && this._ageValueTxt && this._ageSlidable && !this._incrementButtonsLockingSlider)
		{
			if (ControllerBehaviour.Instance.IsLeftStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
			if (ControllerBehaviour.Instance.IsRightStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = this.GetAgeString();
			if (this._progressBarContainer != null)
			{
				this._progressBarContainer.SetActive(this._currentAge > 0);
			}
		}
	}

	// Token: 0x06004199 RID: 16793 RVA: 0x0015C8D0 File Offset: 0x0015AAD0
	public void EnableEditing()
	{
		this._ageSlidable = true;
	}

	// Token: 0x0600419A RID: 16794 RVA: 0x0015C8D9 File Offset: 0x0015AAD9
	public void DisableEditing()
	{
		this._ageSlidable = false;
	}

	// Token: 0x0600419B RID: 16795 RVA: 0x0015C8E4 File Offset: 0x0015AAE4
	public string GetAgeString()
	{
		if (this._confirmButton)
		{
			this._confirmButton.interactable = true;
		}
		if (this._currentAge == 0)
		{
			if (this._confirmButton)
			{
				this._confirmButton.interactable = false;
			}
			return "?";
		}
		if (this._currentAge == this._maxAge)
		{
			return this._maxAge.ToString() + "+";
		}
		return this._currentAge.ToString();
	}

	// Token: 0x0600419C RID: 16796 RVA: 0x0015C960 File Offset: 0x0015AB60
	public void ForceAddAge(int number)
	{
		this._incrementButtonsLockingSlider = true;
		this._currentAge = Math.Min(this._currentAge + number, this._maxAge);
	}

	// Token: 0x0600419D RID: 16797 RVA: 0x0015C982 File Offset: 0x0015AB82
	public void ForceSubtractAge(int number)
	{
		this._incrementButtonsLockingSlider = true;
		this._currentAge = Math.Max(this._currentAge - number, 1);
	}

	// Token: 0x0400526B RID: 21099
	private const int MIN_AGE = 13;

	// Token: 0x0400526C RID: 21100
	[SerializeField]
	private AgeSliderWithProgressBar.SliderHeldEvent m_OnHoldComplete = new AgeSliderWithProgressBar.SliderHeldEvent();

	// Token: 0x0400526D RID: 21101
	[SerializeField]
	private bool _adjustAge;

	// Token: 0x0400526E RID: 21102
	[SerializeField]
	private int _maxAge = 25;

	// Token: 0x0400526F RID: 21103
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04005270 RID: 21104
	[Tooltip("Optional game object that should hold the Progress Bar Fill. Disables Hold functionality if null.")]
	[SerializeField]
	private GameObject _progressBarContainer;

	// Token: 0x04005271 RID: 21105
	[SerializeField]
	private float holdTime = 2.5f;

	// Token: 0x04005272 RID: 21106
	[SerializeField]
	private Image progressBarFill;

	// Token: 0x04005273 RID: 21107
	[SerializeField]
	private TMP_Text _messageText;

	// Token: 0x04005274 RID: 21108
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x04005275 RID: 21109
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x04005276 RID: 21110
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005277 RID: 21111
	private bool _ageSlidable = true;

	// Token: 0x04005278 RID: 21112
	private bool _incrementButtonsLockingSlider;

	// Token: 0x04005279 RID: 21113
	private bool controllerActive;

	// Token: 0x0400527A RID: 21114
	[SerializeField]
	private string _lockMessage;

	// Token: 0x0400527B RID: 21115
	private string _originalText;

	// Token: 0x0400527C RID: 21116
	private int _currentAge;

	// Token: 0x0400527D RID: 21117
	private float progress;

	// Token: 0x02000A03 RID: 2563
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
