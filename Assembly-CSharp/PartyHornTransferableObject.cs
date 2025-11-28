using System;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200019E RID: 414
public class PartyHornTransferableObject : TransferrableObject
{
	// Token: 0x06000B2D RID: 2861 RVA: 0x0003CCB2 File Offset: 0x0003AEB2
	internal override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x0003CCC0 File Offset: 0x0003AEC0
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x0003CCC8 File Offset: 0x0003AEC8
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0003CCD8 File Offset: 0x0003AED8
	protected Vector3 CalcMouthPiecePos()
	{
		if (!this.mouthPiece)
		{
			return base.transform.position + this.mouthPieceZOffset * base.transform.forward;
		}
		return this.mouthPiece.position;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0003CD24 File Offset: 0x0003AF24
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState != TransferrableObject.ItemStates.State0)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3 vector = this.CalcMouthPiecePos();
		float num = this.mouthPieceRadius * this.mouthPieceRadius * GTPlayer.Instance.scale * GTPlayer.Instance.scale;
		bool flag = (GorillaTagger.Instance.offlineVRRig.GetMouthPosition() - vector).sqrMagnitude < num;
		if (this.soundActivated && PhotonNetwork.InRoom)
		{
			bool flag2;
			if (flag)
			{
				GorillaTagger instance = GorillaTagger.Instance;
				if (instance == null)
				{
					flag2 = false;
				}
				else
				{
					Recorder myRecorder = instance.myRecorder;
					bool? flag3 = (myRecorder != null) ? new bool?(myRecorder.IsCurrentlyTransmitting) : default(bool?);
					bool flag4 = true;
					flag2 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
				}
			}
			else
			{
				flag2 = false;
			}
			flag = flag2;
		}
		for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
		{
			VRRig vrrig = GorillaParent.instance.vrrigs[i];
			if (flag)
			{
				break;
			}
			flag = ((vrrig.GetMouthPosition() - vector).sqrMagnitude < num);
			if (this.soundActivated)
			{
				bool flag5;
				if (flag)
				{
					RigContainer rigContainer = vrrig.rigContainer;
					if (rigContainer == null)
					{
						flag5 = false;
					}
					else
					{
						PhotonVoiceView voice = rigContainer.Voice;
						bool? flag3 = (voice != null) ? new bool?(voice.IsSpeaking) : default(bool?);
						bool flag4 = true;
						flag5 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
					}
				}
				else
				{
					flag5 = false;
				}
				flag = flag5;
			}
		}
		this.itemState = (flag ? TransferrableObject.ItemStates.State1 : this.itemState);
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0003CEC0 File Offset: 0x0003B0C0
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (TransferrableObject.ItemStates.State1 != this.itemState)
		{
			return;
		}
		if (!this.localWasActivated)
		{
			if (this.effectsGameObject)
			{
				this.effectsGameObject.SetActive(true);
			}
			this.cooldownRemaining = this.cooldown;
			this.localWasActivated = true;
			UnityEvent onCooldownStart = this.OnCooldownStart;
			if (onCooldownStart != null)
			{
				onCooldownStart.Invoke();
			}
		}
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.InitToDefault();
		}
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0003CF48 File Offset: 0x0003B148
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.effectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.cooldownRemaining = this.cooldown;
		this.localWasActivated = false;
		UnityEvent onCooldownReset = this.OnCooldownReset;
		if (onCooldownReset == null)
		{
			return;
		}
		onCooldownReset.Invoke();
	}

	// Token: 0x04000D95 RID: 3477
	[Tooltip("This GameObject will activate when held to any gorilla's mouth.")]
	public GameObject effectsGameObject;

	// Token: 0x04000D96 RID: 3478
	public float cooldown = 2f;

	// Token: 0x04000D97 RID: 3479
	public float mouthPieceZOffset = -0.18f;

	// Token: 0x04000D98 RID: 3480
	public float mouthPieceRadius = 0.05f;

	// Token: 0x04000D99 RID: 3481
	public Transform mouthPiece;

	// Token: 0x04000D9A RID: 3482
	public bool soundActivated;

	// Token: 0x04000D9B RID: 3483
	public UnityEvent OnCooldownStart;

	// Token: 0x04000D9C RID: 3484
	public UnityEvent OnCooldownReset;

	// Token: 0x04000D9D RID: 3485
	private float cooldownRemaining;

	// Token: 0x04000D9E RID: 3486
	private PartyHornTransferableObject.PartyHornState partyHornStateLastFrame;

	// Token: 0x04000D9F RID: 3487
	private bool localWasActivated;

	// Token: 0x0200019F RID: 415
	private enum PartyHornState
	{
		// Token: 0x04000DA1 RID: 3489
		None = 1,
		// Token: 0x04000DA2 RID: 3490
		CoolingDown
	}
}
