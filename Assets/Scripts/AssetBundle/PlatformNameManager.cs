public static class PlatformNameManager
{
    #region Constants

    /// <summary>
    /// OSX / macOSのプラットフォーム名
    /// </summary>
    public readonly static string OSXPlatformName = "OSX";

    /// <summary>
    /// Windowsのプラットフォーム名
    /// </summary>
    public readonly static string WindowsPlatformName = "Windows";

    /// <summary>
    /// iOSのプラットフォーム名
    /// </summary>
    public readonly static string IOSPlatformName = "iOS";

    /// <summary>
    /// Androidのプラットフォーム名
    /// </summary>
    public readonly static string AndroidPlatformName = "Android";

    /// <summary>
    /// PS4のプラットフォーム名
    /// </summary>
    public readonly static string PS4PlatformName = "PS4";

    /// <summary>
    /// XboxOneのプラットフォーム名
    /// </summary>
    public readonly static string XboxOnePlatformName = "XboxOne";

    /// <summary>
    /// Nintendo Switchのプラットフォーム名
    /// </summary>
    public readonly static string SwitchPlatformName = "NintendoSwitch";

    #endregion // Constants

    #region Methods

    /// <summary>
    /// プラットフォーム名を取得する
    /// </summary>
    /// <returns>プラットフォーム名</returns>
    /// <param name="runtimePlatform">実行中のプラットフォーム</param>
    public static string GetPlatformName (UnityEngine.RuntimePlatform runtimePlatform)
    {
        switch (runtimePlatform)
        {
            case UnityEngine.RuntimePlatform.OSXPlayer:
            case UnityEngine.RuntimePlatform.OSXEditor:
                return PlatformNameManager.OSXPlatformName;

            case UnityEngine.RuntimePlatform.WindowsEditor:
            case UnityEngine.RuntimePlatform.WindowsPlayer:
                return PlatformNameManager.WindowsPlatformName;

            case UnityEngine.RuntimePlatform.IPhonePlayer:
                return PlatformNameManager.IOSPlatformName;

            case UnityEngine.RuntimePlatform.Android:
                return PlatformNameManager.AndroidPlatformName;

            case UnityEngine.RuntimePlatform.PS4:
                return PlatformNameManager.PS4PlatformName;

            case UnityEngine.RuntimePlatform.XboxOne:
                return PlatformNameManager.XboxOnePlatformName;

            case UnityEngine.RuntimePlatform.Switch:
                return PlatformNameManager.SwitchPlatformName;

            default:
                return string.Empty;
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// プラットフォーム名を取得する
    /// </summary>
    /// <returns>プラットフォーム名</returns>
    /// <param name="buildTarget">ビルド対象のプラットフォーム</param>
    public static string GetPlatformName (UnityEditor.BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case UnityEditor.BuildTarget.StandaloneOSXIntel:
            case UnityEditor.BuildTarget.StandaloneOSXIntel64:
            case UnityEditor.BuildTarget.StandaloneOSXUniversal:
                return PlatformNameManager.OSXPlatformName;

            case UnityEditor.BuildTarget.StandaloneWindows:
            case UnityEditor.BuildTarget.StandaloneWindows64:
                return PlatformNameManager.WindowsPlatformName;

            case UnityEditor.BuildTarget.iOS:
                return PlatformNameManager.IOSPlatformName;

            case UnityEditor.BuildTarget.Android:
                return PlatformNameManager.AndroidPlatformName;

            case UnityEditor.BuildTarget.PS4:
                return PlatformNameManager.PS4PlatformName;

            case UnityEditor.BuildTarget.XboxOne:
                return PlatformNameManager.XboxOnePlatformName;

            case UnityEditor.BuildTarget.Switch:
                return PlatformNameManager.SwitchPlatformName;

            default:
                return string.Empty;
        }
    }

#endif // UNITY_EDITOR

    #endregion // Methods
}
