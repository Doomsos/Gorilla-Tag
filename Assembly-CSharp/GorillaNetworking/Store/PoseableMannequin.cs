using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F4B RID: 3915
	public class PoseableMannequin : MonoBehaviour
	{
		// Token: 0x06006213 RID: 25107 RVA: 0x001F9AC5 File Offset: 0x001F7CC5
		public void Start()
		{
			this.skinnedMeshRenderer.gameObject.SetActive(false);
			this.staticGorillaMesh.gameObject.SetActive(true);
		}

		// Token: 0x06006214 RID: 25108 RVA: 0x00147A6B File Offset: 0x00145C6B
		private string GetPrefabPathFromCurrentPrefabStage()
		{
			return "";
		}

		// Token: 0x06006215 RID: 25109 RVA: 0x00147A6B File Offset: 0x00145C6B
		private string GetMeshPathFromPrefabPath(string prefabPath)
		{
			return "";
		}

		// Token: 0x06006216 RID: 25110 RVA: 0x001F9AE9 File Offset: 0x001F7CE9
		public void BakeSkinnedMesh()
		{
			this.BakeAndSaveMeshInPath(this.GetMeshPathFromPrefabPath(this.GetPrefabPathFromCurrentPrefabStage()));
		}

		// Token: 0x06006217 RID: 25111 RVA: 0x00002789 File Offset: 0x00000989
		public void BakeAndSaveMeshInPath(string meshPath)
		{
		}

		// Token: 0x06006218 RID: 25112 RVA: 0x001F9AFD File Offset: 0x001F7CFD
		private void UpdateStaticMeshMannequin()
		{
			this.staticGorillaMesh.sharedMesh = this.BakedColliderMesh;
			this.staticGorillaMeshRenderer.sharedMaterials = this.skinnedMeshRenderer.sharedMaterials;
			this.staticGorillaMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x06006219 RID: 25113 RVA: 0x001F9B37 File Offset: 0x001F7D37
		private void UpdateSkinnedMeshCollider()
		{
			this.skinnedMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x0600621A RID: 25114 RVA: 0x001F9B4C File Offset: 0x001F7D4C
		public void UpdateGTPosRotConstraints()
		{
			GTPosRotConstraints[] array = this.cosmeticConstraints;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].constraints.ForEach(delegate(GorillaPosRotConstraint c)
				{
					c.follower.rotation = c.source.rotation;
					c.follower.position = c.source.position;
				});
			}
		}

		// Token: 0x0600621B RID: 25115 RVA: 0x001F9B9C File Offset: 0x001F7D9C
		private void HookupCosmeticConstraints()
		{
			this.cosmeticConstraints = base.GetComponentsInChildren<GTPosRotConstraints>();
			foreach (GTPosRotConstraints gtposRotConstraints in this.cosmeticConstraints)
			{
				for (int j = 0; j < gtposRotConstraints.constraints.Length; j++)
				{
					gtposRotConstraints.constraints[j].source = this.FindBone(gtposRotConstraints.constraints[j].follower.name);
				}
			}
		}

		// Token: 0x0600621C RID: 25116 RVA: 0x001F9C10 File Offset: 0x001F7E10
		private Transform FindBone(string boneName)
		{
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == boneName)
				{
					return transform;
				}
			}
			return null;
		}

		// Token: 0x0600621D RID: 25117 RVA: 0x00002789 File Offset: 0x00000989
		public void CreasteTestClip()
		{
		}

		// Token: 0x0600621E RID: 25118 RVA: 0x001F9C4C File Offset: 0x001F7E4C
		public void SerializeVRRig()
		{
			base.StartCoroutine(this.SaveLocalPlayerPose());
		}

		// Token: 0x0600621F RID: 25119 RVA: 0x001F9C5B File Offset: 0x001F7E5B
		public IEnumerator SaveLocalPlayerPose()
		{
			yield return null;
			yield break;
		}

		// Token: 0x06006220 RID: 25120 RVA: 0x00002789 File Offset: 0x00000989
		public void SerializeOutBonesFromSkinnedMesh(SkinnedMeshRenderer paramSkinnedMeshRenderer)
		{
		}

		// Token: 0x06006221 RID: 25121 RVA: 0x001F9C64 File Offset: 0x001F7E64
		public void SetCurvesForBone(SkinnedMeshRenderer paramSkinnedMeshRenderer, AnimationClip clip, Transform bone)
		{
			Keyframe[] array = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.x)
			};
			Keyframe[] array2 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.y)
			};
			Keyframe[] array3 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.z)
			};
			Keyframe[] array4 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.w)
			};
			AnimationCurve animationCurve = new AnimationCurve(array);
			AnimationCurve animationCurve2 = new AnimationCurve(array2);
			AnimationCurve animationCurve3 = new AnimationCurve(array3);
			AnimationCurve animationCurve4 = new AnimationCurve(array4);
			string text = "";
			string text2 = bone.name.Replace("_new", "");
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == text2)
				{
					text = transform.GetPath(this.skinnedMeshRenderer.transform.parent).TrimStart('/');
					break;
				}
			}
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.x", animationCurve);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.y", animationCurve2);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.z", animationCurve3);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.w", animationCurve4);
		}

		// Token: 0x06006222 RID: 25122 RVA: 0x00002789 File Offset: 0x00000989
		public void UpdatePrefabWithAnimationClip(string AnimationFileName)
		{
		}

		// Token: 0x06006223 RID: 25123 RVA: 0x00002789 File Offset: 0x00000989
		public void LoadPoseOntoMannequin(AnimationClip clip, float frameTime = 0f)
		{
		}

		// Token: 0x06006224 RID: 25124 RVA: 0x00002789 File Offset: 0x00000989
		public void OnValidate()
		{
		}

		// Token: 0x040070C2 RID: 28866
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x040070C3 RID: 28867
		[FormerlySerializedAs("meshCollider")]
		public MeshCollider skinnedMeshCollider;

		// Token: 0x040070C4 RID: 28868
		public GTPosRotConstraints[] cosmeticConstraints;

		// Token: 0x040070C5 RID: 28869
		public Mesh BakedColliderMesh;

		// Token: 0x040070C6 RID: 28870
		[SerializeField]
		[FormerlySerializedAs("liveAssetPath")]
		protected string prefabAssetPath;

		// Token: 0x040070C7 RID: 28871
		[SerializeField]
		protected string prefabFolderPath;

		// Token: 0x040070C8 RID: 28872
		[SerializeField]
		protected string prefabAssetName;

		// Token: 0x040070C9 RID: 28873
		public MeshFilter staticGorillaMesh;

		// Token: 0x040070CA RID: 28874
		public MeshCollider staticGorillaMeshCollider;

		// Token: 0x040070CB RID: 28875
		public MeshRenderer staticGorillaMeshRenderer;
	}
}
