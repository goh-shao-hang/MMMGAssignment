using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonPunCallbacks<T> : MonoBehaviourPunCallbacks where T: SingletonPunCallbacks<T>
{
    private static T instance = null;
    private static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<T>(); //new GameObject(typeof(T).ToString()).AddComponent<T>();
            return instance;
        }
    }

    private void OnApplicationQuit()
    {
        RemoveInstance();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private void RemoveInstance()
    {
        instance = null;
    }

    public static T GetInstance()
    {
        return Instance;
    }

    protected void SetDontDestroyOnLoad()
    {
        DontDestroyOnLoad(Instance.gameObject);
    }
}
