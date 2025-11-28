using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000045 RID: 69
public static class CrittersGrabberSharedData
{
	// Token: 0x06000151 RID: 337 RVA: 0x000090FD File Offset: 0x000072FD
	public static void Initialize()
	{
		if (CrittersGrabberSharedData.initialized)
		{
			return;
		}
		CrittersGrabberSharedData.initialized = true;
		CrittersGrabberSharedData.enteredCritterActor = new List<CrittersActor>();
		CrittersGrabberSharedData.triggerCollidersToCheck = new List<CapsuleCollider>();
		CrittersGrabberSharedData.heldActor = new List<CrittersActor>();
		CrittersGrabberSharedData.actorGrabbers = new List<CrittersActorGrabber>();
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00009135 File Offset: 0x00007335
	public static void AddEnteredActor(CrittersActor actor)
	{
		CrittersGrabberSharedData.Initialize();
		if (CrittersGrabberSharedData.enteredCritterActor.Contains(actor))
		{
			return;
		}
		CrittersGrabberSharedData.enteredCritterActor.Add(actor);
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00009155 File Offset: 0x00007355
	public static void RemoveEnteredActor(CrittersActor actor)
	{
		CrittersGrabberSharedData.Initialize();
		if (!CrittersGrabberSharedData.enteredCritterActor.Contains(actor))
		{
			return;
		}
		CrittersGrabberSharedData.enteredCritterActor.Remove(actor);
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00009176 File Offset: 0x00007376
	public static void AddTrigger(CapsuleCollider trigger)
	{
		CrittersGrabberSharedData.Initialize();
		if (CrittersGrabberSharedData.triggerCollidersToCheck.Contains(trigger))
		{
			return;
		}
		CrittersGrabberSharedData.triggerCollidersToCheck.Add(trigger);
	}

	// Token: 0x06000155 RID: 341 RVA: 0x00009196 File Offset: 0x00007396
	public static void RemoveTrigger(CapsuleCollider trigger)
	{
		CrittersGrabberSharedData.Initialize();
		if (!CrittersGrabberSharedData.triggerCollidersToCheck.Contains(trigger))
		{
			return;
		}
		CrittersGrabberSharedData.triggerCollidersToCheck.Remove(trigger);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x000091B7 File Offset: 0x000073B7
	public static void AddActorGrabber(CrittersActorGrabber grabber)
	{
		CrittersGrabberSharedData.Initialize();
		if (CrittersGrabberSharedData.actorGrabbers.Contains(grabber))
		{
			return;
		}
		CrittersGrabberSharedData.actorGrabbers.Add(grabber);
	}

	// Token: 0x06000157 RID: 343 RVA: 0x000091D7 File Offset: 0x000073D7
	public static void RemoveActorGrabber(CrittersActorGrabber grabber)
	{
		CrittersGrabberSharedData.Initialize();
		if (!CrittersGrabberSharedData.actorGrabbers.Contains(grabber))
		{
			return;
		}
		CrittersGrabberSharedData.actorGrabbers.Remove(grabber);
	}

	// Token: 0x06000158 RID: 344 RVA: 0x000091F8 File Offset: 0x000073F8
	public static void DisableEmptyGrabberJoints()
	{
		CrittersGrabberSharedData.Initialize();
		for (int i = 0; i < CrittersGrabberSharedData.actorGrabbers.Count; i++)
		{
			if (CrittersGrabberSharedData.actorGrabbers[i].grabber != null && CrittersGrabberSharedData.actorGrabbers[i].actorsStillPresent.Count == 0)
			{
				for (int j = 0; j < CrittersGrabberSharedData.actorGrabbers[i].grabber.grabbedActors.Count; j++)
				{
					CrittersGrabberSharedData.actorGrabbers[i].grabber.grabbedActors[j].DisconnectJoint();
				}
			}
		}
	}

	// Token: 0x0400017A RID: 378
	public static List<CrittersActor> enteredCritterActor;

	// Token: 0x0400017B RID: 379
	public static List<CapsuleCollider> triggerCollidersToCheck;

	// Token: 0x0400017C RID: 380
	public static List<CrittersActor> heldActor;

	// Token: 0x0400017D RID: 381
	public static List<CrittersActorGrabber> actorGrabbers;

	// Token: 0x0400017E RID: 382
	private static bool initialized;
}
