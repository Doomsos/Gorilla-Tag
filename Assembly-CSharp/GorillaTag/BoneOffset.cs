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
		// (get) Token: 0x06006693 RID: 26259 RVA: 0x00216CC4 File Offset: 0x00214EC4
		public Vector3 pos
		{
			get
			{
				return this.offset.pos;
			}
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x06006694 RID: 26260 RVA: 0x00216CD1 File Offset: 0x00214ED1
		public Quaternion rot
		{
			get
			{
				return this.offset.rot;
			}
		}

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x06006695 RID: 26261 RVA: 0x00216CDE File Offset: 0x00214EDE
		public Vector3 scale
		{
			get
			{
				return this.offset.scale;
			}
		}

		// Token: 0x06006696 RID: 26262 RVA: 0x00216CEB File Offset: 0x00214EEB
		public BoneOffset(GTHardCodedBones.EBone bone)
		{
			this.bone = bone;
			this.offset = XformOffset.Identity;
		}

		// Token: 0x06006697 RID: 26263 RVA: 0x00216D04 File Offset: 0x00214F04
		public BoneOffset(GTHardCodedBones.EBone bone, XformOffset offset)
		{
			this.bone = bone;
			this.offset = offset;
		}

		// Token: 0x06006698 RID: 26264 RVA: 0x00216D19 File Offset: 0x00214F19
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot);
		}

		// Token: 0x06006699 RID: 26265 RVA: 0x00216D34 File Offset: 0x00214F34
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles);
		}

		// Token: 0x0600669A RID: 26266 RVA: 0x00216D4F File Offset: 0x00214F4F
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot, scale);
		}

		// Token: 0x0600669B RID: 26267 RVA: 0x00216D6C File Offset: 0x00214F6C
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
