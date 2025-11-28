using System;
using System.Collections.Generic;

// Token: 0x02000344 RID: 836
[Serializable]
public class ScenePerformanceData
{
	// Token: 0x06001417 RID: 5143 RVA: 0x00073F78 File Offset: 0x00072178
	public ScenePerformanceData(string mapName, int gorillaCount, int droppedFrames, int msHigh, int medianMS, int medianFPS, int medianDrawCalls, List<int> msCaptures)
	{
		this._mapName = mapName;
		this._gorillaCount = gorillaCount;
		this._droppedFrames = droppedFrames;
		this._msHigh = msHigh;
		this._medianMS = medianMS;
		this._medianFPS = medianFPS;
		this._medianDrawCallCount = medianDrawCalls;
		this._msCaptures = new List<int>(msCaptures);
	}

	// Token: 0x04001EA9 RID: 7849
	public string _mapName;

	// Token: 0x04001EAA RID: 7850
	public int _gorillaCount;

	// Token: 0x04001EAB RID: 7851
	public int _droppedFrames;

	// Token: 0x04001EAC RID: 7852
	public int _msHigh;

	// Token: 0x04001EAD RID: 7853
	public int _medianMS;

	// Token: 0x04001EAE RID: 7854
	public int _medianFPS;

	// Token: 0x04001EAF RID: 7855
	public int _medianDrawCallCount;

	// Token: 0x04001EB0 RID: 7856
	public List<int> _msCaptures;
}
