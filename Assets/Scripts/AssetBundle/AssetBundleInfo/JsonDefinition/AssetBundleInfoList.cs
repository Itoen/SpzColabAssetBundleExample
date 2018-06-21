using System.Collections.Generic;

[System.Serializable]
public class AssetBundleInfoList
{
    #region Variables

    /// <summary>
    /// アセットバンドル情報一覧
    /// </summary>
    public List<AssetBundleInfo> AssetBundleInfos;

    /// <summary>
    /// バンドル名 <-> アセットバンドル情報の辞書
    /// </summary>
    public Dictionary<string, AssetBundleInfo> AssetBundleInfoDictionary;

    #endregion // Variables

    #region Methods

    /// <summary>
    /// 辞書を作成する
    /// </summary>
    public void CreateDictionary ()
    {
        this.AssetBundleInfoDictionary = new Dictionary<string, AssetBundleInfo>();

        foreach (var assetBundleInfo in this.AssetBundleInfos)
        {
            if (this.AssetBundleInfoDictionary.ContainsKey(assetBundleInfo.AssetBundleName))
            {
                continue;
            }

            this.AssetBundleInfoDictionary.Add(assetBundleInfo.AssetBundleName, assetBundleInfo);
        }
    }

    #endregion // Methods
}
