using System;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class SuperInfectionHandDisplay : MonoBehaviour
{
	// Token: 0x06000941 RID: 2369 RVA: 0x00031C70 File Offset: 0x0002FE70
	public void EnableHands(bool on)
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].SetActive(on);
		}
	}

	// Token: 0x04000B3D RID: 2877
	[SerializeField]
	private GameObject[] gameObjects;
}
