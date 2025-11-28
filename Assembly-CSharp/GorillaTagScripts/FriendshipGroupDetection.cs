using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000DE3 RID: 3555
	public class FriendshipGroupDetection : NetworkSceneObject, ITickSystemTick
	{
		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x06005858 RID: 22616 RVA: 0x001C3998 File Offset: 0x001C1B98
		// (set) Token: 0x06005859 RID: 22617 RVA: 0x001C399F File Offset: 0x001C1B9F
		public static FriendshipGroupDetection Instance { get; private set; }

		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x0600585A RID: 22618 RVA: 0x001C39A7 File Offset: 0x001C1BA7
		// (set) Token: 0x0600585B RID: 22619 RVA: 0x001C39AF File Offset: 0x001C1BAF
		public List<Color> myBeadColors { get; private set; } = new List<Color>();

		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x0600585C RID: 22620 RVA: 0x001C39B8 File Offset: 0x001C1BB8
		// (set) Token: 0x0600585D RID: 22621 RVA: 0x001C39C0 File Offset: 0x001C1BC0
		public Color myBraceletColor { get; private set; }

		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x0600585E RID: 22622 RVA: 0x001C39C9 File Offset: 0x001C1BC9
		// (set) Token: 0x0600585F RID: 22623 RVA: 0x001C39D1 File Offset: 0x001C1BD1
		public int MyBraceletSelfIndex { get; private set; }

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x06005860 RID: 22624 RVA: 0x001C39DA File Offset: 0x001C1BDA
		public List<string> PartyMemberIDs
		{
			get
			{
				return this.myPartyMemberIDs;
			}
		}

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x06005861 RID: 22625 RVA: 0x001C39E2 File Offset: 0x001C1BE2
		public bool IsInParty
		{
			get
			{
				return this.myPartyMemberIDs != null;
			}
		}

		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06005862 RID: 22626 RVA: 0x001C39ED File Offset: 0x001C1BED
		// (set) Token: 0x06005863 RID: 22627 RVA: 0x001C39F5 File Offset: 0x001C1BF5
		public GroupJoinZoneAB partyZone { get; private set; }

		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x06005864 RID: 22628 RVA: 0x001C39FE File Offset: 0x001C1BFE
		// (set) Token: 0x06005865 RID: 22629 RVA: 0x001C3A06 File Offset: 0x001C1C06
		public bool TickRunning { get; set; }

		// Token: 0x06005866 RID: 22630 RVA: 0x001C3A10 File Offset: 0x001C1C10
		private void Awake()
		{
			FriendshipGroupDetection.Instance = this;
			if (this.friendshipBubble)
			{
				this.particleSystem = this.friendshipBubble.GetComponent<ParticleSystem>();
				this.audioSource = this.friendshipBubble.GetComponent<AudioSource>();
			}
			NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(this.OnPlayerJoinedRoom);
		}

		// Token: 0x06005867 RID: 22631 RVA: 0x000C831B File Offset: 0x000C651B
		private new void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06005868 RID: 22632 RVA: 0x000C832F File Offset: 0x000C652F
		private new void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnDisable();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06005869 RID: 22633 RVA: 0x001C3A74 File Offset: 0x001C1C74
		private void OnPlayerJoinedRoom(NetPlayer joiningPlayer)
		{
			if (!this.IsInParty)
			{
				return;
			}
			bool flag = (int)RoomSystem.GetRoomSize("") == NetworkSystem.Instance.RoomPlayerCount;
			Debug.Log(string.Concat(new string[]
			{
				"[FriendshipGroupDetection::OnPlayerJoinedRoom] JoiningPlayer: ",
				joiningPlayer.NickName,
				", ",
				joiningPlayer.UserId,
				" ",
				string.Format("| IsLocal: {0} | Room Full: {1}", joiningPlayer.IsLocal, flag)
			}));
			if (joiningPlayer.IsLocal)
			{
				this.lastJoinedRoomTime = (double)Time.time;
				if (!flag)
				{
					Debug.Log("[FriendshipGroupDetection::OnPlayerJoinedRoom] Delaying PartyRefresh...");
					this.wantsPartyRefreshPostJoin = true;
					return;
				}
			}
			if (flag)
			{
				this.RefreshPartyMembers();
			}
		}

		// Token: 0x0600586A RID: 22634 RVA: 0x001C3B2B File Offset: 0x001C1D2B
		public void AddGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Add(callback);
		}

		// Token: 0x0600586B RID: 22635 RVA: 0x001C3B39 File Offset: 0x001C1D39
		public void RemoveGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Remove(callback);
		}

		// Token: 0x0600586C RID: 22636 RVA: 0x001C3B48 File Offset: 0x001C1D48
		public bool IsInMyGroup(string userID)
		{
			return this.myPartyMemberIDs != null && this.myPartyMemberIDs.Contains(userID);
		}

		// Token: 0x0600586D RID: 22637 RVA: 0x001C3B60 File Offset: 0x001C1D60
		public bool AnyPartyMembersOutsideFriendCollider()
		{
			if (!this.IsInParty)
			{
				return false;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x0600586E RID: 22638 RVA: 0x001C3BF0 File Offset: 0x001C1DF0
		// (set) Token: 0x0600586F RID: 22639 RVA: 0x001C3BF8 File Offset: 0x001C1DF8
		public bool DidJoinLeftHanded { get; private set; }

		// Token: 0x06005870 RID: 22640 RVA: 0x001C3C04 File Offset: 0x001C1E04
		public void Tick()
		{
			if (this.wantsPartyRefreshPostJoin && this.lastJoinedRoomTime + this.joinedRoomRefreshPartyDelay < (double)Time.time)
			{
				this.RefreshPartyMembers();
			}
			if (this.wantsPartyRefreshPostFollowFailed && this.lastFailedToFollowPartyTime + this.failedToFollowRefreshPartyDelay < (double)Time.time)
			{
				this.RefreshPartyMembers();
			}
			List<int> list = this.playersInProvisionalGroup;
			List<int> list2 = this.playersInProvisionalGroup;
			List<int> list3 = this.tempIntList;
			this.tempIntList = list2;
			this.playersInProvisionalGroup = list3;
			Vector3 position;
			this.UpdateProvisionalGroup(out position);
			if (this.playersInProvisionalGroup.Count > 0)
			{
				this.friendshipBubble.transform.position = position;
			}
			bool flag = false;
			if (list.Count == this.playersInProvisionalGroup.Count)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != this.playersInProvisionalGroup[i])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.groupCreateAfterTimestamp = Time.time + this.groupTime;
				this.amFirstProvisionalPlayer = (this.playersInProvisionalGroup.Count > 0 && this.playersInProvisionalGroup[0] == NetworkSystem.Instance.LocalPlayer.ActorNumber);
				if (this.playersInProvisionalGroup.Count > 0 && !this.amFirstProvisionalPlayer)
				{
					List<int> list4 = this.tempIntList;
					list4.Clear();
					NetPlayer netPlayer = null;
					foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
					{
						if (vrrig.creator.ActorNumber == this.playersInProvisionalGroup[0])
						{
							netPlayer = vrrig.creator;
							if (vrrig.IsLocalPartyMember)
							{
								list4.Clear();
								break;
							}
						}
						else if (vrrig.IsLocalPartyMember)
						{
							list4.Add(vrrig.creator.ActorNumber);
						}
					}
					if (list4.Count > 0)
					{
						this.photonView.RPC("NotifyPartyMerging", netPlayer.GetPlayerRef(), new object[]
						{
							list4.ToArray()
						});
					}
					else
					{
						this.photonView.RPC("NotifyNoPartyToMerge", netPlayer.GetPlayerRef(), Array.Empty<object>());
					}
				}
				if (this.playersInProvisionalGroup.Count == 0)
				{
					if (Time.time > this.suppressPartyCreationUntilTimestamp && this.playEffectsAfterTimestamp == 0f)
					{
						this.audioSource.GTStop();
						this.audioSource.GTPlayOneShot(this.fistBumpInterruptedAudio, 1f);
					}
					this.particleSystem.Stop();
					this.playEffectsAfterTimestamp = 0f;
				}
				else
				{
					this.playEffectsAfterTimestamp = Time.time + this.playEffectsDelay;
				}
			}
			else if (this.playEffectsAfterTimestamp > 0f && Time.time > this.playEffectsAfterTimestamp)
			{
				this.audioSource.time = 0f;
				this.audioSource.GTPlay();
				this.particleSystem.Play();
				this.playEffectsAfterTimestamp = 0f;
			}
			else if (this.playersInProvisionalGroup.Count > 0 && Time.time > this.groupCreateAfterTimestamp && this.amFirstProvisionalPlayer)
			{
				List<int> list5 = this.tempIntList;
				list5.Clear();
				list5.AddRange(this.playersInProvisionalGroup);
				int num = 0;
				if (this.IsInParty)
				{
					foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
					{
						if (vrrig2.IsLocalPartyMember)
						{
							list5.Add(vrrig2.creator.ActorNumber);
							num++;
						}
					}
				}
				int num2 = 0;
				foreach (int num3 in this.playersInProvisionalGroup)
				{
					int[] array;
					if (this.partyMergeIDs.TryGetValue(num3, ref array))
					{
						list5.AddRange(array);
						num2++;
					}
				}
				list5.Sort();
				int[] memberIDs = Enumerable.ToArray<int>(Enumerable.Distinct<int>(list5));
				this.myBraceletColor = GTColor.RandomHSV(this.braceletRandomColorHSVRanges);
				this.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(this.myBraceletColor), memberIDs, false);
				this.groupCreateAfterTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			}
			if (this.myPartyMemberIDs != null)
			{
				this.UpdateWarningSigns();
			}
		}

		// Token: 0x06005871 RID: 22641 RVA: 0x001C408C File Offset: 0x001C228C
		private void UpdateProvisionalGroup(out Vector3 midpoint)
		{
			this.playersInProvisionalGroup.Clear();
			bool willJoinLeftHanded;
			VRMap makingFist = VRRig.LocalRig.GetMakingFist(this.debug, out willJoinLeftHanded);
			if (makingFist == null || !NetworkSystem.Instance.InRoom || VRRig.LocalRig.leftHandLink.IsLinkActive() || VRRig.LocalRig.rightHandLink.IsLinkActive() || GorillaParent.instance.vrrigs.Count == 0 || Time.time < this.suppressPartyCreationUntilTimestamp || (GameMode.ActiveGameMode != null && !GameMode.ActiveGameMode.CanJoinFrienship(NetworkSystem.Instance.LocalPlayer)))
			{
				midpoint = Vector3.zero;
				return;
			}
			this.WillJoinLeftHanded = willJoinLeftHanded;
			this.playersToPropagateFrom.Clear();
			this.provisionalGroupUsingLeftHands.Clear();
			this.playersMakingFists.Clear();
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			int num = -1;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				bool isLeftHand;
				VRMap makingFist2 = vrrig.GetMakingFist(this.debug, out isLeftHand);
				if (makingFist2 != null && !vrrig.leftHandLink.IsLinkActive() && !vrrig.rightHandLink.IsLinkActive() && (!(GameMode.ActiveGameMode != null) || GameMode.ActiveGameMode.CanJoinFrienship(vrrig.OwningNetPlayer)))
				{
					FriendshipGroupDetection.PlayerFist playerFist = new FriendshipGroupDetection.PlayerFist
					{
						actorNumber = vrrig.creator.ActorNumber,
						position = makingFist2.rigTarget.position,
						isLeftHand = isLeftHand
					};
					if (vrrig.isOfflineVRRig)
					{
						num = this.playersMakingFists.Count;
					}
					this.playersMakingFists.Add(playerFist);
				}
			}
			if (this.playersMakingFists.Count <= 1 || num == -1)
			{
				midpoint = Vector3.zero;
				return;
			}
			this.playersToPropagateFrom.Enqueue(this.playersMakingFists[num]);
			this.playersInProvisionalGroup.Add(actorNumber);
			midpoint = makingFist.rigTarget.position;
			int num2 = 1 << num;
			FriendshipGroupDetection.PlayerFist playerFist2;
			while (this.playersToPropagateFrom.TryDequeue(ref playerFist2))
			{
				for (int i = 0; i < this.playersMakingFists.Count; i++)
				{
					if ((num2 & 1 << i) == 0)
					{
						FriendshipGroupDetection.PlayerFist playerFist3 = this.playersMakingFists[i];
						if ((playerFist2.position - playerFist3.position).IsShorterThan(this.detectionRadius))
						{
							int num3 = ~this.playersInProvisionalGroup.BinarySearch(playerFist3.actorNumber);
							num2 |= 1 << i;
							this.playersInProvisionalGroup.Insert(num3, playerFist3.actorNumber);
							if (playerFist3.isLeftHand)
							{
								this.provisionalGroupUsingLeftHands.Add(playerFist3.actorNumber);
							}
							this.playersToPropagateFrom.Enqueue(playerFist3);
							midpoint += playerFist3.position;
						}
					}
				}
			}
			if (this.playersInProvisionalGroup.Count == 1)
			{
				this.playersInProvisionalGroup.Clear();
			}
			if (this.playersInProvisionalGroup.Count > 0)
			{
				midpoint /= (float)this.playersInProvisionalGroup.Count;
			}
		}

		// Token: 0x06005872 RID: 22642 RVA: 0x001C43FC File Offset: 0x001C25FC
		private void UpdateWarningSigns()
		{
			ZoneEntityBSP zoneEntity = GorillaTagger.Instance.offlineVRRig.zoneEntity;
			GTZone currentRoomZone = PhotonNetworkController.Instance.CurrentRoomZone;
			GroupJoinZoneAB groupJoinZoneAB = 0;
			if (this.myPartyMemberIDs != null)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
					{
						groupJoinZoneAB |= vrrig.zoneEntity.GroupZone;
					}
				}
			}
			if (groupJoinZoneAB != this.partyZone)
			{
				this.debugStr.Clear();
				foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
				{
					if (vrrig2.IsLocalPartyMember && !vrrig2.isOfflineVRRig)
					{
						this.debugStr.Append(string.Format("{0} in {1};", vrrig2.playerNameVisible, vrrig2.zoneEntity.GroupZone));
					}
				}
				this.partyZone = groupJoinZoneAB;
				foreach (Action<GroupJoinZoneAB> action in this.groupZoneCallbacks)
				{
					action.Invoke(this.partyZone);
				}
			}
		}

		// Token: 0x06005873 RID: 22643 RVA: 0x001C4584 File Offset: 0x001C2784
		[PunRPC]
		private void NotifyNoPartyToMerge(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyNoPartyToMerge");
			if (info.Sender == null || this.partyMergeIDs == null)
			{
				return;
			}
			this.partyMergeIDs.Remove(info.Sender.ActorNumber);
		}

		// Token: 0x06005874 RID: 22644 RVA: 0x001C45BC File Offset: 0x001C27BC
		[Rpc]
		private unsafe static void RPC_NotifyNoPartyToMerge(NetworkRunner runner, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					int num = 8;
					if (SimulationMessage.CanAllocateUserPayload(num))
					{
						if (runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)"));
							int num2 = 8;
							ptr.Offset = num2 * 8;
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
						info = RpcInfo.FromLocal(runner, 0, 0);
						goto IL_10;
					}
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)", num);
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.partyMergeIDs.Remove(info.Source.PlayerId);
		}

		// Token: 0x06005875 RID: 22645 RVA: 0x001C46BD File Offset: 0x001C28BD
		[PunRPC]
		private void NotifyPartyMerging(int[] memberIDs, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyPartyMerging");
			if (memberIDs == null)
			{
				return;
			}
			if (memberIDs.Length > 10)
			{
				return;
			}
			this.partyMergeIDs[info.Sender.ActorNumber] = memberIDs;
		}

		// Token: 0x06005876 RID: 22646 RVA: 0x001C46F0 File Offset: 0x001C28F0
		[Rpc]
		private unsafe static void RPC_NotifyPartyMerging(NetworkRunner runner, [RpcTarget] PlayerRef playerRef, int[] memberIDs, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(playerRef);
					if (rpcTargetStatus == 0)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(playerRef, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == 1)
						{
							info = RpcInfo.FromLocal(runner, 0, 0);
							goto IL_10;
						}
						int num = 8;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)"));
							int num2 = 8;
							*(int*)(ptr2 + num2) = memberIDs.Length;
							num2 += 4;
							num2 = (Native.CopyFromArray<int>((void*)(ptr2 + num2), memberIDs) + 3 & -4) + num2;
							ptr.Offset = num2 * 8;
							ptr.SetTarget(playerRef);
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			if (memberIDs.Length > 10)
			{
				return;
			}
			FriendshipGroupDetection.Instance.partyMergeIDs[info.Source.PlayerId] = memberIDs;
		}

		// Token: 0x06005877 RID: 22647 RVA: 0x001C487C File Offset: 0x001C2A7C
		public void SendAboutToGroupJoin()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Sending group join to ",
					GorillaParent.instance.vrrigs.Count.ToString(),
					" players. Party member:",
					vrrig.OwningNetPlayer.NickName,
					"Is offline rig",
					vrrig.isOfflineVRRig.ToString()
				}));
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
				{
					this.photonView.RPC("PartyMemberIsAboutToGroupJoin", vrrig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
		}

		// Token: 0x06005878 RID: 22648 RVA: 0x001C4964 File Offset: 0x001C2B64
		[PunRPC]
		private void PartyMemberIsAboutToGroupJoin(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PartyMemberIsAboutToGroupJoin");
			this.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005879 RID: 22649 RVA: 0x001C4980 File Offset: 0x001C2B80
		[Rpc]
		private unsafe static void RPC_PartyMemberIsAboutToGroupJoin(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == 0)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == 1)
						{
							info = RpcInfo.FromLocal(runner, 0, 0);
							goto IL_10;
						}
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)"));
							int num2 = 8;
							ptr.Offset = num2 * 8;
							ptr.SetTarget(targetPlayer);
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600587A RID: 22650 RVA: 0x001C4AA0 File Offset: 0x001C2CA0
		private void PartMemberIsAboutToGroupJoinWrapped(PhotonMessageInfoWrapped wrappedInfo)
		{
			float time = Time.time;
			float num = this.aboutToGroupJoin_CooldownUntilTimestamp;
			if (wrappedInfo.senderID < NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.aboutToGroupJoin_CooldownUntilTimestamp = Time.time + 5f;
				if (this.myPartyMembersHash.Contains(wrappedInfo.Sender.UserId))
				{
					PhotonNetworkController.Instance.DeferJoining(2f);
				}
			}
		}

		// Token: 0x0600587B RID: 22651 RVA: 0x001C4B0C File Offset: 0x001C2D0C
		private void SendPartyFormedRPC(short braceletColor, int[] memberIDs, bool forceDebug)
		{
			string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (this.playersInProvisionalGroup.BinarySearch(vrrig.creator.ActorNumber) >= 0)
				{
					this.photonView.RPC("PartyFormedSuccessfully", vrrig.Creator.GetPlayerRef(), new object[]
					{
						text,
						braceletColor,
						memberIDs,
						forceDebug
					});
				}
			}
		}

		// Token: 0x0600587C RID: 22652 RVA: 0x001C4BD8 File Offset: 0x001C2DD8
		[Rpc]
		private unsafe static void RPC_PartyFormedSuccessfully(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == 0)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == 1)
						{
							info = RpcInfo.FromLocal(runner, 0, 0);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(partyGameMode) + 3 & -4);
						num += 4;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						num += 4;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)"));
							int num2 = 8;
							num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), partyGameMode) + 3 & -4) + num2;
							*(short*)(ptr2 + num2) = braceletColor;
							num2 += (2 + 3 & -4);
							*(int*)(ptr2 + num2) = memberIDs.Length;
							num2 += 4;
							num2 = (Native.CopyFromArray<int>((void*)(ptr2 + num2), memberIDs) + 3 & -4) + num2;
							ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), forceDebug);
							num2 += 4;
							ptr.Offset = num2 * 8;
							ptr.SetTarget(targetPlayer);
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			GorillaNot.IncrementRPCCall(info, "PartyFormedSuccessfully");
			FriendshipGroupDetection.Instance.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600587D RID: 22653 RVA: 0x001C4DF5 File Offset: 0x001C2FF5
		[PunRPC]
		private void PartyFormedSuccessfully(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PartyFormedSuccessfully");
			this.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600587E RID: 22654 RVA: 0x001C4E18 File Offset: 0x001C3018
		private void PartyFormedSuccesfullyWrapped(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfoWrapped info)
		{
			if (memberIDs == null || memberIDs.Length > 10 || !Enumerable.Contains<int>(memberIDs, info.Sender.ActorNumber) || this.playersInProvisionalGroup.IndexOf(info.Sender.ActorNumber) != 0 || Mathf.Abs(this.groupCreateAfterTimestamp - Time.time) > this.m_maxGroupJoinTimeDifference || !GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			if (this.IsInParty)
			{
				string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
					{
						this.photonView.RPC("AddPartyMembers", vrrig.Creator.GetPlayerRef(), new object[]
						{
							text,
							braceletColor,
							memberIDs
						});
					}
				}
			}
			this.suppressPartyCreationUntilTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			this.DidJoinLeftHanded = this.WillJoinLeftHanded;
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x0600587F RID: 22655 RVA: 0x001C4F60 File Offset: 0x001C3160
		[PunRPC]
		private void AddPartyMembers(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfo info)
		{
			this.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005880 RID: 22656 RVA: 0x001C4F74 File Offset: 0x001C3174
		[Rpc]
		private unsafe static void RPC_AddPartyMembers(NetworkRunner runner, [RpcTarget] PlayerRef rpcTarget, string partyGameMode, short braceletColor, int[] memberIDs, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(rpcTarget);
					if (rpcTargetStatus == 0)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(rpcTarget, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == 1)
						{
							info = RpcInfo.FromLocal(runner, 0, 0);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(partyGameMode) + 3 & -4);
						num += 4;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)"));
							int num2 = 8;
							num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), partyGameMode) + 3 & -4) + num2;
							*(short*)(ptr2 + num2) = braceletColor;
							num2 += (2 + 3 & -4);
							*(int*)(ptr2 + num2) = memberIDs.Length;
							num2 += 4;
							num2 = (Native.CopyFromArray<int>((void*)(ptr2 + num2), memberIDs) + 3 & -4) + num2;
							ptr.Offset = num2 * 8;
							ptr.SetTarget(rpcTarget);
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005881 RID: 22657 RVA: 0x001C5158 File Offset: 0x001C3358
		private void AddPartyMembersWrapped(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfoWrapped infoWrapped)
		{
			GorillaNot.IncrementRPCCall(infoWrapped, "AddPartyMembersWrapped");
			if (memberIDs.Length > 10 || !this.IsInParty || !this.myPartyMembersHash.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)) || !GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			Debug.Log("Adding party members: [" + string.Join<int>(",", memberIDs) + "]");
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x06005882 RID: 22658 RVA: 0x001C51D0 File Offset: 0x001C33D0
		private void SetNewParty(string partyGameMode, short braceletColor, int[] memberIDs)
		{
			GorillaComputer.instance.SetGameModeWithoutButton(partyGameMode);
			this.myPartyMemberIDs = new List<string>();
			FriendshipGroupDetection.userIdLookup.Clear();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				FriendshipGroupDetection.userIdLookup.Add(vrrig.creator.ActorNumber, vrrig.creator.UserId);
			}
			foreach (int num in memberIDs)
			{
				string text;
				if (FriendshipGroupDetection.userIdLookup.TryGetValue(num, ref text))
				{
					this.myPartyMemberIDs.Add(text);
				}
			}
			this.myBraceletColor = FriendshipGroupDetection.UnpackColor(braceletColor);
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
			this.OnPartyMembershipChanged();
			PlayerGameEvents.MiscEvent("FriendshipGroupJoined", 1);
		}

		// Token: 0x06005883 RID: 22659 RVA: 0x001C52D0 File Offset: 0x001C34D0
		public void LeaveParty()
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
				{
					this.photonView.RPC("PlayerLeftParty", vrrig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
			this.myPartyMemberIDs = null;
			this.OnPartyMembershipChanged();
			PhotonNetworkController.Instance.ClearDeferredJoin();
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005884 RID: 22660 RVA: 0x001C538C File Offset: 0x001C358C
		public void OnFailedToFollowParty()
		{
			if (!this.IsInParty)
			{
				return;
			}
			this.lastFailedToFollowPartyTime = (double)Time.time;
			this.wantsPartyRefreshPostFollowFailed = true;
		}

		// Token: 0x06005885 RID: 22661 RVA: 0x001C53AC File Offset: 0x001C35AC
		public void RefreshPartyMembers()
		{
			if (this.myPartyMemberIDs.IsNullOrEmpty<string>())
			{
				return;
			}
			Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] refreshing...");
			List<string> list = new List<string>(this.myPartyMemberIDs);
			Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] found " + string.Format("{0} current players in Room...", NetworkSystem.Instance.AllNetPlayers.Length));
			for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
			{
				if (NetworkSystem.Instance.AllNetPlayers[i] != null)
				{
					list.Remove(NetworkSystem.Instance.AllNetPlayers[i].UserId);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] removing missing player " + list[j] + " from party...");
				this.PlayerIDLeftParty(list[j]);
			}
			this.wantsPartyRefreshPostJoin = false;
			this.wantsPartyRefreshPostFollowFailed = false;
		}

		// Token: 0x06005886 RID: 22662 RVA: 0x001C548C File Offset: 0x001C368C
		[Rpc]
		private unsafe static void RPC_PlayerLeftParty(NetworkRunner runner, [RpcTarget] PlayerRef player, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(player);
					if (rpcTargetStatus == 0)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(player, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == 1)
						{
							info = RpcInfo.FromLocal(runner, 0, 0);
							goto IL_10;
						}
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)"));
							int num2 = 8;
							ptr.Offset = num2 * 8;
							ptr.SetTarget(player);
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			GorillaNot.IncrementRPCCall(info, "PlayerLeftParty");
			FriendshipGroupDetection.Instance.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005887 RID: 22663 RVA: 0x001C55BC File Offset: 0x001C37BC
		[PunRPC]
		private void PlayerLeftParty(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerLeftParty");
			this.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005888 RID: 22664 RVA: 0x001C55D8 File Offset: 0x001C37D8
		private void PlayerLeftPartyWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(infoWrapped.Sender.UserId))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005889 RID: 22665 RVA: 0x001C5640 File Offset: 0x001C3840
		private void PlayerIDLeftParty(string userID)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(userID))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x0600588A RID: 22666 RVA: 0x001C569C File Offset: 0x001C389C
		public void SendVerifyPartyMember(NetPlayer player)
		{
			this.photonView.RPC("VerifyPartyMember", player.GetPlayerRef(), Array.Empty<object>());
		}

		// Token: 0x0600588B RID: 22667 RVA: 0x001C56B9 File Offset: 0x001C38B9
		[PunRPC]
		private void VerifyPartyMember(PhotonMessageInfo info)
		{
			this.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600588C RID: 22668 RVA: 0x001C56C8 File Offset: 0x001C38C8
		[Rpc]
		private unsafe static void RPC_VerifyPartyMember(NetworkRunner runner, [RpcTarget] PlayerRef rpcTarget, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(rpcTarget);
					if (rpcTargetStatus == 0)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(rpcTarget, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == 1)
						{
							info = RpcInfo.FromLocal(runner, 0, 0);
							goto IL_10;
						}
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)"));
							int num2 = 8;
							ptr.Offset = num2 * 8;
							ptr.SetTarget(rpcTarget);
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600588D RID: 22669 RVA: 0x001C57E8 File Offset: 0x001C39E8
		private void VerifyPartyMemberWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			GorillaNot.IncrementRPCCall(infoWrapped, "VerifyPartyMemberWrapped");
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(infoWrapped.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 15, infoWrapped.SentServerTime))
			{
				return;
			}
			if (this.myPartyMemberIDs == null || !this.myPartyMemberIDs.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)))
			{
				this.photonView.RPC("PlayerLeftParty", infoWrapped.Sender.GetPlayerRef(), Array.Empty<object>());
			}
		}

		// Token: 0x0600588E RID: 22670 RVA: 0x001C5878 File Offset: 0x001C3A78
		public void SendRequestPartyGameMode(string gameMode)
		{
			int num = int.MaxValue;
			NetPlayer netPlayer = null;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && vrrig.creator.ActorNumber < num)
				{
					netPlayer = vrrig.creator;
					num = vrrig.creator.ActorNumber;
				}
			}
			if (netPlayer != null)
			{
				this.photonView.RPC("RequestPartyGameMode", netPlayer.GetPlayerRef(), new object[]
				{
					gameMode
				});
			}
		}

		// Token: 0x0600588F RID: 22671 RVA: 0x001C5920 File Offset: 0x001C3B20
		[Rpc]
		private unsafe static void RPC_RequestPartyGameMode(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string gameMode, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == 0)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == 1)
						{
							info = RpcInfo.FromLocal(runner, 0, 0);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(gameMode) + 3 & -4);
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)"));
							int num2 = 8;
							num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), gameMode) + 3 & -4) + num2;
							ptr.Offset = num2 * 8;
							ptr.SetTarget(targetPlayer);
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005890 RID: 22672 RVA: 0x001C5A7E File Offset: 0x001C3C7E
		[PunRPC]
		private void RequestPartyGameMode(string gameMode, PhotonMessageInfo info)
		{
			this.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005891 RID: 22673 RVA: 0x001C5A90 File Offset: 0x001C3C90
		private void RequestPartyGameModeWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestPartyGameModeWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember)
				{
					this.photonView.RPC("NotifyPartyGameModeChanged", vrrig.creator.GetPlayerRef(), new object[]
					{
						gameMode
					});
				}
			}
		}

		// Token: 0x06005892 RID: 22674 RVA: 0x001C5B40 File Offset: 0x001C3D40
		[Rpc]
		private unsafe static void RPC_NotifyPartyGameModeChanged(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string gameMode, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == 0)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == 1)
						{
							info = RpcInfo.FromLocal(runner, 0, 0);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(gameMode) + 3 & -4);
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)"));
							int num2 = 8;
							num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), gameMode) + 3 & -4) + num2;
							ptr.Offset = num2 * 8;
							ptr.SetTarget(targetPlayer);
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005893 RID: 22675 RVA: 0x001C5C9E File Offset: 0x001C3E9E
		[PunRPC]
		private void NotifyPartyGameModeChanged(string gameMode, PhotonMessageInfo info)
		{
			this.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005894 RID: 22676 RVA: 0x001C5CAD File Offset: 0x001C3EAD
		private void NotifyPartyGameModeChangedWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyPartyGameModeChangedWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			GorillaComputer.instance.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x06005895 RID: 22677 RVA: 0x001C5CEC File Offset: 0x001C3EEC
		private void OnPartyMembershipChanged()
		{
			this.myPartyMembersHash.Clear();
			if (this.myPartyMemberIDs != null)
			{
				foreach (string text in this.myPartyMemberIDs)
				{
					this.myPartyMembersHash.Add(text);
				}
			}
			this.myBeadColors.Clear();
			FriendshipGroupDetection.tempColorLookup.Clear();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				vrrig.ClearPartyMemberStatus();
				if (vrrig.IsLocalPartyMember)
				{
					FriendshipGroupDetection.tempColorLookup.Add(vrrig.Creator.UserId, vrrig.playerColor);
				}
			}
			this.MyBraceletSelfIndex = 0;
			if (this.myPartyMemberIDs != null)
			{
				using (List<string>.Enumerator enumerator = this.myPartyMemberIDs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text2 = enumerator.Current;
						Color color;
						if (FriendshipGroupDetection.tempColorLookup.TryGetValue(text2, ref color))
						{
							if (text2 == PhotonNetwork.LocalPlayer.UserId)
							{
								this.MyBraceletSelfIndex = this.myBeadColors.Count;
							}
							this.myBeadColors.Add(color);
						}
					}
					goto IL_168;
				}
			}
			GorillaComputer.instance.SetGameModeWithoutButton(GorillaComputer.instance.lastPressedGameMode);
			this.wantsPartyRefreshPostJoin = false;
			this.wantsPartyRefreshPostFollowFailed = false;
			IL_168:
			this.myBeadColors.Add(this.myBraceletColor);
			GorillaTagger.Instance.offlineVRRig.UpdateFriendshipBracelet();
			this.UpdateWarningSigns();
		}

		// Token: 0x06005896 RID: 22678 RVA: 0x001C5EB0 File Offset: 0x001C40B0
		public bool IsPartyWithinCollider(GorillaFriendCollider friendCollider)
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig && !friendCollider.playerIDsCurrentlyTouching.Contains(vrrig.Creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005897 RID: 22679 RVA: 0x001C5F34 File Offset: 0x001C4134
		public static short PackColor(Color col)
		{
			return (short)(Mathf.RoundToInt(col.r * 9f) + Mathf.RoundToInt(col.g * 9f) * 10 + Mathf.RoundToInt(col.b * 9f) * 100);
		}

		// Token: 0x06005898 RID: 22680 RVA: 0x001C5F74 File Offset: 0x001C4174
		public static Color UnpackColor(short data)
		{
			Color result = default(Color);
			result.r = (float)(data % 10) / 9f;
			result.g = (float)(data / 10 % 10) / 9f;
			result.b = (float)(data / 100 % 10) / 9f;
			return result;
		}

		// Token: 0x0600589B RID: 22683 RVA: 0x001C60D0 File Offset: 0x001C42D0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyNoPartyToMerge@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyNoPartyToMerge(runner, info);
		}

		// Token: 0x0600589C RID: 22684 RVA: 0x001C6114 File Offset: 0x001C4314
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyPartyMerging@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message.Target;
			int[] array = new int[*(int*)(ptr + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(ptr + num)) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyPartyMerging(runner, target, array, info);
		}

		// Token: 0x0600589D RID: 22685 RVA: 0x001C61A8 File Offset: 0x001C43A8
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PartyMemberIsAboutToGroupJoin@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			PlayerRef target = message.Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PartyMemberIsAboutToGroupJoin(runner, target, info);
		}

		// Token: 0x0600589E RID: 22686 RVA: 0x001C61F8 File Offset: 0x001C43F8
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PartyFormedSuccessfully@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message.Target;
			string partyGameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), ref partyGameMode) + 3 & -4) + num;
			short num2 = *(short*)(ptr + num);
			num += (2 + 3 & -4);
			short braceletColor = num2;
			int[] array = new int[*(int*)(ptr + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(ptr + num)) + 3 & -4) + num;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
			num += 4;
			bool forceDebug = flag;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PartyFormedSuccessfully(runner, target, partyGameMode, braceletColor, array, forceDebug, info);
		}

		// Token: 0x0600589F RID: 22687 RVA: 0x001C62F8 File Offset: 0x001C44F8
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_AddPartyMembers@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message.Target;
			string partyGameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), ref partyGameMode) + 3 & -4) + num;
			short num2 = *(short*)(ptr + num);
			num += (2 + 3 & -4);
			short braceletColor = num2;
			int[] array = new int[*(int*)(ptr + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(ptr + num)) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_AddPartyMembers(runner, target, partyGameMode, braceletColor, array, info);
		}

		// Token: 0x060058A0 RID: 22688 RVA: 0x001C63D8 File Offset: 0x001C45D8
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerLeftParty@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			PlayerRef target = message.Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PlayerLeftParty(runner, target, info);
		}

		// Token: 0x060058A1 RID: 22689 RVA: 0x001C6428 File Offset: 0x001C4628
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_VerifyPartyMember@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			PlayerRef target = message.Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_VerifyPartyMember(runner, target, info);
		}

		// Token: 0x060058A2 RID: 22690 RVA: 0x001C6478 File Offset: 0x001C4678
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_RequestPartyGameMode@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message.Target;
			string gameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), ref gameMode) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_RequestPartyGameMode(runner, target, gameMode, info);
		}

		// Token: 0x060058A3 RID: 22691 RVA: 0x001C64F0 File Offset: 0x001C46F0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyPartyGameModeChanged@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message.Target;
			string gameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), ref gameMode) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyPartyGameModeChanged(runner, target, gameMode, info);
		}

		// Token: 0x0400659E RID: 26014
		[SerializeField]
		private float detectionRadius = 0.5f;

		// Token: 0x0400659F RID: 26015
		[SerializeField]
		private float groupTime = 5f;

		// Token: 0x040065A0 RID: 26016
		[SerializeField]
		private float cooldownAfterCreatingGroup = 5f;

		// Token: 0x040065A1 RID: 26017
		[SerializeField]
		private float hapticStrength = 1.5f;

		// Token: 0x040065A2 RID: 26018
		[SerializeField]
		private float hapticDuration = 2f;

		// Token: 0x040065A3 RID: 26019
		[SerializeField]
		private double joinedRoomRefreshPartyDelay = 30.0;

		// Token: 0x040065A4 RID: 26020
		[SerializeField]
		private double failedToFollowRefreshPartyDelay = 30.0;

		// Token: 0x040065A5 RID: 26021
		public bool debug;

		// Token: 0x040065A6 RID: 26022
		public double offset = 0.5;

		// Token: 0x040065A7 RID: 26023
		[SerializeField]
		private float m_maxGroupJoinTimeDifference = 1f;

		// Token: 0x040065A8 RID: 26024
		private List<string> myPartyMemberIDs;

		// Token: 0x040065A9 RID: 26025
		private HashSet<string> myPartyMembersHash = new HashSet<string>();

		// Token: 0x040065AE RID: 26030
		private List<Action<GroupJoinZoneAB>> groupZoneCallbacks = new List<Action<GroupJoinZoneAB>>();

		// Token: 0x040065AF RID: 26031
		[SerializeField]
		private GTColor.HSVRanges braceletRandomColorHSVRanges;

		// Token: 0x040065B0 RID: 26032
		public GameObject friendshipBubble;

		// Token: 0x040065B1 RID: 26033
		public AudioClip fistBumpInterruptedAudio;

		// Token: 0x040065B2 RID: 26034
		private ParticleSystem particleSystem;

		// Token: 0x040065B3 RID: 26035
		private AudioSource audioSource;

		// Token: 0x040065B4 RID: 26036
		private double lastJoinedRoomTime;

		// Token: 0x040065B5 RID: 26037
		private bool wantsPartyRefreshPostJoin;

		// Token: 0x040065B6 RID: 26038
		private double lastFailedToFollowPartyTime;

		// Token: 0x040065B7 RID: 26039
		private bool wantsPartyRefreshPostFollowFailed;

		// Token: 0x040065B9 RID: 26041
		private Queue<FriendshipGroupDetection.PlayerFist> playersToPropagateFrom = new Queue<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x040065BA RID: 26042
		private List<int> playersInProvisionalGroup = new List<int>();

		// Token: 0x040065BB RID: 26043
		private List<int> provisionalGroupUsingLeftHands = new List<int>();

		// Token: 0x040065BC RID: 26044
		private List<int> tempIntList = new List<int>();

		// Token: 0x040065BD RID: 26045
		private bool amFirstProvisionalPlayer;

		// Token: 0x040065BE RID: 26046
		private Dictionary<int, int[]> partyMergeIDs = new Dictionary<int, int[]>();

		// Token: 0x040065BF RID: 26047
		private float groupCreateAfterTimestamp;

		// Token: 0x040065C0 RID: 26048
		private float playEffectsAfterTimestamp;

		// Token: 0x040065C1 RID: 26049
		[SerializeField]
		private float playEffectsDelay;

		// Token: 0x040065C2 RID: 26050
		private float suppressPartyCreationUntilTimestamp;

		// Token: 0x040065C4 RID: 26052
		private bool WillJoinLeftHanded;

		// Token: 0x040065C5 RID: 26053
		private List<FriendshipGroupDetection.PlayerFist> playersMakingFists = new List<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x040065C6 RID: 26054
		private StringBuilder debugStr = new StringBuilder();

		// Token: 0x040065C7 RID: 26055
		private float aboutToGroupJoin_CooldownUntilTimestamp;

		// Token: 0x040065C8 RID: 26056
		private static Dictionary<int, string> userIdLookup = new Dictionary<int, string>();

		// Token: 0x040065C9 RID: 26057
		private static Dictionary<string, Color> tempColorLookup = new Dictionary<string, Color>();

		// Token: 0x02000DE4 RID: 3556
		private struct PlayerFist
		{
			// Token: 0x040065CA RID: 26058
			public int actorNumber;

			// Token: 0x040065CB RID: 26059
			public Vector3 position;

			// Token: 0x040065CC RID: 26060
			public bool isLeftHand;
		}
	}
}
