using System;
using UnityEngine;

// Token: 0x0200025E RID: 606
[CreateAssetMenu(fileName = "New Hand Gesture", menuName = "Gorilla/Hand Gesture")]
public class GorillaHandGesture : ScriptableObject
{
	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x00052FA8 File Offset: 0x000511A8
	// (set) Token: 0x06000FA5 RID: 4005 RVA: 0x00052FB7 File Offset: 0x000511B7
	public GestureHandNode hand
	{
		get
		{
			return (GestureHandNode)this.nodes[0];
		}
		set
		{
			this.nodes[0] = value;
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x00052FC2 File Offset: 0x000511C2
	// (set) Token: 0x06000FA7 RID: 4007 RVA: 0x00052FCC File Offset: 0x000511CC
	public GestureNode palm
	{
		get
		{
			return this.nodes[1];
		}
		set
		{
			this.nodes[1] = value;
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x00052FD7 File Offset: 0x000511D7
	// (set) Token: 0x06000FA9 RID: 4009 RVA: 0x00052FE1 File Offset: 0x000511E1
	public GestureNode wrist
	{
		get
		{
			return this.nodes[2];
		}
		set
		{
			this.nodes[2] = value;
		}
	}

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000FAA RID: 4010 RVA: 0x00052FEC File Offset: 0x000511EC
	// (set) Token: 0x06000FAB RID: 4011 RVA: 0x00052FF6 File Offset: 0x000511F6
	public GestureNode digits
	{
		get
		{
			return this.nodes[3];
		}
		set
		{
			this.nodes[3] = value;
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000FAC RID: 4012 RVA: 0x00053001 File Offset: 0x00051201
	// (set) Token: 0x06000FAD RID: 4013 RVA: 0x00053010 File Offset: 0x00051210
	public GestureDigitNode thumb
	{
		get
		{
			return (GestureDigitNode)this.nodes[4];
		}
		set
		{
			this.nodes[4] = value;
		}
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000FAE RID: 4014 RVA: 0x0005301B File Offset: 0x0005121B
	// (set) Token: 0x06000FAF RID: 4015 RVA: 0x0005302A File Offset: 0x0005122A
	public GestureDigitNode index
	{
		get
		{
			return (GestureDigitNode)this.nodes[5];
		}
		set
		{
			this.nodes[5] = value;
		}
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x00053035 File Offset: 0x00051235
	// (set) Token: 0x06000FB1 RID: 4017 RVA: 0x00053044 File Offset: 0x00051244
	public GestureDigitNode middle
	{
		get
		{
			return (GestureDigitNode)this.nodes[6];
		}
		set
		{
			this.nodes[6] = value;
		}
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0005304F File Offset: 0x0005124F
	private static GestureNode[] InitNodes()
	{
		return new GestureNode[]
		{
			new GestureHandNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureDigitNode(),
			new GestureDigitNode(),
			new GestureDigitNode()
		};
	}

	// Token: 0x04001380 RID: 4992
	public bool track = true;

	// Token: 0x04001381 RID: 4993
	public GestureNode[] nodes = GorillaHandGesture.InitNodes();
}
