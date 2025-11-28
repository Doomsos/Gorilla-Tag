using System;
using System.Collections.Generic;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics.Summer
{
	// Token: 0x0200112D RID: 4397
	public class Projectile : MonoBehaviour, IProjectile
	{
		// Token: 0x06006DFA RID: 28154 RVA: 0x00241B2D File Offset: 0x0023FD2D
		protected void Awake()
		{
			this.rigidbody = base.GetComponentInChildren<Rigidbody>();
			this.impactEffectSpawned = false;
			this.forceComponent = base.GetComponent<ConstantForce>();
		}

		// Token: 0x06006DFB RID: 28155 RVA: 0x00002789 File Offset: 0x00000989
		protected void OnEnable()
		{
		}

		// Token: 0x06006DFC RID: 28156 RVA: 0x00241B50 File Offset: 0x0023FD50
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progressStep)
		{
			Transform transform = base.transform;
			transform.SetPositionAndRotation(startPosition, startRotation);
			transform.localScale = Vector3.one * ownerRig.scaleFactor;
			if (this.rigidbody != null)
			{
				this.rigidbody.linearVelocity = velocity;
			}
			if (this.audioSource && this.launchAudio)
			{
				this.audioSource.GTPlayOneShot(this.launchAudio, 1f);
			}
			UnityEvent<float> unityEvent = this.onLaunchShared;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(chargeFrac);
		}

		// Token: 0x06006DFD RID: 28157 RVA: 0x00241BDD File Offset: 0x0023FDDD
		private bool IsTagValid(GameObject obj)
		{
			return this.collisionTags.Contains(obj.tag);
		}

		// Token: 0x06006DFE RID: 28158 RVA: 0x00241BF0 File Offset: 0x0023FDF0
		private void HandleImpact(GameObject hitObject, Vector3 hitPosition, Vector3 hitNormal)
		{
			if (this.impactEffectSpawned)
			{
				return;
			}
			if (this.collisionTags.Count > 0 && !this.IsTagValid(hitObject))
			{
				return;
			}
			if ((1 << hitObject.layer & this.collisionLayerMasks) == 0)
			{
				return;
			}
			this.SpawnImpactEffect(this.impactEffect, hitPosition, hitNormal);
			if (this.impactEffect != null)
			{
				SoundBankPlayer component = this.impactEffect.GetComponent<SoundBankPlayer>();
				if (component != null && !component.playOnEnable)
				{
					component.Play();
				}
			}
			this.impactEffectSpawned = true;
			if (this.destroyOnCollisionEnter)
			{
				if (this.destroyDelay > 0f)
				{
					base.Invoke("DestroyProjectile", this.destroyDelay);
					return;
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x06006DFF RID: 28159 RVA: 0x00241CAC File Offset: 0x0023FEAC
		private void GetColliderHitInfo(Collider other, out Vector3 position, out Vector3 normal)
		{
			Vector3 vector = Time.fixedDeltaTime * 2f * this.rigidbody.linearVelocity;
			Vector3 vector2 = base.transform.position - vector;
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			other.Raycast(new Ray(vector2, vector / magnitude), ref raycastHit, 2f * magnitude);
			position = raycastHit.point;
			normal = raycastHit.normal;
		}

		// Token: 0x06006E00 RID: 28160 RVA: 0x00241D28 File Offset: 0x0023FF28
		private void OnCollisionEnter(Collision other)
		{
			ContactPoint contact = other.GetContact(0);
			this.HandleImpact(other.gameObject, contact.point, contact.normal);
		}

		// Token: 0x06006E01 RID: 28161 RVA: 0x00241D58 File Offset: 0x0023FF58
		private void OnCollisionStay(Collision other)
		{
			ContactPoint contact = other.GetContact(0);
			this.HandleImpact(other.gameObject, contact.point, contact.normal);
		}

		// Token: 0x06006E02 RID: 28162 RVA: 0x00241D88 File Offset: 0x0023FF88
		private void OnTriggerEnter(Collider other)
		{
			Vector3 hitPosition;
			Vector3 hitNormal;
			this.GetColliderHitInfo(other, out hitPosition, out hitNormal);
			this.HandleImpact(other.gameObject, hitPosition, hitNormal);
		}

		// Token: 0x06006E03 RID: 28163 RVA: 0x00241DB0 File Offset: 0x0023FFB0
		private void OnTriggerStay(Collider other)
		{
			Transform transform = base.transform;
			this.HandleImpact(other.gameObject, transform.position, -transform.forward);
		}

		// Token: 0x06006E04 RID: 28164 RVA: 0x00241DE4 File Offset: 0x0023FFE4
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			if (prefab != null)
			{
				Vector3 position2 = position + normal * this.impactEffectOffset;
				GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
				gameObject.transform.up = normal;
				gameObject.transform.position = position2;
			}
			this.onImpactShared.Invoke();
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(position, normal);
			}
		}

		// Token: 0x06006E05 RID: 28165 RVA: 0x00241E58 File Offset: 0x00240058
		private void DestroyProjectile()
		{
			this.impactEffectSpawned = false;
			if (this.forceComponent)
			{
				this.forceComponent.enabled = false;
			}
			if (ObjectPools.instance.DoesPoolExist(base.gameObject))
			{
				ObjectPools.instance.Destroy(base.gameObject);
				return;
			}
			Object.Destroy(base.gameObject);
		}

		// Token: 0x04007FBA RID: 32698
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007FBB RID: 32699
		[SerializeField]
		private GameObject impactEffect;

		// Token: 0x04007FBC RID: 32700
		[SerializeField]
		private AudioClip launchAudio;

		// Token: 0x04007FBD RID: 32701
		[SerializeField]
		private LayerMask collisionLayerMasks;

		// Token: 0x04007FBE RID: 32702
		[SerializeField]
		private List<string> collisionTags = new List<string>();

		// Token: 0x04007FBF RID: 32703
		[SerializeField]
		private bool destroyOnCollisionEnter;

		// Token: 0x04007FC0 RID: 32704
		[SerializeField]
		private float destroyDelay = 1f;

		// Token: 0x04007FC1 RID: 32705
		[Tooltip("Distance from the surface that the particle should spawn.")]
		[SerializeField]
		private float impactEffectOffset = 0.1f;

		// Token: 0x04007FC2 RID: 32706
		[SerializeField]
		private SpawnWorldEffects spawnWorldEffects;

		// Token: 0x04007FC3 RID: 32707
		private ConstantForce forceComponent;

		// Token: 0x04007FC4 RID: 32708
		public UnityEvent<float> onLaunchShared;

		// Token: 0x04007FC5 RID: 32709
		public UnityEvent onImpactShared;

		// Token: 0x04007FC6 RID: 32710
		private bool impactEffectSpawned;

		// Token: 0x04007FC7 RID: 32711
		private Rigidbody rigidbody;
	}
}
