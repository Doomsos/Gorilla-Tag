using System;
using UnityEngine;

// Token: 0x02000CA7 RID: 3239
public class GTSubScene : ScriptableObject
{
	// Token: 0x06004F12 RID: 20242 RVA: 0x00198718 File Offset: 0x00196918
	public void SwitchToScene(int index)
	{
		this.scenes[index].LoadAsync();
	}

	// Token: 0x06004F13 RID: 20243 RVA: 0x00198728 File Offset: 0x00196928
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

	// Token: 0x06004F14 RID: 20244 RVA: 0x00198768 File Offset: 0x00196968
	public void LoadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].LoadAsync();
		}
	}

	// Token: 0x06004F15 RID: 20245 RVA: 0x00198798 File Offset: 0x00196998
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
