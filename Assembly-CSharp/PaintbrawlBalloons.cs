using System;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200041A RID: 1050
public class PaintbrawlBalloons : MonoBehaviour
{
	// Token: 0x060019DD RID: 6621 RVA: 0x00089F0C File Offset: 0x0008810C
	protected void Awake()
	{
		this.matPropBlock = new MaterialPropertyBlock();
		this.renderers = new Renderer[this.balloons.Length];
		this.balloonsCachedActiveState = new bool[this.balloons.Length];
		for (int i = 0; i < this.balloons.Length; i++)
		{
			this.renderers[i] = this.balloons[i].GetComponentInChildren<Renderer>();
			this.balloonsCachedActiveState[i] = this.balloons[i].activeSelf;
		}
		this.colorShaderPropID = ShaderProps._Color;
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x00089F92 File Offset: 0x00088192
	protected void OnEnable()
	{
		this.UpdateBalloonColors();
	}

	// Token: 0x060019DF RID: 6623 RVA: 0x00089F9C File Offset: 0x0008819C
	protected void LateUpdate()
	{
		if (GorillaGameManager.instance != null && (this.bMgr != null || GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>() != null))
		{
			if (this.bMgr == null)
			{
				this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			}
			int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
			for (int i = 0; i < this.balloons.Length; i++)
			{
				bool flag = playerLives >= i + 1;
				if (flag != this.balloonsCachedActiveState[i])
				{
					this.balloonsCachedActiveState[i] = flag;
					this.balloons[i].SetActive(flag);
					if (!flag)
					{
						this.PopBalloon(i);
					}
				}
			}
		}
		else if (GorillaGameManager.instance != null)
		{
			base.gameObject.SetActive(false);
		}
		this.UpdateBalloonColors();
	}

	// Token: 0x060019E0 RID: 6624 RVA: 0x0008A088 File Offset: 0x00088288
	private void PopBalloon(int i)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab, true);
		gameObject.transform.position = this.balloons[i].transform.position;
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.teamColor);
		}
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x0008A0E0 File Offset: 0x000882E0
	public void UpdateBalloonColors()
	{
		if (this.bMgr != null && this.myRig.creator != null)
		{
			if (this.bMgr.OnRedTeam(this.myRig.creator))
			{
				this.teamColor = this.orangeColor;
			}
			else if (this.bMgr.OnBlueTeam(this.myRig.creator))
			{
				this.teamColor = this.blueColor;
			}
			else
			{
				this.teamColor = (this.myRig ? this.myRig.playerColor : this.defaultColor);
			}
		}
		if (this.teamColor != this.lastColor)
		{
			this.lastColor = this.teamColor;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer)
				{
					foreach (Material material in renderer.materials)
					{
						if (!(material == null))
						{
							if (material.HasProperty(ShaderProps._BaseColor))
							{
								material.SetColor(ShaderProps._BaseColor, this.teamColor);
							}
							if (material.HasProperty(ShaderProps._Color))
							{
								material.SetColor(ShaderProps._Color, this.teamColor);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x04002348 RID: 9032
	public VRRig myRig;

	// Token: 0x04002349 RID: 9033
	public GameObject[] balloons;

	// Token: 0x0400234A RID: 9034
	public Color orangeColor;

	// Token: 0x0400234B RID: 9035
	public Color blueColor;

	// Token: 0x0400234C RID: 9036
	public Color defaultColor;

	// Token: 0x0400234D RID: 9037
	public Color lastColor;

	// Token: 0x0400234E RID: 9038
	public GameObject balloonPopFXPrefab;

	// Token: 0x0400234F RID: 9039
	[HideInInspector]
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x04002350 RID: 9040
	public Player myPlayer;

	// Token: 0x04002351 RID: 9041
	private int colorShaderPropID;

	// Token: 0x04002352 RID: 9042
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04002353 RID: 9043
	private bool[] balloonsCachedActiveState;

	// Token: 0x04002354 RID: 9044
	private Renderer[] renderers;

	// Token: 0x04002355 RID: 9045
	private Color teamColor;
}
