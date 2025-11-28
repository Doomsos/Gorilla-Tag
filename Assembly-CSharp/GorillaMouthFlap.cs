using System;
using UnityEngine;

// Token: 0x020007A0 RID: 1952
public class GorillaMouthFlap : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060032FD RID: 13053 RVA: 0x001135D8 File Offset: 0x001117D8
	private void Start()
	{
		this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
		this.targetFaceRenderer = this.targetFace.GetComponent<Renderer>();
		this.facePropBlock = new MaterialPropertyBlock();
		this.hasDefaultMouthAtlas = false;
		if (this.targetFaceRenderer != null)
		{
			this.SetDefaultMouthAtlas(this.targetFaceRenderer.material);
		}
	}

	// Token: 0x060032FE RID: 13054 RVA: 0x00113633 File Offset: 0x00111833
	public void EnableLeafBlower()
	{
		this.leafBlowerActiveUntilTimestamp = Time.time + 0.1f;
	}

	// Token: 0x060032FF RID: 13055 RVA: 0x00113646 File Offset: 0x00111846
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTimeUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x06003300 RID: 13056 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003301 RID: 13057 RVA: 0x00113668 File Offset: 0x00111868
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.lastTimeUpdated;
		this.lastTimeUpdated = Time.time;
		if (this.speaker == null)
		{
			this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
			return;
		}
		float currentLoudness = 0f;
		if (this.speaker.IsSpeaking)
		{
			currentLoudness = this.speaker.Loudness;
		}
		this.CheckMouthflapChange(this.speaker.IsMicEnabled, currentLoudness);
		MouthFlapLevel mouthFlap = this.noMicFace;
		if (this.leafBlowerActiveUntilTimestamp > Time.time)
		{
			mouthFlap = this.leafBlowerFace;
		}
		else if (this.useMicEnabled)
		{
			mouthFlap = this.mouthFlapLevels[this.activeFlipbookIndex];
		}
		this.UpdateMouthFlapFlipbook(mouthFlap);
	}

	// Token: 0x06003302 RID: 13058 RVA: 0x0011371C File Offset: 0x0011191C
	private void CheckMouthflapChange(bool isMicEnabled, float currentLoudness)
	{
		if (isMicEnabled)
		{
			this.useMicEnabled = true;
			int i = this.mouthFlapLevels.Length - 1;
			while (i >= 0)
			{
				if (currentLoudness >= this.mouthFlapLevels[i].maxRequiredVolume)
				{
					return;
				}
				if (currentLoudness > this.mouthFlapLevels[i].minRequiredVolume)
				{
					if (this.activeFlipbookIndex != i)
					{
						this.activeFlipbookIndex = i;
						this.activeFlipbookPlayTime = 0f;
						return;
					}
					return;
				}
				else
				{
					i--;
				}
			}
			return;
		}
		if (this.useMicEnabled)
		{
			this.useMicEnabled = false;
			this.activeFlipbookPlayTime = 0f;
		}
	}

	// Token: 0x06003303 RID: 13059 RVA: 0x001137A8 File Offset: 0x001119A8
	private void UpdateMouthFlapFlipbook(MouthFlapLevel mouthFlap)
	{
		Material material = this.targetFaceRenderer.material;
		this.activeFlipbookPlayTime += this.deltaTime;
		this.activeFlipbookPlayTime %= mouthFlap.cycleDuration;
		int num = Mathf.FloorToInt(this.activeFlipbookPlayTime * (float)mouthFlap.faces.Length / mouthFlap.cycleDuration);
		material.SetTextureOffset(this._MouthMap, mouthFlap.faces[num]);
	}

	// Token: 0x06003304 RID: 13060 RVA: 0x00113820 File Offset: 0x00111A20
	public void SetMouthTextureReplacement(Texture2D replacementMouthAtlas)
	{
		Material material = this.targetFaceRenderer.material;
		this.SetDefaultMouthAtlas(material);
		material.SetTexture(this._MouthMap, replacementMouthAtlas);
	}

	// Token: 0x06003305 RID: 13061 RVA: 0x00113852 File Offset: 0x00111A52
	public void ClearMouthTextureReplacement()
	{
		this.targetFaceRenderer.material.SetTexture(this._MouthMap, this.defaultMouthAtlas);
	}

	// Token: 0x06003306 RID: 13062 RVA: 0x00113878 File Offset: 0x00111A78
	public Material SetFaceMaterialReplacement(Material replacementFaceMaterial)
	{
		if (!this.hasDefaultFaceMaterial)
		{
			this.defaultFaceMaterial = this.targetFaceRenderer.material;
			this.hasDefaultFaceMaterial = true;
		}
		this.targetFaceRenderer.material = replacementFaceMaterial;
		if (this.hasDefaultMouthAtlas && this.defaultMouthAtlas != null)
		{
			this.targetFaceRenderer.material.SetTexture(this._MouthMap, this.defaultMouthAtlas);
		}
		return this.targetFaceRenderer.material;
	}

	// Token: 0x06003307 RID: 13063 RVA: 0x001138F3 File Offset: 0x00111AF3
	public void ClearFaceMaterialReplacement()
	{
		if (this.hasDefaultFaceMaterial)
		{
			this.targetFaceRenderer.material = this.defaultFaceMaterial;
		}
	}

	// Token: 0x06003308 RID: 13064 RVA: 0x0011390E File Offset: 0x00111B0E
	private void SetDefaultMouthAtlas(Material face)
	{
		if (!this.hasDefaultMouthAtlas)
		{
			this.defaultMouthAtlas = face.GetTexture(this._MouthMap);
			this.hasDefaultMouthAtlas = true;
		}
	}

	// Token: 0x04004172 RID: 16754
	public GameObject targetFace;

	// Token: 0x04004173 RID: 16755
	public MouthFlapLevel[] mouthFlapLevels;

	// Token: 0x04004174 RID: 16756
	public MouthFlapLevel noMicFace;

	// Token: 0x04004175 RID: 16757
	public MouthFlapLevel leafBlowerFace;

	// Token: 0x04004176 RID: 16758
	private bool useMicEnabled;

	// Token: 0x04004177 RID: 16759
	private float leafBlowerActiveUntilTimestamp;

	// Token: 0x04004178 RID: 16760
	private int activeFlipbookIndex;

	// Token: 0x04004179 RID: 16761
	private float activeFlipbookPlayTime;

	// Token: 0x0400417A RID: 16762
	private GorillaSpeakerLoudness speaker;

	// Token: 0x0400417B RID: 16763
	private float lastTimeUpdated;

	// Token: 0x0400417C RID: 16764
	private float deltaTime;

	// Token: 0x0400417D RID: 16765
	private Renderer targetFaceRenderer;

	// Token: 0x0400417E RID: 16766
	private MaterialPropertyBlock facePropBlock;

	// Token: 0x0400417F RID: 16767
	private Texture defaultMouthAtlas;

	// Token: 0x04004180 RID: 16768
	private Material defaultFaceMaterial;

	// Token: 0x04004181 RID: 16769
	private bool hasDefaultMouthAtlas;

	// Token: 0x04004182 RID: 16770
	private bool hasDefaultFaceMaterial;

	// Token: 0x04004183 RID: 16771
	private ShaderHashId _MouthMap = "_MouthMap";

	// Token: 0x04004184 RID: 16772
	private ShaderHashId _BaseMap = "_BaseMap";
}
