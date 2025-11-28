using System;
using UnityEngine;

// Token: 0x0200048A RID: 1162
public class ManipulatableSlider : ManipulatableObject
{
	// Token: 0x06001DB3 RID: 7603 RVA: 0x0009C4B2 File Offset: 0x0009A6B2
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06001DB4 RID: 7604 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06001DB5 RID: 7605 RVA: 0x0009C4D6 File Offset: 0x0009A6D6
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x06001DB6 RID: 7606 RVA: 0x0009C4F4 File Offset: 0x0009A6F4
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001DB7 RID: 7607 RVA: 0x0009C534 File Offset: 0x0009A734
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x0009C5CC File Offset: 0x0009A7CC
	protected override void OnReleasedUpdate()
	{
		if (this.velocity != Vector3.zero)
		{
			Vector3 vector = this.localSpace.MultiplyPoint(base.transform.position);
			vector += this.velocity * Time.deltaTime;
			if (vector.x < this.minXOffset)
			{
				vector.x = this.minXOffset;
				this.velocity.x = 0f;
			}
			else if (vector.x > this.maxXOffset)
			{
				vector.x = this.maxXOffset;
				this.velocity.x = 0f;
			}
			if (vector.y < this.minYOffset)
			{
				vector.y = this.minYOffset;
				this.velocity.y = 0f;
			}
			else if (vector.y > this.maxYOffset)
			{
				vector.y = this.maxYOffset;
				this.velocity.y = 0f;
			}
			if (vector.z < this.minZOffset)
			{
				vector.z = this.minZOffset;
				this.velocity.z = 0f;
			}
			else if (vector.z > this.maxZOffset)
			{
				vector.z = this.maxZOffset;
				this.velocity.z = 0f;
			}
			vector += this.startingPos;
			base.transform.localPosition = vector;
			this.velocity *= 1f - this.releaseDrag * Time.deltaTime;
			if (this.velocity.sqrMagnitude < 0.001f)
			{
				this.velocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x0009C780 File Offset: 0x0009A980
	public void SetProgress(float x, float y, float z)
	{
		x = Mathf.Clamp(x, 0f, 1f);
		y = Mathf.Clamp(y, 0f, 1f);
		z = Mathf.Clamp(z, 0f, 1f);
		Vector3 localPosition = this.startingPos;
		localPosition.x += Mathf.Lerp(this.minXOffset, this.maxXOffset, x);
		localPosition.y += Mathf.Lerp(this.minYOffset, this.maxYOffset, y);
		localPosition.z += Mathf.Lerp(this.minZOffset, this.maxZOffset, z);
		base.transform.localPosition = localPosition;
	}

	// Token: 0x06001DBA RID: 7610 RVA: 0x0009C82D File Offset: 0x0009AA2D
	public float GetProgressX()
	{
		return ((base.transform.localPosition - this.startingPos).x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x0009C85F File Offset: 0x0009AA5F
	public float GetProgressY()
	{
		return ((base.transform.localPosition - this.startingPos).y - this.minYOffset) / (this.maxYOffset - this.minYOffset);
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x0009C891 File Offset: 0x0009AA91
	public float GetProgressZ()
	{
		return ((base.transform.localPosition - this.startingPos).z - this.minZOffset) / (this.maxZOffset - this.minZOffset);
	}

	// Token: 0x040027AE RID: 10158
	public float breakDistance = 0.2f;

	// Token: 0x040027AF RID: 10159
	public float maxXOffset;

	// Token: 0x040027B0 RID: 10160
	public float minXOffset;

	// Token: 0x040027B1 RID: 10161
	public float maxYOffset;

	// Token: 0x040027B2 RID: 10162
	public float minYOffset;

	// Token: 0x040027B3 RID: 10163
	public float maxZOffset;

	// Token: 0x040027B4 RID: 10164
	public float minZOffset;

	// Token: 0x040027B5 RID: 10165
	public bool applyReleaseVelocity;

	// Token: 0x040027B6 RID: 10166
	public float releaseDrag = 1f;

	// Token: 0x040027B7 RID: 10167
	private Matrix4x4 localSpace;

	// Token: 0x040027B8 RID: 10168
	private Vector3 startingPos;

	// Token: 0x040027B9 RID: 10169
	private Vector3 velocity;
}
