using UnityEngine;

namespace Voxels;

public class TextureArrayUtil : MonoBehaviour
{
	public TextureEntry[] textureEntries;

	public Texture2DArray diffuseArray;

	public Texture2DArray normalArray;

	public Material material;

	public bool linearNormalMaps = true;

	public string diffuseName = "_Diffuse";

	public string normalName = "_Normal";

	private bool UnreadableTextureFound => !TexturesReadable;

	private bool TexturesReadable
	{
		get
		{
			TextureEntry[] array = textureEntries;
			for (int i = 0; i < array.Length; i++)
			{
				TextureEntry textureEntry = array[i];
				if (textureEntry.Diffuse == null || textureEntry.Normal == null)
				{
					return false;
				}
				if (!textureEntry.Diffuse.isReadable || !textureEntry.Normal.isReadable)
				{
					return false;
				}
			}
			return true;
		}
	}
}
