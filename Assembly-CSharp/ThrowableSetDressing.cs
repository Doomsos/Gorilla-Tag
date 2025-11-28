using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000CD2 RID: 3282
[RequireComponent(typeof(NetworkView))]
public class ThrowableSetDressing : TransferrableObject
{
	// Token: 0x17000770 RID: 1904
	// (get) Token: 0x0600500B RID: 20491 RVA: 0x0019B897 File Offset: 0x00199A97
	// (set) Token: 0x0600500C RID: 20492 RVA: 0x0019B89F File Offset: 0x00199A9F
	public bool inInitialPose { get; private set; } = true;

	// Token: 0x0600500D RID: 20493 RVA: 0x0019B8A8 File Offset: 0x00199AA8
	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	// Token: 0x0600500E RID: 20494 RVA: 0x0019B8BA File Offset: 0x00199ABA
	protected override void Awake()
	{
		base.Awake();
		this.netView = base.GetComponent<NetworkView>();
	}

	// Token: 0x0600500F RID: 20495 RVA: 0x0019B8CE File Offset: 0x00199ACE
	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06005010 RID: 20496 RVA: 0x0019B903 File Offset: 0x00199B03
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	// Token: 0x06005011 RID: 20497 RVA: 0x0019B91A File Offset: 0x00199B1A
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.StartRespawnTimer(-1f);
		return true;
	}

	// Token: 0x06005012 RID: 20498 RVA: 0x0019B934 File Offset: 0x00199B34
	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06005013 RID: 20499 RVA: 0x0019B947 File Offset: 0x00199B47
	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	// Token: 0x06005014 RID: 20500 RVA: 0x0019B964 File Offset: 0x00199B64
	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	// Token: 0x06005015 RID: 20501 RVA: 0x0019B974 File Offset: 0x00199B74
	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float timerDuration = (overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration;
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (!this.netView.IsValid || this.netView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(timerDuration));
		}
	}

	// Token: 0x06005016 RID: 20502 RVA: 0x0019B9D3 File Offset: 0x00199BD3
	private IEnumerator RespawnTimerCoroutine(float timerDuration)
	{
		yield return new WaitForSeconds(timerDuration);
		if (base.InHand())
		{
			yield break;
		}
		this.SetWillTeleport();
		base.transform.position = this.respawnAtPos;
		base.transform.rotation = this.respawnAtRot;
		this.inInitialPose = true;
		this.rigidbodyInstance.isKinematic = true;
		yield break;
	}

	// Token: 0x04005E9D RID: 24221
	public float respawnTimerDuration;

	// Token: 0x04005E9F RID: 24223
	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04005EA0 RID: 24224
	private float _respawnTimestamp;

	// Token: 0x04005EA1 RID: 24225
	[SerializeField]
	private CapsuleCollider capsuleCollider;

	// Token: 0x04005EA2 RID: 24226
	private NetworkView netView;

	// Token: 0x04005EA3 RID: 24227
	private Vector3 respawnAtPos;

	// Token: 0x04005EA4 RID: 24228
	private Quaternion respawnAtRot;

	// Token: 0x04005EA5 RID: 24229
	private Coroutine respawnTimer;
}
