using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class CrittersCageDeposit : CrittersActorDeposit
{
	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06000191 RID: 401 RVA: 0x00009F78 File Offset: 0x00008178
	// (remove) Token: 0x06000192 RID: 402 RVA: 0x00009FB0 File Offset: 0x000081B0
	public event Action<Menagerie.CritterData, int> OnDepositCritter;

	// Token: 0x06000193 RID: 403 RVA: 0x00009FE5 File Offset: 0x000081E5
	private void Awake()
	{
		this.attachPoint.OnGrabbedChild += new Action<CrittersActor>(this.StartProcessCage);
	}

	// Token: 0x06000194 RID: 404 RVA: 0x00009FFE File Offset: 0x000081FE
	protected override bool CanDeposit(CrittersActor depositActor)
	{
		return base.CanDeposit(depositActor) && !this.isHandlingDeposit;
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000A014 File Offset: 0x00008214
	private void StartProcessCage(CrittersActor depositedActor)
	{
		this.currentCage = depositedActor;
		base.StartCoroutine(this.ProcessCage());
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000A02A File Offset: 0x0000822A
	private IEnumerator ProcessCage()
	{
		this.isHandlingDeposit = true;
		bool isLocalDeposit = this.currentCage.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber;
		this.depositAudio.GTPlayOneShot(this.depositStartSound, isLocalDeposit ? 1f : 0.5f);
		float transition = 0f;
		CrittersPawn crittersPawn = this.currentCage.GetComponentInChildren<CrittersPawn>();
		int lastGrabbedPlayer = this.currentCage.lastGrabbedPlayer;
		Menagerie.CritterData critterData;
		if (crittersPawn.IsNotNull())
		{
			critterData = new Menagerie.CritterData(crittersPawn.visuals);
		}
		else
		{
			critterData = new Menagerie.CritterData();
		}
		while (transition < this.submitDuration)
		{
			transition += Time.deltaTime;
			this.attachPoint.transform.localPosition = Vector3.Lerp(this.depositStartLocation, this.depositEndLocation, Mathf.Min(transition / this.submitDuration, 1f));
			yield return null;
		}
		if (crittersPawn.IsNotNull())
		{
			Action<Menagerie.CritterData, int> onDepositCritter = this.OnDepositCritter;
			if (onDepositCritter != null)
			{
				onDepositCritter.Invoke(critterData, lastGrabbedPlayer);
			}
			CrittersActor crittersActor = crittersPawn;
			bool keepWorldPosition = false;
			Vector3 zero = Vector3.zero;
			crittersActor.Released(keepWorldPosition, default(Quaternion), zero, default(Vector3), default(Vector3));
			crittersPawn.gameObject.SetActive(false);
			this.depositAudio.GTPlayOneShot(this.depositCritterSound, isLocalDeposit ? 1f : 0.5f);
		}
		else
		{
			this.depositAudio.GTPlayOneShot(this.depositEmptySound, isLocalDeposit ? 1f : 0.5f);
		}
		this.currentCage.transform.position = Vector3.zero;
		this.currentCage.gameObject.SetActive(false);
		this.currentCage = null;
		transition = 0f;
		while (transition < this.returnDuration)
		{
			transition += Time.deltaTime;
			this.attachPoint.transform.localPosition = Vector3.Lerp(this.depositEndLocation, this.depositStartLocation, Mathf.Min(transition / this.returnDuration, 1f));
			yield return null;
		}
		this.isHandlingDeposit = false;
		yield break;
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000A03C File Offset: 0x0000823C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.depositStartLocation), 0.1f);
		Gizmos.DrawLine(base.transform.TransformPoint(this.depositStartLocation), base.transform.TransformPoint(this.depositEndLocation));
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.depositEndLocation), 0.1f);
	}

	// Token: 0x040001CB RID: 459
	private bool isHandlingDeposit;

	// Token: 0x040001CC RID: 460
	public Vector3 depositStartLocation;

	// Token: 0x040001CD RID: 461
	public Vector3 depositEndLocation;

	// Token: 0x040001CE RID: 462
	public float submitDuration = 0.5f;

	// Token: 0x040001CF RID: 463
	public float returnDuration = 1f;

	// Token: 0x040001D0 RID: 464
	public AudioSource depositAudio;

	// Token: 0x040001D1 RID: 465
	public AudioClip depositStartSound;

	// Token: 0x040001D2 RID: 466
	public AudioClip depositEmptySound;

	// Token: 0x040001D3 RID: 467
	public AudioClip depositCritterSound;

	// Token: 0x040001D4 RID: 468
	private CrittersActor currentCage;
}
