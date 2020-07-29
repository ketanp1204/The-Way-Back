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
        HandleSceneAudioChanges(scene);
    }

    private void HandleSceneAudioChanges(Scene scene)
    {
        // Different sounds for when scenes load

        if (scene.name != "LivingRoom")
        {
            if (scene.name == "Garden")
            {
                ChangeVolume("LR_Gramophone", 0.05f);
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
            Stop("Noon_Inside");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");

            // Play living room sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                if(GameEventsTracker.LR_Window_Open)
                {
                    PlaySoundAtCurrentGameTime("Morning_Rain_Outside");
                }
                else
                {
                    PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
                }
                
            }
            else if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("Noon_Inside");
            }
            else
            {
                PlaySoundAtCurrentGameTime("Evening_Inside");
            }
            if(GameEventsTracker.LR_TV_On)
            {
                Play("LR_TV_Static");
            }
            else
            {
                Stop("LR_TV_Static");
            }
        }

        if(scene.name == "Hallway")
        {
            // Stop sounds from other scenes
            if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                Stop("Morning_Rain_Inside");
                Stop("Morning_Rain_Outside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                Stop("Noon_Inside");
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
                PlaySoundAtCurrentGameTime("Noon_Inside");
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
            Stop("Noon_Inside");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");

            // Play bathroom sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("Noon_Inside");
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
            Stop("Noon_Inside");
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
                PlaySoundAtCurrentGameTime("Noon_Inside");
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
            Stop("Noon_Inside");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");

            // Stop intro sound
            Stop("Intro_Sounds");

            // Play bedroom sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("Noon_Inside");
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
            Stop("Noon_Inside");
            Stop("H_Clock_Ticking");
            Stop("Evening_Inside");

            // Play attic sounds
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                PlaySoundAtCurrentGameTime("Morning_Rain_Inside");
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                PlaySoundAtCurrentGameTime("Noon_Inside");
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
