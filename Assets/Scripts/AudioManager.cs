using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;      // Array that holds all the sounds in the game

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

        // For each sound, add the settings and create a new audiosource;
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
        HandleSceneChanges(scene);
    }

    private void HandleSceneChanges(Scene scene)
    {
        // Different sounds for when scenes load

        if (scene.name != "LivingRoom")
        {
            if (scene.name == "Garden")
            {
                Stop("LR_Gramophone");
            }
            else
            {
                ChangeVolume("LR_Gramophone", 0.2f);
            }
        }

        if (scene.name == "LivingRoom")
        {
            // Stop hallway sounds
            Stop("Morning_Rain_Inside");
            Stop("H_Noon");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");

            // Stop intro sound
            Stop("Intro_Sounds");

            // Play living room sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("LR_Noon");
            }
            else
            {
                PlaySoundAtCurrentGameTime("Evening_Inside");
            }
            if(GameEventsTracker.LR_TV_On)
            {
                Play("LR_TV_Static");
                GameAssets.instance.LR_TV_Static.GetComponent<Animator>().enabled = true;
                GameAssets.instance.LR_TV_Static.GetComponent<Animator>().Play("Base Layer.LR_TV_Static");
            }
            if(GameEventsTracker.G_Plant_Planted)
            {
                GameAssets.instance.LR_Plant.SetActive(false);
            }
            else
            {
                GameAssets.instance.LR_Plant.SetActive(true);
            }
        }

        if(scene.name == "Hallway")
        {
            // Stop sounds from other scenes
            if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                Stop("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                Stop("LR_Noon");
                Stop("K_Noon");
                Stop("B_Noon");
                Stop("Bed_Noon");
            }
            else
            {
                Stop("Evening_Inside");
            }
            Stop("B_Water_Dripping");
            Stop("LR_TV_Static");

            // Play hallway sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("H_Noon");
            }
            else
            {
                PlaySoundAtCurrentGameTime("Evening_Inside");
            }
            Play("H_Clock_Ticking");
        }

        if (scene.name == "Bathroom")
        {
            // Stop hallway sounds
            Stop("Morning_Rain_Inside");
            Stop("H_Noon");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");

            // Play bathroom sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("B_Noon");
            }
            else
            {
                PlaySoundAtCurrentGameTime("Evening_Inside");
            }

            Play("B_Water_Dripping");
        }

        if (scene.name == "Kitchen")
        {
            // Stop hallway sounds
            Stop("Morning_Rain_Inside");
            Stop("H_Noon");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");
            
            // Stop Garden Sounds
            Stop("G_Wind_Chime");
            Stop("G_Fire_Burning");
            Stop("G_Water_Fountain");
            Stop("G_Noon");
            Stop("G_Evening");

            // Play kitchen sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("K_Noon");
            }
            else
            {
                PlaySoundAtCurrentGameTime("Evening_Inside");
            }
        }

        if (scene.name == "Garden")
        {
            // Stop kitchen sounds
            Stop("Morning_Rain_Inside");

            // Play garden sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("G_Noon");
            }
            else
            {
                PlaySoundAtCurrentGameTime("G_Evening");
            }
            Play("G_Wind_Chime");
        }

        if (scene.name == "Bedroom")
        {
            // Stop hallway sounds
            Stop("Morning_Rain_Inside");
            Stop("H_Noon");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");

            // Play bedroom sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("Bed_Noon");
            }
            else
            {
                PlaySoundAtCurrentGameTime("Evening_Inside");
            }
        }

        if (scene.name == "Attic")
        {
            // Stop hallway sounds
            Stop("Morning_Rain_Inside");
            Stop("H_Noon");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");

            // Play attic sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("A_Noon");
            }
            else
            {
                PlaySoundAtCurrentGameTime("Evening_Inside");
            }
        }
    }

    // Plays the sound with the given name
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

    public static void PlaySoundAtCurrentGameTime(string name)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogError("Sound " + name + " not found.");
            return;
        }
        s.source.Play();
        s.source.time = GameSession.gameTime - GameSession.GetTimeOfDayIndex() * GameSession.timeOfDayInterval;
    }

    // Stops the sound with the given name
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

    // Changes the volume of the sound with the given name 
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
            s.source.volume = newVolume;
        }
    }

    // Gets the reference to the sound with the given name
    public static Sound GetSound(string soundName)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == soundName);
        return Array.Find(instance.sounds, sound => sound.name == soundName);
    }
}
