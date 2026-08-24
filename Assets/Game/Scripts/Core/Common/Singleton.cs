using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    
    protected virtual bool IsPersistent => false;

    protected void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;

        if (IsPersistent)
        {
            DontDestroyOnLoad(gameObject);
        }
        
        OnInitialize();
    }

    protected virtual void OnInitialize()
    {
        
    }

    protected void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
