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
    public static bool B_Water_Dripping = true;

    // Garden Events
    public static bool G_Pond_Filled = false;
    public static bool G_Plant_Planted = false;

    // Bedroom Events
    public static bool Bed_VacationPictureExamined = false;
    public static bool Bed_BedDone = false;

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
