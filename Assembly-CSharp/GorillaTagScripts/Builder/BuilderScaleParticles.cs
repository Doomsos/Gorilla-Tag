using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E62 RID: 3682
	public class BuilderScaleParticles : MonoBehaviour
	{
		// Token: 0x06005C04 RID: 23556 RVA: 0x001D8768 File Offset: 0x001D6968
		private void OnEnable()
		{
			if (this.useLossyScale)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x06005C05 RID: 23557 RVA: 0x001D8784 File Offset: 0x001D6984
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScale)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x06005C06 RID: 23558 RVA: 0x001D87C0 File Offset: 0x001D69C0
		private void OnDisable()
		{
			if (this.useLossyScale)
			{
				this.RevertScale();
			}
		}

		// Token: 0x06005C07 RID: 23559 RVA: 0x001D87D0 File Offset: 0x001D69D0
		public void SetScale(float inScale)
		{
			bool isPlaying = this.system.isPlaying;
			if (isPlaying)
			{
				this.system.Stop();
				this.system.Clear();
			}
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			this.gravityMod = main.gravityModifierMultiplier;
			main.gravityModifierMultiplier = this.gravityMod * this.scale;
			if (main.startSize3D)
			{
				ParticleSystem.MinMaxCurve startSizeX = main.startSizeX;
				this.sizeCurveXCache = main.startSizeX;
				this.ScaleCurve(ref startSizeX, this.scale);
				main.startSizeX = startSizeX;
				ParticleSystem.MinMaxCurve startSizeY = main.startSizeY;
				this.sizeCurveYCache = main.startSizeY;
				this.ScaleCurve(ref startSizeY, this.scale);
				main.startSizeY = startSizeY;
				ParticleSystem.MinMaxCurve startSizeZ = main.startSizeZ;
				this.sizeCurveZCache = main.startSizeZ;
				this.ScaleCurve(ref startSizeZ, this.scale);
				main.startSizeZ = startSizeZ;
			}
			else
			{
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				this.sizeCurveCache = main.startSize;
				this.ScaleCurve(ref startSize, this.scale);
				main.startSize = startSize;
			}
			ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
			this.speedCurveCache = main.startSpeed;
			this.ScaleCurve(ref startSpeed, this.scale);
			main.startSpeed = startSpeed;
			if (this.scaleShape)
			{
				ParticleSystem.ShapeModule shape = this.system.shape;
				this.shapeScale = shape.scale;
				shape.scale = this.shapeScale * this.scale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				this.lifetimeVelocityX = velocityOverLifetime.x;
				this.lifetimeVelocityY = velocityOverLifetime.y;
				this.lifetimeVelocityZ = velocityOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve = velocityOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.x = minMaxCurve;
				minMaxCurve = velocityOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.y = minMaxCurve;
				minMaxCurve = velocityOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.z = minMaxCurve;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = this.system.limitVelocityOverLifetime;
				this.limitMultiplier = limitVelocityOverLifetime.limitMultiplier;
				limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier * this.scale;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				this.forceX = forceOverLifetime.x;
				this.forceY = forceOverLifetime.y;
				this.forceZ = forceOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve2 = forceOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.x = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.y = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.z = minMaxCurve2;
			}
			if (this.autoPlay || isPlaying)
			{
				this.system.Play(true);
			}
			this.shouldRevert = true;
		}

		// Token: 0x06005C08 RID: 23560 RVA: 0x001D8B40 File Offset: 0x001D6D40
		private void ScaleCurve(ref ParticleSystem.MinMaxCurve curve, float scale)
		{
			switch (curve.mode)
			{
			case 0:
				curve.constant *= scale;
				return;
			case 1:
			case 2:
				curve.curveMultiplier *= scale;
				return;
			case 3:
				curve.constantMin *= scale;
				curve.constantMax *= scale;
				return;
			default:
				return;
			}
		}

		// Token: 0x06005C09 RID: 23561 RVA: 0x001D8BA8 File Offset: 0x001D6DA8
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			main.gravityModifierMultiplier = this.gravityMod;
			main.startSpeed = this.speedCurveCache;
			if (main.startSize3D)
			{
				main.startSizeX = this.sizeCurveXCache;
				main.startSizeY = this.sizeCurveYCache;
				main.startSizeZ = this.sizeCurveZCache;
			}
			else
			{
				main.startSize = this.sizeCurveCache;
			}
			if (this.scaleShape)
			{
				this.system.shape.scale = this.shapeScale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				velocityOverLifetime.x = this.lifetimeVelocityX;
				velocityOverLifetime.y = this.lifetimeVelocityY;
				velocityOverLifetime.z = this.lifetimeVelocityZ;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				this.system.limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				forceOverLifetime.x = this.forceX;
				forceOverLifetime.y = this.forceY;
				forceOverLifetime.z = this.forceZ;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x04006953 RID: 26963
		private float scale = 1f;

		// Token: 0x04006954 RID: 26964
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScale;

		// Token: 0x04006955 RID: 26965
		[Tooltip("Play particles after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x04006956 RID: 26966
		[SerializeField]
		private ParticleSystem system;

		// Token: 0x04006957 RID: 26967
		[SerializeField]
		private bool scaleShape;

		// Token: 0x04006958 RID: 26968
		[SerializeField]
		private bool scaleVelocityLifetime;

		// Token: 0x04006959 RID: 26969
		[SerializeField]
		private bool scaleVelocityLimitLifetime;

		// Token: 0x0400695A RID: 26970
		[SerializeField]
		private bool scaleForceOverLife;

		// Token: 0x0400695B RID: 26971
		private float gravityMod = 1f;

		// Token: 0x0400695C RID: 26972
		private ParticleSystem.MinMaxCurve speedCurveCache;

		// Token: 0x0400695D RID: 26973
		private ParticleSystem.MinMaxCurve sizeCurveCache;

		// Token: 0x0400695E RID: 26974
		private ParticleSystem.MinMaxCurve sizeCurveXCache;

		// Token: 0x0400695F RID: 26975
		private ParticleSystem.MinMaxCurve sizeCurveYCache;

		// Token: 0x04006960 RID: 26976
		private ParticleSystem.MinMaxCurve sizeCurveZCache;

		// Token: 0x04006961 RID: 26977
		private ParticleSystem.MinMaxCurve forceX;

		// Token: 0x04006962 RID: 26978
		private ParticleSystem.MinMaxCurve forceY;

		// Token: 0x04006963 RID: 26979
		private ParticleSystem.MinMaxCurve forceZ;

		// Token: 0x04006964 RID: 26980
		private Vector3 shapeScale = Vector3.one;

		// Token: 0x04006965 RID: 26981
		private ParticleSystem.MinMaxCurve lifetimeVelocityX;

		// Token: 0x04006966 RID: 26982
		private ParticleSystem.MinMaxCurve lifetimeVelocityY;

		// Token: 0x04006967 RID: 26983
		private ParticleSystem.MinMaxCurve lifetimeVelocityZ;

		// Token: 0x04006968 RID: 26984
		private float limitMultiplier = 1f;

		// Token: 0x04006969 RID: 26985
		private bool shouldRevert;

		// Token: 0x0400696A RID: 26986
		private bool setScaleNextFrame;

		// Token: 0x0400696B RID: 26987
		private int enableFrame;
	}
}
