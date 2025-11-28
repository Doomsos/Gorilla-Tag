using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E68 RID: 3688
	public class BuilderTrafficLight : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06005C3D RID: 23613 RVA: 0x001DA183 File Offset: 0x001D8383
		private void Start()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x06005C3E RID: 23614 RVA: 0x001DA190 File Offset: 0x001D8390
		private void SetState(BuilderTrafficLight.LightState state)
		{
			this.lightState = state;
			if (this.materialProps == null)
			{
				this.materialProps = new MaterialPropertyBlock();
			}
			Color color = this.yellowOff;
			Color color2 = this.redOff;
			Color color3 = this.greenOff;
			switch (state)
			{
			case BuilderTrafficLight.LightState.Red:
				color2 = this.redOn;
				break;
			case BuilderTrafficLight.LightState.Yellow:
				color = this.yellowOn;
				break;
			case BuilderTrafficLight.LightState.Green:
				color3 = this.greenOn;
				break;
			}
			this.redLight.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, color2);
			this.redLight.SetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, color);
			this.yellowLight.SetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, color3);
			this.greenLight.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x06005C3F RID: 23615 RVA: 0x001DA270 File Offset: 0x001D8470
		private void Update()
		{
			if (this.piece == null || this.piece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				float num = Time.time;
				if (PhotonNetwork.InRoom)
				{
					uint num2 = (uint)PhotonNetwork.ServerTimestamp;
					if (this.piece != null)
					{
						num2 = (uint)(PhotonNetwork.ServerTimestamp - this.piece.activatedTimeStamp);
					}
					num = num2 / 1000f;
				}
				float num3 = num % this.cycleDuration / this.cycleDuration;
				num3 = (num3 + this.startPercentageOffset) % 1f;
				int num4 = (int)this.stateCurve.Evaluate(num3);
				if (num4 != (int)this.lightState)
				{
					this.SetState((BuilderTrafficLight.LightState)num4);
				}
			}
		}

		// Token: 0x06005C40 RID: 23616 RVA: 0x001DA314 File Offset: 0x001D8514
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.SetState(BuilderTrafficLight.LightState.Off);
		}

		// Token: 0x06005C41 RID: 23617 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06005C42 RID: 23618 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005C43 RID: 23619 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceActivate()
		{
		}

		// Token: 0x06005C44 RID: 23620 RVA: 0x001DA314 File Offset: 0x001D8514
		public void OnPieceDeactivate()
		{
			this.SetState(BuilderTrafficLight.LightState.Off);
		}

		// Token: 0x040069B8 RID: 27064
		[SerializeField]
		private BuilderPiece piece;

		// Token: 0x040069B9 RID: 27065
		[SerializeField]
		private MeshRenderer redLight;

		// Token: 0x040069BA RID: 27066
		[SerializeField]
		private MeshRenderer yellowLight;

		// Token: 0x040069BB RID: 27067
		[SerializeField]
		private MeshRenderer greenLight;

		// Token: 0x040069BC RID: 27068
		[SerializeField]
		private float cycleDuration = 10f;

		// Token: 0x040069BD RID: 27069
		[SerializeField]
		private float startPercentageOffset = 0.5f;

		// Token: 0x040069BE RID: 27070
		[SerializeField]
		private Color redOn = Color.red;

		// Token: 0x040069BF RID: 27071
		[SerializeField]
		private Color redOff = Color.gray;

		// Token: 0x040069C0 RID: 27072
		[SerializeField]
		private Color yellowOn = Color.yellow;

		// Token: 0x040069C1 RID: 27073
		[SerializeField]
		private Color yellowOff = Color.gray;

		// Token: 0x040069C2 RID: 27074
		[SerializeField]
		private Color greenOn = Color.green;

		// Token: 0x040069C3 RID: 27075
		[SerializeField]
		private Color greenOff = Color.gray;

		// Token: 0x040069C4 RID: 27076
		private MaterialPropertyBlock materialProps;

		// Token: 0x040069C5 RID: 27077
		[SerializeField]
		private AnimationCurve stateCurve;

		// Token: 0x040069C6 RID: 27078
		private BuilderTrafficLight.LightState lightState = BuilderTrafficLight.LightState.Off;

		// Token: 0x02000E69 RID: 3689
		private enum LightState
		{
			// Token: 0x040069C8 RID: 27080
			Red,
			// Token: 0x040069C9 RID: 27081
			Yellow,
			// Token: 0x040069CA RID: 27082
			Green,
			// Token: 0x040069CB RID: 27083
			Off
		}
	}
}
