using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DE0 RID: 3552
	[NetworkBehaviourWeaved(13)]
	public class FlowersManager : NetworkComponent
	{
		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x0600583F RID: 22591 RVA: 0x001C31E1 File Offset: 0x001C13E1
		// (set) Token: 0x06005840 RID: 22592 RVA: 0x001C31E8 File Offset: 0x001C13E8
		public static FlowersManager Instance { get; private set; }

		// Token: 0x06005841 RID: 22593 RVA: 0x001C31F0 File Offset: 0x001C13F0
		protected override void Awake()
		{
			base.Awake();
			FlowersManager.Instance = this;
			this.hitNotifiers = base.GetComponentsInChildren<SlingshotProjectileHitNotifier>();
			foreach (SlingshotProjectileHitNotifier slingshotProjectileHitNotifier in this.hitNotifiers)
			{
				if (slingshotProjectileHitNotifier != null)
				{
					slingshotProjectileHitNotifier.OnProjectileTriggerEnter += this.ProjectileHitReceiver;
				}
				else
				{
					Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject children");
				}
			}
			foreach (FlowersManager.FlowersInZone flowersInZone in this.sections)
			{
				foreach (GameObject gameObject in flowersInZone.sections)
				{
					this.sectionToZonesDict[gameObject] = flowersInZone.zone;
					Flower[] componentsInChildren = gameObject.GetComponentsInChildren<Flower>();
					this.allFlowers.AddRange(componentsInChildren);
					this.sectionToFlowersDict[gameObject] = Enumerable.ToList<Flower>(componentsInChildren);
				}
			}
		}

		// Token: 0x06005842 RID: 22594 RVA: 0x001C3314 File Offset: 0x001C1514
		private new void Start()
		{
			NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			if (base.IsMine)
			{
				foreach (Flower flower in this.allFlowers)
				{
					flower.UpdateFlowerState(Flower.FlowerState.Healthy, false, false);
				}
			}
		}

		// Token: 0x06005843 RID: 22595 RVA: 0x001C33A8 File Offset: 0x001C15A8
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (SlingshotProjectileHitNotifier slingshotProjectileHitNotifier in this.hitNotifiers)
			{
				if (slingshotProjectileHitNotifier != null)
				{
					slingshotProjectileHitNotifier.OnProjectileTriggerEnter -= this.ProjectileHitReceiver;
				}
			}
			FlowersManager.Instance = null;
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
		}

		// Token: 0x06005844 RID: 22596 RVA: 0x001C341B File Offset: 0x001C161B
		private void ProjectileHitReceiver(SlingshotProjectile projectile, Collider collider)
		{
			if (!projectile.CompareTag("WaterBalloonProjectile"))
			{
				return;
			}
			this.WaterFlowers(collider);
		}

		// Token: 0x06005845 RID: 22597 RVA: 0x001C3434 File Offset: 0x001C1634
		private void WaterFlowers(Collider collider)
		{
			if (!base.IsMine)
			{
				return;
			}
			GameObject gameObject = collider.gameObject;
			if (gameObject == null)
			{
				Debug.LogError("Could not find any flowers section");
				return;
			}
			foreach (Flower flower in this.sectionToFlowersDict[gameObject])
			{
				flower.WaterFlower(true);
			}
		}

		// Token: 0x06005846 RID: 22598 RVA: 0x001C34B0 File Offset: 0x001C16B0
		private void HandleOnZoneChanged()
		{
			foreach (KeyValuePair<GameObject, GTZone> keyValuePair in this.sectionToZonesDict)
			{
				bool enable = ZoneManagement.instance.IsZoneActive(keyValuePair.Value);
				foreach (Flower flower in this.sectionToFlowersDict[keyValuePair.Key])
				{
					flower.UpdateVisuals(enable);
				}
			}
		}

		// Token: 0x06005847 RID: 22599 RVA: 0x001C355C File Offset: 0x001C175C
		public int GetHealthyFlowersInZoneCount(GTZone zone)
		{
			int num = 0;
			foreach (KeyValuePair<GameObject, GTZone> keyValuePair in this.sectionToZonesDict)
			{
				if (keyValuePair.Value == zone)
				{
					using (List<Flower>.Enumerator enumerator2 = this.sectionToFlowersDict[keyValuePair.Key].GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.GetCurrentState() == Flower.FlowerState.Healthy)
							{
								num++;
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06005848 RID: 22600 RVA: 0x001C3608 File Offset: 0x001C1808
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.allFlowers.Count);
			for (int i = 0; i < this.allFlowers.Count; i++)
			{
				stream.SendNext(this.allFlowers[i].IsWatered);
				stream.SendNext(this.allFlowers[i].GetCurrentState());
			}
		}

		// Token: 0x06005849 RID: 22601 RVA: 0x001C3688 File Offset: 0x001C1888
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				bool isWatered = (bool)stream.ReceiveNext();
				Flower.FlowerState currentState = this.allFlowers[i].GetCurrentState();
				Flower.FlowerState flowerState = (Flower.FlowerState)stream.ReceiveNext();
				if (currentState != flowerState)
				{
					this.allFlowers[i].UpdateFlowerState(flowerState, isWatered, true);
				}
			}
		}

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x0600584A RID: 22602 RVA: 0x001C36FB File Offset: 0x001C18FB
		// (set) Token: 0x0600584B RID: 22603 RVA: 0x001C3725 File Offset: 0x001C1925
		[Networked]
		[NetworkedWeaved(0, 13)]
		private unsafe FlowersDataStruct Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FlowersManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(FlowersDataStruct*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FlowersManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(FlowersDataStruct*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x0600584C RID: 22604 RVA: 0x001C3750 File Offset: 0x001C1950
		public override void WriteDataFusion()
		{
			if (base.HasStateAuthority)
			{
				this.Data = new FlowersDataStruct(this.allFlowers);
			}
		}

		// Token: 0x0600584D RID: 22605 RVA: 0x001C376C File Offset: 0x001C196C
		public override void ReadDataFusion()
		{
			if (this.Data.FlowerCount > 0)
			{
				for (int i = 0; i < this.Data.FlowerCount; i++)
				{
					bool isWatered = this.Data.FlowerWateredData[i] == 1;
					Flower.FlowerState currentState = this.allFlowers[i].GetCurrentState();
					Flower.FlowerState flowerState = (Flower.FlowerState)this.Data.FlowerStateData[i];
					if (currentState != flowerState)
					{
						this.allFlowers[i].UpdateFlowerState(flowerState, isWatered, true);
					}
				}
			}
		}

		// Token: 0x0600584E RID: 22606 RVA: 0x001C3804 File Offset: 0x001C1A04
		private void Update()
		{
			int num = this.flowerCheckIndex + 1;
			while (num < this.allFlowers.Count && num < this.flowerCheckIndex + this.flowersToCheck)
			{
				this.allFlowers[num].AnimCatch();
				num++;
			}
			this.flowerCheckIndex = ((this.flowerCheckIndex + this.flowersToCheck >= this.allFlowers.Count) ? 0 : (this.flowerCheckIndex + this.flowersToCheck));
		}

		// Token: 0x06005850 RID: 22608 RVA: 0x001C38AF File Offset: 0x001C1AAF
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06005851 RID: 22609 RVA: 0x001C38C7 File Offset: 0x001C1AC7
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0400658F RID: 25999
		public List<FlowersManager.FlowersInZone> sections;

		// Token: 0x04006590 RID: 26000
		public int flowersToCheck = 1;

		// Token: 0x04006591 RID: 26001
		public int flowerCheckIndex;

		// Token: 0x04006592 RID: 26002
		private readonly List<Flower> allFlowers = new List<Flower>();

		// Token: 0x04006593 RID: 26003
		private SlingshotProjectileHitNotifier[] hitNotifiers;

		// Token: 0x04006594 RID: 26004
		private readonly Dictionary<GameObject, List<Flower>> sectionToFlowersDict = new Dictionary<GameObject, List<Flower>>();

		// Token: 0x04006595 RID: 26005
		private readonly Dictionary<GameObject, GTZone> sectionToZonesDict = new Dictionary<GameObject, GTZone>();

		// Token: 0x04006596 RID: 26006
		private bool hasBeenSerialized;

		// Token: 0x04006597 RID: 26007
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 13)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private FlowersDataStruct _Data;

		// Token: 0x02000DE1 RID: 3553
		[Serializable]
		public class FlowersInZone
		{
			// Token: 0x04006598 RID: 26008
			public GTZone zone;

			// Token: 0x04006599 RID: 26009
			public List<GameObject> sections;
		}
	}
}
