using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        OnAwakeRoutine();
    }

    protected void OnDestroy()
    {
        if (Instance != this as T)
        {
            return;
        }

        OnDestroyRoutine();
        Instance = null;
    }

    protected virtual void OnAwakeRoutine() { }
    protected virtual void OnDestroyRoutine() { }
}