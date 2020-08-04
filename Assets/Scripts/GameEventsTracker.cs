using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsTracker : MonoBehaviour
{

    public static GameEventsTracker instance;

    // Living Room Events
    public static bool LR_Plant_Interacted;
    public static bool LR_Window_Open;
    public static bool LR_TV_On;
    public static bool LR_Gramophone_Animation_Playing;

    // Bathroom Events
    public static bool B_Buckets_Kept;
    public static bool B_Light_On;
    public static bool B_Water_Dripping;
    public static bool B_SleepingPillsWashedDown;

    // Garden Events
    public static bool G_Pond_Filled;
    public static bool G_Plant_Planted;
    public static bool G_Fire_Animation_Playing;
    public static bool G_Pond_Animation_Playing;

    // Bedroom Events
    public static bool Bed_VacationPictureExamined;
    public static bool Bed_BedDone;
    public static bool Bed_Diary_Kept;

    // Kitchen Events
    public static bool K_Fruits_HalfBasket;
    public static bool K_ShoppingList_Removed;

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

    public static void Initialize()
    {
        LR_Plant_Interacted = false;
        LR_Window_Open = false;
        LR_TV_On = true;
        LR_Gramophone_Animation_Playing = false;

        B_Buckets_Kept = false;
        B_Light_On = false;
        B_Water_Dripping = true;
        B_SleepingPillsWashedDown = false;

        G_Pond_Filled = false;
        G_Plant_Planted = false;
        G_Fire_Animation_Playing = false;
        G_Pond_Animation_Playing = false;

        Bed_VacationPictureExamined = false;
        Bed_BedDone = false;
        Bed_Diary_Kept = false;

        K_Fruits_HalfBasket = false;
        K_ShoppingList_Removed = false;
    }
}
