using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    [SerializeField]
    string assetBundleName;

    [SerializeField]
    string assetName;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(this.DownloadRequestCoroutine());
    }

    // Update is called once per frame
    void Update ()
    {

    }

    private void LoadAsset ()
    {
        var assetBundleManagerSingleton = AssetBundleManagerSingleton.Instance;

        assetBundleManagerSingleton.LoadAssetAsync<GameObject>(assetBundleName, assetName, this.OnAssetBundleError, this.OnLoadedAsset);
    }

    private void OnDownloadedAssetBundle ()
    {
        this.LoadAsset();
    }

    private void OnLoadedAsset (GameObject gameObject)
    {
        Instantiate(gameObject);
    }

    private void OnAssetBundleError (string errorMessage)
    {
        Debug.LogError(errorMessage);
    }

    private IEnumerator DownloadRequestCoroutine ()
    {
        var assetBundleManagerSingleton = AssetBundleManagerSingleton.Instance;

        while (!assetBundleManagerSingleton.IsInitialized)
        {
            yield return null;
        }

        assetBundleManagerSingleton.LoadFromCacheOrDownloadAssetBundle(assetBundleName, this.OnAssetBundleError, this.OnDownloadedAssetBundle);
    }
}
