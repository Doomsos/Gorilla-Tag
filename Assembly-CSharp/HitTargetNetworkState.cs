using System;
using System.Collections;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200040F RID: 1039
[NetworkBehaviourWeaved(1)]
public class HitTargetNetworkState : NetworkComponent
{
	// Token: 0x06001988 RID: 6536 RVA: 0x00088DC4 File Offset: 0x00086FC4
	protected override void Awake()
	{
		base.Awake();
		this.audioPlayer = base.GetComponent<AudioSource>();
		SlingshotProjectileHitNotifier component = base.GetComponent<SlingshotProjectileHitNotifier>();
		if (component != null)
		{
			component.OnProjectileHit += this.ProjectileHitReciever;
			component.OnProjectileCollisionStay += this.ProjectileHitReciever;
			return;
		}
		Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject to increment score");
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x00088E22 File Offset: 0x00087022
	protected override void Start()
	{
		base.Start();
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x00088E45 File Offset: 0x00087045
	private void SetInitialState()
	{
		this.networkedScore.Value = 0;
		this.nextHittableTimestamp = 0f;
		this.audioPlayer.GTStop();
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x00088E69 File Offset: 0x00087069
	public void OnLeftRoom()
	{
		this.SetInitialState();
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x00088E71 File Offset: 0x00087071
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
		this.SetInitialState();
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x00088E99 File Offset: 0x00087099
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.TargetHit(Vector3.zero, Vector3.one);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x00088EA8 File Offset: 0x000870A8
	private void ProjectileHitReciever(SlingshotProjectile projectile, Collision collision)
	{
		this.TargetHit(projectile.launchPosition, collision.contacts[0].point);
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x00088EC8 File Offset: 0x000870C8
	public void TargetHit(Vector3 launchPoint, Vector3 impactPoint)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (Time.time <= this.nextHittableTimestamp)
		{
			return;
		}
		int num = this.networkedScore.Value;
		if (this.scoreIsDistance)
		{
			int num2 = Mathf.RoundToInt((launchPoint - impactPoint).magnitude * 3.28f);
			if (num2 <= num)
			{
				return;
			}
			num = num2;
		}
		else
		{
			num++;
			if (num >= 1000)
			{
				num = 0;
			}
		}
		if (this.resetAfterDuration > 0f && this.resetCoroutine == null)
		{
			this.resetAtTimestamp = Time.time + this.resetAfterDuration;
			this.resetCoroutine = base.StartCoroutine(this.ResetCo());
		}
		this.PlayAudio(this.networkedScore.Value, num);
		this.networkedScore.Value = num;
		this.nextHittableTimestamp = Time.time + (float)this.hitCooldownTime;
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06001990 RID: 6544 RVA: 0x00088FA0 File Offset: 0x000871A0
	// (set) Token: 0x06001991 RID: 6545 RVA: 0x00088FC6 File Offset: 0x000871C6
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe int Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x00088FED File Offset: 0x000871ED
	public override void WriteDataFusion()
	{
		this.Data = this.networkedScore.Value;
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x00089000 File Offset: 0x00087200
	public override void ReadDataFusion()
	{
		int data = this.Data;
		if (data != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, data);
		}
		this.networkedScore.Value = data;
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x00089040 File Offset: 0x00087240
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.networkedScore.Value);
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x00089068 File Offset: 0x00087268
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		if (num != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, num);
		}
		this.networkedScore.Value = num;
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x000890BB File Offset: 0x000872BB
	public void PlayAudio(int oldScore, int newScore)
	{
		if (oldScore > newScore && !this.scoreIsDistance)
		{
			this.audioPlayer.GTPlayOneShot(this.audioClips[1], 1f);
			return;
		}
		this.audioPlayer.GTPlayOneShot(this.audioClips[0], 1f);
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x000890FA File Offset: 0x000872FA
	private IEnumerator ResetCo()
	{
		while (Time.time < this.resetAtTimestamp)
		{
			yield return new WaitForSeconds(this.resetAtTimestamp - Time.time);
		}
		this.networkedScore.Value = 0;
		this.PlayAudio(this.networkedScore.Value, 0);
		this.resetCoroutine = null;
		yield break;
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x00089118 File Offset: 0x00087318
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x00089130 File Offset: 0x00087330
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040022FE RID: 8958
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x040022FF RID: 8959
	[SerializeField]
	private int hitCooldownTime = 1;

	// Token: 0x04002300 RID: 8960
	[SerializeField]
	private bool testPress;

	// Token: 0x04002301 RID: 8961
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04002302 RID: 8962
	[SerializeField]
	private bool scoreIsDistance;

	// Token: 0x04002303 RID: 8963
	[SerializeField]
	private float resetAfterDuration;

	// Token: 0x04002304 RID: 8964
	private AudioSource audioPlayer;

	// Token: 0x04002305 RID: 8965
	private float nextHittableTimestamp;

	// Token: 0x04002306 RID: 8966
	private float resetAtTimestamp;

	// Token: 0x04002307 RID: 8967
	private Coroutine resetCoroutine;

	// Token: 0x04002308 RID: 8968
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private int _Data;
}
