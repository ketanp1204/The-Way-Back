using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;

    // Bathroom
    public Sprite B_Day_ShelfOpen;
    public Sprite B_Day_ShelfClosed;
    public Sprite B_Noon_ShelfOpen;
    public Sprite B_Noon_ShelfClosed;
    public Sprite B_Eve_ShelfOpenLightOn;
    public Sprite B_Eve_ShelfOpenLightOff;
    public Sprite B_Eve_ShelfClosedLightOn;
    public Sprite B_Eve_ShelfClosedLightOff;
    public Sprite B_Eve_Light_On;
    public Sprite B_Eve_Light_Off;
    public Animator B_WaterDripping;
    public GameObject B_Mirror;
    public GameObject B_SleepingPills;
    public GameObject B_MouthMask;

    // Bedroom
    public Sprite Bed_Done_Day;
    public Sprite Bed_Done_Noon;
    public Sprite Bed_Done_Eve;

    // Living Room
    public GameObject LR_Diary;
    public Animator LR_Record;
    public Animator LR_Player;
    public GameObject LR_TV_Static;
    public GameObject LR_Broschure;
    public Sprite LR_Drawer_Open_Day;
    public Sprite LR_Drawer_Open_Noon;
    public Sprite LR_Drawer_Open_Eve;
    public Sprite LR_Drawer_Closed_Day;
    public Sprite LR_Drawer_Closed_Noon;
    public Sprite LR_Drawer_Closed_Eve;
    public GameObject LR_Plant;

    // Garden
    public GameObject G_Plant;
    public GameObject G_SmallHole;
    public GameObject G_BigHole;
    public GameObject G_LooseBrickOut;
    public GameObject G_PluggedInSocket;
    public Animator G_Fountain;
    public Animator G_Brazier;

    void Awake()
    {
        instance = this;
    }
}
