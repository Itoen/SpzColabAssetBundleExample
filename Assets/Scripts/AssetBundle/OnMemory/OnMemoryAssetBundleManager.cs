using System.Collections.Generic;
using UnityEngine;

public class OnMemoryAssetBundleManager
{
    #region Variables

    /// <summary>
    /// メモリ上にあるアセットバンドルの辞書
    /// </summary>
    private Dictionary<string, OnMemoryAssetBundle> onMemoryAssetbundleDictionary =
        new Dictionary<string, OnMemoryAssetBundle>();

    #endregion // Variables

    /// <summary>
    /// アセットバンドルを追加する
    /// </summary>
    /// <param name="assetBundle">アセットバンドル</param>
    public void AddAssetBundle (AssetBundle assetBundle)
    {
        var onMemoryAssetBundle = new OnMemoryAssetBundle(assetBundle);
        this.onMemoryAssetbundleDictionary.Add(assetBundle.name, onMemoryAssetBundle);
    }

    /// <summary>
    /// アセットバンドルを取得する
    /// </summary>
    /// <returns>アセットバンドル</returns>
    /// <param name="assetBundleName">アセットバンドル名</param>
    public AssetBundle GetAssetBundle (string assetBundleName)
    {
        OnMemoryAssetBundle onMemoryAssetBundle;
        if (!this.onMemoryAssetbundleDictionary.TryGetValue(assetBundleName, out onMemoryAssetBundle))
        {
            return null;
        }

        onMemoryAssetBundle.AddRef();
        return onMemoryAssetBundle.AssetBundle;
    }

    /// <summary>
    /// アセットバンドルがメモリ上に存在するか確認する
    /// </summary>
    /// <returns><c>true</c>存在する<c>false</c>しない</returns>
    /// <param name="assetBundleName">アセットバンドル名</param>
    public bool CheckAssetBundleExists (string assetBundleName)
    {
        return this.onMemoryAssetbundleDictionary.ContainsKey(assetBundleName);
    }

    /// <summary>
    /// アセットバンドルのアンロード
    /// </summary>
    /// <param name="assetBundleName">アセットバンドル名</param>
    public void Unload (string assetBundleName)
    {
        OnMemoryAssetBundle onMemoryAssetBundle;
        if (!this.onMemoryAssetbundleDictionary.TryGetValue(assetBundleName, out onMemoryAssetBundle))
        {
            return;
        }

        onMemoryAssetBundle.Unload();

        if (onMemoryAssetBundle.RefCount <= 0)
        {
            onMemoryAssetBundle.AssetBundle.Unload(true);
            this.onMemoryAssetbundleDictionary.Remove(assetBundleName);
        }
    }

}
