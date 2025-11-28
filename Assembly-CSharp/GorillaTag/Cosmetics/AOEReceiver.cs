using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010A3 RID: 4259
	public class AOEReceiver : MonoBehaviour
	{
		// Token: 0x06006A97 RID: 27287 RVA: 0x0022F397 File Offset: 0x0022D597
		public void ReceiveAOE(in AOEReceiver.AOEContext AOEContext)
		{
			if (!this.enabledForAOE)
			{
				return;
			}
			AOEContextEvent onAOEReceived = this.OnAOEReceived;
			if (onAOEReceived == null)
			{
				return;
			}
			onAOEReceived.Invoke(AOEContext);
		}

		// Token: 0x04007AB3 RID: 31411
		public AOEContextEvent OnAOEReceived;

		// Token: 0x04007AB4 RID: 31412
		[Tooltip("Quick toggle to disable receiving without disabling the GameObject.")]
		[SerializeField]
		private bool enabledForAOE = true;

		// Token: 0x020010A4 RID: 4260
		[Serializable]
		public struct AOEContext
		{
			// Token: 0x04007AB5 RID: 31413
			public Vector3 origin;

			// Token: 0x04007AB6 RID: 31414
			public float radius;

			// Token: 0x04007AB7 RID: 31415
			public GameObject instigator;

			// Token: 0x04007AB8 RID: 31416
			public float baseStrength;

			// Token: 0x04007AB9 RID: 31417
			public float finalStrength;

			// Token: 0x04007ABA RID: 31418
			public float distance;

			// Token: 0x04007ABB RID: 31419
			public float normalizedDistance;
		}
	}
}
