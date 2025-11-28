using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000392 RID: 914
[NetworkBehaviourWeaved(128)]
public class ArcadeMachine : NetworkComponent
{
	// Token: 0x060015D6 RID: 5590 RVA: 0x0007A5A0 File Offset: 0x000787A0
	protected override void Awake()
	{
		base.Awake();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x0007A5B4 File Offset: 0x000787B4
	protected override void Start()
	{
		base.Start();
		if (this.arcadeGame != null && this.arcadeGame.Scale.x > 0f && this.arcadeGame.Scale.y > 0f)
		{
			this.arcadeGameInstance = Object.Instantiate<ArcadeGame>(this.arcadeGame, this.screen.transform);
			this.arcadeGameInstance.transform.localScale = new Vector3(1f / this.arcadeGameInstance.Scale.x, 1f / this.arcadeGameInstance.Scale.y, 1f);
			this.screen.forceRenderingOff = true;
			this.arcadeGameInstance.SetMachine(this);
		}
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x0007A684 File Offset: 0x00078884
	public void PlaySound(int soundId, int priority)
	{
		if (!this.audioSource.isPlaying || this.audioSourcePriority >= priority)
		{
			this.audioSource.GTStop();
			this.audioSourcePriority = priority;
			this.audioSource.clip = this.arcadeGameInstance.audioClips[soundId];
			this.audioSource.GTPlay();
			if (this.networkSynchronized && base.IsMine)
			{
				base.GetView.RPC("ArcadeGameInstance_OnPlaySound_RPC", 1, new object[]
				{
					soundId
				});
			}
		}
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x0007A70C File Offset: 0x0007890C
	public bool IsPlayerLocallyControlled(int player)
	{
		return this.sticks[player].heldByLocalPlayer;
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x0007A71C File Offset: 0x0007891C
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		for (int i = 0; i < this.sticks.Length; i++)
		{
			this.sticks[i].Init(this, i);
		}
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x0007A757 File Offset: 0x00078957
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x0007A768 File Offset: 0x00078968
	[PunRPC]
	private void ArcadeGameInstance_OnPlaySound_RPC(int id, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || id > this.arcadeGameInstance.audioClips.Length || id < 0 || !this.soundCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.clip = this.arcadeGameInstance.audioClips[id];
		this.audioSource.GTPlay();
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x0007A7D7 File Offset: 0x000789D7
	public void OnJoystickStateChange(int player, ArcadeButtons buttons)
	{
		if (this.arcadeGameInstance != null)
		{
			this.arcadeGameInstance.OnInputStateChange(player, buttons);
		}
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x0007A7F0 File Offset: 0x000789F0
	public bool IsControllerInUse(int player)
	{
		if (base.IsMine)
		{
			return this.playersPerJoystick[player] != null && Time.time < this.playerIdleTimeouts[player];
		}
		return (this.buttonsStateValue & 1 << player * 8) != 0;
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x060015DF RID: 5599 RVA: 0x0007A828 File Offset: 0x00078A28
	[Networked]
	[Capacity(128)]
	[NetworkedWeaved(0, 128)]
	[NetworkedWeavedArray(128, 1, typeof(ElementReaderWriterByte))]
	public unsafe NetworkArray<byte> Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArcadeMachine.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<byte>((byte*)(this.Ptr + 0), 128, ElementReaderWriterByte.GetInstance());
		}
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x00002789 File Offset: 0x00000989
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x0007A868 File Offset: 0x00078A68
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x0007A875 File Offset: 0x00078A75
	public void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.ReadPlayerDataPUN(player, stream, info);
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x0007A885 File Offset: 0x00078A85
	public void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.WritePlayerDataPUN(player, stream, info);
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x0007A8BC File Offset: 0x00078ABC
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		NetworkBehaviourUtils.InitializeNetworkArray<byte>(this.Data, this._Data, "Data");
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x0007A8DE File Offset: 0x00078ADE
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		NetworkBehaviourUtils.CopyFromNetworkArray<byte>(this.Data, ref this._Data);
	}

	// Token: 0x0400202F RID: 8239
	[SerializeField]
	private ArcadeGame arcadeGame;

	// Token: 0x04002030 RID: 8240
	[SerializeField]
	private ArcadeMachineJoystick[] sticks;

	// Token: 0x04002031 RID: 8241
	[SerializeField]
	private Renderer screen;

	// Token: 0x04002032 RID: 8242
	[SerializeField]
	private bool networkSynchronized = true;

	// Token: 0x04002033 RID: 8243
	[SerializeField]
	private CallLimiter soundCallLimit;

	// Token: 0x04002034 RID: 8244
	private int buttonsStateValue;

	// Token: 0x04002035 RID: 8245
	private AudioSource audioSource;

	// Token: 0x04002036 RID: 8246
	private int audioSourcePriority;

	// Token: 0x04002037 RID: 8247
	private ArcadeGame arcadeGameInstance;

	// Token: 0x04002038 RID: 8248
	private Player[] playersPerJoystick = new Player[4];

	// Token: 0x04002039 RID: 8249
	private float[] playerIdleTimeouts = new float[4];

	// Token: 0x0400203A RID: 8250
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 128)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private byte[] _Data;
}
