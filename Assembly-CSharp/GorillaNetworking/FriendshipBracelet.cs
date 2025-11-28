using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EDC RID: 3804
	public class FriendshipBracelet : MonoBehaviour
	{
		// Token: 0x06005F18 RID: 24344 RVA: 0x001E9222 File Offset: 0x001E7422
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
		}

		// Token: 0x06005F19 RID: 24345 RVA: 0x001E9230 File Offset: 0x001E7430
		private AudioSource GetAudioSource()
		{
			if (!this.isLeftHand)
			{
				return this.ownerRig.rightHandPlayer;
			}
			return this.ownerRig.leftHandPlayer;
		}

		// Token: 0x06005F1A RID: 24346 RVA: 0x001E9251 File Offset: 0x001E7451
		private void OnEnable()
		{
			this.PlayAppearEffects();
		}

		// Token: 0x06005F1B RID: 24347 RVA: 0x001E9259 File Offset: 0x001E7459
		public void PlayAppearEffects()
		{
			this.GetAudioSource().GTPlayOneShot(this.braceletFormedSound, 1f);
			if (this.braceletFormedParticle)
			{
				this.braceletFormedParticle.Play();
			}
		}

		// Token: 0x06005F1C RID: 24348 RVA: 0x001E928C File Offset: 0x001E748C
		private void OnDisable()
		{
			if (!this.ownerRig.gameObject.activeInHierarchy)
			{
				return;
			}
			this.GetAudioSource().GTPlayOneShot(this.braceletBrokenSound, 1f);
			if (this.braceletBrokenParticle)
			{
				this.braceletBrokenParticle.Play();
			}
		}

		// Token: 0x06005F1D RID: 24349 RVA: 0x001E92DC File Offset: 0x001E74DC
		public void UpdateBeads(List<Color> colors, int selfIndex)
		{
			int num = colors.Count - 1;
			int num2 = (this.braceletBeads.Length - num) / 2;
			for (int i = 0; i < this.braceletBeads.Length; i++)
			{
				int num3 = i - num2;
				if (num3 >= 0 && num3 < num)
				{
					this.braceletBeads[i].enabled = true;
					this.braceletBeads[i].material.color = colors[num3];
					this.braceletBananas[i].gameObject.SetActive(num3 == selfIndex);
				}
				else
				{
					this.braceletBeads[i].enabled = false;
					this.braceletBananas[i].gameObject.SetActive(false);
				}
			}
			SkinnedMeshRenderer[] array = this.braceletStrings;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].material.color = colors[colors.Count - 1];
			}
		}

		// Token: 0x04006CF9 RID: 27897
		[SerializeField]
		private SkinnedMeshRenderer[] braceletStrings;

		// Token: 0x04006CFA RID: 27898
		[SerializeField]
		private MeshRenderer[] braceletBeads;

		// Token: 0x04006CFB RID: 27899
		[SerializeField]
		private MeshRenderer[] braceletBananas;

		// Token: 0x04006CFC RID: 27900
		[SerializeField]
		private bool isLeftHand;

		// Token: 0x04006CFD RID: 27901
		[SerializeField]
		private AudioClip braceletFormedSound;

		// Token: 0x04006CFE RID: 27902
		[SerializeField]
		private AudioClip braceletBrokenSound;

		// Token: 0x04006CFF RID: 27903
		[SerializeField]
		private ParticleSystem braceletFormedParticle;

		// Token: 0x04006D00 RID: 27904
		[SerializeField]
		private ParticleSystem braceletBrokenParticle;

		// Token: 0x04006D01 RID: 27905
		private VRRig ownerRig;
	}
}
