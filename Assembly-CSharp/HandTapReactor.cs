using System;
using TagEffects;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class HandTapReactor : MonoBehaviour
{
	// Token: 0x06000FEB RID: 4075 RVA: 0x00053CAD File Offset: 0x00051EAD
	private void LeftDown(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.LeftDown, false);
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x00053CBC File Offset: 0x00051EBC
	private void LeftUp(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.LeftUp, false);
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x00053CCC File Offset: 0x00051ECC
	private void LeftGesture(IHandEffectsTrigger.Mode mode)
	{
		FlagEvents<HandTapReactor.TapType> flagEvents = this.handTapEvents;
		HandTapReactor.TapType test;
		switch (mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			test = HandTapReactor.TapType.LeftHighFive;
			break;
		case IHandEffectsTrigger.Mode.FistBump:
			test = HandTapReactor.TapType.LeftFistBump;
			break;
		case IHandEffectsTrigger.Mode.Tag3P:
			test = HandTapReactor.TapType.LeftTagThirdPerson;
			break;
		case IHandEffectsTrigger.Mode.Tag1P:
			test = HandTapReactor.TapType.LeftTagFirstPerson;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(mode);
			break;
		}
		flagEvents.InvokeAll(test, false);
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x00053D1D File Offset: 0x00051F1D
	private void RightDown(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.RightDown, false);
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x00053D2D File Offset: 0x00051F2D
	private void RightUp(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.RightUp, false);
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x00053D40 File Offset: 0x00051F40
	private void RightGesture(IHandEffectsTrigger.Mode mode)
	{
		FlagEvents<HandTapReactor.TapType> flagEvents = this.handTapEvents;
		HandTapReactor.TapType test;
		switch (mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			test = HandTapReactor.TapType.RightHighFive;
			break;
		case IHandEffectsTrigger.Mode.FistBump:
			test = HandTapReactor.TapType.RightFistBump;
			break;
		case IHandEffectsTrigger.Mode.Tag3P:
			test = HandTapReactor.TapType.RightTagThirdPerson;
			break;
		case IHandEffectsTrigger.Mode.Tag1P:
			test = HandTapReactor.TapType.RightTagFirstPerson;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(mode);
			break;
		}
		flagEvents.InvokeAll(test, false);
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x00053DA0 File Offset: 0x00051FA0
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			IHandEffectsTrigger[] componentsInChildren = this.myRig.GetComponentsInChildren<IHandEffectsTrigger>();
			if (componentsInChildren[0].RightHand)
			{
				this.rightHandTrigger = componentsInChildren[0];
				this.leftHandTrigger = componentsInChildren[1];
			}
			else
			{
				this.rightHandTrigger = componentsInChildren[1];
				this.leftHandTrigger = componentsInChildren[0];
			}
		}
		if (this.myRig != null)
		{
			this.myRig.LeftHandEffect.handTapDown += new Action<HandEffectContext>(this.LeftDown);
			this.myRig.LeftHandEffect.handTapUp += new Action<HandEffectContext>(this.LeftUp);
			IHandEffectsTrigger handEffectsTrigger = this.leftHandTrigger;
			handEffectsTrigger.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Combine(handEffectsTrigger.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.LeftGesture));
			this.myRig.RightHandEffect.handTapDown += new Action<HandEffectContext>(this.RightDown);
			this.myRig.RightHandEffect.handTapUp += new Action<HandEffectContext>(this.RightUp);
			IHandEffectsTrigger handEffectsTrigger2 = this.rightHandTrigger;
			handEffectsTrigger2.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Combine(handEffectsTrigger2.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.RightGesture));
		}
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x00053ED4 File Offset: 0x000520D4
	private void OnDisable()
	{
		if (this.myRig != null)
		{
			this.myRig.LeftHandEffect.handTapDown -= new Action<HandEffectContext>(this.LeftDown);
			this.myRig.LeftHandEffect.handTapUp -= new Action<HandEffectContext>(this.LeftUp);
			IHandEffectsTrigger handEffectsTrigger = this.leftHandTrigger;
			handEffectsTrigger.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Remove(handEffectsTrigger.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.LeftGesture));
			this.myRig.RightHandEffect.handTapDown -= new Action<HandEffectContext>(this.RightDown);
			this.myRig.RightHandEffect.handTapUp -= new Action<HandEffectContext>(this.RightUp);
			IHandEffectsTrigger handEffectsTrigger2 = this.rightHandTrigger;
			handEffectsTrigger2.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Remove(handEffectsTrigger2.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.RightGesture));
		}
	}

	// Token: 0x040013C3 RID: 5059
	[SerializeField]
	private FlagEvents<HandTapReactor.TapType> handTapEvents;

	// Token: 0x040013C4 RID: 5060
	private VRRig myRig;

	// Token: 0x040013C5 RID: 5061
	private IHandEffectsTrigger leftHandTrigger;

	// Token: 0x040013C6 RID: 5062
	private IHandEffectsTrigger rightHandTrigger;

	// Token: 0x0200026D RID: 621
	[Flags]
	private enum TapType
	{
		// Token: 0x040013C8 RID: 5064
		None = 0,
		// Token: 0x040013C9 RID: 5065
		LeftDown = 1,
		// Token: 0x040013CA RID: 5066
		LeftUp = 2,
		// Token: 0x040013CB RID: 5067
		LeftHighFive = 4,
		// Token: 0x040013CC RID: 5068
		LeftFistBump = 8,
		// Token: 0x040013CD RID: 5069
		LeftTagFirstPerson = 16,
		// Token: 0x040013CE RID: 5070
		LeftTagThirdPerson = 32,
		// Token: 0x040013CF RID: 5071
		AllLeft = 63,
		// Token: 0x040013D0 RID: 5072
		RightDown = 64,
		// Token: 0x040013D1 RID: 5073
		RightUp = 128,
		// Token: 0x040013D2 RID: 5074
		RightHighFive = 256,
		// Token: 0x040013D3 RID: 5075
		RightFistBump = 512,
		// Token: 0x040013D4 RID: 5076
		RightTagFirstPerson = 1024,
		// Token: 0x040013D5 RID: 5077
		RightTagThirdPerson = 2048,
		// Token: 0x040013D6 RID: 5078
		AllRight = 4032,
		// Token: 0x040013D7 RID: 5079
		All = -1
	}
}
