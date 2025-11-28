using System;
using System.Collections.Generic;
using GorillaExtensions;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200100D RID: 4109
	public class RigidbodyHighlighter : MonoBehaviour
	{
		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x060067FB RID: 26619 RVA: 0x0021F178 File Offset: 0x0021D378
		private string ButtonText
		{
			get
			{
				if (!this.Active)
				{
					return "Highlight Rigidbodies";
				}
				return "Unhighlight Rigidbodies";
			}
		}

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x060067FC RID: 26620 RVA: 0x0021F18D File Offset: 0x0021D38D
		// (set) Token: 0x060067FD RID: 26621 RVA: 0x0021F195 File Offset: 0x0021D395
		public bool Active { get; set; }

		// Token: 0x060067FE RID: 26622 RVA: 0x0021F1A0 File Offset: 0x0021D3A0
		private void Awake()
		{
			Object.Destroy(base.gameObject);
			if (RigidbodyHighlighter.Instance != null && RigidbodyHighlighter.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			RigidbodyHighlighter.Instance = this;
			this._lineRenderer.startWidth = this._lineWidth;
			this._lineRenderer.endWidth = this._lineWidth;
		}

		// Token: 0x060067FF RID: 26623 RVA: 0x0021F208 File Offset: 0x0021D408
		private void Update()
		{
			if (!this.Active)
			{
				this._lineRenderer.positionCount = 0;
				return;
			}
			this._rigidbodies.Clear();
			this._rigidbodies.AddAll(RigidbodyHighlighter.GetAwakeRigidbodies());
			this.DrawTracers();
			foreach (Rigidbody rigidbody in this._rigidbodies)
			{
				RigidbodyHighlighter.DrawBox(rigidbody.transform, Color.red, 0.1f);
			}
		}

		// Token: 0x06006800 RID: 26624 RVA: 0x0021F2A0 File Offset: 0x0021D4A0
		private static List<Rigidbody> GetAwakeRigidbodies()
		{
			List<Rigidbody> list = new List<Rigidbody>();
			Object[] array = Object.FindObjectsByType(typeof(Rigidbody), 0);
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody rigidbody = array[i] as Rigidbody;
				if (rigidbody == null)
				{
					throw new Exception("Non-rigidbody found by FindObjectsByType.");
				}
				if (!rigidbody.IsSleeping())
				{
					list.Add(rigidbody);
				}
			}
			return list;
		}

		// Token: 0x06006801 RID: 26625 RVA: 0x0021F2F9 File Offset: 0x0021D4F9
		private void HighlightActiveRigidbodies()
		{
			this.Active = !this.Active;
		}

		// Token: 0x06006802 RID: 26626 RVA: 0x0021F30C File Offset: 0x0021D50C
		private void GetRigidbodyNames()
		{
			List<Rigidbody> list = (this._rigidbodies.Count > 0) ? this._rigidbodies : RigidbodyHighlighter.GetAwakeRigidbodies();
			for (int i = 0; i < list.Count; i++)
			{
				Debug.Log(string.Format("Rigidbody {0} of {1}: {2}", i, list.Count, list[i].name));
			}
		}

		// Token: 0x06006803 RID: 26627 RVA: 0x0021F374 File Offset: 0x0021D574
		private void OnDrawGizmos()
		{
			if (!this.Active)
			{
				return;
			}
			Gizmos.color = Color.red;
			foreach (Rigidbody rigidbody in this._rigidbodies)
			{
				Gizmos.DrawWireCube(rigidbody.transform.position, Vector3.one);
			}
		}

		// Token: 0x06006804 RID: 26628 RVA: 0x0021F3E8 File Offset: 0x0021D5E8
		private static void DrawBox(Transform tx, Color color, float duration)
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x.SetTRS(tx.position, tx.rotation, tx.lossyScale);
			Vector3 vector = matrix4x.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));
			Vector3 vector2 = matrix4x.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
			Vector3 vector3 = matrix4x.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));
			Vector3 vector4 = matrix4x.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
			Vector3 vector5 = matrix4x.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
			Vector3 vector6 = matrix4x.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
			Vector3 vector7 = matrix4x.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
			Vector3 vector8 = matrix4x.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
			Debug.DrawLine(vector, vector2, color, duration, false);
			Debug.DrawLine(vector2, vector4, color, duration, false);
			Debug.DrawLine(vector4, vector3, color, duration, false);
			Debug.DrawLine(vector3, vector, color, duration, false);
			Debug.DrawLine(vector8, vector7, color, duration, false);
			Debug.DrawLine(vector7, vector5, color, duration, false);
			Debug.DrawLine(vector5, vector6, color, duration, false);
			Debug.DrawLine(vector6, vector8, color, duration, false);
			Debug.DrawLine(vector, vector5, color, duration, false);
			Debug.DrawLine(vector2, vector6, color, duration, false);
			Debug.DrawLine(vector3, vector7, color, duration, false);
			Debug.DrawLine(vector4, vector8, color, duration, false);
		}

		// Token: 0x06006805 RID: 26629 RVA: 0x0021F584 File Offset: 0x0021D784
		private void DrawTracers()
		{
			Vector3[] array = new Vector3[this._rigidbodies.Count * 2 + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((i % 2 == 0) ? (Camera.main.transform.position + this._tracerOffset) : this._rigidbodies[i / 2].transform.position);
			}
			this._lineRenderer.positionCount = array.Length;
			this._lineRenderer.SetPositions(array);
		}

		// Token: 0x040076E7 RID: 30439
		[CanBeNull]
		public static RigidbodyHighlighter Instance;

		// Token: 0x040076E8 RID: 30440
		[SerializeField]
		private float _inGameDuration = 10f;

		// Token: 0x040076E9 RID: 30441
		[SerializeField]
		private LineRenderer _lineRenderer;

		// Token: 0x040076EA RID: 30442
		[SerializeField]
		private float _lineWidth = 0.01f;

		// Token: 0x040076EB RID: 30443
		[SerializeField]
		private Vector3 _tracerOffset = 0.5f * Vector3.down;

		// Token: 0x040076ED RID: 30445
		private readonly List<Rigidbody> _rigidbodies = new List<Rigidbody>();
	}
}
