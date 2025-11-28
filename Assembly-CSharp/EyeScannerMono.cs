using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Text;
using GorillaLocomotion;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000A0 RID: 160
public class EyeScannerMono : MonoBehaviour, ISpawnable, IGorillaSliceableSimple
{
	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00017AD6 File Offset: 0x00015CD6
	// (set) Token: 0x060003F8 RID: 1016 RVA: 0x00017AE0 File Offset: 0x00015CE0
	private Color32 KeyTextColor
	{
		get
		{
			return this.m_keyTextColor;
		}
		set
		{
			this.m_keyTextColor = value;
			this._keyRichTextColorTagString = string.Format(CultureInfo.InvariantCulture.NumberFormat, "<color=#{0:X2}{1:X2}{2:X2}>", value.r, value.g, value.b);
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00017B2F File Offset: 0x00015D2F
	private List<IEyeScannable> registeredScannables
	{
		get
		{
			return EyeScannerMono._registeredScannables;
		}
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00017B36 File Offset: 0x00015D36
	public static void Register(IEyeScannable scannable)
	{
		if (EyeScannerMono._registeredScannableIds.Add(scannable.scannableId))
		{
			EyeScannerMono._registeredScannables.Add(scannable);
		}
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00017B55 File Offset: 0x00015D55
	public static void Unregister(IEyeScannable scannable)
	{
		if (EyeScannerMono._registeredScannableIds.Remove(scannable.scannableId))
		{
			EyeScannerMono._registeredScannables.Remove(scannable);
		}
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00017B78 File Offset: 0x00015D78
	protected void Awake()
	{
		this._sb = ZString.CreateStringBuilder();
		this.KeyTextColor = this.KeyTextColor;
		math.sign(this.m_textTyper.transform.parent.localScale);
		this.m_textTyper.SetText(string.Empty);
		this.m_reticle.gameObject.SetActive(false);
		this.m_textTyper.gameObject.SetActive(false);
		this.m_overlayBg.SetActive(false);
		this._line = base.GetComponent<LineRenderer>();
		this._line.enabled = false;
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060003FD RID: 1021 RVA: 0x00017C12 File Offset: 0x00015E12
	// (set) Token: 0x060003FE RID: 1022 RVA: 0x00017C1A File Offset: 0x00015E1A
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x060003FF RID: 1023 RVA: 0x00017C23 File Offset: 0x00015E23
	// (set) Token: 0x06000400 RID: 1024 RVA: 0x00017C2B File Offset: 0x00015E2B
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000401 RID: 1025 RVA: 0x00017C34 File Offset: 0x00015E34
	// (set) Token: 0x06000402 RID: 1026 RVA: 0x00017C3C File Offset: 0x00015E3C
	public string DebugData { get; private set; }

	// Token: 0x06000403 RID: 1027 RVA: 0x00017C48 File Offset: 0x00015E48
	public void OnSpawn(VRRig rig)
	{
		if (rig != null && !rig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
		if (GTPlayer.hasInstance)
		{
			GTPlayer instance = GTPlayer.Instance;
			this._firstPersonCamera = instance.GetComponentInChildren<Camera>();
			this._has_firstPersonCamera = (this._firstPersonCamera != null);
		}
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDespawn()
	{
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00017C9C File Offset: 0x00015E9C
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone == GTZone.bayou)
		{
			if (this._oldClosestScannable != null)
			{
				this._OnScannableChanged(null, false);
				this._oldClosestScannable = null;
			}
			return;
		}
		IEyeScannable eyeScannable = null;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		Vector3 forward = transform.forward;
		float num = this.m_LookPrecision;
		for (int i = 0; i < EyeScannerMono._registeredScannables.Count; i++)
		{
			IEyeScannable eyeScannable2 = EyeScannerMono._registeredScannables[i];
			Vector3 normalized = (eyeScannable2.Position - position).normalized;
			float num2 = Vector3.Distance(position, eyeScannable2.Position);
			float num3 = Vector3.Dot(forward, normalized);
			if (num2 >= this.m_scanDistanceMin && num2 <= this.m_scanDistanceMax && num3 > num)
			{
				RaycastHit raycastHit;
				if (!this.m_xrayVision && Physics.Raycast(position, normalized, ref raycastHit, this.m_scanDistanceMax, this._layerMask.value))
				{
					IEyeScannable componentInParent = raycastHit.collider.GetComponentInParent<IEyeScannable>();
					if (componentInParent == null || componentInParent != eyeScannable2)
					{
						goto IL_EF;
					}
				}
				num = num3;
				eyeScannable = eyeScannable2;
			}
			IL_EF:;
		}
		if (eyeScannable != this._oldClosestScannable)
		{
			if (this._oldClosestScannable != null)
			{
				this._oldClosestScannable.OnDataChange -= new Action(this.Scannable_OnDataChange);
			}
			this._OnScannableChanged(eyeScannable, true);
			this._oldClosestScannable = eyeScannable;
			if (this._oldClosestScannable != null)
			{
				this._oldClosestScannable.OnDataChange += new Action(this.Scannable_OnDataChange);
			}
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00017E05 File Offset: 0x00016005
	private void Scannable_OnDataChange()
	{
		this._OnScannableChanged(this._oldClosestScannable, false);
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00017E14 File Offset: 0x00016014
	private void LateUpdate()
	{
		if (this._oldClosestScannable != null)
		{
			this.m_reticle.position = this._oldClosestScannable.Position;
			float num = math.distance(base.transform.position, this.m_reticle.position);
			Mathf.Clamp(num * 0.33333f, 0f, 1f);
			float num2 = num * this.m_reticleScale;
			float num3 = num * this.m_textScale;
			float num4 = num * this.m_overlayScale;
			this.m_reticle.localScale = new Vector3(num2, num2, num2);
			this.m_overlay.localPosition = new Vector3(this.m_position.x * num, this.m_position.y * num, num);
			this.m_overlay.localScale = new Vector3(num4, num4, 1f);
			this._line.SetPosition(0, this.m_reticle.position);
			this._line.SetPosition(1, this.m_textTyper.transform.position + this.m_pointerOffset * num3);
			this._line.widthMultiplier = num2;
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00017F40 File Offset: 0x00016140
	private void _OnScannableChanged(IEyeScannable scannable, bool typeingShow)
	{
		this._sb.Clear();
		if (scannable == null)
		{
			this.m_textTyper.SetText(this._sb);
			this.m_textTyper.gameObject.SetActive(false);
			this.m_reticle.gameObject.SetActive(false);
			this.m_overlayBg.SetActive(false);
			this.m_reticle.parent = base.transform;
			this._line.enabled = false;
			return;
		}
		this.m_reticle.gameObject.SetActive(true);
		this.m_textTyper.gameObject.SetActive(true);
		this.m_overlayBg.SetActive(true);
		this.m_reticle.position = scannable.Position;
		this._line.enabled = true;
		this._sb.AppendLine(this.DebugData);
		this._entryIndexes[0] = 0;
		int i = 1;
		int num = 0;
		for (int j = 0; j < scannable.Entries.Count; j++)
		{
			KeyValueStringPair keyValueStringPair = scannable.Entries[j];
			if (!string.IsNullOrEmpty(keyValueStringPair.Key))
			{
				this._sb.Append(this._keyRichTextColorTagString);
				this._sb.Append(keyValueStringPair.Key);
				this._sb.Append("</color>: ");
				num += keyValueStringPair.Key.Length + 2;
			}
			if (!string.IsNullOrEmpty(keyValueStringPair.Value))
			{
				this._sb.Append(keyValueStringPair.Value);
				num += keyValueStringPair.Value.Length;
			}
			this._sb.AppendLine();
			num += Environment.NewLine.Length;
			if (i < this._entryIndexes.Length)
			{
				this._entryIndexes[i++] = num - 1;
			}
		}
		while (i < this._entryIndexes.Length)
		{
			this._entryIndexes[i] = -1;
			i++;
		}
		if (typeingShow)
		{
			this.m_textTyper.SetText(this._sb, this._entryIndexes, num);
			return;
		}
		this.m_textTyper.UpdateText(this._sb, num);
	}

	// Token: 0x0400046C RID: 1132
	[FormerlySerializedAs("_scanDistance")]
	[Tooltip("Any scannables with transforms beyond this distance will be automatically ignored.")]
	[SerializeField]
	private float m_scanDistanceMax = 10f;

	// Token: 0x0400046D RID: 1133
	[SerializeField]
	private float m_scanDistanceMin = 0.5f;

	// Token: 0x0400046E RID: 1134
	[FormerlySerializedAs("_textTyper")]
	[Tooltip("The component that handles setting text in the TextMeshPro and animates the text typing.")]
	[SerializeField]
	private TextTyperAnimatorMono m_textTyper;

	// Token: 0x0400046F RID: 1135
	[SerializeField]
	private Transform m_reticle;

	// Token: 0x04000470 RID: 1136
	[SerializeField]
	private Transform m_overlay;

	// Token: 0x04000471 RID: 1137
	[SerializeField]
	private GameObject m_overlayBg;

	// Token: 0x04000472 RID: 1138
	[SerializeField]
	private float m_reticleScale = 1f;

	// Token: 0x04000473 RID: 1139
	[SerializeField]
	private float m_textScale = 1f;

	// Token: 0x04000474 RID: 1140
	[SerializeField]
	private float m_overlayScale = 1f;

	// Token: 0x04000475 RID: 1141
	[SerializeField]
	private Vector3 m_pointerOffset;

	// Token: 0x04000476 RID: 1142
	[SerializeField]
	private Vector2 m_position;

	// Token: 0x04000477 RID: 1143
	[HideInInspector]
	[SerializeField]
	private Color32 m_keyTextColor = new Color32(byte.MaxValue, 34, 0, byte.MaxValue);

	// Token: 0x04000478 RID: 1144
	private string _keyRichTextColorTagString = "";

	// Token: 0x04000479 RID: 1145
	private static readonly List<IEyeScannable> _registeredScannables = new List<IEyeScannable>(128);

	// Token: 0x0400047A RID: 1146
	private static readonly HashSet<int> _registeredScannableIds = new HashSet<int>(128);

	// Token: 0x0400047B RID: 1147
	private IEyeScannable _oldClosestScannable;

	// Token: 0x0400047C RID: 1148
	private Utf16ValueStringBuilder _sb;

	// Token: 0x0400047D RID: 1149
	private readonly int[] _entryIndexes = new int[16];

	// Token: 0x0400047E RID: 1150
	[SerializeField]
	private LayerMask _layerMask;

	// Token: 0x0400047F RID: 1151
	private Camera _firstPersonCamera;

	// Token: 0x04000480 RID: 1152
	private bool _has_firstPersonCamera;

	// Token: 0x04000484 RID: 1156
	[SerializeField]
	private float m_LookPrecision = 0.65f;

	// Token: 0x04000485 RID: 1157
	[SerializeField]
	private bool m_xrayVision;

	// Token: 0x04000486 RID: 1158
	private LineRenderer _line;
}
