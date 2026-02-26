using System;
using UnityEngine;

public class SIBlasterRandomAngularVelocity : MonoBehaviour, SIGadgetProjectileModifier
{
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.rb.angularVelocity = new Vector3(UnityEngine.Random.Range(-this.maxVel, this.maxVel), UnityEngine.Random.Range(-this.maxVel, this.maxVel), UnityEngine.Random.Range(-this.maxVel, this.maxVel));
	}

	public float maxVel;
}
