using System;
using UnityEngine;

// Token: 0x0200033F RID: 831
public class XSceneRefTarget : MonoBehaviour
{
	// Token: 0x06001400 RID: 5120 RVA: 0x00073B10 File Offset: 0x00071D10
	private void Awake()
	{
		this.Register(false);
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x00073B19 File Offset: 0x00071D19
	private void Reset()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x00073B26 File Offset: 0x00071D26
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.Register(false);
		}
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x00073B38 File Offset: 0x00071D38
	public void Register(bool force = false)
	{
		if (this.UniqueID == this.lastRegisteredID && !force)
		{
			return;
		}
		if (this.lastRegisteredID != -1)
		{
			XSceneRefGlobalHub.Unregister(this.lastRegisteredID, this);
		}
		XSceneRefGlobalHub.Register(this.UniqueID, this);
		this.lastRegisteredID = this.UniqueID;
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x00073B84 File Offset: 0x00071D84
	private void OnDestroy()
	{
		XSceneRefGlobalHub.Unregister(this.UniqueID, this);
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x00073B92 File Offset: 0x00071D92
	private void AssignNewID()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
		this.Register(false);
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x00073BA8 File Offset: 0x00071DA8
	public static int CreateNewID()
	{
		int num = (int)((DateTime.Now - XSceneRefTarget.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
		if (num <= XSceneRefTarget.lastAssignedID)
		{
			XSceneRefTarget.lastAssignedID++;
			return XSceneRefTarget.lastAssignedID;
		}
		XSceneRefTarget.lastAssignedID = num;
		return num;
	}

	// Token: 0x04001E96 RID: 7830
	public int UniqueID;

	// Token: 0x04001E97 RID: 7831
	[NonSerialized]
	private int lastRegisteredID = -1;

	// Token: 0x04001E98 RID: 7832
	private static DateTime epoch = new DateTime(2024, 1, 1);

	// Token: 0x04001E99 RID: 7833
	private static int lastAssignedID;
}
