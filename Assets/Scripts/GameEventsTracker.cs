using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsTracker : MonoBehaviour
{

    public static GameEventsTracker instance;

    public static bool LR_Plant_Interacted = false;
    
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
