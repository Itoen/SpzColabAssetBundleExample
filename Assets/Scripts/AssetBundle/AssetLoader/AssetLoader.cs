using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader
{

    /// <summary>
    /// アセットバンドルからアセットを非同期読み込みする
    /// </summary>
    /// <returns>コルーチン</returns>
    /// <param name="assetBundle">アセットバンドル</param>
    /// <param name="assetName">アセット名</param>
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    /// <typeparam name="T">読み込むアセットの型</typeparam>
    public IEnumerator LoadAssetAsyncFromAssetBundle<T> (AssetBundle assetBundle,
                                                         string assetName,
                                                         System.Action<string> onError,
                                                         System.Action<T> onCompleted) where T : UnityEngine.Object
    {
        var assetBundleRequest = assetBundle.LoadAssetAsync<T>(assetName);

        while (!assetBundleRequest.isDone)
        {
            yield return null;
        }

        if (assetBundleRequest.asset == null)
        {
            if (onError != null)
            {
                onError.Invoke(string.Format("{0}/{1} AssetLoadFailed.", assetBundle.name, assetName));
            }
            yield break;
        }

        if (onCompleted != null)
        {
            onCompleted.Invoke(assetBundleRequest.asset as T);
        }
    }

    /// <summary>
    /// アセットバンドルから全てのアセットを非同期読み込みする
    /// </summary>
    /// <returns>コルーチン</returns>
    /// <param name="assetBundle">アセットバンドル</param>
    /// <param name="onError">エラー時コールバック</param>
    /// <param name="onCompleted">完了時コールバック</param>
    public IEnumerator LoadAllAssetsAsyncFromAssetBundle (AssetBundle assetBundle,
                                                         System.Action<string> onError,
                                                         System.Action<Object[]> onCompleted)
    {
        var assetBundleRequest = assetBundle.LoadAllAssetsAsync();

        while (!assetBundleRequest.isDone)
        {
            yield return null;
        }

        if (assetBundleRequest.allAssets == null || assetBundleRequest.allAssets.Length <= 0)
        {
            if (onError != null)
            {
                onError.Invoke(string.Format("{0} AllAssetsLoadFailed.", assetBundle.name));
            }
            yield break;
        }

        if (onCompleted != null)
        {
            onCompleted.Invoke(assetBundleRequest.allAssets);
        }
    }
}
