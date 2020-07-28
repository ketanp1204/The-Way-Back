using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsTracker : MonoBehaviour
{

    public static GameEventsTracker instance;

    // Living Room Events
    public static bool LR_Plant_Interacted = false;
    public static bool LR_Window_Open = false;
    public static bool LR_TV_On = true;

    // Bathroom Events
    public static bool B_Buckets_Kept = false;
    public static bool B_Light_On = false;

    // Garden Events
    public static bool G_Pond_Filled = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }


}
