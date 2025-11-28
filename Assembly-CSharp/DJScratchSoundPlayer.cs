using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200024D RID: 589
public class DJScratchSoundPlayer : MonoBehaviour, ISpawnable
{
	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000F58 RID: 3928 RVA: 0x00051761 File Offset: 0x0004F961
	// (set) Token: 0x06000F59 RID: 3929 RVA: 0x00051769 File Offset: 0x0004F969
	public bool IsSpawned { get; set; }

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000F5A RID: 3930 RVA: 0x00051772 File Offset: 0x0004F972
	// (set) Token: 0x06000F5B RID: 3931 RVA: 0x0005177A File Offset: 0x0004F97A
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000F5C RID: 3932 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDespawn()
	{
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x00051784 File Offset: 0x0004F984
	private void OnEnable()
	{
		if (this._events.IsNull())
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = (this.myRig != null) ? ((this.myRig.creator != null) ? this.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null;
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
		}
		this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnPlayEvent);
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x00051816 File Offset: 0x0004FA16
	private void OnDisable()
	{
		if (this._events.IsNotNull())
		{
			this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnPlayEvent);
			this._events.Dispose();
		}
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x00051852 File Offset: 0x0004FA52
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!rig.isLocal)
		{
			this.scratchTableLeft.enabled = false;
			this.scratchTableRight.enabled = false;
		}
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x0005187B File Offset: 0x0004FA7B
	public void Play(ScratchSoundType type, bool isLeft)
	{
		if (this.myRig.isLocal)
		{
			this.PlayLocal(type, isLeft);
			this._events.Activate.RaiseOthers(new object[]
			{
				(int)(type + (isLeft ? 100 : 0))
			});
		}
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x000518BC File Offset: 0x0004FABC
	public void OnPlayEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length != 1)
		{
			Debug.LogError(string.Format("Invalid DJ Scratch Event - expected 1 arg, got {0}", args.Length));
			return;
		}
		int num = (int)args[0];
		bool flag = num >= 100;
		if (flag)
		{
			num -= 100;
		}
		ScratchSoundType scratchSoundType = (ScratchSoundType)num;
		if (scratchSoundType < ScratchSoundType.Pause || scratchSoundType > ScratchSoundType.Back)
		{
			return;
		}
		this.PlayLocal(scratchSoundType, flag);
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x00051934 File Offset: 0x0004FB34
	public void PlayLocal(ScratchSoundType type, bool isLeft)
	{
		switch (type)
		{
		case ScratchSoundType.Pause:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			this.scratchPause.Play();
			return;
		case ScratchSoundType.Resume:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).ResumeTrack();
			this.scratchResume.Play();
			return;
		case ScratchSoundType.Forward:
			this.scratchForward.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		case ScratchSoundType.Back:
			this.scratchBack.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		default:
			return;
		}
	}

	// Token: 0x040012E9 RID: 4841
	[SerializeField]
	private SoundBankPlayer scratchForward;

	// Token: 0x040012EA RID: 4842
	[SerializeField]
	private SoundBankPlayer scratchBack;

	// Token: 0x040012EB RID: 4843
	[SerializeField]
	private SoundBankPlayer scratchPause;

	// Token: 0x040012EC RID: 4844
	[SerializeField]
	private SoundBankPlayer scratchResume;

	// Token: 0x040012ED RID: 4845
	[SerializeField]
	private DJScratchtable scratchTableLeft;

	// Token: 0x040012EE RID: 4846
	[SerializeField]
	private DJScratchtable scratchTableRight;

	// Token: 0x040012EF RID: 4847
	private RubberDuckEvents _events;

	// Token: 0x040012F0 RID: 4848
	private VRRig myRig;
}
