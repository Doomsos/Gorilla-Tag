using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;

// Token: 0x02000CC2 RID: 3266
public class TextureFromURL : MonoBehaviour
{
	// Token: 0x06004FB2 RID: 20402 RVA: 0x0019A302 File Offset: 0x00198502
	private void OnEnable()
	{
		if (this.data.Length == 0)
		{
			return;
		}
		if (this.source == TextureFromURL.Source.TitleData)
		{
			this.LoadFromTitleData();
			return;
		}
		this.applyRemoteTexture(this.data);
	}

	// Token: 0x06004FB3 RID: 20403 RVA: 0x0019A330 File Offset: 0x00198530
	private void LoadFromTitleData()
	{
		TextureFromURL.<LoadFromTitleData>d__7 <LoadFromTitleData>d__;
		<LoadFromTitleData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadFromTitleData>d__.<>4__this = this;
		<LoadFromTitleData>d__.<>1__state = -1;
		<LoadFromTitleData>d__.<>t__builder.Start<TextureFromURL.<LoadFromTitleData>d__7>(ref <LoadFromTitleData>d__);
	}

	// Token: 0x06004FB4 RID: 20404 RVA: 0x0019A367 File Offset: 0x00198567
	private void OnDisable()
	{
		if (this.texture != null)
		{
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06004FB5 RID: 20405 RVA: 0x00002789 File Offset: 0x00000989
	private void OnPlayFabError(PlayFabError error)
	{
	}

	// Token: 0x06004FB6 RID: 20406 RVA: 0x0019A38C File Offset: 0x0019858C
	private void OnTitleDataRequestComplete(string imageUrl)
	{
		imageUrl = imageUrl.Replace("\\r", "\r").Replace("\\n", "\n");
		if (imageUrl.get_Chars(0) == '"' && imageUrl.get_Chars(imageUrl.Length - 1) == '"')
		{
			imageUrl = imageUrl.Substring(1, imageUrl.Length - 2);
		}
		this.applyRemoteTexture(imageUrl);
	}

	// Token: 0x06004FB7 RID: 20407 RVA: 0x0019A3F0 File Offset: 0x001985F0
	private void applyRemoteTexture(string imageUrl)
	{
		TextureFromURL.<applyRemoteTexture>d__11 <applyRemoteTexture>d__;
		<applyRemoteTexture>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<applyRemoteTexture>d__.<>4__this = this;
		<applyRemoteTexture>d__.imageUrl = imageUrl;
		<applyRemoteTexture>d__.<>1__state = -1;
		<applyRemoteTexture>d__.<>t__builder.Start<TextureFromURL.<applyRemoteTexture>d__11>(ref <applyRemoteTexture>d__);
	}

	// Token: 0x06004FB8 RID: 20408 RVA: 0x0019A430 File Offset: 0x00198630
	private Task<Texture2D> GetRemoteTexture(string url)
	{
		TextureFromURL.<GetRemoteTexture>d__12 <GetRemoteTexture>d__;
		<GetRemoteTexture>d__.<>t__builder = AsyncTaskMethodBuilder<Texture2D>.Create();
		<GetRemoteTexture>d__.url = url;
		<GetRemoteTexture>d__.<>1__state = -1;
		<GetRemoteTexture>d__.<>t__builder.Start<TextureFromURL.<GetRemoteTexture>d__12>(ref <GetRemoteTexture>d__);
		return <GetRemoteTexture>d__.<>t__builder.Task;
	}

	// Token: 0x04005E33 RID: 24115
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04005E34 RID: 24116
	[SerializeField]
	private TextureFromURL.Source source;

	// Token: 0x04005E35 RID: 24117
	[Tooltip("If Source is set to 'TitleData' Data should be the id of the title data entry that defines an image URL. If Source is set to 'URL' Data should be a URL that points to an image.")]
	[SerializeField]
	private string data;

	// Token: 0x04005E36 RID: 24118
	private Texture2D texture;

	// Token: 0x04005E37 RID: 24119
	private int maxTitleDataAttempts = 10;

	// Token: 0x02000CC3 RID: 3267
	private enum Source
	{
		// Token: 0x04005E39 RID: 24121
		TitleData,
		// Token: 0x04005E3A RID: 24122
		URL
	}
}
