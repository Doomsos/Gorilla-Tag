using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class VotingCard : MonoBehaviour
{
	// Token: 0x06000D02 RID: 3330 RVA: 0x0004605A File Offset: 0x0004425A
	private void MoveToOffPosition()
	{
		this._card.transform.position = this._offPosition.position;
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00046077 File Offset: 0x00044277
	private void MoveToOnPosition()
	{
		this._card.transform.position = this._onPosition.position;
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00046094 File Offset: 0x00044294
	public void SetVisible(bool showVote, bool instant)
	{
		if (this._isVisible != showVote)
		{
			base.StopAllCoroutines();
		}
		if (instant)
		{
			this._card.transform.position = (showVote ? this._onPosition.position : this._offPosition.position);
			this._card.SetActive(showVote);
		}
		else if (showVote)
		{
			if (this._isVisible != showVote)
			{
				base.StartCoroutine(this.DoActivate());
			}
		}
		else
		{
			this._card.SetActive(false);
			this._card.transform.position = this._offPosition.position;
		}
		this._isVisible = showVote;
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x00046135 File Offset: 0x00044335
	private IEnumerator DoActivate()
	{
		Vector3 from = this._offPosition.position;
		Vector3 to = this._onPosition.position;
		this._card.transform.position = from;
		this._card.SetActive(true);
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / this.activationTime;
			this._card.transform.position = Vector3.Lerp(from, to, lerpVal);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000FFF RID: 4095
	[SerializeField]
	private GameObject _card;

	// Token: 0x04001000 RID: 4096
	[SerializeField]
	private Transform _offPosition;

	// Token: 0x04001001 RID: 4097
	[SerializeField]
	private Transform _onPosition;

	// Token: 0x04001002 RID: 4098
	[SerializeField]
	private float activationTime = 0.5f;

	// Token: 0x04001003 RID: 4099
	private bool _isVisible;
}
