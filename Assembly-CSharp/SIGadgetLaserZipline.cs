using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class SIGadgetLaserZipline : SIGadget, ICallBack
{
	// Token: 0x06000541 RID: 1345 RVA: 0x0001ECEC File Offset: 0x0001CEEC
	private void Awake()
	{
		this.m_buttonActivatable = base.GetComponent<GameButtonActivatable>();
		this.laserBeam.SetActive(false);
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x00002789 File Offset: 0x00000989
	private void OnGrabbed()
	{
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x00002789 File Offset: 0x00000989
	private void OnSnapped()
	{
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001EDC4 File Offset: 0x0001CFC4
	private void OnReleased()
	{
		this.wasTriggerPressed = false;
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001EDC4 File Offset: 0x0001CFC4
	private void OnUnsnapped()
	{
		this.wasTriggerPressed = false;
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0001EDD0 File Offset: 0x0001CFD0
	protected override void OnUpdateAuthority(float dt)
	{
		bool flag = this.m_buttonActivatable.CheckInput(true, true, 0.25f, true, true);
		if (this.coolingDownUntilNextTouchGround && (GTPlayer.Instance.IsGroundedHand || GTPlayer.Instance.IsGroundedButt))
		{
			this.coolingDownUntilNextTouchGround = false;
		}
		if (flag)
		{
			if (this.isLineBroken)
			{
				return;
			}
			if (GTPlayer.Instance.IsGroundedButt || GTPlayer.Instance.IsGroundedHand)
			{
				this.isLineBroken = true;
				this.laserBeam.SetActive(false);
				return;
			}
			if (!this.wasTriggerPressed)
			{
				if (Time.time < this.coolingDownUntilTimestamp || this.coolingDownUntilNextTouchGround)
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					return;
				}
				this.laserBeam.SetActive(true);
				this.laserBeam.transform.localPosition = Vector3.zero;
				VRRig.LocalRig.AddLateUpdateCallback(this);
				this.activatedAtPoint = this.zipline.transform.position;
				this.ziplineDirection = this.zipline.transform.forward;
				if (this.ziplineDirection.y > 0f)
				{
					this.ziplineDirection = -this.ziplineDirection;
				}
				if (this.ziplineDirection.y > -0.5f)
				{
					this.ziplineDirection = GTPlayer.Instance.mainCamera.transform.forward.WithY(-0.5f).normalized;
				}
				this.activatedAtRotation = Quaternion.LookRotation(this.ziplineDirection);
			}
			float magnitude = GTPlayer.Instance.RigidbodyVelocity.magnitude;
			float num = Mathf.Lerp(Vector3.Dot(GTPlayer.Instance.RigidbodyVelocity, this.ziplineDirection), magnitude, 0.5f) - this.speedBoost * this.ziplineDirection.y * Time.deltaTime;
			GTPlayer.Instance.SetVelocity(this.ziplineDirection * num);
			this.wasTriggerPressed = true;
			return;
		}
		else
		{
			if (this.wasTriggerPressed)
			{
				this.laserBeam.SetActive(false);
				this.zipline.transform.localRotation = Quaternion.identity;
				this.isLineBroken = false;
				this.wasTriggerPressed = false;
				this.coolingDownUntilTimestamp = Time.time + this.cooldownDuration;
				this.coolingDownUntilNextTouchGround = this.cooldownOnUseUntilTouchGround;
				GTPlayer.Instance.SetVelocity(GTPlayer.Instance.AveragedVelocity);
				return;
			}
			this.isLineBroken = false;
			return;
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnUpdateRemote(float dt)
	{
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEntityStateChanged(long oldState, long newState)
	{
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0001F02C File Offset: 0x0001D22C
	public void CallBack()
	{
		if (!this.wasTriggerPressed || this.isLineBroken)
		{
			VRRig.LocalRig.RemoveLateUpdateCallback(this);
			return;
		}
		Vector3 vector = this.activatedAtPoint - this.zipline.transform.position;
		vector = vector.ProjectOnPlane(Vector3.zero, this.ziplineDirection);
		if (vector.sqrMagnitude > 1f)
		{
			this.isLineBroken = true;
			this.laserBeam.SetActive(false);
			return;
		}
		GTPlayer.Instance.transform.position += vector;
		this.zipline.transform.rotation = this.activatedAtRotation;
		Vector3 position = this.activatedAtPoint + Vector3.Project(this.zipline.transform.position - this.activatedAtPoint, this.ziplineDirection);
		this.laserBeam.transform.position = position;
	}

	// Token: 0x0400068F RID: 1679
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x04000690 RID: 1680
	[SerializeField]
	private Transform zipline;

	// Token: 0x04000691 RID: 1681
	[SerializeField]
	private GameObject laserBeam;

	// Token: 0x04000692 RID: 1682
	[SerializeField]
	private float speedBoost;

	// Token: 0x04000693 RID: 1683
	[SerializeField]
	private float cooldownDuration;

	// Token: 0x04000694 RID: 1684
	[SerializeField]
	private bool cooldownOnUseUntilTouchGround;

	// Token: 0x04000695 RID: 1685
	private bool wasTriggerPressed;

	// Token: 0x04000696 RID: 1686
	private bool isLineBroken;

	// Token: 0x04000697 RID: 1687
	private Quaternion activatedAtRotation;

	// Token: 0x04000698 RID: 1688
	private Vector3 activatedAtPoint;

	// Token: 0x04000699 RID: 1689
	private Vector3 ziplineDirection;

	// Token: 0x0400069A RID: 1690
	private float coolingDownUntilTimestamp;

	// Token: 0x0400069B RID: 1691
	private bool coolingDownUntilNextTouchGround;
}
