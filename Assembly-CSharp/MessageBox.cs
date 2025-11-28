using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A6D RID: 2669
public class MessageBox : MonoBehaviour
{
	// Token: 0x17000658 RID: 1624
	// (get) Token: 0x0600431E RID: 17182 RVA: 0x001643E7 File Offset: 0x001625E7
	// (set) Token: 0x0600431F RID: 17183 RVA: 0x001643EF File Offset: 0x001625EF
	public MessageBoxResult Result { get; private set; }

	// Token: 0x17000659 RID: 1625
	// (get) Token: 0x06004320 RID: 17184 RVA: 0x001643F8 File Offset: 0x001625F8
	// (set) Token: 0x06004321 RID: 17185 RVA: 0x00164405 File Offset: 0x00162605
	public string Header
	{
		get
		{
			return this._headerText.text;
		}
		set
		{
			this._headerText.text = value;
			this._headerText.gameObject.SetActive(!string.IsNullOrEmpty(value));
		}
	}

	// Token: 0x1700065A RID: 1626
	// (get) Token: 0x06004322 RID: 17186 RVA: 0x0016442C File Offset: 0x0016262C
	// (set) Token: 0x06004323 RID: 17187 RVA: 0x00164439 File Offset: 0x00162639
	public string Body
	{
		get
		{
			return this._bodyText.text;
		}
		set
		{
			this._bodyText.text = value;
		}
	}

	// Token: 0x1700065B RID: 1627
	// (get) Token: 0x06004324 RID: 17188 RVA: 0x00164447 File Offset: 0x00162647
	// (set) Token: 0x06004325 RID: 17189 RVA: 0x00164454 File Offset: 0x00162654
	public string LeftButton
	{
		get
		{
			return this._leftButtonText.text;
		}
		set
		{
			this._leftButtonText.text = value;
			this._leftButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._rightButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition = Vector3.zero;
				return;
			}
			RectTransform component2 = this._rightButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(1f, 0.5f);
			component2.anchorMax = new Vector2(1f, 0.5f);
			component2.pivot = new Vector2(1f, 0.5f);
			component2.anchoredPosition = Vector3.zero;
		}
	}

	// Token: 0x1700065C RID: 1628
	// (get) Token: 0x06004326 RID: 17190 RVA: 0x0016453C File Offset: 0x0016273C
	// (set) Token: 0x06004327 RID: 17191 RVA: 0x0016454C File Offset: 0x0016274C
	public string RightButton
	{
		get
		{
			return this._rightButtonText.text;
		}
		set
		{
			this._rightButtonText.text = value;
			this._rightButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._leftButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition3D = Vector3.zero;
				return;
			}
			RectTransform component2 = this._leftButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(0f, 0.5f);
			component2.anchorMax = new Vector2(0f, 0.5f);
			component2.pivot = new Vector2(0f, 0.5f);
			component2.anchoredPosition3D = Vector3.zero;
		}
	}

	// Token: 0x1700065D RID: 1629
	// (get) Token: 0x06004328 RID: 17192 RVA: 0x0016462A File Offset: 0x0016282A
	public UnityEvent LeftButtonCallback
	{
		get
		{
			return this._leftButtonCallback;
		}
	}

	// Token: 0x1700065E RID: 1630
	// (get) Token: 0x06004329 RID: 17193 RVA: 0x00164632 File Offset: 0x00162832
	public UnityEvent RightButtonCallback
	{
		get
		{
			return this._rightButtonCallback;
		}
	}

	// Token: 0x0600432A RID: 17194 RVA: 0x0016463A File Offset: 0x0016283A
	private void Start()
	{
		this.Result = MessageBoxResult.None;
	}

	// Token: 0x0600432B RID: 17195 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x0600432C RID: 17196 RVA: 0x00164644 File Offset: 0x00162844
	public void ShowQuitButtonAsPrimary()
	{
		this._leftButton.SetActive(false);
		RectTransform component = this._rightButton.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.pivot = new Vector2(0.5f, 0.5f);
		component.anchoredPosition = Vector3.zero;
	}

	// Token: 0x0600432D RID: 17197 RVA: 0x001646B6 File Offset: 0x001628B6
	public void OnClickLeftButton()
	{
		this.Result = MessageBoxResult.Left;
		this._leftButtonCallback.Invoke();
	}

	// Token: 0x0600432E RID: 17198 RVA: 0x001646CA File Offset: 0x001628CA
	public void OnClickRightButton()
	{
		this.Result = MessageBoxResult.Right;
		this._rightButtonCallback.Invoke();
	}

	// Token: 0x0600432F RID: 17199 RVA: 0x001646DE File Offset: 0x001628DE
	public GameObject GetCanvas()
	{
		return base.GetComponentInChildren<Canvas>(true).gameObject;
	}

	// Token: 0x06004330 RID: 17200 RVA: 0x001646EC File Offset: 0x001628EC
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x040054A7 RID: 21671
	[SerializeField]
	private TMP_Text _headerText;

	// Token: 0x040054A8 RID: 21672
	[SerializeField]
	private TMP_Text _bodyText;

	// Token: 0x040054A9 RID: 21673
	[SerializeField]
	private TMP_Text _leftButtonText;

	// Token: 0x040054AA RID: 21674
	[SerializeField]
	private TMP_Text _rightButtonText;

	// Token: 0x040054AB RID: 21675
	[SerializeField]
	private GameObject _leftButton;

	// Token: 0x040054AC RID: 21676
	[SerializeField]
	private GameObject _rightButton;

	// Token: 0x040054AE RID: 21678
	[SerializeField]
	private UnityEvent _leftButtonCallback = new UnityEvent();

	// Token: 0x040054AF RID: 21679
	[SerializeField]
	private UnityEvent _rightButtonCallback = new UnityEvent();
}
