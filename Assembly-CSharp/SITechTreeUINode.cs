using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000148 RID: 328
public class SITechTreeUINode : MonoBehaviour
{
	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x060008C5 RID: 2245 RVA: 0x0002F448 File Offset: 0x0002D648
	public List<Image> UpgradeLines { get; } = new List<Image>();

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x060008C6 RID: 2246 RVA: 0x0002F450 File Offset: 0x0002D650
	public List<SITechTreeUINode> Parents { get; } = new List<SITechTreeUINode>();

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x060008C7 RID: 2247 RVA: 0x0002F458 File Offset: 0x0002D658
	public bool IsConfigured
	{
		get
		{
			return this._node != null;
		}
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x0002F464 File Offset: 0x0002D664
	public void SetTechTreeNode(SITechTreeStation techTreeStation, SIUpgradeType nodeUpgradeType)
	{
		if (!techTreeStation.techTreeSO.TryGetNode(nodeUpgradeType, out this._node))
		{
			Debug.LogError(string.Format("Node {0} doesn't exist in tree.  Disabling.", nodeUpgradeType));
			base.gameObject.SetActive(false);
			return;
		}
		this.upgradeType = nodeUpgradeType;
		float num = (float)(Mathf.Min(this.GetMaxWordLength(this._node.Value.nickName), 14) * 4);
		Vector2 sizeDelta = this.nodeNickName.rectTransform.sizeDelta;
		if (sizeDelta.x < num)
		{
			sizeDelta.x = num;
			this.nodeNickName.rectTransform.sizeDelta = sizeDelta;
		}
		base.name = (this.nodeNickName.text = this._node.Value.nickName);
		this.button.data = this._node.Value.upgradeType.GetNodeId();
		this.button.buttonPressed.RemoveAllListeners();
		this.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(techTreeStation.TouchscreenButtonPressed));
		this.SetGadgetUnlockNode(this._node.Value.unlockedGadgetPrefab);
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x0002F590 File Offset: 0x0002D790
	public void SetNodeLockStateColor(Color color)
	{
		if (color == Color.red)
		{
			this.circle.sharedMaterial = this.redMat;
		}
		else if (color == Color.black)
		{
			this.circle.sharedMaterial = this.blackMat;
		}
		else if (color == Color.green)
		{
			this.circle.sharedMaterial = this.greenMat;
		}
		foreach (Image image in this.UpgradeLines)
		{
			image.color = color;
		}
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0002F640 File Offset: 0x0002D840
	private void SetGadgetUnlockNode(bool isUnlockNode)
	{
		this.triangle.gameObject.SetActive(isUnlockNode);
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0002F654 File Offset: 0x0002D854
	private int GetMaxWordLength(string text)
	{
		string[] array = text.Split(' ', 0);
		int num = 0;
		foreach (string text2 in array)
		{
			if (text2.Length > num)
			{
				num = text2.Length;
			}
		}
		return num;
	}

	// Token: 0x04000AAB RID: 2731
	public SIUpgradeType upgradeType;

	// Token: 0x04000AAC RID: 2732
	public TextMeshProUGUI nodeNickName;

	// Token: 0x04000AAD RID: 2733
	public MeshRenderer circle;

	// Token: 0x04000AAE RID: 2734
	public MeshRenderer triangle;

	// Token: 0x04000AAF RID: 2735
	public SITouchscreenButton button;

	// Token: 0x04000AB0 RID: 2736
	public Material greenMat;

	// Token: 0x04000AB1 RID: 2737
	public Material redMat;

	// Token: 0x04000AB2 RID: 2738
	public Material blackMat;

	// Token: 0x04000AB3 RID: 2739
	public ObjectHierarchyFlattener imageFlattener;

	// Token: 0x04000AB4 RID: 2740
	public ObjectHierarchyFlattener textFlattener;

	// Token: 0x04000AB7 RID: 2743
	private GraphNode<SITechTreeNode> _node;
}
