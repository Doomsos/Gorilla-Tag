using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200072B RID: 1835
public class GRToolPurchaseStation : MonoBehaviour
{
	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x06002F4A RID: 12106 RVA: 0x001011E9 File Offset: 0x000FF3E9
	public int ActiveEntryIndex
	{
		get
		{
			return this.activeEntryIndex;
		}
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x001011F1 File Offset: 0x000FF3F1
	public void Init(GhostReactorManager grManager, GhostReactor reactor)
	{
		this.grManager = grManager;
		this.reactor = reactor;
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x00101201 File Offset: 0x000FF401
	public void RequestPurchaseButton(int actorNumber)
	{
		if (actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.TryPurchase);
		}
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x00101227 File Offset: 0x000FF427
	public void ShiftRightButton()
	{
		this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftRight);
	}

	// Token: 0x06002F4E RID: 12110 RVA: 0x0010123B File Offset: 0x000FF43B
	public void ShiftLeftButton()
	{
		this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftLeft);
	}

	// Token: 0x06002F4F RID: 12111 RVA: 0x0010124F File Offset: 0x000FF44F
	public void ShiftRightAuthority()
	{
		this.activeEntryIndex = (this.activeEntryIndex + 1) % this.toolEntries.Count;
	}

	// Token: 0x06002F50 RID: 12112 RVA: 0x0010126B File Offset: 0x000FF46B
	public void ShiftLeftAuthority()
	{
		this.activeEntryIndex = ((this.activeEntryIndex > 0) ? (this.activeEntryIndex - 1) : (this.toolEntries.Count - 1));
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x00101294 File Offset: 0x000FF494
	public void DebugPurchase()
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
		Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
		Quaternion rotation = this.depositTransform.rotation * localRotation;
		Vector3 position = this.depositTransform.position + this.depositTransform.rotation * localPosition;
		this.grManager.gameEntityManager.RequestCreateItem(entityTypeId, position, rotation, 0L);
		this.OnPurchaseSucceeded();
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x00101354 File Offset: 0x000FF554
	public bool TryPurchaseAuthority(GRPlayer player, out int itemCost)
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		itemCost = this.reactor.GetItemCost(entityTypeId);
		if (this.debugIgnoreToolCost || player.ShiftCredits >= itemCost)
		{
			Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
			Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
			Quaternion rotation = this.depositTransform.rotation * localRotation;
			Vector3 position = this.depositTransform.position + this.depositTransform.rotation * localPosition;
			this.grManager.gameEntityManager.RequestCreateItem(entityTypeId, position, rotation, 0L);
			return true;
		}
		return false;
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x00101434 File Offset: 0x000FF634
	public void OnSelectionUpdate(int newSelectedIndex)
	{
		this.activeEntryIndex = Mathf.Clamp(newSelectedIndex % this.toolEntries.Count, 0, this.toolEntries.Count - 1);
		this.audioSource.PlayOneShot(this.nextItemAudio, this.nextItemVolume);
		this.displayItemNameText.text = this.toolEntries[this.activeEntryIndex].toolName;
		this.displayItemCostText.text = this.toolEntries[this.activeEntryIndex].toolCost.ToString();
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x001014C8 File Offset: 0x000FF6C8
	public void OnPurchaseSucceeded()
	{
		this.animatingDeposit = true;
		this.animationStartTime = Time.time;
		this.audioSource.PlayOneShot(this.purchaseAudio, this.purchaseVolume);
		UnityEvent onSucceeded = this.idCardScanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		if (this.displayedEntryIndex < 0 || this.displayedEntryIndex >= this.toolEntries.Count)
		{
			this.displayedEntryIndex = this.activeEntryIndex;
		}
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x0010153C File Offset: 0x000FF73C
	public void OnPurchaseFailed()
	{
		this.audioSource.PlayOneShot(this.purchaseFailedAudio, this.purchaseFailedVolume);
		UnityEvent onFailed = this.idCardScanner.onFailed;
		if (onFailed == null)
		{
			return;
		}
		onFailed.Invoke();
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x0010156A File Offset: 0x000FF76A
	public Transform GetSpawnMarker()
	{
		return this.toolSpawnLocation;
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x00101572 File Offset: 0x000FF772
	public string GetCurrentToolName()
	{
		return this.toolEntries[this.activeEntryIndex].toolName;
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x0010158A File Offset: 0x000FF78A
	private void Awake()
	{
		this.depositLidOpenRot = Quaternion.Euler(this.depositLidOpenEuler);
		this.toolEntryRot = Quaternion.Euler(this.toolEntryRotEuler);
		this.toolExitRot = Quaternion.Euler(this.toolExitRotEuler);
	}

	// Token: 0x06002F59 RID: 12121 RVA: 0x001015C0 File Offset: 0x000FF7C0
	private void Update()
	{
		if (!this.animatingSwap && !this.animatingDeposit && this.activeEntryIndex != this.displayedEntryIndex)
		{
			this.animatingSwap = true;
			this.animationStartTime = Time.time;
			this.animPrevToolIndex = this.displayedEntryIndex;
			this.animNextToolIndex = this.activeEntryIndex;
			this.toolEntryRot = Quaternion.AngleAxis(this.toolEntryRotDegrees, Random.onUnitSphere);
		}
		if (this.animatingSwap)
		{
			float num = (Time.time - this.animationStartTime) / this.nextToolAnimationTime;
			Transform transform = null;
			if (this.animPrevToolIndex >= 0 && this.animPrevToolIndex < this.toolEntries.Count)
			{
				transform = this.toolEntries[this.animPrevToolIndex].displayToolParent;
				transform.localRotation = Quaternion.Slerp(Quaternion.identity, this.toolExitRot, this.toolExitRotTimingCurve.Evaluate(num));
				transform.localPosition = Vector3.Lerp(Vector3.zero, this.toolExitPosOffset, this.toolExitPosTimingCurve.Evaluate(num));
			}
			Transform displayToolParent = this.toolEntries[this.animNextToolIndex].displayToolParent;
			displayToolParent.localRotation = Quaternion.Slerp(this.toolEntryRot, Quaternion.identity, this.toolEntryRotTimingCurve.Evaluate(num));
			displayToolParent.localPosition = Vector3.Lerp(this.toolEntryPosOffset, Vector3.zero, this.toolEntryPosTimingCurve.Evaluate(num));
			displayToolParent.gameObject.SetActive(true);
			if (num >= 1f)
			{
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
				this.displayedEntryIndex = this.animNextToolIndex;
				this.animatingSwap = false;
				return;
			}
		}
		else if (this.animatingDeposit)
		{
			float num2 = (Time.time - this.animationStartTime) / this.toolDepositAnimationTime;
			Transform displayToolParent2 = this.toolEntries[this.displayedEntryIndex].displayToolParent;
			Vector3 localPosition = displayToolParent2.localPosition;
			localPosition.y = Mathf.Lerp(0f, this.depositTransform.localPosition.y, this.toolDepositMotionCurveY.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			localPosition.z = Mathf.Lerp(0f, this.depositTransform.localPosition.z, this.toolDepositMotionCurveZ.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			displayToolParent2.localPosition = localPosition;
			this.depositLidTransform.localRotation = Quaternion.Slerp(Quaternion.identity, this.depositLidOpenRot, this.depositLidTimingCurve.Evaluate(num2));
			if (num2 >= 1f)
			{
				this.depositLidTransform.localRotation = Quaternion.identity;
				displayToolParent2.gameObject.SetActive(false);
				this.displayedEntryIndex = -1;
				this.animatingDeposit = false;
			}
		}
	}

	// Token: 0x04003DB8 RID: 15800
	[SerializeField]
	private List<GRToolPurchaseStation.ToolEntry> toolEntries = new List<GRToolPurchaseStation.ToolEntry>();

	// Token: 0x04003DB9 RID: 15801
	[SerializeField]
	private Transform displayTransform;

	// Token: 0x04003DBA RID: 15802
	[SerializeField]
	private Transform depositTransform;

	// Token: 0x04003DBB RID: 15803
	[SerializeField]
	private Transform toolSpawnLocation;

	// Token: 0x04003DBC RID: 15804
	[SerializeField]
	private TMP_Text displayItemNameText;

	// Token: 0x04003DBD RID: 15805
	[SerializeField]
	private TMP_Text displayItemCostText;

	// Token: 0x04003DBE RID: 15806
	[SerializeField]
	private float nextToolAnimationTime = 0.5f;

	// Token: 0x04003DBF RID: 15807
	[SerializeField]
	private float toolDepositAnimationTime = 1f;

	// Token: 0x04003DC0 RID: 15808
	[SerializeField]
	private Vector3 toolEntryPosOffset = new Vector3(0f, 0.25f, 0f);

	// Token: 0x04003DC1 RID: 15809
	[SerializeField]
	private Vector3 toolEntryRotEuler = new Vector3(0f, 0f, 15f);

	// Token: 0x04003DC2 RID: 15810
	[SerializeField]
	private float toolEntryRotDegrees = 15f;

	// Token: 0x04003DC3 RID: 15811
	[SerializeField]
	private Vector3 toolExitPosOffset = new Vector3(0f, 0f, -0.25f);

	// Token: 0x04003DC4 RID: 15812
	[SerializeField]
	private Vector3 toolExitRotEuler = new Vector3(180f, 0f, 0f);

	// Token: 0x04003DC5 RID: 15813
	[SerializeField]
	private AnimationCurve toolEntryPosTimingCurve;

	// Token: 0x04003DC6 RID: 15814
	[SerializeField]
	private AnimationCurve toolEntryRotTimingCurve;

	// Token: 0x04003DC7 RID: 15815
	[SerializeField]
	private AnimationCurve toolExitPosTimingCurve;

	// Token: 0x04003DC8 RID: 15816
	[SerializeField]
	private AnimationCurve toolExitRotTimingCurve;

	// Token: 0x04003DC9 RID: 15817
	[SerializeField]
	private AnimationCurve toolDepositTimingCurve;

	// Token: 0x04003DCA RID: 15818
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveY;

	// Token: 0x04003DCB RID: 15819
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveZ;

	// Token: 0x04003DCC RID: 15820
	[SerializeField]
	private Transform depositLidTransform;

	// Token: 0x04003DCD RID: 15821
	[SerializeField]
	private Vector3 depositLidOpenEuler = new Vector3(65f, 0f, 0f);

	// Token: 0x04003DCE RID: 15822
	[SerializeField]
	private AnimationCurve depositLidTimingCurve;

	// Token: 0x04003DCF RID: 15823
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003DD0 RID: 15824
	[SerializeField]
	private AudioClip nextItemAudio;

	// Token: 0x04003DD1 RID: 15825
	[SerializeField]
	private float nextItemVolume = 0.5f;

	// Token: 0x04003DD2 RID: 15826
	[SerializeField]
	private AudioClip purchaseAudio;

	// Token: 0x04003DD3 RID: 15827
	[SerializeField]
	private float purchaseVolume = 0.5f;

	// Token: 0x04003DD4 RID: 15828
	[SerializeField]
	private AudioClip purchaseFailedAudio;

	// Token: 0x04003DD5 RID: 15829
	[SerializeField]
	private float purchaseFailedVolume = 0.5f;

	// Token: 0x04003DD6 RID: 15830
	[SerializeField]
	private IDCardScanner idCardScanner;

	// Token: 0x04003DD7 RID: 15831
	private int activeEntryIndex = 1;

	// Token: 0x04003DD8 RID: 15832
	private int displayedEntryIndex = -1;

	// Token: 0x04003DD9 RID: 15833
	private float animationStartTime;

	// Token: 0x04003DDA RID: 15834
	private bool animatingDeposit;

	// Token: 0x04003DDB RID: 15835
	private bool animatingSwap;

	// Token: 0x04003DDC RID: 15836
	private int animPrevToolIndex;

	// Token: 0x04003DDD RID: 15837
	private int animNextToolIndex;

	// Token: 0x04003DDE RID: 15838
	private Quaternion depositLidOpenRot = Quaternion.identity;

	// Token: 0x04003DDF RID: 15839
	private Quaternion toolEntryRot = Quaternion.identity;

	// Token: 0x04003DE0 RID: 15840
	private Quaternion toolExitRot = Quaternion.identity;

	// Token: 0x04003DE1 RID: 15841
	private Coroutine vendingCoroutine;

	// Token: 0x04003DE2 RID: 15842
	private bool debugIgnoreToolCost;

	// Token: 0x04003DE3 RID: 15843
	[HideInInspector]
	public int PurchaseStationId;

	// Token: 0x04003DE4 RID: 15844
	private GhostReactorManager grManager;

	// Token: 0x04003DE5 RID: 15845
	private GhostReactor reactor;

	// Token: 0x0200072C RID: 1836
	[Serializable]
	public struct ToolEntry
	{
		// Token: 0x06002F5B RID: 12123 RVA: 0x0010197D File Offset: 0x000FFB7D
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x04003DE6 RID: 15846
		public Transform displayToolParent;

		// Token: 0x04003DE7 RID: 15847
		public GameEntity entityPrefab;

		// Token: 0x04003DE8 RID: 15848
		public string toolName;

		// Token: 0x04003DE9 RID: 15849
		public int toolCost;

		// Token: 0x04003DEA RID: 15850
		private int entityTypeId;

		// Token: 0x04003DEB RID: 15851
		private bool entityTypeIdSet;
	}
}
