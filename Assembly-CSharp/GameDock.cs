using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000601 RID: 1537
public class GameDock : MonoBehaviour
{
	// Token: 0x060026C1 RID: 9921 RVA: 0x000CE1B5 File Offset: 0x000CC3B5
	private void Awake()
	{
		this.docked = new List<GameEntity>(1);
		if (this.dockMarker == null)
		{
			this.dockMarker = base.transform;
		}
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000CE1DD File Offset: 0x000CC3DD
	public bool CanDock(GameDockable dockable)
	{
		return !(dockable == null) && (this.dockType != GameDockType.GRToolDock || this.GetDockedCount() <= 0);
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x000CE200 File Offset: 0x000CC400
	public int GetDockedCount()
	{
		return this.docked.Count;
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x000CE20D File Offset: 0x000CC40D
	public void OnDock(GameEntity attachedGameEntity, GameEntity attachedToGameEntity)
	{
		this.dockSound.Play(null);
		this.docked.Add(attachedGameEntity);
		this.dockHaptic.PlayIfSnappedLocal(attachedToGameEntity);
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x000CE233 File Offset: 0x000CC433
	public void OnUndock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
		this.undockSound.Play(null);
		this.docked.Remove(gameEntity);
	}

	// Token: 0x04003293 RID: 12947
	public GameEntity gameEntity;

	// Token: 0x04003294 RID: 12948
	public GameDockType dockType;

	// Token: 0x04003295 RID: 12949
	public float dockRadius = 0.15f;

	// Token: 0x04003296 RID: 12950
	public AbilitySound dockSound;

	// Token: 0x04003297 RID: 12951
	public AbilitySound undockSound;

	// Token: 0x04003298 RID: 12952
	public AbilityHaptic dockHaptic;

	// Token: 0x04003299 RID: 12953
	public Transform dockMarker;

	// Token: 0x0400329A RID: 12954
	private List<GameEntity> docked;
}
