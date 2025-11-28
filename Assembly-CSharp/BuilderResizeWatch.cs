using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000597 RID: 1431
public class BuilderResizeWatch : MonoBehaviour
{
	// Token: 0x1700039D RID: 925
	// (get) Token: 0x06002429 RID: 9257 RVA: 0x000C1A3C File Offset: 0x000BFC3C
	public int SizeLayerMaskGrow
	{
		get
		{
			int num = 0;
			if (this.growSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.growSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.growSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.growSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x0600242A RID: 9258 RVA: 0x000C1A90 File Offset: 0x000BFC90
	public int SizeLayerMaskShrink
	{
		get
		{
			int num = 0;
			if (this.shrinkSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.shrinkSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.shrinkSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.shrinkSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x000C1AE4 File Offset: 0x000BFCE4
	private void Start()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.AddListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.AddListener(new UnityAction(this.OnShrinkButtonPressed));
		}
		this.ownerRig = base.GetComponentInParent<VRRig>();
		this.enableDist = GTPlayer.Instance.bodyCollider.height;
		this.enableDistSq = this.enableDist * this.enableDist;
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x000C1B7C File Offset: 0x000BFD7C
	private void OnDestroy()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.RemoveListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.RemoveListener(new UnityAction(this.OnShrinkButtonPressed));
		}
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000C1BE0 File Offset: 0x000BFDE0
	private void OnEnlargeButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
				return;
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskGrow && !this.updateCollision)
		{
			this.DisableCollisionWithPieces();
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskGrow;
			if (this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, this.ownerRig.transform.position, true);
			}
			this.timeToCheckCollision = (double)(Time.time + this.growDelay);
			this.updateCollision = true;
		}
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000C1CB0 File Offset: 0x000BFEB0
	private void DisableCollisionWithPieces()
	{
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.ownerRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		int num = Physics.OverlapSphereNonAlloc(GTPlayer.Instance.headCollider.transform.position, 1f, this.tempDisableColliders, builderTable.allPiecesMask);
		for (int i = 0; i < num; i++)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.tempDisableColliders[i]);
			if (builderPieceFromCollider != null && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable && !this.collisionDisabledPieces.Contains(builderPieceFromCollider))
			{
				foreach (Collider collider in builderPieceFromCollider.colliders)
				{
					collider.enabled = false;
				}
				foreach (Collider collider2 in builderPieceFromCollider.placedOnlyColliders)
				{
					collider2.enabled = false;
				}
				this.collisionDisabledPieces.Add(builderPieceFromCollider);
			}
		}
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000C1DEC File Offset: 0x000BFFEC
	private void EnableCollisionWithPieces()
	{
		for (int i = this.collisionDisabledPieces.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.collisionDisabledPieces[i];
			if (builderPiece == null)
			{
				this.collisionDisabledPieces.RemoveAt(i);
			}
			else if (Vector3.SqrMagnitude(GTPlayer.Instance.bodyCollider.transform.position - builderPiece.transform.position) >= this.enableDistSq)
			{
				this.EnableCollisionWithPiece(builderPiece);
				this.collisionDisabledPieces.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000C1E7C File Offset: 0x000C007C
	private void EnableCollisionWithPiece(BuilderPiece piece)
	{
		foreach (Collider collider in piece.colliders)
		{
			collider.enabled = (piece.state != BuilderPiece.State.None && piece.state != BuilderPiece.State.Displayed);
		}
		foreach (Collider collider2 in piece.placedOnlyColliders)
		{
			collider2.enabled = (piece.state == BuilderPiece.State.AttachedAndPlaced);
		}
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x000C1F2C File Offset: 0x000C012C
	private void Update()
	{
		if (this.updateCollision && (double)Time.time >= this.timeToCheckCollision)
		{
			this.EnableCollisionWithPieces();
			if (this.collisionDisabledPieces.Count <= 0)
			{
				this.updateCollision = false;
			}
		}
	}

	// Token: 0x06002432 RID: 9266 RVA: 0x000C1F60 File Offset: 0x000C0160
	private void OnShrinkButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskShrink)
		{
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskShrink;
		}
	}

	// Token: 0x04002F7B RID: 12155
	[SerializeField]
	private HeldButton enlargeButton;

	// Token: 0x04002F7C RID: 12156
	[SerializeField]
	private HeldButton shrinkButton;

	// Token: 0x04002F7D RID: 12157
	[SerializeField]
	private GameObject fxForLayerChange;

	// Token: 0x04002F7E RID: 12158
	private VRRig ownerRig;

	// Token: 0x04002F7F RID: 12159
	private SizeManager sizeManager;

	// Token: 0x04002F80 RID: 12160
	[HideInInspector]
	public Collider[] tempDisableColliders = new Collider[128];

	// Token: 0x04002F81 RID: 12161
	[HideInInspector]
	public List<BuilderPiece> collisionDisabledPieces = new List<BuilderPiece>();

	// Token: 0x04002F82 RID: 12162
	private float enableDist = 1f;

	// Token: 0x04002F83 RID: 12163
	private float enableDistSq = 1f;

	// Token: 0x04002F84 RID: 12164
	private bool updateCollision;

	// Token: 0x04002F85 RID: 12165
	private float growDelay = 1f;

	// Token: 0x04002F86 RID: 12166
	private double timeToCheckCollision;

	// Token: 0x04002F87 RID: 12167
	public BuilderResizeWatch.BuilderSizeChangeSettings growSettings;

	// Token: 0x04002F88 RID: 12168
	public BuilderResizeWatch.BuilderSizeChangeSettings shrinkSettings;

	// Token: 0x02000598 RID: 1432
	[Serializable]
	public struct BuilderSizeChangeSettings
	{
		// Token: 0x04002F89 RID: 12169
		public bool affectLayerA;

		// Token: 0x04002F8A RID: 12170
		public bool affectLayerB;

		// Token: 0x04002F8B RID: 12171
		public bool affectLayerC;

		// Token: 0x04002F8C RID: 12172
		public bool affectLayerD;
	}
}
