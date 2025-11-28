using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DE8 RID: 3560
	[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
	public class GorillaCaveCrystalSetup : ScriptableObject
	{
		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x060058B9 RID: 22713 RVA: 0x001C6983 File Offset: 0x001C4B83
		public static GorillaCaveCrystalSetup Instance
		{
			get
			{
				return GorillaCaveCrystalSetup.gInstance;
			}
		}

		// Token: 0x060058BA RID: 22714 RVA: 0x001C698A File Offset: 0x001C4B8A
		private void OnEnable()
		{
			if (GorillaCaveCrystalSetup.gInstance == null)
			{
				GorillaCaveCrystalSetup.gInstance = this;
			}
		}

		// Token: 0x060058BB RID: 22715 RVA: 0x001C69A0 File Offset: 0x001C4BA0
		public GorillaCaveCrystalSetup.CrystalDef[] GetCrystalDefs()
		{
			return Enumerable.ToArray<GorillaCaveCrystalSetup.CrystalDef>(Enumerable.Select<FieldInfo, GorillaCaveCrystalSetup.CrystalDef>(Enumerable.Where<FieldInfo>(RuntimeReflectionExtensions.GetRuntimeFields(typeof(GorillaCaveCrystalSetup)), (FieldInfo f) => f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef)), (FieldInfo f) => (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this)));
		}

		// Token: 0x040065E4 RID: 26084
		public Material SharedBase;

		// Token: 0x040065E5 RID: 26085
		public Texture2D CrystalAlbedo;

		// Token: 0x040065E6 RID: 26086
		public Texture2D CrystalDarkAlbedo;

		// Token: 0x040065E7 RID: 26087
		public GorillaCaveCrystalSetup.CrystalDef Red;

		// Token: 0x040065E8 RID: 26088
		public GorillaCaveCrystalSetup.CrystalDef Orange;

		// Token: 0x040065E9 RID: 26089
		public GorillaCaveCrystalSetup.CrystalDef Yellow;

		// Token: 0x040065EA RID: 26090
		public GorillaCaveCrystalSetup.CrystalDef Green;

		// Token: 0x040065EB RID: 26091
		public GorillaCaveCrystalSetup.CrystalDef Teal;

		// Token: 0x040065EC RID: 26092
		public GorillaCaveCrystalSetup.CrystalDef DarkBlue;

		// Token: 0x040065ED RID: 26093
		public GorillaCaveCrystalSetup.CrystalDef Pink;

		// Token: 0x040065EE RID: 26094
		public GorillaCaveCrystalSetup.CrystalDef Dark;

		// Token: 0x040065EF RID: 26095
		public GorillaCaveCrystalSetup.CrystalDef DarkLight;

		// Token: 0x040065F0 RID: 26096
		public GorillaCaveCrystalSetup.CrystalDef DarkLightUnderWater;

		// Token: 0x040065F1 RID: 26097
		[SerializeField]
		[TextArea(4, 10)]
		private string _notes;

		// Token: 0x040065F2 RID: 26098
		[Space]
		[SerializeField]
		private GameObject _target;

		// Token: 0x040065F3 RID: 26099
		private static GorillaCaveCrystalSetup gInstance;

		// Token: 0x040065F4 RID: 26100
		private static GorillaCaveCrystalSetup.CrystalDef[] gCrystalDefs;

		// Token: 0x02000DE9 RID: 3561
		[Serializable]
		public class CrystalDef
		{
			// Token: 0x040065F5 RID: 26101
			public Material keyMaterial;

			// Token: 0x040065F6 RID: 26102
			public CrystalVisualsPreset visualPreset;

			// Token: 0x040065F7 RID: 26103
			[Space]
			public int low;

			// Token: 0x040065F8 RID: 26104
			public int mid;

			// Token: 0x040065F9 RID: 26105
			public int high;
		}
	}
}
