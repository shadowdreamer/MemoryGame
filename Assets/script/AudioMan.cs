using UnityEngine;
using System.Collections.Generic;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // 单例模式

    [System.Serializable]
    public class Sound
    {
        public string name; // 音效名称
        public AudioClip clip; // 音效文件
        [Range(0, 1)] public float volume = 1f; // 音量
        [Range(0, 3)] public float pitch = 1f; // 音调
        public bool loop = false; // 是否循环

        private AudioSource source;

        public void SetSource(AudioSource _source)
        {
            source = _source;
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
        }

        public void Play()
        {
            source.Play();
        }
        public void Play(bool isShot)
        {
            //source.Play();
            source.PlayOneShot(clip);
        }

        public void Stop()
        {
            source.Stop();
        }
    }

    public List<Sound> sounds; // 音效列表

    void Awake()
    {
        //单例模式
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // 场景切换时不销毁

        // 初始化每个音效的 AudioSource
        foreach (Sound s in sounds)
        {
            GameObject obj = new GameObject(s.name + " Sound");
            obj.transform.parent = this.transform;
            s.SetSource(obj.AddComponent<AudioSource>());
        }
    }

    // 播放音效的方法
    public void Play(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("音效 " + name + " 未找到!");
            return;
        }

        s.Play();
    }
    public void Play(string name, bool isShot = false)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("音效 " + name + " 未找到!");
            return;
        }
        if (isShot)
        {
            s.Play(true);
        }
    }


    // 停止音效的方法
    public void Stop(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("音效 " + name + " 未找到!");
            return;
        }

        s.Stop();
    }
}