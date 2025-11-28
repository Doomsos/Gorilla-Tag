using System;
using System.Collections.Generic;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000451 RID: 1105
[Serializable]
internal class HandEffectContext : IFXEffectContextObject
{
	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001C00 RID: 7168 RVA: 0x0009545F File Offset: 0x0009365F
	public List<int> PrefabPoolIds
	{
		get
		{
			return this.prefabHashes;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001C01 RID: 7169 RVA: 0x00095467 File Offset: 0x00093667
	public Vector3 Position
	{
		get
		{
			return this.position;
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06001C02 RID: 7170 RVA: 0x0009546F File Offset: 0x0009366F
	public Quaternion Rotation
	{
		get
		{
			return this.rotation;
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06001C03 RID: 7171 RVA: 0x00095477 File Offset: 0x00093677
	public float Speed
	{
		get
		{
			return this.speed;
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x06001C04 RID: 7172 RVA: 0x0009547F File Offset: 0x0009367F
	public Color Color
	{
		get
		{
			return this.color;
		}
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x06001C05 RID: 7173 RVA: 0x00095487 File Offset: 0x00093687
	public AudioSource SoundSource
	{
		get
		{
			return this.handSoundSource;
		}
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x06001C06 RID: 7174 RVA: 0x0009548F File Offset: 0x0009368F
	public AudioClip Sound
	{
		get
		{
			return this.soundFX;
		}
	}

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06001C07 RID: 7175 RVA: 0x00095497 File Offset: 0x00093697
	public float Volume
	{
		get
		{
			return this.soundVolume;
		}
	}

	// Token: 0x1700030C RID: 780
	// (get) Token: 0x06001C08 RID: 7176 RVA: 0x0009549F File Offset: 0x0009369F
	public float Pitch
	{
		get
		{
			return this.soundPitch;
		}
	}

	// Token: 0x06001C09 RID: 7177 RVA: 0x000954A7 File Offset: 0x000936A7
	public void AddFXPrefab(int hash)
	{
		this.prefabHashes.Add(hash);
	}

	// Token: 0x06001C0A RID: 7178 RVA: 0x000954B8 File Offset: 0x000936B8
	public void RemoveFXPrefab(int hash)
	{
		int num = this.prefabHashes.IndexOf(hash, 2);
		if (num >= 2)
		{
			this.prefabHashes.RemoveAt(num);
		}
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06001C0B RID: 7179 RVA: 0x000954E3 File Offset: 0x000936E3
	// (set) Token: 0x06001C0C RID: 7180 RVA: 0x000954EE File Offset: 0x000936EE
	public bool SeparateUpTapCooldown
	{
		get
		{
			return this.separateUpTapCooldownCount > 0;
		}
		set
		{
			this.separateUpTapCooldownCount = Mathf.Max(this.separateUpTapCooldownCount + (value ? 1 : -1), 0);
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06001C0D RID: 7181 RVA: 0x0009550A File Offset: 0x0009370A
	// (set) Token: 0x06001C0E RID: 7182 RVA: 0x0009551C File Offset: 0x0009371C
	public HandTapOverrides DownTapOverrides
	{
		get
		{
			return this.downTapOverrides ?? this.defaultDownTapOverrides;
		}
		set
		{
			this.downTapOverrides = value;
		}
	}

	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06001C0F RID: 7183 RVA: 0x00095525 File Offset: 0x00093725
	// (set) Token: 0x06001C10 RID: 7184 RVA: 0x00095537 File Offset: 0x00093737
	public HandTapOverrides UpTapOverrides
	{
		get
		{
			return this.upTapOverrides ?? this.defaultUpTapOverrides;
		}
		set
		{
			this.upTapOverrides = value;
		}
	}

	// Token: 0x1400003E RID: 62
	// (add) Token: 0x06001C11 RID: 7185 RVA: 0x00095540 File Offset: 0x00093740
	// (remove) Token: 0x06001C12 RID: 7186 RVA: 0x00095578 File Offset: 0x00093778
	public event Action<HandEffectContext> handTapDown;

	// Token: 0x1400003F RID: 63
	// (add) Token: 0x06001C13 RID: 7187 RVA: 0x000955B0 File Offset: 0x000937B0
	// (remove) Token: 0x06001C14 RID: 7188 RVA: 0x000955E8 File Offset: 0x000937E8
	public event Action<HandEffectContext> handTapUp;

	// Token: 0x06001C15 RID: 7189 RVA: 0x0009561D File Offset: 0x0009381D
	public void OnTriggerActions()
	{
		if (this.isDownTap)
		{
			Action<HandEffectContext> action = this.handTapDown;
			if (action == null)
			{
				return;
			}
			action.Invoke(this);
			return;
		}
		else
		{
			Action<HandEffectContext> action2 = this.handTapUp;
			if (action2 == null)
			{
				return;
			}
			action2.Invoke(this);
			return;
		}
	}

	// Token: 0x06001C16 RID: 7190 RVA: 0x0009564C File Offset: 0x0009384C
	public void OnPlayVisualFX(int fxID, GameObject fx)
	{
		FXModifier fxmodifier;
		if (fx.TryGetComponent<FXModifier>(ref fxmodifier))
		{
			fxmodifier.UpdateScale(this.soundVolume * ((fxID == GorillaAmbushManager.HandEffectHash) ? GorillaAmbushManager.HandFXScaleModifier : 1f), this.color);
		}
	}

	// Token: 0x06001C17 RID: 7191 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPlaySoundFX(AudioSource audioSource)
	{
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x0009568A File Offset: 0x0009388A
	public HandEffectContext()
	{
		List<int> list = new List<int>();
		list.Add(-1);
		list.Add(-1);
		this.prefabHashes = list;
		this.color = Color.white;
		base..ctor();
	}

	// Token: 0x04002609 RID: 9737
	internal List<int> prefabHashes;

	// Token: 0x0400260A RID: 9738
	internal Vector3 position;

	// Token: 0x0400260B RID: 9739
	internal Quaternion rotation;

	// Token: 0x0400260C RID: 9740
	internal float speed;

	// Token: 0x0400260D RID: 9741
	internal Color color;

	// Token: 0x0400260E RID: 9742
	[SerializeField]
	internal AudioSource handSoundSource;

	// Token: 0x0400260F RID: 9743
	internal AudioClip soundFX;

	// Token: 0x04002610 RID: 9744
	internal float soundVolume;

	// Token: 0x04002611 RID: 9745
	internal float soundPitch;

	// Token: 0x04002612 RID: 9746
	internal int separateUpTapCooldownCount;

	// Token: 0x04002613 RID: 9747
	[SerializeField]
	internal HandTapOverrides defaultDownTapOverrides;

	// Token: 0x04002614 RID: 9748
	internal HandTapOverrides downTapOverrides;

	// Token: 0x04002615 RID: 9749
	[SerializeField]
	internal HandTapOverrides defaultUpTapOverrides;

	// Token: 0x04002616 RID: 9750
	internal HandTapOverrides upTapOverrides;

	// Token: 0x04002619 RID: 9753
	internal bool isDownTap;

	// Token: 0x0400261A RID: 9754
	internal bool isLeftHand;
}
