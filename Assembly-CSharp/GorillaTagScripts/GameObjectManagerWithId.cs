using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DE5 RID: 3557
	public class GameObjectManagerWithId : MonoBehaviour
	{
		// Token: 0x060058A4 RID: 22692 RVA: 0x001C6568 File Offset: 0x001C4768
		private void Awake()
		{
			Transform[] componentsInChildren = this.objectsContainer.GetComponentsInChildren<Transform>(false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				GameObjectManagerWithId.gameObjectData gameObjectData = new GameObjectManagerWithId.gameObjectData();
				gameObjectData.transform = componentsInChildren[i];
				gameObjectData.id = this.zone.ToString() + i.ToString();
				this.objectData.Add(gameObjectData);
			}
		}

		// Token: 0x060058A5 RID: 22693 RVA: 0x001C65CE File Offset: 0x001C47CE
		private void OnDestroy()
		{
			this.objectData.Clear();
		}

		// Token: 0x060058A6 RID: 22694 RVA: 0x001C65DC File Offset: 0x001C47DC
		public void ReceiveEvent(string id, Transform _transform)
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.id == id)
				{
					gameObjectData.isMatched = true;
					gameObjectData.followTransform = _transform;
				}
			}
		}

		// Token: 0x060058A7 RID: 22695 RVA: 0x001C6644 File Offset: 0x001C4844
		private void Update()
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.isMatched)
				{
					gameObjectData.transform.transform.position = gameObjectData.followTransform.position;
					gameObjectData.transform.transform.rotation = gameObjectData.followTransform.rotation;
				}
			}
		}

		// Token: 0x040065CD RID: 26061
		public GameObject objectsContainer;

		// Token: 0x040065CE RID: 26062
		public GTZone zone;

		// Token: 0x040065CF RID: 26063
		private readonly List<GameObjectManagerWithId.gameObjectData> objectData = new List<GameObjectManagerWithId.gameObjectData>();

		// Token: 0x02000DE6 RID: 3558
		private class gameObjectData
		{
			// Token: 0x040065D0 RID: 26064
			public Transform transform;

			// Token: 0x040065D1 RID: 26065
			public Transform followTransform;

			// Token: 0x040065D2 RID: 26066
			public string id;

			// Token: 0x040065D3 RID: 26067
			public bool isMatched;
		}
	}
}
