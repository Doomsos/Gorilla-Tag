using System;
using Unity.Collections;

namespace GorillaTagScripts
{
	public class BuilderTableJobs
	{
		public static void BuildTestPieceListForJob(BuilderPiece testPiece, NativeList<BuilderPieceData> testPieceList, NativeList<BuilderGridPlaneData> testGridPlaneList)
		{
			if (testPiece == null)
			{
				return;
			}
			int length = testPieceList.Length;
			BuilderPieceData builderPieceData = new BuilderPieceData(testPiece);
			testPieceList.Add(ref builderPieceData);
			for (int i = 0; i < testPiece.gridPlanes.Count; i++)
			{
				BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(testPiece.gridPlanes[i], length);
				testGridPlaneList.Add(ref builderGridPlaneData);
			}
			BuilderPiece builderPiece = testPiece.firstChildPiece;
			while (builderPiece != null)
			{
				BuilderTableJobs.BuildTestPieceListForJob(builderPiece, testPieceList, testGridPlaneList);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}

		public static void BuildTestPieceListForJob(BuilderPiece testPiece, NativeList<BuilderGridPlaneData> testGridPlaneList)
		{
			if (testPiece == null)
			{
				return;
			}
			for (int i = 0; i < testPiece.gridPlanes.Count; i++)
			{
				BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(testPiece.gridPlanes[i], -1);
				testGridPlaneList.Add(ref builderGridPlaneData);
			}
			BuilderPiece builderPiece = testPiece.firstChildPiece;
			while (builderPiece != null)
			{
				BuilderTableJobs.BuildTestPieceListForJob(builderPiece, testGridPlaneList);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
	}
}
