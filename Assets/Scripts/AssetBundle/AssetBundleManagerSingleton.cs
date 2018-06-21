using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManagerSingleton : SingletonMonoBehaviour<AssetBundleManagerSingleton>
{
    #region Variables

    /// <summary>
    /// アセットバンドルダウンローダー
    /// </summary>
    private AssetBundleDownloader assetBundleDownloader;

    /// <summary>
    /// アセットローダー
    /// </summary>
    private AssetLoader assetLoader;

    /// <summary>
    /// アセットバンドル情報管理クラス
    /// </summary>
    private AssetBundleInfoManager assetBundleInfoManager;

    /// <summary>
    /// メモリ上のアセットバンドル管理クラス
    /// </summary>
    private OnMemoryAssetBundleManager onMemoryAssetBundleManager;

    #endregion // Variables

    #region Methods

    /// <summary>
    /// アセットバンドルをキャッシュから読み込み、またはダウンロードする
    /// </summary>
    /// <returns>コルーチン</returns>
    /// <param name="assetBundleName">アセットバンドル名</param>
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    public void LoadFromCacheOrDownloadAssetBundle (string assetBundleName, System.Action<string> onError, System.Action onCompleted)
    {
        if (this.onMemoryAssetBundleManager.CheckAssetBundleExists(assetBundleName))
        {
            onCompleted.Invoke();
            return;
        }

        StartCoroutine(this.LoadFormCacheOrDownloadCoroutine(assetBundleName, onError, onCompleted));
    }

    /// <summary>
    /// アセットバンドルをキャッシュから読み込み、またはダウンロードするコルーチン
    /// </summary>
    /// <returns>コルーチン</returns>
    /// <param name="assetBundleName">アセットバンドル名</param>
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    public IEnumerator LoadFormCacheOrDownloadCoroutine (string assetBundleName, System.Action<string> onError, System.Action onCompleted)
    {
        while (!this.assetBundleInfoManager.IsDownloadedList)
        {
            yield return null;
        }

        if (!this.assetBundleInfoManager.CheckAssetBundleNameExists(assetBundleName))
        {
            onError.Invoke("指定された名前のアセットバンドルが見つかりません");
            yield break;
        }

        var assetBundleInfo = this.assetBundleInfoManager.GetAssetBundleInfo(assetBundleName);

        yield return this.assetBundleDownloader.LoadFromCacheOrDownloadAssetBundle(assetBundleInfo.AssetBundleName,
                                                                                     assetBundleInfo.HashString,
                                                                                     assetBundleInfo.Crc,
                                                                                     onError,
                                                                                     onCompleted);
    }

    /// <summary>
    /// アセットの非同期読み込み
    /// </summary>
    /// <param name="assetBundleName">アセットバンドル名</param>
    /// <param name="assetName">アセット名</param>
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    /// <typeparam name="T">読み込むアセットの型</typeparam>
    public void LoadAssetAsync<T> (string assetBundleName,
                                  string assetName,
                                  System.Action<string> onError,
                                  System.Action<T> onCompleted) where T : Object
    {
        if (!this.onMemoryAssetBundleManager.CheckAssetBundleExists(assetBundleName))
        {
            onError.Invoke("アセットバンドルがメモリ上に存在しません、読み込みを先にしてください。");
            return;
        }

        var assetBundle = this.onMemoryAssetBundleManager.GetAssetBundle(assetBundleName);

        StartCoroutine(this.assetLoader.LoadAssetAsyncFromAssetBundle<T>(assetBundle, assetName, onError, onCompleted));
    }

    /// <summary>
    /// アセットバンドル内の全アセットを非同期読み込み
    /// </summary>
    /// <param name="assetBundleName">アセットバンドル名</param>
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    /// <typeparam name="T">読み込むアセットの型</typeparam>
    public void LoadAllAssetsAsync (string assetBundleName, System.Action<string> onError, System.Action<Object[]> onCompleted)
    {
        if (!this.onMemoryAssetBundleManager.CheckAssetBundleExists(assetBundleName))
        {
            onError.Invoke("アセットバンドルがメモリ上に存在しません、読み込みを先にしてください。");
            return;
        }

        var assetBundle = this.onMemoryAssetBundleManager.GetAssetBundle(assetBundleName);

        StartCoroutine(this.assetLoader.LoadAllAssetsAsyncFromAssetBundle(assetBundle, onError, onCompleted));
    }

    /// <summary>
    /// アセットバンドルのアンロード
    /// </summary>
    /// <param name="assetBundleName">アセットバンドル名</param>
    public void Unload (string assetBundleName)
    {
        this.onMemoryAssetBundleManager.Unload(assetBundleName);
    }

    #endregion // Methods

    #region Callback methods

    /// <summary>
    /// アセットバンドル読み込み完了時処理
    /// </summary>
    /// <param name="assetBundle">アセットバンドル</param>
    private void OnAssetBundleDownloaded (AssetBundle assetBundle)
    {
        this.onMemoryAssetBundleManager.AddAssetBundle(assetBundle);
    }

    private void Awake ()
    {
        this.assetBundleDownloader = new AssetBundleDownloader();
        this.assetLoader = new AssetLoader();
        this.assetBundleInfoManager = new AssetBundleInfoManager();
        this.onMemoryAssetBundleManager = new OnMemoryAssetBundleManager();
    }

    private void Start ()
    {
        this.assetBundleDownloader.AssetBundleDownloadedEvent += this.OnAssetBundleDownloaded;
        StartCoroutine(this.assetBundleInfoManager.DownloadAssetBundleInfoList());
    }

    private void OnDestroy ()
    {
        this.assetBundleDownloader.AssetBundleDownloadedEvent -= this.OnAssetBundleDownloaded;
    }

    #endregion // Callback methods
}
