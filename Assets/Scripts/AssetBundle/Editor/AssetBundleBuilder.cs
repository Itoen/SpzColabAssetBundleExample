using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AssetBundleBuilder
{

    /// <summary>
    /// アセットバンドルのビルド
    /// </summary>
    [MenuItem("Assets/AssetBundle/BuildAssetBundle")]
    public static void BuildAssetBundles ()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        var assetBundleBuilds = AssetBundleBuilder.GenerateAssetBundleBuilds(assetBundleNames);

        var currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        var platformName = PlatformNameManager.GetPlatformName(currentBuildTarget);
        var outputPath = string.Format("AssetBundles/{0}", platformName);
        AssetBundleBuilder.CheckAndCreateDirectory(outputPath);

        var assetBundleManifest = BuildPipeline.BuildAssetBundles(outputPath, assetBundleBuilds, BuildAssetBundleOptions.ChunkBasedCompression, currentBuildTarget);
        AssetBundleBuilder.SaveAssetBundleInfoListFile(outputPath, assetBundleManifest);
    }

    /// <summary>
    /// アセットバンドルのビルド設定を生成する
    /// </summary>
    /// <returns>ビルド設定一覧</returns>
    /// <param name="assetBundleNames">ビルドするアセットバンドル名一覧</param>
    private static AssetBundleBuild[] GenerateAssetBundleBuilds (string[] assetBundleNames)
    {
        var assetBundleBuildList = new List<AssetBundleBuild>();
        for (var i = 0; i < assetBundleNames.Length; ++i)
        {
            var assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = assetBundleNames[i];
            assetBundleBuild.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[i]);
            assetBundleBuildList.Add(assetBundleBuild);
        }

        return assetBundleBuildList.ToArray();
    }

    /// <summary>
    /// アセットバンドル情報一覧のファイル保存
    /// </summary>
    /// <param name="outputPath">出力先のパス</param>
    /// <param name="assetBundleManifest">ビルドしたアセットバンドルの全体マニフェスト</param>
    private static void SaveAssetBundleInfoListFile (string outputPath, AssetBundleManifest assetBundleManifest)
    {
        var builtAssetBundleNames = assetBundleManifest.GetAllAssetBundles();

        var assetBundleInfoList = new AssetBundleInfoList();
        assetBundleInfoList.AssetBundleInfos = new List<AssetBundleInfo>();
        for (var i = 0; i < builtAssetBundleNames.Length; ++i)
        {
            var assetBundlePath = string.Format("{0}/{1}", outputPath, builtAssetBundleNames[i]);
            var assetBundleInfo = new AssetBundleInfo();
            assetBundleInfo.AssetBundleName = builtAssetBundleNames[i];
            assetBundleInfo.DependenciesBundleNames = assetBundleManifest.GetAllDependencies(builtAssetBundleNames[i]);
            assetBundleInfo.HashString = assetBundleManifest.GetAssetBundleHash(builtAssetBundleNames[i]).ToString();
            uint crc;
            if (BuildPipeline.GetCRCForAssetBundle(assetBundlePath, out crc))
            {
                assetBundleInfo.Crc = crc;
            }
            assetBundleInfoList.AssetBundleInfos.Add(assetBundleInfo);
        }
        var jsonString = JsonUtility.ToJson(assetBundleInfoList);
        var fullPath = string.Format("{0}/{1}", outputPath, AssetBundleManagerConfig.AssetBundleInfoListFileName);
        System.IO.File.WriteAllText(fullPath, jsonString);
    }

    /// <summary>
    /// ディレクトリが存在するかチェックし、無ければ作成する
    /// </summary>
    /// <param name="path">チェックするパス</param>
    private static void CheckAndCreateDirectory (string path)
    {
        var paths = path.Split('/');
        var checkPath = string.Empty;
        for (var i = 0; i < paths.Length; ++i)
        {
            if (string.IsNullOrEmpty(checkPath))
            {
                checkPath = paths[i];
            }
            else
            {
                checkPath = string.Format("{0}/{1}", checkPath, paths[i]);
            }

            if (System.IO.Directory.Exists(checkPath))
            {
                continue;
            }

            System.IO.Directory.CreateDirectory(checkPath);
        }
    }

}
