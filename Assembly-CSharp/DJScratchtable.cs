using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class DJScratchtable : MonoBehaviour
{
	// Token: 0x06000F64 RID: 3940 RVA: 0x000519DF File Offset: 0x0004FBDF
	public void SetPlaying(bool playing)
	{
		this.isPlaying = playing;
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x000519E8 File Offset: 0x0004FBE8
	private void OnTriggerStay(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 vector = (base.transform.parent.InverseTransformPoint(collider.transform.position) - base.transform.localPosition).WithY(0f);
		float num = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		if (this.isTouching)
		{
			base.transform.localRotation = Quaternion.LookRotation(vector) * this.firstTouchRotation;
			if (this.isPlaying)
			{
				float num2 = Mathf.DeltaAngle(this.lastScratchSoundAngle, num);
				if (num2 > this.scratchMinAngle)
				{
					if (Time.time > this.cantForwardScratchUntilTimestamp)
					{
						this.scratchPlayer.Play(ScratchSoundType.Forward, this.isLeft);
						this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
						this.lastScratchSoundAngle = num;
						GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
					}
				}
				else if (num2 < -this.scratchMinAngle && Time.time > this.cantBackScratchUntilTimestamp)
				{
					this.scratchPlayer.Play(ScratchSoundType.Back, this.isLeft);
					this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
					this.lastScratchSoundAngle = num;
					GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
				}
			}
		}
		else
		{
			this.firstTouchRotation = Quaternion.Inverse(Quaternion.LookRotation(base.transform.InverseTransformPoint(collider.transform.position).WithY(0f)));
			if (this.isPlaying)
			{
				this.PauseTrack();
				this.scratchPlayer.Play(ScratchSoundType.Pause, this.isLeft);
				this.lastScratchSoundAngle = num;
				this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
				this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
			}
		}
		this.isTouching = true;
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x00051BEC File Offset: 0x0004FDEC
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		if (this.isPlaying)
		{
			this.ResumeTrack();
			this.scratchPlayer.Play(ScratchSoundType.Resume, this.isLeft);
		}
		this.isTouching = false;
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x00051C38 File Offset: 0x0004FE38
	public void SelectTrack(int track)
	{
		this.lastSelectedTrack = track;
		if (track == 0)
		{
			this.turntableVisual.Stop();
			this.isPlaying = false;
		}
		else
		{
			this.turntableVisual.Run();
			this.isPlaying = true;
		}
		int num = track - 1;
		for (int i = 0; i < this.tracks.Length; i++)
		{
			if (num == i)
			{
				float time = (float)(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) % this.trackDuration;
				this.tracks[i].Play();
				this.tracks[i].time = time;
			}
			else
			{
				this.tracks[i].Stop();
			}
		}
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x00051CD8 File Offset: 0x0004FED8
	public void PauseTrack()
	{
		for (int i = 0; i < this.tracks.Length; i++)
		{
			this.tracks[i].Stop();
		}
		this.pausedUntilTimestamp = Time.time + 1f;
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x00051D16 File Offset: 0x0004FF16
	public void ResumeTrack()
	{
		this.SelectTrack(this.lastSelectedTrack);
		this.pausedUntilTimestamp = 0f;
	}

	// Token: 0x040012F3 RID: 4851
	[SerializeField]
	private bool isLeft;

	// Token: 0x040012F4 RID: 4852
	[SerializeField]
	private DJScratchSoundPlayer scratchPlayer;

	// Token: 0x040012F5 RID: 4853
	[SerializeField]
	private float scratchCooldown;

	// Token: 0x040012F6 RID: 4854
	[SerializeField]
	private float scratchMinAngle;

	// Token: 0x040012F7 RID: 4855
	[SerializeField]
	private AudioSource[] tracks;

	// Token: 0x040012F8 RID: 4856
	[SerializeField]
	private CosmeticFan turntableVisual;

	// Token: 0x040012F9 RID: 4857
	[SerializeField]
	private float trackDuration;

	// Token: 0x040012FA RID: 4858
	[SerializeField]
	private float hapticStrength;

	// Token: 0x040012FB RID: 4859
	[SerializeField]
	private float hapticDuration;

	// Token: 0x040012FC RID: 4860
	private int lastSelectedTrack;

	// Token: 0x040012FD RID: 4861
	private bool isPlaying;

	// Token: 0x040012FE RID: 4862
	private bool isTouching;

	// Token: 0x040012FF RID: 4863
	private Quaternion firstTouchRotation;

	// Token: 0x04001300 RID: 4864
	private float lastScratchSoundAngle;

	// Token: 0x04001301 RID: 4865
	private float cantForwardScratchUntilTimestamp;

	// Token: 0x04001302 RID: 4866
	private float cantBackScratchUntilTimestamp;

	// Token: 0x04001303 RID: 4867
	private float pausedUntilTimestamp;
}
