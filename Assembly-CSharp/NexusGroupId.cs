using System;
using UnityEngine;

// Token: 0x020003F8 RID: 1016
[CreateAssetMenu(fileName = "NexusGroupId", menuName = "Nexus/NexusGroupId")]
public class NexusGroupId : ScriptableObject
{
	// Token: 0x170002AE RID: 686
	// (get) Token: 0x060018EC RID: 6380 RVA: 0x00085883 File Offset: 0x00083A83
	public string Code
	{
		get
		{
			if (NexusManager.instance != null && NexusManager.instance.CurrentEnvironment == NexusManager.Environment.PRODUCTION)
			{
				return this.code;
			}
			return this.sandboxCode;
		}
	}

	// Token: 0x04002243 RID: 8771
	[SerializeField]
	private string code;

	// Token: 0x04002244 RID: 8772
	[SerializeField]
	private string sandboxCode;
}
