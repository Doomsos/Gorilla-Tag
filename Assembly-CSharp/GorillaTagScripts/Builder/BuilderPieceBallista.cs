using System;
using System.Collections;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E4B RID: 3659
	public class BuilderPieceBallista : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06005B3D RID: 23357 RVA: 0x001D3C14 File Offset: 0x001D1E14
		private void Awake()
		{
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerPressed));
			}
			this.hasLaunchParticles = (this.launchParticles != null);
		}

		// Token: 0x06005B3E RID: 23358 RVA: 0x001D3CDA File Offset: 0x001D1EDA
		private void OnDestroy()
		{
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerPressed));
			}
		}

		// Token: 0x06005B3F RID: 23359 RVA: 0x001D3D06 File Offset: 0x001D1F06
		private void OnHandTriggerPressed()
		{
			if (this.autoLaunch)
			{
				return;
			}
			if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 4);
			}
		}

		// Token: 0x06005B40 RID: 23360 RVA: 0x001D3D3C File Offset: 0x001D1F3C
		private void UpdateStateMaster()
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
			switch (this.ballistaState)
			{
			case BuilderPieceBallista.BallistaState.Idle:
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			case BuilderPieceBallista.BallistaState.Loading:
				if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash && (double)Time.time > this.loadCompleteTime)
				{
					if (this.playerInTrigger && this.playerRigInTrigger != null && (this.launchBigMonkes || (double)this.playerRigInTrigger.scaleFactor < 0.99))
					{
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ballistaState = BuilderPieceBallista.BallistaState.WaitingForTrigger;
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.WaitingForTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					return;
				}
				if (this.playerInTrigger)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PlayerInTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (this.autoLaunch && (double)Time.time > this.enteredTriggerTime + (double)this.autoLaunchDelay)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PrepareForLaunch:
			case BuilderPieceBallista.BallistaState.PrepareForLaunchLocal:
			{
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ResetFlags();
					this.myPiece.functionalPieceState = 0;
					this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
					return;
				}
				Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(this.playerRigInTrigger.transform, this.playerRigInTrigger.scaleFactor);
				Vector3 vector = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 vector2 = playerBodyCenterPosition - vector;
				if (Vector3.Lerp(Vector3.zero, vector2, Mathf.Exp(-this.playerPullInRate * Time.deltaTime)).sqrMagnitude < this.playerReadyToFireDist * this.myPiece.GetScale() * this.playerReadyToFireDist * this.myPiece.GetScale())
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 6, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			}
			case BuilderPieceBallista.BallistaState.Launching:
			case BuilderPieceBallista.BallistaState.LaunchingLocal:
				if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005B41 RID: 23361 RVA: 0x001D414B File Offset: 0x001D234B
		private void ResetFlags()
		{
			this.playerLaunched = false;
			this.loadCompleteTime = double.MaxValue;
		}

		// Token: 0x06005B42 RID: 23362 RVA: 0x001D4164 File Offset: 0x001D2364
		private void UpdatePlayerPosition()
		{
			if (this.ballistaState != BuilderPieceBallista.BallistaState.PrepareForLaunchLocal && this.ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			float deltaTime = Time.deltaTime;
			GTPlayer instance = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale);
			Vector3 vector = playerBodyCenterPosition - this.launchStart.position;
			BuilderPieceBallista.BallistaState ballistaState = this.ballistaState;
			if (ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				Vector3 vector2 = Vector3.Dot(vector, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 vector3 = playerBodyCenterPosition - vector2;
				Vector3 vector4 = Vector3.Lerp(Vector3.zero, vector3, Mathf.Exp(-this.playerPullInRate * deltaTime));
				instance.transform.position = instance.transform.position + (vector4 - vector3);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				return;
			}
			if (ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			if (!this.playerLaunched)
			{
				float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
				float num2 = Vector3.Dot(vector, this.launchDirection) / this.launchRampDistance;
				float num3 = 0.25f * this.myPiece.GetScale() / this.launchRampDistance;
				float num4 = Mathf.Max(num + num3, num2);
				float num5 = num4 * this.launchRampDistance;
				Vector3 vector5 = this.launchDirection * num5 + this.launchStart.position;
				instance.transform.position + (vector5 - playerBodyCenterPosition);
				instance.transform.position = instance.transform.position + (vector5 - playerBodyCenterPosition);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				if (num4 >= 1f)
				{
					this.playerLaunched = true;
					this.launchedTime = (double)Time.time;
					instance.SetPlayerVelocity(this.launchSpeed * this.myPiece.GetScale() * this.launchDirection);
					instance.SetMaximumSlipThisFrame();
					return;
				}
			}
			else if ((double)Time.time < this.launchedTime + (double)this.slipOverrideDuration)
			{
				instance.SetMaximumSlipThisFrame();
			}
		}

		// Token: 0x06005B43 RID: 23363 RVA: 0x001D439C File Offset: 0x001D259C
		private Vector3 GetPlayerBodyCenterPosition(Transform headTransform, float playerScale)
		{
			return headTransform.position + Quaternion.Euler(0f, headTransform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, this.playerBodyOffsetFromHead.z * playerScale) + Vector3.down * (this.playerBodyOffsetFromHead.y * playerScale);
		}

		// Token: 0x06005B44 RID: 23364 RVA: 0x001D4414 File Offset: 0x001D2614
		private void OnTriggerEnter(Collider other)
		{
			if (this.playerRigInTrigger != null)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.launchBigMonkes && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			this.playerRigInTrigger = vrrig;
			this.playerInTrigger = true;
		}

		// Token: 0x06005B45 RID: 23365 RVA: 0x001D44B4 File Offset: 0x001D26B4
		private void OnTriggerExit(Collider other)
		{
			if (this.playerRigInTrigger == null || !this.playerInTrigger)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (this.playerRigInTrigger.Equals(vrrig))
			{
				this.playerInTrigger = false;
				this.playerRigInTrigger = null;
			}
		}

		// Token: 0x06005B46 RID: 23366 RVA: 0x001D454C File Offset: 0x001D274C
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			if (!this.myPiece.GetTable().isTableMutable)
			{
				this.launchBigMonkes = true;
			}
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.playerLaunched = false;
		}

		// Token: 0x06005B47 RID: 23367 RVA: 0x001D4583 File Offset: 0x001D2783
		public void OnPieceDestroy()
		{
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
		}

		// Token: 0x06005B48 RID: 23368 RVA: 0x001D4598 File Offset: 0x001D2798
		public void OnPiecePlacementDeserialized()
		{
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
		}

		// Token: 0x06005B49 RID: 23369 RVA: 0x001D45F0 File Offset: 0x001D27F0
		public void OnPieceActivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = true;
			}
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x06005B4A RID: 23370 RVA: 0x001D46C0 File Offset: 0x001D28C0
		public void OnPieceDeactivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = false;
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Stop();
				this.launchParticles.Clear();
			}
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.ResetFlags();
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
		}

		// Token: 0x06005B4B RID: 23371 RVA: 0x001D4768 File Offset: 0x001D2968
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				return;
			}
			if ((BuilderPieceBallista.BallistaState)newState == this.ballistaState)
			{
				return;
			}
			if (newState == 4)
			{
				if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger && this.playerInTrigger && this.playerRigInTrigger != null)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), timeStamp);
					return;
				}
			}
			else
			{
				Debug.LogWarning("BuilderPiece Ballista unexpected state request for " + newState.ToString());
			}
		}

		// Token: 0x06005B4C RID: 23372 RVA: 0x001D4808 File Offset: 0x001D2A08
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			BuilderPieceBallista.BallistaState ballistaState = (BuilderPieceBallista.BallistaState)newState;
			if (ballistaState == this.ballistaState)
			{
				return;
			}
			switch (newState)
			{
			case 0:
				this.ResetFlags();
				goto IL_2C2;
			case 1:
				this.ResetFlags();
				foreach (Collider collider in this.disableWhileLaunching)
				{
					collider.enabled = true;
				}
				if (this.ballistaState == BuilderPieceBallista.BallistaState.Launching || this.ballistaState == BuilderPieceBallista.BallistaState.LaunchingLocal)
				{
					this.loadCompleteTime = (double)(Time.time + this.reloadDelay);
					if (this.loadSFX != null)
					{
						this.loadSFX.Play();
					}
				}
				else
				{
					this.loadCompleteTime = (double)(Time.time + this.loadTime);
				}
				this.animator.SetTrigger(this.loadTriggerHash);
				goto IL_2C2;
			case 2:
			case 5:
				goto IL_2C2;
			case 3:
				this.enteredTriggerTime = (double)Time.time;
				if (this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
					goto IL_2C2;
				}
				goto IL_2C2;
			case 4:
			{
				this.playerLaunched = false;
				if (!this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
				}
				if (!instigator.IsLocal)
				{
					goto IL_2C2;
				}
				GTPlayer instance = GTPlayer.Instance;
				if (Vector3.Distance(this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale), this.launchStart.position) > this.prepareForLaunchDistance * this.myPiece.GetScale() || (!this.launchBigMonkes && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor >= 0.99))
				{
					goto IL_2C2;
				}
				ballistaState = BuilderPieceBallista.BallistaState.PrepareForLaunchLocal;
				using (List<Collider>.Enumerator enumerator = this.disableWhileLaunching.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Collider collider2 = enumerator.Current;
						collider2.enabled = false;
					}
					goto IL_2C2;
				}
				break;
			}
			case 6:
				break;
			default:
				goto IL_2C2;
			}
			this.playerLaunched = false;
			this.animator.SetTrigger(this.fireTriggerHash);
			if (this.launchSFX != null)
			{
				this.launchSFX.Play();
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Play();
			}
			if (this.debugDrawTrajectoryOnLaunch)
			{
				base.StartCoroutine(this.DebugDrawTrajectory(8f));
			}
			if (instigator.IsLocal && this.ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				ballistaState = BuilderPieceBallista.BallistaState.LaunchingLocal;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
			}
			IL_2C2:
			this.ballistaState = ballistaState;
		}

		// Token: 0x06005B4D RID: 23373 RVA: 0x001D4AFC File Offset: 0x001D2CFC
		public bool IsStateValid(byte state)
		{
			return state < 8;
		}

		// Token: 0x06005B4E RID: 23374 RVA: 0x001D4B02 File Offset: 0x001D2D02
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece == null || this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.UpdateStateMaster();
			}
			this.UpdatePlayerPosition();
		}

		// Token: 0x06005B4F RID: 23375 RVA: 0x001D4B38 File Offset: 0x001D2D38
		private void UpdatePredictionLine()
		{
			float num = 0.033333335f;
			Vector3 vector = this.launchEnd.position;
			Vector3 vector2 = (this.launchEnd.position - this.launchStart.position).normalized * this.launchSpeed;
			for (int i = 0; i < 240; i++)
			{
				this.predictionLinePoints[i] = vector;
				vector += vector2 * num;
				vector2 += Vector3.down * 9.8f * num;
			}
		}

		// Token: 0x06005B50 RID: 23376 RVA: 0x001D4BD2 File Offset: 0x001D2DD2
		private IEnumerator DebugDrawTrajectory(float duration)
		{
			this.UpdatePredictionLine();
			float startTime = Time.time;
			while (Time.time < startTime + duration)
			{
				DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
				DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
				yield return null;
			}
			yield break;
		}

		// Token: 0x04006863 RID: 26723
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04006864 RID: 26724
		[SerializeField]
		private List<Collider> triggers;

		// Token: 0x04006865 RID: 26725
		[SerializeField]
		private List<Collider> disableWhileLaunching;

		// Token: 0x04006866 RID: 26726
		[Tooltip("Trigger to start the launch if not autoLaunch")]
		[SerializeField]
		private BuilderSmallHandTrigger handTrigger;

		// Token: 0x04006867 RID: 26727
		[Tooltip("Should the player launch without a hand trigger press")]
		[SerializeField]
		private bool autoLaunch;

		// Token: 0x04006868 RID: 26728
		[SerializeField]
		private float autoLaunchDelay = 0.75f;

		// Token: 0x04006869 RID: 26729
		private double enteredTriggerTime;

		// Token: 0x0400686A RID: 26730
		public Animator animator;

		// Token: 0x0400686B RID: 26731
		public Transform launchStart;

		// Token: 0x0400686C RID: 26732
		public Transform launchEnd;

		// Token: 0x0400686D RID: 26733
		public Transform launchBone;

		// Token: 0x0400686E RID: 26734
		[SerializeField]
		private SoundBankPlayer loadSFX;

		// Token: 0x0400686F RID: 26735
		[SerializeField]
		private SoundBankPlayer launchSFX;

		// Token: 0x04006870 RID: 26736
		[SerializeField]
		private SoundBankPlayer cockSFX;

		// Token: 0x04006871 RID: 26737
		[SerializeField]
		private ParticleSystem launchParticles;

		// Token: 0x04006872 RID: 26738
		private bool hasLaunchParticles;

		// Token: 0x04006873 RID: 26739
		public float reloadDelay = 1f;

		// Token: 0x04006874 RID: 26740
		public float loadTime = 1.933f;

		// Token: 0x04006875 RID: 26741
		public float slipOverrideDuration = 0.1f;

		// Token: 0x04006876 RID: 26742
		private double launchedTime;

		// Token: 0x04006877 RID: 26743
		public float playerMagnetismStrength = 3f;

		// Token: 0x04006878 RID: 26744
		[Tooltip("Speed will be scaled by piece scale")]
		public float launchSpeed = 20f;

		// Token: 0x04006879 RID: 26745
		[Range(0f, 1f)]
		public float pitch;

		// Token: 0x0400687A RID: 26746
		private bool debugDrawTrajectoryOnLaunch;

		// Token: 0x0400687B RID: 26747
		private int loadTriggerHash = Animator.StringToHash("Load");

		// Token: 0x0400687C RID: 26748
		private int fireTriggerHash = Animator.StringToHash("Fire");

		// Token: 0x0400687D RID: 26749
		private int pitchParamHash = Animator.StringToHash("Pitch");

		// Token: 0x0400687E RID: 26750
		private int idleStateHash = Animator.StringToHash("Idle");

		// Token: 0x0400687F RID: 26751
		private int loadStateHash = Animator.StringToHash("Load");

		// Token: 0x04006880 RID: 26752
		private int fireStateHash = Animator.StringToHash("Fire");

		// Token: 0x04006881 RID: 26753
		private bool playerInTrigger;

		// Token: 0x04006882 RID: 26754
		private VRRig playerRigInTrigger;

		// Token: 0x04006883 RID: 26755
		private bool playerLaunched;

		// Token: 0x04006884 RID: 26756
		private float playerReadyToFireDist = 1.6667f;

		// Token: 0x04006885 RID: 26757
		private float prepareForLaunchDistance = 2.5f;

		// Token: 0x04006886 RID: 26758
		private Vector3 launchDirection;

		// Token: 0x04006887 RID: 26759
		private float launchRampDistance;

		// Token: 0x04006888 RID: 26760
		private float playerPullInRate;

		// Token: 0x04006889 RID: 26761
		private float appliedAnimatorPitch;

		// Token: 0x0400688A RID: 26762
		private bool launchBigMonkes;

		// Token: 0x0400688B RID: 26763
		private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

		// Token: 0x0400688C RID: 26764
		private double loadCompleteTime;

		// Token: 0x0400688D RID: 26765
		private BuilderPieceBallista.BallistaState ballistaState;

		// Token: 0x0400688E RID: 26766
		private const int predictionLineSamples = 240;

		// Token: 0x0400688F RID: 26767
		private Vector3[] predictionLinePoints = new Vector3[240];

		// Token: 0x02000E4C RID: 3660
		private enum BallistaState
		{
			// Token: 0x04006891 RID: 26769
			Idle,
			// Token: 0x04006892 RID: 26770
			Loading,
			// Token: 0x04006893 RID: 26771
			WaitingForTrigger,
			// Token: 0x04006894 RID: 26772
			PlayerInTrigger,
			// Token: 0x04006895 RID: 26773
			PrepareForLaunch,
			// Token: 0x04006896 RID: 26774
			PrepareForLaunchLocal,
			// Token: 0x04006897 RID: 26775
			Launching,
			// Token: 0x04006898 RID: 26776
			LaunchingLocal,
			// Token: 0x04006899 RID: 26777
			Count
		}
	}
}
