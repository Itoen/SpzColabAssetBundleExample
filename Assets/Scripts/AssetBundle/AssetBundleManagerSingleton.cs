using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManagerSingleton : SingletonMonoBehaviour<AssetBundleManagerSingleton>
{
    #region Constants

    /// <summary>
    /// 最大並列ダウンロード数
    /// </summary>
    private readonly static int MaxParallelDownloadCount = 1;

    #endregion // Constatns

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

    /// <summary>
    /// ダウンロード要求のキュー
    /// </summary>
    private Queue<AssetBundleDownloadRequest> downloadRequestQueue =
        new Queue<AssetBundleDownloadRequest>();

    /// <summary>
    /// ダウンロード処理中のコルーチン辞書
    /// </summary>
    private Dictionary<AssetBundleDownloadRequest, Coroutine> downloadCoroutineDictionary =
        new Dictionary<AssetBundleDownloadRequest, Coroutine>();

    #endregion // Variables

    #region Properties

    /// <summary>
    /// 初期化済みか
    /// </summary>
    public bool IsInitialized
    {
        get
        {
            return this.assetBundleInfoManager.IsDownloadedList;
        }
    }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// アセットバンドルをキャッシュから読み込み、またはダウンロードする
    /// </summary>
    /// <returns>コルーチン</returns>
    /// <param name="assetBundleName">アセットバンドル名</param>
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    /// <param name="isDownloadDependencies">依存アセットバンドルをダウンロードするか</param>
    public void LoadFromCacheOrDownloadAssetBundle (string assetBundleName, System.Action<string> onError, System.Action onCompleted, bool isDownloadDependencies = true)
    {
        if (!this.assetBundleInfoManager.IsDownloadedList)
        {
            if (onError != null)
            {
                onError.Invoke("初期化が終了していません");
            }
        }

        if (this.onMemoryAssetBundleManager.CheckAssetBundleExists(assetBundleName))
        {
            if (onCompleted != null)
            {
                onCompleted.Invoke();
            }
            return;
        }

        var assetBundleInfoList = new List<AssetBundleInfo>();
        var assetBundleInfo = this.assetBundleInfoManager.GetAssetBundleInfo(assetBundleName);
        assetBundleInfoList.Add(assetBundleInfo);

        if (isDownloadDependencies)
        {
            var dependenceInfos = this.assetBundleInfoManager.GetDependenceInfos(assetBundleInfo);
            assetBundleInfoList.AddRange(dependenceInfos);
        }

        var downloadRequest = new AssetBundleDownloadRequest(assetBundleInfoList.ToArray(), onError, onCompleted);
        this.downloadRequestQueue.Enqueue(downloadRequest);
    }

    /// <summary>
    /// アセットバンドルをキャッシュから読み込み、またはダウンロードするコルーチン
    /// </summary>
    /// <returns>コルーチン</returns>
    /// <param name="downloadRequest">ダウンロード要求</param>
    private IEnumerator LoadFormCacheOrDownloadCoroutine (AssetBundleDownloadRequest downloadRequest)
    {
        while (!this.assetBundleInfoManager.IsDownloadedList)
        {
            yield return null;
        }

        yield return this.assetBundleDownloader.LoadFromCacheOrDownloadAssetBundle(downloadRequest);
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
            if (onError != null)
            {
                onError.Invoke("アセットバンドルがメモリ上に存在しません、読み込みを先にしてください。");
            }
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
            if (onError != null)
            {
                onError.Invoke("アセットバンドルがメモリ上に存在しません、読み込みを先にしてください。");
            }
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
    private void OnDownloadedAssetBundle (AssetBundle assetBundle)
    {
        this.onMemoryAssetBundleManager.AddAssetBundle(assetBundle);
    }

    /// <summary>
    /// ダウンロード要求完了時イベント
    /// </summary>
    /// <param name="downloadRequest">完了時ダウンロード要求</param>
    public void OnCompletedDownloadRequest (AssetBundleDownloadRequest downloadRequest)
    {
        if (this.downloadCoroutineDictionary.ContainsKey(downloadRequest))
        {
            this.downloadCoroutineDictionary.Remove(downloadRequest);
        }
    }

    /// <summary>
    /// ダウンロード要求失敗時処理
    /// </summary>
    /// <param name="downloadRequest">失敗したダウンロード要求</param>
    private void OnErrorDownloadRequest (AssetBundleDownloadRequest downloadRequest)
    {
        if (this.downloadCoroutineDictionary.ContainsKey(downloadRequest))
        {
            this.downloadCoroutineDictionary.Remove(downloadRequest);
        }
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
        this.assetBundleDownloader.AssetBundleDownloadedEvent += this.OnDownloadedAssetBundle;
        this.assetBundleDownloader.DownloadRequestCompletedEvent += this.OnCompletedDownloadRequest;
        this.assetBundleDownloader.DownloadRequestErrorEvent += this.OnErrorDownloadRequest;
        StartCoroutine(this.assetBundleInfoManager.DownloadAssetBundleInfoList());
    }

    private void Update ()
    {
        if (this.downloadRequestQueue.Count <= 0)
        {
            return;
        }

        if (this.downloadCoroutineDictionary.Count < MaxParallelDownloadCount)
        {
            var downloadRequest = this.downloadRequestQueue.Dequeue();
            var coroutine = StartCoroutine(this.LoadFormCacheOrDownloadCoroutine(downloadRequest));
            this.downloadCoroutineDictionary.Add(downloadRequest, coroutine);
        }
    }

    private void OnDestroy ()
    {
        this.assetBundleDownloader.AssetBundleDownloadedEvent -= this.OnDownloadedAssetBundle;
        this.assetBundleDownloader.DownloadRequestCompletedEvent -= this.OnCompletedDownloadRequest;
        this.assetBundleDownloader.DownloadRequestErrorEvent -= this.OnErrorDownloadRequest;
    }

    #endregion // Callback methods
}
