using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class EyeScannableMono : MonoBehaviour, IEyeScannable
{
	// Token: 0x1400000B RID: 11
	// (add) Token: 0x060003E9 RID: 1001 RVA: 0x00017880 File Offset: 0x00015A80
	// (remove) Token: 0x060003EA RID: 1002 RVA: 0x000178B8 File Offset: 0x00015AB8
	public event Action OnDataChange;

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060003EB RID: 1003 RVA: 0x000178ED File Offset: 0x00015AED
	int IEyeScannable.scannableId
	{
		get
		{
			return base.GetInstanceID();
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060003EC RID: 1004 RVA: 0x000178F5 File Offset: 0x00015AF5
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position - this._initialPosition + this._bounds.center;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060003ED RID: 1005 RVA: 0x0001791D File Offset: 0x00015B1D
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this._bounds;
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060003EE RID: 1006 RVA: 0x00017925 File Offset: 0x00015B25
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.data.Entries;
		}
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x00017932 File Offset: 0x00015B32
	private void Awake()
	{
		this.RecalculateBounds();
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x0001793A File Offset: 0x00015B3A
	public void OnEnable()
	{
		this.RecalculateBoundsLater();
		EyeScannerMono.Register(this);
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x00013C79 File Offset: 0x00011E79
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x00017948 File Offset: 0x00015B48
	private void RecalculateBoundsLater()
	{
		EyeScannableMono.<RecalculateBoundsLater>d__17 <RecalculateBoundsLater>d__;
		<RecalculateBoundsLater>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RecalculateBoundsLater>d__.<>4__this = this;
		<RecalculateBoundsLater>d__.<>1__state = -1;
		<RecalculateBoundsLater>d__.<>t__builder.Start<EyeScannableMono.<RecalculateBoundsLater>d__17>(ref <RecalculateBoundsLater>d__);
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00017980 File Offset: 0x00015B80
	private void RecalculateBounds()
	{
		this._initialPosition = base.transform.position;
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		this._bounds = default(Bounds);
		if (componentsInChildren.Length == 0)
		{
			this._bounds.center = base.transform.position;
			this._bounds.Expand(1f);
			return;
		}
		this._bounds = componentsInChildren[0].bounds;
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			this._bounds.Encapsulate(componentsInChildren[i].bounds);
		}
	}

	// Token: 0x04000465 RID: 1125
	[SerializeField]
	private KeyValuePairSet data;

	// Token: 0x04000466 RID: 1126
	private Bounds _bounds;

	// Token: 0x04000467 RID: 1127
	private Vector3 _initialPosition;
}
