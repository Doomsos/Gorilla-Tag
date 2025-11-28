using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DD6 RID: 3542
	[NetworkBehaviourWeaved(1)]
	public class ChristmasTree : NetworkComponent
	{
		// Token: 0x060057F0 RID: 22512 RVA: 0x001C1BF8 File Offset: 0x001BFDF8
		protected override void Awake()
		{
			base.Awake();
			foreach (AttachPoint attachPoint in this.hangers.GetComponentsInChildren<AttachPoint>())
			{
				this.attachPointsList.Add(attachPoint);
				AttachPoint attachPoint2 = attachPoint;
				attachPoint2.onHookedChanged = (UnityAction)Delegate.Combine(attachPoint2.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.lightRenderers = this.lights.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = this.lightRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material = this.lightsOffMaterial;
			}
			this.wasActive = false;
			this.isActive = false;
		}

		// Token: 0x060057F1 RID: 22513 RVA: 0x001C1C99 File Offset: 0x001BFE99
		private void Update()
		{
			if (this.spinTheTop && this.topOrnament)
			{
				this.topOrnament.transform.Rotate(0f, this.spinSpeed * Time.deltaTime, 0f, 0);
			}
		}

		// Token: 0x060057F2 RID: 22514 RVA: 0x001C1CD8 File Offset: 0x001BFED8
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (AttachPoint attachPoint in this.attachPointsList)
			{
				attachPoint.onHookedChanged = (UnityAction)Delegate.Remove(attachPoint.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.attachPointsList.Clear();
		}

		// Token: 0x060057F3 RID: 22515 RVA: 0x001C1D58 File Offset: 0x001BFF58
		private void UpdateHangers()
		{
			if (this.attachPointsList.Count == 0)
			{
				return;
			}
			using (List<AttachPoint>.Enumerator enumerator = this.attachPointsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsHooked())
					{
						if (base.IsMine)
						{
							this.updateLight(true);
						}
						return;
					}
				}
			}
			if (base.IsMine)
			{
				this.updateLight(false);
			}
		}

		// Token: 0x060057F4 RID: 22516 RVA: 0x001C1DD8 File Offset: 0x001BFFD8
		private void updateLight(bool enable)
		{
			this.isActive = enable;
			for (int i = 0; i < this.lightRenderers.Length; i++)
			{
				this.lightRenderers[i].material = (enable ? this.lightsOnMaterials[i % this.lightsOnMaterials.Length] : this.lightsOffMaterial);
			}
			this.spinTheTop = enable;
		}

		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x060057F5 RID: 22517 RVA: 0x001C1E2F File Offset: 0x001C002F
		// (set) Token: 0x060057F6 RID: 22518 RVA: 0x001C1E59 File Offset: 0x001C0059
		[Networked]
		[NetworkedWeaved(0, 1)]
		private unsafe NetworkBool Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(NetworkBool*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(NetworkBool*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x060057F7 RID: 22519 RVA: 0x001C1E84 File Offset: 0x001C0084
		public override void WriteDataFusion()
		{
			this.Data = this.isActive;
		}

		// Token: 0x060057F8 RID: 22520 RVA: 0x001C1E97 File Offset: 0x001C0097
		public override void ReadDataFusion()
		{
			this.wasActive = this.isActive;
			this.isActive = this.Data;
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x060057F9 RID: 22521 RVA: 0x001C1ED0 File Offset: 0x001C00D0
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			stream.SendNext(this.isActive);
		}

		// Token: 0x060057FA RID: 22522 RVA: 0x001C1EF4 File Offset: 0x001C00F4
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			this.wasActive = this.isActive;
			this.isActive = (bool)stream.ReceiveNext();
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x060057FC RID: 22524 RVA: 0x001C1F64 File Offset: 0x001C0164
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x060057FD RID: 22525 RVA: 0x001C1F7C File Offset: 0x001C017C
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04006543 RID: 25923
		public GameObject hangers;

		// Token: 0x04006544 RID: 25924
		public GameObject lights;

		// Token: 0x04006545 RID: 25925
		public GameObject topOrnament;

		// Token: 0x04006546 RID: 25926
		public float spinSpeed = 60f;

		// Token: 0x04006547 RID: 25927
		private readonly List<AttachPoint> attachPointsList = new List<AttachPoint>();

		// Token: 0x04006548 RID: 25928
		private MeshRenderer[] lightRenderers;

		// Token: 0x04006549 RID: 25929
		private bool wasActive;

		// Token: 0x0400654A RID: 25930
		private bool isActive;

		// Token: 0x0400654B RID: 25931
		private bool spinTheTop;

		// Token: 0x0400654C RID: 25932
		[SerializeField]
		private Material lightsOffMaterial;

		// Token: 0x0400654D RID: 25933
		[SerializeField]
		private Material[] lightsOnMaterials;

		// Token: 0x0400654E RID: 25934
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private NetworkBool _Data;
	}
}
