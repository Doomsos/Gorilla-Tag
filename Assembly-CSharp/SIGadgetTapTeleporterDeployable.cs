using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class SIGadgetTapTeleporterDeployable : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06000639 RID: 1593 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00023503 File Offset: 0x00021703
	private void OnEnable()
	{
		this.activateTime = Time.time + this.activateDelay;
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00023518 File Offset: 0x00021718
	private void LateUpdate()
	{
		if (Time.time > this.timeToDie && this.gameEntity.IsAuthority())
		{
			if (this.linkedPoint != null)
			{
				this.linkedPoint.ClearLink();
			}
			this.gameEntity.manager.RequestDestroyItem(this.gameEntity.id);
		}
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x00023574 File Offset: 0x00021774
	public void OnEntityInit()
	{
		int num;
		BitPackUtils.UnpackIntsFromLong(this.gameEntity.createData, out this.selectionId, out num);
		if ((float)num < 0f)
		{
			this.timeToDie = float.PositiveInfinity;
		}
		else
		{
			this.timeToDie = Time.time + (float)num;
		}
		this.UpdateSelectionDisplay();
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x000235C3 File Offset: 0x000217C3
	private void UpdateSelectionDisplay()
	{
		if (this.selectionId == 0)
		{
			this.selectionColorDisplay.material = this.selectionColor1;
			return;
		}
		if (this.selectionId == 1)
		{
			this.selectionColorDisplay.material = this.selectionColor2;
		}
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x000235FC File Offset: 0x000217FC
	public void OnEntityStateChange(long prevState, long newState)
	{
		if (this.gameEntity.IsAuthority())
		{
			return;
		}
		int netId;
		int netId2;
		BitPackUtils.UnpackIntsFromLong(newState, out netId, out netId2);
		GameEntity gameEntityFromNetId = this.gameEntity.manager.GetGameEntityFromNetId(netId);
		if (gameEntityFromNetId != null)
		{
			SIGadgetTapTeleporter component = gameEntityFromNetId.GetComponent<SIGadgetTapTeleporter>();
			this._pad = component;
			this.identifierColor = this._pad.identifierColor;
		}
		GameEntity gameEntityFromNetId2 = this.gameEntity.manager.GetGameEntityFromNetId(netId2);
		if (gameEntityFromNetId2 != null)
		{
			this.linkedPoint = gameEntityFromNetId2.GetComponent<SIGadgetTapTeleporterDeployable>();
			if (this.linkedPoint.linkedPoint == null)
			{
				this.linkedPoint.linkedPoint = this;
				this.linkedPoint._pad = this._pad;
				this.linkedPoint.identifierColor = this.identifierColor;
				this.linkedPoint.UpdateLinkDisplay();
			}
		}
		else
		{
			this.linkedPoint = null;
		}
		this.UpdateLinkDisplay();
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x000236E0 File Offset: 0x000218E0
	public void SetLink(SIGadgetTapTeleporter newPad, SIGadgetTapTeleporterDeployable newLink)
	{
		this._pad = newPad;
		this.linkedPoint = newLink;
		this.identifierColor = this._pad.identifierColor;
		int value = -1;
		if (this.linkedPoint != null)
		{
			value = this.linkedPoint.gameEntity.GetNetId();
		}
		this.gameEntity.RequestState(this.gameEntity.id, BitPackUtils.PackIntsIntoLong(this._pad.gameEntity.GetNetId(), value));
		this.UpdateLinkDisplay();
		this.stealth.enabled = this._pad.useStealthTeleporters;
		this.maintainVelocity = this._pad.isVelocityPreserved;
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00023786 File Offset: 0x00021986
	private void ClearLink()
	{
		this.linkedPoint = null;
		this.gameEntity.RequestState(this.gameEntity.id, BitPackUtils.PackIntsIntoLong(this._pad.gameEntity.GetNetId(), -1));
		this.UpdateLinkDisplay();
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x000237C4 File Offset: 0x000219C4
	private void UpdateLinkDisplay()
	{
		Renderer[] array = this.identifierColorDisplay;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.color = this.identifierColor;
		}
		if (this.linkedPoint != null)
		{
			Vector3 vector = this.linkedPoint.transform.position - base.transform.position;
			this.linkDirectionIndicator.gameObject.SetActive(true);
			this.linkDirectionIndicator.transform.rotation = Quaternion.LookRotation(base.transform.forward, vector.normalized);
			return;
		}
		this.linkDirectionIndicator.gameObject.SetActive(false);
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00023872 File Offset: 0x00021A72
	public void TryTeleport()
	{
		if (this.activateTime < Time.time && SIGadgetTapTeleporterDeployable.reteleportTime < Time.time && (!this.requiresSurfaceTapSinceTeleport || GorillaTagger.Instance.hasTappedSurface))
		{
			this.TeleportToLinked();
		}
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x000238A7 File Offset: 0x00021AA7
	private void ResetRetriggerBlock()
	{
		SIGadgetTapTeleporterDeployable.reteleportTime = Time.time + SIGadgetTapTeleporterDeployable.reteleportDelay;
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x000238BC File Offset: 0x00021ABC
	private void TeleportToLinked()
	{
		if (this.linkedPoint == null || !this.linkedPoint.gameObject.activeSelf)
		{
			return;
		}
		Vector3 position = this.destination.position;
		if (Vector3.Distance(GTPlayer.Instance.transform.position, position) > this.teleportCheckDistance)
		{
			return;
		}
		this.ResetRetriggerBlock();
		if (this.requiresSurfaceTapSinceTeleport)
		{
			GorillaTagger.Instance.ResetTappedSurfaceCheck();
		}
		Vector3 position2 = this.linkedPoint.destination.position;
		Quaternion rotation = GTPlayer.Instance.transform.rotation;
		GTPlayer.Instance.TeleportTo(position2, rotation, this.maintainVelocity, true);
		this.linkedPoint.teleportSoundbank.Play();
	}

	// Token: 0x040007B9 RID: 1977
	public GameEntity gameEntity;

	// Token: 0x040007BA RID: 1978
	[SerializeField]
	private Transform destination;

	// Token: 0x040007BB RID: 1979
	[SerializeField]
	private Renderer[] identifierColorDisplay;

	// Token: 0x040007BC RID: 1980
	[SerializeField]
	private Transform linkDirectionIndicator;

	// Token: 0x040007BD RID: 1981
	[SerializeField]
	private Renderer selectionColorDisplay;

	// Token: 0x040007BE RID: 1982
	[SerializeField]
	private Material selectionColor1;

	// Token: 0x040007BF RID: 1983
	[SerializeField]
	private Material selectionColor2;

	// Token: 0x040007C0 RID: 1984
	[SerializeField]
	private SoundBankPlayer teleportSoundbank;

	// Token: 0x040007C1 RID: 1985
	[SerializeField]
	private SIGameEntityStealthVisibility stealth;

	// Token: 0x040007C2 RID: 1986
	[SerializeField]
	private bool requiresSurfaceTapSinceTeleport;

	// Token: 0x040007C3 RID: 1987
	private bool maintainVelocity;

	// Token: 0x040007C4 RID: 1988
	private int selectionId;

	// Token: 0x040007C5 RID: 1989
	private SIGadgetTapTeleporter _pad;

	// Token: 0x040007C6 RID: 1990
	private SIGadgetTapTeleporterDeployable linkedPoint;

	// Token: 0x040007C7 RID: 1991
	private float activateDelay = 0.3f;

	// Token: 0x040007C8 RID: 1992
	private float activateTime;

	// Token: 0x040007C9 RID: 1993
	private static float reteleportDelay = 0.3f;

	// Token: 0x040007CA RID: 1994
	private static float reteleportTime;

	// Token: 0x040007CB RID: 1995
	private Color identifierColor;

	// Token: 0x040007CC RID: 1996
	private float timeToDie = -1f;

	// Token: 0x040007CD RID: 1997
	private float teleportCheckDistance = 2f;
}
