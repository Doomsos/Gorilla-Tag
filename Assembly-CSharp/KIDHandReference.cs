using System;
using UnityEngine;

// Token: 0x02000A3F RID: 2623
public class KIDHandReference : MonoBehaviour
{
	// Token: 0x17000640 RID: 1600
	// (get) Token: 0x06004257 RID: 16983 RVA: 0x0015ED0D File Offset: 0x0015CF0D
	public static GameObject LeftHand
	{
		get
		{
			return KIDHandReference._leftHandRef;
		}
	}

	// Token: 0x17000641 RID: 1601
	// (get) Token: 0x06004258 RID: 16984 RVA: 0x0015ED14 File Offset: 0x0015CF14
	public static GameObject RightHand
	{
		get
		{
			return KIDHandReference._rightHandRef;
		}
	}

	// Token: 0x06004259 RID: 16985 RVA: 0x0015ED1B File Offset: 0x0015CF1B
	private void Awake()
	{
		KIDHandReference._leftHandRef = this._leftHand;
		KIDHandReference._rightHandRef = this._rightHand;
	}

	// Token: 0x04005388 RID: 21384
	[SerializeField]
	private GameObject _leftHand;

	// Token: 0x04005389 RID: 21385
	[SerializeField]
	private GameObject _rightHand;

	// Token: 0x0400538A RID: 21386
	private static GameObject _leftHandRef;

	// Token: 0x0400538B RID: 21387
	private static GameObject _rightHandRef;
}
