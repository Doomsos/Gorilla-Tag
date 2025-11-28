using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001B9 RID: 441
public class BeeSwarmManager : MonoBehaviour
{
	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000BB1 RID: 2993 RVA: 0x0004010E File Offset: 0x0003E30E
	// (set) Token: 0x06000BB2 RID: 2994 RVA: 0x00040116 File Offset: 0x0003E316
	public BeePerchPoint BeeHive { get; private set; }

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000BB3 RID: 2995 RVA: 0x0004011F File Offset: 0x0003E31F
	// (set) Token: 0x06000BB4 RID: 2996 RVA: 0x00040127 File Offset: 0x0003E327
	public float BeeSpeed { get; private set; }

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000BB5 RID: 2997 RVA: 0x00040130 File Offset: 0x0003E330
	// (set) Token: 0x06000BB6 RID: 2998 RVA: 0x00040138 File Offset: 0x0003E338
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000BB7 RID: 2999 RVA: 0x00040141 File Offset: 0x0003E341
	// (set) Token: 0x06000BB8 RID: 3000 RVA: 0x00040149 File Offset: 0x0003E349
	public float BeeAcceleration { get; private set; }

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x00040152 File Offset: 0x0003E352
	// (set) Token: 0x06000BBA RID: 3002 RVA: 0x0004015A File Offset: 0x0003E35A
	public float BeeJitterStrength { get; private set; }

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000BBB RID: 3003 RVA: 0x00040163 File Offset: 0x0003E363
	// (set) Token: 0x06000BBC RID: 3004 RVA: 0x0004016B File Offset: 0x0003E36B
	public float BeeJitterDamping { get; private set; }

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000BBD RID: 3005 RVA: 0x00040174 File Offset: 0x0003E374
	// (set) Token: 0x06000BBE RID: 3006 RVA: 0x0004017C File Offset: 0x0003E37C
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000BBF RID: 3007 RVA: 0x00040185 File Offset: 0x0003E385
	// (set) Token: 0x06000BC0 RID: 3008 RVA: 0x0004018D File Offset: 0x0003E38D
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000BC1 RID: 3009 RVA: 0x00040196 File Offset: 0x0003E396
	// (set) Token: 0x06000BC2 RID: 3010 RVA: 0x0004019E File Offset: 0x0003E39E
	public float AvoidPointRadius { get; private set; }

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000BC3 RID: 3011 RVA: 0x000401A7 File Offset: 0x0003E3A7
	// (set) Token: 0x06000BC4 RID: 3012 RVA: 0x000401AF File Offset: 0x0003E3AF
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x000401B8 File Offset: 0x0003E3B8
	// (set) Token: 0x06000BC6 RID: 3014 RVA: 0x000401C0 File Offset: 0x0003E3C0
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000BC7 RID: 3015 RVA: 0x000401C9 File Offset: 0x0003E3C9
	// (set) Token: 0x06000BC8 RID: 3016 RVA: 0x000401D1 File Offset: 0x0003E3D1
	public float GeneralBuzzRange { get; private set; }

	// Token: 0x06000BC9 RID: 3017 RVA: 0x000401DC File Offset: 0x0003E3DC
	private void Awake()
	{
		this.bees = new List<AnimatedBee>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedBee animatedBee = default(AnimatedBee);
			animatedBee.InitVisual(this.beePrefab, this);
			this.bees.Add(animatedBee);
		}
		this.playerCamera = Camera.main.transform;
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x00040240 File Offset: 0x0003E440
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.flowerSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				foreach (BeePerchPoint beePerchPoint in gameObject.GetComponentsInChildren<BeePerchPoint>())
				{
					this.allPerchPoints.Add(beePerchPoint);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x000402C0 File Offset: 0x0003E4C0
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x000402D8 File Offset: 0x0003E4D8
	private void Update()
	{
		Vector3 position = this.playerCamera.transform.position;
		Vector3 position2 = Vector3.zero;
		Vector3 vector = Vector3.zero;
		float num = 1f / (float)this.bees.Count;
		float num2 = float.PositiveInfinity;
		float num3 = this.GeneralBuzzRange * this.GeneralBuzzRange;
		int num4 = 0;
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee animatedBee = this.bees[i];
			animatedBee.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			Vector3 position3 = animatedBee.visual.transform.position;
			float sqrMagnitude = (position3 - position).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				position2 = position3;
				num2 = sqrMagnitude;
			}
			if (sqrMagnitude < num3)
			{
				vector += position3;
				num4++;
			}
			this.bees[i] = animatedBee;
		}
		this.nearbyBeeBuzz.transform.position = position2;
		if (num4 > 0)
		{
			this.generalBeeBuzz.transform.position = vector / (float)num4;
			this.generalBeeBuzz.enabled = true;
			return;
		}
		this.generalBeeBuzz.enabled = false;
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x00040408 File Offset: 0x0003E608
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<BeePerchPoint> pickBuffer = new List<BeePerchPoint>(this.allPerchPoints.Count);
		List<BeePerchPoint> list = new List<BeePerchPoint>(this.loopSizePerBee);
		List<float> list2 = new List<float>(this.loopSizePerBee);
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee animatedBee = this.bees[i];
			list = new List<BeePerchPoint>(this.loopSizePerBee);
			list2 = new List<float>(this.loopSizePerBee);
			this.PickPoints(this.loopSizePerBee, pickBuffer, this.allPerchPoints, ref srand, list);
			for (int j = 0; j < list.Count; j++)
			{
				list2.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			animatedBee.InitRoute(list, list2, this);
			animatedBee.InitRouteTimestamps();
			this.bees[i] = animatedBee;
		}
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x000404FC File Offset: 0x0003E6FC
	private void PickPoints(int n, List<BeePerchPoint> pickBuffer, List<BeePerchPoint> allPerchPoints, ref SRand rand, List<BeePerchPoint> resultBuffer)
	{
		resultBuffer.Add(this.BeeHive);
		n--;
		int num = 100;
		while (pickBuffer.Count < n && num-- > 0)
		{
			n -= pickBuffer.Count;
			resultBuffer.AddRange(pickBuffer);
			pickBuffer.Clear();
			pickBuffer.AddRange(allPerchPoints);
			rand.Shuffle<BeePerchPoint>(pickBuffer);
		}
		resultBuffer.AddRange(pickBuffer.GetRange(pickBuffer.Count - n, n));
		pickBuffer.RemoveRange(pickBuffer.Count - n, n);
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x0004057D File Offset: 0x0003E77D
	public static void RegisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Add(obj);
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x0004058A File Offset: 0x0003E78A
	public static void UnregisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Remove(obj);
	}

	// Token: 0x04000E78 RID: 3704
	[SerializeField]
	private XSceneRef[] flowerSections;

	// Token: 0x04000E79 RID: 3705
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x04000E7A RID: 3706
	[SerializeField]
	private int numBees;

	// Token: 0x04000E7B RID: 3707
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x04000E7C RID: 3708
	[SerializeField]
	private AudioSource nearbyBeeBuzz;

	// Token: 0x04000E7D RID: 3709
	[SerializeField]
	private AudioSource generalBeeBuzz;

	// Token: 0x04000E7E RID: 3710
	private GameObject[] flowerSectionsResolved;

	// Token: 0x04000E8B RID: 3723
	private List<AnimatedBee> bees;

	// Token: 0x04000E8C RID: 3724
	private Transform playerCamera;

	// Token: 0x04000E8D RID: 3725
	private List<BeePerchPoint> allPerchPoints = new List<BeePerchPoint>();

	// Token: 0x04000E8E RID: 3726
	public static readonly List<GameObject> avoidPoints = new List<GameObject>();
}
