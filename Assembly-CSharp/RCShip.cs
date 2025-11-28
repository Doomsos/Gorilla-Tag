using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001C2 RID: 450
public class RCShip : RCHoverboard
{
	// Token: 0x06000C1E RID: 3102 RVA: 0x0004189E File Offset: 0x0003FA9E
	private byte GetDataB()
	{
		if (!this.hasNetworkSync)
		{
			return 0;
		}
		return this.networkSync.syncedState.dataB;
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x000418BA File Offset: 0x0003FABA
	private void SetDataB(byte b)
	{
		if (this.hasNetworkSync)
		{
			this.networkSync.syncedState.dataB = b;
		}
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x000418D8 File Offset: 0x0003FAD8
	private void WriteCannonBit(bool toLeft)
	{
		if (!this.hasNetworkSync)
		{
			return;
		}
		byte b = this.GetDataB();
		b = (toLeft ? (b | 1) : ((byte)((int)b & -2)));
		this.SetDataB(b);
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0004190B File Offset: 0x0003FB0B
	private bool ReadCannonBit()
	{
		if (!this.hasNetworkSync)
		{
			return this.cannonToLeft;
		}
		return (this.GetDataB() & 1) > 0;
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00041927 File Offset: 0x0003FB27
	private bool ReadFireFlip()
	{
		return (this.GetDataB() & 2) > 0;
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x00041934 File Offset: 0x0003FB34
	protected override void AuthorityUpdate(float dt)
	{
		base.AuthorityUpdate(dt);
		float trigger = this.activeInput.trigger;
		float num = (float)this.activeInput.buttons;
		if (this.localState == RCVehicle.State.Mobilized && this.localStatePrev != RCVehicle.State.Mobilized)
		{
			this.armedAfterMobilize = false;
			if (trigger >= this.triggerReleaseThreshold)
			{
				this.triggerIsDown = true;
			}
		}
		if (this.localState == RCVehicle.State.Mobilized)
		{
			if (!this.armedAfterMobilize && trigger <= this.triggerReleaseThreshold)
			{
				this.armedAfterMobilize = true;
				this.triggerIsDown = false;
			}
			if (this.armedAfterMobilize)
			{
				if (!this.triggerIsDown && trigger >= this.triggerPressThreshold)
				{
					this.triggerIsDown = true;
					UnityEvent onFire = this.OnFire;
					if (onFire != null)
					{
						onFire.Invoke();
					}
					if (this.hasNetworkSync)
					{
						byte b = this.GetDataB();
						b ^= 2;
						this.SetDataB(b);
						this.lastFireFlip = ((b & 2) > 0);
					}
				}
				else if (this.triggerIsDown && trigger <= this.triggerReleaseThreshold)
				{
					this.triggerIsDown = false;
				}
			}
			if (!this.faceIsDown && num >= this.facePressThreshold)
			{
				this.faceIsDown = true;
				this.cannonToLeft = !this.cannonToLeft;
				this.WriteCannonBit(this.cannonToLeft);
			}
			else if (this.faceIsDown && num <= this.faceReleaseThreshold)
			{
				this.faceIsDown = false;
			}
		}
		else
		{
			if (this.faceIsDown && num <= this.faceReleaseThreshold)
			{
				this.faceIsDown = false;
			}
			this.armedAfterMobilize = false;
			if (this.triggerIsDown && trigger <= this.triggerReleaseThreshold)
			{
				this.triggerIsDown = false;
			}
		}
		if (this.hasNetworkSync)
		{
			byte b2 = this.GetDataB();
			if (this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold)
			{
				b2 |= 4;
				this.isMovingShared = true;
			}
			else
			{
				b2 = (byte)((int)b2 & -5);
				this.isMovingShared = false;
			}
			this.SetDataB(b2);
			return;
		}
		this.isMovingShared = (this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold);
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x00041B70 File Offset: 0x0003FD70
	protected override void RemoteUpdate(float dt)
	{
		base.RemoteUpdate(dt);
		if (!this.hasNetworkSync)
		{
			return;
		}
		this.cannonToLeft = this.ReadCannonBit();
		bool flag = this.ReadFireFlip();
		if (!base.HasLocalAuthority)
		{
			if (flag != this.lastFireFlip)
			{
				this.lastFireFlip = flag;
				UnityEvent onFire = this.OnFire;
				if (onFire != null)
				{
					onFire.Invoke();
				}
			}
			byte dataB = this.GetDataB();
			this.isMovingShared = ((dataB & 4) > 0);
			return;
		}
		this.lastFireFlip = flag;
		this.isMovingShared = (this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold);
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00041C2C File Offset: 0x0003FE2C
	protected override void SharedUpdate(float dt)
	{
		base.SharedUpdate(dt);
		if (this.cannonTransform != null)
		{
			float num = this.cannonToLeft ? this.leftYaw : this.rightYaw;
			Vector3 localEulerAngles = this.cannonTransform.localEulerAngles;
			localEulerAngles.z = Mathf.MoveTowardsAngle(localEulerAngles.z, num, this.cannonYawSpeed * dt);
			this.cannonTransform.localEulerAngles = localEulerAngles;
		}
		if (this.cannonToLeft != this.lastCannonToLeft)
		{
			this.lastCannonToLeft = this.cannonToLeft;
			UnityEvent<bool> onCannonSideChanged = this.OnCannonSideChanged;
			if (onCannonSideChanged != null)
			{
				onCannonSideChanged.Invoke(this.cannonToLeft);
			}
		}
		bool flag = this.localState == RCVehicle.State.Mobilized && this.isMovingShared;
		if (flag != this.lastIsMoving)
		{
			this.lastIsMoving = flag;
			if (flag)
			{
				UnityEvent onMoveStarted = this.OnMoveStarted;
				if (onMoveStarted == null)
				{
					return;
				}
				onMoveStarted.Invoke();
				return;
			}
			else
			{
				UnityEvent onMoveStopped = this.OnMoveStopped;
				if (onMoveStopped == null)
				{
					return;
				}
				onMoveStopped.Invoke();
			}
		}
	}

	// Token: 0x04000EDC RID: 3804
	[Header("RCShip - Events")]
	public UnityEvent OnFire;

	// Token: 0x04000EDD RID: 3805
	public UnityEvent<bool> OnCannonSideChanged;

	// Token: 0x04000EDE RID: 3806
	public UnityEvent OnMoveStarted;

	// Token: 0x04000EDF RID: 3807
	public UnityEvent OnMoveStopped;

	// Token: 0x04000EE0 RID: 3808
	[Header("RCShip - Cannon Rotation")]
	[SerializeField]
	private Transform cannonTransform;

	// Token: 0x04000EE1 RID: 3809
	[SerializeField]
	private float leftYaw = -45f;

	// Token: 0x04000EE2 RID: 3810
	[SerializeField]
	private float rightYaw = 45f;

	// Token: 0x04000EE3 RID: 3811
	[SerializeField]
	private float cannonYawSpeed = 240f;

	// Token: 0x04000EE4 RID: 3812
	[Header("RCShip - Input")]
	[Range(0f, 1f)]
	[SerializeField]
	private float triggerPressThreshold = 0.6f;

	// Token: 0x04000EE5 RID: 3813
	[Range(0f, 1f)]
	[SerializeField]
	private float triggerReleaseThreshold = 0.1f;

	// Token: 0x04000EE6 RID: 3814
	[Range(0f, 1f)]
	[SerializeField]
	private float facePressThreshold = 0.6f;

	// Token: 0x04000EE7 RID: 3815
	[Range(0f, 1f)]
	[SerializeField]
	private float faceReleaseThreshold = 0.1f;

	// Token: 0x04000EE8 RID: 3816
	[Header("RCShip - Movement Detection")]
	[Tooltip("Minimum speed to consider the ship moving")]
	[SerializeField]
	private float movingSpeedThreshold = 0.05f;

	// Token: 0x04000EE9 RID: 3817
	private bool prevTriggerDown;

	// Token: 0x04000EEA RID: 3818
	private bool prevFaceDown;

	// Token: 0x04000EEB RID: 3819
	private bool faceIsDown;

	// Token: 0x04000EEC RID: 3820
	private bool triggerIsDown;

	// Token: 0x04000EED RID: 3821
	private bool armedAfterMobilize;

	// Token: 0x04000EEE RID: 3822
	private bool cannonToLeft;

	// Token: 0x04000EEF RID: 3823
	private const byte CannonLeftBit = 1;

	// Token: 0x04000EF0 RID: 3824
	private const byte FireFlipBit = 2;

	// Token: 0x04000EF1 RID: 3825
	private const byte MovingBit = 4;

	// Token: 0x04000EF2 RID: 3826
	private bool lastFireFlip;

	// Token: 0x04000EF3 RID: 3827
	private bool lastCannonToLeft;

	// Token: 0x04000EF4 RID: 3828
	private bool lastIsMoving;

	// Token: 0x04000EF5 RID: 3829
	private bool isMovingShared;
}
