[System.Serializable]
public struct AssetBundleInfo
{
    /// <summary>
    /// アセットバンドル名
    /// </summary>
    public string AssetBundleName;

    /// <summary>
    /// 依存関係のあるアセットバンドル名一覧
    /// </summary>
    public string[] DependenciesBundleNames;

    /// <summary>
    /// ハッシュ値文字列
    /// </summary>
    public string HashString;

    /// <summary>
    /// CRC値
    /// </summary>
    public uint Crc;
}
