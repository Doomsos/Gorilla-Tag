using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NexusGroupId", menuName = "Nexus/NexusGroupId")]
public class NexusGroupId : ScriptableObject
{
	public string Code
	{
		get
		{
			if (NexusManager.instance != null && NexusManager.instance.CurrentEnvironment == NexusManager.Environment.PRODUCTION)
			{
				return this.code;
			}
			return this.sandboxCode;
		}
	}

	[SerializeField]
	private string code;

	[SerializeField]
	private string sandboxCode;
}
