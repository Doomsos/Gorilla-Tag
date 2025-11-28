using System;
using ExitGames.Client.Photon;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200092C RID: 2348
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GorillaEnemyAI : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x06003C06 RID: 15366 RVA: 0x0013CEF0 File Offset: 0x0013B0F0
	private void Start()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.r = base.GetComponent<Rigidbody>();
		this.r.useGravity = true;
		if (!base.photonView.IsMine)
		{
			this.agent.enabled = false;
			this.r.isKinematic = true;
		}
	}

	// Token: 0x06003C07 RID: 15367 RVA: 0x0013CF48 File Offset: 0x0013B148
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.eulerAngles);
			return;
		}
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.targetPosition.SetValueSafe(vector);
		vector = (Vector3)stream.ReceiveNext();
		ref this.targetRotation.SetValueSafe(vector);
	}

	// Token: 0x06003C08 RID: 15368 RVA: 0x0013CFBC File Offset: 0x0013B1BC
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.FindClosestPlayer();
			if (this.playerTransform != null)
			{
				this.agent.destination = this.playerTransform.position;
			}
			base.transform.LookAt(new Vector3(this.playerTransform.transform.position.x, base.transform.position.y, this.playerTransform.position.z));
			this.r.linearVelocity *= 0.99f;
			return;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
		base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, this.targetRotation, this.lerpValue);
	}

	// Token: 0x06003C09 RID: 15369 RVA: 0x0013D0AC File Offset: 0x0013B2AC
	private void FindClosestPlayer()
	{
		VRRig[] array = Object.FindObjectsByType<VRRig>(0);
		VRRig vrrig = null;
		float num = 100000f;
		foreach (VRRig vrrig2 in array)
		{
			Vector3 vector = vrrig2.transform.position - base.transform.position;
			if (vector.magnitude < num)
			{
				vrrig = vrrig2;
				num = vector.magnitude;
			}
		}
		this.playerTransform = vrrig.transform;
	}

	// Token: 0x06003C0A RID: 15370 RVA: 0x0013D11E File Offset: 0x0013B31E
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 19)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	// Token: 0x06003C0B RID: 15371 RVA: 0x0013D13A File Offset: 0x0013B33A
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.agent.enabled = true;
			this.r.isKinematic = false;
		}
	}

	// Token: 0x06003C0C RID: 15372 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06003C0D RID: 15373 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06003C0E RID: 15374 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06003C0F RID: 15375 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x04004C92 RID: 19602
	public Transform playerTransform;

	// Token: 0x04004C93 RID: 19603
	private NavMeshAgent agent;

	// Token: 0x04004C94 RID: 19604
	private Rigidbody r;

	// Token: 0x04004C95 RID: 19605
	private Vector3 targetPosition;

	// Token: 0x04004C96 RID: 19606
	private Vector3 targetRotation;

	// Token: 0x04004C97 RID: 19607
	public float lerpValue;
}
