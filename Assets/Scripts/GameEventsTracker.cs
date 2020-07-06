using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsTracker : MonoBehaviour
{

    public static GameEventsTracker instance;

    // Living Room Events
    public static bool LR_Plant_Interacted = false;

    // Bathroom Events
    public static bool B_Buckets_Kept = false;
    
    
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
