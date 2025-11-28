using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001112 RID: 4370
	[RequireComponent(typeof(NetworkedRandomProvider))]
	public class RandomWeightedOutput : MonoBehaviour
	{
		// Token: 0x06006D6B RID: 28011 RVA: 0x0023EB07 File Offset: 0x0023CD07
		private void Awake()
		{
			if (this.networkProvider == null)
			{
				this.networkProvider = base.GetComponentInParent<NetworkedRandomProvider>();
			}
		}

		// Token: 0x06006D6C RID: 28012 RVA: 0x0023EB24 File Offset: 0x0023CD24
		public void PickNextRandom()
		{
			int deterministicPickIndex = this.GetDeterministicPickIndex();
			if (deterministicPickIndex >= 0)
			{
				UnityEvent onPick = this.outputs[deterministicPickIndex].onPick;
				if (onPick != null)
				{
					onPick.Invoke();
				}
				UnityEvent<int> unityEvent = this.onAnyPick;
				if (unityEvent != null)
				{
					unityEvent.Invoke(deterministicPickIndex);
				}
				if (this.debugLog)
				{
					Debug.Log(string.Format("[RandomWeightedOutput] Picked '{0}' (idx={1})", this.outputs[deterministicPickIndex].name, deterministicPickIndex));
				}
			}
		}

		// Token: 0x06006D6D RID: 28013 RVA: 0x0023EB98 File Offset: 0x0023CD98
		private int GetDeterministicPickIndex()
		{
			if (this.networkProvider == null)
			{
				return -1;
			}
			List<int> list = new List<int>(this.outputs.Count);
			for (int i = 0; i < this.outputs.Count; i++)
			{
				RandomWeightedOutput.WeightedOutput weightedOutput = this.outputs[i];
				if (weightedOutput != null && weightedOutput.enabled && weightedOutput.weight > 0f)
				{
					list.Add(i);
				}
			}
			if (list.Count == 0)
			{
				return -1;
			}
			double num = 0.0;
			foreach (int num2 in list)
			{
				num += (double)this.outputs[num2].weight;
			}
			if (num <= 0.0)
			{
				return list[0];
			}
			double num3 = (double)this.networkProvider.GetSelectedAsFloat() * num;
			double num4 = 0.0;
			for (int j = 0; j < list.Count; j++)
			{
				int num5 = list[j];
				num4 += (double)this.outputs[num5].weight;
				if (num3 < num4)
				{
					return num5;
				}
			}
			List<int> list2 = list;
			return list2[list2.Count - 1];
		}

		// Token: 0x04007EB1 RID: 32433
		[Header("Network Provider")]
		[Tooltip("For best result, pick Float01 or Double01 as the output mode in your NetworkedRandomProvider")]
		[SerializeField]
		private NetworkedRandomProvider networkProvider;

		// Token: 0x04007EB2 RID: 32434
		[Header("Weighted Outputs")]
		[SerializeField]
		private List<RandomWeightedOutput.WeightedOutput> outputs = new List<RandomWeightedOutput.WeightedOutput>();

		// Token: 0x04007EB3 RID: 32435
		[Header("Event")]
		[SerializeField]
		public UnityEvent<int> onAnyPick = new UnityEvent<int>();

		// Token: 0x04007EB4 RID: 32436
		[SerializeField]
		private bool debugLog;

		// Token: 0x02001113 RID: 4371
		[Serializable]
		public class WeightedOutput
		{
			// Token: 0x04007EB5 RID: 32437
			[SerializeField]
			public string name = "Event";

			// Token: 0x04007EB6 RID: 32438
			[SerializeField]
			[Range(0f, 100f)]
			public float weight = 1f;

			// Token: 0x04007EB7 RID: 32439
			[SerializeField]
			public bool enabled = true;

			// Token: 0x04007EB8 RID: 32440
			[SerializeField]
			public UnityEvent onPick = new UnityEvent();
		}
	}
}
