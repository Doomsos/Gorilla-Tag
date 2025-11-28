using System;
using UnityEngine;

// Token: 0x020002C0 RID: 704
public class GTContactManager : MonoBehaviour
{
	// Token: 0x06001147 RID: 4423 RVA: 0x00002789 File Offset: 0x00000989
	[RuntimeInitializeOnLoadMethod(1)]
	private static void InitializeOnLoad()
	{
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x0005BF70 File Offset: 0x0005A170
	private static GTContactPoint[] InitContactPoints(int count)
	{
		GTContactPoint[] array = new GTContactPoint[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = new GTContactPoint();
		}
		return array;
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x0005BF9C File Offset: 0x0005A19C
	public static void RaiseContact(Vector3 point, Vector3 normal)
	{
		if (GTContactManager.gNextFree == -1)
		{
			return;
		}
		float time = GTShaderGlobals.Time;
		GTContactPoint gtcontactPoint = GTContactManager._gContactPoints[GTContactManager.gNextFree];
		gtcontactPoint.contactPoint = point;
		gtcontactPoint.radius = 0.04f;
		gtcontactPoint.counterVelocity = normal;
		gtcontactPoint.timestamp = time;
		gtcontactPoint.lifetime = 2f;
		gtcontactPoint.color = GTContactManager.gRND.NextColor();
		gtcontactPoint.free = 0U;
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x0005C004 File Offset: 0x0005A204
	public static void ProcessContacts()
	{
		Matrix4x4[] shaderData = GTContactManager.ShaderData;
		GTContactPoint[] gContactPoints = GTContactManager._gContactPoints;
		int frame = GTShaderGlobals.Frame;
		for (int i = 0; i < 32; i++)
		{
			GTContactManager.Transfer(ref gContactPoints[i].data, ref shaderData[i]);
		}
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x0005C044 File Offset: 0x0005A244
	private static void Transfer(ref Matrix4x4 from, ref Matrix4x4 to)
	{
		to.m00 = from.m00;
		to.m01 = from.m01;
		to.m02 = from.m02;
		to.m03 = from.m03;
		to.m10 = from.m10;
		to.m11 = from.m11;
		to.m12 = from.m12;
		to.m13 = from.m13;
		to.m20 = from.m20;
		to.m21 = from.m21;
		to.m22 = from.m22;
		to.m23 = from.m23;
		to.m30 = from.m30;
		to.m31 = from.m31;
		to.m32 = from.m32;
		to.m33 = from.m33;
	}

	// Token: 0x040015E3 RID: 5603
	public const int MAX_CONTACTS = 32;

	// Token: 0x040015E4 RID: 5604
	public static Matrix4x4[] ShaderData = new Matrix4x4[32];

	// Token: 0x040015E5 RID: 5605
	private static GTContactPoint[] _gContactPoints = GTContactManager.InitContactPoints(32);

	// Token: 0x040015E6 RID: 5606
	private static int gNextFree = 0;

	// Token: 0x040015E7 RID: 5607
	private static SRand gRND = new SRand(DateTime.UtcNow);
}
