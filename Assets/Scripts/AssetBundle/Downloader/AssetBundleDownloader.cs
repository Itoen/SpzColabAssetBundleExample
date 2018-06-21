using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public struct AssetBundleDownloader
{
    #region Variables

    public event System.Action<AssetBundle> AssetBundleDownloadedEvent;

    #endregion // Variables

    #region Methods

    /// <summary>
    /// アセットバンドルのダウンロード
    /// </summary>
    /// <returns>コルーチン</returns>
    /// <param name="assetBundleName">アセットバンドル名</param>
    /// <param name="hashString">ハッシュ値文字列</param>
    /// <param name="crc">CRCチェックサム</param
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    public IEnumerator LoadFromCacheOrDownloadAssetBundle (string assetBundleName, string hashString, uint crc, System.Action<string> onError, System.Action onCompleted)
    {
        while (!Caching.ready)
        {
            yield return null;
        }

        var platformName = PlatformNameManager.GetPlatformName(Application.platform);
        var url = string.Format("{0}/{1}/{2}", AssetBundleManagerConfig.ServerURL, platformName, assetBundleName);

        var webRequest = UnityWebRequest.Get(url);
        var hash = UnityEngine.Hash128.Parse(hashString);
        webRequest.downloadHandler = new DownloadHandlerAssetBundle(url, hash, crc);
        webRequest.timeout = 300;
        webRequest.SetRequestHeader("Cache-Control", "no-cache");

#if UNITY_2017_2_OR_NEWER
        yield return webRequest.SendWebRequest();
#else
        yield return webRequest.Send();
#endif

        while (!webRequest.isDone)
        {
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("AssetBundleInfoList Download Error.");
                webRequest.Dispose();
                yield break;
            }
            Debug.LogFormat("[{0}] Download Progress. : {1}%", assetBundleName, (webRequest.downloadProgress * 100));
            yield return null;
        }


        var handler = webRequest.downloadHandler as DownloadHandlerAssetBundle;
        var assetBundle = handler.assetBundle;

        this.AssetBundleDownloadedEvent.Invoke(assetBundle);
        webRequest.downloadHandler.Dispose();

        if (onCompleted != null)
        {
            onCompleted.Invoke();
        }
    }

    #endregion // Methods
}
