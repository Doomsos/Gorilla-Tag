using System;
using UnityEngine;

// Token: 0x02000491 RID: 1169
public class PlantableObject : TransferrableObject
{
	// Token: 0x06001DF5 RID: 7669 RVA: 0x0009DC7F File Offset: 0x0009BE7F
	protected override void Awake()
	{
		base.Awake();
		this.materialPropertyBlock = new MaterialPropertyBlock();
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x0009DC94 File Offset: 0x0009BE94
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
		this.dippedColors = new PlantableObject.AppliedColors[20];
	}

	// Token: 0x06001DF7 RID: 7671 RVA: 0x0009DCF4 File Offset: 0x0009BEF4
	private void AssureShaderStuff()
	{
		if (!this.flagRenderer)
		{
			return;
		}
		if (this.materialPropertyBlock == null)
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
		}
		try
		{
			this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
			this.materialPropertyBlock.SetColor(ShaderProps._ColorG, this._colorG);
		}
		catch
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
			this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
			this.materialPropertyBlock.SetColor(ShaderProps._ColorG, this._colorG);
		}
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x0009DDC4 File Offset: 0x0009BFC4
	// (set) Token: 0x06001DF9 RID: 7673 RVA: 0x0009DDCC File Offset: 0x0009BFCC
	public Color colorR
	{
		get
		{
			return this._colorR;
		}
		set
		{
			this._colorR = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06001DFA RID: 7674 RVA: 0x0009DDDB File Offset: 0x0009BFDB
	// (set) Token: 0x06001DFB RID: 7675 RVA: 0x0009DDE3 File Offset: 0x0009BFE3
	public Color colorG
	{
		get
		{
			return this._colorG;
		}
		set
		{
			this._colorG = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001DFC RID: 7676 RVA: 0x0009DDF2 File Offset: 0x0009BFF2
	// (set) Token: 0x06001DFD RID: 7677 RVA: 0x0009DDFA File Offset: 0x0009BFFA
	public bool planted { get; private set; }

	// Token: 0x06001DFE RID: 7678 RVA: 0x0009DE04 File Offset: 0x0009C004
	public void SetPlanted(bool newPlanted)
	{
		if (this.planted != newPlanted)
		{
			if (newPlanted)
			{
				if (!this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = true;
				}
				this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
			}
			else
			{
				this.respawnAtTimestamp = 0f;
			}
			this.planted = newPlanted;
		}
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x0009DE5C File Offset: 0x0009C05C
	private void AddRed()
	{
		this.AddColor(PlantableObject.AppliedColors.Red);
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x0009DE65 File Offset: 0x0009C065
	private void AddGreen()
	{
		this.AddColor(PlantableObject.AppliedColors.Blue);
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x0009DE6E File Offset: 0x0009C06E
	private void AddBlue()
	{
		this.AddColor(PlantableObject.AppliedColors.Green);
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x0009DE77 File Offset: 0x0009C077
	private void AddBlack()
	{
		this.AddColor(PlantableObject.AppliedColors.Black);
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x0009DE80 File Offset: 0x0009C080
	public void AddColor(PlantableObject.AppliedColors color)
	{
		this.dippedColors[this.currentDipIndex] = color;
		this.currentDipIndex++;
		if (this.currentDipIndex >= this.dippedColors.Length)
		{
			this.currentDipIndex = 0;
		}
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x0009DEBC File Offset: 0x0009C0BC
	public void ClearColors()
	{
		for (int i = 0; i < this.dippedColors.Length; i++)
		{
			this.dippedColors[i] = PlantableObject.AppliedColors.None;
		}
		this.currentDipIndex = 0;
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x0009DEF4 File Offset: 0x0009C0F4
	public Color CalculateOutputColor()
	{
		Color color = Color.black;
		int num = 0;
		int num2 = 0;
		foreach (PlantableObject.AppliedColors appliedColors in this.dippedColors)
		{
			if (appliedColors == PlantableObject.AppliedColors.None)
			{
				break;
			}
			switch (appliedColors)
			{
			case PlantableObject.AppliedColors.Red:
				color += Color.red;
				num2++;
				break;
			case PlantableObject.AppliedColors.Green:
				color += Color.green;
				num2++;
				break;
			case PlantableObject.AppliedColors.Blue:
				color += Color.blue;
				num2++;
				break;
			case PlantableObject.AppliedColors.Black:
				num++;
				num2++;
				break;
			}
		}
		if (color == Color.black && num == 0)
		{
			return Color.white;
		}
		float num3 = Mathf.Max(new float[]
		{
			color.r,
			color.g,
			color.b
		});
		if (num3 == 0f)
		{
			return Color.black;
		}
		color /= num3;
		float num4 = (float)num / (float)num2;
		if (num4 > 0f)
		{
			color *= 1f - num4;
		}
		return color;
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x0009DFFD File Offset: 0x0009C1FD
	public void UpdateDisplayedDippedColor()
	{
		this.colorR = this.CalculateOutputColor();
	}

	// Token: 0x06001E07 RID: 7687 RVA: 0x0009E00B File Offset: 0x0009C20B
	public override void DropItem()
	{
		base.DropItem();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x0009E038 File Offset: 0x0009C238
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		this.itemState = (this.planted ? TransferrableObject.ItemStates.State1 : TransferrableObject.ItemStates.State0);
		if (this.respawnAtTimestamp != 0f && Time.time > this.respawnAtTimestamp)
		{
			this.respawnAtTimestamp = 0f;
			this.ResetToHome();
		}
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x0009E088 File Offset: 0x0009C288
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06001E0A RID: 7690 RVA: 0x0009E0B2 File Offset: 0x0009C2B2
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	// Token: 0x06001E0B RID: 7691 RVA: 0x0009E0BC File Offset: 0x0009C2BC
	public override bool ShouldBeKinematic()
	{
		return base.ShouldBeKinematic() || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x0009E0D4 File Offset: 0x0009C2D4
	public override void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		base.OnOwnershipTransferred(toPlayer, fromPlayer);
		if (toPlayer == null)
		{
			return;
		}
		if (toPlayer.IsLocal && this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
		}
		Action<Color> <>9__1;
		GorillaGameManager.OnInstanceReady(delegate
		{
			VRRig vrrig = GorillaGameManager.instance.FindPlayerVRRig(toPlayer);
			if (vrrig == null)
			{
				return;
			}
			VRRig vrrig2 = vrrig;
			Action<Color> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(Color color1)
				{
					this.colorG = color1;
				});
			}
			vrrig2.OnColorInitialized(action);
		});
	}

	// Token: 0x04002810 RID: 10256
	public PlantablePoint point;

	// Token: 0x04002811 RID: 10257
	public float respawnAfterDuration;

	// Token: 0x04002812 RID: 10258
	private float respawnAtTimestamp;

	// Token: 0x04002813 RID: 10259
	public SkinnedMeshRenderer flagRenderer;

	// Token: 0x04002814 RID: 10260
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x04002815 RID: 10261
	[HideInInspector]
	[SerializeReference]
	private Color _colorR;

	// Token: 0x04002816 RID: 10262
	[HideInInspector]
	[SerializeReference]
	private Color _colorG;

	// Token: 0x04002818 RID: 10264
	public Transform flagTip;

	// Token: 0x04002819 RID: 10265
	public PlantableObject.AppliedColors[] dippedColors = new PlantableObject.AppliedColors[20];

	// Token: 0x0400281A RID: 10266
	public int currentDipIndex;

	// Token: 0x02000492 RID: 1170
	public enum AppliedColors
	{
		// Token: 0x0400281C RID: 10268
		None,
		// Token: 0x0400281D RID: 10269
		Red,
		// Token: 0x0400281E RID: 10270
		Green,
		// Token: 0x0400281F RID: 10271
		Blue,
		// Token: 0x04002820 RID: 10272
		Black
	}
}
