using System;
using System.Collections;
using GorillaTag;
using UnityEngine;

// Token: 0x02000412 RID: 1042
public class HitTargetScoreDisplay : MonoBehaviour
{
	// Token: 0x060019A7 RID: 6567 RVA: 0x00089268 File Offset: 0x00087468
	protected void Awake()
	{
		this.rotateTimeTotal = 180f / (float)this.rotateSpeed;
		this.matPropBlock = new MaterialPropertyBlock();
		this.networkedScore.AddCallback(new Action<int>(this.OnScoreChanged), true);
		this.ResetRotation();
		this.tensOld = 0;
		this.hundredsOld = 0;
		this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[0]);
		this.singlesRend.SetPropertyBlock(this.matPropBlock);
		this.tensRend.SetPropertyBlock(this.matPropBlock);
		this.hundredsRend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x0008930E File Offset: 0x0008750E
	private void OnDestroy()
	{
		this.networkedScore.RemoveCallback(new Action<int>(this.OnScoreChanged));
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x00089328 File Offset: 0x00087528
	private void ResetRotation()
	{
		Quaternion rotation = base.transform.rotation;
		this.singlesCard.rotation = rotation;
		this.tensCard.rotation = rotation;
		this.hundredsCard.rotation = rotation;
	}

	// Token: 0x060019AA RID: 6570 RVA: 0x00089365 File Offset: 0x00087565
	private IEnumerator RotatingCo()
	{
		float timeElapsedSinceHit = 0f;
		int singlesPlace = this.currentScore % 10;
		int tensPlace = this.currentScore / 10 % 10;
		bool tensChange = this.tensOld != tensPlace;
		this.tensOld = tensPlace;
		int hundredsPlace = this.currentScore / 100 % 10;
		bool hundredsChange = this.hundredsOld != hundredsPlace;
		this.hundredsOld = hundredsPlace;
		bool digitsChange = true;
		while (timeElapsedSinceHit < this.rotateTimeTotal)
		{
			this.singlesCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, 1);
			Vector3 localEulerAngles = this.singlesCard.localEulerAngles;
			localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
			this.singlesCard.localEulerAngles = localEulerAngles;
			if (tensChange)
			{
				this.tensCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, 1);
				Vector3 localEulerAngles2 = this.tensCard.localEulerAngles;
				localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
				this.tensCard.localEulerAngles = localEulerAngles2;
			}
			if (hundredsChange)
			{
				this.hundredsCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, 1);
				Vector3 localEulerAngles3 = this.hundredsCard.localEulerAngles;
				localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
				this.hundredsCard.localEulerAngles = localEulerAngles3;
			}
			if (digitsChange && timeElapsedSinceHit >= this.rotateTimeTotal / 2f)
			{
				this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[singlesPlace]);
				this.singlesRend.SetPropertyBlock(this.matPropBlock);
				if (tensChange)
				{
					this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[tensPlace]);
					this.tensRend.SetPropertyBlock(this.matPropBlock);
				}
				if (hundredsChange)
				{
					this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[hundredsPlace]);
					this.hundredsRend.SetPropertyBlock(this.matPropBlock);
				}
				digitsChange = false;
			}
			yield return null;
			timeElapsedSinceHit += Time.deltaTime;
		}
		this.ResetRotation();
		yield break;
		yield break;
	}

	// Token: 0x060019AB RID: 6571 RVA: 0x00089374 File Offset: 0x00087574
	private void OnScoreChanged(int newScore)
	{
		if (newScore == this.currentScore)
		{
			return;
		}
		if (this.currentRotationCoroutine != null)
		{
			base.StopCoroutine(this.currentRotationCoroutine);
		}
		this.currentScore = newScore;
		if (base.gameObject.activeInHierarchy)
		{
			this.currentRotationCoroutine = base.StartCoroutine(this.RotatingCo());
		}
	}

	// Token: 0x0400230F RID: 8975
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x04002310 RID: 8976
	private int currentScore;

	// Token: 0x04002311 RID: 8977
	private int tensOld;

	// Token: 0x04002312 RID: 8978
	private int hundredsOld;

	// Token: 0x04002313 RID: 8979
	private float rotateTimeTotal;

	// Token: 0x04002314 RID: 8980
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04002315 RID: 8981
	private readonly Vector4[] numberSheet = new Vector4[]
	{
		new Vector4(1f, 1f, 0.8f, -0.5f),
		new Vector4(1f, 1f, 0f, 0f),
		new Vector4(1f, 1f, 0.2f, 0f),
		new Vector4(1f, 1f, 0.4f, 0f),
		new Vector4(1f, 1f, 0.6f, 0f),
		new Vector4(1f, 1f, 0.8f, 0f),
		new Vector4(1f, 1f, 0f, -0.5f),
		new Vector4(1f, 1f, 0.2f, -0.5f),
		new Vector4(1f, 1f, 0.4f, -0.5f),
		new Vector4(1f, 1f, 0.6f, -0.5f)
	};

	// Token: 0x04002316 RID: 8982
	public int rotateSpeed = 180;

	// Token: 0x04002317 RID: 8983
	public Transform singlesCard;

	// Token: 0x04002318 RID: 8984
	public Transform tensCard;

	// Token: 0x04002319 RID: 8985
	public Transform hundredsCard;

	// Token: 0x0400231A RID: 8986
	public Renderer singlesRend;

	// Token: 0x0400231B RID: 8987
	public Renderer tensRend;

	// Token: 0x0400231C RID: 8988
	public Renderer hundredsRend;

	// Token: 0x0400231D RID: 8989
	private Coroutine currentRotationCoroutine;
}
