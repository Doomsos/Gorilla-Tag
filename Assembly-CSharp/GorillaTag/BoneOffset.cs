using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FCE RID: 4046
	[Serializable]
	public struct BoneOffset
	{
		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x06006693 RID: 26259 RVA: 0x00216CE4 File Offset: 0x00214EE4
		public Vector3 pos
		{
			get
			{
				return this.offset.pos;
			}
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x06006694 RID: 26260 RVA: 0x00216CF1 File Offset: 0x00214EF1
		public Quaternion rot
		{
			get
			{
				return this.offset.rot;
			}
		}

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x06006695 RID: 26261 RVA: 0x00216CFE File Offset: 0x00214EFE
		public Vector3 scale
		{
			get
			{
				return this.offset.scale;
			}
		}

		// Token: 0x06006696 RID: 26262 RVA: 0x00216D0B File Offset: 0x00214F0B
		public BoneOffset(GTHardCodedBones.EBone bone)
		{
			this.bone = bone;
			this.offset = XformOffset.Identity;
		}

		// Token: 0x06006697 RID: 26263 RVA: 0x00216D24 File Offset: 0x00214F24
		public BoneOffset(GTHardCodedBones.EBone bone, XformOffset offset)
		{
			this.bone = bone;
			this.offset = offset;
		}

		// Token: 0x06006698 RID: 26264 RVA: 0x00216D39 File Offset: 0x00214F39
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot);
		}

		// Token: 0x06006699 RID: 26265 RVA: 0x00216D54 File Offset: 0x00214F54
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles);
		}

		// Token: 0x0600669A RID: 26266 RVA: 0x00216D6F File Offset: 0x00214F6F
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot, scale);
		}

		// Token: 0x0600669B RID: 26267 RVA: 0x00216D8C File Offset: 0x00214F8C
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles, scale);
		}

		// Token: 0x0400754E RID: 30030
		public GTHardCodedBones.SturdyEBone bone;

		// Token: 0x0400754F RID: 30031
		public XformOffset offset;

		// Token: 0x04007550 RID: 30032
		public static readonly BoneOffset Identity = new BoneOffset
		{
			bone = GTHardCodedBones.EBone.None,
			offset = XformOffset.Identity
		};
	}
}
