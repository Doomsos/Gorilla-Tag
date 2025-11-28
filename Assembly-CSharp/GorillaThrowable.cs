using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000930 RID: 2352
public class GorillaThrowable : MonoBehaviourPun, IPunObservable, IPhotonViewCallback
{
	// Token: 0x06003C1F RID: 15391 RVA: 0x0013D4F8 File Offset: 0x0013B6F8
	public virtual void Start()
	{
		this.offset = Vector3.zero;
		this.headsetTransform = GTPlayer.Instance.headCollider.transform;
		this.velocityHistory = new Vector3[this.trackingHistorySize];
		this.positionHistory = new Vector3[this.trackingHistorySize];
		this.headsetPositionHistory = new Vector3[this.trackingHistorySize];
		this.rotationHistory = new Vector3[this.trackingHistorySize];
		this.rotationalVelocityHistory = new Vector3[this.trackingHistorySize];
		for (int i = 0; i < this.trackingHistorySize; i++)
		{
			this.velocityHistory[i] = Vector3.zero;
			this.positionHistory[i] = base.transform.position - this.headsetTransform.position;
			this.headsetPositionHistory[i] = this.headsetTransform.position;
			this.rotationHistory[i] = base.transform.eulerAngles;
			this.rotationalVelocityHistory[i] = Vector3.zero;
		}
		this.currentIndex = 0;
		this.rigidbody = base.GetComponentInChildren<Rigidbody>();
	}

	// Token: 0x06003C20 RID: 15392 RVA: 0x0013D618 File Offset: 0x0013B818
	public virtual void LateUpdate()
	{
		if (this.isHeld && base.photonView.IsMine)
		{
			base.transform.rotation = this.transformToFollow.rotation * this.offsetRotation;
			if (!this.initialLerp && (base.transform.position - this.transformToFollow.position).magnitude > this.lerpDistanceLimit)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.transformToFollow.position + this.transformToFollow.rotation * this.offset, this.pickupLerp);
			}
			else
			{
				this.initialLerp = true;
				base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
			}
		}
		if (!base.photonView.IsMine)
		{
			this.rigidbody.isKinematic = true;
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.lerpValue);
		}
		this.StoreHistories();
	}

	// Token: 0x06003C21 RID: 15393 RVA: 0x00002789 File Offset: 0x00000989
	private void IsHandPushing(XRNode node)
	{
	}

	// Token: 0x06003C22 RID: 15394 RVA: 0x0013D784 File Offset: 0x0013B984
	private void StoreHistories()
	{
		this.previousPosition = this.positionHistory[this.currentIndex];
		this.previousRotation = this.rotationHistory[this.currentIndex];
		this.previousHeadsetPosition = this.headsetPositionHistory[this.currentIndex];
		this.currentIndex = (this.currentIndex + 1) % this.trackingHistorySize;
		this.currentVelocity = (base.transform.position - this.headsetTransform.position - this.previousPosition) / Time.deltaTime;
		this.currentHeadsetVelocity = (this.headsetTransform.position - this.previousHeadsetPosition) / Time.deltaTime;
		this.currentRotationalVelocity = (base.transform.eulerAngles - this.previousRotation) / Time.deltaTime;
		this.denormalizedVelocityAverage = Vector3.zero;
		this.denormalizedRotationalVelocityAverage = Vector3.zero;
		this.loopIndex = 0;
		while (this.loopIndex < this.trackingHistorySize)
		{
			this.denormalizedVelocityAverage += this.velocityHistory[this.loopIndex];
			this.denormalizedRotationalVelocityAverage += this.rotationalVelocityHistory[this.loopIndex];
			this.loopIndex++;
		}
		this.denormalizedVelocityAverage /= (float)this.trackingHistorySize;
		this.denormalizedRotationalVelocityAverage /= (float)this.trackingHistorySize;
		this.velocityHistory[this.currentIndex] = this.currentVelocity;
		this.positionHistory[this.currentIndex] = base.transform.position - this.headsetTransform.position;
		this.headsetPositionHistory[this.currentIndex] = this.headsetTransform.position;
		this.rotationHistory[this.currentIndex] = base.transform.eulerAngles;
		this.rotationalVelocityHistory[this.currentIndex] = this.currentRotationalVelocity;
	}

	// Token: 0x06003C23 RID: 15395 RVA: 0x0013D9B0 File Offset: 0x0013BBB0
	public virtual void Grabbed(Transform grabTransform)
	{
		this.grabbingTransform = grabTransform;
		this.isHeld = true;
		this.transformToFollow = this.grabbingTransform;
		this.offsetRotation = base.transform.rotation * Quaternion.Inverse(this.transformToFollow.rotation);
		this.initialLerp = false;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		base.photonView.RequestOwnership();
	}

	// Token: 0x06003C24 RID: 15396 RVA: 0x0013DA28 File Offset: 0x0013BC28
	public virtual void ThrowThisThingo()
	{
		this.transformToFollow = null;
		this.isHeld = false;
		this.synchThrow = true;
		this.rigidbody.interpolation = 1;
		this.rigidbody.isKinematic = false;
		this.rigidbody.useGravity = true;
		if (this.isLinear || this.denormalizedVelocityAverage.magnitude < this.linearMax)
		{
			if (this.denormalizedVelocityAverage.magnitude * this.throwMultiplier < this.throwMagnitudeLimit)
			{
				this.rigidbody.linearVelocity = this.denormalizedVelocityAverage * this.throwMultiplier + this.currentHeadsetVelocity;
			}
			else
			{
				this.rigidbody.linearVelocity = this.denormalizedVelocityAverage.normalized * this.throwMagnitudeLimit + this.currentHeadsetVelocity;
			}
		}
		else
		{
			this.rigidbody.linearVelocity = this.denormalizedVelocityAverage.normalized * Mathf.Max(Mathf.Min(Mathf.Pow(this.throwMultiplier * this.denormalizedVelocityAverage.magnitude / this.linearMax, this.exponThrowMultMax), 0.1f) * this.denormalizedHeadsetVelocityAverage.magnitude, this.throwMagnitudeLimit) + this.currentHeadsetVelocity;
		}
		this.rigidbody.angularVelocity = this.denormalizedRotationalVelocityAverage * 3.1415927f / 180f;
		this.rigidbody.MovePosition(this.rigidbody.transform.position + this.rigidbody.linearVelocity * Time.deltaTime);
	}

	// Token: 0x06003C25 RID: 15397 RVA: 0x0013DBC4 File Offset: 0x0013BDC4
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(this.rigidbody.linearVelocity);
			return;
		}
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.targetPosition.SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		ref this.targetRotation.SetValueSafe(quaternion);
		Vector3 linearVelocity = this.rigidbody.linearVelocity;
		vector = (Vector3)stream.ReceiveNext();
		ref linearVelocity.SetValueSafe(vector);
		this.rigidbody.linearVelocity = linearVelocity;
	}

	// Token: 0x06003C26 RID: 15398 RVA: 0x0013DC7C File Offset: 0x0013BE7C
	public virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<GorillaSurfaceOverride>() != null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("PlaySurfaceHit", 1, new object[]
				{
					this.bounceAudioClip,
					this.InterpolateVolume()
				});
			}
			this.PlaySurfaceHit(collision.collider.GetComponent<GorillaSurfaceOverride>().overrideIndex, this.InterpolateVolume());
		}
	}

	// Token: 0x06003C27 RID: 15399 RVA: 0x0013DCF8 File Offset: 0x0013BEF8
	public void PlaySurfaceHit(int soundIndex, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			this.audioSource.volume = tapVolume;
			this.audioSource.clip = (GTPlayer.Instance.materialData[soundIndex].overrideAudio ? GTPlayer.Instance.materialData[soundIndex].audio : GTPlayer.Instance.materialData[0].audio);
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
		}
	}

	// Token: 0x06003C28 RID: 15400 RVA: 0x0013DD94 File Offset: 0x0013BF94
	public float InterpolateVolume()
	{
		return (Mathf.Clamp(this.rigidbody.linearVelocity.magnitude, this.minVelocity, this.maxVelocity) - this.minVelocity) / (this.maxVelocity - this.minVelocity) * (this.maxVolume - this.minVolume) + this.minVolume;
	}

	// Token: 0x04004CA3 RID: 19619
	public int trackingHistorySize;

	// Token: 0x04004CA4 RID: 19620
	public float throwMultiplier;

	// Token: 0x04004CA5 RID: 19621
	public float throwMagnitudeLimit;

	// Token: 0x04004CA6 RID: 19622
	private Vector3[] velocityHistory;

	// Token: 0x04004CA7 RID: 19623
	private Vector3[] headsetVelocityHistory;

	// Token: 0x04004CA8 RID: 19624
	private Vector3[] positionHistory;

	// Token: 0x04004CA9 RID: 19625
	private Vector3[] headsetPositionHistory;

	// Token: 0x04004CAA RID: 19626
	private Vector3[] rotationHistory;

	// Token: 0x04004CAB RID: 19627
	private Vector3[] rotationalVelocityHistory;

	// Token: 0x04004CAC RID: 19628
	private Vector3 previousPosition;

	// Token: 0x04004CAD RID: 19629
	private Vector3 previousRotation;

	// Token: 0x04004CAE RID: 19630
	private Vector3 previousHeadsetPosition;

	// Token: 0x04004CAF RID: 19631
	private int currentIndex;

	// Token: 0x04004CB0 RID: 19632
	private Vector3 currentVelocity;

	// Token: 0x04004CB1 RID: 19633
	private Vector3 currentHeadsetVelocity;

	// Token: 0x04004CB2 RID: 19634
	private Vector3 currentRotationalVelocity;

	// Token: 0x04004CB3 RID: 19635
	public Vector3 denormalizedVelocityAverage;

	// Token: 0x04004CB4 RID: 19636
	private Vector3 denormalizedHeadsetVelocityAverage;

	// Token: 0x04004CB5 RID: 19637
	private Vector3 denormalizedRotationalVelocityAverage;

	// Token: 0x04004CB6 RID: 19638
	private Transform headsetTransform;

	// Token: 0x04004CB7 RID: 19639
	private Vector3 targetPosition;

	// Token: 0x04004CB8 RID: 19640
	private Quaternion targetRotation;

	// Token: 0x04004CB9 RID: 19641
	public bool initialLerp;

	// Token: 0x04004CBA RID: 19642
	public float lerpValue = 0.4f;

	// Token: 0x04004CBB RID: 19643
	public float lerpDistanceLimit = 0.01f;

	// Token: 0x04004CBC RID: 19644
	public bool isHeld;

	// Token: 0x04004CBD RID: 19645
	public Rigidbody rigidbody;

	// Token: 0x04004CBE RID: 19646
	private int loopIndex;

	// Token: 0x04004CBF RID: 19647
	private Transform transformToFollow;

	// Token: 0x04004CC0 RID: 19648
	private Vector3 offset;

	// Token: 0x04004CC1 RID: 19649
	private Quaternion offsetRotation;

	// Token: 0x04004CC2 RID: 19650
	public AudioSource audioSource;

	// Token: 0x04004CC3 RID: 19651
	public int timeLastReceived;

	// Token: 0x04004CC4 RID: 19652
	public bool synchThrow;

	// Token: 0x04004CC5 RID: 19653
	public float tempFloat;

	// Token: 0x04004CC6 RID: 19654
	public Transform grabbingTransform;

	// Token: 0x04004CC7 RID: 19655
	public float pickupLerp;

	// Token: 0x04004CC8 RID: 19656
	public float minVelocity;

	// Token: 0x04004CC9 RID: 19657
	public float maxVelocity;

	// Token: 0x04004CCA RID: 19658
	public float minVolume;

	// Token: 0x04004CCB RID: 19659
	public float maxVolume;

	// Token: 0x04004CCC RID: 19660
	public bool isLinear;

	// Token: 0x04004CCD RID: 19661
	public float linearMax;

	// Token: 0x04004CCE RID: 19662
	public float exponThrowMultMax;

	// Token: 0x04004CCF RID: 19663
	public int bounceAudioClip;
}
