using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Sound s = Array.Find(sounds, sound => sound.name == "LR_Gramophone_Record");
        if (s.source.isPlaying)
        {
            if (scene.name != "LivingRoom")
            {
                s.source.volume = 0.5f;
            }
        }

        if(scene.name == "LivingRoom")
        {
            s = Array.Find(sounds, sound => sound.name == "BathRoom_Morning");
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
            s = Array.Find(sounds, sound => sound.name == "B_Water_Dripping");
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
            Play("LivingRoom_Morning");
        }

        if(scene.name == "Bathroom")
        {
            s = Array.Find(sounds, sound => sound.name == "LivingRoom_Morning");
            if(s.source.isPlaying)
            {
                s.source.Stop();
            }
            s = Array.Find(sounds, sound => sound.name == "Kitchen_Morning");
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
            Play("BathRoom_Morning");
            Play("B_Water_Dripping");
        }

        if (scene.name == "Kitchen")
        {
            s = Array.Find(sounds, sound => sound.name == "BathRoom_Morning");
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
            s = Array.Find(sounds, sound => sound.name == "B_Water_Dripping");
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }

            Play("Kitchen_Morning");
        }
    }

    void Start()
    {
        // Play("Rain_Window");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found.");
            return;
        }
        s.source.Play();
    }

    public Sound GetSound(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        return s;
    }
}
