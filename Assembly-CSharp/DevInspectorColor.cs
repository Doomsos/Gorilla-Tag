using System;

[AttributeUsage(384)]
public class DevInspectorColor : Attribute
{
	public string Color { get; }

	public DevInspectorColor(string color)
	{
		this.Color = color;
	}
}
