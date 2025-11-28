using System;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class AngryBeeAnimator : MonoBehaviour
{
	// Token: 0x06000B77 RID: 2935 RVA: 0x0003E150 File Offset: 0x0003C350
	private void Awake()
	{
		this.bees = new GameObject[this.numBees];
		this.beeOrbits = new GameObject[this.numBees];
		this.beeOrbitalRadii = new float[this.numBees];
		this.beeOrbitalAxes = new Vector3[this.numBees];
		for (int i = 0; i < this.numBees; i++)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = base.transform;
			Vector2 vector = Random.insideUnitCircle * this.orbitMaxCenterDisplacement;
			gameObject.transform.localPosition = new Vector3(vector.x, Random.Range(-this.orbitMaxHeightDisplacement, this.orbitMaxHeightDisplacement), vector.y);
			gameObject.transform.localRotation = Quaternion.Euler(Random.Range(-this.orbitMaxTilt, this.orbitMaxTilt), (float)Random.Range(0, 360), 0f);
			this.beeOrbitalAxes[i] = gameObject.transform.up;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.beePrefab, gameObject.transform);
			float num = Random.Range(this.orbitMinRadius, this.orbitMaxRadius);
			this.beeOrbitalRadii[i] = num;
			gameObject2.transform.localPosition = Vector3.forward * num;
			gameObject2.transform.localRotation = Quaternion.Euler(-90f, 90f, 0f);
			gameObject2.transform.localScale = Vector3.one * this.beeScale;
			this.bees[i] = gameObject2;
			this.beeOrbits[i] = gameObject;
		}
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0003E2EC File Offset: 0x0003C4EC
	private void Update()
	{
		float num = this.orbitSpeed * Time.deltaTime;
		for (int i = 0; i < this.numBees; i++)
		{
			this.beeOrbits[i].transform.Rotate(this.beeOrbitalAxes[i], num);
		}
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x0003E338 File Offset: 0x0003C538
	public void SetEmergeFraction(float fraction)
	{
		for (int i = 0; i < this.numBees; i++)
		{
			this.bees[i].transform.localPosition = Vector3.forward * fraction * this.beeOrbitalRadii[i];
		}
	}

	// Token: 0x04000E0D RID: 3597
	[SerializeField]
	private GameObject beePrefab;

	// Token: 0x04000E0E RID: 3598
	[SerializeField]
	private int numBees;

	// Token: 0x04000E0F RID: 3599
	[SerializeField]
	private float orbitMinRadius;

	// Token: 0x04000E10 RID: 3600
	[SerializeField]
	private float orbitMaxRadius;

	// Token: 0x04000E11 RID: 3601
	[SerializeField]
	private float orbitMaxHeightDisplacement;

	// Token: 0x04000E12 RID: 3602
	[SerializeField]
	private float orbitMaxCenterDisplacement;

	// Token: 0x04000E13 RID: 3603
	[SerializeField]
	private float orbitMaxTilt;

	// Token: 0x04000E14 RID: 3604
	[SerializeField]
	private float orbitSpeed;

	// Token: 0x04000E15 RID: 3605
	[SerializeField]
	private float beeScale;

	// Token: 0x04000E16 RID: 3606
	private GameObject[] beeOrbits;

	// Token: 0x04000E17 RID: 3607
	private GameObject[] bees;

	// Token: 0x04000E18 RID: 3608
	private Vector3[] beeOrbitalAxes;

	// Token: 0x04000E19 RID: 3609
	private float[] beeOrbitalRadii;
}
