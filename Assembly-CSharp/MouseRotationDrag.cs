using System;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class MouseRotationDrag : MonoBehaviour
{
	// Token: 0x06000081 RID: 129 RVA: 0x00004A08 File Offset: 0x00002C08
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00004A18 File Offset: 0x00002C18
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
			this.m_euler = base.transform.rotation.eulerAngles;
			return;
		}
		if (Input.GetMouseButton(0))
		{
			this.m_euler.x = this.m_euler.x + vector.y;
			this.m_euler.y = this.m_euler.y + vector.x;
			base.transform.rotation = Quaternion.Euler(this.m_euler);
		}
	}

	// Token: 0x04000098 RID: 152
	private bool m_currFrameHasFocus;

	// Token: 0x04000099 RID: 153
	private bool m_prevFrameHasFocus;

	// Token: 0x0400009A RID: 154
	private Vector3 m_prevMousePosition;

	// Token: 0x0400009B RID: 155
	private Vector3 m_euler;
}
