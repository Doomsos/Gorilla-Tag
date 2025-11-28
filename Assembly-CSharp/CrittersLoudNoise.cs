using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200005C RID: 92
public class CrittersLoudNoise : CrittersActor
{
	// Token: 0x060001BB RID: 443 RVA: 0x0000AA68 File Offset: 0x00008C68
	public override void OnEnable()
	{
		base.OnEnable();
		this.SetTimeEnabled();
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000AA76 File Offset: 0x00008C76
	public void SpawnData(float _soundVolume, float _soundDuration, float _soundMultiplier, bool _soundEnabled)
	{
		this.soundVolume = _soundVolume;
		this.volumeFearAttractionMultiplier = _soundMultiplier;
		this.soundDuration = _soundDuration;
		this.soundEnabled = _soundEnabled;
		this.Initialize();
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000AA9C File Offset: 0x00008C9C
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (!this.isEnabled)
		{
			return flag;
		}
		this.wasEnabled = base.gameObject.activeSelf;
		this.wasSoundEnabled = this.soundEnabled;
		if (PhotonNetwork.InRoom)
		{
			if (PhotonNetwork.Time > this.timeSoundEnabled + (double)this.soundDuration || this.timeSoundEnabled > PhotonNetwork.Time)
			{
				this.soundEnabled = false;
			}
		}
		else if ((double)Time.time > this.timeSoundEnabled + (double)this.soundDuration || this.timeSoundEnabled > (double)Time.time)
		{
			this.soundEnabled = false;
		}
		if (this.disableWhenSoundDisabled && !this.soundEnabled)
		{
			this.isEnabled = false;
			if (base.gameObject.activeSelf != this.isEnabled)
			{
				base.gameObject.SetActive(this.isEnabled);
			}
		}
		this.updatedSinceLastFrame = (flag || this.wasSoundEnabled != this.soundEnabled || this.wasEnabled != this.isEnabled);
		return this.updatedSinceLastFrame;
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000ABA0 File Offset: 0x00008DA0
	public override void ProcessRemote()
	{
		if (!this.wasEnabled && this.isEnabled)
		{
			this.SetTimeEnabled();
		}
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000ABB8 File Offset: 0x00008DB8
	public void SetTimeEnabled()
	{
		if (PhotonNetwork.InRoom)
		{
			this.timeSoundEnabled = PhotonNetwork.Time;
			return;
		}
		this.timeSoundEnabled = (double)Time.time;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000ABDC File Offset: 0x00008DDC
	public override void CalculateFear(CrittersPawn critter, float multiplier)
	{
		if (this.soundEnabled)
		{
			if (this.soundDuration == 0f)
			{
				critter.IncreaseFear(this.soundVolume * this.volumeFearAttractionMultiplier * multiplier, this);
				return;
			}
			if ((PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.timeSoundEnabled < (double)this.soundDuration)
			{
				critter.IncreaseFear(this.soundVolume * this.volumeFearAttractionMultiplier * Time.deltaTime * multiplier, this);
			}
		}
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000AC58 File Offset: 0x00008E58
	public override void CalculateAttraction(CrittersPawn critter, float multiplier)
	{
		if (this.soundEnabled)
		{
			if (this.soundDuration == 0f)
			{
				critter.IncreaseAttraction(this.soundVolume * this.volumeFearAttractionMultiplier * multiplier, this);
				return;
			}
			if ((PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.timeSoundEnabled < (double)this.soundDuration)
			{
				critter.IncreaseAttraction(this.soundVolume * this.volumeFearAttractionMultiplier * Time.deltaTime * multiplier, this);
			}
		}
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000ACD4 File Offset: 0x00008ED4
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		float value;
		float value2;
		bool flag;
		float value3;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out value) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out value2) & CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out value3)))
		{
			return false;
		}
		this.soundVolume = value.GetFinite();
		this.soundDuration = value2.GetFinite();
		this.soundEnabled = flag;
		this.volumeFearAttractionMultiplier = value3.GetFinite();
		return true;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000AD50 File Offset: 0x00008F50
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.soundVolume);
		stream.SendNext(this.soundDuration);
		stream.SendNext(this.soundEnabled);
		stream.SendNext(this.volumeFearAttractionMultiplier);
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000ADA8 File Offset: 0x00008FA8
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.soundVolume);
		objList.Add(this.soundDuration);
		objList.Add(this.soundEnabled);
		objList.Add(this.volumeFearAttractionMultiplier);
		return this.TotalActorDataLength();
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000AE0B File Offset: 0x0000900B
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 4;
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000AE18 File Offset: 0x00009018
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		float value;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex], out value))
		{
			return this.TotalActorDataLength();
		}
		float value2;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 1], out value2))
		{
			return this.TotalActorDataLength();
		}
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex + 2], out flag))
		{
			return this.TotalActorDataLength();
		}
		float value3;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 3], out value3))
		{
			return this.TotalActorDataLength();
		}
		this.soundVolume = value.GetFinite();
		this.soundDuration = value2.GetFinite();
		this.soundEnabled = flag;
		this.volumeFearAttractionMultiplier = value3.GetFinite();
		return this.TotalActorDataLength();
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000AEB4 File Offset: 0x000090B4
	public void PlayHandTapLocal(bool isLeft)
	{
		this.timeSoundEnabled = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		this.soundEnabled = true;
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000AED7 File Offset: 0x000090D7
	public void PlayHandTapRemote(double serverTime, bool isLeft)
	{
		this.timeSoundEnabled = serverTime;
		this.soundEnabled = true;
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000AEE7 File Offset: 0x000090E7
	public void PlayVoiceSpeechLocal(double serverTime, float duration, float volume)
	{
		this.soundDuration = duration;
		this.timeSoundEnabled = serverTime;
		this.soundVolume = volume;
		this.soundEnabled = true;
	}

	// Token: 0x04000208 RID: 520
	public float soundVolume;

	// Token: 0x04000209 RID: 521
	public float volumeFearAttractionMultiplier;

	// Token: 0x0400020A RID: 522
	public float soundDuration;

	// Token: 0x0400020B RID: 523
	public double timeSoundEnabled;

	// Token: 0x0400020C RID: 524
	public bool soundEnabled;

	// Token: 0x0400020D RID: 525
	private bool wasSoundEnabled;

	// Token: 0x0400020E RID: 526
	public bool disableWhenSoundDisabled;
}
