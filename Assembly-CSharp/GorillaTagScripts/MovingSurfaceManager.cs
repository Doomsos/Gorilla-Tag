using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DAB RID: 3499
	public class MovingSurfaceManager : MonoBehaviour
	{
		// Token: 0x06005609 RID: 22025 RVA: 0x001B08E0 File Offset: 0x001AEAE0
		private void Awake()
		{
			if (MovingSurfaceManager.instance != null && MovingSurfaceManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of MovingSurfaceManager already exists. Destroying.", null);
				Object.Destroy(this);
				return;
			}
			if (MovingSurfaceManager.instance == null)
			{
				MovingSurfaceManager.instance = this;
			}
		}

		// Token: 0x0600560A RID: 22026 RVA: 0x001B092C File Offset: 0x001AEB2C
		public void RegisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.TryAdd(ms.GetID(), ms);
		}

		// Token: 0x0600560B RID: 22027 RVA: 0x001B0941 File Offset: 0x001AEB41
		public void UnregisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.Remove(ms.GetID());
		}

		// Token: 0x0600560C RID: 22028 RVA: 0x001B0955 File Offset: 0x001AEB55
		public void RegisterSurfaceMover(SurfaceMover sm)
		{
			if (!this.surfaceMovers.Contains(sm))
			{
				this.surfaceMovers.Add(sm);
				sm.InitMovingSurface();
			}
		}

		// Token: 0x0600560D RID: 22029 RVA: 0x001B0977 File Offset: 0x001AEB77
		public void UnregisterSurfaceMover(SurfaceMover sm)
		{
			this.surfaceMovers.Remove(sm);
		}

		// Token: 0x0600560E RID: 22030 RVA: 0x001B0986 File Offset: 0x001AEB86
		public bool TryGetMovingSurface(int id, out MovingSurface result)
		{
			return this.movingSurfaces.TryGetValue(id, ref result) && result != null;
		}

		// Token: 0x0600560F RID: 22031 RVA: 0x001B09A4 File Offset: 0x001AEBA4
		private void FixedUpdate()
		{
			foreach (SurfaceMover surfaceMover in this.surfaceMovers)
			{
				if (surfaceMover.isActiveAndEnabled)
				{
					surfaceMover.Move();
				}
			}
		}

		// Token: 0x0400631F RID: 25375
		private List<SurfaceMover> surfaceMovers = new List<SurfaceMover>(5);

		// Token: 0x04006320 RID: 25376
		private Dictionary<int, MovingSurface> movingSurfaces = new Dictionary<int, MovingSurface>(10);

		// Token: 0x04006321 RID: 25377
		public static MovingSurfaceManager instance;
	}
}
