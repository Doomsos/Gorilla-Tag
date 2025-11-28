using System;
using UnityEngine;

// Token: 0x0200024B RID: 587
public class DJDeckEqualizer : MonoBehaviour
{
	// Token: 0x06000F55 RID: 3925 RVA: 0x00051649 File Offset: 0x0004F849
	private void Start()
	{
		this.inputColorHash = this.inputColorProperty;
		this.material = this.display.material;
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x00051670 File Offset: 0x0004F870
	private void Update()
	{
		Color color = default(Color);
		color.r = 0.25f;
		color.g = 0.25f;
		color.b = 0.5f;
		for (int i = 0; i < this.redTracks.Length; i++)
		{
			AudioSource audioSource = this.redTracks[i];
			if (audioSource.isPlaying)
			{
				color.r = Mathf.Lerp(0.25f, 1f, this.redTrackCurves[i].Evaluate(audioSource.time));
				break;
			}
		}
		for (int j = 0; j < this.greenTracks.Length; j++)
		{
			AudioSource audioSource2 = this.greenTracks[j];
			if (audioSource2.isPlaying)
			{
				color.g = Mathf.Lerp(0.25f, 1f, this.greenTrackCurves[j].Evaluate(audioSource2.time));
				break;
			}
		}
		this.material.SetColor(this.inputColorHash, color);
	}

	// Token: 0x040012DC RID: 4828
	[SerializeField]
	private MeshRenderer display;

	// Token: 0x040012DD RID: 4829
	[SerializeField]
	private AnimationCurve[] redTrackCurves;

	// Token: 0x040012DE RID: 4830
	[SerializeField]
	private AnimationCurve[] greenTrackCurves;

	// Token: 0x040012DF RID: 4831
	[SerializeField]
	private AudioSource[] redTracks;

	// Token: 0x040012E0 RID: 4832
	[SerializeField]
	private AudioSource[] greenTracks;

	// Token: 0x040012E1 RID: 4833
	private Material material;

	// Token: 0x040012E2 RID: 4834
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x040012E3 RID: 4835
	private ShaderHashId inputColorHash;
}
