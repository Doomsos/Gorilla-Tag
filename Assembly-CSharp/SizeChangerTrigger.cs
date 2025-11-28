using System;
using UnityEngine;

// Token: 0x0200082B RID: 2091
public class SizeChangerTrigger : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x14000062 RID: 98
	// (add) Token: 0x06003700 RID: 14080 RVA: 0x00128DC4 File Offset: 0x00126FC4
	// (remove) Token: 0x06003701 RID: 14081 RVA: 0x00128DFC File Offset: 0x00126FFC
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnEnter;

	// Token: 0x14000063 RID: 99
	// (add) Token: 0x06003702 RID: 14082 RVA: 0x00128E34 File Offset: 0x00127034
	// (remove) Token: 0x06003703 RID: 14083 RVA: 0x00128E6C File Offset: 0x0012706C
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnExit;

	// Token: 0x06003704 RID: 14084 RVA: 0x00128EA1 File Offset: 0x001270A1
	private void Awake()
	{
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06003705 RID: 14085 RVA: 0x00128EAF File Offset: 0x001270AF
	public void OnTriggerEnter(Collider other)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(other);
		}
	}

	// Token: 0x06003706 RID: 14086 RVA: 0x00128EC5 File Offset: 0x001270C5
	public void OnTriggerExit(Collider other)
	{
		if (this.OnExit != null)
		{
			this.OnExit(other);
		}
	}

	// Token: 0x06003707 RID: 14087 RVA: 0x00128EDB File Offset: 0x001270DB
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.myCollider.ClosestPoint(position);
	}

	// Token: 0x06003708 RID: 14088 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x06003709 RID: 14089 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPieceDestroy()
	{
	}

	// Token: 0x0600370A RID: 14090 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x0600370B RID: 14091 RVA: 0x00128EE9 File Offset: 0x001270E9
	public void OnPieceActivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x0600370C RID: 14092 RVA: 0x00128EE9 File Offset: 0x001270E9
	public void OnPieceDeactivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x04004679 RID: 18041
	private Collider myCollider;

	// Token: 0x0400467C RID: 18044
	public bool builderEnterTrigger;

	// Token: 0x0400467D RID: 18045
	public bool builderExitOnEnterTrigger;

	// Token: 0x0200082C RID: 2092
	// (Invoke) Token: 0x0600370F RID: 14095
	public delegate void SizeChangerTriggerEvent(Collider other);
}
