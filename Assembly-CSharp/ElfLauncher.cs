using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200024F RID: 591
public class ElfLauncher : MonoBehaviour
{
	// Token: 0x06000F6B RID: 3947 RVA: 0x00051D30 File Offset: 0x0004FF30
	private void OnEnable()
	{
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = (this.parentHoldable.myOnlineRig != null) ? this.parentHoldable.myOnlineRig.creator : ((this.parentHoldable.myRig != null) ? ((this.parentHoldable.myRig.creator != null) ? this.parentHoldable.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
			if (netPlayer != null)
			{
				this.m_player = netPlayer;
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ShootShared);
		}
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x00051E1C File Offset: 0x0005001C
	private void OnDisable()
	{
		if (this._events != null)
		{
			this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ShootShared);
			this._events.Dispose();
			this._events = null;
			this.m_player = null;
		}
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x00051E74 File Offset: 0x00050074
	private void Awake()
	{
		this._events = base.GetComponent<RubberDuckEvents>();
		this.elfProjectileHash = PoolUtils.GameObjHashCode(this.elfProjectilePrefab);
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCranked));
		}
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x00051EC8 File Offset: 0x000500C8
	private void OnCranked(float deltaAngle)
	{
		this.currentShootCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentShootCrankAmount) > this.crankShootThreshold)
		{
			this.currentShootCrankAmount = 0f;
			this.Shoot();
		}
		this.currentClickCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentClickCrankAmount) > this.crankClickThreshold)
		{
			this.currentClickCrankAmount = 0f;
			this.crankClickAudio.Play();
		}
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x00051F40 File Offset: 0x00050140
	private void Shoot()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			GorillaTagger.Instance.StartVibration(true, this.shootHapticStrength, this.shootHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.shootHapticStrength, this.shootHapticDuration);
			if (PhotonNetwork.InRoom)
			{
				this._events.Activate.RaiseAll(new object[]
				{
					this.muzzle.transform.position,
					this.muzzle.transform.forward
				});
				return;
			}
			this.ShootShared(this.muzzle.transform.position, this.muzzle.transform.forward);
		}
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x00052000 File Offset: 0x00050200
	private void ShootShared(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (args.Length != 2)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		VRRig ownerRig = this.parentHoldable.ownerRig;
		if (info.senderID != ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length == 2)
		{
			object obj = args[0];
			if (obj is Vector3)
			{
				Vector3 vector = (Vector3)obj;
				obj = args[1];
				if (obj is Vector3)
				{
					Vector3 direction = (Vector3)obj;
					float num = 10000f;
					if (vector.IsValid(num))
					{
						float num2 = 10000f;
						if (direction.IsValid(num2))
						{
							if (!FXSystem.CheckCallSpam(ownerRig.fxSettings, 4, info.SentServerTime) || !ownerRig.IsPositionInRange(vector, 6f))
							{
								return;
							}
							this.ShootShared(vector, direction);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x000520B8 File Offset: 0x000502B8
	protected virtual void ShootShared(Vector3 origin, Vector3 direction)
	{
		this.shootAudio.Play();
		Vector3 lossyScale = base.transform.lossyScale;
		GameObject gameObject = ObjectPools.instance.Instantiate(this.elfProjectileHash, true);
		gameObject.transform.position = origin;
		gameObject.transform.rotation = Quaternion.LookRotation(direction);
		gameObject.transform.localScale = lossyScale;
		gameObject.GetComponent<Rigidbody>().linearVelocity = direction * this.muzzleVelocity * lossyScale.x;
	}

	// Token: 0x04001304 RID: 4868
	[SerializeField]
	protected TransferrableObject parentHoldable;

	// Token: 0x04001305 RID: 4869
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x04001306 RID: 4870
	[SerializeField]
	private float crankShootThreshold = 360f;

	// Token: 0x04001307 RID: 4871
	[SerializeField]
	private float crankClickThreshold = 30f;

	// Token: 0x04001308 RID: 4872
	[SerializeField]
	private Transform muzzle;

	// Token: 0x04001309 RID: 4873
	[SerializeField]
	private GameObject elfProjectilePrefab;

	// Token: 0x0400130A RID: 4874
	protected int elfProjectileHash;

	// Token: 0x0400130B RID: 4875
	[SerializeField]
	protected float muzzleVelocity = 10f;

	// Token: 0x0400130C RID: 4876
	[SerializeField]
	private SoundBankPlayer crankClickAudio;

	// Token: 0x0400130D RID: 4877
	[SerializeField]
	protected SoundBankPlayer shootAudio;

	// Token: 0x0400130E RID: 4878
	[SerializeField]
	private float shootHapticStrength;

	// Token: 0x0400130F RID: 4879
	[SerializeField]
	private float shootHapticDuration;

	// Token: 0x04001310 RID: 4880
	private RubberDuckEvents _events;

	// Token: 0x04001311 RID: 4881
	private float currentShootCrankAmount;

	// Token: 0x04001312 RID: 4882
	private float currentClickCrankAmount;

	// Token: 0x04001313 RID: 4883
	private NetPlayer m_player;
}
