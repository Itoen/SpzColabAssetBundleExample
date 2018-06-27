using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleInfoManager
{
    #region Variables

    private AssetBundleInfoList assetBundleInfoList = null;

    #endregion // Variables

    #region Properties

    /// <summary>
    /// 一覧をダウンロード済みか
    /// </summary>
    public bool IsDownloadedList
    {
        get
        {
            return (this.assetBundleInfoList != null);
        }
    }


    #endregion // Properties

    #region Methods

    /// <summary>
    /// アセットバンドル情報一覧のダウンロード
    /// </summary>
    /// <returns>コルーチン</returns>
    public IEnumerator DownloadAssetBundleInfoList ()
    {
        var platformName = PlatformNameManager.GetPlatformName(Application.platform);
        var url = string.Format("{0}/{1}/{2}", AssetBundleManagerConfig.ServerURL, platformName, AssetBundleManagerConfig.AssetBundleInfoListFileName);
        var webRequest = UnityWebRequest.Get(url);
        webRequest.timeout = 300;

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
            yield return null;
        }

        if (webRequest.responseCode != 200)
        {
            Debug.LogErrorFormat("Respose Error : Code = {0}", webRequest.responseCode);
            yield break;
        }

        this.assetBundleInfoList = JsonUtility.FromJson<AssetBundleInfoList>(webRequest.downloadHandler.text);
        this.assetBundleInfoList.CreateDictionary();
        webRequest.Dispose();
    }

    /// <summary>
    /// アセットバンドル名が一覧に存在するか確認する
    /// </summary>
    /// <returns><c>true</c>存在する<c>false</c>しない</returns>
    /// <param name="assetBundleName">アセットバンドル名</param>
    public bool CheckAssetBundleNameExists (string assetBundleName)
    {
        if (!this.IsDownloadedList)
        {
            return false;
        }

        if (!this.assetBundleInfoList.AssetBundleInfoDictionary.ContainsKey(assetBundleName))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// アセットバンドル情報を取得する
    /// </summary>
    /// <returns>アセットバンドル情報</returns>
    /// <param name="assetBundleName">アセットバンドル名</param>
    public AssetBundleInfo GetAssetBundleInfo (string assetBundleName)
    {
        var result = new AssetBundleInfo();
        if (!this.IsDownloadedList)
        {
            Debug.LogError("アセットバンドル情報一覧のダウンロードが終了していません");
            return result;
        }

        this.assetBundleInfoList.AssetBundleInfoDictionary.TryGetValue(assetBundleName, out result);
        return result;
    }

    /// <summary>
    /// 依存アセットバンドルの情報を取得する
    /// </summary>
    /// <returns>依存アセットバンドルの情報</returns>
    /// <param name="assetBundleInfo">アセットバンドル情報</param>
    public AssetBundleInfo[] GetDependenceInfos (AssetBundleInfo assetBundleInfo)
    {
        var dependenciesInfos = new List<AssetBundleInfo>();
        foreach (var dependenciesName in assetBundleInfo.DependenciesBundleNames)
        {
            if (!this.CheckAssetBundleNameExists(dependenciesName))
            {
                continue;
            }

            var dependenceInfo = this.GetAssetBundleInfo(dependenciesName);
            dependenciesInfos.Add(dependenceInfo);
        }

        return dependenciesInfos.ToArray();
    }

    #endregion // Methods
}
