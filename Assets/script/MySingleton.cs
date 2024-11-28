using UnityEngine;

public class MySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 私有静态实例，使用延迟初始化
    private static T instance;
    public static GameObject ContextPreFab;

    // 线程安全的锁对象
    private static readonly object lockObj = new object();

    // 公共静态属性，用于访问唯一实例
    public static T Instance
    {
        get
        {
            // 如果实例不存在，则尝试查找场景中的实例
            if (instance == null)
            {
                lock (lockObj)
                {
                    // 双重检查锁定
                    if (instance == null)
                    {
                        // 查找场景中是否存在该类型的实例
                        instance = (T)FindObjectOfType(typeof(T));

                        // 如果场景中不存在，则创建一个新的实例
                        if (instance == null)
                        {
                             
                            // 创建一个新的空 GameObject 并附加该类型的组件
                            GameObject singleton = new GameObject(typeof(T).Name + " (Singleton)");
                            instance = singleton.AddComponent<T>();
                        }
                    }
                }
            }
            return instance;
        }
    }

    // 在场景加载时保留单例实例
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject); // 防止单例对象在场景切换时被销毁
        }
        else
        {
            Destroy(gameObject); // 如果存在多个实例，则销毁多余的实例
        }
    }
}