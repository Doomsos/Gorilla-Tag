using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x020002DE RID: 734
public class MirrorCameraScript : MonoBehaviour
{
	// Token: 0x060011F4 RID: 4596 RVA: 0x0005E955 File Offset: 0x0005CB55
	private void Start()
	{
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		GorillaTagger.Instance.MirrorCameraCullingMask.AddCallback(new Action<int>(this.MirrorCullingMaskChanged), true);
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x0005E98C File Offset: 0x0005CB8C
	private void OnDestroy()
	{
		if (GorillaTagger.Instance != null)
		{
			GorillaTagger.Instance.MirrorCameraCullingMask.RemoveCallback(new Action<int>(this.MirrorCullingMaskChanged));
		}
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x0005E9B6 File Offset: 0x0005CBB6
	private void MirrorCullingMaskChanged(int newMask)
	{
		this.mirrorCamera.cullingMask = newMask;
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x0005E9C4 File Offset: 0x0005CBC4
	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		Vector3 right = base.transform.right;
		Vector4 plane;
		plane..ctor(right.x, right.y, right.z, -Vector3.Dot(right, position));
		Matrix4x4 zero = Matrix4x4.zero;
		this.CalculateReflectionMatrix(ref zero, plane);
		this.mirrorCamera.worldToCameraMatrix = this.mainCamera.worldToCameraMatrix * zero;
		Vector4 vector = this.CameraSpacePlane(this.mirrorCamera, position, right, 1f);
		this.mirrorCamera.projectionMatrix = this.mainCamera.CalculateObliqueMatrix(vector);
		Debug.Log(string.Format("Main Camera position {0}", this.mainCamera.transform.position));
		this.mirrorCamera.transform.position = zero.MultiplyPoint(this.mainCamera.transform.position);
		Debug.Log(string.Format("Reflected Camera position {0}", this.mirrorCamera.transform.position));
		this.mirrorCamera.transform.rotation = this.mainCamera.transform.rotation * Quaternion.Inverse(base.transform.rotation);
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
		{
			List<Material> list;
			using (CollectionPool<List<Material>, Material>.Get(ref list))
			{
				renderer.GetSharedMaterials(list);
				foreach (Material material in list)
				{
					if (material.shader == Shader.Find("Reflection"))
					{
						material.SetTexture("_ReflectionTex", this.mirrorCamera.targetTexture);
					}
				}
			}
		}
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x0005EBC8 File Offset: 0x0005CDC8
	private void CalculateReflectionMatrix(ref Matrix4x4 reflectionMatrix, Vector4 plane)
	{
		reflectionMatrix.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMatrix.m01 = -2f * plane[0] * plane[1];
		reflectionMatrix.m02 = -2f * plane[0] * plane[2];
		reflectionMatrix.m03 = -2f * plane[3] * plane[0];
		reflectionMatrix.m10 = -2f * plane[1] * plane[0];
		reflectionMatrix.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMatrix.m12 = -2f * plane[1] * plane[2];
		reflectionMatrix.m13 = -2f * plane[3] * plane[1];
		reflectionMatrix.m20 = -2f * plane[2] * plane[0];
		reflectionMatrix.m21 = -2f * plane[2] * plane[1];
		reflectionMatrix.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMatrix.m23 = -2f * plane[3] * plane[2];
		reflectionMatrix.m30 = 0f;
		reflectionMatrix.m31 = 0f;
		reflectionMatrix.m32 = 0f;
		reflectionMatrix.m33 = 1f;
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x0005ED70 File Offset: 0x0005CF70
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 vector = pos + normal * 0.07f;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 vector2 = worldToCameraMatrix.MultiplyPoint(vector);
		Vector3 vector3 = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector3.x, vector3.y, vector3.z, -Vector3.Dot(vector2, vector3));
	}

	// Token: 0x0400169C RID: 5788
	public Camera mainCamera;

	// Token: 0x0400169D RID: 5789
	public Camera mirrorCamera;
}
