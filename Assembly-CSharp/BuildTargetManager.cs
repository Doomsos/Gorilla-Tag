using System;
using UnityEngine;

// Token: 0x02000C0E RID: 3086
public class BuildTargetManager : MonoBehaviour
{
	// Token: 0x06004C2C RID: 19500 RVA: 0x0018CD7B File Offset: 0x0018AF7B
	public string GetPath()
	{
		return this.path;
	}

	// Token: 0x04005C05 RID: 23557
	public BuildTargetManager.BuildTowards newBuildTarget;

	// Token: 0x04005C06 RID: 23558
	public bool isBeta;

	// Token: 0x04005C07 RID: 23559
	public bool isQA;

	// Token: 0x04005C08 RID: 23560
	public bool spoofIDs;

	// Token: 0x04005C09 RID: 23561
	public bool spoofChild;

	// Token: 0x04005C0A RID: 23562
	public bool enableAllCosmetics;

	// Token: 0x04005C0B RID: 23563
	public OVRManager ovrManager;

	// Token: 0x04005C0C RID: 23564
	private string path = "Assets/csc.rsp";

	// Token: 0x04005C0D RID: 23565
	public BuildTargetManager.BuildTowards currentBuildTargetDONOTCHANGE;

	// Token: 0x04005C0E RID: 23566
	public GorillaTagger gorillaTagger;

	// Token: 0x04005C0F RID: 23567
	public GameObject[] betaDisableObjects;

	// Token: 0x04005C10 RID: 23568
	public GameObject[] betaEnableObjects;

	// Token: 0x04005C11 RID: 23569
	public BuildTargetManager.NetworkBackend networkBackend;

	// Token: 0x02000C0F RID: 3087
	public enum BuildTowards
	{
		// Token: 0x04005C13 RID: 23571
		Steam,
		// Token: 0x04005C14 RID: 23572
		OculusPC,
		// Token: 0x04005C15 RID: 23573
		Quest,
		// Token: 0x04005C16 RID: 23574
		Viveport
	}

	// Token: 0x02000C10 RID: 3088
	public enum NetworkBackend
	{
		// Token: 0x04005C18 RID: 23576
		Pun,
		// Token: 0x04005C19 RID: 23577
		Fusion
	}
}
