using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

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
        if(scene.name != "LivingRoom")
        {
            if (scene.name == "Garden")
            {
                Stop("LR_Gramophone_Record");
            }
            else
            {
                ChangeVolume("LR_Gramophone_Record", 0.5f);
            }
        }

        if(scene.name == "LivingRoom")
        {
            // If coming from the bathroom scene, stop its sounds
            Stop("BathRoom_Morning");   
            Stop("B_Water_Dripping");

            // Play living room sounds
            Play("LivingRoom_Morning");
        }

        if(scene.name == "Bathroom")
        {
            // If coming from the living room or kitchen scene, stop their sounds
            Stop("LivingRoom_Morning");
            Stop("Kitchen_Morning");

            // Play bathroom sounds
            Play("BathRoom_Morning");
            if(GameEventsTracker.B_Tap_Water_Dripping)      // If the water tap hasn't been shut, play the water dripping sound
            {
                Play("B_Water_Dripping");
            }
        }

        if (scene.name == "Kitchen")
        {
            // If coming from the bathroom or garden scene, stop their sounds
            Stop("BathRoom_Morning");
            Stop("B_Water_Dripping");
            
            // Play kitchen sounds
            Play("Kitchen_Morning");
        }
        if (scene.name == "Garden")
        {
            // If coming from the bathroom or garden scene, stop their sounds
            Stop("Kitchen_Morning");
        }
    }

    void Start()
    {
        // Play("Rain_Window");
    }

    public static void Play(string name)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found.");
            return;
        }
        s.source.Play();
    }

    public static void Stop(string name)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogError("Sound " + name + " not found.");
            return;
        }
        if(s.source.isPlaying)
        {
            s.source.Stop();
        }
    }

    public static void ChangeVolume(string name, float newVolume)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogError("Sound " + name + " not found.");
            return;
        }
        if(s.source.isPlaying)
        {
            s.volume = newVolume;
        }
    }

    public static Sound GetSound(string soundName)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == soundName);
        return Array.Find(instance.sounds, sound => sound.name == soundName);
    }
}
