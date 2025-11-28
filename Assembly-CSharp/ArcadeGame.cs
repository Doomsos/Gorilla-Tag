using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000391 RID: 913
public abstract class ArcadeGame : MonoBehaviour
{
	// Token: 0x060015C3 RID: 5571 RVA: 0x0007A3AB File Offset: 0x000785AB
	protected virtual void Awake()
	{
		this.InitializeMemoryStreams();
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x0007A3B3 File Offset: 0x000785B3
	public void InitializeMemoryStreams()
	{
		if (!this.memoryStreamsInitialized)
		{
			this.netStateMemStream = new MemoryStream(this.netStateBuffer, true);
			this.netStateMemStreamAlt = new MemoryStream(this.netStateBufferAlt, true);
			this.memoryStreamsInitialized = true;
		}
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x0007A3E8 File Offset: 0x000785E8
	public void SetMachine(ArcadeMachine machine)
	{
		this.machine = machine;
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x0007A3F1 File Offset: 0x000785F1
	protected bool getButtonState(int player, ArcadeButtons button)
	{
		return this.playerInputs[player].HasFlag(button);
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x0007A40C File Offset: 0x0007860C
	public void OnInputStateChange(int player, ArcadeButtons buttons)
	{
		for (int i = 1; i < 256; i += i)
		{
			ArcadeButtons arcadeButtons = (ArcadeButtons)i;
			bool flag = buttons.HasFlag(arcadeButtons);
			bool flag2 = this.playerInputs[player].HasFlag(arcadeButtons);
			if (flag != flag2)
			{
				if (flag)
				{
					this.ButtonDown(player, arcadeButtons);
				}
				else
				{
					this.ButtonUp(player, arcadeButtons);
				}
			}
		}
		this.playerInputs[player] = buttons;
	}

	// Token: 0x060015C8 RID: 5576
	public abstract byte[] GetNetworkState();

	// Token: 0x060015C9 RID: 5577
	public abstract void SetNetworkState(byte[] obj);

	// Token: 0x060015CA RID: 5578 RVA: 0x0007A478 File Offset: 0x00078678
	protected static void WrapNetState(object ns, MemoryStream stream)
	{
		if (stream == null)
		{
			Debug.LogWarning("Null MemoryStream passed to WrapNetState");
			return;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		stream.SetLength(0L);
		stream.Position = 0L;
		binaryFormatter.Serialize(stream, ns);
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x0007A4A4 File Offset: 0x000786A4
	protected static object UnwrapNetState(byte[] b)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(b);
		memoryStream.Position = 0L;
		object result = binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return result;
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x0007A4DC File Offset: 0x000786DC
	protected void SwapNetStateBuffersAndStreams()
	{
		byte[] array = this.netStateBufferAlt;
		byte[] array2 = this.netStateBuffer;
		this.netStateBuffer = array;
		this.netStateBufferAlt = array2;
		MemoryStream memoryStream = this.netStateMemStreamAlt;
		MemoryStream memoryStream2 = this.netStateMemStream;
		this.netStateMemStream = memoryStream;
		this.netStateMemStreamAlt = memoryStream2;
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x0007A521 File Offset: 0x00078721
	protected void PlaySound(int clipId, int prio = 3)
	{
		this.machine.PlaySound(clipId, prio);
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x0007A530 File Offset: 0x00078730
	protected bool IsPlayerLocallyControlled(int player)
	{
		return this.machine.IsPlayerLocallyControlled(player);
	}

	// Token: 0x060015CF RID: 5583
	protected abstract void ButtonUp(int player, ArcadeButtons button);

	// Token: 0x060015D0 RID: 5584
	protected abstract void ButtonDown(int player, ArcadeButtons button);

	// Token: 0x060015D1 RID: 5585
	public abstract void OnTimeout();

	// Token: 0x060015D2 RID: 5586 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x04002025 RID: 8229
	[SerializeField]
	public Vector2 Scale = new Vector2(1f, 1f);

	// Token: 0x04002026 RID: 8230
	private ArcadeButtons[] playerInputs = new ArcadeButtons[4];

	// Token: 0x04002027 RID: 8231
	public AudioClip[] audioClips;

	// Token: 0x04002028 RID: 8232
	private ArcadeMachine machine;

	// Token: 0x04002029 RID: 8233
	protected static int NetStateBufferSize = 512;

	// Token: 0x0400202A RID: 8234
	protected byte[] netStateBuffer = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x0400202B RID: 8235
	protected byte[] netStateBufferAlt = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x0400202C RID: 8236
	protected MemoryStream netStateMemStream;

	// Token: 0x0400202D RID: 8237
	protected MemoryStream netStateMemStreamAlt;

	// Token: 0x0400202E RID: 8238
	public bool memoryStreamsInitialized;
}
