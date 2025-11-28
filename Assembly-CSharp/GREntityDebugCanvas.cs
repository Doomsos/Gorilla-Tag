using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

// Token: 0x020006C1 RID: 1729
public class GREntityDebugCanvas : MonoBehaviour
{
	// Token: 0x06002C80 RID: 11392 RVA: 0x000F12CC File Offset: 0x000EF4CC
	private void Awake()
	{
		this.builder = new StringBuilder(50);
	}

	// Token: 0x06002C81 RID: 11393 RVA: 0x000F12DC File Offset: 0x000EF4DC
	private void Start()
	{
		if (this.text == null && this.textPanelPrefab != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.textPanelPrefab, base.transform.position + this.prefabAttachOffset, Quaternion.identity, base.transform);
			this.text = gameObject.GetComponent<TMP_Text>();
		}
		if (this.text != null)
		{
			this.text.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x000F1360 File Offset: 0x000EF560
	private bool UpdateActive()
	{
		bool entityDebugEnabled = GhostReactorManager.entityDebugEnabled;
		if (this.text != null)
		{
			this.text.gameObject.SetActive(entityDebugEnabled);
		}
		return entityDebugEnabled;
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x06002C84 RID: 11396 RVA: 0x000F1394 File Offset: 0x000EF594
	private void UpdateText()
	{
		if (this.text)
		{
			this.builder.Clear();
			List<IGameEntityDebugComponent> list = new List<IGameEntityDebugComponent>();
			base.GetComponents<IGameEntityDebugComponent>(list);
			foreach (IGameEntityDebugComponent gameEntityDebugComponent in list)
			{
				List<string> list2 = new List<string>();
				gameEntityDebugComponent.GetDebugTextLines(out list2);
				foreach (string text in list2)
				{
					this.builder.AppendLine(text);
				}
			}
			this.text.text = this.builder.ToString();
		}
	}

	// Token: 0x040039BF RID: 14783
	[SerializeField]
	public TMP_Text text;

	// Token: 0x040039C0 RID: 14784
	public GameObject textPanelPrefab;

	// Token: 0x040039C1 RID: 14785
	public Vector3 prefabAttachOffset = new Vector3(0f, 0.5f, 0f);

	// Token: 0x040039C2 RID: 14786
	private StringBuilder builder;
}
