using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005C1 RID: 1473
public class ChestHeartbeat : MonoBehaviour
{
	// Token: 0x06002540 RID: 9536 RVA: 0x000C7DA8 File Offset: 0x000C5FA8
	public void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			if ((PhotonNetwork.ServerTimestamp > this.lastShot + this.millisMin || Mathf.Abs(PhotonNetwork.ServerTimestamp - this.lastShot) > 10000) && PhotonNetwork.ServerTimestamp % 1500 <= 10)
			{
				this.lastShot = PhotonNetwork.ServerTimestamp;
				this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
				base.StartCoroutine(this.HeartBeat());
				return;
			}
		}
		else if ((Time.time * 1000f > (float)(this.lastShot + this.millisMin) || Mathf.Abs(Time.time * 1000f - (float)this.lastShot) > 10000f) && Time.time * 1000f % 1500f <= 10f)
		{
			this.lastShot = PhotonNetwork.ServerTimestamp;
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
			base.StartCoroutine(this.HeartBeat());
		}
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x000C7EB6 File Offset: 0x000C60B6
	private IEnumerator HeartBeat()
	{
		float startTime = Time.time;
		while (Time.time < startTime + this.endtime)
		{
			if (Time.time < startTime + this.minTime)
			{
				this.deltaTime = Time.time - startTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * this.heartMinSize, this.deltaTime / this.minTime);
			}
			else if (Time.time < startTime + this.maxTime)
			{
				this.deltaTime = Time.time - startTime - this.minTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMinSize, Vector3.one * this.heartMaxSize, this.deltaTime / (this.maxTime - this.minTime));
			}
			else if (Time.time < startTime + this.endtime)
			{
				this.deltaTime = Time.time - startTime - this.maxTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMaxSize, Vector3.one, this.deltaTime / (this.endtime - this.maxTime));
			}
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x040030E5 RID: 12517
	public int millisToWait;

	// Token: 0x040030E6 RID: 12518
	public int millisMin = 300;

	// Token: 0x040030E7 RID: 12519
	public int lastShot;

	// Token: 0x040030E8 RID: 12520
	public AudioSource audioSource;

	// Token: 0x040030E9 RID: 12521
	public Transform scaleTransform;

	// Token: 0x040030EA RID: 12522
	private float deltaTime;

	// Token: 0x040030EB RID: 12523
	private float heartMinSize = 0.9f;

	// Token: 0x040030EC RID: 12524
	private float heartMaxSize = 1.2f;

	// Token: 0x040030ED RID: 12525
	private float minTime = 0.05f;

	// Token: 0x040030EE RID: 12526
	private float maxTime = 0.1f;

	// Token: 0x040030EF RID: 12527
	private float endtime = 0.25f;

	// Token: 0x040030F0 RID: 12528
	private float currentTime;
}
