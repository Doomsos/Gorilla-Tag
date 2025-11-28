using System;
using UnityEngine;

// Token: 0x020003F7 RID: 1015
[CreateAssetMenu(fileName = "NexusCreatorCode", menuName = "Nexus/NexusCreatorCode")]
public class NexusCreatorCode : ScriptableObject
{
	// Token: 0x170002AC RID: 684
	// (get) Token: 0x060018E9 RID: 6377 RVA: 0x00085873 File Offset: 0x00083A73
	public string Code
	{
		get
		{
			return this.code;
		}
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x060018EA RID: 6378 RVA: 0x0008587B File Offset: 0x00083A7B
	public NexusGroupId GroupId
	{
		get
		{
			return this.groupId;
		}
	}

	// Token: 0x04002241 RID: 8769
	[SerializeField]
	private string code;

	// Token: 0x04002242 RID: 8770
	[SerializeField]
	private NexusGroupId groupId;
}
