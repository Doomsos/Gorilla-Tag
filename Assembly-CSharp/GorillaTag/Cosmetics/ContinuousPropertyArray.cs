using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010CF RID: 4303
	[Serializable]
	public class ContinuousPropertyArray
	{
		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x06006BEA RID: 27626 RVA: 0x002369C6 File Offset: 0x00234BC6
		public int Count
		{
			get
			{
				return this.list.Length;
			}
		}

		// Token: 0x06006BEB RID: 27627 RVA: 0x002369D0 File Offset: 0x00234BD0
		private void InitIfNeeded()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			this.inverseMaximum = 1f / this.maxExpectedValue;
			this.value = 0f;
			this.lastApplyTime = Time.time - Time.deltaTime;
			for (int i = 0; i < this.list.Length; i++)
			{
				this.list[i].Init();
			}
			if (Application.isPlaying)
			{
				for (int j = 0; j < this.list.Length; j++)
				{
					this.list[j].InitThreshold();
				}
			}
			this.uniqueShaderPropertyIndices = new List<int>();
			this.mpb = new MaterialPropertyBlock();
			ContinuousPropertyArray.PropertyComparer propertyComparer = new ContinuousPropertyArray.PropertyComparer();
			Array.Sort<ContinuousProperty>(this.list, propertyComparer);
			if (this.list[0].IsShaderProperty_Cached)
			{
				for (int k = 0; k < this.list.Length; k++)
				{
					if (!this.list[k].IsShaderProperty_Cached)
					{
						this.uniqueShaderPropertyIndices.Add(k);
						return;
					}
					if (k == this.list.Length - 1 || (k > 0 && propertyComparer.Compare(this.list[k - 1], this.list[k]) != 0))
					{
						this.uniqueShaderPropertyIndices.Add(k);
					}
				}
			}
		}

		// Token: 0x06006BEC RID: 27628 RVA: 0x00236B01 File Offset: 0x00234D01
		public void ApplyAll(bool leftHand, float f)
		{
			this.ApplyAll(f);
		}

		// Token: 0x06006BED RID: 27629 RVA: 0x00236B0C File Offset: 0x00234D0C
		public void ApplyAll(float f)
		{
			if (this.list.Length == 0)
			{
				return;
			}
			this.InitIfNeeded();
			float num = Time.time - this.lastApplyTime;
			this.value = (this.instant ? (f * this.inverseMaximum) : Mathf.Lerp(this.value, f * this.inverseMaximum, 1f - Mathf.Exp(-this.responsiveness * num)));
			this.lastApplyTime = Time.time;
			int num2 = int.MaxValue;
			if (this.uniqueShaderPropertyIndices.Count > 0)
			{
				num2 = 0;
				((Renderer)this.list[0].Target).GetPropertyBlock(this.mpb, this.list[0].IntValue);
			}
			bool rigIsLocal = this.cachedRigIsLocal;
			for (int i = 0; i < this.list.Length; i++)
			{
				this.list[i].SetRigIsLocal(rigIsLocal);
				this.list[i].Apply(this.value, num, this.mpb);
				if (num2 < this.uniqueShaderPropertyIndices.Count && i >= this.uniqueShaderPropertyIndices[num2] - 1)
				{
					((Renderer)this.list[i].Target).SetPropertyBlock(this.mpb, this.list[0].IntValue);
					if (++num2 < this.uniqueShaderPropertyIndices.Count)
					{
						((Renderer)this.list[i + 1].Target).GetPropertyBlock(this.mpb, this.list[i + 1].IntValue);
					}
				}
			}
		}

		// Token: 0x04007C82 RID: 31874
		[Tooltip("Divides the input value by this number before being fed into the property array. Unless you know what you're doing, you should probably leave this at 1. You can accomplish the same thing by changing the maximum X value for all the curves/gradients, this is just a shorthand.")]
		[SerializeField]
		private float maxExpectedValue = 1f;

		// Token: 0x04007C83 RID: 31875
		private float inverseMaximum;

		// Token: 0x04007C84 RID: 31876
		[Tooltip("Determines how quickly the internal value lerps towards the input value. A low number will take a long time to match but will be more resistant to fluctuations, visa versa for a high value. A good starting point is 5 to 10.")]
		[SerializeField]
		private float responsiveness = 5f;

		// Token: 0x04007C85 RID: 31877
		[Tooltip("If true (default behavior), the input value will be used directly. Disable this if you need better control over how smoothly the properties get applied.")]
		[SerializeField]
		private bool instant = true;

		// Token: 0x04007C86 RID: 31878
		[SerializeField]
		private ContinuousProperty[] list;

		// Token: 0x04007C87 RID: 31879
		private List<int> uniqueShaderPropertyIndices;

		// Token: 0x04007C88 RID: 31880
		private MaterialPropertyBlock mpb;

		// Token: 0x04007C89 RID: 31881
		private bool initialized;

		// Token: 0x04007C8A RID: 31882
		private float value;

		// Token: 0x04007C8B RID: 31883
		private float lastApplyTime;

		// Token: 0x04007C8C RID: 31884
		[NonSerialized]
		public bool cachedRigIsLocal;

		// Token: 0x020010D0 RID: 4304
		private class PropertyComparer : IComparer<ContinuousProperty>
		{
			// Token: 0x06006BEF RID: 27631 RVA: 0x00236CB8 File Offset: 0x00234EB8
			public int Compare(ContinuousProperty x, ContinuousProperty y)
			{
				if (!x.IsShaderProperty_Cached || !y.IsShaderProperty_Cached)
				{
					return y.IsShaderProperty_Cached.CompareTo(x.IsShaderProperty_Cached);
				}
				int num = x.GetTargetInstanceID() ^ x.IntValue;
				int num2 = y.GetTargetInstanceID() ^ y.IntValue;
				return num.CompareTo(num2);
			}
		}
	}
}
