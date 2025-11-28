using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTagScripts
{
	// Token: 0x02000E03 RID: 3587
	public class RandomProjectileThrowable : MonoBehaviour
	{
		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x0600598A RID: 22922 RVA: 0x001CA7E0 File Offset: 0x001C89E0
		// (set) Token: 0x0600598B RID: 22923 RVA: 0x001CA7E8 File Offset: 0x001C89E8
		public float TimeEnabled { get; private set; }

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x0600598C RID: 22924 RVA: 0x001CA7F1 File Offset: 0x001C89F1
		// (set) Token: 0x0600598D RID: 22925 RVA: 0x001CA7F9 File Offset: 0x001C89F9
		public bool ForceDestroy { get; set; }

		// Token: 0x0600598E RID: 22926 RVA: 0x001CA802 File Offset: 0x001C8A02
		private void OnEnable()
		{
			this.TimeEnabled = Time.time;
			this.currentProjectile = this.projectilePrefab;
		}

		// Token: 0x0600598F RID: 22927 RVA: 0x001CA81B File Offset: 0x001C8A1B
		private void OnDisable()
		{
			this.ForceDestroy = false;
		}

		// Token: 0x06005990 RID: 22928 RVA: 0x001CA824 File Offset: 0x001C8A24
		public void ForceDestroyThrowable()
		{
			this.ForceDestroy = true;
		}

		// Token: 0x06005991 RID: 22929 RVA: 0x001CA82D File Offset: 0x001C8A2D
		public void UpdateProjectilePrefab()
		{
			this.currentProjectile = this.alternativeProjectilePrefab;
		}

		// Token: 0x06005992 RID: 22930 RVA: 0x001CA83B File Offset: 0x001C8A3B
		public GameObject GetProjectilePrefab()
		{
			return this.currentProjectile;
		}

		// Token: 0x06005993 RID: 22931 RVA: 0x001CA844 File Offset: 0x001C8A44
		private void OnTriggerEnter(Collider other)
		{
			if (!this.destroyOnTrigger)
			{
				return;
			}
			if (other.gameObject.layer == LayerMask.NameToLayer(this.triggerTag))
			{
				if (this.audioSource && this.triggerClip)
				{
					this.audioSource.GTPlayOneShot(this.triggerClip, 1f);
				}
				UnityEvent onDestroyed = this.OnDestroyed;
				if (onDestroyed != null)
				{
					onDestroyed.Invoke();
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x06005994 RID: 22932 RVA: 0x001CA8B9 File Offset: 0x001C8AB9
		public void DestroyProjectile()
		{
			base.StartCoroutine(this.DestroyProjectileCoroutine(0.25f));
		}

		// Token: 0x06005995 RID: 22933 RVA: 0x001CA8CD File Offset: 0x001C8ACD
		private IEnumerator DestroyProjectileCoroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			UnityAction<bool> onDestroyRandomProjectile = this.OnDestroyRandomProjectile;
			if (onDestroyRandomProjectile != null)
			{
				onDestroyRandomProjectile.Invoke(false);
			}
			yield break;
		}

		// Token: 0x040066B1 RID: 26289
		public GameObject projectilePrefab;

		// Token: 0x040066B2 RID: 26290
		[Tooltip("Use for a different/updated version of the projectile if needed.")]
		public GameObject alternativeProjectilePrefab;

		// Token: 0x040066B3 RID: 26291
		[FormerlySerializedAs("weightedChance")]
		[Range(0f, 1f)]
		public float spawnChance = 1f;

		// Token: 0x040066B4 RID: 26292
		[Tooltip("Requires a collider")]
		public bool destroyOnTrigger = true;

		// Token: 0x040066B5 RID: 26293
		public string triggerTag = "Gorilla Head";

		// Token: 0x040066B6 RID: 26294
		[FormerlySerializedAs("onMoveToHead")]
		public UnityEvent OnDestroyed;

		// Token: 0x040066B7 RID: 26295
		public AudioSource audioSource;

		// Token: 0x040066B8 RID: 26296
		public AudioClip triggerClip;

		// Token: 0x040066B9 RID: 26297
		[Tooltip("Immediately destroys after the release")]
		public bool destroyAfterRelease;

		// Token: 0x040066BA RID: 26298
		[Tooltip("Set a timer to destroy after X seconds is passed and the object is not thrown yet")]
		[FormerlySerializedAs("destroyAfterSeconds")]
		public float autoDestroyAfterSeconds = -1f;

		// Token: 0x040066BB RID: 26299
		[Tooltip("If checked, any amount of passed time will be deducted from the lifetime of the slingshot projectile when thrownShould be less than or equal to lifetime of the slingshot projectile")]
		public bool moveOverPassedLifeTime;

		// Token: 0x040066BE RID: 26302
		public UnityAction<bool> OnDestroyRandomProjectile;

		// Token: 0x040066BF RID: 26303
		private GameObject currentProjectile;
	}
}
