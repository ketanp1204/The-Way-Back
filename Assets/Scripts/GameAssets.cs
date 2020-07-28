using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;

    // Bathroom
    public Sprite B_Day_ShelfOpen;
    public Sprite B_Day_ShelfClosed;
    public Sprite B_Eve_ShelfOpenLightOn;
    public Sprite B_Eve_ShelfOpenLightOff;
    public Sprite B_Eve_ShelfClosedLightOn;
    public Sprite B_Eve_ShelfClosedLightOff;
    public Sprite B_Eve_Light_On;
    public Sprite B_Eve_Light_Off;

    // Bedroom
    public Sprite Bed_Done_Day;
    public Sprite Bed_Done_Noon;
    public Sprite Bed_Done_Eve;

    // Living Room
    public GameObject LR_Diary;

    // Garden
    public GameObject G_Plant;
    public GameObject G_SmallHole;
    public GameObject G_BigHole;
    public GameObject G_LooseBrickOut;
    public GameObject G_PluggedInSocket;

    void Awake()
    {
        instance = this;
    }
}
