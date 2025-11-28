using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020005E0 RID: 1504
public class FixedSizeTrailAdjustBySpeed : MonoBehaviour
{
	// Token: 0x060025E6 RID: 9702 RVA: 0x000CA922 File Offset: 0x000C8B22
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x000CA92A File Offset: 0x000C8B2A
	private void OnEnable()
	{
		this.ResetTrailState();
	}

	// Token: 0x060025E8 RID: 9704 RVA: 0x000CA92A File Offset: 0x000C8B2A
	private void OnDisable()
	{
		this.ResetTrailState();
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x000CA934 File Offset: 0x000C8B34
	private void ResetTrailState()
	{
		this._rawVelocity = Vector3.zero;
		this._rawSpeed = 0f;
		this._speed = 0f;
		this._lastSpeed = 0f;
		this._lastPosition = base.transform.position;
		if (!this.trail)
		{
			return;
		}
		this.trail.length = this.minLength;
		this.trail.Setup();
		this.LerpTrailColors(0f);
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x000CA9B4 File Offset: 0x000C8BB4
	private void Setup()
	{
		this._lastPosition = base.transform.position;
		this._rawVelocity = Vector3.zero;
		this._rawSpeed = 0f;
		this._speed = 0f;
		if (this.trail)
		{
			this._initGravity = this.trail.gravity;
			this.trail.applyPhysics = this.adjustPhysics;
		}
		this.LerpTrailColors(0.5f);
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x000CAA30 File Offset: 0x000C8C30
	private void LerpTrailColors(float t = 0.5f)
	{
		GradientColorKey[] colorKeys = this._mixGradient.colorKeys;
		int num = colorKeys.Length;
		for (int i = 0; i < num; i++)
		{
			float num2 = (float)i / (float)(num - 1);
			Color color = this.minColors.Evaluate(num2);
			Color color2 = this.maxColors.Evaluate(num2);
			Color color3 = Color.Lerp(color, color2, t);
			colorKeys[i].color = color3;
			colorKeys[i].time = num2;
		}
		this._mixGradient.colorKeys = colorKeys;
		if (this.trail)
		{
			this.trail.renderer.colorGradient = this._mixGradient;
		}
	}

	// Token: 0x060025EC RID: 9708 RVA: 0x000CAAD0 File Offset: 0x000C8CD0
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		this._rawVelocity = (position - this._lastPosition) / deltaTime;
		this._rawSpeed = this._rawVelocity.magnitude;
		if (this._rawSpeed > this.retractMin)
		{
			this._speed += this.expandSpeed * deltaTime;
		}
		if (this._rawSpeed <= this.retractMin)
		{
			this._speed -= this.retractSpeed * deltaTime;
		}
		if (this._speed > this.maxSpeed)
		{
			this._speed = this.maxSpeed;
		}
		this._speed = Mathf.Lerp(this._lastSpeed, this._speed, 0.5f);
		if (this._speed < 0.01f)
		{
			this._speed = 0f;
		}
		this.AdjustTrail();
		this._lastSpeed = this._speed;
		this._lastPosition = position;
	}

	// Token: 0x060025ED RID: 9709 RVA: 0x000CABC8 File Offset: 0x000C8DC8
	private void AdjustTrail()
	{
		if (!this.trail)
		{
			return;
		}
		float num = MathUtils.Linear(this._speed, this.minSpeed, this.maxSpeed, 0f, 1f);
		float length = MathUtils.Linear(num, 0f, 1f, this.minLength, this.maxLength);
		this.trail.length = length;
		this.LerpTrailColors(num);
		if (this.adjustPhysics)
		{
			Transform transform = base.transform;
			Vector3 vector = transform.forward * this.gravityOffset.z + transform.right * this.gravityOffset.x + transform.up * this.gravityOffset.y;
			Vector3 vector2 = (this._initGravity + vector) * (1f - num);
			this.trail.gravity = Vector3.Lerp(Vector3.zero, vector2, 0.5f);
		}
	}

	// Token: 0x040031AE RID: 12718
	public FixedSizeTrail trail;

	// Token: 0x040031AF RID: 12719
	public bool adjustPhysics = true;

	// Token: 0x040031B0 RID: 12720
	private Vector3 _rawVelocity;

	// Token: 0x040031B1 RID: 12721
	private float _rawSpeed;

	// Token: 0x040031B2 RID: 12722
	private float _speed;

	// Token: 0x040031B3 RID: 12723
	private float _lastSpeed;

	// Token: 0x040031B4 RID: 12724
	private Vector3 _lastPosition;

	// Token: 0x040031B5 RID: 12725
	private Vector3 _initGravity;

	// Token: 0x040031B6 RID: 12726
	public Vector3 gravityOffset = Vector3.zero;

	// Token: 0x040031B7 RID: 12727
	[Space]
	public float retractMin = 0.5f;

	// Token: 0x040031B8 RID: 12728
	[Space]
	[FormerlySerializedAs("sizeIncreaseSpeed")]
	public float expandSpeed = 16f;

	// Token: 0x040031B9 RID: 12729
	[FormerlySerializedAs("sizeDecreaseSpeed")]
	public float retractSpeed = 4f;

	// Token: 0x040031BA RID: 12730
	[Space]
	public float minSpeed;

	// Token: 0x040031BB RID: 12731
	public float minLength = 1f;

	// Token: 0x040031BC RID: 12732
	public Gradient minColors = GradientHelper.FromColor(new Color(0f, 1f, 1f, 1f));

	// Token: 0x040031BD RID: 12733
	[Space]
	public float maxSpeed = 10f;

	// Token: 0x040031BE RID: 12734
	public float maxLength = 8f;

	// Token: 0x040031BF RID: 12735
	public Gradient maxColors = GradientHelper.FromColor(new Color(1f, 1f, 0f, 1f));

	// Token: 0x040031C0 RID: 12736
	[Space]
	[SerializeField]
	private Gradient _mixGradient = new Gradient
	{
		colorKeys = new GradientColorKey[8],
		alphaKeys = Array.Empty<GradientAlphaKey>()
	};

	// Token: 0x020005E1 RID: 1505
	[Serializable]
	public struct GradientKey
	{
		// Token: 0x060025EF RID: 9711 RVA: 0x000CAD9D File Offset: 0x000C8F9D
		public GradientKey(Color color, float time)
		{
			this.color = color;
			this.time = time;
		}

		// Token: 0x040031C1 RID: 12737
		public Color color;

		// Token: 0x040031C2 RID: 12738
		public float time;
	}
}
