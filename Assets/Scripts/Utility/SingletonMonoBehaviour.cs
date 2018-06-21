using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    #region Variables

    protected static T instance;

    #endregion // Variables

    #region Properties

    public static T Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<T>();
            if (instance != null)
            {
                return instance;
            }
            CreateInstance();

            return Instance;
        }
    }

    #endregion // Properties

    #region Methods

    private static void CreateInstance ()
    {
        var gameObject = new GameObject(typeof(T).Name);
        instance = gameObject.AddComponent<T>();
        DontDestroyOnLoad(gameObject);
    }

    #endregion // Methods
}
