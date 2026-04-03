using System.IO;
using K4os.Compression.LZ4;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels;

public static class ChunkIO
{
	public const uint MAGIC = 1448040524u;

	public const int VERSION = 5;

	private static readonly string Root = Path.Combine(Application.persistentDataPath, "WorldSaves");

	public static string PathFor(int3 id)
	{
		return Path.Combine(Root, $"{id.x}_{id.y}_{id.z}.vox");
	}

	public static void SaveChunk(ChunkDTO dto)
	{
		string text = PathFor(dto.Id);
		Debug.Log($"Saving chunk {dto.Id} to {text}");
		Save(text, in dto);
	}

	public static bool TryLoadChunk(int3 id, out ChunkDTO dto)
	{
		string arg = PathFor(id);
		if (!File.Exists(PathFor(id)))
		{
			dto = default(ChunkDTO);
			return false;
		}
		dto = Load(PathFor(id));
		if (dto.IsValid)
		{
			Debug.Log($"Loaded chunk {id} from {arg}");
		}
		else
		{
			Debug.Log($"Chunk {id} at {arg} magic or version mismatch.");
		}
		return dto.IsValid;
	}

	public static void Save(string path, in ChunkDTO chunk)
	{
		if (!Directory.Exists(Root))
		{
			Directory.CreateDirectory(Root);
		}
		using FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: false);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		binaryWriter.Write(1448040524u);
		binaryWriter.Write(5);
		binaryWriter.Write(chunk.Id.x);
		binaryWriter.Write(chunk.Id.y);
		binaryWriter.Write(chunk.Id.z);
		binaryWriter.Write(chunk.Size.x);
		binaryWriter.Write(chunk.Size.y);
		binaryWriter.Write(chunk.Size.z);
		binaryWriter.Write(chunk.Dimensions.x);
		binaryWriter.Write(chunk.Dimensions.y);
		binaryWriter.Write(chunk.Dimensions.z);
		WriteNativeArray(binaryWriter, chunk.Density);
		WriteNativeArray(binaryWriter, chunk.Material);
	}

	public static ChunkDTO Load(string path, Allocator alloc = Allocator.Persistent)
	{
		using FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: false);
		using BinaryReader binaryReader = new BinaryReader(input);
		uint num = binaryReader.ReadUInt32();
		int num2 = binaryReader.ReadInt32();
		if (num != 1448040524 || num2 != 5)
		{
			return default(ChunkDTO);
		}
		int3 id = new int3(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
		int3 size = new int3(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
		int3 dimensions = new int3(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
		NativeArray<byte> density = ReadNativeArray(binaryReader, alloc);
		NativeArray<byte> material = ReadNativeArray(binaryReader, alloc);
		return new ChunkDTO
		{
			Id = id,
			Size = size,
			Dimensions = dimensions,
			Density = density,
			Material = material
		};
	}

	private static void WriteNativeArray(BinaryWriter bw, NativeArray<byte> src)
	{
		byte[] array = LZ4Pickler.Pickle(src.ToArray());
		bw.Write(array.Length);
		bw.Write(array);
	}

	private static NativeArray<byte> ReadNativeArray(BinaryReader br, Allocator alloc = Allocator.Persistent)
	{
		int count = br.ReadInt32();
		byte[] array = LZ4Pickler.Unpickle(br.ReadBytes(count));
		int length = array.Length;
		NativeArray<byte> nativeArray = new NativeArray<byte>(length, alloc);
		NativeArray<byte>.Copy(array, nativeArray, length);
		return nativeArray;
	}

	public static void DeleteWorld()
	{
		if (Directory.Exists(Root))
		{
			Directory.Delete(Root, recursive: true);
		}
		Debug.Log("All chunks deleted.");
	}
}
