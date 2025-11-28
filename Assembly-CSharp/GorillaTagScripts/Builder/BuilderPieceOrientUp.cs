using System;
using BoingKit;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E52 RID: 3666
	public class BuilderPieceOrientUp : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06005B83 RID: 23427 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06005B84 RID: 23428 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06005B85 RID: 23429 RVA: 0x001D6510 File Offset: 0x001D4710
		public void OnPiecePlacementDeserialized()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion rotation;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out rotation);
				this.alwaysFaceUp.rotation = rotation;
			}
		}

		// Token: 0x06005B86 RID: 23430 RVA: 0x001D6558 File Offset: 0x001D4758
		public void OnPieceActivate()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion rotation;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out rotation);
				this.alwaysFaceUp.rotation = rotation;
			}
		}

		// Token: 0x06005B87 RID: 23431 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDeactivate()
		{
		}

		// Token: 0x040068E0 RID: 26848
		[SerializeField]
		private Transform alwaysFaceUp;
	}
}
