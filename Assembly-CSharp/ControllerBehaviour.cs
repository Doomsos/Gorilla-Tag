using System;
using UnityEngine;

// Token: 0x02000AC8 RID: 2760
[Obsolete("Use ControllerInputPoller instead", false)]
public class ControllerBehaviour : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000674 RID: 1652
	// (get) Token: 0x060044FA RID: 17658 RVA: 0x0016DAA6 File Offset: 0x0016BCA6
	// (set) Token: 0x060044FB RID: 17659 RVA: 0x0016DAAD File Offset: 0x0016BCAD
	public static ControllerBehaviour Instance { get; private set; }

	// Token: 0x17000675 RID: 1653
	// (get) Token: 0x060044FC RID: 17660 RVA: 0x0016DAB5 File Offset: 0x0016BCB5
	private ControllerInputPoller Poller
	{
		get
		{
			if (this.poller != null)
			{
				return this.poller;
			}
			if (ControllerInputPoller.instance != null)
			{
				this.poller = ControllerInputPoller.instance;
				return this.poller;
			}
			return null;
		}
	}

	// Token: 0x17000676 RID: 1654
	// (get) Token: 0x060044FD RID: 17661 RVA: 0x0016DAF0 File Offset: 0x0016BCF0
	public bool ButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerPrimaryButton || this.Poller.leftControllerSecondaryButton || this.Poller.rightControllerPrimaryButton || this.Poller.rightControllerSecondaryButton);
		}
	}

	// Token: 0x17000677 RID: 1655
	// (get) Token: 0x060044FE RID: 17662 RVA: 0x0016DB41 File Offset: 0x0016BD41
	public bool LeftButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerPrimaryButton || this.Poller.leftControllerSecondaryButton || this.Poller.leftControllerTriggerButton);
		}
	}

	// Token: 0x17000678 RID: 1656
	// (get) Token: 0x060044FF RID: 17663 RVA: 0x0016DB7A File Offset: 0x0016BD7A
	public bool RightButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.rightControllerPrimaryButton || this.Poller.rightControllerSecondaryButton || this.Poller.rightControllerTriggerButton);
		}
	}

	// Token: 0x17000679 RID: 1657
	// (get) Token: 0x06004500 RID: 17664 RVA: 0x0016DBB4 File Offset: 0x0016BDB4
	public bool IsLeftStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Min(this.Poller.leftControllerPrimary2DAxis.x, this.Poller.rightControllerPrimary2DAxis.x) < -this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x1700067A RID: 1658
	// (get) Token: 0x06004501 RID: 17665 RVA: 0x0016DC04 File Offset: 0x0016BE04
	public bool IsRightStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Max(this.Poller.leftControllerPrimary2DAxis.x, this.Poller.rightControllerPrimary2DAxis.x) > this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x1700067B RID: 1659
	// (get) Token: 0x06004502 RID: 17666 RVA: 0x0016DC54 File Offset: 0x0016BE54
	public bool IsUpStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Max(this.Poller.leftControllerPrimary2DAxis.y, this.Poller.rightControllerPrimary2DAxis.y) > this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x1700067C RID: 1660
	// (get) Token: 0x06004503 RID: 17667 RVA: 0x0016DCA4 File Offset: 0x0016BEA4
	public bool IsDownStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Min(this.Poller.leftControllerPrimary2DAxis.y, this.Poller.rightControllerPrimary2DAxis.y) < -this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x1700067D RID: 1661
	// (get) Token: 0x06004504 RID: 17668 RVA: 0x0016DCF4 File Offset: 0x0016BEF4
	public float StickXValue
	{
		get
		{
			if (!(this.Poller == null))
			{
				return Mathf.Max(Mathf.Abs(this.Poller.leftControllerPrimary2DAxis.x), Mathf.Abs(this.Poller.rightControllerPrimary2DAxis.x));
			}
			return 0f;
		}
	}

	// Token: 0x1700067E RID: 1662
	// (get) Token: 0x06004505 RID: 17669 RVA: 0x0016DD44 File Offset: 0x0016BF44
	public float StickYValue
	{
		get
		{
			if (!(this.Poller == null))
			{
				return Mathf.Max(Mathf.Abs(this.Poller.leftControllerPrimary2DAxis.y), Mathf.Abs(this.Poller.rightControllerPrimary2DAxis.y));
			}
			return 0f;
		}
	}

	// Token: 0x1700067F RID: 1663
	// (get) Token: 0x06004506 RID: 17670 RVA: 0x0016DD94 File Offset: 0x0016BF94
	public bool TriggerDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerTriggerButton || this.Poller.rightControllerTriggerButton);
		}
	}

	// Token: 0x14000078 RID: 120
	// (add) Token: 0x06004507 RID: 17671 RVA: 0x0016DDC0 File Offset: 0x0016BFC0
	// (remove) Token: 0x06004508 RID: 17672 RVA: 0x0016DDF8 File Offset: 0x0016BFF8
	public event ControllerBehaviour.OnActionEvent OnAction;

	// Token: 0x06004509 RID: 17673 RVA: 0x0016DE2D File Offset: 0x0016C02D
	private void Awake()
	{
		if (ControllerBehaviour.Instance != null)
		{
			Debug.LogError("[CONTROLLER_BEHAVIOUR] Trying to create new singleton but one already exists", base.gameObject);
			Object.DestroyImmediate(this);
			return;
		}
		ControllerBehaviour.Instance = this;
	}

	// Token: 0x0600450A RID: 17674 RVA: 0x0016DE5C File Offset: 0x0016C05C
	private void Update()
	{
		bool flag = (this.IsLeftStick && this.wasLeftStick) || (this.IsRightStick && this.wasRightStick) || (this.IsUpStick && this.wasUpStick) || (this.IsDownStick && this.wasDownStick);
		if (Time.time - this.actionTime < this.actionDelay / this.repeatAction)
		{
			return;
		}
		if (this.wasHeld && flag)
		{
			this.repeatAction += this.actionRepeatDelayReduction;
		}
		else
		{
			this.repeatAction = 1f;
		}
		if (this.IsLeftStick || this.IsRightStick || this.IsUpStick || this.IsDownStick || this.ButtonDown)
		{
			this.actionTime = Time.time;
		}
		if (this.OnAction != null)
		{
			this.OnAction();
		}
		this.wasHeld = flag;
		this.wasDownStick = this.IsDownStick;
		this.wasUpStick = this.IsUpStick;
		this.wasLeftStick = this.IsLeftStick;
		this.wasRightStick = this.IsRightStick;
	}

	// Token: 0x0600450B RID: 17675 RVA: 0x0016DF71 File Offset: 0x0016C171
	public bool BuildValidationCheck()
	{
		if (this.uxSettings == null)
		{
			Debug.LogError("ControllerBehaviour must set UXSettings");
			return false;
		}
		return true;
	}

	// Token: 0x0600450C RID: 17676 RVA: 0x0016DF8E File Offset: 0x0016C18E
	public static ControllerBehaviour CreateNewControllerBehaviour(GameObject gameObject, UXSettings settings)
	{
		ControllerBehaviour controllerBehaviour = gameObject.AddComponent<ControllerBehaviour>();
		controllerBehaviour.uxSettings = settings;
		return controllerBehaviour;
	}

	// Token: 0x040056D0 RID: 22224
	private float actionTime;

	// Token: 0x040056D1 RID: 22225
	private float repeatAction = 1f;

	// Token: 0x040056D2 RID: 22226
	[SerializeField]
	private UXSettings uxSettings;

	// Token: 0x040056D3 RID: 22227
	[SerializeField]
	private float actionDelay = 0.5f;

	// Token: 0x040056D4 RID: 22228
	[SerializeField]
	private float actionRepeatDelayReduction = 0.5f;

	// Token: 0x040056D5 RID: 22229
	[Tooltip("Should the triggers modify the x axis like the sticks do?")]
	[SerializeField]
	private bool useTriggersAsSticks;

	// Token: 0x040056D6 RID: 22230
	private ControllerInputPoller poller;

	// Token: 0x040056D7 RID: 22231
	private bool wasLeftStick;

	// Token: 0x040056D8 RID: 22232
	private bool wasRightStick;

	// Token: 0x040056D9 RID: 22233
	private bool wasUpStick;

	// Token: 0x040056DA RID: 22234
	private bool wasDownStick;

	// Token: 0x040056DB RID: 22235
	private bool wasHeld;

	// Token: 0x02000AC9 RID: 2761
	// (Invoke) Token: 0x0600450F RID: 17679
	public delegate void OnActionEvent();
}
