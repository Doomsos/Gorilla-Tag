using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020007B2 RID: 1970
public class GorillaSlicerSimpleManager : MonoBehaviour
{
	// Token: 0x060033B7 RID: 13239 RVA: 0x00116811 File Offset: 0x00114A11
	protected void Awake()
	{
		if (GorillaSlicerSimpleManager.hasInstance && GorillaSlicerSimpleManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaSlicerSimpleManager.SetInstance(this);
	}

	// Token: 0x060033B8 RID: 13240 RVA: 0x00116834 File Offset: 0x00114A34
	public static void CreateManager()
	{
		GorillaSlicerSimpleManager gorillaSlicerSimpleManager = new GameObject("GorillaSlicerSimpleManager").AddComponent<GorillaSlicerSimpleManager>();
		gorillaSlicerSimpleManager.fixedUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.updateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.lateUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.sW = new Stopwatch();
		GorillaSlicerSimpleManager.SetInstance(gorillaSlicerSimpleManager);
	}

	// Token: 0x060033B9 RID: 13241 RVA: 0x00116881 File Offset: 0x00114A81
	private static void SetInstance(GorillaSlicerSimpleManager manager)
	{
		GorillaSlicerSimpleManager.instance = manager;
		GorillaSlicerSimpleManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x0011689C File Offset: 0x00114A9C
	public static void RegisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (!GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (!GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (!GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Add(gSS);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060033BB RID: 13243 RVA: 0x00116930 File Offset: 0x00114B30
	public static void UnregisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Remove(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Remove(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Remove(gSS);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060033BC RID: 13244 RVA: 0x001169C8 File Offset: 0x00114BC8
	public void FixedUpdate()
	{
		if (this.updateIndex < 0 || this.updateIndex >= this.fixedUpdateSlice.Count + this.updateSlice.Count + this.lateUpdateSlice.Count)
		{
			this.updateIndex = 0;
		}
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && this.updateIndex < this.fixedUpdateSlice.Count)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.fixedUpdateSlice[this.updateIndex];
			if (0 <= this.updateIndex && this.updateIndex < this.fixedUpdateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					gorillaSliceableSimple.SliceUpdate();
				}
			}
			this.updateIndex++;
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x060033BD RID: 13245 RVA: 0x00116AC4 File Offset: 0x00114CC4
	public void Update()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int num = count + count2;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && count <= this.updateIndex && this.updateIndex < num)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.updateSlice[this.updateIndex - count];
			if (0 <= this.updateIndex - count && this.updateIndex - count < this.updateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					gorillaSliceableSimple.SliceUpdate();
				}
			}
			this.updateIndex++;
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x060033BE RID: 13246 RVA: 0x00116BAC File Offset: 0x00114DAC
	public void LateUpdate()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int count3 = this.lateUpdateSlice.Count;
		int num = count + count2;
		int num2 = num + count3;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && num <= this.updateIndex && this.updateIndex < num2)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.lateUpdateSlice[this.updateIndex - num];
			if (0 <= this.updateIndex - num && this.updateIndex - num < this.lateUpdateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					gorillaSliceableSimple.SliceUpdate();
				}
			}
			this.updateIndex++;
		}
		this.sW.Stop();
		if (this.updateIndex >= num2)
		{
			this.updateIndex = -1;
		}
		this.ticksThisFrame = 0L;
	}

	// Token: 0x04004227 RID: 16935
	public static GorillaSlicerSimpleManager instance;

	// Token: 0x04004228 RID: 16936
	public static bool hasInstance;

	// Token: 0x04004229 RID: 16937
	public List<IGorillaSliceableSimple> fixedUpdateSlice;

	// Token: 0x0400422A RID: 16938
	public List<IGorillaSliceableSimple> updateSlice;

	// Token: 0x0400422B RID: 16939
	public List<IGorillaSliceableSimple> lateUpdateSlice;

	// Token: 0x0400422C RID: 16940
	public long ticksPerFrame = 1000L;

	// Token: 0x0400422D RID: 16941
	public long ticksThisFrame;

	// Token: 0x0400422E RID: 16942
	public int updateIndex = -1;

	// Token: 0x0400422F RID: 16943
	public Stopwatch sW;

	// Token: 0x020007B3 RID: 1971
	public enum UpdateStep
	{
		// Token: 0x04004231 RID: 16945
		FixedUpdate,
		// Token: 0x04004232 RID: 16946
		Update,
		// Token: 0x04004233 RID: 16947
		LateUpdate
	}
}
