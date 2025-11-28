using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200067F RID: 1663
public class GRArmorEnemy : MonoBehaviour
{
	// Token: 0x06002A94 RID: 10900 RVA: 0x000E57E2 File Offset: 0x000E39E2
	private void Awake()
	{
		this.SetHp(0);
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x06002A95 RID: 10901 RVA: 0x000E57F7 File Offset: 0x000E39F7
	public void SetHp(int hp)
	{
		this.hp = hp;
		this.RefreshArmor();
	}

	// Token: 0x06002A96 RID: 10902 RVA: 0x000E5808 File Offset: 0x000E3A08
	private void RefreshArmor()
	{
		bool flag = this.hp > 0;
		GREnemy.HideRenderers(this.renderers, !flag);
		GREnemy.HideObjects(this.visibleObjects, !flag);
		if (this.armorStateData.Count > 0)
		{
			int num = -1;
			Material mainRendererMaterial = this.armorStateData[0].mainRendererMaterial;
			for (int i = 0; i < this.armorStateData.Count; i++)
			{
				num = i;
				mainRendererMaterial = this.armorStateData[i].mainRendererMaterial;
				if (this.hp >= this.armorStateData[i].healthThreshold)
				{
					break;
				}
			}
			if (flag && this.materialSwapRenderer != null && mainRendererMaterial != this.materialSwapRenderer.material)
			{
				this.materialSwapRenderer.material = mainRendererMaterial;
				this.SetArmorColor(this.GetArmorColor());
			}
			if (num != -1)
			{
				GREnemy.HideObjects(this.armorStateData[num].visibleObjects, !flag);
				for (int j = 0; j < this.armorStateData[num].hiddenObjects.Count; j++)
				{
					GameObject gameObject = this.armorStateData[num].hiddenObjects[j];
					if (gameObject.activeInHierarchy)
					{
						this.PlayDestroyFx(gameObject.transform.position);
					}
				}
				GREnemy.HideObjects(this.armorStateData[num].hiddenObjects, true);
			}
		}
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x000E5973 File Offset: 0x000E3B73
	public void SetArmorColor(Color newColor)
	{
		if (this.renderers != null && this.renderers.Count > 0)
		{
			this.materialSwapRenderer.material.SetColor("_BaseColor", newColor);
		}
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x000E59A4 File Offset: 0x000E3BA4
	public Color GetArmorColor()
	{
		Color result = Color.white;
		if (this.materialSwapRenderer != null)
		{
			result = this.materialSwapRenderer.material.GetColor("_BaseColor");
		}
		return result;
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x000E59DC File Offset: 0x000E3BDC
	public void PlayHitFx(Vector3 position)
	{
		this.PlayFx(this.fxHit, position);
		this.PlaySound(this.hitSound, this.hitSoundVolume, position);
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x000E59FE File Offset: 0x000E3BFE
	public void PlayBlockFx(Vector3 position)
	{
		this.PlayFx(this.fxBlock, position);
		this.PlaySound(this.blockSound, this.blockSoundVolume, position);
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x000E5A20 File Offset: 0x000E3C20
	public void PlayDestroyFx(Vector3 position)
	{
		this.PlayFx(this.fxDestroy, position);
		this.PlaySound(this.destroySound, this.destroySoundVolume, position);
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x000E5A42 File Offset: 0x000E3C42
	private void PlayFx(GameObject fx, Vector3 position)
	{
		if (fx == null)
		{
			return;
		}
		fx.SetActive(false);
		fx.SetActive(true);
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x000E5A5C File Offset: 0x000E3C5C
	private void PlaySound(AudioClip clip, float volume, Vector3 position)
	{
		this.audioSource.clip = clip;
		this.audioSource.volume = volume;
		this.audioSource.Play();
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x000E5A84 File Offset: 0x000E3C84
	public void FragmentArmor()
	{
		if (this.entity.IsAuthority())
		{
			float num = 0f;
			for (int i = 0; i < this.numFragmentsWhenShattered; i++)
			{
				num += 360f / (float)this.numFragmentsWhenShattered;
				Quaternion quaternion = Quaternion.Euler(0f, num, this.fragmentLaunchPitch);
				Vector3 vector = quaternion * this.fragmentSpawnOffset;
				this.entity.manager.RequestCreateItem(this.armorFragmentPrefab.name.GetStaticHash(), base.transform.position + vector, quaternion, (long)this.entity.GetNetId());
			}
		}
	}

	// Token: 0x040036DC RID: 14044
	[SerializeField]
	private List<Renderer> renderers;

	// Token: 0x040036DD RID: 14045
	[SerializeField]
	private List<GameObject> visibleObjects;

	// Token: 0x040036DE RID: 14046
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040036DF RID: 14047
	[SerializeField]
	private GameObject fxHit;

	// Token: 0x040036E0 RID: 14048
	[SerializeField]
	private AudioClip hitSound;

	// Token: 0x040036E1 RID: 14049
	[SerializeField]
	private float hitSoundVolume;

	// Token: 0x040036E2 RID: 14050
	[SerializeField]
	private GameObject fxBlock;

	// Token: 0x040036E3 RID: 14051
	[SerializeField]
	private AudioClip blockSound;

	// Token: 0x040036E4 RID: 14052
	[SerializeField]
	private float blockSoundVolume;

	// Token: 0x040036E5 RID: 14053
	[SerializeField]
	private GameObject fxDestroy;

	// Token: 0x040036E6 RID: 14054
	[SerializeField]
	private AudioClip destroySound;

	// Token: 0x040036E7 RID: 14055
	[SerializeField]
	private float destroySoundVolume;

	// Token: 0x040036E8 RID: 14056
	[SerializeField]
	public List<GRArmorEnemy.GREnemyArmorLevel> armorStateData;

	// Token: 0x040036E9 RID: 14057
	[SerializeField]
	public Renderer materialSwapRenderer;

	// Token: 0x040036EA RID: 14058
	private GameEntity entity;

	// Token: 0x040036EB RID: 14059
	public GameObject armorFragmentPrefab;

	// Token: 0x040036EC RID: 14060
	public Vector3 fragmentSpawnOffset = new Vector3(0f, 0.5f, 0.5f);

	// Token: 0x040036ED RID: 14061
	public int numFragmentsWhenShattered = 3;

	// Token: 0x040036EE RID: 14062
	public float fragmentLaunchPitch = 30f;

	// Token: 0x040036EF RID: 14063
	private int hp;

	// Token: 0x02000680 RID: 1664
	[Serializable]
	public struct GREnemyArmorLevel
	{
		// Token: 0x040036F0 RID: 14064
		public int healthThreshold;

		// Token: 0x040036F1 RID: 14065
		public Material mainRendererMaterial;

		// Token: 0x040036F2 RID: 14066
		public List<GameObject> visibleObjects;

		// Token: 0x040036F3 RID: 14067
		public List<GameObject> hiddenObjects;
	}
}
