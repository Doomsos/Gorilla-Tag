using System;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020005AB RID: 1451
public class BuilderSetSelector : MonoBehaviour
{
	// Token: 0x06002499 RID: 9369 RVA: 0x000C4ACC File Offset: 0x000C2CCC
	private void Start()
	{
		this.zoneRenderers.Clear();
		foreach (GorillaPressableButton gorillaPressableButton in this.groupButtons)
		{
			this.zoneRenderers.Add(gorillaPressableButton.buttonRenderer);
			TMP_Text myTmpText = gorillaPressableButton.myTmpText;
			Renderer renderer = (myTmpText != null) ? myTmpText.GetComponent<Renderer>() : null;
			if (renderer != null)
			{
				this.zoneRenderers.Add(renderer);
			}
		}
		this.zoneRenderers.Add(this.previousPageButton.buttonRenderer);
		this.zoneRenderers.Add(this.nextPageButton.buttonRenderer);
		TMP_Text myTmpText2 = this.previousPageButton.myTmpText;
		Renderer renderer2 = (myTmpText2 != null) ? myTmpText2.GetComponent<Renderer>() : null;
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		TMP_Text myTmpText3 = this.nextPageButton.myTmpText;
		renderer2 = ((myTmpText3 != null) ? myTmpText3.GetComponent<Renderer>() : null);
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		foreach (Renderer renderer3 in this.zoneRenderers)
		{
			renderer3.enabled = false;
		}
		this.inBuilderZone = false;
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000C4C38 File Offset: 0x000C2E38
	public void Setup(List<BuilderPieceSet.BuilderPieceCategory> categories)
	{
		List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups = BuilderSetManager.instance.GetLiveDisplayGroups();
		this.numLiveDisplayGroups = liveDisplayGroups.Count;
		this.includedGroups = new List<BuilderPieceSet.BuilderDisplayGroup>(liveDisplayGroups.Count);
		this._includedCategories = categories;
		foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in liveDisplayGroups)
		{
			if (this.DoesDisplayGroupHaveIncludedCategories(builderDisplayGroup))
			{
				this.includedGroups.Add(builderDisplayGroup);
			}
		}
		BuilderSetManager.instance.OnOwnedSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedGroups));
		BuilderSetManager.instance.OnLiveSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedGroups));
		this.groupsPerPage = this.groupButtons.Length;
		this.totalPages = this.includedGroups.Count / this.groupsPerPage;
		if (this.includedGroups.Count % this.groupsPerPage > 0)
		{
			this.totalPages++;
		}
		this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
		this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
		this.previousPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.nextPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.pageIndex = 0;
		this.currentGroup = this.includedGroups[this.includedGroupIndex];
		this.previousPageButton.onPressButton.AddListener(new UnityAction(this.OnPreviousPageClicked));
		this.nextPageButton.onPressButton.AddListener(new UnityAction(this.OnNextPageClicked));
		GorillaPressableButton[] array = this.groupButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].onPressed += new Action<GorillaPressableButton, bool>(this.OnSetButtonPressed);
		}
		this.UpdateLabels();
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x000C4E38 File Offset: 0x000C3038
	private void OnDestroy()
	{
		if (this.previousPageButton != null)
		{
			this.previousPageButton.onPressButton.RemoveListener(new UnityAction(this.OnPreviousPageClicked));
		}
		if (this.nextPageButton != null)
		{
			this.nextPageButton.onPressButton.RemoveListener(new UnityAction(this.OnNextPageClicked));
		}
		if (BuilderSetManager.instance != null)
		{
			BuilderSetManager.instance.OnOwnedSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedGroups));
			BuilderSetManager.instance.OnLiveSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedGroups));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.groupButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= new Action<GorillaPressableButton, bool>(this.OnSetButtonPressed);
			}
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x000C4F4C File Offset: 0x000C314C
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Renderer renderer = enumerator.Current;
					renderer.enabled = true;
				}
				goto IL_8B;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			foreach (Renderer renderer2 in this.zoneRenderers)
			{
				renderer2.enabled = false;
			}
		}
		IL_8B:
		this.inBuilderZone = flag;
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x000C5008 File Offset: 0x000C3208
	private void OnSetButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		int num = 0;
		for (int i = 0; i < this.groupButtons.Length; i++)
		{
			if (button.Equals(this.groupButtons[i]))
			{
				num = i;
				break;
			}
		}
		int num2 = this.pageIndex * this.groupsPerPage + num;
		if (num2 < this.includedGroups.Count)
		{
			BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.includedGroups[num2];
			if (this.currentGroup == null || builderDisplayGroup.displayName != this.currentGroup.displayName)
			{
				UnityEvent<int> onSelectedGroup = this.OnSelectedGroup;
				if (onSelectedGroup == null)
				{
					return;
				}
				onSelectedGroup.Invoke(builderDisplayGroup.GetDisplayGroupIdentifier());
			}
		}
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x000C50A0 File Offset: 0x000C32A0
	private void RefreshUnlockedGroups()
	{
		List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups = BuilderSetManager.instance.GetLiveDisplayGroups();
		if (liveDisplayGroups.Count != this.numLiveDisplayGroups)
		{
			string text = (this.currentGroup != null) ? this.currentGroup.displayName : "";
			this.numLiveDisplayGroups = liveDisplayGroups.Count;
			ListExtensions.EnsureCapacity<BuilderPieceSet.BuilderDisplayGroup>(this.includedGroups, this.numLiveDisplayGroups);
			this.includedGroups.Clear();
			int num = 0;
			foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in liveDisplayGroups)
			{
				if (this.DoesDisplayGroupHaveIncludedCategories(builderDisplayGroup))
				{
					if (builderDisplayGroup.displayName.Equals(text))
					{
						num = this.includedGroups.Count;
					}
					this.includedGroups.Add(builderDisplayGroup);
				}
			}
			if (this.includedGroups.Count < 1)
			{
				this.currentGroup = null;
			}
			else
			{
				this.includedGroupIndex = num;
				this.currentGroup = this.includedGroups[this.includedGroupIndex];
			}
			this.totalPages = this.includedGroups.Count / this.groupsPerPage;
			if (this.includedGroups.Count % this.groupsPerPage > 0)
			{
				this.totalPages++;
			}
			this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
			this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
			this.previousPageButton.myTmpText.enabled = (this.totalPages > 1);
			this.nextPageButton.myTmpText.enabled = (this.totalPages > 1);
		}
		this.UpdateLabels();
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x000C5258 File Offset: 0x000C3458
	private void OnPreviousPageClicked()
	{
		this.RefreshUnlockedGroups();
		int num = Mathf.Clamp(this.pageIndex - 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x000C5298 File Offset: 0x000C3498
	private void OnNextPageClicked()
	{
		this.RefreshUnlockedGroups();
		int num = Mathf.Clamp(this.pageIndex + 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x000C52D8 File Offset: 0x000C34D8
	public void SetSelection(int groupID)
	{
		if (BuilderSetManager.instance == null)
		{
			return;
		}
		BuilderPieceSet.BuilderDisplayGroup newGroup = BuilderSetManager.instance.GetDisplayGroupFromIndex(groupID);
		if (newGroup == null)
		{
			return;
		}
		this.currentGroup = newGroup;
		this.includedGroupIndex = this.includedGroups.FindIndex((BuilderPieceSet.BuilderDisplayGroup x) => x.displayName == newGroup.displayName);
		this.UpdateLabels();
	}

	// Token: 0x060024A2 RID: 9378 RVA: 0x000C5348 File Offset: 0x000C3548
	private void UpdateLabels()
	{
		for (int i = 0; i < this.groupLabels.Length; i++)
		{
			int num = this.pageIndex * this.groupsPerPage + i;
			if (num < this.includedGroups.Count && this.includedGroups[num] != null)
			{
				if (!this.groupButtons[i].gameObject.activeSelf)
				{
					this.groupButtons[i].gameObject.SetActive(true);
					this.groupButtons[i].myTmpText.gameObject.SetActive(true);
				}
				if (this.groupButtons[i].myTmpText.text != this.includedGroups[num].displayName)
				{
					this.groupButtons[i].myTmpText.text = this.includedGroups[num].displayName;
				}
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(this.includedGroups[num].setID))
				{
					bool flag = this.currentGroup != null && this.includedGroups[num].displayName == this.currentGroup.displayName;
					if (flag != this.groupButtons[i].isOn || !this.groupButtons[i].enabled)
					{
						this.groupButtons[i].isOn = flag;
						this.groupButtons[i].buttonRenderer.material = (flag ? this.groupButtons[i].pressedMaterial : this.groupButtons[i].unpressedMaterial);
					}
					this.groupButtons[i].enabled = true;
				}
				else
				{
					if (this.groupButtons[i].enabled)
					{
						this.groupButtons[i].buttonRenderer.material = this.disabledMaterial;
					}
					this.groupButtons[i].enabled = false;
				}
			}
			else
			{
				if (this.groupButtons[i].gameObject.activeSelf)
				{
					this.groupButtons[i].gameObject.SetActive(false);
					this.groupButtons[i].myTmpText.gameObject.SetActive(false);
				}
				if (this.groupButtons[i].isOn || this.groupButtons[i].enabled)
				{
					this.groupButtons[i].isOn = false;
					this.groupButtons[i].enabled = false;
				}
			}
		}
		bool flag2 = this.pageIndex > 0 && this.totalPages > 1;
		bool flag3 = this.pageIndex < this.totalPages - 1 && this.totalPages > 1;
		if (this.previousPageButton.myTmpText.enabled != flag2)
		{
			this.previousPageButton.myTmpText.enabled = flag2;
		}
		if (this.nextPageButton.myTmpText.enabled != flag3)
		{
			this.nextPageButton.myTmpText.enabled = flag3;
		}
	}

	// Token: 0x060024A3 RID: 9379 RVA: 0x000C561C File Offset: 0x000C381C
	public bool DoesDisplayGroupHaveIncludedCategories(BuilderPieceSet.BuilderDisplayGroup set)
	{
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in set.pieceSubsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x000C5684 File Offset: 0x000C3884
	public BuilderPieceSet.BuilderDisplayGroup GetSelectedGroup()
	{
		return this.currentGroup;
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x000C568C File Offset: 0x000C388C
	public int GetDefaultGroupID()
	{
		if (this.includedGroups == null || this.includedGroups.Count < 1)
		{
			return -1;
		}
		BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.includedGroups[0];
		if (!BuilderSetManager.instance.IsPieceSetOwnedLocally(builderDisplayGroup.setID))
		{
			foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup2 in this.includedGroups)
			{
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(builderDisplayGroup2.setID))
				{
					return builderDisplayGroup2.GetDisplayGroupIdentifier();
				}
			}
			Debug.LogWarning("No default group available for shelf");
			return -1;
		}
		return builderDisplayGroup.GetDisplayGroupIdentifier();
	}

	// Token: 0x0400301D RID: 12317
	private List<BuilderPieceSet.BuilderDisplayGroup> includedGroups;

	// Token: 0x0400301E RID: 12318
	private int numLiveDisplayGroups;

	// Token: 0x0400301F RID: 12319
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x04003020 RID: 12320
	[Header("UI")]
	[FormerlySerializedAs("setLabels")]
	[SerializeField]
	private Text[] groupLabels;

	// Token: 0x04003021 RID: 12321
	[Header("Buttons")]
	[FormerlySerializedAs("setButtons")]
	[SerializeField]
	private GorillaPressableButton[] groupButtons;

	// Token: 0x04003022 RID: 12322
	[SerializeField]
	private GorillaPressableButton previousPageButton;

	// Token: 0x04003023 RID: 12323
	[SerializeField]
	private GorillaPressableButton nextPageButton;

	// Token: 0x04003024 RID: 12324
	private List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x04003025 RID: 12325
	private int includedGroupIndex;

	// Token: 0x04003026 RID: 12326
	private BuilderPieceSet.BuilderDisplayGroup currentGroup;

	// Token: 0x04003027 RID: 12327
	private int pageIndex;

	// Token: 0x04003028 RID: 12328
	private int groupsPerPage = 3;

	// Token: 0x04003029 RID: 12329
	private int totalPages = 1;

	// Token: 0x0400302A RID: 12330
	private List<Renderer> zoneRenderers = new List<Renderer>(10);

	// Token: 0x0400302B RID: 12331
	private bool inBuilderZone;

	// Token: 0x0400302C RID: 12332
	[HideInInspector]
	public UnityEvent<int> OnSelectedGroup;
}
