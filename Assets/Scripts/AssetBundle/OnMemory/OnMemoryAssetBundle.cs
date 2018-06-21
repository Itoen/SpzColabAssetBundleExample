using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OnMemoryAssetBundle
{

    #region Variables

    private string assetBundleName;

    private AssetBundle assetBundle;

    private int refCount;

    #endregion // Variables

    #region Properties

    /// <summary>
    /// アセットバンドル名
    /// </summary>
    public string AssetBundleName
    {
        get
        {
            return this.assetBundleName;
        }
    }

    /// <summary>
    /// アセットバンドル
    /// </summary>
    public AssetBundle AssetBundle
    {
        get
        {
            return this.assetBundle;
        }
    }

    /// <summary>
    /// 参照カウント
    /// </summary>
    public int RefCount
    {
        get
        {
            return this.refCount;
        }
    }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="assetBundle">アセットバンドル</param>
    public OnMemoryAssetBundle (AssetBundle assetBundle)
    {
        this.assetBundleName = assetBundle.name;
        this.assetBundle = assetBundle;
        this.refCount = 0;
    }

    /// <summary>
    /// 参照カウントを増やす
    /// </summary>
    public void AddRef ()
    {
        this.refCount++;
    }

    /// <summary>
    /// 解放する(参照カウントを減らす)
    /// </summary>
    public void Unload ()
    {
        this.refCount--;
    }

    #endregion // Methods
}
