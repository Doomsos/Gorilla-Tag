using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200105F RID: 4191
	public static class GTHardCodedBones
	{
		// Token: 0x06006953 RID: 26963 RVA: 0x0022434D File Offset: 0x0022254D
		[RuntimeInitializeOnLoadMethod(1)]
		private static void HandleRuntimeInitialize_OnBeforeSceneLoad()
		{
			VRRigCache.OnPostInitialize += new Action(GTHardCodedBones.HandleVRRigCache_OnPostInitialize);
		}

		// Token: 0x06006954 RID: 26964 RVA: 0x00224360 File Offset: 0x00222560
		private static void HandleVRRigCache_OnPostInitialize()
		{
			VRRigCache.OnPostInitialize -= new Action(GTHardCodedBones.HandleVRRigCache_OnPostInitialize);
			GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig();
			VRRigCache.OnPostSpawnRig += new Action(GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig);
		}

		// Token: 0x06006955 RID: 26965 RVA: 0x00224389 File Offset: 0x00222589
		private static void HandleVRRigCache_OnPostSpawnRig()
		{
			if (VRRigCache.isInitialized)
			{
				bool isQuitting = ApplicationQuittingState.IsQuitting;
			}
		}

		// Token: 0x06006956 RID: 26966 RVA: 0x00071346 File Offset: 0x0006F546
		[MethodImpl(256)]
		public static int GetBoneIndex(GTHardCodedBones.EBone bone)
		{
			return (int)bone;
		}

		// Token: 0x06006957 RID: 26967 RVA: 0x00224398 File Offset: 0x00222598
		[MethodImpl(256)]
		public static int GetBoneIndex(string name)
		{
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06006958 RID: 26968 RVA: 0x002243CC File Offset: 0x002225CC
		[MethodImpl(256)]
		public static bool TryGetBoneIndexByName(string name, out int out_index)
		{
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					out_index = i;
					return true;
				}
			}
			out_index = 0;
			return false;
		}

		// Token: 0x06006959 RID: 26969 RVA: 0x00224403 File Offset: 0x00222603
		[MethodImpl(256)]
		public static GTHardCodedBones.EBone GetBone(string name)
		{
			return (GTHardCodedBones.EBone)GTHardCodedBones.GetBoneIndex(name);
		}

		// Token: 0x0600695A RID: 26970 RVA: 0x0022440C File Offset: 0x0022260C
		[MethodImpl(256)]
		public static bool TryGetBoneByName(string name, out GTHardCodedBones.EBone out_eBone)
		{
			int num;
			if (GTHardCodedBones.TryGetBoneIndexByName(name, out num))
			{
				out_eBone = (GTHardCodedBones.EBone)num;
				return true;
			}
			out_eBone = GTHardCodedBones.EBone.None;
			return false;
		}

		// Token: 0x0600695B RID: 26971 RVA: 0x0022442C File Offset: 0x0022262C
		[MethodImpl(256)]
		public static string GetBoneName(int boneIndex)
		{
			return GTHardCodedBones.kBoneNames[boneIndex];
		}

		// Token: 0x0600695C RID: 26972 RVA: 0x00224435 File Offset: 0x00222635
		[MethodImpl(256)]
		public static bool TryGetBoneName(int boneIndex, out string out_name)
		{
			if (boneIndex >= 0 && boneIndex < GTHardCodedBones.kBoneNames.Length)
			{
				out_name = GTHardCodedBones.kBoneNames[boneIndex];
				return true;
			}
			out_name = "None";
			return false;
		}

		// Token: 0x0600695D RID: 26973 RVA: 0x00224458 File Offset: 0x00222658
		[MethodImpl(256)]
		public static string GetBoneName(GTHardCodedBones.EBone bone)
		{
			return GTHardCodedBones.GetBoneName((int)bone);
		}

		// Token: 0x0600695E RID: 26974 RVA: 0x00224460 File Offset: 0x00222660
		[MethodImpl(256)]
		public static bool TryGetBoneName(GTHardCodedBones.EBone bone, out string out_name)
		{
			return GTHardCodedBones.TryGetBoneName((int)bone, out out_name);
		}

		// Token: 0x0600695F RID: 26975 RVA: 0x0022446C File Offset: 0x0022266C
		[MethodImpl(256)]
		public static long GetBoneBitFlag(string name)
		{
			if (name == "None")
			{
				return 0L;
			}
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					return 1L << i - 1;
				}
			}
			return 0L;
		}

		// Token: 0x06006960 RID: 26976 RVA: 0x002244B6 File Offset: 0x002226B6
		[MethodImpl(256)]
		public static long GetBoneBitFlag(GTHardCodedBones.EBone bone)
		{
			if (bone == GTHardCodedBones.EBone.None)
			{
				return 0L;
			}
			return 1L << bone - GTHardCodedBones.EBone.rig;
		}

		// Token: 0x06006961 RID: 26977 RVA: 0x002244C7 File Offset: 0x002226C7
		[MethodImpl(256)]
		public static EHandedness GetHandednessFromBone(GTHardCodedBones.EBone bone)
		{
			if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1728432283058160L) != 0L)
			{
				return EHandedness.Left;
			}
			if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1769114204897280L) == 0L)
			{
				return EHandedness.None;
			}
			return EHandedness.Right;
		}

		// Token: 0x06006962 RID: 26978 RVA: 0x002244F4 File Offset: 0x002226F4
		public static bool TryGetBoneXforms(VRRig vrRig, out Transform[] outBoneXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (vrRig == null)
			{
				outErrorMsg = "The VRRig is null.";
				outBoneXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = vrRig.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceID, ref outBoneXforms))
			{
				return true;
			}
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out outBoneXforms, out outErrorMsg))
			{
				return false;
			}
			VRRigAnchorOverrides componentInChildren = vrRig.GetComponentInChildren<VRRigAnchorOverrides>(true);
			BodyDockPositions componentInChildren2 = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			outBoneXforms[46] = componentInChildren2.leftBackTransform;
			outBoneXforms[47] = componentInChildren2.rightBackTransform;
			outBoneXforms[42] = componentInChildren2.chestTransform;
			outBoneXforms[43] = componentInChildren.CurrentBadgeTransform;
			outBoneXforms[44] = componentInChildren.nameTransform;
			outBoneXforms[52] = componentInChildren.huntComputer;
			outBoneXforms[50] = componentInChildren.friendshipBraceletLeftAnchor;
			outBoneXforms[51] = componentInChildren.friendshipBraceletRightAnchor;
			GTHardCodedBones._gInstIds_To_boneXforms[instanceID] = outBoneXforms;
			return true;
		}

		// Token: 0x06006963 RID: 26979 RVA: 0x002245C0 File Offset: 0x002227C0
		public static bool TryGetSlotAnchorXforms(VRRig vrRig, out Transform[] outSlotXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (vrRig == null)
			{
				outErrorMsg = "The VRRig is null.";
				outSlotXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = vrRig.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_slotXforms.TryGetValue(instanceID, ref outSlotXforms))
			{
				return true;
			}
			Transform[] array;
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out array, out outErrorMsg))
			{
				return false;
			}
			outSlotXforms = new Transform[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				outSlotXforms[i] = array[i];
			}
			BodyDockPositions componentInChildren = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			outSlotXforms[7] = componentInChildren.leftArmTransform;
			outSlotXforms[25] = componentInChildren.rightArmTransform;
			outSlotXforms[8] = componentInChildren.leftHandTransform;
			outSlotXforms[26] = componentInChildren.rightHandTransform;
			GTHardCodedBones._gInstIds_To_slotXforms[instanceID] = outSlotXforms;
			return true;
		}

		// Token: 0x06006964 RID: 26980 RVA: 0x00224678 File Offset: 0x00222878
		public static bool TryGetBoneXforms(SkinnedMeshRenderer skinnedMeshRenderer, out Transform[] outBoneXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (skinnedMeshRenderer == null)
			{
				outErrorMsg = "The SkinnedMeshRenderer was null.";
				outBoneXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = skinnedMeshRenderer.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceID, ref outBoneXforms))
			{
				return true;
			}
			GTHardCodedBones._gMissingBonesReport.Clear();
			Transform[] bones = skinnedMeshRenderer.bones;
			for (int i = 0; i < bones.Length; i++)
			{
				if (bones[i] == null)
				{
					Debug.LogError(string.Format("this should never happen -- skinned mesh bone index {0} is null in component: ", i) + "\"" + skinnedMeshRenderer.GetComponentPath(int.MaxValue) + "\"", skinnedMeshRenderer);
				}
				else if (bones[i].parent == null)
				{
					Debug.LogError(string.Format("unexpected and unhandled scenario -- skinned mesh bone at index {0} has no parent in ", i) + "component: \"" + skinnedMeshRenderer.GetComponentPath(int.MaxValue) + "\"", skinnedMeshRenderer);
				}
				else
				{
					bones[i] = (bones[i].name.EndsWith("_new") ? bones[i].parent : bones[i]);
				}
			}
			outBoneXforms = new Transform[GTHardCodedBones.kBoneNames.Length];
			for (int j = 1; j < GTHardCodedBones.kBoneNames.Length; j++)
			{
				string text = GTHardCodedBones.kBoneNames[j];
				if (!(text == "None") && !text.EndsWith("_end") && !text.Contains("Anchor") && j != 1)
				{
					bool flag = false;
					foreach (Transform transform in bones)
					{
						if (!(transform == null) && !(transform.name != text))
						{
							outBoneXforms[j] = transform;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						GTHardCodedBones._gMissingBonesReport.Add(j);
					}
				}
			}
			for (int l = 1; l < GTHardCodedBones.kBoneNames.Length; l++)
			{
				string text2 = GTHardCodedBones.kBoneNames[l];
				if (text2.EndsWith("_end"))
				{
					string text3 = text2;
					int boneIndex = GTHardCodedBones.GetBoneIndex(text3.Substring(0, text3.Length - 4));
					if (boneIndex < 0)
					{
						GTHardCodedBones._gMissingBonesReport.Add(l);
					}
					else
					{
						Transform transform2 = outBoneXforms[boneIndex];
						if (transform2 == null)
						{
							GTHardCodedBones._gMissingBonesReport.Add(l);
						}
						else
						{
							Transform transform3 = transform2.Find(text2);
							if (transform3 == null)
							{
								GTHardCodedBones._gMissingBonesReport.Add(l);
							}
							else
							{
								outBoneXforms[l] = transform3;
							}
						}
					}
				}
			}
			Transform transform4 = outBoneXforms[2];
			if (transform4 != null && transform4.parent != null)
			{
				outBoneXforms[1] = transform4.parent;
			}
			else
			{
				GTHardCodedBones._gMissingBonesReport.Add(1);
			}
			for (int m = 1; m < GTHardCodedBones.kBoneNames.Length; m++)
			{
				string text4 = GTHardCodedBones.kBoneNames[m];
				if (text4.Contains("Anchor"))
				{
					Transform transform5;
					if (transform4.TryFindByPath("/**/" + text4, out transform5, false))
					{
						outBoneXforms[m] = transform5;
					}
					else
					{
						GameObject gameObject = new GameObject(text4);
						gameObject.transform.SetParent(transform4, false);
						outBoneXforms[m] = gameObject.transform;
					}
				}
			}
			GTHardCodedBones._gInstIds_To_boneXforms[instanceID] = outBoneXforms;
			if (GTHardCodedBones._gMissingBonesReport.Count == 0)
			{
				return true;
			}
			string text5 = "The SkinnedMeshRenderer on \"" + skinnedMeshRenderer.name + "\" did not have these expected bones: ";
			foreach (int num in GTHardCodedBones._gMissingBonesReport)
			{
				text5 = text5 + "\n- " + GTHardCodedBones.kBoneNames[num];
			}
			outErrorMsg = text5;
			return true;
		}

		// Token: 0x06006965 RID: 26981 RVA: 0x00224A18 File Offset: 0x00222C18
		[MethodImpl(256)]
		public static bool TryGetBoneXform(Transform[] boneXforms, string boneName, out Transform boneXform)
		{
			boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(boneName)];
			return boneXform != null;
		}

		// Token: 0x06006966 RID: 26982 RVA: 0x00224A2C File Offset: 0x00222C2C
		[MethodImpl(256)]
		public static bool TryGetBoneXform(Transform[] boneXforms, GTHardCodedBones.EBone eBone, out Transform boneXform)
		{
			boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(eBone)];
			return boneXform != null;
		}

		// Token: 0x06006967 RID: 26983 RVA: 0x00224A40 File Offset: 0x00222C40
		[MethodImpl(256)]
		public static bool TryGetFirstBoneInParents(Transform transform, out GTHardCodedBones.EBone eBone, out Transform boneXform)
		{
			while (transform != null)
			{
				string name = transform.name;
				if (name == "DropZoneAnchor" && transform.parent != null)
				{
					string name2 = transform.parent.name;
					if (name2 == "Slingshot Chest Snap")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemLeftArm")
					{
						eBone = GTHardCodedBones.EBone.forearm_L;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemLeftShoulder")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemRightShoulder")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
						boneXform = transform;
						return true;
					}
				}
				else
				{
					if (name == "TransferrableItemLeftHand")
					{
						eBone = GTHardCodedBones.EBone.hand_L;
						boneXform = transform;
						return true;
					}
					if (name == "TransferrableItemRightHand")
					{
						eBone = GTHardCodedBones.EBone.hand_R;
						boneXform = transform;
						return true;
					}
				}
				GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(transform.name);
				if (bone != GTHardCodedBones.EBone.None)
				{
					eBone = bone;
					boneXform = transform;
					return true;
				}
				transform = transform.parent;
			}
			eBone = GTHardCodedBones.EBone.None;
			boneXform = null;
			return false;
		}

		// Token: 0x06006968 RID: 26984 RVA: 0x00224B30 File Offset: 0x00222D30
		[MethodImpl(256)]
		public static GTHardCodedBones.EBone GetBoneEnumOfCosmeticPosStateFlag(TransferrableObject.PositionState positionState)
		{
			if (positionState <= TransferrableObject.PositionState.OnChest)
			{
				switch (positionState)
				{
				case TransferrableObject.PositionState.None:
					break;
				case TransferrableObject.PositionState.OnLeftArm:
					return GTHardCodedBones.EBone.forearm_L;
				case TransferrableObject.PositionState.OnRightArm:
					return GTHardCodedBones.EBone.forearm_R;
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.InLeftHand:
				case TransferrableObject.PositionState.OnRightArm | TransferrableObject.PositionState.InLeftHand:
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm | TransferrableObject.PositionState.InLeftHand:
					goto IL_5F;
				case TransferrableObject.PositionState.InLeftHand:
					return GTHardCodedBones.EBone.hand_L;
				case TransferrableObject.PositionState.InRightHand:
					return GTHardCodedBones.EBone.hand_R;
				default:
					if (positionState != TransferrableObject.PositionState.OnChest)
					{
						goto IL_5F;
					}
					return GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
				}
			}
			else
			{
				if (positionState == TransferrableObject.PositionState.OnLeftShoulder)
				{
					return GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
				}
				if (positionState == TransferrableObject.PositionState.OnRightShoulder)
				{
					return GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
				}
				if (positionState != TransferrableObject.PositionState.Dropped)
				{
					goto IL_5F;
				}
			}
			return GTHardCodedBones.EBone.None;
			IL_5F:
			throw new ArgumentOutOfRangeException(positionState.ToString());
		}

		// Token: 0x06006969 RID: 26985 RVA: 0x00224BB0 File Offset: 0x00222DB0
		[MethodImpl(256)]
		public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticBodyDockDropPosFlags(BodyDockPositions.DropPositions enumFlags)
		{
			BodyDockPositions.DropPositions[] values = EnumData<BodyDockPositions.DropPositions>.Shared.Values;
			List<GTHardCodedBones.EBone> list = new List<GTHardCodedBones.EBone>(32);
			foreach (BodyDockPositions.DropPositions dropPositions in values)
			{
				if (dropPositions != BodyDockPositions.DropPositions.All && dropPositions != BodyDockPositions.DropPositions.None && dropPositions != BodyDockPositions.DropPositions.MaxDropPostions && (enumFlags & dropPositions) != BodyDockPositions.DropPositions.None)
				{
					list.Add(GTHardCodedBones._k_bodyDockDropPosition_to_eBone[dropPositions]);
				}
			}
			return list;
		}

		// Token: 0x0600696A RID: 26986 RVA: 0x00224C08 File Offset: 0x00222E08
		[MethodImpl(256)]
		public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticTransferrablePosStateFlags(TransferrableObject.PositionState enumFlags)
		{
			TransferrableObject.PositionState[] values = EnumData<TransferrableObject.PositionState>.Shared.Values;
			List<GTHardCodedBones.EBone> list = new List<GTHardCodedBones.EBone>(32);
			foreach (TransferrableObject.PositionState positionState in values)
			{
				if (positionState != TransferrableObject.PositionState.None && positionState != TransferrableObject.PositionState.Dropped && (enumFlags & positionState) != TransferrableObject.PositionState.None)
				{
					list.Add(GTHardCodedBones._k_transferrablePosState_to_eBone[positionState]);
				}
			}
			return list;
		}

		// Token: 0x0600696B RID: 26987 RVA: 0x00224C5C File Offset: 0x00222E5C
		[MethodImpl(256)]
		public static bool TryGetTransferrablePosStateFromBoneEnum(GTHardCodedBones.EBone eBone, out TransferrableObject.PositionState outPosState)
		{
			return GTHardCodedBones._k_eBone_to_transferrablePosState.TryGetValue(eBone, ref outPosState);
		}

		// Token: 0x0600696C RID: 26988 RVA: 0x00224C6C File Offset: 0x00222E6C
		[MethodImpl(256)]
		public static Transform GetBoneXformOfCosmeticPosStateFlag(TransferrableObject.PositionState anchorPosState, Transform[] bones)
		{
			if (bones.Length != 53)
			{
				throw new Exception(string.Format("{0}: Supplied bones array length is {1} but requires ", "GTHardCodedBones", bones.Length) + string.Format("{0}.", 53));
			}
			int boneIndex = GTHardCodedBones.GetBoneIndex(GTHardCodedBones.GetBoneEnumOfCosmeticPosStateFlag(anchorPosState));
			if (boneIndex != -1)
			{
				return bones[boneIndex];
			}
			return null;
		}

		// Token: 0x0600696D RID: 26989 RVA: 0x00224CC8 File Offset: 0x00222EC8
		// Note: this type is marked as 'beforefieldinit'.
		static GTHardCodedBones()
		{
			Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone> dictionary = new Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone>();
			dictionary.Add(BodyDockPositions.DropPositions.None, GTHardCodedBones.EBone.None);
			dictionary.Add(BodyDockPositions.DropPositions.LeftArm, GTHardCodedBones.EBone.forearm_L);
			dictionary.Add(BodyDockPositions.DropPositions.RightArm, GTHardCodedBones.EBone.forearm_R);
			dictionary.Add(BodyDockPositions.DropPositions.Chest, GTHardCodedBones.EBone.body_AnchorFront_StowSlot);
			dictionary.Add(BodyDockPositions.DropPositions.LeftBack, GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot);
			dictionary.Add(BodyDockPositions.DropPositions.RightBack, GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot);
			GTHardCodedBones._k_bodyDockDropPosition_to_eBone = dictionary;
			Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone> dictionary2 = new Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone>();
			dictionary2.Add(TransferrableObject.PositionState.None, GTHardCodedBones.EBone.None);
			dictionary2.Add(TransferrableObject.PositionState.OnLeftArm, GTHardCodedBones.EBone.forearm_L);
			dictionary2.Add(TransferrableObject.PositionState.OnRightArm, GTHardCodedBones.EBone.forearm_R);
			dictionary2.Add(TransferrableObject.PositionState.InLeftHand, GTHardCodedBones.EBone.hand_L);
			dictionary2.Add(TransferrableObject.PositionState.InRightHand, GTHardCodedBones.EBone.hand_R);
			dictionary2.Add(TransferrableObject.PositionState.OnChest, GTHardCodedBones.EBone.body_AnchorFront_StowSlot);
			dictionary2.Add(TransferrableObject.PositionState.OnLeftShoulder, GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot);
			dictionary2.Add(TransferrableObject.PositionState.OnRightShoulder, GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot);
			dictionary2.Add(TransferrableObject.PositionState.Dropped, GTHardCodedBones.EBone.None);
			GTHardCodedBones._k_transferrablePosState_to_eBone = dictionary2;
			Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState> dictionary3 = new Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState>();
			dictionary3.Add(GTHardCodedBones.EBone.None, TransferrableObject.PositionState.None);
			dictionary3.Add(GTHardCodedBones.EBone.forearm_L, TransferrableObject.PositionState.OnLeftArm);
			dictionary3.Add(GTHardCodedBones.EBone.forearm_R, TransferrableObject.PositionState.OnRightArm);
			dictionary3.Add(GTHardCodedBones.EBone.hand_L, TransferrableObject.PositionState.InLeftHand);
			dictionary3.Add(GTHardCodedBones.EBone.hand_R, TransferrableObject.PositionState.InRightHand);
			dictionary3.Add(GTHardCodedBones.EBone.body_AnchorFront_StowSlot, TransferrableObject.PositionState.OnChest);
			dictionary3.Add(GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot, TransferrableObject.PositionState.OnLeftShoulder);
			dictionary3.Add(GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot, TransferrableObject.PositionState.OnRightShoulder);
			GTHardCodedBones._k_eBone_to_transferrablePosState = dictionary3;
			GTHardCodedBones._gMissingBonesReport = new List<int>(53);
			GTHardCodedBones._gInstIds_To_boneXforms = new Dictionary<int, Transform[]>(20);
			GTHardCodedBones._gInstIds_To_slotXforms = new Dictionary<int, Transform[]>(20);
		}

		// Token: 0x04007841 RID: 30785
		public const int kBoneCount = 53;

		// Token: 0x04007842 RID: 30786
		public static readonly string[] kBoneNames = new string[]
		{
			"None",
			"rig",
			"body",
			"head",
			"head_end",
			"shoulder.L",
			"upper_arm.L",
			"forearm.L",
			"hand.L",
			"palm.01.L",
			"palm.02.L",
			"thumb.01.L",
			"thumb.02.L",
			"thumb.03.L",
			"thumb.03.L_end",
			"f_index.01.L",
			"f_index.02.L",
			"f_index.03.L",
			"f_index.03.L_end",
			"f_middle.01.L",
			"f_middle.02.L",
			"f_middle.03.L",
			"f_middle.03.L_end",
			"shoulder.R",
			"upper_arm.R",
			"forearm.R",
			"hand.R",
			"palm.01.R",
			"palm.02.R",
			"thumb.01.R",
			"thumb.02.R",
			"thumb.03.R",
			"thumb.03.R_end",
			"f_index.01.R",
			"f_index.02.R",
			"f_index.03.R",
			"f_index.03.R_end",
			"f_middle.01.R",
			"f_middle.02.R",
			"f_middle.03.R",
			"f_middle.03.R_end",
			"body_AnchorTop_Neck",
			"body_AnchorFront_StowSlot",
			"body_AnchorFrontLeft_Badge",
			"body_AnchorFrontRight_NameTag",
			"body_AnchorBack",
			"body_AnchorBackLeft_StowSlot",
			"body_AnchorBackRight_StowSlot",
			"body_AnchorBottom",
			"body_AnchorBackBottom_Tail",
			"hand_L_AnchorBack",
			"hand_R_AnchorBack",
			"hand_L_AnchorFront_GameModeItemSlot"
		};

		// Token: 0x04007843 RID: 30787
		private const long kLeftSideMask = 1728432283058160L;

		// Token: 0x04007844 RID: 30788
		private const long kRightSideMask = 1769114204897280L;

		// Token: 0x04007845 RID: 30789
		private static readonly Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone> _k_bodyDockDropPosition_to_eBone;

		// Token: 0x04007846 RID: 30790
		private static readonly Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone> _k_transferrablePosState_to_eBone;

		// Token: 0x04007847 RID: 30791
		private static readonly Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState> _k_eBone_to_transferrablePosState;

		// Token: 0x04007848 RID: 30792
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly List<int> _gMissingBonesReport;

		// Token: 0x04007849 RID: 30793
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly Dictionary<int, Transform[]> _gInstIds_To_boneXforms;

		// Token: 0x0400784A RID: 30794
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly Dictionary<int, Transform[]> _gInstIds_To_slotXforms;

		// Token: 0x02001060 RID: 4192
		public enum EBone
		{
			// Token: 0x0400784C RID: 30796
			None,
			// Token: 0x0400784D RID: 30797
			rig,
			// Token: 0x0400784E RID: 30798
			body,
			// Token: 0x0400784F RID: 30799
			head,
			// Token: 0x04007850 RID: 30800
			head_end,
			// Token: 0x04007851 RID: 30801
			shoulder_L,
			// Token: 0x04007852 RID: 30802
			upper_arm_L,
			// Token: 0x04007853 RID: 30803
			forearm_L,
			// Token: 0x04007854 RID: 30804
			hand_L,
			// Token: 0x04007855 RID: 30805
			palm_01_L,
			// Token: 0x04007856 RID: 30806
			palm_02_L,
			// Token: 0x04007857 RID: 30807
			thumb_01_L,
			// Token: 0x04007858 RID: 30808
			thumb_02_L,
			// Token: 0x04007859 RID: 30809
			thumb_03_L,
			// Token: 0x0400785A RID: 30810
			thumb_03_L_end,
			// Token: 0x0400785B RID: 30811
			f_index_01_L,
			// Token: 0x0400785C RID: 30812
			f_index_02_L,
			// Token: 0x0400785D RID: 30813
			f_index_03_L,
			// Token: 0x0400785E RID: 30814
			f_index_03_L_end,
			// Token: 0x0400785F RID: 30815
			f_middle_01_L,
			// Token: 0x04007860 RID: 30816
			f_middle_02_L,
			// Token: 0x04007861 RID: 30817
			f_middle_03_L,
			// Token: 0x04007862 RID: 30818
			f_middle_03_L_end,
			// Token: 0x04007863 RID: 30819
			shoulder_R,
			// Token: 0x04007864 RID: 30820
			upper_arm_R,
			// Token: 0x04007865 RID: 30821
			forearm_R,
			// Token: 0x04007866 RID: 30822
			hand_R,
			// Token: 0x04007867 RID: 30823
			palm_01_R,
			// Token: 0x04007868 RID: 30824
			palm_02_R,
			// Token: 0x04007869 RID: 30825
			thumb_01_R,
			// Token: 0x0400786A RID: 30826
			thumb_02_R,
			// Token: 0x0400786B RID: 30827
			thumb_03_R,
			// Token: 0x0400786C RID: 30828
			thumb_03_R_end,
			// Token: 0x0400786D RID: 30829
			f_index_01_R,
			// Token: 0x0400786E RID: 30830
			f_index_02_R,
			// Token: 0x0400786F RID: 30831
			f_index_03_R,
			// Token: 0x04007870 RID: 30832
			f_index_03_R_end,
			// Token: 0x04007871 RID: 30833
			f_middle_01_R,
			// Token: 0x04007872 RID: 30834
			f_middle_02_R,
			// Token: 0x04007873 RID: 30835
			f_middle_03_R,
			// Token: 0x04007874 RID: 30836
			f_middle_03_R_end,
			// Token: 0x04007875 RID: 30837
			body_AnchorTop_Neck,
			// Token: 0x04007876 RID: 30838
			body_AnchorFront_StowSlot,
			// Token: 0x04007877 RID: 30839
			body_AnchorFrontLeft_Badge,
			// Token: 0x04007878 RID: 30840
			body_AnchorFrontRight_NameTag,
			// Token: 0x04007879 RID: 30841
			body_AnchorBack,
			// Token: 0x0400787A RID: 30842
			body_AnchorBackLeft_StowSlot,
			// Token: 0x0400787B RID: 30843
			body_AnchorBackRight_StowSlot,
			// Token: 0x0400787C RID: 30844
			body_AnchorBottom,
			// Token: 0x0400787D RID: 30845
			body_AnchorBackBottom_Tail,
			// Token: 0x0400787E RID: 30846
			hand_L_AnchorBack,
			// Token: 0x0400787F RID: 30847
			hand_R_AnchorBack,
			// Token: 0x04007880 RID: 30848
			hand_L_AnchorFront_GameModeItemSlot
		}

		// Token: 0x02001061 RID: 4193
		public enum EStowSlots
		{
			// Token: 0x04007882 RID: 30850
			None,
			// Token: 0x04007883 RID: 30851
			forearm_L = 7,
			// Token: 0x04007884 RID: 30852
			forearm_R = 25,
			// Token: 0x04007885 RID: 30853
			body_AnchorFront_Chest = 42,
			// Token: 0x04007886 RID: 30854
			body_AnchorBackLeft = 46,
			// Token: 0x04007887 RID: 30855
			body_AnchorBackRight
		}

		// Token: 0x02001062 RID: 4194
		public enum EHandAndStowSlots
		{
			// Token: 0x04007889 RID: 30857
			None,
			// Token: 0x0400788A RID: 30858
			forearm_L = 7,
			// Token: 0x0400788B RID: 30859
			hand_L,
			// Token: 0x0400788C RID: 30860
			forearm_R = 25,
			// Token: 0x0400788D RID: 30861
			hand_R,
			// Token: 0x0400788E RID: 30862
			body_AnchorFront_Chest = 42,
			// Token: 0x0400788F RID: 30863
			body_AnchorBackLeft = 46,
			// Token: 0x04007890 RID: 30864
			body_AnchorBackRight
		}

		// Token: 0x02001063 RID: 4195
		public enum ECosmeticSlots
		{
			// Token: 0x04007892 RID: 30866
			Hat = 4,
			// Token: 0x04007893 RID: 30867
			Badge = 43,
			// Token: 0x04007894 RID: 30868
			Face = 3,
			// Token: 0x04007895 RID: 30869
			ArmLeft = 6,
			// Token: 0x04007896 RID: 30870
			ArmRight = 24,
			// Token: 0x04007897 RID: 30871
			BackLeft = 46,
			// Token: 0x04007898 RID: 30872
			BackRight,
			// Token: 0x04007899 RID: 30873
			HandLeft = 8,
			// Token: 0x0400789A RID: 30874
			HandRight = 26,
			// Token: 0x0400789B RID: 30875
			Chest = 42,
			// Token: 0x0400789C RID: 30876
			Fur = 1,
			// Token: 0x0400789D RID: 30877
			Shirt,
			// Token: 0x0400789E RID: 30878
			Pants = 48,
			// Token: 0x0400789F RID: 30879
			Back = 45,
			// Token: 0x040078A0 RID: 30880
			Arms = 2,
			// Token: 0x040078A1 RID: 30881
			TagEffect = 0
		}

		// Token: 0x02001064 RID: 4196
		[Serializable]
		public struct SturdyEBone : ISerializationCallbackReceiver
		{
			// Token: 0x170009EE RID: 2542
			// (get) Token: 0x0600696E RID: 26990 RVA: 0x00224FC8 File Offset: 0x002231C8
			// (set) Token: 0x0600696F RID: 26991 RVA: 0x00224FD0 File Offset: 0x002231D0
			public GTHardCodedBones.EBone Bone
			{
				get
				{
					return this._bone;
				}
				set
				{
					this._bone = value;
					this._boneName = GTHardCodedBones.GetBoneName(this._bone);
				}
			}

			// Token: 0x06006970 RID: 26992 RVA: 0x00224FEA File Offset: 0x002231EA
			public SturdyEBone(GTHardCodedBones.EBone bone)
			{
				this._bone = bone;
				this._boneName = null;
			}

			// Token: 0x06006971 RID: 26993 RVA: 0x00224FFA File Offset: 0x002231FA
			public SturdyEBone(string boneName)
			{
				this._bone = GTHardCodedBones.GetBone(boneName);
				this._boneName = null;
			}

			// Token: 0x06006972 RID: 26994 RVA: 0x0022500F File Offset: 0x0022320F
			public static implicit operator GTHardCodedBones.EBone(GTHardCodedBones.SturdyEBone sturdyBone)
			{
				return sturdyBone.Bone;
			}

			// Token: 0x06006973 RID: 26995 RVA: 0x00225018 File Offset: 0x00223218
			public static implicit operator GTHardCodedBones.SturdyEBone(GTHardCodedBones.EBone bone)
			{
				return new GTHardCodedBones.SturdyEBone(bone);
			}

			// Token: 0x06006974 RID: 26996 RVA: 0x0022500F File Offset: 0x0022320F
			public static explicit operator int(GTHardCodedBones.SturdyEBone sturdyBone)
			{
				return (int)sturdyBone.Bone;
			}

			// Token: 0x06006975 RID: 26997 RVA: 0x00225020 File Offset: 0x00223220
			public override string ToString()
			{
				return this._boneName;
			}

			// Token: 0x06006976 RID: 26998 RVA: 0x00002789 File Offset: 0x00000989
			void ISerializationCallbackReceiver.OnBeforeSerialize()
			{
			}

			// Token: 0x06006977 RID: 26999 RVA: 0x00225028 File Offset: 0x00223228
			void ISerializationCallbackReceiver.OnAfterDeserialize()
			{
				if (string.IsNullOrEmpty(this._boneName))
				{
					this._bone = GTHardCodedBones.EBone.None;
					this._boneName = "None";
					return;
				}
				GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(this._boneName);
				if (bone != GTHardCodedBones.EBone.None)
				{
					this._bone = bone;
				}
			}

			// Token: 0x040078A2 RID: 30882
			[SerializeField]
			private GTHardCodedBones.EBone _bone;

			// Token: 0x040078A3 RID: 30883
			[SerializeField]
			private string _boneName;
		}
	}
}
