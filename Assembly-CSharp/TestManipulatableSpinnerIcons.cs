using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000489 RID: 1161
public class TestManipulatableSpinnerIcons : MonoBehaviour
{
	// Token: 0x06001DAD RID: 7597 RVA: 0x0009C1AA File Offset: 0x0009A3AA
	private void Awake()
	{
		this.GenerateRollers();
	}

	// Token: 0x06001DAE RID: 7598 RVA: 0x0009C1B2 File Offset: 0x0009A3B2
	private void LateUpdate()
	{
		this.currentRotation = this.spinner.angle * this.rotationScale;
		this.UpdateSelectedIndex();
		this.UpdateRollers();
	}

	// Token: 0x06001DAF RID: 7599 RVA: 0x0009C1D8 File Offset: 0x0009A3D8
	private void GenerateRollers()
	{
		for (int i = 0; i < this.rollerElementCount; i++)
		{
			float num = this.rollerElementAngle * (float)i + this.rollerElementAngle * 0.5f;
			Object.Instantiate<GameObject>(this.rollerElementTemplate, base.transform).transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			GameObject gameObject = Object.Instantiate<GameObject>(this.iconElementTemplate, this.iconCanvas.transform);
			gameObject.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			this.visibleIcons.Add(gameObject.GetComponentInChildren<Text>());
		}
		this.rollerElementTemplate.SetActive(false);
		this.iconElementTemplate.SetActive(false);
		this.UpdateRollers();
	}

	// Token: 0x06001DB0 RID: 7600 RVA: 0x0009C2A0 File Offset: 0x0009A4A0
	private void UpdateSelectedIndex()
	{
		float num = this.currentRotation / this.rollerElementAngle;
		if (this.rollerElementCount % 2 == 1)
		{
			num += 0.5f;
		}
		this.selectedIndex = Mathf.FloorToInt(num);
		this.selectedIndex %= this.scrollableCount;
		if (this.selectedIndex < 0)
		{
			this.selectedIndex = this.scrollableCount + this.selectedIndex;
		}
	}

	// Token: 0x06001DB1 RID: 7601 RVA: 0x0009C30C File Offset: 0x0009A50C
	private void UpdateRollers()
	{
		float num = this.currentRotation;
		if (Mathf.Abs(num) > this.rollerElementAngle / 2f)
		{
			if (num > 0f)
			{
				num += this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num -= this.rollerElementAngle / 2f;
			}
			else
			{
				num -= this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num += this.rollerElementAngle / 2f;
			}
		}
		num -= (float)this.rollerElementCount / 2f * this.rollerElementAngle;
		base.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		this.iconCanvas.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		int num2 = this.rollerElementCount / 2;
		for (int i = 0; i < this.visibleIcons.Count; i++)
		{
			int num3 = this.selectedIndex - i + num2;
			if (num3 < 0)
			{
				num3 += this.scrollableCount;
			}
			else
			{
				num3 %= this.scrollableCount;
			}
			this.visibleIcons[i].text = string.Format("{0}", num3 + 1);
		}
	}

	// Token: 0x040027A2 RID: 10146
	public ManipulatableSpinner spinner;

	// Token: 0x040027A3 RID: 10147
	public float rotationScale = 1f;

	// Token: 0x040027A4 RID: 10148
	public int rollerElementCount = 5;

	// Token: 0x040027A5 RID: 10149
	public GameObject rollerElementTemplate;

	// Token: 0x040027A6 RID: 10150
	public GameObject iconCanvas;

	// Token: 0x040027A7 RID: 10151
	public GameObject iconElementTemplate;

	// Token: 0x040027A8 RID: 10152
	public float iconOffset = 1f;

	// Token: 0x040027A9 RID: 10153
	public float rollerElementAngle = 15f;

	// Token: 0x040027AA RID: 10154
	private List<Text> visibleIcons = new List<Text>();

	// Token: 0x040027AB RID: 10155
	private float currentRotation;

	// Token: 0x040027AC RID: 10156
	public int scrollableCount = 50;

	// Token: 0x040027AD RID: 10157
	public int selectedIndex;
}
