using System;
using System.Collections.Generic;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200116B RID: 4459
	public class PrimitiveMeshFactory
	{
		// Token: 0x0600706B RID: 28779 RVA: 0x0024903C File Offset: 0x0024723C
		private static Mesh GetPooledLineMesh()
		{
			if (PrimitiveMeshFactory.s_lineMeshPool == null)
			{
				PrimitiveMeshFactory.s_lineMeshPool = new List<Mesh>();
				PrimitiveMeshFactory.s_lineMeshPool.Add(new Mesh());
			}
			if (PrimitiveMeshFactory.s_lastDrawLineFrame != Time.frameCount)
			{
				PrimitiveMeshFactory.s_iPooledMesh = 0;
				PrimitiveMeshFactory.s_lastDrawLineFrame = Time.frameCount;
			}
			if (PrimitiveMeshFactory.s_iPooledMesh == PrimitiveMeshFactory.s_lineMeshPool.Count)
			{
				PrimitiveMeshFactory.s_lineMeshPool.Capacity *= 2;
				for (int i = PrimitiveMeshFactory.s_iPooledMesh; i < PrimitiveMeshFactory.s_lineMeshPool.Capacity; i++)
				{
					PrimitiveMeshFactory.s_lineMeshPool.Add(new Mesh());
				}
			}
			Mesh mesh = PrimitiveMeshFactory.s_lineMeshPool[PrimitiveMeshFactory.s_iPooledMesh++];
			if (mesh == null)
			{
				mesh = (PrimitiveMeshFactory.s_lineMeshPool[PrimitiveMeshFactory.s_iPooledMesh - 1] = new Mesh());
			}
			return mesh;
		}

		// Token: 0x0600706C RID: 28780 RVA: 0x0024910C File Offset: 0x0024730C
		public static Mesh Line(Vector3 v0, Vector3 v1)
		{
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			Vector3[] vertices = new Vector3[]
			{
				v0,
				v1
			};
			int[] array = new int[]
			{
				default(int),
				1
			};
			pooledLineMesh.vertices = vertices;
			pooledLineMesh.SetIndices(array, 3, 0);
			pooledLineMesh.RecalculateBounds();
			return pooledLineMesh;
		}

		// Token: 0x0600706D RID: 28781 RVA: 0x00249164 File Offset: 0x00247364
		public static Mesh Lines(Vector3[] aVert)
		{
			if (aVert.Length <= 1)
			{
				return null;
			}
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			int[] array = new int[aVert.Length];
			for (int i = 0; i < aVert.Length; i++)
			{
				array[i] = i;
			}
			pooledLineMesh.vertices = aVert;
			pooledLineMesh.SetIndices(array, 3, 0);
			return pooledLineMesh;
		}

		// Token: 0x0600706E RID: 28782 RVA: 0x002491B8 File Offset: 0x002473B8
		public static Mesh LineStrip(Vector3[] aVert)
		{
			if (aVert.Length <= 1)
			{
				return null;
			}
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			int[] array = new int[aVert.Length];
			for (int i = 0; i < aVert.Length; i++)
			{
				array[i] = i;
			}
			pooledLineMesh.vertices = aVert;
			pooledLineMesh.SetIndices(array, 4, 0);
			return pooledLineMesh;
		}

		// Token: 0x0600706F RID: 28783 RVA: 0x0024920C File Offset: 0x0024740C
		public static Mesh BoxWireframe()
		{
			if (PrimitiveMeshFactory.s_boxWireframeMesh == null)
			{
				PrimitiveMeshFactory.s_boxWireframeMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				int[] array2 = new int[]
				{
					0,
					1,
					1,
					2,
					2,
					3,
					3,
					0,
					2,
					6,
					6,
					7,
					7,
					3,
					7,
					4,
					4,
					5,
					5,
					6,
					5,
					1,
					1,
					0,
					0,
					4
				};
				PrimitiveMeshFactory.s_boxWireframeMesh.vertices = array;
				PrimitiveMeshFactory.s_boxWireframeMesh.normals = array;
				PrimitiveMeshFactory.s_boxWireframeMesh.SetIndices(array2, 3, 0);
			}
			return PrimitiveMeshFactory.s_boxWireframeMesh;
		}

		// Token: 0x06007070 RID: 28784 RVA: 0x00249350 File Offset: 0x00247550
		public static Mesh BoxSolidColor()
		{
			if (PrimitiveMeshFactory.s_boxSolidColorMesh == null)
			{
				PrimitiveMeshFactory.s_boxSolidColorMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				int[] array = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					3,
					2,
					6,
					3,
					6,
					7,
					7,
					6,
					5,
					7,
					5,
					4,
					4,
					5,
					1,
					4,
					1,
					0,
					1,
					5,
					6,
					1,
					6,
					2,
					0,
					3,
					7,
					0,
					7,
					4
				};
				PrimitiveMeshFactory.s_boxSolidColorMesh.vertices = vertices;
				PrimitiveMeshFactory.s_boxSolidColorMesh.SetIndices(array, 0, 0);
			}
			return PrimitiveMeshFactory.s_boxSolidColorMesh;
		}

		// Token: 0x06007071 RID: 28785 RVA: 0x00249488 File Offset: 0x00247688
		public static Mesh BoxFlatShaded()
		{
			if (PrimitiveMeshFactory.s_boxFlatShadedMesh == null)
			{
				PrimitiveMeshFactory.s_boxFlatShadedMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				Vector3[] array2 = new Vector3[]
				{
					array[0],
					array[1],
					array[2],
					array[0],
					array[2],
					array[3],
					array[3],
					array[2],
					array[6],
					array[3],
					array[6],
					array[7],
					array[7],
					array[6],
					array[5],
					array[7],
					array[5],
					array[4],
					array[4],
					array[5],
					array[1],
					array[4],
					array[1],
					array[0],
					array[1],
					array[5],
					array[6],
					array[1],
					array[6],
					array[2],
					array[0],
					array[3],
					array[7],
					array[0],
					array[7],
					array[4]
				};
				Vector3[] array3 = new Vector3[]
				{
					new Vector3(0f, 0f, -1f),
					new Vector3(1f, 0f, 0f),
					new Vector3(0f, 0f, 1f),
					new Vector3(-1f, 0f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, -1f, 0f)
				};
				Vector3[] normals = new Vector3[]
				{
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[5],
					array3[5],
					array3[5],
					array3[5],
					array3[5],
					array3[5]
				};
				int[] array4 = new int[array2.Length];
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i] = i;
				}
				PrimitiveMeshFactory.s_boxFlatShadedMesh.vertices = array2;
				PrimitiveMeshFactory.s_boxFlatShadedMesh.normals = normals;
				PrimitiveMeshFactory.s_boxFlatShadedMesh.SetIndices(array4, 0, 0);
			}
			return PrimitiveMeshFactory.s_boxFlatShadedMesh;
		}

		// Token: 0x06007072 RID: 28786 RVA: 0x00249ABC File Offset: 0x00247CBC
		public static Mesh RectWireframe()
		{
			if (PrimitiveMeshFactory.s_rectWireframeMesh == null)
			{
				PrimitiveMeshFactory.s_rectWireframeMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				int[] array2 = new int[]
				{
					0,
					1,
					1,
					2,
					2,
					3,
					3,
					0
				};
				PrimitiveMeshFactory.s_rectWireframeMesh.vertices = array;
				PrimitiveMeshFactory.s_rectWireframeMesh.normals = array;
				PrimitiveMeshFactory.s_rectWireframeMesh.SetIndices(array2, 3, 0);
			}
			return PrimitiveMeshFactory.s_rectWireframeMesh;
		}

		// Token: 0x06007073 RID: 28787 RVA: 0x00249B90 File Offset: 0x00247D90
		public static Mesh RectSolidColor()
		{
			if (PrimitiveMeshFactory.s_rectSolidColorMesh == null)
			{
				PrimitiveMeshFactory.s_rectSolidColorMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				int[] array = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					0,
					2,
					1,
					0,
					3,
					2
				};
				PrimitiveMeshFactory.s_rectSolidColorMesh.vertices = vertices;
				PrimitiveMeshFactory.s_rectSolidColorMesh.SetIndices(array, 0, 0);
			}
			return PrimitiveMeshFactory.s_rectSolidColorMesh;
		}

		// Token: 0x06007074 RID: 28788 RVA: 0x00249C5C File Offset: 0x00247E5C
		public static Mesh RectFlatShaded()
		{
			if (PrimitiveMeshFactory.s_rectFlatShadedMesh == null)
			{
				PrimitiveMeshFactory.s_rectFlatShadedMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				Vector3[] normals = new Vector3[]
				{
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f)
				};
				int[] array = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					4,
					6,
					5,
					4,
					7,
					6
				};
				PrimitiveMeshFactory.s_rectFlatShadedMesh.vertices = vertices;
				PrimitiveMeshFactory.s_rectFlatShadedMesh.normals = normals;
				PrimitiveMeshFactory.s_rectFlatShadedMesh.SetIndices(array, 0, 0);
			}
			return PrimitiveMeshFactory.s_rectFlatShadedMesh;
		}

		// Token: 0x06007075 RID: 28789 RVA: 0x00249E7C File Offset: 0x0024807C
		public static Mesh CircleWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleWireframeMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments];
				int[] array2 = new int[numSegments + 1];
				float num = 6.2831855f / (float)numSegments;
				float num2 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num2) * Vector3.right + Mathf.Sin(num2) * Vector3.forward;
					array2[i] = i;
					num2 += num;
				}
				array2[numSegments] = 0;
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, 4, 0);
				if (PrimitiveMeshFactory.s_circleWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007076 RID: 28790 RVA: 0x00249F70 File Offset: 0x00248170
		public static Mesh CircleSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleSolidColorMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 6];
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 += num2;
					array2[num++] = numSegments;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
				}
				array[numSegments] = Vector3.zero;
				mesh.vertices = array;
				mesh.SetIndices(array2, 0, 0);
				if (PrimitiveMeshFactory.s_circleSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007077 RID: 28791 RVA: 0x0024A0A0 File Offset: 0x002482A0
		public static Mesh CircleFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleFlatShadedMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(numSegments + 1) * 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 6];
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 += num2;
					array2[i] = new Vector3(0f, 1f, 0f);
					array3[num++] = numSegments;
					array3[num++] = (i + 1) % numSegments;
					array3[num++] = i;
				}
				array[numSegments] = Vector3.zero;
				array2[numSegments] = new Vector3(0f, 1f, 0f);
				num3 = 0f;
				for (int j = 0; j < numSegments; j++)
				{
					array[j + numSegments + 1] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 -= num2;
					array2[j + numSegments + 1] = new Vector3(0f, -1f, 0f);
					array3[num++] = numSegments * 2 + 1;
					array3[num++] = (j + 1) % numSegments + numSegments + 1;
					array3[num++] = j + (numSegments + 1);
				}
				array[numSegments * 2 + 1] = Vector3.zero;
				array2[numSegments * 2 + 1] = new Vector3(0f, -1f, 0f);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_circleFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007078 RID: 28792 RVA: 0x0024A2DC File Offset: 0x002484DC
		public static Mesh CylinderWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderWireframeMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2];
				int[] array2 = new int[numSegments * 6];
				Vector3 vector;
				vector..ctor(0f, -0.5f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, 0.5f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array[i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments + i;
					array2[num++] = numSegments + i;
					array2[num++] = numSegments + (i + 1) % numSegments;
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, 3, 0);
				if (PrimitiveMeshFactory.s_cylinderWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007079 RID: 28793 RVA: 0x0024A46C File Offset: 0x0024866C
		public static Mesh CylinderSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2 + 2];
				int[] array2 = new int[numSegments * 12];
				Vector3 vector;
				vector..ctor(0f, -0.5f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 2 + 1;
				array[num2] = vector;
				array[num3] = vector2;
				int num4 = 0;
				float num5 = 6.2831855f / (float)numSegments;
				float num6 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num6) * Vector3.right + Mathf.Sin(num6) * Vector3.forward;
					array[num + i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num4++] = num2;
					array2[num4++] = num + i;
					array2[num4++] = num + (i + 1) % numSegments;
					array2[num4++] = num + i;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = num + (i + 1) % numSegments;
					array2[num4++] = num + i;
					array2[num4++] = numSegments + i;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = num3;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = numSegments + i;
					num6 += num5;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, 0, 0);
				if (PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600707A RID: 28794 RVA: 0x0024A684 File Offset: 0x00248884
		public static Mesh CylinderFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 6 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 12];
				Vector3 vector;
				vector..ctor(0f, -0.5f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 6;
				int num4 = numSegments * 6 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, -1f, 0f);
				array2[num4] = new Vector3(0f, 1f, 0f);
				int num5 = 0;
				float num6 = 6.2831855f / (float)numSegments;
				float num7 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num + i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num + i] = new Vector3(0f, -1f, 0f);
					array2[numSegments + i] = new Vector3(0f, 1f, 0f);
					array3[num5++] = num3;
					array3[num5++] = num + i;
					array3[num5++] = num + (i + 1) % numSegments;
					array3[num5++] = num4;
					array3[num5++] = numSegments + (i + 1) % numSegments;
					array3[num5++] = numSegments + i;
					num7 += num6;
					Vector3 vector4 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num2 + i * 4] = vector + vector3;
					array[num2 + i * 4 + 1] = vector2 + vector3;
					array[num2 + i * 4 + 2] = vector + vector4;
					array[num2 + i * 4 + 3] = vector2 + vector4;
					Vector3 normalized = Vector3.Cross(vector2 - vector, vector4 - vector3).normalized;
					array2[num2 + i * 4] = normalized;
					array2[num2 + i * 4 + 1] = normalized;
					array2[num2 + i * 4 + 2] = normalized;
					array2[num2 + i * 4 + 3] = normalized;
					array3[num5++] = num2 + i * 4;
					array3[num5++] = num2 + i * 4 + 3;
					array3[num5++] = num2 + i * 4 + 2;
					array3[num5++] = num2 + i * 4;
					array3[num5++] = num2 + i * 4 + 1;
					array3[num5++] = num2 + i * 4 + 3;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600707B RID: 28795 RVA: 0x0024AA1C File Offset: 0x00248C1C
		public static Mesh CylinderSmoothShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 4 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 12];
				Vector3 vector;
				vector..ctor(0f, -0.5f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 4;
				int num4 = numSegments * 4 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, -1f, 0f);
				array2[num4] = new Vector3(0f, 1f, 0f);
				int num5 = 0;
				float num6 = 6.2831855f / (float)numSegments;
				float num7 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num + i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num + i] = new Vector3(0f, -1f, 0f);
					array2[numSegments + i] = new Vector3(0f, 1f, 0f);
					array3[num5++] = num3;
					array3[num5++] = num + i;
					array3[num5++] = num + (i + 1) % numSegments;
					array3[num5++] = num4;
					array3[num5++] = numSegments + (i + 1) % numSegments;
					array3[num5++] = numSegments + i;
					num7 += num6;
					array[num2 + i * 2] = vector + vector3;
					array[num2 + i * 2 + 1] = vector2 + vector3;
					array2[num2 + i * 2] = vector3;
					array2[num2 + i * 2 + 1] = vector3;
					array3[num5++] = num2 + i * 2;
					array3[num5++] = num2 + (i * 2 + 3) % (numSegments * 2);
					array3[num5++] = num2 + (i * 2 + 2) % (numSegments * 2);
					array3[num5++] = num2 + i * 2;
					array3[num5++] = num2 + (i * 2 + 1) % (numSegments * 2);
					array3[num5++] = num2 + (i * 2 + 3) % (numSegments * 2);
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600707C RID: 28796 RVA: 0x0024AD28 File Offset: 0x00248F28
		public static Mesh SphereWireframe(int latSegments, int longSegments)
		{
			if (latSegments <= 0 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			int num = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereWireframeMeshPool.TryGetValue(num, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegments * (latSegments - 1) + 2];
				int[] array2 = new int[longSegments * (latSegments * 2 - 1) * 2];
				Vector3 vector;
				vector..ctor(0f, 1f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, -1f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				float[] array3 = new float[latSegments];
				float[] array4 = new float[latSegments];
				float num4 = 3.1415927f / (float)latSegments;
				float num5 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num5 += num4;
					array3[i] = Mathf.Sin(num5);
					array4[i] = Mathf.Cos(num5);
				}
				float[] array5 = new float[longSegments];
				float[] array6 = new float[longSegments];
				float num6 = 6.2831855f / (float)longSegments;
				float num7 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num7 += num6;
					array5[j] = Mathf.Sin(num7);
					array6[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num10 = array5[k];
					float num11 = array6[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num12 = array3[l];
						float num13 = array4[l];
						array[num8] = new Vector3(num11 * num12, num13, num10 * num12);
						if (l == 0)
						{
							array2[num9++] = num2;
							array2[num9++] = num8;
						}
						else
						{
							array2[num9++] = num8 - 1;
							array2[num9++] = num8;
						}
						array2[num9++] = num8;
						array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
						if (l == latSegments - 2)
						{
							array2[num9++] = num8;
							array2[num9++] = num3;
						}
						num8++;
					}
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, 3, 0);
				if (PrimitiveMeshFactory.s_sphereWireframeMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_sphereWireframeMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_sphereWireframeMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x0600707D RID: 28797 RVA: 0x0024AFA0 File Offset: 0x002491A0
		public static Mesh SphereSolidColor(int latSegments, int longSegments)
		{
			if (latSegments <= 0 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			int num = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereSolidColorMeshPool.TryGetValue(num, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegments * (latSegments - 1) + 2];
				int[] array2 = new int[longSegments * (latSegments - 1) * 2 * 3];
				Vector3 vector;
				vector..ctor(0f, 1f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, -1f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				float[] array3 = new float[latSegments];
				float[] array4 = new float[latSegments];
				float num4 = 3.1415927f / (float)latSegments;
				float num5 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num5 += num4;
					array3[i] = Mathf.Sin(num5);
					array4[i] = Mathf.Cos(num5);
				}
				float[] array5 = new float[longSegments];
				float[] array6 = new float[longSegments];
				float num6 = 6.2831855f / (float)longSegments;
				float num7 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num7 += num6;
					array5[j] = Mathf.Sin(num7);
					array6[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num10 = array5[k];
					float num11 = array6[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num12 = array3[l];
						float num13 = array4[l];
						array[num8] = new Vector3(num11 * num12, num13, num10 * num12);
						if (l == 0)
						{
							array2[num9++] = num2;
							array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num9++] = num8;
						}
						if (l < latSegments - 2)
						{
							array2[num9++] = num8 + 1;
							array2[num9++] = num8;
							array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num9++] = num8 + 1;
							array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num9++] = (num8 + latSegments) % (longSegments * (latSegments - 1));
						}
						if (l == latSegments - 2)
						{
							array2[num9++] = num8;
							array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num9++] = num3;
						}
						num8++;
					}
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, 0, 0);
				if (PrimitiveMeshFactory.s_sphereSolidColorMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_sphereSolidColorMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_sphereSolidColorMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x0600707E RID: 28798 RVA: 0x0024B26C File Offset: 0x0024946C
		public static Mesh SphereFlatShaded(int latSegments, int longSegments)
		{
			if (latSegments <= 1 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int num = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.TryGetValue(num, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num2 = (latSegments - 2) * 4 + 6;
				int num3 = (latSegments - 2) * 2 + 2;
				Vector3[] array = new Vector3[longSegments * num2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegments * num3 * 3];
				Vector3 vector;
				vector..ctor(0f, 1f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, -1f, 0f);
				float[] array4 = new float[latSegments];
				float[] array5 = new float[latSegments];
				float num4 = 3.1415927f / (float)latSegments;
				float num5 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num5 += num4;
					array4[i] = Mathf.Sin(num5);
					array5[i] = Mathf.Cos(num5);
				}
				float[] array6 = new float[longSegments];
				float[] array7 = new float[longSegments];
				float num6 = 6.2831855f / (float)longSegments;
				float num7 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num7 += num6;
					array6[j] = Mathf.Sin(num7);
					array7[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num11 = array6[k];
					float num12 = array7[k];
					float num13 = array6[(k + 1) % longSegments];
					float num14 = array7[(k + 1) % longSegments];
					int num15 = num8;
					array[num8++] = vector;
					array[num8++] = new Vector3(num12 * array4[0], array5[0], num11 * array4[0]);
					array[num8++] = new Vector3(num14 * array4[0], array5[0], num13 * array4[0]);
					int num16 = num8;
					array[num8++] = vector2;
					array[num8++] = new Vector3(num12 * array4[latSegments - 2], array5[latSegments - 2], num11 * array4[latSegments - 2]);
					array[num8++] = new Vector3(num14 * array4[latSegments - 2], array5[latSegments - 2], num13 * array4[latSegments - 2]);
					Vector3 normalized = Vector3.Cross(array[num15 + 2] - array[num15], array[num15 + 1] - array[num15]).normalized;
					array2[num9++] = normalized;
					array2[num9++] = normalized;
					array2[num9++] = normalized;
					Vector3 normalized2 = Vector3.Cross(array[num16 + 1] - array[num16], array[num16 + 2] - array[num16]).normalized;
					array2[num9++] = normalized2;
					array2[num9++] = normalized2;
					array2[num9++] = normalized2;
					array3[num10++] = num15;
					array3[num10++] = num15 + 2;
					array3[num10++] = num15 + 1;
					array3[num10++] = num16;
					array3[num10++] = num16 + 1;
					array3[num10++] = num16 + 2;
					for (int l = 0; l < latSegments - 2; l++)
					{
						float num17 = array4[l];
						float num18 = array5[l];
						float num19 = array4[l + 1];
						float num20 = array5[l + 1];
						int num21 = num8;
						array[num8++] = new Vector3(num12 * num17, num18, num11 * num17);
						array[num8++] = new Vector3(num14 * num17, num18, num13 * num17);
						array[num8++] = new Vector3(num14 * num19, num20, num13 * num19);
						array[num8++] = new Vector3(num12 * num19, num20, num11 * num19);
						Vector3 normalized3 = Vector3.Cross(array[num21 + 1] - array[num21], array[num21 + 2] - array[num21]).normalized;
						array2[num9++] = normalized3;
						array2[num9++] = normalized3;
						array2[num9++] = normalized3;
						array2[num9++] = normalized3;
						array3[num10++] = num21;
						array3[num10++] = num21 + 1;
						array3[num10++] = num21 + 2;
						array3[num10++] = num21;
						array3[num10++] = num21 + 2;
						array3[num10++] = num21 + 3;
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x0600707F RID: 28799 RVA: 0x0024B7C4 File Offset: 0x002499C4
		public static Mesh SphereSmoothShaded(int latSegments, int longSegments)
		{
			if (latSegments <= 1 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int num = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.TryGetValue(num, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num2 = latSegments - 1;
				int num3 = (latSegments - 2) * 2 + 2;
				Vector3[] array = new Vector3[longSegments * num2 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegments * num3 * 3];
				Vector3 vector;
				vector..ctor(0f, 1f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, -1f, 0f);
				int num4 = longSegments * num2;
				int num5 = num4 + 1;
				array[num4] = vector;
				array[num5] = vector2;
				array2[num4] = new Vector3(0f, 1f, 0f);
				array2[num5] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegments];
				float[] array5 = new float[latSegments];
				float num6 = 3.1415927f / (float)latSegments;
				float num7 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num7 += num6;
					array4[i] = Mathf.Sin(num7);
					array5[i] = Mathf.Cos(num7);
				}
				float[] array6 = new float[longSegments];
				float[] array7 = new float[longSegments];
				float num8 = 6.2831855f / (float)longSegments;
				float num9 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num9 += num8;
					array6[j] = Mathf.Sin(num9);
					array7[j] = Mathf.Cos(num9);
				}
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num13 = array6[k];
					float num14 = array7[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num15 = array4[l];
						float num16 = array5[l];
						Vector3 vector3;
						vector3..ctor(num14 * num15, num16, num13 * num15);
						array[num10++] = vector3;
						array2[num11++] = vector3;
						int num17 = num10 - 1;
						int num18 = num17 + 1;
						int num19 = (num17 + num2) % (longSegments * num2);
						int num20 = (num17 + num2 + 1) % (longSegments * num2);
						if (latSegments == 2)
						{
							array3[num12++] = num4;
							array3[num12++] = num19;
							array3[num12++] = num17;
							array3[num12++] = num5;
							array3[num12++] = num18;
							array3[num12++] = num20;
						}
						else if (l < latSegments - 2)
						{
							if (l == 0)
							{
								array3[num12++] = num4;
								array3[num12++] = num19;
								array3[num12++] = num17;
							}
							else if (l == latSegments - 3)
							{
								array3[num12++] = num5;
								array3[num12++] = num18;
								array3[num12++] = num20;
							}
							array3[num12++] = num17;
							array3[num12++] = num20;
							array3[num12++] = num18;
							array3[num12++] = num17;
							array3[num12++] = num19;
							array3[num12++] = num20;
						}
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06007080 RID: 28800 RVA: 0x0024BB4C File Offset: 0x00249D4C
		public static Mesh CapsuleWireframe(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			int num = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleWireframeMeshPool.TryGetValue(num, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				int[] array2 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4 + 1) * 2];
				Vector3 vector;
				vector..ctor(0f, 1.5f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, -1.5f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				float[] array3 = new float[latSegmentsPerCap];
				float[] array4 = new float[latSegmentsPerCap];
				float num4 = 1.5707964f / (float)latSegmentsPerCap;
				float num5 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num5 += num4;
					array3[i] = Mathf.Sin(num5);
					array4[i] = Mathf.Cos(num5);
				}
				float[] array5 = new float[longSegmentsPerCap];
				float[] array6 = new float[longSegmentsPerCap];
				float num6 = 6.2831855f / (float)longSegmentsPerCap;
				float num7 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num7 += num6;
					array5[j] = Mathf.Sin(num7);
					array6[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num10 = array5[k];
					float num11 = array6[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num12 = array3[l];
						float num13 = array4[l];
						array[num8] = new Vector3(num11 * num12, num13 + 0.5f, num10 * num12);
						array[num8 + 1] = new Vector3(num11 * num12, -num13 - 0.5f, num10 * num12);
						if (caps)
						{
							if (l == 0)
							{
								array2[num9++] = num2;
								array2[num9++] = num8;
								if (!topCapOnly)
								{
									array2[num9++] = num3;
									array2[num9++] = num8 + 1;
								}
							}
							else
							{
								array2[num9++] = num8 - 2;
								array2[num9++] = num8;
								if (!topCapOnly)
								{
									array2[num9++] = num8 - 1;
									array2[num9++] = num8 + 1;
								}
							}
						}
						if (caps || l == latSegmentsPerCap - 1)
						{
							array2[num9++] = num8;
							array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							if (!topCapOnly)
							{
								array2[num9++] = num8 + 1;
								array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						if (sides && l == latSegmentsPerCap - 1)
						{
							array2[num9++] = num8;
							array2[num9++] = num8 + 1;
						}
						num8 += 2;
					}
				}
				Array.Resize<int>(ref array2, num9);
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, 3, 0);
				if (PrimitiveMeshFactory.s_capsuleWireframeMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_capsuleWireframeMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_capsuleWireframeMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06007081 RID: 28801 RVA: 0x0024BE94 File Offset: 0x0024A094
		public static Mesh CapsuleSolidColor(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			int num = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.TryGetValue(num, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				int[] array2 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector;
				vector..ctor(0f, 1.5f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, -1.5f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				float[] array3 = new float[latSegmentsPerCap];
				float[] array4 = new float[latSegmentsPerCap];
				float num4 = 1.5707964f / (float)latSegmentsPerCap;
				float num5 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num5 += num4;
					array3[i] = Mathf.Sin(num5);
					array4[i] = Mathf.Cos(num5);
				}
				float[] array5 = new float[longSegmentsPerCap];
				float[] array6 = new float[longSegmentsPerCap];
				float num6 = 6.2831855f / (float)longSegmentsPerCap;
				float num7 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num7 += num6;
					array5[j] = Mathf.Sin(num7);
					array6[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num10 = array5[k];
					float num11 = array6[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num12 = array3[l];
						float num13 = array4[l];
						array[num8] = new Vector3(num11 * num12, num13 + 0.5f, num10 * num12);
						array[num8 + 1] = new Vector3(num11 * num12, -num13 - 0.5f, num10 * num12);
						if (l == 0)
						{
							if (caps)
							{
								array2[num9++] = num2;
								array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = num8;
								if (!topCapOnly)
								{
									array2[num9++] = num3;
									array2[num9++] = num8 + 1;
									array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
						}
						else
						{
							if (caps)
							{
								array2[num9++] = num8 - 2;
								array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = num8;
								array2[num9++] = num8 - 2;
								array2[num9++] = (num8 - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								if (!topCapOnly)
								{
									array2[num9++] = num8 - 1;
									array2[num9++] = num8 + 1;
									array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array2[num9++] = num8 - 1;
									array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array2[num9++] = (num8 - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
							if (sides && l == latSegmentsPerCap - 1)
							{
								array2[num9++] = num8;
								array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = num8 + 1;
								array2[num9++] = num8;
								array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						num8 += 2;
					}
				}
				Array.Resize<int>(ref array2, num9);
				mesh.vertices = array;
				mesh.SetIndices(array2, 0, 0);
				if (PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06007082 RID: 28802 RVA: 0x0024C2AC File Offset: 0x0024A4AC
		public static Mesh CapsuleFlatShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int num = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.TryGetValue(num, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * (latSegmentsPerCap - 1) * 8 + longSegmentsPerCap * 10];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector;
				vector..ctor(0f, 1.5f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, -1.5f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				array2[num2] = new Vector3(0f, 1f, 0f);
				array2[num3] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegmentsPerCap];
				float[] array5 = new float[latSegmentsPerCap];
				float num4 = 1.5707964f / (float)latSegmentsPerCap;
				float num5 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num5 += num4;
					array4[i] = Mathf.Sin(num5);
					array5[i] = Mathf.Cos(num5);
				}
				float[] array6 = new float[longSegmentsPerCap];
				float[] array7 = new float[longSegmentsPerCap];
				float num6 = 6.2831855f / (float)longSegmentsPerCap;
				float num7 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num7 += num6;
					array6[j] = Mathf.Sin(num7);
					array7[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num11 = array6[k];
					float num12 = array7[k];
					float num13 = array6[(k + 1) % longSegmentsPerCap];
					float num14 = array7[(k + 1) % longSegmentsPerCap];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num15 = array4[l];
						float num16 = array5[l];
						if (caps && l < latSegmentsPerCap - 1)
						{
							if (l == 0)
							{
								int num17 = num8;
								array[num8++] = vector;
								array[num8++] = new Vector3(num12 * num15, num16 + 0.5f, num11 * num15);
								array[num8++] = new Vector3(num14 * num15, num16 + 0.5f, num13 * num15);
								Vector3 vector3 = Vector3.Cross(array[num17 + 2] - array[num17], array[num17 + 1] - array[num17]);
								array2[num9++] = vector3;
								array2[num9++] = vector3;
								array2[num9++] = vector3;
								array3[num10++] = num17;
								array3[num10++] = num17 + 2;
								array3[num10++] = num17 + 1;
								if (!topCapOnly)
								{
									int num18 = num8;
									array[num8++] = vector2;
									array[num8++] = new Vector3(num12 * num15, -num16 - 0.5f, num11 * num15);
									array[num8++] = new Vector3(num14 * num15, -num16 - 0.5f, num13 * num15);
									Vector3 normalized = Vector3.Cross(array[num18 + 1] - array[num18], array[num18 + 2] - array[num18]).normalized;
									array2[num9++] = normalized;
									array2[num9++] = normalized;
									array2[num9++] = normalized;
									array3[num10++] = num18;
									array3[num10++] = num18 + 1;
									array3[num10++] = num18 + 2;
								}
							}
							float num19 = array4[l + 1];
							float num20 = array5[l + 1];
							if (caps)
							{
								int num21 = num8;
								array[num8++] = new Vector3(num12 * num15, num16 + 0.5f, num11 * num15);
								array[num8++] = new Vector3(num12 * num19, num20 + 0.5f, num11 * num19);
								array[num8++] = new Vector3(num14 * num19, num20 + 0.5f, num13 * num19);
								array[num8++] = new Vector3(num14 * num15, num16 + 0.5f, num13 * num15);
								Vector3 vector4 = Vector3.Cross(array[num21 + 3] - array[num21], array[num21 + 1] - array[num21]);
								array2[num9++] = vector4;
								array2[num9++] = vector4;
								array2[num9++] = vector4;
								array2[num9++] = vector4;
								array3[num10++] = num21;
								array3[num10++] = num21 + 2;
								array3[num10++] = num21 + 1;
								array3[num10++] = num21;
								array3[num10++] = num21 + 3;
								array3[num10++] = num21 + 2;
								if (!topCapOnly)
								{
									int num22 = num8;
									array[num8++] = new Vector3(num12 * num15, -num16 - 0.5f, num11 * num15);
									array[num8++] = new Vector3(num12 * num19, -num20 - 0.5f, num11 * num19);
									array[num8++] = new Vector3(num14 * num19, -num20 - 0.5f, num13 * num19);
									array[num8++] = new Vector3(num14 * num15, -num16 - 0.5f, num13 * num15);
									Vector3 vector5 = Vector3.Cross(array[num22 + 1] - array[num22], array[num22 + 3] - array[num22]);
									array2[num9++] = vector5;
									array2[num9++] = vector5;
									array2[num9++] = vector5;
									array2[num9++] = vector5;
									array3[num10++] = num22;
									array3[num10++] = num22 + 1;
									array3[num10++] = num22 + 2;
									array3[num10++] = num22;
									array3[num10++] = num22 + 2;
									array3[num10++] = num22 + 3;
								}
							}
						}
						else if (sides && l == latSegmentsPerCap - 1)
						{
							int num23 = num8;
							array[num8++] = new Vector3(num12 * num15, num16 + 0.5f, num11 * num15);
							array[num8++] = new Vector3(num12 * num15, -num16 - 0.5f, num11 * num15);
							array[num8++] = new Vector3(num14 * num15, -num16 - 0.5f, num13 * num15);
							array[num8++] = new Vector3(num14 * num15, num16 + 0.5f, num13 * num15);
							Vector3 normalized2 = Vector3.Cross(array[num23 + 3] - array[num23], array[num23 + 1] - array[num23]).normalized;
							array2[num9++] = normalized2;
							array2[num9++] = normalized2;
							array2[num9++] = normalized2;
							array2[num9++] = normalized2;
							array3[num10++] = num23;
							array3[num10++] = num23 + 2;
							array3[num10++] = num23 + 1;
							array3[num10++] = num23;
							array3[num10++] = num23 + 3;
							array3[num10++] = num23 + 2;
						}
					}
				}
				Array.Resize<int>(ref array3, num10);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06007083 RID: 28803 RVA: 0x0024CB5C File Offset: 0x0024AD5C
		public static Mesh CapsuleSmoothShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int num = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.TryGetValue(num, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector;
				vector..ctor(0f, 1.5f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, -1.5f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				array2[num2] = new Vector3(0f, 1f, 0f);
				array2[num3] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegmentsPerCap];
				float[] array5 = new float[latSegmentsPerCap];
				float num4 = 1.5707964f / (float)latSegmentsPerCap;
				float num5 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num5 += num4;
					array4[i] = Mathf.Sin(num5);
					array5[i] = Mathf.Cos(num5);
				}
				float[] array6 = new float[longSegmentsPerCap];
				float[] array7 = new float[longSegmentsPerCap];
				float num6 = 6.2831855f / (float)longSegmentsPerCap;
				float num7 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num7 += num6;
					array6[j] = Mathf.Sin(num7);
					array7[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num11 = array6[k];
					float num12 = array7[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num13 = array4[l];
						float num14 = array5[l];
						array[num8] = new Vector3(num12 * num13, num14 + 0.5f, num11 * num13);
						array[num8 + 1] = new Vector3(num12 * num13, -num14 - 0.5f, num11 * num13);
						array2[num9] = new Vector3(num12 * num13, num14, num11 * num13);
						array2[num9 + 1] = new Vector3(num12 * num13, -num14, num11 * num13);
						if (caps && l == 0)
						{
							array3[num10++] = num2;
							array3[num10++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							array3[num10++] = num8;
							if (!topCapOnly)
							{
								array3[num10++] = num3;
								array3[num10++] = num8 + 1;
								array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						else
						{
							if (caps)
							{
								array3[num10++] = num8 - 2;
								array3[num10++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[num10++] = num8;
								array3[num10++] = num8 - 2;
								array3[num10++] = (num8 - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[num10++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								if (!topCapOnly)
								{
									array3[num10++] = num8 - 1;
									array3[num10++] = num8 + 1;
									array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array3[num10++] = num8 - 1;
									array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array3[num10++] = (num8 - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
							if (sides && l == latSegmentsPerCap - 1)
							{
								array3[num10++] = num8;
								array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[num10++] = num8 + 1;
								array3[num10++] = num8;
								array3[num10++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						num8 += 2;
						num9 += 2;
					}
				}
				Array.Resize<int>(ref array3, num10);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06007084 RID: 28804 RVA: 0x0024D010 File Offset: 0x0024B210
		public static Mesh Capsule2DWireframe(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.TryGetValue(capSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(capSegments + 1) * 2];
				int[] array2 = new int[(capSegments + 1) * 4];
				int num = 0;
				int num2 = 0;
				float num3 = 3.1415927f / (float)capSegments;
				float num4 = 0f;
				for (int i = 0; i < capSegments; i++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
				for (int j = 0; j < capSegments; j++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
				for (int k = 0; k < array.Length - 1; k++)
				{
					array2[num2++] = k;
					array2[num2++] = (k + 1) % array.Length;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, 4, 0);
				if (PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007085 RID: 28805 RVA: 0x0024D1C0 File Offset: 0x0024B3C0
		public static Mesh Capsule2DSolidColor(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.TryGetValue(capSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(capSegments + 1) * 2];
				int[] array2 = new int[(capSegments + 1) * 12];
				int num = 0;
				int num2 = 0;
				float num3 = 3.1415927f / (float)capSegments;
				float num4 = 0f;
				for (int i = 0; i < capSegments; i++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
				for (int j = 0; j < capSegments; j++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
				for (int k = 1; k < array.Length; k++)
				{
					array2[num2++] = 0;
					array2[num2++] = (k + 1) % array.Length;
					array2[num2++] = k;
					array2[num2++] = 0;
					array2[num2++] = k;
					array2[num2++] = (k + 1) % array.Length;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, 0, 0);
				if (PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007086 RID: 28806 RVA: 0x0024D398 File Offset: 0x0024B598
		public static Mesh Capsule2DFlatShaded(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.TryGetValue(capSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num = (capSegments + 1) * 2;
				Vector3[] array = new Vector3[num * 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[num * 6];
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				float num5 = 3.1415927f / (float)capSegments;
				float num6 = 0f;
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < capSegments; j++)
					{
						array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) + 0.5f, 0f);
						num6 += num5;
					}
					array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) + 0.5f, 0f);
					for (int k = 0; k < capSegments; k++)
					{
						array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) - 0.5f, 0f);
						num6 += num5;
					}
					array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) - 0.5f, 0f);
					Vector3 vector;
					vector..ctor(0f, 0f, (i == 0) ? -1f : 1f);
					for (int l = 0; l < num; l++)
					{
						array2[num3++] = vector;
					}
				}
				for (int m = 1; m < num; m++)
				{
					array3[num4++] = 0;
					array3[num4++] = (m + 1) % num;
					array3[num4++] = m;
					array3[num4++] = num;
					array3[num4++] = num + m;
					array3[num4++] = num + (m + 1) % num;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007087 RID: 28807 RVA: 0x0024D5EC File Offset: 0x0024B7EC
		public static Mesh ConeWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneWireframeMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 4];
				array[numSegments] = new Vector3(0f, 1f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments;
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, 3, 0);
				if (PrimitiveMeshFactory.s_coneWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007088 RID: 28808 RVA: 0x0024D728 File Offset: 0x0024B928
		public static Mesh ConeSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneSolidColorMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 3 + (numSegments - 2) * 3];
				array[numSegments] = new Vector3(0f, 1f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array2[num++] = numSegments;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					if (i >= 2)
					{
						array2[num++] = 0;
						array2[num++] = i - 1;
						array2[num++] = i;
					}
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, 0, 0);
				if (PrimitiveMeshFactory.s_coneSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06007089 RID: 28809 RVA: 0x0024D884 File Offset: 0x0024BA84
		public static Mesh ConeFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneFlatShadedMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 3 + numSegments];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 3 + (numSegments - 2) * 3];
				Vector3 vector;
				vector..ctor(0f, 1f, 0f);
				Vector3[] array4 = new Vector3[numSegments];
				float num = 6.2831855f / (float)numSegments;
				float num2 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array4[i] = Mathf.Cos(num2) * Vector3.right + Mathf.Sin(num2) * Vector3.forward;
					num2 += num;
				}
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				for (int j = 0; j < numSegments; j++)
				{
					int num6 = num3;
					array[num3++] = vector;
					array[num3++] = array4[j];
					array[num3++] = array4[(j + 1) % numSegments];
					Vector3 normalized = Vector3.Cross(array[num6 + 2] - array[num6], array[num6 + 1] - array[num6]).normalized;
					array2[num5++] = normalized;
					array2[num5++] = normalized;
					array2[num5++] = normalized;
					array3[num4++] = num6;
					array3[num4++] = num6 + 2;
					array3[num4++] = num6 + 1;
				}
				int num7 = num3;
				for (int k = 0; k < numSegments; k++)
				{
					array[num3++] = array4[k];
					array2[num5++] = new Vector3(0f, -1f, 0f);
					if (k >= 2)
					{
						array3[num4++] = num7;
						array3[num4++] = num7 + k - 1;
						array3[num4++] = num7 + k;
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_coneFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600708A RID: 28810 RVA: 0x0024DB0C File Offset: 0x0024BD0C
		public static Mesh ConeSmoothShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneSmoothhadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneSmoothhadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.TryGetValue(numSegments, ref mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2 + 1];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 3 + (numSegments - 2) * 3];
				int num = array.Length - 1;
				array[num] = new Vector3(0f, 1f, 0f);
				array2[num] = new Vector3(0f, 0f, 0f);
				float num2 = Mathf.Sqrt(0.5f);
				int num3 = 0;
				float num4 = 6.2831855f / (float)numSegments;
				float num5 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					float num6 = Mathf.Cos(num5);
					float num7 = Mathf.Sin(num5);
					Vector3 vector = num6 * Vector3.right + num7 * Vector3.forward;
					array[i] = vector;
					array[numSegments + i] = vector;
					array2[i] = new Vector3(num6 * num2, num2, num7 * num2);
					array2[numSegments + i] = new Vector3(0f, -1f, 0f);
					array3[num3++] = num;
					array3[num3++] = (i + 1) % numSegments;
					array3[num3++] = i;
					if (i >= 2)
					{
						array3[num3++] = numSegments;
						array3[num3++] = numSegments + i - 1;
						array3[num3++] = numSegments + i;
					}
					num5 += num4;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, 0, 0);
				if (PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x040080A1 RID: 32929
		private static int s_lastDrawLineFrame = -1;

		// Token: 0x040080A2 RID: 32930
		private static int s_iPooledMesh = 0;

		// Token: 0x040080A3 RID: 32931
		private static List<Mesh> s_lineMeshPool;

		// Token: 0x040080A4 RID: 32932
		private static Mesh s_boxWireframeMesh;

		// Token: 0x040080A5 RID: 32933
		private static Mesh s_boxSolidColorMesh;

		// Token: 0x040080A6 RID: 32934
		private static Mesh s_boxFlatShadedMesh;

		// Token: 0x040080A7 RID: 32935
		private static Mesh s_rectWireframeMesh;

		// Token: 0x040080A8 RID: 32936
		private static Mesh s_rectSolidColorMesh;

		// Token: 0x040080A9 RID: 32937
		private static Mesh s_rectFlatShadedMesh;

		// Token: 0x040080AA RID: 32938
		private static Dictionary<int, Mesh> s_circleWireframeMeshPool;

		// Token: 0x040080AB RID: 32939
		private static Dictionary<int, Mesh> s_circleSolidColorMeshPool;

		// Token: 0x040080AC RID: 32940
		private static Dictionary<int, Mesh> s_circleFlatShadedMeshPool;

		// Token: 0x040080AD RID: 32941
		private static Dictionary<int, Mesh> s_cylinderWireframeMeshPool;

		// Token: 0x040080AE RID: 32942
		private static Dictionary<int, Mesh> s_cylinderSolidColorMeshPool;

		// Token: 0x040080AF RID: 32943
		private static Dictionary<int, Mesh> s_cylinderFlatShadedMeshPool;

		// Token: 0x040080B0 RID: 32944
		private static Dictionary<int, Mesh> s_cylinderSmoothShadedMeshPool;

		// Token: 0x040080B1 RID: 32945
		private static Dictionary<int, Mesh> s_sphereWireframeMeshPool;

		// Token: 0x040080B2 RID: 32946
		private static Dictionary<int, Mesh> s_sphereSolidColorMeshPool;

		// Token: 0x040080B3 RID: 32947
		private static Dictionary<int, Mesh> s_sphereFlatShadedMeshPool;

		// Token: 0x040080B4 RID: 32948
		private static Dictionary<int, Mesh> s_sphereSmoothShadedMeshPool;

		// Token: 0x040080B5 RID: 32949
		private static Dictionary<int, Mesh> s_capsuleWireframeMeshPool;

		// Token: 0x040080B6 RID: 32950
		private static Dictionary<int, Mesh> s_capsuleSolidColorMeshPool;

		// Token: 0x040080B7 RID: 32951
		private static Dictionary<int, Mesh> s_capsuleFlatShadedMeshPool;

		// Token: 0x040080B8 RID: 32952
		private static Dictionary<int, Mesh> s_capsuleSmoothShadedMeshPool;

		// Token: 0x040080B9 RID: 32953
		private static Dictionary<int, Mesh> s_capsule2dWireframeMeshPool;

		// Token: 0x040080BA RID: 32954
		private static Dictionary<int, Mesh> s_capsule2dSolidColorMeshPool;

		// Token: 0x040080BB RID: 32955
		private static Dictionary<int, Mesh> s_capsule2dFlatShadedMeshPool;

		// Token: 0x040080BC RID: 32956
		private static Dictionary<int, Mesh> s_coneWireframeMeshPool;

		// Token: 0x040080BD RID: 32957
		private static Dictionary<int, Mesh> s_coneSolidColorMeshPool;

		// Token: 0x040080BE RID: 32958
		private static Dictionary<int, Mesh> s_coneFlatShadedMeshPool;

		// Token: 0x040080BF RID: 32959
		private static Dictionary<int, Mesh> s_coneSmoothhadedMeshPool;
	}
}
