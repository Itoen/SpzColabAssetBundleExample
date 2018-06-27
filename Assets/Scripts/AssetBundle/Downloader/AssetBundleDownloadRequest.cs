public struct AssetBundleDownloadRequest
{
    #region Variables

    /// <summary>
    /// アセットバンドル名
    /// </summary>
    //public string AssetBundleName;

    public AssetBundleInfo[] AssetBundleInfos;

    /// <summary>
    /// エラー時コールバック
    /// </summary>
    public System.Action<string> OnError;

    /// <summary>
    /// 完了時コールバック
    /// </summary>
    public System.Action OnCompleted;

    #endregion // Variables

    #region Methods

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="assetBundleInfos">アセットバンドル情報</param>
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    public AssetBundleDownloadRequest (AssetBundleInfo[] assetBundleInfos, System.Action<string> onError, System.Action onCompleted)
    {
        this.AssetBundleInfos = assetBundleInfos;
        this.OnError = onError;
        this.OnCompleted = onCompleted;
    }

    #endregion // Methods
}
