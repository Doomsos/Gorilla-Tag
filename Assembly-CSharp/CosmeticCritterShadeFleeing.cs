using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class CosmeticCritterShadeFleeing : CosmeticCritter
{
	// Token: 0x06000471 RID: 1137 RVA: 0x000197FB File Offset: 0x000179FB
	public override void OnSpawn()
	{
		this.spawnFX.Play();
		this.spawnAudioSource.clip = this.spawnAudioClips.GetRandomItem<AudioClip>();
		this.spawnAudioSource.GTPlay();
		this.pullVector = Vector3.zero;
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00019834 File Offset: 0x00017A34
	public void SetFleePosition(Vector3 position, Vector3 fleeFrom)
	{
		this.origin = position;
		Vector3 vector = position - fleeFrom;
		this.fleeForward = vector.normalized;
		this.fleeRight = Vector3.Cross(this.fleeForward, Vector3.up);
		this.fleeUp = Vector3.Cross(this.fleeForward, this.fleeRight);
		this.trailingPosition = position + vector.normalized * 3f;
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x000198A8 File Offset: 0x00017AA8
	public override void SetRandomVariables()
	{
		float num = 0f;
		for (int i = 0; i < this.modelSwaps.Length; i++)
		{
			num += this.modelSwaps[i].relativeProbability;
			this.modelSwaps[i].gameObject.SetActive(false);
		}
		float num2 = Random.value * num;
		for (int j = 0; j < this.modelSwaps.Length; j++)
		{
			if (num2 < this.modelSwaps[j].relativeProbability)
			{
				this.modelSwaps[j].gameObject.SetActive(true);
				break;
			}
			num2 -= this.modelSwaps[j].relativeProbability;
		}
		this.fleeBobFrequencyXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.x, Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.y);
		this.fleeBobMagnitudeXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.x, Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.y);
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x000199C8 File Offset: 0x00017BC8
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		Vector3 vector = this.origin + num * this.fleeForward + this.pullVector + Mathf.Sin(this.fleeBobFrequencyXY.x * num) * this.fleeBobMagnitudeXY.x * this.fleeRight + Mathf.Sin(this.fleeBobFrequencyXY.y * num) * this.fleeBobMagnitudeXY.y * this.fleeUp;
		Quaternion quaternion = Quaternion.LookRotation((vector - this.trailingPosition).normalized, Vector3.up);
		this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector, 0.05f);
		base.transform.SetPositionAndRotation(vector, quaternion);
		this.animator.SetFloat(this.animatorProperty, Mathf.Sin(num * 3f) * 0.5f + 0.5f);
	}

	// Token: 0x04000506 RID: 1286
	[Tooltip("Randomly selects one of these models when spawned, accounting for relative probabilities. For example, if one model has a probability of 1 and another a probability of 2, the second is twice as likely to be picked (and thus will be picked 67% of the time).")]
	[SerializeField]
	private CosmeticCritterShadeFleeing.ModelSwap[] modelSwaps;

	// Token: 0x04000507 RID: 1287
	[Space]
	[Tooltip("Despawn the Shade after it has fled (fleed?) this many meters.")]
	[SerializeField]
	private float fleeDistanceToDespawn = 10f;

	// Token: 0x04000508 RID: 1288
	[Tooltip("Flee away from the spotter at this many meters per second.")]
	[SerializeField]
	private float fleeSpeed;

	// Token: 0x04000509 RID: 1289
	[Tooltip("The maximum strength the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobMagnitudeXYMax;

	// Token: 0x0400050A RID: 1290
	[Tooltip("The maximum frequency the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobFrequencyXYMax;

	// Token: 0x0400050B RID: 1291
	[SerializeField]
	private Animator animator;

	// Token: 0x0400050C RID: 1292
	[SerializeField]
	private ParticleSystem spawnFX;

	// Token: 0x0400050D RID: 1293
	[SerializeField]
	private AudioSource spawnAudioSource;

	// Token: 0x0400050E RID: 1294
	[SerializeField]
	private AudioClip[] spawnAudioClips;

	// Token: 0x0400050F RID: 1295
	[HideInInspector]
	public Vector3 pullVector;

	// Token: 0x04000510 RID: 1296
	private Vector3 origin;

	// Token: 0x04000511 RID: 1297
	private Vector3 fleeForward;

	// Token: 0x04000512 RID: 1298
	private Vector3 fleeRight;

	// Token: 0x04000513 RID: 1299
	private Vector3 fleeUp = Vector3.up;

	// Token: 0x04000514 RID: 1300
	private Vector2 fleeBobFrequencyXY;

	// Token: 0x04000515 RID: 1301
	private Vector2 fleeBobMagnitudeXY;

	// Token: 0x04000516 RID: 1302
	private Vector3 trailingPosition;

	// Token: 0x04000517 RID: 1303
	private float closestCatcherDistance;

	// Token: 0x04000518 RID: 1304
	private int animatorProperty = Animator.StringToHash("Distance");

	// Token: 0x020000B2 RID: 178
	[Serializable]
	private class ModelSwap
	{
		// Token: 0x04000519 RID: 1305
		public float relativeProbability;

		// Token: 0x0400051A RID: 1306
		public GameObject gameObject;
	}
}
