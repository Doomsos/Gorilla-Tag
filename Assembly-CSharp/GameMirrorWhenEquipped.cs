using System;
using UnityEngine;

// Token: 0x02000626 RID: 1574
public class GameMirrorWhenEquipped : MonoBehaviour
{
	// Token: 0x06002806 RID: 10246 RVA: 0x000D5037 File Offset: 0x000D3237
	private void Awake()
	{
		if (this.m_gameEntity == null)
		{
			this.m_gameEntity = base.GetComponent<GameEntity>();
		}
		if (this.m_xformsToMirror == null)
		{
			this.m_xformsToMirror = Array.Empty<Transform>();
		}
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x000D5068 File Offset: 0x000D3268
	protected void OnEnable()
	{
		GameEntity gameEntity = this.m_gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity2 = this.m_gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity3 = this.m_gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity4 = this.m_gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleGameEntityOnEquipChanged));
	}

	// Token: 0x06002808 RID: 10248 RVA: 0x000D5114 File Offset: 0x000D3314
	protected void OnDisable()
	{
		GameEntity gameEntity = this.m_gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity2 = this.m_gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity3 = this.m_gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity4 = this.m_gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this._HandleGameEntityOnEquipChanged));
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x000D51C0 File Offset: 0x000D33C0
	private void _HandleGameEntityOnEquipChanged()
	{
		if (this.m_shouldOnlyMirrorWhenSnapped && this.m_gameEntity.snappedJoint == SnapJointType.None)
		{
			return;
		}
		Vector3 localScale = (this.m_gameEntity.EquippedHandedness == this.m_handednessToMirror) ? new Vector3(-1f, 1f, 1f) : Vector3.one;
		for (int i = 0; i < this.m_xformsToMirror.Length; i++)
		{
			this.m_xformsToMirror[i].localScale = localScale;
		}
	}

	// Token: 0x04003387 RID: 13191
	[SerializeField]
	private GameEntity m_gameEntity;

	// Token: 0x04003388 RID: 13192
	[SerializeField]
	private Transform[] m_xformsToMirror;

	// Token: 0x04003389 RID: 13193
	[SerializeField]
	private bool m_shouldOnlyMirrorWhenSnapped = true;

	// Token: 0x0400338A RID: 13194
	[Tooltip("Set the X axis scale to -1 if the gadget is attached (held or snapped) to the selected side.")]
	[SerializeField]
	private EHandedness m_handednessToMirror = EHandedness.Right;
}
