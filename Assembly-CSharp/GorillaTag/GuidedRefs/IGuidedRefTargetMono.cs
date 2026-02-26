using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	public interface IGuidedRefTargetMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		GuidedRefBasicTargetInfo GRefTargetInfo { get; set; }

		UnityEngine.Object GuidedRefTargetObject { get; }
	}
}
