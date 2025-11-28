using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FE1 RID: 4065
	[CreateAssetMenu(fileName = "MaterialDatasSO", menuName = "Gorilla Tag/MaterialDatasSO")]
	public class MaterialDatasSO : ScriptableObject
	{
		// Token: 0x04007563 RID: 30051
		public List<GTPlayer.MaterialData> datas;

		// Token: 0x04007564 RID: 30052
		public List<HashWrapper> surfaceEffects;
	}
}
