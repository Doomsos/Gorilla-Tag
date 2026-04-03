using System;
using System.Collections.Generic;
using System.IO;
using GorillaNetworking;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PhotoBoothCamera : MonoBehaviour
{
	[SerializeField]
	private Camera cam;

	[SerializeField]
	private RenderTexture renderTexture;

	[SerializeField]
	private TMP_Text imageLabel;

	[SerializeField]
	private Image imageImage;

	[SerializeField]
	private string saveName = "img";

	[SerializeField]
	private bool appendDateToFile;

	[SerializeField]
	private string imageDescription = "";

	private List<RenderTexture> rt = new List<RenderTexture>();

	public Action<Texture, int> OnCapture;

	private bool saveImageToDevice;

	public void SetSaveImageToDevice(bool b)
	{
		saveImageToDevice = b;
	}

	public void Clear()
	{
		rt.Clear();
	}

	public void Capture(float FOV)
	{
		cam.fieldOfView = FOV;
		cam.Render();
		rt.Add(new RenderTexture(renderTexture.width, renderTexture.height, 1));
		Graphics.Blit(renderTexture, rt[rt.Count - 1]);
		OnCapture(rt[rt.Count - 1], rt.Count - 1);
	}

	public void Print()
	{
		if (!saveImageToDevice)
		{
			return;
		}
		string fileName = saveName;
		if (appendDateToFile)
		{
			DateTime dateTime = DateTime.UtcNow;
			if (GorillaComputer.instance != null)
			{
				dateTime = GorillaComputer.instance.GetServerTime();
			}
			fileName += dateTime.ToString("yyyyMMddHHmmss");
		}
		RenderTexture print = new RenderTexture(renderTexture.width, renderTexture.height * rt.Count, 1);
		for (int i = 0; i < rt.Count; i++)
		{
			Graphics.CopyTexture(rt[i], 0, 0, 0, 0, rt[i].width, rt[i].height, print, 0, 0, 0, rt[i].height * i);
		}
		NativeArray<byte> narray = new NativeArray<byte>(print.width * print.height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		AsyncGPUReadback.RequestIntoNativeArray(ref narray, print, 0, delegate(AsyncGPUReadbackRequest request)
		{
			if (!request.hasError)
			{
				SaveImage(print, narray, fileName, imageDescription);
			}
			narray.Dispose();
		});
	}

	private void SaveImage(RenderTexture rt, NativeArray<byte> narray, string fileName, string desc)
	{
		NativeArray<byte> nativeArray = ImageConversion.EncodeNativeArrayToJPG(narray, rt.graphicsFormat, (uint)rt.width, (uint)rt.height);
		File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName + ".jpg"), nativeArray.ToArray());
		nativeArray.Dispose();
	}
}
