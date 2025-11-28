using System;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200055A RID: 1370
public class BuilderPaintBrush : HoldableObject
{
	// Token: 0x060022A8 RID: 8872 RVA: 0x000B51B0 File Offset: 0x000B33B0
	private void Awake()
	{
		this.pieceLayers |= 1 << LayerMask.NameToLayer("Gorilla Object");
		this.pieceLayers |= 1 << LayerMask.NameToLayer("BuilderProp");
		this.pieceLayers |= 1 << LayerMask.NameToLayer("Prop");
		this.paintDistance = Vector3.SqrMagnitude(this.paintVolumeHalfExtents);
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x00002789 File Offset: 0x00000989
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x000B524C File Offset: 0x000B344C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.holdingHand = grabbingHand;
		this.handVelocity = grabbingHand.GetComponent<GorillaVelocityTracker>();
		if (this.handVelocity == null)
		{
			Debug.Log("No Velocity Estimator");
		}
		this.inLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		if (this.inLeftHand)
		{
			base.transform.SetParent(myBodyDockPositions.leftHandTransform, true);
		}
		else
		{
			base.transform.SetParent(myBodyDockPositions.rightHandTransform, true);
		}
		base.transform.localScale = Vector3.one;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.inLeftHand);
		GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		this.brushState = BuilderPaintBrush.PaintBrushState.Held;
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060022AC RID: 8876 RVA: 0x000B534C File Offset: 0x000B354C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (base.OnRelease(zoneReleased, releasingHand))
		{
			this.holdingHand = null;
			EquipmentInteractor.instance.UpdateHandEquipment(null, this.inLeftHand);
			this.inLeftHand = false;
			this.handVelocity = null;
			this.ClearHoveredPiece();
			base.transform.parent = null;
			base.transform.localScale = Vector3.one;
			this.rb.isKinematic = false;
			this.rb.linearVelocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
			this.rb.useGravity = true;
			return true;
		}
		return false;
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x000B53EB File Offset: 0x000B35EB
	private void LateUpdate()
	{
		if (this.brushState == BuilderPaintBrush.PaintBrushState.Inactive)
		{
			return;
		}
		if (this.holdingHand == null || this.materialType == -1)
		{
			this.brushState = BuilderPaintBrush.PaintBrushState.Inactive;
			return;
		}
		this.FindPieceToPaint();
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000B541C File Offset: 0x000B361C
	private void FindPieceToPaint()
	{
		switch (this.brushState)
		{
		case BuilderPaintBrush.PaintBrushState.Held:
		{
			if (this.materialType == -1)
			{
				return;
			}
			Array.Clear(this.hitColliders, 0, this.hitColliders.Length);
			int num = Physics.OverlapBoxNonAlloc(this.brushSurface.transform.position - this.brushSurface.up * this.paintVolumeHalfExtents.y, this.paintVolumeHalfExtents, this.hitColliders, this.brushSurface.transform.rotation, this.pieceLayers, 1);
			BuilderPieceCollider builderPieceCollider = null;
			Collider collider = null;
			float num2 = float.MaxValue;
			for (int i = 0; i < num; i++)
			{
				BuilderPieceCollider component = this.hitColliders[i].GetComponent<BuilderPieceCollider>();
				if (component != null && component.piece.materialType != this.materialType && component.piece.materialType != -1)
				{
					float sqrMagnitude = (this.brushSurface.transform.position - component.transform.position).sqrMagnitude;
					if (sqrMagnitude < num2 && component.piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, component.piece.transform.position))
					{
						num2 = sqrMagnitude;
						builderPieceCollider = component;
						collider = this.hitColliders[i];
					}
				}
			}
			if (builderPieceCollider != null)
			{
				this.ClearHoveredPiece();
				this.hoveredPiece = builderPieceCollider.piece;
				this.hoveredPieceCollider = collider;
				this.hoveredPiece.PaintingTint(true);
				GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
				this.positionDelta = 0f;
				this.lastPosition = this.brushSurface.transform.position;
				this.brushState = BuilderPaintBrush.PaintBrushState.Hover;
				return;
			}
			break;
		}
		case BuilderPaintBrush.PaintBrushState.Hover:
		{
			if (this.hoveredPiece == null || this.hoveredPieceCollider == null)
			{
				this.ClearHoveredPiece();
				return;
			}
			float sqrMagnitude2 = this.handVelocity.GetLatestVelocity(false).sqrMagnitude;
			float sqrMagnitude3 = this.handVelocity.GetAverageVelocity(false, 0.15f, false).sqrMagnitude;
			if (this.handVelocity != null && (sqrMagnitude2 > this.maxPaintVelocitySqrMag || sqrMagnitude3 > this.maxPaintVelocitySqrMag))
			{
				this.ClearHoveredPiece();
				return;
			}
			Vector3 vector = this.brushSurface.position - this.brushSurface.up * this.paintVolumeHalfExtents.y;
			Vector3 vector2 = this.hoveredPieceCollider.ClosestPointOnBounds(vector);
			if (Vector3.SqrMagnitude(vector - vector2) > this.paintDistance)
			{
				this.ClearHoveredPiece();
				return;
			}
			GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, Time.deltaTime);
			float num3 = Vector3.Distance(this.lastPosition, this.brushSurface.position);
			if (num3 < this.minimumWiggleFrameDistance)
			{
				this.lastPosition = this.brushSurface.position;
				return;
			}
			this.positionDelta += Math.Min(num3, this.maximumWiggleFrameDistance);
			this.lastPosition = this.brushSurface.position;
			if (this.positionDelta >= this.wiggleDistanceRequirement)
			{
				this.positionDelta = 0f;
				this.audioSource.clip = this.paintSound;
				this.audioSource.GTPlay();
				this.PaintPiece();
				this.brushState = BuilderPaintBrush.PaintBrushState.JustPainted;
				return;
			}
			break;
		}
		case BuilderPaintBrush.PaintBrushState.JustPainted:
			if (this.paintTimeElapsed > this.paintDelay)
			{
				this.paintTimeElapsed = 0f;
				this.brushState = BuilderPaintBrush.PaintBrushState.Held;
				return;
			}
			this.paintTimeElapsed += Time.deltaTime;
			break;
		default:
			return;
		}
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000B57E4 File Offset: 0x000B39E4
	private void PaintPiece()
	{
		this.hoveredPiece.GetTable().RequestPaintPiece(this.hoveredPiece.pieceId, this.materialType);
		this.hoveredPiece.PaintingTint(false);
		this.hoveredPiece = null;
		this.hoveredPieceCollider = null;
		this.paintTimeElapsed = 0f;
		GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000B5864 File Offset: 0x000B3A64
	private void ClearHoveredPiece()
	{
		if (this.hoveredPiece != null)
		{
			this.hoveredPiece.PaintingTint(false);
		}
		this.hoveredPiece = null;
		this.hoveredPieceCollider = null;
		this.positionDelta = 0f;
		this.brushState = ((this.holdingHand == null || this.materialType == -1) ? BuilderPaintBrush.PaintBrushState.Inactive : BuilderPaintBrush.PaintBrushState.Held);
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000B58C8 File Offset: 0x000B3AC8
	public void SetBrushMaterial(int inMaterialType)
	{
		this.materialType = inMaterialType;
		this.audioSource.clip = this.paintSound;
		this.audioSource.GTPlay();
		if (this.holdingHand != null)
		{
			GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
		if (this.materialType == -1)
		{
			this.ClearHoveredPiece();
		}
		else if (this.brushState == BuilderPaintBrush.PaintBrushState.Inactive && this.holdingHand != null)
		{
			this.brushState = BuilderPaintBrush.PaintBrushState.Held;
		}
		if (this.paintBrushMaterialOptions != null && this.brushRenderer != null)
		{
			Material material;
			int num;
			this.paintBrushMaterialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material != null)
			{
				this.brushRenderer.material = material;
			}
		}
	}

	// Token: 0x04002D3B RID: 11579
	[SerializeField]
	private Transform brushSurface;

	// Token: 0x04002D3C RID: 11580
	[SerializeField]
	private Vector3 paintVolumeHalfExtents;

	// Token: 0x04002D3D RID: 11581
	[SerializeField]
	private BuilderMaterialOptions paintBrushMaterialOptions;

	// Token: 0x04002D3E RID: 11582
	[SerializeField]
	private MeshRenderer brushRenderer;

	// Token: 0x04002D3F RID: 11583
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002D40 RID: 11584
	[SerializeField]
	private AudioClip paintSound;

	// Token: 0x04002D41 RID: 11585
	[SerializeField]
	private AudioClip brushStrokeSound;

	// Token: 0x04002D42 RID: 11586
	private GameObject holdingHand;

	// Token: 0x04002D43 RID: 11587
	private bool inLeftHand;

	// Token: 0x04002D44 RID: 11588
	private GorillaVelocityTracker handVelocity;

	// Token: 0x04002D45 RID: 11589
	private BuilderPiece hoveredPiece;

	// Token: 0x04002D46 RID: 11590
	private Collider hoveredPieceCollider;

	// Token: 0x04002D47 RID: 11591
	private Collider[] hitColliders = new Collider[16];

	// Token: 0x04002D48 RID: 11592
	private LayerMask pieceLayers = 0;

	// Token: 0x04002D49 RID: 11593
	private Vector3 lastPosition = Vector3.zero;

	// Token: 0x04002D4A RID: 11594
	private float positionDelta;

	// Token: 0x04002D4B RID: 11595
	private float wiggleDistanceRequirement = 0.08f;

	// Token: 0x04002D4C RID: 11596
	private float minimumWiggleFrameDistance = 0.005f;

	// Token: 0x04002D4D RID: 11597
	private float maximumWiggleFrameDistance = 0.04f;

	// Token: 0x04002D4E RID: 11598
	private float maxPaintVelocitySqrMag = 0.5f;

	// Token: 0x04002D4F RID: 11599
	private float paintDelay = 0.2f;

	// Token: 0x04002D50 RID: 11600
	private float paintTimeElapsed = -1f;

	// Token: 0x04002D51 RID: 11601
	private float paintDistance;

	// Token: 0x04002D52 RID: 11602
	private int materialType = -1;

	// Token: 0x04002D53 RID: 11603
	private BuilderPaintBrush.PaintBrushState brushState;

	// Token: 0x04002D54 RID: 11604
	private Rigidbody rb;

	// Token: 0x0200055B RID: 1371
	public enum PaintBrushState
	{
		// Token: 0x04002D56 RID: 11606
		Inactive,
		// Token: 0x04002D57 RID: 11607
		HeldRemote,
		// Token: 0x04002D58 RID: 11608
		Held,
		// Token: 0x04002D59 RID: 11609
		Hover,
		// Token: 0x04002D5A RID: 11610
		JustPainted
	}
}
