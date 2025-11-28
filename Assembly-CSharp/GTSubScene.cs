using System;
using UnityEngine;

// Token: 0x02000CA7 RID: 3239
public class GTSubScene : ScriptableObject
{
	// Token: 0x06004F12 RID: 20242 RVA: 0x00198738 File Offset: 0x00196938
	public void SwitchToScene(int index)
	{
		this.scenes[index].LoadAsync();
	}

	// Token: 0x06004F13 RID: 20243 RVA: 0x00198748 File Offset: 0x00196948
	public void SwitchToScene(GTScene scene)
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			GTScene gtscene = this.scenes[i];
			if (!(scene == gtscene))
			{
				gtscene.UnloadAsync();
			}
		}
		scene.LoadAsync();
	}

	// Token: 0x06004F14 RID: 20244 RVA: 0x00198788 File Offset: 0x00196988
	public void LoadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].LoadAsync();
		}
	}

	// Token: 0x06004F15 RID: 20245 RVA: 0x001987B8 File Offset: 0x001969B8
	public void UnloadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].UnloadAsync();
		}
	}

	// Token: 0x04005DB2 RID: 23986
	[DragDropScenes]
	public GTScene[] scenes = new GTScene[0];
}
