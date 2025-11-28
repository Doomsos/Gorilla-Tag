using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class MousePositionDrag : MonoBehaviour
{
	// Token: 0x0600007E RID: 126 RVA: 0x00004976 File Offset: 0x00002B76
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00004988 File Offset: 0x00002B88
	private void Update()
	{
		this.m_currFrameHasFocus = Application.isFocused;
		bool prevFrameHasFocus = this.m_prevFrameHasFocus;
		this.m_prevFrameHasFocus = this.m_currFrameHasFocus;
		if (!prevFrameHasFocus && !this.m_currFrameHasFocus)
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 prevMousePosition = this.m_prevMousePosition;
		Vector3 vector = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			base.transform.position += 0.02f * vector;
		}
	}

	// Token: 0x04000095 RID: 149
	private bool m_currFrameHasFocus;

	// Token: 0x04000096 RID: 150
	private bool m_prevFrameHasFocus;

	// Token: 0x04000097 RID: 151
	private Vector3 m_prevMousePosition;
}
