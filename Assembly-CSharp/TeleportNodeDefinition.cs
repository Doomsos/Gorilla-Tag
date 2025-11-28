using System;
using UnityEngine;

// Token: 0x02000CB8 RID: 3256
[CreateAssetMenu(fileName = "New TeleportNode Definition", menuName = "Teleportation/TeleportNode Definition", order = 1)]
public class TeleportNodeDefinition : ScriptableObject
{
	// Token: 0x17000767 RID: 1895
	// (get) Token: 0x06004F7D RID: 20349 RVA: 0x00199741 File Offset: 0x00197941
	public TeleportNode Forward
	{
		get
		{
			return this.forward;
		}
	}

	// Token: 0x17000768 RID: 1896
	// (get) Token: 0x06004F7E RID: 20350 RVA: 0x00199749 File Offset: 0x00197949
	public TeleportNode Backward
	{
		get
		{
			return this.backward;
		}
	}

	// Token: 0x06004F7F RID: 20351 RVA: 0x00199751 File Offset: 0x00197951
	public void SetForward(TeleportNode node)
	{
		Debug.Log("registered fwd node " + node.name);
		this.forward = node;
	}

	// Token: 0x06004F80 RID: 20352 RVA: 0x0019976F File Offset: 0x0019796F
	public void SetBackward(TeleportNode node)
	{
		Debug.Log("registered bkwd node " + node.name);
		this.backward = node;
	}

	// Token: 0x04005DFF RID: 24063
	[SerializeField]
	private TeleportNode forward;

	// Token: 0x04005E00 RID: 24064
	[SerializeField]
	private TeleportNode backward;
}
