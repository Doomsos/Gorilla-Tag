using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200002C RID: 44
public class LoadScene : MonoBehaviour
{
	// Token: 0x0600009E RID: 158 RVA: 0x00005112 File Offset: 0x00003312
	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(this._delay);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(this._sceneName, 0);
		while (asyncOperation.progress < 0.99f)
		{
			yield return null;
		}
		asyncOperation.allowSceneActivation = true;
		yield break;
	}

	// Token: 0x040000BD RID: 189
	[SerializeField]
	private float _delay;

	// Token: 0x040000BE RID: 190
	[SerializeField]
	private string _sceneName;
}
