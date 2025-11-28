using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000281 RID: 641
public class RadioButtonGroupWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x17000191 RID: 401
	// (get) Token: 0x06001070 RID: 4208 RVA: 0x000560F2 File Offset: 0x000542F2
	// (set) Token: 0x06001071 RID: 4209 RVA: 0x000560FA File Offset: 0x000542FA
	public bool IsSpawned { get; set; }

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06001072 RID: 4210 RVA: 0x00056103 File Offset: 0x00054303
	// (set) Token: 0x06001073 RID: 4211 RVA: 0x0005610B File Offset: 0x0005430B
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001074 RID: 4212 RVA: 0x00056114 File Offset: 0x00054314
	private void Start()
	{
		this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.assignedSlot];
		if (!this.ownerRig.isLocal)
		{
			GorillaPressableButton[] array = this.buttons;
			for (int i = 0; i < array.Length; i++)
			{
				Collider component = array[i].GetComponent<Collider>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x00056172 File Offset: 0x00054372
	private void OnEnable()
	{
		this.SharedRefreshState();
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x0005617A File Offset: 0x0005437A
	private int GetCurrentState()
	{
		return GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x000561A2 File Offset: 0x000543A2
	private void Update()
	{
		if (this.ownerRig.isLocal)
		{
			return;
		}
		if (this.lastReportedState != this.GetCurrentState())
		{
			this.SharedRefreshState();
		}
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x000561C8 File Offset: 0x000543C8
	public void SharedRefreshState()
	{
		int currentState = this.GetCurrentState();
		int num = this.AllowSelectNone ? (currentState - 1) : currentState;
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].isOn = (num == i);
			this.buttons[i].UpdateColor();
		}
		if (this.lastReportedState != currentState)
		{
			this.lastReportedState = currentState;
			this.OnSelectionChanged.Invoke(currentState);
		}
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x00056238 File Offset: 0x00054438
	public void OnPress(GorillaPressableButton button)
	{
		int currentState = this.GetCurrentState();
		int num = Array.IndexOf<GorillaPressableButton>(this.buttons, button);
		if (this.AllowSelectNone)
		{
			num++;
		}
		int value = num;
		if (this.AllowSelectNone && num == currentState)
		{
			value = 0;
		}
		this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, value);
		this.SharedRefreshState();
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x0005629D File Offset: 0x0005449D
	public void OnSpawn(VRRig rig)
	{
		this.ownerRig = rig;
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDespawn()
	{
	}

	// Token: 0x04001474 RID: 5236
	[SerializeField]
	private bool AllowSelectNone = true;

	// Token: 0x04001475 RID: 5237
	[SerializeField]
	private GorillaPressableButton[] buttons;

	// Token: 0x04001476 RID: 5238
	[SerializeField]
	private UnityEvent<int> OnSelectionChanged;

	// Token: 0x04001477 RID: 5239
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	[SerializeField]
	private VRRig.WearablePackedStateSlots assignedSlot = VRRig.WearablePackedStateSlots.Pants1;

	// Token: 0x04001478 RID: 5240
	private int lastReportedState;

	// Token: 0x04001479 RID: 5241
	private VRRig ownerRig;

	// Token: 0x0400147A RID: 5242
	private GTBitOps.BitWriteInfo stateBitsWriteInfo;
}
