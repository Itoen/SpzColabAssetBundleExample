using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public struct AssetBundleDownloader
{
    #region Variables

    /// <summary>
    /// ダウンロード完了時イベント
    /// </summary>
    /// <params>ダウンロードしたアセットバンドル</params>
    public event System.Action<AssetBundle> AssetBundleDownloadedEvent;

    /// <summary>
    /// ダウンロードエラー時イベント
    /// </summary>
    /// <params>ダウンロードに失敗した要求</params>
    public event System.Action<AssetBundleDownloadRequest> DownloadRequestErrorEvent;

    /// <summary>
    /// ダウンロード要求完了時イベント
    /// </summary>
    public event System.Action<AssetBundleDownloadRequest> DownloadRequestCompletedEvent;

    #endregion // Variables

    #region Methods

    /// <summary>
    /// アセットバンドルのダウンロード
    /// </summary>
    /// <returns>コルーチン</returns>
    /// <param name="downloadRequest">ダウンロードリクエスト</param>
    public IEnumerator LoadFromCacheOrDownloadAssetBundle (AssetBundleDownloadRequest downloadRequest)
    {
        while (!Caching.ready)
        {
            yield return null;
        }

        var platformName = PlatformNameManager.GetPlatformName(Application.platform);

        foreach (var assetBundleInfo in downloadRequest.AssetBundleInfos)
        {
            var assetBundleName = assetBundleInfo.AssetBundleName;
            var url = string.Format("{0}/{1}/{2}", AssetBundleManagerConfig.ServerURL, platformName, assetBundleName);

            var webRequest = UnityWebRequest.Get(url);
            var hash = UnityEngine.Hash128.Parse(assetBundleInfo.HashString);
            webRequest.downloadHandler = new DownloadHandlerAssetBundle(url, hash, assetBundleInfo.Crc);
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
                    if (downloadRequest.OnError != null)
                    {
                        downloadRequest.OnError.Invoke("AssetBundleInfoList Download Error.");
                    }
                    this.DownloadRequestErrorEvent.Invoke(downloadRequest);
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
        }


        if (downloadRequest.OnCompleted != null)
        {
            downloadRequest.OnCompleted.Invoke();
        }
        this.DownloadRequestCompletedEvent.Invoke(downloadRequest);
    }

    #endregion // Methods
}
