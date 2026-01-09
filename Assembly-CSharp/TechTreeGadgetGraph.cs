using System;
using System.Linq;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "TechTreePage", menuName = "SuperInfection/TechTree Page")]
public class TechTreeGadgetGraph : NodeGraph
{
	public GadgetNode[] GadgetNodes
	{
		get
		{
			return (from n in this.nodes
			select n as GadgetNode).ToArray<GadgetNode>();
		}
	}

	public string nickName;

	public SITechTreePageId pageId;

	public float costMultiplier = 1f;

	public ESuperGameModes excludedGameModes;

	public EAssetReleaseTier releaseTier;

	private const float XLayoutStep = 300f;

	private const float YLayoutStep = 250f;
}
