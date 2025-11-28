using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010BA RID: 4282
	public class DrillFX : MonoBehaviour
	{
		// Token: 0x06006B3E RID: 27454 RVA: 0x002330C4 File Offset: 0x002312C4
		protected void Awake()
		{
			if (!DrillFX.appIsQuittingHandlerIsSubscribed)
			{
				DrillFX.appIsQuittingHandlerIsSubscribed = true;
				Application.quitting += new Action(DrillFX.HandleApplicationQuitting);
			}
			this.hasFX = (this.fx != null);
			if (this.hasFX)
			{
				this.fxEmissionModule = this.fx.emission;
				this.fxEmissionMaxRate = this.fxEmissionModule.rateOverTimeMultiplier;
				this.fxShapeModule = this.fx.shape;
				this.fxShapeMaxRadius = this.fxShapeModule.radius;
			}
			this.hasAudio = (this.loopAudio != null);
			if (this.hasAudio)
			{
				this.audioMaxVolume = this.loopAudio.volume;
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
		}

		// Token: 0x06006B3F RID: 27455 RVA: 0x002331A0 File Offset: 0x002313A0
		protected void OnEnable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
			this.ValidateLineCastPositions();
		}

		// Token: 0x06006B40 RID: 27456 RVA: 0x00233204 File Offset: 0x00231404
		protected void OnDisable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.GTStop();
			}
		}

		// Token: 0x06006B41 RID: 27457 RVA: 0x00233254 File Offset: 0x00231454
		protected void LateUpdate()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			Transform transform = base.transform;
			RaycastHit raycastHit;
			Vector3 vector = Physics.Linecast(transform.TransformPoint(this.lineCastStart), transform.TransformPoint(this.lineCastEnd), ref raycastHit, this.lineCastLayerMask, 1) ? raycastHit.point : this.lineCastEnd;
			Vector3 vector2 = transform.InverseTransformPoint(vector);
			float num = Mathf.Clamp01(Vector3.Distance(this.lineCastStart, vector2) / this.maxDepth);
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = this.fxEmissionMaxRate * this.fxEmissionCurve.Evaluate(num);
				this.fxShapeModule.position = vector2;
				this.fxShapeModule.radius = Mathf.Lerp(this.fxShapeMaxRadius, this.fxMinRadiusScale * this.fxShapeMaxRadius, num);
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = Mathf.MoveTowards(this.loopAudio.volume, this.audioMaxVolume * this.loopAudioVolumeCurve.Evaluate(num), this.loopAudioVolumeTransitionSpeed * Time.deltaTime);
			}
		}

		// Token: 0x06006B42 RID: 27458 RVA: 0x0023336A File Offset: 0x0023156A
		private static void HandleApplicationQuitting()
		{
			DrillFX.appIsQuitting = true;
		}

		// Token: 0x06006B43 RID: 27459 RVA: 0x00233374 File Offset: 0x00231574
		private bool ValidateLineCastPositions()
		{
			this.maxDepth = Vector3.Distance(this.lineCastStart, this.lineCastEnd);
			if (this.maxDepth > 1E-45f)
			{
				return true;
			}
			if (Application.isPlaying)
			{
				Debug.Log("DrillFX: lineCastStart and End are too close together. Disabling component.", this);
				base.enabled = false;
			}
			return false;
		}

		// Token: 0x04007B93 RID: 31635
		[SerializeField]
		private ParticleSystem fx;

		// Token: 0x04007B94 RID: 31636
		[SerializeField]
		private AnimationCurve fxEmissionCurve;

		// Token: 0x04007B95 RID: 31637
		[SerializeField]
		private float fxMinRadiusScale = 0.01f;

		// Token: 0x04007B96 RID: 31638
		[Tooltip("Right click menu has custom menu items. Anything starting with \"- \" is custom.")]
		[SerializeField]
		private AudioSource loopAudio;

		// Token: 0x04007B97 RID: 31639
		[SerializeField]
		private AnimationCurve loopAudioVolumeCurve;

		// Token: 0x04007B98 RID: 31640
		[Tooltip("Higher value makes it reach the target volume faster.")]
		[SerializeField]
		private float loopAudioVolumeTransitionSpeed = 3f;

		// Token: 0x04007B99 RID: 31641
		[FormerlySerializedAs("layerMask")]
		[Tooltip("The collision layers the line cast should intersect with")]
		[SerializeField]
		private LayerMask lineCastLayerMask;

		// Token: 0x04007B9A RID: 31642
		[Tooltip("The position in local space that the line cast starts.")]
		[SerializeField]
		private Vector3 lineCastStart = Vector3.zero;

		// Token: 0x04007B9B RID: 31643
		[Tooltip("The position in local space that the line cast ends.")]
		[SerializeField]
		private Vector3 lineCastEnd = Vector3.forward;

		// Token: 0x04007B9C RID: 31644
		private static bool appIsQuitting;

		// Token: 0x04007B9D RID: 31645
		private static bool appIsQuittingHandlerIsSubscribed;

		// Token: 0x04007B9E RID: 31646
		private float maxDepth;

		// Token: 0x04007B9F RID: 31647
		private bool hasFX;

		// Token: 0x04007BA0 RID: 31648
		private ParticleSystem.EmissionModule fxEmissionModule;

		// Token: 0x04007BA1 RID: 31649
		private float fxEmissionMaxRate;

		// Token: 0x04007BA2 RID: 31650
		private ParticleSystem.ShapeModule fxShapeModule;

		// Token: 0x04007BA3 RID: 31651
		private float fxShapeMaxRadius;

		// Token: 0x04007BA4 RID: 31652
		private bool hasAudio;

		// Token: 0x04007BA5 RID: 31653
		private float audioMaxVolume;
	}
}
