using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000F71 RID: 3953
	[CreateAssetMenu(fileName = "New Tag Effect Pack", menuName = "Tag Effect Pack")]
	public class TagEffectPack : ScriptableObject
	{
		// Token: 0x04007192 RID: 29074
		public GameObject thirdPerson;

		// Token: 0x04007193 RID: 29075
		public bool thirdPersonParentEffect = true;

		// Token: 0x04007194 RID: 29076
		public GameObject firstPerson;

		// Token: 0x04007195 RID: 29077
		public bool firstPersonParentEffect = true;

		// Token: 0x04007196 RID: 29078
		public GameObject highFive;

		// Token: 0x04007197 RID: 29079
		public bool highFiveParentEffect;

		// Token: 0x04007198 RID: 29080
		public GameObject fistBump;

		// Token: 0x04007199 RID: 29081
		public bool fistBumpParentEffect;

		// Token: 0x0400719A RID: 29082
		public bool shouldFaceTagger;
	}
}
