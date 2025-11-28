using System;
using UnityEngine;

// Token: 0x020007AF RID: 1967
[Obsolete]
public class GorillaPawn : MonoBehaviour
{
	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x06003394 RID: 13204 RVA: 0x00115F85 File Offset: 0x00114185
	public VRRig rig
	{
		get
		{
			return this._rig;
		}
	}

	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x06003395 RID: 13205 RVA: 0x00115F8D File Offset: 0x0011418D
	public ZoneEntityBSP zoneEntity
	{
		get
		{
			return this._zoneEntity;
		}
	}

	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x06003396 RID: 13206 RVA: 0x00115F95 File Offset: 0x00114195
	public Transform transform
	{
		get
		{
			return this._transform;
		}
	}

	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x06003397 RID: 13207 RVA: 0x00115F9D File Offset: 0x0011419D
	public XformNode handLeft
	{
		get
		{
			return this._handLeftXform;
		}
	}

	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x06003398 RID: 13208 RVA: 0x00115FA5 File Offset: 0x001141A5
	public XformNode handRight
	{
		get
		{
			return this._handRightXform;
		}
	}

	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x06003399 RID: 13209 RVA: 0x00115FAD File Offset: 0x001141AD
	public XformNode body
	{
		get
		{
			return this._bodyXform;
		}
	}

	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x0600339A RID: 13210 RVA: 0x00115FB5 File Offset: 0x001141B5
	public XformNode head
	{
		get
		{
			return this._headXform;
		}
	}

	// Token: 0x0600339B RID: 13211 RVA: 0x00115FBD File Offset: 0x001141BD
	private void Awake()
	{
		this.Setup(false);
	}

	// Token: 0x0600339C RID: 13212 RVA: 0x00115FC8 File Offset: 0x001141C8
	private void Setup(bool force)
	{
		this._transform = base.transform;
		this._rig = base.GetComponentInChildren<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._zoneEntity = this._rig.zoneEntity;
		bool flag = force || this._handLeft.AsNull<Transform>() == null;
		bool flag2 = force || this._handRight.AsNull<Transform>() == null;
		bool flag3 = force || this._head.AsNull<Transform>() == null;
		if (!flag && !flag2 && !flag3)
		{
			return;
		}
		foreach (Transform transform in this._rig.mainSkin.bones)
		{
			string name = transform.name;
			if (flag3 && name.StartsWith("head", 5))
			{
				this._head = transform;
				this._headXform = new XformNode();
				this._headXform.localPosition = new Vector3(0f, 0.13f, 0.015f);
				this._headXform.radius = 0.12f;
				this._headXform.parent = transform;
			}
			else if (flag && name.StartsWith("hand.L", 5))
			{
				this._handLeft = transform;
				this._handLeftXform = new XformNode();
				this._handLeftXform.localPosition = new Vector3(-0.014f, 0.034f, 0f);
				this._handLeftXform.radius = 0.044f;
				this._handLeftXform.parent = transform;
			}
			else if (flag2 && name.StartsWith("hand.R", 5))
			{
				this._handRight = transform;
				this._handRightXform = new XformNode();
				this._handRightXform.localPosition = new Vector3(0.014f, 0.034f, 0f);
				this._handRightXform.radius = 0.044f;
				this._handRightXform.parent = transform;
			}
		}
	}

	// Token: 0x0600339D RID: 13213 RVA: 0x001161CF File Offset: 0x001143CF
	private bool CanRun()
	{
		if (GorillaPawn._gPawnActiveCount > 10)
		{
			Debug.LogError(string.Format("Cannot register more than {0} pawns.", 10));
			return false;
		}
		return true;
	}

	// Token: 0x0600339E RID: 13214 RVA: 0x001161F4 File Offset: 0x001143F4
	private void OnEnable()
	{
		if (!this.CanRun())
		{
			return;
		}
		this._id = -1;
		if (this._rig && this._rig.OwningNetPlayer != null)
		{
			this._id = this._rig.OwningNetPlayer.ActorNumber;
		}
		this._index = GorillaPawn._gPawnActiveCount++;
		GorillaPawn._gPawns[this._index] = this;
	}

	// Token: 0x0600339F RID: 13215 RVA: 0x00116264 File Offset: 0x00114464
	private void OnDisable()
	{
		this._id = -1;
		if (!this.CanRun())
		{
			return;
		}
		if (this._index < 0 || this._index >= GorillaPawn._gPawnActiveCount - 1)
		{
			return;
		}
		int num = --GorillaPawn._gPawnActiveCount;
		GorillaPawn._gPawns.Swap(this._index, num);
		this._index = num;
	}

	// Token: 0x060033A0 RID: 13216 RVA: 0x001162C0 File Offset: 0x001144C0
	private void OnDestroy()
	{
		int num = GorillaPawn._gPawns.IndexOfRef(this);
		GorillaPawn._gPawns[num] = null;
		Array.Sort<GorillaPawn>(GorillaPawn._gPawns, new Comparison<GorillaPawn>(GorillaPawn.ComparePawns));
		int num2 = 0;
		while (num2 < GorillaPawn._gPawns.Length && GorillaPawn._gPawns[num2])
		{
			num2++;
		}
		GorillaPawn._gPawnActiveCount = num2;
	}

	// Token: 0x060033A1 RID: 13217 RVA: 0x00116320 File Offset: 0x00114520
	private static int ComparePawns(GorillaPawn x, GorillaPawn y)
	{
		bool flag = x.AsNull<GorillaPawn>() == null;
		bool flag2 = y.AsNull<GorillaPawn>() == null;
		if (flag && flag2)
		{
			return 0;
		}
		if (flag)
		{
			return 1;
		}
		if (flag2)
		{
			return -1;
		}
		return x._index.CompareTo(y._index);
	}

	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x060033A2 RID: 13218 RVA: 0x00116369 File Offset: 0x00114569
	public static GorillaPawn[] AllPawns
	{
		get
		{
			return GorillaPawn._gPawns;
		}
	}

	// Token: 0x17000495 RID: 1173
	// (get) Token: 0x060033A3 RID: 13219 RVA: 0x00116370 File Offset: 0x00114570
	public static int ActiveCount
	{
		get
		{
			return GorillaPawn._gPawnActiveCount;
		}
	}

	// Token: 0x17000496 RID: 1174
	// (get) Token: 0x060033A4 RID: 13220 RVA: 0x00116377 File Offset: 0x00114577
	public static Matrix4x4[] ShaderData
	{
		get
		{
			return GorillaPawn._gShaderData;
		}
	}

	// Token: 0x060033A5 RID: 13221 RVA: 0x00116380 File Offset: 0x00114580
	public static void SyncPawnData()
	{
		Matrix4x4[] gShaderData = GorillaPawn._gShaderData;
		m4x4 m4x = default(m4x4);
		for (int i = 0; i < GorillaPawn._gPawnActiveCount; i++)
		{
			GorillaPawn gorillaPawn = GorillaPawn._gPawns[i];
			Vector4 worldPosition = gorillaPawn._headXform.worldPosition;
			Vector4 worldPosition2 = gorillaPawn._bodyXform.worldPosition;
			Vector4 worldPosition3 = gorillaPawn._handLeftXform.worldPosition;
			Vector4 worldPosition4 = gorillaPawn._handRightXform.worldPosition;
			m4x.SetRow0(ref worldPosition);
			m4x.SetRow1(ref worldPosition2);
			m4x.SetRow2(ref worldPosition3);
			m4x.SetRow3(ref worldPosition4);
			m4x.Push(ref gShaderData[i]);
		}
		for (int j = GorillaPawn._gPawnActiveCount; j < 10; j++)
		{
			MatrixUtils.Clear(ref gShaderData[j]);
		}
	}

	// Token: 0x04004201 RID: 16897
	[SerializeField]
	private Transform _transform;

	// Token: 0x04004202 RID: 16898
	[SerializeField]
	private Transform _handLeft;

	// Token: 0x04004203 RID: 16899
	[SerializeField]
	private Transform _handRight;

	// Token: 0x04004204 RID: 16900
	[SerializeField]
	private Transform _head;

	// Token: 0x04004205 RID: 16901
	[Space]
	[SerializeField]
	private VRRig _rig;

	// Token: 0x04004206 RID: 16902
	[SerializeField]
	private ZoneEntityBSP _zoneEntity;

	// Token: 0x04004207 RID: 16903
	[Space]
	[SerializeField]
	private XformNode _handLeftXform;

	// Token: 0x04004208 RID: 16904
	[SerializeField]
	private XformNode _handRightXform;

	// Token: 0x04004209 RID: 16905
	[SerializeField]
	private XformNode _bodyXform;

	// Token: 0x0400420A RID: 16906
	[SerializeField]
	private XformNode _headXform;

	// Token: 0x0400420B RID: 16907
	[Space]
	private int _id;

	// Token: 0x0400420C RID: 16908
	private int _index;

	// Token: 0x0400420D RID: 16909
	private bool _invalid;

	// Token: 0x0400420E RID: 16910
	public const int MAX_PAWNS = 10;

	// Token: 0x0400420F RID: 16911
	private static GorillaPawn[] _gPawns = new GorillaPawn[10];

	// Token: 0x04004210 RID: 16912
	private static int _gPawnActiveCount = 0;

	// Token: 0x04004211 RID: 16913
	private static Matrix4x4[] _gShaderData = new Matrix4x4[10];
}
