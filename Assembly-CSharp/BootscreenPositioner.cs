using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class BootscreenPositioner : MonoBehaviour
{
	// Token: 0x06000092 RID: 146 RVA: 0x00004F10 File Offset: 0x00003110
	private void Awake()
	{
		base.transform.position = this._pov.position;
		base.transform.rotation = Quaternion.Euler(0f, this._pov.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06000093 RID: 147 RVA: 0x00004F68 File Offset: 0x00003168
	private void LateUpdate()
	{
		if (Vector3.Distance(base.transform.position, this._pov.position) > this._distanceThreshold)
		{
			base.transform.position = this._pov.position;
		}
		if (Mathf.Abs(this._pov.rotation.eulerAngles.y - base.transform.rotation.eulerAngles.y) > this._rotationThreshold)
		{
			base.transform.rotation = Quaternion.Euler(0f, this._pov.rotation.eulerAngles.y, 0f);
		}
	}

	// Token: 0x040000B3 RID: 179
	[SerializeField]
	private Transform _pov;

	// Token: 0x040000B4 RID: 180
	[SerializeField]
	private float _distanceThreshold;

	// Token: 0x040000B5 RID: 181
	[SerializeField]
	private float _rotationThreshold;
}
