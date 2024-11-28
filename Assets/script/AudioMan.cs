using UnityEngine;
using System.Collections.Generic;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // ����ģʽ

    [System.Serializable]
    public class Sound
    {
        public string name; // ��Ч����
        public AudioClip clip; // ��Ч�ļ�
        [Range(0, 1)] public float volume = 1f; // ����
        [Range(0, 3)] public float pitch = 1f; // ����
        public bool loop = false; // �Ƿ�ѭ��

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

    public List<Sound> sounds; // ��Ч�б�

    void Awake()
    {
        //����ģʽ
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // �����л�ʱ������

        // ��ʼ��ÿ����Ч�� AudioSource
        foreach (Sound s in sounds)
        {
            GameObject obj = new GameObject(s.name + " Sound");
            obj.transform.parent = this.transform;
            s.SetSource(obj.AddComponent<AudioSource>());
        }
    }

    // ������Ч�ķ���
    public void Play(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("��Ч " + name + " δ�ҵ�!");
            return;
        }

        s.Play();
    }
    public void Play(string name, bool isShot = false)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("��Ч " + name + " δ�ҵ�!");
            return;
        }
        if (isShot)
        {
            s.Play(true);
        }
    }


    // ֹͣ��Ч�ķ���
    public void Stop(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("��Ч " + name + " δ�ҵ�!");
            return;
        }

        s.Stop();
    }
}