using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200117D RID: 4477
	public class BoingBehavior : BoingBase
	{
		// Token: 0x17000A8B RID: 2699
		// (get) Token: 0x060070FB RID: 28923 RVA: 0x0024FC47 File Offset: 0x0024DE47
		// (set) Token: 0x060070FC RID: 28924 RVA: 0x0024FC59 File Offset: 0x0024DE59
		public Vector3Spring PositionSpring
		{
			get
			{
				return this.Params.Instance.PositionSpring;
			}
			set
			{
				this.Params.Instance.PositionSpring = value;
				this.PositionSpringDirty = true;
			}
		}

		// Token: 0x17000A8C RID: 2700
		// (get) Token: 0x060070FD RID: 28925 RVA: 0x0024FC73 File Offset: 0x0024DE73
		// (set) Token: 0x060070FE RID: 28926 RVA: 0x0024FC85 File Offset: 0x0024DE85
		public QuaternionSpring RotationSpring
		{
			get
			{
				return this.Params.Instance.RotationSpring;
			}
			set
			{
				this.Params.Instance.RotationSpring = value;
				this.RotationSpringDirty = true;
			}
		}

		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x060070FF RID: 28927 RVA: 0x0024FC9F File Offset: 0x0024DE9F
		// (set) Token: 0x06007100 RID: 28928 RVA: 0x0024FCB1 File Offset: 0x0024DEB1
		public Vector3Spring ScaleSpring
		{
			get
			{
				return this.Params.Instance.ScaleSpring;
			}
			set
			{
				this.Params.Instance.ScaleSpring = value;
				this.ScaleSpringDirty = true;
			}
		}

		// Token: 0x06007101 RID: 28929 RVA: 0x0024FCCB File Offset: 0x0024DECB
		public BoingBehavior()
		{
			this.Params.Init();
		}

		// Token: 0x06007102 RID: 28930 RVA: 0x0024FCF4 File Offset: 0x0024DEF4
		public virtual void Reboot()
		{
			this.Params.Instance.PositionSpring.Reset(base.transform.position);
			this.Params.Instance.RotationSpring.Reset(base.transform.rotation);
			this.Params.Instance.ScaleSpring.Reset(base.transform.localScale);
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedPositionWs = base.transform.position;
			this.CachedRotationWs = base.transform.rotation;
			this.CachedScaleLs = base.transform.localScale;
			this.CachedTransformValid = true;
		}

		// Token: 0x06007103 RID: 28931 RVA: 0x0024FDBD File Offset: 0x0024DFBD
		public virtual void OnEnable()
		{
			this.CachedTransformValid = false;
			this.InitRebooted = false;
			this.Register();
		}

		// Token: 0x06007104 RID: 28932 RVA: 0x0024FDD3 File Offset: 0x0024DFD3
		public void Start()
		{
			this.InitRebooted = false;
		}

		// Token: 0x06007105 RID: 28933 RVA: 0x0024FDDC File Offset: 0x0024DFDC
		public virtual void OnDisable()
		{
			this.Unregister();
		}

		// Token: 0x06007106 RID: 28934 RVA: 0x0024FDE4 File Offset: 0x0024DFE4
		protected virtual void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06007107 RID: 28935 RVA: 0x0024FDEC File Offset: 0x0024DFEC
		protected virtual void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06007108 RID: 28936 RVA: 0x0024FDF4 File Offset: 0x0024DFF4
		public void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(5, this.EnableScaleEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(9, this.UpdateMode == BoingManager.UpdateMode.FixedUpdate);
			this.Params.Bits.SetBit(10, this.UpdateMode == BoingManager.UpdateMode.EarlyUpdate);
			this.Params.Bits.SetBit(11, this.UpdateMode == BoingManager.UpdateMode.LateUpdate);
		}

		// Token: 0x06007109 RID: 28937 RVA: 0x0024FEF3 File Offset: 0x0024E0F3
		public virtual void PrepareExecute()
		{
			this.PrepareExecute(false);
		}

		// Token: 0x0600710A RID: 28938 RVA: 0x0024FEFC File Offset: 0x0024E0FC
		protected void PrepareExecute(bool accumulateEffectors)
		{
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.Params.InstanceID = base.GetInstanceID();
			this.Params.Instance.PrepareExecute(ref this.Params, this.CachedPositionWs, this.CachedRotationWs, base.transform.localScale, accumulateEffectors);
		}

		// Token: 0x0600710B RID: 28939 RVA: 0x0024FF72 File Offset: 0x0024E172
		public void Execute(float dt)
		{
			this.Params.Execute(dt);
		}

		// Token: 0x0600710C RID: 28940 RVA: 0x0024FF80 File Offset: 0x0024E180
		public void PullResults()
		{
			this.PullResults(ref this.Params);
		}

		// Token: 0x0600710D RID: 28941 RVA: 0x0024FF90 File Offset: 0x0024E190
		public void GatherOutput(ref BoingWork.Output o)
		{
			if (!BoingManager.UseAsynchronousJobs)
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
				this.Params.Instance.RotationSpring = o.RotationSpring;
				this.Params.Instance.ScaleSpring = o.ScaleSpring;
				return;
			}
			if (this.PositionSpringDirty)
			{
				this.PositionSpringDirty = false;
			}
			else
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
			}
			if (this.RotationSpringDirty)
			{
				this.RotationSpringDirty = false;
			}
			else
			{
				this.Params.Instance.RotationSpring = o.RotationSpring;
			}
			if (this.ScaleSpringDirty)
			{
				this.ScaleSpringDirty = false;
				return;
			}
			this.Params.Instance.ScaleSpring = o.ScaleSpring;
		}

		// Token: 0x0600710E RID: 28942 RVA: 0x0025005C File Offset: 0x0024E25C
		private void PullResults(ref BoingWork.Params p)
		{
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedPositionWs = base.transform.position;
			this.RenderPositionWs = BoingWork.ComputeTranslationalResults(base.transform, base.transform.position, p.Instance.PositionSpring.Value, this);
			base.transform.position = this.RenderPositionWs;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedRotationWs = base.transform.rotation;
			this.RenderRotationWs = p.Instance.RotationSpring.ValueQuat;
			base.transform.rotation = this.RenderRotationWs;
			this.CachedScaleLs = base.transform.localScale;
			this.RenderScaleLs = p.Instance.ScaleSpring.Value;
			base.transform.localScale = this.RenderScaleLs;
			this.CachedTransformValid = true;
		}

		// Token: 0x0600710F RID: 28943 RVA: 0x00250154 File Offset: 0x0024E354
		public virtual void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			if (Application.isEditor)
			{
				if ((base.transform.position - this.RenderPositionWs).sqrMagnitude < 0.0001f)
				{
					base.transform.localPosition = this.CachedPositionLs;
				}
				if (QuaternionUtil.GetAngle(base.transform.rotation * Quaternion.Inverse(this.RenderRotationWs)) < 0.01f)
				{
					base.transform.localRotation = this.CachedRotationLs;
				}
				if ((base.transform.localScale - this.RenderScaleLs).sqrMagnitude < 0.0001f)
				{
					base.transform.localScale = this.CachedScaleLs;
					return;
				}
			}
			else
			{
				base.transform.localPosition = this.CachedPositionLs;
				base.transform.localRotation = this.CachedRotationLs;
				base.transform.localScale = this.CachedScaleLs;
			}
		}

		// Token: 0x04008126 RID: 33062
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04008127 RID: 33063
		public bool TwoDDistanceCheck;

		// Token: 0x04008128 RID: 33064
		public bool TwoDPositionInfluence;

		// Token: 0x04008129 RID: 33065
		public bool TwoDRotationInfluence;

		// Token: 0x0400812A RID: 33066
		public bool EnablePositionEffect = true;

		// Token: 0x0400812B RID: 33067
		public bool EnableRotationEffect = true;

		// Token: 0x0400812C RID: 33068
		public bool EnableScaleEffect;

		// Token: 0x0400812D RID: 33069
		public bool GlobalReactionUpVector;

		// Token: 0x0400812E RID: 33070
		public BoingManager.TranslationLockSpace TranslationLockSpace;

		// Token: 0x0400812F RID: 33071
		public bool LockTranslationX;

		// Token: 0x04008130 RID: 33072
		public bool LockTranslationY;

		// Token: 0x04008131 RID: 33073
		public bool LockTranslationZ;

		// Token: 0x04008132 RID: 33074
		public BoingWork.Params Params;

		// Token: 0x04008133 RID: 33075
		public SharedBoingParams SharedParams;

		// Token: 0x04008134 RID: 33076
		internal bool PositionSpringDirty;

		// Token: 0x04008135 RID: 33077
		internal bool RotationSpringDirty;

		// Token: 0x04008136 RID: 33078
		internal bool ScaleSpringDirty;

		// Token: 0x04008137 RID: 33079
		internal bool CachedTransformValid;

		// Token: 0x04008138 RID: 33080
		internal Vector3 CachedPositionLs;

		// Token: 0x04008139 RID: 33081
		internal Vector3 CachedPositionWs;

		// Token: 0x0400813A RID: 33082
		internal Vector3 RenderPositionWs;

		// Token: 0x0400813B RID: 33083
		internal Quaternion CachedRotationLs;

		// Token: 0x0400813C RID: 33084
		internal Quaternion CachedRotationWs;

		// Token: 0x0400813D RID: 33085
		internal Quaternion RenderRotationWs;

		// Token: 0x0400813E RID: 33086
		internal Vector3 CachedScaleLs;

		// Token: 0x0400813F RID: 33087
		internal Vector3 RenderScaleLs;

		// Token: 0x04008140 RID: 33088
		internal bool InitRebooted;
	}
}
