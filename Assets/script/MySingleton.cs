using UnityEngine;

public class MySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // ˽�о�̬ʵ����ʹ���ӳٳ�ʼ��
    private static T instance;
    public static GameObject ContextPreFab;

    // �̰߳�ȫ��������
    private static readonly object lockObj = new object();

    // ������̬���ԣ����ڷ���Ψһʵ��
    public static T Instance
    {
        get
        {
            // ���ʵ�������ڣ����Բ��ҳ����е�ʵ��
            if (instance == null)
            {
                lock (lockObj)
                {
                    // ˫�ؼ������
                    if (instance == null)
                    {
                        // ���ҳ������Ƿ���ڸ����͵�ʵ��
                        instance = (T)FindObjectOfType(typeof(T));

                        // ��������в����ڣ��򴴽�һ���µ�ʵ��
                        if (instance == null)
                        {
                             
                            // ����һ���µĿ� GameObject �����Ӹ����͵����
                            GameObject singleton = new GameObject(typeof(T).Name + " (Singleton)");
                            instance = singleton.AddComponent<T>();
                        }
                    }
                }
            }
            return instance;
        }
    }

    // �ڳ�������ʱ��������ʵ��
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject); // ��ֹ���������ڳ����л�ʱ������
        }
        else
        {
            Destroy(gameObject); // ������ڶ��ʵ���������ٶ����ʵ��
        }
    }
}