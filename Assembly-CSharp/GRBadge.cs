using System;
using System.Collections;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000684 RID: 1668
public class GRBadge : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002AA9 RID: 10921 RVA: 0x000E5C9C File Offset: 0x000E3E9C
	public void OnEntityInit()
	{
		this.gameEntity.manager.ghostReactorManager.reactor.employeeBadges.LinkBadgeToDispenser(this, (long)((int)this.gameEntity.createData));
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x000E5CCC File Offset: 0x000E3ECC
	private void OnDestroy()
	{
		GhostReactor ghostReactor = GhostReactor.Get(this.gameEntity);
		if (ghostReactor != null && ghostReactor.employeeBadges != null)
		{
			ghostReactor.employeeBadges.RemoveBadge(this);
		}
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x000E5D08 File Offset: 0x000E3F08
	public void Setup(NetPlayer player, int index)
	{
		this.gameEntity.onlyGrabActorNumber = player.ActorNumber;
		this.dispenserIndex = index;
		this.actorNr = player.ActorNumber;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		bool flag = (int)this.gameEntity.GetState() == 1;
		if (player.IsLocal)
		{
			flag |= (Time.timeAsDouble < grplayer.lastLeftWithBadgeAttachedTime + 60.0);
		}
		if (grplayer != null && flag)
		{
			base.transform.position = grplayer.badgeBodyAnchor.position;
			grplayer.AttachBadge(this);
		}
		this.RefreshText(player);
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x000E5DA8 File Offset: 0x000E3FA8
	public void RefreshText(NetPlayer player)
	{
		this.playerName.text = player.SanitizedNickName;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		if (grplayer != null && this.lastRedeemedPoints != grplayer.CurrentProgression.redeemedPoints)
		{
			this.lastRedeemedPoints = grplayer.CurrentProgression.redeemedPoints;
			this.playerTitle.text = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			this.playerLevel.text = GhostReactorProgression.GetGrade(grplayer.CurrentProgression.redeemedPoints).ToString();
		}
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x000E5E40 File Offset: 0x000E4040
	public void Hide()
	{
		this.badgeMesh.enabled = false;
		this.playerName.gameObject.SetActive(false);
		this.playerTitle.gameObject.SetActive(false);
		this.playerLevel.gameObject.SetActive(false);
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x000E5E8C File Offset: 0x000E408C
	public void UnHide()
	{
		this.badgeMesh.enabled = true;
		this.playerName.gameObject.SetActive(true);
		this.playerTitle.gameObject.SetActive(true);
		this.playerLevel.gameObject.SetActive(true);
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x000E5ED8 File Offset: 0x000E40D8
	public bool IsAttachedToPlayer()
	{
		return (int)this.gameEntity.GetState() == 1;
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000E5EEC File Offset: 0x000E40EC
	public void StartRetracting()
	{
		this.gameEntity.RequestState(this.gameEntity.id, 1L);
		this.PlayAttachFx();
		if (this.retractCoroutine != null)
		{
			base.StopCoroutine(this.retractCoroutine);
		}
		this.retractCoroutine = base.StartCoroutine(this.RetractCoroutine());
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x000E5F3D File Offset: 0x000E413D
	private IEnumerator RetractCoroutine()
	{
		base.transform.localRotation = Quaternion.identity;
		Vector3 vector = base.transform.localPosition;
		for (float sqrMagnitude = vector.sqrMagnitude; sqrMagnitude > 1E-05f; sqrMagnitude = vector.sqrMagnitude)
		{
			vector = Vector3.MoveTowards(vector, Vector3.zero, this.retractSpeed * Time.deltaTime);
			base.transform.localPosition = vector;
			yield return null;
			vector = base.transform.localPosition;
		}
		base.transform.localPosition = Vector3.zero;
		yield break;
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x000E5F4C File Offset: 0x000E414C
	private void PlayAttachFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.badgeAttachSoundVolume;
			this.audioSource.clip = this.badgeAttachSound;
			this.audioSource.Play();
		}
	}

	// Token: 0x0400370F RID: 14095
	private const float RESTORE_BADGE_TO_DOCK_WINDOW = 60f;

	// Token: 0x04003710 RID: 14096
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x04003711 RID: 14097
	[SerializeField]
	public TMP_Text playerName;

	// Token: 0x04003712 RID: 14098
	[SerializeField]
	public TMP_Text playerTitle;

	// Token: 0x04003713 RID: 14099
	[SerializeField]
	public TMP_Text playerLevel;

	// Token: 0x04003714 RID: 14100
	[SerializeField]
	private MeshRenderer badgeMesh;

	// Token: 0x04003715 RID: 14101
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003716 RID: 14102
	[SerializeField]
	private float retractSpeed = 4f;

	// Token: 0x04003717 RID: 14103
	[SerializeField]
	private AudioClip badgeAttachSound;

	// Token: 0x04003718 RID: 14104
	[SerializeField]
	private float badgeAttachSoundVolume;

	// Token: 0x04003719 RID: 14105
	[SerializeField]
	public int dispenserIndex;

	// Token: 0x0400371A RID: 14106
	public int actorNr;

	// Token: 0x0400371B RID: 14107
	private Coroutine retractCoroutine;

	// Token: 0x0400371C RID: 14108
	private int lastRedeemedPoints = -1;

	// Token: 0x02000685 RID: 1669
	public enum BadgeState
	{
		// Token: 0x0400371E RID: 14110
		AtDispenser,
		// Token: 0x0400371F RID: 14111
		WithPlayer
	}
}
