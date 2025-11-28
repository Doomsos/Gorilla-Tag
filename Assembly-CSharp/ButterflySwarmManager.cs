using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class ButterflySwarmManager : MonoBehaviour
{
	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000BD3 RID: 3027 RVA: 0x000405B7 File Offset: 0x0003E7B7
	// (set) Token: 0x06000BD4 RID: 3028 RVA: 0x000405BF File Offset: 0x0003E7BF
	public float PerchedFlapSpeed { get; private set; }

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x000405C8 File Offset: 0x0003E7C8
	// (set) Token: 0x06000BD6 RID: 3030 RVA: 0x000405D0 File Offset: 0x0003E7D0
	public float PerchedFlapPhase { get; private set; }

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000BD7 RID: 3031 RVA: 0x000405D9 File Offset: 0x0003E7D9
	// (set) Token: 0x06000BD8 RID: 3032 RVA: 0x000405E1 File Offset: 0x0003E7E1
	public float BeeSpeed { get; private set; }

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x000405EA File Offset: 0x0003E7EA
	// (set) Token: 0x06000BDA RID: 3034 RVA: 0x000405F2 File Offset: 0x0003E7F2
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000BDB RID: 3035 RVA: 0x000405FB File Offset: 0x0003E7FB
	// (set) Token: 0x06000BDC RID: 3036 RVA: 0x00040603 File Offset: 0x0003E803
	public float BeeAcceleration { get; private set; }

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000BDD RID: 3037 RVA: 0x0004060C File Offset: 0x0003E80C
	// (set) Token: 0x06000BDE RID: 3038 RVA: 0x00040614 File Offset: 0x0003E814
	public float BeeJitterStrength { get; private set; }

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000BDF RID: 3039 RVA: 0x0004061D File Offset: 0x0003E81D
	// (set) Token: 0x06000BE0 RID: 3040 RVA: 0x00040625 File Offset: 0x0003E825
	public float BeeJitterDamping { get; private set; }

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x0004062E File Offset: 0x0003E82E
	// (set) Token: 0x06000BE2 RID: 3042 RVA: 0x00040636 File Offset: 0x0003E836
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x0004063F File Offset: 0x0003E83F
	// (set) Token: 0x06000BE4 RID: 3044 RVA: 0x00040647 File Offset: 0x0003E847
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x00040650 File Offset: 0x0003E850
	// (set) Token: 0x06000BE6 RID: 3046 RVA: 0x00040658 File Offset: 0x0003E858
	public float DestRotationAlignmentSpeed { get; private set; }

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x00040661 File Offset: 0x0003E861
	// (set) Token: 0x06000BE8 RID: 3048 RVA: 0x00040669 File Offset: 0x0003E869
	public Vector3 TravellingLocalRotationEuler { get; private set; }

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000BE9 RID: 3049 RVA: 0x00040672 File Offset: 0x0003E872
	// (set) Token: 0x06000BEA RID: 3050 RVA: 0x0004067A File Offset: 0x0003E87A
	public Quaternion TravellingLocalRotation { get; private set; }

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000BEB RID: 3051 RVA: 0x00040683 File Offset: 0x0003E883
	// (set) Token: 0x06000BEC RID: 3052 RVA: 0x0004068B File Offset: 0x0003E88B
	public float AvoidPointRadius { get; private set; }

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000BED RID: 3053 RVA: 0x00040694 File Offset: 0x0003E894
	// (set) Token: 0x06000BEE RID: 3054 RVA: 0x0004069C File Offset: 0x0003E89C
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000BEF RID: 3055 RVA: 0x000406A5 File Offset: 0x0003E8A5
	// (set) Token: 0x06000BF0 RID: 3056 RVA: 0x000406AD File Offset: 0x0003E8AD
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x000406B6 File Offset: 0x0003E8B6
	// (set) Token: 0x06000BF2 RID: 3058 RVA: 0x000406BE File Offset: 0x0003E8BE
	public Color[] BeeColors { get; private set; }

	// Token: 0x06000BF3 RID: 3059 RVA: 0x000406C8 File Offset: 0x0003E8C8
	private void Awake()
	{
		this.TravellingLocalRotation = Quaternion.Euler(this.TravellingLocalRotationEuler);
		this.butterflies = new List<AnimatedButterfly>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedButterfly animatedButterfly = default(AnimatedButterfly);
			animatedButterfly.InitVisual(this.beePrefab, this);
			if (this.BeeColors.Length != 0)
			{
				animatedButterfly.SetColor(this.BeeColors[i % this.BeeColors.Length]);
			}
			this.butterflies.Add(animatedButterfly);
		}
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00040750 File Offset: 0x0003E950
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.perchSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				List<GameObject> list = new List<GameObject>();
				this.allPerchZones.Add(list);
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					list.Add(transform.gameObject);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00040810 File Offset: 0x0003EA10
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00040828 File Offset: 0x0003EA28
	private void Update()
	{
		for (int i = 0; i < this.butterflies.Count; i++)
		{
			AnimatedButterfly animatedButterfly = this.butterflies[i];
			animatedButterfly.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			this.butterflies[i] = animatedButterfly;
		}
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x00040878 File Offset: 0x0003EA78
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<List<GameObject>> list = new List<List<GameObject>>(this.allPerchZones.Count);
		for (int i = 0; i < this.allPerchZones.Count; i++)
		{
			List<GameObject> list2 = new List<GameObject>();
			list2.AddRange(this.allPerchZones[i]);
			list.Add(list2);
		}
		List<GameObject> list3 = new List<GameObject>(this.loopSizePerBee);
		List<float> list4 = new List<float>(this.loopSizePerBee);
		for (int j = 0; j < this.butterflies.Count; j++)
		{
			AnimatedButterfly animatedButterfly = this.butterflies[j];
			animatedButterfly.SetFlapSpeed(srand.NextFloat(this.minFlapSpeed, this.maxFlapSpeed));
			list3.Clear();
			list4.Clear();
			this.PickPoints(this.loopSizePerBee, list, ref srand, list3);
			for (int k = 0; k < list3.Count; k++)
			{
				list4.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			if (list3.Count == 0)
			{
				this.butterflies.Clear();
				return;
			}
			animatedButterfly.InitRoute(list3, list4, this);
			this.butterflies[j] = animatedButterfly;
		}
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x000409BC File Offset: 0x0003EBBC
	private void PickPoints(int n, List<List<GameObject>> pickBuffer, ref SRand rand, List<GameObject> resultBuffer)
	{
		int exclude = rand.NextInt(0, pickBuffer.Count);
		int num = -1;
		int num2 = n - 2;
		while (resultBuffer.Count < n)
		{
			int num3;
			if (resultBuffer.Count < num2)
			{
				num3 = rand.NextIntWithExclusion(0, pickBuffer.Count, num);
			}
			else
			{
				num3 = rand.NextIntWithExclusion2(0, pickBuffer.Count, num, exclude);
			}
			int num4 = 10;
			while (num3 == num || pickBuffer[num3].Count == 0)
			{
				num3 = (num3 + 1) % pickBuffer.Count;
				num4--;
				if (num4 <= 0)
				{
					return;
				}
			}
			num = num3;
			List<GameObject> list = pickBuffer[num];
			while (list.Count == 0)
			{
				num = (num + 1) % pickBuffer.Count;
				list = pickBuffer[num];
			}
			resultBuffer.Add(list[list.Count - 1]);
			list.RemoveAt(list.Count - 1);
		}
	}

	// Token: 0x04000E8F RID: 3727
	[SerializeField]
	private XSceneRef[] perchSections;

	// Token: 0x04000E90 RID: 3728
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x04000E91 RID: 3729
	[SerializeField]
	private int numBees;

	// Token: 0x04000E92 RID: 3730
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x04000E93 RID: 3731
	[SerializeField]
	private float maxFlapSpeed;

	// Token: 0x04000E94 RID: 3732
	[SerializeField]
	private float minFlapSpeed;

	// Token: 0x04000EA5 RID: 3749
	private List<AnimatedButterfly> butterflies;

	// Token: 0x04000EA6 RID: 3750
	private List<List<GameObject>> allPerchZones = new List<List<GameObject>>();
}
