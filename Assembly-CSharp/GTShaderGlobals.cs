using System;
using UnityEngine;

// Token: 0x020002C1 RID: 705
public class GTShaderGlobals : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x170001AC RID: 428
	// (get) Token: 0x0600114E RID: 4430 RVA: 0x0005C140 File Offset: 0x0005A340
	public static Vector3 WorldSpaceCameraPos
	{
		get
		{
			return GTShaderGlobals.gMainCameraWorldPos;
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x0600114F RID: 4431 RVA: 0x0005C147 File Offset: 0x0005A347
	public static float Time
	{
		get
		{
			return GTShaderGlobals.gTime;
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06001150 RID: 4432 RVA: 0x0005C14E File Offset: 0x0005A34E
	public static int Frame
	{
		get
		{
			return GTShaderGlobals.gIFrame;
		}
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x0005C155 File Offset: 0x0005A355
	private void Awake()
	{
		GTShaderGlobals.gMainCamera = Camera.main;
		if (GTShaderGlobals.gMainCamera)
		{
			GTShaderGlobals.gMainCameraXform = GTShaderGlobals.gMainCamera.transform;
			GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		}
		this.SliceUpdate();
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0005C191 File Offset: 0x0005A391
	[RuntimeInitializeOnLoadMethod(1)]
	private static void Initialize()
	{
		GTShaderGlobals.InitBlueNoiseTex();
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0005C198 File Offset: 0x0005A398
	public void SliceUpdate()
	{
		GTShaderGlobals.UpdateTime();
		GTShaderGlobals.UpdateFrame();
		GTShaderGlobals.UpdateCamera();
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x0005C1A9 File Offset: 0x0005A3A9
	private static void UpdateFrame()
	{
		GTShaderGlobals.gIFrame = UnityEngine.Time.frameCount;
		Shader.SetGlobalInteger(GTShaderGlobals._GT_iFrame, GTShaderGlobals.gIFrame);
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x0005C1C9 File Offset: 0x0005A3C9
	private static void UpdateCamera()
	{
		if (!GTShaderGlobals.gMainCameraXform)
		{
			return;
		}
		GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		Shader.SetGlobalVector(GTShaderGlobals._GT_WorldSpaceCameraPos, GTShaderGlobals.gMainCameraWorldPos);
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0005C200 File Offset: 0x0005A400
	private static void UpdateTime()
	{
		GTShaderGlobals.gTime = (float)(DateTime.UtcNow - GTShaderGlobals.gStartTime).TotalSeconds;
		Shader.SetGlobalFloat(GTShaderGlobals._GT_Time, GTShaderGlobals.gTime);
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0005C23E File Offset: 0x0005A43E
	private static void UpdatePawns()
	{
		GTShaderGlobals.gActivePawns = GorillaPawn.ActiveCount;
		GorillaPawn.SyncPawnData();
		Shader.SetGlobalMatrixArray(GTShaderGlobals._GT_PawnData, GTShaderGlobals.gPawnData);
		Shader.SetGlobalInteger(GTShaderGlobals._GT_PawnActiveCount, GTShaderGlobals.gActivePawns);
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0005C278 File Offset: 0x0005A478
	private static void InitBlueNoiseTex()
	{
		GTShaderGlobals.gBlueNoiseTex = Resources.Load<Texture2D>("Graphics/Textures/noise_blue_rgba_128");
		GTShaderGlobals.gBlueNoiseTexWH = GTShaderGlobals.gBlueNoiseTex.GetTexelSize();
		Shader.SetGlobalTexture(GTShaderGlobals._GT_BlueNoiseTex, GTShaderGlobals.gBlueNoiseTex);
		Shader.SetGlobalVector(GTShaderGlobals._GT_BlueNoiseTex_WH, GTShaderGlobals.gBlueNoiseTexWH);
	}

	// Token: 0x040015E8 RID: 5608
	private static Camera gMainCamera;

	// Token: 0x040015E9 RID: 5609
	private static Transform gMainCameraXform;

	// Token: 0x040015EA RID: 5610
	private static Vector3 gMainCameraWorldPos;

	// Token: 0x040015EB RID: 5611
	[Space]
	private static int gIFrame;

	// Token: 0x040015EC RID: 5612
	private static float gTime;

	// Token: 0x040015ED RID: 5613
	[Space]
	private static Texture2D gBlueNoiseTex;

	// Token: 0x040015EE RID: 5614
	private static Vector4 gBlueNoiseTexWH;

	// Token: 0x040015EF RID: 5615
	[Space]
	private static int gActivePawns;

	// Token: 0x040015F0 RID: 5616
	[Space]
	private static DateTime gStartTime = DateTime.Today.AddDays(-1.0).ToUniversalTime();

	// Token: 0x040015F1 RID: 5617
	private static Matrix4x4[] gPawnData = GorillaPawn.ShaderData;

	// Token: 0x040015F2 RID: 5618
	private static ShaderHashId _GT_WorldSpaceCameraPos = "_GT_WorldSpaceCameraPos";

	// Token: 0x040015F3 RID: 5619
	private static ShaderHashId _GT_BlueNoiseTex = "_GT_BlueNoiseTex";

	// Token: 0x040015F4 RID: 5620
	private static ShaderHashId _GT_BlueNoiseTex_WH = "_GT_BlueNoiseTex_WH";

	// Token: 0x040015F5 RID: 5621
	private static ShaderHashId _GT_iFrame = "_GT_iFrame";

	// Token: 0x040015F6 RID: 5622
	private static ShaderHashId _GT_Time = "_GT_Time";

	// Token: 0x040015F7 RID: 5623
	private static ShaderHashId _GT_PawnData = "_GT_PawnData";

	// Token: 0x040015F8 RID: 5624
	private static ShaderHashId _GT_PawnActiveCount = "_GT_PawnActiveCount";
}
