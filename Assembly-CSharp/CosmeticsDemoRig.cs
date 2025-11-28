using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200028E RID: 654
[ExecuteInEditMode]
public class CosmeticsDemoRig : MonoBehaviour
{
	// Token: 0x040014EA RID: 5354
	[SerializeField]
	private VRRig _vrRig;

	// Token: 0x040014EB RID: 5355
	private Transform[] _vrRigBoneXforms;

	// Token: 0x040014EC RID: 5356
	private Transform[] _vrRigSlotXforms;

	// Token: 0x040014ED RID: 5357
	[SerializeField]
	private Transform chestOffset;

	// Token: 0x040014EE RID: 5358
	[SerializeField]
	private Transform leftArmOffset;

	// Token: 0x040014EF RID: 5359
	[SerializeField]
	private Transform rightArmOffset;

	// Token: 0x040014F0 RID: 5360
	private Vector3 badgeDefaultPos;

	// Token: 0x040014F1 RID: 5361
	private Quaternion badgeDefaultRot;

	// Token: 0x040014F2 RID: 5362
	private bool isInitialized;

	// Token: 0x040014F3 RID: 5363
	private CosmeticsDemoRig.EdSpawnedCosmetic emptyCosmetic;

	// Token: 0x040014F4 RID: 5364
	private Material defaultFaceMaterial;

	// Token: 0x040014F5 RID: 5365
	[SerializeField]
	[HideInInspector]
	private Material myDefaultSkinMaterialInstance;

	// Token: 0x040014F6 RID: 5366
	[SerializeField]
	[HideInInspector]
	private Material materialToChangeTo0;

	// Token: 0x040014F7 RID: 5367
	[SerializeField]
	[HideInInspector]
	private Color monkeColor = new Color(0f, 0f, 0f);

	// Token: 0x040014F8 RID: 5368
	[SerializeField]
	[HideInInspector]
	private GorillaSkin currentSkin;

	// Token: 0x040014F9 RID: 5369
	[SerializeField]
	[HideInInspector]
	private GorillaSkin defaultSkin;

	// Token: 0x040014FA RID: 5370
	[SerializeField]
	[HideInInspector]
	private Material[] faceMaterialSwaps = new Material[10];

	// Token: 0x040014FB RID: 5371
	[HideInInspector]
	public int materialIndex;

	// Token: 0x040014FC RID: 5372
	private int selectedMouth;

	// Token: 0x040014FD RID: 5373
	[HideInInspector]
	public UnityEvent<Color> OnColorChange;

	// Token: 0x040014FE RID: 5374
	[SerializeField]
	private CosmeticsDemoRig.EdSpawnedCosmetic[] spawnedCosmetics = new CosmeticsDemoRig.EdSpawnedCosmetic[16];

	// Token: 0x0200028F RID: 655
	[Serializable]
	private struct EdSpawnedCosmetic
	{
		// Token: 0x040014FF RID: 5375
		public string itemName;

		// Token: 0x04001500 RID: 5376
		public CosmeticSO so;

		// Token: 0x04001501 RID: 5377
		public List<GameObject> objects;

		// Token: 0x04001502 RID: 5378
		public List<GameObject> holdableObjects;

		// Token: 0x04001503 RID: 5379
		public bool isEmpty;
	}
}
