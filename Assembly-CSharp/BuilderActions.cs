using System;
using UnityEngine;

// Token: 0x0200056B RID: 1387
public class BuilderActions
{
	// Token: 0x060022DC RID: 8924 RVA: 0x000B62E8 File Offset: 0x000B44E8
	public static BuilderAction CreateAttachToPlayer(int cmdId, int pieceId, Vector3 localPosition, Quaternion localRotation, int actorNumber, bool leftHand)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToPlayer,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber,
			localPosition = localPosition,
			localRotation = localRotation,
			isLeftHand = leftHand
		};
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000B6338 File Offset: 0x000B4538
	public static BuilderAction CreateAttachToPlayerRollback(int cmdId, BuilderPiece piece)
	{
		return BuilderActions.CreateAttachToPlayer(cmdId, piece.pieceId, piece.transform.localPosition, piece.transform.localRotation, piece.heldByPlayerActorNumber, piece.heldInLeftHand);
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000B6368 File Offset: 0x000B4568
	public static BuilderAction CreateDetachFromPlayer(int cmdId, int pieceId, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DetachFromPlayer,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000B63A0 File Offset: 0x000B45A0
	public static BuilderAction CreateAttachToPiece(int cmdId, int pieceId, int parentPieceId, int attachIndex, int parentAttachIndex, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int actorNumber, int timeStamp)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			parentPieceId = parentPieceId,
			attachIndex = attachIndex,
			parentAttachIndex = parentAttachIndex,
			bumpOffsetx = bumpOffsetX,
			bumpOffsetz = bumpOffsetZ,
			twist = twist,
			playerActorNumber = actorNumber,
			timeStamp = timeStamp
		};
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000B6414 File Offset: 0x000B4614
	public static BuilderAction CreateAttachToPieceRollback(int cmdId, BuilderPiece piece, int actorNumber)
	{
		byte pieceTwist = piece.GetPieceTwist();
		sbyte bumpOffsetX;
		sbyte bumpOffsetZ;
		piece.GetPieceBumpOffset(pieceTwist, out bumpOffsetX, out bumpOffsetZ);
		return BuilderActions.CreateAttachToPiece(cmdId, piece.pieceId, piece.parentPiece.pieceId, piece.attachIndex, piece.parentAttachIndex, bumpOffsetX, bumpOffsetZ, pieceTwist, actorNumber, piece.activatedTimeStamp);
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x000B6460 File Offset: 0x000B4660
	public static BuilderAction CreateDetachFromPiece(int cmdId, int pieceId, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DetachFromPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x000B6498 File Offset: 0x000B4698
	public static BuilderAction CreateMakeRoot(int cmdId, int pieceId)
	{
		return new BuilderAction
		{
			type = BuilderActionType.MakePieceRoot,
			localCommandId = cmdId,
			pieceId = pieceId
		};
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x000B64C8 File Offset: 0x000B46C8
	public static BuilderAction CreateDropPiece(int cmdId, int pieceId, Vector3 localPosition, Quaternion localRotation, Vector3 velocity, Vector3 angVelocity, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DropPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			localPosition = localPosition,
			localRotation = localRotation,
			velocity = velocity,
			angVelocity = angVelocity,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x000B6524 File Offset: 0x000B4724
	public static BuilderAction CreateDropPieceRollback(int cmdId, BuilderPiece rootPiece, int actorNumber)
	{
		Vector3 position = rootPiece.transform.position;
		Quaternion rotation = rootPiece.transform.rotation;
		Vector3 velocity = Vector3.zero;
		Vector3 angVelocity = Vector3.zero;
		Rigidbody component = rootPiece.GetComponent<Rigidbody>();
		if (component != null)
		{
			position = component.position;
			rotation = component.rotation;
			velocity = component.linearVelocity;
			angVelocity = component.angularVelocity;
		}
		return BuilderActions.CreateDropPiece(cmdId, rootPiece.pieceId, position, rotation, velocity, angVelocity, actorNumber);
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x000B6598 File Offset: 0x000B4798
	public static BuilderAction CreateAttachToShelfRollback(int cmdId, BuilderPiece piece, int shelfID, bool isConveyor, int timestamp = 0, float splineTime = 0f)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToShelf,
			localCommandId = cmdId,
			pieceId = piece.pieceId,
			attachIndex = shelfID,
			parentAttachIndex = timestamp,
			isLeftHand = isConveyor,
			velocity = new Vector3(splineTime, 0f, 0f),
			localPosition = piece.transform.localPosition,
			localRotation = piece.transform.localRotation
		};
	}
}
