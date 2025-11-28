using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class PropSelector : MonoBehaviour
{
	// Token: 0x06000EB5 RID: 3765 RVA: 0x0004E508 File Offset: 0x0004C708
	private void Start()
	{
		foreach (GameObject gameObject in new List<GameObject>(Enumerable.Take<GameObject>(Enumerable.OrderBy<GameObject, int>(this._props, (GameObject x) => PropSelector._gRandom.Next()), this._desiredActivePropsNum)))
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x040011D5 RID: 4565
	[SerializeField]
	private List<GameObject> _props = new List<GameObject>();

	// Token: 0x040011D6 RID: 4566
	[SerializeField]
	private int _desiredActivePropsNum = 1;

	// Token: 0x040011D7 RID: 4567
	private static readonly Random _gRandom = new Random();
}
