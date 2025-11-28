using System;
using UnityEngine;

// Token: 0x0200054D RID: 1357
[Serializable]
public class MonkeBallTeam
{
	// Token: 0x04002CCF RID: 11471
	public Color color;

	// Token: 0x04002CD0 RID: 11472
	public int score;

	// Token: 0x04002CD1 RID: 11473
	public Transform ballStartLocation;

	// Token: 0x04002CD2 RID: 11474
	public Transform ballLaunchPosition;

	// Token: 0x04002CD3 RID: 11475
	[Tooltip("The min/max random velocity of the ball when launched.")]
	public Vector2 ballLaunchVelocityRange = new Vector2(8f, 15f);

	// Token: 0x04002CD4 RID: 11476
	[Tooltip("The min/max random x-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleXRange = new Vector2(0f, 0f);

	// Token: 0x04002CD5 RID: 11477
	[Tooltip("The min/max random y-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleYRange = new Vector2(0f, 0f);
}
